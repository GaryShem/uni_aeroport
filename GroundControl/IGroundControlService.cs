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
        [WebInvoke(Method = "GET",
             ResponseFormat = WebMessageFormat.Json,
             BodyStyle = WebMessageBodyStyle.Bare,
             UriTemplate = "StartService?id={id}&zone={zoneNum}")]
        void StartService(string id, int zoneNum);

        [OperationContract]
        [WebInvoke(Method = "GET",
             ResponseFormat = WebMessageFormat.Json,
             BodyStyle = WebMessageBodyStyle.Bare,
             UriTemplate = "FinishUnloadingPassengers?id={id}&zone={zoneNum}")]
        void FinishUnloadingPassengers(string id, int zoneNum);

        [OperationContract]
        [WebInvoke(Method = "GET",
             ResponseFormat = WebMessageFormat.Json,
             BodyStyle = WebMessageBodyStyle.Bare,
             UriTemplate = "FinishUnloadingCargo?id={id}&zone={zoneNum}")]
        void FinishUnloadingCargo(string id, int zoneNum);

        [OperationContract]
        [WebInvoke(Method = "GET",
             ResponseFormat = WebMessageFormat.Json,
             BodyStyle = WebMessageBodyStyle.Bare,
             UriTemplate = "FinishRefueling?id={id}&zone={zoneNum}")]
        void FinishRefueling(string id, int zoneNum);

        [OperationContract]
        [WebInvoke(Method = "GET",
             ResponseFormat = WebMessageFormat.Json,
             BodyStyle = WebMessageBodyStyle.Bare,
             UriTemplate = "FinishLoadingCargo?id={id}&zone={zoneNum}")]
        void FinishLoadingCargo(string id, int zoneNum);

        [OperationContract]
        [WebInvoke(Method = "GET",
             ResponseFormat = WebMessageFormat.Json,
             BodyStyle = WebMessageBodyStyle.Bare,
             UriTemplate = "FinishLoadingPassengers?id={id}&zone={zoneNum}")]
        void FinishLoadingPassengers(string id, int zoneNum);

        [OperationContract]
        [WebInvoke(Method = "GET",
             ResponseFormat = WebMessageFormat.Json,
             BodyStyle = WebMessageBodyStyle.Bare,
             UriTemplate = "CheckStage?id={id}")]
        string CheckStage(string id);
    }
}
