namespace schedule
{
    class Classroom
    {
        public long? id;
        public string title;
        public bool hasProjector;
        public bool isComputerLab;

        public Classroom(long? id, string title, bool hasProjector = false, bool isComputerLab = false)
        {
            this.id = id;
            this.title = title;
            this.hasProjector = hasProjector;
            this.isComputerLab = isComputerLab;
        }
    }
}
