using System;
using System.Collections.Generic;

using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace PartialDownload.Library
{
    public class WebRequestHelper
    {
        public bool CheckServerSupportPartialContent(string _url)
        {
            ServicePointManager.ServerCertificateValidationCallback = MyRemoteCertificateValidationCallback;
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
            ServicePointManager.ServerCertificateValidationCallback = MyRemoteCertificateValidationCallback;

            try
            {
                WebRequest req = HttpWebRequest.Create(_url);
                req.Method = "HEAD";

                resp = req.GetResponse() as HttpWebResponse;

                if (Int32.TryParse(resp.Headers.Get("Content-Length"), out ContentLength))
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

        public Stream DownloadParts(string _url, int _start, int _windowSize = 1024)
        {
            ServicePointManager.ServerCertificateValidationCallback = MyRemoteCertificateValidationCallback;
            HttpWebRequest myHttpWebRequest = (HttpWebRequest)WebRequest.Create(_url);
            myHttpWebRequest.AddRange(_start, _start + _windowSize - 1);

            HttpWebResponse myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse();

            return myHttpWebResponse.GetResponseStream();
        }

        public bool MyRemoteCertificateValidationCallback(System.Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            bool isOk = true;
            // If there are errors in the certificate chain,
            // look at each error to determine the cause.
            if (sslPolicyErrors != SslPolicyErrors.None)
            {
                for (int i = 0; i < chain.ChainStatus.Length; i++)
                {
                    if (chain.ChainStatus[i].Status == X509ChainStatusFlags.RevocationStatusUnknown)
                    {
                        continue;
                    }
                    chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
                    chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
                    chain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan(0, 1, 0);
                    chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllFlags;
                    bool chainIsValid = chain.Build((X509Certificate2)certificate);
                    if (!chainIsValid)
                    {
                        isOk = false;
                        break;
                    }
                }
            }
            return isOk;
        }
    }
}
