using System;
using System.Net;
using System.IO;

namespace Update.AccessInterface
{
    /// <summary>
    /// post请求
    /// </summary>
    public static class RequestJson
    {

        public static bool DownloadFile(string url, string savePath)
        {
            try
            {
                // 创建 HttpWebRequest 对象
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
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

                return true;
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
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }
        }

    }
}
