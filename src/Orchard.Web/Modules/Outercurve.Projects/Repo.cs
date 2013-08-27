using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Autofac;
using NHibernate.Linq;
using Orchard.Data;

namespace Outercurve.Projects
{
    public class MyRepository<T> : Repository<T> where T : class
    {
        public MyRepository(ISessionLocator sessionLocator)
            : base(sessionLocator)
        {
        }

        public override IQueryable<T> Table
        {
            get { return Session.Query<T>(); }
        }
    }

    public class MyModule : Module {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterGeneric(typeof(MyRepository<>)).As(typeof(IRepository<>)).InstancePerDependency();
        }
    }

}