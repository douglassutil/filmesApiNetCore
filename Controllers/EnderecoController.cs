using AutoMapper;
using FilmesAPI.Data;
using FilmesAPI.Data.Dtos;
using FilmesAPI.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace FilmesAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class EnderecoController : ControllerBase
{
    private FilmeContext _context;
    private IMapper _mapper;

    public EnderecoController(FilmeContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public IActionResult AdicionarEndereco([FromBody] CreateEnderecoDto enderecoDto)
    {
        Endereco endereco = _mapper.Map<Endereco>(enderecoDto);
        _context.Enderecos.Add(endereco);
        _context.SaveChanges();
        return CreatedAtAction(nameof(RecuperarEnderecoPorId), new { Id = endereco.Id }, endereco);
    }

    [HttpGet]
    public IEnumerable<ReadEnderecoDto> RecuperarEnderecos([FromQuery] int skip = 0, [FromQuery] int take = 50)
    {
        return _mapper.Map<List<ReadEnderecoDto>>(_context.Enderecos.Skip(skip).Take(take));
    }

    [HttpGet("{id}")]
    public IActionResult RecuperarEnderecoPorId(int id)
    {
        var endereco = _context.Enderecos.FirstOrDefault(endereco => endereco.Id == id);
        if (endereco == null) return NotFound();
        ReadEnderecoDto enderecoDto = _mapper.Map<ReadEnderecoDto>(endereco);
        return Ok(enderecoDto);
    }

    [HttpPut("{id}")]
    public IActionResult AtualizarEndereco(int id, [FromBody] UpdateEnderecoDto enderecoDto)
    {
        var endereco = _context.Enderecos.FirstOrDefault(endereco => endereco.Id == id);
        if (endereco == null) return NotFound();
        _mapper.Map(enderecoDto, endereco);
        _context.SaveChanges();
        return NoContent();
    }

    [HttpPatch("{id}")]
    public IActionResult AtualizarEnderecoParcial(int id, [FromBody] JsonPatchDocument<UpdateEnderecoDto> patch)
    {
        var endereco = _context.Filmes.FirstOrDefault(endereco => endereco.Id == id);
        if (endereco == null) return NotFound();

        var enderecoParaAtualizar = _mapper.Map<UpdateEnderecoDto>(endereco);
        patch.ApplyTo(enderecoParaAtualizar, ModelState);

        if (!TryValidateModel(enderecoParaAtualizar)) return ValidationProblem(ModelState);

        _mapper.Map(enderecoParaAtualizar, endereco);
        _context.SaveChanges();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public IActionResult DeletarEndereco(int id, [FromBody] JsonPatchDocument<UpdateEnderecoDto> patch)
    {
        var endereco = _context.Enderecos.FirstOrDefault(endereco => endereco.Id == id);
        if (endereco == null) return NotFound();
        _context.Remove(endereco);
        _context.SaveChanges();
        return NoContent();
    }
}
