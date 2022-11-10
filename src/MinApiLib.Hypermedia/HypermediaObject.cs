namespace MinApiLib.Hypermedia;

public record HypermediaObject<T>(T Value, IEnumerable<HypermediaLink> Links) :  HypermediaResponse(Links);
