using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;

using Crypto.Core;

namespace DecryptFile
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
            Console.WriteLine($"Decrypting {file.FullName}...");

            using var rsa = CryptoExtensions.RetrieveRsa();
            var aesConfig = await rsa.GetAesConfig();

            using var aes = Aes.Create();
            aes.IV = aesConfig.iv;
            aes.Key = aesConfig.key;

            var decryptedFile = await aes.DecryptFile(file);
            Console.WriteLine($"File decrypted at {decryptedFile.FullName}.");
        }

        static string GetPath()
        {
            Console.WriteLine("File to decrypt:");
            return Console.ReadLine();
        }
    }
}
