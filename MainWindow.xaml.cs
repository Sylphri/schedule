using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace schedule
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            CultureInfo ukrainianCulture = new CultureInfo("uk-UA");
            Thread.CurrentThread.CurrentCulture = ukrainianCulture;
            Thread.CurrentThread.CurrentUICulture = ukrainianCulture;
            RefreshGroups();
            CellWidth = 250;
            CellHeight = 200;
            _errorsWindow = new ErrorsWindow();

            DataConverter = (object obj) => {
                if (obj is Table.Cell cell)
                {
                    /*string[] result = {
                        cell.first.discipline,
                        cell.first.lecturer.firstName,
                        (cell.first.classroom?.ToString() ?? "")
                    };*/
                    ComboBox disciplineComboBox = new ComboBox();
                    ComboBox lecturerComboBox = new ComboBox();
                    TextBox classRoomTextBox = new TextBox();
                    disciplineComboBox.DropDownOpened += (object sender, EventArgs e) =>
                    {
                        ComboBox subjectComboBox = (ComboBox)sender;
                        string selectedItem = (string)subjectComboBox.SelectedItem;
                        subjectComboBox.Items.Clear();
                        StackPanel cellStackPanel = (StackPanel)subjectComboBox.Parent;
                        int column = Grid.GetColumn(cellStackPanel);
                        Group group = (Group)_indexHeaderDictionary[column];
                        ScheduleDBConnection scheduleDBConnection = ScheduleDBConnection.GetInstance();
                        ComboBox lecturerComboBox = (ComboBox)cellStackPanel.Children[1];
                        // it still doesnt work
                        if (lecturerComboBox.SelectedValue == "" || lecturerComboBox.SelectedValue == " " || lecturerComboBox.SelectedItem == "")
                        {
                            return;
                        }
                        try
                        {
                            string[] lecturerName = (lecturerComboBox.SelectedValue as string).Split(" ");
                            Lecturer lecturer = scheduleDBConnection.GetLecturer(lecturerName[0], lecturerName[1], lecturerName[2]);
                            List<Subject> subjects = scheduleDBConnection.GetPossibleSubjects(group, lecturer);
                            subjectComboBox.Items.Add("Додати дисципліну");
                            subjectComboBox.Items.Add("");
                            foreach (Subject subject in subjects)
                            {
                                subjectComboBox.Items.Add(subject.title);
                            }
                        }
                        catch
                        {
                            return;
                        }
                    };
                    lecturerComboBox.DropDownOpened += (object sender, EventArgs e) =>
                    {
                        ComboBox lecturerComboBox = (ComboBox)sender;
                        string selectedItem = (string)lecturerComboBox.SelectedItem;
                        lecturerComboBox.Items.Clear();
                        StackPanel cellStackPanel = (StackPanel)lecturerComboBox.Parent;
                        int column = Grid.GetColumn(cellStackPanel);
                        Group group = (Group)_indexHeaderDictionary[column];
                        ScheduleDBConnection scheduleDBConnection = ScheduleDBConnection.GetInstance();
                        List<Lecturer> lecturers = scheduleDBConnection.GetGroupLecturers(group);
                        lecturerComboBox.Items.Add("Додати викладача");
                        lecturerComboBox.Items.Add("");
                        foreach (Lecturer lecturer in lecturers)
                        {
                            string lecturerFullName = lecturer.firstName + " " + lecturer.middleName + " " + lecturer.lastName;
                            lecturerComboBox.Items.Add(lecturerFullName);
                        }
                        lecturerComboBox.SelectedItem = selectedItem;
                    };
                    if (cell.first != null)
                    {
                        disciplineComboBox.Items.Add(cell.first.subject?.title ?? "");
                        disciplineComboBox.SelectedItem = cell.first.subject.title;
                        disciplineComboBox.IsReadOnly = true;
                        
                        string lecturerFullName;
                        if (cell.first.lecturer != null)
                        {
                            lecturerFullName = cell.first.lecturer.firstName + " " + cell.first.lecturer.middleName + " " + cell.first.lecturer.lastName;
                        }
                        else
                        {
                            lecturerFullName = "";
                        }
                        lecturerComboBox.Items.Add(lecturerFullName);
                        lecturerComboBox.SelectedItem = lecturerFullName;
                        classRoomTextBox.Text = cell.first.classroom?.title ?? ""; 
                    }
                    ComboBox secondDisciplineComboBox = new ComboBox();
                    ComboBox secondLecturerComboBox = new ComboBox();
                    secondDisciplineComboBox.DropDownOpened += (object sender, EventArgs e) =>
                    {
                        ComboBox subjectComboBox = (ComboBox)sender;
                        string selectedItem = (string)subjectComboBox.SelectedItem;
                        subjectComboBox.Items.Clear();
                        StackPanel cellStackPanel = (StackPanel)subjectComboBox.Parent;
                        int column = Grid.GetColumn(cellStackPanel);
                        Group group = (Group)_indexHeaderDictionary[column];
                        ScheduleDBConnection scheduleDBConnection = ScheduleDBConnection.GetInstance();
                        ComboBox lecturerComboBox = (ComboBox)cellStackPanel.Children[6];
                        string[] lecturerName = (lecturerComboBox.SelectedValue as string).Split(" ");
                        Lecturer lecturer = scheduleDBConnection.GetLecturer(lecturerName[0], lecturerName[1], lecturerName[2]);
                        List<Subject> subjects = scheduleDBConnection.GetPossibleSubjects(group, lecturer);
                        subjectComboBox.Items.Add("Додати дисципліну");
                        subjectComboBox.Items.Add("");
                        foreach (Subject subject in subjects)
                        {
                            subjectComboBox.Items.Add(subject.title);
                        }
                    };
                    secondLecturerComboBox.DropDownOpened += (object sender, EventArgs e) =>
                    {
                        ComboBox lecturerComboBox = (ComboBox)sender;
                        string selectedItem = (string)lecturerComboBox.SelectedItem;
                        lecturerComboBox.Items.Clear();
                        StackPanel cellStackPanel = (StackPanel)lecturerComboBox.Parent;
                        int column = Grid.GetColumn(cellStackPanel);
                        Group group = (Group)_indexHeaderDictionary[column];
                        ScheduleDBConnection scheduleDBConnection = ScheduleDBConnection.GetInstance();
                        List<Lecturer> lecturers = scheduleDBConnection.GetGroupLecturers(group);
                        lecturerComboBox.Items.Add("Додати викладача");
                        lecturerComboBox.Items.Add("");
                        foreach (Lecturer lecturer in lecturers)
                        {
                            string lecturerFullName = lecturer.firstName + " " + lecturer.middleName + " " + lecturer.lastName;
                            lecturerComboBox.Items.Add(lecturerFullName);
                        }
                        lecturerComboBox.SelectedItem = selectedItem;
                    };
                    Button uniteGroupButton = new Button();
                    uniteGroupButton.Click += (object sender, RoutedEventArgs e) =>
                    {
                        Button button = (Button)sender;
                        StackPanel cellStackPanel = (StackPanel)button.Parent;
                        int rowIndex = Grid.GetRow(cellStackPanel);
                        int columnIndex = Grid.GetColumn(cellStackPanel);
                        object header = _indexHeaderDictionary[columnIndex];
                        object updatedData;
                        if (header is Group group)
                        {
                            updatedData = table[group, rowIndex / 5, rowIndex % 5];
                            (updatedData as Table.Cell).second = null;
                            (updatedData as Table.Cell).isSplitted = false;
                        }
                        else
                        {
                            throw new Exception("Error when uniting group");
                        }
                        _dataChange(table, updatedData, header, rowIndex);
                        RedrawCell(cellStackPanel, columnIndex, rowIndex);
                    };
                    uniteGroupButton.Content = "Об'єднати групу";
                    Label label = new Label();
                    label.Content = "друга підгрупа:";
                    if (cell.isSplitted)
                    {
                        secondDisciplineComboBox.Items.Add(cell.second?.subject?.title ?? "");
                        secondDisciplineComboBox.SelectedItem = (cell.second?.subject?.title ?? "");
                        string secondLecturerFullName;
                        if (cell.second==null || cell.second.lecturer == null)
                        {
                            secondLecturerFullName = "";
                        }
                        else
                        {
                            secondLecturerFullName = cell.second.lecturer.firstName + " " + cell.second.lecturer.middleName + " " + cell.second.lecturer.lastName;
                        }
                        secondLecturerComboBox.Items.Add(secondLecturerFullName);
                        secondLecturerComboBox.SelectedItem = secondLecturerFullName;
                        TextBox secondClassRoomTextBox = new TextBox();
                        secondClassRoomTextBox.Text = (cell.second?.classroom?.title ?? "");
                        UIElement[] result = {
                            disciplineComboBox,
                            lecturerComboBox,
                            classRoomTextBox,
                            uniteGroupButton,
                            label,
                            secondDisciplineComboBox,
                            secondLecturerComboBox,
                            secondClassRoomTextBox
                        };
                        return result;
                    }
                    else
                    {
                        Button splitGroupButton = new Button();
                        splitGroupButton.Click += (object sender, RoutedEventArgs e) =>
                        {
                            Button button = (Button)sender;
                            StackPanel cellStackPanel = (StackPanel)button.Parent;
                            int rowIndex = Grid.GetRow(cellStackPanel);
                            int columnIndex = Grid.GetColumn(cellStackPanel);
                            object header = _indexHeaderDictionary[columnIndex];
                            object updatedData;
                            if (header is Group group)
                            {
                                updatedData = table[group, rowIndex / 5, rowIndex % 5]; // _dataChangeConverter(cellStackPanel.Children.Cast<UIElement>().ToArray());
                                (updatedData as Table.Cell).second = new Table.SubCell(null, null, null, null, false, (updatedData as Table.Cell).first);
                                table[group, rowIndex / 5, rowIndex % 5].isSplitted=true;
                            }
                            else
                            {
                                throw new Exception("Error when splitting table");
                            }
                            _dataChange(table, updatedData, header, rowIndex);
                            RedrawCell(cellStackPanel, columnIndex, rowIndex);
                        };
                        splitGroupButton.Content = "Розділити на підгрупи";
                        UIElement[] result = {
                            disciplineComboBox,
                            lecturerComboBox,
                            classRoomTextBox,
                            splitGroupButton
                        };
                        return result;
                    }
                }
                throw new ArgumentException("У DataConverter потрібний тип об'єкта - Table.Cell");
            };
            HeadersConverter = (object obj) =>
            {
                if (obj is Group group)
                {
                    return group.Name;
                }
                throw new ArgumentException("У HeadersConverter потрібний тип об'єкта - Group");
            };
            HeadersSorting = (ICollection<object> objects) =>
            {
                if (objects.OfType<Group>().Any())
                {
                    Dictionary<object, int> result = new Dictionary<object, int>();
                    int i = 0;
                    foreach (object obj in objects)
                    {
                        Group header = (Group)obj;
                        result.Add(header, i);
                        ++i;
                    }
                    return result;
                }
                throw new ArgumentException("У HeadersSorting потрібний тип об'єкта - ICollection<Group>");
            };
            DataChangeConverter = (UIElement[] dataElements) =>
            {
                ScheduleDBConnection scheduleDBConnection = ScheduleDBConnection.GetInstance();
                Table.Cell result = new Table.Cell();
                // cell.first.discipline,
                // cell.first.lecturer.firstName,
                // (cell.first.classroom?.ToString() ?? "")
                string subject = (string)(dataElements[0] as ComboBox).SelectedItem;
                ComboBox lecturerComboBox = (ComboBox)dataElements[1];
                string lecturerNameStr = (string)lecturerComboBox.SelectedItem;
                if(lecturerNameStr.IsNullOrEmpty() || subject.IsNullOrEmpty())
                {
                    result.first = null;
                }
                else
                {
                    string[] lecturerName = lecturerNameStr.Split(" ");
                    // new Lecturer(lecturerName[0], lecturerName[1], lecturerName[2], new Period[] { });
                    string classroomFieldValue = (dataElements[2] as TextBox).Text;
                    Classroom? classroom = (classroomFieldValue.IsNullOrEmpty()
                        ? null
                        : scheduleDBConnection.GetClassroom(classroomFieldValue));
                    /*if (classroomFieldValue.IsNullOrEmpty())
                    {
                        MessageBox.Show("Введіть номер аудиторії");
                        return null;
                    }*/

                    /*Classroom classroom = scheduleDBConnection.GetClassroom(classroomFieldValue);
                    if (classroom == null)
                        scheduleDBConnection.AddClassroom(new Classroom(null, classroomFieldValue));
                    classroom = scheduleDBConnection.GetClassroom(classroomFieldValue);*/

                    result.first = new Table.SubCell(
                        scheduleDBConnection.GetSubject(subject),
                        scheduleDBConnection.GetLecturer(lecturerName[0], lecturerName[1], lecturerName[2]),
                        classroom,
                        false,
                        null
                    );
                }

                if (dataElements.Length > 5)
                {
                    result.isSplitted = true;
                    string secondSubject = (string)(dataElements[5] as ComboBox).SelectedItem;
                    string fullSecondLecturerName = (dataElements[6] as ComboBox).SelectedItem as string;
                    if(secondSubject.IsNullOrEmpty() || fullSecondLecturerName.IsNullOrEmpty())
                    {
                        result.second = null;
                    }
                    else
                    {
                        string[] secondLecturerName = fullSecondLecturerName.Split(" ");
                        string secondClassroomFieldValue = (dataElements[7] as TextBox).Text;
                        /*if (secondClassroomFieldValue.IsNullOrEmpty())
                        {
                            MessageBox.Show("Введіть номер аудиторії");
                            return null;
                        }

                        Classroom classroom = scheduleDBConnection.GetClassroom(secondClassroomFieldValue);
                        if (classroom == null)
                            scheduleDBConnection.AddClassroom(new Classroom(null, secondClassroomFieldValue));
                        classroom = scheduleDBConnection.GetClassroom(secondClassroomFieldValue);*/

                        Classroom? classroom = (secondClassroomFieldValue.IsNullOrEmpty()
                            ? null
                            : scheduleDBConnection.GetClassroom(secondClassroomFieldValue));

                        result.second = new Table.SubCell(
                            null,
                            scheduleDBConnection.GetSubject(secondSubject),
                            scheduleDBConnection.GetLecturer(secondLecturerName[0], secondLecturerName[1], secondLecturerName[2]),
                            classroom,
                            false,
                            null
                        );
                    }
                }

                return result;
            };
            DataChange = (object originalStorage, object newValue, object header, int rowIndex) =>
            {
                if (originalStorage is Table table && header is Group group && newValue is Table.Cell newValueCell)
                {
                    int dayNumber = rowIndex / 5;
                    int lessonNumber = rowIndex % 5;
                    Table.Position cellPosition = new Table.Position(group, dayNumber, lessonNumber);
                    table[cellPosition] = newValueCell;
                }
                else
                {
                    throw new ArgumentException("originalStorage must be Table and header must be Group && newValue must be Table.Cell");
                }
            };

            DateTime weekDay = DateTime.Now;
            DateTime weekMonday = weekDay.AddDays(-(int)weekDay.DayOfWeek + 1);
            ShowWeek(weekMonday);
        }

        void ShowWeek(DateTime date)
        {
            weekDatePicker.SelectedDate = date;
            ScheduleDBConnection scheduleDBConnection = ScheduleDBConnection.GetInstance();
            table = scheduleDBConnection.GetWeek(date); // new Table(_groups, 5, 5);
            table.AddDefaultCheckers();
            VerticalHeadersF = VerticalHeaders.CreateDayNumberHeaders(1, 5, 5);
            Table.Position position = new Table.Position(new Group("Something 1"), 0, 3);

            /*table[position] = new Table.Cell(new Table.SubCell("disc", new Lecturer("lecturer1", "Lecturovich", "Lecturenko", ), null));
            position = new Table.Position(new Group("Something 4"), 0, 1);
            table[position] = new Table.Cell(new Table.SubCell("Біологія", new Lecturer("lecturer2", "Lecturovich", "Lecturenko"), 124));
            position = new Table.Position(new Group("Something 5"), 4, 0);
            table[position] = new Table.Cell(new Table.SubCell("3rd discipline", new Lecturer("lecturer3", "Lecturovich", "Lecturenko"), 111));*/

            if (table.Content.Count != 0)
            {
                var dataSource = new Dictionary<object, object[]>();
                foreach (var kvp in table.Content)
                {
                    dataSource.Add((object)kvp.Key, (object[])kvp.Value);
                }
                DataSource = dataSource;
            }
            
            UpdateView();
            /*ColsQuantity = groups.Length;
            RowsQuantity = 5*5;
            Headers = groups;*/
        }
        Table table;
        private Group[] _groups;

        /*int _colsQuantity;
        public int ColsQuantity
        {
            get { return _colsQuantity; }
            set {
                if (_colsQuantity != value)
                {
                    _colsQuantity = value;
                    tableGrid.ColumnDefinitions.Clear();
                    headersGrid.ColumnDefinitions.Clear();
                    for (int i = 0; i < _colsQuantity; ++i)
                    {
                        tableGrid.ColumnDefinitions.Add(new ColumnDefinition());
                        headersGrid.ColumnDefinitions.Add(new ColumnDefinition());
                    }
                }
            }
        }
        int _rowsQuantity;
        public int RowsQuantity
        {
            get { return _rowsQuantity; }
            set
            {
                if (_rowsQuantity != value)
                {
                    _rowsQuantity = value;
                    tableGrid.RowDefinitions.Clear();
                    for (int i = 0; i < _rowsQuantity; ++i)
                    {
                        tableGrid.RowDefinitions.Add(new RowDefinition());
                    }
                }
            }
        }
        
        object[] _headers;
        public object[] Headers
        {
            get { return _headers; }
            set
            {
                for (int i = 0; i< _colsQuantity; i++)
                {
                    TextBlock headerTextBlock = new TextBlock();
                    headerTextBlock.Text = _headersConverter(_headers[i]);
                    Grid.SetColumn(headerTextBlock, i);
                    Grid.SetRow(headerTextBlock, 0);
                    headersGrid.Children.Add(headerTextBlock);
                }
            }
        }
        

        int GetColumnIndex(string colHeader)
        {
            for (int i = 0; i<_colsQuantity; ++i)
            {
                if (_headersConverter(_headers[i]) == colHeader)
                {
                    return i;
                }
            }
            throw new KeyNotFoundException("Column not found");
        }
        
        void SetColumnData(string colHeader, object[] data)
        {
            for (int i = 0; i < _rowsQuantity; ++i)
            {
                *//*TextBox headerTextBox = new TextBox();
                Grid.SetColumn(headerTextBox, colIndex);
                Grid.SetRow(headerTextBox, i);
                headersGrid.Children.Add(headerTextBox);*//*
                StackPanel stackPanel = new StackPanel();
                string[] fields = _dataConverter(data[i]);
                foreach(string field in fields)
                {
                    TextBox headerTextBox = new TextBox();
                    Grid.SetColumn(headerTextBox, GetColumnIndex(colHeader));
                    Grid.SetRow(headerTextBox, i);
                    headersGrid.Children.Add(headerTextBox);
                }
            }
        }*/

        Dictionary<object, int> _headerIndexDictionary;
        Dictionary<int, object> _indexHeaderDictionary;

        Dictionary<object, object[]> _dataSource;
        /// <summary>
        /// 
        /// </summary>
        public Dictionary<object, object[]> DataSource
        {
            set
            {
                _dataSource = value;
                headersGrid.ColumnDefinitions.Clear();
                tableGrid.ColumnDefinitions.Clear();
                for (int i = 0; i < _dataSource.Count+1; ++i)
                {
                    ColumnDefinition columnDefinition = new ColumnDefinition();
                    columnDefinition.Width = new System.Windows.GridLength(_cellWidth);
                    headersGrid.ColumnDefinitions.Add(columnDefinition);
                    columnDefinition = new ColumnDefinition();
                    columnDefinition.Width = new System.Windows.GridLength(_cellWidth);
                    tableGrid.ColumnDefinitions.Add(columnDefinition);
                }
                int rowsQuantity = _dataSource.Last().Value.Length;
                headersGrid.RowDefinitions.Clear();
                headersGrid.RowDefinitions.Add(new RowDefinition());
                tableGrid.RowDefinitions.Clear();
                for (int i = 0; i < rowsQuantity; i++)
                {
                    RowDefinition rowDefinition = new RowDefinition();
                    rowDefinition.Height = new System.Windows.GridLength(_cellHeight);
                    tableGrid.RowDefinitions.Add(rowDefinition);
                }
                
            }
            get
            {
                return _dataSource;
            }
        }
        // Не впевнений як правильно називати делегати й відповідні властивості

        /// <summary>
        /// Делегат розділення об'єктів-клітинок на поля в UIElement[]
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public delegate UIElement[] DataConverterDelegate(object obj);
        DataConverterDelegate _dataConverter;
        public DataConverterDelegate DataConverter
        {
            set
            {
                _dataConverter = value;
            }
        }
        /// <summary>
        /// Делегат перетворення об'єкта-заголовка в рядок для відображення
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public delegate string HeadersConverterDelegate(object obj);
        HeadersConverterDelegate _headersConverter;
        public HeadersConverterDelegate HeadersConverter
        {
            set
            {
                _headersConverter = value;
            }
        }
        /// <summary>
        /// Делегат сортування стовпців
        /// </summary>
        /// <param name="headers">Заголовки таблиці</param>
        /// <returns> Dictionary&lt;об'єкт-заголовок, індекс_у_таблиці&gt; </returns>
        public delegate Dictionary<object, int> HeadersSortingDelegate(ICollection<object> headers);
        HeadersSortingDelegate _headersSorting;
        public HeadersSortingDelegate HeadersSorting
        {
            set
            {
                _headersSorting = value;
            }
        }
        public delegate object DataChangeConverterDelegate(UIElement[] dataElements);
        DataChangeConverterDelegate _dataChangeConverter;
        public DataChangeConverterDelegate DataChangeConverter
        {
            set
            {
                _dataChangeConverter = value;
            }
        }
        public delegate void DataChangeDelegate(object originalStorage, object newValue, object header, int rowIndex);
        DataChangeDelegate _dataChange;
        public DataChangeDelegate DataChange
        {
            set
            {
                _dataChange = value;
            }
        }
        /// <summary>
        /// Оновлює відображення таблиці
        /// </summary>
        /// <exception cref="Exception">Виникає коли не встановлено властивості HeadersSorting, DataConverter або HeadersConverter</exception>
        void UpdateView()
        {
            if (_groups.Length == 0)
            {
                TextBox noGroupsTextBox = new TextBox();
                noGroupsTextBox.IsReadOnly = true;
                noGroupsTextBox.Text = "Додайте групи, щоб записувати розклад";
                tableGrid.Children.Clear();
                tableGrid.Children.Add(noGroupsTextBox);
                headersGrid.Children.Clear();
                return;
            }

            if (_dataConverter == null)
            {
                throw new Exception("Data converter is not set");
            }
            if (_headersConverter == null)
            {
                throw new Exception("Headers converter is not set");
            }
            if (_headersSorting == null)
            {
                throw new Exception("Headers sorting is not set");
            }
            headersGrid.Children.Clear();
            _headerIndexDictionary = _headersSorting(_dataSource.Keys.ToArray());
            _indexHeaderDictionary = _headerIndexDictionary.ToDictionary(x => x.Value, x => x.Key);
            int columnDataLength = 0;

            tableGrid.Children.Clear();
            foreach(object header in _dataSource.Keys)
            {
                columnDataLength = _dataSource[header].Length;
                break;
            }
            
            foreach (object header in _dataSource.Keys)
            {
                // Оновлення відображення заголовків:
                TextBlock headerTextBlock = new TextBlock();
                headerTextBlock.Text = _headersConverter(header);
                int headerIndex = _headerIndexDictionary[header];
                Grid.SetColumn(headerTextBlock, headerIndex);
                Grid.SetRow(headerTextBlock, 0);
                headersGrid.Children.Add(headerTextBlock);
                // Оновлення відображення даних:
                object[] columnData = _dataSource[header];

                if (_verticalHeaders != null)
                {
                    for (int iy = 0; iy < columnDataLength; iy++)
                    {
                        RowDefinition rowDefinition = new RowDefinition();
                        rowDefinition.Height = new System.Windows.GridLength(_cellHeight);
                        verticalHintsGrid.RowDefinitions.Add(rowDefinition);
                        
                    }
                    for(int ix = 0; ix<2; ix++)
                    {
                        verticalHintsGrid.ColumnDefinitions.Add(new ColumnDefinition());
                    }
                }

                for (int iy = 0; iy < columnData.Length; ++iy)
                {
                    RedrawCell(columnData, headerIndex, iy);
                }
            }

            // Add group button:
            Button addGroupButton = new Button();
            addGroupButton.Content = "Додати групу";
            int addGroupButtonIndex = _dataSource.Count;
            Grid.SetColumn(addGroupButton, addGroupButtonIndex);

            addGroupButton.Click += Menu_Data_EditGroups;

            headersGrid.Children.Add(addGroupButton);
        }

        public void RedrawCell(object[] columnData, int headerIndex, int rowIndex)
        {
            StackPanel cellStackPanel = new StackPanel();
            Grid.SetColumn(cellStackPanel, headerIndex);
            Grid.SetRow(cellStackPanel, rowIndex);
            // cellStackPanel.Margin = new System.Windows.Thickness(10);
            foreach (UIElement cellDataField in _dataConverter(columnData[rowIndex]))
            {
                /*TextBox cellFieldTextBox = new TextBox();
                cellFieldTextBox.Text = cellData;
                cellFieldTextBox.TextChanged += (object sender, TextChangedEventArgs e) =>
                {
                    TextBox textBox = (TextBox)sender;
                    StackPanel stackPanel = (StackPanel)textBox.Parent;
                    int column = Grid.GetColumn(stackPanel);
                    int row = Grid.GetRow(stackPanel);

                    int buttonsIndex = VisualTreeHelper.GetChildrenCount(stackPanel) - 1;
                    Grid buttonsGrid = (Grid)VisualTreeHelper.GetChild(stackPanel, buttonsIndex);
                    buttonsGrid.Visibility = Visibility.Visible;

                    double horizontalOffset = tableScrollViewer.HorizontalOffset;
                    double verticalOffset = tableScrollViewer.VerticalOffset;
                    headersScrollViewer.ScrollToHorizontalOffset(horizontalOffset);
                    verticalHintsScrollViewer.ScrollToVerticalOffset(verticalOffset);
                };
                cellStackPanel.Children.Add(cellFieldTextBox);*/
                if(cellDataField is TextBox cellDataTextBox)
                {
                    cellDataTextBox.TextChanged += (object sender, TextChangedEventArgs e) =>
                    {
                        TextBox textBox = (TextBox)sender;
                        StackPanel stackPanel = (StackPanel)textBox.Parent;
                        int column = Grid.GetColumn(stackPanel);
                        int row = Grid.GetRow(stackPanel);

                        int buttonsIndex = VisualTreeHelper.GetChildrenCount(stackPanel) - 1;
                        Grid buttonsGrid = (Grid)VisualTreeHelper.GetChild(stackPanel, buttonsIndex);
                        buttonsGrid.Visibility = Visibility.Visible;

                        double horizontalOffset = tableScrollViewer.HorizontalOffset;
                        double verticalOffset = tableScrollViewer.VerticalOffset;
                        headersScrollViewer.ScrollToHorizontalOffset(horizontalOffset);
                        verticalHintsScrollViewer.ScrollToVerticalOffset(verticalOffset);
                    };
                }
                else if(cellDataField is ComboBox cellDataComboBox)
                {
                    cellDataComboBox.SelectionChanged += cellDataComboBox_SelectionChanged;
                }
                cellStackPanel.Children.Add(cellDataField);
            }
            // Додавання кнопок для збереження й відхилення редагування
            Grid buttonsGrid = new Grid();
            buttonsGrid.Visibility = Visibility.Collapsed;
            buttonsGrid.ColumnDefinitions.Add(new ColumnDefinition());
            buttonsGrid.ColumnDefinitions.Add(new ColumnDefinition());
            Button submitButton = new Button();
            Button cancelButton = new Button();

            submitButton.Content = "Редагувати";
            cancelButton.Content = "Скасувати";

            Grid.SetColumn(submitButton, 0);
            Grid.SetColumn(cancelButton, 1);

            submitButton.Click += submitButton_Clicked;

            cancelButton.Click += (object sender, RoutedEventArgs e) =>
            {
                Button button = (Button)sender;
                Grid buttonsGrid = (Grid)button.Parent;
                StackPanel cellStackPanel = (StackPanel)buttonsGrid.Parent;
                int rowIndex = Grid.GetRow(cellStackPanel);
                int columnIndex = Grid.GetColumn(cellStackPanel);
                RedrawCell(cellStackPanel, columnIndex, rowIndex);
            };

            buttonsGrid.Children.Add(submitButton);
            buttonsGrid.Children.Add(cancelButton);

            cellStackPanel.Children.Add(buttonsGrid);

            if (_verticalHeaders == null)
                return;
            for (int verticalHeaderIndex = 0; verticalHeaderIndex < 2; ++verticalHeaderIndex)
            {
                TextBox verticalHeaderTextBox = new TextBox();
                verticalHeaderTextBox.Text = _verticalHeaders[rowIndex, verticalHeaderIndex];
                Grid.SetRow(verticalHeaderTextBox, rowIndex);
                Grid.SetColumn(verticalHeaderTextBox, verticalHeaderIndex);
                verticalHeaderTextBox.IsReadOnly = true;
                verticalHintsGrid.Children.Add(verticalHeaderTextBox);
            }
            tableGrid.Children.Add(cellStackPanel);
        }

        public void RedrawCell(StackPanel cellStackPanel, int headerIndex, int rowIndex)
        {
            /*Dictionary<int, object> indexHeaderDictionary =
                _headerIndexDictionary.ToDictionary(x => x.Value, x => x.Key);*/
            object header = _indexHeaderDictionary[headerIndex];
            object[] columnData = _dataSource[header];
            cellStackPanel.Children.Clear();
            Grid.SetColumn(cellStackPanel, headerIndex);
            Grid.SetRow(cellStackPanel, rowIndex);
            // cellStackPanel.Margin = new System.Windows.Thickness(10);
            foreach (UIElement cellDataField in _dataConverter(columnData[rowIndex]))
            {
                /*TextBox cellFieldTextBox = new TextBox();
                cellFieldTextBox.Text = cellData;
                cellFieldTextBox.TextChanged += (object sender, TextChangedEventArgs e) =>
                {
                    TextBox textBox = (TextBox)sender;
                    StackPanel stackPanel = (StackPanel)textBox.Parent;
                    int column = Grid.GetColumn(stackPanel);
                    int row = Grid.GetRow(stackPanel);

                    int buttonsIndex = VisualTreeHelper.GetChildrenCount(stackPanel) - 1;
                    Grid buttonsGrid = (Grid)VisualTreeHelper.GetChild(stackPanel, buttonsIndex);
                    buttonsGrid.Visibility = Visibility.Visible;

                    double horizontalOffset = tableScrollViewer.HorizontalOffset;
                    double verticalOffset = tableScrollViewer.VerticalOffset;
                    headersScrollViewer.ScrollToHorizontalOffset(horizontalOffset);
                    verticalHintsScrollViewer.ScrollToVerticalOffset(verticalOffset);
                };
                cellStackPanel.Children.Add(cellFieldTextBox);*/
                if (cellDataField is TextBox cellDataTextBox)
                {
                    cellDataTextBox.TextChanged += (object sender, TextChangedEventArgs e) =>
                    {
                        TextBox textBox = (TextBox)sender;
                        StackPanel stackPanel = (StackPanel)textBox.Parent;
                        int column = Grid.GetColumn(stackPanel);
                        int row = Grid.GetRow(stackPanel);

                        int buttonsIndex = VisualTreeHelper.GetChildrenCount(stackPanel) - 1;
                        Grid buttonsGrid = (Grid)VisualTreeHelper.GetChild(stackPanel, buttonsIndex);
                        buttonsGrid.Visibility = Visibility.Visible;

                        double horizontalOffset = tableScrollViewer.HorizontalOffset;
                        double verticalOffset = tableScrollViewer.VerticalOffset;
                        headersScrollViewer.ScrollToHorizontalOffset(horizontalOffset);
                        verticalHintsScrollViewer.ScrollToVerticalOffset(verticalOffset);
                    };
                }
                else if (cellDataField is ComboBox cellDataComboBox)
                {
                    cellDataComboBox.SelectionChanged += cellDataComboBox_SelectionChanged;
                }
                cellStackPanel.Children.Add(cellDataField);
            }
            // Додавання кнопок для збереження й відхилення редагування
            Grid buttonsGrid = new Grid();
            buttonsGrid.Visibility = Visibility.Collapsed;
            buttonsGrid.ColumnDefinitions.Add(new ColumnDefinition());
            buttonsGrid.ColumnDefinitions.Add(new ColumnDefinition());
            Button submitButton = new Button();
            Button cancelButton = new Button();

            submitButton.Content = "Редагувати";
            cancelButton.Content = "Скасувати";

            Grid.SetColumn(submitButton, 0);
            Grid.SetColumn(cancelButton, 1);

            submitButton.Click += submitButton_Clicked;

            cancelButton.Click += (object sender, RoutedEventArgs e) =>
            {
                Button button = (Button)sender;
                Grid buttonsGrid = (Grid)button.Parent;
                StackPanel cellStackPanel = (StackPanel)buttonsGrid.Parent;
                int rowIndex = Grid.GetRow(cellStackPanel);
                int columnIndex = Grid.GetColumn(cellStackPanel);
                RedrawCell(cellStackPanel, columnIndex, rowIndex);
            };

            buttonsGrid.Children.Add(submitButton);
            buttonsGrid.Children.Add(cancelButton);

            cellStackPanel.Children.Add(buttonsGrid);

            if (_verticalHeaders == null)
                return;
            for (int verticalHeaderIndex = 0; verticalHeaderIndex < 2; ++verticalHeaderIndex)
            {
                TextBox verticalHeaderTextBox = new TextBox();
                verticalHeaderTextBox.Text = _verticalHeaders[rowIndex, verticalHeaderIndex];
                Grid.SetRow(verticalHeaderTextBox, rowIndex);
                Grid.SetColumn(verticalHeaderTextBox, verticalHeaderIndex);
                verticalHeaderTextBox.IsReadOnly = true;
                verticalHintsGrid.Children.Add(verticalHeaderTextBox);
            }
        }

        double _cellWidth;
        double CellWidth
        {
            get
            {
                return _cellWidth;
            }
            set
            {
                _cellWidth = value;
            }
        }

        double _cellHeight;
        double CellHeight
        {
            get
            {
                return _cellHeight;
            }
            set
            {
                _cellHeight = value;
            }
        }

        bool scheduleChanged;

        string[,]? _verticalHeaders;
        public string[,]? VerticalHeadersF
        {
            get
            {
                return _verticalHeaders;
            }
            set
            {
                _verticalHeaders = value;
            }
        }

        private void tableScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            ScrollViewer scrollViewer = (ScrollViewer)sender;
            double horizontalOffset = e.HorizontalOffset;
            double verticalOffset = e.VerticalOffset;
            headersScrollViewer.ScrollToHorizontalOffset(horizontalOffset);
            verticalHintsScrollViewer.ScrollToVerticalOffset(verticalOffset);
        }

        public void RefreshGroups()
        {
            ScheduleDBConnection scheduleDBConnection = ScheduleDBConnection.GetInstance();
            _groups = scheduleDBConnection.GetAllGroups().ToArray();
        }

        private void cellDataComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            ComboBox comboBox = (ComboBox)sender;
            StackPanel stackPanel = (StackPanel)comboBox.Parent;
            int column = Grid.GetColumn(stackPanel);
            int row = Grid.GetRow(stackPanel);

            if(comboBox.SelectedValue=="Додати викладача")
            {
                EditLecturersWindow editLecturersWindow = new EditLecturersWindow();
                editLecturersWindow.Owner = this;
                editLecturersWindow.ShowDialog();
                comboBox.SelectedValue = "";
                return;
            }
            if(comboBox.SelectedValue == "Додати дисципліну")
            {
                EditSubjectsWindow editSubjectsWindow = new EditSubjectsWindow();
                editSubjectsWindow.Owner = this;
                editSubjectsWindow.ShowDialog();
                comboBox.SelectedValue = "";
                return;
            }

            int buttonsIndex = VisualTreeHelper.GetChildrenCount(stackPanel) - 1;
            Grid buttonsGrid = (Grid)VisualTreeHelper.GetChild(stackPanel, buttonsIndex);
            buttonsGrid.Visibility = Visibility.Visible;
            ComboBox subjectTextBox = (ComboBox)VisualTreeHelper.GetChild(stackPanel, 0);
            subjectTextBox.IsReadOnly = false;

            double horizontalOffset = tableScrollViewer.HorizontalOffset;
            double verticalOffset = tableScrollViewer.VerticalOffset;
            headersScrollViewer.ScrollToHorizontalOffset(horizontalOffset);
            verticalHintsScrollViewer.ScrollToVerticalOffset(verticalOffset);
        }
        private void Menu_Data_EditGroups(object sender, RoutedEventArgs args)
        {
            EditGroupsWindow editGroupsWindow = new EditGroupsWindow();
            editGroupsWindow.Owner = this;
            editGroupsWindow.ShowDialog();
            RefreshGroups();
            // TODO: refresh `table` field
            DateTime weekDay = (DateTime)weekDatePicker.SelectedDate;
            DateTime weekMonday = weekDay.AddDays(-(int)weekDay.DayOfWeek + 1);
            ShowWeek(weekMonday);
            UpdateView();
        }

        private void Menu_Data_EditLecturers(object sender, RoutedEventArgs args)
        {
            EditLecturersWindow editLecturersWindow = new EditLecturersWindow();
            editLecturersWindow.Owner = this;
            editLecturersWindow.ShowDialog();
        }
        private void Menu_Data_EditSubjects(object sender, RoutedEventArgs args)
        {
            EditSubjectsWindow editSubjectsWindow = new EditSubjectsWindow();
            editSubjectsWindow.Owner = this;
            editSubjectsWindow.ShowDialog();
        }
        private void Menu_Data_EditClassrooms(object sender, RoutedEventArgs args)
        {
            EditClassroomsWindow editClassroomsWindow = new EditClassroomsWindow();
            editClassroomsWindow.Owner = this;
            editClassroomsWindow.ShowDialog();
        }
        private void Menu_Data_EditSubjectGroupLecturer(object sender, RoutedEventArgs args)
        {
            var editRelationsWindow = new EditLecturerGroupSubjectWindow();
            editRelationsWindow.Owner = this;
            editRelationsWindow.ShowDialog();
        }

        private void Menu_Data_EditLecturersAvailability(object sender, RoutedEventArgs e)
        {
            var editLecturerTimeConstraintWindow = new LecturerTimeConstraintWindow();
            editLecturerTimeConstraintWindow.Owner = this;
            editLecturerTimeConstraintWindow.ShowDialog();
        }

        private void Menu_Data_EditMaxSubjectPerWeek(object sender, RoutedEventArgs e)
        {
            var maxSubjectPerWeekWindow = new MaxSubjectPerWeekConstraintWindow();
            maxSubjectPerWeekWindow.Owner = this;
            maxSubjectPerWeekWindow.ShowDialog();
        }

        private void weekDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            DateTime? weekDay = weekDatePicker.SelectedDate;
            if (weekDay != null)
            {
                DateTime weekMonday = weekDay.Value.AddDays(-(int)weekDay.Value.DayOfWeek + 1);
                ShowWeek(weekMonday);
            }
        }

        private void submitButton_Clicked(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            Grid buttonsGrid = (Grid)button.Parent;
            StackPanel cellStackPanel = (StackPanel)buttonsGrid.Parent;
            int rowIndex = Grid.GetRow(cellStackPanel);
            int columnIndex = Grid.GetColumn(cellStackPanel);
            object header = _indexHeaderDictionary[columnIndex];
            object updatedData = _dataChangeConverter(cellStackPanel.Children.Cast<UIElement>().ToArray());
            if (updatedData == null)
                return;
            _dataChange(table, updatedData, header, rowIndex);
            RedrawCell(cellStackPanel, columnIndex, rowIndex);

            Group group = (Group)header;
            int dayIndex = rowIndex / 5;
            int lessonIndex = rowIndex % 5;
            Table.Position cellsPosition = new Table.Position(group, dayIndex, lessonIndex);
            var scheduleDBConnection = ScheduleDBConnection.GetInstance();
            DateTime weekDate = weekDatePicker.SelectedDate.Value;
            DateTime dayDate = weekDate.AddDays(dayIndex);
            scheduleDBConnection.UpdateScheduleCell(table[cellsPosition], dayDate, lessonIndex + 1, group);

            List<ScheduleCheckResult> errors = table.Check();
            if (errors.Count > 0)
            {
                _errorsWindow.ShowErrors(errors);
                _errorsWindow.Owner = this;
                _errorsWindow.Show();
            }
        }
        ErrorsWindow _errorsWindow;

        private void Window_Closed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }
        /*void SaveSchedule()
{
   ScheduleDBConnection scheduleDBConnection = ScheduleDBConnection.GetInstance();
   scheduleDBConnection.UpdateScheduleCell()
}*/
    }
}
