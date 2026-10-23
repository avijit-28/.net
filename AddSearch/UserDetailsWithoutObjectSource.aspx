<%@ Page Language="C#" AutoEventWireup="true"
    CodeBehind="UserDetailsWithoutObjectSource.aspx.cs"
    Inherits="AddSearch.UserDetailsWithoutObjectSource" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>User Details Without ObjectDataSource</title>
</head>
<body>
    <form id="form1" runat="server">

        <asp:GridView ID="GridView1"
            runat="server"
            AutoGenerateColumns="False"
            DataKeyNames="UserId"
            ShowFooter="True"
            OnRowCommand="GridView1_RowCommand" 
            AllowPaging="true"
            PageSize="5">
            

            <Columns>

              

                <asp:TemplateField HeaderText="User ID">
                    <ItemTemplate>
                        <asp:Label ID="lblUserId"
                            runat="server"
                            Text='<%# Eval("UserId") %>'>
                        </asp:Label>
                    </ItemTemplate>

                    <FooterTemplate>
                        <asp:TextBox ID="txtUserId"
                            runat="server">
                        </asp:TextBox>
                    </FooterTemplate>
                </asp:TemplateField>

             

                <asp:TemplateField HeaderText="User Name">
                    <ItemTemplate>
                        <%# Eval("UserName") %>
                    </ItemTemplate>

                    <EditItemTemplate>
                        <asp:TextBox ID="txtUserName"
                            runat="server"
                            Text='<%# Eval("UserName") %>'>
                        </asp:TextBox>
                    </EditItemTemplate>

                    <FooterTemplate>
                        <asp:TextBox ID="txtUserNameFooter"
                            runat="server">
                        </asp:TextBox>
                    </FooterTemplate>
                </asp:TemplateField>

               

                <asp:TemplateField HeaderText="Password">
                    <ItemTemplate>
                        <%# Eval("Password") %>
                    </ItemTemplate>

                    <EditItemTemplate>
                        <asp:TextBox ID="txtPassword"
                            runat="server"
                            Text='<%# Eval("Password") %>'>
                        </asp:TextBox>
                    </EditItemTemplate>

                    <FooterTemplate>
                        <asp:TextBox ID="txtPasswordFooter"
                            runat="server">
                        </asp:TextBox>
                    </FooterTemplate>
                </asp:TemplateField>

         

                <asp:TemplateField HeaderText="Email">
                    <ItemTemplate>
                        <%# Eval("Email") %>
                    </ItemTemplate>

                    <EditItemTemplate>
                        <asp:TextBox ID="txtEmail"
                            runat="server"
                            Text='<%# Eval("Email") %>'>
                        </asp:TextBox>
                    </EditItemTemplate>

                    <FooterTemplate>
                        <asp:TextBox ID="txtEmailFooter"
                            runat="server">
                        </asp:TextBox>
                    </FooterTemplate>
                </asp:TemplateField>

              
                <asp:TemplateField HeaderText="Mobile No">
                    <ItemTemplate>
                        <%# Eval("MobileNo") %>
                    </ItemTemplate>

                    <EditItemTemplate>
                        <asp:TextBox ID="txtMobileNo"
                            runat="server"
                            Text='<%# Eval("MobileNo") %>'>
                        </asp:TextBox>
                    </EditItemTemplate>

                    <FooterTemplate>
                        <asp:TextBox ID="txtMobileNoFooter"
                            runat="server">
                        </asp:TextBox>
                    </FooterTemplate>
                </asp:TemplateField>

            

                <asp:TemplateField HeaderText="City">
                    <ItemTemplate>
                        <%# Eval("City") %>
                    </ItemTemplate>

                    <EditItemTemplate>
                        <asp:TextBox ID="txtCity"
                            runat="server"
                            Text='<%# Eval("City") %>'>
                        </asp:TextBox>
                    </EditItemTemplate>

                    <FooterTemplate>
                        <asp:TextBox ID="txtCityFooter"
                            runat="server">
                        </asp:TextBox>
                    </FooterTemplate>
                </asp:TemplateField>


                <asp:TemplateField HeaderText="Actions">

                    <ItemTemplate>

                        <asp:LinkButton ID="lbEdit"
                            runat="server"
                            CommandName="EditRow"
                            CommandArgument='<%# Eval("UserId") %>'
                            ForeColor="#8C4510">
                            Edit
                        </asp:LinkButton>

                        &nbsp;&nbsp;

                        <asp:LinkButton ID="lbDelete"
                            runat="server"
                            CommandName="DeleteRow"
                            CommandArgument='<%# Eval("UserId") %>'
                            ForeColor="#8C4510"
                            OnClientClick="return confirm('Are you sure you want to delete this row')"
                            CausesValidation="false">
                            Delete
                        </asp:LinkButton>

                    </ItemTemplate>

                    <EditItemTemplate>

                        <asp:LinkButton ID="lbUpdate"
                            runat="server"
                            CommandName="UpdateRow"
                            CommandArgument='<%# Eval("UserId") %>'
                            OnClientClick="return confirm('Are you sure you want to update this row')"
                            ForeColor="#8C4510">
                            Update
                        </asp:LinkButton>

                        &nbsp;&nbsp;

                        <asp:LinkButton ID="lbCancel"
                            runat="server"
                            CommandName="CancelUpdate"
                            CommandArgument='<%# Eval("UserId") %>'
                            ForeColor="#8C4510"
                            CausesValidation="false">
                            Cancel
                        </asp:LinkButton>

                    </EditItemTemplate>

                    <FooterTemplate>

                        <asp:LinkButton ID="lbInsert"
                            runat="server"
                            CommandName="InsertRow"
                            ForeColor="#8C4510">
                            Insert
                        </asp:LinkButton>

                    </FooterTemplate>

                </asp:TemplateField>

            </Columns>

        </asp:GridView>
        <asp:Repeater ID="repeaterPaging" runat="server">
            <ItemTemplate>

            <asp:LinkButton ID="pagingLinkButton"
            runat="server"
            Text='<%# Eval("Text") %>'
            CommandArgument='<%# Eval("Value") %>'
            Enabled='<%# Eval("Enabled") %>'
            OnClick="linkButton_Click">
            </asp:LinkButton>

            &nbsp;

            </ItemTemplate>
    </asp:Repeater>


    </form>
</body>
</html>