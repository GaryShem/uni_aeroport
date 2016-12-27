using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace Passenger
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IPassengerService" in both code and config file together.
    [ServiceContract]
    public interface IPassengerService
    {

        [OperationContract]
        [WebInvoke(Method = "GET",
             ResponseFormat = WebMessageFormat.Json,
             BodyStyle = WebMessageBodyStyle.Bare,
             UriTemplate = "GetPassengerInfo?id={id}")]
        string GetPassengerInfo(string id);

        [OperationContract]
        [WebInvoke(Method = "GET",
             ResponseFormat = WebMessageFormat.Json,
             BodyStyle = WebMessageBodyStyle.Bare,
             UriTemplate = "GeneratePassengers?id={flightId}&count={passengerCount}")]
        string GeneratePassengers(string flightId, int passengerCount);
    }
}
