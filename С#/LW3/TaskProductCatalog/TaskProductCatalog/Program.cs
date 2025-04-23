namespace TaskProductCatalog;

class Program
{
    private static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        
        const string FILE_PATH = "products.bin";
        var catalog = new ProductCatalog(FILE_PATH);
        catalog.LoadFromFile();
        
        var exit = false;
        
        while (!exit)
        {
            try
            {
                Console.WriteLine("\n===== Каталог продукции =====");
                Console.WriteLine("1. Просмотр базы данных");
                Console.WriteLine("2. Добавление товара");
                Console.WriteLine("3. Удаление товара");
                Console.WriteLine("4. Запрос: товары с ценой выше указанной");
                Console.WriteLine("5. Запрос: доступные товары, отсортированные по цене");
                Console.WriteLine("6. Запрос: общая стоимость всех товаров");
                Console.WriteLine("7. Запрос: количество товаров, произведенных после указанной даты");
                Console.WriteLine("8. Сохранить и выйти");
                Console.Write("\nВыберите действие (1-8): ");
                
                var choice = Console.ReadLine()!;
                
                Console.WriteLine();
                
                switch (choice)
                {
                    case "1":
                        catalog.DisplayProducts();
                        break;
                    case "2":
                        catalog.AddProduct();
                        break;
                    case "3":
                        catalog.RemoveProduct();
                        break;
                    case "4":
                        catalog.QueryProductsAbovePrice();
                        break;
                    case "5":
                        catalog.QueryAvailableProductsSortedByPrice();
                        break;
                    case "6":
                        catalog.QueryTotalInventoryValue();
                        break;
                    case "7":
                        catalog.QueryProductsCountAfterDate();
                        break;
                    case "8":
                        catalog.SaveToFile();
                        exit = true;
                        break;
                    default:
                        Console.WriteLine("Неверный выбор. Пожалуйста, выберите число от 1 до 8.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Произошла общая ошибка: {ex.Message}");
                Console.WriteLine("Программа перезапустит текущую операцию.");
            }
            
            if (!exit)
            {
                Console.WriteLine("\nНажмите любую клавишу для продолжения...");
                Console.ReadKey();
                Console.Clear();
            }
        }
        
        Console.WriteLine("Программа завершена.");
    }
}