using System.Windows;
using System.Windows.Controls;

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
                newFirstNameTextBox.TextChanged += (object sender, TextChangedEventArgs e) =>
                {
                    TextBox newFirstNameTextBox = (TextBox)sender;
                    int subjectIndex = Grid.GetRow(newFirstNameTextBox);

                    Button changeButton = (Button)GetUIElement(subjectIndex, CHANGE_BUTTON_INDEX);
                    if (changeButton != null)
                        changeButton.IsEnabled = true;
                };
                AddUIElement(newFirstNameTextBox, lecturerIndex, FNAME_FIELD_INDEX);

                TextBox newMiddleNameTextBox = new TextBox();
                newMiddleNameTextBox.Text = _lecturers[lecturerIndex].middleName;
                newMiddleNameTextBox.TextChanged += (object sender, TextChangedEventArgs e) =>
                {
                    TextBox newMiddleNameTextBox = (TextBox)sender;
                    int subjectIndex = Grid.GetRow(newMiddleNameTextBox);

                    Button changeButton = (Button)GetUIElement(subjectIndex, CHANGE_BUTTON_INDEX);
                    if (changeButton != null)
                        changeButton.IsEnabled = true;
                };
                AddUIElement(newMiddleNameTextBox, lecturerIndex, MNAME_FIELD_INDEX);

                TextBox newLastNameTextBox = new TextBox();
                newLastNameTextBox.Text = _lecturers[lecturerIndex].lastName;
                newLastNameTextBox.TextChanged += (object sender, TextChangedEventArgs e) =>
                {
                    TextBox newLastNameTextBox = (TextBox)sender;
                    int subjectIndex = Grid.GetRow(newLastNameTextBox);

                    Button changeButton = (Button)GetUIElement(subjectIndex, CHANGE_BUTTON_INDEX);
                    if (changeButton != null)
                        changeButton.IsEnabled = true;
                };
                AddUIElement(newLastNameTextBox, lecturerIndex, LNAME_FIELD_INDEX);

                // Here add other fields

                Button changeButton = new Button();
                changeButton.Content = "Змінити";
                changeButton.IsEnabled = false;
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

            Button changeButton = (Button)sender;
            int lecturerIndex = Grid.GetRow(changeButton);

            // Read input data:

            TextBox lecturerFirstNameTextBox = (TextBox)GetUIElement(lecturerIndex, FNAME_FIELD_INDEX);
            string lecturerFirstName = lecturerFirstNameTextBox.Text;

            TextBox lecturerMiddleNameTextBox = (TextBox)GetUIElement(lecturerIndex, MNAME_FIELD_INDEX);
            string lecturerMiddleName = lecturerMiddleNameTextBox.Text;

            TextBox lecturerLastNameTextBox = (TextBox)GetUIElement(lecturerIndex, LNAME_FIELD_INDEX);
            string lecturerLastName = lecturerLastNameTextBox.Text;

            _lecturers[lecturerIndex].firstName = lecturerFirstName;
            _lecturers[lecturerIndex].middleName = lecturerMiddleName;
            _lecturers[lecturerIndex].lastName = lecturerLastName;
            changeButton.IsEnabled = false;

            scheduleDBConnection.UpdateLecturer(_lecturers[lecturerIndex]);
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
            if (scheduleDBConnection.LecturerHasRelations(lecturerToDelete))
            {
                MessageBox.Show($"Для видалення викладача '{lecturerLastName} {lecturerFirstName} {lecturerMiddleName}' спочатку необхідно видалити всі його зв'язки");
                return;
            }
            MessageBoxResult deleteMessageBoxResult = MessageBox.Show($"Ви впевнені, що хочете видалити викладача '{lecturerLastName} {lecturerFirstName} {lecturerMiddleName}' і всі пов'язані з ним записи у розкладі?",
                "Видалення даних", MessageBoxButton.YesNo);

            if (deleteMessageBoxResult == MessageBoxResult.Yes)
            {
                scheduleDBConnection.DeleteLecturer(lecturerToDelete);
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

            Lecturer lecturerToAdd = new Lecturer(null, lecturerFirstName, lecturerMiddleName, lecturerLastName);

            scheduleDBConnection.AddLecturer(lecturerToAdd);
            UpdateFieldsGrid();
        }
    }
}
