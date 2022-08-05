using Microsoft.EntityFrameworkCore;
using WalkthroughApp_API.DAL;
using WalkthroughApp_API.DAL.JobTitles;
using WalkthroughApp_API.DAL.Questions;
using WalkthroughApp_API.DAL.Walkthroughs;
using WalkthroughApp_API.Data;
using WalkthroughApp_API.Entities;
using WalkthroughApp_API.Helpers;

void BuildDependencies(WebApplicationBuilder webApplicationBuilder)
{
    webApplicationBuilder.Services.AddTransient<IEncodePassword, Decryption>();

    webApplicationBuilder.Services.AddTransient(typeof(IGetQuestions), typeof(GetQuestions));
    webApplicationBuilder.Services.AddTransient(typeof(IGet<Question>), typeof(GetQuestions));
    webApplicationBuilder.Services.AddTransient(typeof(ICreate<Question, NewQuestion>), typeof(CreateQuestion));
    webApplicationBuilder.Services.AddTransient(typeof(IDelete<Question>), typeof(DeleteQuestion));
    webApplicationBuilder.Services.AddTransient(typeof(IUpdate<Question>), typeof(UpdateQuestion));

    webApplicationBuilder.Services.AddTransient(typeof(IGet<Walkthrough>), typeof(GetWalkthroughs));
    webApplicationBuilder.Services.AddTransient(typeof(ICreateWalkthrough), typeof(CreateWalkthrough));
    webApplicationBuilder.Services.AddTransient(typeof(ICreate<Walkthrough, NewWalkthrough>), typeof(CreateWalkthrough));
    webApplicationBuilder.Services.AddTransient(typeof(IDelete<Walkthrough>), typeof(DeleteWalkthrough));
    webApplicationBuilder.Services.AddTransient(typeof(IUpdate<Walkthrough>), typeof(UpdateWalkthrough));

    webApplicationBuilder.Services.AddTransient(typeof(IGet<JobTitle>), typeof(GetJobs));
    webApplicationBuilder.Services.AddTransient(typeof(ICreate<JobTitle, NewJobTitle>), typeof(CreateJob));
    webApplicationBuilder.Services.AddTransient(typeof(IDelete<JobTitle>), typeof(DeleteJob));
    webApplicationBuilder.Services.AddTransient(typeof(IUpdate<JobTitle>), typeof(UpdateJob));
}

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

BuildDependencies(builder);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.Run();
