using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard;

namespace Outercurve.Projects.Services
{
    public class CurrentMoreSiteSettingsWorkContext : IWorkContextStateProvider
    {
        private readonly IMoreSiteSettingsService _settings;

        public CurrentMoreSiteSettingsWorkContext(IMoreSiteSettingsService settings) {
            _settings = settings;
        }

        public Func<WorkContext, T> Get<T>(string name) {
            if (name == "MoreSiteSettings") {
                return ctx => (T) _settings.GetMoreSiteSettings();
            }
            return null;
        }
    }
}