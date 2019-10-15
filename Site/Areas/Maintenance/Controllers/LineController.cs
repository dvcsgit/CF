using Models.Maintenance.Line;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Site.Areas.Maintenance.Controllers
{
    public class LineController : Controller
    {
        // GET: Maintenance/Line
        public ActionResult Index()
        {
            return View(new QueryFormModel());
        }
    }
}