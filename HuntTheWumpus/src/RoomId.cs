using System.Diagnostics.CodeAnalysis;

namespace HuntTheWumpus;

public record RoomId(int RingIndex, int RoomIndex)
{
    public RoomId(int packed) : this(packed / 5, packed % 5) { }

    public static bool TryParse(string? str, [MaybeNullWhen(false)] out RoomId id)
    {
        id = null;
        if (str == null || str.Length < 3)
            return false;

        if (!int.TryParse(str[0].ToString(), out var ring))
            return false;
        if (!int.TryParse(str[2].ToString(), out var room))
            return false;

        id = new RoomId(ring, room);
        return true;
    }
    
    public override string ToString()
    {
        return $"{RingIndex}-{RoomIndex}";
    }
}