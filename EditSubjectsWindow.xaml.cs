using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace schedule
{
    /// <summary>
    /// Логика взаимодействия для EditLecturersWindow.xaml
    /// </summary>
    public partial class EditSubjectsWindow : Window
    {
        public EditSubjectsWindow()
        {
            InitializeComponent();
            InitTableParameters();
            UpdateFieldsGrid();
        }
        private int _cellWidth;
        public int CellWidth
        {
            get { return _cellWidth; }
            private set
            {
                _cellWidth = value;
            }
        }
        private int _cellHeight;
        public int CellHeight
        {
            get { return _cellHeight; }
            private set
            {
                _cellHeight = value;
            }
        }
        private Subject[] _subjects;
        public void InitTableParameters()
        {
            CellHeight = 20;
            CellWidth = 100;
        }
        const int TITLE_FIELD_INDEX = 0;
        const int TOTAL_AMOUNT_FIELD_INDEX = 1;
        const int CHANGE_BUTTON_INDEX = 2;
        const int DELETE_BUTTON_INDEX = 3;
        UIElement[,] _elements;
        // Indexes for buttons in new element field

        const int ADD_BUTTON_INDEX = 2;

        private void SetUIElementArraySizes(int groupsQuantity, int fieldsQuantity)
        {
            _elements = new UIElement[groupsQuantity, fieldsQuantity];
            for (int groupIndex = 0; groupIndex < groupsQuantity; ++groupIndex)
            {
                RowDefinition groupRowDefinition = new RowDefinition();
                groupRowDefinition.Height = new System.Windows.GridLength(_cellHeight);
                fieldsGrid.RowDefinitions.Add(groupRowDefinition);
            }
            for (int fieldIndex = 0; fieldIndex < fieldsQuantity; ++fieldIndex)
            {
                ColumnDefinition fieldColumnDefinition = new ColumnDefinition();
                fieldColumnDefinition.Width = new System.Windows.GridLength(_cellWidth);
                fieldsGrid.ColumnDefinitions.Add(fieldColumnDefinition);
                ColumnDefinition headerColumnDefinition = new ColumnDefinition();
                headerColumnDefinition.Width = new System.Windows.GridLength(_cellWidth);
                headersGrid.ColumnDefinitions.Add(headerColumnDefinition);
            }
        }

        private void AddUIElement(UIElement element, int groupIndex, int fieldIndex)
        {
            Grid.SetRow(element, groupIndex);
            Grid.SetColumn(element, fieldIndex);
            fieldsGrid.Children.Add(element);
            _elements[groupIndex, fieldIndex] = element;
        }

        private UIElement GetUIElement(int groupIndex, int fieldIndex)
        {
            return _elements[groupIndex, fieldIndex];
        }

        string[] _headers;

        public string[] Headers
        {
            get { return _headers; }
            set
            {
                _headers = value;
                for(int fieldIndex = 0; fieldIndex < _headers.Length; fieldIndex++)
                {
                    TextBox headerTextBox = new TextBox();
                    headerTextBox.Text = _headers[fieldIndex];
                    Grid.SetColumn(headerTextBox, fieldIndex);
                    headersGrid.Children.Add(headerTextBox);
                }
            }
        }

        public void UpdateFieldsGrid()
        {
            ScheduleDBConnection scheduleDBConnection = ScheduleDBConnection.GetInstance();

            Headers = new string[]
            {
                "Назва",
                "Повна кількість годин"
            };

            _subjects = scheduleDBConnection.GetAllSubjects().ToArray();

            int subjectsQuantity = _subjects.Length;
            int fieldsQuantity = 3+2; // pseudocode: fields.Count + max(buttons.Count)

            SetUIElementArraySizes(subjectsQuantity + 1, fieldsQuantity);

            fieldsGrid.Children.Clear();

            for(int subjectIndex = 0; subjectIndex < _subjects.Length; ++subjectIndex)
            {
                // Edit UIElements of window:
                TextBox titleTextBox = new TextBox();
                titleTextBox.Text = _subjects[subjectIndex].title;
                titleTextBox.IsReadOnly = true; // field is readonly
                AddUIElement(titleTextBox, subjectIndex, TITLE_FIELD_INDEX);

                TextBox totalAmountTextBox = new TextBox(); // changeable field must have next on change handler, or similar, depending on data type
                totalAmountTextBox.TextChanged += (object sender, TextChangedEventArgs e) =>
                {
                    TextBox totalAmountTextBox = (TextBox)sender;
                    int subjectIndex = Grid.GetRow(totalAmountTextBox);

                    Button changeButton = (Button)GetUIElement(subjectIndex, CHANGE_BUTTON_INDEX);
                    if(changeButton!=null)
                        changeButton.Visibility = Visibility.Visible;
                };
                totalAmountTextBox.Text = _subjects[subjectIndex].totalAmount.ToString();
                AddUIElement(totalAmountTextBox, subjectIndex, TOTAL_AMOUNT_FIELD_INDEX);

                // Here add other fields

                Button changeButton = new Button();
                changeButton.Content = "Змінити";
                changeButton.Visibility = Visibility.Collapsed;
                changeButton.Click += changeButton_Clicked;
                AddUIElement(changeButton, subjectIndex, CHANGE_BUTTON_INDEX);

                Button deleteButton = new Button();
                deleteButton.Content = "Видалити";
                deleteButton.Click += deleteButton_Clicked;
                AddUIElement(deleteButton, subjectIndex, DELETE_BUTTON_INDEX);
            }
            // Edit UIElements of new element:

            TextBox newTitleTextBox = new TextBox();
            AddUIElement(newTitleTextBox, _subjects.Length, TITLE_FIELD_INDEX);

            TextBox newMiddleNameTextBox = new TextBox();
            AddUIElement(newMiddleNameTextBox, _subjects.Length, TOTAL_AMOUNT_FIELD_INDEX);

            Button addButton = new Button();
            addButton.Content = "Додати";
            addButton.Click += addButton_Clicked;
            AddUIElement(addButton, _subjects.Length, ADD_BUTTON_INDEX);
            // fieldsGrid.Children.Add(newElementTitleTextBox);
        }

        private void fieldsScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            ScrollViewer scrollViewer = (ScrollViewer)sender;
            double horizontalOffset = e.HorizontalOffset;
            double verticalOffset = e.VerticalOffset;
            headersScrollViewer.ScrollToHorizontalOffset(horizontalOffset);
        }

        private void changeButton_Clicked(object sender, RoutedEventArgs e)
        {
            ScheduleDBConnection scheduleDBConnection = ScheduleDBConnection.GetInstance();

            Button addButton = (Button)sender;
            int subjectIndex = Grid.GetRow(addButton);

            // Read input data:

            TextBox subjectTitleTextBox = (TextBox)GetUIElement(subjectIndex, TITLE_FIELD_INDEX);
            string subjectTitle = subjectTitleTextBox.Text;

            TextBox subjectTotalAmountTextBox = (TextBox)GetUIElement(subjectIndex, TOTAL_AMOUNT_FIELD_INDEX);
            int subjectTotalAmount = Int32.Parse(subjectTotalAmountTextBox.Text);

            Subject subjectToUpdate = scheduleDBConnection.GetSubject(subjectTitle);
            subjectToUpdate.totalAmount = subjectTotalAmount;

            scheduleDBConnection.UpdateSubject(subjectToUpdate);
            MessageBox.Show("Дані оновлено");
            UpdateFieldsGrid();
        }

        private void deleteButton_Clicked(object sender, RoutedEventArgs e)
        {
            ScheduleDBConnection scheduleDBConnection = ScheduleDBConnection.GetInstance();

            Button addButton = (Button)sender;
            int subjectIndex = Grid.GetRow(addButton);

            // Read input data:

            TextBox subjectTitleTextBox = (TextBox)GetUIElement(subjectIndex, TITLE_FIELD_INDEX);
            string subjectTitle = subjectTitleTextBox.Text;

            Subject subjectToDelete = scheduleDBConnection.GetSubject(subjectTitle);
            MessageBoxResult deleteMessageBoxResult = MessageBox.Show($"Ви впевнені, що хочете видалити дисципліну \"{subjectTitle}\"?", "Видалення даних", MessageBoxButton.YesNo);

            if (deleteMessageBoxResult == MessageBoxResult.Yes)
            {
                scheduleDBConnection.DeleteSubject(subjectToDelete);
                MessageBox.Show("Дані видалено");
                UpdateFieldsGrid();
            }
        }

        private void addButton_Clicked(object sender, RoutedEventArgs e)
        {
            ScheduleDBConnection scheduleDBConnection = ScheduleDBConnection.GetInstance();

            Button addButton = (Button)sender;
            int subjectIndex = Grid.GetRow(addButton);

            // Read input data:

            TextBox subjectTitleTextBox = (TextBox)GetUIElement(subjectIndex, TITLE_FIELD_INDEX);
            string subjectTitle = subjectTitleTextBox.Text;

            TextBox subjectTotalAmountTextBox = (TextBox)GetUIElement(subjectIndex, TOTAL_AMOUNT_FIELD_INDEX);
            int subjectTotalAmount = Int32.Parse(subjectTotalAmountTextBox.Text);

            Subject subjectToAdd = new Subject(subjectTitle);
            subjectToAdd.totalAmount = subjectTotalAmount;

            scheduleDBConnection.AddSubject(subjectToAdd);
            MessageBox.Show("Дані додано");
            UpdateFieldsGrid();
        }
    }
}
