using System;
using System.Collections.Generic;
using System.Linq;

namespace SocketsGame.Data
{
    using System.Runtime.InteropServices.WindowsRuntime;

    public class Parser
    {
        private readonly Dictionary<string, Func<IWorld, string>> commands = new Dictionary<string, Func<IWorld, string>>();

        public Parser()
        {
            commands.Add("look", Look);
            commands.Add("n", world => Move(world, Directions.North));
            commands.Add("north", world => Move(world, Directions.North));
            commands.Add("s", world => Move(world, Directions.South));
            commands.Add("south", world => Move(world, Directions.South));
            commands.Add("e", world => Move(world, Directions.East));
            commands.Add("east", world => Move(world, Directions.East));
            commands.Add("w", world => Move(world, Directions.West));
            commands.Add("west", world => Move(world, Directions.West));
            commands.Add("i", Inventory);
            commands.Add("inventory", Inventory);
            commands.Add("get", Get);
        }

        public string Parse(string command, IWorld world)
        {
            if (world.Inventory.Count == world.TrophyCount)
            {
                return "Stop trying to play. You've already won!";
            }

            var cmd = command.ToLower().Trim();
            var result = !this.commands.ContainsKey(cmd) ? "I have no clue what you are talking about." : 
                this.commands[cmd](world);
            
            if (world.Inventory.Count == world.TrophyCount)
            {
                result = string.Format(
                    "{0}{1}{2}",
                    result,
                    Environment.NewLine,
                    "YOU WON THE GAME! Congratulations for finding all of the trophies.");
            }

            return result;
        }
                                                               
        private static string Look(IWorld world)
        {
            return world.CurrentRoom.LongDescription;
        }

        private static string Move(IWorld world, Directions direction)
        {
            if (world.CurrentRoom[direction] == null)
            {
                return "You bounce off the wall.";
            }
            world.CurrentRoom = world.CurrentRoom[direction];
            return string.Format("You move {0}.\r\n{1}", direction, Look(world));
        }

        private static string Inventory(IWorld world)
        {
            return world.Inventory.Count < 1 ? "You have nothing but the shirt on your back." : 
                string.Format("You are carrying {0}.", string.Join(", ", world.Inventory.Select(i => i.Description)));
        }

        private static string Get(IWorld world)
        {
            if (world.CurrentRoom.Things.Count < 1)
            {
                return "You get down.";
            }
            var thingToGet = world.CurrentRoom.Things[0];
            world.Inventory.Add(thingToGet);
            world.CurrentRoom.Things.RemoveAt(0);
            return string.Format("You grab the {0}.", thingToGet.Name);
        }
    }
}
