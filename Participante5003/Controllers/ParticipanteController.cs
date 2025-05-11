using Microsoft.AspNetCore.Mvc;

namespace Participante5003.Controllers;

[ApiController]
[Route("/")]
public class ParticipantController : ControllerBase
{
    private static readonly List<int> TodasAsPortas = new() { 5000, 5001, 5002, 5003 };
    private static readonly HttpClient http = new();

    private readonly int _portaAtual = 5003;
    private readonly string _arquivoLocal;

    public ParticipantController(IConfiguration config)
    {
        _arquivoLocal = $"participante_{_portaAtual}.txt";
    }

    [HttpPost("votar")]
    public ActionResult<bool> Votar([FromBody] string mensagem)
    {
        Console.WriteLine($"[{_portaAtual}] Votando NÃO para: {mensagem}");
        return false;
    }

    [HttpPost("commit")]
    public IActionResult Commitar([FromBody] string mensagem)
    {
        Console.WriteLine($"[{_portaAtual}] Commitando: {mensagem}");
        System.IO.File.AppendAllLines(_arquivoLocal, new[] { mensagem });
        return Ok();
    }

    [HttpPost("abort")]
    public IActionResult Abortar()
    {
        Console.WriteLine($"[{_portaAtual}] Abortado.");
        return Ok();
    }

    [HttpPost("iniciar")]
    public async Task<IActionResult> IniciarTransacao([FromBody] string mensagem)
    {
        Console.WriteLine($"[{_portaAtual}] Iniciando transação: {mensagem}");

        var outrasPortas = TodasAsPortas.Where(p => p != _portaAtual);
        var votos = new List<bool>();

        foreach (var porta in outrasPortas)
        {
            try
            {
                var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
                var resposta = await http.PostAsJsonAsync($"http://localhost:{porta}/votar", mensagem, cts.Token);
                var voto = await resposta.Content.ReadFromJsonAsync<bool>(cts.Token);
                votos.Add(voto);
            }
            catch
            {
                Console.WriteLine($"[{_portaAtual}] Falha ou timeout no participante {porta}");
                votos.Add(false);
            }
        }

        if (votos.All(v => v))
        {
            foreach (var porta in outrasPortas)
                await http.PostAsJsonAsync($"http://localhost:{porta}/commit", mensagem);
            System.IO.File.AppendAllLines(_arquivoLocal, new[] { mensagem });
            return Ok("Commit feito com todos.");
        }
        else
        {
            foreach (var porta in outrasPortas)
                await http.PostAsync($"http://localhost:{porta}/abort", null);
            return Ok("Transação abortada.");
        }
    }
}
