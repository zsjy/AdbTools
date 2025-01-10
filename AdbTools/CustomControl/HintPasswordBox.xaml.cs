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
    /// HintPasswordBox.xaml 的交互逻辑
    /// </summary>
    public partial class HintPasswordBox : UserControl
    {
        public HintPasswordBox()
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
                return this.passwordBox.Foreground;
            }
            set
            {
                this.passwordBox.Foreground = value;
            }
        }

        [Description("设置或获取输入框的字体大小")]
        public double TextFontSize
        {
            get
            {
                return this.passwordBox.FontSize;
            }
            set
            {
                this.passwordBox.FontSize = value;
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


        [Description("设置或获取输入框的文字")]
        public string Text
        {
            get
            {
                return this.passwordBox.Password;
            }
            set
            {
                this.passwordBox.Password = value;
                this.UpdateLayout();
            }
        }


        [Description("设置或获取输入框的文字的最大字符数")]
        public int MaxLength
        {
            get
            {
                return this.passwordBox.MaxLength;
            }
            set
            {
                this.passwordBox.MaxLength = value;
            }
        }

        [Description("设置或获取控件的内边距")]
        public System.Windows.Thickness BoxPadding
        {
            get
            {
                Thickness t = this.passwordBox.Padding;
                return new Thickness(t.Left - 10, t.Top, t.Right, t.Bottom);
            }
            set
            {
                this.passwordBox.Padding = new Thickness(10 + value.Left, value.Top, value.Right, value.Bottom);
                this.hint.Margin = new Thickness(10 + value.Left, 0, 0, 0);
            }
        }

        [Description("设置或获取PasswordBox水平对齐方式")]
        public HorizontalAlignment BoxHorizontalContentAlignment
        {
            get
            {
                return this.passwordBox.HorizontalContentAlignment;
            }
            set
            {
                this.passwordBox.HorizontalContentAlignment = value;
            }
        }

        [Description("设置控件BoxPasting事件")]
        public event EventHandler<TextBoxPastingEventArgs> BoxPasting;
        private void passwordBox_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            BoxPasting?.Invoke(sender, new TextBoxPastingEventArgs(e));
        }

        [Description("设置控件BoxPreviewKeyDown事件")]
        public event KeyEventHandler BoxPreviewKeyDown;

        private void passwordBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            BoxPreviewKeyDown?.Invoke(sender, e);
        }

        [Description("设置控件BoxPreviewTextInput事件")]
        public event TextCompositionEventHandler BoxPreviewTextInput;
        private void passwordBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            BoxPreviewTextInput?.Invoke(sender, e);
        }

        [Description("设置控件PasswordBoxPasswordChanged事件")]
        public event RoutedEventHandler PasswordBoxPasswordChanged;

        private void passwordBox_PasswordChanged(object sender, RoutedEventArgs e)
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

            PasswordBoxPasswordChanged?.Invoke(sender, e);
        }

        private bool IsLoad;

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.IsLoad = true;

            if (string.IsNullOrEmpty(this.Text))
            {
                this.hint.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                this.hint.Visibility = System.Windows.Visibility.Collapsed;
            }
        }


        public void TextFocus()
        {
            this.passwordBox.Focus();
        }


    }
}
