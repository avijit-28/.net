<%@ Page Title="Product Catalog" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="MiniProject.Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container">
        <!-- Hero Section -->
        <div class="hero-banner mb-4">
            <div class="row align-items-center">
                <div class="col-lg-8">
                    <span class="badge bg-primary text-uppercase px-3 py-2 mb-3 fw-bold tracking-wider">
                        <i class="bi bi-stars me-1"></i> New Season Tech Arrivals
                    </span>
                    <h1 class="display-5 fw-extrabold mb-3">Upgrade Your Digital Lifestyle</h1>
                    <p class="lead text-light opacity-75 mb-4">
                        Discover top-rated headphones, powerhouse laptops, smart wearables, and next-gen gaming hardware.
                    </p>
                    <div class="d-flex flex-wrap gap-2">
                        <asp:LinkButton ID="btnQuickAll" runat="server" OnClick="btnQuickFilter_Click" CommandArgument="All" CssClass="btn btn-light btn-sm fw-semibold rounded-pill px-3">All Products</asp:LinkButton>
                        <asp:LinkButton ID="btnQuickAudio" runat="server" OnClick="btnQuickFilter_Click" CommandArgument="Audio" CssClass="btn btn-outline-light btn-sm fw-semibold rounded-pill px-3">Audio</asp:LinkButton>
                        <asp:LinkButton ID="btnQuickLaptops" runat="server" OnClick="btnQuickFilter_Click" CommandArgument="Laptops" CssClass="btn btn-outline-light btn-sm fw-semibold rounded-pill px-3">Laptops</asp:LinkButton>
                        <asp:LinkButton ID="btnQuickGaming" runat="server" OnClick="btnQuickFilter_Click" CommandArgument="Gaming" CssClass="btn btn-outline-light btn-sm fw-semibold rounded-pill px-3">Gaming</asp:LinkButton>
                        <asp:LinkButton ID="btnQuickWearables" runat="server" OnClick="btnQuickFilter_Click" CommandArgument="Wearables" CssClass="btn btn-outline-light btn-sm fw-semibold rounded-pill px-3">Wearables</asp:LinkButton>
                    </div>
                </div>
            </div>
        </div>

        <!-- Notification Alert -->
        <asp:Panel ID="pnlAlert" runat="server" Visible="false" CssClass="alert alert-success alert-dismissible fade show d-flex align-items-center mb-4" role="alert">
            <i class="bi bi-check-circle-fill fs-5 me-2"></i>
            <div>
                <asp:Label ID="lblAlertMessage" runat="server" Text=""></asp:Label>
            </div>
            <button type="button" class="btn-close ms-auto" data-bs-dismiss="alert" aria-label="Close"></button>
        </asp:Panel>

        <!-- Search & Filter Controls -->
        <div class="filter-bar">
            <div class="row g-3 align-items-center">
                <div class="col-md-5">
                    <div class="input-group">
                        <span class="input-group-text bg-white border-end-0 text-muted"><i class="bi bi-search"></i></span>
                        <asp:TextBox ID="txtSearch" runat="server" CssClass="form-control border-start-0" Placeholder="Search by product name, category, or features..."></asp:TextBox>
                        <asp:Button ID="btnSearch" runat="server" Text="Search" OnClick="btnSearch_Click" CssClass="btn btn-primary px-3" />
                    </div>
                </div>

                <div class="col-md-3">
                    <div class="input-group">
                        <label class="input-group-text bg-white text-muted"><i class="bi bi-funnel"></i></label>
                        <asp:DropDownList ID="ddlCategory" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlFilter_Changed" CssClass="form-select">
                        </asp:DropDownList>
                    </div>
                </div>

                <div class="col-md-3">
                    <div class="input-group">
                        <label class="input-group-text bg-white text-muted"><i class="bi bi-sort-down"></i></label>
                        <asp:DropDownList ID="ddlSort" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlFilter_Changed" CssClass="form-select">
                            <asp:ListItem Value="featured" Text="Sort: Featured" Selected="True"></asp:ListItem>
                            <asp:ListItem Value="price_asc" Text="Price: Low to High"></asp:ListItem>
                            <asp:ListItem Value="price_desc" Text="Price: High to Low"></asp:ListItem>
                            <asp:ListItem Value="rating" Text="Customer Rating"></asp:ListItem>
                            <asp:ListItem Value="name" Text="Name: A to Z"></asp:ListItem>
                        </asp:DropDownList>
                    </div>
                </div>

                <div class="col-md-1 text-md-end">
                    <asp:LinkButton ID="btnReset" runat="server" OnClick="btnReset_Click" CssClass="btn btn-outline-secondary w-100" ToolTip="Reset Filters">
                        <i class="bi bi-arrow-counterclockwise"></i>
                    </asp:LinkButton>
                </div>
            </div>
        </div>

        <!-- Catalog Header & Counter -->
        <div class="d-flex justify-content-between align-items-center mb-3">
            <div>
                <h3 class="fw-bold mb-0 text-dark">Explore Our Products</h3>
                <small class="text-muted">
                    Showing <asp:Label ID="lblProductCount" runat="server" Font-Bold="true" Text="0"></asp:Label> items
                </small>
            </div>
        </div>

        <!-- Products Grid Repeater -->
        <div class="row g-4">
            <asp:Repeater ID="rptProducts" runat="server" OnItemCommand="rptProducts_ItemCommand">
                <ItemTemplate>
                    <div class="col-12 col-sm-6 col-lg-4 col-xl-3">
                        <div class="product-card">
                            <div class="product-img-wrapper">
                                <a href='ProductDetails.aspx?id=<%# Eval("Id") %>'>
                                    <img src='<%# Eval("ImageUrl") %>' alt='<%# Eval("Name") %>' loading="lazy" />
                                </a>

                                <%# (decimal)Eval("DiscountPercent") > 0 
                                    ? "<span class='product-badge-discount'>-" + Eval("DiscountPercent") + "%</span>" 
                                    : "" %>
                                
                                <span class="product-badge-cat"><%# Eval("Category") %></span>
                            </div>

                            <div class="card-body-custom">
                                <div class="d-flex justify-content-between align-items-center mb-2">
                                    <div class="rating-stars">
                                        <i class="bi bi-star-fill"></i>
                                        <span class="fw-bold text-dark ms-1"><%# Eval("Rating") %></span>
                                        <span class="text-muted small">(<%# Eval("ReviewCount") %>)</span>
                                    </div>
                                    <span class='<%# (int)Eval("Stock") > 0 ? "badge bg-success-subtle text-success border border-success-subtle" : "badge bg-danger-subtle text-danger" %>'>
                                        <%# (int)Eval("Stock") > 0 ? "In Stock" : "Out of Stock" %>
                                    </span>
                                </div>

                                <a href='ProductDetails.aspx?id=<%# Eval("Id") %>' class="product-title" title='<%# Eval("Name") %>'>
                                    <%# Eval("Name") %>
                                </a>

                                <p class="product-desc"><%# Eval("Description") %></p>

                                <div class="mt-auto pt-3 border-top d-flex flex-column gap-2">
                                    <div class="d-flex align-items-baseline">
                                        <span class="price-current">$<%# string.Format("{0:N2}", Eval("Price")) %></span>
                                        <%# Eval("OriginalPrice") != null 
                                            ? "<span class='price-original'>$" + string.Format("{0:N2}", Eval("OriginalPrice")) + "</span>" 
                                            : "" %>
                                    </div>

                                    <div class="d-grid gap-2 d-flex">
                                        <a href='ProductDetails.aspx?id=<%# Eval("Id") %>' class="btn btn-outline-secondary btn-sm flex-grow-1">
                                            <i class="bi bi-eye"></i> Details
                                        </a>
                                        <asp:LinkButton ID="btnAddToCart" runat="server" 
                                            CommandName="AddToCart" 
                                            CommandArgument='<%# Eval("Id") %>' 
                                            CssClass="btn btn-primary btn-sm flex-grow-1"
                                            Enabled='<%# (int)Eval("Stock") > 0 %>'>
                                            <i class="bi bi-cart-plus"></i> Add
                                        </asp:LinkButton>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </div>

        <!-- No Products Found State -->
        <asp:Panel ID="pnlNoProducts" runat="server" Visible="false" CssClass="text-center py-5">
            <div class="py-5">
                <i class="bi bi-search text-muted" style="font-size: 3.5rem;"></i>
                <h4 class="fw-bold mt-3 text-dark">No products found</h4>
                <p class="text-muted">We couldn't find any products matching your search criteria.</p>
                <asp:Button ID="btnClearSearch" runat="server" Text="Clear Filters" OnClick="btnReset_Click" CssClass="btn btn-primary" />
            </div>
        </asp:Panel>
    </div>
</asp:Content>
