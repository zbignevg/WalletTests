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
using Wallet.Api.Services;

namespace Wallet.Controllers;

[ApiController]
[Route("api/user")]
public class UserController : ControllerBase
{
    public IConfiguration _configuration;
    private readonly IUserService _userService;
    private readonly ILogger<TransactionProcessorService> _logger;

    public UserController(IConfiguration config, IUserService userService, ILogger<TransactionProcessorService> logger)
    {
        _configuration = config;
        _userService = userService;
        _logger = logger;
    }

    [Authorize]
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
}
