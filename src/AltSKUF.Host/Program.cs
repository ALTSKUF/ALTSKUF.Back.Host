using Dutchskull.Aspire.PolyRepo;
var builder = DistributedApplication.CreateBuilder(args);

var repository = builder.AddRepository(
    "repository",
    "https://github.com/ALTSKUF/ALTSKUF.BACK.HealthCheck.git",
    c => c.WithDefaultBranch("master")
        .WithTargetPath("../../repos"));

var rmq = builder.AddRabbitMQ("rmq");
var redis = builder.AddRedis("Redis");


#region Services
var webApi = builder
    .AddProject<Projects.AltSKUF_WebApi>("Api")
    .WithReference(rmq)
    .WithReference(redis);

var testService = builder
    .AddProjectFromRepository("healthcheck", repository,
        "../../repos/ALTSKUF.BACK.HealthCheck/HealthCheck.csproj").WithReference(rmq);
   
#endregion

builder.Build().Run();