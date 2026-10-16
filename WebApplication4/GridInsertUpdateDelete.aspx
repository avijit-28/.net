<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GridInsertUpdateDelete.aspx.cs" Inherits="WebApplication4.GridInsertUpdateDelete" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>GridView Insert, Update, and Delete (Slides 27 - 38)</title>
    <style>
        body { font-family: Arial, sans-serif; margin: 30px; background-color: #f4f6f9; }
        .box { background: #fff; padding: 20px; border-radius: 8px; box-shadow: 0 2px 8px rgba(0,0,0,0.1); max-width: 900px; }
        h2 { color: #333; margin-top: 0; }
        p { color: #666; font-size: 14px; }
        .back-link { display: inline-block; margin-bottom: 15px; color: #0066cc; text-decoration: none; font-weight: bold; }
        .back-link:hover { text-decoration: underline; }
        .val-summary { margin-top: 15px; padding: 10px; background: #fee2e2; border: 1px solid #f87171; border-radius: 4px; }
        .status { margin-top: 10px; color: green; font-weight: bold; }
        .btn-insert { font-weight: bold; color: #059669; text-decoration: none; }
        .btn-insert:hover { text-decoration: underline; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="box">
            <a href="AllGridProp.aspx" class="back-link">&larr; Back to Topics List</a>
            <h2>Part 6: GridView Inline Insert, Update, and Delete</h2>
            <p>
                Demonstrates complete inline data manipulation:<br />
                &bull; <strong>ShowFooter="True"</strong> to enable the footer row for inserting new records.<br />
                &bull; <strong>LinkButton (lbInsert)</strong> in the FooterTemplate with <code>ValidationGroup="Insert"</code> and <code>OnClick="lbInsert_Click"</code>.<br />
                &bull; <strong>CommandField</strong> with <code>ShowEditButton="True"</code> and <code>ShowDeleteButton="True"</code>.<br />
                &bull; Separate <strong>ValidationSummary</strong> controls for Edit (<code>ValidationGroup="Edit"</code>) and Insert (<code>ValidationGroup="Insert"</code>).
            </p>
            
            <asp:GridView ID="GridView1" runat="server" 
                AutoGenerateColumns="False" 
                DataKeyNames="EmployeeId" 
                ShowFooter="True" 
                BackColor="#DEBA84" 
                BorderColor="#DEBA84" 
                BorderStyle="None" 
                BorderWidth="1px" 
                CellPadding="5" 
                CellSpacing="2"
                OnRowEditing="GridView1_RowEditing"
                OnRowCancelingEdit="GridView1_RowCancelingEdit"
                OnRowUpdating="GridView1_RowUpdating"
                OnRowDeleting="GridView1_RowDeleting">
                <Columns>
                    <asp:CommandField ShowDeleteButton="True" ShowEditButton="True" ValidationGroup="Edit" />

                    <asp:TemplateField HeaderText="EmployeeId" InsertVisible="False" SortExpression="EmployeeId">
                        <EditItemTemplate>
                            <asp:Label ID="Label1" runat="server" Text='<%# Eval("EmployeeId") %>'></asp:Label>
                        </EditItemTemplate>
                        <ItemTemplate>
                            <asp:Label ID="Label1" runat="server" Text='<%# Bind("EmployeeId") %>'></asp:Label>
                        </ItemTemplate>
                        <FooterTemplate>
                            <asp:LinkButton ID="lbInsert" ValidationGroup="Insert" runat="server" OnClick="lbInsert_Click" CssClass="btn-insert">+ Insert</asp:LinkButton>
                        </FooterTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Name" SortExpression="Name">
                        <EditItemTemplate>
                            <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("Name") %>'></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvEditName" runat="server" 
                                ErrorMessage="Name is a required field"
                                ControlToValidate="TextBox1" Text="*" ForeColor="Red" ValidationGroup="Edit">
                            </asp:RequiredFieldValidator>
                        </EditItemTemplate>
                        <ItemTemplate>
                            <asp:Label ID="Label2" runat="server" Text='<%# Bind("Name") %>'></asp:Label>
                        </ItemTemplate>
                        <FooterTemplate>
                            <asp:TextBox ID="txtName" runat="server" placeholder="Name"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvInsertName" runat="server" 
                                ErrorMessage="Name is a required field" 
                                ControlToValidate="txtName" Text="*" ForeColor="Red"
                                ValidationGroup="Insert">
                            </asp:RequiredFieldValidator>
                        </FooterTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Gender" SortExpression="Gender">
                        <EditItemTemplate>
                            <asp:DropDownList ID="DropDownList1" runat="server" SelectedValue='<%# Bind("Gender") %>'>
                                <asp:ListItem>Select Gender</asp:ListItem>
                                <asp:ListItem>Male</asp:ListItem>
                                <asp:ListItem>Female</asp:ListItem>
                            </asp:DropDownList>
                            <asp:RequiredFieldValidator ID="rfvEditGender" runat="server" 
                                ErrorMessage="Gender is a required field" Text="*"
                                ControlToValidate="DropDownList1" ForeColor="Red" 
                                InitialValue="Select Gender" ValidationGroup="Edit">
                            </asp:RequiredFieldValidator>
                        </EditItemTemplate>
                        <ItemTemplate>
                            <asp:Label ID="Label3" runat="server" Text='<%# Bind("Gender") %>'></asp:Label>
                        </ItemTemplate>
                        <FooterTemplate>
                            <asp:DropDownList ID="ddlInsertGender" runat="server">
                                <asp:ListItem>Select Gender</asp:ListItem>
                                <asp:ListItem>Male</asp:ListItem>
                                <asp:ListItem>Female</asp:ListItem>
                            </asp:DropDownList>
                            <asp:RequiredFieldValidator ID="rfvInsertGender" runat="server" 
                                ErrorMessage="Gender is a required field" Text="*"
                                ControlToValidate="ddlInsertGender" ForeColor="Red" 
                                InitialValue="Select Gender" ValidationGroup="Insert">
                            </asp:RequiredFieldValidator>
                        </FooterTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="City" SortExpression="City">
                        <EditItemTemplate>
                            <asp:TextBox ID="TextBox3" runat="server" Text='<%# Bind("City") %>'></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvEditCity" runat="server" 
                                ErrorMessage="City is a required field" Text="*"
                                ControlToValidate="TextBox3" ForeColor="Red" ValidationGroup="Edit">
                            </asp:RequiredFieldValidator>
                        </EditItemTemplate>
                        <ItemTemplate>
                            <asp:Label ID="Label4" runat="server" Text='<%# Bind("City") %>'></asp:Label>
                        </ItemTemplate>
                        <FooterTemplate>
                            <asp:TextBox ID="txtCity" runat="server" placeholder="City"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvInsertCity" runat="server" 
                                ErrorMessage="City is a required field" Text="*" 
                                ControlToValidate="txtCity" ForeColor="Red" 
                                ValidationGroup="Insert">
                            </asp:RequiredFieldValidator>
                        </FooterTemplate>
                    </asp:TemplateField>
                </Columns>
                <FooterStyle BackColor="#F7DFB5" ForeColor="#8C4510" />
                <HeaderStyle BackColor="#A55129" Font-Bold="True" ForeColor="White" />
                <PagerStyle ForeColor="#8C4510" HorizontalAlign="Center" />
                <RowStyle BackColor="#FFF7E7" ForeColor="#8C4510" />
            </asp:GridView>

            <asp:ValidationSummary ID="ValidationSummary1" runat="server" 
                ForeColor="Red" HeaderText="Please correct the following edit errors:" 
                ValidationGroup="Edit" CssClass="val-summary" />

            <asp:ValidationSummary ID="ValidationSummaryInsert" runat="server" 
                ForeColor="Red" HeaderText="Please correct the following insert errors:" 
                ValidationGroup="Insert" CssClass="val-summary" />

            <asp:Label ID="lblStatus" runat="server" CssClass="status" Visible="false"></asp:Label>
        </div>
    </form>
</body>
</html>
