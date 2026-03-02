using System.Linq;
using System.Windows;
using LibraryManager.Data;
using LibraryManager.Models;

namespace LibraryManager
{
    public partial class BookWindow : Window
    {
        private LibraryContext _context;
        private Book _currentBook;
        private bool _isEditMode;

        // В конструктор можно передать книгу. Если передали null, значит добавляем новую.
        public BookWindow(Book book = null)
        {
            InitializeComponent();
            _context = new LibraryContext();
            
            // Загружаем авторов и жанры для выпадающих списков
            AuthorComboBox.ItemsSource = _context.Authors.ToList();
            GenreComboBox.ItemsSource = _context.Genres.ToList();

            if (book != null)
            {
                _isEditMode = true;
                _currentBook = book;
                Title = "Редактирование книги";
                
                // Заполняем поля старыми данными
                TitleTextBox.Text = book.Title;
                AuthorComboBox.SelectedValue = book.AuthorId;
                GenreComboBox.SelectedValue = book.GenreId;
                YearTextBox.Text = book.PublishYear.ToString();
                IsbnTextBox.Text = book.ISBN;
                QuantityTextBox.Text = book.QuantityInStock.ToString();
            }
            else
            {
                _isEditMode = false;
                _currentBook = new Book();
                Title = "Добавление книги";
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            // Проверка, что обязательные поля заполнены
            if (string.IsNullOrWhiteSpace(TitleTextBox.Text) || 
                AuthorComboBox.SelectedValue == null || 
                GenreComboBox.SelectedValue == null)
            {
                MessageBox.Show("Пожалуйста, введите название, выберите автора и жанр.");
                return;
            }

            // Переносим данные из формы в объект книги
            _currentBook.Title = TitleTextBox.Text;
            _currentBook.AuthorId = (int)AuthorComboBox.SelectedValue;
            _currentBook.GenreId = (int)GenreComboBox.SelectedValue;
            _currentBook.ISBN = IsbnTextBox.Text ?? "";
            
            // Пробуем безопасно перевести текст в числа
            if (int.TryParse(YearTextBox.Text, out int year))
                _currentBook.PublishYear = year;
                
            if (int.TryParse(QuantityTextBox.Text, out int quantity))
                _currentBook.QuantityInStock = quantity;

            // Если создаем новую, добавляем в базу. Если редактируем, обновляем.
            if (!_isEditMode)
            {
                _context.Books.Add(_currentBook);
            }
            else
            {
                _context.Books.Update(_currentBook);
            }

            _context.SaveChanges();
            
            // Закрываем окно с сигналом успешного завершения
            DialogResult = true; 
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            // Просто закрываем окно без сохранения
            DialogResult = false;
        }
    }
}