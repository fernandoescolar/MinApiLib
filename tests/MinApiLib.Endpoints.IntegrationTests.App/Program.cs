global using MinApiLib.Endpoints;
global using MinApiLib.HashedIds;
global using MinApiLib.Hypermedia;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHashedIds(op => op.Passphrase = "my secret passphrase");
builder.Services.AddHypermedia();

var app = builder.Build();
app.MapEndpoints()
   .WithHypermedia();

app.Run();
