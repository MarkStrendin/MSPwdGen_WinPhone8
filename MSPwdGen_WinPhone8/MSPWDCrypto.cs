using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MSPwdGen_WinPhone8
{
    public static class MSPWDCrypto
    {
        public static string CreatePassword_Alpha(string input, string salt)
        {
            char[] characterArray_Alpha = {'1','2','3','4','5','6','7','8','9','0','a','b','c','d','e','f',
                                            'g','h','i','j','k','l','m','n','o','p','q','r','s','t','u','v',
                                            'w','x','y','z','A','B','C','D','E','F','G','H','I','J','K','L',
                                            'M','N','O','P','Q','R','S','T','U','V','W','X','Y','Z'};

            return GenPasswordWithThisHash(characterArray_Alpha, HashThis_SHA512(input + salt));
        }

        public static string CreatePassword_Special(string input, string salt)
        {
            char[] characterArray_Special = {'1','2','3','4','5','6','7','8','9','0','a','b','c','d','e','f',
                                            'g','h','i','j','k','l','m','n','o','p','q','r','s','t','u','v',
                                            'w','x','y','z','A','B','C','D','E','F','G','H','I','J','K','L',
                                            'M','N','O','P','Q','R','S','T','U','V','W','X','Y','Z','!','@','#',
                                            '$','%','^','*','(',')','_','+','?'};

            return GenPasswordWithThisHash(characterArray_Special, HashThis_SHA512(input + salt));
        }

        public static string Encrypt(string plainText, string sharedSecret)
        {
            byte[] clearBytes = System.Text.Encoding.Unicode.GetBytes(plainText);
            PasswordDeriveBytes pdb = new PasswordDeriveBytes(sharedSecret,
                new byte[] {0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 
            0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76});

            byte[] encryptedData = Encrypt(clearBytes,
                     pdb.GetBytes(32), pdb.GetBytes(16));

            return Convert.ToBase64String(encryptedData);
        }

        public static string Decrypt(string cipherText, string sharedSecret)
        {
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            PasswordDeriveBytes pdb = new PasswordDeriveBytes(sharedSecret, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });

            byte[] decryptedData = Decrypt(cipherBytes, pdb.GetBytes(32), pdb.GetBytes(16));

            return System.Text.Encoding.Unicode.GetString(decryptedData);
        }

        public static string ConvertByteArrayToString(byte[] convertThis)
        {
            string returnMe = String.Empty;

            foreach (byte x in convertThis)
            {
                returnMe += String.Format("{0:x2}", x);
            }
            return returnMe;
        }

        public static byte[] HashThis(string hashThis)
        {
            return HashThis_SHA512(hashThis);
        }


        private static string GenPasswordWithThisHash(char[] characterSet, byte[] input)
        {
            string returnMe = String.Empty;
            foreach (byte thisByte in input)
            {
                int thisInt = (int)thisByte;
                while (thisInt > characterSet.Length - 1)
                {
                    thisInt -= characterSet.Length;
                }
                returnMe += characterSet[thisInt];
            }
            return returnMe;
        }

        private static byte[] HashThis_SHA512(string hashThis)
        {
            byte[] hashValue;
            byte[] message = Encoding.ASCII.GetBytes(hashThis);

            SHA512Managed hashString = new SHA512Managed();
            hashValue = hashString.ComputeHash(message);

            return hashValue;
        }

        private static byte[] HashThis_SHA256(string hashThis)
        {
            UnicodeEncoding UE = new UnicodeEncoding();

            byte[] hashValue;
            byte[] message = Encoding.ASCII.GetBytes(hashThis);

            SHA256Managed hashString = new SHA256Managed();

            hashValue = hashString.ComputeHash(message);
            return hashValue;
        }
        
        private static byte[] Encrypt(byte[] clearData, byte[] Key, byte[] IV)
        {
            MemoryStream ms = new MemoryStream();
            Rijndael alg = Rijndael.Create();
            alg.Padding = PaddingMode.Zeros;
            alg.Key = Key;
            alg.IV = IV;

            CryptoStream cs = new CryptoStream(ms, alg.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(clearData, 0, clearData.Length);
            cs.Close();

            byte[] encryptedData = ms.ToArray();
            return encryptedData;
        }

        private static byte[] Decrypt(byte[] cipherData, byte[] Key, byte[] IV)
        {
            MemoryStream ms = new MemoryStream();

            Rijndael alg = Rijndael.Create();
            alg.Padding = PaddingMode.Zeros;
            alg.Key = Key;
            alg.IV = IV;

            CryptoStream cs = new CryptoStream(ms,
                alg.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(cipherData, 0, cipherData.Length);
            cs.Close();
            byte[] decryptedData = ms.ToArray();
            return decryptedData;
        }

    }    
}
