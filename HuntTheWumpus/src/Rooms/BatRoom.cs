namespace HuntTheWumpus.Rooms;

public class BatRoom : Room
{
    public override string Type => "Bat";
    
    public override void AddMessages(List<string> messages)
    {
        base.AddMessages(messages);
        messages.Add(Lang.BatNearby);
    }
}