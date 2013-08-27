using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Core.Common.Models;
using Orchard.Core.Containers.Models;
using Orchard.Core.Title.Models;
using Orchard.Localization;
using Orchard.Users.Models;
using Outercurve.Projects.ViewModels;

namespace Outercurve.Projects.Services
{

    public interface  IGalleryService : IDependency {
        void CreateGallery(CreateGalleryViewModel model,  IUpdateModel updater);
        IEnumerable<SelectListEntry> GetGalleriesAsSelectList();
    }

    public class GalleryService : IGalleryService {
        private readonly IContentManager _contentManager;
        private Localizer T { get; set; }


        public GalleryService(IContentManager contentManager) {
            _contentManager = contentManager;

            T = NullLocalizer.Instance;

        }


        public IEnumerable<SelectListEntry> GetGalleriesAsSelectList() {
            var galleries = _contentManager.Query("Gallery").List();
            return galleries.Select(i => new SelectListEntry {Id = i.Id.ToString(), Name = i.As<TitlePart>().Title});
        }

        public void CreateGallery(CreateGalleryViewModel model,  IUpdateModel updater) {
            
            
            //verifyTitle is unique
          
            //verify title is unique

            bool failed = false;
            if (_contentManager.Query<TitlePart, TitlePartRecord>().Where(r => r.Title == model.Title).List().Any()) {
                updater.AddModelError("Title", T("That's not a unique title"));
                failed = true;
            }

            var owner = _contentManager.Query<UserPart, UserPartRecord>().Where(r => r.NormalizedUserName == model.OwnerUserName).List().FirstOrDefault();
            if (owner == null) {
                updater.AddModelError("OwnerUserName", T("That's not a valid user"));
                failed = true;
            }


            if (failed) {
                return;
            }

            //let's do our updating and stuff!
            var newGallery = _contentManager.New("Gallery");

            newGallery.As<TitlePart>().Title = model.Title;
            newGallery.As<CommonPart>().Owner = owner;
            var container = newGallery.As<ContainerPart>();
            container.ItemContentType = "Project";
            container.Paginated = false;
            
        }
    }
}