using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard;

namespace Outercurve.Projects.Services
{

    public interface  IUTCifierService : IDependency {
        DateTime GetUtcFromLocalDate(string date);
    }

    public class UTCifierService : IUTCifierService
    {
        public DateTime GetUtcFromLocalDate(string date) {
            var datetime = DateTime.Parse(date);
            return TimeZoneInfo.ConvertTimeToUtc(datetime);
        }
    }
}