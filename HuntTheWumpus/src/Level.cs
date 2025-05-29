using HuntTheWumpus.Rooms;

namespace HuntTheWumpus;

public class Level
{
    private readonly Room[,] _rooms = new Room[4, 5]; // 4 rings per level, 5 rooms per ring
    
    public Level()
    {
        var wumpusRoom = new RoomId(Program.Random.Next(20));
        var roomSelector = new ExclusiveRngSelector(Program.Random);
        RoomId[] batRooms =
        [
            new(roomSelector.Next(20)),
            new(roomSelector.Next(20))
        ];
        RoomId[] pitRooms =
        [
            new(roomSelector.Next(20)),
            new(roomSelector.Next(20))
        ];
        
        for (var ringIndex = 0; ringIndex < 4; ringIndex++)
        {
            for (var roomIndex = 0; roomIndex < 5; roomIndex++)
            {
                var id = new RoomId(ringIndex, roomIndex);
                Room room;
                
                if (batRooms.Any(r => r == id))
                    room = new BatRoom();
                else if (pitRooms.Any(r => r == id))
                    room = new PitRoom();
                else
                    room = new EmptyRoom();
                
                room.HasWumpus = id == wumpusRoom;
                
                room.Id = id;
                _rooms[ringIndex, roomIndex] = room;
            }
        }
    }
    
    public Room this[RoomId id]
    {
        get => _rooms[id.RingIndex, id.RoomIndex];
        set => SetRoom(value, id);
    }
    
    public Room this[int ringIndex, int roomIndex]
    {
        get => _rooms[ringIndex, roomIndex];
        set => SetRoom(value, new RoomId(ringIndex, roomIndex));
    }
    
    private void SetRoom(Room room, RoomId id)
    {
        room.Id = id;
        _rooms[id.RingIndex, id.RoomIndex] = room;
    }
    
    public List<string> GetNearbyMessages(RoomId room)
    {
        var messages = new List<string>();
        
        foreach (var roomConnection in this[room].GetRoomConnections())
        {
            var connectedRoom = this[roomConnection];
            connectedRoom.AddMessages(messages);
        }
        
        return messages;
    }

    private class ExclusiveRngSelector(Random random)
    {
        private readonly HashSet<int> _previousValues = [];
        
        public int Next(int max = int.MaxValue, int min = 0)
        {
            int value;
            while (true)
            {
                value = random.Next(min, max);

                var exit = _previousValues.Contains(value);
                _previousValues.Add(value);
                
                if (exit) break;
            }
            
            return value;
        }
    }
}