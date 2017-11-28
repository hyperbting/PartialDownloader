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

        public long CheckFileSize(string _path)
        {
            FileInfo finfo = new FileInfo(_path);

            if (finfo.Exists)
                return finfo.Length;
            else
                return -1;
        }

        public void AppendTo(string _path, byte[] _data)
        {
            //File.WriteAllBytes("", _data);
            using (var stream = new FileStream(_path, FileMode.Append))
            {
                stream.Write(_data, 0, _data.Length);
            }
        }
    }
}
