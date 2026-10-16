<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GridHiddenCulture.aspx.cs" Inherits="WebApplication4.GridHiddenCulture" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Hidden Culture Column Formatting (Slides 14 - 16)</title>
    <style>
        body { font-family: Arial, sans-serif; margin: 30px; background-color: #f4f6f9; }
        .box { background: #fff; padding: 20px; border-radius: 8px; box-shadow: 0 2px 8px rgba(0,0,0,0.1); max-width: 800px; }
        h2 { color: #333; margin-top: 0; }
        p { color: #666; font-size: 14px; }
        .back-link { display: inline-block; margin-bottom: 15px; color: #0066cc; text-decoration: none; font-weight: bold; }
        .back-link:hover { text-decoration: underline; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="box">
            <a href="AllGridProp.aspx" class="back-link">&larr; Back to Topics List</a>
            <h2>Part 2: Formatting Currency using a Hidden Culture Column</h2>
            <p>Demonstrates retrieving the culture code from a <strong>CountryCulture</strong> column, formatting the salary accordingly, and hiding the <strong>CountryCulture</strong> column from the end user in both header and data rows.</p>
            
            <asp:GridView ID="GridView1" runat="server" 
                AutoGenerateColumns="False" 
                BackColor="#DEBA84" 
                BorderColor="#DEBA84" 
                BorderStyle="None" 
                BorderWidth="1px" 
                CellPadding="5" 
                CellSpacing="2" 
                OnRowDataBound="GridView1_RowDataBound">
                <Columns>
                    <asp:BoundField DataField="EmployeeId" HeaderText="EmployeeId" ReadOnly="True" />
                    <asp:BoundField DataField="FirstName" HeaderText="FirstName" />
                    <asp:BoundField DataField="AnnualSalary" HeaderText="AnnualSalary" />
                    <asp:BoundField DataField="Country" HeaderText="Country" />
                    <asp:BoundField DataField="CountryCulture" HeaderText="CountryCulture" />
                </Columns>
                <FooterStyle BackColor="#F7DFB5" ForeColor="#8C4510" />
                <HeaderStyle BackColor="#A55129" Font-Bold="True" ForeColor="White" />
                <PagerStyle ForeColor="#8C4510" HorizontalAlign="Center" />
                <RowStyle BackColor="#FFF7E7" ForeColor="#8C4510" />
            </asp:GridView>
        </div>
    </form>
</body>
</html>
