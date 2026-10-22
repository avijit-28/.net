<%@ Page Title="User Details Management" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="UserDetails.aspx.cs" Inherits="WebApplication4.UserDetailsPage" %>

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
            background: radial-gradient(circle, rgba(56, 189, 248, 0.18) 0%, transparent 70%);
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
            background: rgba(56, 189, 248, 0.18);
            border: 1px solid rgba(56, 189, 248, 0.35);
            color: #7dd3fc;
            font-size: 11px;
            font-weight: 700;
            padding: 3px 10px;
            border-radius: 20px;
            letter-spacing: 0.04em;
            text-transform: uppercase;
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
            z-index: 1;
        }

        .btn-hub-return:hover {
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

        /* SQL Badge & Filter Bar */
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

        .sql-source-pill {
            background: #eff6ff;
            color: #1d4ed8;
            border: 1px solid #bfdbfe;
            font-size: 12px;
            font-weight: 700;
            padding: 5px 12px;
            border-radius: 7px;
            display: inline-flex;
            align-items: center;
            gap: 6px;
        }

        .filter-search-box {
            display: inline-flex;
            align-items: center;
            gap: 8px;
            background: #f8fafc;
            border: 1px solid #cbd5e1;
            border-radius: 8px;
            padding: 4px 12px;
        }

        .filter-search-input {
            border: none;
            background: transparent;
            font-size: 13px;
            outline: none;
            width: 220px;
            color: #0f172a;
        }

        /* Status Banner */
        .status-msg {
            display: block;
            margin-bottom: 16px;
            padding: 10px 16px;
            border-radius: 8px;
            font-size: 13px;
            font-weight: 600;
            background: #f0fdf4;
            color: #15803d;
            border: 1px solid #bbf7d0;
        }

        .status-msg-error {
            background: #fef2f2;
            color: #b91c1c;
            border-color: #fecaca;
        }

        /* Modern GridView Table */
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
            padding: 12px 14px;
            text-align: left;
            font-weight: 700;
            font-size: 12.5px;
            text-transform: uppercase;
            letter-spacing: 0.04em;
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
            padding: 4px 10px;
            border-radius: 6px;
            font-size: 12px;
            font-weight: 700;
            text-decoration: none !important;
            transition: all 0.15s ease;
            margin-right: 4px;
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
            font-size: 12.5px;
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
                    <span class="hero-badge">&#128101; User Directory</span>
                    <span class="hero-badge">&#9881; ObjectDataSource Architecture</span>
                    <span class="hero-badge">&#128737; Stored Procedures</span>
                </div>
                <h1>User Details Management</h1>
                <p>
                    Full-featured user administration system powered by <code>ObjectDataSource</code> and SQL Server Stored Procedures (<code>sp_GetAllUserDetails</code>, <code>sp_InsertUserDetails</code>, <code>sp_UpdateUserDetails</code>, <code>sp_DeleteUserDetails</code>) with interactive pre-action confirmation popups.
                </p>
            </div>
            <a href="<%= ResolveUrl("~/Home") %>" class="btn-hub-return">&larr; Return to Home</a>
        </div>

        <!-- Main Content Card -->
        <div class="content-card">
            
            <!-- Toolbar: SQL Status & Instant Filter -->
            <div class="action-toolbar">
                <span class="sql-source-pill">
                    &#128451; SQL Server: <code>[Assignmet].[dbo].[UserDetails]</code> via Stored Procedures
                </span>

                <div class="filter-search-box">
                    <span>&#128269;</span>
                    <input type="text" id="userQuickSearch" class="filter-search-input" placeholder="Quick filter users in grid..." onkeyup="filterUserGrid()" />
                </div>
            </div>

            <!-- Feedback Status Label -->
            <asp:Label ID="lblStatus" runat="server" CssClass="status-msg" Visible="false"></asp:Label>

            <!-- ObjectDataSource Component -->
            <asp:ObjectDataSource ID="ObjectDataSource1" runat="server"
                TypeName="WebApplication4.UserDetailsAccessLayer"
                SelectMethod="GetAllUsers"
                InsertMethod="InsertUser"
                UpdateMethod="UpdateUser"
                DeleteMethod="DeleteUser"
                OnInserted="ObjectDataSource1_Inserted"
                OnUpdated="ObjectDataSource1_Updated"
                OnDeleted="ObjectDataSource1_Deleted">
                <InsertParameters>
                    <asp:Parameter Name="UserId" Type="Int32" />
                    <asp:Parameter Name="UserName" Type="String" />
                    <asp:Parameter Name="Password" Type="String" />
                    <asp:Parameter Name="Email" Type="String" />
                    <asp:Parameter Name="MobileNo" Type="String" />
                    <asp:Parameter Name="City" Type="String" />
                </InsertParameters>
                <UpdateParameters>
                    <asp:Parameter Name="UserId" Type="Int32" />
                    <asp:Parameter Name="UserName" Type="String" />
                    <asp:Parameter Name="Password" Type="String" />
                    <asp:Parameter Name="Email" Type="String" />
                    <asp:Parameter Name="MobileNo" Type="String" />
                    <asp:Parameter Name="City" Type="String" />
                </UpdateParameters>
                <DeleteParameters>
                    <asp:Parameter Name="UserId" Type="Int32" />
                </DeleteParameters>
            </asp:ObjectDataSource>

            <!-- Responsive GridView -->
            <div class="grid-responsive-wrap">
                <asp:GridView ID="GridView1" runat="server"
                    AutoGenerateColumns="False"
                    DataKeyNames="UserId"
                    DataSourceID="ObjectDataSource1"
                    ShowFooter="True"
                    AllowPaging="True"
                    PageSize="10"
                    CssClass="gridview-table"
                    GridLines="None"
                    OnRowCommand="GridView1_RowCommand"
                    OnRowDataBound="GridView1_RowDataBound">
                    
                    <PagerStyle CssClass="gridview-pager" HorizontalAlign="Center" />

                    <Columns>
                        <%-- Actions Column with Custom Styled Links and Confirmation Popups --%>
                        <asp:TemplateField HeaderText="Actions" ItemStyle-Width="160px">
                            <ItemTemplate>
                                <asp:LinkButton ID="btnEdit" runat="server" CommandName="Edit" Text="&#9998; Edit" CssClass="btn-action-edit" CausesValidation="false" />
                                <asp:LinkButton ID="btnDelete" runat="server" CommandName="Delete" Text="&#128465; Delete" CssClass="btn-action-delete" CausesValidation="false" />
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:LinkButton ID="btnUpdate" runat="server" CommandName="Update" Text="&#10004; Update" CssClass="btn-action-update" />
                                <asp:LinkButton ID="btnCancel" runat="server" CommandName="Cancel" Text="&#10006; Cancel" CssClass="btn-action-cancel" CausesValidation="false" />
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:Button ID="btnAdd" runat="server" Text="&#10010; Insert User" CommandName="AddNew" CssClass="btn-insert-user" />
                            </FooterTemplate>
                        </asp:TemplateField>

                        <%-- User ID --%>
                        <asp:TemplateField HeaderText="User ID" ItemStyle-Width="90px">
                            <ItemTemplate>
                                <span class="id-badge">#<%# Eval("UserId") %></span>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:Label ID="lblUserIdEdit" runat="server" Text='<%# Eval("UserId") %>' CssClass="id-badge" />
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtUserId" runat="server" CssClass="table-input" placeholder="ID (Auto/Num)" />
                            </FooterTemplate>
                        </asp:TemplateField>

                        <%-- User Name --%>
                        <asp:TemplateField HeaderText="User Name">
                            <ItemTemplate>
                                <strong><%# Eval("UserName") %></strong>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtUserNameEdit" runat="server" Text='<%# Bind("UserName") %>' CssClass="table-input" />
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtUserName" runat="server" CssClass="table-input" placeholder="Full Username" />
                            </FooterTemplate>
                        </asp:TemplateField>

                        <%-- Password --%>
                        <asp:TemplateField HeaderText="Password">
                            <ItemTemplate>
                                <span style="font-family: monospace; letter-spacing: 2px; color: #64748b;">&bull;&bull;&bull;&bull;&bull;&bull;&bull;&bull;</span>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtPasswordEdit" runat="server" Text='<%# Bind("Password") %>' CssClass="table-input" />
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtPassword" runat="server" CssClass="table-input" placeholder="Password" />
                            </FooterTemplate>
                        </asp:TemplateField>

                        <%-- Email --%>
                        <asp:TemplateField HeaderText="Email Address">
                            <ItemTemplate>
                                <a href='mailto:<%# Eval("Email") %>' style="color: #2563eb; text-decoration: none; font-weight: 500;">
                                    <%# Eval("Email") %>
                                </a>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtEmailEdit" runat="server" Text='<%# Bind("Email") %>' CssClass="table-input" />
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtEmail" runat="server" CssClass="table-input" placeholder="user@domain.com" />
                            </FooterTemplate>
                        </asp:TemplateField>

                        <%-- Mobile No --%>
                        <asp:TemplateField HeaderText="Mobile Number">
                            <ItemTemplate>
                                <span>&#128222; <%# Eval("MobileNo") %></span>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtMobileNoEdit" runat="server" Text='<%# Bind("MobileNo") %>' CssClass="table-input" />
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtMobileNo" runat="server" CssClass="table-input" placeholder="Mobile No" />
                            </FooterTemplate>
                        </asp:TemplateField>

                        <%-- City --%>
                        <asp:TemplateField HeaderText="City">
                            <ItemTemplate>
                                <span class="city-pill">&#127961; <%# Eval("City") %></span>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtCityEdit" runat="server" Text='<%# Bind("City") %>' CssClass="table-input" />
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtCity" runat="server" CssClass="table-input" placeholder="City Name" />
                            </FooterTemplate>
                        </asp:TemplateField>

                    </Columns>
                </asp:GridView>
            </div>
        </div>
    </div>

    <!-- Client-Side Quick Table Filter Script -->
    <script type="text/javascript">
        function filterUserGrid() {
            var input = document.getElementById('userQuickSearch');
            var filter = input.value.toLowerCase();
            var table = document.querySelector('.gridview-table');
            if (!table) return;

            var rows = table.getElementsByTagName('tr');
            // Skip header (i=0) and footer (last row)
            for (var i = 1; i < rows.length - 1; i++) {
                var rowText = rows[i].textContent || rows[i].innerText;
                if (rowText.toLowerCase().indexOf(filter) > -1) {
                    rows[i].style.display = "";
                } else {
                    rows[i].style.display = "none";
                }
            }
        }
    </script>
</asp:Content>
