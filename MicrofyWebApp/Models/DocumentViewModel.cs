using System;
using System.Collections.Generic;

namespace MicrofyWebApp.Models
{
    public class DocumentViewModel
    {
        public string id { get; set; }
        public List<DocumentRepos> documentRepository { get; set; }
        public string selectedPhase { get; set; }
        public string uploadedBy { get; set; }
        public DateTime uploadedOn { get; set; }
    }

    public class DocumentRepos
    {
        public string phase { get; set; }
        public List<Documents> documents { get; set; }
    }
    public class Documents{
        public string name { get; set; }
        public string description { get; set;}
        public string url { get; set;}
        //public List<Dictionary<string,string>> Tags { get; set; }
        public List<string> tags { get; set; }


    }
    public class CreateDocuments
    {
        public string phase { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string url { get; set; }
        //public List<Dictionary<string,string>> Tags { get; set; }
        public List<string> tags { get; set; }

    }
}
