using System.Collections.Generic;
using System.Linq;

namespace LibraryManager.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int PublishYear { get; set; }
        public string ISBN { get; set; }
        public int QuantityInStock { get; set; }

        // Теперь тут списки (многие-ко-многим)
        public List<Author> Authors { get; set; } = new List<Author>();
        public List<Genre> Genres { get; set; } = new List<Genre>();

        // Вспомогательные свойства для красивого вывода в таблицу
        public string AuthorsString => string.Join(", ", Authors.Select(a => a.LastName));
        public string GenresString => string.Join(", ", Genres.Select(g => g.Name));
    }
}