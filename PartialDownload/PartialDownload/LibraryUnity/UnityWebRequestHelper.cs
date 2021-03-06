﻿using System;
using System.Collections;
using UnityEngine.Networking;

namespace PartialDownloader
{
    public class UnityWebRequestHelper
    {
        private bool atWork = false;

        //public SSLSupporter ssls = new SSLSupporter();

        /// <summary>
        /// UnityWebRequest Version of get header
        /// </summary>
        /// <param name="_url"></param>
        /// <param name="_resultAct"></param>
        /// <returns></returns>
        public IEnumerator CheckFileSize(string _url, Action<int, bool> _resultAct)
        {
            using (UnityWebRequest www = UnityWebRequest.Head(_url))
            {
                yield return www.Send();

                if (www.isNetworkError || www.isHttpError)
                {
                    _resultAct(-(int)www.responseCode, false);
                }
                else
                {
                    string result = www.GetResponseHeader("Content-Length");
                    _resultAct(int.Parse(result), www.GetResponseHeaders().ContainsKey("Accept-Ranges"));
                }
            }
        }

        public IEnumerator CheckServerSupportPartialContent(string _url, Action<bool> _resultAct, Action<string> _errorAct = null)
        {
            using (UnityWebRequest www = UnityWebRequest.Head(_url))
            {
                yield return www.Send();

                while (!www.isDone)
                    yield return null;

                if (www.isNetworkError || www.isHttpError)
                {
                    if (_errorAct != null)
                        _errorAct(www.error);

                    _resultAct(false);
                }
                else
                {
                    _resultAct(www.GetResponseHeaders().ContainsKey("Accept-Ranges"));
                }
            }
        }

        public IEnumerator DownloadParts(Action<byte[]> _successAct, string _url, int _start, int _windowSize = 1048576, Action<float> _progressAct = null, Action<string> _errorAct = null)
        {
            DownloadHandler downloadHandler = new DownloadHandlerBuffer();

            using (UnityWebRequest www = new UnityWebRequest(_url, UnityWebRequest.kHttpVerbGET, downloadHandler, null))
            {
                www.SetRequestHeader("Range", string.Format("bytes={0}-{1}", _start, (_start + _windowSize - 1)));
                yield return www.Send();

                while (!www.isDone)
                {
                    if (_progressAct != null)
                        _progressAct(www.downloadProgress);
                    yield return null;
                }

                if (www.isNetworkError || www.isHttpError)
                {
                    if (_errorAct != null)
                        _errorAct(www.error);
                }
                else
                {
                    _successAct(www.downloadHandler.data);
                }
            }
        }

        public IEnumerator Download(string _url, Action<byte[]> _successAct, Action<float> _pregressAct = null, Action<string> _errorAct = null)
        {
            using (UnityWebRequest request = UnityWebRequest.Get(_url))
            {
                while (!request.isDone)
                {
                    if (_pregressAct != null)
                        _pregressAct(request.downloadProgress);
                    yield return null;
                }

                if (request.isNetworkError || request.isHttpError) // Error
                {
                    if(_errorAct != null)
                        _errorAct(request.error);
                    yield break;
                }

                _successAct(request.downloadHandler.data);
            }
        }

        #region Utility
        public IEnumerator CheckInternetConnection(Action<bool> _act, string _url)
        {
            if (atWork)
                yield break;

            atWork = true;
            using (UnityWebRequest www = UnityWebRequest.Get(_url))
            {
                yield return www.Send();

                if (www.isNetworkError)
                    _act(false);
                _act(true);
            }
            atWork = false;
        }
        #endregion
    }
}
