<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ProductLinqCurd.aspx.cs" Inherits="AddSearch.Connected_Services.ProductLinqCurd" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Product LINQ CRUD</title>
</head>
<body>
    <form id="form1" runat="server">

        <asp:GridView ID="GridView1" runat="server"
            AutoGenerateColumns="False"
            DataKeyNames="ID,Name"
            ShowFooter="True"
            AllowPaging="True"
            PageSize="10"
            
            OnRowEditing="GridView1_RowEditing"
            OnRowUpdating="GridView1_RowUpdating"
            OnRowCancelingEdit="GridView1_RowCancelingEdit"
            OnRowDeleting="GridView1_RowDeleting"
            OnRowCommand="GridView1_RowCommand">

            <Columns>

                
                <asp:TemplateField HeaderText="ID">
                    <ItemTemplate>
                        <asp:Label ID="lblID" runat="server"
                            Text='<%# Eval("ID") %>'></asp:Label>
                    </ItemTemplate>

                    <EditItemTemplate>
                        <asp:TextBox ID="txtID" runat="server"
                            Text='<%# Eval("ID") %>'
                            ReadOnly="true"></asp:TextBox>
                    </EditItemTemplate>

                    <FooterTemplate>
                        <asp:TextBox ID="txtfID" runat="server"></asp:TextBox>
                    </FooterTemplate>
                </asp:TemplateField>

      
                <asp:TemplateField HeaderText="Name">
                    <ItemTemplate>
                        <asp:Label ID="lblName" runat="server"
                            Text='<%# Eval("Name") %>'></asp:Label>
                    </ItemTemplate>

                    <EditItemTemplate>
                        <asp:TextBox ID="txtName" runat="server"
                            Text='<%# Eval("Name") %>'></asp:TextBox>
                    </EditItemTemplate>

                    <FooterTemplate>
                        <asp:TextBox ID="txtfName" runat="server"></asp:TextBox>
                    </FooterTemplate>
                </asp:TemplateField>

              
                <asp:TemplateField HeaderText="Price">
                    <ItemTemplate>
                        <asp:Label ID="lblPrice" runat="server"
                            Text='<%# Eval("Price") %>'></asp:Label>
                    </ItemTemplate>

                    <EditItemTemplate>
                        <asp:TextBox ID="txtPrice" runat="server"
                            Text='<%# Eval("Price") %>'></asp:TextBox>
                    </EditItemTemplate>

                    <FooterTemplate>
                        <asp:TextBox ID="txtfPrice" runat="server"></asp:TextBox>
                    </FooterTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Stock">
                    <ItemTemplate>
                        <asp:Label ID="lblStock" runat="server"
                            Text='<%# Eval("stock") %>'></asp:Label>
                    </ItemTemplate>

                    <EditItemTemplate>
                        <asp:TextBox ID="txtStock" runat="server"
                            Text='<%# Eval("stock") %>'></asp:TextBox>
                    </EditItemTemplate>

                    <FooterTemplate>
                        <asp:TextBox ID="txtfStock" runat="server"></asp:TextBox>

                        <asp:Button ID="btnAdd"
                            runat="server"
                            Text="Add"
                            CommandName="AddNew" />
                    </FooterTemplate>
                </asp:TemplateField>

               <asp:CommandField ShowEditButton="True"
                  ShowDeleteButton="True" />
                

              

            </Columns>

        </asp:GridView>

        <br />

        <asp:Label ID="lblResult" runat="server"></asp:Label>

    </form>
</body>
</html>