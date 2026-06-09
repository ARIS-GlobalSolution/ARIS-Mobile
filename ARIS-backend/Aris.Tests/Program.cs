using System.ComponentModel.DataAnnotations;
using Aris.Application.DTOs.Culturas;
using Aris.Application.DTOs.Estufas;
using Aris.Application.DTOs.Sensores;
using Aris.Application.DTOs.Usuarios;
using Aris.Domain.Entities;

var failures = new List<string>();

Run("Usuario normaliza dados", () =>
{
    var usuario = new Usuario("  Ana Lima  ", "ANA@EMAIL.COM", "  hash123  ");

    AssertEqual("Ana Lima", usuario.Nome, "Nome");
    AssertEqual("ana@email.com", usuario.Email, "Email");
    AssertEqual("hash123", usuario.SenhaHash, "SenhaHash");
});

Run("Estufa valida usuario", () =>
{
    ExpectException<ArgumentException>(() => new Estufa("Estufa 01", "SP", 0));
});

Run("Sensor valida ids", () =>
{
    ExpectException<ArgumentException>(() => new Sensor(0, 1));
    ExpectException<ArgumentException>(() => new Sensor(1, 0));
});

Run("Cultura valida dados", () =>
{
    var cultura = new Cultura("  Alface  ", 1);
    AssertEqual("Alface", cultura.Nome, "Nome");
    AssertEqual(1, cultura.EstufaId, "EstufaId");
});

Run("Alerta normaliza risco", () =>
{
    var alerta = new Alerta("  umidade alta  ", "  critico  ", 1);
    AssertEqual("umidade alta", alerta.Mensagem, "Mensagem");
    AssertEqual("CRITICO", alerta.NivelRisco, "NivelRisco");
});

Run("Validacao DTO usuario", () =>
{
    var valid = TryValidate(new CreateUsuarioDto("Ana", "ana@email.com", "senha123"));
    AssertTrue(valid.isValid, "DTO válido deveria passar");

    var invalid = TryValidate(new CreateUsuarioDto("", "email-invalido", "123"));
    AssertTrue(!invalid.isValid, "DTO inválido deveria falhar");
});

Run("Validacao DTO estufa", () =>
{
    var valid = TryValidate(new CreateEstufaDto("Estufa 01", "SP", 1));
    AssertTrue(valid.isValid, "DTO válido deveria passar");

    var invalid = TryValidate(new CreateEstufaDto("", "SP", 0));
    AssertTrue(!invalid.isValid, "DTO inválido deveria falhar");
});

Run("Validacao DTO cultura", () =>
{
    var valid = TryValidate(new CreateCulturaDto("Alface", 1, 10, 25, 60, 80));
    AssertTrue(valid.isValid, "DTO válido deveria passar");

    var invalid = TryValidate(new CreateCulturaDto("", 0, null, null, null, null));
    AssertTrue(!invalid.isValid, "DTO inválido deveria falhar");
});

Run("Validacao DTO sensor", () =>
{
    var valid = TryValidate(new CreateSensorDto(1, 1));
    AssertTrue(valid.isValid, "DTO válido deveria passar");

    var invalid = TryValidate(new CreateSensorDto(0, 0));
    AssertTrue(!invalid.isValid, "DTO inválido deveria falhar");
});

if (failures.Count == 0)
{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("All ARIS tests passed.");
    Console.ResetColor();
    Environment.Exit(0);
}

Console.ForegroundColor = ConsoleColor.Red;
Console.WriteLine("Some ARIS tests failed:");
foreach (var failure in failures)
{
    Console.WriteLine("- " + failure);
}
Console.ResetColor();
Environment.Exit(1);

void Run(string name, Action test)
{
    try
    {
        test();
        Console.WriteLine($"PASS: {name}");
    }
    catch (Exception ex)
    {
        failures.Add($"{name}: {ex.Message}");
        Console.WriteLine($"FAIL: {name} - {ex.Message}");
    }
}

void AssertTrue(bool condition, string message)
{
    if (!condition)
        throw new InvalidOperationException(message);
}

void AssertEqual<T>(T expected, T actual, string label)
{
    if (!EqualityComparer<T>.Default.Equals(expected, actual))
        throw new InvalidOperationException($"{label} esperado '{expected}' mas foi '{actual}'.");
}

void ExpectException<TException>(Action action) where TException : Exception
{
    try
    {
        action();
    }
    catch (TException)
    {
        return;
    }

    throw new InvalidOperationException($"Era esperado {typeof(TException).Name}.");
}

(bool isValid, List<string> errors) TryValidate(object instance)
{
    var results = new List<ValidationResult>();
    var context = new ValidationContext(instance);
    var isValid = Validator.TryValidateObject(instance, context, results, validateAllProperties: true);
    return (isValid, results.Select(r => r.ErrorMessage ?? "Erro de validacao").ToList());
}
