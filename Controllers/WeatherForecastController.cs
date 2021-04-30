using System;
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
            return Ok(new { response = "testapi: success" });

            // return Ok(new { error = "access denied" });
        }

        [HttpGet("userdata")]
        [Authorize]
        public async Task<ActionResult> GetData()
        {
            var response = await Helper.Sendrequest("/userdata", Method.GET, Helper.GetTokenFromRequest(this.Request));
            Dictionary<string, string> dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(response.Content);
            var userid = dictionary["sub"];
            return Ok(new { response = response.Content });
        }

        [HttpGet("loggedout")]
        public async Task<ActionResult> GetNotSignedin()
        {
            return Ok(new { response = "testapi: success" });
        }

        [HttpGet("isadmin")]
        [Authorize("manage:awebsite")]
        public async Task<ActionResult> GetAdmin()
        {
            return Ok(new { response = "testapi: success" });
        }

        [HttpGet("ismoderator")]
        [Authorize("manage:forums")]
        public async Task<ActionResult> GetMod()
        {
            return Ok(new { response = "testapi: success" });
        }

        [HttpGet("addadmin")]
        [Authorize]
        public async Task<ActionResult> AddAdmin()
        {
            var response1 = await Helper.Sendrequest("/userdata", Method.GET, Helper.GetTokenFromRequest(this.Request));
            var dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(response1.Content);
            string userid = dictionary["sub"];
            var response = await Helper.Sendrequest("/role/Admin", Method.POST, Helper.GetTokenFromRequest(this.Request), userid);
            System.Console.WriteLine("response: " + response.Content);
            return Ok(new { response = "testapi: success" });
        }

        private int Dictionary<T1, T2>(T2 content)
        {
            throw new NotImplementedException();
        }

        [HttpGet("removeadmin")]
        [Authorize]
        public async Task<ActionResult> removeadmin()
        {
            var response = await Helper.Sendrequest("/role/Admin", Method.DELETE, Helper.GetTokenFromRequest(this.Request));
            System.Console.WriteLine("response: " + response.Content);
            return Ok(new { response = "testapi: success" });
        }



    }
}
