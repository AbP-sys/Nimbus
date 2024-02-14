using System;
using System.IO;
using System.Security.Cryptography;

string path = "../../../../.env"; 
Console.WriteLine("WARNING!!! You are about to generate new encryptions keys. Make sure that the data you have encrypted with the previous keys have been decrypted.\n");
var option = Console.Read();
byte[] encryptionKey;
byte[] initializationVector;
using (var aes = Aes.Create())
{
    encryptionKey = aes.Key;
    initializationVector = aes.IV; 
}
string keyString = $"KEY_STRING={Convert.ToBase64String(encryptionKey)}";
string ivString = $"IV_STRING={Convert.ToBase64String(initializationVector)}";
File.AppendAllText(path, keyString + "\n");
File.AppendAllText(path, ivString + "\n");
