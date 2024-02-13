using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace Nimbus.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UploadController : ControllerBase
    {
        [HttpPost]
        public IActionResult SubmitForm([FromBody] string path)
        {
            if (!Directory.Exists(path))
            {
                return UnprocessableEntity(path);
            }
            return Ok();
        }
    }
}
