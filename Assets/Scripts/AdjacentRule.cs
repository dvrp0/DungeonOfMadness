using UnityEngine.Tilemaps;

public class AdjacentRule
{
    public TileBase From { get; set; }
    public TileBase To { get; set; }
    public Direction Direction { get; set; }

    public AdjacentRule(TileBase from, TileBase to, Direction direction)
    {
        this.From = from;
        this.To = to;
        this.Direction = direction;
    }

    public override bool Equals(object obj)
    {
        return obj is AdjacentRule other && (From == other.From) && (To == other.To) && Direction.Equals(other.Direction);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = From?.GetHashCode() ?? 0;
            hashCode = (hashCode * 397) ^ To?.GetHashCode() ?? 0;
            hashCode = (hashCode * 397) ^ Direction.GetHashCode();

            return hashCode;
        }
    }
}