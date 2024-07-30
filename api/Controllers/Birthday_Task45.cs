using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class Birthday_Task45
    {
        [HttpGet]
        public string Get([FromQuery] string name = "", [FromQuery] int year = 0, [FromQuery] int month = 0, [FromQuery] int day = 0){
            if (string.IsNullOrEmpty(name))
            {
                if (year == 0 || month == 0 || day == 0){
                    return "Hello anonymous "+" I can not calculate your age without knowing your birthdate!";
                }
                else{
                    
                    return "Hello anonymous "+" your age is " + (DateTime.Now.Year - year);
                }
            }
            else
            {
                if( year == 0 || month == 0 || day == 0){
                    return "Hello " + name + " "+" I can not calculate your age without knowing your birthdate!";
                }
                else{
                    return "Hello " + name + " "+" your age is " + (DateTime.Now.Year - year);
                }
            }
        }
    }
}