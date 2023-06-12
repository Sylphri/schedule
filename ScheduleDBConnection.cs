using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Windows;
using System;

namespace schedule
{
    class ScheduleDBConnection
    {
        private static ScheduleDBConnection? _instance;
        
        private SqlConnection _connection;

        private ScheduleDBConnection() 
        {
            _connection = new SqlConnection("Server=localhost\\SQLEXPRESS;Database=Schedule;Trusted_Connection=True;TrustServerCertificate=True;");
            _connection.Open();
        }

        public static ScheduleDBConnection GetInstance()
        {
            if (_instance == null)
                _instance = new ScheduleDBConnection();
            return _instance;
        }

        public Group GetGroup(string title)
        {
            string query = $"SELECT * FROM ColledgeGroup WHERE Title = \'{title}\'";
            using (SqlCommand command = new SqlCommand(query, _connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (!reader.HasRows)
                        return null;
                    reader.Read();
                    return new Group(reader.GetInt32(0), reader.GetString(1), reader.GetBoolean(2));
                }
            }
        }
        
        public Classroom GetClassroom(string title)
        {
            string query = $"SELECT * FROM Room WHERE Title = \'{title}\'";
            using (SqlCommand command = new SqlCommand(query, _connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (!reader.HasRows)
                        return null;
                    reader.Read();
                    return new Classroom(reader.GetInt32(0), title, reader.GetBoolean(2), reader.GetBoolean(3));
                }
            }
        }
        
        public Lecturer GetLecturer(string firstName, string middleName, string lastName)
        {
            string query = $"SELECT * FROM Lecturer WHERE FirstName = \'{firstName}\' AND LastName = \'{lastName}\' AND MiddleName = \'{middleName}\'";
            int lecturerId = 0;
            using (SqlCommand command = new SqlCommand(query, _connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (!reader.HasRows)
                        return null;
                    reader.Read();
                    lecturerId = reader.GetInt32(0);
                }
            }
            query = $"SELECT * FROM LecturerAvailability WHERE LecturerId = {lecturerId}";
            Period[] availability = null;
            using (SqlCommand command = new SqlCommand(query, _connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        availability = new Period[6];
                        while (reader.Read())
                        {
                            availability[reader.GetByte(2)] = new Period
                            {
                                start = reader.GetByte(3),
                                end = reader.GetByte(4),
                            };
                        }
                    }
                }
            }
            return new Lecturer(lecturerId, firstName, lastName, middleName, availability);
        }

