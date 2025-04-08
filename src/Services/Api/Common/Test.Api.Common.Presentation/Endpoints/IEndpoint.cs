﻿using Microsoft.AspNetCore.Routing;

namespace Test.Api.Common.Presentation.Endpoints;

public interface IEndpoint
{
    void MapEndpoint(IEndpointRouteBuilder app);
}