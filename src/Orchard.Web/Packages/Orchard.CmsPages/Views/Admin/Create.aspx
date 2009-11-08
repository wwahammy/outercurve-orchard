<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<Orchard.CmsPages.ViewModels.PageCreateViewModel>" %>

<%@ Import Namespace="Orchard.Mvc.Html" %>
<!DOCTYPE html>
<html>
<head id="Head1" runat="server">
    <title>Index2</title>
    <% Html.Include("Head"); %>
</head>
<body>
    <% Html.Include("Header"); %>
    <div class="yui-u">
        <h2 class="separator">Add a Page</h2>
        <p class="bottomSpacer">
            Select your layout from one of the templates below.</p>
    </div>
    <div class="yui-u">
        <%using (Html.BeginForm()) { %>
        <%= Html.ValidationSummary() %>
        <%= Html.EditorForModel() %>
        <%}/*EndForm*/%>
    </div>
    <% Html.Include("Footer"); %>
</body>
</html>