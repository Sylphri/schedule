namespace schedule
{
    class Subject
    {
        public long? id;
        public string title;
        public string shortTitle;
        public bool isPCMandatory;
        public bool hasLabWork;
        public int lessonsPerWeek;
        public int labWorksAmount;
        public int totalAmount;

        public Subject() : this(0, "", "", false, false, 0, 0, 0) {}

        public Subject(string title) : this(0, title, "", false, false, 0, 0, 0) {}

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
    }
}
