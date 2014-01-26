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
using System.Data.Entity.Infrastructure;

namespace Eres.MVC.Controllers
{
    public class GradesController : Controller
    {
        private NTR2013Entities db = new NTR2013Entities();
        private RealisationsStorage realisationsStorage = new RealisationsStorage();
        private SubjectsStorage subjectsStorage = new SubjectsStorage();
        private SemestersStorage semestersStorage = new SemestersStorage();
        public GradesStorage gradesStorage = new GradesStorage();

        // GET: /Grades/
        public ActionResult Index()
        {
            if (TempData["RealisationId"] != null)
            {
                var grades = gradesStorage.getGrades().Where(g => g.RealisationID == (int)TempData["RealisationId"]).ToList();
                grades.Sort((x, y) => x.Name.CompareTo(y.Name));
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
                semestersList.Sort((x, y) => x.Name.CompareTo(y.Name));
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
                var realisation = realisationsStorage.getRealisationBySubjectAndSemester(subjectId, (int)TempData["SemesterId"]);
                TempData["RealisationId"] = realisation.RealisationID;
                return RedirectToAction("Index");
            }
            if (TempData["SemesterId"] != null)
            {   
                ViewBag.SubjectsList = subjectsStorage.getSubjectsBySemester((int)TempData["SemesterId"]).OrderBy(s => s.Name);
                TempData["SemesterId"] = TempData["SemesterId"];
                return View();
            }
           
            
            return RedirectToAction("SelectSemester");
        }

        // GET: /Grades/Create
        public ActionResult Create()
        {
            if (TempData["RealisationId"] == null)
                return RedirectToAction("SelectSemester");
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
                try
                {
                    gradesStorage.createGrade(grades.Name, grades.MaxValue, (int)TempData["RealisationId"]);
                }
                catch
                {
                    TempData["Error"] = "Error while creating grade";
                }
                return RedirectToAction("Index");
            }
            
            return View(grades);
        }

        // GET: /Grades/Edit/5
        public ActionResult Edit(int? id)
        {
            if (TempData["RealisationId"] == null)
                return RedirectToAction("SelectSemester");
            TempData["RealisationId"] = TempData["RealisationId"];

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Grades grades = db.Grades.Find(id);
            if (grades == null)
            {
                TempData["Error"] = "Grade not found";
                return RedirectToAction("Index");
            }
            return View(grades);
        }

        // POST: /Grades/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="GradeID,Name,MaxValue,TimeStamp")] Grades grades)
        {
            TempData["RealisationId"] = TempData["RealisationId"];
            if (ModelState.IsValid)
            {
                try
                {
                    grades.RealisationID = (int)TempData["RealisationId"];
                    gradesStorage.updateGrade(grades);
                }
                catch (DbUpdateConcurrencyException)
                {
                    TempData["Error"] = "Grade was already edited or deleted";
                }
                catch
                {
                    TempData["Error"] = "Error while editing grade.";
                }
                return RedirectToAction("Index");
            }
            //ViewBag.RealisationID = new SelectList(db.Realisations, "RealisationID", "RealisationID", grades.RealisationID);
            return View(grades);
        }

        // GET: /Grades/Delete/5
        public ActionResult Delete(int? id)
        {
            if (TempData["RealisationId"] == null)
                return RedirectToAction("SelectSemester");
            TempData["RealisationId"] = TempData["RealisationId"];

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Grades grades = db.Grades.Find(id);
            if (grades == null)
            {
                TempData["Error"] = "Grade not found";
                return RedirectToAction("Index");
            }
            return View(grades);
        }

        // POST: /Grades/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                gradesStorage.deleteGrade(id);
            }
            catch
            {
                TempData["Error"]="Error while deleting grade";
            }
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
