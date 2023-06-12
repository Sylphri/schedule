namespace schedule
{
    class Classroom
    {
        public long? id;
        public string title;
        public bool hasProjector;
        public bool isComputerLab;

        public Classroom() : this(0, "", false, false) {}

        public Classroom(string title) : this(0, title, false, false) { }

        public Classroom(long? id, string title, bool hasProjector, bool isComputerLab)
        {
            this.id = id;
            this.title = title;
            this.hasProjector = hasProjector;
            this.isComputerLab = isComputerLab;
        }
    }
}
