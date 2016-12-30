using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace Bus
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IBusService" in both code and config file together.
    [ServiceContract]
    public interface IBusService
    {
        [OperationContract]
        [WebInvoke(Method = "GET",
             ResponseFormat = WebMessageFormat.Json,
             BodyStyle = WebMessageBodyStyle.Wrapped,
             UriTemplate = "UnloadPassengers")]
        string UnloadPassengers();

        [OperationContract]
        [WebInvoke(Method = "GET",
             ResponseFormat = WebMessageFormat.Json,
             BodyStyle = WebMessageBodyStyle.Wrapped,
             UriTemplate = "AddAction?flightId={flightId}&zone={zoneNum}&action={actionNum}")]
        void AddAction(string flightId, int zoneNum, int actionNum);
    }
}
