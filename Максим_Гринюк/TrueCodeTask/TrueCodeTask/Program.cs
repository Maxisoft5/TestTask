using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using TrueCodeTask.DataAccess.EFCore;
using TrueCodeTask.Helpers;
using TrueCodeTask.Interfaces;
using TrueCodeTask.Models;
using TrueCodeTask.Services.FileReaders;
using TrueCodeTask.Services.Posts;
using TrueCodeTask.Services.Users;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<StreamReaderSettings>(
    builder.Configuration.GetSection("StreamReaderSettings"));
var dbContextString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<DataContext>(o =>
{
    o.UseSqlServer(dbContextString);
});
var provider = builder.Services.BuildServiceProvider();
var context = provider.GetRequiredService<DataContext>();
context.Database.Migrate();

builder.Services.AddTransient<IPostsServices, PostsService>();
builder.Services.AddTransient<IUserService, UserService>();

builder.Services.AddTransient<IStreamReaderService, TxtFileReaderService>();
builder.Services.AddSingleton<FileParserFactory>();



builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt => {
    opt.SwaggerDoc("v1", new OpenApiInfo()
    {
        Version = "v1",
        Title = "Test API"
    });
    opt.EnableAnnotations();
});

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

public partial class Program { }