<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AllGridProp.aspx.cs" Inherits="AddSearch.AllGridProp" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:GridView ID="GridView1" runat="server">
            </asp:GridView>
        </div>
        <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:AssignmetConnectionString %>" ProviderName="<%$ ConnectionStrings:AssignmetConnectionString.ProviderName %>" SelectCommand="SELECT [UserId], [UserName], [Password], [Email], [MobileNo], [City] FROM [UserDetails]"></asp:SqlDataSource>
    </form>
</body>
</html>
