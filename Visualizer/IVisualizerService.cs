using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using Common;

namespace Visualizer
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IVisualizerService" in both code and config file together.
    [ServiceContract]
    public interface IVisualizerService
    {

        [OperationContract]
        [WebInvoke(Method = "GET",
             ResponseFormat = WebMessageFormat.Json,
             BodyStyle = WebMessageBodyStyle.Bare,
             UriTemplate = "Spawn?type={entityNum}&id={id}&zone={zoneNum}&cargo={cargoCount}")]
        void Spawn(int entityNum, string id, int zoneNum, int cargoCount);

        [OperationContract]
        [WebInvoke(Method = "GET",
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Bare,
            UriTemplate = "SpawnPlane?type={entityNum}&id={id}&zone={zoneNum}&cargo={cargoCount}&passengerCount={passengerCount}&fuelCount={fuelCount}")]
        void SpawnPlane(int entityNum, string id, int zoneNum, int cargoCount, int passengerCount, int fuelCount);

        [OperationContract]
        [WebInvoke(Method = "GET",
             ResponseFormat = WebMessageFormat.Json,
             BodyStyle = WebMessageBodyStyle.Bare,
             UriTemplate = "Despawn?id={id}")]
        void Despawn(string id);

        [OperationContract]
        [WebInvoke(Method = "GET",
             ResponseFormat = WebMessageFormat.Json,
             BodyStyle = WebMessageBodyStyle.Bare,
             UriTemplate = "Move?type={entityNum}&id={id}&zone={zoneNum}")]
        void Move(int entityNum, string id, int zoneNum);

        [OperationContract]
        [WebInvoke(Method = "GET",
             ResponseFormat = WebMessageFormat.Json,
             BodyStyle = WebMessageBodyStyle.Bare,
             UriTemplate = "GetAllVehicles")]
        string GetAllVehicles();

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
             UriTemplate = "GetAllPassengers")]
        string GetAllPassengers();

        [OperationContract]
        [WebInvoke(Method = "GET",
             ResponseFormat = WebMessageFormat.Json,
             BodyStyle = WebMessageBodyStyle.Bare,
             UriTemplate = "UpdateCargo?id={id}&cargoCount={cargoCount}")]
        void UpdateCargo(string id, int cargoCount);

        [OperationContract]
        [WebInvoke(Method = "GET",
             ResponseFormat = WebMessageFormat.Json,
             BodyStyle = WebMessageBodyStyle.Bare,
             UriTemplate = "UpdatePlane?id={id}&passengerCount={passengerCount}&cargoCount={cargoCount}&fuelCount={fuelCount}")]
        void UpdatePlane(string id, int passengerCount, int cargoCount, int fuelCount);
    }
}