        // TODO: get labwork lecturers
        public List<Lecturer> GetGroupLecturers(Group group)
        {
            string query = $"SELECT Id, FirstName, MiddleName, LastName FROM Lecturer JOIN LecturerGroupSubject ON Lecturer.Id = LecturerGroupSubject.LecturerId WHERE LecturerGroupSubject.GroupId = {group.Id}";
            List<Lecturer> lecturers = new List<Lecturer>();
            using (SqlCommand command = new SqlCommand(query, _connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while(reader.Read())
                    {
                        lecturers.Add(new Lecturer(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), null));
                    }
                }
            }
            /*for (int i = 0; i < lecturers.Count; i++)
            {
                lecturers[i] = GetLecturer(lecturers[i].firstName, lecturers[i].middleName, lecturers[i].lastName);
            }*/
            return lecturers;
        }

        public List<Subject> GetPossibleSubjects(Group group, Lecturer lecturer)
        {
            string query = $"SELECT Subject.* FROM Subject JOIN LecturerGroupSubject ON Subject.Id = LecturerGroupSubject.SubjectId WHERE LecturerGroupSubject.GroupId = {group.Id} AND LecturerGroupSubject.LecturerId = {lecturer.id}";
            using (SqlCommand command = new SqlCommand(query, _connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    List<Subject> subjects = new List<Subject>();
                    while(reader.Read())
                    {
                        subjects.Add(new Subject(
                            reader.GetInt32(0),
                            reader.GetString(1),
                            reader.GetString(2),
                            reader.GetBoolean(3),
                            reader.GetBoolean(4),
                            reader.GetByte(5),
                            reader.GetByte(6),
                            reader.GetInt32(7)
                        ));
                    }
                    return subjects;
                }
            }
        }

        public List<Group> GetAllGroups()
        {
            string query = "SELECT * FROM ColledgeGroup";
            using (SqlCommand command = new SqlCommand(query, _connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    List<Group> groups = new List<Group>();
                    while(reader.Read())
                    {
                        groups.Add(new Group(reader.GetInt32(0), reader.GetString(1), reader.GetBoolean(2)));
                    }
                    return groups;
                }
            }
        }
        
        public List<Classroom> GetAllClassrooms()
        {
            string query = "SELECT * FROM Room";
            using (SqlCommand command = new SqlCommand(query, _connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    List<Classroom> classrooms = new List<Classroom>();
                    while(reader.Read())
                    {
                        classrooms.Add(new Classroom(reader.GetInt32(0), reader.GetString(1), reader.GetBoolean(2), reader.GetBoolean(3)));
                    }
                    return classrooms;
                }
            }
        }
        
        public List<Lecturer> GetAllLecturers()
        {
            string query = "SELECT * FROM Lecturer";
            List<Lecturer> lecturers = new List<Lecturer>();
            using (SqlCommand command = new SqlCommand(query, _connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while(reader.Read())
                    {
                        lecturers.Add(new Lecturer(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), new Period[6]));
                    }
                }
            }
            for (int i = 0; i < lecturers.Count; i++)
            {
                query = $"SELECT * FROM LecturerAvailability WHERE LecturerId = {lecturers[i].id}";
                using (SqlCommand command = new SqlCommand(query, _connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (!reader.HasRows)
                            return null;
                        while(reader.Read())
                        {
                            lecturers[i].availability[reader.GetByte(2)] = new Period {
                                start = reader.GetByte(3),
                                end = reader.GetByte(4),
                            };
                        }
                    }
                }
            }
            return lecturers;
        }

        public List<Subject> GetAllSubjects()
        {
            string query = "SELECT * FROM Subject";
            using (SqlCommand command = new SqlCommand(query, _connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    List<Subject> subjects = new List<Subject>();
                    while(reader.Read())
                    {
                        subjects.Add(new Subject(
                            reader.GetInt32(0),
                            reader.GetString(1),
                            reader.GetString(2),
                            reader.GetBoolean(3),
                            reader.GetBoolean(4),
                            reader.GetByte(5),
                            reader.GetByte(6),
                            reader.GetInt32(7)
                        ));
                    }
                    return subjects;
                }
            }
        }

        public void UpdateLecturer(Lecturer lecturer)
        {
            string query = $"UPDATE Lecturer SET FirstName = \'{lecturer.firstName}\', MiddleName = \'{lecturer.middleName}\', LastName = \'{lecturer.lastName}\' WHERE Id = {lecturer.id}";
            SqlCommand command = new SqlCommand(query, _connection);
            command.ExecuteNonQuery();
            query = $"DELETE FROM LecturerAvailability WHERE LecturerId = {lecturer.id}";
            command = new SqlCommand(query, _connection);
            command.ExecuteNonQuery();
            for (int i = 0; i < lecturer.availability.Length; i++)
            {
                query = $"INSERT INTO LecturerAvailability VALUES ({lecturer.id}, {i}, {lecturer.availability[i].start}, {lecturer.availability[i].end})";
                command = new SqlCommand(query, _connection);
                command.ExecuteNonQuery();
            }
        }
    
        public void UpdateGroup(Group group)
        {
            string query = $"UPDATE Group SET Title = \'{group.Name}\', HasSubgroup = {(group.HasSubgroup ? 1 : 0)} WHERE Id = {group.Id}";
            SqlCommand command = new SqlCommand(query, _connection);
            command.ExecuteNonQuery();
        }
        
        public void UpdateClassroom(Classroom classroom)
        {
            string query = $"UPDATE Room SET Title = \'{classroom.title}\', HasProjector = {(classroom.hasProjector ? 1 : 0)}, IsComputerLab = {(classroom.isComputerLab ? 1 : 0)} WHERE Id = {classroom.id}";
            SqlCommand command = new SqlCommand(query, _connection);
            command.ExecuteNonQuery();
        }
        
        public void UpdateSubject(Subject subject)
        {
            string query = $"UPDATE Subject SET Title = \'{subject.title}\', ShortTitle = \'{subject.shortTitle}\', IsPCMandatory = {(subject.isPCMandatory ? 1 : 0)}, HasLabWork = {(subject.hasLabWork)}, LessonsPerWeek = {subject.lessonsPerWeek}, LabWorksAmount = {subject.labWorksAmount}, TotalAmount = {subject.totalAmount} WHERE Id = {subject.id}";
            SqlCommand command = new SqlCommand(query, _connection);
            command.ExecuteNonQuery();
        }

        public void UpdateScheduleCell(Table.SubCell subcell, DateTime date, int lessonNumber, Group group, int subgroupNumber)
        {
            string query = $"IF NOT EXISTS(SELECT * FROM ScheduleCell WHERE Id = {subcell.id}) INSERT INTO ScheduleCell VALUES " +
            $"('{date.Year}-{date.Month}-{date.Day}', {lessonNumber}, {(subcell.isLabWork ? 1 : 0)}, {subcell.classroom.id}, {group.Id}, " + 
            $"{subcell.subject.id}, {subgroupNumber}, {(subcell.anotherHalf == null ? "NULL" : subcell.anotherHalf.id.ToString())}) " +
            $"ELSE UPDATE ScheduleCell SET LessonDate = '{date.Year}-{date.Month}-{date.Day}', LessonNumber = {lessonNumber}, IsLabWork = {(subcell.isLabWork ? 1 : 0)}, " + 
            $"RoomId = {subcell.classroom.id}, GroupId = {group.Id}, SubjectId = {subcell.subject.id}, SubgroupNumber = {subgroupNumber}, " + 
            $"OtherId = {(subcell.anotherHalf == null ? "NULL" : subcell.anotherHalf.id.ToString())}";
            SqlCommand command = new SqlCommand(query, _connection);
            command.ExecuteNonQuery();
        }

        public void AddLecturer(Lecturer lecturer)
        {
            string query = $"INSERT INTO Lecturer VALUES (\'{lecturer.firstName}\', \'{lecturer.middleName}\', \'{lecturer.lastName}\'); SELECT IDENT_CURRENT('Lecturer')";
            int lecturerId = 0;
            using (SqlCommand command = new SqlCommand(query, _connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    reader.Read();
                    lecturerId = (int)reader.GetDecimal(0);
                }
            }
            for (int i = 0; i < lecturer.availability.Length; i++)
            {
                query = $"INSERT INTO LecturerAvailability VALUES ({lecturerId}, {i}, {lecturer.availability[i].start}, {lecturer.availability[i].end})";
                SqlCommand command = new SqlCommand(query, _connection);
                command.ExecuteNonQuery();
            }
        }

        public void AddGroup(Group group)
        {
            string query = $"INSERT INTO ColledgeGroup VALUES (\'{group.Name}\', {(group.HasSubgroup ? 1 : 0)})";
            SqlCommand command = new SqlCommand(query, _connection);
            command.ExecuteNonQuery();
        }
        
        public void AddClassroom(Classroom classroom)
        {
            string query = $"INSERT INTO Room VALUES (\'{classroom.title}\', {(classroom.hasProjector ? 1 : 0)}, {(classroom.isComputerLab ? 1 : 0)})";
            SqlCommand command = new SqlCommand(query, _connection);
            command.ExecuteNonQuery();
        }
        
        public void AddSubject(Subject subject)
        {
            string query = $"INSERT INTO Subject VALUES (\'{subject.title}\', \'{subject.shortTitle}\', {(subject.isPCMandatory ? 1 : 0)}, {(subject.hasLabWork ? 1 : 0)}, {subject.lessonsPerWeek}, {subject.labWorksAmount}, {subject.totalAmount})";
            SqlCommand command = new SqlCommand(query, _connection);
            command.ExecuteNonQuery();
        }

        public void DeleteLecturer(long id)
        {
            string query = $"DELETE FROM LecturerAvailability WHERE LecturerId = {id}";
            SqlCommand command = new SqlCommand(query, _connection);
            command.ExecuteNonQuery();
            query = $"DELETE FROM Lecturer WHERE Id = {id}";
            command = new SqlCommand(query, _connection);
            command.ExecuteNonQuery();
        }

        public void DeleteGroup(long id)
        {
            string query = $"DELETE FROM ColledgeGroup WHERE Id = {id}";
            SqlCommand command = new SqlCommand(query, _connection);
            command.ExecuteNonQuery();
        }
        
        public void DeleteClassroom(long id)
        {
            string query = $"DELETE FROM Room WHERE Id = {id}";
            SqlCommand command = new SqlCommand(query, _connection);
            command.ExecuteNonQuery();
        }

        public void DeleteSubject(long id)
        {
            string query = $"DELETE FROM Subject WHERE Id = {id}";
            SqlCommand command = new SqlCommand(query, _connection);
            command.ExecuteNonQuery();
        }

        public void DeleteScheduleCell(long id)
        {
            string query = $"DELETE FROM ScheduleCell WHERE Id = {id}";
            SqlCommand command = new SqlCommand(query, _connection);
            command.ExecuteNonQuery();
        }

        public Table GetWeek(DateTime date)
        {
            Table table = new Table(5, 5);
            string query = $"SELECT ColledgeGroup.* FROM ScheduleCell JOIN ColledgeGroup ON ColledgeGroup.Id = ScheduleCell.GroupId";
            using (SqlCommand command = new SqlCommand(query, _connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while(reader.Read())
                    {
                        Group group = new Group(reader.GetInt32(0), reader.GetString(1), reader.GetBoolean(2));
                        if (!table.Content.ContainsKey(group))
                            table.AddGroup(group);
                    }
                }
            }
            
            // TODO: ScheduleCell.SubroupNumber in DB has incorrect name
            query = "SELECT ScheduleCell.Id AS 'ScheduleCell.Id', ScheduleCell.LessonDate AS 'ScheduleCell.LessonDate', " + 
                "ScheduleCell.LessonNumber AS 'ScheduleCell.LessonNumber', ScheduleCell.IsLabWork AS 'ScheduleCell.IsLabWork', " +
                "ScheduleCell.SubroupNumber AS 'ScheduleCell.SubgroupNumber', ScheduleCell.OtherId AS 'ScheduleCell.OtherId', " + 
                "DATEPART(dw, ScheduleCell.LessonDate) AS 'ScheduleCell.DayWeek', Room.Id AS 'Room.Id', Room.Title AS 'Room.Title', " +
                "Room.HasProjector AS 'Room.HasProjector', " +
                "Room.IsComputerLab AS 'Room.IsComputerLab', ColledgeGroup.Id AS 'ColledgeGroup.Id', ColledgeGroup.Title AS 'ColledgeGroup.Title', " + 
                "ColledgeGroup.HasSubgroup AS 'ColledgeGroup.HasSubgroup', Subject.Id AS 'Subject.Id', Subject.Title AS 'Subject.Title', " +
                "Subject.ShortTitle AS 'Subject.ShortTitle', Subject.IsPCMandatory AS 'Subject.IsPCMandatory', Subject.HasLabWork AS 'Subject.HasLabWork', " +
                "Subject.LessonsPerWeek AS 'Subject.LessonsPerWeek', Subject.LabWorksAmount AS 'Subject.LabWorksAmount', Subject.TotalAmount AS " +
                "'Subject.TotalAmount', Lecturer.Id AS 'Lecturer.Id', Lecturer.FirstName AS 'Lecturer.FirstName', Lecturer.MiddleName AS 'Lecturer.MiddleName', " +
                "Lecturer.LastName AS 'Lecturer.LastName' FROM ScheduleCell JOIN Room ON Room.Id = ScheduleCell.RoomId JOIN ColledgeGroup ON ColledgeGroup.Id = ScheduleCell.GroupId " + 
                "JOIN [Subject] ON Subject.Id = ScheduleCell.SubjectId JOIN LecturerGroupSubject ON LecturerGroupSubject.GroupId = ColledgeGroup.Id AND " +
                "LecturerGroupSubject.SubjectId = Subject.Id JOIN Lecturer ON Lecturer.Id = LecturerGroupSubject.LecturerId WHERE ScheduleCell.LessonDate >= " +
                $"'{date.Year}-{date.Month}-{date.Day}' AND ScheduleCell.LessonDate <= DATEADD(d, 7, '{date.Year}-{date.Month}-{date.Day}')";
            List<(Table.Position, byte, long)> anotherSubcells = new List<(Table.Position, byte, long)>();
            using (SqlCommand command = new SqlCommand(query, _connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while(reader.Read())
                    {
                        Group group = new Group(
                            reader.GetInt32(reader.GetOrdinal("ColledgeGroup.Id")),
                            reader.GetString(reader.GetOrdinal("ColledgeGroup.Title")),
                            reader.GetBoolean(reader.GetOrdinal("ColledgeGroup.HasSubgroup"))
                        );
                        // TODO: DayWeek starts from Sunday
                        Table.Position position = new Table.Position(
                            group,
                            reader.GetInt32(reader.GetOrdinal("ScheduleCell.DayWeek")),
                            reader.GetByte(reader.GetOrdinal("ScheduleCell.LessonNumber"))
                        );
                        byte subcellNumber = (byte)reader.GetInt32(reader.GetOrdinal("ScheduleCell.SubgroupNumber"));
                        Subject subject = new Subject(
                            reader.GetInt32(reader.GetOrdinal("Subject.Id")),
                            reader.GetString(reader.GetOrdinal("Subject.Title")),
                            reader.GetString(reader.GetOrdinal("Subject.ShortTitle")),
                            reader.GetBoolean(reader.GetOrdinal("Subject.IsPCMandatory")),
                            reader.GetBoolean(reader.GetOrdinal("Subject.HasLabWork")),
                            reader.GetByte(reader.GetOrdinal("Subject.LessonsPerWeek")),
                            reader.GetByte(reader.GetOrdinal("Subject.LabWorksAmount")),
                            reader.GetInt32(reader.GetOrdinal("Subject.TotalAmount"))
                        );
                        Lecturer lecturer = new Lecturer(
                            reader.GetInt32(reader.GetOrdinal("Lecturer.Id")),
                            reader.GetString(reader.GetOrdinal("Lecturer.FirstName")),
                            reader.GetString(reader.GetOrdinal("Lecturer.MiddleName")),
                            reader.GetString(reader.GetOrdinal("Lecturer.LastName")),
                            null
                        );
                        Classroom classroom = new Classroom(
                            reader.GetInt32(reader.GetOrdinal("Room.Id")),
                            reader.GetString(reader.GetOrdinal("Room.Title")),
                            reader.GetBoolean(reader.GetOrdinal("Room.HasProjector")),
                            reader.GetBoolean(reader.GetOrdinal("Room.IsComputerLab"))
                        );
                        if (!reader.IsDBNull(reader.GetOrdinal("ScheduleCell.OtherId")))
                            anotherSubcells.Add((position, subcellNumber, reader.GetInt64(reader.GetOrdinal("ScheduleCell.OtherId"))));
                        Table.SubCell subcell = new Table.SubCell(
                            reader.GetInt64(reader.GetOrdinal("ScheduleCell.Id")),
                            subject,
                            lecturer,
                            classroom,
                            reader.GetBoolean(reader.GetOrdinal("ScheduleCell.IsLabWork")),
                            null
                        );
                        table[position, subcellNumber] = subcell;
                    }
                }
            }
            // TODO: add lecturer id to ScheduleCell in database
            foreach (var pair in table.Content)
            {
                foreach (var cell in pair.Value)
                {
                    if (cell == null || cell.first == null)
                        continue;
                    query = $"SELECT * FROM LecturerAvailability WHERE LecturerId = {cell.first.lecturer.id}";
                    using (SqlCommand command = new SqlCommand(query, _connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while(reader.Read())
                            {
                                cell.first.lecturer.availability = new Period[6];
                                cell.first.lecturer.availability[reader.GetByte(2)] = new Period {
                                    start = reader.GetByte(3),
                                    end = reader.GetByte(4),
                                };
                            }
                        }
                    }
                    if (cell.second == null)
                        continue;
                    query = $"SELECT * FROM LecturerAvailability WHERE LecturerId = {cell.second.lecturer.id}";
                    using (SqlCommand command = new SqlCommand(query, _connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while(reader.Read())
                            {
                                cell.second.lecturer.availability = new Period[6];
                                cell.second.lecturer.availability[reader.GetByte(2)] = new Period {
                                    start = reader.GetByte(3),
                                    end = reader.GetByte(4),
                                };
                            }
                        }
                    }
                }
            }
            foreach (var another in anotherSubcells)
            {
                query = "SELECT ScheduleCell.LessonNumber AS 'ScheduleCell.LessonNumber', ScheduleCell.SubroupNumber AS 'ScheduleCell.SubgroupNumber', " + 
                    "DATEPART(dw, ScheduleCell.LessonDate) AS 'ScheduleCell.DayWeek', ColledgeGroup.Id AS 'ColledgeGroup.Id', ColledgeGroup.Title AS " + 
                    $"'ColledgeGroup.Title', ColledgeGroup.HasSubgroup AS 'ColledgeGroup.HasSubgroup' FROM ScheduleCell JOIN ColledgeGroup ON ColledgeGroup.Id = ScheduleCell.GroupId WHERE ScheduleCell.Id = {another.Item3}";
                using (SqlCommand command = new SqlCommand(query, _connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        reader.Read();
                        Group group = new Group(
                            reader.GetInt32(reader.GetOrdinal("ColledgeGroup.Id")),
                            reader.GetString(reader.GetOrdinal("ColledgeGroup.Title")),
                            reader.GetBoolean(reader.GetOrdinal("ColledgeGroup.HasSubgroup"))
                        );
                        // TODO: DayWeek starts from Sunday
                        Table.Position position = new Table.Position(
                            group,
                            reader.GetInt32(reader.GetOrdinal("ScheduleCell.DayWeek")),
                            reader.GetByte(reader.GetOrdinal("ScheduleCell.LessonNumber"))
                        );
                        table[another.Item1, another.Item2].anotherHalf = table[position, reader.GetInt32(reader.GetOrdinal("ScheduleCell.SubgroupNumber"))];
                        table[position, reader.GetInt32(reader.GetOrdinal("ScheduleCell.SubgroupNumber"))].anotherHalf = table[another.Item1, another.Item2];
                    }
                }
            }
            return table;
        }
    }
}
