using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using NHibernate.Criterion;
using Orchard.ContentManagement;
using Outercurve.Projects.Models;

namespace Outercurve.Projects.Helpers
{
    public static class ContentQueryExtensions {

       
        public static IContentQuery<TPart> WhereCLAPartRecordIsValid<TPart>(this IContentQuery<TPart> query) where TPart : IContent {
            return query.Where(CLAPartRecord.IsValidExpression);
        }
    }
}