﻿using Curry.Auth;
using Curry.Models;
using Curry.Services;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;

namespace Curry.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ITokenFactory<JwtSecurityToken> _tokenFactory;
        //IConfiguration config;
        public AccountsController(IUserService service, ITokenFactory<JwtSecurityToken> tokenFactory)
        {
            _userService = service;
            _tokenFactory = tokenFactory;
        }
        [HttpPost]
        public async Task<IActionResult> AddUser([FromBody]User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            await _userService.AddUserAsync(user);
            return Ok();
        }
        [HttpGet("{name}")]
        public async Task<IActionResult> GetUser(string name)
        {
            var user = await _userService.FindUserByName(name);
            if (user == null) return NotFound();
            return Ok(user);
        }
        [HttpPost]
        [Route("auth")]
        public async Task<IActionResult> Login(User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var verified = await _userService.Authenticate(user);
            if (verified == null) return Unauthorized();
            var token = _tokenFactory.GenerateToken(verified);

            return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
        }
    }
}
