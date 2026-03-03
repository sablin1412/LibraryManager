using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using LibraryManager.Data;
using LibraryManager.Models;

namespace LibraryManager
{
    public partial class AuthorsWindow : Window
    {
        private LibraryContext _context;
        private Author _selectedAuthor;

        public AuthorsWindow()
        {
            InitializeComponent();
            _context = new LibraryContext();
            LoadAuthors();
        }

        private void LoadAuthors() => AuthorsDataGrid.ItemsSource = _context.Authors.ToList();

        private void AuthorsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AuthorsDataGrid.SelectedItem is Author author)
            {
                _selectedAuthor = author;
                FirstNameTextBox.Text = author.FirstName;
                LastNameTextBox.Text = author.LastName;
                CountryTextBox.Text = author.Country;
                BirthDatePicker.SelectedDate = author.BirthDate;
            }
        }

        private void AddAuthor_Click(object sender, RoutedEventArgs e)
        {
            var firstName = FirstNameTextBox.Text.Trim();
            var lastName = LastNameTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName)) return;

            // Строгая проверка на дубликат (Иван Иванов == иван иванов)
            bool exists = _context.Authors.ToList().Any(a => 
                a.FirstName.ToLower() == firstName.ToLower() && 
                a.LastName.ToLower() == lastName.ToLower());
            
            if (exists)
            {
                MessageBox.Show("Такой автор уже добавлен!");
                return;
            }

            _context.Authors.Add(new Author { 
                FirstName = firstName, 
                LastName = lastName, 
                Country = CountryTextBox.Text, 
                BirthDate = BirthDatePicker.SelectedDate ?? DateTime.Now 
            });
            _context.SaveChanges();
            ClearInputs();
            LoadAuthors();
        }

        private void EditAuthor_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedAuthor == null) return;
            _selectedAuthor.FirstName = FirstNameTextBox.Text.Trim();
            _selectedAuthor.LastName = LastNameTextBox.Text.Trim();
            _selectedAuthor.Country = CountryTextBox.Text;
            _selectedAuthor.BirthDate = BirthDatePicker.SelectedDate ?? DateTime.Now;
            _context.SaveChanges();
            ClearInputs();
            LoadAuthors();
        }

        private void DeleteAuthor_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedAuthor != null && MessageBox.Show("Удалить автора?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                _context.Authors.Remove(_selectedAuthor);
                _context.SaveChanges();
                ClearInputs();
                LoadAuthors();
            }
        }

        private void ClearInputs()
        {
            _selectedAuthor = null;
            FirstNameTextBox.Clear();
            LastNameTextBox.Clear();
            CountryTextBox.Clear();
            BirthDatePicker.SelectedDate = null;
            AuthorsDataGrid.SelectedItem = null;
        }
    }
}