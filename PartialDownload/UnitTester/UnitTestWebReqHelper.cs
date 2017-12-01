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
            int fsize = wrh.CheckFileSize("https://scholar.princeton.edu/sites/default/files/oversize_pdf_test_0.pdf");
            Console.WriteLine(fsize);
            Assert.AreEqual(101688487, fsize);

            int fsize2 = wrh.CheckFileSize("https://scholar.princeton.edu/sites/default/files/oversize_pdf_test_0.pdffffffff");
            Console.WriteLine(fsize2);
            Assert.AreEqual(-404, fsize2);
        }

        [TestMethod]
        public void TestMethodCheckServerSupportPartialContent()
        {
            bool serverSupport01 = WebRequestHelper.CheckServerSupportPartialContent("https://s3-ap-northeast-1.amazonaws.com/hooloop360travelmap/hooloop360travelmap/Video/taidong/taidong_%E5%8E%9F%E5%A7%8B%E9%83%A8%E8%90%BD%E5%B1%B1%E5%9C%B0%E7%BE%8E%E9%A3%9F.mp4");
            Assert.AreEqual(true, serverSupport01);
            Console.WriteLine(serverSupport01);

            bool serverSupport02 = WebRequestHelper.CheckServerSupportPartialContent("https://s3-ap-northeast-1.amazonaws.com");
            Assert.AreEqual(false, serverSupport02);
            Console.WriteLine(serverSupport02);

            bool serverSupport03 = WebRequestHelper.CheckServerSupportPartialContent("https://scholar.princeton.edu/sites/default/files/oversize_pdf_test_0.pdf");
            Assert.AreEqual(true, serverSupport03);
            Console.WriteLine(serverSupport03);
        }


        [TestMethod]
        public void TestMethodCheckForInternetConnection()
        {
            for (int i = 0; i < 5; i++)
                Console.WriteLine(WebRequestHelper.CheckForInternetConnection("https://www.google.com"));
            Console.WriteLine("OK");
        }
    }
}
