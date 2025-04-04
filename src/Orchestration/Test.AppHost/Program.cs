var builder = DistributedApplication.CreateBuilder(args);

var idpPostgres = builder.AddPostgres("idpPostgres")
    .WithPgAdmin()
    .WithLifetime(ContainerLifetime.Persistent)
    .AddDatabase("idpDb");

var idp = builder.AddProject<Projects.Test_Idp>("idp")
    .WaitFor(idpPostgres)
    .WithReference(idpPostgres)
    .WithExternalHttpEndpoints();
    
var api = builder.AddProject<Projects.Test_Api>("api");

var web = builder.AddProject<Projects.Test_Web>("web")
    .WithExternalHttpEndpoints()
    .WithReference(api);

var idpHttps = idp.GetEndpoint("https");
var webHttps = web.GetEndpoint("https");

idp.WithEnvironment("WEB_HTTPS", webHttps);
web.WithEnvironment("IDP_HTTPS", idpHttps);
api.WithEnvironment("IDP_HTTPS", idpHttps);

builder.Build().Run();