<%@ Page Title="Product LINQ CRUD Operations" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ProductLinqCurd.aspx.cs" Inherits="WebApplication4.ProductLinqCurd" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .linq-page-container {
            width: 100%;
            max-width: 1200px;
            margin: 0 auto;
            display: flex;
            flex-direction: column;
            gap: 24px;
        }

        /* Top Module Header Banner */
        .module-header-card {
            background: linear-gradient(135deg, #0f172a 0%, #1e293b 50%, #2563eb 100%);
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

        .module-header-card::after {
            content: "";
            position: absolute;
            right: -20px;
            bottom: -20px;
            width: 160px;
            height: 160px;
            background: radial-gradient(circle, rgba(37, 99, 235, 0.25) 0%, transparent 70%);
            pointer-events: none;
        }

        .header-badge-wrap {
            display: inline-flex;
            align-items: center;
            gap: 6px;
            background: rgba(255, 255, 255, 0.15);
            backdrop-filter: blur(8px);
            color: #93c5fd;
            border: 1px solid rgba(255, 255, 255, 0.25);
            padding: 4px 12px;
            border-radius: 20px;
            font-size: 11.5px;
            font-weight: 700;
            text-transform: uppercase;
            letter-spacing: 0.05em;
            margin-bottom: 8px;
        }

        .module-header-info h1 {
            font-size: 24px;
            font-weight: 800;
            color: #ffffff;
            letter-spacing: -0.02em;
            margin-bottom: 6px;
        }

        .module-header-info p {
            font-size: 13.5px;
            color: #cbd5e1;
            line-height: 1.5;
            max-width: 750px;
            margin: 0;
        }

        .header-actions {
            display: flex;
            align-items: center;
            gap: 10px;
            flex-wrap: wrap;
            z-index: 1;
        }

        .btn-header-action {
            display: inline-flex;
            align-items: center;
            gap: 6px;
            padding: 9px 16px;
            background: rgba(255, 255, 255, 0.12);
            color: #ffffff;
            border: 1px solid rgba(255, 255, 255, 0.25);
            border-radius: 8px;
            font-size: 13px;
            font-weight: 600;
            text-decoration: none;
            transition: all 0.2s ease;
        }

        .btn-header-action:hover {
            background: #ffffff;
            color: #0f172a;
            border-color: #ffffff;
            transform: translateY(-1px);
        }

        .btn-header-primary {
            background: linear-gradient(135deg, #10b981 0%, #059669 100%);
            border: none;
            color: #ffffff;
            font-weight: 700;
            box-shadow: 0 4px 12px rgba(16, 185, 129, 0.3);
        }

        .btn-header-primary:hover {
            background: linear-gradient(135deg, #059669 0%, #047857 100%);
            color: #ffffff;
        }

        /* KPI Metrics Cards */
        .metrics-grid {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(220px, 1fr));
            gap: 16px;
        }

        .metric-card {
            background: #ffffff;
            border: 1px solid #e2e8f0;
            border-radius: 14px;
            padding: 18px 20px;
            display: flex;
            align-items: center;
            gap: 16px;
            box-shadow: 0 2px 8px rgba(0, 0, 0, 0.04);
            transition: transform 0.2s ease, box-shadow 0.2s ease;
        }

        .metric-card:hover {
            transform: translateY(-2px);
            box-shadow: 0 6px 16px rgba(0, 0, 0, 0.06);
        }

        .metric-icon-wrap {
            width: 48px;
            height: 48px;
            border-radius: 12px;
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 22px;
            flex-shrink: 0;
        }

        .icon-blue { background: #eff6ff; color: #2563eb; }
        .icon-green { background: #ecfdf5; color: #059669; }
        .icon-amber { background: #fffbeb; color: #d97706; }
        .icon-purple { background: #faf5ff; color: #9333ea; }

        .metric-data {
            display: flex;
            flex-direction: column;
        }

        .metric-val {
            font-size: 22px;
            font-weight: 800;
            color: #0f172a;
            line-height: 1.2;
        }

        .metric-label {
            font-size: 12px;
            font-weight: 600;
            color: #64748b;
            text-transform: uppercase;
            letter-spacing: 0.04em;
            margin-top: 2px;
        }

        /* Alert / Message Banner */
        .alert-banner {
            padding: 14px 18px;
            border-radius: 10px;
            font-size: 13.5px;
            font-weight: 600;
            display: flex;
            align-items: center;
            gap: 10px;
            box-shadow: 0 2px 6px rgba(0, 0, 0, 0.04);
            animation: fadeIn 0.3s ease;
        }

        .alert-banner-success {
            background-color: #ecfdf5;
            color: #065f46;
            border: 1px solid #a7f3d0;
        }

        .alert-banner-info {
            background-color: #eff6ff;
            color: #1e40af;
            border: 1px solid #bfdbfe;
        }

        .alert-banner-danger {
            background-color: #fef2f2;
            color: #991b1b;
            border: 1px solid #fecaca;
        }

        .alert-banner-warning {
            background-color: #fffbeb;
            color: #92400e;
            border: 1px solid #fde68a;
        }

        @keyframes fadeIn {
            from { opacity: 0; transform: translateY(-4px); }
            to { opacity: 1; transform: translateY(0); }
        }

        /* Toolbar Filter Card */
        .filter-toolbar-card {
            background: #ffffff;
            border: 1px solid #e2e8f0;
            border-radius: 14px 14px 0 0;
            padding: 16px 20px;
            display: flex;
            align-items: center;
            justify-content: space-between;
            flex-wrap: wrap;
            gap: 12px;
            border-bottom: 2px solid #f1f5f9;
        }

        .search-box-wrap {
            position: relative;
            display: flex;
            align-items: center;
            min-width: 280px;
            flex: 1;
            max-width: 440px;
        }

        .search-icon {
            position: absolute;
            left: 12px;
            color: #94a3b8;
            font-size: 14px;
            pointer-events: none;
        }

        .search-input {
            width: 100%;
            height: 38px;
            padding: 8px 12px 8px 36px;
            font-size: 13.5px;
            border: 1.5px solid #cbd5e1;
            border-radius: 8px;
            outline: none;
            background: #f8fafc;
            color: #0f172a;
            font-family: inherit;
            transition: all 0.2s ease;
        }

        .search-input:focus {
            background: #ffffff;
            border-color: #2563eb;
            box-shadow: 0 0 0 3px rgba(37, 99, 235, 0.12);
        }

        .provider-pill {
            display: inline-flex;
            align-items: center;
            gap: 6px;
            background: #eff6ff;
            color: #1e40af;
            border: 1px solid #bfdbfe;
            font-size: 12px;
            font-weight: 700;
            padding: 5px 12px;
            border-radius: 20px;
        }

        /* Table Card & GridView */
        .table-card {
            background: #ffffff;
            border: 1px solid #e2e8f0;
            border-radius: 0 0 16px 16px;
            box-shadow: 0 10px 30px -4px rgba(0, 0, 0, 0.06);
            overflow: hidden;
        }

        .table-responsive {
            width: 100%;
            overflow-x: auto;
        }

        .gridview-table {
            width: 100%;
            border-collapse: collapse;
            font-size: 13.5px;
        }

        .gridview-table th {
            background: #f8fafc;
            color: #475569;
            font-weight: 700;
            font-size: 12px;
            text-transform: uppercase;
            letter-spacing: 0.05em;
            padding: 14px 18px;
            text-align: left;
            border-bottom: 2px solid #e2e8f0;
            white-space: nowrap;
        }

        .gridview-table td {
            padding: 13px 18px;
            border-bottom: 1px solid #f1f5f9;
            vertical-align: middle;
            color: #334155;
        }

        .gridview-table tr:hover {
            background: #f8fafc;
        }

        .gridview-table tr.edit-row {
            background: #eff6ff !important;
        }

        /* Footer Row for Adding New Record */
        .gridview-table tr.footer-row td {
            background: #f0fdf4 !important;
            border-top: 2px solid #86efac !important;
            padding: 14px 18px;
        }

        /* Table Inputs */
        .tbl-input {
            width: 100%;
            padding: 7px 10px;
            font-size: 13px;
            border: 1.5px solid #cbd5e1;
            border-radius: 6px;
            background: #ffffff;
            color: #0f172a;
            font-family: inherit;
            outline: none;
            transition: border-color 0.15s ease;
            box-sizing: border-box;
        }

        .tbl-input:focus {
            border-color: #2563eb;
            box-shadow: 0 0 0 2px rgba(37, 99, 235, 0.15);
        }

        .tbl-input-readonly {
            background: #f1f5f9;
            color: #64748b;
            cursor: not-allowed;
            font-weight: 700;
        }

        /* Badges & Elements */
        .badge-id {
            display: inline-flex;
            align-items: center;
            gap: 2px;
            padding: 4px 10px;
            background: #eff6ff;
            color: #2563eb;
            border: 1px solid #bfdbfe;
            border-radius: 8px;
            font-weight: 800;
            font-size: 12px;
        }

        .product-name-txt {
            font-weight: 700;
            color: #0f172a;
            display: inline-flex;
            align-items: center;
            gap: 6px;
        }

        .price-tag {
            font-family: monospace;
            font-size: 14px;
            font-weight: 700;
            color: #0f172a;
            background: #f8fafc;
            padding: 4px 8px;
            border-radius: 6px;
            border: 1px solid #e2e8f0;
            display: inline-block;
        }

        .stock-pill {
            display: inline-flex;
            align-items: center;
            gap: 5px;
            padding: 4px 12px;
            border-radius: 16px;
            font-size: 12px;
            font-weight: 700;
            white-space: nowrap;
        }

        .stock-in {
            background: #ecfdf5;
            color: #065f46;
            border: 1px solid #a7f3d0;
        }

        .stock-low {
            background: #fffbeb;
            color: #92400e;
            border: 1px solid #fde68a;
        }

        .stock-out {
            background: #fff1f2;
            color: #9f1239;
            border: 1px solid #fecdd3;
        }

        .stock-dot {
            width: 6px;
            height: 6px;
            border-radius: 50%;
            background: currentColor;
        }

        /* Action Buttons */
        .action-group {
            display: flex;
            align-items: center;
            gap: 6px;
            flex-wrap: wrap;
        }

        .btn-act {
            display: inline-flex;
            align-items: center;
            gap: 4px;
            padding: 6px 12px;
            border-radius: 6px;
            font-size: 12px;
            font-weight: 700;
            cursor: pointer;
            text-decoration: none;
            border: 1px solid transparent;
            transition: all 0.15s ease;
        }

        .btn-act-edit {
            background: #eff6ff;
            color: #2563eb;
            border-color: #bfdbfe;
        }

        .btn-act-edit:hover {
            background: #2563eb;
            color: #ffffff;
            box-shadow: 0 2px 6px rgba(37, 99, 235, 0.25);
        }

        .btn-act-delete {
            background: #fef2f2;
            color: #dc2626;
            border-color: #fecaca;
        }

        .btn-act-delete:hover {
            background: #dc2626;
            color: #ffffff;
            box-shadow: 0 2px 6px rgba(220, 38, 38, 0.25);
        }

        .btn-act-update {
            background: #10b981;
            color: #ffffff;
            border-color: #059669;
            box-shadow: 0 2px 6px rgba(16, 185, 129, 0.25);
        }

        .btn-act-update:hover {
            background: #059669;
        }

        .btn-act-cancel {
            background: #f1f5f9;
            color: #475569;
            border-color: #cbd5e1;
        }

        .btn-act-cancel:hover {
            background: #e2e8f0;
            color: #0f172a;
        }

        .btn-act-add {
            background: linear-gradient(135deg, #10b981 0%, #059669 100%);
            color: #ffffff;
            border: none;
            padding: 8px 16px;
            border-radius: 6px;
            font-size: 12.5px;
            font-weight: 700;
            cursor: pointer;
            box-shadow: 0 2px 8px rgba(16, 185, 129, 0.3);
            transition: all 0.2s;
        }

        .btn-act-add:hover {
            background: linear-gradient(135deg, #059669 0%, #047857 100%);
            transform: translateY(-1px);
        }

        /* Pager Styling */
        .pager-container {
            padding: 14px 20px;
            background: #f8fafc;
            border-top: 1px solid #e2e8f0;
            display: flex;
            align-items: center;
            justify-content: space-between;
            flex-wrap: wrap;
            gap: 12px;
        }

        .gridview-pager table {
            margin: 0;
        }

        .gridview-pager td {
            padding: 2px 4px !important;
            border: none !important;
        }

        .gridview-pager a,
        .gridview-pager span {
            display: inline-flex;
            align-items: center;
            justify-content: center;
            min-width: 32px;
            height: 32px;
            padding: 0 8px;
            border-radius: 6px;
            font-size: 12.5px;
            font-weight: 700;
            text-decoration: none;
            transition: all 0.15s ease;
        }

        .gridview-pager a {
            background: #ffffff;
            color: #334155;
            border: 1px solid #cbd5e1;
        }

        .gridview-pager a:hover {
            background: #eff6ff;
            color: #2563eb;
            border-color: #93c5fd;
        }

        .gridview-pager span {
            background: #2563eb;
            color: #ffffff;
            border: 1px solid #2563eb;
            box-shadow: 0 2px 6px rgba(37, 99, 235, 0.3);
        }

        /* Educational / Technical Info Note */
        .info-card {
            background: #eff6ff;
            border: 1px solid #bfdbfe;
            border-radius: 12px;
            padding: 16px 20px;
            display: flex;
            align-items: flex-start;
            gap: 12px;
            color: #1e40af;
            font-size: 13px;
            line-height: 1.5;
        }

        .info-card strong {
            color: #1e3a8a;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="linq-page-container">

        <!-- Module Header Card -->
        <div class="module-header-card">
            <div class="module-header-info">
                <div class="header-badge-wrap">
                    <span>&#9881;</span> LINQ to SQL DataContext
                </div>
                <h1>Product Management (LINQ to SQL CRUD)</h1>
                <p>Manage <code>dbo.Products</code> records seamlessly via <strong>ProductsDataContext</strong>. Features in-line row editing, footer row record addition, page-preserving operations, and center-screen confirmation modals for Insert, Update, and Delete.</p>
            </div>
            <div class="header-actions">
                <a href="<%= ResolveUrl("~/products.aspx") %>" class="btn-header-action">
                    <span>&#128230;</span> Products Catalog
                </a>
                <a href="<%= ResolveUrl("~/ProductForm") %>" class="btn-header-action btn-header-primary">
                    <span>&#10010;</span> Standard Add Form
                </a>
            </div>
        </div>

        <!-- KPI Metrics Grid -->
        <div class="metrics-grid">
            <div class="metric-card">
                <div class="metric-icon-wrap icon-blue">&#128230;</div>
                <div class="metric-data">
                    <asp:Label ID="lblTotalProducts" runat="server" CssClass="metric-val" Text="0"></asp:Label>
                    <span class="metric-label">Total Catalog Items</span>
                </div>
            </div>

            <div class="metric-card">
                <div class="metric-icon-wrap icon-green">&#9989;</div>
                <div class="metric-data">
                    <asp:Label ID="lblInStock" runat="server" CssClass="metric-val" Text="0"></asp:Label>
                    <span class="metric-label">In-Stock Products</span>
                </div>
            </div>

            <div class="metric-card">
                <div class="metric-icon-wrap icon-amber">&#9888;</div>
                <div class="metric-data">
                    <asp:Label ID="lblLowStock" runat="server" CssClass="metric-val" Text="0"></asp:Label>
                    <span class="metric-label">Low / Out of Stock</span>
                </div>
            </div>

            <div class="metric-card">
                <div class="metric-icon-wrap icon-purple">&#128176;</div>
                <div class="metric-data">
                    <asp:Label ID="lblAvgPrice" runat="server" CssClass="metric-val" Text="$ 0.00"></asp:Label>
                    <span class="metric-label">Avg Product Price</span>
                </div>
            </div>
        </div>

        <!-- Alert / Feedback Banner -->
        <asp:Panel ID="pnlResult" runat="server" Visible="false">
            <div id="divAlert" runat="server" class="alert-banner alert-banner-info">
                <span id="spAlertIcon" runat="server">&#9432;</span>
                <asp:Label ID="lblResult" runat="server"></asp:Label>
            </div>
        </asp:Panel>

        <!-- Filter & Search Toolbar -->
        <div class="filter-toolbar-card">
            <div class="search-box-wrap">
                <span class="search-icon">&#128269;</span>
                <input type="text" id="linqSearchInput" class="search-input" placeholder="Quick filter products by ID, Name or Price..." onkeyup="filterLinqTable()" />
            </div>

            <div class="provider-pill">
                <span>&#128451;</span> DataContext: <code>ProductsDataContext</code> (Table: <code>Products</code>)
            </div>
        </div>

        <!-- Table Card with GridView -->
        <div class="table-card">
            <div class="table-responsive">
                <asp:GridView ID="GridView1" runat="server"
                    AutoGenerateColumns="False"
                    DataKeyNames="ID,Name"
                    ShowFooter="True"
                    AllowPaging="True"
                    PageSize="8"
                    CssClass="gridview-table"
                    GridLines="None"
                    PagerStyle-CssClass="gridview-pager"
                    EditRowStyle-CssClass="edit-row"
                    FooterStyle-CssClass="footer-row"
                    OnPageIndexChanging="GridView1_PageIndexChanging"
                    OnRowEditing="GridView1_RowEditing"
                    OnRowUpdating="GridView1_RowUpdating"
                    OnRowCancelingEdit="GridView1_RowCancelingEdit"
                    OnRowDeleting="GridView1_RowDeleting"
                    OnRowCommand="GridView1_RowCommand">

                    <Columns>
                        <%-- ID Column --%>
                        <asp:TemplateField HeaderText="Product ID" HeaderStyle-Width="120px" ItemStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <span class="badge-id">#<%# Eval("ID") %></span>
                            </ItemTemplate>

                            <EditItemTemplate>
                                <asp:TextBox ID="txtID" runat="server"
                                    Text='<%# Eval("ID") %>'
                                    CssClass="tbl-input tbl-input-readonly"
                                    ReadOnly="true"
                                    ToolTip="Product ID cannot be modified (Primary Key)"></asp:TextBox>
                            </EditItemTemplate>

                            <FooterTemplate>
                                <asp:TextBox ID="txtfID" runat="server"
                                    CssClass="tbl-input"
                                    placeholder="ID (e.g. 6)"
                                    autocomplete="off"></asp:TextBox>
                            </FooterTemplate>
                        </asp:TemplateField>

                        <%-- Name Column --%>
                        <asp:TemplateField HeaderText="Product Name" HeaderStyle-Width="280px">
                            <ItemTemplate>
                                <span class="product-name-txt">
                                    <span style="color:#94a3b8;">&#128230;</span>
                                    <%# Eval("Name") %>
                                </span>
                            </ItemTemplate>

                            <EditItemTemplate>
                                <asp:TextBox ID="txtName" runat="server"
                                    Text='<%# Eval("Name") %>'
                                    CssClass="tbl-input"
                                    placeholder="Product Name"
                                    autocomplete="off"></asp:TextBox>
                            </EditItemTemplate>

                            <FooterTemplate>
                                <asp:TextBox ID="txtfName" runat="server"
                                    CssClass="tbl-input"
                                    placeholder="Enter Name"
                                    autocomplete="off"></asp:TextBox>
                            </FooterTemplate>
                        </asp:TemplateField>

                        <%-- Price Column --%>
                        <asp:TemplateField HeaderText="Price ($)" HeaderStyle-Width="160px">
                            <ItemTemplate>
                                <span class="price-tag">
                                    $ <%# Eval("Price") != null ? Convert.ToDecimal(Eval("Price")).ToString("N2") : "0.00" %>
                                </span>
                            </ItemTemplate>

                            <EditItemTemplate>
                                <asp:TextBox ID="txtPrice" runat="server"
                                    Text='<%# Eval("Price") %>'
                                    CssClass="tbl-input"
                                    placeholder="Price"
                                    autocomplete="off"></asp:TextBox>
                            </EditItemTemplate>

                            <FooterTemplate>
                                <asp:TextBox ID="txtfPrice" runat="server"
                                    CssClass="tbl-input"
                                    placeholder="Price (e.g. 25)"
                                    autocomplete="off"></asp:TextBox>
                            </FooterTemplate>
                        </asp:TemplateField>

                        <%-- Stock Column --%>
                        <asp:TemplateField HeaderText="Stock Availability" HeaderStyle-Width="220px">
                            <ItemTemplate>
                                <%# GetStockBadgeHtml(Eval("stock")) %>
                            </ItemTemplate>

                            <EditItemTemplate>
                                <asp:TextBox ID="txtStock" runat="server"
                                    Text='<%# Eval("stock") %>'
                                    CssClass="tbl-input"
                                    placeholder="Stock units"
                                    autocomplete="off"></asp:TextBox>
                            </EditItemTemplate>

                            <FooterTemplate>
                                <asp:TextBox ID="txtfStock" runat="server"
                                    CssClass="tbl-input"
                                    placeholder="Stock units"
                                    autocomplete="off"></asp:TextBox>
                            </FooterTemplate>
                        </asp:TemplateField>

                        <%-- Actions Column --%>
                        <asp:TemplateField HeaderText="Actions" HeaderStyle-Width="220px" ItemStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <div class="action-group">
                                    <asp:LinkButton ID="lnkEdit"
                                        runat="server"
                                        CommandName="Edit"
                                        CssClass="btn-act btn-act-edit"
                                        ToolTip="Edit this product row inline">
                                        &#9998; Edit
                                    </asp:LinkButton>

                                    <asp:LinkButton ID="lnkDelete"
                                        runat="server"
                                        CommandName="Delete"
                                        CssClass="btn-act btn-act-delete"
                                        OnClientClick='<%# "return confirm(\"Are you sure you want to delete product #" + Eval("ID") + " (" + Eval("Name") + ")?\");" %>'
                                        ToolTip="Delete this product record from SQL Server">
                                        &#128465; Delete
                                    </asp:LinkButton>
                                </div>
                            </ItemTemplate>

                            <EditItemTemplate>
                                <div class="action-group">
                                    <asp:LinkButton ID="lnkUpdate"
                                        runat="server"
                                        CommandName="Update"
                                        CssClass="btn-act btn-act-update"
                                        OnClientClick="return confirm('Are you sure you want to update this product record?');"
                                        ToolTip="Save updates to database">
                                        &#10003; Update
                                    </asp:LinkButton>

                                    <asp:LinkButton ID="lnkCancel"
                                        runat="server"
                                        CommandName="Cancel"
                                        CssClass="btn-act btn-act-cancel"
                                        ToolTip="Cancel changes and exit edit mode">
                                        &#10005; Cancel
                                    </asp:LinkButton>
                                </div>
                            </EditItemTemplate>

                            <FooterTemplate>
                                <asp:Button ID="btnAdd"
                                    runat="server"
                                    Text="+ Add Product"
                                    CommandName="AddNew"
                                    CssClass="btn-act-add btn-insert-confirm"
                                    OnClientClick="return confirm('Are you sure you want to add this new product to the catalog?');"
                                    ToolTip="Insert new product into database" />
                            </FooterTemplate>
                        </asp:TemplateField>
                    </Columns>

                    <EmptyDataTemplate>
                        <div style="padding: 40px; text-align: center; color: #64748b;">
                            <div style="font-size: 40px; margin-bottom: 10px;">&#128230;</div>
                            <h3 style="font-size: 18px; color: #1e293b; margin-bottom: 6px;">No Products Found in Catalog</h3>
                            <p style="font-size: 13.5px;">Table <code>dbo.Products</code> currently contains no rows. Use the input boxes in the footer to add the first item.</p>
                        </div>
                    </EmptyDataTemplate>
                </asp:GridView>
            </div>
        </div>

        <!-- Technical & Educational Card -->
        <div class="info-card">
            <span style="font-size: 20px;">&#128161;</span>
            <div>
                <strong>LINQ to SQL Architectural Highlights:</strong>
                Queries and persistence utilize strongly typed entity mapping defined in <code>Products.dbml</code>. Changes are committed via <code>db.Products.InsertOnSubmit()</code>, <code>db.Products.DeleteOnSubmit()</code>, and <code>db.SubmitChanges()</code>. All operations preserve the current grid page index, and centered confirmation dialogs safeguard data integrity during Insert, Update, and Delete operations.
            </div>
        </div>

    </div>

    <!-- Client-side Search Filter Script -->
    <script type="text/javascript">
        function filterLinqTable() {
            var input = document.getElementById("linqSearchInput");
            var filter = input.value.toLowerCase();
            var rows = document.querySelectorAll(".gridview-table tr:not(:first-child):not(.footer-row)");

            rows.forEach(function (row) {
                var text = row.textContent.toLowerCase();
                if (text.indexOf(filter) > -1) {
                    row.style.display = "";
                } else {
                    row.style.display = "none";
                }
            });
        }
    </script>
</asp:Content>
