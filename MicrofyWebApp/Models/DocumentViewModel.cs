using System;
using System.Collections.Generic;
using System.IO;

namespace MicrofyWebApp.Models
{
    public class DocumentViewModel
    {
        public string id { get; set; }
        public List<DocumentRepos> documentRepository { get; set; }
        public string selectedPhase { get; set; }
        public string uploadedBy { get; set; }
        public DateTime uploadedOn { get; set; }
        public string selectedSubPhases { get; set; }
        public string selectedDocName { get; set; }
        public string UserRole { get; set; }
    }

    public class DocumentRepos
    {
        public string phase { get; set; }
        public List<Documents> documents { get; set; }
        public List<SubPhase> subphase { get; set; }
    }
    public class Documents
    {
        public string name { get; set; }
        public string description { get; set; }
        public string url { get; set; }
        //public List<Dictionary<string,string>> Tags { get; set; }
        public List<string> tags { get; set; }
        public string filetype
        {
            get
            {
                string extension = string.Empty;
                extension = Path.GetExtension(url);
                string icons = string.Empty;
                if (extension == null) { return ""; }
                if (extension.Equals(".doc") || extension.Equals(".docm") || extension.Equals(".docx"))
                {
                    icons = "fa-file-word";
                }
                else if (extension.Equals(".xlsx") || extension.Equals(".xlsm") || extension.Equals(".xls"))
                {
                    icons = "fa-file-excel";
                }
                else if (extension.Equals(".pptm") || extension.Equals(".pptx") || extension.Equals(".ppsx") || extension.Equals(".ppt"))
                {
                    icons = "fa-file-powerpoint";
                }
                else if (extension.Equals(".pdf"))
                {
                    icons = "fa-file-pdf";
                }
                else if (extension.Equals(".csv"))
                {
                    icons = "fa-file-csv";
                }
                else if (extension.Equals(".gif"))
                {
                    icons = "fa-file-image";
                }
                else if (extension.Equals(".png"))
                {
                    icons = "fa-file-image";
                }
                else if (extension.Equals(".jpg") || extension.Equals(".jpeg"))
                {
                    icons = "fa-file-image";
                }
                else
                {
                    icons = "fa-file-alt";
                }
                return icons;
            }

        }
    }
    public class SubPhase
    {
        public string name { get; set; }
        public List<SubDocument> documents { get; set; }
    }
    public class SubDocument
    {
        public string name { get; set; }
        public string description { get; set; }
        public string url { get; set; }
        //public List<Dictionary<string,string>> Tags { get; set; }
        public List<string> tags { get; set; }
        public string filetype
        {
            get
            {
                string extension = string.Empty;
                extension = Path.GetExtension(url);
                string icons = string.Empty;
                if (extension == null) { return ""; }
                if (extension.Equals(".doc") || extension.Equals(".docm") || extension.Equals(".docx"))
                {
                    icons = "fa-file-word";
                }
                else if (extension.Equals(".xlsx") || extension.Equals(".xlsm") || extension.Equals(".xls"))
                {
                    icons = "fa-file-excel";
                }
                else if (extension.Equals(".pptm") || extension.Equals(".pptx") || extension.Equals(".ppsx") || extension.Equals(".ppt"))
                {
                    icons = "fa-file-powerpoint";
                }
                else if (extension.Equals(".pdf"))
                {
                    icons = "fa-file-pdf";
                }
                else if (extension.Equals(".csv"))
                {
                    icons = "fa-file-csv";
                }
                else if (extension.Equals(".gif"))
                {
                    icons = "fa-file-image";
                }
                else if (extension.Equals(".png"))
                {
                    icons = "fa-file-image";
                }
                else if (extension.Equals(".jpg") || extension.Equals(".jpeg"))
                {
                    icons = "fa-file-image";
                }
                else
                {
                    icons = "fa-file-alt";
                }
                return icons;
            }

        }
    }
    public class CreateDocuments
    {
        public string Phase { get; set; }
        public string SubPhase { get; set; }
        public string DocumentName { get; set; }
        public string Description { get; set; }
        public string URL { get; set; }
        //public List<Dictionary<string,string>> Tags { get; set; }
        public List<string> Tags { get; set; }


    }
    public class FileUploadResponse
    {
        public bool statuscode { get; set; }
        public string filename { get; set; }
        public string url { get; set; }
        public bool isDuplicate { get; set; }
        public string errorMessage { get; set; }
    }
}
