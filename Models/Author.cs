using System;
using System.Collections.Generic;

namespace LibraryManager.Models
{
    public class Author
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public string Country { get; set; }

        // Навигационное свойство для связи "один-ко-многим"
        public List<Book> Books { get; set; } = new List<Book>();
    }
}