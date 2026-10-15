<%@ Page Title="Send & Store Cookies" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="CookiesSend.aspx.cs" Inherits="MiniProject.CookiesSend" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .cookies-container {
            width: 100%;
            max-width: 480px;
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

        /* Info Banner */
        .info-banner {
            background: #fffbeb;
            border: 1px solid #fde68a;
            border-radius: 10px;
            padding: 12px 14px;
            font-size: 13px;
            color: #92400e;
            display: flex;
            align-items: flex-start;
            gap: 10px;
            margin-bottom: 24px;
            line-height: 1.5;
        }

        .info-icon {
            font-size: 18px;
            line-height: 1;
            flex-shrink: 0;
        }

        .form-group {
            display: flex;
            flex-direction: column;
            gap: 6px;
            margin-bottom: 20px;
        }

        .form-label {
            font-size: 13px;
            font-weight: 600;
            color: #334155;
            display: flex;
            align-items: center;
            gap: 4px;
        }

        .req-star {
            color: #ef4444;
            font-weight: 700;
        }

        .input-wrapper {
            position: relative;
            display: flex;
            align-items: center;
        }

        .input-icon {
            position: absolute;
            left: 14px;
            color: #94a3b8;
            font-size: 15px;
            pointer-events: none;
        }

        .form-input {
            width: 100%;
            height: 46px;
            padding: 10px 14px 10px 42px;
            font-size: 14px;
            font-family: inherit;
            color: #0f172a;
            background: #f8fafc;
            border: 1.5px solid #cbd5e1;
            border-radius: 8px;
            outline: none;
            transition: all 0.2s ease;
        }

        .form-input:focus {
            background: #ffffff;
            border-color: #d97706;
            box-shadow: 0 0 0 3px rgba(217, 119, 6, 0.18);
        }

        .form-input::placeholder {
            color: #94a3b8;
            font-size: 13px;
        }

        /* Cookie Specs Box */
        .specs-box {
            background: #f8fafc;
            border: 1px dashed #cbd5e1;
            border-radius: 8px;
            padding: 12px 14px;
            margin-bottom: 24px;
            font-size: 12px;
            color: #64748b;
        }

        .specs-title {
            font-weight: 600;
            color: #475569;
            margin-bottom: 6px;
            display: flex;
            align-items: center;
            gap: 6px;
        }

        .specs-list {
            list-style: none;
            display: flex;
            flex-direction: column;
            gap: 4px;
            padding-left: 2px;
        }

        .specs-list li {
            display: flex;
            justify-content: space-between;
        }

        .specs-key {
            color: #64748b;
        }

        .specs-val {
            font-weight: 600;
            color: #334155;
            font-family: ui-monospace, SFMono-Regular, Menlo, Monaco, Consolas, monospace;
        }

        /* Action Button */
        .btn-submit {
            display: inline-flex;
            align-items: center;
            justify-content: center;
            gap: 8px;
            width: 100%;
            height: 48px;
            background: linear-gradient(135deg, #d97706 0%, #b45309 100%);
            color: #ffffff;
            border: none;
            border-radius: 8px;
            font-size: 15px;
            font-weight: 600;
            cursor: pointer;
            box-shadow: 0 4px 14px 0 rgba(217, 119, 6, 0.35);
            transition: all 0.2s ease;
        }

        .btn-submit:hover {
            background: linear-gradient(135deg, #b45309 0%, #92400e 100%);
            box-shadow: 0 6px 20px 0 rgba(217, 119, 6, 0.45);
            transform: translateY(-1px);
        }

        .btn-submit:active {
            transform: translateY(0);
            box-shadow: 0 2px 8px 0 rgba(217, 119, 6, 0.3);
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
                <div class="header-badge">&#127850; HTTP Cookie State</div>
                <h1 class="header-title">Send &amp; Store Cookies</h1>
                <p class="header-subtitle">Save user details into a client-side HTTP cookie valid for 2 minutes.</p>
            </div>

            <!-- Body -->
            <div class="card-body">
                <div class="info-banner">
                    <span class="info-icon">&#128161;</span>
                    <div>
                        Cookies are saved directly in your web browser. When submitted, this page writes a multi-value cookie named <strong>UserDetails</strong> and redirects you to read it.
                    </div>
                </div>

                <!-- Name Field -->
                <div class="form-group">
                    <label class="form-label" for="TxtName">
                        <asp:Label ID="LblName" runat="server" Text="User Name"></asp:Label>
                        <span class="req-star">*</span>
                    </label>
                    <div class="input-wrapper">
                        <span class="input-icon">&#128100;</span>
                        <asp:TextBox ID="TxtName" runat="server" CssClass="form-input" placeholder="e.g. John Doe" autocomplete="off" required="required"></asp:TextBox>
                    </div>
                </div>

                <!-- Email Field -->
                <div class="form-group">
                    <label class="form-label" for="TxtEmail">
                        <asp:Label ID="LblEmail" runat="server" Text="Email Address"></asp:Label>
                        <span class="req-star">*</span>
                    </label>
                    <div class="input-wrapper">
                        <span class="input-icon">&#9993;</span>
                        <asp:TextBox ID="TxtEmail" runat="server" TextMode="Email" CssClass="form-input" placeholder="e.g. john@example.com" autocomplete="off" required="required"></asp:TextBox>
                    </div>
                </div>

                <!-- Cookie Properties Specs -->
                <div class="specs-box">
                    <div class="specs-title">&#9881; Cookie Configuration</div>
                    <ul class="specs-list">
                        <li>
                            <span class="specs-key">Cookie Identifier:</span>
                            <span class="specs-val">UserDetails</span>
                        </li>
                        <li>
                            <span class="specs-key">Expiration Time:</span>
                            <span class="specs-val">2 Minutes (DateTime.Now + 2m)</span>
                        </li>
                        <li>
                            <span class="specs-key">Storage Location:</span>
                            <span class="specs-val">Client Browser (HTTP Header)</span>
                        </li>
                    </ul>
                </div>

                <!-- Submit Button -->
                <asp:Button ID="BtnCookies" runat="server" OnClick="BtnCookies_Click" Text="&#127850; Send &amp; Save Cookie" CssClass="btn-submit" />
            </div>

            <!-- Footer -->
            <div class="card-footer">
                <a href="<%= ResolveUrl("~/SignUp") %>" class="nav-link">&larr; Back to Dashboard</a>
                <a href="<%= ResolveUrl("~/CookiesGet") %>" class="nav-link">View Stored Cookie &rarr;</a>
            </div>
        </div>
    </div>
</asp:Content>
