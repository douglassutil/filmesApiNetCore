using AutoMapper;
using FilmesAPI.Data;
using FilmesAPI.Data.Dtos;
using FilmesAPI.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FilmesAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class CinemaController : ControllerBase
{
    private FilmeContext _context;
    private IMapper _mapper;

    public CinemaController(FilmeContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public IActionResult AdicionarCinema([FromBody] CreateCinemaDto cinemaDto)
    {
        Cinema cinema = _mapper.Map<Cinema>(cinemaDto);
        _context.Cinemas.Add(cinema);
        _context.SaveChanges();
        return CreatedAtAction(nameof(RecuperarCinemaPorId), new { Id = cinema.Id }, cinema);
    }

    [HttpGet]
    public IEnumerable<ReadCinemaDto> RecuperarCinemas([FromQuery] int? enderecoId = null)
    {
        if (enderecoId == null)
        {
            return _mapper.Map<List<ReadCinemaDto>>(_context.Cinemas.ToList());
        }

        return _mapper.Map<List<ReadCinemaDto>>(
            //_context.Cinemas.FromSqlRaw(
            //    $"SELECT Id, Nome, EnderecoId FROM cinemas WHERE cinemas.EnderecoId = {enderecoId}"
            //).ToList()

            _context.Cinemas.Where(cinema => cinema.EnderecoId == enderecoId).ToList()
        );
    }

    [HttpGet("{id}")]
    public IActionResult RecuperarCinemaPorId(int id)
    {
        var cinenma = _context.Cinemas.FirstOrDefault(cinema => cinema.Id == id);
        if (cinenma == null) return NotFound();
        ReadCinemaDto cinemaDto = _mapper.Map<ReadCinemaDto>(cinenma);
        return Ok(cinemaDto);
    }

    [HttpPut("{id}")]
    public IActionResult AtualizarCinema(int id, [FromBody] UpdateCinemaDto cinemaDto)
    {
        var cinema = _context.Cinemas.FirstOrDefault(cinema => cinema.Id == id);
        if (cinema == null) return NotFound();
        _mapper.Map(cinemaDto, cinema);
        _context.SaveChanges();
        return NoContent();
    }

    [HttpPatch("{id}")]
    public IActionResult AtualizarCinemaParcial(int id, [FromBody] JsonPatchDocument<UpdateCinemaDto> patch)
    {
        var cinema = _context.Cinemas.FirstOrDefault(cinema => cinema.Id == id);
        if (cinema == null) return NotFound();

        UpdateCinemaDto cinemaParaAtualizar = _mapper.Map<UpdateCinemaDto>(cinema);
        patch.ApplyTo(cinemaParaAtualizar, ModelState);

        if (!TryValidateModel(cinemaParaAtualizar)) return ValidationProblem(ModelState);

        _mapper.Map(cinemaParaAtualizar, cinema);
        _context.SaveChanges();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public IActionResult DeletarCinema(int id, [FromBody] JsonPatchDocument<UpdateCinemaDto> patch)
    {
        var cinema = _context.Cinemas.FirstOrDefault(cinema => cinema.Id == id);
        if (cinema == null) return NotFound();
        _context.Remove(cinema);
        _context.SaveChanges();
        return NoContent();
    }
}
