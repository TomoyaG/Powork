﻿using Powork.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Powork.View
{
    /// <summary>
    /// SettingsPage.xaml 的交互逻辑
    /// </summary>
    public partial class SharePage : Page
    {
        public SharePage()
        {
            InitializeComponent();
            this.DataContext = new SharePageViewModel(GlobalVariables.SelfInfo.Count == 0 ? null : GlobalVariables.SelfInfo[0]);
        }
    }
}
