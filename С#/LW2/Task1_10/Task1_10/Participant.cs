namespace Task1_10;

/// <summary>
/// Класс, представляющий участника олимпиады.
/// </summary>
public class Participant
{
    private string _lastName;
    private string _firstName;
    private int _grade;
    private int _score;
    
    /// <summary>
    /// Создает нового участника олимпиады.
    /// </summary>
    /// <param name="lastName">Фамилия.</param>
    /// <param name="firstName">Имя.</param>
    /// <param name="grade">Класс.</param>
    /// <param name="score">Баллы.</param>
    public Participant(string lastName, string firstName, int grade, int score)
    {
        LastName = lastName;
        FirstName = firstName;
        Grade = grade;
        Score = score;
    }

    public string LastName
    {
        get { return _lastName; }
        private set { _lastName = value; }
    }

    public string FirstName
    {
        get { return _firstName; }
        private set { _firstName = value; }
    }

    public int Grade
    {
        get { return _grade; }
        private set { _grade = value; }
    }

    public int Score
    {
        get { return _score; }
        private set { _score = value; }
    }
}