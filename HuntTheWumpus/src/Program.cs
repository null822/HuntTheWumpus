using HuntTheWumpus.Rooms;

namespace HuntTheWumpus;

public static class Program
{
    public static readonly Random Random = new();
    
    public static Level Level { get; private set; } = null!;
    
    private static RoomId _playerPos = new(0);
    private static RoomId[] _connectedRooms = [];
    private static int _arrowCount = 5;
    private static bool _debugMode;
    
    public static void Main()
    {
        Level = new Level();
        _playerPos = new RoomId(Random.Next(20));
        
        while (true)
        {
            ActionResult action;
            
            if (_debugMode)
            {
                action = HandleDebug();
            }
            else
            {
                Console.WriteLine(Lang.BreakMinor);
                
                action = HandleRoomEnter();
                
                if (action is not (ActionResult.Win or ActionResult.Death))
                {
                    _connectedRooms = Level[_playerPos].GetRoomConnections();

                    Console.WriteLine(Lang.CurrentRoom, _playerPos);
                    Console.WriteLine(Lang.TunnelConnections, 
                        _connectedRooms[0],
                        _connectedRooms[1],
                        _connectedRooms[2]);
                    var messages = Level.GetNearbyMessages(_playerPos);
                    foreach (var message in messages)
                    {
                        Console.WriteLine(message);
                    }

                    action = HandleAction();
                }
            }
            
            if (action == ActionResult.Death)
            {
                Console.WriteLine(Lang.Death);
                break;
            }
            if (action == ActionResult.Win)
            {
                Console.WriteLine(Lang.Win);
                break;
            }
        }
    }

    private static ActionResult HandleAction()
    {
        Console.Write(Lang.Action);
        var action = Console.ReadLine()?.ToLower();
        if (action == null) return ActionResult.Fail;

        if (action == "dbg")
        {
            _debugMode = true;
            Console.WriteLine("Debug Mode Enabled");
            return ActionResult.Success;
        }
        
        var result = ActionResult.Fail;
        if (RoomId.TryParse(action, out var moveRoomShortcut))
        {
            result = HandleMove(moveRoomShortcut);
        }
        else
            result = action switch
            {
                "s" => HandleShoot(),
                "m" => HandleMove(),
                _ => result
            };

        return result;
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

        return ActionResult.Success;
    }

    private static ActionResult HandleRoomEnter()
    {
        var newRoom = Level[_playerPos];
        if (newRoom.HasWumpus)
        {
            var action = WakeWumpus(_playerPos);
            if (action is ActionResult.Death or ActionResult.Win)
                return action;
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

    private static ActionResult HandleDebug()
    {
        Console.Write("dbg > ");
        var cmd = Console.ReadLine()?.ToLower().Split(' ');
        if (cmd is null) return ActionResult.Fail;
        var args = cmd[1..];
        
        switch (cmd[0])
        {
            case "ls":
                for (var i = 0; i < 20; i++)
                {
                    var id = new RoomId(i);
                    Console.WriteLine($"{id} : {Level[id]}");
                }
                break;
            case "tp":
                if (RoomId.TryParse(args[0], out var tpPos))
                    _playerPos = tpPos;
                break;
            case "set":
                if (!RoomId.TryParse(args[0], out var setPos))
                    break;
                var withWumpus = args is [_, _, "wumpus", ..];
                Level[setPos] = args[1] switch
                {
                    "bat" => new BatRoom { HasWumpus = withWumpus },
                    "pit" => new PitRoom { HasWumpus = withWumpus },
                    "empty" => new EmptyRoom { HasWumpus = withWumpus },
                };
                break;
            case "exit":
                _debugMode = false;
                break;
            case "death": return ActionResult.Death;
            case "win": return ActionResult.Win;
            
            default:
                Console.WriteLine($"Command {cmd[0]} not found!");
                return ActionResult.Fail;
        }

        return ActionResult.Success;
    }
    
    private enum ActionResult
    {
        Success,
        Fail,
        Win,
        Death
    }
}