using MinApiLib.FluentValidation;

namespace MinApiLib.FluentValidation.Tests;

public readonly record struct QueryRequest([FromQuery] int Page);

public class QueryRequestValidator : AbstractValidator<QueryRequest>
{
    public QueryRequestValidator()
    {
        RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
    }
}

public record BodyOnly(string Name);

public class BodyOnlyValidator : AbstractValidator<BodyOnly>
{
    public BodyOnlyValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
    }
}

public record NoValidator(string Name);

public class TokenCapturingValidator : AbstractValidator<NoValidator>
{
    public CancellationToken Token { get; private set; }

    public override Task<global::FluentValidation.Results.ValidationResult> ValidateAsync(ValidationContext<NoValidator> context, CancellationToken cancellation = default)
    {
        Token = cancellation;
        return Task.FromResult(new global::FluentValidation.Results.ValidationResult());
    }
}

public class ValidationBehaviorTests
{
    private static async Task<(WebApplication App, HttpClient Client)> StartAsync(Action<WebApplication> map, bool registerValidators = true)
    {
        var builder = WebApplication.CreateBuilder();
        builder.WebHost.UseTestServer();
        if (registerValidators)
        {
            builder.Services.AddValidatorsFromAssembly(typeof(BodyOnlyValidator).Assembly);
            builder.Services.RemoveAll(typeof(IValidator<NoValidator>));
        }

        var app = builder.Build();
        map(app);
        await app.StartAsync();
        return (app, app.GetTestClient());
    }

    private static StringContent Json(string json) => new(json, Encoding.UTF8, "application/json");

    [Theory]
    [InlineData("/q?page=0", HttpStatusCode.BadRequest)]
    [InlineData("/q?page=1", HttpStatusCode.OK)]
    public async Task Validates_query_values(string uri, HttpStatusCode expected)
    {
        var (app, client) = await StartAsync(a =>
            a.MapGet("/q", ([AsParameters] QueryRequest r) => Results.Ok()).WithValidation());
        await using var _ = app;

        Assert.Equal(expected, (await client.GetAsync(uri)).StatusCode);
    }

    [Fact]
    public async Task Finds_the_validated_parameter_in_any_position()
    {
        var (app, client) = await StartAsync(a =>
            a.MapPost("/b", (HttpContext c, BodyOnly body) => Results.Ok()).WithValidation());
        await using var _ = app;

        Assert.Equal(HttpStatusCode.BadRequest, (await client.PostAsync("/b", Json("""{"name":""}"""))).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await client.PostAsync("/b", Json("""{"name":"x"}"""))).StatusCode);
    }

    [Fact]
    public async Task Generic_overload_validates_the_parameter_of_that_type()
    {
        var (app, client) = await StartAsync(a =>
            a.MapPost("/b", (HttpContext c, BodyOnly body) => Results.Ok()).WithValidation<BodyOnly>());
        await using var _ = app;

        Assert.Equal(HttpStatusCode.BadRequest, (await client.PostAsync("/b", Json("""{"name":""}"""))).StatusCode);
    }

    [Fact]
    public async Task Without_a_validator_passes_and_logs_a_warning()
    {
        var logs = new CapturingLoggerProvider();
        var builder = WebApplication.CreateBuilder();
        builder.WebHost.UseTestServer();
        builder.Logging.AddProvider(logs);
        builder.Services.AddValidationFilter();
        await using var app = builder.Build();
        app.MapPost("/n", (NoValidator body) => Results.Ok()).WithValidation();
        await app.StartAsync();

        var response = await app.GetTestClient().PostAsync("/n", Json("""{"name":""}"""));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains(logs.Warnings, x => x.Contains("obsolete"));
    }

    [Fact]
    public async Task Generic_overload_without_a_validator_passes_and_logs_a_warning()
    {
        var logs = new CapturingLoggerProvider();
        var builder = WebApplication.CreateBuilder();
        builder.WebHost.UseTestServer();
        builder.Logging.AddProvider(logs);
        builder.Services.AddValidationFilter();
        await using var app = builder.Build();
        app.MapPost("/n", (NoValidator body) => Results.Ok()).WithValidation<NoValidator>();
        await app.StartAsync();

        var response = await app.GetTestClient().PostAsync("/n", Json("""{"name":""}"""));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains(logs.Warnings, x => x.Contains("obsolete"));
    }

    [Fact]
    public async Task With_a_validator_logs_no_warning()
    {
        var logs = new CapturingLoggerProvider();
        var builder = WebApplication.CreateBuilder();
        builder.WebHost.UseTestServer();
        builder.Logging.AddProvider(logs);
        builder.Services.AddValidatorsFromAssembly(typeof(BodyOnlyValidator).Assembly);
        await using var app = builder.Build();
        app.MapPost("/b", (BodyOnly body) => Results.Ok()).WithValidation();
        await app.StartAsync();

        await app.GetTestClient().PostAsync("/b", Json("""{"name":"x"}"""));

        Assert.DoesNotContain(logs.Warnings, x => x.Contains("obsolete"));
    }

    [Fact]
    public async Task Fails_when_the_filter_is_not_registered_even_without_a_validator()
    {
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            var builder = WebApplication.CreateBuilder();
            builder.WebHost.UseTestServer();
            await using var app = builder.Build();
            app.MapPost("/n", (NoValidator body) => Results.Ok()).WithValidation();
            await app.StartAsync();
            await app.GetTestClient().PostAsync("/n", Json("""{"name":"x"}"""));
        });

        Assert.Contains("AddValidationFilter", exception.Message);
    }

