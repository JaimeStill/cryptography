using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Runtime.Versioning;
using System.Threading.Tasks;

namespace CryptoCli
{
    [SupportedOSPlatform("windows")]
    public static class Commands
    {
        public static RootCommand Initialize()
        {
            var commands = BuildCommands();
            return commands.BuildRootCommand();
        }

        static RootCommand BuildRootCommand(this List<Command> commands)
        {
            var root = new RootCommand("Cryptography CLI");
            commands.ForEach(command => root.AddCommand(command));
            return root;
        }

        static List<Command> BuildCommands() => new List<Command>
        {
            BuildRegisterRsa(),
            BuildRemoveRsa(),
            BuildGenerateAes(),
            BuildEncrypt(),
            BuildDecrypt()
        };

        static Command BuildRegisterRsa () =>
            BuildCommand(
                "register-rsa",
                "Register an RSA key inside of a Key Container",
                new List<Option>
                {
                    new Option<string>(
                        new[] { "--key", "-k" },
                        getDefaultValue: () => "CryptoCliRsa",
                        description: "The name of the Key Container"
                    ),
                    new Option<bool>(
                        new[] { "--machine-store", "-m" },
                        getDefaultValue: () => false,
                        description: "Register the Key Container in the Machine Key Store"
                    )
                },
                new Action<string, bool>((key, useMachine) => Crypto.RegisterRsa(key, useMachine))
            );

        static Command BuildRemoveRsa() =>
            BuildCommand(
                "remove-rsa",
                "Remove an RSA key inside of a Key Container",
                new List<Option>
                {
                    new Option<string>(
                        new[] { "--key", "-k" },
                        getDefaultValue: () => "CryptoCliRsa",
                        description: "The name of the Key Container"                        
                    )
                },
                new Action<string>((key) => Crypto.RemoveRsa(key))
            );

        static Command BuildGenerateAes() =>
            BuildCommand(
                "generate-aes",
                "Generate an AES key file with a .crypt extension for use with this CLI app",
                new List<Option>
                {
                    new Option<string>(
                        new[] { "--path", "-p" },
                        getDefaultValue: () => @".\aes.crypt",
                        description: "The path to the file to be generated (including the file name)"
                    ),
                    new Option<string>(
                        new[] { "--key", "-k" },
                        getDefaultValue: () => "CryptoCliRsa",
                        description: "The name of the Key Container for AES key encryption"
                    )
                },
                new Action<string, string>(async (path, key) => await Crypto.GenerateAes(path, key))
            );

        static Command BuildEncrypt() =>
            BuildCommand(
                "encrypt",
                "Encrypt a file using a .crypt AES key file",
                new List<Option>
                {
                    new Option<string>(
                        new[] { "--file", "-f" },
                        getDefaultValue: () => string.Empty,
                        description: "The path to the file to be encrypted (including the file name)"
                    ),
                    new Option<string>(
                        new[] { "--result", "-r" },
                        getDefaultValue: () => string.Empty,
                        description: "The path to the resulting encrypted file (including the file name)"
                    ),
                    new Option<string>(
                        new[] { "--aes", "-a" },
                        getDefaultValue: () => @".\aes.crypt",
                        description: "The path to the .crypt AES key file"
                    ),
                    new Option<string>(
                        new[] { "--key", "-k" },
                        getDefaultValue: () => "CryptoCliRsa",
                        description: "The name of the Key Container used to encrypt the .crypt AES key file"
                    )
                },
                new Func<string, string, string, string, Task>(
                    async (file, result, aes, key) => await Crypto.Encrypt(file, result, aes, key)
                )
            );

        static Command BuildDecrypt() =>
            BuildCommand(
                "decrypt",
                "Decrypt a file using a .crypt AES key file",
                new List<Option>
                {
                    new Option<string>(
                        new[] { "--file", "-f" },
                        getDefaultValue: () => string.Empty,
                        description: "The path to the file to be decrypted (including the file name)"
                    ),
                    new Option<string>(
                        new[] { "--result", "-r" },
                        getDefaultValue: () => string.Empty,
                        description: "The path to the resulting decrypted file (including the file name)"
                    ),
                    new Option<string>(
                        new[] { "--aes", "-a" },
                        getDefaultValue: () => @".\aes.crypt",
                        description: "The path to the .crypt AES key file"
                    ),
                    new Option<string>(
                        new[] { "--key", "-k" },
                        getDefaultValue: () => "CryptoCliRsa",
                        description: "The name of the Key Container used to encrypt the .crypt AES key file"
                    )
                },
                new Func<string, string, string, string, Task>(
                    async (file, result, aes, key) => await Crypto.Decrypt(file, result, aes, key)
                )
            );

        static Command BuildCommand(string name, string description, List<Option> options, Delegate @delegate)
        {
            var command = new Command(name, description);

            options.ForEach(option => command.AddOption(option));

            command.Handler = CommandHandler.Create(@delegate);

            return command;
        }        
    }
}