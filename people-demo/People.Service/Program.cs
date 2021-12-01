var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options => options.ListenLocalhost(9874));

// Add services to the container.
builder.Services.AddSingleton<IPeopleProvider, HardCodedPeopleProvider>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/people", async (IPeopleProvider provider) =>
    {
        await Task.Delay(3000);
        provider.GetPeople();
    })
    .WithName("GetPeople");

app.MapGet("/people/{id}", async (IPeopleProvider provider, int id) =>
    {
        await Task.Delay(1000);
        return provider.GetPeople().FirstOrDefault(p => p.Id == id);
    })
    .WithName("GetPersonById");

app.MapGet("/people/ids", 
    (IPeopleProvider provider) => provider.GetPeople().Select(p => p.Id).ToList())
    .WithName("GetAllPersonIds");

app.Run();
