using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;

using Crypto.Core;

namespace EncryptFile
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var path = args.Length > 0
                ? args[0]
                : GetPath();

            while (!File.Exists(path))
            {
                Console.WriteLine($"{path} is not a valid file path\n");
                path = GetPath();
            }

            var file = new FileInfo(path);
            Console.WriteLine($"Encrypting {file.FullName}...");

            using var rsa = CryptoExtensions.RetrieveRsa();
            var aesConfig = await rsa.GetAesConfig();

            using var aes = Aes.Create();
            aes.IV = aesConfig.iv;
            aes.Key = aesConfig.key;

            var encryptedFile = await aes.EncryptFile(file);
            Console.WriteLine($"File encrypted at {encryptedFile.FullName}.");
        }

        static string GetPath()
        {
            Console.WriteLine("File to encrypt:");
            return Console.ReadLine();
        }
    }
}
