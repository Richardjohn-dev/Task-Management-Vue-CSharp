using FastEndpoints;
using FastEndpoints.Swagger;
using HangfireParallelTasks.Services.Registration;
using System.Reflection;
var builder = WebApplication.CreateBuilder(args);


RegisterServices(builder.Services, builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerGen();
}


app.UseFastEndpoints();

app.UseCors("VueSPA");


app.UseHttpsRedirection();

app.Run();

static void RegisterServices(IServiceCollection services, IConfiguration configuration)
{
    services.AddFastEndpoints();
    services.AddSwaggerDocument();


    services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

    services.ConfigureSPACors(configuration);
    services.RegisterTaskManager(configuration);

}