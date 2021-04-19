using System;
using System.Collections.Generic;
using System.Text;

namespace customerportalapi.Entities
{
    public class Document
    {
        public string Content { get; set; }
        public string FileName { get; set; }
        public int FileSize { get; set; }
        public byte[] File { get; set; }
        public string FileExtension { get; set; }
        public DocumentMetadata Metadata { get; set; }
        public string ContentType { get; set; }
    }
}
