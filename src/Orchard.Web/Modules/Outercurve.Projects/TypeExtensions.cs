using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;


namespace Outercurve.Projects
{
    public static class TypeExtensions
    {
        public static bool IsRequired<TModel, TProperty>(this HtmlHelper<TModel> html, Expression<Func<TModel, TProperty>> expression) {
            var property = ExpressionHelper.GetExpressionText(expression);
            return IsRequired(html, property);
        }

        public static bool IsRequired<TModel>(this HtmlHelper<TModel> html, string property) {
            return typeof (TModel).GetProperties().Any(p => p.Name == property && Attribute.IsDefined(p, typeof (RequiredAttribute)));
        }
    }
}