using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class Filter_TransOnwer_Task49 : ControllerBase
    {
        public class FormData 
        {
            public string? Owner { get; set; }
            public DateTime CreationDate { get; set; } = DateTime.Now;
            public DateTime ModificationDate { get; set; } = DateTime.Now;
            public FilterType? FilterType { get; set; }
        }
        
        public enum FilterType
        {
            ByOwner = 1,
            ByCreationDateAscending,
            ByCreationDateDescending,
            ByModificationDate
        }

        private class Metadata
        {
            public string? Ownername { get; set; }
            public DateTime CreationDate { get; set; }
            public DateTime LastModifiedDate { get; set; }
        }

        [HttpPost("Filter")]
        public IActionResult Filter([FromBody] FormData formData)
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
            var metaPath = Path.Combine(path, "metadata");

            // Check if directories exist
            if (!Directory.Exists(path) || !Directory.Exists(metaPath))
                return BadRequest("Directory or file does not exist");

            // Fetch files from directories
            var files = Directory.EnumerateFiles(path, "*.jpg", SearchOption.AllDirectories).ToList();
            var metaFiles = Directory.EnumerateFiles(metaPath, "*.json", SearchOption.AllDirectories).ToList();

            // Check if form data was provided
            if (formData == null)
                return BadRequest("Invalid request");

            // Check if the owner was provided
            if (string.IsNullOrEmpty(formData.Owner))
                return BadRequest("No owner was provided");

            // Read and parse metadata
            var metadataDict = ReadMetadataFromFiles(metaFiles);
            // Filter files based on the filter type
            IEnumerable<string> filteredFiles;
            switch (formData.FilterType)
            {
                case FilterType.ByOwner:
                    filteredFiles = files.Where(file => metadataDict.ContainsKey(Path.GetFileNameWithoutExtension(file)) && metadataDict[Path.GetFileNameWithoutExtension(file)].Ownername == formData.Owner);
                    break;

                case FilterType.ByCreationDateAscending:
                    filteredFiles = files.Where(file => metadataDict[Path.GetFileNameWithoutExtension(file)].CreationDate <= formData.CreationDate);

                    break;

                case FilterType.ByCreationDateDescending:
                    filteredFiles = files.Where(file => metadataDict[Path.GetFileNameWithoutExtension(file)].CreationDate <= formData.CreationDate);
                    filteredFiles = filteredFiles.Reverse();
                    break;

                case FilterType.ByModificationDate:
                    filteredFiles = files.Where(file => metadataDict[Path.GetFileNameWithoutExtension(file)].LastModifiedDate <= formData.ModificationDate);
                    break;

                default:
                    return BadRequest("Invalid filter type");
            }

            if (filteredFiles.Any())
            {
                return Ok(filteredFiles.ToList());
            }
            else
            {
                return BadRequest("No files found");
            }
        }

        private static Dictionary<string, Metadata> ReadMetadataFromFiles(IEnumerable<string> metaFiles)
        {
            var metadataDict = new Dictionary<string, Metadata>();

            foreach (var file in metaFiles)
            {
                var json = System.IO.File.ReadAllText(file);
                var data = JsonConvert.DeserializeObject<Metadata>(json);

                if (data != null)
                {
                    var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file);
                    if (!metadataDict.ContainsKey(fileNameWithoutExtension))
                    {
                        metadataDict.TryAdd(fileNameWithoutExtension, data);
                    }
                }
            }

            return metadataDict;
        }
        [HttpGet("SwitchOwnerShip")]
public IActionResult SwitchOwnerShip([FromQuery] string OldOwner, [FromQuery] string NewOwner)
{
    // Check if both names were provided
    if (string.IsNullOrEmpty(OldOwner) || string.IsNullOrEmpty(NewOwner))
        return BadRequest("OldOwner or NewOwner is missing");
    var path = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
    var metaPath = Path.Combine(path, "metadata");
    // Check if directories exist
    if (!Directory.Exists(path) || !Directory.Exists(metaPath))
        return BadRequest("Directory or file does not exist");
    // Fetch files from directories
    var files = Directory.EnumerateFiles(path, "*.jpg", SearchOption.AllDirectories).ToList();
    var metaFiles = Directory.EnumerateFiles(metaPath, "*.json", SearchOption.AllDirectories).ToList();
    // Read and parse metadata
    var metadataDict = ReadMetadataFromFiles(metaFiles);
    // Check if old owner exists
    if (files.Where(file => Path.GetFileNameWithoutExtension(file) == OldOwner).Any())
    {
        return BadRequest("Old owner does not exist");
    }
    // Filter files based on old owner
    var oldOwnerFiles = files.Where(file => metadataDict[Path.GetFileNameWithoutExtension(file)].Ownername == OldOwner).ToList();
    try
    {
        // Rename files
        foreach (var file in oldOwnerFiles)
        {
            var oldFileName = Path.GetFileNameWithoutExtension(file);
            var newFileName = $"{NewOwner}_{oldFileName.Split('_').Skip(1).FirstOrDefault()}";
            System.IO.File.Move(file, Path.Combine(path, newFileName + ".jpg"));
        }
        // Update metadata and rename metadata files
        foreach (var file in oldOwnerFiles)
        {
            var oldFileName = Path.GetFileNameWithoutExtension(file);
            if (metadataDict.TryGetValue(oldFileName, out var metadata))
            {
                metadata.Ownername = NewOwner;
                var json = JsonConvert.SerializeObject(metadata);
                var newMetaFileName = $"{NewOwner}_{oldFileName.Split('_').Skip(1).FirstOrDefault()}.json";
                System.IO.File.WriteAllText(Path.Combine(metaPath, newMetaFileName), json);

                // Remove old metadata file
                System.IO.File.Delete(Path.Combine(metaPath, $"{oldFileName}.json"));
            }
        }
    }
    catch (Exception ex)
    {
        return BadRequest($"Error while switching ownership: {ex.Message}");
    }

    return Ok("Owner switched successfully");
}

    }
}
