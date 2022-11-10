namespace MinApiLib.HashedIds;

[JsonConverter(typeof(HashedIdJsonConverter))]
public record struct HashedId(int Value)
{
    public override string ToString()
    {
        var hasher = Hasher.Instance;
        var encoded = hasher.Encode(Value);
        return encoded;
    }

    public static bool TryParse(string value, out HashedId result)
    {
        if (!string.IsNullOrWhiteSpace(value))
        {
            var decoded = Hasher.Instance.Decode(value);
            if (decoded.Length == 1)
            {
                result = new HashedId(decoded.FirstOrDefault());
                return true;
            }
        }

        result = new HashedId(-1);
        return false;
    }

    public static implicit operator int(HashedId id)
    {
        return id.Value;
    }

    public static implicit operator HashedId(int id)
    {
        return new HashedId(id);
    }
}
