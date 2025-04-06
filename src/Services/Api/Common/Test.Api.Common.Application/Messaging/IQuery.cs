using MediatR;
using Test.Api.Common.Domain;

namespace Test.Api.Common.Application.Messaging;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>;