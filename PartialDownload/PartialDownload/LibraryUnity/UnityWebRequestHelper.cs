using PartialDownload.Library;
using System;
using System.Collections;
using System.IO;
using System.Net;
using UnityEngine.Networking;

namespace PartialDownload.LibraryUnity
{
    public class UnityWebRequestHelper
    {
        public SSLSupporter ssls = new SSLSupporter();

        /// <summary>
        /// UnityWebRequest Version of get header
        /// </summary>
        /// <param name="_url"></param>
        /// <param name="_resultAct"></param>
        /// <returns></returns>
        public IEnumerator CheckFileSize(string _url, Action<int> _resultAct)
        {
            using (UnityWebRequest www = UnityWebRequest.Head(_url))
            {
                yield return www.Send();

                if (www.isNetworkError || www.isHttpError)
                {
                    _resultAct(-1);
                }
                else
                {
                    string result = www.GetResponseHeader("Content-Length");
                    string ContentRanges = www.GetResponseHeader("Accept-Ranges");
                    _resultAct(int.Parse(result));
                }
            }
        }        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_resultStreamAct"></param>
        /// <param name="_url"></param>
        /// <param name="_start"></param>
        /// <param name="_windowSize">1048576 = 1MB</param>
        /// <returns></returns>
        public IEnumerator DownloadParts(Action<Stream> _resultStreamAct, string _url, int _start, int _windowSize = 1048576)
        {
            ServicePointManager.ServerCertificateValidationCallback = ssls.MyRemoteCertificateValidationCallback;

            HttpWebRequest myHttpWebRequest = (HttpWebRequest)WebRequest.Create(_url);
            myHttpWebRequest.AddRange(_start, _start + _windowSize - 1);
            yield return null;

            bool locked = true;
            myHttpWebRequest.BeginGetResponse(
                new AsyncCallback(
                    (IAsyncResult result) => {
                        HttpWebResponse response = (result.AsyncState as HttpWebRequest).EndGetResponse(result) as HttpWebResponse;
                        _resultStreamAct( response.GetResponseStream() );
                        locked = false;
                    }
                ), myHttpWebRequest);

            while (locked)
            {
                yield return null;
            }
        }
    }
}
