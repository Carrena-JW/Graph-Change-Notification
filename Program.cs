using System.Net;
using msgraphapp;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var graphConfig = new GraphConfig();
builder.Configuration.Bind("GraphConfig", graphConfig);
builder.Services.AddSingleton(graphConfig);

var graphHelper = new GraphHelper(graphConfig);
builder.Services.AddSingleton(graphHelper);
// set listen ports
 
//builder.WebHost.ConfigureKestrel((context, serverOptions) =>
//{
//    serverOptions.Listen(IPAddress.Loopback, 5000);
//});


var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
