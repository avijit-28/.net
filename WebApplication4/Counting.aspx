<%@ Page Title="Click Counter" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Counting.aspx.cs" Inherits="MiniProject.Counting" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .counting-container {
            width: 100%;
            max-width: 480px;
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
            background: linear-gradient(135deg, #059669 0%, #047857 100%);
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
            color: #a7f3d0;
            line-height: 1.4;
        }

        /* Card Body */
        .card-body {
            padding: 32px;
            text-align: center;
        }

        /* Counter Readout Box */
        .counter-section {
            background: #f8fafc;
            border: 2px solid #e2e8f0;
            border-radius: 14px;
            padding: 24px 20px;
            margin-bottom: 24px;
            transition: all 0.3s ease;
        }

        .counter-label {
            font-size: 12px;
            font-weight: 700;
            text-transform: uppercase;
            letter-spacing: 0.08em;
            color: #64748b;
            margin-bottom: 8px;
            display: block;
        }

        .counter-display {
            width: 100%;
            border: none;
            background: transparent;
            font-size: 56px;
            font-weight: 800;
            color: #047857;
            text-align: center;
            outline: none;
            font-family: inherit;
            cursor: default;
        }

        /* Buttons Area */
        .actions-group {
            display: flex;
            flex-direction: column;
            gap: 12px;
            margin-bottom: 24px;
        }

        .btn-count {
            display: inline-flex;
            align-items: center;
            justify-content: center;
            gap: 8px;
            width: 100%;
            height: 50px;
            background: linear-gradient(135deg, #059669 0%, #047857 100%);
            color: #ffffff;
            border: none;
            border-radius: 10px;
            font-size: 16px;
            font-weight: 700;
            cursor: pointer;
            box-shadow: 0 4px 14px rgba(5, 150, 105, 0.35);
            transition: all 0.2s ease;
        }

        .btn-count:hover {
            background: linear-gradient(135deg, #047857 0%, #065f46 100%);
            box-shadow: 0 6px 20px rgba(5, 150, 105, 0.45);
            transform: translateY(-2px);
        }

        .btn-count:active {
            transform: translateY(0);
        }

        .btn-reset {
            display: inline-flex;
            align-items: center;
            justify-content: center;
            gap: 6px;
            width: 100%;
            height: 40px;
            background: #f1f5f9;
            color: #475569;
            border: 1px solid #cbd5e1;
            border-radius: 8px;
            font-size: 14px;
            font-weight: 600;
            cursor: pointer;
            transition: all 0.2s ease;
        }

        .btn-reset:hover {
            background: #e2e8f0;
            color: #1e293b;
        }

        /* Explanatory Info Card */
        .info-card {
            background: #f0fdf4;
            border: 1px solid #bbf7d0;
            border-radius: 10px;
            padding: 14px 16px;
            font-size: 13px;
            color: #166534;
            line-height: 1.5;
            text-align: left;
        }

        /* Card Footer */
        .card-footer {
            background: #f8fafc;
            border-top: 1px solid #e2e8f0;
            padding: 16px 24px;
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
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="counting-container">
        <div class="card">
            <!-- Header -->
            <div class="card-header">
                <div class="header-badge">State Management</div>
                <h1 class="header-title">&#128290; Click Counter</h1>
                <p class="header-subtitle">Tracks and persists clicks across postbacks using ASP.NET ViewState.</p>
            </div>

            <!-- Body -->
            <div class="card-body">
                <!-- Counter Readout Box -->
                <div class="counter-section">
                    <span class="counter-label">Total Clicks Recorded</span>
                    <asp:TextBox ID="TextBox1" runat="server" ReadOnly="true" CssClass="counter-display" Text="0"></asp:TextBox>
                </div>

                <!-- Action Buttons -->
                <div class="actions-group">
                    <asp:Button ID="Button1" runat="server" OnClick="Button1_Click" Text="&#9889; Click to Count (+1)" CssClass="btn-count" />
                    <asp:Button ID="BtnReset" runat="server" OnClick="BtnReset_Click" Text="&#8634; Reset Counter" CssClass="btn-reset" />
                </div>

                <!-- Explanatory Info -->
                <div class="info-card">
                    &#128161; <strong>How it works:</strong> Each button click sends a postback to the server. The count is stored and preserved in <code>ViewState["Clicks"]</code> across HTTP roundtrips.
                </div>
            </div>

            <!-- Footer Navigation -->
            <div class="card-footer">
                <a href="SignUp.aspx" class="nav-link">&larr; Back to Dashboard</a>
                <a href="ProductForm.aspx" class="nav-link">&#128230; Product Form &rarr;</a>
            </div>
        </div>
    </div>
</asp:Content>
