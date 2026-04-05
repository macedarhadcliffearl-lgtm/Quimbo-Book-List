using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Quimbo_Book_List
{
    public partial class MainWindow : Window
    {
        private ObservableCollection<Book> books = new ObservableCollection<Book>();

        // Track selected item (for edit mode)
        private Book selectedBook = null;

        public MainWindow()
        {
            InitializeComponent();
            dataGrid.ItemsSource = books;
        }

        // SAVE / UPDATE
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInputs(out double price)) return;

            string category = GetSelectedCategory();

            // EDIT MODE
            if (selectedBook != null)
            {
                selectedBook.Name = txtName.Text;
                selectedBook.Brand = txtBrand.Text;
                selectedBook.Price = price;
                selectedBook.Category = category;

                dataGrid.Items.Refresh(); // update UI
                MessageBox.Show("Book updated successfully!");
            }
            else // ADD MODE
            {
                if (books.Any(b => b.Name == txtName.Text && b.Brand == txtBrand.Text))
                {
                    MessageBox.Show("This book already exists.");
                    return;
                }

                books.Add(new Book
                {
                    Name = txtName.Text,
                    Brand = txtBrand.Text,
                    Price = price,
                    Category = category
                });

                MessageBox.Show("Book added successfully!");
            }

            ClearForm();
        }

        // DELETE
        private void btnDeleteData_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid.SelectedItem is Book book)
            {
                var result = MessageBox.Show("Are you sure you want to delete this book?",
                                            "Confirm Delete",
                                            MessageBoxButton.YesNo);

                if (result == MessageBoxResult.Yes)
                {
                    books.Remove(book);
                    ClearForm();
                }
            }
        }

        // SELECT ROW (AUTO-FILL FORM)
        private void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dataGrid.SelectedItem is Book book)
            {
                selectedBook = book;

                txtName.Text = book.Name;
                txtBrand.Text = book.Brand;
                txtPrice.Text = book.Price.ToString();

                SetSelectedCategory(book.Category);

                btnDeleteData.IsEnabled = true;
            }
            else
            {
                btnDeleteData.IsEnabled = false;
            }
        }

        // VALIDATION
        private bool ValidateInputs(out double price)
        {
            price = 0;

            if (string.IsNullOrWhiteSpace(txtName.Text) ||
                string.IsNullOrWhiteSpace(txtBrand.Text) ||
                string.IsNullOrWhiteSpace(txtPrice.Text))
            {
                MessageBox.Show("Please fill in all fields.");
                return false;
            }

            if (!double.TryParse(txtPrice.Text, out price))
            {
                MessageBox.Show("Price must be a valid number.");
                return false;
            }

            if (GetSelectedCategory() == "")
            {
                MessageBox.Show("Please select a category.");
                return false;
            }

            return true;
        }

        // GET CATEGORY
        private string GetSelectedCategory()
        {
            RadioButton[] radios = { rbA, rbB, rbC, rbD };

            foreach (var rb in radios)
            {
                if (rb.IsChecked == true)
                    return rb.Content.ToString();
            }

            return "";
        }

        // SET CATEGORY (for edit mode)
        private void SetSelectedCategory(string category)
        {
            rbA.IsChecked = category == "Finance";
            rbB.IsChecked = category == "Self-help";
            rbC.IsChecked = category == "Classic";
            rbD.IsChecked = category == "Science";
        }

        // CLEAR FORM
        private void ClearForm()
        {
            txtName.Clear();
            txtBrand.Clear();
            txtPrice.Clear();

            rbA.IsChecked = false;
            rbB.IsChecked = false;
            rbC.IsChecked = false;
            rbD.IsChecked = false;

            selectedBook = null;
            dataGrid.SelectedItem = null;
            btnDeleteData.IsEnabled = false;
        }
    }

    // MODEL
    public class Book
    {
        public string Name { get; set; }
        public string Brand { get; set; }
        public double Price { get; set; }
        public string Category { get; set; }
    }
}