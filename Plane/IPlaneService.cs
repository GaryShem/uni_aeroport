using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace Plane
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IPlaneService" in both code and config file together.
    [ServiceContract]
    public interface IPlaneService
    {
        [OperationContract]
        [WebInvoke(Method = "GET",
             ResponseFormat = WebMessageFormat.Json,
             BodyStyle = WebMessageBodyStyle.Bare,
             UriTemplate = "CompleteMove?id={id}&zone={zone}")]
        void CompleteMove(string id, int zone);
        [OperationContract]
        [WebInvoke(Method = "GET",
             ResponseFormat = WebMessageFormat.Json,
             BodyStyle = WebMessageBodyStyle.Bare,
             UriTemplate = "GetAllPlanes")]
        string GetAllPlanes();
        

        [OperationContract]
        [WebInvoke(Method = "GET",
             ResponseFormat = WebMessageFormat.Json,
             BodyStyle = WebMessageBodyStyle.Bare,
             UriTemplate = "GoAway?id={id}")]
        void GoAway(string id);

        [OperationContract]
        [WebInvoke(Method = "GET",
             ResponseFormat = WebMessageFormat.Json,
             BodyStyle = WebMessageBodyStyle.Bare,
             UriTemplate = "TakePassengersFromBus?flightId={id}")]
        void TakePassengersFromBus(string id);

        [OperationContract]
        [WebInvoke(Method = "GET",
             ResponseFormat = WebMessageFormat.Json,
             BodyStyle = WebMessageBodyStyle.Bare,
             UriTemplate = "UnloadPassengers?flightId={flightId}&count={count}")]
        string UnloadPassengers(string flightId, int count);

        [OperationContract]
        [WebInvoke(Method = "GET",
             ResponseFormat = WebMessageFormat.Json,
             BodyStyle = WebMessageBodyStyle.Bare,
             UriTemplate = "AcceptFuel?flightId={flightId}&count={count}")]
        string AcceptFuel(string flightId, int count);

        [OperationContract]
        [WebInvoke(Method = "GET",
             ResponseFormat = WebMessageFormat.Json,
             BodyStyle = WebMessageBodyStyle.Bare,
             UriTemplate = "Start")]
        void Start();

        [OperationContract]
        [WebInvoke(Method = "GET",
             ResponseFormat = WebMessageFormat.Json,
             BodyStyle = WebMessageBodyStyle.Bare,
             UriTemplate = "TakeCargoFromTruck?flightId={id}&cargo={cargoCount}")]
        void TakeCargoFromTruck(string id, int cargoCount);

        [OperationContract]
        [WebInvoke(Method = "GET",
             ResponseFormat = WebMessageFormat.Json,
             BodyStyle = WebMessageBodyStyle.Bare,
             UriTemplate = "GiveCargoToTruck?flightId={id}&cargo={cargoCount}")]
        string GiveCargoToTruck(string id, int cargoCount);

        [OperationContract]
        [WebInvoke(Method = "GET",
             ResponseFormat = WebMessageFormat.Json,
             BodyStyle = WebMessageBodyStyle.Bare,
             UriTemplate = "GetRemainingCargoToLoad?flightId={flightId}")]
        string GetRemainingCargoToLoad(string flightId);

        [OperationContract]
        [WebInvoke(Method = "GET",
             ResponseFormat = WebMessageFormat.Json,
             BodyStyle = WebMessageBodyStyle.Bare,
             UriTemplate = "GeneratePlane?passengerCount={passengerCount}")]
        void GeneratePlane(int passengerCount);
    }
}