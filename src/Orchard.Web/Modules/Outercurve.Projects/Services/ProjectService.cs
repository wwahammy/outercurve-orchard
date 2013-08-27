using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Core.Common.Models;
using Orchard.Core.Containers.Models;
using Orchard.Core.Title.Models;
using Orchard.Data;
using Orchard.Localization;
using Orchard.Users.Models;
using Outercurve.Projects.Models;
using Outercurve.Projects.ViewModels;
using Outercurve.Projects.ViewModels.Parts;

namespace Outercurve.Projects.Services
{
    public interface IProjectService : IDependency {
     //   void CreateProject(CreateProjectViewModel model, IUpdateModel updater);
        IEnumerable<SelectListEntry> GetAllProjectsEntries();
        void UpdateProjectPart(ContentItem item, EditProjectViewModel model);
        bool Validate(EditProjectViewModel model, IUpdateModel update);
    }
    
    public class ProjectService :IProjectService {
        private readonly IContentManager _contentManager;
        private readonly IRepository<CLATemplatePartRecord> _templates;
        private Localizer T { get; set;}

        public ProjectService(IContentManager contentManager, IRepository<CLATemplatePartRecord> templates ) {
            _contentManager = contentManager;
            _templates = templates;
        }

        public IEnumerable<SelectListEntry> GetAllProjectsEntries() {
            return _contentManager.Query("Project").List().Select(ci => new SelectListEntry {Id = ci.Id.ToString(), Name = ci.As<TitlePart>().Title});
        }

        public bool Validate(EditProjectViewModel model, IUpdateModel update) {
            var template = _contentManager.Get(model.SelectedTemplate_Id);
            
            var valid = _contentManager.Get(model.SelectedTemplate_Id) != null;
            valid = template == null ? valid : template.ContentType == "CLATemplate";

            if (!valid) {
                update.AddModelError(model, m => m.SelectedTemplate_Id, T("That isn't a valid CLATemplate"));
                
            }

            return valid;
        }

        public void UpdateProjectPart(ContentItem item, EditProjectViewModel model)
        {
            var part = item.As<ProjectPart>();

            part.CLATemplate = _templates.Get(model.SelectedTemplate_Id);
        }


        /*
        public void CreateProject(CreateProjectViewModel model, IUpdateModel updater) {
            
          
            //verify title is unique

            bool failed = false;
            if (_contentManager.Query<TitlePart, TitlePartRecord>().Where(r => r.Title == model.Title).List().Any()) {
                updater.AddModelError("Title", T("That's not a unique title"));
                failed = true;
            }

            var owner = _contentManager.Query<UserPart, UserPartRecord>("User").Where(r => r.NormalizedUserName == model.OwnerUserName).List().FirstOrDefault();
            if (owner == null)
            {
                updater.AddModelError("OwnerUserName", T("That's not a valid user"));
                failed = true;
            }

            var gallery = _contentManager.Query<TitlePart, TitlePartRecord>("Gallery").Where(r => r.Id == model.GalleryId).List().FirstOrDefault();

            if (gallery == null) {
                updater.AddModelError("GalleryId", T("That's not a valid gallery"));
            }

            if (failed) {
                return;
            }

            var proj = _contentManager.Create("Project");

            proj.As<TitlePart>().Title = model.Title;

            proj.As<CommonPart>().Owner = owner;

            proj.As<CommonPart>().Container = gallery;

            proj.As<ContainerPart>().ItemContentType = "CLA";

        }*/

        
    }

    
}