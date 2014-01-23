using System;
using System.Collections.Generic;
using System.Data;
using EresData.Storage;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using EresData;

namespace Eres.MVC.Controllers
{
    public class GradesController : Controller
    {
        private NTR2013Entities db = new NTR2013Entities();
        private RealisationsStorage realisationsStorage = new RealisationsStorage();
        private SubjectsStorage subjectsStorage = new SubjectsStorage();
        private SemestersStorage semestersStorage = new SemestersStorage();
        private GradesStorage gradesStorage = new GradesStorage();

        // GET: /Grades/
        public ActionResult Index()
        {
            if (TempData["SubjectId"] != null && TempData["SemesterId"] != null)
            {
               var grades = gradesStorage.getGradesBySemesterAndSubject((int)TempData["SemesterId"], (int)TempData["SubjectId"]).ToList();
               TempData["RealisationId"] = realisationsStorage.getRealisationBySubjectAndSemester((int)TempData["SubjectId"], (int)TempData["SemesterId"]).RealisationID;
               return View(grades);
            }
            if (TempData["RealisationId"] != null)
            {
                var grades = gradesStorage.getGrades().Where(g => g.RealisationID == (int)TempData["RealisationId"]).ToList();
                TempData["RealisationId"] = TempData["RealisationId"];
                return View(grades);
            }
            return RedirectToAction("SelectSemester");
        }

        public ActionResult SelectSemester(string SemesterId)
        {
            int semesterId;
            if (!int.TryParse(SemesterId, out semesterId))
            {
                var semestersList = semestersStorage.getSemesters();
                ViewBag.SemesterId = new SelectList(semestersList, "SemesterID", "Name");
                return View();
            }
            TempData["SemesterId"] = semesterId;
            return RedirectToAction("SelectSubject");
        }

        public ActionResult SelectSubject(string SubjectId)
        {
            int subjectId;
            if (!String.IsNullOrEmpty(SubjectId) && int.TryParse(SubjectId, out subjectId))
            {
                TempData["SemesterId"] = TempData["SemesterId"];
                TempData["SubjectId"] = subjectId;
                return RedirectToAction("Index");
            }
            if (TempData["SemesterId"] != null)
            {          
                ViewBag.SubjectsList = subjectsStorage.getSubjectsBySemester((int)TempData["SemesterId"]);
                TempData["SemesterId"] = TempData["SemesterId"];
                return View();
            }
           
            
            return RedirectToAction("SelectSemester");
        }
        // GET: /Grades/Details/5
        //public ActionResult Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Grades grades = db.Grades.Find(id);
        //    if (grades == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(grades);
        //}

        // GET: /Grades/Create
        public ActionResult Create()
        {
            TempData["RealisationId"] = TempData["RealisationId"];
            return View();
        }

        // POST: /Grades/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="Name,MaxValue")] Grades grades)
        {
            TempData["RealisationId"] = TempData["RealisationId"];

            if (ModelState.IsValid)
            {
                //db.Grades.Add(grades);
                //db.SaveChanges();
                return RedirectToAction("Index");
            }
            
            return View(grades);
        }

        // GET: /Grades/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Grades grades = db.Grades.Find(id);
            if (grades == null)
            {
                return HttpNotFound();
            }
            ViewBag.RealisationID = new SelectList(db.Realisations, "RealisationID", "RealisationID", grades.RealisationID);
            return View(grades);
        }

        // POST: /Grades/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="GradeID,RealisationID,Name,MaxValue,TimeStamp")] Grades grades)
        {
            if (ModelState.IsValid)
            {
                db.Entry(grades).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.RealisationID = new SelectList(db.Realisations, "RealisationID", "RealisationID", grades.RealisationID);
            return View(grades);
        }

        // GET: /Grades/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Grades grades = db.Grades.Find(id);
            if (grades == null)
            {
                return HttpNotFound();
            }
            return View(grades);
        }

        // POST: /Grades/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Grades grades = db.Grades.Find(id);
            db.Grades.Remove(grades);
            db.SaveChanges();
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
