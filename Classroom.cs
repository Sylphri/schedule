namespace schedule
{
    class Classroom
    {
        public string title;
        public bool hasProjector;
        public bool isComputerLab;

        public Classroom() : this("", false, false) {}

        public Classroom(string title, bool hasProjector, bool isComputerLab)
        {
            this.title = title;
            this.hasProjector = hasProjector;
            this.isComputerLab = isComputerLab;
        }
    }
}
