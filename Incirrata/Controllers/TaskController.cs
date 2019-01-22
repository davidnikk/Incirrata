using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Incirrata.DataLayer;
using Incirrata.Models;
using Microsoft.AspNet.Identity;
using System.Globalization;
using PagedList;

namespace Incirrata.Controllers
{

    [Authorize]
    public class TaskController : Controller
    {
        // GET: Task
        private dbTask db = new dbTask();
        public ActionResult Index(int? page)
        {
            ViewBag.Task = "class = active";

            //get user id to show only his tasks and not assigned tasks
            var userId = User.Identity.GetUserId();

            //get all tasks from db for developers and project manager
            if (!User.IsInRole("Administrator"))
            {
                IEnumerable<TaskViewModel> tasks =
                from task in db.Tasks
                where task.UserId == userId
                orderby task.CreatedAt descending
                select new TaskViewModel
                {
                    Id = task.Id,
                    Deadline = task.Deadline,
                    Status = task.Status,
                    Description = task.Description,
                    Progress = task.Progress,
                    CurrentProject = task.Project.Name,
                    CreatedAt = task.CreatedAt,
                    CurrentDeveloper = task.AspNetUser1.Name + " " + task.AspNetUser1.Surname
                };
                int pageSize = 6;
                int pageNumber = (page ?? 1);

                if (tasks.Count() < 1)
                {
                    tasks = from task in db.Tasks
                            where task.UserId == null
                            orderby task.CreatedAt descending
                            select new TaskViewModel
                            {
                                Id = task.Id,
                                Deadline = task.Deadline,
                                Status = task.Status,
                                Description = task.Description,
                                Progress = task.Progress,
                                CurrentProject = task.Project.Name,
                                CreatedAt = task.CreatedAt,
                                CurrentDeveloper = task.AspNetUser1.Name + " " + task.AspNetUser1.Surname
                            };
                }

                return View(tasks.ToPagedList(pageNumber, pageSize));
            }
            else //admin can view all tasks
            {
                IEnumerable<TaskViewModel> tasks =
                from task in db.Tasks
                orderby task.CreatedAt descending
                select new TaskViewModel
                {
                    Id = task.Id,
                    Deadline = task.Deadline,
                    Status = task.Status,
                    Description = task.Description,
                    Progress = task.Progress,
                    CurrentProject = task.Project.Name,
                    CreatedAt = task.CreatedAt,
                    CurrentDeveloper = task.AspNetUser1.Name + " " + task.AspNetUser1.Surname
                };
                int pageSize = 6;
                int pageNumber = (page ?? 1);
                return View(tasks.ToPagedList(pageNumber, pageSize));
            }
        }

        // GET: Task/Create
        [Authorize(Roles = "Administrator, Project Manager")]
        public ActionResult Create()
        {
            ViewBag.Task = "class = active";

            var model = new TaskViewModel();
            model.UsersDDL = new List<UserViewModelDDL>();

            // get all users with dev and manager role
            var developers = db.AspNetRoles.SingleOrDefault(r => r.Name == "Developer").AspNetUsers.ToList();
            var managers = db.AspNetRoles.SingleOrDefault(r => r.Name == "Project Manager").AspNetUsers.ToList();


            //map manager and developers in one list for ddl
            foreach (var userItem in developers)
            {
                var currentUser = new UserViewModelDDL
                {
                    Id = userItem.Id,
                    Name = userItem.Name,
                    Surname = userItem.Surname
                };
                model.UsersDDL.Add(currentUser);
            }

            // only admin can assign task to both user -> dev and project manager
            if (User.IsInRole("Administrator"))
            {
                foreach (var managerItem in managers)
                {
                    var currentUser = new UserViewModelDDL
                    {
                        Id = managerItem.Id,
                        Name = managerItem.Name,
                        Surname = managerItem.Surname
                    };
                    model.UsersDDL.Add(currentUser);
                }
            }

            // get all projects
            model.Projects =
                (from item in db.Projects
                 select new ProjectViewModel
                 {
                     Id = item.Id,
                     Name = item.Name
                 }).ToList();
            return View(model);
        }

