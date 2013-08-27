using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Outercurve.Projects.Services;
using Outercurve.Projects.ViewModels;

namespace Outercurve.Projects.Controllers
{
    public class SimpleController : Controller
    {
        private readonly IExtendedUserPartService _extUserService;

        public SimpleController(IExtendedUserPartService extUserService) {
            _extUserService = extUserService;
        }

        public ActionResult AddOwner() {

            SetAllUsers();
            return View("MultipleLeaderRowPartial", (object)new MultipleLeaderRowViewModel());

        }

        protected void SetAllUsers()
        {
            var users = _extUserService.GetExtendedUserListEntries().ToList();
            this.ViewBag.AllUsers = users;
        }
    }
}