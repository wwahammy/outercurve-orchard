using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;
using Orchard.Users.Models;

namespace Outercurve.Projects.Models
{
    public class MultipleLeaderPart : ContentPart<MultipleLeaderPartRecord>
    {
        
        public IEnumerable<UserPartRecord> Leaders {
            get { return Record.Owners.Select(r => r.UserPartRecord); }
        }

    }


    public class MultipleLeaderPartRecord : ContentPartRecord {


        public MultipleLeaderPartRecord() {
            Owners = new List<ContentMultipleLeaderUserRecord>();
        }
        public virtual IList<ContentMultipleLeaderUserRecord> Owners { get; set; }



    }


    public class ContentMultipleLeaderUserRecord
    {
        public virtual int Id { get; set; }
        public virtual MultipleLeaderPartRecord MultipleLeaderPartRecord { get; set; }
        public virtual UserPartRecord UserPartRecord { get; set; }
    }
}