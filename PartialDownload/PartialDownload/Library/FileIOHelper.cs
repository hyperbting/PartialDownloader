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

        public void AppendTo(string _path, byte[] _data)
        {
            using (var fstream = new FileStream(_path, FileMode.Append))
            {
                //using (var bw = new BinaryWriter(fstream))
                //{
                //    bw.Write(_data);
                //}
                fstream.Write(_data, 0, _data.Length);
            }
        }

        public byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[65536];//new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int bytesRead;
                while ((bytesRead = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                        ms.Write(buffer, 0, bytesRead);
                }
                return ms.ToArray();

                //      input.CopyTo(ms);
                //      return ms.ToArray();
            }

            //return ((MemoryStream)input).ToArray();
        }
    }
}
