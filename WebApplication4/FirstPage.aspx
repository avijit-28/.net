<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FirstPage.aspx.cs" Inherits="WebApplication4.FirstPage" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="lblMsg" runat="server">
        welcome<br />
        hiii how are you<br />
        <p>
            <asp:Label ID="lblEnd" runat="server" Text="Thank you"></asp:Label>
        </p>
        <asp:Button ID="Button1" runat="server" OnClick="Button1_Click" Text="Click Me" />
        <p>
            <asp:Button ID="Button2" runat="server" BackColor="#FFFF66" BorderColor="#FFFF66" Font-Italic="True" Font-Names="Arial Black" ForeColor="Red" OnClick="Button2_Click" Text="Go back" />
        </p>
    </form>
</body>
</html>
