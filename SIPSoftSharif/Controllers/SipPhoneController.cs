using Newtonsoft.Json;
using SIPSoftSharif.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Migrations;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web;
using System.Web.Http;
using System.Web.Routing;

namespace SIPSoftSharif.Controllers
{
    public class SipPhoneController : ApiController
    {
        /// <summery>
        ///بانک اطلاعاتی مربوط به اطلاعات شریف
        ///</summery>
        BpmsSharifDataEntities SharifDataEntity = new BpmsSharifDataEntities();
        ///بانک اطلاعاتی مربوط به اطلاعات نرم افزار مددکار آنلاین
        MadadkarOnlineEntities SipDataEntity = new MadadkarOnlineEntities();

        /// <summary>
        /// ثبت نتیجه تماس مددکار
        /// </summary>
        /// <param name="call"></param>
        /// <returns></returns>
        
        [HttpPost]
        [Authorize(Roles = "Madadkar")]
        [Route("api/v2/SipPhone/AddCallForResults")]
        public IHttpActionResult setCallResults(callStatusInput2 call)

        {
            var identity = (ClaimsIdentity)User.Identity;

            var hami = SharifDataEntity.FG_Hamis.Where(x => x.HamiId == call.HamiId).FirstOrDefault();
            var MadadkarId = identity.Claims.Where(s => s.Type == "MadadkarId").FirstOrDefault();
            var MadadkarName = identity.Name;
            try
            {
                var addresult = SipDataEntity.CallStatus.Add(new CallStatus()
                {
                    Date = call.dateof,
                    Description = call.Description,
                    StartTime = call.StartTime,
                    EndTime = call.EndTime,
                    HamiId = call.HamiId,
                    HamiName = hami.HamiFName + ' ' + hami.HamiLName,
                    MaadadkarName = MadadkarName,
                    MadadkarId = int.Parse(MadadkarId.Value),
                    PhoneNumber = call.MobileNum,
                    Status = call.status
                });
                SipDataEntity.SaveChanges();
                return Ok("9001");

            }
            catch (Exception er)
            {
                throw;
            }
            //return NotFound()

        }
        public class callStatusInput2
        {
            public int HamiId { get; set; }
            public string dateof { get; set; }
            public string StartTime { get; set; }
            public string EndTime { get; set; }
            public string Description { get; set; }
            public string MobileNum { get; set; }
            public int status { get; set; }
        }

    }
}
