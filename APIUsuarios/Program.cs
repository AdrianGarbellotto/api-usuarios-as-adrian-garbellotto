using APIUsuarios.Application.DTOs;
using APIUsuarios.Application.Interfaces;
using APIUsuarios.Application.Services;
using APIUsuarios.Application.Validators;
using APIUsuarios.Infrastructure.Persistence;
using APIUsuarios.Infrastructure.Repositories;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();


builder.Services.AddScoped<IValidator<UsuarioCreateDto>, UsuarioCreateDtoValidator>();
builder.Services.AddScoped<IValidator<UsuarioUpdateDto>, UsuarioUpdateDtoValidator>();


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() 
    { 
        Title = "API de Gerenciamento de Usuários", 
        Version = "v1",
        Description = "API REST para gerenciamento de usuários utilizando ASP.NET Core Minimal APIs"
    });
});

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.MapGet("/usuarios", async (IUsuarioService service, CancellationToken ct) =>
{
    try
    {
        var usuarios = await service.ListarAsync(ct);
        return Results.Ok(usuarios);
    }
    catch (Exception ex)
    {
        return Results.Problem(
            detail: ex.Message,
            statusCode: StatusCodes.Status500InternalServerError
        );
    }
})
.WithName("ListarUsuarios")
.WithTags("Usuarios")
.Produces<IEnumerable<UsuarioReadDto>>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status500InternalServerError);

app.MapGet("/usuarios/{id:int}", async (int id, IUsuarioService service, CancellationToken ct) =>
{
    try
    {
        var usuario = await service.ObterAsync(id, ct);
        
        if (usuario is null)
            return Results.NotFound(new { message = $"Usuário com ID {id} não encontrado." });

        return Results.Ok(usuario);
    }
    catch (Exception ex)
    {
        return Results.Problem(
            detail: ex.Message,
            statusCode: StatusCodes.Status500InternalServerError
        );
    }
})
.WithName("ObterUsuario")
.WithTags("Usuarios")
.Produces<UsuarioReadDto>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status404NotFound)
.Produces(StatusCodes.Status500InternalServerError);

app.MapPost("/usuarios", async (
    UsuarioCreateDto dto, 
    IUsuarioService service, 
    IValidator<UsuarioCreateDto> validator,
    CancellationToken ct) =>
{
    try
    {
     
        var validationResult = await validator.ValidateAsync(dto, ct);
        
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .Select(e => new { field = e.PropertyName, message = e.ErrorMessage });
            return Results.BadRequest(new { errors });
        }

        if (await service.EmailJaCadastradoAsync(dto.Email, ct))
        {
            return Results.Conflict(new { message = "Este e-mail já está cadastrado." });
        }

        var usuario = await service.CriarAsync(dto, ct);
        return Results.Created($"/usuarios/{usuario.Id}", usuario);
    }
    catch (Exception ex)
    {
        return Results.Problem(
            detail: ex.Message,
            statusCode: StatusCodes.Status500InternalServerError
        );
    }
})
.WithName("CriarUsuario")
.WithTags("Usuarios")
.Produces<UsuarioReadDto>(StatusCodes.Status201Created)
.Produces(StatusCodes.Status400BadRequest)
.Produces(StatusCodes.Status409Conflict)
.Produces(StatusCodes.Status500InternalServerError);


app.MapPut("/usuarios/{id:int}", async (
    int id,
    UsuarioUpdateDto dto,
    IUsuarioService service,
    IValidator<UsuarioUpdateDto> validator,
    IUsuarioRepository repository,
    CancellationToken ct) =>
{
    try
    {
        var validationResult = await validator.ValidateAsync(dto, ct);
        
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .Select(e => new { field = e.PropertyName, message = e.ErrorMessage });
            return Results.BadRequest(new { errors });
        }

        var usuarioExistente = await service.ObterAsync(id, ct);
        if (usuarioExistente is null)
        {
            return Results.NotFound(new { message = $"Usuário com ID {id} não encontrado." });
        }

        var usuarioComEmail = await repository.GetByEmailAsync(dto.Email, ct);
        if (usuarioComEmail is not null && usuarioComEmail.Id != id)
        {
            return Results.Conflict(new { message = "Este e-mail já está cadastrado para outro usuário." });
        }

        var usuario = await service.AtualizarAsync(id, dto, ct);
        return Results.Ok(usuario);
    }
    catch (KeyNotFoundException ex)
    {
        return Results.NotFound(new { message = ex.Message });
    }
    catch (Exception ex)
    {
        return Results.Problem(
            detail: ex.Message,
            statusCode: StatusCodes.Status500InternalServerError
        );
    }
})
.WithName("AtualizarUsuario")
.WithTags("Usuarios")
.Produces<UsuarioReadDto>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status400BadRequest)
.Produces(StatusCodes.Status404NotFound)
.Produces(StatusCodes.Status409Conflict)
.Produces(StatusCodes.Status500InternalServerError);


app.MapDelete("/usuarios/{id:int}", async (int id, IUsuarioService service, CancellationToken ct) =>
{
    try
    {
        var removido = await service.RemoverAsync(id, ct);
        
        if (!removido)
            return Results.NotFound(new { message = $"Usuário com ID {id} não encontrado." });

        return Results.NoContent();
    }
    catch (Exception ex)
    {
        return Results.Problem(
            detail: ex.Message,
            statusCode: StatusCodes.Status500InternalServerError
        );
    }
})
.WithName("RemoverUsuario")
.WithTags("Usuarios")
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status404NotFound)
.Produces(StatusCodes.Status500InternalServerError);

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

app.Run();
