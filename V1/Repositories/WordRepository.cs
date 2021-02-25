using MimicWebApi.DataBase;
using MimicWebApi.Helpers;
using MimicWebApi.V1.Models;
using MimicWebApi.V1.Repositories.Contracts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MimicWebApi.V1.Repositories
{
    public class WordRepository : IWordRepository
    {
        private readonly ApiContext _banco;
        public WordRepository(ApiContext banco)
        {
            _banco = banco;
        }

        public PaginationList<Palavra> FindAllWords(PalavraUrlQuery query)
        {
            var lista = new PaginationList<Palavra>();
            var item = _banco.Palavras.AsQueryable();
            if (query.Date.HasValue)
            {
                item = item.Where(a => a.Criado > query.Date.Value || a.Atualizado > query.Date.Value);

            }
            if (query.PagNumero.HasValue)
            {
                var quantidadeTotalRegistros = item.Count();
                item = item.Skip((query.PagNumero.Value - 1) * query.PagRegistro.Value).Take(query.PagRegistro.Value);
                var paginacao = new Paginacao();
                paginacao.NumeroPagina = query.PagNumero.Value;
                paginacao.RegistroPorPagina = query.PagRegistro.Value;
                paginacao.TotalRegistros = quantidadeTotalRegistros;
                paginacao.TotalPaginas = (int)Math.Ceiling((double)quantidadeTotalRegistros / query.PagRegistro.Value);

                lista.Paginacao = paginacao;
               
            }
            lista.Results.AddRange(item.ToList());

            return lista;
            
        }

        public Palavra FindByIdWord(int id)
        {
            return _banco.Palavras.Find(id);
        }

        public void RegisterWord(Palavra palavra)
        {
             _banco.Palavras.Add(palavra);
            _banco.SaveChanges();
        }

        public void UpdateWord(Palavra palavra)
        {
            _banco.Palavras.Update(palavra);
            _banco.SaveChanges();
        }
        public void DeleteWord(int id)
        {
            Palavra palavra =FindByIdWord(id);
            palavra.Ativo = false;
            _banco.Palavras.Update(palavra);
            _banco.SaveChanges();
        }
    }
}
