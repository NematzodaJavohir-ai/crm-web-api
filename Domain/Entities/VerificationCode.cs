namespace Domain.Entities;

public class VerificationCode
{
    public  int Id {get;set;} 
    public string Code {get;set;}=null!;
    public int UserId{get;set;}
    public DateTime Expiration { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public User User { get; set; } = null!;

}
