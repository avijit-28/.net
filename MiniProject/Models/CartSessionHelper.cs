using System.Web;

namespace MiniProject.Models
{
    public static class CartSessionHelper
    {
        private const string CartSessionKey = "UserCartSession";
        private const string OrderSessionKey = "UserLastOrderSession";

        public static Cart GetCart()
        {
            if (HttpContext.Current.Session[CartSessionKey] == null)
            {
                HttpContext.Current.Session[CartSessionKey] = new Cart();
            }
            return (Cart)HttpContext.Current.Session[CartSessionKey];
        }

        public static void SaveCart(Cart cart)
        {
            HttpContext.Current.Session[CartSessionKey] = cart;
        }

        public static void ClearCart()
        {
            var cart = GetCart();
            cart.Clear();
            SaveCart(cart);
        }

        public static void SetLastOrder(Order order)
        {
            HttpContext.Current.Session[OrderSessionKey] = order;
        }

        public static Order GetLastOrder()
        {
            return HttpContext.Current.Session[OrderSessionKey] as Order;
        }
    }
}
