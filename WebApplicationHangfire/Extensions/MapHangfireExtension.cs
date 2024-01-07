using Hangfire;
using Hangfire.Storage;

namespace WebApplicationHangfire.Extensions;

public static class MapHangfireExtension
{
    public static void MapHangfireEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/recurring-job", () => GetRecurringJobs)
        .WithName("RecurringJobActive")
        .WithSummary("Return all recurring jobs active")
        .WithOpenApi();

        endpoints.MapPost("/recurring-job", (string jobid, string schedule) =>
            CreateRecurringJob(jobid, schedule)
        )
        .WithName("RecurringJobCreate")
        .WithSummary("Create new recurring job")
        .WithDescription("Obs.:If the informed jobid exists, it will be deleted and a new one will be created")
        .WithOpenApi();

        endpoints.MapDelete("/recurring-job", (string jobid) =>
        {
            RemoveRecurringJob(jobid);
        })
        .WithName("RecurringJobDelete")
        .WithSummary("Remove new recurring job")
        .WithOpenApi();
    }

    private static IResult RemoveRecurringJob(string jobid)
    {
        RecurringJob.RemoveIfExists(jobid);

        return Results.NotFound();
    }

    private static IResult CreateRecurringJob(string jobid, string schedule)
    {
        RecurringJob.RemoveIfExists(jobid);

        RecurringJob.AddOrUpdate(jobid, () => Console.WriteLine($"Easy! JobId: {jobid} - Cron: {schedule}"), schedule);

        return Results.Created($"/{jobid}", null);
    }

    private static IResult GetRecurringJobs()
    {
        var jobs = new List<RecurringJobDto>();

        using var connection = JobStorage.Current.GetConnection();

        foreach (var recurringJob in connection.GetRecurringJobs())
            jobs.Add(recurringJob);

        return Results.Ok(jobs?.Select(s => new
        {
            s.Id,
            s.Cron,
            s.Queue,
            s.NextExecution,
            s.LastJobId,
            s.LastJobState,
            s.LastExecution,
            s.CreatedAt,
            s.Removed,
            s.TimeZoneId,
            s.Error,
            s.RetryAttempt
        })?.ToList());
    }
}
