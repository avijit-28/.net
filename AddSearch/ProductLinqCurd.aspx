<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ProductLinqCurd.aspx.cs" Inherits="AddSearch.Connected_Services.ProductLinqCurd" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Product LINQ CRUD Operations</title>
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <link rel="preconnect" href="https://fonts.googleapis.com" />
    <link rel="preconnect" href="https://fonts.gstatic.com" crossorigin="anonymous" />
    <link href="https://fonts.googleapis.com/css2?family=Plus+Jakarta+Sans:wght@400;500;600;700;800&display=swap" rel="stylesheet" />

    <style type="text/css">
        :root {
            --primary: #2563eb;
            --primary-hover: #1d4ed8;
            --emerald: #059669;
            --slate-50: #f8fafc;
            --slate-100: #f1f5f9;
            --slate-200: #e2e8f0;
            --slate-600: #475569;
            --slate-700: #334155;
            --slate-800: #1e293b;
            --slate-900: #0f172a;
        }

        * {
            box-sizing: border-box;
            margin: 0;
            padding: 0;
        }

        body {
            font-family: 'Plus Jakarta Sans', system-ui, -apple-system, sans-serif;
            background: linear-gradient(135deg, #f0f4ff 0%, #f8fafc 50%, #eff6ff 100%);
            color: var(--slate-800);
            min-height: 100vh;
            padding: 30px 20px;
        }

        .container {
            max-width: 1180px;
            margin: 0 auto;
            display: flex;
            flex-direction: column;
            gap: 22px;
        }

        /* Header Card */
        .header-card {
            background: linear-gradient(135deg, #0f172a 0%, #1e293b 50%, #2563eb 100%);
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

        .header-badge {
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

        .header-title h1 {
            font-size: 24px;
            font-weight: 800;
            letter-spacing: -0.02em;
            margin-bottom: 4px;
        }

        .header-title p {
            color: #cbd5e1;
            font-size: 13.5px;
            max-width: 700px;
            line-height: 1.5;
        }

        .header-actions {
            display: flex;
            align-items: center;
            gap: 10px;
        }

        .btn-header {
            padding: 8px 16px;
            background: rgba(255, 255, 255, 0.12);
            color: #ffffff;
            border: 1px solid rgba(255, 255, 255, 0.25);
            border-radius: 8px;
            font-size: 13px;
            font-weight: 600;
            text-decoration: none;
            transition: all 0.2s;
        }

        .btn-header:hover {
            background: #ffffff;
            color: #0f172a;
        }

        /* KPI Metrics */
        .metrics-grid {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(220px, 1fr));
            gap: 16px;
        }

        .metric-card {
            background: #ffffff;
            border: 1px solid var(--slate-200);
            border-radius: 14px;
            padding: 18px 20px;
            display: flex;
            align-items: center;
            gap: 16px;
            box-shadow: 0 2px 8px rgba(0, 0, 0, 0.04);
            transition: transform 0.2s ease;
        }

        .metric-card:hover {
            transform: translateY(-2px);
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

        .metric-val {
            font-size: 22px;
            font-weight: 800;
            color: var(--slate-900);
            line-height: 1.2;
        }

        .metric-label {
            font-size: 12px;
            font-weight: 600;
            color: var(--slate-600);
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
        }

        .alert-banner-success { background: #ecfdf5; color: #065f46; border: 1px solid #a7f3d0; }
        .alert-banner-danger { background: #fef2f2; color: #991b1b; border: 1px solid #fecaca; }
        .alert-banner-warning { background: #fffbeb; color: #92400e; border: 1px solid #fde68a; }
        .alert-banner-info { background: #eff6ff; color: #1e40af; border: 1px solid #bfdbfe; }

        /* Toolbar */
        .filter-toolbar-card {
            background: #ffffff;
            border: 1px solid var(--slate-200);
            border-radius: 14px 14px 0 0;
            padding: 16px 20px;
            display: flex;
            align-items: center;
            justify-content: space-between;
            flex-wrap: wrap;
            gap: 12px;
            border-bottom: 2px solid #f1f5f9;
        }

        .search-input {
            width: 320px;
            height: 38px;
            padding: 8px 14px;
            font-size: 13.5px;
            border: 1.5px solid var(--slate-200);
            border-radius: 8px;
            outline: none;
            background: #f8fafc;
            color: #0f172a;
            font-family: inherit;
        }

        .search-input:focus {
            border-color: var(--primary);
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
            border: 1px solid var(--slate-200);
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
            color: var(--slate-700);
            font-weight: 700;
            font-size: 12px;
            text-transform: uppercase;
            letter-spacing: 0.05em;
            padding: 14px 18px;
            text-align: left;
            border-bottom: 2px solid var(--slate-200);
            white-space: nowrap;
        }

        .gridview-table td {
            padding: 13px 18px;
            border-bottom: 1px solid var(--slate-100);
            vertical-align: middle;
        }

        .gridview-table tr:hover { background: #f8fafc; }
        .gridview-table tr.edit-row { background: #eff6ff !important; }
        .gridview-table tr.footer-row td {
            background: #f0fdf4 !important;
            border-top: 2px solid #86efac !important;
            padding: 14px 18px;
        }

        .tbl-input {
            width: 100%;
            padding: 7px 10px;
            font-size: 13px;
            border: 1.5px solid var(--slate-200);
            border-radius: 6px;
            background: #ffffff;
            color: #0f172a;
            font-family: inherit;
            outline: none;
        }

        .tbl-input:focus {
            border-color: var(--primary);
            box-shadow: 0 0 0 2px rgba(37, 99, 235, 0.15);
        }

        .tbl-input-readonly {
            background: #f1f5f9;
            color: var(--slate-600);
            cursor: not-allowed;
            font-weight: 700;
        }

        .badge-id {
            display: inline-flex;
            align-items: center;
            padding: 4px 10px;
            background: #eff6ff;
            color: var(--primary);
            border: 1px solid #bfdbfe;
            border-radius: 8px;
            font-weight: 800;
            font-size: 12px;
        }

        .price-tag {
            font-family: monospace;
            font-size: 14px;
            font-weight: 700;
            color: #0f172a;
            background: #f8fafc;
            padding: 4px 8px;
            border-radius: 6px;
            border: 1px solid var(--slate-200);
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

        .stock-in { background: #ecfdf5; color: #065f46; border: 1px solid #a7f3d0; }
        .stock-low { background: #fffbeb; color: #92400e; border: 1px solid #fde68a; }
        .stock-out { background: #fff1f2; color: #9f1239; border: 1px solid #fecdd3; }
        .stock-dot { width: 6px; height: 6px; border-radius: 50%; background: currentColor; }

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

        .btn-act-edit { background: #eff6ff; color: var(--primary); border-color: #bfdbfe; }
        .btn-act-edit:hover { background: var(--primary); color: #ffffff; }

        .btn-act-delete { background: #fef2f2; color: #dc2626; border-color: #fecaca; }
        .btn-act-delete:hover { background: #dc2626; color: #ffffff; }

        .btn-act-update { background: #10b981; color: #ffffff; border-color: #059669; }
        .btn-act-update:hover { background: #059669; }

        .btn-act-cancel { background: #f1f5f9; color: var(--slate-700); border-color: var(--slate-200); }
        .btn-act-cancel:hover { background: #e2e8f0; }

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
        }

        .gridview-pager table { margin: 0; }
        .gridview-pager td { padding: 2px 4px !important; border: none !important; }
        .gridview-pager a, .gridview-pager span {
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
        }
        .gridview-pager a { background: #ffffff; color: var(--slate-700); border: 1px solid var(--slate-200); }
        .gridview-pager span { background: var(--primary); color: #ffffff; border: 1px solid var(--primary); }

        .info-card {
            background: #f0fdf4;
            border: 1px solid #bbf7d0;
            border-radius: 12px;
            padding: 16px 20px;
            font-size: 13px;
            color: #166534;
            line-height: 1.5;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">

            <div class="header-card">
                <div class="header-title">
                    <div class="header-badge">&#9881; LINQ to SQL Engine</div>
                    <h1>Product LINQ Operations</h1>
                    <p>CRUD operations on table <code>dbo.Products</code> utilizing <strong>ProductsDataContext</strong> with in-line editing and confirmation dialogs.</p>
                </div>
                <div class="header-actions">
                    <a href="products.aspx" class="btn-header">&#128230; Products Catalog</a>
                </div>
            </div>

            <!-- KPI Metrics -->
            <div class="metrics-grid">
                <div class="metric-card">
                    <div class="metric-icon-wrap icon-blue">&#128230;</div>
                    <div>
                        <asp:Label ID="lblTotalProducts" runat="server" CssClass="metric-val" Text="0"></asp:Label>
                        <div class="metric-label">Total Products</div>
                    </div>
                </div>

                <div class="metric-card">
                    <div class="metric-icon-wrap icon-green">&#9989;</div>
                    <div>
                        <asp:Label ID="lblInStock" runat="server" CssClass="metric-val" Text="0"></asp:Label>
                        <div class="metric-label">In-Stock Items</div>
                    </div>
                </div>

                <div class="metric-card">
                    <div class="metric-icon-wrap icon-amber">&#9888;</div>
                    <div>
                        <asp:Label ID="lblLowStock" runat="server" CssClass="metric-val" Text="0"></asp:Label>
                        <div class="metric-label">Low / Out of Stock</div>
                    </div>
                </div>

                <div class="metric-card">
                    <div class="metric-icon-wrap icon-purple">&#128176;</div>
                    <div>
                        <asp:Label ID="lblAvgPrice" runat="server" CssClass="metric-val" Text="$ 0.00"></asp:Label>
                        <div class="metric-label">Avg Price</div>
                    </div>
                </div>
            </div>

            <!-- Result Alert -->
            <asp:Panel ID="pnlResult" runat="server" Visible="false">
                <div id="divAlert" runat="server" class="alert-banner alert-banner-info">
                    <span id="spAlertIcon" runat="server">&#9432;</span>
                    <asp:Label ID="lblResult" runat="server"></asp:Label>
                </div>
            </asp:Panel>

            <!-- Toolbar -->
            <div class="filter-toolbar-card">
                <input type="text" id="searchInput" class="search-input" placeholder="Quick filter by ID, Name or Price..." onkeyup="filterProducts()" />
                <span class="provider-pill">&#128451; DataContext: <code>ProductsDataContext</code></span>
            </div>

            <!-- Table Card -->
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
                                        ReadOnly="true"></asp:TextBox>
                                </EditItemTemplate>

                                <FooterTemplate>
                                    <asp:TextBox ID="txtfID" runat="server"
                                        CssClass="tbl-input"
                                        placeholder="ID (e.g. 6)"></asp:TextBox>
                                </FooterTemplate>
                            </asp:TemplateField>

                            <%-- Name Column --%>
                            <asp:TemplateField HeaderText="Product Name" HeaderStyle-Width="280px">
                                <ItemTemplate>
                                    <strong><%# Eval("Name") %></strong>
                                </ItemTemplate>

                                <EditItemTemplate>
                                    <asp:TextBox ID="txtName" runat="server"
                                        Text='<%# Eval("Name") %>'
                                        CssClass="tbl-input"></asp:TextBox>
                                </EditItemTemplate>

                                <FooterTemplate>
                                    <asp:TextBox ID="txtfName" runat="server"
                                        CssClass="tbl-input"
                                        placeholder="Enter Name"></asp:TextBox>
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
                                        CssClass="tbl-input"></asp:TextBox>
                                </EditItemTemplate>

                                <FooterTemplate>
                                    <asp:TextBox ID="txtfPrice" runat="server"
                                        CssClass="tbl-input"
                                        placeholder="Price (e.g. 25)"></asp:TextBox>
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
                                        CssClass="tbl-input"></asp:TextBox>
                                </EditItemTemplate>

                                <FooterTemplate>
                                    <asp:TextBox ID="txtfStock" runat="server"
                                        CssClass="tbl-input"
                                        placeholder="Stock units"></asp:TextBox>
                                </FooterTemplate>
                            </asp:TemplateField>

                            <%-- Actions Column --%>
                            <asp:TemplateField HeaderText="Actions" HeaderStyle-Width="220px" ItemStyle-HorizontalAlign="Center">
                                <ItemTemplate>
                                    <div class="action-group">
                                        <asp:LinkButton ID="lnkEdit"
                                            runat="server"
                                            CommandName="Edit"
                                            CssClass="btn-act btn-act-edit">
                                            &#9998; Edit
                                        </asp:LinkButton>

                                        <asp:LinkButton ID="lnkDelete"
                                            runat="server"
                                            CommandName="Delete"
                                            CssClass="btn-act btn-act-delete"
                                            OnClientClick='<%# "return confirm(\"Are you sure you want to delete product #" + Eval("ID") + " (" + Eval("Name") + ")?\");" %>'>
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
                                            OnClientClick="return confirm('Are you sure you want to update this product record?');">
                                            &#10003; Update
                                        </asp:LinkButton>

                                        <asp:LinkButton ID="lnkCancel"
                                            runat="server"
                                            CommandName="Cancel"
                                            CssClass="btn-act btn-act-cancel">
                                            &#10005; Cancel
                                        </asp:LinkButton>
                                    </div>
                                </EditItemTemplate>

                                <FooterTemplate>
                                    <asp:Button ID="btnAdd"
                                        runat="server"
                                        Text="+ Add Product"
                                        CommandName="AddNew"
                                        CssClass="btn-act-add"
                                        OnClientClick="return confirm('Are you sure you want to add this new product to the catalog?');" />
                                </FooterTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>
            </div>

            <div class="info-card">
                &#128161; <strong>LINQ to SQL CRUD:</strong> Insert, Update, and Delete operations commit directly through <code>ProductsDataContext</code> while maintaining the current grid page position.
            </div>

        </div>
    </form>

    <script type="text/javascript">
        function filterProducts() {
            var input = document.getElementById("searchInput");
            var filter = input.value.toLowerCase();
            var rows = document.querySelectorAll(".gridview-table tr:not(:first-child):not(.footer-row)");

            rows.forEach(function (row) {
                var text = row.textContent.toLowerCase();
                row.style.display = text.indexOf(filter) > -1 ? "" : "none";
            });
        }
    </script>
</body>
</html>
