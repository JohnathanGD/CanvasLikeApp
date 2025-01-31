﻿var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
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

app.UseHttpsRedirection();

app.MapPost("/uploadFile", async (HttpRequest httpResquest) =>
{
    if (!httpResquest.HasFormContentType)
    {
        return Results.BadRequest();
    }

    try
    {
        var formCollection = await httpResquest.ReadFormAsync();

        var iFormFile = formCollection.Files["fileContent"];

        if (iFormFile is null || iFormFile.Length == 0)
        {
            return Results.BadRequest();
        }

        using var stream = iFormFile.OpenReadStream();

        var localFilePath = Path.Combine("Files", iFormFile.FileName);

        using var localFileStream = File.OpenWrite(localFilePath);

        await stream.CopyToAsync(localFileStream);

        return Results.NoContent();
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex.Message);
        return Results.BadRequest();
    }
})
.Produces(StatusCodes.Status201Created)
.WithName("UploadFile")
.WithOpenApi();

app.Run();

