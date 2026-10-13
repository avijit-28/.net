<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FirstPage.aspx.cs" Inherits="WebApplication4.FirstPage" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Sign In</title>
    <style type="text/css">
        * {
            box-sizing: border-box;
            margin: 0;
            padding: 0;
        }

        body {
            font-family: -apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, "Helvetica Neue", Arial, sans-serif;
            background: #f1f5f9;
            min-height: 100vh;
            display: flex;
            align-items: center;
            justify-content: center;
            padding: 20px;
        }

        .card {
            background: #ffffff;
            border-radius: 12px;
            box-shadow: 0 4px 20px rgba(0, 0, 0, 0.08);
            padding: 32px;
            width: 100%;
            max-width: 400px;
        }


        /* Online Users Badge Design */
        .online-badge {
            display: inline-flex;
            align-items: center;
            gap: 10px;
            background: #ecfdf5;
            border: 1px solid #a7f3d0;
            padding: 8px 16px;
            border-radius: 20px;
            margin-bottom: 24px;
            font-size: 14px;
            color: #065f46;
            width: 100%;
            justify-content: center;
        }

        .status-dot {
            width: 10px;
            height: 10px;
            background-color: #10b981;
            border-radius: 50%;
            box-shadow: 0 0 0 0 rgba(16, 185, 129, 0.7);
            animation: pulse-ring 2s infinite cubic-bezier(0.4, 0, 0.6, 1);
        }

        @keyframes pulse-ring {
            0% {
                box-shadow: 0 0 0 0 rgba(16, 185, 129, 0.7);
            }
            70% {
                box-shadow: 0 0 0 8px rgba(16, 185, 129, 0);
            }
            100% {
                box-shadow: 0 0 0 0 rgba(16, 185, 129, 0);
            }
        }

        .online-count {
            font-weight: 700;
            font-size: 16px;
            color: #047857;
        }

        h2 {
            font-size: 22px;
            color: #1e293b;
            margin-bottom: 20px;
            text-align: center;
        }

        .form-group {
            margin-bottom: 16px;
        }

        .form-group label {
            display: block;
            margin-bottom: 6px;
            font-size: 14px;
            font-weight: 500;
            color: #475569;
        }

        .form-control {
            width: 100%;
            padding: 10px 14px;
            font-size: 14px;
            border: 1px solid #cbd5e1;
            border-radius: 6px;
            outline: none;
            transition: border-color 0.2s, box-shadow 0.2s;
        }

        .form-control:focus {
            border-color: #3b82f6;
            box-shadow: 0 0 0 3px rgba(59, 130, 246, 0.15);
        }

        .btn-submit {
            width: 100%;
            padding: 12px;
            background: #2563eb;
            color: #ffffff;
            border: none;
            border-radius: 6px;
            font-size: 15px;
            font-weight: 600;
            cursor: pointer;
            transition: background 0.2s;
            margin-top: 8px;
        }

        .btn-submit:hover {
            background: #1d4ed8;
        }
        .val-error {
            display: block;
            font-size: 12px;
            color: #ef4444;
            font-weight: 500;
            margin-top: 4px;
        }
    </style>
</head>
<body>

    <form id="form1" runat="server" class="card">
        <!-- Online Users Counter Badge -->
        <div class="online-badge">
            <span class="status-dot"></span>
            <span>The num of users online = </span>
            <asp:Label ID="lblOnlineUsers" runat="server" CssClass="online-count" Text="0"></asp:Label>
        </div>

        <h2>Sign In</h2>

        <div class="form-group">
            <asp:Label ID="lblUser" runat="server" Text="User Name" AssociatedControlID="txtUserName"></asp:Label>
            <asp:TextBox ID="txtUserName" runat="server" CssClass="form-control" placeholder="you@example.com"></asp:TextBox>
            <asp:RegularExpressionValidator ID="RegularExpressionValidatorEmail" runat="server" 
                    ControlToValidate="txtUserName" 
                    ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"
                    ErrorMessage="Invalid Email" 
                    ForeColor="Red"
                    Display = "Dynamic">
            </asp:RegularExpressionValidator>
        </div>

        <div class="form-group">
            <asp:Label ID="lblPassword" runat="server" Text="Password" AssociatedControlID="txtPassword"></asp:Label>
            <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control" TextMode="Password" placeholder="Enter password"></asp:TextBox>
            <asp:Label ID="lblRePassword" runat="server" Text="Comfirm Password" AssociatedControlID="txtRePassword"></asp:Label>
            <asp:TextBox ID="txtRePassword" runat="server" CssClass="form-control" TextMode="Password" placeholder="Enter password"></asp:TextBox>
            <asp:CompareValidator ID="CompareValidatorPassword" runat="server"
                ErrorMessage="Password and Retype Password must match"
                ControlToValidate="txtRePassword"
                ControlToCompare="txtPassword"
                Type="String"
                Operator="Equal"
                ForeColor="Red"
                Display="Dynamic">
            </asp:CompareValidator>
        </div>
        

        <asp:Button ID="BtnSign" runat="server" OnClick="BtnSign_Click" Text="Sign In" CssClass="btn-submit" />
        <asp:ValidationSummary ID="ValidationSummary1" runat="server" 
        ForeColor="Red" HeaderText="Page Errors" ShowMessageBox="True"
        ShowSummary="true" DisplayMode="List"  />
    </form>

</body>
</html>
