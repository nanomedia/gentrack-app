using GentrackAppLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace GentrackApp.Tests.UnitTests
{
    [TestClass]
    public class StartProcessTest
    {
        [TestMethod]
        public void StartProcessSuccess()
        {
            var currentDirectory = Environment.CurrentDirectory;
            var currentFilePath = Path.Combine(currentDirectory, $"testfile.xml");

            //var currentFilePath = Path.Combine(currentDirectory, $"testfileWithIssues.xml");

            DocumentProcessor.currentFilePath = currentFilePath;
            DocumentProcessor.destinationPath = "C:\\GentrackFiles";
            var response = DocumentProcessor.Start();
            Assert.AreEqual(ProcessResponseEnum.success, response);
        }

        [TestMethod]
        public void ValidateFile()
        {
            var currentDirectory = Environment.CurrentDirectory;
            var currentFilePath = Path.Combine(currentDirectory, $"testfile.xml");
            //var currentFilePath = Path.Combine(currentDirectory, $"testfileWithIssues.xml");

            DocumentProcessor.currentFilePath = currentFilePath;
            var response = DocumentProcessor.validateXmlDocument();
            Assert.AreEqual(true, response);
        }
    }
}