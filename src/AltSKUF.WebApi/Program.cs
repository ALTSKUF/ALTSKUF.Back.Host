var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddApiVersioning();
builder.Services.AddHttpClient();
builder.Services.AddLogging(c=>c.AddConsole());

var messagingConnectionString = builder.Configuration["ConnectionStrings:Messaging"];
builder.Services.AddSingleton(_ => new RpcServer(messagingConnectionString));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
app.MapSwagger();

var rpcServer = app.Services.GetRequiredService<RpcServer>();
await rpcServer.StartAsync();

app.MapPost("/api/rpc/send", async (string message, RpcServer rpcServer) =>
{
    if (string.IsNullOrEmpty(message))
    {
        return Results.BadRequest("Message cannot be empty.");
    }

    var response = await rpcServer.SendMessageToClientAsync(message);
    Console.WriteLine(response);
    return Results.Ok(new { SentMessage = message, ReceivedMessage = response });
});

app.UseSwagger();
app.UseSwaggerUI();
app.UseApiVersioning();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();