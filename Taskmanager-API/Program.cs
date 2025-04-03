using FastEndpoints;
using FastEndpoints.Swagger;
using HangfireParallelTasks.Services.Registration;
using System.Reflection;
using Taskmanager.Infrastructure;
var builder = WebApplication.CreateBuilder(args);


RegisterServices(builder.Services, builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerGen();
}


//app.UseFastEndpoints();
app.UseFastEndpoints(x => x.Errors.UseProblemDetails());


app.UseCors("VueSPA");


app.UseHttpsRedirection();

//// Converts unhandled exceptions into Problem Details responses
app.UseExceptionHandler();

//// Returns the Problem Details response for (empty) non-successful responses
//app.UseStatusCodePages();

app.Run();

static void RegisterServices(IServiceCollection services, IConfiguration configuration)
{
    services.AddExceptionHandler<GlobalExceptionHandler>();
    services.AddProblemDetails();

    services.AddFastEndpoints();
    services.AddSwaggerDocument();


    services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

    services.ConfigureSPACors(configuration);
    services.RegisterTaskManager(configuration);

}