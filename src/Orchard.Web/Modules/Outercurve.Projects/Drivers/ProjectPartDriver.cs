using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.Data;
using Orchard.Localization;
using Outercurve.Projects.Models;
using Outercurve.Projects.Services;
using Outercurve.Projects.ViewModels.Parts;

namespace Outercurve.Projects.Drivers
{
    public class ProjectPartDriver : ContentPartDriver<ProjectPart>
    {
        private readonly IContentManager _contentManager;
        private readonly IProjectService _projectService;
        private readonly ITransactionManager _transaction;

        private const string TemplateName = "Parts/Project";
        public Localizer T;

        public ProjectPartDriver( IContentManager contentManager, IProjectService projectService, ITransactionManager transaction) {
            _contentManager = contentManager;
            _projectService = projectService;
            _transaction = transaction;
        }


        protected override string Prefix
        {
            get { return "Project"; }
        }

        protected override DriverResult Display(ProjectPart part, string displayType, dynamic shapeHelper)
        {
            return ContentShape("Parts_Project", () => shapeHelper.Parts_Project(
                ContentItem: part.ContentItem,
                ContentPart: part,
                CLATemplate: part.CLATemplate
                
               ));
        }

        protected override DriverResult Editor(ProjectPart part, dynamic shapeHelper) {
            return CreateShape(BuildEditorViewModel(part), shapeHelper);
          
        }

        protected override DriverResult Editor(ProjectPart part, IUpdateModel updater, dynamic shapeHelper)
        {
            var model = new EditProjectViewModel();
            if (updater.TryUpdateModel(model, Prefix, null, new []{"AllTemplates"}) && _projectService.Validate(model, updater))
            {
                _projectService.UpdateProjectPart(part.ContentItem, model);
               
            }
            else {
                _transaction.Cancel();
            }
            model.AllTemplates = GetAllTemplates();

            return CreateShape(model, shapeHelper);
        }

        //protected for testing purproses
        protected EditProjectViewModel BuildEditorViewModel(ProjectPart part) {
            var vm = new EditProjectViewModel {
                Id = part.Id,
                SelectedTemplate_Id = part.CLATemplate == null ?  -1 : part.CLATemplate.Id

            };

            vm.AllTemplates = GetAllTemplates();

            return vm;
        }

        private IList<CLATemplatePart> GetAllTemplates() {
            return _contentManager.Query<CLATemplatePart>().List().ToList();
        }

        private DriverResult CreateShape(EditProjectViewModel vm, dynamic shapeHelper)
        {
            return ContentShape("Parts_Project_Edit",
                                () => shapeHelper.EditorTemplate(
                                    TemplateName: TemplateName,
                                    Model: vm,
                                    Prefix: Prefix));
        }

        protected override void Importing(ProjectPart part, Orchard.ContentManagement.Handlers.ImportContentContext context) {
            part.CLATemplate = context.GetItemFromSession(context.Attribute(part.PartDefinition.Name, "CLATemplateId")).As<CLATemplatePart>().Record;
        }

        protected override void Exporting(ProjectPart part, Orchard.ContentManagement.Handlers.ExportContentContext context)
        {

            context.Element(part.PartDefinition.Name).SetAttributeValue("CLATemplateId", _contentManager.GetItemMetadata(_contentManager.Get(part.CLATemplate.Id)).Identity);
        }
    }
}