        // POST: Task/Create
        [HttpPost]
        [Authorize(Roles = "Administrator, Project Manager")]
        public ActionResult Create(FormCollection collection)
        {
            var deadline = collection["Deadline"];
            var userId = collection["Developer"];
            var projectId = collection["Project"];
            var description = collection["Description"];
            var creatorId = User.Identity.GetUserId();

            DateTime date = new DateTime();
            //locale information
            CultureInfo provider = CultureInfo.InvariantCulture;

            try
            {
                //try to parse date
                date = DateTime.Parse(deadline, provider);
                // check date
                if (date < DateTime.Now)
                {
                    Session["error"] = "Date value must be greater then current";
                    return RedirectToAction("Create");
                }

            }
            catch (Exception e)
            {
                Session["error"] = "Wrong date format!";
                return RedirectToAction("Create");
            }



            //get user and project for validation
            var user = db.AspNetUsers.Find(userId);
            var project = db.Projects.Find(projectId);

            //get all tasks for current dev
            var devTasks = db.Tasks.Where(t => t.UserId == userId).ToArray();

            if (user is null) // set not assigned if user is not selected
            {
                userId = null;
            }
            if (project is null) // check if project exists
            {
                Session["error"] = "Choose project.";
                return RedirectToAction("Create");
            }
            if (devTasks.Length > 2) // check for assigned task number
            {
                Session["error"] = user.Name + " " + user.Surname + " has more than three tasks assigned.";
                return RedirectToAction("Create");
            }

            try
            {
                var newTask = new Task
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = userId,
                    Status = "New",
                    Description = description,
                    Progress = 0,
                    ProjectId = projectId,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    CreatorId = creatorId,
                    Deadline = date
                };

                db.Tasks.Add(newTask);
                db.SaveChanges();

                Session["msg"] = "Successfully created new task.";
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                Session["error"] = "Something went wrong with database, try later!";
                return RedirectToAction("Index");
            }
        }

        // GET: Task/Edit/5
        public ActionResult Edit(string id)
        {
            ViewBag.Task = "class = active";

            // find task by id
            var task = db.Tasks.Find(id);

            var model = new TaskViewModel();

            if (task is null)
            {
                return RedirectToAction("Index");
            }

            model.UsersDDL = new List<UserViewModelDDL>();

            // get all users with dev and manager role
            var developers = db.AspNetRoles.SingleOrDefault(r => r.Name == "Developer").AspNetUsers.ToList();
            var managers = db.AspNetRoles.SingleOrDefault(r => r.Name == "Project Manager").AspNetUsers.ToList();


            //map manager and developers in one list for ddl
            foreach (var userItem in developers)
            {
                var currentUser = new UserViewModelDDL
                {
                    Id = userItem.Id,
                    Name = userItem.Name,
                    Surname = userItem.Surname
                };
                model.UsersDDL.Add(currentUser);
            }

            foreach (var managerItem in managers)
            {
                var currentUser = new UserViewModelDDL
                {
                    Id = managerItem.Id,
                    Name = managerItem.Name,
                    Surname = managerItem.Surname
                };
                model.UsersDDL.Add(currentUser);
            }


            if (task.AspNetUser1 is null) //check if task isn't assigned
            {
                model.Id = task.Id;
                model.Deadline = task.Deadline;
                model.Description = task.Description;
                model.Progress = task.Progress;
                model.Status = task.Status;
            }
            else // if task is assigned to someone, return for selected item in dropdown list
            {
                model.Id = task.Id;
                model.Deadline = task.Deadline;
                model.Description = task.Description;
                model.Progress = task.Progress;
                model.Status = task.Status;
                model.CurrentDeveloper = task.AspNetUser1.Name + " " + task.AspNetUser1.Surname;
                model.CurrentDeveloperId = task.AspNetUser1.Id;
            }
            return View(model);
        }

