using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace UserProfileMgmtService.Controllers
{
    public class ProfileViewController : Controller
    {
        // GET: ProfileView
        public ActionResult ProfileDisplay()
        {
            return View();
        }

        // GET: ProfileView/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ProfileView/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ProfileView/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: ProfileView/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ProfileView/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: ProfileView/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ProfileView/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
