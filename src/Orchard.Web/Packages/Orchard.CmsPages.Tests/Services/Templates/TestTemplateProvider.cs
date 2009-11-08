using System.Collections.Generic;
using System.IO;
using Orchard.CmsPages.Services.Templates;

namespace Orchard.CmsPages.Tests.Services.Templates {
    public class StubTemplateEntryProvider : ITemplateEntryProvider {
        private readonly List<TemplateEntry> _templates = new List<TemplateEntry>();

        public IEnumerable<TemplateEntry> List() {
            return _templates;
        }

        public void AddTemplate(string fileName, string fileContent) {
            _templates.Add(new TemplateEntry { Name = fileName, Content = new StringReader(fileContent) });
        }
    }
}