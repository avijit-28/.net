<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AllGridProp.aspx.cs" Inherits="WebApplication4.AllGridProp" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>GridView Properties (Pages 10 - 30)</title>
    <style type="text/css">
        body {
            font-family: Arial, sans-serif;
            background-color: #f4f6f9;
            margin: 0;
            padding: 30px;
        }

        .container {
            max-width: 950px;
            margin: 0 auto;
            background: #ffffff;
            padding: 25px 30px;
            border-radius: 8px;
            box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
        }

        h2 {
            margin-top: 0;
            color: #333333;
            border-bottom: 2px solid #a55129;
            padding-bottom: 10px;
        }

        .summary-text {
            color: #666666;
            font-size: 14px;
            margin-bottom: 15px;
            line-height: 1.5;
        }

        .status-msg {
            display: block;
            margin: 10px 0;
            font-weight: bold;
            color: #15803d;
        }

        /* Grid inputs & buttons styling */
        .grid-view input[type="text"], .grid-view select {
            padding: 4px 6px;
            font-size: 13px;
            border: 1px solid #cccccc;
            border-radius: 3px;
            box-sizing: border-box;
        }

        .grid-view a {
            color: #8c4510;
            font-weight: bold;
            text-decoration: underline;
            margin-right: 5px;
        }

        .grid-view a:hover {
            color: #a55129;
        }

        .val-summary {
            margin: 10px 0;
            font-size: 13px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <h2>GridView Features &amp; Properties (Pages 10 - 30)</h2>
            <p class="summary-text">
                This GridView demonstrates:
                <strong>1.</strong> Multi-culture currency formatting per row via <code>RowDataBound</code>,
                <strong>2.</strong> Hidden <code>CountryCulture</code> column,
                <strong>3.</strong> Conditional row formatting (Rows with Salary &gt; 70,000 highlighted in <strong>Red</strong>),
                <strong>4.</strong> <code>TemplateField</code> with <code>DropDownList</code> for Gender,
                <strong>5.</strong> <code>RequiredFieldValidator</code> controls, and
                <strong>6.</strong> Inline Insert, Update, and Delete using <code>FooterTemplate</code>.
            </p>

            <asp:Label ID="lblMessage" runat="server" CssClass="status-msg" Visible="false"></asp:Label>

            <!-- Validation Summaries for Edit and Insert -->
            <asp:ValidationSummary ID="ValidationSummaryEdit" runat="server" 
                ForeColor="Red" 
                HeaderText="Please correct the following errors:" 
                ValidationGroup="Edit" 
                CssClass="val-summary" />

            <asp:ValidationSummary ID="ValidationSummaryInsert" runat="server" 
                ForeColor="Red" 
                HeaderText="Please correct the following insert errors:" 
                ValidationGroup="Insert" 
                CssClass="val-summary" />

            <!-- Single Standalone GridView with All Properties from Slides -->
            <asp:GridView ID="GridView1" runat="server" 
                AutoGenerateColumns="False" 
                DataKeyNames="EmployeeId" 
                ShowFooter="True" 
                BackColor="#DEBA84" 
                BorderColor="#DEBA84" 
                BorderStyle="None" 
                BorderWidth="1px" 
                CellPadding="6" 
                CellSpacing="2"
                CssClass="grid-view"
                OnRowDataBound="GridView1_RowDataBound"
                OnRowEditing="GridView1_RowEditing"
                OnRowCancelingEdit="GridView1_RowCancelingEdit"
                OnRowUpdating="GridView1_RowUpdating"
                OnRowDeleting="GridView1_RowDeleting">
                <Columns>
                    <%-- Command Field for Edit and Delete (Pages 27 - 30) --%>
                    <asp:CommandField ShowDeleteButton="True" ShowEditButton="True" ValidationGroup="Edit" />

                    <%-- EmployeeId TemplateField with Insert LinkButton in Footer (Pages 27 - 30) --%>
                    <asp:TemplateField HeaderText="EmployeeId" InsertVisible="False" SortExpression="EmployeeId">
                        <EditItemTemplate>
                            <asp:Label ID="LabelId" runat="server" Text='<%# Eval("EmployeeId") %>'></asp:Label>
                        </EditItemTemplate>
                        <ItemTemplate>
                            <asp:Label ID="LabelId" runat="server" Text='<%# Bind("EmployeeId") %>'></asp:Label>
                        </ItemTemplate>
                        <FooterTemplate>
                            <asp:LinkButton ID="lbInsert" ValidationGroup="Insert" runat="server" OnClick="lbInsert_Click">Insert</asp:LinkButton>
                        </FooterTemplate>
                    </asp:TemplateField>

                    <%-- Name TemplateField with Validators (Pages 27 - 30) --%>
                    <asp:TemplateField HeaderText="Name" SortExpression="Name">
                        <EditItemTemplate>
                            <asp:TextBox ID="txtNameEdit" runat="server" Text='<%# Bind("Name") %>'></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvEditName" runat="server" 
                                ErrorMessage="Name is a required field"
                                ControlToValidate="txtNameEdit" Text="*" ForeColor="Red" ValidationGroup="Edit">
                            </asp:RequiredFieldValidator>
                        </EditItemTemplate>
                        <ItemTemplate>
                            <asp:Label ID="LabelName" runat="server" Text='<%# Bind("Name") %>'></asp:Label>
                        </ItemTemplate>
                        <FooterTemplate>
                            <asp:TextBox ID="txtName" runat="server" placeholder="Name"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvInsertName" runat="server" 
                                ErrorMessage="Name is a required field" 
                                ControlToValidate="txtName" Text="*" ForeColor="Red"
                                ValidationGroup="Insert">
                            </asp:RequiredFieldValidator>
                        </FooterTemplate>
                    </asp:TemplateField>

                    <%-- Gender TemplateField with DropDownList and InitialValue Validator (Pages 20 - 27) --%>
                    <asp:TemplateField HeaderText="Gender" SortExpression="Gender">
                        <EditItemTemplate>
                            <asp:DropDownList ID="DropDownList1" runat="server" SelectedValue='<%# Bind("Gender") %>'>
                                <asp:ListItem>Select Gender</asp:ListItem>
                                <asp:ListItem>Male</asp:ListItem>
                                <asp:ListItem>Female</asp:ListItem>
                            </asp:DropDownList>
                            <asp:RequiredFieldValidator ID="rfvEditGender" runat="server" 
                                ErrorMessage="Gender is a required field" Text="*"
                                ControlToValidate="DropDownList1" ForeColor="Red" 
                                InitialValue="Select Gender" ValidationGroup="Edit">
                            </asp:RequiredFieldValidator>
                        </EditItemTemplate>
                        <ItemTemplate>
                            <asp:Label ID="LabelGender" runat="server" Text='<%# Bind("Gender") %>'></asp:Label>
                        </ItemTemplate>
                        <FooterTemplate>
                            <asp:DropDownList ID="ddlInsertGender" runat="server">
                                <asp:ListItem>Select Gender</asp:ListItem>
                                <asp:ListItem>Male</asp:ListItem>
                                <asp:ListItem>Female</asp:ListItem>
                            </asp:DropDownList>
                            <asp:RequiredFieldValidator ID="rfvInsertGender" runat="server" 
                                ErrorMessage="Gender is a required field" Text="*"
                                ControlToValidate="ddlInsertGender" ForeColor="Red" 
                                InitialValue="Select Gender" ValidationGroup="Insert">
                            </asp:RequiredFieldValidator>
                        </FooterTemplate>
                    </asp:TemplateField>

                    <%-- City TemplateField with Validators (Pages 27 - 30) --%>
                    <asp:TemplateField HeaderText="City" SortExpression="City">
                        <EditItemTemplate>
                            <asp:TextBox ID="txtCityEdit" runat="server" Text='<%# Bind("City") %>'></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvEditCity" runat="server" 
                                ErrorMessage="City is a required field" Text="*"
                                ControlToValidate="txtCityEdit" ForeColor="Red" ValidationGroup="Edit">
                            </asp:RequiredFieldValidator>
                        </EditItemTemplate>
                        <ItemTemplate>
                            <asp:Label ID="LabelCity" runat="server" Text='<%# Bind("City") %>'></asp:Label>
                        </ItemTemplate>
                        <FooterTemplate>
                            <asp:TextBox ID="txtCity" runat="server" placeholder="City"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvInsertCity" runat="server" 
                                ErrorMessage="City is a required field" Text="*" 
                                ControlToValidate="txtCity" ForeColor="Red" 
                                ValidationGroup="Insert">
                            </asp:RequiredFieldValidator>
                        </FooterTemplate>
                    </asp:TemplateField>

                    <%-- AnnualSalary with Multi-Culture Formatting & Conditional Highlight (Pages 10 - 19) --%>
                    <asp:TemplateField HeaderText="AnnualSalary" SortExpression="AnnualSalary">
                        <EditItemTemplate>
                            <asp:TextBox ID="txtSalaryEdit" runat="server" Text='<%# Bind("AnnualSalary") %>'></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvEditSalary" runat="server" 
                                ErrorMessage="Salary is a required field" Text="*"
                                ControlToValidate="txtSalaryEdit" ForeColor="Red" ValidationGroup="Edit">
                            </asp:RequiredFieldValidator>
                        </EditItemTemplate>
                        <ItemTemplate>
                            <asp:Label ID="LabelSalary" runat="server" Text='<%# Eval("AnnualSalary") %>'></asp:Label>
                        </ItemTemplate>
                        <FooterTemplate>
                            <asp:TextBox ID="txtSalary" runat="server" placeholder="Salary"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvInsertSalary" runat="server" 
                                ErrorMessage="Salary is a required field" Text="*" 
                                ControlToValidate="txtSalary" ForeColor="Red" 
                                ValidationGroup="Insert">
                            </asp:RequiredFieldValidator>
                        </FooterTemplate>
                    </asp:TemplateField>

                    <%-- Country Column (Pages 10 - 13) --%>
                    <asp:TemplateField HeaderText="Country" SortExpression="Country">
                        <EditItemTemplate>
                            <asp:TextBox ID="txtCountryEdit" runat="server" Text='<%# Bind("Country") %>'></asp:TextBox>
                        </EditItemTemplate>
                        <ItemTemplate>
                            <asp:Label ID="LabelCountry" runat="server" Text='<%# Bind("Country") %>'></asp:Label>
                        </ItemTemplate>
                        <FooterTemplate>
                            <asp:TextBox ID="txtCountry" runat="server" placeholder="Country (US/UK/India)"></asp:TextBox>
                        </FooterTemplate>
                    </asp:TemplateField>

                    <%-- CountryCulture Hidden Column (Pages 14 - 16) --%>
                    <asp:BoundField DataField="CountryCulture" HeaderText="CountryCulture" SortExpression="CountryCulture" />
                </Columns>

                <%-- Design-Time Styles from Slides (Pages 17 - 19) --%>
                <FooterStyle BackColor="#F7DFB5" ForeColor="#8C4510" />
                <HeaderStyle BackColor="#A55129" Font-Bold="True" ForeColor="White" />
                <PagerStyle ForeColor="#8C4510" HorizontalAlign="Center" />
                <RowStyle BackColor="#FFF7E7" ForeColor="#8C4510" />
                <SelectedRowStyle BackColor="#738A9C" Font-Bold="True" ForeColor="White" />
                <SortedAscendingCellStyle BackColor="#FFF1D4" />
                <SortedAscendingHeaderStyle BackColor="#B95C30" />
                <SortedDescendingCellStyle BackColor="#F1E5CE" />
                <SortedDescendingHeaderStyle BackColor="#93451F" />
            </asp:GridView>
        </div>
    </form>
</body>
</html>
