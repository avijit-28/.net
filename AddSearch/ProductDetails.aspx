<%@ Page Language="C#" AutoEventWireup="true"
    CodeBehind="ProductDetails.aspx.cs"
    Inherits="AddSearch.ProductDetails" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Product Details</title>

    <style type="text/css">
        body {
            font-family: Arial, Helvetica, sans-serif;
            background-color: #f4f6f9;
            margin: 0;
            padding: 30px;
        }

        .card {
            width: 600px;
            margin: auto;
            background: white;
            border-radius: 10px;
            box-shadow: 0 2px 10px rgba(0,0,0,0.15);
            overflow: hidden;
        }

        .header {
            background-color: #0078D7;
            color: white;
            padding: 15px;
            text-align: center;
            font-size: 24px;
            font-weight: bold;
        }

        .details-table {
            width: 100%;
            border-collapse: collapse;
        }

        .details-table td {
            padding: 12px;
            border-bottom: 1px solid #e0e0e0;
        }

        .label-cell {
            width: 40%;
            font-weight: bold;
            background-color: #f8f9fa;
        }

        .value-cell {
            color: #333333;
        }

        .details-table tr:hover {
            background-color: #f1f7ff;
        }

        .back-btn {
            display: inline-block;
            margin: 15px;
            padding: 8px 15px;
            background-color: #0078D7;
            color: white;
            text-decoration: none;
            border-radius: 4px;
        }

        .back-btn:hover {
            background-color: #005fa3;
        }
    </style>

</head>
<body>

    <form id="form1" runat="server">

        <div class="card">

            <div class="header">
                Product Details
            </div>

            <table class="details-table">

                <tr>
                    <td class="label-cell">Product ID</td>
                    <td class="value-cell">
                        <asp:Label ID="lblID" runat="server"></asp:Label>
                    </td>
                </tr>

                <tr>
                    <td class="label-cell">Product Name</td>
                    <td class="value-cell">
                        <asp:Label ID="lblName" runat="server"></asp:Label>
                    </td>
                </tr>

                <tr>
                    <td class="label-cell">Price</td>
                    <td class="value-cell">
                        ₹ <asp:Label ID="lblPrice" runat="server"></asp:Label>
                    </td>
                </tr>

                <tr>
                    <td class="label-cell">Color</td>
                    <td class="value-cell">
                        <asp:Label ID="lblColor" runat="server"></asp:Label>
                    </td>
                </tr>

                <tr>
                    <td class="label-cell">Size</td>
                    <td class="value-cell">
                        <asp:Label ID="lblSize" runat="server"></asp:Label>
                    </td>
                </tr>

                <tr>
                    <td class="label-cell">Weight</td>
                    <td class="value-cell">
                        <asp:Label ID="lblWeight" runat="server"></asp:Label>
                    </td>
                </tr>

                <tr>
                    <td class="label-cell">Quantity In Stock</td>
                    <td class="value-cell">
                        <asp:Label ID="lblQuantityInStock" runat="server"></asp:Label>
                    </td>
                </tr>

                <tr>
                    <td class="label-cell">Discontinued</td>
                    <td class="value-cell">
                        <asp:Label ID="lblDiscontinued" runat="server"></asp:Label>
                    </td>
                </tr>

                <tr>
                    <td class="label-cell">Supplier</td>
                    <td class="value-cell">
                        <asp:Label ID="lblSupplier" runat="server"></asp:Label>
                    </td>
                </tr>

            </table>

            <div style="text-align:center;padding:15px;">
                <asp:HyperLink ID="lnkBack"
                    runat="server"
                    NavigateUrl="~/Products.aspx"
                    CssClass="back-btn">
                    Back to Products
                </asp:HyperLink>
            </div>

        </div>

    </form>

</body>
</html>
