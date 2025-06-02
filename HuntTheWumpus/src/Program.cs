using HuntTheWumpus.Rooms;

namespace HuntTheWumpus;

public static class Program
{
    public static readonly Random Random = new();
    
    public static Level Level { get; private set; } = null!;
    
    private static RoomId _playerPos = new(0);
    private static RoomId[] _connectedRooms = [];
    private static int _arrowCount = 5;
    
    public static void Main()
    {
        Level = new Level();
        _playerPos = new RoomId(Random.Next(20));

        for (var i = 0; i < 20; i++)
        {
            var id = new RoomId(i);
            Console.WriteLine($"{id} : {Level[id]}");
        }
        
        while (true)
        {
            Console.WriteLine(Lang.BreakMinor);
            
            _connectedRooms = Level[_playerPos].GetRoomConnections();
            
            Console.WriteLine(Lang.CurrentRoom, _playerPos);
            Console.WriteLine(Lang.TunnelConnections, _connectedRooms[0], _connectedRooms[1], _connectedRooms[2]);
            var messages = Level.GetNearbyMessages(_playerPos);
            foreach (var message in messages)
            {
                Console.WriteLine(message);
            }
            
            var exit = false;
            switch (HandleAction())
            {
                case ActionResult.Death:
                    Console.WriteLine(Lang.BreakMajor);
                    Console.WriteLine(Lang.Death);
                    exit = true;
                    break;
                case ActionResult.Win:
                    Console.WriteLine(Lang.BreakMajor);
                    Console.WriteLine(Lang.Win);
                    exit = true;
                    break;
            }
            if (exit) break;
        }
    }

    private static ActionResult HandleAction()
    {
        Console.Write(Lang.Action);
        var action = Console.ReadLine()?.ToUpper();
        if (action == null) return ActionResult.Fail;
        
        var result = ActionResult.Fail;
        if (RoomId.TryParse(action, out var moveRoomShortcut))
        {
            result = HandleMove(moveRoomShortcut);
        }
        else
            result = action switch
            {
                "S" => HandleShoot(),
                "M" => HandleMove(),
                _ => result
            };
        
        return result switch
        {
            ActionResult.Death => ActionResult.Death,
            ActionResult.Win => ActionResult.Win,
            _ => result
        };
    }

    private static ActionResult HandleMove(RoomId? moveRoom = null)
    {
        if (moveRoom == null)
        {
            Console.Write(Lang.ActionMove);
            var moveRoomStr = Console.ReadLine();

            if (!RoomId.TryParse(moveRoomStr, out moveRoom))
                return ActionResult.Fail;
        }
        
        if (!_connectedRooms.Contains(moveRoom))
        {
            Console.WriteLine(Lang.InvalidMove);
            return ActionResult.Fail;
        }
        
        _playerPos = moveRoom;

        return HandleRoomEnter();
    }

    private static ActionResult HandleRoomEnter()
    {
        var newRoom = Level[_playerPos];
        if (newRoom.HasWumpus)
        {
            return WakeWumpus(_playerPos);
        }
        switch (newRoom)
        {
            case BatRoom:
                Console.WriteLine(Lang.BatEnter);
                _playerPos = new RoomId(Random.Next(20));
                return ActionResult.Success;
            case PitRoom:
                Console.WriteLine(Lang.PitEnter);
                return ActionResult.Death;
            default:
                return ActionResult.Success;
        }
    }
    
    private static ActionResult HandleShoot()
    {
        Console.Write(Lang.ActionShoot);
        var roomsStr = Console.ReadLine();
        if (roomsStr == null)
            return ActionResult.Fail;
        
        var connectedRooms = _connectedRooms;
        foreach (var shootRoomStr in roomsStr.Split(' '))
        {
            if (!RoomId.TryParse(shootRoomStr, out var shootRoom))
                continue;
            
            if (!connectedRooms.Contains(shootRoom))
            {
                Console.WriteLine(Lang.InvalidShoot);
                return ActionResult.Fail;
            }
            
            if (Level[shootRoom].HasWumpus)
            {
                Console.WriteLine(Lang.ArrowHit);
                var wakeResult = WakeWumpus(shootRoom);
                switch (wakeResult)
                {
                    case ActionResult.Fail:
                        return ActionResult.Win;
                    default:
                        return ActionResult.Success;
                }
            }
            
            Console.WriteLine(Lang.ArrowMiss);

            connectedRooms = Level[shootRoom].GetRoomConnections();
        }
        
        _arrowCount--;
        if (_arrowCount == 0)
        {
            Console.WriteLine(Lang.NoArrows);
            return ActionResult.Death;
        }
        Console.WriteLine(Lang.ArrowsLeft, _arrowCount);
        
        return ActionResult.Success;
    }

    private static ActionResult WakeWumpus(RoomId wumpusRoomId)
    {
        var choice = Random.Next(4);
        if (choice != 3)
        {
            var wumpusRoom = Level[wumpusRoomId];
            wumpusRoom.HasWumpus = false;
            wumpusRoomId = wumpusRoom.GetRoomConnections()[choice];
            wumpusRoom = Level[wumpusRoomId];
            wumpusRoom.HasWumpus = true;
        }
        
        Console.WriteLine(Lang.WumpusWake);
        
        if (_playerPos == wumpusRoomId)
        {
            Console.WriteLine(Lang.WumpusDeath);
            return ActionResult.Death;
        }
        
        return choice == 3 ? ActionResult.Fail : ActionResult.Success;
    }

    private enum ActionResult
    {
        Success,
        Fail,
        Win,
        Death
    }
}