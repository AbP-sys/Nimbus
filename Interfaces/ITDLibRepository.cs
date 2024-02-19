using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Nimbus.Interfaces
{
    public interface ITDLibRepository
    {
        public Task<bool> InitClient(bool sendCode);
        public Task<bool> InitClient(int apiID, string apiHash, string phoneNumber, bool sendVerificationCode);
        public Task UploadFiles(string uploadPath);
        public Task<JArray> DownloadFiles(int offset = 0);
    }
}