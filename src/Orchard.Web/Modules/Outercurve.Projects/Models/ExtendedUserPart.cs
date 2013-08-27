using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;
using Orchard.Users.Models;

namespace Outercurve.Projects.Models
{
    public class ExtendedUserPart : ContentPart<ExtendedUserPartRecord>
    {


        public string FullName
        {
            get { return Record.FullName(); }
        }

        public string FirstName { get { return Record.FirstName; } set { Record.FirstName = value; } }

        public string LastName { get { return Record.LastName; } set { Record.LastName = value; } }

       

        public bool AutoRegistered { get { return Record.AutoRegistered; } set { Record.AutoRegistered = value; } }
    }

   

    public class ExtendedUserPartRecord : ContentPartRecord {
        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
        public virtual bool AutoRegistered { get; set; }
        
        public virtual string FullName() {
            return (FirstName ?? "") + " " + (LastName ?? "");
        }
    }
}