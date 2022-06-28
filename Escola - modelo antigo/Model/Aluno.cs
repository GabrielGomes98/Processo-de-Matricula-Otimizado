using System.Text.Json;
using System.Text.Json.Serialization;
namespace Escola.Model;

public class Aluno{
    public String? RA{get; set;}
    public String? Nome{get; set;}
    public int  Serie{get; set;}
    public decimal Nota{get; set;}
}