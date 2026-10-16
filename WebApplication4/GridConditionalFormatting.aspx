<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GridConditionalFormatting.aspx.cs" Inherits="WebApplication4.GridConditionalFormatting" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Design-Time Styling & Conditional Formatting (Slides 17 - 19)</title>
    <style>
        body { font-family: Arial, sans-serif; margin: 30px; background-color: #f4f6f9; }
        .box { background: #fff; padding: 20px; border-radius: 8px; box-shadow: 0 2px 8px rgba(0,0,0,0.1); max-width: 800px; }
        h2 { color: #333; margin-top: 0; }
        p { color: #666; font-size: 14px; }
        .back-link { display: inline-block; margin-bottom: 15px; color: #0066cc; text-decoration: none; font-weight: bold; }
        .back-link:hover { text-decoration: underline; }
        .legend { margin: 12px 0; font-size: 13px; color: #555; }
        .dot-red { display: inline-block; width: 12px; height: 12px; background: red; vertical-align: middle; margin-right: 5px; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="box">
            <a href="AllGridProp.aspx" class="back-link">&larr; Back to Topics List</a>
            <h2>Part 3: Design-Time Styles &amp; Conditional Formatting</h2>
            <p>Demonstrates setting design-time styles (<code>HeaderStyle</code>, <code>RowStyle</code>, <code>AlternatingRowStyle</code>, etc.) and dynamically highlighting rows with <strong>AnnualSalary &gt; 70,000</strong> with a <strong>Red background</strong> and <strong>White text</strong> at runtime in <code>RowDataBound</code>.</p>
            
            <div class="legend">
                <span class="dot-red"></span> Highlighted Row: AnnualSalary &gt; 70,000
            </div>

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
                    <asp:BoundField DataField="AnnualSalary" HeaderText="AnnualSalary" DataFormatString="{0:c}" />
                    <asp:BoundField DataField="Department" HeaderText="Department" />
                </Columns>
                <FooterStyle BackColor="#F7DFB5" ForeColor="#8C4510" />
                <HeaderStyle BackColor="#A55129" Font-Bold="True" ForeColor="White" />
                <PagerStyle ForeColor="#8C4510" HorizontalAlign="Center" />
                <RowStyle BackColor="#FFF7E7" ForeColor="#8C4510" />
                <AlternatingRowStyle BackColor="#FFFFFF" ForeColor="#8C4510" />
            </asp:GridView>
        </div>
    </form>
</body>
</html>
