<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GridMultiCulture.aspx.cs" Inherits="WebApplication4.GridMultiCulture" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Multi-Culture Currency Formatting (Slides 10 - 13)</title>
    <style>
        body {
            font-family: Arial, sans-serif;
            margin: 30px;
            background-color: #f8fafc;
            color: #333;
        }
        .container {
            max-width: 800px;
            margin: 0 auto;
            background: #ffffff;
            padding: 25px;
            border-radius: 8px;
            box-shadow: 0 2px 10px rgba(0,0,0,0.1);
        }
        h2 {
            color: #1e3a8a;
            margin-bottom: 5px;
        }
        p {
            color: #64748b;
            font-size: 14px;
            margin-bottom: 20px;
        }
        .nav-link {
            display: inline-block;
            margin-bottom: 15px;
            color: #2563eb;
            text-decoration: none;
            font-weight: bold;
        }
        .nav-link:hover {
            text-decoration: underline;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <a href="AllGridProp.aspx" class="nav-link">&larr; Back to Main Hub</a>
            <h2>Multi-Culture Currency Formatting per Row</h2>
            <p><strong>Slides 10 &ndash; 13:</strong> The <code>RowDataBound</code> event inspects the <code>Country</code> column and applies country-specific currency symbols ($, &pound;, &#8377;, R, RM) dynamically.</p>

            <asp:GridView ID="GridView1" runat="server" 
                AutoGenerateColumns="False" 
                DataKeyNames="EmployeeId" 
                BackColor="#DEBA84" 
                BorderColor="#DEBA84" 
                BorderStyle="None" 
                BorderWidth="1px" 
                CellPadding="6" 
                CellSpacing="2" 
                OnRowDataBound="GridView1_RowDataBound"
                Width="100%">
                <Columns>
                    <asp:BoundField DataField="EmployeeId" HeaderText="EmployeeId" ReadOnly="True" />
                    <asp:BoundField DataField="FirstName" HeaderText="FirstName" />
                    <asp:BoundField DataField="Salary" HeaderText="Salary" />
                    <asp:BoundField DataField="Country" HeaderText="Country" />
                </Columns>
                <FooterStyle BackColor="#F7DFB5" ForeColor="#8C4510" />
                <HeaderStyle BackColor="#A55129" Font-Bold="True" ForeColor="White" />
                <RowStyle BackColor="#FFF7E7" ForeColor="#8C4510" />
            </asp:GridView>
        </div>
    </form>
</body>
</html>
