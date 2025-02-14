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
        public static bool UnzipFile(string zipFilePath, string extractPath)
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

                // 打开 ZIP 文件
                using (ZipArchive archive = ZipFile.OpenRead(zipFilePath))
                {
                    foreach (ZipArchiveEntry entry in archive.Entries)
                    {
                        string fullPath = Path.Combine(extractPath, entry.FullName);

                        // 如果是目录，则创建目录
                        if (string.IsNullOrEmpty(entry.Name))
                        {
                            Directory.CreateDirectory(fullPath);
                        }
                        else
                        {
                            // 如果是文件，检查是否已存在
                            if (File.Exists(fullPath))
                            {
                                File.Delete(fullPath); // 删除已存在的文件
                            }

                            // 解压文件
                            entry.ExtractToFile(fullPath);
                        }
                    }
                }
                File.Delete(zipFilePath);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error extracting ZIP file: {ex.Message}");
                throw ex;
            }
        }

    }
}
