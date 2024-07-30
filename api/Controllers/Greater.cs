using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [ApiController]
    [Route("api/Greater_Task44")]
    public class Greater_Task44 : ControllerBase
    {
        [HttpGet]
        public string Get([FromQuery] string name = "")
        {
            if (string.IsNullOrEmpty(name))
            {
                return "Hello anonymous";
            }
            else
            {
                return "Hello " + name;
            }
        }
    }
}