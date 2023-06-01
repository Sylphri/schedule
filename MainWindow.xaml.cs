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
            Group[] groups = {
                new Group("Something 1"),
                new Group("Something 2"),
                new Group("Something 3"),
                new Group("Something 4"),
                new Group("Something 5"),
                new Group("Something 6"),
                new Group("Something 7"),
                new Group("Something 8"),
                new Group("Something 9"),
                new Group("Something 10"),
                new Group("Something 11"),
                new Group("Something 12"),
                new Group("Something 13"),
                new Group("Something 14"),
                new Group("Something 15"),
                new Group("Something 16"),
                new Group("Something 17"),
                new Group("Something 18"),
                new Group("Something 19"),
                new Group("Something 20"),
                new Group("Something 21"),
                new Group("Something 22"),
                new Group("Something 23"),
                new Group("Something 24"),
                new Group("Something 25"),
                new Group("Something 26"),
                new Group("Something 27"),
                new Group("Something 28"),
                new Group("Something 29"),
                new Group("Something 30")
            };
            CellWidth = 200;
            CellHeight = 75;

            table = new Table(groups, 5, 5);
            VerticalHeadersF = VerticalHeaders.CreateDayNumberHeaders(1, 5, 5);
            Table.Position position = new Table.Position(new Group("Something 1"), 0, 3);
            table[position] = new Table.Cell(
                new Table.SubCell(
                    "disc", 
                    new Lecturer("lecturer1", "", "", new Period[6]), 
                    new Classroom(100, false, false)
                )
            );
            position = new Table.Position(new Group("Something 4"), 0, 1);
            table[position] = new Table.Cell(
                new Table.SubCell(
                    "biology", 
                    new Lecturer("lecturer2", "", "", new Period[6]), 
                    new Classroom(124, false, false)
                )
            );
            position = new Table.Position(new Group("Something 5"), 4, 0);
            table[position] = new Table.Cell(
                new Table.SubCell(
                    "3rd discipline", 
                    new Lecturer("lecturer3", "", "", new Period[6]), 
                    new Classroom(111, false, false)
                )
            );

            var dataSource = new Dictionary<object, object[]>();
            foreach (var kvp in table.Content)
            {
                dataSource.Add((object)kvp.Key, (object[])kvp.Value);
            }
            DataSource = dataSource;
            DataConverter = (object obj) => {
                if(obj is Table.Cell cell)
                {
                    string[] result = {
                        cell.first.discipline,
                        cell.first.lecturer.firstName,
                        cell.first.classroom.number.ToString()
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
                if(objects.OfType<Group>().Any())
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
            UpdateView();
            /*ColsQuantity = groups.Length;
            RowsQuantity = 5*5;
            Headers = groups;*/
        }
        Table table;
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
                for (int i = 0; i < _dataSource.Count; ++i)
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
                for (int i = 0; i<rowsQuantity; i++)
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
        /// Делегат розділення об'єктів-клітинок на поля в string
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public delegate string[] DataConverterDelegate(object obj);
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
            Dictionary<object, int> headerIndexDictionary = _headersSorting(_dataSource.Keys.ToArray());
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
                int headerIndex = headerIndexDictionary[header];
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
                    StackPanel cellStackPanel = new StackPanel();
                    Grid.SetColumn(cellStackPanel, headerIndex);
                    Grid.SetRow(cellStackPanel, iy);
                    // cellStackPanel.Margin = new System.Windows.Thickness(10);
                    foreach (string cellData in _dataConverter(columnData[iy]))
                    {
                        TextBox cellFieldTextBox = new TextBox();
                        cellFieldTextBox.Text = cellData;
                        cellStackPanel.Children.Add(cellFieldTextBox);
                    }
                    if (_verticalHeaders == null)
                        continue;
                    for(int verticalHeaderIndex = 0; verticalHeaderIndex<2; ++verticalHeaderIndex)
                    {
                        TextBox verticalHeaderTextBox = new TextBox();
                        verticalHeaderTextBox.Text = _verticalHeaders[iy, verticalHeaderIndex];
                        Grid.SetRow(verticalHeaderTextBox, iy);
                        Grid.SetColumn(verticalHeaderTextBox, verticalHeaderIndex);
                        verticalHeaderTextBox.IsReadOnly = true;
                        verticalHintsGrid.Children.Add(verticalHeaderTextBox);
                    }
                    tableGrid.Children.Add(cellStackPanel);
                }
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
            double horizontalOffset = e.HorizontalOffset;
            double verticalOffset = e.VerticalOffset;
            headersScrollViewer.ScrollToHorizontalOffset(horizontalOffset);
            verticalHintsScrollViewer.ScrollToVerticalOffset(verticalOffset);
        }
    }
}
