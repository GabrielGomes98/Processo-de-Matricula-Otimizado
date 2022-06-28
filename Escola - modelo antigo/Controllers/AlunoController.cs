using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;
using Escola.Model;
using System.Text.Json;

namespace Escola.Controllers;

[ApiController]
[Route("[controller]")]
public class AlunoController : ControllerBase
{

    private static List<Aluno> matriculas = new List<Aluno>();
    private static int montaRA = 1 ;

    public bool ValidaRA(string ra){
        try{
            int raNumerico = Int16.Parse(ra);
            return true;
        }catch{
            return false;
        }
    }

    [HttpPost]
    public ActionResult<bool> realizarMatricula(Aluno aluno){
        int Posicao;
        int maiorPosicao = 0;
        Aluno maiorComMaiorPosicao = matriculas.Where(x => x.Serie == aluno.Serie).OrderBy(x => x.RA).LastOrDefault();
        if(maiorComMaiorPosicao != null){
        maiorPosicao = int.Parse(maiorComMaiorPosicao.RA.Substring(maiorComMaiorPosicao.RA.Length -2));
        }

        bool contemNumeroNoNome = aluno.Nome.Any(char.IsDigit);
        if (contemNumeroNoNome){
          return StatusCode(500, "O nome do aluno não pode conter números.");
        }
                
        if(maiorComMaiorPosicao == null){
            Posicao = 1;
        }else{
            Posicao = maiorPosicao + 1;
        }
        if(Posicao <= 9){
            aluno.RA = $"0{aluno.Serie.ToString()}0{Posicao}";
        }else{
            aluno.RA = $"0{aluno.Serie.ToString()}{Posicao}";
        }

        bool raContainsInt =  aluno.RA.Any(c => char.IsDigit(c));
        if(!raContainsInt){
          return StatusCode(500, "O RA do aluno deve conter apenas números inteiros.");
        }

        String RAtexto = $"{aluno.Serie} {Posicao}";
        //aluno.RA = $"{aluno.Serie.ToString()}0{Posicao} ";
             
        matriculas.Add(aluno);           
         return StatusCode(200, $"O aluno {aluno.Nome} foi adicionado a série {aluno.Serie.ToString()} com sucesso.");
    }

    

    [HttpDelete("{ra}")]
    public ActionResult<bool> cancelarMatricula(string ra){
        Aluno? aux;
        try{
            int raNumerico = Int16.Parse(ra);
        }catch{
            return StatusCode(500, "RA inserido não possui um formato aceitável(numérico).");
        }
        try{
            aux = matriculas.Where(x => Int16.Parse(x.RA) == Int16.Parse(ra)).FirstOrDefault();
        } catch{
            return StatusCode(500, "Plataforma incompátivel");
        }
        if (aux != null){
            matriculas.Remove(aux);
            return  StatusCode(200, "Aluno excluído com sucesso !!");
        }else{
            return NotFound("Aluno não encontrado!"); //returna http status 404
        }
    }



    [HttpPut("{ra}")]
    public ActionResult<bool> alterarMatricula(String ra, Aluno aluno){
        Aluno alunoOld;
        int Posicao = 0;
        ValidaRA(ra);
        Aluno alunoComMaiorPosicaoNaNovaSerie = matriculas.Where(x => x.Serie == aluno.Serie).OrderBy(x => x.RA).LastOrDefault();
        List<Aluno> alunosNovaSerie = matriculas.Where(x => x.RA == ra).ToList();
        try{
            alunoOld = matriculas.Where(x => x.RA == ra).FirstOrDefault();
        } catch (System.Exception){
            return StatusCode(500, "A plataforma não está preparada para essa chamada.");
        }
        if(alunoOld == null){
            return NotFound("Aluno não existe");
        }

        if(aluno.Nome != "" || aluno.Nome != null){
            alunoOld.Nome = aluno.Nome;
        }
        if(aluno.Serie > 0 || aluno.Serie < 10){
            alunoOld.Serie = aluno.Serie;
        }
        if(alunoComMaiorPosicaoNaNovaSerie == null){
            alunoOld.RA = $"0{aluno.Serie.ToString()}01";
        }else{
            Posicao =  int.Parse(alunoComMaiorPosicaoNaNovaSerie.RA.Substring(alunoComMaiorPosicaoNaNovaSerie.RA.Length -2)) + 1;
            alunoOld.RA = $"0{aluno.Serie.ToString()}0{Posicao}";
        }


        return Ok(true);
    }

    [HttpPatch("{ra}/{nota}")]
    public ActionResult<bool> lancaNota(int ra, decimal nota){
        Aluno alunoOld;
        try{
            alunoOld = matriculas.Where(x => int.Parse(x.RA) == ra).FirstOrDefault();
        }catch(System.Exception){
            return StatusCode(500, "A plataforma não está preparada para essa chamada");
        }
        if(alunoOld == null){
            return NotFound("Aluno não existe");
        }

        if(nota >= 0 && nota <= 10){
            alunoOld.Nota = nota;
        }
        return Ok(true);
    }

    
    [HttpGet("{serie}")]
    public ActionResult<List<Aluno>> criarDiario(int serie){
        List<Aluno> sala;
        try{
            sala = matriculas.Where(x => x.Serie == serie).OrderBy(x => x.RA).ToList();
        }catch{
            return StatusCode(500, "Plataforma Incompátivel"); 
        }
        if(sala == null) {
            return NotFound("Série pesquisada não existe !");
        }
         /* else {
            return StatusCode(500, "Não possui alunos cadastrado nesta sala.");
        } */
        return Ok(sala);
    } 

        [HttpGet]
    public ActionResult<string> aprovadosResprovados(int serie){
        List<Aluno> sala;
        List<Aluno> aprovados;
        List<Aluno> reprovados;
        try{
            sala = matriculas.OrderBy(x => x.Nome).ToList();
            aprovados = sala.Where(x => x.Nota >= 6).OrderBy(x => x.Nome).ToList();
            reprovados = sala.Where(x => x.Nota < 6).OrderBy(x => x.Nome).ToList();
        }catch{
            return StatusCode(500, "Plataforma Incompátivel"); 
        }
        
         /* else {
            return StatusCode(500, "Não possui alunos cadastrado nesta sala.");
        } */
        return Ok($"ALUNO APROVADOS :  { string.Join(", ",JsonSerializer.Serialize(aprovados)) } ALUNOS REPROVADOS: { string.Join(", ",JsonSerializer.Serialize(reprovados)) }");
    }
}
