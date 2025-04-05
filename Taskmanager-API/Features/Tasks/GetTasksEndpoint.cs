
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
        new (new GroupItem("a1b2c3"), new SharedGroupIdentifier("group1")),
        new (new GroupItem("d4e5f6"), new SharedGroupIdentifier("group1")),
        new (new GroupItem("g7h8i9"), new SharedGroupIdentifier("group1")),
        new (new GroupItem("j1k2l3"), new SharedGroupIdentifier("group1")),
        new (new GroupItem("m4n5o6"), new SharedGroupIdentifier("group1")),

        new (new GroupItem("p7q8r9"), new SharedGroupIdentifier("group2")),
        new (new GroupItem("s1t2u3"), new SharedGroupIdentifier("group2")),
        new (new GroupItem("v4w5x6"), new SharedGroupIdentifier("group2")),
        new (new GroupItem("y7z8a9"), new SharedGroupIdentifier("group2")),
        new (new GroupItem("b1c2d3"), new SharedGroupIdentifier("group2")),
        new (new GroupItem("e4f5g6"), new SharedGroupIdentifier("group2")),

        new (new GroupItem("h7i8j9"), new SharedGroupIdentifier("group3")),
        new (new GroupItem("k1l2m3"), new SharedGroupIdentifier("group3")),
        new (new GroupItem("n4o5p6"), new SharedGroupIdentifier("group3")),
        new (new GroupItem("q7r8s9"), new SharedGroupIdentifier("group3")),
        new (new GroupItem("t1u2v3"), new SharedGroupIdentifier("group3")),
        new (new GroupItem("w4x5y6"), new SharedGroupIdentifier("group3")),
        new (new GroupItem("z7a8b9"), new SharedGroupIdentifier("group3")),

        new (new GroupItem("c1d2e3"), new SharedGroupIdentifier("group4")),
        new (new GroupItem("f4g5h6"), new SharedGroupIdentifier("group4")),
        new (new GroupItem("i7j8k9"), new SharedGroupIdentifier("group4")),
        new (new GroupItem("l1m2n3"), new SharedGroupIdentifier("group4")),
        new (new GroupItem("o4p5q6"), new SharedGroupIdentifier("group4")),
        new (new GroupItem("r7s8t9"), new SharedGroupIdentifier("group4")),
        new (new GroupItem("u1v2w3"), new SharedGroupIdentifier("group4")),

        new (new GroupItem("x4y5z6"), new SharedGroupIdentifier("group5")),
        new (new GroupItem("a7b8c9"), new SharedGroupIdentifier("group5")),
        new (new GroupItem("d1e2f3"), new SharedGroupIdentifier("group5")),
        new (new GroupItem("g4h5i6"), new SharedGroupIdentifier("group5")),
        new (new GroupItem("j7k8l9"), new SharedGroupIdentifier("group5")),
        new (new GroupItem("m1n2o3"), new SharedGroupIdentifier("group5")),
    ];

    public static GroupItems[] GetGroupedItems() =>
      GetAll
          .GroupBy(x => x.Group)
          .Select(g => new GroupItems(
              Group: g.Key,
              Items: g.Select(d => d.Item).ToArray()
          ))
          .ToArray();


    internal static bool DetailsExist(DomainEntityDetails details)
    {
        return GetAll.Contains(details);
    }
}