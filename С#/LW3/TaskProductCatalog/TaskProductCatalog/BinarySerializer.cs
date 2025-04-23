namespace TaskProductCatalog;

/// <summary>
/// Класс для сериализации/десериализации списка продуктов в бинарный файл
/// </summary>
public class BinarySerializer
{
    /// <summary>
    /// Сериализует список продуктов в бинарный файл
    /// </summary>
    /// <param name="products">Список продуктов</param>
    /// <param name="filePath">Путь к файлу</param>
    public static void SerializeToFile(List<Product> products, string filePath)
    {
        using (var fs = new FileStream(filePath, FileMode.Create))
        {
            using (var writer = new BinaryWriter(fs))
            {
                writer.Write(products.Count);
                
                foreach (var product in products)
                {
                    writer.Write(product.Id);
                    writer.Write(product.Name);
                    writer.Write(product.Price);
                    writer.Write(product.Quantity);
                    writer.Write(product.ManufacturingDate.ToBinary());
                    writer.Write(product.IsAvailable);
                }
            }
        }
    }

    /// <summary>
    /// Десериализует список продуктов из бинарного файла
    /// </summary>
    /// <param name="filePath">Путь к файлу</param>
    /// <returns>Список продуктов</returns>
    public static List<Product> DeserializeFromFile(string filePath)
    {
        var products = new List<Product>();
        
        if (!File.Exists(filePath))
            return products;
            
        using (var fs = new FileStream(filePath, FileMode.Open))
        {
            using (var reader = new BinaryReader(fs))
            {
                var count = reader.ReadInt32();
                
                for (var i = 0; i < count; i++)
                {
                    var id = reader.ReadInt32();
                    var name = reader.ReadString();
                    var price = reader.ReadDecimal();
                    var quantity = reader.ReadInt32();
                    var manufacturingDate = DateTime.FromBinary(reader.ReadInt64());
                    var isAvailable = reader.ReadBoolean();
                    
                    products.Add(new Product(id, name, price, quantity, manufacturingDate, isAvailable));
                }
            }
        }
        
        return products;
    }
}