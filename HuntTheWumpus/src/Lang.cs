namespace HuntTheWumpus;

public static class Lang
{
    public const string BreakMinor = "--------------------------------";
    public const string BreakMajor = "================================";
    
    public const string CurrentRoom = "You are in Room {0}";
    public const string TunnelConnections = "Tunnels lead to Rooms {0}, {1}, {2}";
    
    public const string Action = "Shoot or Move (S/M) ? ";
    public const string ActionMove = "Where To? ";
    public const string ActionShoot = "Which Room? ";

    public const string InvalidMove = "Can't get there!";
    public const string InvalidShoot = "ARROWS AREN'T THAT CROOKED - TRY ANOTHER ROOM";

    public const string ArrowsLeft = "{0} Arrows Left";
    public const string ArrowMiss = "Missed!";
    public const string ArrowHit = "AHA! YOU GOT THE WUMPUS!";

    public const string WumpusNearby = "You smell a wumpus nearby";
    public const string BatNearby = "You hear flapping nearby";
    public const string PitNearby = "You feel a breeze nearby";

    public const string Death = "HA HA HA - YOU LOSE!";
    public const string Win = "HEE HEE HEE - THE WUMPUS'LL GETCHA NEXT TIME!!";
    
    public const string WumpusEnter = "...OOPS! BUMPED A WUMPUS!\nTSK TSK TSK - WUMPUS GOT YOU!";
    public const string PitEnter = "YYYIIIIEEEE . . . FELL IN PIT";
    public const string BatEnter = "ZAP--SUPER BAT SNATCH! ELSEWHEREVILLE FOR YOU!";
    public const string NoArrows = "YOU'VE RUN OUT OF ARROWS!";
    
}