﻿using System;
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
                pvtPages.Items.Remove(tabAlpha);
                pvtPages.Items.Remove(tabSpecial);

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
            btnGenerate_Special_32.IsEnabled = true;
        }

        private void EnableClipboardButtons_Alpha()
        {
            btnGenerate_Alpha_8.IsEnabled = true;
            btnGenerate_Alpha_12.IsEnabled = true;
            btnGenerate_Alpha_15.IsEnabled = true;
            btnGenerate_Alpha_20.IsEnabled = true;
            btnGenerate_Alpha_32.IsEnabled = true;
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

            if ((string.IsNullOrEmpty(newKeyText)) || (newKeyText.Length < 3))
            {
                MessageBox.Show("ERROR: Master key not long enough. Key must be longer than 3 characters");
            }
            else
            {
                MSPWDStorage.SetMasterKeyFile(MSPWDCrypto.CreateMasterKey(txtNewMasterKey.Text.Trim()));
                txtNewMasterKey.Text = string.Empty;

                // Add the "delete master key" page back
                if (!pvtPages.Items.Contains(tabSpecial))
                    pvtPages.Items.Add(tabSpecial);
                if (!pvtPages.Items.Contains(tabAlpha))
                    pvtPages.Items.Add(tabAlpha);
                if (!pvtPages.Items.Contains(tabClean))
                    pvtPages.Items.Add(tabClean);

                pvtPages.SelectedItem = tabSpecial;
            }
        }

        #endregion

        #region Delete master key tab event handlers
        
        private void btnEraseKey_Click(object sender, RoutedEventArgs e)
        {
            MSPWDStorage.DeleteMasterKey();
            pvtPages.SelectedItem = tabKey;
            pvtPages.Items.Remove(tabSpecial);
            pvtPages.Items.Remove(tabAlpha);
            pvtPages.Items.Remove(tabClean);

            txtCleanDescription.Visibility = System.Windows.Visibility.Visible;
            btnEraseKeyPrompt.Visibility = System.Windows.Visibility.Visible;

            txtCleanSure.Visibility = System.Windows.Visibility.Collapsed;
            btnEraseKey.Visibility = System.Windows.Visibility.Collapsed;
            btnDoNotErase.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void btnEraseKeyPrompt_Click(object sender, RoutedEventArgs e)
        {
            txtCleanDescription.Visibility = System.Windows.Visibility.Collapsed;
            btnEraseKeyPrompt.Visibility = System.Windows.Visibility.Collapsed;

            txtCleanSure.Visibility = System.Windows.Visibility.Visible;
            btnEraseKey.Visibility = System.Windows.Visibility.Visible;
            btnDoNotErase.Visibility = System.Windows.Visibility.Visible;

        }

        private void btnDoNotErase_Click(object sender, RoutedEventArgs e)
        {
            txtCleanDescription.Visibility = System.Windows.Visibility.Visible;
            btnEraseKeyPrompt.Visibility = System.Windows.Visibility.Visible;

            txtCleanSure.Visibility = System.Windows.Visibility.Collapsed;
            btnEraseKey.Visibility = System.Windows.Visibility.Collapsed;
            btnDoNotErase.Visibility = System.Windows.Visibility.Collapsed;
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

        private void btnGenerate_Special_32_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(txtOutput_Special.Text.Substring(0, 32));
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

        private void btnGenerate_Alpha_32_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(txtOutput_Alpha.Text.Substring(0, 32));
        }

        #endregion 
        
    }
}