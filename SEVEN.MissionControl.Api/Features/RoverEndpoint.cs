using SEVEN.Core.Models.Files;
using SEVEN.MissionControl.Api.Data.Contexts;
using SEVEN.MissionControl.Api.Models.Files;

namespace SEVEN.MissionControl.Api.Features;

public static class RoverEndpoint
{
    public static RouteGroupBuilder RoverGroup(this RouteGroupBuilder group)
    {
        group.MapGet("/{id}", GetRover).WithName("GetRover").WithOpenApi();
        group.MapPost("/", PostSingleFile).WithName("PostSingleFile").WithOpenApi();

        return group;
    }

    private static async Task<IResult> GetRover(Guid id, HttpContext httpContext, MissionControlContext context)
    {
        var rover = await context.Rovers.FindAsync(id);
        if (rover != null)
        {
            context.Entry(rover).Collection(b => b.Tasks).Load();
            return Results.Ok(rover);
        }

        return Results.NotFound();
    }

    private static async Task<IResult> PostSingleFile(FileUploadRequest request, MissionControlContext context)
    {
        if (request.FormFile is null)
            return Results.BadRequest();
        
        var fileDetail = new FileDetail()
        {
            Id = Guid.NewGuid(),
            FileName = request.FormFile.FileName,
            Type = request.Type,
        };

        using (var stream = new MemoryStream())
        {
            await request.FormFile.CopyToAsync(stream);
            fileDetail.FileData = stream.ToArray();
        }

        await context.FileDetails.AddAsync(fileDetail);
        await context.SaveChangesAsync();
        return Results.Ok();
    }
}