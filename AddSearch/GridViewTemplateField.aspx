<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GridViewTemplateField.aspx.cs" Inherits="AddSearch.GridViewTemplateField" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:AssignConn %>" SelectCommand="SELECT [ID], [Name], [Salary], [Gender], [City], [Dept] FROM [Employee]"></asp:SqlDataSource>
            <br />
            <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" 
                    DataKeyNames="Id" DataSourceID="SqlDataSource1" >
                <Columns>
                    <asp:TemplateField ConvertEmptyStringToNull="False" HeaderText="Gender" SortExpression="Gender">
                        <EditItemTemplate>
                            <asp:DropDownList ID="DropDownList1" runat="server">
                                <asp:ListItem>Male</asp:ListItem>
                                <asp:ListItem>Female</asp:ListItem>
                                <asp:ListItem>Others</asp:ListItem>
                            </asp:DropDownList>
                        </EditItemTemplate>
                        <ItemTemplate>
                            <asp:DynamicControl ID="DynamicControl1" runat="server" DataField="Gender" Mode="ReadOnly" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </form>
</body>
</html>
