using System;
using System.Collections.Generic;

using System.IO;

namespace PartialDownloadManager.Library
{
    public class FileIOHelper
    {
        public bool CheckFileExist(string _localPath)
        {
            return File.Exists(_localPath);
        }

        public int CheckFileSize(string _localPath)
        {
            FileInfo finfo = new FileInfo(_localPath);

            if (finfo.Exists)
                return (int)finfo.Length;
            else
                return -1;
        }

        public void AppendTo(string _localPath, Stream _sdata, int _windowsSize = 1048576)
        {
            byte[] buffer = new byte[_windowsSize];

            using (MemoryStream ms = new MemoryStream())
            {
                int bytesRead;
                while ((bytesRead = _sdata.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, bytesRead);
                }

                byte[] result = ms.ToArray();
                AppendTo(_localPath, result);
            }
        }

        public void AppendTo(string _localPath, byte[] _data)
        {
            using (var fstream = new FileStream(_localPath, FileMode.Append))
            {
                fstream.Write(_data, 0, _data.Length);
            }
        }

        public void Remove(string _localPath)
        {
            if (!CheckFileExist(_localPath))
                return;

            File.Delete(_localPath);
        }

        public void Touch(string _localPath)
        {
            if (!CheckFileExist(_localPath))
            {
                using (File.Create(_localPath))
                {
                }
                return;
            }

            File.SetLastWriteTimeUtc(_localPath, DateTime.UtcNow);
        }
    }
}
