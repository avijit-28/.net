<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="products.aspx.cs" Inherits="AddSearch.products" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Product Inventory - SQL Server Catalog</title>
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <link rel="preconnect" href="https://fonts.googleapis.com" />
    <link rel="preconnect" href="https://fonts.gstatic.com" crossorigin="anonymous" />
    <link href="https://fonts.googleapis.com/css2?family=Plus+Jakarta+Sans:wght@400;500;600;700;800&display=swap" rel="stylesheet" />

    <style>
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
            max-width: 1100px;
            margin: 0 auto;
        }

        .header-card {
            background: #ffffff;
            border-radius: 16px;
            padding: 24px 30px;
            margin-bottom: 24px;
            box-shadow: 0 4px 20px -2px rgba(0, 0, 0, 0.05);
            border: 1px solid var(--slate-200);
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

        .header-title h1 {
            font-size: 24px;
            font-weight: 800;
            color: var(--slate-900);
            letter-spacing: -0.02em;
        }

        .header-title p {
            color: var(--slate-600);
            font-size: 13.5px;
            margin-top: 4px;
        }

        .table-card {
            background: #ffffff;
            border-radius: 16px;
            box-shadow: 0 10px 30px -4px rgba(0, 0, 0, 0.06);
            border: 1px solid var(--slate-200);
            overflow: hidden;
            margin-bottom: 24px;
        }

        .toolbar {
            padding: 16px 24px;
            background: #fafafa;
            border-bottom: 1px solid var(--slate-200);
            display: flex;
            justify-content: space-between;
            align-items: center;
            flex-wrap: wrap;
            gap: 12px;
        }

        .search-input {
            padding: 8px 14px;
            border: 1.5px solid var(--slate-200);
            border-radius: 8px;
            font-size: 13px;
            font-family: inherit;
            width: 280px;
            outline: none;
            transition: all 0.2s;
        }

        .search-input:focus {
            border-color: var(--primary);
            box-shadow: 0 0 0 3px rgba(37, 99, 235, 0.15);
        }

        .sp-tag {
            font-size: 12px;
            font-weight: 700;
            color: #1e40af;
            background: #eff6ff;
            border: 1px solid #bfdbfe;
            padding: 4px 12px;
            border-radius: 20px;
        }

        .table-responsive {
            width: 100%;
            overflow-x: auto;
        }

        .grid {
            width: 100%;
            border-collapse: collapse;
            font-size: 13.5px;
        }

        .grid th {
            background: var(--slate-100);
            color: var(--slate-700);
            font-weight: 700;
            text-transform: uppercase;
            font-size: 11.5px;
            letter-spacing: 0.05em;
            padding: 14px 18px;
            text-align: left;
            border-bottom: 2px solid var(--slate-200);
        }

        .grid td {
            padding: 13px 18px;
            border-bottom: 1px solid var(--slate-200);
            vertical-align: middle;
        }

        .grid tr:hover {
            background: #f8fafc;
        }

        .product-id-link {
            display: inline-flex;
            align-items: center;
            gap: 4px;
            padding: 4px 10px;
            background: #eff6ff;
            color: var(--primary);
            border: 1px solid #bfdbfe;
            border-radius: 6px;
            font-weight: 800;
            font-size: 12.5px;
            text-decoration: none;
            transition: all 0.2s;
        }

        .product-id-link:hover {
            background: var(--primary);
            color: #ffffff;
        }

        .product-name-link {
            color: var(--slate-900);
            font-weight: 700;
            text-decoration: none;
            transition: color 0.15s;
        }

        .product-name-link:hover {
            color: var(--primary);
            text-decoration: underline;
        }

        .price-badge {
            font-family: monospace;
            font-weight: 700;
            font-size: 14px;
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

        .btn-view-details {
            padding: 6px 14px;
            background: #eff6ff;
            color: var(--primary);
            border: 1px solid #bfdbfe;
            border-radius: 6px;
            font-size: 12.5px;
            font-weight: 700;
            text-decoration: none;
            transition: all 0.2s;
            display: inline-flex;
            align-items: center;
            gap: 4px;
        }

        .btn-view-details:hover {
            background: var(--primary);
            color: #ffffff;
        }

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
                    <div class="header-badge">&#128230; Products Catalog</div>
                    <h1>Products Directory</h1>
                    <p>Click on any Product ID or Name to view specifications passed via <strong>QueryString</strong>.</p>
                </div>
            </div>

            <div class="table-card">
                <div class="toolbar">
                    <input type="text" id="searchInput" class="search-input" placeholder="Quick filter products..." onkeyup="filterProducts()" />
                    <span class="sp-tag">&#128451; Stored Procedure: <code>sp_GetBasicProductDetails</code></span>
                </div>

                <div class="table-responsive">
                    <asp:GridView ID="GridView1"
                        runat="server"
                        AutoGenerateColumns="False"
                        CssClass="grid"
                        GridLines="None">

                        <Columns>
                            <%-- Product ID --%>
                            <asp:TemplateField HeaderText="Product ID" HeaderStyle-Width="120px" ItemStyle-HorizontalAlign="Center">
                                <ItemTemplate>
                                    <asp:LinkButton ID="LinkButton1"
                                        runat="server"
                                        CssClass="product-id-link"
                                        Text='<%# "#" + Eval("ID") %>'
                                        CommandArgument='<%# Eval("ID") %>'
                                        OnClick="LinkButton1_Click">
                                    </asp:LinkButton>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <%-- Product Name --%>
                            <asp:TemplateField HeaderText="Product Name" HeaderStyle-Width="280px">
                                <ItemTemplate>
                                    <asp:LinkButton ID="lnkName"
                                        runat="server"
                                        CssClass="product-name-link"
                                        Text='<%# Eval("Name") %>'
                                        CommandArgument='<%# Eval("ID") %>'
                                        OnClick="LinkButton1_Click">
                                    </asp:LinkButton>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <%-- Price --%>
                            <asp:TemplateField HeaderText="Price ($)" HeaderStyle-Width="160px">
                                <ItemTemplate>
                                    <span class="price-badge">$ <%# Convert.ToDecimal(Eval("Price")).ToString("N2") %></span>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <%-- Stock Available --%>
                            <asp:TemplateField HeaderText="Stock Availability" HeaderStyle-Width="200px">
                                <ItemTemplate>
                                    <%# GetStockPill(Eval("Stock")) %>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <%-- Action --%>
                            <asp:TemplateField HeaderText="QueryString Action" HeaderStyle-Width="160px" ItemStyle-HorizontalAlign="Center">
                                <ItemTemplate>
                                    <a href='<%# "ProductDetails.aspx?ID=" + Eval("ID") %>' class="btn-view-details">
                                        View Specs &rarr;
                                    </a>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>

                    </asp:GridView>
                </div>
            </div>

            <div class="info-card">
                &#128161; <strong>QueryString Navigation:</strong> Clicking a product performs <code>Response.Redirect("~/ProductDetails.aspx?ID=" + id)</code>. The destination page reads <code>Request.QueryString["ID"]</code> and queries <code>sp_GetProductDetailsByID</code>.
            </div>

        </div>
    </form>

    <script>
        function filterProducts() {
            var input = document.getElementById("searchInput");
            var filter = input.value.toLowerCase();
            var rows = document.querySelectorAll(".grid tr:not(:first-child)");
            rows.forEach(function (row) {
                var text = row.textContent.toLowerCase();
                row.style.display = text.indexOf(filter) > -1 ? "" : "none";
            });
        }
    </script>
</body>
</html>