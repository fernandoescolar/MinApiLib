namespace MinApiLib.HashedIds;

public class HashedIdJsonConverter : JsonConverter<HashedId>
{
    public override HashedId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            var value = reader.GetString();
            if (HashedId.TryParse(value, out var result))
            {
                return result;
            }
        }

        return new HashedId(-1);
    }

    public override void Write(Utf8JsonWriter writer, HashedId value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}
