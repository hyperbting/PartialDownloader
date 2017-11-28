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
            //if (!CheckFileExist(_path))
            //    return;

            Console.WriteLine("ok");
            using (var strea = new FileStream(_path, FileMode.Append))
            {
                using (var bw = new BinaryWriter(strea))
                {
                    bw.Write(_data);
                }
                //strea.Write(_data, 0, _data.Length);
            }
        }

        //public void AppendTo(string _path, Stream _data)
        //{
        //    if (!CheckFileExist(_path))
        //        return;

        //    using (var strea = new FileStream(_path, FileMode.Append))
        //    {
        //        strea.Write(_data, 0, _data.Length);
        //    }
        //}

        public static void CopyStream(Stream input, Stream output)
        {
            byte[] buffer = new byte[4096];//new byte[81920];//new byte[32768];

            int read;

            while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, read);
            }
        }
    }
}
