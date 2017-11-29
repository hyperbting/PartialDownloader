using PartialDownload.Library;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PartialDownload
{
    public class PartialDownloadManager
    {
        private WebRequestHelper myWebRequestHelper;
        private FileIOHelper myFileIOHelper;

        public PartialDownloadManager()
        {
            myWebRequestHelper = new WebRequestHelper();
            myFileIOHelper = new FileIOHelper();
        }

        public bool PartialDownloadCheck(string _url, string _localPath)
        {
            //check local file exist
            int localFileSize = myFileIOHelper.CheckFileSize(_localPath);
            if (localFileSize < 0)
            {
                //file not exist
                return false;
            }

            //check remote file size
            if (myWebRequestHelper.CheckServerSupportPartialContent(_url))
            {
                //not support partial download
                return false;
            }

            int targetLen = myWebRequestHelper.CheckFileSize(_url);

            if (targetLen < 0)
            {
                //file not exist
                return false;
            }

            if(localFileSize > targetLen)
            {
                //file must changed
                return false;
            }

            if (localFileSize == targetLen)
            {
                //file Matched?! job done here
                return false;
            }

            return true;
        }

        public void Download()
        {
            string url = "https://s3-ap-northeast-1.amazonaws.com/hooloop360travelmap/hooloop360travelmap/Video/taidong/taidong_%E5%8E%9F%E5%A7%8B%E9%83%A8%E8%90%BD%E5%B1%B1%E5%9C%B0%E7%BE%8E%E9%A3%9F.mp4";
            string localPath = @"d:\123.mp4";

            int windowSize = 65536;

            int rfsize = myWebRequestHelper.CheckFileSize(url);
            int lfsize = myFileIOHelper.CheckFileSize(localPath);

            Console.WriteLine(rfsize);
            Console.WriteLine(lfsize);
            for (int i = lfsize + 1; i < rfsize; i += windowSize)
            {
                System.IO.Stream stream = myWebRequestHelper.DownloadParts( url, i, windowSize);

                byte[] byteData = myFileIOHelper.ReadFully(stream);
                //myFileIOHelper.AppendTo("d:/123", myFileIOHelper.ReadFully(stream));

                myFileIOHelper.AppendTo(localPath, byteData);
            }
        }

        public void Download2()
        {
            string url = "https://s3-ap-northeast-1.amazonaws.com/hooloop360travelmap/hooloop360travelmap/Video/taidong/taidong_%E5%8E%9F%E5%A7%8B%E9%83%A8%E8%90%BD%E5%B1%B1%E5%9C%B0%E7%BE%8E%E9%A3%9F.mp4";
            string localPath = @"d:\123_2.mp4";

            int rfsize = myWebRequestHelper.CheckFileSize(url);
            int lfsize = myFileIOHelper.CheckFileSize(localPath);

            Console.WriteLine(rfsize);
            Console.WriteLine(lfsize);
            for (int i = lfsize + 1; i < rfsize; i += rfsize)
            {
                System.IO.Stream stream = myWebRequestHelper.DownloadParts(url, i, rfsize);

                byte[] byteData = myFileIOHelper.ReadFully(stream);

                myFileIOHelper.AppendTo(localPath, byteData);
            }
        }
    }
}
