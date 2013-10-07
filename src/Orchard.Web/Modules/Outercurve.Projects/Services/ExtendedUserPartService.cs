using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;
using Orchard.Core.Common.Models;
using Orchard.Data;
using Orchard.Security;
using Orchard.Users.Models;
using Orchard.Users.Services;
using Outercurve.Projects.Models;
using Outercurve.Projects.ViewModels;
using Outercurve.Projects.ViewModels.Parts;

namespace Outercurve.Projects.Services
{
    public interface IExtendedUserPartService : IDependency {
        void UpdateItemWithExtendedUserInfo(ContentItem item, EditExtendedUserViewModel model);
      //  UserViewModel GetViewModelFromId(int id);
        //List<UserViewModel> GetAllUserViewModels();

        ExtendedUserEntry GetExtendedUserEntry(UserPartRecord user);
      

       
        IUser CreateAutoRegisteredUser(string email, string firstName, string lastName);
        IEnumerable<ExtendedUserEntry>  GetSortedExtendedUserEntries();
        IEnumerable<KeyValuePair<string, string>> GetSortedUserNameToFullName();
        IEnumerable<SelectListEntry> GetExtendedUserListEntries();
        string GetFullName(ContentPartRecord r);
        string GetFullName(ContentItem u);
        bool VerifyUserEmailUnicity(string email);
        T GetPartFromIUser<T>(IUser user) where T : IContent;
        string GetFullName(IUser u);

        bool IsAContributor(IUser u);
        bool IsAProjectLeader(IUser u);
    }

    public class ExtendedUserPartService : IExtendedUserPartService
    {
        private readonly IRepository<ExtendedUserPartRecord> _extUserRepo;
        private readonly IContentManager _contentManager;
        private readonly IRepository<UserPartRecord> _userRepo;
        private readonly IMembershipService _membershipService;
        private readonly IUserService _userService;

        public ExtendedUserPartService(IRepository<ExtendedUserPartRecord> extUserRepo, 
            IContentManager contentManager, 
            IRepository<UserPartRecord> userRepo, 
            IMembershipService membershipService,
            IUserService userService) {
            _extUserRepo = extUserRepo;
            _contentManager = contentManager;
            _userRepo = userRepo;
            _membershipService = membershipService;
            _userService = userService;
        }

        public IEnumerable<KeyValuePair<string, string>> GetSortedUserNameToFullName() {
            var users = _contentManager.Query("User").List();
            return users.Select(u => new KeyValuePair<string, string>(u.As<UserPart>().NormalizedUserName, GetFullName(u))).OrderBy(i => i.Key);
        }
        public IEnumerable<SelectListEntry> GetExtendedUserListEntries() {
            return GetSortedUserNameToFullName().Select(u => new SelectListEntry {Id = u.Key, Name = u.Value}); 
        }

        public string GetFullName(IUser u) {
            return GetFullName(u.ContentItem);
        }

        public string GetFullName(ContentPartRecord r) {
            return GetFullName(_contentManager.Get(r.Id));
        }

        public string GetFullName(ContentItem u) {
            return String.IsNullOrWhiteSpace(u.As<ExtendedUserPart>().FullName) ? u.As<UserPart>().NormalizedUserName : u.As<ExtendedUserPart>().FullName;
        }


        public void UpdateItemWithExtendedUserInfo(ContentItem item, EditExtendedUserViewModel model) {
            var part = item.As<ExtendedUserPart>();
            part.FirstName = model.FirstName;
            part.LastName = model.LastName;


        }

        public T GetPartFromIUser<T>(IUser user) where T : IContent {
            return user.As<T>();
        }
        /*
        public UserViewModel GetViewModelFromId(int id) {
            var user = _contentManager.Query("User").Where<UserPartRecord>(i => i.Id == id).List().SingleOrDefault();
            if (user == null)
            {
                return null;
            }

            return new UserViewModel {
                UserPartId = user.As<UserPart>().Id,
                FullName = user.As<ExtendedUserPart>().FullName
            };

        }*/

        

       

        public ExtendedUserEntry GetExtendedUserEntry(UserPartRecord user) {
            if (user == null) {
                return null;
            }
            var extPart = _contentManager.Get<UserPart>(user.Id).As<ExtendedUserPart>();
            return new ExtendedUserEntry {UserPart = user, ExtendedUserPart = extPart};
        }

       // public IContent GetExtendedUserPart(ExtendedUserPartRecord record) {
          //  return _contentManager.Get(record.Id);
        //}

        public IEnumerable<ExtendedUserEntry>  GetSortedExtendedUserEntries() {
           return _contentManager.Query<ExtendedUserPart>("User").OrderBy<ExtendedUserPartRecord>(i => i.FirstName).OrderBy<ExtendedUserPartRecord>(i => i.LastName).List().Select(i => new ExtendedUserEntry {ExtendedUserPart = i, UserPart = i.As<UserPart>().Record}).ToList();
       }

        public IUser CreateAutoRegisteredUser(string email, string firstName, string lastName) {
            if (VerifyUserEmailUnicity(email)) {
                var userName = GetUnregisteredUserName(email, firstName, lastName);
                if (_userService.VerifyUserUnicity(userName, email)) {
                    var user = _membershipService.CreateUser(new CreateUserParams(userName, Guid.NewGuid().ToString("N"), email, null, null, false));
                    var extUser = user.As<ExtendedUserPart>();
                    extUser.FirstName = firstName;
                    extUser.LastName = lastName;
                    extUser.AutoRegistered = true;
                    return user;
                }
            }
            return null;
        }

 
       private string GetUnregisteredUserName(string email, string firstName, string lastName) {

           var userName = firstName.ToLowerInvariant()[0] + lastName.ToLowerInvariant();
           if (_userService.VerifyUserUnicity(userName, email))
               return userName;
           userName = email.Split('@')[0].ToLowerInvariant();
           if (_userService.VerifyUserUnicity(userName, email))
               return userName;
           for (int i = 1; true; i++) {
               userName = firstName.ToLowerInvariant()[0] + lastName.ToLowerInvariant() + i;
               if (_userService.VerifyUserUnicity(userName, email))
                   return userName;
           }
       }

        public bool VerifyUserEmailUnicity(string email) {
            return !_contentManager.Query<UserPart, UserPartRecord>().Where(user =>
                                                                    user.Email == email).List().Any();
        }

#if false
        public List<UserViewModel> GetAllUserViewModels() {
            var users = _contentManager.Query("User");
            return users.List().Select(a =>
                new UserViewModel { FullName = 
                    String.IsNullOrWhiteSpace(a.As<ExtendedUserPart>().FullName) ? a.As<UserPart>().UserName : a.As<ExtendedUserPart>().FullName, UserPartId = a.As<UserPart>().Id }).ToList();
        }
#endif


        public bool IsAContributor(IUser u) {
            return _contentManager.Query("CLA").Where<CLAPartRecord>(cla => cla.CLASigner.Id == u.Id && cla.IsValid()).Count() > 0;
        }

        public bool IsAProjectLeader(IUser u) {
            return _contentManager.Query("Project").Where<MultipleLeaderPartRecord>(mpr => mpr.Owners.Any(i => i.UserPartRecord.Id == u.Id)).Count() > 0;
        }
    }



    public class ExtendedUserEntry {
        public UserPartRecord UserPart { get; set; }
        public ExtendedUserPart ExtendedUserPart { get; set; }
    }
}