﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SignOut.aspx.cs" Inherits="Jhu.Graywulf.Web.Auth.SignOut"
    MasterPageFile="~/Auth.master" %>

<asp:Content runat="server" ContentPlaceHolderID="middle">
    <jgwc:Form runat="server" Text="Sign out" SkinID="SignOut">
        <FormTemplate>
            <p>You have successfully signed out from <asp:Label runat="server" ID="ShortTitle" />.</p>
        </FormTemplate>
        <ButtonsTemplate>
            <input type="button" runat="server" value="Ok" ID="Ok" class="FormButton" />
        </ButtonsTemplate>
    </jgwc:Form>
</asp:Content>