using testmaker.Api.Features.Classes;
using testmaker.Api.Features.Subjects;
using testmaker.Api.Features.Schools;
using testmaker.Api.Features.Questions;
using testmaker.Api.Features.Tests;
using testmaker.Api.Features.QuestionTypes;
using testmaker.Api.Features.TestTypes;
using testmaker.Api.Features.QuestionDifficulties;
using testmaker.Api.Middleware;
using testmaker.Application;
using testmaker.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.CustomSchemaIds(type => type.FullName?.Replace('+', '.') ?? type.Name);
});

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAuthorization();

// Map feature endpoints
app.MapClassEndpoints();
app.MapSubjectEndpoints();
app.MapSchoolEndpoints();
app.MapQuestionEndpoints();
app.MapTestEndpoints();
app.MapQuestionTypeEndpoints();
app.MapTestTypeEndpoints();
app.MapQuestionDifficultyEndpoints();

app.Run();
