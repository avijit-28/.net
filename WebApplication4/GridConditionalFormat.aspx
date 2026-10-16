<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GridConditionalFormat.aspx.cs" Inherits="WebApplication4.GridConditionalFormat" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Hidden Column & Conditional Formatting (Slides 14 - 19)</title>
    <style>
        body {
            font-family: Arial, sans-serif;
            margin: 30px;
            background-color: #f8fafc;
            color: #333;
        }
        .container {
            max-width: 850px;
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
        .legend {
            margin-bottom: 15px;
            font-size: 13px;
        }
        .legend-box {
            display: inline-block;
            width: 14px;
            height: 14px;
            vertical-align: middle;
            margin-right: 5px;
        }
        .red-box {
            background-color: Red;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <a href="AllGridProp.aspx" class="nav-link">&larr; Back to Main Hub</a>
            <h2>Hidden Culture Column &amp; Conditional Row Highlighting</h2>
            <p><strong>Slides 14 &ndash; 19:</strong> <code>CountryCulture</code> column is hidden dynamically in <code>RowDataBound</code> for both header and data rows. Rows with <code>AnnualSalary &gt; 70000</code> are highlighted in <strong>RED</strong> with white text.</p>

            <div class="legend">
                <span class="legend-box red-box"></span> <strong>Highlight:</strong> Rows with AnnualSalary &gt; 70,000
            </div>

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
                    <asp:BoundField DataField="AnnualSalary" HeaderText="AnnualSalary" />
                    <asp:BoundField DataField="Country" HeaderText="Country" />
                    <asp:BoundField DataField="CountryCulture" HeaderText="CountryCulture" />
                </Columns>
                <FooterStyle BackColor="#F7DFB5" ForeColor="#8C4510" />
                <HeaderStyle BackColor="#A55129" Font-Bold="True" ForeColor="White" />
                <RowStyle BackColor="#FFF7E7" ForeColor="#8C4510" />
            </asp:GridView>
        </div>
    </form>
</body>
</html>
