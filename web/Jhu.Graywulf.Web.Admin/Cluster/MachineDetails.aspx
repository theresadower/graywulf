﻿<%@ Page Language="C#" Inherits="Jhu.Graywulf.Web.Admin.Cluster.MachineDetails" MasterPageFile="~/EntityChildren.master"
    CodeBehind="MachineDetails.aspx.cs" %>

<asp:Content runat="server" ContentPlaceHolderID="FormDetails">
    <table class="DetailsForm dock-top">
        <tr>
            <td class="FormLabel">
                <asp:Label ID="HostNameLabel" runat="server" Text="Host Name:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:Label ID="HostName" runat="server" Text="Label"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="AdminUrlLabel" runat="server" Text="Admin URL:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:HyperLink ID="AdminUrl" runat="server" Target="_blank">HyperLink</asp:HyperLink>
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="DeployUncPathLabel" runat="server" Text="Deployment Path:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:HyperLink ID="DeployUncPath" runat="server" Target="_blank">HyperLink</asp:HyperLink>
            </td>
        </tr>
    </table>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="FormButtons">
    <jgwac:EntityButtons runat="server" ID="EntityButtons" RunningStateButtonsVisible="true" />
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="FormTabs">
    <jgwac:EntityChildren runat="server" ID="EntityChildren">
        <jgwac:EntityList runat="server" ID="DiskGroupList" ChildrenType="DiskGroup" EntityGroup="Cluster">
            <columns>
                <asp:BoundField DataField="Type" HeaderText="Type" />
                <asp:BoundField DataField="FullSpace" HeaderText="Full Space" />
            </columns>
        </jgwac:EntityList>
        <jgwac:EntityList runat="server" ID="ServerInstanceList" ChildrenType="ServerInstance"
            EntityGroup="Cluster">
            <Columns>
                <asp:BoundField DataField="RunningState" HeaderText="Running state" />
            </Columns>
        </jgwac:EntityList>
    </jgwac:EntityChildren>
</asp:Content>
