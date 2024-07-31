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
        public class Metadata {
            public string? Ownername { get; set; }
            public string CreationDate { get; set; } = DateTime.Now.ToString();
            public string LastModifiedDate { get; set; } = DateTime.Now.ToString();
        }

        [HttpPost("Create")]
        public IActionResult Create([FromForm] Upload upload)
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
            //Creating the Metadata folder
            string metadatapath = Path.Combine(path, "metadata");
            string metadataFilePath = Path.Combine(metadatapath, $"{safeOwnerName}_{Path.GetFileNameWithoutExtension(file.FileName)}.json");
        
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
                //Check if the Metadata folder exists
                var metadataDir = Path.GetDirectoryName(metadataFilePath);
                if (metadataDir != null && !Directory.Exists(metadataDir)){
                    Directory.CreateDirectory(metadataDir);
                }
                // Create the metadata
                var metadata = new Metadata(){
                    Ownername = upload.Owner,
                    CreationDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    LastModifiedDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
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

        [HttpDelete("Delete")]
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
