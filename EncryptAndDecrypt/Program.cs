using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace EncryptAndDecrypt
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var file = args.Length > 0 && !(string.IsNullOrEmpty(args[0]))
                ? args[0]
                : "TestData.txt";

            using var aes = InitializeAes();
            var rsa = RSA.Create();
            await Encrypt(file, aes, rsa);
            await Decrypt(file, aes, rsa);
        }

        static Aes InitializeAes()
        {
            var aes = Aes.Create();
            aes.GenerateIV();
            aes.GenerateKey();

            return aes;
        }

        static async Task Encrypt(string file, Aes aes, RSA rsa)
        {
            try
            {
                using FileStream fs = new(file, FileMode.OpenOrCreate);
                await fs.WriteAsync(aes.IV, 0, aes.IV.Length);

                using CryptoStream cs = new(
                    fs,
                    aes.CreateEncryptor(),
                    CryptoStreamMode.Write
                );

                using StreamWriter ew = new(cs);
                
                ew.WriteLine(Convert.ToBase64String(rsa.Encrypt(aes.Key, RSAEncryptionPadding.Pkcs1)));

                Console.WriteLine("The file was encrypted.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"The encryption failed. {ex}");
            }
        }

        static async Task Decrypt(string file, Aes aes, RSA rsa)
        {
            try
            {
                using FileStream fs = new(file, FileMode.Open);
                int numBytesToRead = aes.IV.Length;
                int numBytesRead = 0;

                while (numBytesToRead > 0)
                {
                    var n = await fs.ReadAsync(aes.IV, numBytesRead, numBytesToRead);
                    if (n == 0) break;

                    numBytesRead += n;
                    numBytesToRead -= n;
                }

                using CryptoStream cs = new(
                    fs,
                    aes.CreateDecryptor(aes.Key, aes.IV),
                    CryptoStreamMode.Read
                );

                using StreamReader dr = new (cs);

                string encryptedString = await dr.ReadToEndAsync();
                byte[] encryptedKey = Convert.FromBase64String(encryptedString);
                byte[] key = rsa.Decrypt(encryptedKey, RSAEncryptionPadding.Pkcs1);

                Console.WriteLine($"The decrypted AES key: \n{Convert.ToBase64String(key)}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"The decryption failed. {ex}");
            }
        }
    }
}
