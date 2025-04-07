using FastEndpoints;
using FastEndpoints.Swagger;
using HangfireParallelTasks.Services.Registration;
using Microsoft.AspNetCore.Http.Features;
using System.Reflection;
using Taskmanager.Infrastructure;
using TaskManager.SignalR;
var builder = WebApplication.CreateBuilder(args);


RegisterServices(builder.Services, builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerGen();
}



app.UseFastEndpoints(x => x.Errors.UseProblemDetails());


app.UseCors("VueSPA");


app.UseHttpsRedirection();

app.UseExceptionHandler();

app.UseStatusCodePages();

app.MapHub<TaskHub>("/taskHub");


app.Run();

static void RegisterServices(IServiceCollection services, IConfiguration configuration)
{

    services.AddSignalR();


    services.AddExceptionHandler<GlobalExceptionHandler>();

    services.AddProblemDetails(options => options.CustomizeProblemDetails = problemContext =>
    {
        problemContext.ProblemDetails.Instance = $"{problemContext.HttpContext.Request.Method} {problemContext.HttpContext.Request.Path}";
        problemContext.ProblemDetails.Extensions.TryAdd("requestId", problemContext.HttpContext.TraceIdentifier);
        var activity = problemContext.HttpContext.Features.Get<IHttpActivityFeature>()?.Activity;
        problemContext.ProblemDetails.Extensions.TryAdd("traceId", activity?.Id);
    });

    services.AddFastEndpoints();
    services.AddSwaggerDocument();


    services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

    services.ConfigureSPACors(configuration);

    services.RegisterTaskManager(configuration);

}