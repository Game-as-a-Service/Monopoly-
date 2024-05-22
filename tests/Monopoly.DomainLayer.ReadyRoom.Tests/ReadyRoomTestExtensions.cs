using Monopoly.DomainLayer.Common;
using Monopoly.DomainLayer.ReadyRoom.Common;

namespace Monopoly.DomainLayer.ReadyRoom.Tests;

public static class ReadyRoomTestExtensions
{
    public static IEnumerable<DomainEvent> NextShouldBe(this IEnumerable<DomainEvent> events, DomainEvent expectedEvent)
    {
        using var enumerator = events.GetEnumerator();

        if (!enumerator.MoveNext())
        {
            throw new InvalidOperationException("No events to process.");
        }

        var actualEvent = enumerator.Current;

        Assert.AreEqual(expectedEvent, actualEvent);

        while (enumerator.MoveNext())
        {
            yield return enumerator.Current;
        }
    }
    
    public static IEnumerable<DomainEvent> IgnoreEvent<TEvent>(this IEnumerable<DomainEvent> events)
        where TEvent : DomainEvent
    {
        using var enumerator = events.GetEnumerator();

        while (enumerator.MoveNext())
        {
            if (enumerator.Current is TEvent)
            {
                continue;
            }

            yield return enumerator.Current;
        }
    }
    
    public static void WithNoEvents(this IEnumerable<DomainEvent> events)
    {
        using var enumerator = events.GetEnumerator();

        if (enumerator.MoveNext())
        {
            throw new InvalidOperationException("There are still events to process.");
        }
    }
}