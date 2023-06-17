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
    /// Логика взаимодействия для EditLecturerGroupSubjectRelationWindow.xaml
    /// </summary>
    public partial class EditLecturerGroupSubjectWindow : Window
    {
        public EditLecturerGroupSubjectWindow()
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
        private LecturerGroupSubjectRelation[] _relations;
        public void InitTableParameters()
        {
            CellHeight = 20;
            CellWidth = 100;
        }
        const int GROUP_FIELD_INDEX = 0;
        const int LECTURER_FIELD_INDEX = 1;
        const int SUBJECT_FIELD_INDEX = 2;
        const int CHANGE_BUTTON_INDEX = 3;
        const int DELETE_BUTTON_INDEX = 4;
        UIElement[,] _elements;
        // Indexes for buttons in new element field

        const int ADD_BUTTON_INDEX = 3;

        private void SetUIElementArraySizes(int relationsQuantity, int fieldsQuantity)
        {
            _elements = new UIElement[relationsQuantity, fieldsQuantity];
            for (int groupIndex = 0; groupIndex < relationsQuantity; ++groupIndex)
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

        private void AddUIElement(UIElement element, int relationIndex, int fieldIndex)
        {
            Grid.SetRow(element, relationIndex);
            Grid.SetColumn(element, fieldIndex);
            fieldsGrid.Children.Add(element);
            _elements[relationIndex, fieldIndex] = element;
        }

        private UIElement GetUIElement(int relationIndex, int fieldIndex)
        {
            return _elements[relationIndex, fieldIndex];
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
                "Назва групи",
                "Ім'я вчителя",
                "Дисципліна"
            };

            _relations = scheduleDBConnection.GetAllLecturerGroupSubjectRelations().ToArray();

            int subjectsQuantity = _relations.Length;
            int fieldsQuantity = 3+2; // pseudocode: fields.Count + max(buttons.Count)

            SetUIElementArraySizes(subjectsQuantity + 1, fieldsQuantity);

            fieldsGrid.Children.Clear();

            for(int subjectIndex = 0; subjectIndex < _relations.Length; ++subjectIndex)
            {
                // Edit UIElements of window:
                TextBox groupTitleTextBox = new TextBox();
                groupTitleTextBox.Text = _relations[subjectIndex].Group.Name;
                groupTitleTextBox.IsReadOnly = true; // field is readonly
                AddUIElement(groupTitleTextBox, subjectIndex, GROUP_FIELD_INDEX);

                TextBox lecturerNameTextBox = new TextBox();
                lecturerNameTextBox.Text = _relations[subjectIndex].Lecturer.firstName+" "+
                    _relations[subjectIndex].Lecturer.middleName + " " +
                    _relations[subjectIndex].Lecturer.lastName;
                lecturerNameTextBox.IsReadOnly = true;
                AddUIElement(lecturerNameTextBox, subjectIndex, LECTURER_FIELD_INDEX);

                TextBox subjectTitleTextBox = new TextBox();
                subjectTitleTextBox.Text = _relations[subjectIndex].Subject.title;
                subjectTitleTextBox.IsReadOnly = true;
                AddUIElement(subjectTitleTextBox, subjectIndex, SUBJECT_FIELD_INDEX);

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

            ComboBox newRelationGroupComboBox = new ComboBox();

            List<Group> allGroups = scheduleDBConnection.GetAllGroups();

            foreach (Group group in allGroups)
            {
                newRelationGroupComboBox.Items.Add(group.Name);
            }

            AddUIElement(newRelationGroupComboBox, _relations.Length, GROUP_FIELD_INDEX);

            ComboBox newRelationLecturerComboBox = new ComboBox();

            List<Lecturer> allLecturers = scheduleDBConnection.GetAllLecturers();

            foreach (Lecturer lecturer in allLecturers)
            {
                newRelationLecturerComboBox.Items.Add(lecturer.firstName+" "+lecturer.middleName+" "+lecturer.lastName);
            }

            AddUIElement(newRelationLecturerComboBox, _relations.Length, LECTURER_FIELD_INDEX);

            ComboBox newRelationSubjectComboBox = new ComboBox();

            List<Subject> allSubjects = scheduleDBConnection.GetAllSubjects();

            foreach (Subject subject in allSubjects)
            {
                newRelationSubjectComboBox.Items.Add(subject.title);
            }

            AddUIElement(newRelationSubjectComboBox, _relations.Length, SUBJECT_FIELD_INDEX);

            Button addButton = new Button();
            addButton.Content = "Додати";
            addButton.Click += addButton_Clicked;
            AddUIElement(addButton, _relations.Length, ADD_BUTTON_INDEX);
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

            TextBox groupTitleTextBox = (TextBox)GetUIElement(subjectIndex, GROUP_FIELD_INDEX);
            string groupTitle = groupTitleTextBox.Text;

            Group relationGroup = scheduleDBConnection.GetGroup(groupTitle);

            TextBox lecturerTitleTextBox = (TextBox)GetUIElement(subjectIndex, LECTURER_FIELD_INDEX);
            string[] lecturerName = lecturerTitleTextBox.Text.Split(" ");

            string lecturerFirstName = lecturerName[0];
            string lecturerMiddleName = lecturerName[1];
            string lecturerLastName = lecturerName[2];

            Lecturer relationLecturer = scheduleDBConnection.GetLecturer(lecturerFirstName, lecturerMiddleName, lecturerLastName);

            TextBox subjectTitleTextBox = (TextBox)GetUIElement(subjectIndex, SUBJECT_FIELD_INDEX);
            string subjectTitle = subjectTitleTextBox.Text;

            Subject relationSubject = scheduleDBConnection.GetSubject(subjectTitle);

            var relationToUpdate = scheduleDBConnection.GetLecturerGroupSubjectRelation(relationGroup, relationLecturer, relationSubject);

            scheduleDBConnection.UpdateLecturerGroupSubjectRelation(relationToUpdate);
            UpdateFieldsGrid();
        }

        private void deleteButton_Clicked(object sender, RoutedEventArgs e)
        {
            ScheduleDBConnection scheduleDBConnection = ScheduleDBConnection.GetInstance();

            Button addButton = (Button)sender;
            int subjectIndex = Grid.GetRow(addButton);

            // Read input data:

            TextBox groupTitleTextBox = (TextBox)GetUIElement(subjectIndex, GROUP_FIELD_INDEX);
            string groupTitle = groupTitleTextBox.Text;

            Group relationGroup = scheduleDBConnection.GetGroup(groupTitle);

            TextBox lecturerTitleTextBox = (TextBox)GetUIElement(subjectIndex, LECTURER_FIELD_INDEX);
            string lecturerName = lecturerTitleTextBox.Text;
            string[] lecturerSplittedName = lecturerName.Split(" ");

            string lecturerFirstName = lecturerSplittedName[0];
            string lecturerMiddleName = lecturerSplittedName[1];
            string lecturerLastName = lecturerSplittedName[2];

            Lecturer relationLecturer = scheduleDBConnection.GetLecturer(lecturerFirstName, lecturerMiddleName, lecturerLastName);

            TextBox subjectTitleTextBox = (TextBox)GetUIElement(subjectIndex, SUBJECT_FIELD_INDEX);
            string subjectTitle = subjectTitleTextBox.Text;

            Subject relationSubject = scheduleDBConnection.GetSubject(subjectTitle);

            var relationToDelete = scheduleDBConnection.GetLecturerGroupSubjectRelation(relationGroup, relationLecturer, relationSubject);

            MessageBoxResult deleteMessageBoxResult = MessageBox.Show($"Ви впевнені, що хочете видалити зв'язок '{groupTitle} : {lecturerName} : {subjectTitle}'?", "Видалення даних", MessageBoxButton.YesNo);

            if (deleteMessageBoxResult == MessageBoxResult.Yes)
            {
                scheduleDBConnection.DeleteLecturerGroupSubjectRelation(relationToDelete);
                UpdateFieldsGrid();
            }
        }

        private void addButton_Clicked(object sender, RoutedEventArgs e)
        {
            ScheduleDBConnection scheduleDBConnection = ScheduleDBConnection.GetInstance();

            Button addButton = (Button)sender;
            int subjectIndex = Grid.GetRow(addButton);

            // Read input data:

            ComboBox groupTitleComboBox = (ComboBox)GetUIElement(subjectIndex, GROUP_FIELD_INDEX);
            string groupTitle = (string)groupTitleComboBox.SelectedValue;

            Group relationGroup = scheduleDBConnection.GetGroup(groupTitle);

            ComboBox lecturerTitleTextBox = (ComboBox)GetUIElement(subjectIndex, LECTURER_FIELD_INDEX);
            string lecturerName = (string)lecturerTitleTextBox.SelectedValue;
            string[] lecturerSplittedName = lecturerName.Split(" ");

            string lecturerFirstName = lecturerSplittedName[0];
            string lecturerMiddleName = lecturerSplittedName[1];
            string lecturerLastName = lecturerSplittedName[2];

            Lecturer relationLecturer = scheduleDBConnection.GetLecturer(lecturerFirstName, lecturerMiddleName, lecturerLastName);

            ComboBox subjectTitleComboBox = (ComboBox)GetUIElement(subjectIndex, SUBJECT_FIELD_INDEX);
            string subjectTitle = (string)subjectTitleComboBox.SelectedValue;

            Subject relationSubject = scheduleDBConnection.GetSubject(subjectTitle);

            var relationToAdd = new LecturerGroupSubjectRelation(relationGroup, relationLecturer, relationSubject);

            scheduleDBConnection.AddLecturerGroupSubjectRelation(relationToAdd);
            UpdateFieldsGrid();
        }
    }
}
