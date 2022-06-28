using Escola.Model;
namespace Escola.Service;
using System.Text.Json;

public class EscolaService : IEscolaService
{
    private static List<Aluno> matriculas = new List<Aluno>();
    private static int montaRA = 1;

    public List<Aluno> getSerie(int serie)
    {
        List<Aluno> sala;
        sala = matriculas.Where(x => x.Serie == serie).OrderBy(x => x.RA).ToList();

        return sala;
    }

    public string Save(Aluno aluno)
    {
        if (aluno == null)
        {
            throw new NullReferenceException();
        }
        if (aluno.Nome == null || aluno.Nome == "" || aluno.Serie < 0 || aluno.Serie > 9)
        {
            throw new InvalidDataException();
        }

        int Posicao;
        int maiorPosicao = 0;
        Aluno maiorComMaiorPosicao = matriculas.Where(x => x.Serie == aluno.Serie).OrderBy(x => x.RA).LastOrDefault();
        if (maiorComMaiorPosicao != null)
        {
            maiorPosicao = int.Parse(maiorComMaiorPosicao.RA.Substring(maiorComMaiorPosicao.RA.Length - 2));
        }

        bool contemNumeroNoNome = aluno.Nome.Any(char.IsDigit);
        if (contemNumeroNoNome)
        {
            throw new InvalidDataException("O nome do aluno não pode conter números.");
        }

        if (maiorComMaiorPosicao == null)
        {
            Posicao = 1;
        }
        else
        {
            Posicao = maiorPosicao + 1;
        }
        if (Posicao <= 9)
        {
            aluno.RA = $"0{aluno.Serie.ToString()}0{Posicao}";
        }
        else
        {
            aluno.RA = $"0{aluno.Serie.ToString()}{Posicao}";
        }

        bool raContainsInt = aluno.RA.Any(c => char.IsDigit(c));
        if (!raContainsInt)
        {
            throw new InvalidDataException("O RA do aluno deve conter apenas números inteiros.");
        }

        String RAtexto = $"{aluno.Serie} {Posicao}";
        //aluno.RA = $"{aluno.Serie.ToString()}0{Posicao} ";

        matriculas.Add(aluno);
        return ($"O aluno {aluno.Nome} foi adicionado a série {aluno.Serie.ToString()} com sucesso.");
    }

    public void Nota(int ra, double nota)
    {
        Aluno alunoOld;
        alunoOld = getAlunoByRa(ra);

        if (nota < 0 || nota > 10)
        {
            throw new InvalidDataException();
        }
        alunoOld.Notas.Add(nota);
    }

    public bool Delete(int ra)
    {
        Aluno aluno = getAlunoByRa(ra);
        if (aluno == null)
        {
            throw new NullReferenceException();
        }
        matriculas.Remove(aluno);
        return true;
    }

    public void Update(string ra, Aluno aluno)
    {
        if (aluno == null)
        {
            throw new NullReferenceException();
        }
        if (aluno.Nome == "" || aluno.Nome == null)
        {
            throw new InvalidDataException();
        }
        if (aluno.Serie < 0 || aluno.Serie > 10)
        {
            throw new InvalidDataException();
        }
        int Posicao = 0;
        ValidaRA(ra);
        Aluno alunoComMaiorPosicaoNaNovaSerie = matriculas.Where(x => x.Serie == aluno.Serie).OrderBy(x => x.RA).LastOrDefault();
        Aluno alunoOld = getAlunoByRa(int.Parse(ra));

        if (aluno.Serie == 0)
        {
            aluno.Serie = alunoOld.Serie;

        }
        else if (alunoComMaiorPosicaoNaNovaSerie == null)
        {
            alunoOld.RA = $"0{aluno.Serie.ToString()}01";
        }
        else
        {
            if (alunoOld.Serie != aluno.Serie)
            {
                Posicao = int.Parse(alunoComMaiorPosicaoNaNovaSerie.RA.Substring(alunoComMaiorPosicaoNaNovaSerie.RA.Length - 2)) + 1;
                alunoOld.RA = $"0{aluno.Serie.ToString()}0{Posicao}";
            }
        }
        alunoOld.Nome = aluno.Nome;
        alunoOld.Serie = aluno.Serie;
    }

    public string Aprovacao(int serie)
    {
        List<Aluno> sala;
        List<Aluno> aprovados;
        List<Aluno> reprovados;

        sala = matriculas.OrderBy(x => x.Nome).ToList();
        aprovados = sala.Where(x => x.Nota >= 6).OrderBy(x => x.Nome).ToList();
        reprovados = sala.Where(x => x.Nota < 6).OrderBy(x => x.Nome).ToList();


        return $"ALUNO APROVADOS :  {string.Join(", ", JsonSerializer.Serialize(aprovados))} ALUNOS REPROVADOS: {string.Join(", ", JsonSerializer.Serialize(reprovados))}";
    }

    public bool ValidaRA(string ra)
    {
        try
        {
            int raNumerico = Int16.Parse(ra);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public Aluno getAlunoByRa(int ra)
    {
        Aluno aluno;
        aluno = matriculas.Where(x => int.Parse(x.RA) == ra).FirstOrDefault();

        return aluno;
    }
}