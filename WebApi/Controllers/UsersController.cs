﻿namespace WebApi.Controllers;

using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WebApi.Authorization;
using WebApi.Helpers;
using WebApi.Models.Users;
using WebApi.Services;

[Authorize]
[ApiController]
[Route("api/user")]
public class UsersController : ControllerBase
{
    private IUserService _userService;
    private IMapper _mapper;
    private readonly AppSettings _appSettings;

    public UsersController(
        IUserService userService,
        IMapper mapper,
        IOptions<AppSettings> appSettings)
    {
        _userService = userService;
        _mapper = mapper;
        _appSettings = appSettings.Value;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public IActionResult Authenticate(AuthenticateRequest model)
    {
       
        var response = _userService.Authenticate(model);
        return Ok(new { code = StatusCodes.Status200OK,  data = response, message="Success"});
    }

    [AllowAnonymous]
    [HttpPost("signup")]
    public IActionResult Register(RegisterRequest model)
    {
        _userService.Register(model);
        return Ok(new { message = "Registration successful" });
    }

    [HttpPost("logout")]
    public IActionResult Logout(int id) {
        // todo
        return Ok(new {message = ""});
    }
    
    [HttpGet("info")]
    public IActionResult GetInfo()
    {
        // todo
        return Ok(new { message = "" });
    }

    [AllowAnonymous]
    [HttpGet("list")]
    public IActionResult GetAll()
    {
        var users = _userService.GetAll();

        return Ok(users);
    }

    [HttpGet("{id}")]
    public IActionResult GetById(string id)
    {
        var user = _userService.GetById(id);

        return Ok(user);
    }

    [HttpPut("{id}")]
    public IActionResult Update(string id, UpdateRequest model)
    {
        _userService.Update(id, model);
        return Ok(new { message = "User updated successfully" });
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(string id)
    {
        _userService.Delete(id);
        return Ok(new { message = "User deleted successfully" });
    }
}