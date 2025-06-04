namespace HuntTheWumpus.Rooms;

public class PitRoom : Room
{
    public override string Type => "Pit";
    
    public override void AddMessages(List<string> messages)
    {
        base.AddMessages(messages);
        messages.Add(Lang.PitNearby);
    }
}