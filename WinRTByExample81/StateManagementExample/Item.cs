using System;

namespace StateManagementExample
{
    public class Item : IEquatable<Item>
    {
        private static int nextId = 1;

        public Item()
        {
            Id = nextId++;
            Text = string.Empty;
        }

        public int Id { get; set; }

        public string Text { get; set; }

        public bool Equals(Item other)
        {
            return other != null && other.Id.Equals(Id);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            return obj is Item && ((Item)obj).Id.Equals(Id);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
