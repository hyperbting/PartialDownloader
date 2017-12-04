using System;
using System.IO;
using System.Net;

namespace PartialDownloader.Library
{
    public class WebRequestHelper
    {
        public static SSLSupporter ssls = new SSLSupporter();

        public WebRequestHelper()
        {
            ServicePointManager.ServerCertificateValidationCallback = ssls.MyRemoteCertificateValidationCallback;
        }

        public static bool CheckForInternetConnection( string _path)
        {
            try
            {
                using (var client = new WebClient())
                {
                    using (client.OpenRead(_path))
                    {
                        return true;
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        public static bool CheckServerSupportPartialContent(string _url)
        {
            string ContentRanges = "";
            HttpWebResponse resp = null;

            try
            {
                WebRequest req = HttpWebRequest.Create(_url);
                req.Method = "HEAD";

                resp = req.GetResponse() as HttpWebResponse;
                ContentRanges = resp.Headers.Get("Accept-Ranges");

                if (!string.IsNullOrEmpty(ContentRanges))
                {
                    //Do something useful with ContentLength here 
                    if(ContentRanges.Contains("bytes"))
                    return true;
                }
            }
            catch (WebException e)
            {
                if (e.Status == WebExceptionStatus.ProtocolError)
                {
                    resp = (HttpWebResponse)e.Response;
                    return false;//Console.Write("Errorcode: {0}", (int)resp.StatusCode);
                }
                else
                {
                    Console.Write("Error: {0}", e.Status);
                }
            }
            finally
            {
                if (resp != null)
                {
                    resp.Close();
                }
            }
            return false;
        }

        public int CheckFileSize(string _url)
        {
            int ContentLength = -1;
            HttpWebResponse resp = null;

            try
            {
                WebRequest req = HttpWebRequest.Create(_url);
                req.Method = "HEAD";

                resp = req.GetResponse() as HttpWebResponse;

                if (Int32.TryParse(resp.Headers.Get("Content-Length"), out ContentLength))
                {
                    return ContentLength;
                }
            }
            catch (WebException e)
            {
                if (e.Status == WebExceptionStatus.ProtocolError)
                {
                    resp = (HttpWebResponse)e.Response;
                    return -(int)resp.StatusCode;//Console.Write("Errorcode: {0}", (int)resp.StatusCode);
                }
                else
                {
                    Console.Write("Error: {0}", e.Status);
                }
            }
            finally
            {
                if (resp != null)
                {
                    resp.Close();
                }
            }
            return -1;
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="_resultStreamAct"></param>
        ///// <param name="_url"></param>
        ///// <param name="_start"></param>
        ///// <param name="_windowSize">1048576 = 1MB</param>
        ///// <returns></returns>
        //public IEnumerator DownloadParts(Action<Stream> _resultStreamAct, string _url, int _start, int _windowSize = 1048576)
        //{
        //    //ServicePointManager.ServerCertificateValidationCallback = ssls.MyRemoteCertificateValidationCallback;

        //    HttpWebRequest myHttpWebRequest = (HttpWebRequest)WebRequest.Create(_url);
        //    myHttpWebRequest.AddRange(_start, _start + _windowSize - 1);

        //    bool locked = true;
        //    myHttpWebRequest.BeginGetResponse(
        //        new AsyncCallback(
        //            (IAsyncResult result) =>
        //            {
        //                HttpWebResponse response = (result.AsyncState as HttpWebRequest).EndGetResponse(result) as HttpWebResponse;
        //                _resultStreamAct(response.GetResponseStream());
        //                locked = false;
        //            }
        //        ), myHttpWebRequest);

        //    while (locked)
        //    {
        //        yield return null;
        //    }
        //}

        [System.Obsolete("Non-Async will hurt performance, use Async instead")]
        public Stream DownloadParts(string _url, int _start, int _windowSize = 1024)
        {
            HttpWebRequest myHttpWebRequest = (HttpWebRequest)WebRequest.Create(_url);
            myHttpWebRequest.AddRange(_start, _start + _windowSize - 1);

            //this will hurt performance
            HttpWebResponse myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse();

            return myHttpWebResponse.GetResponseStream();
        }        
    }
}
