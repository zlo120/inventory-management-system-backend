using Core.Interfaces;
using Infrastructure;
using Infrastructure.Repositories;
using Infrastructure.Services;
using inventory_management_system_backend.TokenAuthHandler;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<Context>(options => options.UseSqlServer(builder.Configuration["ConnectionStrings:DBConnection"]));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(o =>
{
    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey
        (Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true
    };
});

builder.Services.AddSingleton<
    IAuthorizationHandler, TokenRequirementAuthorizationHandler>();

builder.Services.AddSingleton<
    IAuthorizationHandler, AdminRequiredAuthorizationHandler>();

//builder.Services.AddAuthorization();
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("BearerToken", policy =>
    {
        policy.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme);
        policy.RequireAuthenticatedUser();

        // add the custom requirement to the policy
        policy.Requirements.Add(new TokenRequirement());
    });
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminToken", policy =>
    {
        policy.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme);
        policy.RequireAuthenticatedUser();

        // add the custom requirement to the policy
        policy.Requirements.Add(new AdminRequired());
    });
});

builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

builder.Services.AddTransient<ISecurityService, SecurityService>();
builder.Services.AddTransient<ISecurityRepository, SecurityRepository>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IUserRepository, UserRepository>();
builder.Services.AddTransient<IInventoryItemService, InventoryItemService>();
builder.Services.AddTransient<IInventoryItemRepository, InventoryItemRepository>();
builder.Services.AddTransient<ILocationService, LocationService>();
builder.Services.AddTransient<ILocationRepository, LocationRepository>();
builder.Services.AddTransient<IStatusService, StatusService>();
builder.Services.AddTransient<IStatusRepository, StatusRepository>();

builder.Services.AddHttpContextAccessor();

builder.Services.AddControllers().AddNewtonsoftJson();
builder.Services.AddCors();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "Demo API", Version = "v1" });
    option.AddSecurityDefinition(
        "Bearer",
        new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = "Please enter a valid token",
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            BearerFormat = "JWT",
            Scheme = "Bearer"
        }
    );

    option.AddSecurityRequirement(
        new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                new string[] { }
            }
        }
    );
});

builder.Services.AddTransient<Context>(c => new Context(builder.Configuration["ConnectionStrings:DBConnection"]));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();

// Handle cors better than this
app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader().WithExposedHeaders("X-Pagination"));

app.UseAuthorization();

app.MapControllers();

app.Run();