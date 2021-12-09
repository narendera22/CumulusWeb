using System;
using System.Collections.Generic;
using System.IO;
using System.Web;

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
        public List<SearchResult> searchResults { get; set; }
    }

    public class DocumentRepos
    {
        public string phase { get; set; }
        public List<Documents> documents { get; set; }
        public List<SubPhase> subphase { get; set; }
        public string icon
        {
            get
            {
                string icons = string.Empty;
                if (phase == "Pre - Plan")
                {
                    //icons = "fa fa-list";
                    icons = "strategy.png";
                }
                else if (phase == "Plan")
                {
                    //icons = "fa fa-tasks";
                    icons = "plan.png";
                }
                else if (phase == "Ready")
                {
                    //icons = "fa fa-check";
                    icons = "ready.png";
                }
                else if (phase == "Adopt")
                {
                    //icons = "fa fa-cogs";
                    icons = "govern.png";
                }
                //else
                //{
                //    icons = "fa fa-file-alt";
                //}
                return icons;
            }
        }
        public string dashboardTag
        {
            get
            {
                string tag = string.Empty;
                if (phase == "Pre - Plan")
                {
                    tag = "SoW Template,  Roles & Responsibilities, Risk Register, High Level Plan, RACI Matrix etc.";
                }
                else if (phase == "Plan")
                {
                    tag = "Task List, Detail Plan, Discovery questionnaire, Reference Architecture, Guidelines etc.";
                }
                else if (phase == "Ready")
                {
                    tag = "Design Documents – Infrastructure, Containerization, Logging & Monitoring, DevOps etc.";
                }
                else if (phase == "Adopt")
                {
                    tag = "Refactoring for Containerization, Basic Building Blocks for Containerization and Orchestration etc.";
                }
                else
                {
                    tag = "";
                }
                return tag;
            }
        }
    }
    public class Documents
    {
        public string name { get; set; }
        public string description { get; set; }
        public string url { get; set; }
        public string Decodedurl { get { return HttpUtility.UrlDecode(url); } }
        //public List<Dictionary<string,string>> Tags { get; set; }
        public List<Tag> tags { get; set; }

        public string filetype
        {
            get
            {
                string extension = string.Empty;
                extension = Path.GetExtension(Decodedurl);
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
        public string filename { get { return Path.GetFileName(Decodedurl); } }
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
        public string Decodedurl { get { return HttpUtility.UrlDecode(url); } }
        //public List<Dictionary<string,string>> Tags { get; set; }
        public List<Tag> tags { get; set; }
        public string filetype
        {
            get
            {
                string extension = string.Empty;
                extension = Path.GetExtension(Decodedurl);
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
        public string filename { get { return Path.GetFileName(Decodedurl); } }

    }
    public class CreateDocuments
    {
        public string Phase { get; set; }
        public string SubPhase { get; set; }
        public string DocumentName { get; set; }
        public string Description { get; set; }
        public string URL { get; set; }
        public List<Tag> Tags { get; set; }
        //public List<string> Tags { get; set; }


    }
    public class FileUploadResponse
    {
        public bool statuscode { get; set; }
        public string filename { get; set; }
        public string url { get; set; }
        public string Encodedurl { get { return HttpUtility.UrlEncode(url); } }
        public bool isDuplicate { get; set; }
        public string errorMessage { get; set; }
    }

    public class ActivityTracker
    {
        public string UserName { get; set; }
        public string ActivityType { get; set; }
        public string ActivityDetails { get; set; }
    }
    public class Tag
    {
        public List<Dictionary<string, string>> AzureService { get; set; }
        public List<Dictionary<string, string>> ProcessRelated { get; set; }
        public List<Dictionary<string, string>> ManagementRelated { get; set; }
    }

    public class SearchResult
    {
        public string name { get; set; }
        public string uploadedOn { get; set; }
        public string highlightedContent { get; set; }
        public string description { get; set; }
        public string documentName { get; set; }
        public string phase { get; set; }
        public string subPhase { get; set; }
        public string documentUrl { get; set; }
        public string filetype
        {
            get
            {
                string extension = string.Empty;
                extension = Path.GetExtension(documentUrl);
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
        public string filename { get { return Path.GetFileName(documentUrl); } }
        public List<Dictionary<string, string>> Tags { get; set; }
    }
    public class UpdateMetadata
    {
        public string filename { get; set; }
        public string displaytags { get; set; }
        public string usertags { get; set; }
        public string phase { get; set; }
        public string subphase { get; set; }
        public string Description { get; set; }
    }
    public class DeleteDoc
    {
        public string filename { get; set; }
        public string documentname { get; set; }
        public string phase { get; set; }
        public string subphase { get; set; }

    }
}
