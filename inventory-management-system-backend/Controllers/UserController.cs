using Core.DataValidators.User;
using Core.Exceptions;
using Core.Interfaces;
using Core.Models;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace inventory_management_system_backend.Controllers
{
    [Authorize(Policy = "BearerToken")]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ISecurityService _securityService;
        private readonly ILogger<UserController> _logger;
        private readonly IConfiguration _configuration;
        public UserController(ILogger<UserController> logger, IUserService userService, ISecurityService securityService, IConfiguration configuration)
        {
            _logger = logger;
            _userService = userService;
            _securityService = securityService;
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<IActionResult> Get([Required][FromQuery] string email)
        {
            var userId = HttpContext.User.Claims.FirstOrDefault(x => x.Value == email);
            var userEmail = HttpContext.User.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Email)?.Value;

            var user = await _userService.GetUserByEmail(email);

            if (user is null)
            {
                return NotFound("No user exists with that email");
            }

            return Ok(user);
        }

        [HttpGet("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsers();
            return Ok(users);
        }

        [HttpPost("Authenticate")]
        [AllowAnonymous]
        public async Task<IActionResult> Authenticate(CreateUserValidator userInfo)
        {
            var userInDb = await _userService.GetUserByEmail(userInfo.Email);
            if (userInDb is null) return BadRequest(new
            {
                Message = "Invalid credentials",
            });

            var securityInDb = await _securityService.GetByUserEmail(userInfo.Email);
            if (securityInDb is null) return BadRequest(new
            {
                Message = "An unexpected error has occurred.",
            });

            if (SecurityService.VerifyPassword(userInfo.Password, securityInDb))
            {
                if (!userInDb.UserCreatedPassword)
                {
                    return Ok("You must now create your own user password.");
                }

                return Ok(GenerateTokens(userInDb.Email, userInDb.GroupId));
            }

            return BadRequest(new
            {
                Message = "Invalid credentials",
            });
        }

        [HttpGet("Refresh")]
        public async Task<IActionResult> Refresh()
        {
            string type;
            try
            {
                type = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "type").Value;
            }
            catch (NullReferenceException)
            {
                return BadRequest("Bad token: can't read type claim");
            }

            if (type != "refresh") return BadRequest("This token is not a refresh token");

            string email;
            try
            {
                email = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email).Value;
            }
            catch (NullReferenceException)
            {
                return BadRequest("Bad token: can't read email claim");
            }

            int groupId;
            try
            {
                groupId = int.Parse(HttpContext.User.Claims.FirstOrDefault(c => c.Type == "group_id").Value);
            }
            catch (NullReferenceException)
            {
                return BadRequest("Bad token: can't read group id claim");
            }
            catch (FormatException)
            {
                return BadRequest("Bad token: group id claim is not an int");
            }

            return Ok(GenerateTokens(email, groupId));
        }

        [HttpPost]
        public async Task<IActionResult> Post(CreateUserValidator userInfo)
        {
            int groupValue = int.Parse(HttpContext.User.Claims.FirstOrDefault(c => c.Type == "group_id").Value);
            if (!IsValidGroup(UserGroups.Admin, groupValue))
            {
                return BadRequest("You do not have permission to do this");
            }

            var userInDb = await _userService.GetUserByEmail(userInfo.Email);
            if (userInDb is not null) return BadRequest(new
            {
                Message = "A user with that email already exists"
            });

            if (!await _userService.Create(userInfo)) return BadRequest(new
            {
                Message = "Something went wrong creating your account"
            });

            var user = _userService.GetUserByEmail(userInfo.Email).Result;

            var userSecurity = new Security()
            {
                UserId = user.Id,
                HashedPassword = SecurityService.HashPassword(userInfo.Password, out byte[] salt),
                Salt = Convert.ToHexString(salt)
            };

            if (!await _securityService.Create(userSecurity)) return BadRequest(new
            {
                Message = "Something went wrong creating your account"
            });

            return Ok(new
            {
                Message = "User has been created!"
            });
        }

        [HttpPut]
        public async Task<IActionResult> Update(UpdateUserValidator updatedUserInfo)
        {
            var userInDb = await _userService.GetUserByEmail(updatedUserInfo.Email);
            if (userInDb is null)
            {
                return BadRequest("Something went wrong");
            }

            if (await _userService.UpdateUser(updatedUserInfo))
            {
                return Ok("User has been updated");
            }

            return BadRequest("Something went wrong");
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int userId)
        {
            int groupValue = int.Parse(HttpContext.User.Claims.FirstOrDefault(c => c.Type == "group_id").Value);
            if (!IsValidGroup(UserGroups.Admin, groupValue)) return BadRequest("You do not have permission to do this");
            var userInDb = await _userService.GetUserById(userId);
            if (userInDb is null)
            {
                return BadRequest("Something went wrong");
            }

            if (await _userService.DeleteUser(userId))
            {
                return Ok("User has been deleted");
            }

            return BadRequest("Something went wrong");
        }

        [HttpPut("UpdateGroup")]
        public async Task<IActionResult> UpdateGroup(UpdateUserGroupValidator updateInfo)
        {
            try
            {
                var user = await _userService.GetUserById(updateInfo.UserId) ?? throw new UserNotFoundException();
                var emailInToken = HttpContext.User.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Email)?.Value;
                var groupId = int.Parse(HttpContext.User.Claims.FirstOrDefault(c => c.Type == "group_id").Value);
                if (!HasPermissionOverUser(user, groupId))
                {
                    return BadRequest("You do not have permission to do this");
                }

                if (emailInToken == user.Email)
                {
                    return BadRequest("You cannot edit your own user group");
                }

                await _userService.ChangeUserGroup(updateInfo.UserId, (UserGroups)updateInfo.GroupId);
            }
            catch (UserNotFoundException)
            {
                return BadRequest("A User with that ID does not exist");
            }
            catch (NullReferenceException)
            {
                return BadRequest("Unable to read the email in the token");
            }

            return Ok("User group updated");
        }

        [HttpPut("CreateUserPassword")]
        [AllowAnonymous]
        public async Task<IActionResult> CreateUserPassword(CreateUserValidator userInfo)
        {
            var user = await _userService.GetUserByEmail(userInfo.Email);
            if (user.UserCreatedPassword) return BadRequest("You already have a user created password");
            if (! await _userService.UserHasCreatedPassword(userInfo.Email, userInfo.Password)) 
                return BadRequest("Something went wrong");
          
            return Ok("Your password is now created!");
        }

        private object GenerateTokens(string email, int group)
        {
            // Generate JWT
            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);

            var bearerDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                    {
                            new Claim("id", Guid.NewGuid().ToString()),
                            new Claim("type", "bearer"),
                            new Claim("group_id", group.ToString()),
                            new Claim(JwtRegisteredClaimNames.Email, email),
                            new Claim(JwtRegisteredClaimNames.Jti,
                                Guid.NewGuid().ToString()),
                    }
                ),
                Expires = DateTime.UtcNow.AddDays(1),
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials
                    (new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha512Signature)
            };

            var refreshDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                    {
                            new Claim("Id", Guid.NewGuid().ToString()),
                            new Claim("type", "refresh"),
                            new Claim("group_id", group.ToString()),
                            new Claim(JwtRegisteredClaimNames.Email, email),
                            new Claim(JwtRegisteredClaimNames.Jti,
                            Guid.NewGuid().ToString())
                        }),
                Expires = DateTime.UtcNow.AddDays(7),
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials
                    (new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha512Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var refresh = tokenHandler.CreateToken(refreshDescriptor);
            var stringRefresh = tokenHandler.WriteToken(refresh);

            var bearer = tokenHandler.CreateToken(bearerDescriptor);
            var stringBearer = tokenHandler.WriteToken(bearer);

            return new
            {
                Bearer = stringBearer,
                Refresh = stringRefresh
            };
        }
        private bool IsValidGroup(UserGroups targetUserGroup, int group)
        {
            if (group >= (int)targetUserGroup)
            {
                return true;
            }

            return false;
        }
        private bool HasPermissionOverUser(User user, int group)
        {
            // This method checks if the user in the bearer token has access to work on the user provided in the method parameter
            if (user.GroupId > group) return false;
            return true;
        }
    }
}