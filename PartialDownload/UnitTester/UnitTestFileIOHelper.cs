using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PartialDownload.Library;

namespace UnitTester
{
    [TestClass]
    public class UnitTestFileIOHelper
    {
        [TestMethod]
        public void TestMethodCheckLocalFileSize()
        {
            FileIOHelper fileIOHelper = new FileIOHelper();
            Console.WriteLine(fileIOHelper.CheckFileSize("C:\\Users/User/.bash_history"));
            Console.WriteLine(fileIOHelper.CheckFileSize("c:\\123.546"));
        }
    }
}
