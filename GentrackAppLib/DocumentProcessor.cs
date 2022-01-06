using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace GentrackAppLib
{
    public class DocumentProcessor
    {
        public static string currentFilePath { get; set; }

        public static string destinationPath { get; set; }

        public static bool validateXmlDocument()
        {
            var isValid = true;
            var xdoc = XDocument.Load(currentFilePath);

            //RULE 1: A Header element
            #region Rule1

            var rule1 = xdoc.Descendants("Header").Any();
            #endregion

            //RULE 2: A Transactions element, containing 1 Transaction element
            #region Rule2
            var rule2 = xdoc.Descendants("Transactions")
                               .Elements("Transaction")
                               .Count() == 1;
            #endregion

            //RULE 3: A Transaction element will contain transactionDate and transactionID attributes
            #region Rule3
            var rule3 = (from ele in xdoc.Descendants("Transactions")
                            .Elements("Transaction")
                         where ele.Attribute("transactionDate") != null &&
                               ele.Attribute("transactionID") != null
                         select ele).Any();
            #endregion

            //RULE 4: Required data will be in
            //Transactions->Transaction->MeterDataNotification->CSVIntervalData
            #region rule4
            var rule4 = (xdoc.Descendants("Transactions")
                               .Elements("Transaction")
                               .Elements("MeterDataNotification")
                               .Elements("CSVIntervalData")).Any();

            #endregion

            if (!rule1 || !rule2 || !rule3 || !rule4)
            {
                isValid = false;
            }


            return isValid;
        }

        public static ProcessResponseEnum Start()
        {
            var response = ProcessResponseEnum.success;
            try
            {
                var xdoc = XDocument.Load(currentFilePath);
                var data = xdoc.Descendants("CSVIntervalData").First().Value;
                var lines = new List<string>();

                using (StringReader sr = new StringReader(data))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        lines.Add(line);
                    }
                    var docResponse = processingLines(lines);
                    generatingFiles(docResponse);
                }

            }
            catch (Exception)
            {
                response = ProcessResponseEnum.error;
            }

            return response;
        }

        private static CsvDocumentResponse processingLines(List<string> lines)
        {
            var docs = new List<CsvDocument>();
            var fileId = string.Empty;
            var docHeader = new CsvDocumentRow();
            var docTrailer = string.Empty;

            lines.ForEach(line =>
            {
                var lineData = line.Split(',');
                var delimiter = lineData[0];
                var rowData = lineData.Skip(1).ToArray();
                var header = new List<string>();

                switch (delimiter)
                {
                    case "100":
                        docHeader.columns = rowData;
                        break;
                    case "200":
                        var doc = new CsvDocument();
                        doc.fileId = Guid.NewGuid().ToString();
                        doc.fileName = $"{rowData[0]}.csv";
                        doc.mainRow = new CsvDocumentRow
                        {
                            columns = rowData.SkipLast(1).ToArray()
                        };
                        fileId = doc.fileId;
                        docs.Add(doc);
                        break;
                    case "300":
                        if (!string.IsNullOrEmpty(fileId))
                        {
                            var docFile = docs.FirstOrDefault(doc => doc.fileId.Equals(fileId));
                            if (docFile != null)
                                docFile.rows.Add(new CsvDocumentRow
                                {
                                    columns = rowData.SkipLast(1).ToArray()
                                });
                        }
                        break;
                    case "900":
                        docTrailer = line;
                        break;
                }
            });

            return new CsvDocumentResponse
            {
                docHeader = docHeader,
                docTrailer = docTrailer,
                docs = docs
            };
        }

        private static void generatingFiles(CsvDocumentResponse docResponse)
        {
            if (!Directory.Exists(destinationPath))
            {
                Directory.CreateDirectory(destinationPath);
            }

            if (docResponse.docs.Any())
            {
                var directoryName = Guid.NewGuid().ToString();
                var directoryPath = Path.Combine(destinationPath, directoryName);
                Directory.CreateDirectory(directoryPath);

                docResponse.docs.ForEach(doc =>
                {
                    using (var writer = new StreamWriter(Path.Combine(directoryPath, doc.fileName)))
                    {
                        writer.WriteLine(String.Join(",", docResponse.docHeader.columns));
                        writer.WriteLine(String.Join(",", doc.mainRow.columns));
                        doc.rows.ForEach(row =>
                        {
                            writer.WriteLine(String.Join(",", row.columns));
                        });
                        writer.WriteLine(docResponse.docTrailer);
                        writer.Flush();
                    }
                });
            }
        }
    }
}