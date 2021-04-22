using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace TestAPI.Helpers
{

    public class TestMiddleware
    {
        private readonly RequestDelegate _next;

        // Dependency Injection
        public TestMiddleware(RequestDelegate next)
        {
            _next = next;
        }



        public async Task Invoke(HttpContext context)
        {
            //Reading the AuthHeader which is signed with JWT
            string authHeader = context.Request.Headers["Authorization"];

            if (authHeader != null)
            {
                // //Reading the JWT middle part           
                // int startPoint = authHeader.IndexOf(".") + 1;
                // int endPoint = authHeader.LastIndexOf(".");
                System.Console.WriteLine(authHeader);
                var token = Helper.GetTokenFromRequest(context.Request);
                var response = await Helper.Sendrequest("", RestSharp.Method.GET, token);
                System.Console.WriteLine();
                System.Console.WriteLine();
                if (response.IsSuccessful)
                {
                    var respObj = JsonConvert.DeserializeObject<PermissionObj>(response.Content);

                    var claims = new List<Claim>();
                    respObj.permissions.ForEach(per =>
                    {
                        claims.Add(new Claim(per, "true"));
                    });
                    // context.User.Claims.Append();
                    var identity = new ClaimsIdentity(claims, "basic");
                    context.User = new ClaimsPrincipal(identity);
                }


                // var tokenString = authHeader
                // .Substring(startPoint, endPoint - startPoint)
                // .Split(".");
                // var token = tokenString[0].ToString() + "==";

                // var credentialString = Encoding.UTF8
                // .GetString(Convert.FromBase64String(token));

                // // Splitting the data from Jwt
                // var credentials = credentialString.Split(new char[] { ':', ',' });

                // // Trim this Username and UserRole.
                // var userRule = credentials[5].Replace("\"", "");
                // var userName = credentials[3].Replace("\"", "");

                // // Identity Principal
                // var claims = new[]
                // {
                //     new Claim("name", userName),
                //     new Claim(ClaimTypes.Role, userRule),
                // };
                // var identity = new ClaimsIdentity(claims, "basic");
                // context.User = new ClaimsPrincipal(identity);
            }
            //Pass to the next middleware
            await _next(context);
        }
    }

    class PermissionObj
    {
        public string access { get; set; }
        public List<string> permissions { get; set; }
    }

}
