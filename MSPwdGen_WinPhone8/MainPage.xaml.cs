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
            pvtPages.SelectedItem = tabKey;
            
            //try
            {
                
            }/*
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            //*/
            // Load the key, or generate a new one

        }

        private void btnGenerate_Special_Click(object sender, RoutedEventArgs e)
        {
            txtOutput_Special.Text = ":" + MSPWDStorage.GetMasterKey();
        }



    }
}