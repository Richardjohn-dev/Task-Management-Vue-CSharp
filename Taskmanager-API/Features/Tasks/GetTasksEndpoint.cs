
using Domain.Tasks.Enqueue;
using FastEndpoints;
using HangfireParallelTasks.Domain.Primitives;

public class GetTasksEndpoint : EndpointWithoutRequest<ApiResponse<DomainEntityDetails[]>>
{
    public override void Configure()
    {
        Get("/api/sample-data/");
        AllowAnonymous();

    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var data = SampleData.GetAll;
        await SendAsync(ApiResponse<DomainEntityDetails[]>.Success(data));
    }
}



public static class SampleData
{
    public static DomainEntityDetails[] GetAll { get; } =
    [
        new (new SomeItemInfo("a1b2c3"), new SharedGroupIdentifier("group1")),
        new (new SomeItemInfo("d4e5f6"), new SharedGroupIdentifier("group1")),
        new (new SomeItemInfo("g7h8i9"), new SharedGroupIdentifier("group1")),
        new (new SomeItemInfo("j1k2l3"), new SharedGroupIdentifier("group1")),
        new (new SomeItemInfo("m4n5o6"), new SharedGroupIdentifier("group1")),

        new (new SomeItemInfo("p7q8r9"), new SharedGroupIdentifier("group2")),
        new (new SomeItemInfo("s1t2u3"), new SharedGroupIdentifier("group2")),
        new (new SomeItemInfo("v4w5x6"), new SharedGroupIdentifier("group2")),
        new (new SomeItemInfo("y7z8a9"), new SharedGroupIdentifier("group2")),
        new (new SomeItemInfo("b1c2d3"), new SharedGroupIdentifier("group2")),
        new (new SomeItemInfo("e4f5g6"), new SharedGroupIdentifier("group2")),

        new (new SomeItemInfo("h7i8j9"), new SharedGroupIdentifier("group3")),
        new (new SomeItemInfo("k1l2m3"), new SharedGroupIdentifier("group3")),
        new (new SomeItemInfo("n4o5p6"), new SharedGroupIdentifier("group3")),
        new (new SomeItemInfo("q7r8s9"), new SharedGroupIdentifier("group3")),
        new (new SomeItemInfo("t1u2v3"), new SharedGroupIdentifier("group3")),
        new (new SomeItemInfo("w4x5y6"), new SharedGroupIdentifier("group3")),
        new (new SomeItemInfo("z7a8b9"), new SharedGroupIdentifier("group3")),

        new (new SomeItemInfo("c1d2e3"), new SharedGroupIdentifier("group4")),
        new (new SomeItemInfo("f4g5h6"), new SharedGroupIdentifier("group4")),
        new (new SomeItemInfo("i7j8k9"), new SharedGroupIdentifier("group4")),
        new (new SomeItemInfo("l1m2n3"), new SharedGroupIdentifier("group4")),
        new (new SomeItemInfo("o4p5q6"), new SharedGroupIdentifier("group4")),
        new (new SomeItemInfo("r7s8t9"), new SharedGroupIdentifier("group4")),
        new (new SomeItemInfo("u1v2w3"), new SharedGroupIdentifier("group4")),

        new (new SomeItemInfo("x4y5z6"), new SharedGroupIdentifier("group5")),
        new (new SomeItemInfo("a7b8c9"), new SharedGroupIdentifier("group5")),
        new (new SomeItemInfo("d1e2f3"), new SharedGroupIdentifier("group5")),
        new (new SomeItemInfo("g4h5i6"), new SharedGroupIdentifier("group5")),
        new (new SomeItemInfo("j7k8l9"), new SharedGroupIdentifier("group5")),
        new (new SomeItemInfo("m1n2o3"), new SharedGroupIdentifier("group5")),
    ];

    internal static bool DetailsExist(DomainEntityDetails details)
    {
        return GetAll.Contains(details);
    }
}