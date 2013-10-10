using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DocumentFormat.OpenXml.Office.CustomUI;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Localization;
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
        bool Validate(EditCLATemplateViewModel model, IUpdateModel updater, IContent itemToUpdate);
    }

    public class CLATemplateService : ICLATemplateService
    {
        private readonly IContentManager _contentManager;
        private readonly Localizer T;

        public CLATemplateService(IContentManager contentManager) {
            _contentManager = contentManager;

            T = NullLocalizer.Instance;
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

        public bool Validate(EditCLATemplateViewModel model, IUpdateModel updater, IContent itemToUpdate) {
            bool hasError = false;
            if (_contentManager.Query("CLATemplate").Where<CLATemplatePartRecord>(r => r.CLATitle == model.Title && r.Id != itemToUpdate.Id).List().Any()) {
                updater.AddModelError(model, m => m.Title, T("'{0}' is already the title of an agreement template. Please use another.", model.Title));
                hasError = true;
            }

            return !hasError;
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