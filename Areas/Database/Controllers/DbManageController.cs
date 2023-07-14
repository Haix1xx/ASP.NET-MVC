using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace MVC.Areas.Database.Controllers
{
    [Area("Database")]
    [Route("database/{action=Index}")]
    public class DbManageController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}