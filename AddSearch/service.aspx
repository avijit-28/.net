<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="service.aspx.cs" Inherits="AddSearch.service" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
   <form id="form1" runat="server">

    <div>

        <table>
            <tr>
                <td>
                    <b>Enter User Name:</b>
                </td>

                <td>
                    <asp:TextBox ID="txtUserName"
                        runat="server">
                    </asp:TextBox>
                </td>

                <td>
                    <asp:Button ID="btnSubmit"
                        runat="server"
                        Text="Submit"
                        OnClick="btnSubmit_Click" />
                </td>
            </tr>
        </table>

    </div>

    <br />

    <div>

        <asp:GridView ID="gvUserDetails"
            runat="server"
            EmptyDataText="No Record Found">

            <RowStyle BackColor="#EFF3FB" />

            <FooterStyle BackColor="#507CD1"
                Font-Bold="True"
                ForeColor="White" />

            <PagerStyle BackColor="#2461BF"
                ForeColor="White"
                HorizontalAlign="Center" />

            <HeaderStyle BackColor="#507CD1"
                Font-Bold="True"
                ForeColor="White" />

            <AlternatingRowStyle BackColor="White" />

        </asp:GridView>

    </div>

</form>
</body>
</html>
