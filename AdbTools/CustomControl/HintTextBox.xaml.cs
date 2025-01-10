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
    /// HintTextBox.xaml 的交互逻辑
    /// </summary>
    public partial class HintTextBox : UserControl
    {
        public HintTextBox()
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
        [Bindable(true)]
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


        [Description("获取TextBox")]
        public TextBox GetTextBox
        {
            get
            {
                return this.textBox;
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

        [Description("设置或获取光标颜色")]
        [Bindable(true)]
        public Brush BoxCaretBrush
        {
            get
            {
                return this.textBox.CaretBrush;
            }
            set
            {
                this.textBox.CaretBrush = value;
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

        [Description("设置或获取指定文本在到达包含框的边缘时是否换行")]
        public TextWrapping TextBoxWrapping
        {
            get
            {
                return this.textBox.TextWrapping;
            }
            set
            {
                this.textBox.TextWrapping = value;
            }
        }

        [Description("获取或设置可见行的最大数目。")]
        public int TextBoxMaxLines
        {
            get
            {
                return this.textBox.MaxLines;
            }
            set
            {
                this.textBox.MaxLines = value;
            }
        }


        [Description("设置或获取输入框的字体颜色")]
        public System.Windows.Media.Brush TextForeground
        {
            get
            {
                return this.textBox.Foreground;
            }
            set
            {
                this.textBox.Foreground = value;
            }
        }

        [Description("设置或获取输入框的字体大小")]
        public double TextFontSize
        {
            get
            {
                return this.textBox.FontSize;
            }
            set
            {
                this.textBox.FontSize = value;
            }
        }

        [Description("获取或设置一个值，确定当用户导航控件通过使用 TAB 键元素接收焦点的顺序。")]
        public int TextTabIndex
        {
            get
            {
                return this.textBox.TabIndex;
            }
            set
            {
                this.textBox.TabIndex = value;
            }
        }


        public void TextFocus()
        {
            this.textBox.Focus();
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
                return this.textBox.Text;
            }
            set
            {
                this.textBox.Text = value;
                this.UpdateLayout();
            }
        }

        [Description("设置或获取输入框的文字的最大字符数")]
        public int MaxLength
        {
            get
            {
                return this.textBox.MaxLength;
            }
            set
            {
                this.textBox.MaxLength = value;
            }
        }

        [Description("设置或获取控件的内边距")]
        public System.Windows.Thickness BoxPadding
        {
            get
            {
                Thickness t = this.textBox.Padding;
                return new Thickness(t.Left - 10, t.Top, t.Right, t.Bottom);
            }
            set
            {
                this.textBox.Padding = new Thickness(10 + value.Left, value.Top, value.Right, value.Bottom);
                this.hint.Margin = new Thickness(10 + value.Left, 0, 0, 0);
            }
        }

        [Description("设置或获取Hint水平对齐方式")]
        public HorizontalAlignment HintHorizontalContentAlignment
        {
            get
            {
                return this.hint.HorizontalContentAlignment;
            }
            set
            {
                this.hint.HorizontalContentAlignment = value;
            }
        }

        [Description("设置或获取TextBox水平对齐方式")]
        public HorizontalAlignment BoxHorizontalContentAlignment
        {
            get
            {
                return this.textBox.HorizontalContentAlignment;
            }
            set
            {
                this.textBox.HorizontalContentAlignment = value;
            }
        }


        [Description("设置或获取下划线的显示和隐藏")]
        public Visibility LineVisibility
        {
            get
            {
                return this.line.Visibility;
            }
            set
            {
                this.line.Visibility = value;
            }
        }

        [Description("设置或获取控件是否只读")]
        public bool IsReadOnly
        {
            get
            {
                return this.textBox.IsReadOnly;
            }
            set
            {
                this.textBox.IsReadOnly = value;
            }
        }

        [Description("设置控件BoxPasting事件")]
        public event EventHandler<TextBoxPastingEventArgs> BoxPasting;
        private void textBox_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            BoxPasting?.Invoke(sender, new TextBoxPastingEventArgs(e));
        }

        [Description("设置控件BoxPreviewKeyDown事件")]
        public event KeyEventHandler BoxPreviewKeyDown;

        private void textBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            BoxPreviewKeyDown?.Invoke(sender, e);
        }

        [Description("设置控件BoxPreviewTextInput事件")]
        public event TextCompositionEventHandler BoxPreviewTextInput;
        private void textBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            BoxPreviewTextInput?.Invoke(sender, e);
        }

        [Description("设置控件TextBoxTextChanged事件")]
        public event TextChangedEventHandler TextBoxTextChanged;

        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
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

            TextBoxTextChanged?.Invoke(sender, e);
        }

        private bool IsLoad;
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.IsLoad = true;

            if (string.IsNullOrEmpty(Text))
            {
                this.hint.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                this.hint.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        [Description("文本输入事件")]
        public event EventHandler<TextCompositionEventArgs> TextBoxInput;
        private void textBox_TextInput(object sender, TextCompositionEventArgs e)
        {
            if (TextBoxInput != null)
            {
                TextBoxInput(this, e);
            }
        }

    }

    public class TextBoxPastingEventArgs : EventArgs
    {

        public DataObjectPastingEventArgs e
        {
            get;
            private set;
        }
        public TextBoxPastingEventArgs(DataObjectPastingEventArgs e)
        {
            this.e = e;
        }
    }
}
