using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PartialDownload.Library;

namespace UnitTester
{
    [TestClass]
    public class UnitTestWebReqHelper
    {
        [TestMethod]
        public void TestMethodCheckFileSize()
        {
            WebRequestHelper wrh = new WebRequestHelper();
            int fsize = wrh.CheckFileSize("https://s3-ap-northeast-1.amazonaws.com/hooloop360travelmap/hooloop360travelmap/Video/taidong/taidong_%E5%8E%9F%E5%A7%8B%E9%83%A8%E8%90%BD%E5%B1%B1%E5%9C%B0%E7%BE%8E%E9%A3%9F.mp4");

            Console.WriteLine(fsize);

            int fsize2 = wrh.CheckFileSize("https://s3-ap-northeast-1.amazonaws.com/hooloop360travelmap/hooloop360travelmap/Video/taidong/taidong_%E5%8E%9F%E5%A7%8B%E9%83%A8%E8%90%BD%E5%B1%B1%E5%9C%B0%E7%BE%8E%E9%A3%9F");

            Console.WriteLine(fsize2);
        }

        [TestMethod]
        public void TestMethodCheckServerSupportPartialContent()
        {
            WebRequestHelper wrh = new WebRequestHelper();
            bool fsize = wrh.CheckServerSupportPartialContent("https://s3-ap-northeast-1.amazonaws.com/hooloop360travelmap/hooloop360travelmap/Video/taidong/taidong_%E5%8E%9F%E5%A7%8B%E9%83%A8%E8%90%BD%E5%B1%B1%E5%9C%B0%E7%BE%8E%E9%A3%9F.mp4");

            Console.WriteLine(fsize);

            bool fsize2 = wrh.CheckServerSupportPartialContent("https://s3-ap-northeast-1.amazonaws.com/hooloop360travelmap/hooloop360travelmap/Android/Android");
            Console.WriteLine(fsize2);
        }
    }
}
