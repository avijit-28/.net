<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CheckAvailability.aspx.cs" Inherits="WebApplication4.CheckAvailability" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="scriptmanager1" runat="server">   </asp:ScriptManager>  
        <div>
            <asp:UpdateProgress ID="UpdateProgress1"
                                runat="server"
                                AssociatedUpdatePanelID="updatepnl">

            <ProgressTemplate>

            <div style="padding:10px;">
                <asp:Image ID="imgLoading"
                    runat="server"
                    ImageUrl="~/Images/loading.gif"
                    Width="30px"
                    Height="30px" />

                    Loading...
            </div>

    </ProgressTemplate>

</asp:UpdateProgress>


        <asp:UpdatePanel ID="updatepnl" runat="server"> 
        <ContentTemplate>

        <table>
            <tr>
                <td>Enter UserName</td>
                <td>:</td>
                <td>
                    <asp:TextBox ID="txtun"
                        runat="server"
                        AutoPostBack="true"
                        OnTextChanged="CheckUserNameAvailability">
                    </asp:TextBox>
                </td>
                <td>
                    <div id="checkun" runat="server" visible="false">

                        <asp:Image ID="shwimg"
                            runat="server"
                            Width="17px"
                            Height="17px" />

                        <asp:Label ID="lblmsg"
                            runat="server">
                        </asp:Label>

                    </div>
                </td>
            </tr>

            <tr>
                <td>Enter Password</td>
                <td>:</td>
                <td>
                    <asp:TextBox ID="txpwd"
                        runat="server"
                        TextMode="Password">
                    </asp:TextBox>
                </td>
            </tr>

            <tr>
                <td>Enter Confirm Password</td>
                <td>:</td>
                <td>
                    <asp:TextBox ID="txtconpwd"
                        runat="server"
                        TextMode="Password">
                    </asp:TextBox>
                </td>
            </tr>

            <tr>
                <td colspan="3" align="center">
                    <asp:Button ID="txtbtn"
                        runat="server"
                        Text="SUBMIT" OnClick="txtbtn_Click" />
                </td>
            </tr>
        </table>


    </ContentTemplate>
            
</asp:UpdatePanel>


        </div>
    </form>
</body>
</html>
