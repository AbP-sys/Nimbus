using System;
using System.Data;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Nimbus.Interfaces;
using Nimbus.Services;

namespace Nimbus.Controllers
{
    [ApiController]
    [Route("")]
    public class HomeController : ControllerBase
    {

        private readonly ITDLibRepository _tdLibRepository;
        private readonly string TempDirectoryPath = Path.Combine(Directory.GetCurrentDirectory(), ".temp");
        private const int fetchAtOnce = 10;
        public HomeController(ITDLibRepository tdLibRepository)
        {
            _tdLibRepository = tdLibRepository;
        }

        [HttpGet("home&{offset}")]
        public async Task<IActionResult> GetPhotos(int offset)
        {
            bool login = await _tdLibRepository.InitClient(false);
            JArray results = new();
            if (login)
            {
                results = await _tdLibRepository.DownloadFiles(offset, fetchAtOnce);
            }
            else
                Console.WriteLine("Please login");
            return Ok(results.ToString());
        }

        [HttpGet("{filename}")]
        public IActionResult GetFile(string filename)
        {
            string imagePath = Path.Combine(TempDirectoryPath, filename);

            if (System.IO.File.Exists(imagePath))
            {
                byte[] imageBytes = System.IO.File.ReadAllBytes(imagePath);
                string contentType = "image/jpg";
                return File(imageBytes, contentType);
            }
            else
            {
                return NotFound();
            }
        }

    }
}
