<%@ Page Title="Employee Directory - WebService GridView" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="GridViewAll.aspx.cs" Inherits="WebApplication4.GridViewAll" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .emp-portal-container {
            width: 100%;
            max-width: 1100px;
            margin: 0 auto;
            display: flex;
            flex-direction: column;
            gap: 22px;
        }

        /* Hero Banner Card */
        .emp-header-card {
            background: linear-gradient(135deg, #0f172a 0%, #1e293b 100%);
            border-radius: 16px;
            padding: 26px 32px;
            color: #ffffff;
            box-shadow: 0 10px 25px -5px rgba(15, 23, 42, 0.25);
            border: 1px solid rgba(255, 255, 255, 0.1);
            display: flex;
            justify-content: space-between;
            align-items: center;
            flex-wrap: wrap;
            gap: 20px;
        }

        .emp-badge {
            display: inline-flex;
            align-items: center;
            gap: 6px;
            background: rgba(37, 99, 235, 0.35);
            border: 1px solid rgba(147, 197, 253, 0.3);
            color: #bfdbfe;
            font-size: 11.5px;
            font-weight: 700;
            padding: 4px 12px;
            border-radius: 20px;
            letter-spacing: 0.05em;
            text-transform: uppercase;
        }

        .emp-title {
            font-size: 24px;
            font-weight: 800;
            color: #ffffff;
            margin-top: 6px;
            letter-spacing: -0.02em;
        }

        .emp-subtitle {
            font-size: 13.5px;
            color: #94a3b8;
            margin-top: 4px;
        }

        .emp-actions-group {
            display: flex;
            align-items: center;
            gap: 10px;
            flex-wrap: wrap;
        }

        .btn-header-action {
            display: inline-flex;
            align-items: center;
            gap: 6px;
            background: rgba(255, 255, 255, 0.1);
            color: #ffffff;
            border: 1px solid rgba(255, 255, 255, 0.2);
            padding: 8px 16px;
            border-radius: 8px;
            font-size: 13px;
            font-weight: 600;
            text-decoration: none;
            transition: all 0.2s ease;
        }

        .btn-header-action:hover {
            background: rgba(255, 255, 255, 0.2);
            color: #ffffff;
            transform: translateY(-1px);
        }

        /* Filter & Search Bar Card */
        .emp-filter-card {
            background: #ffffff;
            border-radius: 14px;
            padding: 20px 24px;
            box-shadow: 0 4px 16px rgba(0, 0, 0, 0.04);
            border: 1.5px solid #e2e8f0;
            display: flex;
            align-items: center;
            justify-content: space-between;
            flex-wrap: wrap;
            gap: 16px;
        }

        .filter-controls {
            display: flex;
            align-items: center;
            gap: 12px;
            flex-wrap: wrap;
            flex: 1;
        }

        .filter-label {
            font-size: 14px;
            font-weight: 700;
            color: #1e293b;
            display: flex;
            align-items: center;
            gap: 6px;
            white-space: nowrap;
        }

        .filter-input-wrap {
            position: relative;
            display: inline-flex;
            align-items: center;
        }

        .filter-icon {
            position: absolute;
            left: 12px;
            font-size: 13px;
            color: #64748b;
            pointer-events: none;
        }

        .filter-textbox {
            width: 280px;
            max-width: 100%;
            padding: 9px 12px 9px 34px;
            border: 1.5px solid #cbd5e1;
            border-radius: 8px;
            font-size: 13.5px;
            color: #0f172a;
            font-family: inherit;
            transition: all 0.2s ease;
        }

        .filter-textbox:focus {
            outline: none;
            border-color: #2563eb;
            box-shadow: 0 0 0 3px rgba(37, 99, 235, 0.15);
        }

        .btn-search {
            display: inline-flex;
            align-items: center;
            gap: 6px;
            padding: 9px 18px;
            background: linear-gradient(135deg, #2563eb 0%, #1d4ed8 100%);
            color: #ffffff;
            border: none;
            border-radius: 8px;
            font-size: 13.5px;
            font-weight: 600;
            cursor: pointer;
            transition: all 0.2s ease;
            box-shadow: 0 2px 6px rgba(37, 99, 235, 0.25);
            font-family: inherit;
        }

        .btn-search:hover {
            background: linear-gradient(135deg, #1d4ed8 0%, #1e40af 100%);
            transform: translateY(-1px);
            box-shadow: 0 4px 10px rgba(37, 99, 235, 0.35);
        }

        .btn-reset {
            display: inline-flex;
            align-items: center;
            gap: 6px;
            padding: 9px 16px;
            background: #f1f5f9;
            color: #475569;
            border: 1.5px solid #cbd5e1;
            border-radius: 8px;
            font-size: 13.5px;
            font-weight: 600;
            cursor: pointer;
            transition: all 0.2s ease;
            font-family: inherit;
        }

        .btn-reset:hover {
            background: #e2e8f0;
            color: #0f172a;
        }

        .record-count-badge {
            display: inline-flex;
            align-items: center;
            gap: 6px;
            padding: 6px 14px;
            background: #eff6ff;
            color: #1e40af;
            border: 1px solid #bfdbfe;
            border-radius: 20px;
            font-size: 12.5px;
            font-weight: 700;
            white-space: nowrap;
        }

        /* Message / Alert Card */
        .emp-alert {
            padding: 14px 18px;
            border-radius: 10px;
            font-size: 13.5px;
            font-weight: 500;
            display: flex;
            align-items: center;
            gap: 10px;
        }

        .emp-alert-warning {
            background: #fffbeb;
            color: #b45309;
            border: 1px solid #fde68a;
        }

        /* GridView Container Card */
        .emp-grid-card {
            background: #ffffff;
            border-radius: 16px;
            padding: 24px;
            box-shadow: 0 4px 16px rgba(0, 0, 0, 0.05);
            border: 1.5px solid #e2e8f0;
            overflow-x: auto;
        }

        .modern-grid {
            width: 100%;
            border-collapse: separate;
            border-spacing: 0;
            border: 1px solid #e2e8f0;
            border-radius: 12px;
            overflow: hidden;
            font-size: 13.5px;
        }

        .modern-grid th {
            background: linear-gradient(135deg, #1e293b 0%, #0f172a 100%);
            color: #ffffff;
            padding: 14px 16px;
            font-size: 12.5px;
            font-weight: 700;
            text-transform: uppercase;
            letter-spacing: 0.05em;
            text-align: left;
            border: none;
            white-space: nowrap;
        }

        .modern-grid th.col-center {
            text-align: center;
        }

        .modern-grid td {
            padding: 13px 16px;
            border-top: 1px solid #f1f5f9;
            color: #1e293b;
            vertical-align: middle;
        }

        .modern-grid tr:nth-child(even) td {
            background-color: #f8fafc;
        }

        .modern-grid tr:hover td {
            background-color: #eff6ff !important;
        }

        /* Cell Styling Badges */
        .id-badge {
            display: inline-flex;
            align-items: center;
            justify-content: center;
            width: 28px;
            height: 28px;
            background: #e2e8f0;
            color: #334155;
            border-radius: 50%;
            font-weight: 700;
            font-size: 12px;
        }

        .emp-name-cell {
            display: flex;
            align-items: center;
            gap: 10px;
            font-weight: 700;
            color: #0f172a;
        }

        .emp-avatar {
            width: 32px;
            height: 32px;
            background: #dbeafe;
            color: #1d4ed8;
            border-radius: 8px;
            display: inline-flex;
            align-items: center;
            justify-content: center;
            font-size: 14px;
            font-weight: 700;
        }

        .dept-badge {
            display: inline-flex;
            align-items: center;
            gap: 4px;
            padding: 3px 10px;
            background: #f1f5f9;
            color: #334155;
            border: 1px solid #cbd5e1;
            border-radius: 6px;
            font-size: 12px;
            font-weight: 600;
        }

        .salary-badge {
            font-weight: 700;
            color: #059669;
            font-size: 13.5px;
            display: inline-flex;
            align-items: center;
            gap: 4px;
        }

        .gender-badge {
            display: inline-flex;
            align-items: center;
            gap: 4px;
            padding: 3px 9px;
            border-radius: 12px;
            font-size: 11.5px;
            font-weight: 700;
            text-transform: capitalize;
        }

        .gender-male {
            background: #eff6ff;
            color: #1d4ed8;
            border: 1px solid #bfdbfe;
        }

        .gender-female {
            background: #fff1f2;
            color: #be123c;
            border: 1px solid #fecdd3;
        }

        .city-cell {
            display: inline-flex;
            align-items: center;
            gap: 5px;
            color: #475569;
        }

        /* Custom Modern Pager */
        .custom-pager {
            background: #ffffff !important;
            border-top: 1.5px solid #e2e8f0 !important;
            padding: 14px 16px !important;
        }

        .custom-pager table {
            margin: 0 auto;
        }

        .custom-pager td {
            padding: 2px 4px !important;
            border: none !important;
            background: transparent !important;
        }

        .custom-pager a,
        .custom-pager span {
            display: inline-flex;
            align-items: center;
            justify-content: center;
            min-width: 32px;
            height: 32px;
            padding: 0 8px;
            border-radius: 6px;
            font-size: 13px;
            font-weight: 600;
            text-decoration: none;
            transition: all 0.15s ease;
        }

        .custom-pager a {
            color: #475569;
            background: #f8fafc;
            border: 1px solid #cbd5e1;
        }

        .custom-pager a:hover {
            color: #ffffff;
            background: #2563eb;
            border-color: #2563eb;
            box-shadow: 0 2px 6px rgba(37, 99, 235, 0.25);
        }

        .custom-pager span {
            color: #ffffff;
            background: #2563eb;
            border: 1px solid #2563eb;
            box-shadow: 0 2px 6px rgba(37, 99, 235, 0.25);
        }

        /* Service Info Banner */
        .service-info-footer {
            background: #f8fafc;
            border: 1px dashed #cbd5e1;
            border-radius: 12px;
            padding: 16px 20px;
            display: flex;
            align-items: center;
            justify-content: space-between;
            font-size: 12.5px;
            color: #64748b;
            flex-wrap: wrap;
            gap: 12px;
        }

        .service-info-footer code {
            background: #e2e8f0;
            color: #0f172a;
            padding: 2px 6px;
            border-radius: 4px;
            font-family: Consolas, monospace;
            font-size: 12px;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="emp-portal-container">
        <!-- Hero Header -->
        <div class="emp-header-card">
            <div>
                <span class="emp-badge">&#9881; ASMX WEB SERVICE &bull; DATA BINDING &amp; PAGING</span>
                <h1 class="emp-title">Employee Directory (GridView All)</h1>
                <p class="emp-subtitle">Dynamic data binding with custom server-side paging consuming WebService1.asmx</p>
            </div>
            <div class="emp-actions-group">
                <a href="<%= ResolveUrl("~/GetUserDetails") %>" class="btn-header-action">&#128269; User Search WS</a>
                <a href="<%= ResolveUrl("~/Home") %>" class="btn-header-action">&#127968; Home Portal</a>
            </div>
        </div>

        <!-- Filter & Search Controls -->
        <asp:Panel ID="pnlFilter" runat="server" DefaultButton="btnSearch" CssClass="emp-filter-card">
            <div class="filter-controls">
                <span class="filter-label">&#128269; Filter by Name:</span>
                <div class="filter-input-wrap">
                    <span class="filter-icon">&#128100;</span>
                    <asp:TextBox ID="txtSearch" runat="server" CssClass="filter-textbox" placeholder="Search employee by name..." autocomplete="off"></asp:TextBox>
                </div>
                <asp:Button ID="btnSearch" runat="server" Text="Filter" OnClick="btnSearch_Click" CssClass="btn-search" CausesValidation="false" />
                <asp:Button ID="btnReset" runat="server" Text="Reset / Show All" OnClick="btnReset_Click" CssClass="btn-reset" CausesValidation="false" />
            </div>
            <asp:Label ID="lblRecordCount" runat="server" CssClass="record-count-badge" Text="Loading..."></asp:Label>
        </asp:Panel>

        <!-- Optional Warning/Error Message -->
        <asp:Panel ID="pnlMessage" runat="server" Visible="false" CssClass="emp-alert emp-alert-warning">
            <span>&#9888;</span>
            <asp:Label ID="lblMessage" runat="server"></asp:Label>
        </asp:Panel>

        <!-- GridView Card with Paging -->
        <div class="emp-grid-card">
            <asp:GridView ID="gvEmployees" runat="server"
                AutoGenerateColumns="False"
                AllowPaging="True"
                PageSize="5"
                OnPageIndexChanging="gvEmployees_PageIndexChanging"
                CssClass="modern-grid"
                GridLines="None">
                <Columns>
                    <asp:TemplateField HeaderText="ID" ItemStyle-Width="60px" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="col-center">
                        <ItemTemplate>
                            <span class="id-badge"><%# Eval("ID") %></span>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Employee Name">
                        <ItemTemplate>
                            <div class="emp-name-cell">
                                <span class="emp-avatar">&#128100;</span>
                                <span><%# Eval("Name") %></span>
                            </div>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Department">
                        <ItemTemplate>
                            <span class="dept-badge">&#128188; <%# Eval("Dept") %></span>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Salary">
                        <ItemTemplate>
                            <span class="salary-badge">&#36; <%# string.Format("{0:N2}", Eval("Salary")) %></span>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Gender" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="col-center">
                        <ItemTemplate>
                            <span class='<%# "gender-badge " + (Eval("Gender") != null && Eval("Gender").ToString().Trim().ToLower() == "male" ? "gender-male" : "gender-female") %>'>
                                <%# Eval("Gender") %>
                            </span>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="City">
                        <ItemTemplate>
                            <span class="city-cell">&#128205; <%# Eval("City") %></span>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>

                <PagerSettings Mode="NumericFirstLast"
                    FirstPageText="&laquo; First"
                    PreviousPageText="&lsaquo; Prev"
                    NextPageText="Next &rsaquo;"
                    LastPageText="Last &raquo;"
                    PageButtonCount="5"
                    Position="Bottom" />

                <PagerStyle CssClass="custom-pager" HorizontalAlign="Center" />

                <EmptyDataTemplate>
                    <div style="padding: 36px 20px; text-align: center; color: #64748b;">
                        <div style="font-size: 32px; margin-bottom: 8px;">&#128194;</div>
                        <h4 style="font-size: 16px; font-weight: 700; color: #1e293b; margin-bottom: 6px;">No Employee Records Found</h4>
                        <p style="font-size: 13px; color: #64748b; margin-bottom: 14px;">Try searching with a different employee name prefix or reset the filter.</p>
                        <asp:Button ID="btnEmptyReset" runat="server" Text="Reset Search Filter" OnClick="btnReset_Click" CssClass="btn-reset" CausesValidation="false" />
                    </div>
                </EmptyDataTemplate>
            </asp:GridView>
        </div>

        <!-- Technical Integration Notes -->
        <div class="service-info-footer">
            <div>
                <strong>Connected Endpoint:</strong> <code>~/WebService1.asmx/GetEmployeeDetails</code> (SQL Server <code>Assignmet..Employee</code>)
            </div>
            <div>
                <strong>Page Mode:</strong> Server-side <code>AllowPaging="True"</code> with <code>PageSize="5"</code>
            </div>
        </div>
    </div>
</asp:Content>
