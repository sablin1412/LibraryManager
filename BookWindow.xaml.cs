using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using LibraryManager.Data;
using LibraryManager.Models;

namespace LibraryManager
{
    public partial class BookWindow : Window
    {
        private LibraryContext _context;
        private Book _currentBook;
        private bool _isEditMode;

        public BookWindow(LibraryContext context, Book book = null)
        {
            InitializeComponent();
            _context = context;
            
            AuthorsListBox.ItemsSource = _context.Authors.ToList();
            GenresListBox.ItemsSource = _context.Genres.ToList();

            if (book != null)
            {
                _isEditMode = true;
                _currentBook = book;
                Title = "Редактирование книги";
                
                TitleTextBox.Text = book.Title;
                YearTextBox.Text = book.PublishYear.ToString();
                IsbnTextBox.Text = book.ISBN;
                QuantityTextBox.Text = book.QuantityInStock.ToString();

                foreach (var author in book.Authors)
                    AuthorsListBox.SelectedItems.Add(author);
                foreach (var genre in book.Genres)
                    GenresListBox.SelectedItems.Add(genre);
            }
            else
            {
                _isEditMode = false;
                _currentBook = new Book();
                Title = "Добавление книги";
            }
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TitleTextBox.Text) || AuthorsListBox.SelectedItems.Count == 0 || GenresListBox.SelectedItems.Count == 0)
            {
                MessageBox.Show("Пожалуйста, введите название, выберите хотя бы одного автора и жанр.");
                return;
            }

            _currentBook.Title = TitleTextBox.Text;
            _currentBook.ISBN = IsbnTextBox.Text ?? "";
            
            if (int.TryParse(YearTextBox.Text, out int year)) _currentBook.PublishYear = year;
            if (int.TryParse(QuantityTextBox.Text, out int quantity)) _currentBook.QuantityInStock = quantity;

            _currentBook.Authors.Clear();
            foreach (Author author in AuthorsListBox.SelectedItems)
                _currentBook.Authors.Add(author);

            _currentBook.Genres.Clear();
            foreach (Genre genre in GenresListBox.SelectedItems)
                _currentBook.Genres.Add(genre);

            if (!_isEditMode) _context.Books.Add(_currentBook);
            else _context.Books.Update(_currentBook);

            _context.SaveChanges();
            DialogResult = true; 
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}