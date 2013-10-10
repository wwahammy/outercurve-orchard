using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web;
using NHibernate.Hql.Ast.ANTLR;

namespace Outercurve.Projects
{
    /// <summary>
    /// FROM MOQ
    /// </summary>
    public static class ExpressionExtensions
    {
        
		/// <summary>
		/// Checks whether the body of the lambda expression is a property access.
		/// </summary>
		public static bool IsProperty(this LambdaExpression expression)
		{
			//Guard.NotNull(() => expression, expression);

			return IsProperty(expression.Body);
		}

		/// <summary>
		/// Checks whether the expression is a property access.
		/// </summary>
		public static bool IsProperty(this Expression expression)
		{
			//Guard.NotNull(() => expression, expression);

			var prop = expression as MemberExpression;

			return prop != null && prop.Member is PropertyInfo;
		}

        public static string GetPropertyName(this LambdaExpression expression) {
            if (IsProperty(expression.Body)) {
                return GetPropertyName(expression.Body);
            }

            return null;
        }

        public static string GetPropertyName(this Expression expression) {
            if (IsProperty(expression)) {
                try {

                    var prop = expression as MemberExpression;

                    var propInfo = prop.Member as PropertyInfo;
                    return propInfo.Name;
                }
                catch (Exception) {

                    return null;
                }
            }
            return null;
        }
    }
}