using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using CsvHelper;
using CsvHelper.Configuration;
using FluentNHibernate.Utils;
using NHibernate.Linq;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Core.Common.Models;
using Orchard.Core.Title.Models;
using Orchard.Roles.Models;
using Orchard.Users.Models;
using Outercurve.Projects.Models;
using Outercurve.Projects.MoreLinq;

namespace Outercurve.Projects.Services
{
    public interface ICLAToOfficeService : IDependency {
        byte[] CreateCLASpreadsheet();
    }

    public class CLAToOfficeService : ICLAToOfficeService
    {
        private readonly IContentManager _contentManager;

        public CLAToOfficeService(IContentManager contentManager) {
            _contentManager = contentManager;
        }
       
        public byte[] CreateCLASpreadsheet() {
            var claData = ConvertCLAPartToCLAData();
            using (var textWriter = new StringWriter()) {

                var csv = new CsvWriter(textWriter);
                csv.Configuration.RegisterClassMap<CLADataMap>();
                csv.WriteRecords(claData);
                csv.Dispose();
                return System.Text.Encoding.Unicode.GetBytes(textWriter.ToString());
            }

            
        }

        private IEnumerable<CLAData> ConvertCLAPartToCLAData() {
            var allTheCLAs = _contentManager.Query<CLAPart, CLAPartRecord>().WithQueryHintsFor("CLA");
            var results = allTheCLAs.List();
            var setOfAllNeededUsers = CLAPart.GetUsersFromListOfCLAs(results);

            var userDictionary = _contentManager.GetMany<UserPart>(setOfAllNeededUsers, VersionOptions.Latest, new QueryHints().ExpandParts<ExtendedUserPart>().
                ExpandParts<UserRolesPart>()).ToDictionary(i => i.Id, i => i);

            foreach (var cla in results) {
                var data = new CLAData {
                    
                    CLASignerName = userDictionary[cla.CLASigner.Id].As<ExtendedUserPart>().FullName,
                    
                    HasFoundationSigner = cla.HasFoundationSigner,
                    
                     RequiresEmployerSignature = cla.RequiresEmployerSigner,
                    Employer = cla.Employer,
                    LocationOfCLA = cla.LocationOfCLA, 
                    IsValid = cla.IsValid, 
                    IsCommitter = cla.IsCommitter, 
                    Comments = cla.Comments
                };

                if (cla.IsSignedByUser) {
                    data.IsSigned = cla.IsSignedByUser;
                    data.SignedDate = cla.SignedDate;
                }

                if (cla.HasFoundationSigner) {
                    data.HasFoundationSigner = cla.HasFoundationSigner;
                    data.FoundationSigner = userDictionary[cla.FoundationSigner.Id].As<ExtendedUserPart>().FullName;
                    data.FoundationSignerDate = cla.FoundationSignedOn;
                }

                if (cla.HasEmployerSignature) {
                    data.HasEmployerSignature = cla.HasEmployerSignature;
                    data.SignerFromEmployer = cla.SignerFromCompany;
                    data.EmployerSignedDate = cla.EmployerSignedOn;
                }

                if (cla.As<CommonPart>().Container != null) {
                    data.Project = cla.As<CommonPart>().Container.As<TitlePart>().Title;
                }

                yield return data;


            }
        }
    }

    public class CLAData {
        public string Project { get; set; }
        public string CLASignerName { get; set; }
        public bool IsSigned { get; set; }
        public DateTime? SignedDate { get; set; }
        public bool HasFoundationSigner { get; set; }
        public string FoundationSigner { get; set; }
        public DateTime? FoundationSignerDate { get; set; }
        public bool RequiresEmployerSignature { get; set; }
        public bool HasEmployerSignature { get; set; }
        public string SignerFromEmployer { get; set; }
        public DateTime? EmployerSignedDate { get; set; }
        public string Employer { get; set; }
        public string LocationOfCLA { get; set; }
        public bool IsValid { get; set; }
        public bool IsCommitter { get; set; }
        public string Comments { get; set; }
    }
  

    public class CLADataMap : CsvClassMap<CLAData> {
        public override void CreateMap() {
            Map(p => p.Project).Name("Project");
            Map(p => p.CLASignerName).Name("CLASignerName");
            Map(p => p.IsSigned).Name("Is Signed?");
            
            Map(p => p.SignedDate).Name("Signed Date");

            Map(p => p.HasFoundationSigner).Name("Has Foundation Signer");
            Map(p => p.FoundationSigner).Name("Foundation Signer");
            Map(p => p.FoundationSignerDate).Name("Foundation Signer Date");
            Map(p => p.RequiresEmployerSignature).Name("Needs Employer Signature");
            Map(p => p.HasEmployerSignature).Name("Has Employer Signature");
            Map(p => p.SignerFromEmployer).Name("Signer From Employer");
            Map(p => p.EmployerSignedDate).Name("Employer Signed Date");
            Map(p => p.Employer).Name("Employer");
            Map(p => p.LocationOfCLA).Name("Location of CLA");
            Map(p => p.IsValid).Name("Is CLA Valid");
            Map(p => p.IsCommitter).Name("Is Committer");
            Map(p => p.Comments).Name("Comments");

        }

        
    }
    
}