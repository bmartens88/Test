var builder = DistributedApplication.CreateBuilder(args);

var idpPostgres = builder.AddPostgres("idpPostgres")
    .WithPgAdmin()
    .WithLifetime(ContainerLifetime.Persistent)
    .AddDatabase("idpDb");

var idp = builder.AddProject<Projects.Test_Idp>("idp")
    .WaitFor(idpPostgres)
    .WithReference(idpPostgres)
    .WithExternalHttpEndpoints();

var api = builder.AddProject<Projects.Test_Api>("api")
    .WithExternalHttpEndpoints();

var idpHttps = idp.GetEndpoint("https");
var apiHttps = api.GetEndpoint("https");

idp.WithEnvironment("WEB_HTTPS", apiHttps);
api.WithEnvironment("IDP_HTTPS", idpHttps);

builder.Build().Run();