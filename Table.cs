using System.Collections.Generic;

namespace schedule
{
    enum Collision
    {
        Ok,
        SameTeacher,
        SameClassroom,
    }

    class Position
    {
        public string group;
        public int dayNumber;
        public int lessonNumber;
        
        public Position(string group, int dayNumber, int lessonNumber)
        {
            this.group = group;
            this.dayNumber = dayNumber;
            this.lessonNumber = lessonNumber;
        }
    }
    
    struct CheckResult
    {
        public Collision collision;
        public Position? position;

        public CheckResult(Collision collision, Position? position)
        {
            this.collision = collision;
            this.position = position;
        }
    }
    
    class Table
    {
        public class Cell
        {
            public string discipline;
            public string teacher;
            public int? classroom;

            public Cell(string discipline = "", string teacher = "", int? classroom = null)
            {
                this.discipline = discipline;
                this.teacher = teacher;
                this.classroom = classroom;
            }
        }
        
        private Dictionary<string, Cell[]> _content;
        
        public int MaxLessonsPerDay { private set; get; }
        public int WorkingDays { private set; get; }
        public int GroupsCount => _content.Count;
        
        public Table(int maxLessonsPerDay, int workingDays)
        {
            MaxLessonsPerDay = maxLessonsPerDay;
            WorkingDays = workingDays;
            _content = new Dictionary<string, Cell[]>();
        }

        public void AddGroup(string group)
        {
            Cell[] cells = new Cell[MaxLessonsPerDay * WorkingDays];
            for (int i = 0; i < cells.Length; ++i)
            {
                cells[i] = new Cell();
            }
            _content.Add(group, cells);
        }

        public Cell this[string group, int dayNumber, int lessonNumber]
        {
            get { return _content[group][dayNumber * MaxLessonsPerDay + lessonNumber]; }
            set { _content[group][dayNumber * MaxLessonsPerDay + lessonNumber] = value; }
        }

        public Dictionary<string, Cell> Row(int dayNumber, int lessonNumber)
        {
            var row = new Dictionary<string, Cell>();
            foreach (var pair in _content)
            {
                row.Add(pair.Key, pair.Value[dayNumber * MaxLessonsPerDay + lessonNumber]);
            }
            return row;
        }

        public CheckResult Check(int dayNumber, int lessonNumber, Cell cell)
        {
            foreach (var pair in _content)
            {
                Cell other = pair.Value[dayNumber * MaxLessonsPerDay + lessonNumber];
                if (other.teacher == cell.teacher)
                    return new CheckResult(Collision.SameTeacher, new Position(pair.Key, dayNumber, lessonNumber));
                if (cell.classroom != null && other.classroom != null && other.classroom == cell.classroom)
                    return new CheckResult(Collision.SameClassroom, new Position(pair.Key, dayNumber, lessonNumber));
            }
            return new CheckResult(Collision.Ok, null);
        }
    }
}
