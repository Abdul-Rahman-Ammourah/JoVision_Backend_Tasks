using System;
using System.Security.Principal;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class Birthday_Task46 : ControllerBase
    {
        public class BirthdateRequest
        {
            public string Name { get; set; } = string.Empty;
            public int Year { get; set; }
            public int  Month { get; set; }
            public int  Day { get; set; }
        }

        [HttpPost]
        public IActionResult Post([FromBody] BirthdateRequest request)
        {
            if (request == null)return BadRequest("Invalid request");

            if (string.IsNullOrEmpty(request.Name)){
                if (request.Year == 0 || request.Month == 0 || request.Day == 0){
                    return Ok("Hello anonymous I can not calculate your age without knowing your birthdate!");
                }
                else{
                    return Ok("Hello anonymous your age is " + CalculateAge(new DateTime(request.Year, request.Month, request.Day)));
                }
            }else{
                if( request.Year == 0 || request.Month == 0 || request.Day == 0){
                    return Ok("Hello " + request.Name + " I can not calculate your age without knowing your birthdate!");
                }
                else{
                    return Ok("Hello " + request.Name + " your age is " + CalculateAge(new DateTime(request.Year, request.Month, request.Day)));
                }
            }
        }

        private static int CalculateAge(DateTime birthdate)
        {
            return DateTime.Now.Year - birthdate.Year;
        }
    }
}
