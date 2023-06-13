using System.Collections;
using System.Collections.Generic;

namespace schedule
{
    enum Collision
    {
        Ok,
        SameTeacher,
        SameClassroom,
    }
    
    struct CheckResult
    {
        public Collision collision;
        public Table.Position? position;

        public CheckResult(Collision collision, Table.Position? position)
        {
            this.collision = collision;
            this.position = position;
        }
    }
    
    class Table
    {
        public class Cell
        {
            public SubCell first;
            public SubCell? second;

            public Cell() : this(null)
            {
            }

            public Cell(SubCell first, SubCell? second = null)
            {
                this.first = first;
                this.second = second;
            }
        }
        
        public class SubCell
        {
            public long? id;
            public Subject subject;
            public Lecturer lecturer;
            public Classroom classroom;
            public bool isLabWork;
            public SubCell? anotherHalf;

            public SubCell()
            {
                id = null;
                subject = new Subject();
                lecturer = new Lecturer();
                classroom = new Classroom();
                isLabWork = false;
            }
            
            public SubCell(long? id, Subject subject, Lecturer lecturer, Classroom classroom, bool isLabWork, SubCell? anotherHalf = null)
            {
                this.subject = subject;
                this.lecturer = lecturer;
                this.classroom = classroom;
                this.isLabWork = isLabWork;
                this.anotherHalf = anotherHalf;
            }
        }

        public class Position
        {
            public Group group;
            public int dayNumber;
            public int lessonNumber;

            public Position(Group group, int dayNumber, int lessonNumber)
            {
                this.group = group;
                this.dayNumber = dayNumber;
                this.lessonNumber = lessonNumber;
            }
        }

        private Dictionary<Group, Cell[]> _content;
        
        public int MaxLessonsPerDay { private set; get; }
        public int WorkingDays { private set; get; }
        public int GroupsCount => _content.Count;
        public Dictionary<Group, Cell[]> Content => _content;

        public Table(ICollection<Group> groups, int workingDays, int maxLessonsPerDay)
        {
            MaxLessonsPerDay = maxLessonsPerDay;
            WorkingDays = workingDays;
            _content = new Dictionary<Group, Cell[]>();
            foreach(Group group in groups)
            {
                int groupCellsQuantity = MaxLessonsPerDay * WorkingDays;
                _content.Add(group, new Cell[groupCellsQuantity]);
                for(int i = 0; i < groupCellsQuantity; ++i)
                {
                    _content[group][i] = new Cell();
                    _content[group][i].first = new SubCell();
                }
            }
        }
        
        public Table(int workingDays, int maxLessonsPerDay)
        {
            MaxLessonsPerDay = maxLessonsPerDay;
            WorkingDays = workingDays;
            _content = new Dictionary<Group, Cell[]>();
        }

        public void AddGroup(Group group)
        {
            Cell[] cells = new Cell[MaxLessonsPerDay * WorkingDays];
            for (int i = 0; i < cells.Length; ++i)
            {
                cells[i] = new Cell();
            }
            _content.Add(group, cells);
        }

        public Cell this[Group group, int dayNumber, int lessonNumber]
        {
            get { return _content[group][dayNumber * MaxLessonsPerDay + lessonNumber]; }
            set { _content[group][dayNumber * MaxLessonsPerDay + lessonNumber] = value; }
        }

        public Cell this[Position position]
        {
            get { return _content[position.group][position.dayNumber * MaxLessonsPerDay + position.lessonNumber]; }
            set { _content[position.group][position.dayNumber * MaxLessonsPerDay + position.lessonNumber] = value; }
        }

        public SubCell this[Position position, int subgroupNumber]
        {
            get 
            { 
                return subgroupNumber == 1 ? 
                _content[position.group][position.dayNumber * MaxLessonsPerDay + position.lessonNumber].first : 
                _content[position.group][position.dayNumber * MaxLessonsPerDay + position.lessonNumber].second;
            }
            set 
            { 
                if (subgroupNumber == 1)
                    _content[position.group][position.dayNumber * MaxLessonsPerDay + position.lessonNumber].first = value;
                else
                    _content[position.group][position.dayNumber * MaxLessonsPerDay + position.lessonNumber].second = value;
            }
        }

        public Dictionary<Group, Cell> Row(int dayNumber, int lessonNumber)
        {
            var row = new Dictionary<Group, Cell>();
            foreach (var pair in _content)
            {
                row.Add(pair.Key, pair.Value[dayNumber * MaxLessonsPerDay + lessonNumber]);
            }
            return row;
        }

        public CheckResult Check(int dayNumber, int lessonNumber, Cell cell)
        {
            /*foreach (var pair in _content)
            {
                Cell other = pair.Value[dayNumber * MaxLessonsPerDay + lessonNumber];
                if (other.teacher == cell.teacher)
                    return new CheckResult(Collision.SameTeacher, new Position(pair.Key, dayNumber, lessonNumber));
                if (cell.classroom != null && other.classroom != null && other.classroom == cell.classroom)
                    return new CheckResult(Collision.SameClassroom, new Position(pair.Key, dayNumber, lessonNumber));
            }*/
            return new CheckResult(Collision.Ok, null);
        }
    }
}
