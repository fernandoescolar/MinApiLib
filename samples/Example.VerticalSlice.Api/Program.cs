var builder = WebApplication.CreateBuilder(args);
builder.Services.AddBeerDbContext();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => { c.MapType<HashedId>(() => new OpenApiSchema { Type = JsonSchemaType.String }); c.CustomSchemaIds(x => x.FullName); });
builder.Services.AddHashedIds(op => op.Passphrase = builder.Configuration["HashedIds:Passphrase"]);
builder.Services.AddHypermedia();
builder.Services.AddAssembly();
builder.Services.AddValidatorsFromAssembly();

var app = builder.Build();
app.SeedBeerDbData(); // do not do that in your code. Data seed is only for demo purposes. If want to seed data, use a IHostedService.
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.CatchOperationCanceled();
app.MapEndpoints()
   .WithHypermedia();

app.Run();
