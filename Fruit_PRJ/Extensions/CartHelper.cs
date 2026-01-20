using Fruit_PRJ.Models;

namespace Fruit_PRJ.Extensions
{
    public static class CartHelper
    {
        public static void Add(ISession session, int id, int qty)
        {
            var cart = session.GetObject<List<CartItem>>("CART") ?? new();

            var item = cart.FirstOrDefault(x => x.ProductId == id);

            if (item == null)
                cart.Add(new CartItem { ProductId = id, Quantity = qty });
            else
                item.Quantity += qty;

            session.SetObject("CART", cart);
        }
    }

}
