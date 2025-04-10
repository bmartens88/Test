﻿using System.Diagnostics.CodeAnalysis;
using Test.Api.Common.Domain.Errors;

namespace Test.Api.Common.Domain;

public class Result
{
    public Result(bool isSuccess, Error error)
    {
        if ((!isSuccess && error == Error.None)
            || (isSuccess && error != Error.None))
            throw new ArgumentException("Invalid value.", nameof(error));
        IsSuccess = isSuccess;
        Error = error;
    }

    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public Error Error { get; }

    public static Result Success()
    {
        return new Result(true, Error.None);
    }

    public static Result Failure(Error error)
    {
        return new Result(false, error);
    }

    public static Result<TValue> Success<TValue>(TValue value)
    {
        return new Result<TValue>(value, true, Error.None);
    }

    public static Result<TValue> Failure<TValue>(Error error)
    {
        return new Result<TValue>(default, false, Error.NullValue);
    }
}

public class Result<TValue>(TValue? value, bool isSuccess, Error error) : Result(isSuccess, error)
{
    private readonly TValue? _value = value;

    [NotNull]
    public TValue Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("The value of a failed result can't be accessed.");

    public static implicit operator Result<TValue>(TValue? value)
    {
        return value is not null ? Success(value) : Failure<TValue>(Error.NullValue);
    }

    public static Result<TValue> ValidationFailure(Error error)
    {
        return new Result<TValue>(default, false, error);
    }
}