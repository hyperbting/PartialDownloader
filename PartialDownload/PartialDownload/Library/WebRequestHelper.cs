using System;
using System.Collections.Generic;

using System.Net;

namespace PartialDownload.Library
{
    public class WebRequestHelper
    {
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
    }
}
