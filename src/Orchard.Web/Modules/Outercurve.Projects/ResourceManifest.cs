using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.UI.Resources;

namespace Outercurve.Projects
{
    public class ResourceManifest : IResourceManifestProvider
    {
        public void BuildManifests(ResourceManifestBuilder builder)
        {

            // Create and add a new manifest
            var manifest = builder.Add();
            manifest.DefineScript("listEditor").SetUrl("listEditor.js").SetDependencies("jQuery");
            manifest.DefineStyle("bootstrap-datetime").SetUrl("bootstrap-datetimepicker.min.css");
            manifest.DefineScript("bootstrap-datetime").SetUrl("bootstrap-datetimepicker.min.js").SetDependencies("jQuery");
            manifest.DefineStyle("local").SetUrl("local.css");
            //manifest.DefineScript("CLA").SetUrl("CLA.js").SetDependencies("jQuery", "jQuery_LinqJs", "ko");
        }
    }
}