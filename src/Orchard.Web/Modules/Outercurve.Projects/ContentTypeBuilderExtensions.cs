using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.ContentManagement.MetaData;
using Orchard.ContentManagement.MetaData.Builders;

namespace Outercurve.Projects
{
    public static class ContentTypeDefinitionBuilderExtensions
    {
        public static ContentTypeDefinitionBuilder WithPart<T>(this ContentTypeDefinitionBuilder typeDefBuilder) {
            return typeDefBuilder.WithPart(typeof (T).Name);
        }

        public static ContentTypeDefinitionBuilder WithPart<T>(this ContentTypeDefinitionBuilder typeDefBuilder, Action<ContentTypePartDefinitionBuilder> configuration) {
            return typeDefBuilder.WithPart(typeof (T).Name, configuration);
        }

        public static void AlterPartDefinition<TPart>(this IContentDefinitionManager manager,  Action<ContentPartDefinitionBuilder> alteration) {
            manager.AlterPartDefinition(typeof(TPart).Name, alteration);
        }
    }
}