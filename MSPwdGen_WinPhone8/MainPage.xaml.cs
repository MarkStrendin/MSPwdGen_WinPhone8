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
using System.Globalization;

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

                // We don't need to delete a key that doesn't exist, so hide the tab
                pvtPages.Items.Remove(tabClean);
            }
            else
            {
                pvtPages.SelectedItem = tabSpecial;
            }

            // Event handlers for clearing data if the app is closed or backgrounded
            PhoneApplicationService.Current.Closing += Current_Closing;
            PhoneApplicationService.Current.Deactivated += Current_Deactivated;            
        }

        #region Application state event handlers

        void Current_Deactivated(object sender, DeactivatedEventArgs e)
        {
            ClearAllTextFields();
        }

        void Current_Closing(object sender, ClosingEventArgs e)
        {
            ClearAllTextFields();
        }

        #endregion

        #region UI helper methods

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

        #endregion

        #region Set master key tab event handlers

        private void btnSetMasterKey_Click(object sender, RoutedEventArgs e)
        {
            string newKeyText = txtNewMasterKey.Text.Trim();

            if (string.IsNullOrEmpty(newKeyText))
            {
                newKeyText = MSPWDCrypto.CreateRandomString();
            }

            MSPWDStorage.SetMasterKey(MSPWDCrypto.CreateMasterKey(txtNewMasterKey.Text.Trim()));
            txtNewMasterKey.Text = string.Empty;

            // Add the "delete master key" page back
            pvtPages.Items.Add(tabClean);
        }

        #endregion

        #region Delete master key tab event handlers
        
        private void btnEraseKey_Click(object sender, RoutedEventArgs e)
        {
            MSPWDStorage.DeleteMasterKey();
        }

        #endregion 

        #region Special Character tab event handlers
        
        private void btnGenerate_Special_Click(object sender, RoutedEventArgs e)
        {
            txtOutput_Special.Text = MSPWDCrypto.CreatePassword_Special(txtInput_Special.Text);
            EnableClipboardButtons_Special();
            txtInput_Special.Text = string.Empty;
        }

        private void btnGenerate_Special_8_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(txtOutput_Special.Text.Substring(0,8));
        }

        private void btnGenerate_Special_12_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(txtOutput_Special.Text.Substring(0, 12));
        }

        private void btnGenerate_Special_15_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(txtOutput_Special.Text.Substring(0, 15));
        }

        private void btnGenerate_Special_20_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(txtOutput_Special.Text.Substring(0, 20));
        }

        #endregion

        #region Alpha only tab event handlers

        private void btnGenerate_Alpha_Click(object sender, RoutedEventArgs e)
        {
            txtOutput_Alpha.Text = MSPWDCrypto.CreatePassword_Alpha(txtInput_Alpha.Text);
            EnableClipboardButtons_Alpha();
            txtInput_Alpha.Text = string.Empty;
        }

        private void btnGenerate_Alpha_8_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(txtOutput_Alpha.Text.Substring(0, 8));
        }

        private void btnGenerate_Alpha_12_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(txtOutput_Alpha.Text.Substring(0, 12));
        }

        private void btnGenerate_Alpha_15_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(txtOutput_Alpha.Text.Substring(0, 15));
        }

        private void btnGenerate_Alpha_20_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(txtOutput_Alpha.Text.Substring(0, 20));
        }

        #endregion

    }
}