using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace RegistrationStand
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IRegistrationStandService" in both code and config file together.
    [ServiceContract]
    public interface IRegistrationStandService
    {

        [OperationContract]
        [WebInvoke(Method = "GET",
             ResponseFormat = WebMessageFormat.Json,
             BodyStyle = WebMessageBodyStyle.Bare,
             UriTemplate = "OpenRegistration?flightId={flightId}")]
        void OpenRegistration(string flightId);

        [OperationContract]
        [WebInvoke(Method = "GET",
             ResponseFormat = WebMessageFormat.Json,
             BodyStyle = WebMessageBodyStyle.Bare,
             UriTemplate = "Register?flightId={flightId}&passengerId={passengerId}&cargo={cargoCount}")]
        string Register(string flightId, string passengerId, int cargoCount);

        [OperationContract]
        [WebInvoke(Method = "GET",
             ResponseFormat = WebMessageFormat.Json,
             BodyStyle = WebMessageBodyStyle.Bare,
             UriTemplate = "CloseRegistration?flightId={flightId}")]
        void CloseRegistration(string flightId);

        [OperationContract]
        [WebInvoke(Method = "GET",
             ResponseFormat = WebMessageFormat.Json,
             BodyStyle = WebMessageBodyStyle.Bare,
             UriTemplate = "GetPassengers?flightId={flightId}")]
        string GetPassengers(string flightId);

        [OperationContract]
        [WebInvoke(Method = "GET",
             ResponseFormat = WebMessageFormat.Json,
             BodyStyle = WebMessageBodyStyle.Bare,
             UriTemplate = "GetCargo?flightId={flightId}")]
        string GetCargo(string flightId);

        [OperationContract]
        [WebInvoke(Method = "GET",
             ResponseFormat = WebMessageFormat.Json,
             BodyStyle = WebMessageBodyStyle.Bare,
             UriTemplate = "DeleteList?flightId={flightId}")]
        void DeleteList(string flightId);
    }
}
