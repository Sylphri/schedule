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
    public partial class MaxSubjectPerWeekConstraintWindow : Window
    {
        public MaxSubjectPerWeekConstraintWindow()
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
        public void InitTableParameters()
        {
            ScheduleDBConnection scheduleDBConnection = ScheduleDBConnection.GetInstance();
            CellHeight = 24;
            CellWidth = 200;
        }
        const int SUBJECT_FIELD_INDEX = 0;
        const int LESSONS_PER_WEEK_FIELD_INDEX = 1;
        const int CHANGE_BUTTON_INDEX = 2;
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
        Subject[] _subjects;
        public void UpdateFieldsGrid()
        {
            ScheduleDBConnection scheduleDBConnection = ScheduleDBConnection.GetInstance();

            Headers = new string[]
            {
                "Дисципліна",
                "Кількість годин на тиждень"
            };

            _subjects = scheduleDBConnection.GetAllSubjects().ToArray();

            int subjectsQuantity = _subjects.Length;
            int fieldsQuantity = 2 + 1; // pseudocode: fields.Count + max(buttons.Count)

            SetUIElementArraySizes(subjectsQuantity + 1, fieldsQuantity);

            fieldsGrid.Children.Clear();

            for (int subjectIndex = 0; subjectIndex < subjectsQuantity; ++subjectIndex)
            {
                // Edit UIElements of window:
                TextBox subjectTextBox = new TextBox();
                
                subjectTextBox.IsReadOnly = true; // field is readonly
                subjectTextBox.Text = _subjects[subjectIndex].title;
                AddUIElement(subjectTextBox, subjectIndex, SUBJECT_FIELD_INDEX);

                TextBox lessonsPerWeekTextBox = new TextBox(); // changeable field must have next on change handler, or similar, depending on data type
                lessonsPerWeekTextBox.TextChanged += (object sender, TextChangedEventArgs e) =>
                {
                    TextBox lessonsPerWeekTextBox = (TextBox)sender;
                    int subjectIndex = Grid.GetRow(lessonsPerWeekTextBox);

                    Button changeButton = (Button)GetUIElement(subjectIndex, CHANGE_BUTTON_INDEX);
                    if (changeButton != null)
                        changeButton.Visibility = Visibility.Visible;
                };
                lessonsPerWeekTextBox.Text = _subjects[subjectIndex].lessonsPerWeek.ToString();
                AddUIElement(lessonsPerWeekTextBox, subjectIndex, LESSONS_PER_WEEK_FIELD_INDEX);

                // Here add other fields

                Button changeButton = new Button();
                changeButton.Content = "Змінити";
                changeButton.Visibility = Visibility.Collapsed;
                changeButton.Click += changeButton_Clicked;
                AddUIElement(changeButton, subjectIndex, CHANGE_BUTTON_INDEX);
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
            int subjectIndex = Grid.GetRow(addButton);

            // Read input data:

            TextBox subjectTextBox = (TextBox)GetUIElement(subjectIndex, SUBJECT_FIELD_INDEX);
            string subjectTitle = subjectTextBox.Text;

            Subject subjectToUpdate = scheduleDBConnection.GetSubject(subjectTitle);

            TextBox lessonsPerWeekTextBox = (TextBox)GetUIElement(subjectIndex, LESSONS_PER_WEEK_FIELD_INDEX);
            byte lessonsPerWeek = byte.Parse(lessonsPerWeekTextBox.Text);

            subjectToUpdate.lessonsPerWeek = lessonsPerWeek;

            scheduleDBConnection.UpdateSubject(subjectToUpdate);
            MessageBox.Show("Дані оновлено");
            UpdateFieldsGrid();
        }

        private void lecturerComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateFieldsGrid();
        }
    }
}
