using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PartialDownloader.Library;

namespace UnitTester
{
    [TestClass]
    public class UnitTestWebReqHelper
    {
        public string url = "https://s3-ap-northeast-1.amazonaws.com/hooloop360travelmap/hooloop360travelmap/Video/taidong/taidong_%E5%8E%9F%E5%A7%8B%E9%83%A8%E8%90%BD%E5%B1%B1%E5%9C%B0%E7%BE%8E%E9%A3%9F.mp4";
        public string url2 = "https://scholar.princeton.edu/sites/default/files/oversize_pdf_test_0.pdf";
        public string url2_2 = "https://scholar.princeton.edu/sites/default/files/oversize_pdf_test_0.pdff";
        public string url3 = "https://downloads.dell.com/FOLDER04561408M/1/Dell-Digital-Delivery-Application_8FNF1_WIN_3.4.1002.0_A00.E";
        public string url4 = "https://cdn.wallpaper.com/main/2017_in_pictures_wallpaper_01.jpg";

        [TestMethod]
        public void TestMethodCheckFileSize()
        {
            WebRequestHelper wrh = new WebRequestHelper();
            int fsize = wrh.CheckFileSize(url2);
            Console.WriteLine(fsize);
            Assert.AreEqual(101688487, fsize);

            int fsize2 = wrh.CheckFileSize(url2_2);
            Console.WriteLine(fsize2);
            Assert.AreEqual(-404, fsize2);

            int fsize3 = wrh.CheckFileSize(url4);
            Console.WriteLine(fsize3);
            Assert.AreEqual(122047, fsize3);
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

            bool serverSupport04 = WebRequestHelper.CheckServerSupportPartialContent(url4);
            Assert.AreEqual(true, serverSupport04);
            Console.WriteLine(serverSupport04);
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
