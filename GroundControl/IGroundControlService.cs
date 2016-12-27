using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace GroundControl
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IGroundControlService" in both code and config file together.
    [ServiceContract]
    public interface IGroundControlService
    {

        [OperationContract]
        string GetData(int value);

        // TODO: Add your service operations here
    }
}
