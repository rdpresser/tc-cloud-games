using FastEndpoints;
using FastEndpoints.Swagger;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddFastEndpoints()
    .SwaggerDocument();

builder.Services
    .AddHttpClient();

var app = builder.Build();

//app.UseAuthentication();
//app.UseAuthorization();

//app.UseFastEndpoints(config =>
//    {
//        config.Errors.UseProblemDetails();
//    })
//    .UseSwaggerGen();

app.UseFastEndpoints()
    .UseSwaggerGen();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Run();