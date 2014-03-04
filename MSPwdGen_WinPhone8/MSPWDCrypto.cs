using Microsoft.Phone.Info;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MSPwdGen_WinPhone8
{
    public static class MSPWDCrypto
    {
        // Set up the characters that we will use to generate the passwords. 
        // It is important that these remain identical on all platforms, so that passwords are consistent
        private static char[] characterArray_Alpha = {'1','2','3','4','5','6','7','8','9','0','a','b','c','d','e','f',
                                            'g','h','i','j','k','l','m','n','o','p','q','r','s','t','u','v',
                                            'w','x','y','z','A','B','C','D','E','F','G','H','I','J','K','L',
                                            'M','N','O','P','Q','R','S','T','U','V','W','X','Y','Z'};
        
        private static char[] characterArray_Special = {'1','2','3','4','5','6','7','8','9','0','a','b','c','d','e','f',
                                            'g','h','i','j','k','l','m','n','o','p','q','r','s','t','u','v',
                                            'w','x','y','z','A','B','C','D','E','F','G','H','I','J','K','L',
                                            'M','N','O','P','Q','R','S','T','U','V','W','X','Y','Z','!','@','#',
                                            '$','%','^','*','(',')','_','+','?'};

        // This string is used as part of the Windows Phone Protection methods to encrypt the master key file.
        private const string ProtectionEntropyString = @"K\=c>_m2T;ExJ;I>.Xk$v':=*x@|8Nqj7N).G$FnQ|*2Ge}0?V7^36Xw'*!27eH";

        /// <summary>
        /// Creates an alphanumeric only password using the specified string as a seed
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string CreatePassword_Alpha(string input)
        {
            // Convert the given string to a byte array so we can work with it
            byte[] inputBytes = Encoding.Unicode.GetBytes(input);

            // Retreive the user's master key
            byte[] MasterKey = MSPWDStorage.GetMasterKey();

            return GenPasswordWithThisHash(characterArray_Alpha, SHA256(CombineByteArrays(inputBytes, MasterKey)));
        }

        /// <summary>
        /// Creates a password with special characters, using the specified string as a seed
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string CreatePassword_Special(string input)
        {
            // Convert the given string to a byte array so we can work with it
            byte[] inputBytes = Encoding.Unicode.GetBytes(input);

            // Retreive the user's master key
            byte[] MasterKey = MSPWDStorage.GetMasterKey();

            return GenPasswordWithThisHash(characterArray_Special, SHA256(CombineByteArrays(inputBytes, MasterKey)));
        }
               
        /// <summary>
        /// Creates a master key given the specified string as a seed.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static byte[] CreateMasterKey(string input)
        {
            byte[] inputByes = Encoding.Unicode.GetBytes(input);

            // This salt needs to be the same on each platform, so it can't be as random as I had hoped.
            // At the very least, this prevents the hash from being a straight SHA256 hash of whatever the user enters as a key.
            // Of course anyone looking at this code would see the hash, but any existing rainbow tables would be useless and they would have to compute new ones.
            byte[] salt = Encoding.Unicode.GetBytes(@"AG/Fh&QC;7wY0>CPd;gM0*3JBTl0>*pN>DBb-^*sb_+Oa+toLIZS}'/1ne^6Y@6");

            return SHA256(CombineByteArrays(inputByes, salt));
        }
        
        /// <summary>
        /// Encrypt the given bytes so that we can store them on the device
        /// </summary>
        /// <param name="plaintext"></param>
        /// <returns></returns>
        public static byte[] Encrypt(byte[] plaintext)
        {
            // This is used by the windows phone "Protection" API to differentiate between data protected by different programs. 
            // The protection API is supposed to handle dealing with a unique encryption key, so this doesn't have to be 
            // cryptographically strong, it merely has to seperate data between different programs.
            byte[] ProtectionEntropy = Encoding.UTF8.GetBytes(ProtectionEntropyString);

            // Encrypt the byte array
            byte[] EncryptedBytes = ProtectedData.Protect(plaintext, ProtectionEntropy);

            return EncryptedBytes;
        }

        /// <summary>
        /// Decrypt the given bytes - used to retreive the stored master key from the device
        /// </summary>
        /// <param name="cyphertext"></param>
        /// <returns></returns>
        public static byte[] Decrypt(byte[] cyphertext)
        {
            // Provide a bit more entropy for the DPAPI
            byte[] ProtectionEntropy = Encoding.UTF8.GetBytes(ProtectionEntropyString);

            byte[] DecryptedBytes = ProtectedData.Unprotect(cyphertext, ProtectionEntropy);

            return DecryptedBytes;
        }


        /// <summary>
        /// Creates a random string to use as a master key. I have tried to move away from automatically setting the key to something, but if the user
        /// finds a way to bypass the master key input screen, we need to do have something available. This effectively shoots the user in the foot, 
        /// but the app won't crash as soon as the generate button is pressed.
        /// </summary>
        /// <returns></returns>
        public static string CreateRandomString()
        {
            Random random = new Random();
            return GenPasswordWithThisHash(characterArray_Special, SHA256(Encoding.Unicode.GetBytes(DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture) + DeviceStatus.DeviceName + random.Next(0, 9999).ToString())));
        }
        
        /// <summary>
        /// Takes an array of bytes and translates those bytes into characters from an array of available characters.
        /// </summary>
        /// <param name="characterSet"></param>
        /// <param name="input"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Concatenate two byte arrays - used to combine input with salt for hashing
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        private static byte[] CombineByteArrays(byte[] first, byte[] second)
        {
            byte[] ret = new byte[first.Length + second.Length];
            Buffer.BlockCopy(first, 0, ret, 0, first.Length);
            Buffer.BlockCopy(second, 0, ret, first.Length, second.Length);
            return ret;
        }

        /// <summary>
        /// Get an SHA256 hash of the specified byte array
        /// </summary>
        /// <param name="hashThis"></param>
        /// <returns></returns>
        private static byte[] SHA256(byte[] hashThis)
        {            
            SHA256Managed sha256 = new SHA256Managed();
            return sha256.ComputeHash(hashThis);
        }

        
    }    
}
