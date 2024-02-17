using ElectronNET.API;
using Microsoft.AspNetCore.Mvc;
using Nimbus.Interfaces;
using Nimbus.Services;
using System.IO;
using System.Threading.Tasks;

namespace Nimbus.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly ITDLibRepository _tDLibRepository;
        public LoginController(ITDLibRepository tDLibRepository)
        {
            _tDLibRepository = tDLibRepository;
        }
        [HttpPost]
        public async Task<IActionResult> InitConfig([FromBody] ConfigModel configRequest)
        {
            configRequest.SetEnvironmentVariables();
            await _tDLibRepository.InitClient(configRequest.API_ID, configRequest.API_HASH, configRequest.PHONE_NUMBER, true);
            return Ok(new { Message = "Configuration initialized successfully" });
        }
    }

    public class ConfigModel
    {
        public int API_ID { get; set; }
        public string API_HASH { get; set; }
        public string PHONE_NUMBER { get; set; }

        public void SetEnvironmentVariables()
        {
            var path = "../../../.env";
            string apiID = $"API_ID={API_ID}";
            string apiHash = $"API_HASH={API_HASH}";
            string phoneNumber = $"PHONE_NUMBER={PHONE_NUMBER}";
            File.WriteAllText(path, apiID + "\n");
            File.AppendAllText(path, apiHash + "\n");
            File.AppendAllText(path, phoneNumber + "\n");

        }
    }
}