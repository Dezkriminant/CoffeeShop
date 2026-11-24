using System;
using System.Collections.Generic;
using System.Linq;

namespace CoffeeShop
{
    class Program
    {
        static CoffeeShopService shop = new CoffeeShopService();

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== ГЛАВНОЕ МЕНЮ ===");
                Console.WriteLine("1. Сделать заказ");
                Console.WriteLine("2. Показать текущую выручку");
                Console.WriteLine("3. Выйти");
                Console.Write("Выберите действие: ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        MakeOrder();
                        break;
                    case "2":
                        Console.WriteLine($"\nТекущая выручка за смену: {shop.TotalRevenue} руб.");
                        Console.WriteLine("Нажмите любую клавишу для возврата...");
                        Console.ReadKey();
                        break;
                    case "3":
                        return;
                    default:
                        Console.WriteLine("Неверный ввод. Нажмите любую клавишу...");
                        Console.ReadKey();
                        break;
                }
            }
        }

        static void MakeOrder()
        {
            Console.Clear();
            Console.WriteLine("=== ВЫБОР НАПИТКА ===");
            
            foreach (var drink in shop.Drinks)
            {
                Console.WriteLine($"{drink.Key}. {drink.Value.Name} - {drink.Value.Price} руб.");
            }
            Console.WriteLine("0. Отменить заказ и выйти в меню");
            Console.Write("Выберите напиток: ");

            if (!int.TryParse(Console.ReadLine(), out int drinkId) || drinkId == 0)
            {
                Console.WriteLine("Заказ отменён.");
                return;
            }
            
            if (!shop.IsValidDrink(drinkId))
            {
                Console.WriteLine("Такого напитка нет. Заказ отменён.");
                return;
            }

            var selectedDrink = shop.Drinks[drinkId];
            var addedIngredients = new HashSet<int>(); 
            while (true)
            {
                Console.Clear();
                Console.WriteLine($"Выбран напиток: {selectedDrink.Name} ({selectedDrink.Price} руб.)");
                
                if (addedIngredients.Count > 0)
                {
                    var addedNames = addedIngredients.Select(id => shop.Ingredients[id].Name);
                    Console.WriteLine("Добавлено: " + string.Join(", ", addedNames));
                }

                Console.WriteLine("\nДоступные дополнительные ингредиенты:");
                foreach (var ing in shop.Ingredients)
                {
                    if (addedIngredients.Contains(ing.Key))
                        Console.WriteLine($"{ing.Key}. {ing.Value.Name} (уже добавлено)");
                    else
                        Console.WriteLine($"{ing.Key}. {ing.Value.Name} (+{ing.Value.Price} руб.)");
                }

                Console.WriteLine("\n7. Подтвердить заказ");
                Console.WriteLine("0. Отменить заказ и выйти в меню");
                Console.Write("Выберите ингредиент: ");

                string input = Console.ReadLine();

                if (input == "0")
                {
                    Console.WriteLine("Заказ отменён.");
                    return;
                }
                
                if (input == "7")
                {


                    break;
                }

                if (int.TryParse(input, out int ingId))
                {
                    if (!shop.CanAddIngredient(ingId, addedIngredients))
                    {
                        Console.WriteLine("Ошибка: Этот ингредиент уже добавлен или не существует!");
                        Console.WriteLine("Нажмите любую клавишу...");
                        Console.ReadKey();
                    }
                    else
                    {
                        addedIngredients.Add(ingId);
                    }
                }
                else
                {
                    Console.WriteLine("Неверный ввод. Нажмите любую клавишу...");
                    Console.ReadKey();
                }
            }
            
            int orderTotal = shop.CalculateTotal(drinkId, addedIngredients);
            
            Console.Clear();
            Console.WriteLine("=== ЧЕК ===");
            Console.WriteLine($"Напиток: {selectedDrink.Name} - {selectedDrink.Price} руб.");
            
            foreach (var ingId in addedIngredients)
            {
                var ing = shop.Ingredients[ingId];
                Console.WriteLine($"+ {ing.Name} - {ing.Price} руб.");
            }
            
            Console.WriteLine("--------------------");
            Console.WriteLine($"Итого: {orderTotal} руб.");
            
            while (true)
            {
                Console.Write("\nВведите сумму наличными (или 0 для отмены): ");
                if (!int.TryParse(Console.ReadLine(), out int cash) || cash < 0)
                {
                    Console.WriteLine("Пожалуйста, введите корректное целое число.");
                    continue;
                }

                if (cash == 0)
                {
                    Console.WriteLine("Оплата отменена. Заказ аннулирован.");
                    Console.ReadKey();
                    return;
                }
                
                var paymentResult = shop.ProcessPayment(orderTotal, cash);

                if (!paymentResult.IsSuccess)
                {
                    Console.WriteLine($"Ошибка: «{paymentResult.Message}».");
                }
                else
                {
                    Console.WriteLine($"«{paymentResult.Message}»");
                    
                    if (paymentResult.Change > 0)
                    {
                        Console.WriteLine($"Ваша сдача: {paymentResult.Change} руб.");
                    }
                    break;
                }
            }

            Console.WriteLine("\nНажмите любую клавишу для возврата в главное меню...");
            Console.ReadKey();
        }
    }
}