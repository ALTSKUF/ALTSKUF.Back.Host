using Dutchskull.Aspire.PolyRepo;
var builder = DistributedApplication.CreateBuilder(args);

var repository = builder.AddRepository(
    "repository",
    "https://github.com/ALTSKUF/ALTSKUF.BACK.HealthCheck.git",
    c => c.WithDefaultBranch("master")
        .WithTargetPath("../../repos"));




var msgBroker = builder.AddRabbitMQ("Messaging", port: 5672)
.WithManagementPlugin();


var redis = builder.AddRedis("Redis");


#region Services
var webApi = builder
    .AddProject<Projects.AltSKUF_WebApi>("Api")
    .WithReference(msgBroker)
    .WaitFor(msgBroker)
    .WithReference(redis);

var testService = builder
    .AddProjectFromRepository("healthCheck", repository,
        "../../repos/ALTSKUF.BACK.HealthCheck/HealthCheck.csproj")
    .WithReference(msgBroker)
    .WaitFor(msgBroker);
   
#endregion

builder.Build().Run();