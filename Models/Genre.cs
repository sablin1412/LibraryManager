using System.Collections.Generic;

namespace LibraryManager.Models
{
    public class Genre
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        // Навигационное свойство для связи "один-ко-многим"
        public List<Book> Books { get; set; } = new List<Book>();
    }
}