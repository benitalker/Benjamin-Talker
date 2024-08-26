﻿using AgentsApi.Dto;
using AgentsApi.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Immutable;

namespace AgentsApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class LoginController(IJwtService jwtService) : ControllerBase
    {
        private static readonly ImmutableList<string> allowedNames = [
            "SimulationServer", "MVCServer"
        ];

        [HttpPost]
        public ActionResult<string> Login([FromBody] LoginDto loginDto)
        {
            return allowedNames.Contains(loginDto.Id)
                ? Ok(new TokenDto() { Token = jwtService.CreateToken(loginDto.Id) })
                : BadRequest();
        }
    }
}

