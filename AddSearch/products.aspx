<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="products.aspx.cs" Inherits="AddSearch.products" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Products</title>

    <style>
        body {
            font-family: Arial, Helvetica, sans-serif;
            background-color: #f4f6f9;
            margin: 0;
            padding: 20px;
        }

        .container {
            width: 80%;
            margin: auto;
            background: white;
            padding: 20px;
            border-radius: 10px;
            box-shadow: 0 2px 8px rgba(0,0,0,0.15);
        }

        .heading {
            text-align: center;
            color: #2c3e50;
            margin-bottom: 20px;
        }

        .grid {
            width: 100%;
            border-collapse: collapse;
        }

        .grid th {
            background-color: #0078D7;
            color: white;
            padding: 12px;
            text-align: left;
        }

        .grid td {
            padding: 10px;
            border-bottom: 1px solid #ddd;
        }

        .grid tr:nth-child(even) {
            background-color: #f9f9f9;
        }

        .grid tr:hover {
            background-color: #eef6ff;
        }

        .product-link {
            color: #0078D7;
            text-decoration: none;
            font-weight: bold;
        }

        .product-link:hover {
            color: #0056a3;
            text-decoration: underline;
        }
    </style>
</head>
<body>

    <form id="form1" runat="server">

        <div class="container">

            <h2 class="heading">Product List</h2>

            <asp:GridView ID="GridView1"
                runat="server"
                AutoGenerateColumns="False"
                CssClass="grid"
                GridLines="None">

                <Columns>

                    <asp:TemplateField HeaderText="Product ID">
                        <ItemTemplate>
                            <asp:LinkButton ID="LinkButton1"
                                runat="server"
                                CssClass="product-link"
                                Text='<%# Eval("ID") %>'
                                CommandArgument='<%# Eval("ID") %>'
                                OnClick="LinkButton1_Click">
                            </asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:BoundField DataField="Name"
                        HeaderText="Product Name" />

                    <asp:BoundField DataField="Price"
                        HeaderText="Price" />

                    <asp:BoundField DataField="Stock"
                        HeaderText="Stock Available" />

                </Columns>

            </asp:GridView>

        </div>

    </form>

</body>
</html>