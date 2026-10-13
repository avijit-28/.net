<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ProductForm.aspx.cs" Inherits="WebApplication4.ProductForm" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Product Management - Add Product</title>
    <style type="text/css">
        * {
            box-sizing: border-box;
            margin: 0;
            padding: 0;
        }

        body {
            font-family: -apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, "Helvetica Neue", Arial, sans-serif;
            background: linear-gradient(135deg, #f0f4f8 0%, #e2e8f0 100%);
            min-height: 100vh;
            display: flex;
            align-items: center;
            justify-content: center;
            padding: 30px 15px;
            color: #1e293b;
        }

        .container {
            width: 100%;
            max-width: 620px;
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
            background: linear-gradient(135deg, #2563eb 0%, #1d4ed8 100%);
            padding: 28px 32px;
            color: #ffffff;
            position: relative;
        }

        .header-badge {
            display: inline-flex;
            align-items: center;
            gap: 6px;
            background: rgba(255, 255, 255, 0.18);
            backdrop-filter: blur(8px);
            padding: 4px 12px;
            border-radius: 20px;
            font-size: 12px;
            font-weight: 500;
            letter-spacing: 0.03em;
            text-transform: uppercase;
            margin-bottom: 12px;
            border: 1px solid rgba(255, 255, 255, 0.25);
        }

        .header-title {
            font-size: 22px;
            font-weight: 700;
            letter-spacing: -0.02em;
            margin-bottom: 6px;
        }

        .header-subtitle {
            font-size: 14px;
            color: #bfdbfe;
            line-height: 1.4;
        }

        /* Card Body */
        .card-body {
            padding: 32px;
        }

        /* 2-Column Responsive Grid */
        .form-grid {
            display: grid;
            grid-template-columns: 1fr 1fr;
            gap: 20px;
            margin-bottom: 24px;
        }

        @media (max-width: 540px) {
            .form-grid {
                grid-template-columns: 1fr;
                gap: 16px;
            }
            .card-body {
                padding: 24px 20px;
            }
            .card-header {
                padding: 24px 20px;
            }
        }

        /* Form Group & Controls */
        .form-group {
            display: flex;
            flex-direction: column;
            gap: 6px;
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
            left: 12px;
            color: #94a3b8;
            font-size: 14px;
            pointer-events: none;
        }

        .form-input {
            width: 100%;
            height: 44px;
            padding: 10px 14px 10px 36px;
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
            border-color: #2563eb;
            box-shadow: 0 0 0 3px rgba(37, 99, 235, 0.15);
        }

        .form-input::placeholder {
            color: #94a3b8;
            font-size: 13px;
        }

        .val-error {
            display: block;
            font-size: 12px;
            color: #ef4444;
            font-weight: 500;
            margin-top: 4px;
        }

        /* Action Button */
        .btn-submit {
            display: inline-flex;
            align-items: center;
            justify-content: center;
            gap: 8px;
            width: 100%;
            height: 48px;
            background: linear-gradient(135deg, #2563eb 0%, #1d4ed8 100%);
            color: #ffffff;
            border: none;
            border-radius: 8px;
            font-size: 15px;
            font-weight: 600;
            cursor: pointer;
            box-shadow: 0 4px 14px 0 rgba(37, 99, 235, 0.35);
            transition: all 0.2s ease;
        }

        .btn-submit:hover {
            background: linear-gradient(135deg, #1d4ed8 0%, #1e40af 100%);
            box-shadow: 0 6px 20px 0 rgba(37, 99, 235, 0.45);
            transform: translateY(-1px);
        }

        .btn-submit:active {
            transform: translateY(0);
            box-shadow: 0 2px 8px 0 rgba(37, 99, 235, 0.3);
        }

        /* Alert / Message Box */
        .alert-box {
            display: block;
            margin-top: 24px;
            padding: 16px 18px;
            border-radius: 10px;
            font-size: 14px;
            line-height: 1.6;
            white-space: pre-line;
            border: 1px solid transparent;
            animation: fadeIn 0.3s ease;
        }

        .alert-success {
            background-color: #ecfdf5;
            border-color: #a7f3d0;
            color: #065f46;
        }

        .alert-warning {
            background-color: #fffbeb;
            border-color: #fde68a;
            color: #92400e;
        }

        .alert-danger {
            background-color: #fef2f2;
            border-color: #fecaca;
            color: #991b1b;
        }

        @keyframes fadeIn {
            from {
                opacity: 0;
                transform: translateY(6px);
            }
            to {
                opacity: 1;
                transform: translateY(0);
            }
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
            color: #64748b;
        }

        .nav-link {
            color: #2563eb;
            text-decoration: none;
            font-weight: 500;
            display: inline-flex;
            align-items: center;
            gap: 4px;
            transition: color 0.2s ease;
        }

        .nav-link:hover {
            color: #1d4ed8;
            text-decoration: underline;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server" class="container">
        <div class="card">
            <!-- Header -->
            <div class="card-header">
                <div class="header-badge">Inventory System</div>
                <h1 class="header-title">Add New Product</h1>
                <p class="header-subtitle">Register a new product item into the database catalog.</p>
            </div>

            <!-- Body -->
            <div class="card-body">
                <div class="form-grid">
                    <!-- Product ID -->
                    <div class="form-group">
                        <label class="form-label" for="TxtPdrId">
                            Product ID <span class="req-star">*</span>
                        </label>
                        <div class="input-wrapper">
                            <span class="input-icon">#</span>
                            <asp:TextBox ID="TxtPdrId" runat="server" CssClass="form-input" placeholder="e.g. PRD-101" autocomplete="off"></asp:TextBox>
                        </div>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidatorPdrId" runat="server" 
                            ErrorMessage="Product Id is required" 
                            ForeColor="Red" 
                            ControlToValidate="TxtPdrId" 
                            Display="Dynamic" 
                            CssClass="val-error">
                        </asp:RequiredFieldValidator>
                    </div>

                    <!-- Product Name -->
                    <div class="form-group">
                        <label class="form-label" for="TxtPrdName">
                            Product Name <span class="req-star">*</span>
                        </label>
                        <div class="input-wrapper">
                            <span class="input-icon">&#128230;</span>
                            <asp:TextBox ID="TxtPrdName" runat="server" CssClass="form-input" placeholder="e.g. Wireless Mouse" autocomplete="off"></asp:TextBox>
                        </div>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidatorPrdName" runat="server" 
                            ErrorMessage="Product Name is required" 
                            ForeColor="Red" 
                            ControlToValidate="TxtPrdName" 
                            Display="Dynamic" 
                            CssClass="val-error">
                        </asp:RequiredFieldValidator>
                    </div>

                    <!-- Product Price -->
                    <div class="form-group">
                        <label class="form-label" for="TxtPrdPrc">
                            Product Price <span class="req-star">*</span>
                        </label>
                        <div class="input-wrapper">
                            <span class="input-icon">&#8377;</span>
                            <asp:TextBox ID="TxtPrdPrc" runat="server" CssClass="form-input" placeholder="e.g. 499.00" autocomplete="off"></asp:TextBox>
                        </div>
                        <asp:RangeValidator ID="RangeValidatorPrdPrc" runat="server" 
                            ErrorMessage="Product Price in between 10 and 1000 required" 
                            ForeColor="Red" 
                            ControlToValidate="TxtPrdPrc" 
                            Display="Dynamic" 
                            MinimumValue="1" 
                            MaximumValue="1000"
                            Type ="Integer"
                            CssClass="val-error">

                        </asp:RangeValidator>
                    </div>

                    <!-- Stock -->
                    <div class="form-group">
                        <label class="form-label" for="TxtPrdStk">
                            Available Stock <span class="req-star">*</span>
                        </label>
                        <div class="input-wrapper">
                            <span class="input-icon">&#128202;</span>
                            <asp:TextBox ID="TxtPrdStk" runat="server" CssClass="form-input" placeholder="e.g. 50" autocomplete="off"></asp:TextBox>
                        </div>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidatorPrdStk" runat="server" 
                            ErrorMessage="Product Stock is required" 
                            ForeColor="Red" 
                            ControlToValidate="TxtPrdStk" 
                            Display="Dynamic" 
                            CssClass="val-error">
                        </asp:RequiredFieldValidator>
                    </div>
                </div>

                <!-- Submit Button -->
                <asp:Button ID="BtnAddPrd" runat="server" OnClick="BtnAddPrd_Click" Text="+ Add Product to Catalog" CssClass="btn-submit" />

                <!-- Message / Feedback Alert -->
                <asp:Label ID="LblMsg" runat="server" CssClass="alert-box" Visible="false"></asp:Label>
            </div>

            <!-- Footer -->
            <div class="card-footer">
                <span>&#128717; Product Catalog</span>
                <a href="SignUp.aspx" class="nav-link">&larr; Back to Dashboard</a>
            </div>
        </div>
    </form>
</body>
</html>
