using MimicWebApi.Helpers;
using MimicWebApi.V1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MimicWebApi.V1.Repositories.Contracts
{
    public interface IWordRepository
    {
        PaginationList<Palavra> FindAllWords(PalavraUrlQuery query);
        Palavra FindByIdWord(int id);
        void RegisterWord(Palavra palavra);
        void UpdateWord(Palavra palavra);
        void DeleteWord(int id);

    }
}
