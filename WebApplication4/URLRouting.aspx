<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="URLRouting.aspx.cs" Inherits="WebApplication4.URLRouting" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>URL Routing - Article Catalog</title>
    <style type="text/css">
        * {
            box-sizing: border-box;
            margin: 0;
            padding: 0;
        }

        body {
            font-family: -apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, "Helvetica Neue", Arial, sans-serif;
            background: linear-gradient(135deg, #f0fdf4 0%, #e2e8f0 100%);
            min-height: 100vh;
            display: flex;
            align-items: center;
            justify-content: center;
            padding: 30px 15px;
            color: #1e293b;
        }

        .container {
            width: 100%;
            max-width: 860px;
        }

        /* Card Container */
        .card {
            background: #ffffff;
            border-radius: 16px;
            box-shadow: 0 10px 30px -5px rgba(0, 0, 0, 0.08), 0 4px 12px -2px rgba(0, 0, 0, 0.04);
            border: 1px solid #e2e8f0;
            overflow: hidden;
            transition: box-shadow 0.3s ease;
        }

        .card:hover {
            box-shadow: 0 20px 35px -5px rgba(0, 0, 0, 0.12), 0 8px 16px -2px rgba(0, 0, 0, 0.06);
        }

        /* Card Header */
        .card-header {
            background: linear-gradient(135deg, #10b981 0%, #059669 100%);
            padding: 28px 32px;
            color: #ffffff;
            text-align: center;
        }

        .header-badge {
            display: inline-flex;
            align-items: center;
            gap: 6px;
            background: rgba(255, 255, 255, 0.2);
            backdrop-filter: blur(8px);
            padding: 4px 14px;
            border-radius: 20px;
            font-size: 12px;
            font-weight: 600;
            letter-spacing: 0.05em;
            text-transform: uppercase;
            margin-bottom: 12px;
            border: 1px solid rgba(255, 255, 255, 0.3);
        }

        .header-title {
            font-size: 24px;
            font-weight: 700;
            letter-spacing: -0.02em;
            margin-bottom: 6px;
        }

        .header-subtitle {
            font-size: 14px;
            color: #d1fae5;
            line-height: 1.4;
        }

        /* Card Body */
        .card-body {
            padding: 32px;
        }

        /* Grid View Styling */
        .grid-container {
            width: 100%;
            overflow-x: auto;
            border-radius: 10px;
            border: 1px solid #e2e8f0;
        }

        .custom-grid {
            width: 100%;
            border-collapse: collapse;
            font-size: 14px;
            text-align: left;
        }

        .custom-grid th {
            background: #f8fafc;
            color: #334155;
            font-weight: 600;
            padding: 14px 18px;
            border-bottom: 2px solid #e2e8f0;
            text-transform: uppercase;
            font-size: 12px;
            letter-spacing: 0.03em;
        }

        .custom-grid td {
            padding: 14px 18px;
            border-bottom: 1px solid #f1f5f9;
            color: #475569;
            vertical-align: middle;
        }

        .custom-grid tr:last-child td {
            border-bottom: none;
        }

        .custom-grid tr:hover td {
            background-color: #f8fafc;
        }

        /* Article Link */
        .ArticleTitle {
            color: #059669;
            text-decoration: none;
            font-weight: 600;
            font-size: 15px;
            display: inline-flex;
            align-items: center;
            gap: 6px;
            transition: all 0.2s ease;
        }

        .ArticleTitle:hover {
            color: #047857;
            text-decoration: underline;
        }

        .desc-text {
            color: #64748b;
            line-height: 1.5;
            font-size: 13.5px;
        }

        .author-badge {
            display: inline-block;
            background: #ecfdf5;
            color: #065f46;
            padding: 4px 10px;
            border-radius: 12px;
            font-size: 12px;
            font-weight: 600;
            border: 1px solid #a7f3d0;
        }

        /* Card Footer */
        .card-footer {
            background: #f8fafc;
            border-top: 1px solid #e2e8f0;
            padding: 16px 32px;
            display: flex;
            justify-content: space-between;
            align-items: center;
            font-size: 13px;
        }

        .nav-link {
            color: #059669;
            text-decoration: none;
            font-weight: 600;
            display: inline-flex;
            align-items: center;
            gap: 4px;
            transition: color 0.2s ease;
        }

        .nav-link:hover {
            color: #047857;
            text-decoration: underline;
        }

        @media (max-width: 600px) {
            .card-body {
                padding: 20px 16px;
            }
            .card-header {
                padding: 24px 20px;
            }
            .card-footer {
                padding: 14px 20px;
                flex-direction: column;
                gap: 10px;
            }
        }
    </style>
</head>
<body>
    <form id="form1" runat="server" class="container">
        <div class="card">
            <!-- Header -->
            <div class="card-header">
                <div class="header-badge">&#128279; ASP.NET URL Routing</div>
                <h1 class="header-title">Article Catalog</h1>
                <p class="header-subtitle">Friendly, search-engine friendly URLs generated dynamically with RouteUrl.</p>
            </div>

            <!-- Body -->
            <div class="card-body">
                <div class="grid-container">
                    <asp:GridView ID="GridView1" runat="server"
                        AutoGenerateColumns="False"
                        CssClass="custom-grid"
                        GridLines="None">
                        <Columns>
                            <asp:TemplateField HeaderText="Article Title">
                                <ItemTemplate>
                                    <asp:HyperLink ID="HyperLink1"
                                        runat="server"
                                        CssClass="ArticleTitle"
                                        NavigateUrl='<%# GetRouteUrl("RouteForArticle", new { id = Eval("Id"), Title = GetTitle(Eval("Title")) }) %>'
                                        Text='<%# Eval("Title") %>'>
                                    </asp:HyperLink>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Description">
                                <ItemTemplate>
                                    <asp:Label ID="lblDesc"
                                        runat="server"
                                        CssClass="desc-text"
                                        Text='<%# DataBinder.Eval(Container.DataItem, "Description") %>'>
                                    </asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Author">
                                <ItemTemplate>
                                    <span class="author-badge">
                                        &#9997; <asp:Label ID="lblauthor" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "Author") %>'></asp:Label>
                                    </span>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>
            </div>

            <!-- Footer -->
            <div class="card-footer">
                <a href="SignUp.aspx" class="nav-link">&larr; Back to Dashboard</a>
                <span style="color: #94a3b8; font-size: 12px;">Route: /Articles/{id}/{Title}</span>
            </div>
        </div>
    </form>
</body>
</html>
