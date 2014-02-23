using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSPwdGen_WinPhone8
{
    public class MSPWDStorage
    {
        const string KeyFileName = "MarkPasswordGen.blob";

        /// <summary>
        /// Sets the master key
        /// </summary>
        /// <param name="input"></param>
        public static void SetMasterKey(string input)
        {
            using (IsolatedStorageFile isoStore = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null))
            {
                using (IsolatedStorageFileStream oStream = new IsolatedStorageFileStream(KeyFileName, FileMode.Create, isoStore))
                {
                    using (StreamWriter writer = new StreamWriter(oStream))
                    {
                        if ((input.Length > 0))
                        {
                            string sharedSecret = MSPWDCrypto.ConvertByteArrayToString(MSPWDCrypto.HashThis(System.Environment.MachineName.ToString()));
                            writer.Write(MSPWDCrypto.Encrypt(input, sharedSecret));
                        }
                        else
                        {
                            writer.Write(MSPWDCrypto.ConvertByteArrayToString(MSPWDCrypto.HashThis(DateTime.Now.ToString())));
                        }
                        writer.Close();
                    }
                }
            }
        }

        /// <summary>
        /// Checks if the master key exists
        /// </summary>
        /// <returns></returns>
        public static bool MasterKeyExists()
        {
            bool returnMe = false;
            using (IsolatedStorageFile isoStore = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null))
            {
                string[] fileNames = isoStore.GetFileNames(KeyFileName);
                
                foreach (string file in fileNames)
                {
                    if (file == KeyFileName)
                    {
                        returnMe = true;
                    }
                }
            }
            return returnMe;
        }

        /// <summary>
        /// Retreives the master key from isolated storage
        /// </summary>
        /// <returns></returns>
        public static string GetMasterKey()
        {
            string returnMe = "";
            using (IsolatedStorageFile isoStore = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null))
            {
                string[] fileNames = isoStore.GetFileNames(KeyFileName);

                Boolean foundFile = false;

                foreach (string file in fileNames)
                {
                    if (file == KeyFileName)
                    {
                        foundFile = true;
                        //The file exists
                    }
                }

                if (foundFile == false)
                {
                    GenerateNewKeyFile();
                    return GetMasterKey();
                }
                else
                {
                    using (IsolatedStorageFileStream iStream = new IsolatedStorageFileStream(KeyFileName, System.IO.FileMode.Open, isoStore))
                    {
                        StreamReader reader = new StreamReader(iStream);
                        String line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            returnMe = line;
                        }
                    }

                }
            }

            string sharedSecret = MSPWDCrypto.ConvertByteArrayToString(MSPWDCrypto.HashThis(System.Environment.MachineName.ToString()));
            return MSPWDCrypto.Decrypt(returnMe, sharedSecret);
            //return returnMe;
        }

        /// <summary>
        /// Generates a new salt file, if one doesn't already exist
        /// </summary>
        public static void GenerateNewKeyFile()
        {
            using (IsolatedStorageFile isoStore = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null))
            {
                using (IsolatedStorageFileStream oStream = new IsolatedStorageFileStream(KeyFileName, FileMode.Create, isoStore))
                {
                    using (StreamWriter writer = new StreamWriter(oStream))
                    {
                        string timeString = DateTime.Now.ToString();
                        string newSalt = MSPWDCrypto.ConvertByteArrayToString(MSPWDCrypto.HashThis(timeString));
                        string sharedSecret = MSPWDCrypto.ConvertByteArrayToString(MSPWDCrypto.HashThis(System.Environment.MachineName.ToString()));
                        writer.Write(MSPWDCrypto.Encrypt(newSalt, sharedSecret));
                        writer.Close();
                    }
                }
            }


        }
    }
}
