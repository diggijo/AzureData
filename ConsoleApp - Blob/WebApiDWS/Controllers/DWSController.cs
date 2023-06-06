using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using static WebApiDWS.Controllers.DWSController;
using static Opc2.Program;

namespace WebApiDWS.Controllers
{
    public class DWSController : ApiController
    {
        string Result = "";
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [System.Web.Http.HttpGet]
        [System.Web.Http.HttpPost]
       
        public HttpResponseMessage OpcData(CraneData craneData)
        {
            Result = "HoistValue: " + craneData.WindV + " WindValue: " + craneData.TimeV + "ReturnedToYou...";
            return Request.CreateResponse(HttpStatusCode.OK, Result);
        }

        //public IHttpActionResult GetAllData()
        //{
        //    return (IHttpActionResult)Request.CreateResponse(HttpStatusCode.OK, value: Result);
            
        //}
        //using static Opc2.Program;
        public class CraneData
        {
            public int WindV { get; set; }
            public string TimeV { get; set; }
        }
    }
}
