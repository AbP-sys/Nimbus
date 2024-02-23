using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IO;


namespace Nimbus.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DriveController : ControllerBase
    {
        private string indexPath = "fileSystem.json";

        [HttpGet("{**path}")]
        public IActionResult GetFile(string? path)
        {
            try
            {
                string fullPath = Path.Join(Directory.GetCurrentDirectory(), indexPath);
                var root = JsonConvert.DeserializeObject<FileSystemItem>(System.IO.File.ReadAllText(fullPath));
                var item = FindItemByPath(root, path);

                if (item != null)
                {
                    return Ok(item);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = "Internal Server Error" });
            }
        }

        public class FileSystemItem
        {
            public string Name { get; set; }
            public bool IsFolder { get; set; }
            public List<FileSystemItem> Children { get; set; }
        }
        private FileSystemItem FindItemByPath(FileSystemItem root, string path)
        {
            if (path == null)
            {
                return root;
            }
            string[] parts = path.Split('/');

            FileSystemItem current = root;
            foreach (var part in parts)
            {
                if (current.Children != null)
                {
                    current = current.Children.FirstOrDefault(c => c.Name == part);
                    if (current == null)
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }

            return current;
        }
    }

}

