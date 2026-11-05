using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;

// The hasher is static, and some tests configure it differently.
[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace MinApiLib.HashedIds.Tests;

public class HashedIdTests
{
    private record Holder(HashedId Id);
    private record LongHolder(HashedLongId Id);
    private class Required
    {
        public required HashedId Id { get; set; }
    }

    public HashedIdTests()
    {
        new ServiceCollection().AddHashedIds(op => op.Passphrase = "tests");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(int.MaxValue)]
    public void Round_trips_through_its_hash(int value)
    {
        Assert.True(HashedId.TryParse(new HashedId(value).ToString(), out var parsed));
        Assert.Equal(value, parsed.Value);
    }

    [Theory]
    [InlineData(0L)]
    [InlineData(int.MaxValue + 1L)]
    [InlineData(long.MaxValue / 4)]
    public void Long_round_trips_through_its_hash(long value)
    {
        Assert.True(HashedLongId.TryParse(new HashedLongId(value).ToString(), out var parsed));
        Assert.Equal(value, parsed.Value);
    }

    [Fact]
    public void Converts_implicitly_from_and_to_its_value()
    {
        HashedId id = 5;
        int value = id;
        HashedLongId longId = 5L;
        long longValue = longId;

        Assert.Equal(5, value);
        Assert.Equal(5L, longValue);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("!!not-a-hash!!")]
    [InlineData("0")]
    [InlineData("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa")]
    [InlineData("zzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzz")]
    public void Rejects_values_that_are_not_a_hash(string? value)
    {
        Assert.False(HashedId.TryParse(value, out _));
        Assert.False(HashedLongId.TryParse(value, out _));
    }

    [Fact]
    public void Rejects_a_hash_of_several_numbers()
    {
        var hash = new HashidsNet.Hashids("tests").Encode(1, 2);

        Assert.False(HashedId.TryParse(hash, out _));
    }

    [Fact]
    public void Writes_the_hash_in_json()
    {
        var id = new HashedId(5);

        Assert.Equal($"{{\"Id\":\"{id}\"}}", JsonSerializer.Serialize(new Holder(id)));
    }

    [Fact]
    public void Reads_the_hash_from_json()
    {
        var id = new HashedId(5);
        var longId = new HashedLongId(5);

        Assert.Equal(id, JsonSerializer.Deserialize<Holder>($"{{\"Id\":\"{id}\"}}")!.Id);
        Assert.Equal(longId, JsonSerializer.Deserialize<LongHolder>($"{{\"Id\":\"{longId}\"}}")!.Id);
    }

    [Theory]
    [InlineData("""{"Id":"!!not-a-hash!!"}""")]
    [InlineData("""{"Id":5}""")]
    [InlineData("""{"Id":null}""")]
    [InlineData("""{"Id":{}}""")]
    public void Rejects_invalid_json_values(string json)
    {
        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<Holder>(json));
        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<LongHolder>(json));
    }

    [Fact]
    public void A_required_property_rejects_a_missing_json_value()
        => Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<Required>("{}"));

    [Fact]
    public void Applies_the_minimum_length()
    {
        new ServiceCollection().AddHashedIds(op =>
        {
            op.Passphrase = "tests";
            op.MinHashLength = 12;
        });

        var hash = new HashedId(1).ToString();

        Assert.True(hash.Length >= 12, hash);
        Assert.True(HashedId.TryParse(hash, out var parsed));
        Assert.Equal(1, parsed.Value);
    }

    [Fact]
    public void Requires_a_passphrase()
        => Assert.Throws<ArgumentNullException>(() => new ServiceCollection().AddHashedIds(_ => { }));
}
