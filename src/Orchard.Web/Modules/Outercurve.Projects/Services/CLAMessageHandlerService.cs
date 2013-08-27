using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using Orchard.Messaging.Events;
using Orchard.Messaging.Models;

namespace Outercurve.Projects.Services
{
    public class CLAMessageHandlerService : IMessageEventHandler
    {
        public void Sending(MessageContext context) {
            if (context.MessagePrepared) 
                return;
            switch (context.Type) {
                case "CLAMessage":
                    context.MailMessage.Subject = context.Properties["Subject"];
                    context.MailMessage.Sender = new MailAddress(context.Properties["Sender"]);
                    context.MailMessage.Body = context.Properties["Body"];
                    context.MessagePrepared = true;
                    break;
            }
        }

        public void Sent(MessageContext context) {
            
        }
    }
}