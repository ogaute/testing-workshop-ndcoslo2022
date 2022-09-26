namespace EdgeCases;

public interface IDateTimeProvider
{
    public DateTimeOffset Now { get; }
}

public class DateTimeProvider
{
    public DateTimeOffset Now => DateTimeOffset.Now;
}
