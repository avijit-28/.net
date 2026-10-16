<%@ Page Title="Hidden Column Currency Binding" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="GridHiddenCulture.aspx.cs" Inherits="WebApplication4.GridHiddenCulture" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .module-page-container {
            width: 100%;
            max-width: 1000px;
            margin: 0 auto;
            display: flex;
            flex-direction: column;
            gap: 20px;
        }

        .module-header-card {
            background: linear-gradient(135deg, #0f172a 0%, #1e293b 100%);
            border-radius: 16px;
            padding: 24px 30px;
            color: #ffffff;
            box-shadow: 0 10px 25px -5px rgba(15, 23, 42, 0.25);
            border: 1px solid rgba(255, 255, 255, 0.1);
            display: flex;
            justify-content: space-between;
            align-items: center;
            flex-wrap: wrap;
            gap: 16px;
        }

        .module-header-info h1 {
            font-size: 22px;
            font-weight: 800;
            color: #ffffff;
            margin-bottom: 6px;
            letter-spacing: -0.02em;
        }

        .module-header-info p {
            font-size: 13.5px;
            color: #94a3b8;
            margin: 0;
            line-height: 1.5;
        }

        .btn-hub-return {
            display: inline-flex;
            align-items: center;
            gap: 6px;
            padding: 9px 16px;
            background: rgba(255, 255, 255, 0.1);
            color: #ffffff !important;
            border: 1px solid rgba(255, 255, 255, 0.2);
            border-radius: 8px;
            font-size: 13px;
            font-weight: 600;
            text-decoration: none;
            transition: all 0.2s;
        }

        .btn-hub-return:hover {
            background: rgba(255, 255, 255, 0.2);
            transform: translateY(-1px);
        }

        .content-card {
            background: #ffffff;
            border-radius: 14px;
            border: 1px solid #e2e8f0;
            box-shadow: 0 4px 15px -3px rgba(0, 0, 0, 0.05);
            padding: 24px;
        }

        .info-box {
            background: #f8fafc;
            border-left: 4px solid #8b5cf6;
            padding: 12px 18px;
            border-radius: 0 8px 8px 0;
            margin-bottom: 20px;
            font-size: 13px;
            color: #475569;
        }

        .grid-responsive-wrap {
            overflow-x: auto;
            border-radius: 10px;
            border: 1px solid #e2e8f0;
        }

        .custom-gridview {
            width: 100%;
            border-collapse: collapse;
            font-family: inherit;
            font-size: 13.5px;
        }

        .custom-gridview th {
            background: #0f172a;
            color: #ffffff;
            padding: 12px 16px;
            text-align: left;
            font-weight: 700;
        }

        .custom-gridview td {
            padding: 12px 16px;
            border-bottom: 1px solid #f1f5f9;
            color: #1e293b;
        }

        .custom-gridview tr:nth-child(even) td {
            background: #f8fafc;
        }

        .custom-gridview tr:hover td {
            background: #f1f5f9;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="module-page-container">
        
        <!-- Header Card -->
        <div class="module-header-card">
            <div class="module-header-info">
                <h1>Hidden Column Currency Binding</h1>
                <p>Retrieves the culture information from a bound <code>CountryCulture</code> column to format annual salary, and programmatically hides the column from users.</p>
            </div>
            <a href="<%= ResolveUrl("~/AllGridProp") %>" class="btn-hub-return">&larr; Back to GridView Hub</a>
        </div>

        <!-- Content Card -->
        <div class="content-card">
            <div class="info-box">
                <strong>Mechanism:</strong> The <code>CountryCulture</code> column (index 4) contains culture codes (<code>en-US</code>, <code>en-IN</code>, etc.). In <code>RowDataBound</code>, <code>e.Row.Cells[4].Visible = false</code> is invoked on both the Header and Data rows after extracting the value.
            </div>

            <div class="grid-responsive-wrap">
                <asp:GridView ID="GridView1" runat="server" 
                    AutoGenerateColumns="False" 
                    OnRowDataBound="GridView1_RowDataBound"
                    CssClass="custom-gridview"
                    GridLines="None">
                    <Columns>
                        <asp:BoundField DataField="EmployeeId" HeaderText="Employee ID" ReadOnly="True" />
                        <asp:BoundField DataField="FirstName" HeaderText="First Name" />
                        <asp:BoundField DataField="AnnualSalary" HeaderText="Annual Salary" />
                        <asp:BoundField DataField="Country" HeaderText="Country" />
                        <asp:BoundField DataField="CountryCulture" HeaderText="CountryCulture" />
                    </Columns>
                </asp:GridView>
            </div>
        </div>
    </div>
</asp:Content>
