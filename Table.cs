using System.Collections.Generic;
using System.Linq;

namespace schedule
{
    class Table
    {
        public class Cell
        {
            public SubCell first;
            public SubCell second;
            public bool isSplitted;

            public Cell(SubCell first = null, SubCell second = null)
            {
                this.first = first;
                this.second = second;
                isSplitted = false;
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
            
            public SubCell(long? id, Subject subject, Lecturer lecturer, Classroom classroom, bool isLabWork, SubCell? anotherHalf = null)
            {
                this.id = id;
                this.subject = subject;
                this.lecturer = lecturer;
                this.classroom = classroom;
                this.isLabWork = isLabWork;
                this.anotherHalf = anotherHalf;
            }

            public SubCell(Subject subject, Lecturer lecturer, Classroom classroom, bool isLabWork, SubCell? anotherHalf = null)
            {
                this.id = null;
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
            _checkers = new List<CheckScheduleDelegate>();
            MaxLessonsPerDay = maxLessonsPerDay;
            WorkingDays = workingDays;
            _content = new Dictionary<Group, Cell[]>();
            _checkers = new List<CheckScheduleDelegate>();
            foreach(Group group in groups)
            {
                int groupCellsQuantity = MaxLessonsPerDay * WorkingDays;
                _content.Add(group, new Cell[groupCellsQuantity]);
                for(int i = 0; i < groupCellsQuantity; ++i)
                {
                    _content[group][i] = new Cell();
                }
            }
        }
        
        public Table(int workingDays, int maxLessonsPerDay)
        {
            MaxLessonsPerDay = maxLessonsPerDay;
            WorkingDays = workingDays;
            _content = new Dictionary<Group, Cell[]>();
            _checkers = new List<CheckScheduleDelegate>();
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

        List<CheckScheduleDelegate> _checkers;
        public List<CheckScheduleDelegate> Checkers
        {
            get { return _checkers; }
            set { _checkers = value; }
        }

        public void AddDefaultCheckers()
        {
            _checkers.Add((Table table) =>
            {
                string errorName = "Один вчитель на кілька груп";
                Group[] groups = _content.Keys.ToArray();
                for (int i = 0; i < MaxLessonsPerDay * WorkingDays; i++)
                {
                    for (int j = 0; j < groups.Length; j++)
                    {
                        if (_content[groups[j]][i].first != null && _content[groups[j]][i].second != null &&
                            _content[groups[j]][i].first.lecturer.id == _content[groups[j]][i].second.lecturer.id)
                            return new ScheduleCheckResult(errorName, "");
                        for (int k = j + 1; k < groups.Length; k++)
                        {
                            if (_content[groups[j]][i].first != null && _content[groups[k]][i].first != null && 
                                _content[groups[k]][i].first.lecturer.id == _content[groups[j]][i].first.lecturer.id)
                                return new ScheduleCheckResult(errorName, "");
                            if (_content[groups[j]][i].second != null && _content[groups[k]][i].first != null && 
                                _content[groups[k]][i].first.lecturer.id == _content[groups[j]][i].second.lecturer.id)
                                return new ScheduleCheckResult(errorName, "");
                            if (_content[groups[j]][i].first != null && _content[groups[k]][i].second != null && 
                                _content[groups[k]][i].second.lecturer.id == _content[groups[j]][i].first.lecturer.id)
                                return new ScheduleCheckResult(errorName, "");
                            if (_content[groups[j]][i].second != null && _content[groups[k]][i].second != null && 
                                _content[groups[k]][i].second.lecturer.id == _content[groups[j]][i].second.lecturer.id)
                                return new ScheduleCheckResult(errorName, "");
                        }
                    }
                }
                return null;
            });
            _checkers.Add((Table table) =>
            {
                string errorName = "Одна аудиторія на кілька груп";
                Group[] groups = _content.Keys.ToArray();
                for (int rowIndex = 0; rowIndex < MaxLessonsPerDay * WorkingDays; rowIndex++)
                {
                    for (int groupIndex = 0; groupIndex < groups.Length; groupIndex++)
                    {
                        /*if (_content[groups[groupIndex]][rowIndex].first == null &&
                        _content[groups[groupIndex]][rowIndex].second == null)
                        {
                            return null;
                        }
                        if (_content[groups[groupIndex]][rowIndex].first.classroom == null &&
                        _content[groups[groupIndex]][rowIndex].second.classroom == null)
                        {
                            return null;
                        }*/
                        if (_content[groups[groupIndex]][rowIndex].first != null && _content[groups[groupIndex]][rowIndex].second != null &&
                            _content[groups[groupIndex]][rowIndex].first.classroom != null && _content[groups[groupIndex]][rowIndex].second.classroom != null &&
                            _content[groups[groupIndex]][rowIndex].first.classroom.id == _content[groups[groupIndex]][rowIndex].second.classroom.id)
                            return new ScheduleCheckResult(errorName, "");

                        for (int otherGroupIndex = groupIndex + 1; otherGroupIndex < groups.Length; otherGroupIndex++)
                        {
                            if (_content[groups[groupIndex]][rowIndex].first != null && _content[groups[otherGroupIndex]][rowIndex].first != null &&
                                _content[groups[groupIndex]][rowIndex].first.classroom != null && _content[groups[otherGroupIndex]][rowIndex].first.classroom != null &&
                                _content[groups[otherGroupIndex]][rowIndex].first.lecturer.id == _content[groups[groupIndex]][rowIndex].first.lecturer.id)
                                return new ScheduleCheckResult(errorName, "");
                            if (_content[groups[groupIndex]][rowIndex].second != null && _content[groups[otherGroupIndex]][rowIndex].first != null &&
                                _content[groups[groupIndex]][rowIndex].second.classroom != null && _content[groups[otherGroupIndex]][rowIndex].first.classroom != null &&
                                _content[groups[otherGroupIndex]][rowIndex].first.lecturer.id == _content[groups[groupIndex]][rowIndex].second.lecturer.id)
                                return new ScheduleCheckResult(errorName, "");
                            if (_content[groups[groupIndex]][rowIndex].first != null && _content[groups[otherGroupIndex]][rowIndex].second != null &&
                                _content[groups[groupIndex]][rowIndex].first.classroom != null && _content[groups[otherGroupIndex]][rowIndex].second.classroom != null &&
                                _content[groups[otherGroupIndex]][rowIndex].second.lecturer.id == _content[groups[groupIndex]][rowIndex].first.lecturer.id)
                                return new ScheduleCheckResult(errorName, "");
                            if (_content[groups[groupIndex]][rowIndex].second != null && _content[groups[otherGroupIndex]][rowIndex].second != null &&
                                _content[groups[groupIndex]][rowIndex].second.classroom != null && _content[groups[otherGroupIndex]][rowIndex].second.classroom != null &&
                                _content[groups[otherGroupIndex]][rowIndex].second.lecturer.id == _content[groups[groupIndex]][rowIndex].second.lecturer.id)
                                return new ScheduleCheckResult(errorName, "");
                        }
                    }
                }
                return null;
            });
            _checkers.Add((Table table) =>
            {
                string errorName = "Викладач не доступний у цей час";
                Group[] groups = _content.Keys.ToArray();
                for (int i = 0; i < MaxLessonsPerDay * WorkingDays; i++)
                {
                    int dayNumber = i / MaxLessonsPerDay;
                    int lessonNumber = (i % MaxLessonsPerDay)+1;
                    for (int j = 0; j < groups.Length - 1; j++)
                    {
                        if (_content[groups[j]][i].first != null && 
                            (_content[groups[j]][i].first.lecturer.availability[dayNumber].start > lessonNumber ||
                             _content[groups[j]][i].first.lecturer.availability[dayNumber].end < lessonNumber))
                            return new ScheduleCheckResult(errorName, "");
                        if (_content[groups[j]][i].second != null && 
                            (_content[groups[j]][i].second.lecturer.availability[dayNumber].start > lessonNumber ||
                             _content[groups[j]][i].second.lecturer.availability[dayNumber].end < lessonNumber))
                            return new ScheduleCheckResult(errorName, "");
                    }
                }
                return null;
            });
            
            _checkers.Add((Table table) =>
            {
                string moreErrorName = "Забагато пар по предмету";
                string resultMessage = "";
                Group[] groups = _content.Keys.ToArray();
                for (int groupIndex = 0; groupIndex < groups.Length; groupIndex++)
                {
                    Dictionary<Subject, int> subjectLessonsCount = new Dictionary<Subject, int>();
                    List<Table.SubCell> ignore = new List<Table.SubCell>(); // already checked?
                    for (int rowIndex = 0; rowIndex < MaxLessonsPerDay * WorkingDays; rowIndex++)
                    {
                        if (_content[groups[groupIndex]][rowIndex].first != null)
                        {
                            if (ignore.Contains(_content[groups[groupIndex]][rowIndex].first))
                            {
                                ignore.Remove(_content[groups[groupIndex]][rowIndex].first);
                            }
                            else
                            {
                                if (subjectLessonsCount.ContainsKey(_content[groups[groupIndex]][rowIndex].first.subject))
                                    subjectLessonsCount[_content[groups[groupIndex]][rowIndex].first.subject] += 1;
                                else
                                    subjectLessonsCount.Add(_content[groups[groupIndex]][rowIndex].first.subject, 1);
                            }
                        }
                        if (_content[groups[groupIndex]][rowIndex].second != null)
                        {
                            if (ignore.Contains(_content[groups[groupIndex]][rowIndex].second))
                            {
                                ignore.Remove(_content[groups[groupIndex]][rowIndex].second);
                            }
                            else
                            {
                                if (subjectLessonsCount.ContainsKey(_content[groups[groupIndex]][rowIndex].second.subject))
                                    subjectLessonsCount[_content[groups[groupIndex]][rowIndex].second.subject] += 1;
                                else
                                    subjectLessonsCount.Add(_content[groups[groupIndex]][rowIndex].second.subject, 1);
                            }
                        }
                    }
                    foreach (var pair in subjectLessonsCount)
                    {
                        if (pair.Value > pair.Key.lessonsPerWeek)
                            return new ScheduleCheckResult(moreErrorName, "");
                    }
                }
                return null;
            });
        }

        public List<ScheduleCheckResult> Check()
        {
            List<ScheduleCheckResult> result = new List<ScheduleCheckResult>();

            foreach(CheckScheduleDelegate checker in _checkers)
            {
                ScheduleCheckResult checkResult = checker(this);
                if (checkResult!=null)
                    result.Add(checkResult);
            }

            return result;
        }
    }
}
