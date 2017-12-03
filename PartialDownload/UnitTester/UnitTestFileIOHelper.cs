using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PartialDownloadManager.Library;

namespace UnitTester
{
    [TestClass]
    public class UnitTestFileIOHelper
    {
        [TestMethod]
        public void TestMethodCheckLocalFileSize()
        {
            FileIOHelper fileIOHelper = new FileIOHelper();

            string path = "d:\\.testfile";
            int afile = fileIOHelper.CheckFileSize(path);
            Console.WriteLine(afile);//not exist
            Assert.AreEqual<int>(-1, afile);

            fileIOHelper.Touch(path);
            afile = fileIOHelper.CheckFileSize(path);
            Console.WriteLine();//exist
            Assert.AreEqual<int>(0, afile);
            fileIOHelper.Remove(path);

            path = @"d:\123.546";
            int nonExistFile = fileIOHelper.CheckFileSize(path);
            Console.WriteLine(nonExistFile);//File not exist
            Assert.AreEqual<int>(-1, nonExistFile);
        }

        [TestMethod]
        public void TestMethodAppendTo()
        {
            FileIOHelper fileIOHelper = new FileIOHelper();
            byte[] bytess = new byte[] { 1,2,3,4,5,6,7};
            string filepath = @"d:\123.546";

            int fileSize = fileIOHelper.CheckFileSize(filepath);
            fileIOHelper.AppendTo(filepath, bytess);
            Console.WriteLine("Append to d:\\123.546 from " + fileSize);
            Assert.AreEqual<int>( fileSize+7, fileIOHelper.CheckFileSize(filepath) );

            System.IO.Stream st = new System.IO.MemoryStream(new byte[] { 5, 4, 3, 2, 1 });
            fileSize = fileIOHelper.CheckFileSize(filepath);
            fileIOHelper.AppendTo(filepath, st);
            Console.WriteLine("Append to d:\\123.546");
            Assert.AreEqual<int>(fileSize + 5, fileIOHelper.CheckFileSize(filepath));
        }

        [TestMethod]
        public void TestMethodTouch()
        {
            FileIOHelper fileIOHelper = new FileIOHelper();
            string filepath = @"d:\212.224";

            fileIOHelper.Remove(filepath);
            Assert.AreEqual<bool>(false, fileIOHelper.CheckFileExist(filepath));

            Assert.AreEqual<bool>(false, fileIOHelper.CheckFileExist(filepath));
            fileIOHelper.Touch(filepath);

            Assert.AreEqual<bool>(true, fileIOHelper.CheckFileExist(filepath));

            fileIOHelper.Remove(filepath);
            Assert.AreEqual<bool>(false, fileIOHelper.CheckFileExist(filepath));

        }
    }
}
