using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CRM_TTV.Controllers
{
    
    public class DDLController : Controller
    {
        private CRM_TTVEntities db = new CRM_TTVEntities();
        // GET: DDL/tbSpecialized/5
        //[Route("DDL/tbSpecialized/{idEducation}")]
        public ActionResult tbSpecialized(int id)
        {
            var tbSpecialized = db.tbSpecializeds.Where(x=>x.idEducation == id);
            string option = "";
            foreach (var item in tbSpecialized)
            {
                option += "<option value=" + item.idSpecialized + ">" + item.name + "</option>";
            }
            //return View(await tbSciences.ToListAsync());
            return Content(option);
        }
        public ActionResult tbServicePack(int id)
        {
            var tbServicePack = db.tbServicePacks.Where(x => x.idSpecialized == id);
            string option = "";
            foreach (var item in tbServicePack)
            {
                option += "<option value=" + item.idSpecialized + ">" + item.name + "</option>";
            }
            return Content(option);
        }
    }
}
