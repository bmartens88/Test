namespace Test.Api.Common.Domain.Enums;

public record SmartEnum<TEnum>(string Name, int Value) : SmartEnum<TEnum, int>(Name, Value)
    where TEnum : SmartEnum<TEnum, int>;

public abstract record SmartEnum<TEnum, TValue>
    where TEnum : SmartEnum<TEnum, TValue>
    where TValue : IEquatable<TValue>
{
    private static readonly List<TEnum> EnumValues = [];

    protected SmartEnum(string name, TValue value)
    {
        Name = name;
        Value = value;
        EnumValues.Add((TEnum)this);
    }

    public string Name { get; }
    public TValue Value { get; }

    public static bool TryFromName(string name, out TEnum? enumValue)
    {
        enumValue = EnumValues.FirstOrDefault(val =>
            val.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
        return enumValue is not null;
    }

    public static bool TryFromValue(TValue value, out TEnum? enumValue)
    {
        enumValue = EnumValues.FirstOrDefault(val =>
            val.Value.Equals(value));
        return enumValue is not null;
    }

    public static TEnum FromName(string name)
    {
        return EnumValues.FirstOrDefault(val => val.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
               ?? throw new ArgumentException("Invalid value.", nameof(name));
    }

    public static TEnum FromValue(TValue value)
    {
        return EnumValues.FirstOrDefault(val => val.Value.Equals(value))
               ?? throw new ArgumentException("Invalid value.", nameof(value));
    }
}