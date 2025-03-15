using Dutchskull.Aspire.PolyRepo;
var builder = DistributedApplication.CreateBuilder(args);

var repository = builder.AddRepository(
    "repository",
    "https://github.com/ALTSKUF/ALTSKUF.BACK.HealthCheck.git",
    c => c.WithDefaultBranch("master")
        .WithTargetPath("../../repos"));

var kafka = builder.AddKafka("Kafka");
var redis = builder.AddRedis("Redis");


#region Services
var webApi = builder
    .AddProject<Projects.AltSKUF_WebApi>("Api")
    .WithReference(kafka)
    .WithReference(redis);

var testService = builder
    .AddProjectFromRepository("healthcheck", repository,
        "../../repos/ALTSKUF.BACK.HealthCheck/HealthCheck.csproj");
   
#endregion

builder.Build().Run();