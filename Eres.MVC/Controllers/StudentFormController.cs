using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EresData;
using EresData.Storage;
using System.Net;

namespace Eres.MVC.Controllers
{
    public class StudentFormController : Controller
    {
        private GradeValuesStorage gradeValuesStorage = new GradeValuesStorage();
        private SemestersStorage semestersStorage = new SemestersStorage();
        private SubjectsStorage subjectsStorage = new SubjectsStorage();
        private StudentsStorage studentsStorage = new StudentsStorage();
        private RegistrationsStorage registrationsStorage = new RegistrationsStorage();
        private RealisationsStorage realisationsStorage = new RealisationsStorage();

        public ActionResult SelectStudent(int? StudentId)
        {
            if (StudentId == null)
            {
                var students = studentsStorage.getStudents();
                return View(students);
            }
            TempData["StudentId"] = StudentId;
            return RedirectToAction("SelectSemester");
        }

        [HttpGet]
        public ActionResult SelectSemester()
        {
            if (TempData["StudentId"] == null)
                return RedirectToAction("SelectStudent");

            var semesters = semestersStorage.getSemestersByStudent((int)TempData["StudentId"]);
            ViewBag.SemesterId = new SelectList(semesters, "SemesterId", "Name");
            TempData["StudentId"] = TempData["StudentId"];
            return View();
        }

        [HttpPost]
        public ActionResult SelectSemester(string SemesterId)
        {
            int semesterId;
            if (String.IsNullOrEmpty(SemesterId) || !int.TryParse(SemesterId, out semesterId))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (TempData["StudentId"] == null)
                return RedirectToAction("SelectStudent");

            TempData["StudentId"] = TempData["StudentId"];
            TempData["SemesterId"] = semesterId;
            return RedirectToAction("SelectSubject");

        }

        [HttpGet]
        public ActionResult SelectSubject()
        {
            if (TempData["StudentId"] == null || TempData["SemesterId"] == null)
                return RedirectToAction("SelectStudent");

            var subjects = subjectsStorage.getSubjectsByStudentAndSemester((int)TempData["StudentId"], (int)TempData["SemesterId"]);
            ViewBag.SubjectId = new SelectList(subjects, "SubjectId", "Name");
            TempData["StudentId"] = TempData["StudentId"];
            TempData["SemesterId"] = TempData["SemesterId"];
            return View();
        }

        [HttpPost]
        public ActionResult SelectSubject(string SubjectId)
        {
            int subjectId;
            if (String.IsNullOrEmpty(SubjectId) || !int.TryParse(SubjectId, out subjectId))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var realisation = realisationsStorage.getRealisationBySubjectAndSemester(subjectId, (int)TempData["SemesterId"]);
            var registration = registrationsStorage.getRegistrationByRealisationAndStudent(realisation.RealisationID, (int)TempData["StudentId"]);
            return RedirectToAction("StudentView", "GradesValues", new { RegistrationId = registration.RegistrationID });
        }
	}
}