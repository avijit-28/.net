<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FirstPage.aspx.cs" Inherits="WebApplication4.FirstPage" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
        #form1 {
            height: 147px;
        }
    </style>
</head>
<body>

    <form id="form1" runat="server">
        <asp:Label ID="lblUser" runat="server" Text="UserName"></asp:Label>
        <asp:TextBox ID="txtUserName" runat="server" Height="19px" TextMode="Email" Width="167px"></asp:TextBox>
        <p>
            <asp:Label ID="lblPassword" runat="server" Text="Password"></asp:Label>
            <asp:TextBox ID="txtPassword" runat="server" Height="19px" TextMode="Password" Width="167px"></asp:TextBox>
        </p>
        <asp:Button ID="BtnSign" runat="server" OnClick="BtnSign_Click" Text="Sign In" />
    </form>

</body>
</html>
