namespace SyriaNews.Abstractions;

public class Error
{
    public static Error None = new Error(string.Empty, string.Empty, null);
    
    public string Code { get; }
    public string Description { get; }
    public int? StatuCode { get; } = null;

    public Error(string code, string description, int? statuCode)
    {
        Code = code;
        Description = description;
        StatuCode = statuCode;
    }
}
