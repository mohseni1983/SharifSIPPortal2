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
    public class ReserveController : ApiController
    {
        /// <summery>
        ///بانک اطلاعاتی مربوط به اطلاعات شریف
        ///</summery>
        BpmsSharifDataEntities SharifDataEntity = new BpmsSharifDataEntities();
        ///بانک اطلاعاتی مربوط به اطلاعات نرم افزار مددکار آنلاین
        MadadkarOnlineEntities SipDataEntity = new MadadkarOnlineEntities();

        /// <summary>
        /// افزودن شیفت کاری برای ادمین
        /// </summary>
        /// <param name="job"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/v2/Shifts/Admin/AddSchedule")]
        public IHttpActionResult AddJobSchedule(JobSchedule job)
        {
            if (job.id != 0)
            {
                var exist = SipDataEntity.JobSchedule.Where(x => x.id == job.id).FirstOrDefault();
                if (exist != null)
                {
                    foreach (JobShift item in job.JobShift)
                    {
                        foreach (ShiftPersons it in item.ShiftPersons)
                        {
                            SipDataEntity.ShiftPersons.AddOrUpdate(it);
                        }
                        SipDataEntity.JobShift.AddOrUpdate(item);
                    }
                    SipDataEntity.JobSchedule.AddOrUpdate(job);
                }
            }
            else
            {
                SipDataEntity.JobSchedule.Add(job);
            }
            SipDataEntity.SaveChanges();
            return Ok("Saved");
        }

        /// <summary>
        /// ثبت ساعت ورود مددکار برای ادمین
        /// </summary>
        /// <param name="ShiftPersonId"></param>
        /// <param name="JobShiftId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/v2/Shifts/Admin/AddEntranceTime")]
        public IHttpActionResult addEntrance(int ShiftPersonId, int JobShiftId)
        {
            var shift = SipDataEntity.JobShift.Where(x => x.id == JobShiftId).FirstOrDefault();
            var Result = SipDataEntity.ShiftPersons.Where(x => x.JobShift.id == JobShiftId && x.id == ShiftPersonId).FirstOrDefault();
            if (DateTime.Now.TimeOfDay > shift.ShiftStartTime)
                Result.EnterTime = DateTime.Now.TimeOfDay;
            else
                Result.EnterTime = shift.ShiftStartTime;
            SipDataEntity.SaveChanges();
            return Ok();

        }

        /// <summary>
        /// ثبت خروج مددکار برای ادمین
        /// </summary>
        /// <param name="ShiftPersonId"></param>
        /// <param name="JobShiftId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/v2/Shifts/Admin/AddExitTime")]
        public IHttpActionResult addExit(int ShiftPersonId, int JobShiftId)
        {

            var shift = SipDataEntity.JobShift.Where(x => x.id == JobShiftId).FirstOrDefault();
            var Result = SipDataEntity.ShiftPersons.Where(x => x.JobShift.id == JobShiftId && x.id == ShiftPersonId).FirstOrDefault();
            if (DateTime.Now.TimeOfDay < shift.ShiftEndTime)
                Result.ExitTime = DateTime.Now.TimeOfDay;
            else
                Result.ExitTime = shift.ShiftEndTime;
            SipDataEntity.SaveChanges();
            return Ok();

        }

        /// <summary>
        /// دریافت اطلاعات یک شیفت برای ادمین
        /// </summary>
        /// <param name="ShiftId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/v2/Shifts/Admin/GetShiftById")]
        public IHttpActionResult GetShiftById(int ShiftId)
        {
            JobShift result = SipDataEntity.JobShift.Where(x => x.id == ShiftId).FirstOrDefault();
            //var result2 = result.ShiftPersons;
            var s = result.id;
            if (result != null)
                return Ok(result);
            return NotFound();
        }

        /// <summary>
        /// دریافت شیفت کاری یک روز خاص برای ادمین و مددکار
        /// </summary>
        /// <param name="now"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/v2/Shifts/AdminMadadkar/GetTodyJobSchedule")]
        public IHttpActionResult GetTodayJobSchedule(string now = null, int duration = 0)
        {
            DateTime nowdate = new DateTime();
            if (now != null)
                nowdate = DateTime.ParseExact(now, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            else
            {
                nowdate = DateTime.Now;
                string strDate = nowdate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                nowdate = DateTime.ParseExact(strDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            }
            if (duration != 0)
            {
                nowdate = nowdate.AddDays(duration);
            }

            var result = SipDataEntity.JobSchedule.Where(x => x.JobDate == nowdate).FirstOrDefault();
            if (result != null)

                return Ok(result);
            return NotFound();
        }

        /// <summary>
        /// رزرو شیفت کاری برا ی مددکار
        /// </summary>
        /// <param name="shiftid"></param>
        /// <param name="madadkarId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/v2/Shifts/Madadkar/AddShiftForMadadkar")]
        public IHttpActionResult AddShiftForMadadkar(int shiftid, int madadkarId)
        {
            if (DateTime.Now.Hour >= 12 && DateTime.Now.Hour <= 21)
            {
                var result = SipDataEntity.JobShift.Where(x => x.id == shiftid).FirstOrDefault();
                if (result != null)
                {
                    var presult = result.ShiftPersons.Where(x => x.MadadkarId == madadkarId).FirstOrDefault();
                    var cnt = result.ShiftPersons.Count();
                    if (presult == null && cnt < result.ShiftQuantity)
                    {
                        var madadkar = SharifDataEntity.FG_madadkarsInfo.Where(x => x.MadadkarId == madadkarId).Select(s => s.MadadkarName).FirstOrDefault();
                        string madadkarName = madadkar;
                        var finalresult = SipDataEntity.JobShift.Where(x => x.id == shiftid).FirstOrDefault();
                        finalresult.ShiftPersons.Add(new ShiftPersons() { MadadkarId = madadkarId, MadadkarName = madadkarName });
                        SipDataEntity.SaveChanges();
                        return Ok("Shift Added");
                    }
                }
            }
            return NotFound();
        }

        /// <summary>
        /// حذف شیفت کاری برای مددکار
        /// </summary>
        /// <param name="shiftid"></param>
        /// <param name="madadkarId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/v2/Shifts/Madadkar/RemoveShiftForMadadkar")]
        public IHttpActionResult RemoveShiftForMadadkar(int shiftid, int madadkarId)
        {
            if (DateTime.Now.Hour >= 12 && DateTime.Now.Hour <= 21)
            {
                var result = SipDataEntity.JobShift.Where(x => x.id == shiftid).FirstOrDefault();
                if (result != null)
                {
                    var presult = result.ShiftPersons.Where(x => x.MadadkarId == madadkarId).FirstOrDefault();
                    if (presult != null)
                    {
                        //var madadkar = SharifDataEntity.FG_madadkarsInfo.Where(x => x.MadadkarId == madadkarId).FirstOrDefault();
                        //string madadkarName = madadkar.MadadkarName;
                        //var finalresult = SipDataEntity.ShiftPersons.Where(x=>x.MadadkarId==madadkarId).FirstOrDefault();
                        SipDataEntity.ShiftPersons.Remove(presult);
                        SipDataEntity.SaveChanges();
                        return Ok("Shift Removed");
                    }
                }
            }
            return Ok("Peyda nashod");


        }

        /// <summary>
        /// گرفتن ساعت مجاز شروع رزرو شیفت برای مددکار
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/v2/Shifts/Madadkar/GetClock")]
        public IHttpActionResult getClock()
        {
            if (DateTime.Now.Hour >= 12 && DateTime.Now.Hour <= 21)
            {
                return Ok(DateTime.Now);
            }
            return NotFound();
        }

        /// <summary>
        /// دریافت شیفت های مددکار در ماه جاری
        /// </summary>
        /// <param name="madadkar_id"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/v2/Shifts/Madadkar/GetMadadkarShiftsCount")]
        public IHttpActionResult getMadadkarShiftCount(int madadkar_id)
        {
            var today = DateTime.Now;
            DateTime start_date;
            DateTime end_date;

            PersianCalendar pc = new PersianCalendar();
            var shamsi_month = pc.GetMonth(today);
            var shamsi_year = pc.GetYear(today);
            if (shamsi_month > 6)
            {
                end_date = pc.ToDateTime(shamsi_year, shamsi_month, 30, 23, 59, 59, 00);
            }
            else
            {
                end_date = pc.ToDateTime(shamsi_year, shamsi_month, 31, 23, 59, 59, 0);

            }
            start_date = pc.ToDateTime(shamsi_year, shamsi_month, 1, 0, 0, 0, 0);

            var result = SipDataEntity.JobShiftsView.Where(x => x.MadadkarId == madadkar_id && x.JobDate >= start_date && x.JobDate <= end_date).ToList();
            var allowed = SipDataEntity.settings.FirstOrDefault(x => x.setting_name == "allowed_shifts_in_month");
            MadadkarShiftsInfo info = new MadadkarShiftsInfo()
            {
                AllowedShiftsInMonth = Int32.Parse(allowed.setting_value ?? "10"),
                ShiftListInMonth = result
            };

            return Ok(info);
        }
        /// <summary>
        /// کلاس برای متد دریافت شیفتهای مددکار در ماه جاری
        /// </summary>
        class MadadkarShiftsInfo
        {
            public int AllowedShiftsInMonth { get; set; }
            public ICollection<JobShiftsView> ShiftListInMonth { get; set; }
        }


    }
}