using System;

namespace schedule
{
    /// <summary>
    /// numbers from 1 to 5
    /// </summary>
    struct Period 
    {
        public byte start;
        public byte end;
        public Period()
        {
            start = 1;
            end = 5;
        }
    }
    
    class Lecturer
    {
        public long? id;
        public string firstName;
        public string lastName;
        public string middleName;
        /// <summary>
        /// availability values from 1 to 5
        /// </summary>
        public Period[] availability;

        public Lecturer(long? id, string firstName, string middleName, string lastName, Period[] availability = null)
        {
            this.id = id;
            this.firstName = firstName;
            this.lastName = lastName;
            this.middleName = middleName;
            if (availability == null)
            {
                this.availability = new Period[6];
                for (int i = 0; i < 6; ++i)
                {
                    this.availability[i] = new Period();
                }
            }
            else
            {
                this.availability = availability;
            }
        }
    }
}
