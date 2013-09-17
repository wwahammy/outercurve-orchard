using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.Security;
using Orchard.Users.Models;
using Outercurve.Projects.Models;
using Outercurve.Projects.Services;
using Outercurve.Projects.ViewModels;
using Outercurve.Projects.Helpers;
namespace Outercurve.Projects.Drivers
{
    public class MultipleLeaderPartDriver :ContentPartDriver<MultipleLeaderPart> {
        private readonly IMultipleLeaderService _multLeadService;
        private readonly IExtendedUserPartService _extUserService;
        private readonly IMembershipService _membershipService;
        private readonly IContentManager _contentManager;
        private const string TemplateName = "Parts/MultipleLeader";

        public MultipleLeaderPartDriver(IMultipleLeaderService multLeadService, IExtendedUserPartService extUserService, IMembershipService membershipService, IContentManager contentManager) {
            _multLeadService = multLeadService;
            _extUserService = extUserService;
            _membershipService = membershipService;
            _contentManager = contentManager;
        }

        protected override string Prefix {
            get { return "MultipleLeader"; }
        }

        protected override DriverResult Display(MultipleLeaderPart part, string displayType, dynamic shapeHelper) {
            return ContentShape("Parts_MultipleLeader", () => shapeHelper.Parts_MultipleLeader(ContentPart: part, Leaders: part.Leaders.Select(l => _extUserService.GetFullName(l))));
        }

        protected override DriverResult Editor(MultipleLeaderPart part, dynamic shapeHelper) {
            return CreateShape(BuildEditorViewModel(part), shapeHelper);
        }

        protected override DriverResult Editor(MultipleLeaderPart part, Orchard.ContentManagement.IUpdateModel updater, dynamic shapeHelper) {
            var model = new EditMultipleLeaderViewModel();
            if (updater.TryUpdateModel(model, Prefix, null, null)) {
                if (_multLeadService.Validate(model, updater)) {
                    _multLeadService.UpdateLeadersForContentItem(part.ContentItem, model);
                    return Editor(part, shapeHelper);
                }
            }
            
            return CreateShape(model, shapeHelper);
        }


        private DriverResult CreateShape(EditMultipleLeaderViewModel vm, dynamic shapeHelper) {
            MustSetEveryTime(vm);
            return ContentShape("Parts_MultipleLeader_Edit", () => shapeHelper.EditorTemplate(
                TemplateName: TemplateName,
                Model: vm,
                Prefix: Prefix));
        }

        private void MustSetEveryTime(EditMultipleLeaderViewModel vm) {
            vm.UsernamesToNiceNames = _extUserService.GetSortedUserNameToFullName().ToList();
        }

        private EditMultipleLeaderViewModel BuildEditorViewModel(MultipleLeaderPart part) {
            var model = new EditMultipleLeaderViewModel {
                SelectedUsernames = part.Leaders.Select(i => i.UserName).ToList()
                
            };

            return model;
        }

       
    }
}