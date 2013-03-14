using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FLBS14.Data
{
    public interface ILoadable<DataT>
    {
        void Load(DataT baseData);
        ILoadable(DataT baseData);
    }
}
