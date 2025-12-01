using APIUsuarios.Application.DTOs;
using APIUsuarios.Application.Interfaces;
using APIUsuarios.Domain.Entities;

namespace APIUsuarios.Application.Services;

public class UsuarioService : IUsuarioService
{
    private readonly IUsuarioRepository _repository;

    public UsuarioService(IUsuarioRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<UsuarioReadDto>> ListarAsync(CancellationToken ct)
    {
        var usuarios = await _repository.GetAllAsync(ct);
        return usuarios.Select(MapToReadDto);
    }

    public async Task<UsuarioReadDto?> ObterAsync(int id, CancellationToken ct)
    {
        var usuario = await _repository.GetByIdAsync(id, ct);
        return usuario is null ? null : MapToReadDto(usuario);
    }

    public async Task<UsuarioReadDto> CriarAsync(UsuarioCreateDto dto, CancellationToken ct)
    {
        var usuario = new Usuario
        {
            Nome = dto.Nome.Trim(),
            Email = dto.Email.ToLowerInvariant().Trim(),
            Senha = dto.Senha, 
            DataNascimento = dto.DataNascimento,
            Telefone = dto.Telefone?.Trim(),
            Ativo = true,
            DataCriacao = DateTime.UtcNow
        };

        await _repository.AddAsync(usuario, ct);
        await _repository.SaveChangesAsync(ct);

        return MapToReadDto(usuario);
    }

    public async Task<UsuarioReadDto> AtualizarAsync(int id, UsuarioUpdateDto dto, CancellationToken ct)
    {
        var usuario = await _repository.GetByIdAsync(id, ct);
        
        if (usuario is null)
            throw new KeyNotFoundException($"Usuário com ID {id} não encontrado.");

        usuario.Nome = dto.Nome.Trim();
        usuario.Email = dto.Email.ToLowerInvariant().Trim();
        usuario.DataNascimento = dto.DataNascimento;
        usuario.Telefone = dto.Telefone?.Trim();
        usuario.Ativo = dto.Ativo;
        usuario.DataAtualizacao = DateTime.UtcNow;

        await _repository.UpdateAsync(usuario, ct);
        await _repository.SaveChangesAsync(ct);

        return MapToReadDto(usuario);
    }

    public async Task<bool> RemoverAsync(int id, CancellationToken ct)
    {
        var usuario = await _repository.GetByIdAsync(id, ct);
        
        if (usuario is null)
            return false;

        usuario.Ativo = false;
        usuario.DataAtualizacao = DateTime.UtcNow;
        
        await _repository.UpdateAsync(usuario, ct);
        await _repository.SaveChangesAsync(ct);

        return true;
    }

    public async Task<bool> EmailJaCadastradoAsync(string email, CancellationToken ct)
    {
        return await _repository.EmailExistsAsync(email.ToLowerInvariant().Trim(), ct);
    }

    private static UsuarioReadDto MapToReadDto(Usuario usuario)
    {
        return new UsuarioReadDto(
            usuario.Id,
            usuario.Nome,
            usuario.Email,
            usuario.DataNascimento,
            usuario.Telefone,
            usuario.Ativo,
            usuario.DataCriacao
        );
    }
}
