using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.ContentManagement.Drivers;
using Outercurve.Projects.Models;
using Outercurve.Projects.ViewModels.Parts;

namespace Outercurve.Projects.Drivers
{
    public class MoreSiteSettingsPartDriver : ContentPartDriver<MoreSiteSettingsPart> {

        public const string TemplateName = "Parts/MoreSiteSettings";

        protected override string Prefix
        {
            get { return "MoreSiteSettings"; }
        }
       

        protected override DriverResult Editor(MoreSiteSettingsPart part, dynamic shapeHelper) {
            var model = new EditMoreSiteSettingsViewModel {
                                                              Part = part
                                                          };

            return CreateEditor(model, shapeHelper);
        }

        protected override DriverResult Editor(MoreSiteSettingsPart part, Orchard.ContentManagement.IUpdateModel updater, dynamic shapeHelper) {
            var model = new EditMoreSiteSettingsViewModel {Part = part};

            updater.TryUpdateModel(model, Prefix, null, null);

            return CreateEditor(model, shapeHelper);
        }

        private DriverResult CreateEditor(EditMoreSiteSettingsViewModel model, dynamic shapeHelper) {
            return ContentShape("Parts_MoreSiteSettings_Edit",
                                () =>
                                    shapeHelper.EditorTemplate(
                                        TemplateName: TemplateName, Model: model, Prefix: Prefix));
        }
    }
}