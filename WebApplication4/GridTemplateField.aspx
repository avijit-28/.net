<%@ Page Title="TemplateField DropDownList Editing" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="GridTemplateField.aspx.cs" Inherits="WebApplication4.GridTemplateField" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .module-page-container {
            width: 100%;
            max-width: 1000px;
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
            background: #1e293b;
            color: #ffffff;
            padding: 12px 16px;
            text-align: left;
            font-weight: 700;
        }

        .gridview-table td {
            padding: 12px 16px;
            border-bottom: 1px solid #f1f5f9;
            color: #1e293b;
        }

        .gridview-table tr:nth-child(even) td {
            background: #f8fafc;
        }

        .gridview-table select,
        .gridview-table input[type="text"] {
            padding: 6px 10px;
            border: 1px solid #cbd5e1;
            border-radius: 6px;
            font-size: 13px;
            outline: none;
        }

        .gridview-table a {
            color: #2563eb;
            font-weight: 600;
            text-decoration: none;
            padding: 4px 8px;
            border-radius: 4px;
        }

        .gridview-table a:hover {
            background: #eff6ff;
            text-decoration: underline;
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
                <h1>TemplateField DropDownList Editing</h1>
                <p>Converts the Gender column into a <code>TemplateField</code>. During row editing, an interactive <code>DropDownList</code> renders with the employee's current gender pre-selected.</p>
            </div>
            <a href="<%= ResolveUrl("~/AllGridProp") %>" class="btn-hub-return">&larr; Back to GridView Hub</a>
        </div>

        <!-- Content Card -->
        <div class="content-card">
            <div style="margin-bottom: 12px; display: flex; align-items: center; justify-content: space-between; flex-wrap: wrap; gap: 8px;">
                <span style="background: #eff6ff; color: #1d4ed8; border: 1px solid #bfdbfe; font-size: 12px; font-weight: 700; padding: 4px 10px; border-radius: 6px; display: inline-flex; align-items: center; gap: 6px;">
                    &#128451; Live SQL Table: <code>[Assignmet].[dbo].[Employee]</code> (Direct UPDATE Enabled)
                </span>
            </div>

            <asp:Label ID="lblStatus" runat="server" CssClass="status-msg" Visible="false"></asp:Label>

            <div class="grid-responsive-wrap">
                <asp:GridView ID="GridView1" runat="server" 
                    AutoGenerateColumns="False" 
                    DataKeyNames="EmployeeId"
                    CssClass="gridview-table"
                    GridLines="None"
                    AllowPaging="True"
                    PageSize="10"
                    OnPageIndexChanging="GridView1_PageIndexChanging"
                    OnRowEditing="GridView1_RowEditing"
                    OnRowCancelingEdit="GridView1_RowCancelingEdit"
                    OnRowUpdating="GridView1_RowUpdating">
                    <PagerStyle CssClass="gridview-pager" HorizontalAlign="Center" />
                    <Columns>
                        <asp:CommandField ShowEditButton="True" />
                        <asp:BoundField DataField="EmployeeId" HeaderText="Employee ID" ReadOnly="True" />
                        <asp:BoundField DataField="Name" HeaderText="Employee Name" />
                        
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
                </asp:GridView>
            </div>
        </div>
    </div>
</asp:Content>
