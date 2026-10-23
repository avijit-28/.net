<%@ Page Title="User Directory - Search" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="SearchUser.aspx.cs" Inherits="MiniProject.SearchUser" %>
<%@ MasterType VirtualPath="~/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .directory-container {
            width: 100%;
            max-width: 1080px;
            margin: 0 auto;
            display: flex;
            flex-direction: column;
            gap: 20px;
        }

        /* Hero Card Header */
        .directory-header-card {
            background: linear-gradient(135deg, #1e293b 0%, #0f172a 100%);
            border-radius: 16px;
            padding: 28px 32px;
            color: #ffffff;
            box-shadow: 0 10px 25px -5px rgba(15, 23, 42, 0.25);
            border: 1px solid rgba(255, 255, 255, 0.1);
            display: flex;
            justify-content: space-between;
            align-items: center;
            flex-wrap: wrap;
            gap: 20px;
        }

        .header-content {
            display: flex;
            flex-direction: column;
            gap: 6px;
        }

        .demo-badge {
            display: inline-flex;
            align-items: center;
            gap: 6px;
            background: rgba(37, 99, 235, 0.35);
            border: 1px solid rgba(147, 197, 253, 0.3);
            color: #bfdbfe;
            font-size: 12px;
            font-weight: 600;
            padding: 4px 12px;
            border-radius: 20px;
            letter-spacing: 0.04em;
            text-transform: uppercase;
            align-self: flex-start;
        }

        .header-title {
            font-size: 26px;
            font-weight: 800;
            letter-spacing: -0.02em;
            color: #ffffff;
            margin: 0;
            display: flex;
            align-items: center;
            gap: 10px;
        }

        .header-subtitle {
            font-size: 14px;
            color: #94a3b8;
            margin: 0;
            line-height: 1.5;
        }

        /* Demo Notice Callout */
        .demo-notice {
            background: #eff6ff;
            border-left: 4px solid #2563eb;
            border-radius: 8px;
            padding: 12px 18px;
            display: flex;
            align-items: center;
            justify-content: space-between;
            flex-wrap: wrap;
            gap: 12px;
            font-size: 13.5px;
            color: #1e40af;
        }

        .demo-notice-text {
            display: flex;
            align-items: center;
            gap: 8px;
        }

        /* Status & Filter Toolbar */
        .filter-toolbar {
            display: flex;
            align-items: center;
            justify-content: space-between;
            flex-wrap: wrap;
            gap: 12px;
            background: #ffffff;
            border: 1px solid #e2e8f0;
            border-radius: 12px;
            padding: 14px 20px;
            box-shadow: 0 2px 6px rgba(0, 0, 0, 0.03);
        }

        .filter-status-group {
            display: flex;
            align-items: center;
            gap: 10px;
            flex-wrap: wrap;
        }

        .status-pill {
            display: inline-flex;
            align-items: center;
            gap: 6px;
            padding: 6px 14px;
            border-radius: 20px;
            font-size: 13px;
            font-weight: 600;
        }

        .status-pill-count {
            background: #f1f5f9;
            color: #334155;
            border: 1px solid #cbd5e1;
        }

        .status-pill-filter {
            background: #fef3c7;
            color: #92400e;
            border: 1px solid #fde68a;
        }

        .btn-clear-filter {
            display: inline-flex;
            align-items: center;
            gap: 6px;
            padding: 6px 14px;
            background: #fee2e2;
            color: #b91c1c;
            border: 1px solid #fca5a5;
            border-radius: 20px;
            font-size: 12.5px;
            font-weight: 600;
            cursor: pointer;
            transition: all 0.2s ease;
            text-decoration: none;
            font-family: inherit;
        }

        .btn-clear-filter:hover {
            background: #fecaca;
            color: #991b1b;
            transform: translateY(-1px);
        }

        /* GridView Table Container Card */
        .table-card {
            background: #ffffff;
            border-radius: 16px;
            border: 1px solid #e2e8f0;
            box-shadow: 0 4px 16px rgba(0, 0, 0, 0.05);
            overflow: hidden;
        }

        .table-responsive {
            width: 100%;
            overflow-x: auto;
        }

        /* GridView Styles */
        .gridview-table {
            width: 100%;
            border-collapse: collapse;
            text-align: left;
            font-size: 13.5px;
            font-family: inherit;
        }

        .gridview-table th {
            background: #0f172a;
            color: #ffffff;
            font-weight: 600;
            text-transform: uppercase;
            font-size: 12px;
            letter-spacing: 0.05em;
            padding: 14px 18px;
            border: none;
            white-space: nowrap;
        }

        .gridview-table td {
            padding: 14px 18px;
            border-top: 1px solid #f1f5f9;
            color: #334155;
            vertical-align: middle;
        }

        .gridview-row {
            background: #ffffff;
            transition: background-color 0.15s ease;
        }

        .gridview-alt-row {
            background: #fafafa;
            transition: background-color 0.15s ease;
        }

        .gridview-row:hover,
        .gridview-alt-row:hover {
            background: #eff6ff !important;
        }

        /* Cell Component Elements */
        .badge-id {
            display: inline-block;
            padding: 3px 10px;
            background: #e0f2fe;
            color: #0369a1;
            font-weight: 700;
            font-size: 12px;
            border-radius: 12px;
            border: 1px solid #bae6fd;
        }

        .User-name-cell {
            display: flex;
            align-items: center;
            gap: 10px;
            font-weight: 600;
            color: #0f172a;
        }

        .User-avatar {
            width: 32px;
            height: 32px;
            border-radius: 50%;
            background: linear-gradient(135deg, #3b82f6 0%, #1d4ed8 100%);
            color: #ffffff;
            font-size: 13px;
            font-weight: 700;
            display: flex;
            align-items: center;
            justify-content: center;
            flex-shrink: 0;
            box-shadow: 0 2px 4px rgba(37, 99, 235, 0.25);
        }

        .email-link {
            color: #2563eb;
            text-decoration: none;
            display: inline-flex;
            align-items: center;
            gap: 6px;
        }

        .email-link:hover {
            text-decoration: underline;
            color: #1d4ed8;
        }

        .badge-city {
            display: inline-block;
            padding: 4px 12px;
            background: #ecfdf5;
            color: #065f46;
            font-size: 12px;
            font-weight: 600;
            border-radius: 12px;
            border: 1px solid #a7f3d0;
        }

        .phone-display {
            display: inline-flex;
            align-items: center;
            gap: 6px;
            color: #475569;
            font-family: monospace;
            font-size: 13px;
        }

        /* Empty Data Template */
        .empty-state {
            padding: 60px 24px;
            text-align: center;
            background: #ffffff;
        }

        .empty-icon {
            font-size: 48px;
            color: #94a3b8;
            margin-bottom: 14px;
        }

        .empty-title {
            font-size: 18px;
            font-weight: 700;
            color: #1e293b;
            margin-bottom: 8px;
        }

        .empty-desc {
            font-size: 14px;
            color: #64748b;
            max-width: 480px;
            margin: 0 auto 20px;
            line-height: 1.5;
        }

        /* Navigation Footer */
        .nav-footer-card {
            display: flex;
            justify-content: space-between;
            align-items: center;
            flex-wrap: wrap;
            gap: 14px;
            padding: 16px 20px;
            background: #ffffff;
            border: 1px solid #e2e8f0;
            border-radius: 12px;
        }

        .btn-nav-back {
            display: inline-flex;
            align-items: center;
            gap: 8px;
            padding: 10px 18px;
            background: #f8fafc;
            color: #334155;
            border: 1.5px solid #cbd5e1;
            border-radius: 8px;
            font-size: 13.5px;
            font-weight: 600;
            cursor: pointer;
            transition: all 0.2s ease;
            text-decoration: none;
            font-family: inherit;
        }

        .btn-nav-back:hover {
            background: #f1f5f9;
            color: #0f172a;
            border-color: #94a3b8;
            box-shadow: 0 2px 6px rgba(0, 0, 0, 0.05);
        }

        .quick-links {
            display: flex;
            gap: 14px;
            align-items: center;
            font-size: 13px;
        }

        .quick-link-item {
            color: #2563eb;
            text-decoration: none;
            font-weight: 500;
            transition: color 0.2s;
        }

        .quick-link-item:hover {
            color: #1d4ed8;
            text-decoration: underline;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="directory-container">
        
        <!-- Header Banner -->
        <div class="directory-header-card">
            <div class="header-content">
                <span class="demo-badge">&#9881; Master &rarr; Content Data Passing</span>
                <h1 class="header-title">&#128101; User &amp; User Directory</h1>
                <p class="header-subtitle">
                    Live record viewer bound to database table <code>[Assignmet].[dbo].[UserDetails]</code>. Filter records by entering keywords in the Master Page search bar above.
                </p>
            </div>
        </div>

        <!-- Callout Banner -->
        <div class="demo-notice">
            <div class="demo-notice-text">
                <span>&#128161;</span>
                <span><strong>How to search:</strong> Enter any User's name into the search bar in the top navigation header and click <strong>Search</strong>.</span>
            </div>
            <div>
                <asp:Button ID="btnShowAllTop" runat="server" Text="&#128260; View All Records" OnClick="btnShowAll_Click" CssClass="btn-clear-filter" CausesValidation="false" />
            </div>
        </div>

        <!-- Filter Status Bar -->
        <div class="filter-toolbar">
            <div class="filter-status-group">
                <!-- Record count pill -->
                <div class="status-pill status-pill-count">
                    <span>&#128196;</span>
                    <asp:Label ID="lblRecordCount" runat="server" Text="Loading records..."></asp:Label>
                </div>

                <!-- Active search filter pill -->
                <asp:Panel ID="pnlActiveFilter" runat="server" Visible="false" CssClass="status-pill status-pill-filter">
                    <span>&#128269; Filtered by:</span>
                    <strong>&ldquo;<asp:Label ID="lblFilterTerm" runat="server"></asp:Label>&rdquo;</strong>
                </asp:Panel>
            </div>

            <!-- Clear button -->
            <asp:Button ID="btnShowAll" runat="server" Text="&#10005; Clear Filter" OnClick="btnShowAll_Click" CssClass="btn-clear-filter" Visible="false" CausesValidation="false" />
        </div>

        <!-- Main GridView Table -->
        <div class="table-card">
            <div class="table-responsive">
                <asp:GridView ID="GridViewUsers" runat="server" 
                    AutoGenerateColumns="False" 
                    GridLines="None"
                    CssClass="gridview-table"
                    RowStyle-CssClass="gridview-row"
                    AlternatingRowStyle-CssClass="gridview-alt-row"
                    EmptyDataText="">
                    <Columns>
                        <%-- ID Column --%>
                        <asp:TemplateField HeaderText="User ID" HeaderStyle-Width="90px" ItemStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <span class="badge-id">#<%# Eval("UserId") %></span>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <%-- User / User Name Column --%>
                        <asp:TemplateField HeaderText="User / User Name" HeaderStyle-Width="220px">
                            <ItemTemplate>
                                <div class="User-name-cell">
                                    <div class="User-avatar">
                                        <%# Eval("UserName") != null && Eval("UserName").ToString().Length > 0 ? Eval("UserName").ToString().Substring(0, 1).ToUpper() : "U" %>
                                    </div>
                                    <a href='<%# ResolveUrl("~/GetUser?user=" + HttpUtility.UrlEncode(Eval("UserName").ToString()) + "&email=" + HttpUtility.UrlEncode(Eval("Email").ToString()) + "&id=" + Eval("UserId")) %>' 
                                       title="View details via QueryString" style="color:#1d4ed8;text-decoration:none;font-weight:700;">
                                        <%# Eval("UserName") %>
                                    </a>
                                </div>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <%-- Email Column --%>
                        <asp:TemplateField HeaderText="Email Address" HeaderStyle-Width="240px">
                            <ItemTemplate>
                                <a href='<%# "mailto:" + Eval("Email") %>' class="email-link">
                                    <span>&#9993;</span>
                                    <span><%# Eval("Email") %></span>
                                </a>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <%-- Mobile Number Column --%>
                        <asp:TemplateField HeaderText="Mobile No" HeaderStyle-Width="150px">
                            <ItemTemplate>
                                <span class="phone-display">
                                    <span>&#128222;</span>
                                    <span><%# Eval("MobileNo") %></span>
                                </span>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <%-- City Column --%>
                        <asp:TemplateField HeaderText="City" HeaderStyle-Width="130px">
                            <ItemTemplate>
                                <span class="badge-city">
                                    &#128205; <%# Eval("City") %>
                                </span>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <%-- QueryString Action Column --%>
                        <asp:TemplateField HeaderText="Profile via QueryString" HeaderStyle-Width="160px" ItemStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <a href='<%# ResolveUrl("~/GetUser?user=" + HttpUtility.UrlEncode(Eval("UserName").ToString()) + "&email=" + HttpUtility.UrlEncode(Eval("Email").ToString()) + "&id=" + Eval("UserId")) %>' 
                                   class="btn-view-profile" title="Pass user details via QueryString to GetUser.aspx">
                                    &#128100; View Profile &rarr;
                                </a>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>

                    <EmptyDataTemplate>
                        <div class="empty-state">
                            <div class="empty-icon">&#128269;</div>
                            <h3 class="empty-title">No Users Found</h3>
                            <p class="empty-desc">
                                We couldn't find any Users whose name contains your search keyword. Try another name or click below to view the full directory.
                            </p>
                            <asp:Button ID="btnEmptyReset" runat="server" Text="&#128260; Show All Users" OnClick="btnShowAll_Click" CssClass="btn-clear-filter" CausesValidation="false" />
                        </div>
                    </EmptyDataTemplate>
                </asp:GridView>
            </div>
        </div>

        <!-- Navigation Footer -->
        <div class="nav-footer-card">
            <asp:Button ID="BtnGoToSignUp" runat="server" Text="&larr; Return to User Dashboard" OnClick="BtnGoToSignUp_Click" CssClass="btn-nav-back" CausesValidation="false" />
            
            <div class="quick-links">
                <span>Quick links:</span>
                <a href="<%= ResolveUrl("~/Home") %>" class="quick-link-item">&#127968; Home</a>
                <span>&bull;</span>
                <a href="<%= ResolveUrl("~/ProductForm") %>" class="quick-link-item">&#128230; Products</a>
                <span>&bull;</span>
                <a href="<%= ResolveUrl("~/CheckAvailability") %>" class="quick-link-item">&#9989; Availability</a>
            </div>
        </div>

    </div>
</asp:Content>
