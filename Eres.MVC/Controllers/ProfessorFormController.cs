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
    public class ProfessorFormController : Controller
    {
        private GradeValuesStorage gradeValuesStorage = new GradeValuesStorage();
        private SemestersStorage semestersStorage = new SemestersStorage();
        private SubjectsStorage subjectsStorage = new SubjectsStorage();
        private RealisationsStorage realisationsStorage = new RealisationsStorage();
        private GradesStorage gradesStorage = new GradesStorage();

        [HttpGet]
        public ActionResult SelectSemester()
        {
            var semesters = semestersStorage.getSemesters();
            ViewBag.SemesterId = new SelectList(semesters, "SemesterId", "Name");
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

            TempData["SemesterId"] = semesterId;
            return RedirectToAction("SelectSubject");

        }

        [HttpGet]
        public ActionResult SelectSubject()
        {
            if (TempData["SemesterId"] == null)
                return RedirectToAction("SelectSemester");

            var subjects = subjectsStorage.getSubjectsBySemester((int)TempData["SemesterId"]);
            ViewBag.SubjectId = new SelectList(subjects, "SubjectId", "Name");
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
            TempData["RealisationId"] = realisation.RealisationID;

            return RedirectToAction("SelectGrade");
        }

        [HttpGet]
        public ActionResult SelectGrade()
        {
            if (TempData["RealisationId"] == null)
                return RedirectToAction("SelectSemester");

            var grades = gradesStorage.getGradesByRealisation((int)TempData["RealisationId"]);
            ViewBag.GradeId = new SelectList(grades, "GradeId", "Name");
            return View();
        }

        [HttpPost]
        public ActionResult SelectGrade(string GradeId)
        {
            if (String.IsNullOrEmpty(GradeId))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            return RedirectToAction("ProfessorView", "GradesValues", new { GradeId = GradeId });
        }
	}
}