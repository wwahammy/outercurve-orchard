using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls.WebParts;
using MarkdownDeep;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.Data;
using Orchard.Localization;
using Outercurve.Projects.Models;
using Outercurve.Projects.Services;
using Outercurve.Projects.ViewModels.Parts;

namespace Outercurve.Projects.Drivers
{
    public class CLATextPartDriver : ContentPartDriver<CLATextPart> {
        private readonly ICLATemplateService _templateService;
        private readonly IContentManager _contentManager;
        private readonly ICLATextPartService _claTextPartService;
        private readonly ITransactionManager _transaction;
        private const string TemplateName = "Parts/CLAText";
        public Localizer T;

        public CLATextPartDriver(ICLATemplateService templateService, IContentManager contentManager, ICLATextPartService claTextPartService, ITransactionManager transaction) {
            _templateService = templateService;
            _contentManager = contentManager;
            _claTextPartService = claTextPartService;
            _transaction = transaction;
        }

        protected override string Prefix
        {
            get { return "CLAText"; }
        }

        protected override DriverResult Display(CLATextPart part, string displayType, dynamic shapeHelper) {
            return ContentShape("Parts_CLAText", () => shapeHelper.Parts_CLAText(
                ContentItem: part.ContentItem,
                CLATemplate: part.CLATemplate
                ));


        }

        protected override DriverResult Editor(CLATextPart part, dynamic shapeHelper) {
            return CreateShape(BuildEditorViewModel(part), shapeHelper);
           
        }

       

        protected override DriverResult Editor(CLATextPart part, IUpdateModel updater, dynamic shapeHelper) {
            var model = new EditCLATextViewModel();

            if (updater.TryUpdateModel(model, Prefix, null, null)) {
                var hadError = false;
                _claTextPartService.UpdatePart(part.ContentItem, 
                    _templateService.GetCLATemplateFromIdVersion(model.SelectedTemplate));
            }
            else {
                _transaction.Cancel();
            }

            return CreateShape(model, shapeHelper);
        }

        protected virtual DriverResult CreateShape(EditCLATextViewModel vm, dynamic shapeHelper)
        {


            MustSetEveryTime(vm);

            return ContentShape("Parts_CLAText_Edit",
                                () => shapeHelper.EditorTemplate(
                                    TemplateName: TemplateName,
                                    Model: vm,
                                    Prefix: Prefix));
        }


        

        private EditCLATextViewModel BuildEditorViewModel(CLATextPart part) {
            var selectedTemplate = part.CLATemplate ?? _contentManager.Query("CLATemplate").List().FirstOrDefault();
            EditCLATextViewModel vm;
            if (selectedTemplate == null) {
                vm = new EditCLATextViewModel {TemplateDoesntExist = true};
            }
            else {
                vm = new EditCLATextViewModel {
                    SelectedTemplate = _templateService.CreateCLATemplateIdVersion(selectedTemplate)
                };
            }


            return vm;
        }

        private void MustSetEveryTime(EditCLATextViewModel vm)
        {
            if (!vm.TemplateDoesntExist) {
                var selectedTemplate = _templateService.GetCLATemplateFromIdVersion(vm.SelectedTemplate);

                var allTemplatesIdVersionAndNiceName =
                    _contentManager.Query(VersionOptions.AllVersions, "CLATemplate").
                        List().Select(i => new KeyValuePair<string, string>(_templateService.CreateCLATemplateIdVersion(i), i.As<CLATemplatePart>().CLATitle + ", v" + i.Version));
                vm.TemplateInfo = new TemplateDetailViewModel {
                    CurrentHtmlForTemplate = new Markdown().Transform(selectedTemplate.As<CLATemplatePart>().CLA),
                    TemplateNameVersionsAndIds = allTemplatesIdVersionAndNiceName
                };
            }
        }
    }
}