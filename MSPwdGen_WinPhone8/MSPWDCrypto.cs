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
        private const string ProtectionEntropyString = "Mark's Password Generator";

        public static string CreatePassword_Alpha(string input)
        {
            // Set up the characters that we will use to generate the password. 
            // It is important that these remain identical on all platforms, so that passwords are consistent
            char[] characterArray_Alpha = {'1','2','3','4','5','6','7','8','9','0','a','b','c','d','e','f',
                                            'g','h','i','j','k','l','m','n','o','p','q','r','s','t','u','v',
                                            'w','x','y','z','A','B','C','D','E','F','G','H','I','J','K','L',
                                            'M','N','O','P','Q','R','S','T','U','V','W','X','Y','Z'};

            // Convert the given string to a byte array so we can work with it
            byte[] inputBytes = Encoding.Unicode.GetBytes(input);

            // Retreive the user's master key
            byte[] MasterKey = MSPWDStorage.GetMasterKey();

            return GenPasswordWithThisHash(characterArray_Alpha, SHA256(CombineByteArrays(inputBytes, MasterKey)));
        }

        public static string CreatePassword_Special(string input)
        {
            // Set up the characters that we will use to generate the password. 
            // It is important that these remain identical on all platforms, so that passwords are consistent
            char[] characterArray_Special = {'1','2','3','4','5','6','7','8','9','0','a','b','c','d','e','f',
                                            'g','h','i','j','k','l','m','n','o','p','q','r','s','t','u','v',
                                            'w','x','y','z','A','B','C','D','E','F','G','H','I','J','K','L',
                                            'M','N','O','P','Q','R','S','T','U','V','W','X','Y','Z','!','@','#',
                                            '$','%','^','*','(',')','_','+','?'};

            // Convert the given string to a byte array so we can work with it
            byte[] inputBytes = Encoding.Unicode.GetBytes(input);

            // Retreive the user's master key
            byte[] MasterKey = MSPWDStorage.GetMasterKey();

            return GenPasswordWithThisHash(characterArray_Special, SHA256(CombineByteArrays(inputBytes, MasterKey)));
        }

        /*
        private static string ConvertByteArrayToString(byte[] convertThis)
        {
            string returnMe = String.Empty;

            foreach (byte x in convertThis)
            {
                returnMe += String.Format("{0:x2}", x);
            }
            return returnMe;
        }
        */

        public static byte[] CreateMasterKey(string input)
        {
            byte[] inputByes = Encoding.Unicode.GetBytes(input);

            // This salt needs to be the same on each platform, so it can't be as random as I had hoped.
            byte[] salt = Encoding.Unicode.GetBytes(@"AG/Fh&QC;7wY0>CPd;gM0*3JBTl0>*pN>DBb-^*sb_+Oa+toLIZS}'/1ne^6Y@6");

            return SHA256(CombineByteArrays(inputByes, salt));
        }

        public static string VisualizeByteArray(byte[] input)
        {
            char[] characterArray_Alpha = {'1','2','3','4','5','6','7','8','9','0','a','b','c','d','e','f',
                                            'g','h','i','j','k','l','m','n','o','p','q','r','s','t','u','v',
                                            'w','x','y','z','A','B','C','D','E','F','G','H','I','J','K','L',
                                            'M','N','O','P','Q','R','S','T','U','V','W','X','Y','Z'};

            return GenPasswordWithThisHash(characterArray_Alpha, input);
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

        private static byte[] CombineByteArrays(byte[] first, byte[] second)
        {
            byte[] ret = new byte[first.Length + second.Length];
            Buffer.BlockCopy(first, 0, ret, 0, first.Length);
            Buffer.BlockCopy(second, 0, ret, first.Length, second.Length);
            return ret;
        }

        private static byte[] SHA256(byte[] hashThis)
        {            
            SHA256Managed sha256 = new SHA256Managed();
            return sha256.ComputeHash(hashThis);
        }

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

        public static byte[] Decrypt(byte[] cyphertext)
        {
            // This is used by the windows phone "Protection" API to differentiate between data protected by different programs. 
            // The protection API is supposed to handle dealing with a unique encryption key, so this doesn't have to be 
            // cryptographically strong, it merely has to seperate data between different programs.
            byte[] ProtectionEntropy = Encoding.UTF8.GetBytes(ProtectionEntropyString);

            byte[] DecryptedBytes = ProtectedData.Unprotect(cyphertext, ProtectionEntropy);
            
            return DecryptedBytes;
        }
        
    }    
}
