using Microsoft.Data.SqlClient;
using System.IO;
using System.Collections.Generic;

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
            using (SqlCommand command = new SqlCommand(query, _connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (!reader.HasRows)
                        return null;
                    Period[] availability = new Period[6];
                    while(reader.Read())
                    {
                        availability[reader.GetByte(2)] = new Period {
                            start = reader.GetByte(3),
                            end = reader.GetByte(4),
                        };
                    }
                    return new Lecturer(lecturerId, firstName, lastName, middleName, availability);
                }
            }
        }

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
            for (int i = 0; i < lecturers.Count; i++)
            {
                lecturers[i] = GetLecturer(lecturers[i].firstName, lecturers[i].middleName, lecturers[i].lastName);
            }
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
    }
}
