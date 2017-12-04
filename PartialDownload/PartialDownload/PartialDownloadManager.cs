using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace PartialDownloader
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
        private FileIOHelper myFileIOHelper;

        #region Delegate
        public delegate void FloatDelegate(float _fl);
        #endregion
        public FloatDelegate myProgressDelegate;
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

        protected void OnEnable()
        {
            myProgressDelegate += DefaultProgressDelegate;
        }

        protected void OnDisable()
        {
            myProgressDelegate -= DefaultProgressDelegate;
        }


        protected void Awake()
        {
            if (instance == null)
                instance = this;
        }

        protected void Start()
        {
            myFileIOHelper = new FileIOHelper();
            myUnityWebRequestHelper = new UnityWebRequestHelper();
            //ssls = new SSLSupporter();

            //first check networking as soon as possibile
            if (checkAtStart)
                CheckInternetConnection(true);
        }

        public IEnumerator DownloadBehaviourCheck(string _url, string _localPath, Action<DownloadProcess,int,int> _resultAct)
        {
            int localFileSize = myFileIOHelper.CheckFileSize(_localPath);

            bool processed = false;
            bool supportPartialDL = false;
            int remoteFileSize = -1;
            StartCoroutine(myUnityWebRequestHelper.CheckFileSize(_url, 
                (int _val, bool _supportPartialDL) => 
                {
                    remoteFileSize = _val;
                    supportPartialDL = _supportPartialDL;
                    processed = true;
                }
                ));

            while (!processed)
                yield return null;

            //check remote server support
            if(!supportPartialDL)//if (WebRequestHelper.CheckServerSupportPartialContent(_url))
            {
                //not support partial download
                Debug.Log("Remote Server Not Support Partial Download");
                _resultAct( DownloadProcess.RedownloadFromBeginning, localFileSize, remoteFileSize);
                yield break;
            }

            _resultAct(DownloadBehaviourCheck(localFileSize, remoteFileSize), localFileSize, remoteFileSize);
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
                    _remoteURL,
                    _localPath,
                    _windowSize
                )
            );
        }

        protected IEnumerator DownloadWholeFile(string _remoteURL, string _localPath, int _windowSize)
        {
            int rfsize = -1;
            int lfsize = -1;

            DownloadProcess pd = DownloadProcess.BeforeProcess;
            StartCoroutine(DownloadBehaviourCheck(_remoteURL, _localPath, 
                (DownloadProcess _dp, int _local, int _remote) =>
                {
                    pd = _dp;
                    rfsize = _remote;
                    lfsize = _local;
                }
                ));

            while (pd == DownloadProcess.BeforeProcess)
                yield return null;

            Debug.LogFormat("remote file size {0}; local file size {1}; Next Process will be {2}", rfsize, lfsize, pd);
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
                StartCoroutine(
                    myUnityWebRequestHelper.DownloadParts(
                        (byte[] _bytes) => {
                            myFileIOHelper.AppendTo(_localPath, _bytes);
                            locker = false;
                            //Debug.Log(i + " finished");
                            myProgressDelegate((float)i / (float)rfsize);
                        }, _remoteURL, i, _windowSize)
                );

                while (locker)
                    yield return null;//wait for this chunk downloaded
            }
            myProgressDelegate(1);
            downloaderCounter--;
        }

        #region Utility
        public bool CheckInternetConnection(bool _forceRecheck = false)
        {
            if ( _forceRecheck  || hasInternetConnection == InternetConnectionSyayue.UnKnOwN)
                StartCoroutine(CheckInternetConnection(checkerAddress,
                    (bool _hasConnection) =>
                    {
                        hasInternetConnection = (_hasConnection) ? InternetConnectionSyayue.Okay : InternetConnectionSyayue.Blocked;
                    }
                ));

            if (hasInternetConnection == InternetConnectionSyayue.Okay)
                return true;
            return false;
        }        

        protected IEnumerator CheckInternetConnection(List<string> _url, Action<bool> _successAct)
        {
            bool successConnect = false;
            bool atWork = false;
            
            foreach (var url in checkerAddress)
            {
                atWork = true;
                StartCoroutine(
                    myUnityWebRequestHelper.CheckInternetConnection(
                        (bool _canConnect) =>
                        {
                            atWork = false;
                            if (_canConnect)
                                successConnect = true;
                        }, 
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

        #region Default Delegate Action
        void DefaultProgressDelegate(float _val)
        {
            Debug.Log(_val);
        }
        #endregion
    }

    public enum DownloadProcess
    {
        BeforeProcess,
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
