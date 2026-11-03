using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FoodArrival.Models
{
    public static class CartSession
    {
        private const string Key = "FoodArrival.Cart";

        public static IList<CartLine> Get(HttpSessionStateBase session)
        {
            var lines = session[Key] as IList<CartLine>;
            if (lines == null)
            {
                lines = new List<CartLine>();
                session[Key] = lines;
            }
            return lines;
        }

        public static decimal Total(HttpSessionStateBase session)
        {
            return Get(session).Sum(x => x.LineTotal);
        }
    }
}
