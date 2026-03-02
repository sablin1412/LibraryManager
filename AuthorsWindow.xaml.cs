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

        private void LoadAuthors()
        {
            // Выгружаем всех авторов из БД
            AuthorsDataGrid.ItemsSource = _context.Authors.ToList();
        }

        // Заполнение полей при клике на автора в таблице
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
            if (string.IsNullOrWhiteSpace(FirstNameTextBox.Text) || string.IsNullOrWhiteSpace(LastNameTextBox.Text))
            {
                MessageBox.Show("Имя и фамилия обязательны для заполнения!");
                return;
            }

            var newAuthor = new Author
            {
                FirstName = FirstNameTextBox.Text,
                LastName = LastNameTextBox.Text,
                Country = CountryTextBox.Text,
                // Если дата не выбрана, ставим сегодняшнюю
                BirthDate = BirthDatePicker.SelectedDate ?? DateTime.Now 
            };

            _context.Authors.Add(newAuthor);
            _context.SaveChanges();
            
            ClearInputs();
            LoadAuthors();
        }

        private void EditAuthor_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedAuthor == null)
            {
                MessageBox.Show("Сначала выберите автора в таблице.");
                return;
            }

            _selectedAuthor.FirstName = FirstNameTextBox.Text;
            _selectedAuthor.LastName = LastNameTextBox.Text;
            _selectedAuthor.Country = CountryTextBox.Text;
            _selectedAuthor.BirthDate = BirthDatePicker.SelectedDate ?? DateTime.Now;

            _context.SaveChanges();
            
            ClearInputs();
            LoadAuthors();
        }

        private void DeleteAuthor_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedAuthor == null) return;

            var result = MessageBox.Show($"Удалить автора '{_selectedAuthor.FirstName} {_selectedAuthor.LastName}'? Это также удалит все его книги!", 
                                         "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            
            if (result == MessageBoxResult.Yes)
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