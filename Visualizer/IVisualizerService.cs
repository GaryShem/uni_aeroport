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
             UriTemplate = "Spawn?type={entity}&id={id}&zone={zone}&cargo={cargoCount}")]
        void Spawn(Entity entity, string id, Zone zone, int cargoCount);

        [OperationContract]
        [WebInvoke(Method = "GET",
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Bare,
            UriTemplate = "Spawn?type={entity}&id={id}&zone={zone}&cargo={cargoCount}&passengerCount{passengerCount}")]
        void SpawnPlane(Entity entity, string id, Zone zone, int cargoCount, int passengerCount);

        [OperationContract]
        [WebInvoke(Method = "GET",
             ResponseFormat = WebMessageFormat.Json,
             BodyStyle = WebMessageBodyStyle.Bare,
             UriTemplate = "Despawn?id={id}&zone={zone}")]
        void Despawn(string id, Zone zone);

        [OperationContract]
        [WebInvoke(Method = "GET",
             ResponseFormat = WebMessageFormat.Json,
             BodyStyle = WebMessageBodyStyle.Bare,
             UriTemplate = "Move?id={id}&zone={zone}")]
        void Move(string id, Zone zone);
    }
}
