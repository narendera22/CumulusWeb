using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicrofyWebApp.Models
{
    public class ProjectViewModel
    {
        public string responseMessage { get; set; }

        public string ProjectName { get; set; }

        public string CustomerName { get; set; }

        public string Application { get; set; }

        public string UserId { get; set; }

        public List<AzureTech> AzureTechnologies { get; set; }
        public bool StatusCode { get;  set; }
    }

    public class AzureTech
    {
        public string ServiceName { get; set; }

    }
}

