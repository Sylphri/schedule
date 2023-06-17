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
    /// Логика взаимодействия для EditClassroomsWindow.xaml
    /// </summary>
    public partial class EditClassroomsWindow : Window
    {
        public EditClassroomsWindow()
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
        private Classroom[] _classrooms;
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

        private void SetUIElementArraySizes(int classroomsQuantity, int fieldsQuantity)
        {
            _elements = new UIElement[classroomsQuantity, fieldsQuantity];
            for (int classroomIndex = 0; classroomIndex < classroomsQuantity; ++classroomIndex)
            {
                RowDefinition classroomRowDefinition = new RowDefinition();
                classroomRowDefinition.Height = new System.Windows.GridLength(_cellHeight);
                fieldsGrid.RowDefinitions.Add(classroomRowDefinition);
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

        private void AddUIElement(UIElement element, int classroomIndex, int fieldIndex)
        {
            Grid.SetRow(element, classroomIndex);
            Grid.SetColumn(element, fieldIndex);
            fieldsGrid.Children.Add(element);
            _elements[classroomIndex, fieldIndex] = element;
        }

        private UIElement GetUIElement(int classroomIndex, int fieldIndex)
        {
            return _elements[classroomIndex, fieldIndex];
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

            _classrooms = scheduleDBConnection.GetAllClassrooms().ToArray();

            int classroomsQuantity = _classrooms.Length;
            int fieldsQuantity = 1+2; // pseudocode: fields.Count + max(buttons.Count)

            SetUIElementArraySizes(classroomsQuantity + 1, fieldsQuantity);

            fieldsGrid.Children.Clear();

            for(int classroomIndex = 0; classroomIndex < _classrooms.Length; ++classroomIndex)
            {
                TextBox titleTextBox = new TextBox();
                titleTextBox.Text = _classrooms[classroomIndex].title;
                titleTextBox.IsReadOnly = true; // field title is readonly
                AddUIElement(titleTextBox, classroomIndex, TITLE_FIELD_INDEX);
                
                // Here add other fields

                Button changeButton = new Button();
                changeButton.Content = "Змінити";
                changeButton.Visibility = Visibility.Collapsed;
                changeButton.Click += changeButton_Clicked;
                AddUIElement(changeButton, classroomIndex, CHANGE_BUTTON_INDEX);

                Button deleteButton = new Button();
                deleteButton.Content = "Видалити";
                deleteButton.Click += deleteButton_Clicked;
                AddUIElement(deleteButton, classroomIndex, DELETE_BUTTON_INDEX);
            }
            TextBox newElementTitleTextBox = new TextBox();
            AddUIElement(newElementTitleTextBox, _classrooms.Length, TITLE_FIELD_INDEX);
            Button addButton = new Button();
            addButton.Content = "Додати";
            addButton.Click += addButton_Clicked;
            AddUIElement(addButton, _classrooms.Length, ADD_BUTTON_INDEX);
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
            int classroomIndex = Grid.GetRow(addButton);
            TextBox classroomNameTextBox = (TextBox)GetUIElement(classroomIndex, TITLE_FIELD_INDEX);
            string classroomName = classroomNameTextBox.Text;
            Classroom classroomToDelete = scheduleDBConnection.GetClassroom(classroomName);

            scheduleDBConnection.UpdateClassroom(classroomToDelete);
            MessageBox.Show("Дані оновлено");
            UpdateFieldsGrid();
        }

        private void deleteButton_Clicked(object sender, RoutedEventArgs e)
        {
            

            ScheduleDBConnection scheduleDBConnection = ScheduleDBConnection.GetInstance();

            Button addButton = (Button)sender;
            int classroomIndex = Grid.GetRow(addButton);
            TextBox classroomNameTextBox = (TextBox)GetUIElement(classroomIndex, TITLE_FIELD_INDEX);
            string classroomName = classroomNameTextBox.Text;
            Classroom classroomToDelete = scheduleDBConnection.GetClassroom(classroomName);

            MessageBoxResult deleteMessageBoxResult = MessageBox.Show($"Ви впевнені, що хочете видалити аудиторію \"{classroomName}\"?", "Видалення даних", MessageBoxButton.YesNo);

            if (deleteMessageBoxResult == MessageBoxResult.Yes)
            {
                scheduleDBConnection.DeleteClassroom(classroomToDelete);
                MessageBox.Show("Дані видалено");
                UpdateFieldsGrid();
            }
        }

        private void addButton_Clicked(object sender, RoutedEventArgs e)
        {
            Button addButton = (Button)sender;
            int classroomIndex = Grid.GetRow(addButton);
            TextBox classroomNameTextBox = (TextBox)GetUIElement(classroomIndex, TITLE_FIELD_INDEX);
            string classroomName = classroomNameTextBox.Text;
            Classroom classroomToAdd = new Classroom(null, classroomName);

            ScheduleDBConnection scheduleDBConnection = ScheduleDBConnection.GetInstance();
            scheduleDBConnection.AddClassroom(classroomToAdd);
            MessageBox.Show("Дані додано");
            UpdateFieldsGrid();
        }
    }
}
