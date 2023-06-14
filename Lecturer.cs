using System;

namespace schedule
{
    struct Period
    {
        public byte start;
        public byte end;
        public Period()
        {
            start = 0;
            end = 4;
        }
    }
    
    class Lecturer
    {
        public long? id;
        public string firstName;
        public string lastName;
        public string middleName;
        public Period[] availability;

        public Lecturer() : this(0, "", "", "", new Period[6]) {}

        public Lecturer(long? id, string firstName, string middleName, string lastName, Period[] availability)
        {
            this.id = id;
            this.firstName = firstName;
            this.lastName = lastName;
            this.middleName = middleName;
            this.availability = availability;
        }

        public Lecturer(string firstName, string middleName, string lastName, Period[] availability)
        {
            this.id = null;
            this.firstName = firstName;
            this.lastName = lastName;
            this.middleName = middleName;
            this.availability = availability;
        }
    }
}
