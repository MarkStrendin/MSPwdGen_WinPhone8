using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using MSPwdGen_WinPhone8.Resources;

namespace MSPwdGen_WinPhone8
{
    public partial class MainPage : PhoneApplicationPage
    {
        public MainPage()
        {
            InitializeComponent();

            // If this is the first run of the app, show the master key page
            if (!MSPWDStorage.MasterKeyFileExists())
            {
                pvtPages.SelectedItem = tabKey;
            }
            else
            {
                pvtPages.SelectedItem = tabSpecial;
            }
            

        }

        private void EnableClipboardButtons_Special()
        {
            btnGenerate_Special_8.IsEnabled = true;
            btnGenerate_Special_12.IsEnabled = true;
            btnGenerate_Special_15.IsEnabled = true;
            btnGenerate_Special_20.IsEnabled = true;
        }

        private void EnableClipboardButtons_Alpha()
        {
            btnGenerate_Alpha_8.IsEnabled = true;
            btnGenerate_Alpha_12.IsEnabled = true;
            btnGenerate_Alpha_15.IsEnabled = true;
            btnGenerate_Alpha_20.IsEnabled = true;
        }

        private void ClearAllTextFields()
        {
            txtInput_Special.Text = string.Empty;
            txtInput_Alpha.Text = string.Empty;
            txtOutput_Alpha.Text = string.Empty;
            txtOutput_Special.Text = string.Empty;
        }

        private void btnGenerate_Special_Click(object sender, RoutedEventArgs e)
        {
            txtOutput_Special.Text = MSPWDCrypto.CreatePassword_Special(txtInput_Special.Text);
            EnableClipboardButtons_Special();
            txtInput_Special.Text = string.Empty;
        }

        private void btnEraseKey_Click(object sender, RoutedEventArgs e)
        {
            MSPWDStorage.DeleteMasterKey();
        }

        private void btnSetMasterKey_Click(object sender, RoutedEventArgs e)
        {
            MSPWDStorage.SetMasterKey(MSPWDCrypto.CreateMasterKey(txtNewMasterKey.Text.Trim()));
            txtNewMasterKey.Text = string.Empty;
            
        }



    }
}