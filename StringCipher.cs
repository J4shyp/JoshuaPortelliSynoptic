using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Crypto.Parameters;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

public static class CipherHelper
{
    //encryption key size in bits
    private const int Keysize = 256;

    //iterations for password
    private const int DerivationIterations = 1000;

    public static string Encrypt(string plainText, string passPhrase)
    {
        //encrypted cipher text to be used when decrypting
        var saltStringBytes = Generate256BitsOfRandomEntropy();
        var ivStringBytes = Generate256BitsOfRandomEntropy();
        var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
        using (var password = new Rfc2898DeriveBytes(passPhrase, saltStringBytes, DerivationIterations))
        {
            var keyBytes = password.GetBytes(Keysize / 8);
            var engine = new RijndaelEngine(256);
            var blockCipher = new CbcBlockCipher(engine);
            var cipher = new PaddedBufferedBlockCipher(blockCipher, new Pkcs7Padding());
            var keyParam = new KeyParameter(keyBytes);
            var keyParamWithIV = new ParametersWithIV(keyParam, ivStringBytes, 0, 32);

            cipher.Init(true, keyParamWithIV);
            var comparisonBytes = new byte[cipher.GetOutputSize(plainTextBytes.Length)];
            var length = cipher.ProcessBytes(plainTextBytes, comparisonBytes, 0);

            cipher.DoFinal(comparisonBytes, length);
            return Convert.ToBase64String(saltStringBytes.Concat(ivStringBytes).Concat(comparisonBytes).ToArray());
        }
    }

    public static string Decrypt(string cipherText, string passPhrase)
    {
        //get bytes
        var cipherTextBytesWithSaltAndIv = Convert.FromBase64String(cipherText);
        //get first 32 bytes from cipherText bytes
        var saltStringBytes = cipherTextBytesWithSaltAndIv.Take(Keysize / 8).ToArray();
        //get next 32 bytes from cipherText bytes
        var ivStringBytes = cipherTextBytesWithSaltAndIv.Skip(Keysize / 8).Take(Keysize / 8).ToArray();
        // get actual cipher bytes
        var cipherTextBytes = cipherTextBytesWithSaltAndIv.Skip((Keysize / 8) * 2).Take(cipherTextBytesWithSaltAndIv.Length - ((Keysize / 8) * 2)).ToArray();

        using (var password = new Rfc2898DeriveBytes(passPhrase, saltStringBytes, DerivationIterations))
        {
            var keyBytes = password.GetBytes(Keysize / 8);
            var engine = new RijndaelEngine(256);
            var blockCipher = new CbcBlockCipher(engine);
            var cipher = new PaddedBufferedBlockCipher(blockCipher, new Pkcs7Padding());
            var keyParam = new KeyParameter(keyBytes);
            var keyParamWithIV = new ParametersWithIV(keyParam, ivStringBytes, 0, 32);

            cipher.Init(false, keyParamWithIV);
            var comparisonBytes = new byte[cipher.GetOutputSize(cipherTextBytes.Length)];
            var length = cipher.ProcessBytes(cipherTextBytes, comparisonBytes, 0);

            try
            {
                cipher.DoFinal(comparisonBytes, length);
            }
            catch(InvalidCipherTextException e)
            {
                System.Diagnostics.Debug.Print("error");
            }

            try
            {
                var nullIndex = comparisonBytes.Length - 1;
                while (comparisonBytes[nullIndex] == (byte)0)
                    nullIndex--;
                comparisonBytes = comparisonBytes.Take(nullIndex + 1).ToArray();

                var result = Encoding.UTF8.GetString(comparisonBytes, 0, comparisonBytes.Length);

                return result;
            }
            catch(System.IndexOutOfRangeException e)
            {
                System.Diagnostics.Debug.Print("error");
                return null; 
            }
        }
    }

    private static byte[] Generate256BitsOfRandomEntropy()
    {
        var randomBytes = new byte[32];
        using (var rngCsp = new RNGCryptoServiceProvider())
        {
            rngCsp.GetBytes(randomBytes);
        }
        return randomBytes;
    }
}