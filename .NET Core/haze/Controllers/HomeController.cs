﻿using haze.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace haze.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

       
        [HttpGet("/GetUsers")]
        public IActionResult Get()
        {
            List<User> users = new List<User> { new User { Id = 1, FullName = "Nik" }, new User { Id = 2, FullName = "Brian" } };

            return Ok(users);
        }

        [HttpPost("/CreateUser")]
        public IActionResult Create([FromBody] User request)
        {
            return Ok();
        }

        [HttpPut("/UpdateUser")]
        public IActionResult Update([FromBody] User request)
        {
            return Ok();
        }

        [HttpDelete("/DeleteUser/{Id}")]
        public IActionResult Delete(int Id)
        {
            return Ok();
        }

        [HttpGet("/TestAuth")]
        [Authorize]
        public IActionResult TestAuthRoute()
        {
            return Ok();
        }
    }
}