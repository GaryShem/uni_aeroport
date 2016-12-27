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
             BodyStyle = WebMessageBodyStyle.Wrapped,
             UriTemplate = "GeneratePlane?passengerCount={passengerCount}&fuelCount={fuelCount}")]
        void GeneratePlane(int passengerCount, int fuelCount);

        [OperationContract]
        [WebInvoke(Method = "GET",
             ResponseFormat = WebMessageFormat.Json,
             BodyStyle = WebMessageBodyStyle.Wrapped,
             UriTemplate = "CompleteMove?id={id}&zone={zone}")]
        void CompleteMove(string id, int zone);
        [OperationContract]
        [WebInvoke(Method = "GET",
             ResponseFormat = WebMessageFormat.Json,
             BodyStyle = WebMessageBodyStyle.Wrapped,
             UriTemplate = "GetAllPlanes")]
        string GetAllPlanes();

        [OperationContract]
        [WebInvoke(Method = "GET",
             ResponseFormat = WebMessageFormat.Json,
             BodyStyle = WebMessageBodyStyle.Wrapped,
             UriTemplate = "LoadPassengers?id={id}")]
        void LoadPassengers(string id);
    }
    
}
