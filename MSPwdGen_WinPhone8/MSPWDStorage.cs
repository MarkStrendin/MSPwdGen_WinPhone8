﻿using System;
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
        /// <summary>
        /// The file name to use for the master key file in isolated storage.
        /// </summary>
        const string KeyFileName = "MSPWDKey.blob";

        /// <summary>
        /// Sets the master key file to the specified value.
        /// </summary>
        /// <param name="input"></param>
        public static void SetMasterKeyFile(byte[] input)
        {
            using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (IsolatedStorageFileStream file = store.OpenFile(KeyFileName, FileMode.OpenOrCreate))
                {
                    file.Write(input, 0, input.Length);                    
                }
            }
        }
       
        /// <summary>
        /// Deletes the master key file from storage
        /// </summary>
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

        /// <summary>
        /// Returns true if the master key file exists already. This is used to indicate if the app has ever been run before.
        /// </summary>
        /// <returns></returns>
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
        /// Retreives the master key from isolated storage. If a key does not exist, it will create a new one.
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
                    MasterKey = MSPWDCrypto.CreateMasterKey(MSPWDCrypto.CreateRandomString());
                    SetMasterKeyFile(MasterKey);
                }
            }
            
            return MasterKey;
        }
    }
}
