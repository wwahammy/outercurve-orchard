using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard;
using Orchard.Caching;
using Orchard.ContentManagement;
using Orchard.Core.Settings.Models;
using Outercurve.Projects.Models;

namespace Outercurve.Projects.Services
{
    public interface IMoreSiteSettingsService : IDependency
    {
        IMoreSiteSettings GetMoreSiteSettings();
    }


    public class MoreSiteSettingsService : IMoreSiteSettingsService
    {
        private readonly ICacheManager _cacheManager;
        private readonly IContentManager _contentManager;

        public MoreSiteSettingsService(ICacheManager cacheManager, IContentManager contentManager) {
            _cacheManager = cacheManager;
            _contentManager = contentManager;
        }

        public IMoreSiteSettings GetMoreSiteSettings() {

            return null;
            //return _contentManager.Query<IMoreSiteSettings>();

        }
    }

   
}