using Dutchskull.Aspire.PolyRepo;

var builder = DistributedApplication.CreateBuilder(args);

var testRepo = builder.AddRepository(
    "TestService",
    "https://github.com/ALTSKUF/ALTSKUF.BACK.HealthCheck.git",
    c => c.WithDefaultBranch("master")
        .WithTargetPath("../../repos"));

var userRepo = builder.AddRepository(
    "UserService",
    "https://github.com/ALTSKUF/AltSKUF.Back.Users.git",
    c => c.WithDefaultBranch("dev")
        .WithTargetPath("../../repos"));

var authRepo = builder.AddRepository(
    "AuthService",
    "https://github.com/ALTSKUF/AltSKUF.Back.Authentication.git",
    c => c.WithDefaultBranch("master")
        .WithTargetPath("../../repos"));

var msgBroker = builder
    .AddRabbitMQ("Messaging", port: 5672)
    .WithManagementPlugin();

var redis = builder.AddRedis("Redis");

var npgsqlUser = builder
    .AddPostgres("PostgresDB")
    .WithPgAdmin()
    .AddDatabase("userdb");

#region Services

var userService = builder
    .AddProjectFromRepository("UserService", userRepo,
        "../../repos/AltSKUF.Back.Users/src/AltSKUF.Server.Users/AltSKUF.Back.Users.csproj")
    .WithReference(msgBroker)
    .WithReference(npgsqlUser)
    .WaitFor(msgBroker)
    .WithHttpsEndpoint(port: 5020, name: "user")
    .WithEnvironment("DefaultOptions__AuthenticationServiceAddress", "https://localhost:5030");

var authService = builder
    .AddProjectFromRepository("AuthService", authRepo,
        "../../repos/AltSKUF.Back.Authentication/src/AltSKUF.Back.Authentication/AltSKUF.Back.Authentication.csproj")
    .WithReference(msgBroker)
    .WaitFor(msgBroker)
    .WaitFor(npgsqlUser)
    .WithHttpsEndpoint(port: 5030, name: "auth")
    .WaitFor(userService);

var testService = builder
    .AddProjectFromRepository("TestService", testRepo,
        "../../repos/ALTSKUF.BACK.HealthCheck/HealthCheck.csproj")
    .WithReference(msgBroker)
    .WaitFor(msgBroker)
    .WithHttpsEndpoint(port: 5110, name: "test");

var webApi = builder
    .AddProject<Projects.AltSKUF_WebApi>("Api")
    .WithReference(msgBroker)
    .WaitFor(msgBroker)
    .WithReference(redis)
    .WithHttpsEndpoint(port: 5010, name: "api")
    .WithEnvironment("UserPort", "5020");
#endregion

builder.Build().Run();