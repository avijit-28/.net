<%@ Page Title="Article Details" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="DetailPage.aspx.cs" Inherits="WebApplication4.DetailPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .detail-container {
            width: 100%;
            max-width: 680px;
            margin: 0 auto;
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

        /* Article Meta */
        .article-meta {
            display: flex;
            align-items: center;
            gap: 12px;
            margin-bottom: 20px;
            padding-bottom: 16px;
            border-bottom: 1px solid #f1f5f9;
        }

        .author-chip {
            display: inline-flex;
            align-items: center;
            gap: 6px;
            background: #ecfdf5;
            color: #065f46;
            padding: 6px 14px;
            border-radius: 20px;
            font-size: 13px;
            font-weight: 600;
            border: 1px solid #a7f3d0;
        }

        /* Article Content Box */
        .article-content {
            background: #f8fafc;
            border: 1px solid #e2e8f0;
            border-radius: 12px;
            padding: 24px;
            margin-bottom: 24px;
            line-height: 1.7;
            font-size: 15px;
            color: #334155;
            white-space: pre-line;
        }

        .content-title {
            font-size: 20px;
            font-weight: 700;
            color: #0f172a;
            margin-bottom: 14px;
            display: block;
        }

        /* Route Inspector */
        .route-info-box {
            background: #f8fafc;
            border: 1px dashed #cbd5e1;
            border-radius: 10px;
            padding: 14px 18px;
            margin-bottom: 24px;
            font-size: 12px;
            color: #64748b;
        }

        .route-info-title {
            font-weight: 600;
            color: #475569;
            margin-bottom: 8px;
            display: flex;
            align-items: center;
            gap: 6px;
        }

        .route-row {
            display: flex;
            justify-content: space-between;
            padding: 3px 0;
            border-bottom: 1px solid #f1f5f9;
        }

        .route-row:last-child {
            border-bottom: none;
        }

        .route-key {
            color: #64748b;
        }

        .route-val {
            font-weight: 600;
            color: #0f172a;
            font-family: ui-monospace, SFMono-Regular, Menlo, Monaco, Consolas, monospace;
        }

        /* Action Buttons */
        .actions-group {
            display: flex;
            gap: 12px;
            flex-wrap: wrap;
        }

        .btn-back {
            display: inline-flex;
            align-items: center;
            justify-content: center;
            gap: 8px;
            height: 44px;
            padding: 0 20px;
            background: linear-gradient(135deg, #10b981 0%, #059669 100%);
            color: #ffffff;
            text-decoration: none;
            border-radius: 8px;
            font-size: 14px;
            font-weight: 600;
            box-shadow: 0 4px 12px rgba(16, 185, 129, 0.3);
            transition: all 0.2s ease;
        }

        .btn-back:hover {
            background: linear-gradient(135deg, #059669 0%, #047857 100%);
            box-shadow: 0 6px 16px rgba(16, 185, 129, 0.4);
            transform: translateY(-1px);
        }

        .btn-outline {
            display: inline-flex;
            align-items: center;
            justify-content: center;
            gap: 6px;
            height: 44px;
            padding: 0 16px;
            background: #f1f5f9;
            color: #334155;
            text-decoration: none;
            border-radius: 8px;
            font-size: 14px;
            font-weight: 600;
            transition: all 0.2s ease;
        }

        .btn-outline:hover {
            background: #e2e8f0;
            color: #0f172a;
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

        @media (max-width: 500px) {
            .card-body {
                padding: 24px 20px;
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
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="detail-container">
        <div class="card">
            <!-- Header -->
            <div class="card-header">
                <div class="header-badge">&#128196; Article Details</div>
                <h1 class="header-title">Article Information</h1>
                <p class="header-subtitle">Loaded dynamically from SQL database using ASP.NET Page RouteData.</p>
            </div>

            <!-- Body -->
            <div class="card-body">
                <!-- Meta Info -->
                <div class="article-meta">
                    <span class="author-chip">
                        &#9997; Author: <asp:Label ID="lblauthor" runat="server" Text="Author"></asp:Label>
                    </span>
                </div>

                <!-- Article Card -->
                <div class="article-content">
                    <asp:Label ID="lblTitle" runat="server" CssClass="content-title" Text="Article Title"></asp:Label>
                    <asp:Label ID="lblDescription" runat="server" Text="Article Description"></asp:Label>
                </div>

                <!-- Route Inspector Info -->
                <div class="route-info-box">
                    <div class="route-info-title">&#128279; Current Route Data Values</div>
                    <div class="route-row">
                        <span class="route-key">Route Data ["id"]:</span>
                        <span class="route-val"><%= Page.RouteData.Values["id"] ?? "N/A" %></span>
                    </div>
                    <div class="route-row">
                        <span class="route-key">Route Data ["Title"]:</span>
                        <span class="route-val"><%= Page.RouteData.Values["Title"] ?? "N/A" %></span>
                    </div>
                    <div class="route-row">
                        <span class="route-key">Raw Request URL:</span>
                        <span class="route-val"><%= Request.RawUrl %></span>
                    </div>
                </div>

                <!-- Actions -->
                <div class="actions-group">
                    <a href="<%= ResolveUrl("~/URLRouting.aspx") %>" class="btn-back">&larr; Back to Article Catalog</a>
                    <a href="<%= ResolveUrl("~/SignUp.aspx") %>" class="btn-outline">&#127968; Dashboard</a>
                </div>
            </div>

            <!-- Footer -->
            <div class="card-footer">
                <a href="<%= ResolveUrl("~/URLRouting.aspx") %>" class="nav-link">&larr; Articles</a>
                <span style="color: #94a3b8; font-size: 12px;">ASP.NET WebForms URL Routing</span>
            </div>
        </div>
    </div>
</asp:Content>
