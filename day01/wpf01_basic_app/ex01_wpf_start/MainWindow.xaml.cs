﻿using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ex01_wpf_start
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

     

        private void BtnClick_Click_(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Hello, WPF!!!", "알림", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}