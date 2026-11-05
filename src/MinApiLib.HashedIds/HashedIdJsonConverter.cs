namespace MinApiLib.HashedIds;

public class HashedIdJsonConverter : JsonConverter<HashedId>
{
    public override HashedId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String && HashedId.TryParse(reader.GetString(), out var result))
        {
            return result;
        }

        throw new JsonException("The value is not a valid hashed id.");
    }

    public override void Write(Utf8JsonWriter writer, HashedId value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}

public class HashedLongIdJsonConverter : JsonConverter<HashedLongId>
{
    public override HashedLongId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String && HashedLongId.TryParse(reader.GetString(), out var result))
        {
            return result;
        }

        throw new JsonException("The value is not a valid hashed id.");
    }

    public override void Write(Utf8JsonWriter writer, HashedLongId value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}
