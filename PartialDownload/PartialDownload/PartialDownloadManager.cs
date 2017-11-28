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
            long localFileSize = myFileIOHelper.CheckFileSize(_localPath);
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

            long targetLen = myWebRequestHelper.CheckFileSize(_url);

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
            System.IO.Stream stream = myWebRequestHelper.DownloadParts("https://s3-ap-northeast-1.amazonaws.com/hooloop360travelmap/hooloop360travelmap/Android/Android", 0);
            //myFileIOHelper.AppendTo(@"c:\123.546", );

        }
    }
}
