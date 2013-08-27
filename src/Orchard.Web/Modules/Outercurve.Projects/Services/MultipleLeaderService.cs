using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Data;
using Orchard.Localization;
using Orchard.Security;
using Orchard.Users.Models;
using Outercurve.Projects.Models;
using Outercurve.Projects.ViewModels;

namespace Outercurve.Projects.Services
{
    public interface IMultipleLeaderService : IDependency {
        void UpdateLeadersForContentItem(ContentItem item, EditMultipleLeaderViewModel model);
        bool Validate(EditMultipleLeaderViewModel model, IUpdateModel update);
    }

    public class MultipleLeaderService : IMultipleLeaderService
    {
        private readonly IRepository<ExtendedUserPartRecord> _userRepo;
        private readonly IRepository<ContentMultipleLeaderUserRecord> _contentLeaderRepo;
        private readonly IMembershipService _membershipService;

        private Localizer T { get; set; }

        public MultipleLeaderService(IRepository<ExtendedUserPartRecord> userRepo, IRepository<ContentMultipleLeaderUserRecord> contentLeaderRepo, IMembershipService membershipService) {
            _userRepo = userRepo;
            _contentLeaderRepo = contentLeaderRepo;
            _membershipService = membershipService;

            T = NullLocalizer.Instance;
        }

        public bool Validate(EditMultipleLeaderViewModel model, IUpdateModel update) {

            var valid = true;
            var duplicates = model.UsernamesForLeadersSelected.Select(u => u.SelectedUserName).GroupBy(s => s).Where(g => g.Count() > 1)
                                  .Select(g => g.Key);
            foreach (var i in duplicates) {
                update.AddModelError("UsernamesForLeadersSelected", T("The username {0} was selected twice", i));
                valid = false;
            }

            foreach (var i in model.UsernamesForLeadersSelected) {
                if (_membershipService.GetUser(i.SelectedUserName) == null) {
                    update.AddModelError("UsernamesForLeadersSelected", T("The username {0} is not a valid user", i.SelectedUserName));
                    valid = false;
                }
            }

            return valid;
        }

        public void UpdateLeadersForContentItem(ContentItem item, EditMultipleLeaderViewModel model) {

            var record = item.As<MultipleLeaderPart>().Record;
            var oldLeaders =  _contentLeaderRepo.Fetch(r => r.MultipleLeaderPartRecord == record);

            var newOwners = model.UsernamesForLeadersSelected.Select(p => _membershipService.GetUser(p.SelectedUserName).As<UserPart>().Record).
                Distinct().ToDictionary(r => r, r => false);

            //delete oldRecords

            foreach (var conLeaderRecord in oldLeaders) {
                if (newOwners.ContainsKey(conLeaderRecord.UserPartRecord)) {
                    newOwners[conLeaderRecord.UserPartRecord] = true;
                }
                else {
                    _contentLeaderRepo.Delete(conLeaderRecord);
                }
            }

            foreach (var leader in newOwners.Where(kvp => !kvp.Value).Select(kvp => kvp.Key)) {
                _contentLeaderRepo.Create(new ContentMultipleLeaderUserRecord {UserPartRecord = leader, MultipleLeaderPartRecord = record});
            }
        }
    }

    
}