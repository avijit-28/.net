<%@ Page Title="User Details (Direct DAL / Without ObjectDataSource)" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="UserDetailsWithoutObjectSource.aspx.cs" Inherits="WebApplication4.UserDetailsWithoutObjectSource" %>

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
            background: radial-gradient(circle, rgba(245, 158, 11, 0.18) 0%, transparent 70%);
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
            background: rgba(245, 158, 11, 0.2);
            border: 1px solid rgba(245, 158, 11, 0.4);
            color: #fcd34d;
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
            border-radius: 8px;
            font-size: 13px;
            font-weight: 600;
            text-decoration: none;
            transition: all 0.2s;
        }

        .btn-hero-link:hover {
            background: rgba(255, 255, 255, 0.2);
            transform: translateY(-1px);
        }

        /* Content Card */
        .content-card {
            background: #ffffff;
            border-radius: 14px;
            border: 1px solid #e2e8f0;
            box-shadow: 0 4px 15px -3px rgba(0, 0, 0, 0.05);
            padding: 24px;
        }

        /* Action Toolbar */
        .action-toolbar {
            display: flex;
            justify-content: space-between;
            align-items: center;
            flex-wrap: wrap;
            gap: 14px;
            margin-bottom: 18px;
            padding-bottom: 14px;
            border-bottom: 1px solid #f1f5f9;
        }

        .sql-badge-group {
            display: flex;
            flex-wrap: wrap;
            align-items: center;
            gap: 8px;
        }

        .sql-pill {
            display: inline-flex;
            align-items: center;
            gap: 6px;
            background: #eff6ff;
            color: #1d4ed8;
            border: 1px solid #bfdbfe;
            font-size: 12px;
            font-weight: 700;
            padding: 5px 12px;
            border-radius: 8px;
        }

        .sql-pill-sp {
            background: #f0fdf4;
            color: #15803d;
            border: 1px solid #bbf7d0;
            font-size: 11.5px;
            font-weight: 600;
            padding: 5px 10px;
            border-radius: 8px;
        }

        .table-search-box {
            display: inline-flex;
            align-items: center;
            background: #f8fafc;
            border: 1.5px solid #cbd5e1;
            border-radius: 8px;
            padding: 6px 12px;
            gap: 8px;
            transition: all 0.2s;
        }

        .table-search-box:focus-within {
            background: #ffffff;
            border-color: #2563eb;
            box-shadow: 0 0 0 3px rgba(37, 99, 235, 0.15);
        }

        .table-search-input {
            border: none;
            background: transparent;
            outline: none;
            font-size: 13px;
            color: #0f172a;
            width: 190px;
            font-family: inherit;
        }

        /* Status Banner */
        .status-msg {
            display: block;
            margin-bottom: 18px;
            padding: 12px 18px;
            border-radius: 10px;
            font-size: 13.5px;
            font-weight: 600;
            background: #f0fdf4;
            color: #15803d;
            border: 1px solid #bbf7d0;
            animation: fadeIn 0.3s ease;
        }

        .status-msg-error {
            background: #fef2f2;
            color: #b91c1c;
            border-color: #fecaca;
        }

        @keyframes fadeIn {
            from { opacity: 0; transform: translateY(-4px); }
            to { opacity: 1; transform: translateY(0); }
        }

        /* GridView Table */
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
            padding: 13px 14px;
            text-align: left;
            font-weight: 700;
            letter-spacing: 0.02em;
            border-bottom: 2px solid #334155;
            white-space: nowrap;
        }

        .gridview-table td {
            padding: 11px 14px;
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

        .gridview-table tfoot tr td {
            background: #f8fafc;
            border-top: 2px solid #cbd5e1;
            padding: 12px 14px;
        }

        /* Action Buttons */
        .btn-action-edit, .btn-action-delete, .btn-action-update, .btn-action-cancel {
            display: inline-flex;
            align-items: center;
            gap: 4px;
            padding: 5px 11px;
            border-radius: 6px;
            font-size: 12px;
            font-weight: 700;
            text-decoration: none !important;
            transition: all 0.15s ease;
            margin-right: 4px;
            cursor: pointer;
        }

        .btn-action-edit {
            background: #eff6ff;
            color: #2563eb !important;
            border: 1px solid #bfdbfe;
        }
        .btn-action-edit:hover {
            background: #2563eb;
            color: #ffffff !important;
        }

        .btn-action-delete {
            background: #fef2f2;
            color: #dc2626 !important;
            border: 1px solid #fecaca;
        }
        .btn-action-delete:hover {
            background: #dc2626;
            color: #ffffff !important;
        }

        .btn-action-update {
            background: #10b981;
            color: #ffffff !important;
            border: 1px solid #059669;
        }
        .btn-action-update:hover {
            background: #059669;
        }

        .btn-action-cancel {
            background: #f1f5f9;
            color: #475569 !important;
            border: 1px solid #cbd5e1;
        }
        .btn-action-cancel:hover {
            background: #e2e8f0;
        }

        .btn-insert-user {
            background: linear-gradient(135deg, #10b981 0%, #059669 100%);
            color: #ffffff !important;
            padding: 8px 16px;
            border-radius: 6px;
            font-size: 12.5px;
            font-weight: 700;
            text-decoration: none !important;
            display: inline-flex;
            align-items: center;
            gap: 6px;
            box-shadow: 0 2px 6px rgba(16, 185, 129, 0.25);
            border: none;
            cursor: pointer;
            transition: all 0.2s;
        }

        .btn-insert-user:hover {
            background: linear-gradient(135deg, #059669 0%, #047857 100%);
            box-shadow: 0 4px 10px rgba(16, 185, 129, 0.35);
            transform: translateY(-1px);
        }

        /* Inputs in Edit and Footer */
        .table-input {
            width: 100%;
            min-width: 90px;
            padding: 6px 9px;
            border: 1px solid #cbd5e1;
            border-radius: 6px;
            font-size: 13px;
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

        /* Pager Styling */
        .gridview-pager {
            background: #f8fafc;
            padding: 12px 16px;
            text-align: center;
        }
        .gridview-pager td {
            padding: 4px 8px;
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
    <div class="user-mgmt-container">
        
        <!-- Hero Card -->
        <div class="user-hero-card">
            <div class="user-hero-info">
                <div class="hero-badge-row">
                    <span class="hero-badge">&#128736; Direct DAL Binding</span>
                    <span class="hero-badge hero-badge-blue">&#9881; Manual RowCommand CRUD</span>
                    <span class="hero-badge">&#128451; Stored Procedures</span>
                </div>
                <h1>User Details (Without ObjectDataSource)</h1>
                <p>Direct code-behind data manipulation utilizing <code>UserDetailsAccessLayer</code> and manual <code>RowCommand</code> event handling (<code>EditRow</code>, <code>DeleteRow</code>, <code>UpdateRow</code>, <code>InsertRow</code>) without declarative ObjectDataSource controls.</p>
            </div>
            <div class="hero-nav-actions">
                <a href="<%= ResolveUrl("~/UserDetails") %>" class="btn-hero-link">&#128260; View ObjectDataSource Version</a>
                <a href="<%= ResolveUrl("~/AllGridProp") %>" class="btn-hero-link">&larr; GridView Hub</a>
            </div>
        </div>

        <!-- Content Card -->
        <div class="content-card">
            
            <!-- Action & Filter Toolbar -->
            <div class="action-toolbar">
                <div class="sql-badge-group">
                    <span class="sql-pill">&#128451; Table: <code>[Assignmet].[dbo].[UserDetails]</code></span>
                    <span class="sql-pill-sp">&#10003; sp_GetAllUserDetails</span>
                    <span class="sql-pill-sp">&#10003; sp_InsertUserDetails</span>
                    <span class="sql-pill-sp">&#10003; sp_UpdateUserDetails</span>
                    <span class="sql-pill-sp">&#10003; sp_DeleteUserDetails</span>
                </div>

                <div class="table-search-box">
                    <span style="color:#64748b; font-size:13px;">&#128269;</span>
                    <input type="text" id="txtTableFilter" class="table-search-input" placeholder="Instant table search..." onkeyup="filterUserTable()" />
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
                    PageSize="10"
                    OnPageIndexChanging="GridView1_PageIndexChanging"
                    OnRowCommand="GridView1_RowCommand"
                    OnRowDataBound="GridView1_RowDataBound">
                    
                    <PagerStyle CssClass="gridview-pager" HorizontalAlign="Center" />

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
                                <asp:LinkButton ID="lbDelete" runat="server" CommandName="DeleteRow" CommandArgument='<%# Eval("UserId") %>' CssClass="btn-action-delete" CausesValidation="false">
                                    &#128465; Delete
                                </asp:LinkButton>
                            </ItemTemplate>
                            
                            <EditItemTemplate>
                                <asp:LinkButton ID="lbUpdate" runat="server" CommandName="UpdateRow" CommandArgument='<%# Eval("UserId") %>' CssClass="btn-action-update">
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
                // Skip header or footer rows if present in tbody
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
