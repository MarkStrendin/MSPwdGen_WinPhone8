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
        /*
         * Notes to self regarding storage of the master key
         *  - Store the raw bytes of the master key, rather than converting it to a string, because that screws things up
         *  - Hash whatever the user puts in as a master key with SHA256 (since it returns bytes anyways). 
         *  - We won't be able to display what the user entered as a master key (here, or in the windows version), because it will be a hash. This is OK.
         *  - In windows version, prevent the display of the master key like we do here. You can only overwrite the key with a new one, or a random one.
         * 
         */

        const string KeyFileName = "MarkPasswordGen.blob";

        /// <summary>
        /// Sets the master key
        /// </summary>
        public static void SetMasterKey(byte[] input)
        {
            using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (IsolatedStorageFileStream file = store.OpenFile(KeyFileName, FileMode.OpenOrCreate))
                {
                    file.Write(input, 0, input.Length);                    
                }
            }
        }
       
        public static void DeleteMasterKey()
        {
            using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (store.FileExists(KeyFileName))
                {
                    store.DeleteFile(KeyFileName);
                }                
            }
        }

        public static bool MasterKeyFileExists()
        {
            using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (store.FileExists(KeyFileName))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Retreives the master key from isolated storage
        /// </summary>
        /// <returns></returns>
        public static byte[] GetMasterKey()
        {
            byte[] MasterKey = new byte[0];

            using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (store.FileExists(KeyFileName))
                {
                    // Load the file
                    using (IsolatedStorageFileStream file = store.OpenFile(KeyFileName, FileMode.Open))
                    {
                        MasterKey = new byte[file.Length];
                        file.Read(MasterKey, 0, Convert.ToInt32(file.Length));
                    }
                }
                else
                {
                    // Generate new master key, and save it to a file
                    // The method we generate this does not have to match other platforms, it just has to be random
                    MasterKey = MSPWDCrypto.CreateMasterKey(DateTime.Now.ToString("Fo"));
                    SetMasterKey(MasterKey);
                }
            }
            
            return MasterKey;
        }
    }
}
