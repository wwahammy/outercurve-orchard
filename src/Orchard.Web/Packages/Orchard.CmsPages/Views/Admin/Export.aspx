<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<System.Collections.Generic.IEnumerable<Orchard.CmsPages.Models.Page>>" %>
<%@ Import Namespace="Orchard.Mvc.Html" %>

<!DOCTYPE html>
<html>
<head id="Head1" runat="server">
    <title>Index2</title>
    <% Html.Include("Head"); %>
</head>
<body>
    <% Html.Include("Header"); %>
    <div class="yui-g">
        <h2>
            Export</h2>
        <p class="bottomSpacer">
            Possible text about setting up a page goes here. Lorem ipsum dolor sit amet, consectetur
            adipiscing elit. Nulla erat turpis, blandit eget feugiat nec, tempus vel quam. Mauris
            et neque eget justo suscipit blandit.</p>
            <ol><%foreach (var page in Model) { %><li>
            Page Id <%=page.Id %> 
            <ol><%foreach (var revision in page.Revisions) { %>
            <li>Revision <%=revision.Number %> <b><%=revision.Title %></b> ~/<%=revision.Slug %>
            <br />Modified:<%=revision.ModifiedDate %> 
            <br />Published:<%=revision.PublishedDate %></li>
            <%} %></ol>
            </li><%} %></ol>
    </div>
    <% Html.Include("Footer"); %>
</body>
</html>