using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CRM_TTV;

namespace CRM_TTV.Controllers
{
    
    public class RegionController : Controller
    {
        private CRM_TTVEntities db = new CRM_TTVEntities();

        // GET: Region
        public async Task<ActionResult> Index()
        {
            return View(await db.tbRegions.ToListAsync());
        }

        // GET: Region/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbRegion tbRegion = await db.tbRegions.FindAsync(id);
            if (tbRegion == null)
            {
                return HttpNotFound();
            }
            return View(tbRegion);
        }

        // GET: Region/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Region/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "idRegion,parentID,name,sort,del,icon")] tbRegion tbRegion)
        {
            if (ModelState.IsValid)
            {
                db.tbRegions.Add(tbRegion);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(tbRegion);
        }

        // GET: Region/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbRegion tbRegion = await db.tbRegions.FindAsync(id);
            if (tbRegion == null)
            {
                return HttpNotFound();
            }
            return View(tbRegion);
        }

        // POST: Region/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "idRegion,parentID,name,sort,del,icon")] tbRegion tbRegion)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tbRegion).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(tbRegion);
        }

        // GET: Region/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbRegion tbRegion = await db.tbRegions.FindAsync(id);
            if (tbRegion == null)
            {
                return HttpNotFound();
            }
            return View(tbRegion);
        }

        // POST: Region/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            tbRegion tbRegion = await db.tbRegions.FindAsync(id);
            db.tbRegions.Remove(tbRegion);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
