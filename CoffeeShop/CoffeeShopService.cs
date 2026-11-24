
namespace CoffeeShop
{
    public class CoffeeShopService
    {
        public Dictionary<int, (string Name, int Price)> Drinks { get; } = new()
        {
            { 1, ("Эспрессо", 50) }, { 2, ("Американо", 60) },
            { 3, ("Капучино", 80) }, { 4, ("Латте", 85) },
            { 5, ("Чай (чёрный/зелёный)", 40) }
        };

        public Dictionary<int, (string Name, int Price)> Ingredients { get; } = new()
        {
            { 1, ("Молоко", 10) }, { 2, ("Сахар", 0) },
            { 3, ("Сироп ванильный", 15) }, { 4, ("Сироп карамельный", 15) },
            { 5, ("Взбитые сливки", 20) }, { 6, ("Корица", 5) }
        };

        public int TotalRevenue { get; private set; } = 0;
        
        public bool IsValidDrink(int drinkId)
        {
            return Drinks.ContainsKey(drinkId);
        }
        
        public bool CanAddIngredient(int ingredientId, HashSet<int> addedIngredients)
        {
            return Ingredients.ContainsKey(ingredientId) && !addedIngredients.Contains(ingredientId);
        }
        
        public int CalculateTotal(int drinkId, HashSet<int> addedIngredients)
        {
            int total = Drinks[drinkId].Price;
            foreach (var ingId in addedIngredients)
            {
                total += Ingredients[ingId].Price;
            }
            return total;
        }
        
        public (bool IsSuccess, string Message, int Change) ProcessPayment(int orderTotal, int cash)
        {
            if (cash < orderTotal)
            {
                return (false, "Недостаточно средств", 0);
            }

            int change = cash - orderTotal;
            TotalRevenue += orderTotal;

            return (true, "Спасибо за покупку!", change);
        }
    }
}