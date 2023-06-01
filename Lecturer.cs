using System;

namespace schedule
{
    struct Period
    {
        public DateTime start;
        public DateTime end;
    }
    
    class Lecturer
    {
        public string firstName;
        public string lastName;
        public string middleName;
        public Period[] availability;

        public Lecturer() : this("", "", "", new Period[6]) {}

        public Lecturer(string firstName, string lastName, string middleName, Period[] availability)
        {
            if (availability.Length != 6)
                throw new Exception("Lecturer availability must be length of 6");
            this.firstName = firstName;
            this.lastName = lastName;
            this.middleName = middleName;
            this.availability = availability;
        }
    }
}
