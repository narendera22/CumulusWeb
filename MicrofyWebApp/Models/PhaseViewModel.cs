using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicrofyWebApp.Models
{
    public class PhaseViewModel
    {
        public List<phases> phases { get; set; }
    }
    public class phases
    {
        public string name { get; set; }
        public List<string> subPhase { get; set; }
    }
}
