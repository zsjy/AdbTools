using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Update
{
    public class ZipFileUtils
    {
        public void UnzipFile(string zipFilePath, string extractPath)
        {
            try
            {
                // 检查 ZIP 文件是否存在
                if (!File.Exists(zipFilePath))
                {
                    throw new FileNotFoundException($"ZIP file not found: {zipFilePath}");
                }

                // 检查目标文件夹是否存在，如果不存在则创建
                if (!Directory.Exists(extractPath))
                {
                    Directory.CreateDirectory(extractPath);
                }

                // 解压 ZIP 文件
                ZipFile.ExtractToDirectory(zipFilePath, extractPath);

                Console.WriteLine($"ZIP file extracted to: {extractPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error extracting ZIP file: {ex.Message}");
            }
        }

    }
}
