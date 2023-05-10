using Wallet.Models;
using Wallet.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Isopoh.Cryptography.Argon2;
using Wallet.Helpers;
using System.Security.Cryptography;
using System.ComponentModel;

namespace Wallet.Controllers;

[ApiController]
[Route("api/user")]
public class UserController : ControllerBase
{
    public IConfiguration _configuration;
    private readonly UserService _userService;
    private readonly ILogger<TransactionProcessorService> _logger;

    public UserController(IConfiguration config, UserService userService, ILogger<TransactionProcessorService> logger)
    {
        _configuration = config;
        _userService = userService;
        _logger = logger;
    }

    //[Authorize]
    [HttpGet("{id:length(24)}")]
    public async Task<ActionResult<User>> Get(string id)
    {
        var user = await _userService.Get(id);

        if (user is null)
        {
            return NotFound();
        }

        return user;
    }

    [HttpPost("create")]
    //[TODO: implement custom validation, to make emails unique]
    public async Task<IActionResult> CreateUser(User user)
    {
        user.Salt = Guid.NewGuid().ToString("N");
        user.Password = SecurityManager.HashPassword(_configuration, user.Password, user.Salt);

        await _userService.Create(user);
        var token = SecurityManager.GenerateJWT(_configuration, user);

        return Ok(token);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(string email, string password)
    {
        var user = await _userService.GetByEmail(email);
        if (user is null)
        {
            return NotFound();
        }

        if (!SecurityManager.VerifyPassword(_configuration, password, user.Salt, user.Password))
        {
            return NotFound();
        }

        _logger.LogInformation($"{user.Email} user has logged in (user_id={user.Id})");

        var token = SecurityManager.GenerateJWT(_configuration, user);

        return Ok(token);
    }

    [Authorize]
    [HttpPut("{id:length(24)}")]
    public async Task<IActionResult> Update(string id, User updatedUser)
    {
        var user = await _userService.Get(id);

        if (user is null)
        {
            return NotFound();
        }

        updatedUser.Id = user.Id;

        await _userService.Update(id, updatedUser);

        return NoContent();
    }

    [Authorize]
    [HttpDelete("{id:length(24)}")]
    public async Task<IActionResult> Delete(string id)
    {
        var user = await _userService.Get(id);

        if (user is null)
        {
            return NotFound();
        }

        await _userService.Remove(id);

        return NoContent();
    }

    //[HttpPost("createUser")]
    //public async Task<IActionResult> CreateUSer(User user)
    //{
    //    await _userService.CreateAsync(user);

    //    return CreatedAtAction(nameof(Get), new { id = user.Id }, user);
    //}

    //[HttpPost]
    //public async Task<IActionResult> Post(string email, string password)
    //{
    //    if (email != null && password != null)
    //    {
    //        var user = await _userService.GetUserByEmailAndPassword(email, password);

    //        if (user != null)
    //        {
    //            //create claims details based on the user information
    //            var claims = new[] {
    //                    new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
    //                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
    //                    new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
    //                    new Claim("UserId", user.Id),
    //                    new Claim("FirstName", user.FirstName),
    //                    new Claim("Email", user.Email)
    //                };

    //            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
    //            var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
    //            var token = new JwtSecurityToken(
    //                _configuration["Jwt:Issuer"],
    //                _configuration["Jwt:Audience"],
    //                claims,
    //                expires: DateTime.UtcNow.AddMinutes(10),
    //                signingCredentials: signIn);

    //            return Ok(new JwtSecurityTokenHandler().WriteToken(token));
    //        }
    //        else
    //        {
    //            return BadRequest("Invalid credentials");
    //        }
    //    }
    //    else
    //    {
    //        return BadRequest();
    //    }
    //}
}
