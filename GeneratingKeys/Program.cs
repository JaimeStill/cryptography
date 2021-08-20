using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace GeneratingKeys
{
    class Program
    {
        static void Main(string[] args)
        {
            var aes = InitializeAes();
            Console.WriteLine(aes.Stringify());

            var rsa = InitializeRsa();            
            Console.WriteLine(rsa.pk.Stringify());
        }

        static Aes InitializeAes()
        {
            var aes = Aes.Create();
            aes.GenerateIV();
            aes.GenerateKey();

            return aes;
        }

        static (RSA value, RSAParameters pk) InitializeRsa()
        {
            var value = RSA.Create();
            var pk = value.ExportParameters(false);

            return (value, pk);
        }
    }

    static class ProgramExtensions
    {
        public static string Stringify(this byte[]? data) =>
            data?.Length > 0
                ? data
                    .Select(x => x.ToString())
                    .Aggregate((accumulate, value) => $"{accumulate}-{value}")
                : "N/A";

        public static string Stringify(this Aes aes) =>
            new StringBuilder()
                .AppendLine("Key:")
                .AppendLine(aes.Key.Stringify())
                .AppendLine("IV:")
                .AppendLine(aes.IV.Stringify())
                .ToString();

        public static string Stringify(this RSAParameters pk) =>
            new StringBuilder()
                .AppendLine("RSA PK Parameters")
                .AppendLine("Exponent:")
                .AppendLine(pk.Exponent.Stringify())
                .AppendLine("Modulus:")
                .AppendLine(pk.Modulus.Stringify())
                .ToString();
    }
}
