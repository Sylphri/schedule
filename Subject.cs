using System;

namespace schedule
{
    class Subject : IEquatable<Subject?>
    {
        public long? id;
        public string title;
        public string shortTitle;
        public bool isPCMandatory;
        public bool hasLabWork;
        public int lessonsPerWeek;
        public int labWorksAmount;
        public int totalAmount;

        public Subject(string title) : this(null, title, "", false, false, 20, 0, 0) {}

        public Subject(long? id, string title, string shortTitle, bool isPCMandatory, bool hasLabWork, int lessonsPerWeek, int labWorksAmount, int totalAmount)
        {
            this.id = id;
            this.title = title;
            this.shortTitle = shortTitle;
            this.isPCMandatory = isPCMandatory;
            this.hasLabWork = hasLabWork;
            this.lessonsPerWeek = lessonsPerWeek;
            this.labWorksAmount = labWorksAmount;
            this.totalAmount = totalAmount;
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as Subject);
        }

        public bool Equals(Subject? other)
        {
            return other is not null &&
                   title == other.title;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(title);
        }
    }
}
