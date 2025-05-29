using HuntTheWumpus.Rooms;

namespace HuntTheWumpus;

// TODO: arrow count
public static class Program
{
    public static readonly Random Random = new();

    public static Level CurrentLevel { get; private set; } = null!;

    private static RoomId _playerPos = new(0);
    private static RoomId[] _connectedRooms = [];
    private static int _arrowCount = 5;
    
    public static void Main()
    {
        CurrentLevel = new Level();
        _playerPos = new RoomId(Random.Next(20));

        for (var i = 0; i < 20; i++)
        {
            var id = new RoomId(i);
            Console.WriteLine($"{id} : {CurrentLevel[id]}");
        }
        
        while (true)
        {
            Console.WriteLine(Lang.BreakMinor);
            
            _connectedRooms = CurrentLevel[_playerPos].GetRoomConnections();
            
            Console.WriteLine(Lang.CurrentRoom, _playerPos);
            Console.WriteLine(Lang.TunnelConnections, _connectedRooms[0], _connectedRooms[1], _connectedRooms[2]);
            var messages = CurrentLevel.GetNearbyMessages(_playerPos);
            foreach (var message in messages)
            {
                Console.WriteLine(message);
            }
            
            var exit = false;
            switch (HandleAction())
            {
                case ActionResult.Death:
                    Console.WriteLine(Lang.Death);
                    exit = true;
                    break;
                case ActionResult.Win:
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
        
        var exit = false;
        switch (result)
        {
            case ActionResult.Death:
                return ActionResult.Death;
            case ActionResult.Win:
                return ActionResult.Win;
            default:
                return result;
        }
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
        var newRoom = CurrentLevel[_playerPos];
        if (newRoom.HasWumpus)
        {
            Console.WriteLine(Lang.WumpusEnter);
            return ActionResult.Death;
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
        var shootRoomStr = Console.ReadLine();
        if (!RoomId.TryParse(shootRoomStr, out var shootRoom))
            return ActionResult.Fail;
        
        if (!_connectedRooms.Contains(shootRoom))
        {
            Console.WriteLine(Lang.InvalidShoot);
            return ActionResult.Fail;
        }
        
        _arrowCount--;
        
        if (CurrentLevel[shootRoom].HasWumpus)
        {
            Console.WriteLine(Lang.BreakMajor);
            Console.WriteLine(Lang.ArrowHit);
            return ActionResult.Win;
        }
        
        Console.WriteLine(Lang.ArrowMiss);
        if (_arrowCount == 0)
        {
            Console.WriteLine(Lang.NoArrows);
            return ActionResult.Death;
        }
        Console.WriteLine(Lang.ArrowsLeft, _arrowCount);
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