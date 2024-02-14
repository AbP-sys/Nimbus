using System.Security.Cryptography;
using Microsoft.VisualBasic;
using Nimbus.Interfaces;
using Nimbus.Services.Encryption;
using TL;
using WTelegram;

namespace Nimbus.Services
{
    public class TDLibRepository: ITDLibRepository
    {
         static string Config(string what)
        {
            if (what == "api_id") return System.Environment.GetEnvironmentVariable("API_ID");
            if (what == "api_hash") return System.Environment.GetEnvironmentVariable("API_HASH");
            if (what == "phone_number") return System.Environment.GetEnvironmentVariable("PHONE_NUMBER");
            if (what == "verification_code") return null; // let WTelegramClient ask the user with a console prompt 
            if (what == "first_name") return "John";      // if sign-up is required
            if (what == "last_name") return "Doe";        // if sign-up is required
            if (what == "password") return "secret!";     // if user has enabled 2FA
            return null;
        }

        public TDLibRepository()
        {
            DotNetEnv.Env.TraversePath().Load();
        }
        public static async Task UploadFiles(string uploadPath)
        {
            
            using var client = new WTelegram.Client(Config);
            var user = await client.LoginUserIfNeeded();
            Console.WriteLine($"Logged-in as {user.username ?? user.first_name + " " + user.last_name} (id {user.id})");

            var rootPath = uploadPath;
            string? keyString = System.Environment.GetEnvironmentVariable("KEY_STRING");
            string? ivString = System.Environment.GetEnvironmentVariable("IV_STRING");
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
    }   
}