#pragma warning disable CS0618 // The obsolete method is what these tests cover.
    [Fact]
    public void Obsolete_AddValidation_registers_the_filter_with_the_given_lifetime()
    {
        var services = new ServiceCollection().AddValidation(ServiceLifetime.Scoped);

        Assert.Equal(ServiceLifetime.Scoped, services.Single(x => x.ServiceType == typeof(ValidationFilter<>)).Lifetime);
    }

    [Fact]
    public void AddValidation_without_arguments_compiles_and_only_registers_the_filter_before_net10()
    {
        var services = new ServiceCollection().AddValidation();

#if NET10_0_OR_GREATER
        // It's the method from ASP.NET Core: this package's one requires the lifetime.
        Assert.DoesNotContain(services, x => x.ServiceType == typeof(ValidationFilter<>));
#else
        Assert.Equal(ServiceLifetime.Singleton, services.Single(x => x.ServiceType == typeof(ValidationFilter<>)).Lifetime);
#endif
    }
#pragma warning restore CS0618

    [Fact]
    public void Obsolete_AddValidation_points_to_the_new_method()
    {
        var method = typeof(ValidationServiceCollectionExtensions).GetMethod(nameof(ValidationServiceCollectionExtensions.AddValidation));

        var attribute = Assert.Single(method!.GetCustomAttributes(typeof(ObsoleteAttribute), false));

        Assert.Contains("AddValidationFilter", ((ObsoleteAttribute)attribute).Message);
    }

    [Fact]
    public async Task A_custom_filter_without_a_validator_passes()
    {
        var http = new DefaultHttpContext { RequestServices = new ServiceCollection().BuildServiceProvider() };
        var context = EndpointFilterInvocationContext.Create(http, new NoValidator("x"));

        var result = await new ValidationFilter<NoValidator>().InvokeAsync(context, _ => ValueTask.FromResult<object?>("next"));

        Assert.Equal("next", result);
    }

    private sealed class CapturingLoggerProvider : ILoggerProvider, ILogger
    {
        public List<string> Warnings { get; } = new();

        public ILogger CreateLogger(string categoryName) => this;
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            if (logLevel == LogLevel.Warning)
            {
                lock (Warnings)
                {
                    Warnings.Add(formatter(state, exception));
                }
            }
        }

        public void Dispose()
        {
        }
    }

    [Fact]
    public async Task Generic_overload_fails_when_the_handler_has_no_parameter_of_that_type()
    {
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            var (app, client) = await StartAsync(a =>
                a.MapPost("/n", (NoValidator body) => Results.Ok()).WithValidation<BodyOnly>());
            await using var _ = app;
            await client.PostAsync("/n", Json("""{"name":"x"}"""));
        });
    }

    [Fact]
    public async Task Fails_with_a_clear_message_when_the_filter_is_not_registered()
    {
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            var builder = WebApplication.CreateBuilder();
            builder.WebHost.UseTestServer();
            builder.Services.AddSingleton<IValidator<BodyOnly>, BodyOnlyValidator>();
            await using var app = builder.Build();
            app.MapPost("/b", (BodyOnly body) => Results.Ok()).WithValidation();
            await app.StartAsync();
            await app.GetTestClient().PostAsync("/b", Json("""{"name":"x"}"""));
        });

        Assert.Contains("AddValidationFilter", exception.Message);
    }

    [Fact]
    public async Task Group_validates_endpoints_with_a_validator_and_skips_the_others()
    {
        var (app, client) = await StartAsync(a =>
        {
            var group = a.MapGroup("/g").WithValidation();
            group.MapPost("/validated", (BodyOnly body) => Results.Ok());
            group.MapPost("/unvalidated", (NoValidator body) => Results.Ok());
        });
        await using var _ = app;

        Assert.Equal(HttpStatusCode.BadRequest, (await client.PostAsync("/g/validated", Json("""{"name":""}"""))).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await client.PostAsync("/g/unvalidated", Json("""{"name":""}"""))).StatusCode);
    }

    [Fact]
    public async Task Passes_the_request_token_to_the_validator()
    {
        using var cts = new CancellationTokenSource();
        var validator = new TokenCapturingValidator();
        var services = new ServiceCollection().AddSingleton<IValidator<NoValidator>>(validator).BuildServiceProvider();
        var http = new DefaultHttpContext { RequestServices = services, RequestAborted = cts.Token };
        var context = EndpointFilterInvocationContext.Create(http, new NoValidator("x"));

        await new ValidationFilter<NoValidator>().InvokeAsync(context, _ => ValueTask.FromResult<object?>(null));

        Assert.Equal(cts.Token, validator.Token);
    }

    [Fact]
    public void Registers_the_filter_with_the_given_lifetime()
    {
        var services = new ServiceCollection().AddValidationFilter(ServiceLifetime.Scoped);

        Assert.Equal(ServiceLifetime.Scoped, services.Single(x => x.ServiceType == typeof(ValidationFilter<>)).Lifetime);
    }
}
