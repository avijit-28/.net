<%@ Page Title="Product Details" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ProductDetails.aspx.cs" Inherits="MiniProject.ProductDetails" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container">
        <!-- Breadcrumbs -->
        <nav aria-label="breadcrumb" class="mb-4">
            <ol class="breadcrumb">
                <li class="breadcrumb-item"><a href="Default.aspx" class="text-decoration-none">Catalog</a></li>
                <li class="breadcrumb-item">
                    <asp:HyperLink ID="lnkCategoryBreadcrumb" runat="server" CssClass="text-decoration-none"></asp:HyperLink>
                </li>
                <li class="breadcrumb-item active" aria-current="page">
                    <asp:Label ID="lblBreadcrumbTitle" runat="server"></asp:Label>
                </li>
            </ol>
        </nav>

        <!-- Notification Banner -->
        <asp:Panel ID="pnlAlert" runat="server" Visible="false" CssClass="alert alert-success alert-dismissible fade show d-flex align-items-center mb-4" role="alert">
            <i class="bi bi-check-circle-fill fs-5 me-2"></i>
            <div>
                <asp:Label ID="lblAlertMessage" runat="server" Text=""></asp:Label>
            </div>
            <button type="button" class="btn-close ms-auto" data-bs-dismiss="alert" aria-label="Close"></button>
        </asp:Panel>

        <!-- Product Not Found Panel -->
        <asp:Panel ID="pnlNotFound" runat="server" Visible="false" CssClass="text-center py-5">
            <div class="py-5">
                <i class="bi bi-exclamation-circle text-danger" style="font-size: 3.5rem;"></i>
                <h3 class="fw-bold mt-3 text-dark">Product Not Found</h3>
                <p class="text-muted">The product you are looking for might have been removed or does not exist.</p>
                <a href="Default.aspx" class="btn btn-primary mt-2"><i class="bi bi-arrow-left me-1"></i> Back to Catalog</a>
            </div>
        </asp:Panel>

        <!-- Product Details Container -->
        <asp:Panel ID="pnlDetails" runat="server">
            <div class="row g-5">
                <!-- Product Image Column -->
                <div class="col-lg-6">
                    <div class="position-relative">
                        <asp:Image ID="imgProduct" runat="server" CssClass="product-detail-img" />
                        <asp:Label ID="lblDiscountBadge" runat="server" CssClass="badge bg-danger position-absolute top-0 start-0 m-3 fs-6 rounded-pill px-3 py-2 shadow-sm"></asp:Label>
                    </div>
                </div>

                <!-- Product Info & Purchase Column -->
                <div class="col-lg-6">
                    <div class="d-flex align-items-center gap-2 mb-2">
                        <asp:Label ID="lblCategory" runat="server" CssClass="badge bg-primary-subtle text-primary border border-primary-subtle px-3 py-2 fs-6 rounded-pill"></asp:Label>
                        <asp:Label ID="lblStockStatus" runat="server" CssClass="badge px-3 py-2 fs-6 rounded-pill"></asp:Label>
                    </div>

                    <h1 class="fw-extrabold text-dark mb-3">
                        <asp:Label ID="lblProductName" runat="server"></asp:Label>
                    </h1>

                    <div class="d-flex align-items-center gap-3 mb-4">
                        <div class="text-warning">
                            <i class="bi bi-star-fill"></i>
                            <asp:Label ID="lblRating" runat="server" Font-Bold="true" CssClass="text-dark ms-1"></asp:Label>
                        </div>
                        <span class="text-muted">|</span>
                        <span class="text-muted">
                            <asp:Label ID="lblReviewCount" runat="server"></asp:Label> customer reviews
                        </span>
                    </div>

                    <div class="d-flex align-items-baseline mb-4 p-3 bg-white rounded-3 border">
                        <span class="price-badge-detail">
                            $<asp:Label ID="lblPrice" runat="server"></asp:Label>
                        </span>
                        <asp:Label ID="lblOriginalPrice" runat="server" CssClass="original-price-detail"></asp:Label>
                    </div>

                    <h5 class="fw-bold text-dark mb-2">Description</h5>
                    <p class="text-muted mb-4 lead fs-6">
                        <asp:Label ID="lblDescription" runat="server"></asp:Label>
                    </p>

                    <div class="spec-card mb-4">
                        <h6 class="fw-bold text-dark mb-2"><i class="bi bi-check2-circle text-primary me-2"></i>Product Highlights</h6>
                        <ul class="mb-0 text-muted small ps-3">
                            <li>100% Genuine guaranteed brand authentic device</li>
                            <li>Standard 1-year manufacturer warranty included</li>
                            <li>Free shipping on qualifying orders</li>
                            <li>Hassle-free 30-day return policy</li>
                        </ul>
                    </div>

                    <!-- Quantity & Add to Cart -->
                    <div class="d-flex flex-wrap align-items-center gap-3 mb-4">
                        <div style="width: 120px;">
                            <label class="form-label small fw-semibold text-muted mb-1">Quantity</label>
                            <asp:TextBox ID="txtQuantity" runat="server" TextMode="Number" Text="1" min="1" max="99" CssClass="form-control text-center fw-bold"></asp:TextBox>
                        </div>

                        <div class="flex-grow-1" style="padding-top: 23px;">
                            <asp:Button ID="btnAddToCart" runat="server" Text="Add to Cart" OnClick="btnAddToCart_Click" CssClass="btn btn-primary btn-lg w-100 fw-semibold" />
                        </div>
                    </div>

                    <div class="d-flex gap-2">
                        <a href="Default.aspx" class="btn btn-outline-secondary">
                            <i class="bi bi-arrow-left me-1"></i> Back to Catalog
                        </a>
                        <a href="Cart.aspx" class="btn btn-outline-primary">
                            <i class="bi bi-cart3 me-1"></i> View Cart
                        </a>
                    </div>
                </div>
            </div>

            <!-- Related Products -->
            <div class="mt-5 pt-4 border-top">
                <h4 class="fw-bold text-dark mb-4">Related Products in this Category</h4>
                <div class="row g-4">
                    <asp:Repeater ID="rptRelated" runat="server">
                        <ItemTemplate>
                            <div class="col-md-4">
                                <div class="related-card h-100 d-flex flex-column">
                                    <img src='<%# Eval("ImageUrl") %>' alt='<%# Eval("Name") %>' class="related-img" />
                                    <div class="p-3 d-flex flex-column flex-grow-1">
                                        <h6 class="fw-bold mb-1 text-dark">
                                            <a href='ProductDetails.aspx?id=<%# Eval("Id") %>' class="text-decoration-none text-dark">
                                                <%# Eval("Name") %>
                                            </a>
                                        </h6>
                                        <div class="mt-auto pt-2 d-flex justify-content-between align-items-center">
                                            <span class="fw-bold text-primary">$<%# string.Format("{0:N2}", Eval("Price")) %></span>
                                            <a href='ProductDetails.aspx?id=<%# Eval("Id") %>' class="btn btn-sm btn-outline-primary">View</a>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
            </div>
        </asp:Panel>
    </div>
</asp:Content>
