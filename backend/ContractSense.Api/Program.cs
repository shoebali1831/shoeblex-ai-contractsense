using ContractSense.Api.Data;
using ContractSense.Api.Models.Dto;
using ContractSense.Api.Services;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Pgvector.EntityFrameworkCore;

LoadLocalSecretsEnv();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", cors =>
    {
        cors.SetIsOriginAllowed(origin =>
            Uri.TryCreate(origin, UriKind.Absolute, out var uri) &&
            (uri.Host.Equals("localhost", StringComparison.OrdinalIgnoreCase) || uri.Host == "127.0.0.1"))
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseNpgsql(connectionString, npgsql => npgsql.UseVector());
});

builder.Services.AddHttpClient();
builder.Services.AddScoped<IDocumentService, DocumentService>();
builder.Services.AddScoped<IPdfExtractionService, PdfExtractionService>();
builder.Services.AddScoped<IChunkingService, ChunkingService>();
builder.Services.AddScoped<IOpenAiService, OpenAiService>();
builder.Services.AddScoped<IRagService, RagService>();
builder.Services.AddScoped<IClauseAnalysisService, ClauseAnalysisService>();
builder.Services.AddScoped<IRiskScoringService, RiskScoringService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseExceptionHandler(exceptionApp =>
{
    exceptionApp.Run(async context =>
    {
        var logger = context.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger("GlobalException");
        var exceptionFeature = context.Features.Get<IExceptionHandlerFeature>();
        if (exceptionFeature?.Error is not null)
        {
            logger.LogError(exceptionFeature.Error, "Unhandled API exception");
        }

        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/json";

        var payload = new ApiErrorResponseDto
        {
            Code = "internal_error",
            Message = "An unexpected server error occurred.",
            TraceId = context.TraceIdentifier
        };

        await context.Response.WriteAsJsonAsync(payload);
    });
});

app.UseHttpsRedirection();
app.UseCors("FrontendPolicy");
app.UseAuthorization();
app.MapControllers();

app.Run();

static void LoadLocalSecretsEnv()
{
    var cwd = Directory.GetCurrentDirectory();
    var candidatePaths = new[]
    {
        Path.Combine(cwd, "secrets", "keys.local.env"),
        Path.GetFullPath(Path.Combine(cwd, "..", "..", "secrets", "keys.local.env"))
    };

    var envPath = candidatePaths.FirstOrDefault(File.Exists);
    if (string.IsNullOrWhiteSpace(envPath))
    {
        return;
    }

    var mapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        ["OPENAI_API_KEY"] = "OpenAI__ApiKey",
        ["OPENAI_BASE_URL"] = "OpenAI__BaseUrl",
        ["OPENAI_CHAT_MODEL"] = "OpenAI__ChatModel",
        ["OPENAI_EMBEDDING_MODEL"] = "OpenAI__EmbeddingModel",
        ["DATABASE_URL"] = "ConnectionStrings__DefaultConnection"
    };

    foreach (var rawLine in File.ReadAllLines(envPath))
    {
        var line = rawLine.Trim();
        if (line.Length == 0 || line.StartsWith('#'))
        {
            continue;
        }

        var equalsIndex = line.IndexOf('=');
        if (equalsIndex <= 0)
        {
            continue;
        }

        var key = line[..equalsIndex].Trim();
        var value = line[(equalsIndex + 1)..].Trim();
        value = TrimWrappingQuotes(value);

        if (key.Length == 0 || value.Length == 0)
        {
            continue;
        }

        SetIfMissing(key, value);

        if (mapping.TryGetValue(key, out var mappedKey))
        {
            SetIfMissing(mappedKey, value);
        }
    }
}

static void SetIfMissing(string key, string value)
{
    if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable(key)))
    {
        Environment.SetEnvironmentVariable(key, value);
    }
}

static string TrimWrappingQuotes(string value)
{
    if (value.Length >= 2 &&
        ((value.StartsWith('"') && value.EndsWith('"')) ||
         (value.StartsWith('\'') && value.EndsWith('\''))))
    {
        return value[1..^1];
    }

    return value;
}
