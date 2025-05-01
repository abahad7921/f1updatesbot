using F1UpdatesBot.Src.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F1UpdatesBot.Src.Services.Interfaces
{   
    public interface ISessionsService
    {
        Task<List<Session>> getAll();
        Task<int> getCurrentSessionKey();
    }
}

