using PlatformService.Data;
using PlatformService.Dtos;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using PlatformService.Models;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(opt => opt.UseInMemoryDatabase("InMem"));

builder.Services.AddControllers(
      options => {
        options.SuppressAsyncSuffixInActionNames = false;
    }
);
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IPlatformRepo, PlatformRepo>();


var app = builder.Build();

app.MapGet("api/platforms", async (IPlatformRepo repo, IMapper mapper) =>
{
    var platforms = await repo.GetAllPlatforms();
    return mapper.Map<IEnumerable<PlatformReadDto>>(platforms);
})
.Produces<IEnumerable<PlatformReadDto>>();

app.MapGet("api/platforms/{id}", GetPlatformById)
.WithMetadata(new EndpointNameMetadata("GetPlatformById"))
.Produces<PlatformReadDto>()
.Produces(StatusCodes.Status404NotFound);

app.MapPost("api/platforms", CreatePlatform)
.Produces<PlatformReadDto>()
.Produces(StatusCodes.Status201Created);

async Task<IResult> GetPlatformById(int id, IPlatformRepo repo, IMapper mapper)
{
    var platform = await repo.GetPlatformById(id);
    if (platform != null)
    {
        return Results.Ok(mapper.Map<PlatformReadDto>(platform));
    }
    else
    {
        return Results.NotFound();
    }
}

async Task<IResult> CreatePlatform([FromBody]PlatformCreateDto platformCreateDto, IPlatformRepo repo, IMapper mapper, LinkGenerator linker, HttpContext httpContext) {
    var platform = mapper.Map<Platform>(platformCreateDto);
    await repo.CreatePlatform(platform);
    await repo.SaveChanges();
    var platformReadDto = mapper.Map<PlatformReadDto>(platform);
    return Results.Created(linker.GetUriByName(httpContext, "GetPlatformById", new { id = platformReadDto.Id})!, platformReadDto);
}


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

PrepDb.PrepPopulation(app);

app.Run();
