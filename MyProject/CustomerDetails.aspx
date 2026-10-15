<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CustomerDetails.aspx.cs" Inherits="MyProject.CustomerDetails" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <div>
<b>Customer Id:</b>
<asp:Label ID="lblCustomerId" runat="server" />
<asp:GridView ID="GridView1" runat="server"></asp:GridView>
</div>
        </div>
    </form>
</body>
</html>
