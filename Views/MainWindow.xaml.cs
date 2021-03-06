﻿using System.Windows;
using Prism.Regions;

namespace AddonsDuck2.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(RegionManager regionManager)
        {
            InitializeComponent();
            regionManager.RegisterViewWithRegion("CategoriesRegion", typeof(CategoriesView));
            regionManager.RegisterViewWithRegion("AddonsRegion", typeof(AddonsView));
            regionManager.RegisterViewWithRegion("DownloadListRegion", typeof(DownloadView));
        }
    }
}
