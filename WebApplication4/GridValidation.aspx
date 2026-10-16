<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GridValidation.aspx.cs" Inherits="WebApplication4.GridValidation" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>GridView Validation Controls (Slides 25 - 27)</title>
    <style>
        body { font-family: Arial, sans-serif; margin: 30px; background-color: #f4f6f9; }
        .box { background: #fff; padding: 20px; border-radius: 8px; box-shadow: 0 2px 8px rgba(0,0,0,0.1); max-width: 800px; }
        h2 { color: #333; margin-top: 0; }
        p { color: #666; font-size: 14px; }
        .back-link { display: inline-block; margin-bottom: 15px; color: #0066cc; text-decoration: none; font-weight: bold; }
        .back-link:hover { text-decoration: underline; }
        .val-summary { margin-top: 15px; padding: 10px; background: #fee2e2; border: 1px solid #f87171; border-radius: 4px; }
        .status { margin-top: 10px; color: green; font-weight: bold; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="box">
            <a href="AllGridProp.aspx" class="back-link">&larr; Back to Topics List</a>
            <h2>Part 5: Validation Controls in GridView Edit Mode</h2>
            <p>Adds a <strong>RequiredFieldValidator</strong> to the <code>DropDownList</code> in <code>EditItemTemplate</code> with <code>InitialValue="Select Gender"</code>. If a user tries to save with "Select Gender", the page blocks submission and displays error messages in the <strong>ValidationSummary</strong>.</p>
            
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
                    <asp:CommandField ShowEditButton="True" ValidationGroup="EditGroup" />
                    
                    <asp:BoundField DataField="EmployeeId" HeaderText="EmployeeId" ReadOnly="True" />
                    
                    <asp:TemplateField HeaderText="Name">
                        <ItemTemplate>
                            <asp:Label ID="LabelName" runat="server" Text='<%# Bind("Name") %>'></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="txtName" runat="server" Text='<%# Bind("Name") %>'></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvName" runat="server" 
                                ControlToValidate="txtName" ErrorMessage="Name is a required field" 
                                Text="*" ForeColor="Red" ValidationGroup="EditGroup" />
                        </EditItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Gender">
                        <ItemTemplate>
                            <asp:Label ID="LabelGender" runat="server" Text='<%# Bind("Gender") %>'></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:DropDownList ID="DropDownList1" runat="server" SelectedValue='<%# Bind("Gender") %>'>
                                <asp:ListItem>Select Gender</asp:ListItem>
                                <asp:ListItem>Male</asp:ListItem>
                                <asp:ListItem>Female</asp:ListItem>
                            </asp:DropDownList>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" 
                                ErrorMessage="Gender is a required field" 
                                ControlToValidate="DropDownList1" 
                                InitialValue="Select Gender" 
                                Text="*" ForeColor="Red" 
                                ValidationGroup="EditGroup" />
                        </EditItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="City">
                        <ItemTemplate>
                            <asp:Label ID="LabelCity" runat="server" Text='<%# Bind("City") %>'></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="txtCity" runat="server" Text='<%# Bind("City") %>'></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvCity" runat="server" 
                                ControlToValidate="txtCity" ErrorMessage="City is a required field" 
                                Text="*" ForeColor="Red" ValidationGroup="EditGroup" />
                        </EditItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <FooterStyle BackColor="#F7DFB5" ForeColor="#8C4510" />
                <HeaderStyle BackColor="#A55129" Font-Bold="True" ForeColor="White" />
                <PagerStyle ForeColor="#8C4510" HorizontalAlign="Center" />
                <RowStyle BackColor="#FFF7E7" ForeColor="#8C4510" />
            </asp:GridView>

            <asp:ValidationSummary ID="ValidationSummary1" runat="server" 
                ForeColor="Red" HeaderText="Please correct the following errors:" 
                ValidationGroup="EditGroup" CssClass="val-summary" />

            <asp:Label ID="lblStatus" runat="server" CssClass="status" Visible="false"></asp:Label>
        </div>
    </form>
</body>
</html>
