﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RestSharp;
using TestAPI.Helpers;

namespace TestAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        // [Authorize("loggedin")]
        [Authorize]
        public async Task<ActionResult> Get()
        {
            this.User.Claims.ToList().ForEach(claim =>
            {
                System.Console.WriteLine("claim type: " + claim.Type + " value: " + claim.Value);
            });
            return Ok(new { response = "success2" });

            // return Ok(new { error = "access denied" });
        }

        [HttpGet("loggedout")]
        public async Task<ActionResult> GetNotSignedin()
        {
            return Ok(new { response = "success2" });
        }

        [HttpGet("isadmin")]
        [Authorize("manage:awebsite")]
        public async Task<ActionResult> GetAdmin()
        {
            string token = Helper.GetTokenFromRequest(this.Request);
            var response = await Helper.Sendrequest("/isadmin", Method.GET, token);
            if (response.IsSuccessful)
            {
                return Ok(new { response = "success2" });
            }
            return Ok(new { error = "access denied" });
        }

        [HttpGet("ismoderator")]
        [Authorize("manage:forums")]
        public async Task<ActionResult> GetMod()
        {
            string token = Helper.GetTokenFromRequest(this.Request);
            var response = await Helper.Sendrequest("/ismoderator", Method.GET, token);
            if (response.IsSuccessful)
            {
                return Ok(new { response = "success2" });
            }
            return Ok(new { error = "access denied" });
        }

    }
}
