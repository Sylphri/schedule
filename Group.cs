﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace schedule
{
    class Group : IEquatable<Group>
    {
        public Group(string name)
        {
            _name = name;
        }
        string _name;
        public string Name
        {
            get { return _name; }
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