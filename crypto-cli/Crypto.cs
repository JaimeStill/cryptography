using System;
using System.IO;
using System.Runtime.Versioning;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace CryptoCli
{
    [SupportedOSPlatform("windows")]
    public static class Crypto
    {
        public static void RegisterRsa(string key, bool useMachine)
        {
            var parameters = useMachine
                ? new CspParameters { KeyContainerName = key, Flags = CspProviderFlags.UseMachineKeyStore }
                : new CspParameters { KeyContainerName = key };

            using var rsa = new RSACryptoServiceProvider(parameters);

            Console.WriteLine($"RSA Key registered to container {key}");
        }

        public static void RemoveRsa(string key)
        {
            var parameters = new CspParameters
            {
                KeyContainerName = key
            };

            using var rsa = new RSACryptoServiceProvider(parameters)
            {
                PersistKeyInCsp = false
            };

            rsa.Clear();

            Console.WriteLine($"RSA Key container removed: {key}");
        }

        public static async Task<(byte[] iv, byte[] key)> GenerateAes(string path, string key, bool showMessage = true)
        {
            try
            {
                path = EnsureCryptFile(path);

                var info = new FileInfo(path);

                if (info.Exists)
                    throw new IOException($"{info.FullName} already exists");

                using var rsa = RetrieveRsa(key);
                using var aes = Aes.Create();

                aes.Mode = CipherMode.CBC;
                aes.KeySize = 128;
                aes.BlockSize = 128;
                aes.FeedbackSize = 128;
                aes.Padding = PaddingMode.Zeros;

                aes.GenerateIV();
                aes.GenerateKey();

                using FileStream fs = new(path, FileMode.CreateNew);
                using StreamWriter writer = new(fs);

                var ivString = aes.IV.RsaEncrypt(rsa);
                var keyString = aes.Key.RsaEncrypt(rsa);

                await writer.WriteLineAsync(ivString);
                await writer.WriteLineAsync(keyString);

                if (showMessage) Console.WriteLine($"{info.FullName} successfully generated");

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

        static async Task<(byte[] iv, byte[] key)> RetrieveAes(string path, string rsaKey)
        {
            try
            {
                using var rsa = RetrieveRsa(rsaKey);
                using FileStream fs = new(path, FileMode.Open);
                using StreamReader reader = new(fs);

                var ivString = await reader.ReadLineAsync();
                var keyString = await reader.ReadLineAsync();

                byte[] iv = ivString.RsaDecrypt(rsa);
                byte[] key = keyString.RsaDecrypt(rsa);

                return (iv, key);
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

        public static async Task Encrypt(string file, string result, string aesPath, string key)
        {
            try
            {
                (file, result) = EnsureFiles(
                    file, result,
                    "A path to the file to be encrypted must be specified:",
                    "A path to the resulting encrypted file must be specified:",
                    "The file intended for encryption does not exist.",
                    "A file already exists at the path intended for the encrypted file."
                );

                Console.WriteLine($"Encrypting {file} to {result}...");

                aesPath = EnsureCryptFile(aesPath);

                var aesConfig = await InitializeAes(aesPath, key);

                using var aes = Aes.Create();

                aes.Mode = CipherMode.CBC;
                aes.KeySize = 128;
                aes.BlockSize = 128;
                aes.FeedbackSize = 128;
                aes.Padding = PaddingMode.Zeros;

                aes.IV = aesConfig.iv;
                aes.Key = aesConfig.key;

                await aes.EncryptFile(file, result);

                Console.WriteLine($"{result} successfully created with {aesPath}");
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

        public static async Task Decrypt(string file, string result, string aesPath, string key)
        {
            try
            {
                (file, result) = EnsureFiles(
                    file, result,
                    "A path to the file to be decrypted must be specified:",
                    "A path to the resulting decrypted file must be specified:",
                    "The file intended for decryption does not exist.",
                    "A file already exists at the path intended for the decrypted file."
                );

                Console.WriteLine($"Decrypting {file} to {result}...");

                aesPath = EnsureCryptFile(aesPath);
                var aesConfig = await InitializeAes(aesPath, key);

                using var aes = Aes.Create();

                aes.Mode = CipherMode.CBC;
                aes.KeySize = 128;
                aes.BlockSize = 128;
                aes.FeedbackSize = 128;
                aes.Padding = PaddingMode.Zeros;

                aes.IV = aesConfig.iv;
                aes.Key = aesConfig.key;

                await aes.DecryptFile(file, result);

                Console.WriteLine($"{result} successfully decrypted with {aesPath}");
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

        static string RsaEncrypt(this byte[] data, RSACryptoServiceProvider rsa) =>
            Convert.ToBase64String(rsa.Encrypt(data, RSAEncryptionPadding.Pkcs1));

        static byte[] RsaDecrypt(this string data, RSACryptoServiceProvider rsa) =>
            Convert.FromBase64String(data)
                .RsaDecrypt(rsa);

        static byte[] RsaDecrypt(this byte[] data, RSACryptoServiceProvider rsa) =>
            rsa.Decrypt(data, RSAEncryptionPadding.Pkcs1);

        static RSACryptoServiceProvider RetrieveRsa(string key) =>
            new RSACryptoServiceProvider(
                new CspParameters
                {
                    KeyContainerName = key
                }
            );

        static string EnsureValue(this string value, string message)
        {
            while (string.IsNullOrEmpty(value))
            {
                Console.WriteLine(message);
                value = Console.ReadLine();
            }

            return value;
        }

        static (string, string) EnsureFiles(
            string file,
            string result,
            string ensureFile,
            string ensureResult,
            string fileError,
            string resultError
        )
        {
            file = file.EnsureValue(ensureFile);
            result = result.EnsureValue(ensureResult);

            if (!File.Exists(file))
                throw new Exception(fileError);

            if (File.Exists(result))
                throw new Exception(resultError);

            return (file, result);
        }

        static string EnsureCryptFile(string file) =>
            file.EndsWith(".crypt")
                ? file
                : $"{file}.crypt";

        static async Task<(byte[] iv, byte[] key)> InitializeAes(string aesPath, string key) => 
            File.Exists(aesPath)
                ? await RetrieveAes(aesPath, key)
                : await GenerateAes(aesPath, key, false);

        static async Task InjectIv(this FileStream fs, byte[] iv) => await fs.WriteAsync(iv, 0, iv.Length);

        static async Task ProcessIv(this FileStream fs, byte[] iv)
        {
            int numBytesToRead = iv.Length;
            int numBytesRead = 0;

            while (numBytesToRead > 0)
            {
                var n = await fs.ReadAsync(iv, numBytesRead, numBytesToRead);
                if (n == 0) break;

                numBytesRead += n;
                numBytesToRead -= n;
            }
        }

        static async Task EncryptFile(this Aes aes, string file, string result)
        {
            try
            {
                using FileStream output = new(result, FileMode.CreateNew);

                await output.InjectIv(aes.IV);

                using CryptoStream cs = new(
                    output,
                    aes.CreateEncryptor(),
                    CryptoStreamMode.Write
                );

                using FileStream input = new(file, FileMode.Open);
                
                var buffer = new byte[1024];
                var read = await input.ReadAsync(buffer, 0, buffer.Length);

                while (read > 0)
                {
                    await cs.WriteAsync(buffer, 0, read);
                    read = await input.ReadAsync(buffer, 0, buffer.Length);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error encrypting file", ex);
            }
        }

        static async Task DecryptFile(this Aes aes, string file, string result)
        {
            using FileStream input = new(file, FileMode.Open);

            await input.ProcessIv(aes.IV);

            using CryptoStream cs = new(
                input,
                aes.CreateDecryptor(),
                CryptoStreamMode.Read
            );

            using FileStream output = new(result, FileMode.CreateNew);            
            
            var buffer = new byte[1024];
            var read = await cs.ReadAsync(buffer, 0, buffer.Length);

            while (read > 0)
            {
                await output.WriteAsync(buffer, 0, read);
                read = await cs.ReadAsync(buffer, 0, buffer.Length);
            }
        }
    }
}