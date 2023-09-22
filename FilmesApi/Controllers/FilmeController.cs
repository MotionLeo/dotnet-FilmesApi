using AutoMapper;
using FilmesApi.Data;
using FilmesApi.Data.DTOs;
using FilmesApi.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace FilmesApi.Controllers;

[ApiController]
[Route("[controller]")]
public class FilmeController : ControllerBase
{
    private FilmeContext _context;
    private IMapper _mapper;

    public FilmeController(FilmeContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    /// <summary>
    /// Adiciona um filme ao banco de dados
    /// </summary>
    /// <param name="filmeDto">Objeto com os campos necessários para criação de um filme</param>
    /// <returns>IActionResult</returns>
    /// <response code="201">Caso inserção seja feita com sucesso</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public IActionResult AdicionaFilme([FromBody] CreateFilmeDto filmeDto)
    {
        Filme filme = _mapper.Map<Filme>(filmeDto);
        _context.Filmes.Add(filme);
        _context.SaveChanges();
        return CreatedAtAction(nameof(LeFilmePorId), new { id = filme.Id }, filme );
    }


    /// <summary>
    /// Busca todos os filmes no banco de dados com uma páginação de 50
    /// </summary>
    /// <param name="skip">Início de onde deverá partir a páginação</param>
    /// <param name="take">Quantos itens a página irá exibir</param>
    /// <returns>IEnumerable de filmes</returns>
    [HttpGet]
    public IEnumerable<ReadFilmeDto> LeFilmes([FromQuery] int skip = 0, [FromQuery] int take = 50)
    {
        return _mapper.Map<List<ReadFilmeDto>>(_context.Filmes.Skip(skip).Take(take));
    }

    /// <summary>
    /// Busca apenas um filme através do seu ID
    /// </summary>
    /// <param name="id">ID do filme que será buscado na lista</param>
    /// <returns>IActionResult</returns>
    /// <response code="200">Caso seja encontrado um filme com o ID passado via parâmetro</response>
    [HttpGet("{id}")]
    public IActionResult LeFilmePorId(int id)
    {
        var filme = _context.Filmes.FirstOrDefault(filme => filme.Id == id);
        if (filme == null) return NotFound();
        var filmeDto = _mapper.Map<ReadFilmeDto>(filme);
        return Ok(filmeDto);
    }

    /// <summary>
    /// Atualiza um filme encontrado pelo ID através do corpo passado
    /// </summary>
    /// <param name="id">ID do filme que será buscado para atualização</param>
    /// <param name="filmeDto">Corpo da requisição</param>
    /// <returns>IActionResult</returns>
    /// <response code="204">Quando a atualização do filme for feita com sucesso</response>
    [HttpPut("{id}")]
    public IActionResult AtualizaFilme(int id, [FromBody] UpdateFilmeDto filmeDto)
    {
        var filme = _context.Filmes.FirstOrDefault(filme => filme.Id == id);
        if(filme == null) return NotFound();
        _mapper.Map(filmeDto, filme);
        _context.SaveChanges();
        return NoContent();
    }

    /// <summary>
    /// Atualiza um filme de forma parcial, não precisando digitar toda sua estrutura
    /// </summary>
    /// <param name="id">ID do filme que será buscado para atualização</param>
    /// <param name="patch">JSON que será passado demonstrando quais campos serão atualizados através do patch</param>
    /// <returns>IActionResult</returns>
    /// <response code="204">Quando a atualização do filme for feita com sucesso</response>
    [HttpPatch("{id}")]
    public IActionResult AtualizaFilmeParcial(int id, JsonPatchDocument<UpdateFilmeDto> patch)
    {
        var filme = _context.Filmes.FirstOrDefault(filme => filme.Id == id);
        if (filme == null) return NotFound();
        var filmeParaAtualizar = _mapper.Map<UpdateFilmeDto>(filme);

        patch.ApplyTo(filmeParaAtualizar, ModelState);
        if(!TryValidateModel(filmeParaAtualizar)) return ValidationProblem(ModelState);

        _mapper.Map(filmeParaAtualizar, filme);
        _context.SaveChanges();
        return NoContent();
    }

    /// <summary>
    /// Faz a deleção de um filme do banco de dados através do seu ID
    /// </summary>
    /// <param name="id">ID do filme que será deletado</param>
    /// <returns>IActionResult</returns>
    /// <response code="204">Quando a deleção do filme for feita com sucesso</response>
    [HttpDelete("{id}")]
    public IActionResult DeletaFilme(int id)
    {
        var filme = _context.Filmes.FirstOrDefault(filme => filme.Id == id);
        if(filme == null) return NotFound();
        _context.Remove(filme);
        _context.SaveChanges();
        return NoContent();
    }
}
