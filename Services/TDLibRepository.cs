using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;
using ElectronNET.API;
using Microsoft.Extensions.Logging.Abstractions;
using Nimbus.Interfaces;
using Nimbus.Services.Encryption;
using TL;
using WTelegram;

namespace Nimbus.Services
{
    public class TDLibRepository : ITDLibRepository
    {
        private static Client client;
        private string? _phone_number;

        public TDLibRepository(int? apiID, string? apiHash, string? phoneNumber)
        {
            if (apiID != null && apiHash != null)
            {
                client = new WTelegram.Client((int)apiID, apiHash);
                _phone_number = phoneNumber;
            }
        }

        public async Task<bool> InitClient(int apiID, string apiHash, string phoneNumber, bool getVerificationCode)
        {
            client = new WTelegram.Client(apiID, apiHash);
            _phone_number = phoneNumber;
            bool login = await DoLogin(_phone_number, getVerificationCode);
            return login;
        }
        public async Task<bool> InitClient(bool getVerificationCode)
        {
            bool login = await DoLogin(_phone_number, getVerificationCode);
            return login;
        }

        private static async Task<bool> DoLogin(string loginInfo, bool getVerificationCode)
        {
            if (getVerificationCode == false && client == null)
                return false;
            if (getVerificationCode == false && client != null)
            {
                return await client.Login(loginInfo) == null;
            }
            while (client.User == null)
            {
                switch (await client.Login(loginInfo))
                {
                    case "verification_code":
                        // Use TaskCompletionSource to await the IPC event
                        var verificationCodeTaskCompletionSource = new TaskCompletionSource<string>();

                        // Set up IPC event handler
                        await Electron.IpcMain.On("login", async (args) =>
                        {
                            string code = args.ToString();
                            verificationCodeTaskCompletionSource.SetResult(code);
                        });

                        loginInfo = await verificationCodeTaskCompletionSource.Task;
                        break;

                    //these cases are for signing up on Telegram, Nimbus doesn't support this yet
                    case "name":
                        loginInfo = "John Doe";
                        break;

                    case "password":
                        loginInfo = "secret!";
                        break;

                    default:
                        loginInfo = null;
                        break;
                }
            }
            Console.WriteLine($"We are logged-in as {client.User} (id {client.User.id})");
            return true;
        }

        public async Task UploadFiles(string uploadPath)
        {
            try
            {
                var rootPath = uploadPath;
                string keyString = System.Environment.GetEnvironmentVariable("KEY_STRING") ?? throw new NullReferenceException("KEY_STRING is null");
                string ivString = System.Environment.GetEnvironmentVariable("IV_STRING") ?? throw new NullReferenceException("IV_STRING is null");
                byte[] encryptionKey;
                byte[] initializationVector;

                using (var aes = Aes.Create())
                {
                    encryptionKey = Convert.FromBase64String(keyString);
                    initializationVector = Convert.FromBase64String(ivString);
                }
                Console.WriteLine($"{Convert.ToBase64String(encryptionKey)}");
                Console.WriteLine($"{Convert.ToBase64String(initializationVector)}");
                // Get all files with the specified extension in the root directory
                var photoFiles = Directory.GetFiles(rootPath);
                Directory.CreateDirectory(".temp");
                int i = 0;
                foreach (var photoFile in photoFiles)
                {
                    var fileName = Path.GetFileName(photoFile);
                    Console.WriteLine("Encrypting...");
                    var encryptedFile = AESEncryptor.EncryptFile(photoFile, encryptionKey, initializationVector);

                    Console.WriteLine($"Sending document: {fileName}");

                    var inputFile = await client.UploadFileAsync(encryptedFile);
                    await client.SendMediaAsync(InputPeer.Self, "Here is the photo", inputFile);
                    Console.WriteLine($"Uploaded {++i} / {photoFiles.Length}");


                }

                var encFiles = Directory.GetFiles(Directory.GetCurrentDirectory() + "/.temp", "*.enc");
                foreach (var photoFile in encFiles)
                {
                    var fileName = Path.GetFileName(photoFile);
                    Console.WriteLine("Decrypting..." + fileName);
                    var decryptedFile = AESEncryptor.DecryptFile(photoFile, encryptionKey, initializationVector);

                    Console.WriteLine($"Sending document: {fileName}");
                }
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine($"ENV variable not found: {e.Message}. Please generate encryption keys using the installation script");
            }

        }
    }
}
