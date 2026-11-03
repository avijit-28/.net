// FoodReady Site JavaScript
$(document).ready(function () {
    // Quick Add to Cart via AJAX
    $(document).on('click', '.btn-add-to-cart', function (e) {
        e.preventDefault();
        var $btn = $(this);
        var foodId = $btn.data('id');
        var $qtyInput = $btn.closest('.food-card, .food-detail-card').find('.input-quantity');
        var qty = $qtyInput.length ? parseInt($qtyInput.val()) || 1 : 1;

        $btn.prop('disabled', true).html('<span class="spinner-border spinner-border-sm" role="status"></span> Adding...');

        $.ajax({
            url: '/Order/AddToCart',
            type: 'POST',
            data: { foodItemId: foodId, quantity: qty },
            success: function (res) {
                if (res.success) {
                    $('#cart-badge').text(res.totalCount).removeClass('d-none');
                    showToast(res.message || 'Item added to cart!');
                } else {
                    alert(res.message || 'Could not add item to cart.');
                }
            },
            error: function () {
                alert('An error occurred while adding the item.');
            },
            complete: function () {
                $btn.prop('disabled', false).html('<i class="bi bi-cart-plus me-1"></i> Add to Cart');
            }
        });
    });

    // Quantity Increment / Decrement
    $(document).on('click', '.btn-qty-plus', function () {
        var $input = $(this).siblings('.input-quantity');
        var val = parseInt($input.val()) || 1;
        $input.val(val + 1).trigger('change');
    });

    $(document).on('click', '.btn-qty-minus', function () {
        var $input = $(this).siblings('.input-quantity');
        var val = parseInt($input.val()) || 1;
        if (val > 1) {
            $input.val(val - 1).trigger('change');
        }
    });

    // Cart Page Quantity change
    $(document).on('change', '.cart-qty-input', function () {
        var $input = $(this);
        var foodId = $input.data('id');
        var qty = parseInt($input.val()) || 0;

        $.ajax({
            url: '/Order/UpdateQuantity',
            type: 'POST',
            data: { foodItemId: foodId, quantity: qty },
            success: function (res) {
                if (res.success) {
                    if (qty <= 0) {
                        $('#cart-row-' + foodId).fadeOut(300, function () { $(this).remove(); checkEmptyCart(); });
                    } else {
                        $('#item-subtotal-' + foodId).text('₹' + res.itemSubtotal.toFixed(2));
                    }
                    $('#cart-subtotal').text('₹' + res.subtotal.toFixed(2));
                    $('#cart-delivery-fee').text('₹' + res.deliveryFee.toFixed(2));
                    $('#cart-total').text('₹' + res.totalAmount.toFixed(2));
                    $('#cart-badge').text(res.totalCount);
                    if (res.totalCount === 0) {
                        $('#cart-badge').addClass('d-none');
                    }
                }
            }
        });
    });

    function checkEmptyCart() {
        if ($('.cart-table tbody tr').length === 0) {
            location.reload();
        }
    }

    // Toast alert helper
    function showToast(msg) {
        var $toast = $('#site-toast');
        if ($toast.length === 0) {
            $('body').append('<div id="site-toast" class="site-toast position-fixed bottom-0 end-0 p-3" style="z-index: 99999;"><div class="toast align-items-center text-white bg-dark border-0 show" role="alert"><div class="d-flex"><div class="toast-body"><i class="bi bi-check-circle-fill text-success me-2"></i><span id="site-toast-msg">' + msg + '</span></div><button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast"></button></div></div></div>');
            setTimeout(function () { $('#site-toast').fadeOut(400, function () { $(this).remove(); }); }, 3000);
        } else {
            $('#site-toast-msg').text(msg);
            $toast.show();
            setTimeout(function () { $toast.fadeOut(400); }, 3000);
        }
    }
});
