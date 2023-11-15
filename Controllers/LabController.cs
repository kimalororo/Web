using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using LB2and3.Models.Entities;
using LB2and3.Models.ViewModels;
using System.Security.Cryptography;
using System.Web.Security;
using System.Diagnostics;
using System.Text;

namespace LB2and3.Controllers
{
    public class LabController : Controller
    {
        // GET: Lab
        [AllowAnonymous]
        public ActionResult StudentsList()
        {
            List<Students> student = new List<Students>();
            using (var db = new Entities())
            {
                student = db.Students.OrderBy(x => x.LastName)
                    .ThenBy(x => x.FirstName)
                    .ThenBy(x => x.BIrthDate)
                    .ThenBy(x => x.InsertDateTime)
                    .ThenBy(x => x.WakeUpTime).ToList();
            }
            return View(student);
        }

        [HttpGet]
        [Authorize]
        public ActionResult StudentDetails(Guid studentId)
        {
            Students model = new Students();
            using (var db = new Entities())
            {
                model = db.Students.Find(studentId);
            }
            return View(model);
        }


        [HttpGet]
        [Authorize(Roles = "monitor")]
        public ActionResult CreateStudent()
        {
            ViewBag.Genders = new SelectList(GetGendersList(), "Item1", "Item2");
            return View();
        }

        List<Tuple<string, string>> GetGendersList()
        {
            List<Tuple<string, string>> genders = new List<Tuple<string, string>>
            {
                new Tuple<string, string>("ж", "Женский"),
                new Tuple<string, string>("м", "Мужской")
            };
            return genders;
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult CreateStudent(StudentVM newStudent)
        {
            if (ModelState.IsValid)
            {
                using (var context = new Entities())
                {
                    Students student = new Students
                    {
                        StudentId = Guid.NewGuid(),
                        LastName = newStudent.LastName,
                        FirstName = newStudent.FirstName,
                        Patronymic = newStudent.Patronymic,
                        Gender = newStudent.Gender,
                        BIrthDate = newStudent.BIrthDate
                    };
                    context.Students.Add(student);
                    context.SaveChanges();
                }
                return RedirectToAction("StudentsList");
            }
            ViewBag.Genders = new SelectList(GetGendersList(), "Item1", "Item2");
            return View(newStudent);
        }

        [HttpGet]
        [Authorize(Roles = "monitor")]
        public ActionResult EditStudent(Guid studentId)
        {
            StudentVM model;
            using (var context = new Entities())
            {
                Students students = context.Students.Find(studentId);
                model = new StudentVM
                {
                    StudentId = students.StudentId,
                    LastName = students.LastName,
                    FirstName = students.FirstName,
                    Patronymic = students.Patronymic,
                    Gender = students.Gender,
                    BIrthDate = students.BIrthDate,
                    InsertDateTime = students.InsertDateTime,
                    WakeUpTime = students.WakeUpTime
                };
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult EditStudent(StudentVM model)
        {
            if (ModelState.IsValid)
            {
                using (var context = new Entities())
                {
                    Students editedStudent = new Students
                    {
                        StudentId = model.StudentId,
                        LastName = model.LastName,
                        FirstName = model.FirstName,
                        Patronymic = model.Patronymic,
                        Gender = model.Gender,
                        BIrthDate = model.BIrthDate,
                        InsertDateTime = model.InsertDateTime,
                        WakeUpTime = model.WakeUpTime
                    };
                    context.Students.Attach(editedStudent);
                    context.Entry(editedStudent).State = System.Data.Entity.EntityState.Modified;
                    context.SaveChanges();
                }
                return RedirectToAction("StudentsList");
            }
            return View(model);
        }

        
        [HttpGet]
        [Authorize(Roles = "monitor")]
        public ActionResult DeleteStudent(Guid studentId)
        {
            Students studentToDelete;
            using (var context = new Entities())
            {
                studentToDelete = context.Students.Find(studentId);
            }
            return View(studentToDelete);
        }

        [HttpPost, ActionName("DeleteStudent")]
        public ActionResult DeletePost(Guid studentId)
        {
            using (var context = new Entities())
            {
                Students studentToDelete = context.Students.Find(studentId);
                context.Students.Remove(studentToDelete);
                context.SaveChanges();
            }
            return RedirectToAction("StudentsList");
        }

        [AllowAnonymous]
        [ChildActionOnly]
        public ActionResult GetHobbie(Guid studentId)
        {
            string message = "";
            using (var context = new Entities())
            {
                int hobbie = context.Students.Find(studentId).StudentHobbies.Count;
                if (hobbie != 0)
                    message = $"Количество хобби у студента: {hobbie}.";
                else
                    message = "У студента нет хобби";
            }
            return PartialView("GetHobbie", message);
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(UserVm webUser)
        {
            if (ModelState.IsValid)
            {
                using (Entities context = new Entities())
                {
                    User user = null;
                    user = context.User.Where(u => u.Login == webUser.Login).FirstOrDefault();
                    if (user != null)
                    {
                        string passwordHash = ReturnHashCode(webUser.Password + user.Salt.ToString().ToUpper());
                        if (passwordHash == user.PasswordHash)
                        {
                            string userRole = "";
                            switch (user.UserRole)
                            {
                                case 1:
                                    userRole = "default";
                                    break;
                                case 2:
                                    userRole = "monitor";
                                    break;
                            }

                            FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(
                                1,
                                user.Login,
                                DateTime.Now,
                                DateTime.Now.AddDays(1),
                                false,
                                userRole);

                            string encryptedTicket = FormsAuthentication.Encrypt(authTicket);
                            HttpContext.Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket));
                            return RedirectToAction("StudentsList", "Lab");
                        }
                    }
                }
            }
            ViewBag.Error = "Польлзователь не найден. Попробуйте еще раз";
            return View(webUser);
        }

        string ReturnHashCode(string loginAndSalt)
        {
            string hash = "";
            using (SHA1 sha1Hash = SHA1.Create()) 
            {
                byte[] data = sha1Hash.ComputeHash(Encoding.UTF8.GetBytes(loginAndSalt));
                StringBuilder sBuilder = new StringBuilder();
                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }
            hash = sBuilder.ToString().ToUpper();
            }
            return hash;
        }
        [Authorize]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("StudentsList", "Lab");
        }
    }
}