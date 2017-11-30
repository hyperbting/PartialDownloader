using PartialDownload.Library;
using PartialDownload.LibraryUnity;

using System;
using System.Collections;
using System.IO;
using UnityEngine;

namespace PartialDownload
{
    public class PartialDownloadManager: MonoBehaviour
    {
        private UnityWebRequestHelper myUnityWebRequestHelper;
        private WebRequestHelper myWebRequestHelper;
        private FileIOHelper myFileIOHelper;

        protected void Start()
        {
            myWebRequestHelper = new WebRequestHelper();
            myFileIOHelper = new FileIOHelper();
            myUnityWebRequestHelper = new UnityWebRequestHelper();
        }

        /// <summary>
        /// Checker
        /// </summary>
        /// <param name="_url"></param>
        /// <param name="_localPath"></param>
        /// <returns></returns>
        public bool PartialDownloadCheck(string _url, string _localPath)
        {
            //check local file exist
            int localFileSize = myFileIOHelper.CheckFileSize(_localPath);
            if (localFileSize < 0)
            {
                //file not exist
                return true;
            }

            //check remote file size
            if (myWebRequestHelper.CheckServerSupportPartialContent(_url))
            {
                //not support partial download
                return true;
            }

            int targetLen = myWebRequestHelper.CheckFileSize(_url);
            if (targetLen <= 0)
            {                
                return false; //remote file not exist
            }

            if(localFileSize != targetLen)
            {
                //file must changed
                return true;
            }

            if (localFileSize == targetLen)
            {
                //file Matched?! job done here
                return false;
            }
            return true;
        }

        public IEnumerator DownloadWholeFile(Action<Stream> _resultStreamAct, string _remoteURL, string _localPath, int _windowSize = 1048576)
        {
            bool locker;
            int rfsize = myWebRequestHelper.CheckFileSize(_remoteURL);
            int lfsize = myFileIOHelper.CheckFileSize(_localPath);

            Debug.Log("remote file size " + rfsize);
            Debug.Log("local file size " + lfsize);

            for (int i = lfsize + 1; i < rfsize; i += _windowSize)
            {
                locker = true;
                Coroutine cr = StartCoroutine(
                    myUnityWebRequestHelper.DownloadParts(
                        (Stream _st) => {
                            myFileIOHelper.AppendTo(_localPath, _st);
                            locker = false;
                            Debug.Log(i + " finished");
                        }, _remoteURL, i, _windowSize)
                );

                while(locker)
                    yield return null;
            }

        }
    }
}
