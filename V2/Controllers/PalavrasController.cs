using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace MimicWebApi.V2.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]   
    [ApiVersion("2.0")]
    public class PalavrasController : ControllerBase
    {
        /// <summary>
        /// Operação que retorna todas as palavras existentes
        /// </summary>
        /// <param>Filtros de Pequisa</param>
        /// <returns>List palavras</returns>
        [MapToApiVersion("2.0")]
        [HttpGet(Name = "FindAllWords")]
        public string FindAllWords()
        {
            return "Versão 2.0";
        }
    }
}