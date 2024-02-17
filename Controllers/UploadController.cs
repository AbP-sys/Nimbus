using System;
using System.Data;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nimbus.Interfaces;
using Nimbus.Services;

namespace Nimbus.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UploadController : ControllerBase
    {

        private readonly ITDLibRepository _tdLibRepository;
        public UploadController(ITDLibRepository tdLibRepository)
        {
            _tdLibRepository = tdLibRepository;
        }
        [HttpPost]
        public async Task<IActionResult> SubmitForm([FromBody] string path)
        {
            if (!Directory.Exists(path))
            {
                return UnprocessableEntity(path);
            }
            bool login = await _tdLibRepository.InitClient(false);
            if (login)
                _tdLibRepository.UploadFiles(path);
            else
                Console.WriteLine("Please login");
            return Ok();
        }
    }
}
