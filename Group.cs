using System;

namespace schedule
{
    class Group : IEquatable<Group>
    {
        private long? _id;
        private string _name;
        private bool _hasSubgroup;
        
        public Group(long? id, string name, bool hasSubgroup)
        {
            _id = id;
            _name = name;
            _hasSubgroup = hasSubgroup;
        }

        public Group(string name, bool hasSubgroup = false)
        {
            _id = null;
            _name = name;
            _hasSubgroup = hasSubgroup;
        }

        public long? Id => _id;
        public bool HasSubgroup => _hasSubgroup;
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        
        public bool Equals(Group? other)
        {
            if (other == null)
                return false;
            return _name == other._name;
        }
        
        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}
