namespace schedule
{
    class Classroom
    {
        public int number;
        public bool hasProjector;
        public bool isComputerLab;

        public Classroom() : this(0, false, false) {}

        public Classroom(int number, bool hasProjector, bool isComputerLab)
        {
            this.number = number;
            this.hasProjector = hasProjector;
            this.isComputerLab = isComputerLab;
        }
    }
}
