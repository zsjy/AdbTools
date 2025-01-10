using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AdbTools.CustomControl
{
    /// <summary>
    /// RoundProcessBarLoading.xaml 的交互逻辑
    /// </summary>
    public partial class RoundProcessBarLoading : UserControl
    {
        public RoundProcessBarLoading()
        {
            InitializeComponent();
        }

        [Description("获取或设置是否显示动画")]
        public bool IsShowAnimation
        {
            set
            {
                if (value)
                {
                    this.defaultGrid.Visibility = Visibility.Collapsed;
                    this.animationGrid.Visibility = Visibility.Visible;
                }
                else
                {
                    this.defaultGrid.Visibility = Visibility.Visible;
                    this.animationGrid.Visibility = Visibility.Collapsed;
                }
            }
            get
            {
                return Visibility.Visible == this.animationGrid.Visibility;
            }
        }
    }
}
