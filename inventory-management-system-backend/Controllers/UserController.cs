using Core.DataValidators;
using Core.Interfaces;
using Core.Models;
using Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace inventory_management_system_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ISecurityService _securityService;
        private readonly ILogger<UserController> _logger;
        public UserController(ILogger<UserController> logger, IUserService userService, ISecurityService securityService)
        {
            _logger = logger;
            _userService = userService;
            _securityService = securityService;
        }

        [HttpGet]
        public async Task<IActionResult> Get([Required][FromQuery] string email)
        {
            var user = await _userService.GetUserByEmail(email);

            if (user is null)
            {
                return NotFound("No user exists with that email");
            }

            return Ok(user);
        }

        [HttpPost("Authenticate")]
        public async Task<IActionResult> Authenticate(CreateUserValidator userInfo)
        {
            var userInDb = await _userService.GetUserByEmail(userInfo.Email);
            if (userInDb is null) return BadRequest("A user with that email does not exists");

            var securityInDb = await _securityService.GetByUserEmail(userInfo.Email);
            if (securityInDb is null) return BadRequest("An unexpected error has occured.");

            if (SecurityService.VerifyPassword(userInfo.Password, securityInDb))
            {
                return Ok("You are signed in!");
            }

            return BadRequest("Incorrect password!");
        }

        [HttpPost]
        public async Task<IActionResult> Post(CreateUserValidator userInfo)
        {
            var userInDb = await _userService.GetUserByEmail(userInfo.Email);
            if (userInDb is not null) return BadRequest("A user with that email already exists");

            if (!await _userService.Create(userInfo)) return BadRequest("Something went wrong creating your account");

            var user = _userService.GetUserByEmail(userInfo.Email).Result;

            var userSecurity = new Security()
            {
                UserId = user.Id,
                HashedPassword = SecurityService.HashPassword(userInfo.Password, out byte[] salt),
                Salt = Convert.ToHexString(salt)
            };

            if (!await _securityService.Create(userSecurity)) return BadRequest("Something went wrong creating your security");

            return Ok("User has been created");
        }

        [HttpPut]
        public async Task<IActionResult> Update(UpdateUserValidator updatedUserInfo)
        {
            // Authenticate first

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
    }
}
