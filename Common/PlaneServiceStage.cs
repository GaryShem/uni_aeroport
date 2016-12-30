using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public enum PlaneServiceStage
    {
        UNLOAD_PASSENGERS,
        UNLOAD_CARGO,
        REFUEL,
        LOAD_CARGO,
        LOAD_PASSENGERS,
        TAKEOFF,
        NOT_IN_SERVICE
    }
}
