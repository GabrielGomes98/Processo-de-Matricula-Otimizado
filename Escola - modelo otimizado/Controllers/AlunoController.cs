using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;
using Escola.Model;
using Escola.Service;

namespace Escola.Controllers;

[ApiController]
[Route("api/matriculas")]
public class AlunoController : ControllerBase
{
    private readonly IEscolaService _escolaService;

    public AlunoController(IEscolaService escolaService)
    {
        _escolaService = escolaService;
    }

    [HttpPost]
    public ActionResult<bool> realizarMatricula(Aluno aluno)
    {
        try
        {
            _escolaService.Save(aluno);
        }
        catch (NullReferenceException)
        {
            return StatusCode(400, "Objeto inválido.");
        }
        catch (InvalidDataException)
        {
            return StatusCode(400, "É necessário informar nome e serie válidos.");
        }

        return Ok(true);
    }

    [HttpDelete("{ra}")]
    public ActionResult<bool> cancelarMatricula(int ra)
    {
        Aluno aluno = _escolaService.getAlunoByRa(ra);
        if (aluno == null)
        {
            return NotFound("Aluno não existe.");
        }
        return Ok(_escolaService.Delete(ra));
    }

    [HttpPut("{ra}")]
    public ActionResult<bool> alterarMatricula(String ra, Aluno aluno)
    {
        Aluno alunoOld = _escolaService.getAlunoByRa(int.Parse(ra));

        if (alunoOld == null)
        {
            return StatusCode(404, "Aluno não encontrado.");
        }
        try
        {
            _escolaService.Update(ra, aluno);
        }
        catch (NullReferenceException)
        {
            return StatusCode(400, "Parâmetro inválido.");
        }
        catch (InvalidDataException)
        {
            return StatusCode(400, "É necessário informar nome e série válidos.");
        }

        return Ok(true);
    }

    [HttpPatch("{ra}/nota/{nota}")]
    public ActionResult<bool> lancaNota(int ra, double nota)
    {
        try
        {
            _escolaService.Nota(ra, nota);
        }
        catch (NullReferenceException)
        {
            return StatusCode(404, "Aluno não encontrado.");
        }
        catch (InvalidDataException)
        {
            return StatusCode(404, "Nota inválida.");
        }
        return Ok(true);
    }

    [HttpGet("{serie}")]
    public ActionResult<List<Aluno>> criarDiario(int serie)
    {
        List<Aluno> list = _escolaService.getSerie(serie);

        if (!list.Any())
        {
            return StatusCode(200, "Não existem alunos cadastrados nessa série");
        }
        return Ok(list);
    }

    [HttpGet]
    public ActionResult<string> aprovadosResprovados(int serie)
    {
        try
        {
            return _escolaService.Aprovacao(serie);
        }
        catch
        {
            return StatusCode(500, "Plataforma Incompátivel");
        }
    }
}
