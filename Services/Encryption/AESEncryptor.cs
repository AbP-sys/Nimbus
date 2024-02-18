using System;
using System.IO;
using System.Security.Cryptography;

namespace Nimbus.Services.Encryption
{
    public static class AESEncryptor
    {
        public static string EncryptFile(string inputFile, byte[] key, byte[] iv)
        {
            string outputFile = ".temp/" + Path.GetFileName(inputFile) + ".enc";

            using (var aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;

                using (var inputStream = new FileStream(inputFile, FileMode.Open, FileAccess.Read))
                using (var outputStream = new FileStream(outputFile, FileMode.Create, FileAccess.Write))
                using (var cryptoStream = new CryptoStream(outputStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    inputStream.CopyTo(cryptoStream);
                }
            }
            Console.WriteLine($"Encryption complete. Encrypted file saved to: {outputFile}");
            return outputFile;
        }

        public static string DecryptFile(string inputFile, byte[] key, byte[] iv)
        {
            string outputFile = inputFile.Substring(0, inputFile.Length - 4);

            using (var aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;

                using (var inputStream = new FileStream(inputFile, FileMode.Open, FileAccess.Read))
                using (var outputStream = new FileStream(outputFile, FileMode.Create, FileAccess.Write))
                using (var cryptoStream = new CryptoStream(outputStream, aes.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    inputStream.CopyTo(cryptoStream);
                }
            }

            Console.WriteLine($"Decryption complete. Decrypted file saved to: {outputFile}");
            return outputFile;
        }

    }

}