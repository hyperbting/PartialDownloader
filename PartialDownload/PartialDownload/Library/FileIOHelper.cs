using System;
using System.Collections.Generic;

using System.IO;

namespace PartialDownload.Library
{
    public class FileIOHelper
    {
        public bool CheckFileExist(string _path)
        {
            return File.Exists(_path);
        }

        public int CheckFileSize(string _path)
        {
            FileInfo finfo = new FileInfo(_path);

            if (finfo.Exists)
                return (int)finfo.Length;
            else
                return -1;
        }

        public void AppendTo(string _path, Stream _sdata, int _windowsSize = 1048576)
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
                AppendTo(_path, result);
            }
        }

        public void AppendTo(string _path, byte[] _data)
        {
            using (var fstream = new FileStream(_path, FileMode.Append))
            {
                fstream.Write(_data, 0, _data.Length);
            }
        }
    }
}
