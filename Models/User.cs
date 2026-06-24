using System.ComponentModel.DataAnnotations;

namespace MessengerGui.Models
{
    public class User
    {
        [Key] 
        public int Id { get; set; }          
        public string Username { get; set; } = "";
    }
}