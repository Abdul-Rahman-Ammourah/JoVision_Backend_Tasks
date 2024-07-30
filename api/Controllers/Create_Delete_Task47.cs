using System;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class Create_Delete_Task47 : ControllerBase
    {
        public class Upload
        {
            public IFormFile? File { get; set; }
            public string? Owner { get; set; }
        }

        [HttpPost]
        public IActionResult Post([FromForm] Upload upload)
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

            //Path to the uploads folder
            string path = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
            string safeOwnerName = Path.GetFileNameWithoutExtension(upload.Owner ?? "unknown").Replace(" ", "_");
            string filePath = Path.Combine(path, $"{safeOwnerName}_{file.FileName}");

            string metadataFilePath = Path.Combine(path, $"{safeOwnerName}_{Path.GetFileNameWithoutExtension(file.FileName)}.json");
        
            //check if the file exists
            if (System.IO.File.Exists(filePath)){
                return BadRequest("File already exists");
            }
            //upload the file
            try
            {
                // Ensure the directory exists
                var directory = Path.GetDirectoryName(filePath);
                if (directory != null && !Directory.Exists(directory))
                {   
                    
                    Directory.CreateDirectory(directory);

                }
                // Create the file
                using (var fs = new FileStream(filePath, FileMode.Create,FileAccess.Write)){

                    file.CopyTo(fs);
                }

                var metadata = new {
                    Ownername = upload.Owner,
                    Creation = DateTime.Now,
                    LastModified = DateTime.Now
                };
                using (var metadataStream = new FileStream(metadataFilePath, FileMode.Create, FileAccess.Write))
                using (var writer = new StreamWriter(metadataStream))
                {
                    var json = JsonConvert.SerializeObject(metadata, Formatting.Indented);
                    writer.Write(json);
                }

                return Ok("File uploaded successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete]
        public IActionResult Delete([FromQuery] string FileName,[FromQuery] string Ownername){
            if (string.IsNullOrEmpty(FileName) || string.IsNullOrEmpty(Ownername)){
                return BadRequest("FileName or Ownername is missing");
            }
            string path = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");

            string filePath = Path.Combine(path, $"{Ownername}_{FileName}");
            string metadataFilePath = Path.Combine(path, $"{Ownername}_{Path.GetFileNameWithoutExtension(FileName)}.json");
            if (!System.IO.File.Exists(filePath)){
                System.IO.File.Delete(filePath);
                System.IO.File.Delete(metadataFilePath);
                return Ok("File deleted successfully");
            }else{
                return BadRequest("File not found");
            }
            
        }
    }
}
