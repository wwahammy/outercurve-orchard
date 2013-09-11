using System.Data;
using System.Linq;
using Orchard.ContentManagement.MetaData;
using Orchard.ContentManagement.Records;
using Orchard.Core.Common.Models;
using Orchard.Core.Containers.Models;
using Orchard.Core.Contents.Extensions;
using Orchard.Core.Title.Models;
using Orchard.Data;
using Orchard.Data.Migration;
using Orchard.Users.Models;
using Outercurve.Projects.Models;
using Outercurve.Projects.MoreLinq;

namespace Outercurve.Projects {
    public class Migrations : DataMigrationImpl {
        private readonly IRepository<ProjectPartRecord> _projectRepository;
        private readonly IRepository<ContentItemVersionRecord> _contentItemVersionRepository;

        public Migrations(IRepository<ProjectPartRecord> projectRepository, IRepository<ContentItemVersionRecord>  contentItemVersionRepository) {
            _projectRepository = projectRepository;
            _contentItemVersionRepository = contentItemVersionRepository;
        }

        public int Create() {
            SchemaBuilder.CreateTable("ContentMultipleLeaderUserRecord", table => table
                .Column("Id", DbType.Int32, column => column.PrimaryKey().Identity())
                .Column("MultipleLeaderPartRecord_id", DbType.Int32)
                .Column("UserPartRecord_id", DbType.Int32)
            );

            // Creating table CLAPartRecord
            SchemaBuilder.CreateTable("CLAPartRecord", table => table
                .ContentPartRecord()
                .Column("SignerFromCompany", DbType.String)
                .Column("SignerFromCompanyEmail", DbType.String)
                .Column("SignedDate", DbType.DateTime)
                .Column("FoundationSignedOn", DbType.DateTime)
                .Column("Employer", DbType.String)
                .Column("EmployerSignedOn", DbType.DateTime)
                .Column("EmployerMustSignBy", DbType.DateTime)
                .Column("LocationOfCLA", DbType.String)
                .Column("OfficeValidOverride", DbType.Boolean)
                .Column("Comments", DbType.String)
                .Column("IsCommitter", DbType.Boolean)
                .Column("FirstName", DbType.String)
                .Column("LastName", DbType.String)
                .Column("FoundationSignerName", DbType.String)
                .Column("SignerEmail", DbType.String)
                .Column("Address1", DbType.String)
                .Column("Address2", DbType.String)
                .Column("City", DbType.String)
                .Column("State", DbType.String)
                .Column("ZipCode", DbType.String)
                .Column("Country", DbType.String)
                .Column("RequiresEmployerSigner", DbType.Boolean)
                .Column("FoundationSigner_id", DbType.Int32)
                .Column("CLAText", DbType.String, c => c.Unlimited())
                .Column("CLASigner_id", DbType.Int32)
                .Column("TemplateId", DbType.Int32)
                .Column("TemplateVersion", DbType.Int32)
            );

            // Creating table CLATemplatePartRecord
            SchemaBuilder.CreateTable("CLATemplatePartRecord", table => table
                .ContentPartRecord()
                .Column("CLATitle", DbType.String, c => c.Unique())
                .Column("CLA", DbType.String, c => c.Unlimited())
            );

            // Creating table ExtendedUserPartRecord
            SchemaBuilder.CreateTable("ExtendedUserPartRecord", table => table
                .ContentPartRecord()
                .Column("FirstName", DbType.String)
                .Column("LastName", DbType.String)
                .Column("AutoRegistered", DbType.Boolean)
            );

            // Creating table MultipleLeaderPartRecord
            SchemaBuilder.CreateTable("MultipleLeaderPartRecord", table => table
                .ContentPartRecord()
            );

            // Creating table ProjectPartRecord
            SchemaBuilder.CreateTable("ProjectPartRecord", table => table
                .ContentPartRecord()
                .Column("CLATemplate_id", DbType.Int32)
            );
            
           
            ContentDefinitionManager.AlterPartDefinition(typeof(MultipleLeaderPart).Name, builder => builder.Attachable());


    
    

    ContentDefinitionManager.AlterPartDefinition(
        typeof(CLAPart).Name, cfg => cfg.Attachable());


    ContentDefinitionManager.AlterPartDefinition<ExtendedUserPart>( cfg => cfg.Attachable());
    ContentDefinitionManager.AlterPartDefinition<CLATemplatePart>(cfg => cfg.Attachable());
    ContentDefinitionManager.AlterPartDefinition<ProjectPart>(cfg => cfg.Attachable());

    ContentDefinitionManager.AlterTypeDefinition("Gallery", b => b.WithPart<ContainerPart>().WithPart<TitlePart>().WithPart<MultipleLeaderPart>().Draftable());
    ContentDefinitionManager.AlterTypeDefinition("Project", cfg => cfg.WithPart<ContainerPart>().WithPart<TitlePart>().WithPart<ContainablePart>().WithPart<CommonPart>(p => p.WithSetting("OwnerEditorSettings.ShowOwnerEditor", false.ToString())).WithPart<MultipleLeaderPart>().WithPart<ProjectPart>().Draftable());
    ContentDefinitionManager.AlterTypeDefinition("CLA", b => b.WithPart<CLAPart>().WithPart<ContainablePart>().WithPart<CommonPart>(p => p.WithSetting("OwnerEditorSettings.ShowOwnerEditor", false.ToString())).Draftable());       
    ContentDefinitionManager.AlterTypeDefinition("User", b => b.WithPart<UserPart>().WithPart<ExtendedUserPart>());

    ContentDefinitionManager.AlterTypeDefinition("CLATemplate", b => b.WithPart<CLATemplatePart>().WithPart<CommonPart>(p => p.WithSetting("OwnerEditorSettings.ShowOwnerEditor", false.ToString())).Creatable());


            return 1;
        }

       
/*       
        public int UpdateFrom1() {

            var existingProjects = _projectRepository.Table.Select(p => p.Id);
            
             var result = (from p in existingProjects
                          from i in _contentItemVersionRepository.Table
                          group i by i.ContentItemRecord.Id == p into g
                          select g).Select(g => g.MaxBy(i))
                          


            foreach (var p in existingProjects) {
                _contentItemVersionRepository.Table    
            }
            var versionRecords = 

            
            SchemaBuilder.AlterTable("ProjectPartRecord", a => a.AddColumn(""))
            return 2;
        }*/
    }
}