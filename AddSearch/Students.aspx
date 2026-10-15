<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="WebForm1.aspx.cs" Inherits="AddSearch.WebForm1" %>

<asp:Content ID="Content1"
    ContentPlaceHolderID="MainContent"
    runat="server">

    <asp:GridView ID="GridView1"
        runat="server"
        AutoGenerateColumns="true">
    </asp:GridView>

</asp:Content>