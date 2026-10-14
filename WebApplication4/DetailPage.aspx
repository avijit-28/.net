<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DetailPage.aspx.cs" Inherits="WebApplication4.DetailPage" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <h3>Article</h3>
            <b>Title:</b><asp:Label ID="lblTitle" runat="server" Text="Label" Font-Bold="true" ForeColor="blue"></asp:Label><br />
            <b>AuthorName:</b>  <asp:Label ID="lblauthor" runat="server" Text="Label" Font-Bold="true" 
                ForeColor="blue"></asp:Label><br />
            <b>Description:</b><br />
            <asp:Label ID="lblDescription" runat="server" Text="Label"></asp:Label><br />
        </div>
    </form>
</body>
</html>
