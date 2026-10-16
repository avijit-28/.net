<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GridViewAll.aspx.cs" Inherits="WebApplication4.GridViewAll" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:TextBox ID="txtSearch" runat="server"></asp:TextBox>

    <asp:Button ID="btnSearch"
    runat="server"
    Text="Search"
    OnClick="btnSearch_Click" />

<br /><br />

<asp:GridView ID="gvEmployees"
    runat="server"
    AutoGenerateColumns="False">
    <Columns>
        <asp:BoundField DataField="ID" HeaderText="ID" />
        <asp:BoundField DataField="Name" HeaderText="Name" />
        <asp:BoundField DataField="Salary" HeaderText="Salary" />
        <asp:BoundField DataField="Gender" HeaderText="Gender" />
        <asp:BoundField DataField="City" HeaderText="City" />
        <asp:BoundField DataField="Dept" HeaderText="Department" />
    </Columns>
</asp:GridView>
        </div>
    </form>
</body>
</html>
