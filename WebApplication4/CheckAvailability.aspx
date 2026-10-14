<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CheckAvailability.aspx.cs" Inherits="WebApplication4.CheckAvailability" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Username Availability Checker</title>
    <style type="text/css">
        * {
            box-sizing: border-box;
            margin: 0;
            padding: 0;
        }

        body {
            font-family: -apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, "Helvetica Neue", Arial, sans-serif;
            background: linear-gradient(135deg, #f0fdfa 0%, #e2e8f0 100%);
            min-height: 100vh;
            display: flex;
            align-items: center;
            justify-content: center;
            padding: 30px 15px;
            color: #1e293b;
        }

        .container {
            width: 100%;
            max-width: 500px;
        }

        /* Card Container */
        .card {
            background: #ffffff;
            border-radius: 16px;
            box-shadow: 0 10px 30px -5px rgba(0, 0, 0, 0.08), 0 4px 12px -2px rgba(0, 0, 0, 0.04);
            border: 1px solid #ccfbf1;
            overflow: hidden;
            transition: box-shadow 0.3s ease;
        }

        .card:hover {
            box-shadow: 0 20px 35px -5px rgba(13, 148, 136, 0.12), 0 8px 16px -2px rgba(0, 0, 0, 0.06);
        }

        /* Card Header */
        .card-header {
            background: linear-gradient(135deg, #0d9488 0%, #0f766e 100%);
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
            color: #ccfbf1;
            line-height: 1.4;
        }

        /* Card Body */
        .card-body {
            padding: 32px;
        }

        /* Form Group */
        .form-group {
            display: flex;
            flex-direction: column;
            gap: 8px;
            margin-bottom: 20px;
        }

        .form-label {
            font-size: 13px;
            font-weight: 600;
            color: #334155;
            display: flex;
            align-items: center;
            gap: 6px;
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
            font-size: 16px;
            pointer-events: none;
        }

        .form-input {
            width: 100%;
            height: 48px;
            padding: 10px 14px 10px 42px;
            font-size: 15px;
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
            border-color: #0d9488;
            box-shadow: 0 0 0 3px rgba(13, 148, 136, 0.18);
        }

        .form-input::placeholder {
            color: #94a3b8;
            font-size: 14px;
        }

        /* Check Button */
        .btn-check {
            display: inline-flex;
            align-items: center;
            justify-content: center;
            gap: 8px;
            width: 100%;
            height: 48px;
            background: linear-gradient(135deg, #0d9488 0%, #0f766e 100%);
            color: #ffffff;
            border: none;
            border-radius: 8px;
            font-size: 15px;
            font-weight: 600;
            cursor: pointer;
            box-shadow: 0 4px 14px 0 rgba(13, 148, 136, 0.35);
            transition: all 0.2s ease;
            margin-bottom: 20px;
        }

        .btn-check:hover {
            background: linear-gradient(135deg, #0f766e 0%, #115e59 100%);
            box-shadow: 0 6px 20px 0 rgba(13, 148, 136, 0.45);
            transform: translateY(-1px);
        }

        .btn-check:active {
            transform: translateY(0);
            box-shadow: 0 2px 8px 0 rgba(13, 148, 136, 0.3);
        }

        /* Loading Progress Overlay */
        .progress-box {
            display: flex;
            align-items: center;
            justify-content: center;
            gap: 12px;
            padding: 14px;
            margin-bottom: 20px;
            background: #f0fdfa;
            border: 1px solid #99f6e4;
            border-radius: 8px;
            color: #0f766e;
            font-size: 14px;
            font-weight: 500;
            animation: fadeIn 0.2s ease-in-out;
        }

        .spinner {
            width: 20px;
            height: 20px;
            border: 2.5px solid #ccfbf1;
            border-top: 2.5px solid #0d9488;
            border-radius: 50%;
            animation: spin 0.8s linear infinite;
        }

        @keyframes spin {
            0% { transform: rotate(0deg); }
            100% { transform: rotate(360deg); }
        }

        @keyframes fadeIn {
            from { opacity: 0; transform: translateY(-4px); }
            to { opacity: 1; transform: translateY(0); }
        }

        /* Status Result Box */
        .status-box {
            display: flex;
            align-items: center;
            gap: 12px;
            padding: 14px 16px;
            border-radius: 8px;
            font-size: 14px;
            line-height: 1.5;
            margin-bottom: 20px;
            animation: fadeIn 0.3s ease;
        }

        .status-success {
            background-color: #ecfdf5;
            border: 1px solid #a7f3d0;
            color: #065f46;
        }

        .status-danger {
            background-color: #fef2f2;
            border: 1px solid #fecaca;
            color: #991b1b;
        }

        .status-warning {
            background-color: #fffbeb;
            border: 1px solid #fde68a;
            color: #92400e;
        }

        .status-icon {
            flex-shrink: 0;
            vertical-align: middle;
        }

        .status-text {
            font-weight: 600;
        }

        /* Feature Info Box */
        .info-card {
            background: #f8fafc;
            border: 1px dashed #cbd5e1;
            border-radius: 8px;
            padding: 14px 16px;
            font-size: 12.5px;
            color: #64748b;
            line-height: 1.5;
        }

        .info-card-title {
            font-weight: 600;
            color: #475569;
            margin-bottom: 4px;
            display: flex;
            align-items: center;
            gap: 6px;
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
            color: #0d9488;
            text-decoration: none;
            font-weight: 600;
            display: inline-flex;
            align-items: center;
            gap: 4px;
            transition: color 0.2s ease;
        }

        .nav-link:hover {
            color: #0f766e;
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
</head>
<body>
    <form id="form1" runat="server" class="container">
        <asp:ScriptManager ID="scriptmanager1" runat="server"></asp:ScriptManager>

        <div class="card">
            <!-- Header -->
            <div class="card-header">
                <div class="header-badge">&#128737; AJAX Real-Time Verification</div>
                <h1 class="header-title">Check Availability</h1>
                <p class="header-subtitle">Instant username verification in the database using ASP.NET AJAX UpdatePanel.</p>
            </div>

            <!-- Body -->
            <div class="card-body">
                <!-- UpdateProgress Indicator -->
                <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="updatepnl" DisplayAfter="0">
                    <ProgressTemplate>
                        <div class="progress-box">
                            <div class="spinner"></div>
                            <asp:Image ID="imgLoading" runat="server" ImageUrl="~/images/loading.gif" Width="20px" Height="20px" Visible="false" />
                            <span>Checking database availability...</span>
                        </div>
                    </ProgressTemplate>
                </asp:UpdateProgress>

                <!-- UpdatePanel containing form and feedback -->
                <asp:UpdatePanel ID="updatepnl" runat="server">
                    <ContentTemplate>
                        <div class="form-group">
                            <label class="form-label" for="txtun">
                                &#128100; Desired Username <span class="req-star">*</span>
                            </label>
                            <div class="input-wrapper">
                                <span class="input-icon">&#64;</span>
                                <asp:TextBox ID="txtun" runat="server" CssClass="form-input" placeholder="e.g. avijit_pakhira" autocomplete="off"></asp:TextBox>
                            </div>
                        </div>

                        <!-- Action Button to perform check -->
                        <asp:Button ID="txtbtn" runat="server" Text="&#128269; Check Availability" OnClick="txtbtn_Click" CssClass="btn-check" />

                        <!-- Status Result Display -->
                        <div id="checkun" runat="server" visible="false" class="status-box status-success">
                            <asp:Image ID="shwimg" runat="server" Width="22px" Height="22px" CssClass="status-icon" />
                            <asp:Label ID="lblmsg" runat="server" CssClass="status-text"></asp:Label>
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>

                <!-- Information Note -->
                <div class="info-card">
                    <div class="info-card-title">&#128161; How it works</div>
                    Clicking <strong>Check Availability</strong> triggers an asynchronous postback via <code>UpdatePanel</code>. The server queries <code>UserDetails</code> and returns immediate feedback without refreshing the entire page.
                </div>
            </div>

            <!-- Footer Navigation -->
            <div class="card-footer">
                <a href="SignUp.aspx" class="nav-link">&larr; Back to Dashboard</a>
                <a href="SearchUser.aspx" class="nav-link">Search User Directory &rarr;</a>
            </div>
        </div>
    </form>
</body>
</html>
