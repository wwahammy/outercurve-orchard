﻿<%@ Control Language="C#" Inherits="Orchard.Mvc.ViewUserControl<ArchiveLater.ViewModels.ArchiveLaterViewModel>" %>
<% Html.RegisterStyle("datetime.css"); %>
<% Html.RegisterStyle("jquery-ui-1.7.2.custom.css"); %>
<% Html.RegisterStyle("ui.datepicker.css"); %>
<% Html.RegisterStyle("ui.timepickr.css"); %>
<% Html.RegisterFootScript("jquery.ui.core.js"); %>
<% Html.RegisterFootScript("jquery.ui.widget.js"); %>
<% Html.RegisterFootScript("jquery.ui.datepicker.js"); %>
<% Html.RegisterFootScript("jquery.utils.js"); %>
<% Html.RegisterFootScript("ui.timepickr.js"); %>
<fieldset>
    <legend><%: T("Archive Settings")%></legend>
    <div>
        <%: Html.CheckBox("ArchiveLater", Model.ScheduledArchiveUtc.HasValue, new { id = ViewData.TemplateInfo.GetFullHtmlFieldId("Command_ArchiveLater") })%>
        <label class="forcheckbox" for="<%:ViewData.TemplateInfo.GetFullHtmlFieldId("Command_ArchiveLater") %>"><%: T("Archive Later")%></label>
    </div>
    <div>
        <label class="forpicker" for="<%:ViewData.TemplateInfo.GetFullHtmlFieldId("ScheduledArchiveDate") %>"><%: T("Date")%></label>
        <%: Html.EditorFor(m => m.ScheduledArchiveDate)%>
        <label class="forpicker" for="<%:ViewData.TemplateInfo.GetFullHtmlFieldId("ScheduledArchiveTime") %>"><%: T("Time")%></label>
        <%: Html.EditorFor(m => m.ScheduledArchiveTime)%>
    </div>
</fieldset>
<script type="text/javascript">    $(function () {
        //todo: (heskew) make a plugin
        $("label.forpicker").each(function () {
            var $this = $(this);
            var pickerInput = $("#" + $this.attr("for"));
            pickerInput.data("hint", $this.text());
            if (!pickerInput.val()) {
                pickerInput.addClass("hinted")
            .val(pickerInput.data("hint"))
            .focus(function () { var $this = $(this); if ($this.val() == $this.data("hint")) { $this.removeClass("hinted").val("") } })
            .blur(function () { var $this = $(this); setTimeout(function () { if (!$this.val()) { $this.addClass("hinted").val($this.data("hint")) } }, 300) });
            }
        });
        $(<%=string.Format("\"#{0}\"", ViewData.TemplateInfo.GetFullHtmlFieldId("ScheduledArchiveDate")) %>).datepicker({ showAnim: "" }).focus(function () { $(<%=string.Format("\"#{0}\"", ViewData.TemplateInfo.GetFullHtmlFieldId("Command_ArchiveLater")) %>).attr("checked", "checked") });
        $(<%=string.Format("\"#{0}\"", ViewData.TemplateInfo.GetFullHtmlFieldId("ScheduledArchiveTime")) %>).timepickr().focus(function () { $(<%=string.Format("\"#{0}\"", ViewData.TemplateInfo.GetFullHtmlFieldId("Command_ArchiveLater")) %>).attr("checked", "checked") });
    })</script>