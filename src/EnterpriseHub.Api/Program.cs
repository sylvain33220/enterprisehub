using EnterpriseHub.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ✅ DB (centralisée)
builder.Services.AddDatabase(builder.Configuration);

var app = builder.Build();

// Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// (Option) tu peux commenter en dev si tu veux éviter le warning
app.UseHttpsRedirection();

app.MapControllers();

app.Run();
