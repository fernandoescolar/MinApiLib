global using MinApiLib.Endpoints;
global using MinApiLib.FluentValidation;
global using MinApiLib.HashedIds;
global using MinApiLib.Hypermedia;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHashedIds(op => op.Passphrase = "my secret passphrase");
builder.Services.AddHypermedia();
builder.Services.AddValidatorsFromAssembly();

var app = builder.Build();
app.MapEndpoints()
   .WithHypermedia();

app.Run();

public partial class Program;
