using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Crypto.Core
{
    public static class CryptoExtensions
    {
        static readonly string AES_FILE = @"..\aes.crypt";
        static readonly string RSA_CONTAINER = "EncryptFileRsa";
        
        public static RSACryptoServiceProvider RetrieveRsa() => new RSACryptoServiceProvider(
            new CspParameters
            {
                KeyContainerName = RSA_CONTAINER
            }
        );

        public static async Task<(byte[] iv, byte[] key)> GetAesConfig(this RSACryptoServiceProvider rsa)
        {
            return File.Exists(AES_FILE)
                ? await rsa.RetrieveAesKeys()
                : await rsa.RegisterAesKeys();
        }

        public static async Task<FileInfo> EncryptFile(this Aes aes, FileInfo file)
        {
            try
            {
                var encryptedFile = Path.Join(file.DirectoryName, $"encrypted-{file.Name}");

                if (File.Exists(encryptedFile))
                    File.Delete(encryptedFile);

                using FileStream fs = new(encryptedFile, FileMode.CreateNew);
                await fs.WriteAsync(aes.IV, 0, aes.IV.Length);

                using CryptoStream cs = new(
                    fs,
                    aes.CreateEncryptor(),
                    CryptoStreamMode.Write
                );

                using StreamWriter writer = new(cs);
                using var reader = file.OpenText();
                await writer.WriteAsync(await reader.ReadToEndAsync());

                return new FileInfo(encryptedFile);
            }
            catch (IOException ex)
            {
                throw new Exception("File creation failed", ex);
            }
            catch (CryptographicException ex)
            {
                throw new Exception("The encryption failed", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred.", ex);
            }
        }

        public static async Task<FileInfo> DecryptFile(this Aes aes, FileInfo file)
        {
            try
            {
                var name = file.Name.StartsWith("encrypted-")
                    ? file.Name.Replace("encrypted-", "decrypted-")
                    : $"decrypted-{file.Name}";

                var decryptedFile = Path.Join(file.DirectoryName, name);

                if (File.Exists(decryptedFile))
                    File.Delete(decryptedFile);

                using FileStream fs = new(file.FullName, FileMode.Open);
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
                    aes.CreateDecryptor(),
                    CryptoStreamMode.Read
                );

                using StreamReader reader = new (cs);
                using var writer = File.CreateText(decryptedFile);
                
                string data = await reader.ReadToEndAsync();
                await writer.WriteAsync(data);

                return new FileInfo(decryptedFile);
            }
            catch (IOException ex)
            {
                throw new Exception("File creation failed", ex);
            }
            catch (CryptographicException ex)
            {
                throw new Exception("The encryption failed", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred.", ex);
            }
        }

        static async Task<(byte[] iv, byte[] key)> RetrieveAesKeys(this RSACryptoServiceProvider rsa)
        {
            try
            {
                using FileStream fs = new(AES_FILE, FileMode.Open);
                using StreamReader reader = new(fs);

                var ivString = await reader.ReadLineAsync();
                var keyString = await reader.ReadLineAsync();

                Console.WriteLine($"Retrieved IV: {ivString}");
                Console.WriteLine($"Retrieved Key: {keyString}");

                byte[] encryptedIv = Convert.FromBase64String(ivString);
                byte[] encryptedKey = Convert.FromBase64String(keyString);

                return (
                    rsa.Decrypt(encryptedIv, RSAEncryptionPadding.Pkcs1),
                    rsa.Decrypt(encryptedKey, RSAEncryptionPadding.Pkcs1)
                );
            }
            catch (CryptographicException ex)
            {
                throw new Exception("The decryption failed", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred.", ex);
            }
        }

        static async Task<(byte[] iv, byte[] key)> RegisterAesKeys(this RSACryptoServiceProvider rsa)
        {
            try
            {
                using var aes = Aes.Create();
                aes.GenerateIV();
                aes.GenerateKey();

                using FileStream fs = new(AES_FILE, FileMode.CreateNew);
                using StreamWriter writer = new(fs);

                var ivString = Convert.ToBase64String(rsa.Encrypt(aes.IV, RSAEncryptionPadding.Pkcs1));
                var keyString = Convert.ToBase64String(rsa.Encrypt(aes.Key, RSAEncryptionPadding.Pkcs1));
                
                Console.WriteLine($"Registering IV: {ivString}");
                Console.WriteLine($"Registering Key: {keyString}");

                await writer.WriteLineAsync(ivString);
                await writer.WriteLineAsync(keyString);

                return (aes.IV, aes.Key);
            }
            catch (IOException ex)
            {
                throw new Exception("File creation failed", ex);
            }
            catch (CryptographicException ex)
            {
                throw new Exception("The encryption failed", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred.", ex);
            }
        }
    }
}
