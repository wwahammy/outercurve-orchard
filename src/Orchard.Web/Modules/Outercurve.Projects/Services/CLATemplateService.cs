using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard;
using Orchard.ContentManagement;
using Outercurve.Projects.Models;
using Outercurve.Projects.ViewModels;
using Outercurve.Projects.ViewModels.Parts;

namespace Outercurve.Projects.Services
{
    public interface ICLATemplateService : IDependency {
        void UpdateCLATemplatePart(ContentItem item, EditCLATemplateViewModel model);
        string CreateCLATemplateIdVersion(IContent item);
        string CreateCLATemplateIdVersion(int id, int version);
        ContentItem GetCLATemplateFromIdVersion(string idVersion);
    }

    public class CLATemplateService : ICLATemplateService
    {
        private readonly IContentManager _contentManager;

        public CLATemplateService(IContentManager contentManager) {
            _contentManager = contentManager;
        }

        public void UpdateCLATemplatePart(ContentItem item, EditCLATemplateViewModel model) {
            var part = item.As<CLATemplatePart>();

            part.CLA = model.CLA;
            part.CLATitle = model.Title;
        }

        public ContentItem GetCLATemplateFromIdVersion(string idVersion) {
            try {
                if (idVersion == null) {
                    return _contentManager.Query("CLATemplate").List().FirstOrDefault();
                }
                else {
                    var items = idVersion.Split('_');
                    var id = int.Parse(items[1]);
                    var version = int.Parse(items[2]);
                    return _contentManager.Get(id, VersionOptions.Number(version));    
                }
                
            }
            catch (Exception) {

                return null;
            }
            
        }

        public string CreateCLATemplateIdVersion(IContent item) {
            if (item == null || item.ContentItem == null)
                return null;

            return CreateCLATemplateIdVersion(item.Id, item.ContentItem.Version);
        }

        public string CreateCLATemplateIdVersion(int id, int version) {
            return "_" + id + "_" + version;
        }
    }


}