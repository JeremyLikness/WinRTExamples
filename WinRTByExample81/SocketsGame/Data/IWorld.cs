namespace SocketsGame.Data
{
    using System.Collections.Generic;

    public interface IWorld
    {
        int TrophyCount { get; set; }
        IList<IRoom> Rooms { get; set; }
        IList<IThing> Inventory { get; set; }
        IRoom CurrentRoom { get; set; }        
    }
}