        // POST: Task/Edit/5
        [HttpPost]
        public ActionResult Edit(string id, FormCollection collection)
        {

            // get all data from request

            var userId = collection["Developer"];
            var description = collection["Description"];

            // =======> if user is allowed to change status uncomment <======== \\
            // and comment automatic status change
            //var status = collection["Status"];
            // =======> end <======== \\

            string status;
            var progress = Convert.ToInt32(collection["Progress"]);
            DateTime date = new DateTime();

            //find task
            var task = db.Tasks.Find(id);

            // =======> automatic status change <======== \\
            // check for progress value and set correct  status according to value
            if (progress < 0)
            {
                Session["error"] = "Invalid progress value provided!";
                return RedirectToAction("Index");
            }
            else if (progress == 0)
            {
                status = "New";
            }
            else if (progress < 100)
            {
                status = "In progress";
            }
            else if (progress == 100)
            {
                status = "Finished";
            }
            else
            {
                Session["error"] = "Invalid progress value provided!";
                return RedirectToAction("Index");
            }
            // =======> end <======== \\

            if (!User.IsInRole("Developer"))
            {
                var deadline = collection["Deadline"];

                //locale information
                CultureInfo provider = CultureInfo.InvariantCulture;

                try
                {
                    //try to parse date
                    date = DateTime.Parse(deadline, provider);
                    // check date
                    if (date < DateTime.Now)
                    {
                        Session["error"] = "Date value must be greater then current";
                        return RedirectToAction("Index");
                    }

                }
                catch (Exception e)
                {
                    Session["error"] = "Wrong date format!";
                    return RedirectToAction("Index");
                }
            }
            else
            {
                date = task.Deadline;
                userId = task.UserId;
            }



            try
            {
                //check if dev parameter is 'not assigned'
                if (userId == null)
                {
                    // if 'not assigned' update 
                    task.Progress = progress;
                    task.Status = status;
                    task.UpdatedAt = DateTime.Now;
                    task.Deadline = date;
                    task.Description = description;
                    task.UserId = null;

                    // save changes to db and return success message
                    db.SaveChanges();
                    Session["msg"] = "Successfully updated task.";
                    return RedirectToAction("Index");
                }
                else
                {
                    //check if assigned to another dev
                    if (task.UserId != userId)
                    {
                        // check for number of tasks assigned to dev(MAX 3)
                        var devTasks = db.Tasks.Where(t => t.UserId == userId).ToArray();
                        if (devTasks.Length > 2)
                        {
                            Session["error"] = "This dev has more than three tasks assigned.";
                            return RedirectToAction("Edit/" + id);
                        }
                    }
                    if (task != null)
                    {
                        task.Progress = Convert.ToInt32(progress);
                        task.Status = status;
                        task.UpdatedAt = DateTime.Now;
                        task.Deadline = date;
                        task.UserId = userId;
                        task.Description = description;
                    }

                    // save changes to db and return success message
                    db.SaveChanges();
                    Session["msg"] = "Successfully updated task.";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception e)
            {
                Session["error"] = "Something went wrong with database, try later!";
                return RedirectToAction("Index");
            }
        }

        // GET: Task/Delete/5
        [Authorize(Roles = "Administrator")]
        public ActionResult Delete(string id)
        {
            var task = db.Tasks.Find(id);
            if (task != null)
            {
                try
                {
                    db.Tasks.Remove(task);
                    db.SaveChanges();
                    Session["msg"] = "Successfully removed task.";
                    return RedirectToAction("Index");
                }
                catch
                {
                    Session["error"] = "Something went wrong with the database, try later!";
                    return RedirectToAction("Index");
                }
            }
            else
            {
                Session["error"] = "Invalid ID parameter";
                return RedirectToAction("Index");
            }
        }
    }
}