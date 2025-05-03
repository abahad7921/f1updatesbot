using F1UpdatesBot.Src.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F1UpdatesBot.Src.Services.Interfaces
{   
    /// <summary>
    /// The Openf1service interface.
    /// </summary>
    public interface IOpenf1Service
    {
        Task<List<Lap>> GetLapsAsync();
        Task<List<Driver>> GetDriversAsync();
    }
}
