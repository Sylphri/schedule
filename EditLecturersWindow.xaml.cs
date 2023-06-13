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
    public partial class EditLecturersWindow : Window
    {
        public EditLecturersWindow()
        {
            InitializeComponent();
            InitTableParameters();
            UpdateFieldsGrid();
        }
        /*private int _width;
        public int Width
        {
            get { return _width; }
            private set
            {
                _width = value;
                headersGrid.ColumnDefinitions.Clear();
                fieldsGrid.ColumnDefinitions.Clear();
                for (int i = 0; i<_width; ++i)
                {
                    ColumnDefinition headerColumnDefinition = new ColumnDefinition();
                    headerColumnDefinition.Width = ;
                    headersGrid.ColumnDefinitions.Add(headerColumnDefinition);
                    ColumnDefinition fieldColumnDefinition = new ColumnDefinition();
                    fieldsGrid.ColumnDefinitions.Add(fieldColumnDefinition);
                }
            }
        }*/
        /*private int _height;
        public int Height
        {
            get { return _height; }
            private set
            {
                _height = value;
                fieldsGrid.ColumnDefinitions.Clear();
                for (int i = 0; i < _height; ++i)
                {
                    RowDefinition fieldRowDefinition = new RowDefinition();
                    fieldsGrid.RowDefinitions.Add(fieldRowDefinition);
                }
            }
        }*/
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
        private Lecturer[] _lecturers;
        public void InitTableParameters()
        {
            CellHeight = 20;
            CellWidth = 100;
        }
        const int LNAME_FIELD_INDEX = 0;
        const int FNAME_FIELD_INDEX = 1;
        const int MNAME_FIELD_INDEX = 2;
        const int CHANGE_BUTTON_INDEX = 3;
        const int DELETE_BUTTON_INDEX = 4;
        UIElement[,] _elements;
        // Indexes for buttons in new element field

        const int ADD_BUTTON_INDEX = 3;

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
                "Прізвище",
                "Ім'я",
                "По батькові",
            };

            _lecturers = scheduleDBConnection.GetAllLecturers().ToArray();

            int lecturersQuantity = _lecturers.Length;
            int fieldsQuantity = 4+2; // pseudocode: fields.Count + max(buttons.Count)

            SetUIElementArraySizes(lecturersQuantity + 1, fieldsQuantity);

            fieldsGrid.Children.Clear();

            for(int lecturerIndex = 0; lecturerIndex < _lecturers.Length; ++lecturerIndex)
            {
                // Edit UIElements of window:
                TextBox newFirstNameTextBox = new TextBox();
                newFirstNameTextBox.Text = _lecturers[lecturerIndex].firstName;
                newFirstNameTextBox.IsReadOnly = true; // field is readonly
                AddUIElement(newFirstNameTextBox, lecturerIndex, FNAME_FIELD_INDEX);

                TextBox newMiddleNameTextBox = new TextBox();
                newMiddleNameTextBox.Text = _lecturers[lecturerIndex].middleName;
                newMiddleNameTextBox.IsReadOnly = true;
                AddUIElement(newMiddleNameTextBox, lecturerIndex, MNAME_FIELD_INDEX);

                TextBox newLastNameTextBox = new TextBox();
                newLastNameTextBox.Text = _lecturers[lecturerIndex].lastName;
                newLastNameTextBox.IsReadOnly = true;
                AddUIElement(newLastNameTextBox, lecturerIndex, LNAME_FIELD_INDEX);

                // Here add other fields

                Button changeButton = new Button();
                changeButton.Content = "Змінити";
                changeButton.Visibility = Visibility.Collapsed;
                changeButton.Click += changeButton_Clicked;
                AddUIElement(changeButton, lecturerIndex, CHANGE_BUTTON_INDEX);

                Button deleteButton = new Button();
                deleteButton.Content = "Видалити";
                deleteButton.Click += deleteButton_Clicked;
                AddUIElement(deleteButton, lecturerIndex, DELETE_BUTTON_INDEX);
            }
            // Edit UIElements of new element:

            TextBox firstNameTextBox = new TextBox();
            AddUIElement(firstNameTextBox, _lecturers.Length, FNAME_FIELD_INDEX);

            TextBox middleNameTextBox = new TextBox();
            AddUIElement(middleNameTextBox, _lecturers.Length, MNAME_FIELD_INDEX);

            TextBox lastNameTextBox = new TextBox();
            AddUIElement(lastNameTextBox, _lecturers.Length, LNAME_FIELD_INDEX);

            Button addButton = new Button();
            addButton.Content = "Додати";
            addButton.Click += addButton_Clicked;
            AddUIElement(addButton, _lecturers.Length, ADD_BUTTON_INDEX);
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
            int lecturerIndex = Grid.GetRow(addButton);

            // Read input data:

            TextBox lecturerFirstNameTextBox = (TextBox)GetUIElement(lecturerIndex, FNAME_FIELD_INDEX);
            string lecturerFirstName = lecturerFirstNameTextBox.Text;

            TextBox lecturerMiddleNameTextBox = (TextBox)GetUIElement(lecturerIndex, MNAME_FIELD_INDEX);
            string lecturerMiddleName = lecturerMiddleNameTextBox.Text;

            TextBox lecturerLastNameTextBox = (TextBox)GetUIElement(lecturerIndex, LNAME_FIELD_INDEX);
            string lecturerLastName = lecturerLastNameTextBox.Text;

            Lecturer lecturerToUpdate = scheduleDBConnection.GetLecturer(lecturerFirstName, lecturerMiddleName, lecturerLastName);

            scheduleDBConnection.UpdateLecturer(lecturerToUpdate);
            MessageBox.Show("Дані оновлено");
            UpdateFieldsGrid();
        }

        private void deleteButton_Clicked(object sender, RoutedEventArgs e)
        {
            ScheduleDBConnection scheduleDBConnection = ScheduleDBConnection.GetInstance();

            Button addButton = (Button)sender;
            int lecturerIndex = Grid.GetRow(addButton);

            // Read input data:

            TextBox lecturerFirstNameTextBox = (TextBox)GetUIElement(lecturerIndex, FNAME_FIELD_INDEX);
            string lecturerFirstName = lecturerFirstNameTextBox.Text;

            TextBox lecturerMiddleNameTextBox = (TextBox)GetUIElement(lecturerIndex, MNAME_FIELD_INDEX);
            string lecturerMiddleName = lecturerMiddleNameTextBox.Text;

            TextBox lecturerLastNameTextBox = (TextBox)GetUIElement(lecturerIndex, LNAME_FIELD_INDEX);
            string lecturerLastName = lecturerLastNameTextBox.Text;

            Lecturer lecturerToDelete = scheduleDBConnection.GetLecturer(lecturerFirstName, lecturerMiddleName, lecturerLastName);
            MessageBoxResult deleteMessageBoxResult = MessageBox.Show($"Ви впевнені, що хочете видалити викладача \"{lecturerLastName} {lecturerFirstName} {lecturerMiddleName}\"?", "Видалення даних", MessageBoxButton.YesNo);

            if (deleteMessageBoxResult == MessageBoxResult.Yes)
            {
                scheduleDBConnection.DeleteLecturer(lecturerToDelete);
                MessageBox.Show("Дані видалено");
                UpdateFieldsGrid();
            }
        }

        private void addButton_Clicked(object sender, RoutedEventArgs e)
        {
            ScheduleDBConnection scheduleDBConnection = ScheduleDBConnection.GetInstance();

            Button addButton = (Button)sender;
            int lecturerIndex = Grid.GetRow(addButton);

            // Read input data:

            TextBox lecturerFirstNameTextBox = (TextBox)GetUIElement(lecturerIndex, FNAME_FIELD_INDEX);
            string lecturerFirstName = lecturerFirstNameTextBox.Text;

            TextBox lecturerMiddleNameTextBox = (TextBox)GetUIElement(lecturerIndex, MNAME_FIELD_INDEX);
            string lecturerMiddleName = lecturerMiddleNameTextBox.Text;

            TextBox lecturerLastNameTextBox = (TextBox)GetUIElement(lecturerIndex, LNAME_FIELD_INDEX);
            string lecturerLastName = lecturerLastNameTextBox.Text;

            Lecturer lecturerToAdd = new Lecturer(lecturerFirstName, lecturerMiddleName, lecturerLastName, new Period[6]);

            scheduleDBConnection.AddLecturer(lecturerToAdd);
            MessageBox.Show("Дані додано");
            UpdateFieldsGrid();
        }
    }
}
