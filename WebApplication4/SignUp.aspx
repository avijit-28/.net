<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SignUp.aspx.cs" Inherits="WebApplication4.SignUp" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div style="height: 459px">

            <asp:Image ID="Image1" runat="server" Height="400px" ImageUrl="https://images.unsplash.com/photo-1791608047988-9c48157584ad?q=80&amp;w=387&amp;auto=format&amp;fit=crop&amp;ixlib=rb-4.1.0&amp;ixid=NHwxMjA3fDA%3D" Width="220px" />
            <asp:ImageMap ID="ImageMap1" runat="server" Height="400px" ImageUrl="https://images.unsplash.com/photo-1791724389897-0ece687f2b7a?q=80&amp;w=387&amp;auto=format&amp;fit=crop&amp;ixlib=rb-4.1.0&amp;ixid=NHwxMjA3fDA%3D">
            </asp:ImageMap>
            
            <br/>
            <asp:Image ID="Image2" runat="server" Height="400px" ImageUrl="https://images.unsplash.com/photo-1791724389693-b873064e086d?q=80&amp;w=387&amp;auto=format&amp;fit=crop&amp;ixlib=rb-4.1.0&amp;ixid=NHwxMjA3fDA%3D" />
            <asp:Label ID="outUserName" runat="server" Text="Label"></asp:Label>
            
            <br />
            <asp:Label ID="outPassword" runat="server" Text="Label"></asp:Label>
            
        </div>
    </form>
</body>
</html>
