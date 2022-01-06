// See https://aka.ms/new-console-template for more information
using GentrackAppLib;

var currentDirectory = Environment.CurrentDirectory;
var currentFilePath = Path.Combine(currentDirectory, $"testfile.xml");

DocumentProcessor.currentFilePath= currentFilePath;
DocumentProcessor.destinationPath = "C:\\GentrackFiles";
DocumentProcessor.Start();

