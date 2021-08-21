# Cryptography in .NET

> This is purely for academic purposes and should not be assumed to be safe.

Experiments written through studying the [Cryptography Model](https://docs.microsoft.com/en-us/dotnet/standard/security/cryptography-model).

[![Publish](https://github.com/JaimeStill/cryptography/actions/workflows/publish.yml/badge.svg)](https://github.com/JaimeStill/cryptography/actions/workflows/publish.yml)

## Latest Release: [crypto-cli](https://github.com/JaimeStill/cryptography/releases/tag/v0.1-alpha)  

[![crypto-cli](https://user-images.githubusercontent.com/14102723/130334492-ee7030ce-e51e-4a91-bf3c-ca4964e5b272.gif)](https://user-images.githubusercontent.com/14102723/130334492-ee7030ce-e51e-4a91-bf3c-ca4964e5b272.gif)

Solution | Project Type | Arguments | Description
---------|--------------|-----------|------------
**GeneratingKeys** | *console* | N/A | Writes out information based on [Generating keys for encryption and decryption](https://docs.microsoft.com/en-us/dotnet/standard/security/generating-keys-for-encryption-and-decryption).
**AsymmetricKeyContainer** | *console* | N/A | Fairly directly follows the example from [Store asymmetric keys in a key container](https://docs.microsoft.com/en-us/dotnet/standard/security/how-to-store-asymmetric-keys-in-a-key-container).
**EncryptAndDecrypt** | *console* | `path` (optional) | Combines and expands the examples in [Encrypting Data](https://docs.microsoft.com/en-us/dotnet/standard/security/encrypting-data) and [Decrypting Data](https://docs.microsoft.com/en-us/dotnet/standard/security/decrypting-data). The Symmetric key is encrypted with an asymmetric RSA key and stored in a symmetrically encrypted file. The encrypted file is then decrypted symmetrically, and the stored key is decrypted asymmetrically.
**Crypto.Core** | *classlib* | N/A | Defines extension methods for the **EncryptFile** and **DecryptFile** solutions.
**EncryptFile** | *console* | `path` | Encrypts the file provided by the `path` parameter.
**DecryptFile** | *console* | `path` | Decrypts the file provided by the `path` parameter.
**crypto-cli** | *console* | [See docs](./crypto-cli) | A comprehensive command line utility that includes the ability to: Register and RSA Key, Remove an RSA Key, Generate an AES key file, Encrypt a file, and Decrypt a file.