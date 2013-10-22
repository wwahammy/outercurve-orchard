using System.Collections.Generic;
using Orchard.Environment.Extensions.Models;
using Orchard.Security.Permissions;

namespace Outercurve.Projects {
    public class ProjectPermissions :IPermissionProvider {



        public static readonly Permission AllProjectPermissions = new Permission {Name = "AllProjectPermissions", Description = "All Outercurve project permissions"};


        public static readonly Permission ModifyCLAs = new Permission {Name = "ModifyAgreements", Description = "Modify agreements", ImpliedBy = new[] {AllProjectPermissions}};
        public static readonly Permission ModifyProjects = new Permission {Name = "ModifyProjectOwnersAndNames", Description = "Modify Project Owners and Names", ImpliedBy = new [] { AllProjectPermissions}};
        public static readonly Permission ModifyAgreementTemplates = new Permission {Name = "ModifyAgreementTemplates", Description = "Modify Agreement Templates", ImpliedBy = new[] {AllProjectPermissions}};
        public static readonly Permission ModifyGalleries = new Permission {Name = "ModifyGalleries", Description = "Modify Galleries", ImpliedBy = new[] {AllProjectPermissions}};

        public static readonly Permission QuickCreateUser = new Permission {Name = "QuickCreateUser", Description = "Quick create users", ImpliedBy = new[] {ModifyCLAs, ModifyProjects, ModifyGalleries, AllProjectPermissions}};

        public virtual Feature Feature { get; set; }
        public IEnumerable<Permission> GetPermissions() {
            return new[] {
                AllProjectPermissions,
                ModifyCLAs,
                ModifyProjects,
                ModifyAgreementTemplates,
                ModifyGalleries,
                QuickCreateUser
            };
        }

        public IEnumerable<PermissionStereotype> GetDefaultStereotypes() {
            return new[] {
                new PermissionStereotype {
                    Name = "Administrator",
                    Permissions = new[] {AllProjectPermissions}
                }
            };
        }
    }
}