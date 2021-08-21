# crypto-cli

Cryptography CLI

**Usage**:  

```cmd
crypto-cli [options] [command]
```

**Options**:  

Option | Description
-------|------------
`--version` | Show version information
`-?, -h, --help` | Show help and usage information

**Commands**:  

Command | Description
--------|------------
`register-rsa` | Register an RSA key inside of a Key Container
`remove-rsa` | Remove an RSA key inside of a Key Container
`generate-aes` | Generate an AES key file with a `.crypt` extension for use with this CLI app
`encrypt` | Encrypt a file using a `.crypt` AES key file
`decrypt` | Decrypt a file using a `.crypt` AES key file

## `register-rsa`

Register an RSA key inside of a Key Container

**Options**:  

Option | Description | Default
-------|-------------|--------
`-k, --key <key>` | The name of the Key Container | *CryptoCliRsa*
`-m, --machine-store` | Register the Key Container in the Machine Key Store | *false*
`-?, -h, --help` | Show help and usage information | 

## `remove-rsa`

Remove an RSA key inside of a Key Container

**Options**:

Option | Description | Default
-------|-------------|--------
`-k, --key <key>` | The name of the Key Container | *CryptoCliRsa*
`-?, -h, --help` | Show help and usage information | 

## `generate-aes`

Generate an AES key file with a `.crypt` extension for use with this CLI app

**Options**:

Option | Description | Default
-------|-------------|--------
`-p, --path <path>` | The path to the file to be generated (including the file name) | *.\aes.crypt*
`-k, --key <key>` | The name of the Key Container for AES key encryption | *CryptoCliRsa*
`-?, -h, --help` | Show help and usage information | 

## `encrypt`

Encrypt a file using a `.crypt` AES key file

**Options**:

Option | Required | Description | Default
-------|----------|-------------|--------
`-f, --file <file>` | Yes | The path to the file to be encrypted (including the file name) | `string.Empty`
`-r, --result <result>` | Yes | The path to the resulting encrypted file (including the file name) | `string.Empty`
`-a, --aes <aes>` | No | The path to the `.crypt` AES key file | *.\aes.crypt*
`-k, --key <key>` | No | The name of the Key Container used to encrypt the `.crypt` AES key file | *CryptoCliRsa*
`-?, -h, --help` | Show help and usage information | 

## `decrypt`

Decrypt a file using a `.crypt` AES key file

**Options**:

Option | Required | Description | Default
-------|----------|-------------|--------
`-f, --file <file>` | Yes | The path to the file to be decrypted (including the file name) | `string.Empty`
`-r, --result <result>` | Yes | The path to the resulting decrypted file (including the file name) | `string.Empty`
`-a, --aes <aes>` | No | The path to the `.crypt` AES key file | *.\aes.crypt*
`-k, --key <key>` | No | The name of the Key Container used to encrypt the `.crypt` AES key file | *CryptoCliRsa*
`-?, -h, --help` | Show help and usage information | 
