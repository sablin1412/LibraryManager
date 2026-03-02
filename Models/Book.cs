namespace LibraryManager.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int PublishYear { get; set; }
        public string ISBN { get; set; }
        public int QuantityInStock { get; set; }

        // Внешние ключи и навигационные свойства
        public int AuthorId { get; set; }
        public Author Author { get; set; } 

        public int GenreId { get; set; }
        public Genre Genre { get; set; } 
    }
}