using Escola.Model;
namespace Escola.Service;

public interface IEscolaService
{
    public List<Aluno> getSerie(int serie);
    public void Nota(int ra, double nota);
    public string Save(Aluno aluno);
    public bool Delete(int ra);
    public void Update(string ra, Aluno aluno);
    public string Aprovacao(int serie);
    public Aluno getAlunoByRa(int ra);
}