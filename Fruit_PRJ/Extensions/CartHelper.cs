using Fruit_PRJ.Models;

namespace Fruit_PRJ.Extensions
{
    public static class CartHelper
    {
        public static void Add(ISession session, Product product, int qty)
        {
            var cart = session.GetObject<List<CartItem>>("CART") ?? new();
            var item = cart.FirstOrDefault(x => x.ProductId == product.Id);

            if (item == null)
            {
                cart.Add(new CartItem
                {
                    ProductId = product.Id,
                    Quantity = qty,
                    Name = product.Name,
                    Price = product.Price,
                    Image = product.ProductImages.FirstOrDefault()?.ImageUrl ?? "/no-image.png"
                });
            }
            else
            {
                item.Quantity += qty;
            }

            session.SetObject("CART", cart);
        }
    }

}
