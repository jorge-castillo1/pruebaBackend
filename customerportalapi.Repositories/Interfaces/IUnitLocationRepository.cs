using customerportalapi.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace customerportalapi.Repositories.Interfaces
{
    public interface IUnitLocationRepository
    {
        UnitLocation GetBySizeCode(string sizeCode);
        UnitLocation Update(UnitLocation sizeCode);
        Task<bool> Create(UnitLocation sizeCode);
        Task<bool> Delete(UnitLocation sizeCode);
        List<UnitLocation> Find(UnitLocationSearchFilter filter);

    }
}
