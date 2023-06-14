using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
            RefreshGroups();
            CellWidth = 200;
            CellHeight = 75;

            table = new Table(_groups, 5, 5);
            VerticalHeadersF = VerticalHeaders.CreateDayNumberHeaders(1, 5, 5);
            Table.Position position = new Table.Position(new Group("Something 1"), 0, 3);

            /*table[position] = new Table.Cell(new Table.SubCell("disc", new Lecturer("lecturer1", "Lecturovich", "Lecturenko", ), null));
            position = new Table.Position(new Group("Something 4"), 0, 1);
            table[position] = new Table.Cell(new Table.SubCell("Біологія", new Lecturer("lecturer2", "Lecturovich", "Lecturenko"), 124));
            position = new Table.Position(new Group("Something 5"), 4, 0);
            table[position] = new Table.Cell(new Table.SubCell("3rd discipline", new Lecturer("lecturer3", "Lecturovich", "Lecturenko"), 111));*/

            var dataSource = new Dictionary<object, object[]>();
            foreach (var kvp in table.Content)
            {
                dataSource.Add((object)kvp.Key, (object[])kvp.Value);
            }
            DataSource = dataSource;
            DataConverter = (object obj) => {
                if (obj is Table.Cell cell)
                {
                    /*string[] result = {
                        cell.first.discipline,
                        cell.first.lecturer.firstName,
                        (cell.first.classroom?.ToString() ?? "")
                    };*/
                    ComboBox disciplineComboBox = new ComboBox();
                    disciplineComboBox.Items.Add(cell.first.subject?.title ?? "");
                    disciplineComboBox.SelectedItem = cell.first.subject.title;
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
                        string[] lecturerName = (lecturerComboBox.SelectedValue as string).Split(" ");
                        Lecturer lecturer = scheduleDBConnection.GetLecturer(lecturerName[0], lecturerName[2], lecturerName[1]);
                        List<Subject> subjects = scheduleDBConnection.GetPossibleSubjects(group, lecturer);
                        subjectComboBox.Items.Add("Додати дисципліну");
                        subjectComboBox.Items.Add("");
                        foreach (Subject subject in subjects)
                        {
                            subjectComboBox.Items.Add(subject.title);
                        }
                    };
                    ComboBox lecturerComboBox = new ComboBox();
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
                            string lecturerFullName = lecturer.firstName + " " + lecturer.lastName + " " + lecturer.middleName;
                            lecturerComboBox.Items.Add(lecturerFullName);
                        }
                        lecturerComboBox.SelectedItem = selectedItem;
                    };
                    TextBox classRoomTextBox = new TextBox();
                    classRoomTextBox.Text = cell.first.classroom.title;
                    UIElement[] result = {
                        disciplineComboBox,
                        lecturerComboBox,
                        classRoomTextBox
                    };
                    return result;
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
                Table.Cell result = new Table.Cell();
                // cell.first.discipline,
                // cell.first.lecturer.firstName,
                // (cell.first.classroom?.ToString() ?? "")
                string subject = (string)(dataElements[0] as ComboBox).SelectedItem;
                result.first.subject = new Subject(subject);
                ComboBox lecturerComboBox = (ComboBox)dataElements[1];
                string lecturerNameStr = (string)lecturerComboBox.SelectedItem;
                string[] lecturerName = lecturerNameStr.Split(" ");
                result.first.lecturer = new Lecturer(lecturerName[0], lecturerName[1], lecturerName[2], new Period[] { });
                string classroomFieldValue = (string)(dataElements[2] as TextBox).Text;
                if (classroomFieldValue != "")
                    result.first.classroom = new Classroom(classroomFieldValue);
                else
                    result.first.classroom = null;
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
                // Add group button:
                Button addGroupButton = new Button();
                addGroupButton.Content = "Додати групу";
                int addGroupButtonIndex = _dataSource.Count;
                Grid.SetColumn(addGroupButton, addGroupButtonIndex);

                addGroupButton.Click += Menu_Data_EditGroups;

                headersGrid.Children.Add(addGroupButton);
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
            _headerIndexDictionary = _headersSorting(_dataSource.Keys.ToArray());
            _indexHeaderDictionary = _headerIndexDictionary.ToDictionary(x => x.Value, x => x.Key);
            int columnDataLength = 0;
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

            submitButton.Click += (object sender, RoutedEventArgs e) =>
            {
                Button button = (Button)sender;
                Grid buttonsGrid = (Grid)button.Parent;
                StackPanel cellStackPanel = (StackPanel)buttonsGrid.Parent;
                int rowIndex = Grid.GetRow(cellStackPanel);
                int columnIndex = Grid.GetColumn(cellStackPanel);
                object header = _indexHeaderDictionary[columnIndex];
                object updatedData = _dataChangeConverter(cellStackPanel.Children.Cast<UIElement>().ToArray());
                _dataChange(table, updatedData, header, rowIndex);
                RedrawCell(cellStackPanel, columnIndex, rowIndex);
            };

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

            submitButton.Click += (object sender, RoutedEventArgs e) =>
            {
                Button button = (Button)sender;
                Grid buttonsGrid = (Grid)button.Parent;
                StackPanel cellStackPanel = (StackPanel)buttonsGrid.Parent;
                int rowIndex = Grid.GetRow(cellStackPanel);
                int columnIndex = Grid.GetColumn(cellStackPanel);
                object header = _indexHeaderDictionary[columnIndex];
                object updatedData = _dataChangeConverter(cellStackPanel.Children.Cast<UIElement>().ToArray());
                _dataChange(table, updatedData, header, rowIndex);
                RedrawCell(cellStackPanel, columnIndex, rowIndex);
            };

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
        private void Menu_Data_EditSubjectGroupLecturer(object sender, RoutedEventArgs args)
        {
            var editRelationsWindow = new EditLecturerGroupSubjectWindow();
            editRelationsWindow.Owner = this;
            editRelationsWindow.ShowDialog();
        }
    }
}
