using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.Core.Common.Models;
using Orchard.Core.Title.Models;
using Orchard.Data;
using Orchard.Localization;
using Outercurve.Projects.Models;
using Outercurve.Projects.Services;
using Outercurve.Projects.ViewModels;
using Outercurve.Projects.ViewModels.Parts;

namespace Outercurve.Projects.Drivers
{
    public class CLATemplatePartDriver : ContentPartDriver<CLATemplatePart> {
        private readonly ICLATemplateService _templateService;
        private readonly ITransactionManager _transaction;
        private const string TemplateName = "Parts/CLATemplate";
        public Localizer T;


        public CLATemplatePartDriver(ICLATemplateService templateService, ITransactionManager transaction) {
            _templateService = templateService;
            _transaction = transaction;
        }


        protected override string Prefix {
            get { return "CLATemplate"; }
        }

        protected override DriverResult Display(CLATemplatePart part, string displayType, dynamic shapeHelper) {
            return ContentShape("Parts_CLATemplate", () => shapeHelper.Parts_CLATemplate(
                ContentItem: part.ContentItem,
                ContentPart: part,
                Title: part.CLATitle,
                CLA: part.CLA));
        }


        protected override DriverResult Editor(CLATemplatePart part, dynamic shapeHelper)
        {
            return CreateShape(BuildEditorViewModel(part), shapeHelper);
        }

        

        protected override DriverResult Editor(CLATemplatePart part, IUpdateModel updater, dynamic shapeHelper) {
            var model = new EditCLATemplateViewModel();
            if (updater.TryUpdateModel(model, Prefix, null, null)) {
                _templateService.UpdateCLATemplatePart(part.ContentItem, model);
                //return good thing
            }
            else {
                _transaction.Cancel();
            }

            return CreateShape(model, shapeHelper);
        }

        private EditCLATemplateViewModel BuildEditorViewModel(CLATemplatePart part) {
            var vm = new EditCLATemplateViewModel {
                Id = part.Id,
                CLA = part.CLA,
                Title = part.CLATitle
            };

            return vm;
        }

        private DriverResult CreateShape(EditCLATemplateViewModel vm, dynamic shapeHelper)
        {
            return ContentShape("Parts_CLATemplate_Edit",
                                () => shapeHelper.EditorTemplate(
                                    TemplateName: TemplateName,
                                    Model: vm,
                                    Prefix: Prefix));
        }

        protected override void Importing(CLATemplatePart part, Orchard.ContentManagement.Handlers.ImportContentContext context)
        {
            part.CLA = context.Attribute(part.PartDefinition.Name, "CLA");
            part.CLATitle = context.Attribute(part.PartDefinition.Name, "CLATitle");
        }

        protected override void Exporting(CLATemplatePart part, Orchard.ContentManagement.Handlers.ExportContentContext context)
        {
            context.Element(part.PartDefinition.Name).SetAttributeValue("CLA", part.Record.CLA);
            context.Element(part.PartDefinition.Name).SetAttributeValue("CLATitle", part.Record.CLATitle);
        }
    }
}