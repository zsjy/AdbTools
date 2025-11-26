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
                        _GITHUB_PROXY = GetKeyVlaue("GITHUB_PROXY");
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
            /// 
            /// </summary>
            public static bool IS_GITHUB_PROXY
            {
                get
                {
                    string v = GetKeyVlaue("GITHUB_PROXY");
                    return null != v;
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

        public static class FileCounter
        {
            /// <summary>
            /// 统计文件数量（包括子文件夹），支持上限控制
            /// </summary>
            /// <param name="paths">要统计的路径列表</param>
            /// <param name="exceededMax">是否超过上限</param>
            /// <param name="maxCount">最大文件数量上限（默认100）</param>
            /// <param name="searchPattern">搜索模式（默认"*.*"）</param>
            /// <param name="includeSubdirectories">是否包含子文件夹（默认true）</param>
            /// <returns>文件数量</returns>
            public static int CountFiles(
                string[] paths,
                out bool exceededMax,
                int maxCount = 100,
                string searchPattern = "*.*",
                bool includeSubdirectories = true)
            {
                exceededMax = false;

                if (paths == null || paths.Length == 0)
                    return 0;

                int count = 0;
                var searchOption = includeSubdirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

                foreach (string path in paths)
                {
                    if (count >= maxCount)
                    {
                        exceededMax = true;
                        return maxCount;
                    }

                    if (File.Exists(path))
                    {
                        count++;
                        if (count >= maxCount)
                        {
                            exceededMax = true;
                            return maxCount;
                        }
                    }
                    else if (Directory.Exists(path))
                    {
                        try
                        {
                            var files = Directory.EnumerateFiles(path, searchPattern, searchOption);
                            foreach (var file in files)
                            {
                                count++;
                                if (count >= maxCount)
                                {
                                    exceededMax = true;
                                    return maxCount;
                                }
                            }
                        }
                        catch (UnauthorizedAccessException)
                        {
                            if (includeSubdirectories)
                            {
                                count = CountDirectorySafe(path, searchPattern, count, maxCount, out exceededMax);
                                if (exceededMax) return maxCount;
                            }
                            else
                            {
                                try
                                {
                                    var files = Directory.EnumerateFiles(path, searchPattern, SearchOption.TopDirectoryOnly);
                                    foreach (var file in files)
                                    {
                                        count++;
                                        if (count >= maxCount)
                                        {
                                            exceededMax = true;
                                            return maxCount;
                                        }
                                    }
                                }
                                catch
                                {
                                    // 忽略错误
                                }
                            }
                        }
                        catch (Exception)
                        {
                            // 忽略其他错误
                        }
                    }
                }

                exceededMax = count >= maxCount;
                return count;
            }

            private static int CountDirectorySafe(string directoryPath, string searchPattern, int currentCount, int maxCount, out bool exceededMax)
            {
                exceededMax = false;
                var stack = new Stack<string>();
                stack.Push(directoryPath);
                int count = currentCount;

                while (stack.Count > 0)
                {
                    if (count >= maxCount)
                    {
                        exceededMax = true;
                        return maxCount;
                    }

                    string currentDir = stack.Pop();

                    try
                    {
                        foreach (string file in Directory.EnumerateFiles(currentDir, searchPattern, SearchOption.TopDirectoryOnly))
                        {
                            count++;
                            if (count >= maxCount)
                            {
                                exceededMax = true;
                                return maxCount;
                            }
                        }

                        foreach (string subDir in Directory.EnumerateDirectories(currentDir))
                        {
                            stack.Push(subDir);
                        }
                    }
                    catch
                    {
                        // 忽略错误
                    }
                }

                return count;
            }
        }
    }
}
