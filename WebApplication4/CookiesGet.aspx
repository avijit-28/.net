<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CookiesGet.aspx.cs" Inherits="MiniProject.CookiesGet" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
        .auto-style1 {
            width: 100%;
        }
    </style>
</head>
<body style="height: 231px">
    <form id="form1" runat="server">
        <div>
            <table class="auto-style1">
                <tr>
                    <td>
                        <asp:Label ID="LblName" runat="server" Text="Name"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="LblEmail" runat="server" Text="Email"></asp:Label>
                    </td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>
