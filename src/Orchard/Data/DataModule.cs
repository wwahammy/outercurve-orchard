﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac;
using Autofac.Builder;

namespace Orchard.Data {
    public class DataModule : Module {
        protected override void Load(ContainerBuilder builder) {
            builder.RegisterGeneric(typeof (Repository<>)).As(typeof (IRepository<>)).FactoryScoped();
            builder.Register<HackSessionLocator>().As<ISessionLocator>().ContainerScoped();
        }
    }
}