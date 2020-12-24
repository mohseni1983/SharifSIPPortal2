﻿using Newtonsoft.Json;
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
    public class MadadjouController : ApiController
    {
        /// <summery>
        ///بانک اطلاعاتی مربوط به اطلاعات شریف
        ///</summery>
        BpmsSharifDataEntities SharifDataEntity = new BpmsSharifDataEntities();
        ///بانک اطلاعاتی مربوط به اطلاعات نرم افزار مددکار آنلاین
        MadadkarOnlineEntities SipDataEntity = new MadadkarOnlineEntities();


        /// <summary>
        /// دریافت لیست کودکان
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/v2/Madadjou/GetMadadjouList")]
        public IHttpActionResult GetMadadjouList()
        {
            try
            {
                var result = SharifDataEntity.FG_MadadjusInfo2.Where(a => a.Deleted == false).Select(x => new { x.MadadjuId, x.MadadjuFName, x.MadadjuLName });
                return Ok(result);
            }
            catch (Exception err)
            {
                throw err;
            }
        }
    }
}
