using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Script.Serialization;
using System.Configuration;
using AdbTools.bean;
using System.Threading;
using System.Windows;
using System.IO;

namespace AdbTools.AccessInterface
{
    /// <summary>
    /// post请求
    /// </summary>
    public static class RequestJson
    {
        public const string ApiUrl = @"https://api.github.com/repos/zsjy/AdbTools/releases";
        public const string UpdateFileName = "update.zip";
        private static JavaScriptSerializer jss = new JavaScriptSerializer();

        public static void UpdateCheck(MainWindow mainWindow)
        {
            Thread thread = new Thread(new ThreadStart(() =>
            {
                GithubReleases githubReleases = Request();
                if (null == githubReleases || App.Version.Equals(githubReleases.Name))
                {
                    return;
                }
                GithubReleasesAssets githubReleasesAssets = null;
                foreach (GithubReleasesAssets assets in githubReleases.Assets)
                {
                    if (UpdateFileName.Equals(assets.Name, StringComparison.CurrentCultureIgnoreCase))
                    {
                        githubReleasesAssets = assets;
                    }
                }
                if (null == githubReleasesAssets)
                {
                    return;
                }

                mainWindow.Dispatcher.Invoke(new Action(() =>
                {
                    if (MessageBoxResult.OK != MessageBox.Show($"发现新版本【V{githubReleases.Name}】是否更新？", "新版本", MessageBoxButton.OKCancel, MessageBoxImage.Question))
                    {
                        return;
                    }



                }));
            }));
            thread.Start();
        }

        /// <summary>
        /// 执行请求，返回返回类型对象
        /// </summary>
        /// <typeparam name="T">请求的类型</typeparam>
        /// <typeparam name="S">返回的类型</typeparam>
        /// <param name="requestType">接口名称</param>
        /// <param name="obj">请求的参数Json对象</param>
        /// <returns>返回的类型的对象</returns>
        public static GithubReleases Request()
        {
            try
            {
                string strURL = ApiUrl;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(strURL);
                request.UserAgent = "User-Agent:Mozilla/5.0 (Windows NT 5.1) AppleWebKit/535.1 (KHTML, like Gecko) Chrome/14.0.835.202 Safari/535.1";
                request.ContentType = "application/x-www-form-urlencoded";
                request.Credentials = CredentialCache.DefaultCredentials;
                request.Method = "GET";
                request.Proxy = null;
                request.Timeout = 60000;
                System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)request.GetResponse();
                System.IO.StreamReader myreader = new System.IO.StreamReader(response.GetResponseStream(), Encoding.UTF8);
                string responseText = myreader.ReadToEnd();
                myreader.Close();

                //return responseText;
                List<GithubReleases> s = jss.Deserialize<List<GithubReleases>>(responseText);
                return s[0];
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public static void DownloadFile(string url, string savePath)
        {
            try
            {
                // 创建 HttpWebRequest 对象
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create($"{Globals.AppSettings.GITHUB_PROXY}{url}");
                request.Method = "GET";

                // 发送请求并获取响应
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream responseStream = response.GetResponseStream())
                using (FileStream fileStream = new FileStream(savePath, FileMode.Create, FileAccess.Write))
                {
                    // 缓冲区大小（可以根据需要调整）
                    byte[] buffer = new byte[4096];
                    int bytesRead;

                    // 从响应流中读取数据并写入文件
                    while ((bytesRead = responseStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        fileStream.Write(buffer, 0, bytesRead);
                    }
                }

                Console.WriteLine($"File downloaded and saved to: {savePath}");
            }
            catch (WebException ex)
            {
                Console.WriteLine($"WebException: {ex.Message}");
                if (ex.Response != null)
                {
                    using (var errorResponse = (HttpWebResponse)ex.Response)
                    using (var reader = new StreamReader(errorResponse.GetResponseStream()))
                    {
                        string errorText = reader.ReadToEnd();
                        Console.WriteLine($"Error details: {errorText}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

    }
}
