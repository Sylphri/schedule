using System.Windows;
using System.Windows.Controls;

namespace schedule
{
    /// <summary>
    /// Логика взаимодействия для EditGroupsWindow.xaml
    /// </summary>
    public partial class EditGroupsWindow : Window
    {
        public EditGroupsWindow()
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
        private Group[] _groups;
        public void InitTableParameters()
        {
            CellHeight = 20;
            CellWidth = 100;
        }
        const int TITLE_FIELD_INDEX = 0;
        const int CHANGE_BUTTON_INDEX = 1;
        const int DELETE_BUTTON_INDEX = 2;
        UIElement[,] _elements;
        // Indexes for buttons in new element field

        const int ADD_BUTTON_INDEX = 1;

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
                "Назва"
            };

            _groups = scheduleDBConnection.GetAllGroups().ToArray();

            int groupsQuantity = _groups.Length;
            int fieldsQuantity = 1+2; // pseudocode: fields.Count + max(buttons.Count)

            SetUIElementArraySizes(groupsQuantity + 1, fieldsQuantity);

            fieldsGrid.Children.Clear();

            for(int groupIndex = 0; groupIndex < _groups.Length; ++groupIndex)
            {
                TextBox titleTextBox = new TextBox();
                titleTextBox.Text = _groups[groupIndex].Name;
                titleTextBox.TextChanged += (object sender, TextChangedEventArgs e) =>
                {
                    TextBox titleTextBox = (TextBox)sender;
                    int subjectIndex = Grid.GetRow(titleTextBox);

                    Button changeButton = (Button)GetUIElement(subjectIndex, CHANGE_BUTTON_INDEX);
                    if (changeButton != null)
                        changeButton.IsEnabled = true;
                };
                AddUIElement(titleTextBox, groupIndex, TITLE_FIELD_INDEX);
                
                // Here add other fields

                Button changeButton = new Button();
                changeButton.Content = "Змінити";
                changeButton.IsEnabled = false;
                changeButton.Click += changeButton_Clicked;
                AddUIElement(changeButton, groupIndex, CHANGE_BUTTON_INDEX);

                Button deleteButton = new Button();
                deleteButton.Content = "Видалити";
                deleteButton.Click += deleteButton_Clicked;
                AddUIElement(deleteButton, groupIndex, DELETE_BUTTON_INDEX);
            }
            TextBox newElementTitleTextBox = new TextBox();
            AddUIElement(newElementTitleTextBox, _groups.Length, TITLE_FIELD_INDEX);
            Button addButton = new Button();
            addButton.Content = "Додати";
            addButton.Click += addButton_Clicked;
            AddUIElement(addButton, _groups.Length, ADD_BUTTON_INDEX);
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
            int groupIndex = Grid.GetRow(changeButton);
            TextBox groupNameTextBox = (TextBox)GetUIElement(groupIndex, TITLE_FIELD_INDEX);
            string groupName = groupNameTextBox.Text;
            _groups[groupIndex].Name = groupName;
            changeButton.IsEnabled = false;

            scheduleDBConnection.UpdateGroup(_groups[groupIndex]);
            UpdateFieldsGrid();
        }

        private void deleteButton_Clicked(object sender, RoutedEventArgs e)
        {
            ScheduleDBConnection scheduleDBConnection = ScheduleDBConnection.GetInstance();

            Button addButton = (Button)sender;
            int groupIndex = Grid.GetRow(addButton);
            TextBox groupNameTextBox = (TextBox)GetUIElement(groupIndex, TITLE_FIELD_INDEX);
            string groupName = groupNameTextBox.Text;
            Group groupToDelete = scheduleDBConnection.GetGroup(groupName);

            if (scheduleDBConnection.GroupHasRelations(groupToDelete))
            {
                MessageBox.Show($"Для видалення групи '{groupToDelete.Name}' спочатку необхідно видалити всі її зв'язки");
                return;
            }

            MessageBoxResult deleteMessageBoxResult = MessageBox.Show($"Ви впевнені, що хочете видалити групу '{groupName}' і всі пов'язані з нею записи у розкладі?", 
                "Видалення даних", MessageBoxButton.YesNo);

            if (deleteMessageBoxResult == MessageBoxResult.Yes)
            {
                scheduleDBConnection.DeleteGroup(groupToDelete);
                UpdateFieldsGrid();
            }
        }

        private void addButton_Clicked(object sender, RoutedEventArgs e)
        {
            Button addButton = (Button)sender;
            int groupIndex = Grid.GetRow(addButton);
            TextBox groupNameTextBox = (TextBox)GetUIElement(groupIndex, TITLE_FIELD_INDEX);
            string groupName = groupNameTextBox.Text;
            Group groupToAdd = new Group(groupName);

            ScheduleDBConnection scheduleDBConnection = ScheduleDBConnection.GetInstance();
            scheduleDBConnection.AddGroup(groupToAdd);
            UpdateFieldsGrid();
        }
    }
}
