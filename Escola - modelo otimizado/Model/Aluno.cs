using System.Text.Json;
using System.Text.Json.Serialization;
namespace Escola.Model;

public class Aluno
{
    public Aluno()
    {
        Notas = new List<double>();
    }

    public Aluno(string ra, string nome, int serie) : this()
    {
        this.RA = ra;
        this.Nome = nome;
        this.Serie = serie;
    }

    public String? RA { get; set; }
    public String? Nome { get; set; }
    public int Serie { get; set; }
    [JsonIgnore]
    public List<double>? Notas { get; set; }
    public double Nota => Notas.Any() ? Math.Round(Notas.Average(), 1) : 0.0;
}