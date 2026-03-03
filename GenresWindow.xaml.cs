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

        private void LoadGenres() => GenresDataGrid.ItemsSource = _context.Genres.ToList();

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
            var name = NameTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(name)) return;

            // Строгая проверка на дубликат (Роман == роман)
            bool exists = _context.Genres.ToList().Any(g => g.Name.ToLower() == name.ToLower());
            if (exists)
            {
                MessageBox.Show("Такой жанр уже существует!");
                return;
            }

            _context.Genres.Add(new Genre { Name = name, Description = DescriptionTextBox.Text });
            _context.SaveChanges();
            ClearInputs();
            LoadGenres();
        }

        private void EditGenre_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedGenre == null) return;
            _selectedGenre.Name = NameTextBox.Text.Trim();
            _selectedGenre.Description = DescriptionTextBox.Text;
            _context.SaveChanges();
            ClearInputs();
            LoadGenres();
        }

        private void DeleteGenre_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedGenre != null && MessageBox.Show("Удалить жанр?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
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