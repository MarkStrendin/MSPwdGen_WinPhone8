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
            if (MSPWDStorage.MasterKeyExists())
            {
            }

            pvtPages.SelectedItem = tabKey;

            // Load the key, or generate a new one

        }

    }
}