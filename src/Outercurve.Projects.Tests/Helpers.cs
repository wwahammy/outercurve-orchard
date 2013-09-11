using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExpectedObjects;
using Orchard.ContentManagement;
using Orchard.ContentManagement.MetaData.Models;
using Orchard.ContentManagement.Records;
using Xunit;

namespace Outercurve.Projects.Tests
{
    public static class Helpers
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TPart"></typeparam>
        /// <typeparam name="TRecord"></typeparam>
        /// <param name="part"></param>
        /// <param name="contentType"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <from>http://weblogs.asp.net/bleroy/archive/2013/04/15/testing-orchard-drivers.aspx</from>
        public static ContentItem PreparePart<TPart, TRecord>(
                TPart part, string contentType, int id = -1)
            where TPart : ContentPart<TRecord>
            where TRecord : ContentPartRecord, new()
        {

            part.Record = new TRecord();
            part.TypePartDefinition = new ContentTypePartDefinition(new ContentPartDefinition(part.GetType().Name), new SettingsDictionary());
            var contentItem = part.ContentItem = new ContentItem
            {
                VersionRecord = new ContentItemVersionRecord
                {
                    ContentItemRecord = new ContentItemRecord()
                },
                ContentType = contentType
            };
            contentItem.Record.Id = id;
            contentItem.Weld(part);
            return contentItem;
        }

        public static bool IsEqualTo<T>(this ExpectedObject expected, T actual) {
            try {
                expected.ShouldEqual(actual);
                return true;
            }
            catch (Exception e) {
                Console.Write(e.Message);
                return false;
            }
            
        }
    }
}
