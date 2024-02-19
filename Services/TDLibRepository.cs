using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Threading.Tasks;
using ElectronNET.API;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json.Linq;
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
        private string? keyString;
        private string? ivString;
        byte[] encryptionKey;
        byte[] initializationVector;
        private string tempPath;

        public TDLibRepository(int? apiID, string? apiHash, string? phoneNumber)
        {
            if (apiID != null && apiHash != null)
            {
                client = new WTelegram.Client((int)apiID, apiHash);
                _phone_number = phoneNumber;
            }
            Directory.CreateDirectory(".temp");
            tempPath = ".temp/";
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
            try
            {
                keyString = System.Environment.GetEnvironmentVariable("KEY_STRING") ?? throw new NullReferenceException("KEY_STRING is null");
                ivString = System.Environment.GetEnvironmentVariable("IV_STRING") ?? throw new NullReferenceException("IV_STRING is null");
                using (var aes = Aes.Create())
                {
                    encryptionKey = Convert.FromBase64String(keyString);
                    initializationVector = Convert.FromBase64String(ivString);
                }
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine($"ENV variable not found: {e.Message}. Please generate encryption keys using the installation script");
            }


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
            var rootPath = uploadPath;
            // Get all files with the specified extension in the root directory
            var photoFiles = Directory.GetFiles(rootPath);
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
        }

        public async Task<JArray> DownloadFiles(int offset = 0)
        {
            Messages_MessagesBase messages = await client.Messages_GetHistory(InputPeer.Self, add_offset: offset, limit: 20);
            JArray results = new();
            foreach (var message in messages.Messages)
            {
                if (message is Message msg && msg.media is MessageMediaDocument media)
                {
                    // Get the document object and its attributes
                    var document = media.document as Document;
                    var attributes = document.attributes;


                    var fileName = document.Filename; //document.Filename ends with .enc
                    var decryptedFileName = document.Filename.Substring(0, document.Filename.Length - 4);
                    var fileSize = document.size / (1024 * 1024);
                    Console.WriteLine($"Processing document: {fileName} ({fileSize} MB)");

                    if (File.Exists(tempPath + decryptedFileName))
                    {
                        results.Add(Path.GetFileName(decryptedFileName));
                        continue;
                    }

                    using (FileStream fileStreamWrite = new FileStream(tempPath + fileName, FileMode.Create, FileAccess.Write))
                    {
                        await client.DownloadFileAsync(document, fileStreamWrite);
                    }

                    Console.WriteLine("Decrypting..." + document.Filename);
                    var decryptedFile = AESEncryptor.DecryptFile(tempPath + fileName, encryptionKey, initializationVector);
                    results.Add(Path.GetFileName(decryptedFile));
                }
                else
                {
                    Console.WriteLine($"Unable to download message: {message.ID} sent on {message.Date}");
                }

            }
            return results;
        }
    }
}
