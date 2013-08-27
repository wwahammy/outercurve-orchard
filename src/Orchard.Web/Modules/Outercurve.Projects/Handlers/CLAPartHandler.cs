using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;
using Outercurve.Projects.Models;

namespace Outercurve.Projects.Handlers
{
    public class CLAPartHandler : ContentHandler
    {
        private readonly IContentManager _contentManager;

        public CLAPartHandler(IRepository<CLAPartRecord> repository, IContentManager contentManager) {
            _contentManager = contentManager;
            Filters.Add(StorageFilter.For(repository));
            
            OnActivated<CLAPart>(PropertySetHandlers);
           
            OnLoading<CLAPart>((context,part) => LazyLoadHandlers(part));
            OnVersioning<CLAPart>((context, part, newVersionPart) => LazyLoadHandlers(newVersionPart));

        }

        private void LazyLoadHandlers(CLAPart part) {
            part.CLATemplateField.Loader(() => _contentManager.Get(part.Record.TemplateId, VersionOptions.Number(part.Record.TemplateVersion)));
        }

        private void PropertySetHandlers(ActivatedContentContext context, CLAPart part) {
            
            part.CLATemplateField.Setter(template => {
                            if (template == null) {
                                part.Record.TemplateId = 0;
                                part.Record.TemplateVersion = 0;
                            }
                            else {
                                part.Record.TemplateId = template.Id;
                                part.Record.TemplateVersion = template.ContentItem.Version;
                            }

                return template;
            });
            // Force call to setter if we had already set a value
            if (part.CLATemplateField.Value != null) 
                part.CLATemplateField.Value = part.CLATemplateField.Value;
            
        }
    }
}