<%@ Page Title="Shopping Cart" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Cart.aspx.cs" Inherits="MiniProject.CartPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container">
        <h2 class="fw-extrabold text-dark mb-4">
            <i class="bi bi-cart3 me-2 text-primary"></i>Shopping Cart
        </h2>

        <!-- Alert Notification -->
        <asp:Panel ID="pnlAlert" runat="server" Visible="false" CssClass="alert alert-info alert-dismissible fade show mb-4" role="alert">
            <asp:Label ID="lblAlertMessage" runat="server"></asp:Label>
            <button type="button" class="btn-close ms-auto" data-bs-dismiss="alert" aria-label="Close"></button>
        </asp:Panel>

        <!-- Empty Cart Panel -->
        <asp:Panel ID="pnlEmptyCart" runat="server" Visible="false" CssClass="text-center py-5 bg-white rounded-4 border p-5">
            <div class="py-4">
                <i class="bi bi-cart-x text-muted" style="font-size: 4rem;"></i>
                <h3 class="fw-bold mt-3 text-dark">Your Cart is Currently Empty</h3>
                <p class="text-muted mb-4">Looks like you haven't added any tech items to your cart yet.</p>
                <a href="Default.aspx" class="btn btn-primary btn-lg px-4">
                    <i class="bi bi-bag-plus me-1"></i> Start Shopping Now
                </a>
            </div>
        </asp:Panel>

        <!-- Active Cart Panel -->
        <asp:Panel ID="pnlCartContent" runat="server">
            <div class="row g-4">
                <!-- Cart Items Table Column -->
                <div class="col-lg-8">
                    <div class="bg-white rounded-4 border p-4 shadow-sm mb-3">
                        <div class="table-responsive">
                            <table class="table align-middle mb-0">
                                <thead class="table-light">
                                    <tr>
                                        <th scope="col" style="min-width: 250px;">Product</th>
                                        <th scope="col" class="text-center">Price</th>
                                        <th scope="col" class="text-center" style="min-width: 140px;">Quantity</th>
                                        <th scope="col" class="text-end">Subtotal</th>
                                        <th scope="col" class="text-center"></th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <asp:Repeater ID="rptCartItems" runat="server" OnItemCommand="rptCartItems_ItemCommand">
                                        <ItemTemplate>
                                            <tr>
                                                <td>
                                                    <div class="d-flex align-items-center gap-3">
                                                        <img src='<%# Eval("Product.ImageUrl") %>' alt='<%# Eval("Product.Name") %>' class="cart-img" />
                                                        <div>
                                                            <a href='ProductDetails.aspx?id=<%# Eval("Product.Id") %>' class="fw-bold text-dark text-decoration-none">
                                                                <%# Eval("Product.Name") %>
                                                            </a>
                                                            <div class="small text-muted"><%# Eval("Product.Category") %></div>
                                                        </div>
                                                    </div>
                                                </td>
                                                <td class="text-center fw-medium">
                                                    $<%# string.Format("{0:N2}", Eval("Product.Price")) %>
                                                </td>
                                                <td class="text-center">
                                                    <div class="d-inline-flex align-items-center border rounded-2 p-1 bg-light">
                                                        <asp:LinkButton ID="btnMinus" runat="server" 
                                                            CommandName="Decrease" 
                                                            CommandArgument='<%# Eval("Product.Id") %>' 
                                                            CssClass="btn btn-sm btn-light qty-btn text-dark">
                                                            <i class="bi bi-dash"></i>
                                                        </asp:LinkButton>
                                                        
                                                        <span class="px-3 fw-bold"><%# Eval("Quantity") %></span>
                                                        
                                                        <asp:LinkButton ID="btnPlus" runat="server" 
                                                            CommandName="Increase" 
                                                            CommandArgument='<%# Eval("Product.Id") %>' 
                                                            CssClass="btn btn-sm btn-light qty-btn text-dark">
                                                            <i class="bi bi-plus"></i>
                                                        </asp:LinkButton>
                                                    </div>
                                                </td>
                                                <td class="text-end fw-bold text-dark">
                                                    $<%# string.Format("{0:N2}", Eval("Subtotal")) %>
                                                </td>
                                                <td class="text-center">
                                                    <asp:LinkButton ID="btnRemove" runat="server" 
                                                        CommandName="Remove" 
                                                        CommandArgument='<%# Eval("Product.Id") %>' 
                                                        CssClass="btn btn-sm text-danger" 
                                                        ToolTip="Remove Item">
                                                        <i class="bi bi-trash3 fs-5"></i>
                                                    </asp:LinkButton>
                                                </td>
                                            </tr>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </tbody>
                            </table>
                        </div>

                        <div class="d-flex flex-wrap justify-content-between align-items-center mt-4 pt-3 border-top gap-2">
                            <a href="Default.aspx" class="btn btn-outline-secondary">
                                <i class="bi bi-arrow-left me-1"></i> Continue Shopping
                            </a>
                            <asp:Button ID="btnClearCart" runat="server" Text="Clear All Items" OnClick="btnClearCart_Click" 
                                OnClientClick="return confirm('Are you sure you want to empty your cart?');" 
                                CssClass="btn btn-outline-danger" />
                        </div>
                    </div>
                </div>

                <!-- Summary Column -->
                <div class="col-lg-4">
                    <div class="summary-card">
                        <h5 class="fw-bold text-dark mb-4 pb-2 border-bottom">Order Summary</h5>

                        <div class="d-flex justify-content-between mb-3 text-muted">
                            <span>Items (<asp:Label ID="lblItemCount" runat="server" Text="0"></asp:Label>)</span>
                            <span class="text-dark fw-medium">$<asp:Label ID="lblSubtotal" runat="server" Text="0.00"></asp:Label></span>
                        </div>

                        <div class="d-flex justify-content-between mb-3 text-muted">
                            <span>Shipping</span>
                            <span class="text-dark fw-medium">
                                <asp:Label ID="lblShipping" runat="server" Text="0.00"></asp:Label>
                            </span>
                        </div>

                        <div class="d-flex justify-content-between mb-4 text-muted">
                            <span>Estimated Tax (8%)</span>
                            <span class="text-dark fw-medium">$<asp:Label ID="lblTax" runat="server" Text="0.00"></asp:Label></span>
                        </div>

                        <div class="border-top pt-3 mb-4">
                            <div class="d-flex justify-content-between align-items-center">
                                <span class="fs-5 fw-bold text-dark">Grand Total</span>
                                <span class="fs-4 fw-extrabold text-primary">$<asp:Label ID="lblGrandTotal" runat="server" Text="0.00"></asp:Label></span>
                            </div>
                            <small class="text-muted d-block mt-1">Includes applicable taxes & delivery fees.</small>
                        </div>

                        <asp:Button ID="btnCheckout" runat="server" Text="Proceed to Checkout" OnClick="btnCheckout_Click" CssClass="btn btn-primary btn-lg w-100 fw-bold shadow-sm mb-3" />

                        <div class="p-3 bg-light rounded-3 text-center small text-muted">
                            <i class="bi bi-lock-fill text-success me-1"></i> Secure 256-bit SSL Checkout Encrypted
                        </div>
                    </div>
                </div>
            </div>
        </asp:Panel>
    </div>
</asp:Content>
