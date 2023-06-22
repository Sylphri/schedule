using System;
using System.Collections.Generic;
using System.Windows;
using Microsoft.Data.SqlClient;

namespace schedule
{
    class ScheduleDBConnection
    {
        private static ScheduleDBConnection? _instance;
        
        private SqlConnection _connection;

        private ScheduleDBConnection() 
        {
            _connection = new SqlConnection("Server=localhost\\SQLEXPRESS;Database=master;Trusted_Connection=True;TrustServerCertificate=True;");
            _connection.Open();
            SqlCommand command = new SqlCommand("SELECT name FROM sys.databases WHERE name = 'Schedule'", _connection);
            SqlDataReader reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                _connection.Close();
                _connection = new SqlConnection("Server=localhost\\SQLEXPRESS;Database=Schedule;Trusted_Connection=True;TrustServerCertificate=True;");
                _connection.Open();
            }
            else
            {
                reader.Close();
                if (MessageBox.Show("Не вдалося знайти базу даних, бажаете створити нову?", "Помилка", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    command = new SqlCommand("CREATE DATABASE Schedule", _connection);
                    command.ExecuteNonQuery();
                    command = new SqlCommand(System.IO.File.ReadAllText(".\\CreateDB.txt"), _connection);
                    command.ExecuteNonQuery();
                    _connection.Close();
                    _connection = new SqlConnection("Server=localhost\\SQLEXPRESS;Database=Schedule;Trusted_Connection=True;TrustServerCertificate=True;");
                    _connection.Open();
                }
                else
                {
                    Environment.Exit(0);
                }
            }
        }

        public static ScheduleDBConnection GetInstance()
        {
            if (_instance == null)
                _instance = new ScheduleDBConnection();
            return _instance;
        }

        public Group GetGroup(string title)
        {
            string query = $"SELECT * FROM ColledgeGroup WHERE Title = '{title}'";
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

        private Group GetGroup(int id)
        {
            string query = $"SELECT * FROM ColledgeGroup WHERE Id = '{id}'";
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
            string query = $"SELECT * FROM Room WHERE Title = '{title}'";
            using (SqlCommand command = new SqlCommand(query, _connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (!reader.HasRows)
                        return null;
                    reader.Read();
                    return new Classroom(reader.GetInt32(0), reader.GetString(1), reader.GetBoolean(2), reader.GetBoolean(3));
                }
            }
        }

        private Classroom GetClassroom(int id)
        {
            string query = $"SELECT * FROM Room WHERE Id = '{id}'";
            using (SqlCommand command = new SqlCommand(query, _connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (!reader.HasRows)
                        return null;
                    reader.Read();
                    return new Classroom(reader.GetInt32(0), reader.GetString(1), reader.GetBoolean(2), reader.GetBoolean(3));
                }
            }
        }

        public Lecturer GetLecturer(string firstName, string middleName, string lastName)
        {
            string query = $"SELECT * FROM Lecturer WHERE FirstName = '{firstName}' AND LastName = '{lastName}' AND MiddleName = '{middleName}'";
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
            Period[] availability = new Period[6];
            using (SqlCommand command = new SqlCommand(query, _connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            availability[reader.GetByte(2)-1] = new Period
                            {
                                start = reader.GetByte(3),
                                end = reader.GetByte(4),
                            };
                        }
                    }
                    else
                    {
                        for(int i = 0; i<6; i++)
                        {
                            availability[i] = new Period();
                        }
                    }
                }
            }
            return new Lecturer(lecturerId, firstName, middleName, lastName, availability);
        }

