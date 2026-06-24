using System.ComponentModel.DataAnnotations;

namespace MessengerGui.Models
{
    public class User
    {
        [Key] // Тепер компілятор точно знає, що це таке
        public int Id { get; set; }          
        public string Username { get; set; } = "";
    }
}