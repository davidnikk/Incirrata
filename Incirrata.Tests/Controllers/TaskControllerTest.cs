using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Incirrata;
using Incirrata.Controllers;
using Incirrata.DataLayer;
using Incirrata.Models;

namespace Incirrata.Tests.Controllers
{
    class MockHttpContext : HttpContextBase
    {
        private readonly IPrincipal user;

        public MockHttpContext(string username, string[] roles = null)
        {
            var identity = new GenericIdentity(username);
            var principal = new GenericPrincipal(identity, roles ?? new string[] { });
            user = principal;
        }

        public override IPrincipal User
        {
            get
            {
                return user;
            }
            set
            {
                base.User = value;
            }
        }
    }

    [TestClass]
    public class TaskControllerTest
    {
        [TestMethod]
        public void IndexTask()
        {
            // Arrange
            TaskController controller = new TaskController();
            controller.ControllerContext = new ControllerContext
            {
                Controller = controller,
                HttpContext = new MockHttpContext("fakeuser@example.com")
            };

            // Act
            ViewResult result = controller.Index(1) as ViewResult;
            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void IndexUser()
        {
            // Arrange
            var controller = new UserController();
            controller.ControllerContext = new ControllerContext
            {
                Controller = controller,
                HttpContext = new MockHttpContext("fakeuser@example.com")
            };

            // Act
            ViewResult result = controller.Index() as ViewResult;
            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void IndexProject()
        {
            // Arrange
            var controller = new ProjectController();
            controller.ControllerContext = new ControllerContext
            {
                Controller = controller,
                HttpContext = new MockHttpContext("fakeuser@example.com")
            };

            // Act
            ViewResult result = controller.Index(1) as ViewResult;
            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void EditTask()
        {
            // Arrange
            TaskController controller = new TaskController();
            // Act
            ViewResult result = controller.Edit("5b166886-0165-49b5-bb8e-5cc15e660bba") as ViewResult;
            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void TaskEditRedirect()
        {
            var controller = new TaskController();
            var result = (RedirectToRouteResult)controller.Edit("aasd9-dasda9das-fsaf");
            Assert.AreEqual("Index", result.RouteValues["action"]);
        }


        //public void FixEfProviderServicesProblem()
        //{
        //    //The Entity Framework provider type 'System.Data.Entity.SqlServer.SqlProviderServices, EntityFramework.SqlServer'
        //    //for the 'System.Data.SqlClient' ADO.NET provider could not be loaded. 
        //    //Make sure the provider assembly is available to the running application. 
        //    //See http://go.microsoft.com/fwlink/?LinkId=260882 for more information.


        //    var instance = System.Data.Entity.SqlServer.SqlProviderServices.Instance;
        //}
    }
}
