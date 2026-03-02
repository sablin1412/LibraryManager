using System.Linq;
using System.Windows;
using System.Windows.Controls;
using LibraryManager.Data;
using LibraryManager.Models;

namespace LibraryManager
{
    public partial class GenresWindow : Window
    {
        private LibraryContext _context;
        private Genre _selectedGenre;

        public GenresWindow()
        {
            InitializeComponent();
            _context = new LibraryContext();
            LoadGenres();
        }

        private void LoadGenres()
        {
            GenresDataGrid.ItemsSource = _context.Genres.ToList();
        }

        // При клике на жанр в таблице, заполняем поля для редактирования
        private void GenresDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (GenresDataGrid.SelectedItem is Genre genre)
            {
                _selectedGenre = genre;
                NameTextBox.Text = genre.Name;
                DescriptionTextBox.Text = genre.Description;
            }
        }

        private void AddGenre_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameTextBox.Text))
            {
                MessageBox.Show("Введите название жанра.");
                return;
            }

            var newGenre = new Genre
            {
                Name = NameTextBox.Text,
                Description = DescriptionTextBox.Text
            };

            _context.Genres.Add(newGenre);
            _context.SaveChanges();
            
            ClearInputs();
            LoadGenres();
        }

        private void EditGenre_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedGenre == null)
            {
                MessageBox.Show("Выберите жанр для редактирования.");
                return;
            }

            _selectedGenre.Name = NameTextBox.Text;
            _selectedGenre.Description = DescriptionTextBox.Text;

            _context.SaveChanges();
            
            ClearInputs();
            LoadGenres();
        }

        private void DeleteGenre_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedGenre == null) return;

            var result = MessageBox.Show($"Удалить жанр '{_selectedGenre.Name}'? Это также удалит все книги этого жанра!", 
                                         "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            
            if (result == MessageBoxResult.Yes)
            {
                _context.Genres.Remove(_selectedGenre);
                _context.SaveChanges();
                
                ClearInputs();
                LoadGenres();
            }
        }

        private void ClearInputs()
        {
            _selectedGenre = null;
            NameTextBox.Clear();
            DescriptionTextBox.Clear();
            GenresDataGrid.SelectedItem = null;
        }
    }
}