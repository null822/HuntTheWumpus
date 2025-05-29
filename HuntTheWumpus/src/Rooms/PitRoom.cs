namespace HuntTheWumpus.Rooms;

public class PitRoom : Room
{
    public override string Type => "Pit";
    
    public PitRoom()
    {
        GetMessages += messages => messages.Add(Lang.PitNearby);
    }
}