        private Lecturer GetLecturer(int id)
        {
            string query = $"SELECT * FROM Lecturer WHERE Id = '{id}'";
            int lecturerId = id;
            string firstName, middleName, lastName;
            using (SqlCommand command = new SqlCommand(query, _connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (!reader.HasRows)
                        return null;
                    reader.Read();
                    firstName = reader.GetString(1);
                    middleName = reader.GetString(2);
                    lastName = reader.GetString(3);
                }
            }
            query = $"SELECT * FROM LecturerAvailability WHERE LecturerId = {lecturerId}";
            Period[] availability = new Period[6];
            using (SqlCommand command = new SqlCommand(query, _connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            availability[reader.GetByte(2)-1] = new Period
                            {
                                start = reader.GetByte(3),
                                end = reader.GetByte(4),
                            };
                        }
                    }
                    else
                    {
                        for (int i = 0; i < 6; i++)
                        {
                            availability[i] = new Period();
                        }
                    }
                }
            }
            return new Lecturer(lecturerId, firstName, middleName, lastName, availability);
        }

        public LecturerGroupSubjectRelation GetLecturerGroupSubjectRelation(Group group, Lecturer lecturer, Subject subject)
        {
            string query = $"SELECT * FROM LecturerGroupSubject WHERE GroupId={group.Id} AND LecturerId={lecturer.id} AND SubjectId={subject.id}";
            using (SqlCommand command = new SqlCommand(query, _connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        return new LecturerGroupSubjectRelation(group, lecturer, subject);
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }

        // TODO: get labwork lecturers
        public List<Lecturer> GetGroupLecturers(Group group)
        {
            string query = $"SELECT Id, FirstName, MiddleName, LastName FROM Lecturer JOIN LecturerGroupSubject ON Lecturer.Id = LecturerGroupSubject.LecturerId WHERE LecturerGroupSubject.GroupId = {group.Id} GROUP BY Id, FirstName, MiddleName, LastName";
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

        public Subject GetSubject(string title)
        {
            string query = $"SELECT * FROM Subject WHERE Title = \'{title}\'";
            using (SqlCommand command = new SqlCommand(query, _connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (!reader.HasRows)
                        return null;
                    reader.Read();
                    return new Subject(reader.GetInt32(0), reader.GetString(1), reader.GetString(2),
                        reader.GetBoolean(3), reader.GetBoolean(4), reader.GetByte(5),
                        reader.GetByte(6), reader.GetInt32(7));
                }
            }
        }

        private Subject GetSubject(int id)
        {
            string query = $"SELECT * FROM Subject WHERE Id = {id}";
            using (SqlCommand command = new SqlCommand(query, _connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (!reader.HasRows)
                        return null;
                    reader.Read();
                    return new Subject(reader.GetInt32(0), reader.GetString(1), reader.GetString(2),
                        reader.GetBoolean(3), reader.GetBoolean(4), reader.GetByte(5),
                        reader.GetByte(6), reader.GetInt32(7));
                }
            }
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
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                lecturers[i].availability[reader.GetByte(2)-1] = new Period
                                {
                                    start = reader.GetByte(3),
                                    end = reader.GetByte(4),
                                };
                            }
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

        public bool GroupHasRelations(Group group)
        {
            string query = $"SELECT * FROM LecturerGroupSubject WHERE GroupId = {group.Id}";
            using SqlCommand command = new SqlCommand(query, _connection);
            using SqlDataReader reader = command.ExecuteReader();
            return reader.HasRows;
        }

        public bool SubjectHasRelations(Subject subject)
        {
            string query = $"SELECT * FROM LecturerGroupSubject WHERE SubjectId = {subject.id}";
            using SqlCommand command = new SqlCommand(query, _connection);
            using SqlDataReader reader = command.ExecuteReader();
            return reader.HasRows;
        }

        public bool LecturerHasRelations(Lecturer lecturer)
        {
            string query = $"SELECT * FROM LecturerGroupSubject WHERE LecturerId = {lecturer.id}";
            using SqlCommand command = new SqlCommand(query, _connection);
            using SqlDataReader reader = command.ExecuteReader();
            return reader.HasRows;
        }

        public List<LecturerGroupSubjectRelation> GetAllLecturerGroupSubjectRelations()
        {
            string query =
                "SELECT Lecturer.FirstName, Lecturer.MiddleName, Lecturer.LastName, ColledgeGroup.Title, [Subject].Title"
                +" FROM LecturerGroupSubject"
                +" INNER JOIN Lecturer ON LecturerId=Lecturer.Id"
                +" INNER JOIN ColledgeGroup ON GroupId=ColledgeGroup.Id"
                +" INNER JOIN [Subject] ON SubjectId=[Subject].Id";


            List<string[]> resultParamsList = new List<string[]>();
            List<LecturerGroupSubjectRelation> result = new List<LecturerGroupSubjectRelation>();

            using (SqlCommand command = new SqlCommand(query, _connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    List<LecturerGroupSubjectRelation> relations = new List<LecturerGroupSubjectRelation>();
                    while (reader.Read())
                    {
                        /*Lecturer relationLecturer = GetLecturer(reader.GetString(0), reader.GetString(1), reader.GetString(2));
                        Group relationGroup = GetGroup(reader.GetString(3));
                        Subject relationSubject = GetSubject(reader.GetString(4));
                        LecturerGroupSubjectRelation relationToAdd = new LecturerGroupSubjectRelation(
                            relationGroup,
                            relationLecturer,
                            relationSubject
                        );
                        relations.Add(relationToAdd);*/
                        resultParamsList.Add(new string[5]
                        {
                            reader.GetString(0),
                            reader.GetString(1),
                            reader.GetString(2),
                            reader.GetString(3),
                            reader.GetString(4),
                        });
                    }
                }
            }

            foreach(var resultParams in resultParamsList)
            {
                string lecturerFirstName = resultParams[0];
                string lecturerMiddleName = resultParams[1];
                string lecturerLastName = resultParams[2];
                string groupTitle = resultParams[3];
                string subjectTitle = resultParams[4];

                Lecturer relationLecturer = GetLecturer(lecturerFirstName, lecturerMiddleName, lecturerLastName);
                Group relationGroup = GetGroup(groupTitle);
                Subject relationSubject = GetSubject(subjectTitle);

                LecturerGroupSubjectRelation relation = GetLecturerGroupSubjectRelation(relationGroup, relationLecturer, relationSubject);

                result.Add(relation);
            }
            return result;
        }

        public void UpdateLecturer(Lecturer lecturer)
        {
            string query = $"UPDATE Lecturer SET FirstName = '{lecturer.firstName}', MiddleName = '{lecturer.middleName}', LastName = '{lecturer.lastName}' WHERE Id = {lecturer.id}";
            SqlCommand command = new SqlCommand(query, _connection);
            command.ExecuteNonQuery();
            query = $"DELETE FROM LecturerAvailability WHERE LecturerId = {lecturer.id}";
            command = new SqlCommand(query, _connection);
            command.ExecuteNonQuery();
            for (int i = 0; i < lecturer.availability.Length; i++)
            {
                query = $"INSERT INTO LecturerAvailability VALUES ({lecturer.id}, {i+1}, {lecturer.availability[i].start}, {lecturer.availability[i].end})";
                command = new SqlCommand(query, _connection);
                command.ExecuteNonQuery();
            }
        }
    
        public void UpdateGroup(Group group)
        {
            string query = $"UPDATE ColledgeGroup SET Title = '{group.Name}', HasSubgroup = {(group.HasSubgroup ? 1 : 0)} WHERE Id = {group.Id}";
            SqlCommand command = new SqlCommand(query, _connection);
            command.ExecuteNonQuery();
        }
        
        public void UpdateClassroom(Classroom classroom)
        {
            string query = $"UPDATE Room SET Title = '{classroom.title}', HasProjector = {(classroom.hasProjector ? 1 : 0)}, IsComputerLab = {(classroom.isComputerLab ? 1 : 0)} WHERE Id = {classroom.id}";
            SqlCommand command = new SqlCommand(query, _connection);
            command.ExecuteNonQuery();
        }
        
        public void UpdateSubject(Subject subject)
        {
            string query = $"UPDATE Subject SET Title = '{subject.title}', ShortTitle = '{subject.shortTitle}', IsPCMandatory = {(subject.isPCMandatory ? 1 : 0)}, HasLabWork = {(subject.hasLabWork ? 1 : 0)}, LessonsPerWeek = {subject.lessonsPerWeek}, LabWorksAmount = {subject.labWorksAmount}, TotalAmount = {subject.totalAmount} WHERE Id = {subject.id}";
            SqlCommand command = new SqlCommand(query, _connection);
            command.ExecuteNonQuery();
        }

        public void UpdateLecturerGroupSubjectRelation(LecturerGroupSubjectRelation relation)
        {
            string query = $"UPDATE LecturerGroupSubjectRelation SET Labwork1LecturerId = {relation.Labwork1Lecturer.id}, Labwork2LecturerId = {relation.Labwork2Lecturer.id}"
                + " WHERE GroupId={relation.Group.Id} AND LecturerId={relation.Lecturer.id} AND SubjectId={relation.Subject.id}";
            SqlCommand command = new SqlCommand(query, _connection);
            command.ExecuteNonQuery();
        }

        public void UpdateScheduleCell(Table.SubCell subcell, DateTime date, int lessonNumber, Group group, int subgroupNumber, bool isSplitted)
        {
            // ��� �������� � subcell ���� id ����� ������ �� �����:
            if (subcell.id == null)
            {
                bool alreadyExists;
                using (SqlCommand command = new SqlCommand(
                        $"IF EXISTS(SELECT * FROM ScheduleCell WHERE " +
                        $"LessonDate='{date.Year}-{date.Month}-{date.Day}' AND " +
                        $"LessonNumber={lessonNumber} AND " +
                        $"GroupId={group.Id} AND " +
                        $"SubgroupNumber={subgroupNumber}" +
                        $") SELECT 1 " +
                        $"ELSE SELECT 0",
                    _connection))
                {
                    alreadyExists = ((int)command.ExecuteScalar()) == 1;
                }
                if (alreadyExists)
                {
                    using (SqlCommand command = new SqlCommand(
                            $"SELECT Id FROM ScheduleCell WHERE " +
                            $"LessonDate='{date.Year}-{date.Month}-{date.Day}' AND " +
                            $"LessonNumber={lessonNumber} AND " +
                            $"GroupId={group.Id} AND " +
                            $"SubgroupNumber={subgroupNumber}"
                        , _connection))
                    {
                        subcell.id = (long)command.ExecuteScalar();
                    }
                    using (SqlCommand command = new SqlCommand(
                            $"UPDATE ScheduleCell SET " +
                            $"IsLabWork={Convert.ToInt32(subcell.isLabWork)}, " +
                            $"RoomId={subcell.classroom?.id.ToString() ?? "NULL"}, " +
                            $"GroupId={group.Id}, " +
                            $"LecturerId={subcell.lecturer.id}, " +
                            $"SubjectId={subcell.subject.id}, " +
                            $"SubgroupNumber={subgroupNumber}, " +
                            $"OtherId={(subcell.anotherHalf == null ? "NULL" : subcell.anotherHalf.id.ToString())}, " +
                            $"IsSplitted="+(isSplitted ? 1 : 0)+" " +
                            $"WHERE Id={subcell.id}"
                        , _connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
                else
                {
                    using (SqlCommand command = new SqlCommand(
                            $"INSERT INTO ScheduleCell" +
                            $"(LessonDate, LessonNumber, IsLabWork, " +
                            $"RoomId, GroupId, SubjectId, LecturerId, " +
                            $"SubgroupNumber, OtherId) VALUES (" +
                            $"'{date.Year}-{date.Month}-{date.Day}', {lessonNumber}, {Convert.ToInt32(subcell.isLabWork)}, " +
                            $"{subcell.classroom?.id.ToString() ?? "NULL"}, {group.Id}, {subcell.subject.id}, {subcell.lecturer.id}, " +
                            $"{subgroupNumber}, {(subcell.anotherHalf == null ? "NULL" : subcell.anotherHalf.id.ToString())}" +
                            $")"
                        , _connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
            }
            else
            {
                // ���� � subcell � id ��������� �� ��������� �� �����
                using (SqlCommand command = new SqlCommand(
                            $"UPDATE ScheduleCell SET " +
                            $"IsLabWork={Convert.ToInt32(subcell.isLabWork)}, " +
                            $"RoomId={subcell.classroom?.id.ToString() ?? "NULL"}, " +
                            $"GroupId={group.Id}, " +
                            $"LecturerId={subcell.lecturer.id}, " +
                            $"SubjectId={subcell.subject.id}, " +
                            $"SubgroupNumber={subgroupNumber}, " +
                            $"OtherId={(subcell.anotherHalf == null ? "NULL" : subcell.anotherHalf.id.ToString())}, " +
                            $"IsSplitted=" + (isSplitted ? 1 : 0) + " " +
                            $"WHERE Id={subcell.id}"
                        , _connection))
                {
                    command.ExecuteNonQuery();
                }
                //if (subgroupNumber == 1) // ��������� �������� �� �������
                //{
                //    using (SqlCommand command = new SqlCommand(
                //            $"UPDATE ScheduleCell SET " +
                //            $"OtherId = {subcell.anotherHalf.id}" + // �� ����� ���� �� �� ���� �����������
                //            $"WHERE Id={subcell.id}"
                //        , _connection))
                //    {
                //        command.ExecuteNonQuery();
                //    }
                //    using (SqlCommand command = new SqlCommand(
                //            $"UPDATE ScheduleCell SET " +
                //            $"OtherId = {subcell.id}" +
                //            $"WHERE Id={subcell.anotherHalf.id}"
                //        , _connection))
                //    {
                //        command.ExecuteNonQuery();
                //    }
                //}
            }
            /*$"IF NOT EXISTS(SELECT * FROM ScheduleCell WHERE " +
            $"LessonDate='{date.Year}-{date.Month}-{date.Day}') AND " + // identify cell by date-lesson-group
            $"LessonNumber={lessonNumber} AND " +
            $"GroupId={group.Id}" +
            $"BEGIN SET IDENTITY_INSERT ScheduleCell ON INSERT INTO ScheduleCell " +
            $"(LessonDate, LessonNumber, IsLabWork, RoomId, GroupId, SubjectId, SubgroupNumber, OtherId) VALUES " +
            $"('{date.Year}-{date.Month}-{date.Day}', {lessonNumber}, {(subcell.isLabWork ? 1 : 0)}, {subcell.classroom.id}, {group.Id}, " + 
            $"{subcell.subject.id}, {subgroupNumber}, {(subcell.anotherHalf == null ? "NULL" : subcell.anotherHalf.id.ToString())}) SET IDENTITY_INSERT ScheduleCell OFF END " +
            $"ELSE UPDATE ScheduleCell SET LessonDate = '{date.Year}-{date.Month}-{date.Day}', LessonNumber = {lessonNumber}, IsLabWork = {(subcell.isLabWork ? 1 : 0)}, " + 
            $"RoomId = {subcell.classroom.id}, GroupId = {group.Id}, SubjectId = {subcell.subject.id}, SubgroupNumber = {subgroupNumber}, " + 
            $"OtherId = {(subcell.anotherHalf == null ? "NULL" : subcell.anotherHalf.id.ToString())} WHERE " +
            $"LessonDate='{date.Year}-{date.Month}-{date.Day}') AND " + // identify cell by date-lesson-group
            $"LessonNumber={lessonNumber} AND " +
            $"groupId={group.Id}";*/
            // SqlCommand command = new SqlCommand(query, _connection);
            // command.ExecuteNonQuery();
        }

        public void UpdateScheduleCell(Table.Cell cell, DateTime date, int lessonNumber, Group group)
        {
            if (cell.first != null)
                UpdateScheduleCell(cell.first, date, lessonNumber, group, 0, cell.isSplitted);
            else
                DeleteScheduleCell(cell.first, date, lessonNumber, group, 0);
            if (cell.isSplitted)
            {
                if (cell.second != null)
                {
                    UpdateScheduleCell(cell.second, date, lessonNumber, group, 1, cell.isSplitted);
                }
                else
                    DeleteScheduleCell(cell.second, date, lessonNumber, group, 1);
            }
        }

        public void AddLecturer(Lecturer lecturer)
        {
            string query = $"INSERT INTO Lecturer VALUES ('{lecturer.firstName}', '{lecturer.middleName}', '{lecturer.lastName}'); SELECT IDENT_CURRENT('Lecturer')";
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
                query = $"INSERT INTO LecturerAvailability VALUES ({lecturerId}, {i+1}, {lecturer.availability[i].start}, {lecturer.availability[i].end})";
                SqlCommand command = new SqlCommand(query, _connection);
                command.ExecuteNonQuery();
            }
        }

        public void AddGroup(Group group)
        {
            string query = $"INSERT INTO ColledgeGroup VALUES ('{group.Name}', {(group.HasSubgroup ? 1 : 0)})";
            SqlCommand command = new SqlCommand(query, _connection);
            command.ExecuteNonQuery();
        }
        
        public void AddClassroom(Classroom classroom)
        {
            string query = $"INSERT INTO Room VALUES ('{classroom.title}', {(classroom.hasProjector ? 1 : 0)}, {(classroom.isComputerLab ? 1 : 0)})";
            SqlCommand command = new SqlCommand(query, _connection);
            command.ExecuteNonQuery();
        }
        
        public void AddSubject(Subject subject)
        {
            string query = $"INSERT INTO Subject VALUES ('{subject.title}', '{subject.shortTitle}', {(subject.isPCMandatory ? 1 : 0)}, {(subject.hasLabWork ? 1 : 0)}, {subject.lessonsPerWeek}, {subject.labWorksAmount}, {subject.totalAmount})";
            SqlCommand command = new SqlCommand(query, _connection);
            command.ExecuteNonQuery();
        }

        public void AddLecturerGroupSubjectRelation(LecturerGroupSubjectRelation relation)
        {
            string query = $"INSERT INTO LecturerGroupSubject VALUES " +
                $"({relation.Lecturer.id}, {relation.Group.Id}, {relation.Subject.id}, {relation.Labwork1Lecturer.id}, {relation.Labwork2Lecturer.id})";
            SqlCommand command = new SqlCommand(query, _connection);
            command.ExecuteNonQuery();
        }

        public void DeleteLecturer(Lecturer lecturer)
        {
            string query = $"DELETE FROM ScheduleCell WHERE LecturerId = {lecturer.id}";
            SqlCommand command = new SqlCommand(query, _connection);
            command.ExecuteNonQuery();
            query = $"DELETE FROM LecturerAvailability WHERE LecturerId = {lecturer.id}";
            command = new SqlCommand(query, _connection);
            command.ExecuteNonQuery();
            query = $"DELETE FROM Lecturer WHERE Id = {lecturer.id}";
            command = new SqlCommand(query, _connection);
            command.ExecuteNonQuery();
        }

        public void DeleteGroup(Group group)
        {
            string query = $"DELETE FROM ScheduleCell WHERE GroupId = {group.Id}";
            SqlCommand command = new SqlCommand(query, _connection);
            command.ExecuteNonQuery();
            query = $"DELETE FROM ColledgeGroup WHERE Id = {group.Id}";
            command = new SqlCommand(query, _connection);
            command.ExecuteNonQuery();
        }
        
        public void DeleteClassroom(Classroom classroom)
        {
            string query = $"DELETE FROM ScheduleCell WHERE RoomId = {classroom.id}";
            SqlCommand command = new SqlCommand(query, _connection);
            command.ExecuteNonQuery();
            query = $"DELETE FROM Room WHERE Id = {classroom.id}";
            command = new SqlCommand(query, _connection);
            command.ExecuteNonQuery();
        }

        public void DeleteSubject(Subject subject)
        {
            string query = $"DELETE FROM ScheduleCell WHERE SubjectId = {subject.id}";
            SqlCommand command = new SqlCommand(query, _connection);
            command.ExecuteNonQuery();
            query = $"DELETE FROM Subject WHERE Id = {subject.id}";
            command = new SqlCommand(query, _connection);
            command.ExecuteNonQuery();
        }

        public void DeleteLecturerGroupSubjectRelation(LecturerGroupSubjectRelation relation)
        {
            string query = $"DELETE FROM LecturerGroupSubject WHERE GroupId={relation.Group.Id} AND LecturerId={relation.Lecturer.id} AND SubjectId={relation.Subject.id}";
            SqlCommand command = new SqlCommand(query, _connection);
            command.ExecuteNonQuery();
        }

        public void DeleteScheduleCell(long id)
        {
            string query = $"DELETE FROM ScheduleCell WHERE Id = {id}";
            SqlCommand command = new SqlCommand(query, _connection);
            command.ExecuteNonQuery();
        }

        public void DeleteScheduleCell(Table.SubCell subcell, DateTime date, int lessonNumber, Group group, int subgroupNumber)
        {
            string query = $"DELETE FROM ScheduleCell WHERE " +
                $"LessonDate='{date.Year}-{date.Month}-{date.Day}' AND " +
                $"LessonNumber={lessonNumber} AND " +
                $"GroupId={group.Id} AND " +
                $"SubgroupNumber={subgroupNumber}";
            SqlCommand command = new SqlCommand(query, _connection);
            command.ExecuteNonQuery();
        }

        public Table GetWeek(DateTime date)
        {
            List<Group> allGroups = GetAllGroups();
            Table table = new Table(allGroups, 5, 5);
            /*string query = $"SELECT ColledgeGroup.* FROM ScheduleCell JOIN ColledgeGroup ON ColledgeGroup.Id = ScheduleCell.GroupId";
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
            }*/

            /*// TODO: ScheduleCell.SubgroupNumber in DB has incorrect name
            string query = "SELECT ScheduleCell.Id AS 'ScheduleCell.Id', ScheduleCell.LessonDate AS 'ScheduleCell.LessonDate', " + 
                "ScheduleCell.LessonNumber AS 'ScheduleCell.LessonNumber', ScheduleCell.IsLabWork AS 'ScheduleCell.IsLabWork', " +
                "ScheduleCell.SubgroupNumber AS 'ScheduleCell.SubgroupNumber', ScheduleCell.OtherId AS 'ScheduleCell.OtherId', " + 
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
                $"'{date.Year}-{date.Month}-{date.Day}' AND ScheduleCell.LessonDate < DATEADD(day, 7, '{date.Year}-{date.Day}-{date.Month}')"; // ������ ���� � DATEADD �������� �� ����������� �����������
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
                        int weekDay = reader.GetInt32(reader.GetOrdinal("ScheduleCell.DayWeek"));
                        Table.Position position = new Table.Position(
                            group,
                            weekDay == 1 ? 7 : weekDay - 1,
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
                            reader.GetString(reader.GetOrdinal("Lecturer.LastName"))
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
                    if (cell.first != null)
                    {
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
                    }
                    if (cell.second != null)
                    {
                        query = $"SELECT * FROM LecturerAvailability WHERE LecturerId = {cell.second.lecturer.id}";
                        using (SqlCommand command = new SqlCommand(query, _connection))
                        {
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                while(reader.Read())
                                {
                                    cell.second.lecturer.availability = new Period[6];
                                    cell.second.lecturer.availability[reader.GetByte(2)-1] = new Period {
                                        start = reader.GetByte(3),
                                        end = reader.GetByte(4),
                                    };
                                }
                            }
                        }
                    }
                }
            }
            foreach (var another in anotherSubcells)
            {
                query = "SELECT ScheduleCell.LessonNumber AS 'ScheduleCell.LessonNumber', ScheduleCell.SubgroupNumber AS 'ScheduleCell.SubgroupNumber', " + 
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
                        int weekDay = reader.GetInt32(reader.GetOrdinal("ScheduleCell.DayWeek"));
                        Table.Position position = new Table.Position(
                            group,
                            weekDay == 1 ? 7 : weekDay - 1,
                            reader.GetByte(reader.GetOrdinal("ScheduleCell.LessonNumber"))
                        );
                        table[another.Item1, another.Item2].anotherHalf = table[position, reader.GetInt32(reader.GetOrdinal("ScheduleCell.SubgroupNumber"))];
                        table[position, reader.GetInt32(reader.GetOrdinal("ScheduleCell.SubgroupNumber"))].anotherHalf = table[another.Item1, another.Item2];
                    }
                }
            }
            return table;*/

            DateTime endDate = date.AddDays(7);
            List<Dictionary<string, object>> dataList = new List<Dictionary<string, object>>();
            using (
                SqlCommand command = new SqlCommand($"SELECT " +
                    $"Id, LessonDate, LessonNumber-1, IsLabWork, RoomId, GroupId, SubjectId, LecturerId, " +
                    $"SubgroupNumber, OtherId, IsSplitted " +
                    $"FROM ScheduleCell WHERE " +
                    $"LessonDate>='{date.Year}-{date.Month}-{date.Day}' AND " +
                    $"LessonDate<'{endDate.Year}-{endDate.Month}-{endDate.Day}'",
                _connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            var id = reader.GetInt64(0);
                            var lessonDate = reader.GetDateTime(1);
                            var lessonNumber = reader.GetInt32(2);
                            var isLabWork = reader.GetBoolean(3);
                            int? roomId = reader.IsDBNull(4) ? null : reader.GetInt32(4);
                            var groupId = reader.GetInt32(5);
                            var subjectId = reader.GetInt32(6);
                            var lecturerId = reader.GetInt64(7); // Change in db to 32
                            var subgroupNumber = reader.GetInt32(8);
                            long? otherId = reader.IsDBNull(9) ? null : reader.GetInt64(9);
                            var isSplitted = reader.GetBoolean(10);

                            Dictionary<string, object> newData = new Dictionary<string, object>()
                            {
                                { "Id", id },
                                { "LessonDate", lessonDate },
                                { "LessonNumber", lessonNumber },
                                { "IsLabWork", isLabWork },
                                { "RoomId", roomId },
                                { "GroupId", groupId },
                                { "SubjectId", subjectId },
                                { "LecturerId", lecturerId },
                                { "SubgroupNumber", subgroupNumber },
                                { "OtherId",  otherId},
                                { "IsSplitted",  isSplitted}
                            };
                            dataList.Add(newData);
                        }
                    }
                }
            }

            foreach(Dictionary<string, object> data in dataList)
            {
                long id = (long)data["Id"];
                DateTime lessonDate = (DateTime)data["LessonDate"];
                int lessonNumber = (int)data["LessonNumber"];
                bool isLabWork = (bool)data["IsLabWork"];
                int? roomId = (int?)data["RoomId"];
                int groupId = (int)data["GroupId"];
                int subjectId = (int)data["SubjectId"];
                long lecturerId = (long)data["LecturerId"];
                int SubgroupNumber = (int)data["SubgroupNumber"];
                long? otherId = (long?)data["OtherId"];
                bool isSplitted = (bool)data["IsSplitted"];

                Classroom relationClassroom;

                if (roomId != null)
                {
                    relationClassroom = GetClassroom(roomId.Value);
                }
                else
                {
                    relationClassroom = null;
                }
                Group relationGroup = GetGroup(groupId);
                Subject relationSubject = GetSubject(subjectId);
                Lecturer relationLecturer = GetLecturer((int)lecturerId);

                Table.Position cellPosition = new Table.Position(relationGroup, (lessonDate - date).Days, lessonNumber);

                table[cellPosition, SubgroupNumber + 1] = new Table.SubCell(
                    id, relationSubject, relationLecturer,
                    relationClassroom, isLabWork, null);

                table[cellPosition].isSplitted = isSplitted;

                //if (otherId != null)
                //{
                //    DateTime otherLessonDate = DateTime.Today;
                //    int otherLessonNumber = 0;
                //    bool otherIsLabWork = false;
                //    int? otherRoomId = null;
                //    int otherGroupId = 0;
                //    int otherSubjectId = 0;
                //    long otherLecturerId = 0L;
                //    int otherSubgroupNumber = 0;
                //    using (
                //        SqlCommand command = new SqlCommand(
                //            $"SELECT " +
                //            $"LessonDate, LessonNumber-1, IsLabWork, RoomId, GroupId, " +
                //            $"SubjectId, LecturerId, SubgroupNumber FROM ScheduleCell " +
                //            $"WHERE Id={otherId}",
                //        _connection))
                //    {
                //        using (SqlDataReader reader = command.ExecuteReader())
                //        {
                //            if (reader.HasRows)
                //            {
                //                while (reader.Read())
                //                {
                //                    otherLessonDate = reader.GetDateTime(0);
                //                    otherLessonNumber = reader.GetInt32(1);
                //                    otherIsLabWork = reader.GetBoolean(2);
                //                    otherRoomId = reader.IsDBNull(3) ? null : reader.GetInt32(3);
                //                    otherGroupId = reader.GetInt32(4);
                //                    otherSubjectId = reader.GetInt32(5);
                //                    otherLecturerId = reader.GetInt64(6);
                //                    otherSubgroupNumber = reader.GetInt32(7);
                //                }
                //            }
                //        }
                //    }
                //    Classroom otherRelationClassroom;

                //    if (otherRoomId != null)
                //    {
                //        otherRelationClassroom = GetClassroom(otherRoomId.Value);
                //    }
                //    else
                //    {
                //        otherRelationClassroom = null;
                //    }
                //    Subject otherRelationSubject = GetSubject(otherSubjectId);
                //    Lecturer otherRelationLecturer = GetLecturer((int)otherLecturerId);

                //    table[cellPosition, 2] = new Table.SubCell(
                //        otherId, otherRelationSubject, otherRelationLecturer,
                //        otherRelationClassroom, otherIsLabWork, table[cellPosition, 0]);
                //}
            }

            return table;
        }
    }
}
