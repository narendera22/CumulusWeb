using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicrofyWebApp.Models
{
    public class UserViewModel
    {
        public string username { get; set; }
        public string password { get; set; }
        public string fullName { get; set; }
        public string userRole { get; set; }
        public bool isActive { get; set; }
        public List<projects> projects { get; set; }
        public bool StatusCode { get; set; }
        public string responseMessage { get; set; }
        public List<ListUserDetails> usersDetails { get; set; }
        public string DefaultPassword { get;  set; }
    }
    public class projects
    {
        public string projectName { get; set; }
        public string customerName { get; set; }
    }
    public class LoginDetails
    {
        public string UserId { get; set; }
        public string Password { get; set; }
    }
    public class ListUserDetails
    {
        public string username { get; set; }
        public string password { get; set; }
        public string fullName { get; set; }
        public string userRole { get; set; }
        public bool isActive { get; set; }
        public List<projects> projects { get; set; }
    }
}
