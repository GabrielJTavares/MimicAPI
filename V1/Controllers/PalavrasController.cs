using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MimicWebApi.Helpers;
using MimicWebApi.V1.Models;
using MimicWebApi.V1.Models.DTO;
using MimicWebApi.V1.Repositories.Contracts;
using Newtonsoft.Json;

namespace MimicWebApi.V1.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class PalavrasController : ControllerBase
    {
        private readonly IWordRepository _wordRepository;
        private readonly IMapper _mapper;
        public PalavrasController(IWordRepository wordRepository, IMapper mapper)
        {
            _wordRepository = wordRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Operação que retorna todas as palavras existentes
        /// </summary>
        /// <param name="query">Filtros de Pequisa</param>
        /// <returns>List palavras</returns>

        [MapToApiVersion("1.0")]
        [HttpGet(Name = "FindAllWords")]
        public ActionResult FindAllWords([FromQuery]PalavraUrlQuery query)
        {
            var item = _wordRepository.FindAllWords(query);

            if (item.Results.Count == 0)
            {
                return NotFound();
            }

            PaginationList<PalavraDTO> list = CriarLinks(query, item);

            return Ok(list);

        }
        
        private PaginationList<PalavraDTO> CriarLinks(PalavraUrlQuery query, PaginationList<Palavra> item)
        {
            var list = _mapper.Map<PaginationList<Palavra>, PaginationList<PalavraDTO>>(item);

            foreach (var palavra in list.Results)
            {

                palavra.Links.Add(new LinkDTO("self", Url.Link("FindByIdWord", new { id = palavra.Id }), "GET"));
            }
            list.Links.Add(new LinkDTO("self", Url.Link("FindAllWords", query), "GET"));


            if (item.Paginacao != null)
            {
                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(item.Paginacao));

                if (query.PagNumero + 1 <= item.Paginacao.TotalPaginas)
                {
                    var queryString = new PalavraUrlQuery() { PagNumero = query.PagNumero + 1, PagRegistro = query.PagRegistro, Date = query.Date };
                    list.Links.Add(new LinkDTO("next", Url.Link("FindAllWords", queryString), "GET"));
                }
                if (query.PagNumero - 1 > 0)
                {
                    var queryString = new PalavraUrlQuery() { PagNumero = query.PagNumero - 1, PagRegistro = query.PagRegistro, Date = query.Date };
                    list.Links.Add(new LinkDTO("prev", Url.Link("FindAllWords", queryString), "GET"));
                }
            }

            return list;
        }
        /// <summary>
        /// Encontra uma palavra por seu id
        /// </summary>
        /// <param name="id">Identificação</param>
        /// <returns>Entidade Palavra</returns>
        [MapToApiVersion("1.0")]
        [HttpGet("{id}", Name = "FindByIdWord")]
        public ActionResult FindByIdWord(int id)
        {
            var obj = _wordRepository.FindByIdWord(id);
            if (obj == null)
                return NotFound();


            PalavraDTO palavraDTO = _mapper.Map<Palavra, PalavraDTO>(obj);

            palavraDTO.Links.Add(
                new LinkDTO("self", Url.Link("FindByIdWord", new { id = palavraDTO.Id }), "GET")
                );
            palavraDTO.Links.Add(
                new LinkDTO("update", Url.Link("UpdateWord", new { id = palavraDTO.Id }), "PUT")
                );
            palavraDTO.Links.Add(
                new LinkDTO("delete", Url.Link("DeleteWord", new { id = palavraDTO.Id }), "DELETE")
                );


            return Ok(palavraDTO);

        }
        /// <summary>
        /// Cadatra um obj palavra e sua pontuação
        /// </summary>
        /// <param name="palavra">a palvra que desej</param>
        /// <returns>um objeto palavra com seu id</returns>
        [MapToApiVersion("1.0")]
        [HttpPost("", Name = "Register")]
        public ActionResult Register([FromBody]Palavra palavra)
        {
            if (palavra == null)
                return StatusCode(400);

            if (!ModelState.IsValid)
                return UnprocessableEntity(ModelState);


            palavra.Ativo = true;
            palavra.Criado = DateTime.Now;
            _wordRepository.RegisterWord(palavra);

            var palavraDTO = _mapper.Map<Palavra, PalavraDTO>(palavra);


            palavraDTO.Links.Add(
                new LinkDTO("self", Url.Link("Register", new { id = palavraDTO.Id }), "POST")
                );

            return Created($"/api/palavras/{palavraDTO.Id}", palavraDTO);
        }
        /// <summary>
        /// Atualiza as informações da uma palabra
        /// </summary>
        /// <param name="id"> identidicador</param>
        /// <param name="palavra"> item a ser alterado</param>
        /// <returns></returns>
        [MapToApiVersion("1.0")]
        [HttpPut("{id}", Name = "UpdateWord")]
        public ActionResult UpdateWord(int id, [FromBody]Palavra palavra)
        {
            var obj = _wordRepository.FindByIdWord(id);


            if (obj == null)
                return NotFound();

            if (palavra == null)
                return BadRequest();

            if (!ModelState.IsValid)
                return UnprocessableEntity(ModelState);




            palavra.Id = id;
            palavra.Ativo = obj.Ativo;
            palavra.Criado = obj.Criado;
            palavra.Atualizado = DateTime.Now;


            _wordRepository.UpdateWord(palavra);

            var palavraDTO = _mapper.Map<Palavra, PalavraDTO>(palavra);

            palavraDTO.Links.Add(
                new LinkDTO("self", Url.Link("UpdateWord", new { id = palavraDTO.Id }), "PUT")
                );
            return Ok();

        }
        /// <summary>
        /// Operação que desativa uma palavra
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [MapToApiVersion("1.0")]
        [HttpDelete("{id}", Name = "DeleteWord")]
        public ActionResult DeleteWord(int id)
        {
            var palavra = _wordRepository.FindByIdWord(id);
            if (palavra == null)
            {
                return NotFound();
            }
            else
            {
                _wordRepository.DeleteWord(id);

                return NoContent();
            }
        }

    }
}