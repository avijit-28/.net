<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="currency.aspx.cs" Inherits="AddSearch.currency" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:AssignConn %>" SelectCommand="SELECT [EmployeeId], [FirstName], [Country], [AnnualSalary] FROM [AllEmployee]"></asp:SqlDataSource>
            <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" 
                    DataKeyNames="EmployeeId" DataSourceID="SqlDataSource1" 
                    BackColor="#DEBA84" BorderColor="#DEBA84" BorderStyle="None" BorderWidth="1px" 
                    CellPadding="3" CellSpacing="2" onrowdatabound="GridView1_RowDataBound">

                 <Columns>
                    <asp:BoundField DataField="EmployeeId" HeaderText="EmployeeId" InsertVisible="False" ReadOnly="True" SortExpression="EmployeeId" />
                    <asp:BoundField DataField="FirstName" HeaderText="FirstName" SortExpression="FirstName" />
                    <asp:BoundField DataField="AnnualSalary" HeaderText="Salary" SortExpression="Salary"/>
                     <asp:TemplateField HeaderText="Country" SortExpression="Country">
                         <EditItemTemplate>
                             <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("Country") %>'></asp:TextBox>
                         </EditItemTemplate>
                         <ItemTemplate>
                             <asp:DropDownList ID="DropDownList1" runat="server"
     SelectedValue='<%# Bind("Country") %>'>
    <asp:ListItem>US</asp:ListItem>
    <asp:ListItem>India</asp:ListItem>
    <asp:ListItem>UK</asp:ListItem>
    <asp:ListItem>South Africa</asp:ListItem>
    <asp:ListItem>Malaysia</asp:ListItem>
</asp:DropDownList>
                         </ItemTemplate>
                     </asp:TemplateField>
                 </Columns>
            </asp:GridView>

        </div>
    </form>
</body>
</html>
