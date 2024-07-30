using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace api.Controllers
{   
    [ApiController]
    [Route("api/[controller]")]
    public class Update_Retrieve_Task48 : ControllerBase
    {
        public class Upload
        {
            public IFormFile? File { get; set; }
            public string? Owner { get; set; }
        }
        


        [HttpPost]
        public IActionResult Update([FromForm] Upload upload)
        {
            //Check if the file was uploaded
            if (upload.File == null || upload.File.Length == 0)
                return BadRequest("No file selected");
            //Check if the owner was provided
            if (string.IsNullOrEmpty(upload.Owner))
                return BadRequest("Owner is required");

            //Get the extension
            var file = upload.File;
            var extension = Path.GetExtension(file.FileName);

            //Check if the extension is valid
            if (extension != ".jpg")
                return BadRequest("Invalid file type. Only .jpg files are allowed.");


            string path = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
            
            // Ensure the directory exists
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            
            string filePath = Path.Combine(path, $"{upload.Owner}_{file.FileName}");

            if(System.IO.File.Exists(filePath)){
                try{
                using (var fs = new FileStream(filePath, FileMode.Create,FileAccess.Write))
                {
                    file.CopyTo(fs);
                    return Ok("File uploaded successfully");
                }
                }catch(Exception ex){
                    return StatusCode(500, $"Internal server error: {ex.Message}");
                }
            }else{
                return BadRequest("File does not exist");
            }
        }
        [HttpGet]
        public IActionResult Retrieve([FromQuery] string Owner = "", [FromQuery] string FileName = "")
        {
            if (string.IsNullOrEmpty(Owner) || string.IsNullOrEmpty(FileName))
                return BadRequest("Owner or FileName is missing");

            string path = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
            string filePath = Path.Combine(path, $"{Owner}_{FileName}");

            if (System.IO.File.Exists(filePath))
            {
                byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);

                // Set the Content-Disposition header to prompt a download
                var contentDisposition = new System.Net.Mime.ContentDisposition
                {
                    FileName = FileName,
                    Inline = false
                };

                Response.Headers.Append("Content-Disposition", contentDisposition.ToString());

                return File(fileBytes, "image/jpeg");
            }
            else
            {
                return BadRequest("File does not exist");
            }   
        }
    }
}