using System.ComponentModel.DataAnnotations;

public class User
{
    [Key]
    public int Id { get; set; }          
    public string Username { get; set; } = "";
}