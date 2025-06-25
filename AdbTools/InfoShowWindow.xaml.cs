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
using System.Windows.Shapes;

namespace AdbTools
{
    /// <summary>
    /// InfoShowWindow.xaml 的交互逻辑
    /// </summary>
    public partial class InfoShowWindow : Window
    {
        public InfoShowWindow()
        {
            InitializeComponent();
        }

        public string DescriptionContent { get; set; }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.message.Text = DescriptionContent;
        }
    }
}
