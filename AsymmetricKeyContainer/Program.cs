using System;
using System.Runtime.Versioning;
using System.Security.Cryptography;

namespace AsymmetricKeyContainer
{
    
    [SupportedOSPlatform("windows")]
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                GetContainerKey("MyKeyContainer");
                DeleteContainerKey("MyKeyContainer");
                SaveMachineContainerKey("MyMachineContainer");
                DeleteContainerKey("MyMachineContainer");
            }   
            catch (CryptographicException e)
            {
                Console.WriteLine(e.Message);
            }         
        }

        static void SaveMachineContainerKey(string containerName)
        {
            var parameters = new CspParameters
            {
                KeyContainerName = containerName,
                Flags = CspProviderFlags.UseMachineKeyStore
            };

            using var rsa = new RSACryptoServiceProvider(parameters);

            Console.WriteLine($"Key added to container: \n {rsa.ToXmlString(true)}");
        }

        static void SaveContainerKey(string containerName)
        {
            var parameters = new CspParameters
            {
                KeyContainerName = containerName
            };

            using var rsa = new RSACryptoServiceProvider(parameters);

            Console.WriteLine($"Key added to container: \n {rsa.ToXmlString(true)}");
        }

        static void GetContainerKey(string containerName)
        {
            var parameters = new CspParameters
            {
                KeyContainerName = containerName
            };

            using var rsa = new RSACryptoServiceProvider(parameters);

            Console.WriteLine($"Key retrieved from container: \n {rsa.ToXmlString(true)}");
        }

        static void DeleteContainerKey(string containerName)
        {
            var parameters = new CspParameters
            {
                KeyContainerName = containerName
            };

            using var rsa = new RSACryptoServiceProvider(parameters)
            {
                PersistKeyInCsp = false
            };

            rsa.Clear();

            Console.WriteLine("Key deleted.");
        }
    }
}
