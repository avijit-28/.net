<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AllGridProp.aspx.cs" Inherits="AddSearch.AllGridProp" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div style="height: 233px">
            <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:AssignConn %>" SelectCommand="SELECT [UserId], [UserName], [Password], [Email], [MobileNo], [City] FROM [UserDetails]" ProviderName="<%$ ConnectionStrings:AssignConn.ProviderName %>"></asp:SqlDataSource>

            <asp:GridView ID="gvUsers" DataSourceID="SqlDataSource1"   DataKeyNames="UserId" runat="server" AutoGenerateColumns="False">

    <Columns>
        <asp:BoundField DataField="UserId"
            HeaderText="User ID" />

        <asp:BoundField DataField="UserName"
            HeaderText="User Name" />

        <asp:BoundField DataField="Password"
            HeaderText="Password" />

        <asp:BoundField DataField="Email"
            HeaderText="Email" />

        <asp:BoundField DataField="MobileNo"
            HeaderText="Mobile No" />

        <asp:BoundField DataField="City"
            HeaderText="City" />
    </Columns>

</asp:GridView>
            

        </div>
    </form>
</body>
</html>
