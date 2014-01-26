using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Eres.MVC.Controllers;
using MvcContrib.TestHelper;
using System.Web.Mvc;
using EresData;
using EresData.Storage;

namespace Eres.Tests.Controllers
{
    [TestClass]
    public class GradeValuesControllerTests
    {
        TestControllerBuilder builder = new TestControllerBuilder();
        GradesValuesController gradeValuesController = new GradesValuesController();

        [TestInitialize]
        public void Initialize()
        {
            builder.InitializeController(gradeValuesController);
        }

        [TestMethod]
        public void CreateSelectStudentTest()
        {
            var result = gradeValuesController.CreateSelectStudent(1);
            result.AssertActionRedirect().ToController("ProfessorForm").ToAction("SelectSemester");

            gradeValuesController.TempData["GradeId"] = 1;
            result = gradeValuesController.CreateSelectStudent(null);
            result.AssertViewRendered().ForView("");

            result = gradeValuesController.CreateSelectStudent(1);
            result.AssertActionRedirect().ToAction("Create");
        }

        [TestMethod]
        public void CreateTest()
        {
            var result = gradeValuesController.Create();
            result.AssertActionRedirect().ToController("ProfessorForm").ToAction("SelectSemester");

            gradeValuesController.TempData["GradeId"] = 1;
            gradeValuesController.TempData["RegistrationId"] = 1;
            result = gradeValuesController.Create();
            result.AssertViewRendered().ForView("");
        }

        [TestMethod]
        public void ProfessorViewTest()
        {
            var result = gradeValuesController.ProfessorView(null);
            result.AssertActionRedirect().ToController("ProfessorForm").ToAction("SelectSemester");

            result = gradeValuesController.ProfessorView("1");
            result.AssertViewRendered().ForView("");
            Assert.AreEqual(1, gradeValuesController.TempData["GradeId"]);
        }

        [TestMethod]
        public void StudentViewTest()
        {
            var result = gradeValuesController.StudentView(null);
            result.AssertActionRedirect().ToController("StudentForm").ToAction("SelectStudent");

            result = gradeValuesController.StudentView("1");
            result.AssertViewRendered().ForView("");
        }
    }
}
