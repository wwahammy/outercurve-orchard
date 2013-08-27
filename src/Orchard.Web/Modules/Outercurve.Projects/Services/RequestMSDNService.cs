using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using Orchard;
using Orchard.Security;
using Outercurve.Projects.ViewModels;
using ServiceStack.ServiceInterface;
using ServiceStack.Text;

namespace Outercurve.Projects.Services
{
    public interface  IRequestMsdnService : IDependency {
        bool SubmitMsdnRequest(RegisterForMsdnViewModel viewModel);
    }

    public class RequestMsdnService : IRequestMsdnService
    {
        public bool SubmitMsdnRequest(RegisterForMsdnViewModel viewModel) {

            try {
                
                 var postData = new Dictionary<string, string> {
                {"entry.12.single", viewModel.UserName}, //username
                {"entry.0.single", viewModel.FirstName}, //firstname
                {"entry.1.single", viewModel.LastName}, //lastname
                {"entry.2.single", viewModel.Company}, //company
                {"entry.3.single", viewModel.Email}, // email
                {"entry.4.single", viewModel.Phone}, //phone
                {"entry.5.single", viewModel.Address1}, //address1
                {"entry.6.single", viewModel.Address2}, //address2
                {"entry.7.single", viewModel.City}, //city
                {"entry.8.single", viewModel.State}, //state
                {"entry.9.single", viewModel.ZipCode}, //zip
                {"entry.10.single", viewModel.Country}, //country
                {"entry.11.single", viewModel.CurrentMsdnNumber}, //current msdn
                {"entry.13.single", viewModel.Rationale}, //rationale

            };

            var result = "https://spreadsheets0.google.com/formResponse"
                .AddQueryParam("formkey", "dDd4QkFQTk5fRTBrcXhxdVg4c2o2WGc6MQ")
                .AddQueryParam("embedded", "true")
                .AddQueryParam("ifq", "")
                .PostToUrl(postData);

                return true;
            }
            catch (Exception) {
                return false;
                
            }
           
            
           
        }
    }
}