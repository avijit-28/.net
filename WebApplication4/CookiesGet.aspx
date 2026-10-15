<%@ Page Title="Retrieved Cookie Data" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="CookiesGet.aspx.cs" Inherits="MiniProject.CookiesGet" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .cookies-container {
            width: 100%;
            max-width: 520px;
            margin: 0 auto;
        }

        /* Card Container */
        .card {
            background: #ffffff;
            border-radius: 16px;
            box-shadow: 0 10px 30px -5px rgba(0, 0, 0, 0.08), 0 4px 12px -2px rgba(0, 0, 0, 0.04);
            border: 1px solid #fde68a;
            overflow: hidden;
            transition: box-shadow 0.3s ease;
        }

        .card:hover {
            box-shadow: 0 20px 35px -5px rgba(217, 119, 6, 0.12), 0 8px 16px -2px rgba(0, 0, 0, 0.06);
        }

        /* Card Header */
        .card-header {
            background: linear-gradient(135deg, #d97706 0%, #b45309 100%);
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
            color: #fef3c7;
            line-height: 1.4;
        }

        /* Card Body */
        .card-body {
            padding: 32px;
        }

        /* Cookie Overview Pill */
        .cookie-status-box {
            background: #fffbeb;
            border: 1px solid #fde68a;
            border-radius: 12px;
            padding: 16px;
            display: flex;
            align-items: center;
            gap: 16px;
            margin-bottom: 24px;
        }

        .cookie-avatar {
            width: 52px;
            height: 52px;
            border-radius: 12px;
            background: linear-gradient(135deg, #f59e0b 0%, #d97706 100%);
            color: #ffffff;
            font-size: 26px;
            display: flex;
            align-items: center;
            justify-content: center;
            box-shadow: 0 4px 10px rgba(217, 119, 6, 0.3);
            flex-shrink: 0;
        }

        .cookie-summary {
            flex-grow: 1;
        }

        .cookie-summary-title {
            font-size: 15px;
            font-weight: 700;
            color: #78350f;
            margin-bottom: 2px;
        }

        .cookie-summary-sub {
            font-size: 12px;
            color: #92400e;
        }

        /* Values List */
        .data-list {
            display: flex;
            flex-direction: column;
            gap: 14px;
            margin-bottom: 24px;
        }

        .data-item {
            background: #f8fafc;
            border: 1px solid #e2e8f0;
            border-radius: 10px;
            padding: 14px 18px;
            display: flex;
            align-items: center;
            justify-content: space-between;
            transition: all 0.2s ease;
        }

        .data-item:hover {
            border-color: #cbd5e1;
            background: #ffffff;
            box-shadow: 0 4px 12px rgba(0, 0, 0, 0.04);
        }

        .data-label-group {
            display: flex;
            align-items: center;
            gap: 10px;
        }

        .data-icon {
            font-size: 18px;
            color: #d97706;
        }

        .data-label {
            font-size: 13px;
            font-weight: 600;
            color: #475569;
        }

        .data-val {
            font-size: 15px;
            font-weight: 700;
            color: #0f172a;
            font-family: inherit;
            word-break: break-all;
            text-align: right;
            padding-left: 12px;
        }

        /* Technical Info Panel */
        .tech-panel {
            background: #f8fafc;
            border: 1px dashed #cbd5e1;
            border-radius: 10px;
            padding: 14px 16px;
            margin-bottom: 24px;
            font-size: 12px;
            color: #64748b;
        }

        .tech-header {
            font-weight: 600;
            color: #475569;
            margin-bottom: 8px;
            display: flex;
            align-items: center;
            gap: 6px;
        }

        .tech-row {
            display: flex;
            justify-content: space-between;
            padding: 3px 0;
            border-bottom: 1px solid #f1f5f9;
        }

        .tech-row:last-child {
            border-bottom: none;
        }

        .tech-key {
            color: #64748b;
        }

        .tech-val {
            font-weight: 600;
            color: #334155;
            font-family: ui-monospace, SFMono-Regular, Menlo, Monaco, Consolas, monospace;
        }

        /* Action Buttons */
        .actions-row {
            display: flex;
            gap: 12px;
            flex-wrap: wrap;
        }

        .btn-primary {
            flex: 1;
            min-width: 140px;
            display: inline-flex;
            align-items: center;
            justify-content: center;
            gap: 8px;
            height: 46px;
            background: linear-gradient(135deg, #d97706 0%, #b45309 100%);
            color: #ffffff;
            text-decoration: none;
            border-radius: 8px;
            font-size: 14px;
            font-weight: 600;
            cursor: pointer;
            box-shadow: 0 4px 14px 0 rgba(217, 119, 6, 0.35);
            transition: all 0.2s ease;
        }

        .btn-primary:hover {
            background: linear-gradient(135deg, #b45309 0%, #92400e 100%);
            box-shadow: 0 6px 18px 0 rgba(217, 119, 6, 0.45);
            transform: translateY(-1px);
        }

        .btn-secondary {
            display: inline-flex;
            align-items: center;
            justify-content: center;
            gap: 6px;
            height: 46px;
            padding: 0 16px;
            background: #f1f5f9;
            color: #334155;
            text-decoration: none;
            border-radius: 8px;
            font-size: 14px;
            font-weight: 600;
            transition: all 0.2s ease;
        }

        .btn-secondary:hover {
            background: #e2e8f0;
            color: #0f172a;
        }

        /* Card Footer */
        .card-footer {
            background: #fffdf5;
            border-top: 1px solid #fef3c7;
            padding: 16px 32px;
            display: flex;
            justify-content: space-between;
            align-items: center;
            font-size: 13px;
        }

        .nav-link {
            color: #d97706;
            text-decoration: none;
            font-weight: 600;
            display: inline-flex;
            align-items: center;
            gap: 4px;
            transition: color 0.2s ease;
        }

        .nav-link:hover {
            color: #92400e;
            text-decoration: underline;
        }

        @media (max-width: 500px) {
            .card-body {
                padding: 24px 20px;
            }
            .card-header {
                padding: 24px 20px;
            }
            .actions-row {
                flex-direction: column;
            }
            .btn-primary, .btn-secondary {
                width: 100%;
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
    <div class="cookies-container">
        <div class="card">
            <!-- Header -->
            <div class="card-header">
                <div class="header-badge">&#127850; Browser Cookie Reader</div>
                <h1 class="header-title">Retrieved Cookie Data</h1>
                <p class="header-subtitle">Reading stored HTTP cookie values from the client browser request.</p>
            </div>

            <!-- Body -->
            <div class="card-body">
                <!-- Status Box -->
                <div class="cookie-status-box">
                    <div class="cookie-avatar">&#127850;</div>
                    <div class="cookie-summary">
                        <div class="cookie-summary-title">Cookie: UserDetails</div>
                        <div class="cookie-summary-sub">Multi-value HTTP state persistence</div>
                    </div>
                </div>

                <!-- Cookie Content List -->
                <div class="data-list">
                    <!-- User Name -->
                    <div class="data-item">
                        <div class="data-label-group">
                            <span class="data-icon">&#128100;</span>
                            <span class="data-label">User Name</span>
                        </div>
                        <asp:Label ID="LblName" runat="server" CssClass="data-val" Text="--"></asp:Label>
                    </div>

                    <!-- Email Address -->
                    <div class="data-item">
                        <div class="data-label-group">
                            <span class="data-icon">&#9993;</span>
                            <span class="data-label">Email Address</span>
                        </div>
                        <asp:Label ID="LblEmail" runat="server" CssClass="data-val" Text="--"></asp:Label>
                    </div>
                </div>

                <!-- Technical Details -->
                <div class="tech-panel">
                    <div class="tech-header">&#9881; Cookie Runtime Information</div>
                    <div class="tech-row">
                        <span class="tech-key">Request Source:</span>
                        <span class="tech-val">Request.Cookies["UserDetails"]</span>
                    </div>
                    <div class="tech-row">
                        <span class="tech-key">Subkeys Read:</span>
                        <span class="tech-val">cookie["Name"], cookie["Email"]</span>
                    </div>
                    <div class="tech-row">
                        <span class="tech-key">Lifespan Window:</span>
                        <span class="tech-val">2 Minutes from generation</span>
                    </div>
                </div>

                <!-- Actions -->
                <div class="actions-row">
                    <a href="<%= ResolveUrl("~/CookiesSend") %>" class="btn-primary">&#127850; Send New Cookie</a>
                    <a href="<%= ResolveUrl("~/CookiesGet") %>" class="btn-secondary">&#128260; Refresh Cookie</a>
                </div>
            </div>

            <!-- Footer -->
            <div class="card-footer">
                <a href="<%= ResolveUrl("~/SignUp") %>" class="nav-link">&larr; Back to Dashboard</a>
                <a href="<%= ResolveUrl("~/CookiesSend") %>" class="nav-link">Configure Cookies &rarr;</a>
            </div>
        </div>
    </div>
</asp:Content>
