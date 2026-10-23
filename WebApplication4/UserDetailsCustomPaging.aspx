<%@ Page Title="User Details (Custom Stored Procedure Paging)" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="UserDetailsCustomPaging.aspx.cs" Inherits="WebApplication4.UserDetailsCustomPaging" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .user-mgmt-container {
            width: 100%;
            max-width: 1180px;
            margin: 0 auto;
            display: flex;
            flex-direction: column;
            gap: 22px;
        }

        /* Hero Header Banner */
        .user-hero-card {
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
            gap: 18px;
            position: relative;
            overflow: hidden;
        }

        .user-hero-card::after {
            content: "";
            position: absolute;
            right: -30px;
            bottom: -30px;
            width: 180px;
            height: 180px;
            background: radial-gradient(circle, rgba(168, 85, 247, 0.2) 0%, transparent 70%);
            pointer-events: none;
        }

        .user-hero-info h1 {
            font-size: 24px;
            font-weight: 800;
            color: #ffffff;
            margin: 0 0 6px 0;
            letter-spacing: -0.02em;
        }

        .user-hero-info p {
            font-size: 13.5px;
            color: #94a3b8;
            margin: 0;
            max-width: 800px;
            line-height: 1.5;
        }

        .hero-badge-row {
            display: flex;
            flex-wrap: wrap;
            gap: 8px;
            margin-bottom: 10px;
        }

        .hero-badge {
            display: inline-flex;
            align-items: center;
            gap: 5px;
            background: rgba(168, 85, 247, 0.2);
            border: 1px solid rgba(168, 85, 247, 0.4);
            color: #d8b4fe;
            font-size: 11px;
            font-weight: 700;
            padding: 3px 10px;
            border-radius: 20px;
            letter-spacing: 0.04em;
            text-transform: uppercase;
        }

        .hero-badge-blue {
            background: rgba(56, 189, 248, 0.18);
            border: 1px solid rgba(56, 189, 248, 0.35);
            color: #7dd3fc;
        }

        .hero-badge-green {
            background: rgba(34, 197, 94, 0.18);
            border: 1px solid rgba(34, 197, 94, 0.35);
            color: #86efac;
        }

        .hero-nav-actions {
            display: flex;
            align-items: center;
            gap: 10px;
            flex-wrap: wrap;
            z-index: 1;
        }

        .btn-hero-link {
            display: inline-flex;
            align-items: center;
            gap: 6px;
            padding: 9px 16px;
            background: rgba(255, 255, 255, 0.1);
            color: #ffffff !important;
            border: 1px solid rgba(255, 255, 255, 0.2);
            border-radius: 10px;
            font-size: 13px;
            font-weight: 600;
            text-decoration: none;
            transition: all 0.2s ease;
        }

        .btn-hero-link:hover {
            background: rgba(255, 255, 255, 0.2);
            transform: translateY(-1px);
        }

        /* Live Metrics Cards */
        .metrics-grid {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
            gap: 16px;
        }

        .metric-card {
            background: #ffffff;
            border: 1px solid #e2e8f0;
            border-radius: 14px;
            padding: 16px 20px;
            display: flex;
            align-items: center;
            gap: 14px;
            box-shadow: 0 2px 8px rgba(0, 0, 0, 0.02);
            transition: transform 0.2s ease;
        }

        .metric-card:hover {
            transform: translateY(-2px);
            border-color: #cbd5e1;
        }

        .metric-icon {
            width: 44px;
            height: 44px;
            border-radius: 10px;
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 22px;
            flex-shrink: 0;
        }

        .metric-purple { background: #faf5ff; color: #9333ea; }
        .metric-blue { background: #eff6ff; color: #2563eb; }
        .metric-amber { background: #fffbeb; color: #d97706; }
        .metric-emerald { background: #ecfdf5; color: #059669; }

        .metric-content {
            display: flex;
            flex-direction: column;
        }

        .metric-label {
            font-size: 11.5px;
            font-weight: 600;
            color: #64748b;
            text-transform: uppercase;
            letter-spacing: 0.04em;
        }

        .metric-value {
            font-size: 20px;
            font-weight: 800;
            color: #0f172a;
        }

        /* Main Content Card */
        .content-card {
            background: #ffffff;
            border-radius: 16px;
            border: 1px solid #e2e8f0;
            box-shadow: 0 4px 14px rgba(0, 0, 0, 0.03);
            overflow: hidden;
        }

        .action-toolbar {
            padding: 18px 24px;
            background: #f8fafc;
            border-bottom: 1px solid #e2e8f0;
            display: flex;
            justify-content: space-between;
            align-items: center;
            flex-wrap: wrap;
            gap: 14px;
        }

        .sql-badge-group {
            display: flex;
            align-items: center;
            flex-wrap: wrap;
            gap: 8px;
        }

        .sql-pill {
            font-family: monospace;
            font-size: 11.5px;
            background: #f1f5f9;
            color: #334155;
            padding: 4px 10px;
            border-radius: 6px;
            border: 1px solid #cbd5e1;
            font-weight: 600;
        }

        .sql-pill-sp {
            font-family: monospace;
            font-size: 11.5px;
            background: #f5f3ff;
            color: #7c3aed;
            padding: 4px 10px;
            border-radius: 6px;
            border: 1px solid #ddd6fe;
            font-weight: 600;
        }

        .table-search-box {
            display: flex;
            align-items: center;
            background: #ffffff;
            border: 1px solid #cbd5e1;
            border-radius: 8px;
            padding: 6px 12px;
            gap: 8px;
            width: 250px;
        }

        .table-search-input {
            border: none;
            outline: none;
            font-size: 13px;
            width: 100%;
            background: transparent;
        }

        /* Status Alert Banner */
        .status-msg {
            display: block;
            margin: 16px 24px 0 24px;
            padding: 12px 18px;
            border-radius: 10px;
            font-size: 13.5px;
            font-weight: 600;
            background: #ecfdf5;
            border: 1px solid #a7f3d0;
            color: #065f46;
        }

        .status-msg-error {
            background: #fef2f2 !important;
            border-color: #fecaca !important;
            color: #991b1b !important;
        }

        /* Table Styling */
        .grid-responsive-wrap {
            overflow-x: auto;
            width: 100%;
        }

        .gridview-table {
            width: 100%;
            border-collapse: separate;
            border-spacing: 0;
            font-size: 13px;
        }

        .gridview-table th {
            background: #f8fafc;
            color: #475569;
            font-weight: 700;
            text-transform: uppercase;
            font-size: 11px;
            letter-spacing: 0.05em;
            padding: 14px 18px;
            border-bottom: 2px solid #e2e8f0;
            text-align: left;
            white-space: nowrap;
        }

        .gridview-table td {
            padding: 13px 18px;
            border-bottom: 1px solid #f1f5f9;
            color: #1e293b;
            vertical-align: middle;
        }

        .gridview-table tr:hover td {
            background: #f8fafc;
        }

        .gridview-table tr:last-child td {
            border-bottom: none;
        }

        /* Footer Row Styling (Insert row) */
        .gridview-table tr.gridview-footer td,
        .gridview-table tfoot tr td {
            background: #f8fafc;
            border-top: 2px solid #e2e8f0;
            padding: 12px 18px;
        }

        /* Action Buttons */
        .btn-action-edit,
        .btn-action-delete,
        .btn-action-update,
        .btn-action-cancel,
        .btn-insert-user {
            display: inline-flex;
            align-items: center;
            gap: 4px;
            padding: 6px 12px;
            border-radius: 6px;
            font-size: 12px;
            font-weight: 600;
            text-decoration: none;
            cursor: pointer;
            border: none;
            transition: all 0.15s ease;
        }

        .btn-action-edit {
            background: #eff6ff;
            color: #1d4ed8 !important;
            border: 1px solid #bfdbfe;
        }
        .btn-action-edit:hover {
            background: #dbeafe;
            color: #1e40af !important;
        }

        .btn-action-delete {
            background: #fef2f2;
            color: #b91c1c !important;
            border: 1px solid #fecaca;
            margin-left: 6px;
        }
        .btn-action-delete:hover {
            background: #fee2e2;
            color: #991b1b !important;
        }

        .btn-action-update {
            background: #ecfdf5;
            color: #047857 !important;
            border: 1px solid #a7f3d0;
        }
        .btn-action-update:hover {
            background: #d1fae5;
            color: #065f46 !important;
        }

        .btn-action-cancel {
            background: #f1f5f9;
            color: #475569 !important;
            border: 1px solid #cbd5e1;
            margin-left: 6px;
        }
        .btn-action-cancel:hover {
            background: #e2e8f0;
            color: #1e293b !important;
        }

        .btn-insert-user {
            background: #2563eb;
            color: #ffffff !important;
            border: 1px solid #1d4ed8;
            padding: 7px 16px;
        }
        .btn-insert-user:hover {
            background: #1d4ed8;
            transform: translateY(-1px);
        }

        /* Input Elements */
        .table-input {
            width: 100%;
            padding: 7px 10px;
            border: 1px solid #cbd5e1;
            border-radius: 6px;
            font-size: 12.5px;
            outline: none;
            box-sizing: border-box;
            background: #ffffff;
            transition: border-color 0.15s;
        }

        .table-input:focus {
            border-color: #2563eb;
            box-shadow: 0 0 0 2px rgba(37, 99, 235, 0.18);
        }

        .id-badge {
            background: #f1f5f9;
            color: #334155;
            font-weight: 700;
            font-size: 12px;
            padding: 3px 8px;
            border-radius: 6px;
            border: 1px solid #e2e8f0;
            display: inline-block;
        }

        .city-pill {
            background: #f0fdfa;
            color: #0f766e;
            font-weight: 600;
            font-size: 12px;
            padding: 2px 8px;
            border-radius: 6px;
            border: 1px solid #ccfbf1;
            display: inline-block;
        }

        .mobile-pill {
            font-family: monospace;
            font-size: 12px;
            color: #0369a1;
            background: #f0f9ff;
            padding: 2px 7px;
            border-radius: 5px;
            border: 1px solid #e0f2fe;
        }

        /* Custom Repeater Pager Styling */
        .custom-pager-container {
            padding: 16px 24px;
            background: #f8fafc;
            border-top: 1px solid #e2e8f0;
            display: flex;
            justify-content: space-between;
            align-items: center;
            flex-wrap: wrap;
            gap: 16px;
        }

        .pager-info-text {
            font-size: 13px;
            color: #64748b;
            font-weight: 500;
        }

        .pager-pills-wrap {
            display: inline-flex;
            align-items: center;
            gap: 6px;
            flex-wrap: wrap;
        }

        /* Pager LinkButtons */
        .pager-page-btn {
            display: inline-flex;
            align-items: center;
            justify-content: center;
            min-width: 38px;
            height: 38px;
            padding: 0 10px;
            border-radius: 8px;
            font-size: 13px;
            font-weight: 700;
            text-decoration: none;
            transition: all 0.15s ease;
            box-sizing: border-box;
            border: 1px solid #cbd5e1;
            background: #ffffff;
            color: #334155 !important;
            cursor: pointer;
        }

        .pager-page-btn:hover {
            background: #eff6ff;
            border-color: #93c5fd;
            color: #1d4ed8 !important;
            transform: translateY(-1px);
        }

        /* Active Page (Disabled LinkButton) */
        .pager-page-btn.active,
        .pager-pills-wrap a[disabled="disabled"],
        .pager-pills-wrap a:disabled,
        .pager-pills-wrap span.pager-page-btn {
            background: #2563eb !important;
            border-color: #1d4ed8 !important;
            color: #ffffff !important;
            box-shadow: 0 2px 6px rgba(37, 99, 235, 0.35);
            cursor: default;
            pointer-events: none;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="user-mgmt-container">
        
        <!-- Hero Card -->
        <div class="user-hero-card">
            <div class="user-hero-info">
                <div class="hero-badge-row">
                    <span class="hero-badge">&#128209; Custom Server-Side Paging</span>
                    <span class="hero-badge hero-badge-blue">&#9881; Repeater Navigation</span>
                    <span class="hero-badge hero-badge-green">&#128451; spGetUsers Stored Procedure</span>
                </div>
                <h1>User Details (Repeater Pagination &amp; Stored Procedure)</h1>
                <p>High-efficiency SQL Server server-side pagination powered by <code>spGetUsers_by_PageIndex_and_PageSize</code> and <code>asp:Repeater</code>. Only 5 records per page are fetched across the network, reducing memory footprint and database overhead.</p>
            </div>
            <div class="hero-nav-actions">
                <a href="<%= ResolveUrl("~/UserDetailsWithoutObjectSource") %>" class="btn-hero-link">&#128736; Direct DAL (Standard)</a>
                <a href="<%= ResolveUrl("~/UserDetails") %>" class="btn-hero-link">&#128101; ObjectDataSource Version</a>
                <a href="<%= ResolveUrl("~/AllGridProp") %>" class="btn-hero-link">&larr; GridView Hub</a>
            </div>
        </div>

        <!-- Live Metrics Bar -->
        <div class="metrics-grid">
            <div class="metric-card">
                <div class="metric-icon metric-purple">&#128101;</div>
                <div class="metric-content">
                    <span class="metric-label">Total Users (DB)</span>
                    <span class="metric-value"><asp:Label ID="lblTotalUsers" runat="server" Text="0"></asp:Label></span>
                </div>
            </div>

            <div class="metric-card">
                <div class="metric-icon metric-blue">&#128196;</div>
                <div class="metric-content">
                    <span class="metric-label">Current Page</span>
                    <span class="metric-value">Page <asp:Label ID="lblCurrentPage" runat="server" Text="1"></asp:Label></span>
                </div>
            </div>

            <div class="metric-card">
                <div class="metric-icon metric-amber">&#128450;</div>
                <div class="metric-content">
                    <span class="metric-label">Total Pages</span>
                    <span class="metric-value"><asp:Label ID="lblTotalPages" runat="server" Text="1"></asp:Label></span>
                </div>
            </div>

            <div class="metric-card">
                <div class="metric-icon metric-emerald">&#9889;</div>
                <div class="metric-content">
                    <span class="metric-label">Page Size</span>
                    <span class="metric-value"><asp:Label ID="lblPageSize" runat="server" Text="5"></asp:Label> / page</span>
                </div>
            </div>
        </div>

        <!-- Content Card -->
        <div class="content-card">
            
            <!-- Action & Filter Toolbar -->
            <div class="action-toolbar">
                <div class="sql-badge-group">
                    <span class="sql-pill">&#128451; Table: <code>[Assignmet].[dbo].[UserDetails]</code></span>
                    <span class="sql-pill-sp">&#10003; spGetUsers_by_PageIndex_and_PageSize</span>
                    <span class="sql-pill-sp">&#10003; sp_InsertUserDetails</span>
                    <span class="sql-pill-sp">&#10003; sp_UpdateUserDetails</span>
                    <span class="sql-pill-sp">&#10003; sp_DeleteUserDetails</span>
                </div>

                <div class="table-search-box">
                    <span style="color:#64748b; font-size:13px;">&#128269;</span>
                    <input type="text" id="txtTableFilter" class="table-search-input" placeholder="Instant page filter..." onkeyup="filterUserTable()" />
                </div>
            </div>

            <!-- Status Feedback Message -->
            <asp:Label ID="lblStatus" runat="server" CssClass="status-msg" Visible="false"></asp:Label>

            <!-- Responsive GridView Table -->
            <div class="grid-responsive-wrap">
                <asp:GridView ID="GridView1" runat="server" 
                    AutoGenerateColumns="False" 
                    DataKeyNames="UserId" 
                    ShowFooter="True" 
                    CssClass="gridview-table"
                    GridLines="None"
                    AllowPaging="True"
                    PageSize="5"
                    OnRowCommand="GridView1_RowCommand"
                    OnRowDataBound="GridView1_RowDataBound">
                    
                    <%-- Note: GridView default pager is hidden because we use the custom Repeater below --%>
                    <PagerSettings Visible="false" />

                    <Columns>
                        
                        <%-- User ID Column --%>
                        <asp:TemplateField HeaderText="User ID" HeaderStyle-Width="90px">
                            <ItemTemplate>
                                <span class="id-badge">#<%# Eval("UserId") %></span>
                            </ItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtUserId" runat="server" CssClass="table-input" placeholder="Auto" ToolTip="Leave blank for auto-generated ID"></asp:TextBox>
                            </FooterTemplate>
                        </asp:TemplateField>

                        <%-- User Name Column --%>
                        <asp:TemplateField HeaderText="User Name">
                            <ItemTemplate>
                                <span style="font-weight: 700; color: #0f172a;"><%# Eval("UserName") %></span>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtUserName" runat="server" Text='<%# Eval("UserName") %>' CssClass="table-input" placeholder="User name"></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtUserNameFooter" runat="server" CssClass="table-input" placeholder="User name *"></asp:TextBox>
                            </FooterTemplate>
                        </asp:TemplateField>

                        <%-- Password Column --%>
                        <asp:TemplateField HeaderText="Password">
                            <ItemTemplate>
                                <span style="font-family: monospace; color: #94a3b8; letter-spacing: 2px;">&bull;&bull;&bull;&bull;&bull;&bull;</span>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtPassword" runat="server" Text='<%# Eval("Password") %>' CssClass="table-input" placeholder="Password"></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtPasswordFooter" runat="server" CssClass="table-input" placeholder="Password"></asp:TextBox>
                            </FooterTemplate>
                        </asp:TemplateField>

                        <%-- Email Column --%>
                        <asp:TemplateField HeaderText="Email Address">
                            <ItemTemplate>
                                <a href='mailto:<%# Eval("Email") %>' style="color: #2563eb; text-decoration: none; font-weight: 500;"><%# Eval("Email") %></a>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtEmail" runat="server" Text='<%# Eval("Email") %>' CssClass="table-input" placeholder="Email"></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtEmailFooter" runat="server" CssClass="table-input" placeholder="Email address"></asp:TextBox>
                            </FooterTemplate>
                        </asp:TemplateField>

                        <%-- Mobile No Column --%>
                        <asp:TemplateField HeaderText="Mobile No" HeaderStyle-Width="130px">
                            <ItemTemplate>
                                <span class="mobile-pill"><%# Eval("MobileNo") %></span>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtMobileNo" runat="server" Text='<%# Eval("MobileNo") %>' CssClass="table-input" placeholder="Mobile No"></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtMobileNoFooter" runat="server" CssClass="table-input" placeholder="Mobile number"></asp:TextBox>
                            </FooterTemplate>
                        </asp:TemplateField>

                        <%-- City Column --%>
                        <asp:TemplateField HeaderText="City" HeaderStyle-Width="120px">
                            <ItemTemplate>
                                <span class="city-pill"><%# Eval("City") %></span>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtCity" runat="server" Text='<%# Eval("City") %>' CssClass="table-input" placeholder="City"></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtCityFooter" runat="server" CssClass="table-input" placeholder="City name"></asp:TextBox>
                            </FooterTemplate>
                        </asp:TemplateField>

                        <%-- Actions Column --%>
                        <asp:TemplateField HeaderText="Actions" HeaderStyle-Width="180px">
                            <ItemTemplate>
                                <asp:LinkButton ID="lbEdit" runat="server" CommandName="EditRow" CommandArgument='<%# Eval("UserId") %>' CssClass="btn-action-edit">
                                    &#9998; Edit
                                </asp:LinkButton>
                                <asp:LinkButton ID="lbDelete" runat="server" CommandName="DeleteRow" CommandArgument='<%# Eval("UserId") %>' CssClass="btn-action-delete" CausesValidation="false" OnClientClick="return confirm('Are you sure you want to delete this row');">
                                    &#128465; Delete
                                </asp:LinkButton>
                            </ItemTemplate>
                            
                            <EditItemTemplate>
                                <asp:LinkButton ID="lbUpdate" runat="server" CommandName="UpdateRow" CommandArgument='<%# Eval("UserId") %>' CssClass="btn-action-update" OnClientClick="return confirm('Are you sure you want to update this row');">
                                    &#10003; Update
                                </asp:LinkButton>
                                <asp:LinkButton ID="lbCancel" runat="server" CommandName="CancelUpdate" CommandArgument='<%# Eval("UserId") %>' CssClass="btn-action-cancel" CausesValidation="false">
                                    &#10005; Cancel
                                </asp:LinkButton>
                            </EditItemTemplate>

                            <FooterTemplate>
                                <asp:LinkButton ID="lbInsert" runat="server" CommandName="InsertRow" CssClass="btn-insert-user">
                                    &#10010; Add User
                                </asp:LinkButton>
                            </FooterTemplate>
                        </asp:TemplateField>

                    </Columns>
                </asp:GridView>
            </div>

            <!-- Custom Repeater Pager Bar -->
            <div class="custom-pager-container">
                <div class="pager-info-text">
                    <asp:Label ID="lblPagingSummary" runat="server" Text="Showing records..."></asp:Label>
                </div>
                
                <div class="pager-pills-wrap">
                    <span style="font-size: 12px; font-weight: 700; color: #64748b; margin-right: 4px; text-transform: uppercase;">Page:</span>
                    <asp:Repeater ID="repeaterPaging" runat="server">
                        <ItemTemplate>
                            <asp:LinkButton ID="pagingLinkButton"
                                runat="server"
                                Text='<%# Eval("Text") %>'
                                CommandArgument='<%# Eval("Value") %>'
                                Enabled='<%# Eval("Enabled") %>'
                                OnClick="linkButton_Click"
                                CssClass='<%# Convert.ToBoolean(Eval("Enabled")) ? "pager-page-btn" : "pager-page-btn active" %>'>
                            </asp:LinkButton>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
            </div>

        </div>
    </div>

    <script type="text/javascript">
        function filterUserTable() {
            var input = document.getElementById('txtTableFilter');
            var filter = input.value.toLowerCase();
            var table = document.querySelector('.gridview-table');
            if (!table) return;

            var rows = table.querySelectorAll('tbody tr');
            for (var i = 0; i < rows.length; i++) {
                var row = rows[i];
                if (row.closest('tfoot') || row.querySelector('th')) continue;

                var text = row.textContent || row.innerText;
                if (text.toLowerCase().indexOf(filter) > -1) {
                    row.style.display = '';
                } else {
                    row.style.display = 'none';
                }
            }
        }
    </script>
</asp:Content>
