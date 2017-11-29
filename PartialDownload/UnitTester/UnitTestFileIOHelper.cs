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

            string url = "C:\\Users/User/.bash_history";
            Console.WriteLine(fileIOHelper.CheckFileSize(url));//exist
            url = @"C:\Users\User\.bash_history";
            Console.WriteLine(fileIOHelper.CheckFileSize(url));//exist
            url = @"c:\123.546";
            Console.WriteLine(fileIOHelper.CheckFileSize(url));//File not exist
        }

        [TestMethod]
        public void TestMethodAppendTo()
        {
            FileIOHelper fileIOHelper = new FileIOHelper();
            byte[] bytess = new byte[] { 1,2,3,4,5,6,7};

            foreach (var bytt in bytess)
                Console.Write(bytt);

            fileIOHelper.AppendTo(@"d:\123.546" , bytess);



        }
    }
}
