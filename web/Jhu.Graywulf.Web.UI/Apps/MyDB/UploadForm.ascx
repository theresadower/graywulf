﻿<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UploadForm.ascx.cs" Inherits="Jhu.Graywulf.Web.UI.Apps.MyDB.UploadForm" %>

<table class="FormTable">
    <tr>
        <td class="FormLabel" style="width: 50px">
            <asp:Label runat="server" ID="Label1">File:</asp:Label>&nbsp;&nbsp;
        </td>
        <td class="FormField" style="width: 420px">
            <input type="file" id="importedFile" name="importedFile" runat="server" class="FormField"
                style="width: 420px" />
            <asp:RequiredFieldValidator runat="server" ControlToValidate="importedFile"
                display="Dynamic" ErrorMessage="<br/ >A file must be selected" />
        </td>
    </tr>
</table>
