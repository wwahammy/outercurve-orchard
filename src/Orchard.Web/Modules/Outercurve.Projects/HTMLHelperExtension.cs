using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Outercurve.Projects.Services;

namespace Outercurve.Projects
{
    public static class HTMLHelperExtensions
    {
        public static IEnumerable<SelectListItem> ToSelectListItems(this HtmlHelper helper, object list, string selectedValue) {
            return helper.ToSelectListItems(list as IEnumerable<SelectListEntry>, selectedValue);
        }

        public static IEnumerable<SelectListItem> ToSelectListItems(this HtmlHelper helper, IEnumerable<SelectListEntry> list, string selectedValue) {
            return list.Select(l => new SelectListItem {Text = l.Name, Value = l.Id, Selected = l.Id == selectedValue});
        }
    }
}