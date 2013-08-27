using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Orchard.ContentManagement;
using Orchard.Localization;

namespace Outercurve.Projects.Controllers
{
    public class VeryLowLevelBaseController : Controller, IUpdateModel
    {
        protected Localizer T { get; set; }

        public VeryLowLevelBaseController() {
            T = NullLocalizer.Instance;
        }

        bool IUpdateModel.TryUpdateModel<TModel>(TModel model, string prefix, string[] includeProperties, string[] excludeProperties)
        {
            return base.TryUpdateModel(model, prefix, includeProperties, excludeProperties);
        }

        public void AddModelError(string key, LocalizedString errorMessage)
        {
            ModelState.AddModelError(key, errorMessage.ToString());
        }
    }
}