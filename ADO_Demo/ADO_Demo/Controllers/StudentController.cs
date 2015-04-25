using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ADO_Demo.BAL;
using Newtonsoft.Json;

namespace ADO_Demo.Controllers
{
    public class StudentController : Controller
    {
        // GET: ADO_Demo
        public ActionResult Index()
        {
            return View(Student_BAL.GetUserAll());

            //return Json(Student_BAL.GetUserAll(), JsonRequestBehavior.AllowGet);
        }

        //public JsonResult FetchStudents()
        //{
        //    return Json(Student_BAL.GetUserAll(), JsonRequestBehavior.AllowGet);
        //}

        //public JsonResult FetchStudents()
        //{
        //    return JsonConvert.SerializeObject(Student_BAL.GetUserAll());
        //}

        // GET: ADO_Demo/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ADO_Demo/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ADO_Demo/Create
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

        // GET: ADO_Demo/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ADO_Demo/Edit/5
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

        // GET: ADO_Demo/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ADO_Demo/Delete/5
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
