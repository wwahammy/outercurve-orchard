using System;
using System.Collections.Generic;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;
using Orchard.Core.Common.Utilities;
using Orchard.Users.Models;
using Outercurve.Projects.Models;

namespace Outercurve.Projects.Models
{
    public class CLAPart : ContentPart<CLAPartRecord> {

        private readonly LazyField<IContent> _templateId = new LazyField<IContent>();

        public LazyField<IContent> CLATemplateField { get { return _templateId; } }


        public string SignerFromCompany {
            get { return Record.SignerFromCompany; }
            set { Record.SignerFromCompany = value; }
        }

        public ExtendedUserPartRecord CLASigner {
            get { return Record.CLASigner; }
            set { Record.CLASigner = value; }

        }

        public ExtendedUserPartRecord FoundationSigner
        {
            get { return Record.FoundationSigner; }
            set { Record.FoundationSigner = value; }
        }


        public bool IsSignedByUser { get { return SignedDate.HasValue; }}
        

        public DateTime? SignedDate {
            get { return Record.SignedDate; }
            set { Record.SignedDate = value; }
        }

        public bool HasFoundationSigner {
            get { return FoundationSignedOn.HasValue; }
        }

        public DateTime? FoundationSignedOn {
            get { return Record.FoundationSignedOn; }
            set { Record.FoundationSignedOn = value; }
        }

        public bool RequiresEmployerSigner {
            get { return Record.RequiresEmployerSigner; }
            set { Record.RequiresEmployerSigner = value; }
        }

        public string Employer {
            get { return Record.Employer; }
            set { Record.Employer = value; }
        }

        public DateTime? EmployerSignedOn {
            get { return Record.EmployerSignedOn; }
            set { Record.EmployerSignedOn = value; }
        }

        public string LocationOfCLA { 
            get { return Record.LocationOfCLA; }
            set { Record.LocationOfCLA = value; }
        }

        public bool IsCommitter {
            get { return Record.IsCommitter; }
            set { Record.IsCommitter = value; }
        }

        public string Comments {
            get { return Record.Comments; }
            set { Record.Comments = value; }
        }

         public string Address1 { get { return Record.Address1; } set { Record.Address1 = value; } }

        public string Address2 { get { return Record.Address2; } set { Record.Address2 = value; } }


        public string City { get { return Record.City; } set { Record.City = value; } }

        public string State { get { return Record.State; } set { Record.State = value; } }
        public string ZipCode { get { return Record.ZipCode; } set { Record.ZipCode = value; } }

        public string Country {
            get { return Record.Country; }
            set { Record.Country = value; }} 


        public bool OfficeValidOverride {
            get { return Record.OfficeValidOverride; }
            set { Record.OfficeValidOverride = value; }

        }


        public string FirstName{
            get { return Record.FirstName; }
            set { Record.FirstName = value; }
        }

        public string LastName {
            get { return Record.LastName; }
            set { Record.LastName = value; }
        }



       

        public string SignerEmail {
            get { return Record.SignerEmail; }
            set { Record.SignerEmail = value; }
        }


        public bool IsValid {
            get { return Record.IsValid(); }
        
        }


        public IContent CLATemplate {
            get { return CLATemplateField.Value; }
            set { CLATemplateField.Value = value; }
        }
        
        /*
        public int TemplateId {
            get { return Record.TemplateId; }
            set { Record.TemplateId = value; }
        }


        public int TemplateVersion
        {
            get { return Record.TemplateVersion; }
            set { Record.TemplateVersion = value; }
        }*/

        public string SignerFromCompanyEmail {
            get { return Record.SignerFromCompanyEmail; }
            set { Record.SignerFromCompanyEmail = value; }
        }

        public DateTime? EmployerMustSignBy {
            get { return Record.EmployerMustSignBy; }
            set { Record.EmployerMustSignBy = value; }
        }


    }

    public class CLAPartRecord : ContentPartRecord {
        public virtual string SignerFromCompany { get; set; }
        public virtual string SignerFromCompanyEmail { get; set; }
        public virtual ExtendedUserPartRecord FoundationSigner { get; set; }
        public virtual ExtendedUserPartRecord CLASigner { get; set; }
        public virtual DateTime? SignedDate { get; set; }


        public virtual DateTime? FoundationSignedOn { get; set; }
        public virtual string Employer { get; set; }
        public virtual DateTime? EmployerSignedOn { get; set; }
        public virtual string LocationOfCLA { get; set; }

        public virtual DateTime? EmployerMustSignBy { get; set; }

        public virtual bool IsValid() {
            return SignedDate.HasValue && (!RequiresEmployerSigner || RequiresEmployerSigner && EmployerSignedOn.HasValue) && !OfficeValidOverride;
        }
  

        public virtual bool OfficeValidOverride { get; set; }

        public virtual string Comments { get; set; }
        public virtual bool IsCommitter { get; set; }

       
      
        public virtual string SignerEmail { get; set; }

        public virtual string Address1 { get; set; }
        public virtual string Address2 { get; set; }
        public virtual string City { get; set; }

        public virtual string State { get; set; }
        public virtual string ZipCode { get; set; }
        public virtual string Country { get; set; }

        public virtual bool RequiresEmployerSigner { get; set; }

        public virtual int TemplateId { get; set; }

        public virtual int TemplateVersion { get; set; }

        

        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }

    }



}