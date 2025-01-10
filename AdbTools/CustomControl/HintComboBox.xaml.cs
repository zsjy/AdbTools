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
    /// HintComboBox.xaml 的交互逻辑
    /// </summary>
    public partial class HintComboBox : UserControl
    {
        public HintComboBox()
        {
            InitializeComponent();
        }



        //[Description("获取或设置图片")]
        //public System.Windows.Media.ImageSource ImageSource
        //{
        //    set
        //    {
        //        this.image.Source = value;
        //    }
        //    get
        //    {
        //        return this.image.Source;
        //    }
        //}

        [Description("设置或获取提示信息")]
        public object Hint
        {
            get
            {
                return this.hint.Content;
            }
            set
            {
                this.hint.Content = value;
            }
        }


        [Description("设置或获取提示信息的颜色")]
        public System.Windows.Media.Brush HintForeground
        {
            get
            {
                return this.hint.Foreground;
            }
            set
            {
                this.hint.Foreground = value;
            }
        }

        [Description("设置或获取提示信息的字体大小")]
        public double HintFontSize
        {
            get
            {
                return this.hint.FontSize;
            }
            set
            {
                this.hint.FontSize = value;
            }
        }


        [Description("设置或获取输入框的字体颜色")]
        public System.Windows.Media.Brush TextForeground
        {
            get
            {
                return this.comboBox.Foreground;
            }
            set
            {
                this.comboBox.Foreground = value;
            }
        }

        [Description("设置或获取输入框的字体大小")]
        public double TextFontSize
        {
            get
            {
                return this.comboBox.FontSize;
            }
            set
            {
                this.comboBox.FontSize = value;
            }
        }


        [Description("设置或获取下划线的颜色")]
        public System.Windows.Media.Brush LineStroke
        {
            get
            {
                return this.line.Stroke;
            }
            set
            {
                this.line.Stroke = value;
            }
        }


        [Description("设置或获取输入框的文本")]
        public string Text
        {
            get
            {
                return this.comboBox.Text;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    this.hint.Visibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    this.hint.Visibility = System.Windows.Visibility.Collapsed;
                }
                this.comboBox.Text = value;
                this.UpdateLayout();
            }
        }

        [Description("设置或获取控件是否只读")]
        public bool IsReadOnly
        {
            get
            {
                return this.comboBox.IsReadOnly;
            }
            set
            {
                this.comboBox.IsReadOnly = value;
            }
        }

        [Description("获取控件的所有项")]
        public System.Windows.Controls.ItemCollection Items
        {
            get
            {
                return this.comboBox.Items;
            }
        }

        [Description("设置或获取控件的内边距")]
        public System.Windows.Thickness ComboBoxPadding
        {
            get
            {
                Thickness t = this.comboBox.Padding;
                return new Thickness(t.Left - 10, t.Top, t.Right, t.Bottom);
            }
            set
            {
                this.comboBox.Padding = new Thickness(10 + value.Left, value.Top, value.Right, value.Bottom);
                this.hint.Margin = new Thickness(10 + value.Left, 0, 0, 0);
            }
        }

        [Description("设置或获取控件的下拉条高度")]
        public double MaxDropDownHeight
        {
            get
            {
                return this.comboBox.MaxDropDownHeight;
            }
            set
            {
                this.comboBox.MaxDropDownHeight = value;
            }
        }

        [Description("下拉改变事件，可以获取到选择的项")]
        public event EventHandler<SelectionChangedEventArgs> SelectionChanged;

        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.IsLoad)
            {
                if (e.AddedItems.Count > 0)
                {
                    this.hint.Visibility = System.Windows.Visibility.Collapsed;
                }
                else
                {
                    this.hint.Visibility = System.Windows.Visibility.Visible;
                }

                if (SelectionChanged != null)
                {
                    SelectionChanged(sender, e);
                }
            }
        }

        private bool IsLoad;
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.IsLoad = true;
            this.comboBox.Height = this.ActualHeight;
            this.comboBox.Width = this.ActualWidth;

            if (string.IsNullOrEmpty(Text))
            {
                this.hint.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                this.hint.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private void comboBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (this.IsLoad)
            {
                if (string.IsNullOrEmpty(this.Text))
                {
                    this.hint.Visibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    this.hint.Visibility = System.Windows.Visibility.Collapsed;
                }
            }
        }

        private void comboBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (this.IsLoad)
            {
                if (string.IsNullOrEmpty(this.Text))
                {
                    this.hint.Visibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    this.hint.Visibility = System.Windows.Visibility.Collapsed;
                }
            }
        }




    }
}
