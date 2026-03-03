using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using LibraryManager.Data;
using LibraryManager.Models;

namespace LibraryManager
{
    public partial class MainWindow : Window
    {
        private LibraryContext _context;

        public MainWindow()
        {
            InitializeComponent();
            _context = new LibraryContext();
            LoadData();
        }

        private void LoadData()
        {
            _context.Books.Include(b => b.Authors).Include(b => b.Genres).Load();
            _context.Authors.Load();
            _context.Genres.Load();

            var allAuthors = _context.Authors.ToList();
            allAuthors.Insert(0, new Author { Id = 0, LastName = "Все авторы" });
            AuthorFilterComboBox.ItemsSource = allAuthors;
            AuthorFilterComboBox.SelectedIndex = 0;

            var allGenres = _context.Genres.ToList();
            allGenres.Insert(0, new Genre { Id = 0, Name = "Все жанры" });
            GenreFilterComboBox.ItemsSource = allGenres;
            GenreFilterComboBox.SelectedIndex = 0;

            UpdateBooksGrid();
        }

        private void UpdateBooksGrid()
        {
            if (_context == null) return;

            var query = _context.Books.Include(b => b.Authors).Include(b => b.Genres).AsQueryable();

            var selectedAuthor = AuthorFilterComboBox.SelectedItem as Author;
            if (selectedAuthor != null && selectedAuthor.Id != 0)
            {
                query = query.Where(b => b.Authors.Any(a => a.Id == selectedAuthor.Id));
            }

            var selectedGenre = GenreFilterComboBox.SelectedItem as Genre;
            if (selectedGenre != null && selectedGenre.Id != 0)
            {
                query = query.Where(b => b.Genres.Any(g => g.Id == selectedGenre.Id));
            }

            var books = query.AsEnumerable();

            var searchText = SearchTextBox.Text?.Trim();
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                books = books.Where(b => b.Title.Contains(searchText, System.StringComparison.OrdinalIgnoreCase));
            }

            var finalList = books.ToList();
            BooksDataGrid.ItemsSource = finalList;

            int totalStock = finalList.Sum(b => b.QuantityInStock);
            BooksCountTextBlock.Text = $"Всего наименований: {finalList.Count} | Штук в наличии: {totalStock}";
        }

        private void Search_Click(object sender, RoutedEventArgs e) => UpdateBooksGrid();
        private void Filter_SelectionChanged(object sender, SelectionChangedEventArgs e) => UpdateBooksGrid();

        private void ResetFilters_Click(object sender, RoutedEventArgs e)
        {
            SearchTextBox.Clear();
            AuthorFilterComboBox.SelectedIndex = 0;
            GenreFilterComboBox.SelectedIndex = 0;
            UpdateBooksGrid();
        }

        private void DeleteBook_Click(object sender, RoutedEventArgs e)
        {
            if (BooksDataGrid.SelectedItem is Book selectedBook)
            {
                if (MessageBox.Show($"Удалить книгу '{selectedBook.Title}'?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    _context.Books.Remove(selectedBook);
                    _context.SaveChanges();
                    UpdateBooksGrid();
                }
            }
        }

        private void AddBook_Click(object sender, RoutedEventArgs e)
        {
            var bookWindow = new BookWindow(_context);
            bookWindow.Owner = this;
            if (bookWindow.ShowDialog() == true) LoadData();
        }

        private void EditBook_Click(object sender, RoutedEventArgs e)
        {
            if (BooksDataGrid.SelectedItem is Book selectedBook)
            {
                var bookWindow = new BookWindow(_context, selectedBook);
                bookWindow.Owner = this;
                if (bookWindow.ShowDialog() == true) LoadData();
            }
            else
            {
                MessageBox.Show("Сначала выберите книгу в таблице для редактирования.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ManageAuthors_Click(object sender, RoutedEventArgs e)
        {
            new AuthorsWindow().ShowDialog();
            LoadData();
        }

        private void ManageGenres_Click(object sender, RoutedEventArgs e)
        {
            new GenresWindow().ShowDialog();
            LoadData();
        }
    }
}