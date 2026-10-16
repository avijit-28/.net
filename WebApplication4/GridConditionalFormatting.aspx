<%@ Page Title="Conditional Row Formatting" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="GridConditionalFormatting.aspx.cs" Inherits="WebApplication4.GridConditionalFormatting" %>

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

        .legend-bar {
            display: flex;
            align-items: center;
            gap: 20px;
            margin-bottom: 18px;
            font-size: 13px;
            color: #475569;
        }

        .legend-badge {
            display: inline-flex;
            align-items: center;
            gap: 6px;
        }

        .legend-indicator-red {
            width: 14px;
            height: 14px;
            background: #dc2626;
            border-radius: 4px;
        }

        .legend-indicator-default {
            width: 14px;
            height: 14px;
            background: #fff7e7;
            border: 1px solid #deba84;
            border-radius: 4px;
        }

        .grid-responsive-wrap {
            overflow-x: auto;
            border-radius: 10px;
            border: 1px solid #deba84;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="module-page-container">
        
        <!-- Header Card -->
        <div class="module-header-card">
            <div class="module-header-info">
                <h1>Design-Time Styles &amp; Conditional Row Highlighting</h1>
                <p>Combines declarative design-time styles with runtime <code>RowDataBound</code> logic to highlight employees with high annual salaries.</p>
            </div>
            <a href="<%= ResolveUrl("~/AllGridProp") %>" class="btn-hub-return">&larr; Back to GridView Hub</a>
        </div>

        <!-- Content Card -->
        <div class="content-card">
            <div style="margin-bottom: 12px; display: flex; align-items: center; justify-content: space-between; flex-wrap: wrap; gap: 8px;">
                <span style="background: #eff6ff; color: #1d4ed8; border: 1px solid #bfdbfe; font-size: 12px; font-weight: 700; padding: 4px 10px; border-radius: 6px; display: inline-flex; align-items: center; gap: 6px;">
                    &#128451; Live SQL Table: <code>[Assignmet].[dbo].[AllEmployee]</code>
                </span>
            </div>

            <div class="legend-bar">
                <div class="legend-badge">
                    <span class="legend-indicator-default"></span>
                    <span>Standard Salary (&le; $70,000)</span>
                </div>
                <div class="legend-badge">
                    <span class="legend-indicator-red"></span>
                    <strong>High Salary (&gt; $70,000) &mdash; Highlighted Red at Runtime</strong>
                </div>
            </div>

            <div class="grid-responsive-wrap">
                <asp:GridView ID="GridView1" runat="server" 
                    AutoGenerateColumns="False" 
                    BackColor="#DEBA84" 
                    BorderColor="#DEBA84" 
                    BorderStyle="None" 
                    BorderWidth="1px" 
                    CellPadding="8" 
                    CellSpacing="2" 
                    Width="100%"
                    OnRowDataBound="GridView1_RowDataBound">
                    <Columns>
                        <asp:BoundField DataField="EmployeeId" HeaderText="Employee ID" ReadOnly="True" />
                        <asp:BoundField DataField="FirstName" HeaderText="First Name" />
                        <asp:BoundField DataField="AnnualSalary" HeaderText="Annual Salary" DataFormatString="{0:c}" />
                        <asp:BoundField DataField="Department" HeaderText="Department" />
                    </Columns>
                    <FooterStyle BackColor="#F7DFB5" ForeColor="#8C4510" />
                    <HeaderStyle BackColor="#A55129" Font-Bold="True" ForeColor="White" />
                    <PagerStyle ForeColor="#8C4510" HorizontalAlign="Center" />
                    <RowStyle BackColor="#FFF7E7" ForeColor="#8C4510" />
                    <AlternatingRowStyle BackColor="#FFFFFF" ForeColor="#8C4510" />
                </asp:GridView>
            </div>
        </div>
    </div>
</asp:Content>
