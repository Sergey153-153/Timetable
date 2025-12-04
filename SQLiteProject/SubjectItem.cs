using System;

namespace SQLiteProject
{
    public class SubjectItem
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}