namespace SocketsGame.Data
{
    using System.Collections.Generic;

    public interface IRoom
    {
        IRoom this[Directions direction] { get; set; }
        string Name { get; set; }
        string Description { get; set; }
        string LongDescription { get; }
        Directions[] Walls { get; } 
        IRoom North { get; set; }
        IRoom South { get; set; }
        IRoom East { get; set; }
        IRoom West { get; set; }
        IList<IThing> Things { get; set; }
    }
}