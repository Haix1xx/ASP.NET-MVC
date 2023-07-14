using Microsoft.AspNetCore.Mvc;
using MVC.Services;
using MVC.Models;

namespace MVC.Controllers
{
    [Route("planet")]
    public class PlanetController : Controller
    {
        private PlanetServices PlanetServices { get; set; }
        public PlanetController(PlanetServices planetServices)
        {
            PlanetServices= planetServices;
        }

        [Route("")]
        public IActionResult Index()
        {
            return View();
        }

        [Route("{id:int?}")]
        public IActionResult Detail(int? id)
        {
            if (id == null)
                return NotFound();
            Planet? planet = PlanetServices?.GetPlanet(id.Value);
            if (planet == null)
                return NotFound();


            return View(planet);
        }
    }
}
