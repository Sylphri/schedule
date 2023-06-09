namespace schedule
{
    class Subject
    {
        public string title;
        public string shortTitle;
        public bool isPCMandatory;
        public bool hasLabWork;
        public int lessonsPerWeek;
        public int labWorksAmount;
        public int totalAmount;

        public Subject() : this("", "", false, false, 0, 0, 0) {}

        public Subject(string title) : this(title, "", false, false, 0, 0, 0) {}

        public Subject(string title, string shortTitle, bool isPCMandatory, bool hasLabWork, int lessonsPerWeek, int labWorksAmount, int totalAmount)
        {
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
