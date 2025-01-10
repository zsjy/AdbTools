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
    /// ProgressBarControl.xaml 的交互逻辑
    /// </summary>
    public partial class ProgressBarControl : UserControl
    {
        public ProgressBarControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 获取或设置进度条的背景色
        /// </summary>
        [Description("获取或设置进度条的背景色")]
        public System.Windows.Media.Brush ProgressBackground
        {
            get { return progressBG.Fill; }
            set { progressBG.Fill = value; }
        }

        /// <summary>
        /// 获取或设置进度条的边框色
        /// </summary>
        [Description("获取或设置进度条的边框色")]
        public System.Windows.Media.Brush ProgressStroke
        {
            get { return progressBG.Stroke; }
            set { progressBG.Stroke = value; }
        }

        /// <summary>
        /// 获取或设置进度条的边框粗细
        /// </summary>
        [Description("获取或设置进度条的边框粗细")]
        public double ProgressStrokeThickness
        {
            get { return progressBG.StrokeThickness; }
            set { progressBG.StrokeThickness = value; }
        }

        /// <summary>
        /// 获取或设置进度条的进度色
        /// </summary>
        [Description("获取或设置进度条的进度色")]
        public System.Windows.Media.Brush ProgressColor
        {
            get { return progressValue.Fill; }
            set { progressValue.Fill = value; }
        }

        /// <summary>
        /// 获取或设置进度条提示信息
        /// </summary>
        [Description("获取或设置进度条提示信息")]
        public string HintProgress
        {
            get { return progressTip.Content.ToString(); }
            set { progressTip.Content = value; }
        }

        /// <summary>
        /// 获取或设置进度条提示信息文字大小
        /// </summary>
        [Description("获取或设置进度条提示信息文字大小")]
        public double HintFontSize
        {
            get { return progressTip.FontSize; }
            set { progressTip.FontSize = value; }
        }

        /// <summary>
        /// 获取或设置进度条提示信息字体颜色
        /// </summary>
        [Description("获取或设置进度条提示信息字体颜色")]
        public System.Windows.Media.Brush HintForeground
        {
            get { return progressTip.Foreground; }
            set { progressTip.Foreground = value; }
        }

        private double _Maximum = 100.0;

        /// <summary>
        /// 获取或设置进度条的最大进度值,默认值为100
        /// </summary>
        /// <exception cref="InvalidOperationException">进度值不可小于等于零</exception>
        [Description("获取或设置进度条的最大进度值")]
        public double Maximum
        {
            get { return _Maximum; }
            set 
            {
                if (value <= 0)
                    throw new InvalidOperationException("进度值不可小于等于零");
                _Maximum = value;
            }
        }

        private double _Value;

        /// <summary>
        /// 获取或设置进度条的进度值
        /// </summary>
        [Description("获取或设置进度条的进度值")]
        public double Value
        {
            get { return _Value; }
            set
            {
                if (value < 0)
                    _Value = 0;
                else if(value > _Maximum)
                    _Value = 100;
                _Value = value;

                this.progressValue.Width = (_Value / _Maximum) * this.progressBG.ActualWidth;
            }
        }




    }
}
