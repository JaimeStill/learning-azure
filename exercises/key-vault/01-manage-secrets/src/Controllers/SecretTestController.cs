using Microsoft.AspNetCore.Mvc;

namespace KeyVaultApp.Controllers;

[Route("api/[controller]")]
public class SecretTestController : Controller
{
    readonly IConfiguration config;

    public SecretTestController(IConfiguration config)
    {
        this.config = config;
    }

    IActionResult NoSecret(string name) => StatusCode(
        StatusCodes.Status500InternalServerError,
        $"Error: No secret named {name} was found..."
    );

    IActionResult Secret(string name, string value) => Content(
        $"{name}: {value}\n\n" +
        "This is for testing only! Never output a secret " +
        "to a response or anywhere else in a real app!"
    );

    [HttpGet]
    public IActionResult Get()
    {
        string name = "SecretPassword";
        string? value = config[name];

        return value is null
            ? NoSecret(name)
            : Secret(name, value);
    }
}