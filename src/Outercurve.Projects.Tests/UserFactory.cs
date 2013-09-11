using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using Orchard.ContentManagement;
using Orchard.Security;
using Orchard.Users.Models;
using Outercurve.Projects.Models;
using Proligence.Orchard.Testing;

namespace Outercurve.Projects.Tests
{
    public static class UserFactory
    {
        public static IUser CreateUser(UserCreationArgs args) {


            var userPart = new UserPart { Record = new UserPartRecord(), Email = args.Email, UserName = args.Username};
            var extUserPart = new ExtendedUserPart { Record =  new ExtendedUserPartRecord(), FirstName = args.FirstName, LastName = args.LastName };
            var ci = ContentFactory.CreateContentItem(args.Id, "User", userPart, extUserPart);
            userPart.ContentItem = ci;
            extUserPart.ContentItem = ci;
            return ci.As<UserPart>();

        }
    }

    public class UserCreationArgs {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
