using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;

namespace AdbTools
{
    public class Globals
    {


        /// <summary>
        /// Config配置文件读取
        /// </summary>
        public class AppSettings
        {
            private static string _LAST_DEVICE_ADDRESS = null;
            private static List<string> _DEVICE_ADDRESS_HISTORY = null;
            private static string _GITHUB_PROXY = null;
            /// <summary>
            /// 
            /// </summary>
            public static string LAST_DEVICE_ADDRESS
            {
                get
                {
                    if (null == _LAST_DEVICE_ADDRESS)
                    {
                        _LAST_DEVICE_ADDRESS = GetKeyVlaue("LAST_DEVICE_ADDRESS");
                    }
                    if (null == _LAST_DEVICE_ADDRESS)
                    {
                        _LAST_DEVICE_ADDRESS = "";
                    }
                    return _LAST_DEVICE_ADDRESS;
                }
                set
                {
                    UpdateAppConfig("LAST_DEVICE_ADDRESS", value);
                    _LAST_DEVICE_ADDRESS = value;
                }
            }

            /// <summary>
            /// 
            /// </summary>
            public static List<string> DEVICE_ADDRESS_HISTORY
            {
                get
                {
                    if (null == _DEVICE_ADDRESS_HISTORY)
                    {
                        string v = GetKeyVlaue("DEVICE_ADDRESS_HISTORY");
                        if (!string.IsNullOrWhiteSpace(v))
                        {
                            _DEVICE_ADDRESS_HISTORY = new List<string>(v.Split(','));

                        }
                    }
                    if (null == _DEVICE_ADDRESS_HISTORY)
                    {
                        _DEVICE_ADDRESS_HISTORY = new List<string>();
                    }
                    return _DEVICE_ADDRESS_HISTORY;
                }
                set
                {
                    UpdateAppConfig("DEVICE_ADDRESS_HISTORY", string.Join(",", value));
                    _DEVICE_ADDRESS_HISTORY = value;
                }
            }

            /// <summary>
            /// 
            /// </summary>
            public static string GITHUB_PROXY
            {
                get
                {
                    if (null == _GITHUB_PROXY)
                    {
                        string v = GetKeyVlaue("GITHUB_PROXY");
                        if (!string.IsNullOrWhiteSpace(v))
                        {
                            _GITHUB_PROXY = v;

                        }
                    }
                    if (null == _GITHUB_PROXY)
                    {
                        _GITHUB_PROXY = "https://ghfast.top/";
                    }
                    return _GITHUB_PROXY;
                }
                set
                {
                    UpdateAppConfig("GITHUB_PROXY", value);
                    _GITHUB_PROXY = value;
                }
            }

            /// <summary>
            /// 获取配置的值
            /// </summary>
            /// <param name="key">key</param>
            /// <returns>读取错误时为null</returns>
            public static string GetKeyVlaue(string key)
            {
                try
                {
                    return ConfigurationManager.AppSettings[key];
                }
                catch (Exception e)
                {
                    return null;
                }
            }

            ///<summary>
            ///在*.exe.config文件中appSettings配置节增加一对键、值对
            ///</summary>
            ///<param name="newKey"></param>
            ///<param name="newValue"></param>
            public static void UpdateAppConfig(string newKey, string newValue)
            {
                try
                {
                    bool isModified = false;
                    foreach (string key in ConfigurationManager.AppSettings)
                    {
                        if (key == newKey)
                        {
                            isModified = true;
                        }
                    }

                    // Open App.Config of executable
                    Configuration config =
                        ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                    // You need to remove the old settings object before you can replace it
                    if (isModified)
                    {
                        config.AppSettings.Settings.Remove(newKey);
                    }
                    // Add an Application Setting.
                    config.AppSettings.Settings.Add(newKey, newValue);
                    // Save the changes in App.config file.
                    config.Save(ConfigurationSaveMode.Modified);
                    // Force a reload of a changed section.
                    ConfigurationManager.RefreshSection("appSettings");
                }
                catch (Exception e)
                {
                }
            }

        }




    }
}
