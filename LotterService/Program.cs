using LotterService.Handlers;
using LotterService.Interfaces;
using LotterService.Repositories;
using LotterService.Workers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHostedService<LotteryWorker>();
 
builder.Services.AddTransient<ILotteryResultsHandler, LotteryResultsHandler>();
builder.Services.AddTransient<ILotteryRepository, LotteryRepository>();

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
