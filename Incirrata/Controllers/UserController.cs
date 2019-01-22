using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Incirrata.DataLayer;
using Incirrata.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.Owin;

namespace Incirrata.Controllers
{
    [Authorize(Roles ="Administrator")]
    public class UserController : Controller
    {
        private ApplicationUserManager _userManager;
        private dbTask db = new dbTask();

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        // GET: User
        public ActionResult Index()
        {
            ViewBag.User = "class = active";
            //get all users from db
            IEnumerable<UserViewModel> users =
                from user in db.AspNetUsers
                where user.AspNetRoles.FirstOrDefault().Name != "Administrator"
                select new UserViewModel
                {
                    Id = user.Id,
                    Email = user.Email,
                    Name = user.Name,
                    Surname = user.Surname,
                    UserName = user.UserName,
                    CreatedAt = user.CreatedAt,
                    Role = user.AspNetRoles.FirstOrDefault().Name
                };
            

            return View(users);
        }


        // GET: User/Edit/5
        public ActionResult Edit(string id)
        {
            ViewBag.User = "class = active";

            var model = (from user in db.AspNetUsers
                        where user.Id == id
                        select new UserViewModel
                        {
                            Id = user.Id,
                            Email = user.Email,
                            Name = user.Name,
                            Surname = user.Surname,
                            UserName = user.UserName,
                            CreatedAt = user.CreatedAt,
                            Role = user.AspNetRoles.FirstOrDefault().Name,
                            Roles = (from role in db.AspNetRoles
                                     select new RoleViewModel
                                     {
                                         Id = role.Id,
                                         Name = role.Name
                                     }).ToList()
                        }).FirstOrDefault();
            return View(model);
        }

        // POST: User/Edit/5
        [HttpPost]
        public async Task<ActionResult> Edit(string id, FormCollection collection)
        {
            //get data from form
            var username = collection["UserName"];
            var name = collection["Name"];
            var surname = collection["Surname"];
            var email = collection["Email"];
            var role = collection["Role"];
            
            try
            {

                //get user and his old role
                var user = db.AspNetUsers.Find(id);
                var userOldRole = user.AspNetRoles.FirstOrDefault().Name;

                if (!role.Equals(userOldRole)) //check if role updated
                {
                    //check if current user has an opened project or task
                    if(role == "Developer")
                    {
                        //remove current developer from all tasks
                        var checkDev = db.Tasks.Where(t => t.UserId == id).ToList();
                        checkDev.ForEach(t => t.UserId = null);
                    }else if(role == "Project Manager")
                    {
                        // first update project then then role
                        var checkPM = db.Projects.Where(p => p.ProjectManager == id).ToArray();
                        if(checkPM.Length > 0)
                        {
                            Session["error"] = "There are still open projects with " + user.Name + " " + user.Surname;
                            return RedirectToAction("Index");
                        }
                    }

                    // update user role
                    await UserManager.RemoveFromRoleAsync(id, userOldRole);
                    await UserManager.AddToRoleAsync(id, role);
                }



                // update user
                user.UserName = username;
                user.Name = name;
                user.Surname = surname;
                user.Email = email;
                
                db.SaveChanges();

                Session["msg"] = "Successfully updated user.";
                return RedirectToAction("Index");
            }
            catch
            {
                Session["error"] = "Something went wrong with the database, try later!";
                return RedirectToAction("Index");
            }
        }

        // DELETE 
        public ActionResult Delete(string id)
        {
            //get user from  db
            var user = db.AspNetUsers.Find(id);
            var userRole = user.AspNetRoles.FirstOrDefault().Name;
            try
            {
                if(userRole.Equals("Developer"))
                {
                    var devTask = db.Tasks.Where(t => t.UserId == id).ToList();
                    if(devTask != null)
                    {
                        // remove developer from all projects
                        devTask.ForEach(t => t.UserId = null);
                    }
                }
                
                // remove user from database
                db.AspNetUsers.Remove(user);
                db.SaveChanges();

                Session["msg"] = "Succefully deleted user.";
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                Session["error"] = "There are still open projects with " +user.Name+ " " +user.Surname + ", update project first!";
                return RedirectToAction("Index");
            }
        }
    }
}
