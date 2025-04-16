namespace Task1_10;

/// <summary>
/// Класс, представляющий игрушку.
/// </summary>
[Serializable]
public class Toy
{
    private string _name;
    private int _price;
    private int _minAge;
    private int _maxAge;

    public string Name
    {
        get { return _name; }
        set { _name = value; }
    }

    public int Price
    {
        get { return _price; }
        set { _price = value; }
    }

    public int MinAge
    {
        get { return _minAge; }
        set { _minAge = value; }
    }

    public int MaxAge
    {
        get { return _maxAge; }
        set { _maxAge = value; }
    }
}