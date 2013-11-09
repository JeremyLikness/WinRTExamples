namespace SocketsGame.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class Room : IRoom 
    {
        private readonly IRoom[] directions = new IRoom[4];
        
        public Room()
        {
            Things = new List<IThing>();
            this.North = null;
            this.South = null;
            this.East = null;
            this.West = null; 
        }

        public string Name { get; set; }
        public string Description { get; set; }

        public string LongDescription
        {
            get
            {
                return this.GenerateDescription();
            }
        }

        public Directions[] Walls
        {
            get
            {
                return Enum.GetValues(typeof(Directions))
                    .Cast<Directions>()
                    .Where(direction => this.directions[direction.AsIndex()] == null)
                    .ToArray();
            }
        }

        public IRoom this[Directions direction]
        {
            get
            {
                return this.directions[direction.AsIndex()];
            }

            set
            {
                this.directions[direction.AsIndex()] = value;
            }
        }

        public IRoom North
        {
            get
            {
                return directions[Directions.North.AsIndex()];
            }

            set
            {
                directions[Directions.North.AsIndex()] = value;
            }
        }
        
        public IRoom South
        {
            get
            {
                return directions[Directions.South.AsIndex()];
            }

            set
            {
                directions[Directions.South.AsIndex()] = value;
            }
        }

        public IRoom East
        {
            get
            {
                return directions[Directions.East.AsIndex()];
            }

            set
            {
                directions[Directions.East.AsIndex()] = value;
            }
        }

        public IRoom West
        {
            get
            {
                return directions[Directions.West.AsIndex()];
            }

            set
            {
                directions[Directions.West.AsIndex()] = value;
            }
        }
        
        public IList<IThing> Things { get; set; }

        private string GenerateDescription()
        {
            var sb = new StringBuilder(this.Name).Append(": ").Append(this.Description).Append("\r\n");

            if (this.Things.Count > 0)
            {
                sb.Append("You see ");
                sb.Append(string.Join(", and ", this.Things.Select(t => t.Description)));
                sb.Append(" on the floor.\r\n");
            }

            var exits = (from Directions dir in Enum.GetValues(typeof(Directions)) where this.directions[dir.AsIndex()] != null select dir.ToString()).ToList();

            if (exits.Count <= 1)
            {
                sb.Append("There is an exit to the ");
                sb.Append(exits[0]);
            }
            else
            {
                sb.Append("You see exits in the directions: ");
                sb.Append(string.Join(", ", exits));
            }

            sb.Append(".\r\n");

            return sb.ToString();
        }
    }
}