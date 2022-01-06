using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GentrackAppLib
{
    public class CsvDocument
    {
        public CsvDocument()
        {
            this.rows = new List<CsvDocumentRow>();
        }

        public string fileId { get; set; }
        public string fileName { get; set; }
        public CsvDocumentRow mainRow { get; set; }
        public List<CsvDocumentRow> rows { get; set; }
    }

    public class CsvDocumentRow
    {
        public string[] columns { get; set; }
    }

    public class CsvDocumentResponse
    {        
        public CsvDocumentRow docHeader { get; set; }

        public string docTrailer { get; set; }

        public List<CsvDocument> docs { get; set; }
    }
}
