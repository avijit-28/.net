<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ProductForm.aspx.cs" Inherits="WebApplication4.ProductForm" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
        .auto-style1 {
            width: 100%;
            height: 173px;
        }
        .auto-style2 {
            width: 346px;
        }
    </style>
</head>
<body style="height: 290px">
    <form id="form1" runat="server">
        <div>
            <table class="auto-style1">
                <tr>
                    <td class="auto-style2">Enter Product Id</td>
                    <td>
                        <asp:TextBox ID="TxtPdrId" runat="server" Width="220px"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="auto-style2">Enter Product Name</td>
                    <td>
                        <asp:TextBox ID="TxtPrdName" runat="server" Width="220px"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="auto-style2">Enter Product Price</td>
                    <td>
                        <asp:TextBox ID="TxtPrdPrc" runat="server" Width="220px"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="auto-style2">Stock</td>
                    <td>
                        <asp:TextBox ID="TxtPrdStk" runat="server" Width="220px"></asp:TextBox>
                    </td>
                </tr>
            </table>
            <asp:Button ID="BtnAddPrd" runat="server" Height="32px" Text="Add Product" Width="115px" />
        </div>
        <asp:Label ID="LblMsg" runat="server" Height="35px" Text="Product successfully added" Width="283px"></asp:Label>
    </form>
</body>
</html>
