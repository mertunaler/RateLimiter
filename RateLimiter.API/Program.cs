using RateLimiter;
using System.Globalization;
using static System.Net.Mime.MediaTypeNames;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<ITokenBucket, TokenBucket>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();

app.Use(async (context, next) =>
{
    var bucket = context.RequestServices.GetRequiredService<ITokenBucket>();

    if (!bucket.IsValid(1))
    {
        context.Response.StatusCode = 429; 
        await context.Response.WriteAsync("Rate limit exceeded");
        return;
    }

    await next(context);
});

app.MapGet("/", () => "Hello, world!");

app.Run();


