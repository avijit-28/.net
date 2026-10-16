<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GridTemplateField.aspx.cs" Inherits="WebApplication4.GridTemplateField" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>TemplateField with DropDownList (Slides 20 - 24)</title>
    <style>
        body { font-family: Arial, sans-serif; margin: 30px; background-color: #f4f6f9; }
        .box { background: #fff; padding: 20px; border-radius: 8px; box-shadow: 0 2px 8px rgba(0,0,0,0.1); max-width: 800px; }
        h2 { color: #333; margin-top: 0; }
        p { color: #666; font-size: 14px; }
        .back-link { display: inline-block; margin-bottom: 15px; color: #0066cc; text-decoration: none; font-weight: bold; }
        .back-link:hover { text-decoration: underline; }
        .status { margin-top: 10px; color: green; font-weight: bold; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="box">
            <a href="AllGridProp.aspx" class="back-link">&larr; Back to Topics List</a>
            <h2>Part 4: TemplateField with DropDownList for Editing</h2>
            <p>Converts the <strong>Gender</strong> column to a <code>TemplateField</code>. In edit mode (<code>EditItemTemplate</code>), a <strong>DropDownList</strong> is displayed instead of a default TextBox, with <code>SelectedValue='<%# Bind("Gender") %>'</code>.</p>
            
            <asp:GridView ID="GridView1" runat="server" 
                AutoGenerateColumns="False" 
                DataKeyNames="EmployeeId"
                BackColor="#DEBA84" 
                BorderColor="#DEBA84" 
                BorderStyle="None" 
                BorderWidth="1px" 
                CellPadding="5" 
                CellSpacing="2"
                OnRowEditing="GridView1_RowEditing"
                OnRowCancelingEdit="GridView1_RowCancelingEdit"
                OnRowUpdating="GridView1_RowUpdating">
                <Columns>
                    <asp:CommandField ShowEditButton="True" />
                    <asp:BoundField DataField="EmployeeId" HeaderText="EmployeeId" ReadOnly="True" />
                    <asp:BoundField DataField="Name" HeaderText="Name" />
                    <asp:TemplateField HeaderText="Gender">
                        <ItemTemplate>
                            <asp:Label ID="Label1" runat="server" Text='<%# Bind("Gender") %>'></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:DropDownList ID="DropDownList1" runat="server" SelectedValue='<%# Bind("Gender") %>'>
                                <asp:ListItem>Select Gender</asp:ListItem>
                                <asp:ListItem>Male</asp:ListItem>
                                <asp:ListItem>Female</asp:ListItem>
                            </asp:DropDownList>
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="City" HeaderText="City" />
                </Columns>
                <FooterStyle BackColor="#F7DFB5" ForeColor="#8C4510" />
                <HeaderStyle BackColor="#A55129" Font-Bold="True" ForeColor="White" />
                <PagerStyle ForeColor="#8C4510" HorizontalAlign="Center" />
                <RowStyle BackColor="#FFF7E7" ForeColor="#8C4510" />
            </asp:GridView>

            <asp:Label ID="lblStatus" runat="server" CssClass="status" Visible="false"></asp:Label>
        </div>
    </form>
</body>
</html>
