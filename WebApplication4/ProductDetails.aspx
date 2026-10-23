<%@ Page Title="Product Details" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ProductDetails.aspx.cs" Inherits="WebApplication4.ProductDetails" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .product-details-container {
            width: 100%;
            max-width: 1000px;
            margin: 0 auto;
        }

        /* Top Breadcrumb & Return Bar */
        .details-top-bar {
            display: flex;
            align-items: center;
            justify-content: space-between;
            margin-bottom: 20px;
            flex-wrap: wrap;
            gap: 12px;
        }

        .breadcrumb-wrap {
            display: flex;
            align-items: center;
            gap: 8px;
            font-size: 13.5px;
            color: #64748b;
        }

        .breadcrumb-wrap a {
            color: #2563eb;
            text-decoration: none;
            font-weight: 600;
        }

        .breadcrumb-wrap a:hover {
            text-decoration: underline;
        }

        .btn-catalog-return {
            display: inline-flex;
            align-items: center;
            gap: 6px;
            padding: 8px 16px;
            background: #ffffff;
            color: #1e293b;
            border: 1.5px solid #cbd5e1;
            border-radius: 8px;
            font-size: 13px;
            font-weight: 700;
            text-decoration: none;
            transition: all 0.2s ease;
            box-shadow: 0 1px 3px rgba(0, 0, 0, 0.05);
        }

        .btn-catalog-return:hover {
            background: #eff6ff;
            color: #2563eb;
            border-color: #bfdbfe;
        }

        /* Main Master Card */
        .product-card {
            background: #ffffff;
            border-radius: 18px;
            border: 1px solid #e2e8f0;
            box-shadow: 0 10px 30px -5px rgba(0, 0, 0, 0.08);
            overflow: hidden;
            margin-bottom: 24px;
        }

        /* Card Header */
        .card-header-gradient {
            background: linear-gradient(135deg, #1e40af 0%, #2563eb 50%, #3b82f6 100%);
            padding: 30px 36px;
            color: #ffffff;
            display: flex;
            justify-content: space-between;
            align-items: center;
            flex-wrap: wrap;
            gap: 20px;
        }

        .header-title-box {
            max-width: 600px;
        }

        .header-badge-tag {
            display: inline-flex;
            align-items: center;
            gap: 6px;
            background: rgba(255, 255, 255, 0.2);
            backdrop-filter: blur(8px);
            padding: 4px 14px;
            border-radius: 20px;
            font-size: 11.5px;
            font-weight: 700;
            text-transform: uppercase;
            letter-spacing: 0.05em;
            margin-bottom: 10px;
            border: 1px solid rgba(255, 255, 255, 0.3);
        }

        .header-product-title {
            font-size: 28px;
            font-weight: 800;
            letter-spacing: -0.02em;
            display: flex;
            align-items: center;
            gap: 12px;
            margin-bottom: 6px;
        }

        .header-product-id-pill {
            background: #ffffff;
            color: #1e40af;
            font-size: 14px;
            padding: 3px 12px;
            border-radius: 12px;
            font-weight: 800;
        }

        .header-subtitle {
            font-size: 14px;
            color: #bfdbfe;
            line-height: 1.4;
        }

        /* Dropdown Product Switcher in Header */
        .product-switcher-box {
            background: rgba(255, 255, 255, 0.15);
            backdrop-filter: blur(8px);
            padding: 12px 18px;
            border-radius: 12px;
            border: 1px solid rgba(255, 255, 255, 0.25);
            display: flex;
            flex-direction: column;
            gap: 6px;
        }

        .switcher-label {
            font-size: 11px;
            font-weight: 700;
            color: #e0f2fe;
            text-transform: uppercase;
            letter-spacing: 0.05em;
        }

        .product-switcher-select {
            padding: 8px 12px;
            border-radius: 8px;
            font-size: 13px;
            border: 1.5px solid #ffffff;
            background: #ffffff;
            color: #0f172a;
            font-family: inherit;
            font-weight: 600;
            cursor: pointer;
            outline: none;
        }

        /* QueryString Status Notice */
        .qs-status-bar {
            padding: 12px 24px;
            background: #f8fafc;
            border-bottom: 1px solid #e2e8f0;
            display: flex;
            align-items: center;
            justify-content: space-between;
            flex-wrap: wrap;
            gap: 10px;
            font-size: 13px;
        }

        .qs-pill {
            display: inline-flex;
            align-items: center;
            gap: 6px;
            background: #eff6ff;
            color: #1e40af;
            border: 1px solid #bfdbfe;
            padding: 4px 12px;
            border-radius: 16px;
            font-weight: 700;
            font-family: monospace;
            font-size: 12px;
        }

        .qs-notice-warn {
            color: #b45309;
            font-weight: 600;
            display: inline-flex;
            align-items: center;
            gap: 6px;
        }

        /* Product Showcase Grid Layout */
        .showcase-body {
            padding: 32px;
            display: grid;
            grid-template-columns: 320px 1fr;
            gap: 32px;
        }

        @media (max-width: 860px) {
            .showcase-body {
                grid-template-columns: 1fr;
            }
        }

        /* Left Side: Highlight Cards */
        .highlight-col {
            display: flex;
            flex-direction: column;
            gap: 16px;
        }

        .price-highlight-card {
            background: linear-gradient(135deg, #f8fafc 0%, #eff6ff 100%);
            border: 1.5px solid #bfdbfe;
            border-radius: 14px;
            padding: 22px;
            text-align: center;
        }

        .price-label {
            font-size: 12px;
            font-weight: 700;
            color: #64748b;
            text-transform: uppercase;
            letter-spacing: 0.05em;
            margin-bottom: 6px;
        }

        .price-huge {
            font-size: 36px;
            font-weight: 800;
            color: #1e40af;
            font-family: monospace;
            line-height: 1.1;
        }

        .spec-tile {
            background: #ffffff;
            border: 1px solid #e2e8f0;
            border-radius: 12px;
            padding: 16px;
            display: flex;
            align-items: center;
            gap: 14px;
            box-shadow: 0 1px 4px rgba(0, 0, 0, 0.03);
        }

        .spec-tile-icon {
            width: 44px;
            height: 44px;
            border-radius: 10px;
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 20px;
            flex-shrink: 0;
        }

        .spec-tile-data {
            display: flex;
            flex-direction: column;
        }

        .spec-tile-title {
            font-size: 11.5px;
            font-weight: 700;
            color: #64748b;
            text-transform: uppercase;
            letter-spacing: 0.04em;
        }

        .spec-tile-value {
            font-size: 15px;
            font-weight: 700;
            color: #0f172a;
            margin-top: 2px;
        }

        /* Right Side: Specifications Table */
        .specs-col {
            display: flex;
            flex-direction: column;
        }

        .specs-title {
            font-size: 18px;
            font-weight: 800;
            color: #0f172a;
            margin-bottom: 16px;
            display: flex;
            align-items: center;
            gap: 8px;
        }

        .specs-table {
            width: 100%;
            border-collapse: collapse;
            font-size: 14px;
            border: 1px solid #e2e8f0;
            border-radius: 12px;
            overflow: hidden;
        }

        .specs-table td {
            padding: 13px 18px;
            border-bottom: 1px solid #f1f5f9;
        }

        .specs-label-col {
            width: 38%;
            background: #f8fafc;
            color: #475569;
            font-weight: 700;
            border-right: 1px solid #e2e8f0;
        }

        .specs-val-col {
            color: #0f172a;
            font-weight: 600;
        }

        .specs-table tr:hover td {
            background: #fafafa;
        }

        /* Color Swatch Indicator */
        .color-bubble {
            display: inline-flex;
            align-items: center;
            gap: 8px;
            padding: 3px 10px;
            background: #f8fafc;
            border: 1px solid #cbd5e1;
            border-radius: 16px;
            font-size: 13px;
            font-weight: 700;
        }

        .color-dot {
            width: 14px;
            height: 14px;
            border-radius: 50%;
            border: 1px solid rgba(0, 0, 0, 0.2);
            box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
            display: inline-block;
        }

        /* Status Pills */
        .status-pill {
            display: inline-flex;
            align-items: center;
            gap: 5px;
            padding: 4px 12px;
            border-radius: 20px;
            font-size: 12px;
            font-weight: 700;
        }

        .status-active {
            background: #ecfdf5;
            color: #065f46;
            border: 1px solid #a7f3d0;
        }

        .status-discontinued {
            background: #fff1f2;
            color: #9f1239;
            border: 1px solid #fecdd3;
        }

        /* Bottom Info & Action Bar */
        .bottom-action-bar {
            padding: 20px 32px;
            background: #f8fafc;
            border-top: 1px solid #e2e8f0;
            display: flex;
            align-items: center;
            justify-content: space-between;
            flex-wrap: wrap;
            gap: 16px;
        }

        .action-btns {
            display: flex;
            align-items: center;
            gap: 10px;
        }

        .btn-primary-action {
            display: inline-flex;
            align-items: center;
            gap: 6px;
            padding: 10px 20px;
            background: linear-gradient(135deg, #2563eb 0%, #1d4ed8 100%);
            color: #ffffff;
            border-radius: 8px;
            font-size: 13.5px;
            font-weight: 700;
            text-decoration: none;
            box-shadow: 0 4px 12px rgba(37, 99, 235, 0.25);
            transition: all 0.2s ease;
        }

        .btn-primary-action:hover {
            background: linear-gradient(135deg, #1d4ed8 0%, #1e40af 100%);
            box-shadow: 0 6px 16px rgba(37, 99, 235, 0.35);
            transform: translateY(-1px);
        }

        .btn-secondary-action {
            display: inline-flex;
            align-items: center;
            gap: 6px;
            padding: 10px 18px;
            background: #ffffff;
            color: #334155;
            border: 1.5px solid #cbd5e1;
            border-radius: 8px;
            font-size: 13.5px;
            font-weight: 700;
            text-decoration: none;
            transition: all 0.2s ease;
        }

        .btn-secondary-action:hover {
            background: #f1f5f9;
            color: #0f172a;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="product-details-container">
        
        <!-- Top Breadcrumb & Return Bar -->
        <div class="details-top-bar">
            <div class="breadcrumb-wrap">
                <a href="<%= ResolveUrl("~/Home") %>">&#127968; Home</a>
                <span>&rsaquo;</span>
                <a href="<%= ResolveUrl("~/products.aspx") %>">&#128230; Products Catalog</a>
                <span>&rsaquo;</span>
                <span>Product #<asp:Literal ID="litBreadcrumbID" runat="server" Text="1" /></span>
            </div>

            <a href="<%= ResolveUrl("~/products.aspx") %>" class="btn-catalog-return">
                &larr; Return to Products Directory
            </a>
        </div>

        <!-- Master Product Card -->
        <div class="product-card">
            
            <!-- Card Header -->
            <div class="card-header-gradient">
                <div class="header-title-box">
                    <div class="header-badge-tag">&#128196; Product Specification Sheet</div>
                    <div class="header-product-title">
                        <span><asp:Literal ID="litHeaderProductName" runat="server" Text="Product Details" /></span>
                        <span class="header-product-id-pill">ID #<asp:Literal ID="litHeaderProductID" runat="server" Text="1" /></span>
                    </div>
                    <p class="header-subtitle">
                        Complete inventory specifications retrieved from SQL Server procedure <code>sp_GetProductDetailsByID</code>.
                    </p>
                </div>

                <!-- Interactive Switcher to test QueryString directly -->
                <div class="product-switcher-box">
                    <span class="switcher-label">&#128260; Switch Product (QueryString Test)</span>
                    <asp:DropDownList ID="ddlSwitchProduct" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlSwitchProduct_SelectedIndexChanged" CssClass="product-switcher-select">
                    </asp:DropDownList>
                </div>
            </div>

            <!-- QueryString Diagnostic Bar -->
            <div class="qs-status-bar">
                <div class="qs-pill">
                    <span>&#128279;</span> Request.QueryString["ID"] = "<asp:Literal ID="litQueryStringID" runat="server" Text="1" />"
                </div>

                <asp:Panel ID="pnlDefaultNotice" runat="server" Visible="false" CssClass="qs-notice-warn">
                    <span>&#9888;</span> Note: No ID was specified in the URL QueryString. Product #1 is displayed by default.
                </asp:Panel>
            </div>

            <!-- Product Showcase Body -->
            <div class="showcase-body">
                
                <!-- Left Column: Key Metric Highlights -->
                <div class="highlight-col">
                    <!-- Price Card -->
                    <div class="price-highlight-card">
                        <div class="price-label">Retail Price</div>
                        <div class="price-huge">₹ <asp:Label ID="lblPriceHighlight" runat="server" Text="0.00"></asp:Label></div>
                    </div>

                    <!-- Stock Status Tile -->
                    <div class="spec-tile">
                        <div class="spec-tile-icon" style="background:#ecfdf5;color:#059669;">&#128230;</div>
                        <div class="spec-tile-data">
                            <span class="spec-tile-title">Stock Available</span>
                            <span class="spec-tile-value"><asp:Label ID="lblStockCount" runat="server" Text="0"></asp:Label> Units</span>
                        </div>
                    </div>

                    <!-- Size Tile -->
                    <div class="spec-tile">
                        <div class="spec-tile-icon" style="background:#eff6ff;color:#2563eb;">&#128207;</div>
                        <div class="spec-tile-data">
                            <span class="spec-tile-title">Product Size</span>
                            <span class="spec-tile-value"><asp:Label ID="lblSizeHighlight" runat="server" Text="Normal"></asp:Label></span>
                        </div>
                    </div>

                    <!-- Weight Tile -->
                    <div class="spec-tile">
                        <div class="spec-tile-icon" style="background:#fdf4ff;color:#c026d3;">&#9878;</div>
                        <div class="spec-tile-data">
                            <span class="spec-tile-title">Weight</span>
                            <span class="spec-tile-value"><asp:Label ID="lblWeightHighlight" runat="server" Text="0.00"></asp:Label> kg</span>
                        </div>
                    </div>
                </div>

                <!-- Right Column: Full Specifications Table -->
                <div class="specs-col">
                    <div class="specs-title">
                        <span>&#128221;</span> Detailed Technical Specifications
                    </div>

                    <table class="specs-table">
                        <tr>
                            <td class="specs-label-col">Product ID</td>
                            <td class="specs-val-col">
                                <strong>#<asp:Label ID="lblID" runat="server"></asp:Label></strong>
                            </td>
                        </tr>

                        <tr>
                            <td class="specs-label-col">Product Name</td>
                            <td class="specs-val-col">
                                <asp:Label ID="lblName" runat="server"></asp:Label>
                            </td>
                        </tr>

                        <tr>
                            <td class="specs-label-col">Price (INR)</td>
                            <td class="specs-val-col">
                                ₹ <asp:Label ID="lblPrice" runat="server"></asp:Label>
                            </td>
                        </tr>

                        <tr>
                            <td class="specs-label-col">Color Variant</td>
                            <td class="specs-val-col">
                                <div class="color-bubble">
                                    <span class="color-dot" id="colorDotIndicator" runat="server"></span>
                                    <asp:Label ID="lblColor" runat="server"></asp:Label>
                                </div>
                            </td>
                        </tr>

                        <tr>
                            <td class="specs-label-col">Form Factor / Size</td>
                            <td class="specs-val-col">
                                <asp:Label ID="lblSize" runat="server"></asp:Label>
                            </td>
                        </tr>

                        <tr>
                            <td class="specs-label-col">Item Weight</td>
                            <td class="specs-val-col">
                                <asp:Label ID="lblWeight" runat="server"></asp:Label> kg
                            </td>
                        </tr>

                        <tr>
                            <td class="specs-label-col">Warehouse Stock</td>
                            <td class="specs-val-col">
                                <asp:Label ID="lblQuantityInStock" runat="server"></asp:Label> Units
                            </td>
                        </tr>

                        <tr>
                            <td class="specs-label-col">Lifecycle Status</td>
                            <td class="specs-val-col">
                                <asp:Label ID="lblDiscontinued" runat="server"></asp:Label>
                            </td>
                        </tr>

                        <tr>
                            <td class="specs-label-col">Authorized Supplier</td>
                            <td class="specs-val-col">
                                <span style="display:inline-flex;align-items:center;gap:6px;">
                                    <span>&#127970;</span>
                                    <asp:Label ID="lblSupplier" runat="server"></asp:Label>
                                </span>
                            </td>
                        </tr>
                    </table>
                </div>

            </div>

            <!-- Bottom Action & Info Bar -->
            <div class="bottom-action-bar">
                <div class="action-btns">
                    <a href="<%= ResolveUrl("~/products.aspx") %>" class="btn-primary-action">
                        &larr; Back to Products Directory
                    </a>
                    <a href="<%= ResolveUrl("~/ProductForm") %>" class="btn-secondary-action">
                        + Add Another Product
                    </a>
                </div>

                <div style="font-size:12.5px;color:#64748b;">
                    Joined SQL Tables: <code>Products</code> &amp; <code>ProductDetails</code> via Stored Procedure
                </div>
            </div>

        </div>

    </div>
</asp:Content>
