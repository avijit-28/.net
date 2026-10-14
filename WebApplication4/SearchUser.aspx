<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="QueryUser.aspx.cs" Inherits="QueryString1.QueryUser" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
        .auto-style1 {
            width: 100%;
            height: 157px;
        }
        .auto-style3 {
            width: 427px;
            height: 58px;
        }
        .auto-style4 {
            height: 58px;
        }
        .auto-style5 {
            width: 427px;
            height: 37px;
        }
        .auto-style6 {
            height: 37px;
        }
        .auto-style7 {
            width: 427px;
            height: 63px;
        }
        .auto-style8 {
            height: 63px;
        }
    </style>
</head>
<body style="height: 247px">
    <form id="form1" runat="server">
        <div>
            <div style="height: 102px">
                <table class="auto-style1">
                    <tr>
                        <td class="auto-style5">
                            <asp:Label ID="LblUser" runat="server" Text="UserName"></asp:Label>
                        </td>
                        <td class="auto-style6">
                            <asp:TextBox ID="TxtUser" runat="server" Height="22px" Width="190px"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="auto-style3">
                            <asp:Label ID="LblEmail" runat="server" Text="Email"></asp:Label>
                        </td>
                        <td class="auto-style4">
                            <asp:TextBox ID="TxtEmail" runat="server" Height="22px" Width="190px"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="auto-style7"></td>
                        <td class="auto-style8">
                            <asp:Button ID="BtnSend" runat="server" OnClick="BtnSend_Click" Text="Send" Width="71px" />
                        </td>
                    </tr>
                </table>
            </div>
        </div>
    </form>
</body>
</html>
