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
    public class HamiController : ApiController
    {

        /// <summery>
        ///بانک اطلاعاتی مربوط به اطلاعات شریف
        ///</summery>
        BpmsSharifDataEntities SharifDataEntity = new BpmsSharifDataEntities();
        ///بانک اطلاعاتی مربوط به اطلاعات نرم افزار مددکار آنلاین
        MadadkarOnlineEntities SipDataEntity = new MadadkarOnlineEntities();

        /// <summary>
        /// دریافت لیست حامیان 
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Madadkar")]
        [HttpPost]
        [Route("api/v2/hami/GetHamisForEdit")]
        public IHttpActionResult GetHamisForEdit()
        {
            var identity = (ClaimsIdentity)User.Identity;
            var MadadkarId = identity.Claims.Where(s => s.Type == "MadadkarId").FirstOrDefault();
            int MadadkarID = int.Parse(MadadkarId.Value);
            //int MadadkarID = 230;
            var result = SharifDataEntity.FG_HamiMadadkarsInfo.Where(x => x.MadadkarId == MadadkarID && x.Deleted != true && x.HamiMobile1 != null).OrderBy(r => r.HamiLName);
            foreach (var item in result)
            {
                var search = SipDataEntity.HamiEditSet.FirstOrDefault(x => x.HamiId == item.HamiId);
                if (search == null)
                {
                    HamiEditSet hami = new HamiEditSet();
                    hami.HamiId = (int)item.HamiId;
                    hami.HamiFname = item.HamiFName;
                    hami.HamiLname = item.HamiLName;
                    hami.OldMobile1 = item.HamiMobile1;
                    hami.OldMobile2 = item.HamiMobile2 ?? "";
                    hami.MadadkarId = MadadkarID;
                    hami.MadadkarName = identity.Name;
                    SipDataEntity.HamiEditSet.AddOrUpdate(hami);
                    SipDataEntity.SaveChanges();
                }
            }
            var fresult = SipDataEntity.HamiEditSet.Where(x => x.MadadkarId == MadadkarID);
            var res = SipDataEntity.HamiEditSet.Where(x => x.MadadkarId == MadadkarID && x.Deleted != true).Select(d => new
            {
                d.Id,
                d.HamiId,
                d.HamiFname,
                d.HamiLname,
                d.MadadkarId,
                d.MadadkarName,
                d.NationalCode,
                d.NewHamiFname,
                d.NewHamiLname,
                d.NewMobile1,
                d.NewMobile2,
                d.NewPhone1,
                d.NewPhone2,
                d.OldMobile1,
                d.OldMobile2,
                d.OldPhone1,
                d.OldPhone2,
                d.TempSave,
                d.DeleteOldMobile1,
                d.DeleteOldMobile2,
                d.DeleteOldPhone1,
                d.DeleteOldPhone2,
                d.EditDate,
                d.Email,
                d.FinalSave,
                HamiMadadjouSet = SipDataEntity.HamiMadadjouSet.Where(f => f.Deleted != true && f.HamiId == d.HamiId).Select(
                      t => new { t.HamiId, t.Id, t.MadadjouFname, t.MadadjouLname, t.MadadjouId, t.Deleted }
                      ).ToList()

            }).ToList();
            return Ok(res);
        }

        /// <summary>
        /// حذف حامی
        /// </summary>
        /// <param name="deleteHami"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/v2/hami/DeleteHamiById")]
        public IHttpActionResult DeleteHamiById(deleteHamicls deleteHami)
        {
            var result = SipDataEntity.HamiEditSet.FirstOrDefault(x => x.HamiId == deleteHami.HamiId);
            try
            {
                result.Deleted = true;
                result.DeleteCuase = deleteHami.deleteCuase;
                SipDataEntity.SaveChanges();
                return Ok("Deleted " + deleteHami.HamiId);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public class deleteHamicls
        {
            public int HamiId { get; set; }
            public string deleteCuase { get; set; }
        }

        /// <summary>
        /// افزودن کودک برای حامی
        /// </summary>
        /// <param name="hamiMadadjou"></param>
        /// <returns></returns>
        [Route("api/v2/hami/AddMadadjouToHami")]
        [HttpPost]
        public IHttpActionResult AddMadadjouToHami(HamiMadadjouSet hamiMadadjou)
        {
            try
            {

                SipDataEntity.HamiMadadjouSet.AddOrUpdate(hamiMadadjou);
                SipDataEntity.SaveChanges();
                return Ok("Saved");
            }
            catch (Exception err)
            {
                throw err;
            }
        }

        /// <summary>
        /// حذف کودک برای حامی
        /// </summary>
        /// <param name="hamiMadadjou"></param>
        /// <returns></returns>
        [Route("api/v2/hami/RemoveMadadjouToHami")]
        [HttpPost]
        public IHttpActionResult removeMadadjouToHami(HamiMadadjouSet hamiMadadjou)
        {
            //var hamiResult = SipDataEntity.HamiEditSet.FirstOrDefault(x=>x.HamiId==hamiMadadjou.HamiId).HamiMadadjouSet.FirstOrDefault(r=>r.Id==hamiMadadjou.Id);
            //var madadjouResult = hamiResult.HamiMadadjouSet.FirstOrDefault(x => x.MadadjouId == hamiMadadjou.MadadjouId);
            try
            {
                var result = SipDataEntity.HamiMadadjouSet.FirstOrDefault(x => x.Id == hamiMadadjou.Id);
                result.Deleted = true;
                SipDataEntity.SaveChanges();
                return Ok("deleted");

            }
            catch (Exception er)
            {
                throw er;
            }
        }



    }
}
