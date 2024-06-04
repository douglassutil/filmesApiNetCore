using AutoMapper;
using FilmesAPI.Data;
using FilmesAPI.Data.Dtos;
using FilmesAPI.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace FilmesAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class SessaoController : ControllerBase
{
    private FilmeContext _context;
    private IMapper _mapper;

    public SessaoController(FilmeContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public IActionResult AdicionarSessao([FromBody] CreateSessaoDto sessaoDto)
    {
        Sessao sessao = _mapper.Map<Sessao>(sessaoDto);
        _context.Sessoes.Add(sessao);
        _context.SaveChanges();
        return CreatedAtAction(
            nameof(RecuperarSessaoPorId), 
            new { filmeId = sessao.FilmeId, cinemaId = sessao.CinemaId },
            sessao
        );
    }

    [HttpGet]
    public IEnumerable<ReadSessaoDto> RecuperarSessoes()
    {
        return _mapper.Map<List<ReadSessaoDto>>(_context.Sessoes.ToList());
    }

    [HttpGet("{filmeId}/{cinemaId}")]
    public IActionResult RecuperarSessaoPorId(int filmeId, int cinemaId)
    {
        var sessao = _context.Sessoes.FirstOrDefault(sessao =>
            sessao.FilmeId == filmeId && sessao.CinemaId == cinemaId);
        if (sessao == null) return NotFound();
        ReadSessaoDto sessaoDto = _mapper.Map<ReadSessaoDto>(sessao);
        return Ok(sessaoDto);
    }

    [HttpPut("{filmeId}/{cinemaId}")]
    public IActionResult AtualizarSessao(int filmeId, int cinemaId, [FromBody] UpdateSessaoDto sessaoDto)
    {
        var sessao = _context.Sessoes.FirstOrDefault(sessao =>
            sessao.FilmeId == filmeId && sessao.CinemaId == cinemaId);
        if (sessao == null) return NotFound();
        _mapper.Map(sessaoDto, sessao);
        _context.SaveChanges();
        return NoContent(); 
    }

    [HttpPatch("{filmeId}/{cinemaId}")]
    public IActionResult AtualizarSessaoParcial(int filmeId, int cinemaId, [FromBody] JsonPatchDocument<UpdateSessaoDto> patch)
    {
        var sessao = _context.Sessoes.FirstOrDefault(sessao =>
            sessao.FilmeId == filmeId && sessao.CinemaId == cinemaId);
        if (sessao == null) return NotFound();

        UpdateSessaoDto sessaoParaAtualizar = _mapper.Map<UpdateSessaoDto>(sessao);
        patch.ApplyTo(sessaoParaAtualizar, ModelState);

        if (!TryValidateModel(sessaoParaAtualizar)) return ValidationProblem(ModelState);

        _mapper.Map(sessaoParaAtualizar, sessao);
        _context.SaveChanges();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public IActionResult DeletarSessao(int filmeId, int cinemaId, [FromBody] JsonPatchDocument<UpdateSessaoDto> patch)
    {
        var sessao = _context.Sessoes.FirstOrDefault(sessao =>
            sessao.FilmeId == filmeId && sessao.CinemaId == cinemaId);
        if (sessao == null) return NotFound();
        _context.Remove(sessao);
        _context.SaveChanges();
        return NoContent();
    }
}
