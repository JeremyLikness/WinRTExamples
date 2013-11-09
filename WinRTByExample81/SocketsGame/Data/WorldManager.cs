namespace SocketsGame.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Windows.Data.Json;
    using Windows.Storage;

    public class WorldManager
    {
        private readonly IWorld world; 
        private readonly List<IThing> things; 
 
        public WorldManager()
        {
            world = new World();
            things = new List<IThing>();
        }

        public async Task<IWorld> Initialize()
        {
            await this.InitializeThings();

            var random = new Random();
            await this.InitializeRooms(random);
            this.ConnectRooms(random);
            this.PlaceThings(random);
            this.world.CurrentRoom = this.world.Rooms[random.Next(0, 100)];
            return world;
        }

        private async Task InitializeThings()
        {
            var thingSource = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/Things.json"));
            var json = await FileIO.ReadTextAsync(thingSource);
            var array = JsonObject.Parse(json)["things"].GetArray();
            foreach (var thingObject in from JsonValue thing in array select thing.GetObject())
            {
                this.things.Add(new Thing { Name = thingObject["name"].GetString(), Description = thingObject["description"].GetString() });
            }
            this.world.TrophyCount = this.things.Count;
        }

        private async Task InitializeRooms(Random random)
        {
            var generatorSource =
                await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/Generator.json"));
            var json = await FileIO.ReadTextAsync(generatorSource);
            var generatorObject = JsonObject.Parse(json);
            var roomsArray = generatorObject["rooms"].GetArray();
            var rooms =
                (from JsonValue room in roomsArray select room.GetObject()).Select(
                    roomsObject => roomsObject["description"].GetString()).ToList();
            var wallsArray = generatorObject["walls"].GetArray();
            var walls =
                (from JsonValue wall in wallsArray select wall.GetObject()).Select(
                    wallObject => wallObject["description"].GetString()).ToList();
            var featuresArray = generatorObject["features"].GetArray();
            var features =
                (from JsonValue feature in featuresArray select feature.GetObject()).Select(
                    featureObject => featureObject["description"].GetString()).ToList();

            // first pass - populate the initial template for the rooms
            for (var x = 0; x < 100; x++)
            {
                var room = new Room();
                var description = rooms[random.Next(0, rooms.Count - 1)];
                room.Name = string.Format("A {0} room", description);
                room.Description = string.Format(
                    "You are standing inside a {0} room. You are surrounded by {1}. {2}",
                    description,
                    walls[random.Next(0, walls.Count - 1)],
                    features[random.Next(0, features.Count - 1)]);
                this.world.Rooms.Add(room);
            }
        }

        private void ConnectRooms(Random random)
        {
            // first connect everything
            for (var northToSouth = 0; northToSouth < 10; northToSouth++)
            {
                for (var westToEast = 0; westToEast < 10; westToEast++)
                {
                    var cell = northToSouth * 10 + westToEast;
                    if (northToSouth < 9)
                    {
                        this.Assign(cell, Directions.South);
                    }
                    if (westToEast < 9)
                    {
                        this.Assign(cell, Directions.East);
                    }
                }
            }

            // next ensure every room has at least one wall 
            for (var northToSouth = 1; northToSouth < 9; northToSouth++)
            {
                for (var westToEast = 1; westToEast < 9; westToEast++)
                {
                    var cell = northToSouth * 10 + westToEast;
                    if (this.world.Rooms[cell].Walls.Length > 0)
                    {
                        continue;
                    }
                    var directionOfWall = (Directions)random.Next(0, 3);
                    var room = this.world.Rooms[cell];
                    var otherRoom = room[directionOfWall];
                    room[directionOfWall] = null;
                    otherRoom[inversionMap[directionOfWall.AsIndex()]] = null;
                }
            }
        }

        private void PlaceThings(Random random)
        {
            // finally add things 
            foreach (var thing in this.things)
            {
                var roomIndex = random.Next(0, 99);
                this.world.Rooms[roomIndex].Things.Add(thing);
            }
        }

        private readonly int[] directionMap = { -10, 10, 1, -1 };

        private readonly Directions[] inversionMap =
            {
                Directions.South, Directions.North, Directions.West,
                Directions.East
            };

        private void Assign(int idx, Directions direction)
        {
            var room = this.world.Rooms[idx];
            var otherRoom = this.world.Rooms[idx + directionMap[direction.AsIndex()]];
            room[direction] = otherRoom;
            otherRoom[inversionMap[direction.AsIndex()]] = room;
        }
    }
}