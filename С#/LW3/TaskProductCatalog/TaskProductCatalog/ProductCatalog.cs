namespace TaskProductCatalog;

using System.Text.Json;

public class ProductCatalog
{
    private List<Product> _products;
    private string _filePath;

    /// <summary>
    /// Конструктор каталога продукции
    /// </summary>
    /// <param name="filePath">Путь к файлу для хранения данных</param>
    public ProductCatalog(string filePath)
    {
        _filePath = filePath;
        _products = [];
    }

    /// <summary>
    /// Сохраняет базу данных в JSON файл
    /// </summary>
    public void SaveToFile()
    {
        try
        {
            var jsonString = JsonSerializer.Serialize(_products, 
                new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_filePath, jsonString);
            Console.WriteLine("База данных успешно сохранена в файл.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при сохранении файла: {ex.Message}");
            Console.WriteLine("Повторная попытка сохранения...");
            SaveToFile();
        }
    }

    /// <summary>
    /// Загружает базу данных из JSON файла
    /// </summary>
    public void LoadFromFile()
    {
        try
        {
            if (File.Exists(_filePath))
            {
                var jsonString = File.ReadAllText(_filePath);
                _products = JsonSerializer.Deserialize<List<Product>>(jsonString) ?? [];
                Console.WriteLine("База данных успешно загружена из файла.");
            }
            else
            {
                Console.WriteLine("Файл базы данных не найден. Создана новая база данных.");
                _products = [];
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при загрузке файла: {ex.Message}");
            Console.WriteLine("Создана новая база данных.");
            _products = [];
        }
    }

    /// <summary>
    /// Отображает все товары из базы данных
    /// </summary>
    public void DisplayProducts()
    {
        try
        {
            if (_products.Count == 0)
            {
                Console.WriteLine("База данных пуста.");
                return;
            }

            Console.WriteLine("Список товаров:");
            foreach (var product in _products)
            {
                Console.WriteLine(product);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при отображении товаров: {ex.Message}");
            Console.WriteLine("Повторная попытка отображения...");
            DisplayProducts();
        }
    }

    /// <summary>
    /// Добавляет новый товар в базу данных
    /// </summary>
    public void AddProduct()
    {
        try
        {
            Console.WriteLine("Добавление нового товара");
            
            int id;
            if (_products.Count > 0)
                id = _products.Max(p => p.Id) + 1;
            else
                id = 1;
                
            Console.Write("Введите название товара: ");
            var name = Console.ReadLine()!;
            
            Console.Write("Введите цену товара: ");
            var price = decimal.Parse(Console.ReadLine()!);
            
            Console.Write("Введите количество товара: ");
            var quantity = int.Parse(Console.ReadLine()!);
            
            Console.Write("Введите дату производства (дд.мм.гггг): ");
            var manufacturingDate = DateTime.Parse(Console.ReadLine()!);
            
            Console.Write("Товар доступен (да/нет): ");
            var isAvailable = Console.ReadLine()?.ToLower() == "да";
            
            var newProduct = new Product(id, name, price, quantity, manufacturingDate, isAvailable);
            _products.Add(newProduct);
            
            Console.WriteLine("Товар успешно добавлен.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при добавлении товара: {ex.Message}");
            Console.WriteLine("Повторите попытку добавления товара.");
            AddProduct();
        }
    }

    /// <summary>
    /// Удаляет товар из базы данных по указанному идентификатору
    /// </summary>
    public void RemoveProduct()
    {
        try
        {
            if (_products.Count == 0)
            {
                Console.WriteLine("База данных пуста.");
                return;
            }
            
            Console.Write("Введите ID товара для удаления: ");
            var id = int.Parse(Console.ReadLine()!);
            
            var productToRemove = _products.FirstOrDefault(p => p.Id == id);
            
            if (productToRemove != null)
            {
                _products.Remove(productToRemove);
                Console.WriteLine("Товар успешно удален.");
            }
            else
            {
                Console.WriteLine("Товар с указанным ID не найден.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при удалении товара: {ex.Message}");
            Console.WriteLine("Повторите попытку удаления товара.");
            RemoveProduct();
        }
    }

    /// <summary>
    /// Запрос: Возвращает список товаров с ценой выше указанной
    /// </summary>
    public void QueryProductsAbovePrice()
    {
        try
        {
            Console.Write("Введите минимальную цену: ");
            var minPrice = decimal.Parse(Console.ReadLine()!);
            
            var result = _products.Where(p => p.Price > minPrice).ToList();
            
            if (result.Count > 0)
            {
                Console.WriteLine($"Товары с ценой выше {minPrice:C}:");
                foreach (var product in result)
                {
                    Console.WriteLine(product);
                }
            }
            else
            {
                Console.WriteLine($"Товары с ценой выше {minPrice:C} не найдены.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при выполнении запроса: {ex.Message}");
            Console.WriteLine("Повторите запрос.");
            QueryProductsAbovePrice();
        }
    }

    /// <summary>
    /// Запрос: Возвращает список доступных товаров, отсортированных по цене
    /// </summary>
    public void QueryAvailableProductsSortedByPrice()
    {
        try
        {
            var result = _products.Where(p => p.IsAvailable)
                                  .OrderBy(p => p.Price)
                                  .ToList();
            
            if (result.Count > 0)
            {
                Console.WriteLine("Доступные товары, отсортированные по цене:");
                foreach (var product in result)
                {
                    Console.WriteLine(product);
                }
            }
            else
            {
                Console.WriteLine("Доступные товары не найдены.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при выполнении запроса: {ex.Message}");
            Console.WriteLine("Повторите запрос.");
            QueryAvailableProductsSortedByPrice();
        }
    }

    /// <summary>
    /// Запрос: Возвращает общую стоимость всех товаров в наличии
    /// </summary>
    public void QueryTotalInventoryValue()
    {
        try
        {
            var totalValue = _products.Sum(p => p.Price * p.Quantity);
            Console.WriteLine($"Общая стоимость всех товаров: {totalValue:C}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при выполнении запроса: {ex.Message}");
            Console.WriteLine("Повторите запрос.");
            QueryTotalInventoryValue();
        }
    }

    /// <summary>
    /// Запрос: Возвращает количество товаров, произведенных после указанной даты
    /// </summary>
    public void QueryProductsCountAfterDate()
    {
        try
        {
            Console.Write("Введите дату (дд.мм.гггг): ");
            var date = DateTime.Parse(Console.ReadLine()!);
            
            var count = _products.Count(p => p.ManufacturingDate > date);
            
            Console.WriteLine($"Количество товаров, произведенных после {date.ToShortDateString()}: {count}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при выполнении запроса: {ex.Message}");
            Console.WriteLine("Повторите запрос.");
            QueryProductsCountAfterDate();
        }
    }
}