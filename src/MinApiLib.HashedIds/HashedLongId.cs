namespace MinApiLib.HashedIds;

[JsonConverter(typeof(HashedLongIdJsonConverter))]
public record struct HashedLongId(long Value)
{
    public override string ToString()
        => Hasher.Instance.EncodeLong(Value);

    public static bool TryParse(string? value, out HashedLongId result)
    {
        if (!string.IsNullOrWhiteSpace(value))
        {
            var decoded = Hasher.Instance.DecodeLong(value);
            if (decoded.Length == 1)
            {
                result = new HashedLongId(decoded[0]);
                return true;
            }
        }

        result = new HashedLongId(-1);
        return false;
    }

    public static implicit operator long(HashedLongId id)
        => id.Value;

    public static implicit operator HashedLongId(long id)
        => new HashedLongId(id);
}
