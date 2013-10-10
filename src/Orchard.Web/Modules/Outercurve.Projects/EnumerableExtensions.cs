using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using Orchard.ContentManagement;
using Orchard.Localization;

namespace Outercurve.Projects
{
    public static class EnumerableExtensions
    {

        public static string ToLocalDateString(this DateTime? dateTime) {
            return dateTime.HasValue ? dateTime.Value.ToLocalTime().ToString("d") : null;
        }

        public static void AddModelError<T,TProperty>(this IUpdateModel update, T model, Expression<Func<T, TProperty>> property, LocalizedString message) {
            if (property.IsProperty()) {
                update.AddModelError(property.GetPropertyName(), message);
            }
            else
                throw new Exception("BAD BAD BAD");

        }
    }
}