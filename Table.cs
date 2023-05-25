namespace schedule
{
    struct Cell
    {
        public string discipline;
        public string teacher;
        public int classroom;
    }

    enum Collision
    {
        Ok,
        SameTeacher,
        SameClassroom,
    }

    struct CheckResult
    {
        public Collision collision;
        public (int, int) position;
    }
    
    class Table
    {
        private Cell[,] _content;
        private int _groups;

        public int Groups => _groups;
        
        public Table(int groups)
        {
            _content = new Cell[25, groups];
            _groups = groups;
        }

        public Cell this[int row, int col]
        {
            get { return _content[row, col]; }
            set { _content[row, col] = value; }
        }

        public CheckResult Check(int row, int col, Cell cell)
        {
            for (int i = 0; i < _groups; ++i)
            {
                if (i == col) continue;
                if (_content[row, i].teacher == cell.teacher)
                    return new CheckResult {collision = Collision.SameTeacher, position = (row, i)};
                if (_content[row, i].classroom == cell.classroom)
                    return new CheckResult {collision = Collision.SameClassroom, position = (row, i)};
            }
            return new CheckResult {collision = Collision.Ok, position = (0, 0)};
        }
    }
}
