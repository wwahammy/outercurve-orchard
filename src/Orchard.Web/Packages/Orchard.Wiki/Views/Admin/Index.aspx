<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<AdminViewModel>" %>
<%@ Import Namespace="Orchard.Mvc.Html"%>
<%@ Import Namespace="Orchard.Mvc.ViewModels"%>


<!DOCTYPE html>
<html>
<head id="Head1" runat="server">
    <title>Index2</title>
    <% Html.Include("Head"); %>
</head>
<body>
    <% Html.Include("Header"); %>
    <% Html.BeginForm(); %>
    <div class="yui-g">
        <h2>Wiki Admin</h2>
    </div>
    <% Html.EndForm(); %>
    <% Html.Include("Footer"); %>
</body>
</html>