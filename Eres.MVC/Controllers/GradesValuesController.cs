using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using EresData;
using EresData.Storage;
using System.Data.Entity.Infrastructure;

namespace Eres.MVC.Controllers
{
    public class GradesValuesController : Controller
    {
        private NTR2013Entities db = new NTR2013Entities();
        private GradesStorage gradesStorage = new GradesStorage();
        private RegistrationsStorage registrationsStorage = new RegistrationsStorage();
        private StudentsStorage studentsStorage = new StudentsStorage();
        private GradeValuesStorage gradeValuesStorage = new GradeValuesStorage();

        // GET: /GradesValues/
        public ActionResult ProfessorView(string GradeId)
        {
            if (String.IsNullOrEmpty(GradeId))
                return RedirectToAction("SelectSemester", "ProfessorForm");
            int gradeId;
            if (!int.TryParse(GradeId, out gradeId))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var gradevalues = db.GradeValues.Where(g => g.GradeID == gradeId).Include(g => g.Registrations.Students).ToList().OrderBy(g => g.Registrations.Students.LastName);
            TempData["GradeId"] = gradeId;
            return View(gradevalues);
        }

     
        public ActionResult StudentView(string RegistrationId)
        {
            if (String.IsNullOrEmpty(RegistrationId))
                return RedirectToAction("SelectStudent", "StudentForm");
            int registrationId;
            if(!int.TryParse(RegistrationId, out registrationId))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var gradevalues = db.GradeValues.Where(g => g.RegistrationID == registrationId).Include(g => g.Grades).ToList();
            return View(gradevalues);
        }

        public ActionResult CreateSelectStudent(int? RegistrationId)
        {
            if (TempData["GradeId"] == null)
                return RedirectToAction("SelectSemester", "ProfessorForm");
            if (RegistrationId == null)
            {
                var registrations = registrationsStorage.getRegistrationsWithoutGrade((int)TempData["GradeId"]);
                registrations.Sort((x, y) => x.Students.LastName.CompareTo(y.Students.LastName));
                TempData["GradeId"] = TempData["GradeId"];
                return View(registrations);
            }
            TempData["GradeId"] = TempData["GradeId"];
            TempData["RegistrationId"] = RegistrationId;
            return RedirectToAction("Create");
        }

        // GET: /GradesValues/Create
        public ActionResult Create()
        {
            if (TempData["GradeId"] == null || TempData["RegistrationId"] == null)
                return RedirectToAction("SelectSemester", "ProfessorForm");

            TempData["GradeId"] = TempData["GradeId"];
            TempData["RegistrationId"] = TempData["RegistrationId"];
            return View();
        }

        // POST: /GradesValues/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="Value")] GradeValues gradevalues)
        {
            TempData["GradeId"] = TempData["GradeId"];

            if (ModelState.IsValid)
            {
                try
                {
                    gradeValuesStorage.createGradeValue(gradevalues.Value, (int)TempData["RegistrationId"], (int)TempData["GradeId"]);
                }
                catch (GradeValueFormatException exc)
                {
                    TempData["Error"] = exc.Message;
                }
                catch
                {
                    TempData["Error"] = "Error while creating grade value";
                }
                return RedirectToAction("ProfessorView", new { GradeId = TempData["GradeId"] });
            }

            return View(gradevalues);
        }

        // GET: /GradesValues/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GradeValues gradevalues = db.GradeValues.Find(id);
            if (gradevalues == null)
            {
                return HttpNotFound();
            }

            return View(gradevalues);
        }

        // POST: /GradesValues/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="GradeValueID,GradeID,RegistrationID,Value,TimeStamp")] GradeValues gradevalues)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    gradeValuesStorage.updateGradeValue(gradevalues);
                }
                catch (GradeValueFormatException exc)
                {
                    TempData["Error"] = exc.Message;
                }
                catch (DbUpdateConcurrencyException)
                {
                    TempData["Error"] = "Grade Value was already edited or deleted";
                }
                catch
                {
                    TempData["Error"] = "Error while editing grade value";
                }
                return RedirectToAction("ProfessorView", new { GradeId = gradevalues.GradeID.ToString() });
            }

            return View(gradevalues);
        }

        // GET: /GradesValues/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GradeValues gradevalues = db.GradeValues.Find(id);
            if (gradevalues == null)
            {
                return HttpNotFound();
            }
            return View(gradevalues);
        }

        // POST: /GradesValues/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                gradeValuesStorage.deleteGradeValue(id);
            }
            catch
            {
                TempData["Error"] = "Error while deleting grade value";
            }
            return RedirectToAction("ProfessorView", new { GradeId = TempData["GradeId"] });
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
