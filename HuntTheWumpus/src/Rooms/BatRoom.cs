namespace HuntTheWumpus.Rooms;

public class BatRoom : Room
{
    public override string Type => "Bat";

    public BatRoom()
    {
        GetMessages += messages => messages.Add(Lang.BatNearby);
    }
}