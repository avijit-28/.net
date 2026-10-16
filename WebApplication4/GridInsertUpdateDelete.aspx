<%@ Page Title="Inline Record Management (CRUD)" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="GridInsertUpdateDelete.aspx.cs" Inherits="WebApplication4.GridInsertUpdateDelete" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .module-page-container {
            width: 100%;
            max-width: 1050px;
            margin: 0 auto;
            display: flex;
            flex-direction: column;
            gap: 20px;
        }

        .module-header-card {
            background: linear-gradient(135deg, #0f172a 0%, #1e293b 100%);
            border-radius: 16px;
            padding: 24px 30px;
            color: #ffffff;
            box-shadow: 0 10px 25px -5px rgba(15, 23, 42, 0.25);
            border: 1px solid rgba(255, 255, 255, 0.1);
            display: flex;
            justify-content: space-between;
            align-items: center;
            flex-wrap: wrap;
            gap: 16px;
        }

        .module-header-info h1 {
            font-size: 22px;
            font-weight: 800;
            color: #ffffff;
            margin-bottom: 6px;
            letter-spacing: -0.02em;
        }

        .module-header-info p {
            font-size: 13.5px;
            color: #94a3b8;
            margin: 0;
            line-height: 1.5;
        }

        .btn-hub-return {
            display: inline-flex;
            align-items: center;
            gap: 6px;
            padding: 9px 16px;
            background: rgba(255, 255, 255, 0.1);
            color: #ffffff !important;
            border: 1px solid rgba(255, 255, 255, 0.2);
            border-radius: 8px;
            font-size: 13px;
            font-weight: 600;
            text-decoration: none;
            transition: all 0.2s;
        }

        .btn-hub-return:hover {
            background: rgba(255, 255, 255, 0.2);
            transform: translateY(-1px);
        }

        .content-card {
            background: #ffffff;
            border-radius: 14px;
            border: 1px solid #e2e8f0;
            box-shadow: 0 4px 15px -3px rgba(0, 0, 0, 0.05);
            padding: 24px;
        }

        .val-summary-box {
            background: #fee2e2;
            border: 1px solid #f87171;
            border-radius: 8px;
            padding: 12px 18px;
            margin-bottom: 14px;
            color: #b91c1c;
            font-size: 13px;
        }

        .status-msg {
            display: inline-block;
            margin-bottom: 14px;
            padding: 8px 14px;
            border-radius: 8px;
            font-size: 13px;
            font-weight: 600;
            background: #f0fdf4;
            color: #15803d;
            border: 1px solid #bbf7d0;
        }

        .grid-responsive-wrap {
            overflow-x: auto;
            border-radius: 10px;
            border: 1px solid #e2e8f0;
        }

        .gridview-table {
            width: 100%;
            border-collapse: collapse;
            font-family: inherit;
            font-size: 13.5px;
        }

        .gridview-table th {
            background: #0f172a;
            color: #ffffff;
            padding: 12px 16px;
            text-align: left;
            font-weight: 700;
        }

        .gridview-table td {
            padding: 12px 16px;
            border-bottom: 1px solid #f1f5f9;
            color: #1e293b;
            vertical-align: middle;
        }

        .gridview-table tr:nth-child(even) td {
            background: #f8fafc;
        }

        .gridview-table tr:hover td {
            background: #f1f5f9;
        }

        .gridview-table tfoot td {
            background: #f1f5f9;
            border-top: 2px solid #cbd5e1;
            padding: 12px 16px;
        }

        .gridview-table select,
        .gridview-table input[type="text"] {
            padding: 6px 10px;
            border: 1px solid #cbd5e1;
            border-radius: 6px;
            font-size: 13px;
            outline: none;
            width: 100%;
            max-width: 160px;
            box-sizing: border-box;
        }

        .gridview-table select:focus,
        .gridview-table input[type="text"]:focus {
            border-color: #2563eb;
            box-shadow: 0 0 0 2px rgba(37, 99, 235, 0.2);
        }

        .btn-action-link {
            color: #2563eb;
            font-weight: 600;
            text-decoration: none;
            padding: 4px 8px;
            border-radius: 4px;
            display: inline-block;
        }

        .btn-action-link:hover {
            background: #eff6ff;
            text-decoration: underline;
        }

        .btn-insert-confirm {
            background: linear-gradient(135deg, #10b981 0%, #059669 100%);
            color: #ffffff !important;
            padding: 7px 16px;
            border-radius: 6px;
            font-weight: 700;
            text-decoration: none;
            display: inline-flex;
            align-items: center;
            gap: 6px;
            box-shadow: 0 2px 6px rgba(16, 185, 129, 0.25);
            transition: all 0.2s;
        }

        .btn-insert-confirm:hover {
            background: linear-gradient(135deg, #059669 0%, #047857 100%);
            box-shadow: 0 4px 10px rgba(16, 185, 129, 0.35);
        }

        .gridview-pager {
            background: #f8fafc;
            padding: 10px 16px;
            text-align: center;
        }
        .gridview-pager td {
            padding: 6px 10px;
        }
        .gridview-pager a, .gridview-pager span {
            padding: 6px 12px;
            border-radius: 6px;
            margin: 0 2px;
            text-decoration: none;
            font-weight: 700;
        }
        .gridview-pager span {
            background: #2563eb;
            color: #ffffff;
        }
        .gridview-pager a {
            background: #ffffff;
            color: #1e293b;
            border: 1px solid #cbd5e1;
        }
        .gridview-pager a:hover {
            background: #eff6ff;
            border-color: #2563eb;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="module-page-container">
        
        <!-- Header Card -->
        <div class="module-header-card">
            <div class="module-header-info">
                <h1>Inline Record Management (Insert, Update &amp; Delete)</h1>
                <p>Complete inline data maintenance utilizing <code>ShowFooter="True"</code> with dedicated validation groups for edit and insert operations.</p>
            </div>
            <a href="<%= ResolveUrl("~/AllGridProp") %>" class="btn-hub-return">&larr; Back to GridView Hub</a>
        </div>

        <!-- Content Card -->
        <div class="content-card">
            <div style="margin-bottom: 12px; display: flex; align-items: center; justify-content: space-between; flex-wrap: wrap; gap: 8px;">
                <span style="background: #eff6ff; color: #1d4ed8; border: 1px solid #bfdbfe; font-size: 12px; font-weight: 700; padding: 4px 10px; border-radius: 6px; display: inline-flex; align-items: center; gap: 6px;">
                    &#128451; Live SQL Table: <code>[Assignmet].[dbo].[Employee]</code> (Full INSERT, UPDATE, DELETE Enabled)
                </span>
            </div>

            <asp:ValidationSummary ID="ValidationSummary1" runat="server" 
                ForeColor="#b91c1c" 
                HeaderText="Please correct the following edit errors:" 
                ValidationGroup="Edit" 
                CssClass="val-summary-box" />

            <asp:ValidationSummary ID="ValidationSummaryInsert" runat="server" 
                ForeColor="#b91c1c" 
                HeaderText="Please correct the following insert errors:" 
                ValidationGroup="Insert" 
                CssClass="val-summary-box" />

            <asp:Label ID="lblStatus" runat="server" CssClass="status-msg" Visible="false"></asp:Label>

            <div class="grid-responsive-wrap">
                <asp:GridView ID="GridView1" runat="server" 
                    AutoGenerateColumns="False" 
                    DataKeyNames="EmployeeId" 
                    ShowFooter="True" 
                    CssClass="gridview-table"
                    GridLines="None"
                    AllowPaging="True"
                    PageSize="10"
                    OnPageIndexChanging="GridView1_PageIndexChanging"
                    OnRowEditing="GridView1_RowEditing"
                    OnRowCancelingEdit="GridView1_RowCancelingEdit"
                    OnRowUpdating="GridView1_RowUpdating"
                    OnRowDeleting="GridView1_RowDeleting">
                    <PagerStyle CssClass="gridview-pager" HorizontalAlign="Center" />
                    <Columns>
                        <asp:CommandField ShowDeleteButton="True" ShowEditButton="True" ValidationGroup="Edit" ControlStyle-CssClass="btn-action-link" />

                        <%-- Employee ID & Footer Insert Trigger --%>
                        <asp:TemplateField HeaderText="Employee ID" InsertVisible="False" SortExpression="EmployeeId">
                            <EditItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Eval("EmployeeId") %>'></asp:Label>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Bind("EmployeeId") %>'></asp:Label>
                            </ItemTemplate>
                            <FooterTemplate>
                                <asp:LinkButton ID="lbInsert" ValidationGroup="Insert" runat="server" OnClick="lbInsert_Click" CssClass="btn-insert-confirm">&#10010; Insert</asp:LinkButton>
                            </FooterTemplate>
                        </asp:TemplateField>

                        <%-- Employee Name --%>
                        <asp:TemplateField HeaderText="Employee Name" SortExpression="Name">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("Name") %>'></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvEditName" runat="server" 
                                    ErrorMessage="Employee Name is required."
                                    ControlToValidate="TextBox1" Text="*" ForeColor="#dc2626" ValidationGroup="Edit">
                                </asp:RequiredFieldValidator>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# Bind("Name") %>'></asp:Label>
                            </ItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtName" runat="server" placeholder="Enter Full Name"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvInsertName" runat="server" 
                                    ErrorMessage="Employee Name is required." 
                                    ControlToValidate="txtName" Text="*" ForeColor="#dc2626"
                                    ValidationGroup="Insert">
                                </asp:RequiredFieldValidator>
                            </FooterTemplate>
                        </asp:TemplateField>

                        <%-- Gender --%>
                        <asp:TemplateField HeaderText="Gender" SortExpression="Gender">
                            <EditItemTemplate>
                                <asp:DropDownList ID="DropDownList1" runat="server" SelectedValue='<%# Bind("Gender") %>'>
                                    <asp:ListItem>Select Gender</asp:ListItem>
                                    <asp:ListItem>Male</asp:ListItem>
                                    <asp:ListItem>Female</asp:ListItem>
                                </asp:DropDownList>
                                <asp:RequiredFieldValidator ID="rfvEditGender" runat="server" 
                                    ErrorMessage="Gender is required (Please select Male or Female)." Text="*"
                                    ControlToValidate="DropDownList1" ForeColor="#dc2626" 
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
                                    ErrorMessage="Gender is required (Please select Male or Female)." Text="*"
                                    ControlToValidate="ddlInsertGender" ForeColor="#dc2626" 
                                    InitialValue="Select Gender" ValidationGroup="Insert">
                                </asp:RequiredFieldValidator>
                            </FooterTemplate>
                        </asp:TemplateField>

                        <%-- City --%>
                        <asp:TemplateField HeaderText="City" SortExpression="City">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox3" runat="server" Text='<%# Bind("City") %>'></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvEditCity" runat="server" 
                                    ErrorMessage="City is required." Text="*"
                                    ControlToValidate="TextBox3" ForeColor="#dc2626" ValidationGroup="Edit">
                                </asp:RequiredFieldValidator>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label4" runat="server" Text='<%# Bind("City") %>'></asp:Label>
                            </ItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtCity" runat="server" placeholder="Enter City"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvInsertCity" runat="server" 
                                    ErrorMessage="City is required." Text="*" 
                                    ControlToValidate="txtCity" ForeColor="#dc2626" 
                                    ValidationGroup="Insert">
                                </asp:RequiredFieldValidator>
                            </FooterTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>
        </div>
    </div>
</asp:Content>
