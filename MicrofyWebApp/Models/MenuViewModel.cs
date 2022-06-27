using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicrofyWebApp.Models
{
    public class MenuViewModel
    {
        public List<Menu> Menu { get; set; }
        public List<DocumentRepos> documentRepository { get; set; }
        public string UserRole { get; set; }


    }
    public class Menu
    {
        public string Name { get; set; }

        public string Visible { get; set; }

        public List<MenuLevel1> MenuLevel1 { get; set; }
        public string icon
        {
            get
            {
                string icons = string.Empty;
                if (Name == "Repository")
                {
                    icons = "Repos.png";
                }
                else if (Name == "Application")
                {
                    icons = "Checklist.jpg";
                }
                else if (Name == "Activity")
                {
                    icons = "activity.png";
                }
                else if (Name == "Settings")
                {
                    icons = "Settings.jpg";
                }
                
                return icons;
            }
        }
    }
    public class MenuLevel1
    {
        public string Name { get; set; }

        public string Visible { get; set; }

        public List<MenuLevel2> MenuLevel2 { get; set; }
    }
    public class MenuLevel2
    {
        public string Name { get; set; }

        public string Visible { get; set; }

    }

}
