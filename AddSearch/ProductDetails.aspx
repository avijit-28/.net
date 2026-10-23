<%@ Page Language="C#" AutoEventWireup="true"
    CodeBehind="ProductDetails.aspx.cs"
    Inherits="AddSearch.ProductDetails" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Product Specifications Sheet</title>
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <link rel="preconnect" href="https://fonts.googleapis.com" />
    <link rel="preconnect" href="https://fonts.gstatic.com" crossorigin="anonymous" />
    <link href="https://fonts.googleapis.com/css2?family=Plus+Jakarta+Sans:wght@400;500;600;700;800&display=swap" rel="stylesheet" />

    <style type="text/css">
        :root {
            --primary: #2563eb;
            --primary-hover: #1d4ed8;
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
            max-width: 860px;
            margin: 0 auto;
        }

        .card {
            background: #ffffff;
            border-radius: 18px;
            box-shadow: 0 10px 30px -5px rgba(0,0,0,0.08);
            border: 1px solid var(--slate-200);
            overflow: hidden;
            margin-bottom: 24px;
        }

        .header {
            background: linear-gradient(135deg, #1e40af 0%, #2563eb 100%);
            color: #ffffff;
            padding: 26px 30px;
            display: flex;
            justify-content: space-between;
            align-items: center;
            flex-wrap: wrap;
            gap: 16px;
        }

        .header-title-box h1 {
            font-size: 24px;
            font-weight: 800;
            letter-spacing: -0.02em;
            margin-bottom: 4px;
        }

        .header-title-box p {
            color: #bfdbfe;
            font-size: 13.5px;
        }

        .switcher-box {
            display: flex;
            align-items: center;
            gap: 8px;
            background: rgba(255, 255, 255, 0.15);
            padding: 6px 12px;
            border-radius: 10px;
        }

        .switcher-select {
            padding: 6px 10px;
            border-radius: 6px;
            border: none;
            font-family: inherit;
            font-size: 13px;
            font-weight: 600;
            background: #ffffff;
            color: #0f172a;
            cursor: pointer;
            outline: none;
        }

        .qs-info-bar {
            padding: 10px 24px;
            background: #f8fafc;
            border-bottom: 1px solid var(--slate-200);
            font-size: 12.5px;
            display: flex;
            align-items: center;
            gap: 8px;
        }

        .qs-badge {
            background: #eff6ff;
            color: #1e40af;
            border: 1px solid #bfdbfe;
            padding: 2px 10px;
            border-radius: 12px;
            font-weight: 700;
            font-family: monospace;
        }

        .details-table {
            width: 100%;
            border-collapse: collapse;
            font-size: 14px;
        }

        .details-table td {
            padding: 14px 24px;
            border-bottom: 1px solid var(--slate-200);
        }

        .label-cell {
            width: 35%;
            font-weight: 700;
            background-color: #f8fafc;
            color: var(--slate-700);
            border-right: 1px solid var(--slate-200);
        }

        .value-cell {
            color: #0f172a;
            font-weight: 600;
        }

        .details-table tr:hover td {
            background-color: #fafafa;
        }

        .price-text {
            font-family: monospace;
            font-size: 18px;
            font-weight: 800;
            color: #1e40af;
        }

        .status-pill {
            display: inline-flex;
            align-items: center;
            gap: 4px;
            padding: 3px 10px;
            border-radius: 16px;
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

        .card-footer-action {
            text-align: center;
            padding: 20px;
            background: #f8fafc;
            border-top: 1px solid var(--slate-200);
            display: flex;
            align-items: center;
            justify-content: space-between;
            flex-wrap: wrap;
            gap: 12px;
        }

        .back-btn {
            display: inline-flex;
            align-items: center;
            gap: 6px;
            padding: 8px 18px;
            background-color: var(--primary);
            color: white;
            text-decoration: none;
            border-radius: 8px;
            font-weight: 700;
            font-size: 13.5px;
            box-shadow: 0 2px 6px rgba(37, 99, 235, 0.25);
            transition: all 0.2s;
        }

        .back-btn:hover {
            background-color: var(--primary-hover);
        }
    </style>
</head>
<body>

    <form id="form1" runat="server">
        <div class="container">

            <div class="card">
                <div class="header">
                    <div class="header-title-box">
                        <h1>Product Specification Details</h1>
                        <p>Retrieved from SQL Server via QueryString ID</p>
                    </div>

                    <div class="switcher-box">
                        <span style="font-size:12px;font-weight:700;">Switch ID:</span>
                        <asp:DropDownList ID="ddlProductSwitch" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlProductSwitch_SelectedIndexChanged" CssClass="switcher-select">
                        </asp:DropDownList>
                    </div>
                </div>

                <div class="qs-info-bar">
                    <span>&#128279; <strong>QueryString Data:</strong></span>
                    <span class="qs-badge">Request.QueryString["ID"] = "<asp:Literal ID="litIdDisplay" runat="server" />"</span>
                </div>

                <table class="details-table">
                    <tr>
                        <td class="label-cell">Product ID</td>
                        <td class="value-cell">
                            <strong>#<asp:Label ID="lblID" runat="server"></asp:Label></strong>
                        </td>
                    </tr>

                    <tr>
                        <td class="label-cell">Product Name</td>
                        <td class="value-cell">
                            <asp:Label ID="lblName" runat="server"></asp:Label>
                        </td>
                    </tr>

                    <tr>
                        <td class="label-cell">Retail Price</td>
                        <td class="value-cell">
                            <span class="price-text">₹ <asp:Label ID="lblPrice" runat="server"></asp:Label></span>
                        </td>
                    </tr>

                    <tr>
                        <td class="label-cell">Color Variant</td>
                        <td class="value-cell">
                            <asp:Label ID="lblColor" runat="server"></asp:Label>
                        </td>
                    </tr>

                    <tr>
                        <td class="label-cell">Size / Form Factor</td>
                        <td class="value-cell">
                            <asp:Label ID="lblSize" runat="server"></asp:Label>
                        </td>
                    </tr>

                    <tr>
                        <td class="label-cell">Item Weight</td>
                        <td class="value-cell">
                            <asp:Label ID="lblWeight" runat="server"></asp:Label> kg
                        </td>
                    </tr>

                    <tr>
                        <td class="label-cell">Quantity In Stock</td>
                        <td class="value-cell">
                            <asp:Label ID="lblQuantityInStock" runat="server"></asp:Label> units
                        </td>
                    </tr>

                    <tr>
                        <td class="label-cell">Lifecycle Status</td>
                        <td class="value-cell">
                            <asp:Label ID="lblDiscontinued" runat="server"></asp:Label>
                        </td>
                    </tr>

                    <tr>
                        <td class="label-cell">Authorized Supplier</td>
                        <td class="value-cell">
                            &#127970; <asp:Label ID="lblSupplier" runat="server"></asp:Label>
                        </td>
                    </tr>
                </table>

                <div class="card-footer-action">
                    <asp:HyperLink ID="lnkBack"
                        runat="server"
                        NavigateUrl="~/Products.aspx"
                        CssClass="back-btn">
                        &larr; Back to Products Directory
                    </asp:HyperLink>

                    <span style="font-size:12px;color:#64748b;">
                        Joined SQL Tables: <code>Products</code> &amp; <code>ProductDetails</code>
                    </span>
                </div>
            </div>

        </div>
    </form>

</body>
</html>
