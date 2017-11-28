using System;
using System.Collections.Generic;

using System.IO;
using System.Net;

namespace PartialDownload.Library
{
    public class WebRequestHelper
    {
        public bool CheckServerSupportPartialContent(string _url)
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

        public long CheckFileSize(string _url)
        {
            long ContentLength = -1;
            HttpWebResponse resp = null;

            try
            {
                WebRequest req = HttpWebRequest.Create(_url);
                req.Method = "HEAD";

                resp = req.GetResponse() as HttpWebResponse;

                if (long.TryParse(resp.Headers.Get("Content-Length"), out ContentLength))
                {
                    //Do something useful with ContentLength here 
                    return ContentLength;
                }
            }
            catch (WebException e)
            {
                if (e.Status == WebExceptionStatus.ProtocolError)
                {
                    resp = (HttpWebResponse)e.Response;
                    return -(long)resp.StatusCode;//Console.Write("Errorcode: {0}", (int)resp.StatusCode);
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

        public Stream DownloadParts(string _url, int _start)
        {
            HttpWebRequest myHttpWebRequest = (HttpWebRequest)WebRequest.Create(_url);
            myHttpWebRequest.AddRange(_start, _start + 1023);

            HttpWebResponse myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse();

            return myHttpWebResponse.GetResponseStream();
        }

        //public byte[] DownloadPartsBytes(string _url, int _start)
        //{
        //    System.IO.Stream stream = DownloadParts(_url, _start);

        //    using (MemoryStream ms = new MemoryStream())
        //    {
        //        //stream.CopyTo(ms);

        //        return ms.ToArray();
        //    }
        //}
    }
}
