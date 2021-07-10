using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicrofyWebApp.Pages
{
    public class MicrofyModel : PageModel
    {
        private readonly ILogger<MicrofyModel> _logger;

        public MicrofyModel(ILogger<MicrofyModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {

        }
    }
}
