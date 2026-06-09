using System.ComponentModel.DataAnnotations;

namespace Aris.Application.DTOs.Usuarios;

public sealed record CreateUsuarioDto(
    [param: Required, MinLength(3)] string Nome,
    [param: Required, EmailAddress] string Email,
    [param: Required, MinLength(6)] string Senha);

public sealed record UpdateUsuarioDto(
    [param: Required, MinLength(3)] string Nome,
    [param: Required, EmailAddress] string Email,
    [param: Required, MinLength(6)] string Senha);

public sealed record UsuarioResponseDto(int Id, string Nome, string Email, DateTime DataCadastro);
