<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Customers.aspx.cs" Inherits="MyProject.Customers" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="false">
<Columns>
<asp:BoundField DataField="customer_Id" HeaderText="Customer Id" ItemStyle-Width="100" />
<asp:BoundField DataField="first_Name" HeaderText="First Name" ItemStyle-Width="100" />
<asp:BoundField DataField="state" HeaderText="State" ItemStyle-Width="100" />
<asp:HyperLinkField Text="View" DataNavigateUrlFormatString="~/Customers/{0}"
DataNavigateUrlFields="customer_Id" />
</Columns>
</asp:GridView>
        </div>
    </form>
</body>
</html>
