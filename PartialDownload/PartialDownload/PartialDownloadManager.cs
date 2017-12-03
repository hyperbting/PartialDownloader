using PartialDownloadManager.Library;
using PartialDownloadManager.LibraryUnity;

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEngine;

namespace PartialDownloadManager
{
    public class PartialDownloadManager: MonoBehaviour
    {
        [Header("Internet Connection Check Address")]
        [SerializeField]
        private bool checkAtStart = true;
        [SerializeField]
        [Tooltip("Address to check")]
        private List<string> checkerAddress = new List<string>() { "https://www.google.com", "http://baidu.com" };
        [SerializeField]
        private InternetConnectionSyayue hasInternetConnection = InternetConnectionSyayue.UnKnOwN;

        [Header("Download Events Behaviour")]
        public int maxCocurrentDownloader = 1;
        [Tooltip("Behaviour when local file size is LARGER than remote file size.")]
        public DownloadProcess localLarger = DownloadProcess.RedownloadFromBeginning;
        [Tooltip("Behaviour when local file size is SMALLER than remote file size.")]
        public DownloadProcess localSmaller = DownloadProcess.Resume;

        [Header("Debug Info")]
        [SerializeField]
        private int downloaderCounter = 0;

        private UnityWebRequestHelper myUnityWebRequestHelper;
        private WebRequestHelper myWebRequestHelper;
        private FileIOHelper myFileIOHelper;
        //private SSLSupporter ssls;

        public static PartialDownloadManager instance;

        public static void Initialize()
        {
            if (instance == null)
            {
                var go = new GameObject("PartialDownloadManager", typeof(PartialDownloadManager));
            }
            else
            {
                Debug.Log("PartialDownloadManager Exist!");
            }
        }

        protected void Awake()
        {
            if (instance == null)
                instance = this;
        }

        protected void Start()
        {
            myWebRequestHelper = new WebRequestHelper();
            myFileIOHelper = new FileIOHelper();
            myUnityWebRequestHelper = new UnityWebRequestHelper();
            //ssls = new SSLSupporter();

            //first check networking as soon as possibile
            if (checkAtStart)
                CheckInternetConnection(true);
        }

        public DownloadProcess DownloadBehaviourCheck(string _url, string _localPath, ref int _localFileSize, ref int _remoteFileSize)
        {
            _remoteFileSize = myWebRequestHelper.CheckFileSize(_url);
            _localFileSize = myFileIOHelper.CheckFileSize(_localPath);

            //check remote server support
            if (WebRequestHelper.CheckServerSupportPartialContent(_url))
            {
                //not support partial download
                Debug.Log("Remote Server Not Support Partial Download");
                return DownloadProcess.RedownloadFromBeginning;
            }

            return DownloadBehaviourCheck(_localFileSize, _remoteFileSize);
        }

        protected DownloadProcess DownloadBehaviourCheck(int localFileSize, int remoteFileSize)
        {
            if (remoteFileSize <= 0)//remote file not exist
            {
                Debug.Log("Remote File Not Found");
                return DownloadProcess.DoNothing; 
            }

            if (localFileSize < 0)//local file not exist
            {
                return DownloadProcess.Resume;
            }

            if (localFileSize < remoteFileSize)
            {
                return localSmaller;
            }
            else if (localFileSize > remoteFileSize)//file must changed
            {
                return localLarger;
            }
            else//file size Matched?! job done here
            {
                Debug.Log("File Size Matched");
                return DownloadProcess.DoNothing;
            }
        }

        public void DownloadWholeFileToDisk(string _remoteURL, string _localPath, int _windowSize = 1048576)
        {
            StartCoroutine(
                DownloadWholeFile(
                    (Stream _st) => { myFileIOHelper.AppendTo(_localPath, _st, _windowSize); },
                    _remoteURL,
                    _localPath,
                    _windowSize
                )
            );
        }

        protected IEnumerator DownloadWholeFile(Action<Stream> _resultStreamAct, string _remoteURL, string _localPath, int _windowSize)
        {
            int rfsize = -1; //myWebRequestHelper.CheckFileSize(_remoteURL);
            int lfsize = -1; //myFileIOHelper.CheckFileSize(_localPath);

            DownloadProcess pd = DownloadBehaviourCheck(_remoteURL, _localPath, ref lfsize, ref rfsize);

            Debug.LogFormat("remote file size {0}; local file size {1}; Next Process {2}", rfsize, lfsize, pd);
            switch (pd)
            {
                case DownloadProcess.RedownloadFromBeginning:
                    myFileIOHelper.Remove(_localPath); //Delete TargetFile first
                    lfsize = -1;
                    break;
                case DownloadProcess.Resume: //just keep doing
                    break;
                case DownloadProcess.DoNothing:
                default:
                    Debug.Log("Do Nothing");
                    yield break;
            }

            while (downloaderCounter >= maxCocurrentDownloader)
                yield return null;

            downloaderCounter++;
            bool locker;

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
                    yield return null;//wait for this chunk downloaded
            }
            downloaderCounter--;
        }

        #region Utility
        public bool CheckInternetConnection(bool _forceRecheck = false)
        {
            void DefaultInternetWorking(bool _hasConnection)
            {
                hasInternetConnection = (_hasConnection) ? InternetConnectionSyayue.Okay : InternetConnectionSyayue.Blocked;
            }

            if ( _forceRecheck  || hasInternetConnection == InternetConnectionSyayue.UnKnOwN)
                StartCoroutine(CheckInternetConnection(checkerAddress, DefaultInternetWorking));

            if (hasInternetConnection == InternetConnectionSyayue.Okay)
                return true;
            return false;
        }        

        protected IEnumerator CheckInternetConnection(List<string> _url, Action<bool> _successAct)
        {
            bool successConnect = false;
            bool atWork = false;

            void DefaultCheckInternetConnection(bool _canConnect)
            {
                atWork = false;
                if (_canConnect)
                    successConnect = true;
            }
            
            foreach (var url in checkerAddress)
            {
                atWork = true;
                StartCoroutine(
                    myUnityWebRequestHelper.CheckInternetConnection(
                        DefaultCheckInternetConnection, 
                        url));

                while (atWork)
                    yield return null;

                if (successConnect)
                {
                    _successAct(true);
                    yield break;//break foreach check
                }
            }
            _successAct(false);
        }
        #endregion
    }

    public enum DownloadProcess
    {
        DoNothing,
        RedownloadFromBeginning,
        Resume
    }

    public enum InternetConnectionSyayue
    {
        UnKnOwN,
        Okay,
        Blocked
    }
}
