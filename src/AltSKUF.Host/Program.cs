using Dutchskull.Aspire.PolyRepo;
using k8s.Models;

var builder = DistributedApplication.CreateBuilder(args);

var repository = builder.AddRepository(
    "repository",
    "https://github.com/ALTSKUF/ALTSKUF.BACK.HealthCheck",
    c => c.WithDefaultBranch("master")
        .WithTargetPath("../../repos"));

var dotnetProject = builder
    .AddProjectFromRepository("healthcheck", repository,
        "../../repos/ALTSKUF.BACK.HealthCheck/HealthCheck.csproj");

builder.Build().Run();