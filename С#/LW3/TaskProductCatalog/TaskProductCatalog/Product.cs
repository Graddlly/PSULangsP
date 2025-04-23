namespace TaskProductCatalog;

using System.Runtime.Serialization;

[Serializable]
public class Product : ISerializable
{
    private int _id;
    private string _name;
    private decimal _price;
    private int _quantity;
    private DateTime _manufacturingDate;
    private bool _isAvailable;
    
    public Product()
    {
        _id = 0;
        _name = "Неизвестный";
        _price = 0;
        _quantity = 0;
        _manufacturingDate = DateTime.Now;
        _isAvailable = false;
    }

    public Product(int id, string name, decimal price, int quantity, 
                    DateTime manufacturingDate, bool isAvailable)
    {
        Id = id;
        Name = name;
        Price = price;
        Quantity = quantity;
        ManufacturingDate = manufacturingDate;
        IsAvailable = isAvailable;
    }
    
    /// <summary>
    /// Конструктор для десериализации
    /// </summary>
    /// <param name="info">Информация для сериализации</param>
    /// <param name="context">Контекст потока</param>
    public Product(SerializationInfo info, StreamingContext context)
    {
        Id = info.GetInt32("Id");
        Name = info.GetString("Name")!;
        Price = info.GetDecimal("Price");
        Quantity = info.GetInt32("Quantity");
        ManufacturingDate = info.GetDateTime("ManufacturingDate");
        IsAvailable = info.GetBoolean("IsAvailable");
    }
    
    public int Id
    {
        get
        {
            return _id;
        }
        set
        {
            _id = value;
        }
    }

    public string Name
    {
        get
        {
            return _name;
        }
        set
        {
            _name = value;
        }
    }

    public decimal Price
    {
        get
        {
            return _price;
        }
        set
        {
            _price = value > 0 ? value : 
                throw new ArgumentException("Цена должна быть положительной");
        }
    }

    public int Quantity
    {
        get
        {
            return _quantity;
        }
        set
        {
            _quantity = value >= 0 ? value : 
                throw new ArgumentException("Количество не может быть отрицательным");
        }
    }

    public DateTime ManufacturingDate
    {
        get
        {
            return _manufacturingDate;
        }
        set
        {
            _manufacturingDate = value;
        }
    }

    public bool IsAvailable
    {
        get
        {
            return _isAvailable;
        }
        set
        {
            _isAvailable = value;
        }
    }
    
    public override string ToString()
    {
        return $"ID: {Id}, Название: {Name}, Цена: {Price:C}, Количество: {Quantity}, " +
               $"Дата производства: {ManufacturingDate.ToShortDateString()}, " + 
               $"Доступен: {(IsAvailable ? "Да" : "Нет")}";
    }
    
    /// <summary>
    /// Метод для сериализации объекта
    /// </summary>
    /// <param name="info">Информация для сериализации</param>
    /// <param name="context">Контекст потока</param>
    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        info.AddValue("Id", Id);
        info.AddValue("Name", Name);
        info.AddValue("Price", Price);
        info.AddValue("Quantity", Quantity);
        info.AddValue("ManufacturingDate", ManufacturingDate);
        info.AddValue("IsAvailable", IsAvailable);
    }
}