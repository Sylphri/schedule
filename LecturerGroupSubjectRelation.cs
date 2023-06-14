using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace schedule
{
    internal class LecturerGroupSubjectRelation
    {
        public LecturerGroupSubjectRelation(Group group, Lecturer lecturer, Subject subject)
        {
            _group = group;
            _lecturer = lecturer;
            _subject = subject;
            _labwork1Lecturer = lecturer;
            _labwork2Lecturer = lecturer;
        }

        Group _group;
        Lecturer _lecturer;
        Subject _subject;
        Lecturer _labwork1Lecturer;
        Lecturer _labwork2Lecturer;

        public Group Group { get { return _group; } }
        public Lecturer Lecturer { get { return _lecturer; } }
        public Subject Subject { get { return _subject; } }

        public Lecturer Labwork1Lecturer
        {
            get { return _labwork1Lecturer; }
            set { _labwork1Lecturer = value; }
        }

        public Lecturer Labwork2Lecturer
        {
            get { return _labwork2Lecturer; }
            set { _labwork2Lecturer = value; }
        }
    }
}
