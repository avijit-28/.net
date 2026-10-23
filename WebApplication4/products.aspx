<%@ Page Title="Products Catalog" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="products.aspx.cs" Inherits="WebApplication4.products" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .products-page-container {
            width: 100%;
            max-width: 1200px;
            margin: 0 auto;
        }

        /* Top Module Header Card */
        .module-header-card {
            background: #ffffff;
            border: 1px solid #e2e8f0;
            border-radius: 16px;
            padding: 24px 30px;
            margin-bottom: 24px;
            display: flex;
            align-items: center;
            justify-content: space-between;
            flex-wrap: wrap;
            gap: 16px;
            box-shadow: 0 4px 20px -2px rgba(0, 0, 0, 0.05);
        }

        .module-header-info {
            max-width: 720px;
        }

        .header-badge-wrap {
            display: inline-flex;
            align-items: center;
            gap: 6px;
            background: #eff6ff;
            color: #1d4ed8;
            border: 1px solid #bfdbfe;
            padding: 4px 12px;
            border-radius: 20px;
            font-size: 11.5px;
            font-weight: 700;
            text-transform: uppercase;
            letter-spacing: 0.05em;
            margin-bottom: 8px;
        }

        .module-header-info h1 {
            font-size: 26px;
            font-weight: 800;
            color: #0f172a;
            letter-spacing: -0.02em;
            margin-bottom: 6px;
        }

        .module-header-info p {
            font-size: 14px;
            color: #64748b;
            line-height: 1.5;
        }

        .header-actions {
            display: flex;
            align-items: center;
            gap: 10px;
            flex-wrap: wrap;
        }

        .btn-add-product {
            display: inline-flex;
            align-items: center;
            gap: 6px;
            padding: 10px 18px;
            background: linear-gradient(135deg, #10b981 0%, #059669 100%);
            color: #ffffff;
            border-radius: 8px;
            font-size: 13.5px;
            font-weight: 700;
            text-decoration: none;
            box-shadow: 0 4px 12px rgba(16, 185, 129, 0.25);
            transition: all 0.2s ease;
        }

        .btn-add-product:hover {
            background: linear-gradient(135deg, #059669 0%, #047857 100%);
            box-shadow: 0 6px 16px rgba(16, 185, 129, 0.35);
            transform: translateY(-1px);
        }

        .btn-hub-return {
            display: inline-flex;
            align-items: center;
            gap: 6px;
            padding: 10px 16px;
            background: #f8fafc;
            color: #475569;
            border: 1.5px solid #cbd5e1;
            border-radius: 8px;
            font-size: 13px;
            font-weight: 600;
            text-decoration: none;
            transition: all 0.2s ease;
        }

        .btn-hub-return:hover {
            background: #e2e8f0;
            color: #0f172a;
        }

        /* Metrics Dashboard Cards */
        .metrics-grid {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(220px, 1fr));
            gap: 16px;
            margin-bottom: 24px;
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

        /* Filter Toolbar Card */
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
            max-width: 420px;
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

        .sp-pill-indicator {
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
            margin-bottom: 24px;
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
            padding: 14px 18px;
            border-bottom: 1px solid #f1f5f9;
            vertical-align: middle;
            color: #334155;
        }

        .gridview-table tr:hover {
            background: #f8fafc;
        }

        /* Column Elements */
        .badge-product-id {
            display: inline-flex;
            align-items: center;
            gap: 3px;
            padding: 4px 10px;
            background: #eff6ff;
            color: #2563eb;
            border: 1px solid #bfdbfe;
            border-radius: 8px;
            font-weight: 800;
            font-size: 12px;
            text-decoration: none;
            transition: all 0.2s;
        }

        .badge-product-id:hover {
            background: #2563eb;
            color: #ffffff;
            box-shadow: 0 2px 6px rgba(37, 99, 235, 0.25);
        }

        .product-name-cell {
            display: flex;
            align-items: center;
            gap: 10px;
        }

        .product-icon-wrap {
            width: 34px;
            height: 34px;
            background: #f1f5f9;
            border-radius: 8px;
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 16px;
            flex-shrink: 0;
            color: #334155;
        }

        .product-link {
            color: #0f172a;
            font-weight: 700;
            font-size: 14px;
            text-decoration: none;
            transition: color 0.15s;
        }

        .product-link:hover {
            color: #2563eb;
            text-decoration: underline;
        }

        .price-tag {
            font-family: monospace;
            font-size: 14.5px;
            font-weight: 700;
            color: #0f172a;
            background: #f8fafc;
            padding: 4px 10px;
            border-radius: 6px;
            border: 1px solid #e2e8f0;
            display: inline-block;
        }

        /* Stock Status Badges */
        .stock-badge {
            display: inline-flex;
            align-items: center;
            gap: 5px;
            padding: 4px 12px;
            border-radius: 20px;
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

        /* Action View Specs Button */
        .btn-view-specs {
            display: inline-flex;
            align-items: center;
            gap: 4px;
            padding: 7px 14px;
            background: #eff6ff;
            color: #2563eb;
            border: 1px solid #bfdbfe;
            border-radius: 6px;
            font-size: 12.5px;
            font-weight: 700;
            text-decoration: none;
            transition: all 0.2s;
            cursor: pointer;
        }

        .btn-view-specs:hover {
            background: #2563eb;
            color: #ffffff;
            box-shadow: 0 2px 6px rgba(37, 99, 235, 0.25);
            transform: translateY(-1px);
        }

        /* Info Card Bottom */
        .querystring-info-card {
            background: #f0fdf4;
            border: 1px solid #bbf7d0;
            border-radius: 12px;
            padding: 16px 20px;
            display: flex;
            align-items: flex-start;
            gap: 12px;
            color: #166534;
            font-size: 13px;
            line-height: 1.5;
        }

        .querystring-info-card strong {
            color: #14532d;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="products-page-container">
        
        <!-- Module Header Card -->
        <div class="module-header-card">
            <div class="module-header-info">
                <div class="header-badge-wrap">
                    <span>&#128230;</span> SQL Server Inventory
                </div>
                <h1>Products Directory &amp; Catalog</h1>
                <p>Browse products fetched from SQL Server via <code>sp_GetBasicProductDetails</code>. Click on any product to navigate to <code>ProductDetails.aspx</code> with its parameters transferred via <strong>QueryString</strong>.</p>
            </div>
            <div class="header-actions">
                <a href="<%= ResolveUrl("~/ProductForm") %>" class="btn-add-product">
                    <span>&#10010;</span> Add New Product
                </a>
                <a href="<%= ResolveUrl("~/Home") %>" class="btn-hub-return">
                    <span>&larr;</span> Dashboard
                </a>
            </div>
        </div>

        <!-- Metric KPI Cards -->
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
                    <asp:Label ID="lblAvgPrice" runat="server" CssClass="metric-val" Text="₹ 0.00"></asp:Label>
                    <span class="metric-label">Avg Product Price</span>
                </div>
            </div>
        </div>

        <!-- Status Message Banner if any -->
        <asp:Label ID="lblStatus" runat="server" Visible="false" CssClass="status-msg"></asp:Label>

        <!-- Filter Toolbar -->
        <div class="filter-toolbar-card">
            <div class="search-box-wrap">
                <span class="search-icon">&#128269;</span>
                <input type="text" id="productSearchInput" class="search-input" placeholder="Quick filter by Product Name, ID or Price..." onkeyup="filterProductsTable()" />
            </div>

            <div class="sp-pill-indicator">
                <span>&#128451;</span> Procedure: <code>sp_GetBasicProductDetails</code>
            </div>
        </div>

        <!-- Table Card with GridView -->
        <div class="table-card">
            <div class="table-responsive">
                <asp:GridView ID="GridView1"
                    runat="server"
                    AutoGenerateColumns="False"
                    CssClass="gridview-table"
                    GridLines="None"
                    RowStyle-CssClass="product-row"
                    AlternatingRowStyle-CssClass="product-alt-row">

                    <Columns>
                        <%-- Product ID Column with Clickable LinkButton --%>
                        <asp:TemplateField HeaderText="Product ID" HeaderStyle-Width="120px" ItemStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <asp:LinkButton ID="LinkButton1"
                                    runat="server"
                                    CssClass="badge-product-id"
                                    Text='<%# "#" + Eval("ID") %>'
                                    CommandArgument='<%# Eval("ID") %>'
                                    OnClick="LinkButton1_Click"
                                    ToolTip="Click to view full specifications via QueryString">
                                </asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <%-- Product Name Column --%>
                        <asp:TemplateField HeaderText="Product Name" HeaderStyle-Width="300px">
                            <ItemTemplate>
                                <div class="product-name-cell">
                                    <div class="product-icon-wrap">&#128230;</div>
                                    <asp:LinkButton ID="lnkName"
                                        runat="server"
                                        CssClass="product-link"
                                        Text='<%# Eval("Name") %>'
                                        CommandArgument='<%# Eval("ID") %>'
                                        OnClick="LinkButton1_Click"
                                        ToolTip="Click to view details via QueryString">
                                    </asp:LinkButton>
                                </div>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <%-- Price Column --%>
                        <asp:TemplateField HeaderText="Price (INR)" HeaderStyle-Width="180px">
                            <ItemTemplate>
                                <span class="price-tag">
                                     <%# Convert.ToDecimal(Eval("Price")).ToString("N2") %>
                                </span>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <%-- Stock Available Column with Status Pill --%>
                        <asp:TemplateField HeaderText="Stock Availability" HeaderStyle-Width="220px">
                            <ItemTemplate>
                                <%# GetStockBadgeHtml(Eval("Stock")) %>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <%-- Actions Column --%>
                        <asp:TemplateField HeaderText="QueryString Action" HeaderStyle-Width="180px" ItemStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <a href='<%# ResolveUrl("~/ProductDetails.aspx?ID=" + Eval("ID")) %>' class="btn-view-specs" title="Inspect full specifications using QueryString ID">
                                    &#128269; View Specs &rarr;
                                </a>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>

                    <EmptyDataTemplate>
                        <div style="padding: 40px; text-align: center; color: #64748b;">
                            <div style="font-size: 40px; margin-bottom: 10px;">&#128230;</div>
                            <h3 style="font-size: 18px; color: #1e293b; margin-bottom: 6px;">No Products Found in Database</h3>
                            <p style="font-size: 13.5px;">Click the button below to insert your first product into SQL Server table <code>Products</code>.</p>
                            <a href='<%= ResolveUrl("~/ProductForm") %>' class="btn-add-product" style="margin-top: 14px; display: inline-flex;">+ Add New Product</a>
                        </div>
                    </EmptyDataTemplate>
                </asp:GridView>
            </div>
        </div>

        <!-- QueryString Educational Note -->
        <div class="querystring-info-card">
            <span style="font-size: 20px;">&#128161;</span>
            <div>
                <strong>How QueryString Data Transfer Works:</strong> When you click on any Product ID, Name, or <em>View Specs &rarr;</em> button, ASP.NET captures the product ID and initiates a client redirect: <code>Response.Redirect("~/ProductDetails.aspx?ID=" + id)</code>. The destination page reads <code>Request.QueryString["ID"]</code> to fetch comprehensive specs (Color, Size, Weight, Supplier, Discontinued status) using stored procedure <code>sp_GetProductDetailsByID</code>.
            </div>
        </div>

    </div>

    <!-- Client-side Quick Search Filter Script -->
    <script type="text/javascript">
        function filterProductsTable() {
            var input = document.getElementById("productSearchInput");
            var filter = input.value.toLowerCase();
            var rows = document.querySelectorAll(".gridview-table tr:not(:first-child)");

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
