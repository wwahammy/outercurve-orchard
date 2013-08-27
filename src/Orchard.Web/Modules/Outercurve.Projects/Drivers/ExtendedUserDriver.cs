using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.Data;
using Orchard.Security;
using Outercurve.Projects.Models;
using Outercurve.Projects.Services;
using Outercurve.Projects.ViewModels;

namespace Outercurve.Projects.Drivers
{
    public class ExtendedUserPartDriver : ContentPartDriver<ExtendedUserPart>
    {
        private readonly IExtendedUserPartService _extUserService;
        private readonly IRepository<ExtendedUserPartRecord> _extUserRepo;
        private const string TemplateName = "Parts/ExtendedUser";

        public ExtendedUserPartDriver(IExtendedUserPartService extUserService, IRepository<ExtendedUserPartRecord> extUserRepo) {
            _extUserService = extUserService;
            _extUserRepo = extUserRepo;
        }

        protected override string Prefix
        {
            get { return "ExtendedUser"; }
        }


        protected override DriverResult Display(ExtendedUserPart part, string displayType, dynamic shapeHelper) {

            return ContentShape("Parts_ExtendedUser", () => shapeHelper.Parts_ExtendedUser(
                ContentPart: part,
                FirstName: part.FirstName,
                LastName: part.LastName
           ));
        }

        protected override DriverResult Editor(ExtendedUserPart part, dynamic shapeHelper) {
            return CreateShape(BuildEditorViewModel(part), shapeHelper);
            
        }

        protected override DriverResult Editor(ExtendedUserPart part, IUpdateModel updater, dynamic shapeHelper) {
            var model = BuildEditorViewModel(part);
            if (updater.TryUpdateModel(model, Prefix, null, null)) {
                
                _extUserService.UpdateItemWithExtendedUserInfo(part.ContentItem, model);
                return Editor(part, shapeHelper);
            }
         
            return CreateShape(model, shapeHelper);
        }


        protected override void Exporting(ExtendedUserPart part, Orchard.ContentManagement.Handlers.ExportContentContext context)
        {
           context.Element(part.PartDefinition.Name).SetAttributeValue("FirstName", part.Record.FirstName);
           context.Element(part.PartDefinition.Name).SetAttributeValue("LastName", part.Record.FirstName);
           context.Element(part.PartDefinition.Name).SetAttributeValue("AutoRegistered", part.Record.AutoRegistered);
        }

        protected override void Importing(ExtendedUserPart part, Orchard.ContentManagement.Handlers.ImportContentContext context) {
            part.Record.FirstName = context.Attribute(part.PartDefinition.Name, "FirstName");
            part.Record.LastName = context.Attribute(part.PartDefinition.Name, "LastName");
            var autoRegistered = context.Attribute(part.PartDefinition.Name, "AutoRegistered");
            if (autoRegistered != null) {
                part.Record.AutoRegistered = bool.Parse(autoRegistered);    
            }
            
            
        }
      


        private DriverResult CreateShape(EditExtendedUserViewModel vm, dynamic shapeHelper) {
            return ContentShape("Parts_ExtendedUser_Edit",
                                () => shapeHelper.EditorTemplate(
                                    TemplateName: TemplateName,
                                    Model: vm,
                                    Prefix: Prefix));
        }

        private EditExtendedUserViewModel BuildEditorViewModel(ExtendedUserPart part) {
            var vm = new EditExtendedUserViewModel {
                User = part.As<IUser>(),
                FirstName =  part.FirstName,
                LastName =  part.LastName,
                Id = part.Id
            };
            return vm;
        }

        
    }
}