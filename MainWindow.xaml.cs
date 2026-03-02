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
            _context.Books.Include(b => b.Author).Include(b => b.Genre).Load();
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

            var query = _context.Books.Include(b => b.Author).Include(b => b.Genre).AsQueryable();

            var selectedAuthor = AuthorFilterComboBox.SelectedItem as Author;
            if (selectedAuthor != null && selectedAuthor.Id != 0)
            {
                query = query.Where(b => b.AuthorId == selectedAuthor.Id);
            }

            var selectedGenre = GenreFilterComboBox.SelectedItem as Genre;
            if (selectedGenre != null && selectedGenre.Id != 0)
            {
                query = query.Where(b => b.GenreId == selectedGenre.Id);
            }

            var books = query.ToList();
            BooksDataGrid.ItemsSource = books;
            
            int totalStock = books.Sum(b => b.QuantityInStock);
            BooksCountTextBlock.Text = $"Всего наименований: {books.Count} | Штук в наличии: {totalStock}";
        }

        private void Filter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateBooksGrid();
        }

        private void ResetFilters_Click(object sender, RoutedEventArgs e)
        {
            AuthorFilterComboBox.SelectedIndex = 0;
            GenreFilterComboBox.SelectedIndex = 0;
            UpdateBooksGrid();
        }

        private void DeleteBook_Click(object sender, RoutedEventArgs e)
        {
            if (BooksDataGrid.SelectedItem is Book selectedBook)
            {
                var result = MessageBox.Show($"Вы уверены, что хотите удалить книгу '{selectedBook.Title}'?", 
                                             "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    _context.Books.Remove(selectedBook);
                    _context.SaveChanges(); 
                    UpdateBooksGrid();      
                }
            }
            else
            {
                MessageBox.Show("Сначала выберите книгу в таблице для удаления.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        
        private void AddBook_Click(object sender, RoutedEventArgs e)
        {
            // Открываем окно создания (не передаем книгу)
            var bookWindow = new BookWindow();
            bookWindow.Owner = this;
            
            // Если окно закрылось по кнопке "Сохранить", обновляем таблицу
            if (bookWindow.ShowDialog() == true)
            {
                LoadData();
            }
        }

        private void EditBook_Click(object sender, RoutedEventArgs e)
        {
            // Проверяем, выделена ли книга в таблице
            if (BooksDataGrid.SelectedItem is Book selectedBook)
            {
                // Открываем окно и передаем в него выбранную книгу
                var bookWindow = new BookWindow(selectedBook);
                bookWindow.Owner = this;
                
                if (bookWindow.ShowDialog() == true)
                {
                    LoadData();
                }
            }
            else
            {
                MessageBox.Show("Сначала выберите книгу в таблице для редактирования.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ManageAuthors_Click(object sender, RoutedEventArgs e)
        {
            var authorsWindow = new AuthorsWindow();
            authorsWindow.Owner = this;
            authorsWindow.ShowDialog();
            
            // После закрытия окна авторов обновляем данные в главном окне
            LoadData(); 
        }

        private void ManageGenres_Click(object sender, RoutedEventArgs e)
        {
            // Создаем и открываем окно жанров
            var genresWindow = new GenresWindow();
            genresWindow.Owner = this; // Делаем главное окно родителем
            genresWindow.ShowDialog(); // ShowDialog блокирует главное окно, пока открыто это
            
            // Когда окно жанров закроется, обновляем списки в главном окне (вдруг мы добавили новый жанр)
            LoadData(); 
        }
    }
}