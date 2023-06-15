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
    /// Логика взаимодействия для LecturerTimeConstraintWindow.xaml
    /// </summary>
    public partial class LecturerTimeConstraintWindow : Window
    {
        public LecturerTimeConstraintWindow()
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
            ScheduleDBConnection scheduleDBConnection = ScheduleDBConnection.GetInstance();
            CellHeight = 24;
            CellWidth = 150;
            List<Lecturer> allLecturers = scheduleDBConnection.GetAllLecturers();
            foreach (Lecturer lecturer in allLecturers)
            {
                string fullName = lecturer.firstName + " " + lecturer.middleName + " " + lecturer.lastName;
                lecturerComboBox.Items.Add(fullName);
            }
        }
        const int DAY_FIELD_INDEX = 0;
        const int PERIOD_BEGIN_FIELD_INDEX = 1;
        const int PERIOD_END_FIELD_INDEX = 2;
        const int CHANGE_BUTTON_INDEX = 3;
        UIElement[,] _elements;

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
                for (int fieldIndex = 0; fieldIndex < _headers.Length; fieldIndex++)
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
            int fieldsQuantity = 3 + 1; // pseudocode: fields.Count + max(buttons.Count)

            SetUIElementArraySizes(subjectsQuantity + 1, fieldsQuantity);

            fieldsGrid.Children.Clear();

            Dictionary<int, string> indexDayNameDict = new Dictionary<int, string>
            {
                { 0, "Понеділок" },
                { 1, "Вівторок" },
                { 2, "Середа" },
                { 3, "Четвер" },
                { 4, "П'ятниця" },
                { 5, "Субота" }
            };

            Lecturer currentLecturer;

            {
                string fullName = (string)lecturerComboBox.SelectedValue;
                if (fullName==null)
                {
                    return;
                }
                string[] splittedFullName = fullName.Split(" ");
                string firstName = splittedFullName[0];
                string middleName = splittedFullName[1];
                string lastName = splittedFullName[2];
                currentLecturer = scheduleDBConnection.GetLecturer(firstName, middleName, lastName);
            }

            for (int dayIndex = 0; dayIndex < 6; ++dayIndex)
            {
                // Edit UIElements of window:
                TextBox dayTextBox = new TextBox();
                dayTextBox.Text = indexDayNameDict[dayIndex];
                dayTextBox.IsReadOnly = true; // field is readonly
                AddUIElement(dayTextBox, dayIndex, DAY_FIELD_INDEX);

                TextBox periodBeginTextBox = new TextBox(); // changeable field must have next on change handler, or similar, depending on data type
                periodBeginTextBox.TextChanged += (object sender, TextChangedEventArgs e) =>
                {
                    TextBox periodBeginTextBox = (TextBox)sender;
                    int subjectIndex = Grid.GetRow(periodBeginTextBox);

                    Button changeButton = (Button)GetUIElement(subjectIndex, CHANGE_BUTTON_INDEX);
                    if (changeButton != null)
                        changeButton.Visibility = Visibility.Visible;
                };
                periodBeginTextBox.Text = currentLecturer.availability[dayIndex].start.ToString();
                AddUIElement(periodBeginTextBox, dayIndex, PERIOD_BEGIN_FIELD_INDEX);

                TextBox periodEndTextBox = new TextBox(); // changeable field must have next on change handler, or similar, depending on data type
                periodEndTextBox.TextChanged += (object sender, TextChangedEventArgs e) =>
                {
                    TextBox periodEndTextBox = (TextBox)sender;
                    int subjectIndex = Grid.GetRow(periodEndTextBox);

                    Button changeButton = (Button)GetUIElement(subjectIndex, CHANGE_BUTTON_INDEX);
                    if (changeButton != null)
                        changeButton.Visibility = Visibility.Visible;
                };
                periodEndTextBox.Text = currentLecturer.availability[dayIndex].end.ToString();
                AddUIElement(periodEndTextBox, dayIndex, PERIOD_END_FIELD_INDEX);

                // Here add other fields

                Button changeButton = new Button();
                changeButton.Content = "Змінити";
                changeButton.Visibility = Visibility.Collapsed;
                changeButton.Click += changeButton_Clicked;
                AddUIElement(changeButton, dayIndex, CHANGE_BUTTON_INDEX);
            }
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
            int dayIndex = Grid.GetRow(addButton);

            // Read input data:

            Lecturer currentLecturer;

            {
                string fullName = (string)lecturerComboBox.SelectedValue;
                string[] splittedFullName = fullName.Split(" ");
                string firstName = splittedFullName[0];
                string middleName = splittedFullName[1];
                string lastName = splittedFullName[2];
                currentLecturer = scheduleDBConnection.GetLecturer(firstName, middleName, lastName);
            }

            TextBox periodBeginTextBox = (TextBox)GetUIElement(dayIndex, PERIOD_BEGIN_FIELD_INDEX);
            currentLecturer.availability[dayIndex].start = byte.Parse(periodBeginTextBox.Text);

            TextBox periodEndTextBox = (TextBox)GetUIElement(dayIndex, PERIOD_END_FIELD_INDEX);
            currentLecturer.availability[dayIndex].end = byte.Parse(periodEndTextBox.Text);

            scheduleDBConnection.UpdateLecturer(currentLecturer);
            MessageBox.Show("Дані оновлено");
            UpdateFieldsGrid();
        }

        private void lecturerComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateFieldsGrid();
        }
    }
}
