namespace HuntTheWumpus;

public abstract class Room
{
    public RoomId Id = new RoomId(-1, -1);
    public bool HasWumpus = false;

    protected event AddMessage GetMessages = _ => { };
    protected delegate void AddMessage(List<string> messages);

    public abstract string Type { get; }
    
    protected Room()
    {
        GetMessages += messages =>
        {
            if (HasWumpus)
                messages.Add(Lang.WumpusNearby);
        };
    }

    public RoomId[] GetRoomConnections()
    {
        int newRingInter;
        int roomOffsetInter1;
        int roomOffsetInter2;
        if (Id.RingIndex is 1 or 2)
        {
            newRingInter = Id.RingIndex == 1 ? 2 : 1;
            roomOffsetInter1 = Id.RingIndex == 1 ? -1 : 0;
            roomOffsetInter2 = Id.RingIndex == 1 ?  0 : 1;
        }
        else
        {
            newRingInter = Id.RingIndex;
            roomOffsetInter1 = -1;
            roomOffsetInter2 =  1;
        }
        
        var inter1 = new RoomId(newRingInter, (Id.RoomIndex + roomOffsetInter1 + 5) % 5);
        var inter2 = new RoomId(newRingInter, (Id.RoomIndex + roomOffsetInter2 + 5) % 5);
        

        var ringOffset = Id.RingIndex % 2 == 0 ? 1 : -1;
        var newRingIntra = Id.RingIndex + ringOffset;
        var intra = new RoomId(newRingIntra, Id.RoomIndex);
        
        return [inter1, inter2, intra];
    }

    public void AddMessages(List<string> messageList)
    {
        GetMessages(messageList);
    }
    
    public override string ToString()
    {
        return $"{Type} Room" + (HasWumpus ? " with Wumpus" : "");
    }
}