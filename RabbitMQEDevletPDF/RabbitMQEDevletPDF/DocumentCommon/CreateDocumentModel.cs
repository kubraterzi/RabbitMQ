using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentCommon
{
    public class CreateDocumentModel
    {
        public int UserId { get; set; }
        public string Url { get; set; }
        public DocumentTypes DocumentType { get; set; }          
    }

    public enum DocumentTypes
    {
        Pdf,
        Html,
        Xlsx,
        Png
    }
}
