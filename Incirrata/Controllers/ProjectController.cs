using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Incirrata.DataLayer;
using Microsoft.AspNet.Identity;
using Incirrata.Models;
using PagedList;

namespace Incirrata.Controllers
{
    public class ProjectController : Controller
    {
        private dbTask db = new dbTask();
        // GET: Project

        [Authorize(Roles ="Administrator, Project Manager")]
        public ActionResult Index(int? page)
        {
            ViewBag.Project = "class = active";

            IEnumerable<ProjectViewModel> projects =
                from project in db.Projects
                orderby project.CreatedAt descending
                select new ProjectViewModel
                {
                    Id = project.Id,
                    Code = project.Code,
                    CreatedAt = project.CreatedAt,
                    CurrentProjectManager = project.AspNetUser.Name + " " + project.AspNetUser.Surname,
                    Name = project.Name,

                    Tasks = (from item in db.Tasks
                             where item.ProjectId == project.Id
                             select new TaskViewModel
                             {
                                 Progress = item.Progress
                             }).ToList()
                };

            int pageSize = 6;
            int pageNumber = (page ?? 1);
            return View(projects.ToPagedList(pageNumber, pageSize));
        }


        // GET: Project/Create
        [Authorize(Roles = "Administrator, Project Manager")]
        public ActionResult Create()
        {
            ViewBag.Project = "class = active";

            var role = db.AspNetRoles.SingleOrDefault(r => r.Name == "Project Manager");
            var se = role.AspNetUsers;
            ProjectViewModel model = new ProjectViewModel
            {
                ProjectManagers = se.ToList()
            };
            return View(model);
        }

        // POST: Project/Create
        [HttpPost]
        [Authorize(Roles = "Administrator, Project Manager")]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                var name = collection["Name"];
                var code = collection["Code"];
                var pm = collection["project-manager"];

                //check if project with project code already exists
                var checkProject = db.Projects.Where(p => p.Code == code).ToArray();
                if(checkProject.Length > 0)
                {
                    Session["error"] = "Project with this code already exists.";
                    return RedirectToAction("Index");
                }


                //check if pm created project
                if(User.IsInRole("Project Manager"))
                {
                    pm = User.Identity.GetUserId();
                }
                else
                {
                    //check if pm exists
                    var pmExists = db.AspNetUsers.Find(pm);
                    if (pmExists is null)
                    {
                        //return filled form with previous data
                        var role = db.AspNetRoles.SingleOrDefault(r => r.Name == "Project Manager");
                        var se = role.AspNetUsers;
                        ProjectViewModel model = new ProjectViewModel
                        {
                            Name = name,
                            Code = code,
                            ProjectManagers = se.ToList()
                        };
                        Session["error"] = "Chose project manager";
                        return View(model);
                    }
                }

                var project = new Project
                {
                    Id = Guid.NewGuid().ToString(),
                    Code = code,
                    Name = name,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    ProjectManager = pm
                };

                db.Projects.Add(project);
                db.SaveChanges();

                Session["msg"] = "Successfully created project.";
                return RedirectToAction("Index");
            }
            catch(Exception e)
            {
                
                Session["error"] = "Something went wrong with the database, please try later.";
                return RedirectToAction("Index");
            }
        }

        // GET: Project/Edit/5
        [Authorize(Roles = "Administrator, Project Manager")]
        public ActionResult Edit(string id)
        {
            // get project 
            var project = db.Projects.Find(id);

            if(project is null)
            {
                Session["error"] = "Invalid ID parameter";
                return RedirectToAction("Index");
            }

            //get all pms for admin view
            var role = db.AspNetRoles.SingleOrDefault(r => r.Name == "Project Manager");
            ProjectViewModel model = new ProjectViewModel
            {
                Id = project.Id,
                Name = project.Name,
                Code = project.Code,
                ProjectManagers = role.AspNetUsers.ToList(),
                CurrentProjectManagerId = project.ProjectManager
            };
            return View(model);
        }

        // POST: Project/Edit/5
        [HttpPost]
        [Authorize(Roles = "Administrator, Project Manager")]
        public ActionResult Edit(string id, FormCollection collection)
        {
            try
            {
                // get all values from request form
                var name = collection["Name"];
                var code = collection["Code"];
                var pm = collection["project-manager"];
                var project = db.Projects.Find(id);

                // check if project exists
                if(project is null)
                {
                    Session["error"] = "Project does not exists";
                    return RedirectToAction("Index");
                }

                if (project.Code != code) //check for project's unique code 
                {
                    var checkCode = db.Projects.Where(p => p.Code == code).ToArray();
                    if(checkCode.Length > 0)
                    {
                        Session["error"] = "Project with this code already exists";
                        return RedirectToAction("Index");
                    }
                }

                if (User.IsInRole("Administrator"))
                {
                    //update project if user is admin
                    project.Code = code;
                    project.Name = name;
                    project.UpdatedAt = DateTime.Now;
                    project.ProjectManager = pm;
                }
                else
                {
                    //update project if user is pm
                    project.Code = code;
                    project.Name = name;
                    project.UpdatedAt = DateTime.Now;
                }

                //save project to db
                db.SaveChanges();

                Session["msg"] = "Successfully updated project";
                return RedirectToAction("Index");
            }
            catch(Exception e)
            {
                Session["error"] = "Something went wrong with the database, please try later.";
                return RedirectToAction("Index");
            }
        }

        // GET: Project/Delete/5
        [Authorize(Roles = "Administrator")]
        public ActionResult Delete(string id)
        {
            // find project and remove it
            var project = db.Projects.Find(id);
            if(project is null)
            {
                Session["error"] = "Invalid ID parameter!";
                return RedirectToAction("Index");
            }
            db.Projects.Remove(project);
            db.SaveChanges();
            Session["msg"] = "Successfully deleted project";
            return RedirectToAction("Index");
        }
    }
}
