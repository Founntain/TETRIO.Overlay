using System.Security.Cryptography;
using System.Text;

namespace Tetrio.Overlay.Database;

public class EncryptionService
{
    private readonly string _encryptionKey;

    public EncryptionService()
    {
        if (File.Exists("/run/secrets/encryption-key"))
        {
            _encryptionKey = File.ReadAllText("/run/secrets/encryption-key");

            Console.WriteLine("loaded encryption key from secrets");

            return;
        }

        var encryptionKey = Environment.GetEnvironmentVariable("encryption-key");

        if (string.IsNullOrEmpty(encryptionKey))
        {
            throw new ArgumentException("encryption-key environment variable is not set.");
        }

        _encryptionKey = encryptionKey;

        Console.WriteLine("loaded encryption key from environment variable");
    }

    public string EncryptWithIv(string plaintext)
    {
        using var aes = Aes.Create();

        var key = Convert.FromBase64String(_encryptionKey);

        aes.Key = key;
        aes.GenerateIV();
        var iv = aes.IV;

        using var encryptor = aes.CreateEncryptor(aes.Key, iv);

        var inputBytes = Encoding.UTF8.GetBytes(plaintext);
        var encrypted = encryptor.TransformFinalBlock(inputBytes, 0, inputBytes.Length);

        // Return the IV and encrypted data combined (example)
        var result = new byte[iv.Length + encrypted.Length];
        iv.CopyTo(result, 0);
        encrypted.CopyTo(result, iv.Length);
        return Convert.ToBase64String(result);
    }

    public string DecryptWithIv(string encryptedData)
    {
        var fullCipher = Convert.FromBase64String(encryptedData);
        var iv = new byte[16];
        var cipherText = new byte[fullCipher.Length - iv.Length];

        Array.Copy(fullCipher, iv, iv.Length);
        Array.Copy(fullCipher, iv.Length, cipherText, 0, cipherText.Length);

        using var aes = Aes.Create();

        aes.Key = Convert.FromBase64String(_encryptionKey);
        aes.IV = iv;

        using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

        var decrypted = decryptor.TransformFinalBlock(cipherText, 0, cipherText.Length);
        return Encoding.UTF8.GetString(decrypted);
    }

}