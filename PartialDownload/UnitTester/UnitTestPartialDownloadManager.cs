using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PartialDownload;

namespace UnitTester
{
    [TestClass]
    public class UnitTestPartialDownloadManager
    {
        [TestMethod]
        public void TestMethodDownload()
        {
            PartialDownloadManager pdm = new PartialDownloadManager();

            pdm.Download();
        }

        [TestMethod]
        public void TestMethodDownloadFull()
        {
            PartialDownloadManager pdm = new PartialDownloadManager();

            pdm.Download2();
        }
    }
}
