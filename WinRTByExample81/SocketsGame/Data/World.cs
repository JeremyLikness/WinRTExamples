namespace SocketsGame.Data
{
    using System.Collections.Generic;

    public class World : IWorld 
    {
        public World()
        {
            Rooms = new List<IRoom>();
            Inventory = new List<IThing>();
        }

        public IList<IRoom> Rooms { get; set; }
        public IList<IThing> Inventory { get; set; }
        public IRoom CurrentRoom { get; set; }
        public int TrophyCount { get; set; }
    }
}