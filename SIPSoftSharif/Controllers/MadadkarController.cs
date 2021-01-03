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
using static SIPSoftSharif.Models.Paging;

namespace SIPSoftSharif.Controllers
{
    public class MadadkarController : ApiController
    {
        BpmsSharifDataEntities SharifDataEntity = new BpmsSharifDataEntities();
        //MadadkarOnlineDbEntities MadadkarEnt = new MadadkarOnlineDbEntities();
        MadadkarOnlineEntities SipDataEntity = new MadadkarOnlineEntities();
        //دریافت لیست حامیان مددکار
        [Authorize(Roles ="Madadkar")]
        [HttpPost]
        [Route("api/Madadkar/GetHamis")]
        public IHttpActionResult GetHamis()
        {
            var identity = (ClaimsIdentity)User.Identity;
            var MadadkarId = identity.Claims.Where(s => s.Type == "MadadkarId").FirstOrDefault();
            int MadadkarID = int.Parse(MadadkarId.Value);
            
            var result = SharifDataEntity.FG_HamiMadadkarsInfo.Where(x => x.MadadkarId == MadadkarID && x.Deleted!=true && x.HamiMobile1!=null).OrderBy(r=>r.HamiLName).ToList();
            return Json(result);

        }

        //دریافت لیست حامیان در نسخه دوم
       [Authorize(Roles ="Madadkar")]
        [HttpPost]
        [Route("api/Madadkar/GetHamisForEdit")]
        public IHttpActionResult GetHamisForEdit()
        {
            var identity = (ClaimsIdentity)User.Identity;
            var MadadkarId = identity.Claims.Where(s => s.Type == "MadadkarId").FirstOrDefault();
            int MadadkarID = int.Parse(MadadkarId.Value);
            //int MadadkarID = 230;
            var result = SharifDataEntity.FG_HamiMadadkarsInfo.Where(x => x.MadadkarId == MadadkarID && x.Deleted != true && x.HamiMobile1 != null ).OrderBy(r => r.HamiLName);
            foreach(var item in result)
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
            var res = SipDataEntity.HamiEditSet.Where(x => x.MadadkarId == MadadkarID && x.Deleted!=true).Select(d => new
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

        //حذف حامی در نسخه دوم
        [HttpPost]
        [Route("api/Madadkar/DeleteHamiById")]
        public IHttpActionResult DeleteHamiById(deleteHami deleteHami)
        {
            var result = SipDataEntity.HamiEditSet.FirstOrDefault(x => x.HamiId == deleteHami.HamiId);
            try
            {
                result.Deleted = true;
                result.DeleteCuase = deleteHami.deleteCuase;
                SipDataEntity.SaveChanges();
                return Ok("Deleted " + deleteHami.HamiId);
            }
            catch (Exception ex) {
                throw ex;
            }
            
        }

        public class deleteHami
        {
            public int HamiId { get; set; }
            public string deleteCuase { get; set; }
        }


        //دریافت حامی با کد حامی
       [Authorize(Roles = "Madadkar")]
        [HttpPost]
        [Route("api/Madadkar/GetHamisById")]
        public IHttpActionResult GetHamisById(int hamiId)
        {
            var identity = (ClaimsIdentity)User.Identity;
            var MadadkarId = identity.Claims.Where(s => s.Type == "MadadkarId").FirstOrDefault();
            int MadadkarID = int.Parse(MadadkarId.Value);

            var result = SharifDataEntity.FG_HamiMadadkarsInfo.Where(x => x.MadadkarId == MadadkarID && x.Deleted != true && x.HamiId == hamiId).FirstOrDefault();
            
            return Json(result);

        }

        //دریافت اطلاعات مددکار
        [Authorize(Roles ="Madadkar")]
        [HttpPost]
        [Route("api/Madadkar/GetMadadkarInfo")]
        public IHttpActionResult GetMadadkarInfo()
        {
            MadadkarModel model = new MadadkarModel();
            var identity = (ClaimsIdentity)User.Identity;
            var MadadkarId = identity.Claims.Where(s => s.Type == "MadadkarId").FirstOrDefault();
            int MadadkarID = int.Parse(MadadkarId.Value);
            var SIPResult = SipDataEntity.SIPExtensions.Where(s => s.MadadkarId == MadadkarID).FirstOrDefault();
            if (SIPResult == null )
            {
                if(SipDataEntity.SIPExtensions.Where(s=>s.MadadkarId==null).Count()==0)
                {
                    int max =int.Parse( SipDataEntity.SIPExtensions.OrderByDescending(x=>x.Id).Select(x=>x.DisplayName).FirstOrDefault());
                    max += 1;

                    var s = new SIPExtensions() {
                        DisplayName = max.ToString(),
                        Extention = max,
                        MadadkarId = MadadkarID,
                        MadadkarName = identity.Name,
                        RegDate = DateTime.Now,
                        Password = System.Web.Security.Membership.GeneratePassword(15,5),
                        Enabled=false


                    
                    };
                    SipDataEntity.SIPExtensions.Add(s);
                    SipDataEntity.SaveChanges();

                }
                else
                {
                    var selectedExt = SipDataEntity.SIPExtensions.Where(s => s.MadadkarId == null).FirstOrDefault();
                    selectedExt.MadadkarId = MadadkarID;
                    selectedExt.MadadkarName = identity.Name;
                    selectedExt.RegDate = DateTime.Now;
                    SipDataEntity.SaveChanges();
                }
            }
            SIPResult = SipDataEntity.SIPExtensions.Where(s => s.MadadkarId == MadadkarID).FirstOrDefault();

            return Ok(new MadadkarModel {
                MadadkarName = identity.Name,
                MadadkarId = MadadkarID,
                SipDisplayname = SIPResult.DisplayName,
                SipExtention = SIPResult.Extention.ToString(),
                SipPassword = SIPResult.Password,
                SipUrl = string.Format("sip:{0}@vs.sharifngo.com", SIPResult.Extention),
                SipWsUrl = "ws://vs.sharifngo.com:8088/ws"
            });
        }


        //دریافت لیست تماس های قبلی مددکار
        [Route("api/Madadkar/GetCallHistory")]
        [HttpPost]
        [Authorize(Roles ="Madadkar")]
        public IHttpActionResult GetCallHistory(int HamiId)
        {
            var result = SipDataEntity.CallStatus.Where(x => x.HamiId == HamiId).OrderByDescending(s => s.id).ToList();
            var result2 = result.Take(5);
            return Ok(result2);
        }

        //ثبت نتیجه تماس مددکار
        [Route("api/Madadkar/AddCallResults")]
        [HttpPost]
        [Authorize(Roles ="Madadkar")]
        public  IHttpActionResult  AddCallResults (callStatusInput call)
            
        {
            var identity = (ClaimsIdentity)User.Identity;

            var hami = SharifDataEntity.FG_Hamis.Where(x => x.HamiId == call.HamiId).FirstOrDefault();
            var MadadkarId = identity.Claims.Where(s => s.Type == "MadadkarId").FirstOrDefault();
            var MadadkarName = identity.Name;
            try {
                var addresult = SipDataEntity.CallStatus.Add(new CallStatus()
                {
                    Date = call.dateof,
                    Description = call.Description,
                    StartTime = call.StartTime,
                    EndTime=call.EndTime,
                    HamiId=call.HamiId,
                    HamiName=hami.HamiFName+' '+hami.HamiLName,
                    MaadadkarName=MadadkarName,
                    MadadkarId=int.Parse(MadadkarId.Value),
                    PhoneNumber=call.MobileNum,
                     Status=call.status
                }) ;
                SipDataEntity.SaveChanges();
                return Ok("9001");
            
            }
            catch(Exception er)
            {
                throw;
            }
            //return NotFound()

        }
        public class callStatusInput
        {
            public int HamiId { get; set; }
            public string dateof { get; set; }
            public string StartTime { get; set; }
            public string EndTime   { get; set; }
            public string Description { get; set; }
            public string MobileNum { get; set; }
            public int status { get; set; }
        }

        //افزودن یک رخداد در تقویم
        [Route("api/Madadkar/AddCalendarEvent")]
        [HttpPost]
        [Authorize(Roles ="Madadkar,Admin")]
        public IHttpActionResult AddCalendarEvent(CalendarEvent calendarEvent)
        {
            try
            {
                SipDataEntity.CalendarEvent.Add(calendarEvent);
                var result = SipDataEntity.SaveChanges();
                return Ok(result);

            }catch (Exception e)
            {
                throw e;
            }
        }

        //Get Calendar for Madadkar
        [Route("api/Madadkar/GetCalendarEvents")]
        [HttpPost]
        [Authorize(Roles ="Madadkar,Admin")]
        public IHttpActionResult GetCalendarForMadadkar(PagingParameterModel paging)
        {
            var identity = (ClaimsIdentity)User.Identity;
            var MadadkarId = identity.Claims.Where(s => s.Type == "MadadkarId").FirstOrDefault();
            int MadadkarID = int.Parse(MadadkarId.Value);

            var source = SipDataEntity.CalendarEvent.Where(b=>b.MadadkarId==MadadkarID).OrderByDescending(c => c.Date).AsQueryable();


            int count = source.Count();

            // Parameter is passed from Query string if it is null then it default Value will be pageNumber:1  
            int CurrentPage = paging.pageNumber;

            // Parameter is passed from Query string if it is null then it default Value will be pageSize:20  
            int PageSize = paging.pageSize;

            // Display TotalCount to Records to User  
            int TotalCount = count;

            // Calculating Totalpage by Dividing (No of Records / Pagesize)  
            int TotalPages = (int)Math.Ceiling(count / (double)PageSize);

            // Returns List of Customer after applying Paging   
            var items = source.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();

            // if CurrentPage is greater than 1 means it has previousPage  
            var previousPage = CurrentPage > 1 ? "Yes" : "No";

            // if TotalPages is greater than CurrentPage means it has nextPage  
            var nextPage = CurrentPage < TotalPages ? "Yes" : "No";

            // Object which we are going to send in header   
            var paginationMetadata = new
            {
                totalCount = TotalCount,
                pageSize = PageSize,
                currentPage = CurrentPage,
                totalPages = TotalPages,
                previousPage,
                nextPage
            };

            // Setting Header  
            HttpContext.Current.Response.Headers.Add("Paging-Headers", JsonConvert.SerializeObject(paginationMetadata));
            // Returing List of Customers Collections  
            return Ok(items);
        }
        // دریافت اطلاعات حامی جهت ذخیره سازی در جدول موقت
        [Route("api/Madadkar/GetHamiInfo")]
       // [Authorize(Roles ="Madadkar")]
        [HttpPost]
        public IHttpActionResult GetHamiInfo(int HamiId)
        {
            //var identity = (ClaimsIdentity)User.Identity;
            //var MadadkarId = identity.Claims.Where(s => s.Type == "MadadkarId").FirstOrDefault();
           // int MadadkarID = int.Parse(MadadkarId.Value);
            //var MadadkarFullName = identity.Name;
            HamiEditSet hamiEdit = new HamiEditSet();


            try
            {
                var result = SipDataEntity.HamiEditSet.FirstOrDefault(a => a.HamiId == HamiId);
                if (result == null)
                {
                    
                    var request = SharifDataEntity.FG_Hamis.FirstOrDefault(a => a.HamiId == HamiId);
                    hamiEdit.HamiId = HamiId;
                    hamiEdit.HamiFname = request.HamiFName??"";
                    hamiEdit.HamiLname = request.HamiLName??"";
                    hamiEdit.OldMobile1 = request.HamiMobile1??"";
                    hamiEdit.OldMobile2 = request.HamiMobile2??"";
                    hamiEdit.OldPhone1 = request.HamiPhone1??"";
                    hamiEdit.OldPhone2 = request.HamiPhone2??"";
                    hamiEdit.Email = request.HamiEmail??"";
                    SipDataEntity.HamiEditSet.Add(hamiEdit);
                    SipDataEntity.SaveChanges();


                }
                
                
            }
            catch(Exception err)
            {
                throw err;
            }
            var res = SipDataEntity.HamiEditSet.Where(x => x.HamiId == HamiId).Select(d=> new { d.Id,d.HamiId,d.HamiFname,d.HamiLname,d.MadadkarId,d.MadadkarName,d.NationalCode,
                d.NewHamiFname,d.NewHamiLname,d.NewMobile1,d.NewMobile2,d.NewPhone1,d.NewPhone2,d.OldMobile1,d.OldMobile2,d.OldPhone1,d.OldPhone2,d.TempSave,d.DeleteOldMobile1,d.DeleteOldMobile2,
                d.DeleteOldPhone1,d.DeleteOldPhone2,d.EditDate,d.Email,d.FinalSave,HamiMadadjouSet=SipDataEntity.HamiMadadjouSet.Where(f=>f.Deleted!=true && f.HamiId==HamiId).Select(
                    t=>new {t.HamiId,t.Id,t.MadadjouFname,t.MadadjouLname,t.MadadjouId,t.Deleted }
                    ).ToList()
            
            }).FirstOrDefault();
            
            

            return Ok(res);
        }
        //ذخیره اصلاحات مربوط به مشخصات حامی
        [Route("api/Madadkar/SaveHamiEditInfo")]
        [HttpPost]
        public IHttpActionResult SaveHamiEditInfo(HamiEditSet hami)
        {
            try
            {
                SipDataEntity.HamiEditSet.AddOrUpdate(hami);
                SipDataEntity.SaveChanges();
                return Ok("Saved");
            }catch (Exception err)
            {
                throw err;
            }
            
        }


        //دریافت اطلاعات کودکان حامی
        [Route("api/Madadkar/GetHamiMadadjous")]
        [HttpPost]
        public IHttpActionResult GetHamiMadadjous(int HamiId)
        {
            var result = SipDataEntity.HamiMadadjouSet.Where(a => a.HamiId == HamiId && a.Deleted==false);
            if (result.Count() <= 0)
            {
                return NotFound();
            }
            return Ok(result);
        }

        //افزودن کودک برای حامی
        [Route("api/Madadkar/AddMadadjouToHami")]
        [HttpPost]
        public IHttpActionResult AddMadadjouToHami(HamiMadadjouSet hamiMadadjou)
        {
            try
            {

                SipDataEntity.HamiMadadjouSet.AddOrUpdate(hamiMadadjou);
                SipDataEntity.SaveChanges();
                return Ok("Saved");
            }catch (Exception err)
            {
                throw err;
            }
        }

        //حذف کودک برای حامی
        [Route("api/Madadkar/RemoveMadadjouToHami")]
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
            catch (Exception er) { 
            throw er;
            }
        }

        //دریافت لیست کودکان
        [HttpPost]
        [Route("api/Madadkar/GetMadadjouList")]
        public IHttpActionResult GetMadadjouList()
        {
            try
            {
                var result=SharifDataEntity.FG_MadadjusInfo2.Where(a => a.Deleted == false).Select(x=>new { x.MadadjuId,x.MadadjuFName,x.MadadjuLName});
                return Ok(result);
            }catch(Exception err)
            {
                throw err;
            }
        }

        //اصلاح تلفن حامی
        [Route("api/Madadkar/ModifyPhone")]
        [Authorize(Roles ="Madadkar")]
        [HttpPost]
        public IHttpActionResult ModifyPhone(HamiPhone hamiPhone)
        {
            /*            var identity = (ClaimsIdentity)User.Identity;
                        var MadadkarId = identity.Claims.Where(s => s.Type == "MadadkarId").FirstOrDefault();
                        int MadadkarID = int.Parse(MadadkarId.Value);
                        var MadadkarFullName = identity.Name;
                        try
                        {
                            var source = SharifDataEntity.FG_Hamis.Where(s => s.HamiId == hamiPhone.HamiId ).FirstOrDefault();
                            var hami = source.HamiFName + " " + source.HamiLName;

                            PhoneChange  change= new PhoneChange()
                            {
                                HamiId = hamiPhone.HamiId,
                                HamiName = hami ,
                                Confirmed = true,
                                Date = DateTime.Now,
                                MadadkarId = MadadkarID,
                                MadadkarName = MadadkarFullName,
                                NewPhoneNumber = hamiPhone.NewPhoneNumber,
                                OldPhoneNumber = hamiPhone.PrePhoneNumber

                            };
                            if (source.HamiMobile1==hamiPhone.PrePhoneNumber)
                            {
                                source.HamiMobile1 = hamiPhone.NewPhoneNumber;
                                SharifDataEntity.SaveChanges();
                                SipDataEntity.PhoneChange.Add(change);
                                SipDataEntity.SaveChanges();
                            }

                            if (source.HamiMobile2 == hamiPhone.PrePhoneNumber)
                            {
                                source.HamiMobile2 = hamiPhone.NewPhoneNumber;
                                SharifDataEntity.SaveChanges();
                                SipDataEntity.PhoneChange.Add(change);
                                SipDataEntity.SaveChanges();
                            }


                        }catch (Exception er)
                        {
                            throw er;
                        }
                        return Ok(SharifDataEntity.FG_Hamis.Where(s=>s.HamiId==hamiPhone.HamiId).FirstOrDefault());
            */
            return Ok();




        }

        //دریافت کد یکتا برای پرداخت
        [HttpGet]
        [Route("api/Madadkar/GetUniquePaymentID")]
        [Authorize(Roles ="Madadkar")]
        public IHttpActionResult GetPaymentUrl(int Hami_Id) 
        {
            var identity = (ClaimsIdentity)User.Identity;
            var MadadkarId = identity.Claims.Where(s => s.Type == "MadadkarId").FirstOrDefault();
            int MadadkarID = int.Parse(MadadkarId.Value);
            var MadadkarFullName = identity.Name;
            var hami = SharifDataEntity.FG_Hamis.Where(x => x.HamiId == Hami_Id).FirstOrDefault();
            var HamiName = hami.HamiFName + " " + hami.HamiLName;
            string uniqe = (DateTime.Now.ToString() + MadadkarID.ToString() + Hami_Id.ToString()).GetHashCode().ToString("x");
            
            SipDataEntity.PayRequests.Add(new PayRequests() {
            CreateDate=DateTime.Now,
            HamiId=Hami_Id,
            HamiName=HamiName,
            MadadkarId=MadadkarID,
            MadadkarName=MadadkarFullName,
            UniqeId=uniqe
            });
            SipDataEntity.SaveChanges();
            return Ok("https://paysb.sharifngo.com/?id="+ uniqe);

        }


        //دریافت لیست مددجویان ویژه یک حامی
       [HttpGet]
       [Route("api/Madadkar/GetHamiMadadjous")]
      // [Authorize(Roles ="Madadkar")]
       public IHttpActionResult GetMadadjous(int hamiId)
        {
            var result = SharifDataEntity.FG_HamiMadadjusInfo.Where(x => x.HamiId == hamiId).Select(x => new { x.MadadjuId, x.MadadjuFName, x.MadadjuLName }).ToList();
            return Ok(result);
        }
        public class MadadjouInfo
        {
            public int MadadjouId { get; set; }
            public String MadadjouName { get; set; }

        }
        
        //افزودن شیفت کاری
        [HttpPost]
        [Route("api/Job/AddSchedule")]
        public IHttpActionResult AddJobSchedule(JobSchedule job)
        {
            if( job.id!=0)
            {
                var exist = SipDataEntity.JobSchedule.Where(x => x.id == job.id).FirstOrDefault();
                if(exist!=null)
                {
                    //SipDataEntity.JobSchedule.Remove(exist);
                   
                    
                    foreach (JobShift item in job.JobShift)
                    {
                        foreach(ShiftPersons it in item.ShiftPersons)
                        {
                            SipDataEntity.ShiftPersons.AddOrUpdate(it);
                        }
                        SipDataEntity.JobShift.AddOrUpdate(item);
                        
                    }
                    SipDataEntity.JobSchedule.AddOrUpdate(job);

                   // SipDataEntity.SaveChanges();
                }
            }
            else
            {
                SipDataEntity.JobSchedule.Add(job);
            }
            SipDataEntity.SaveChanges();

          
            return Ok("Saved");
        }

        //افزودن شیفت کاری
        [HttpPost]
        [Route("api/Job/Differ")]
        public IHttpActionResult Differ(JobSchedule job) {
            var exist = SipDataEntity.JobSchedule.Where(x => x.id == job.id).FirstOrDefault();
            if (exist != null)
            {
                var side1 = job.JobShift;
                var master = SipDataEntity.JobSchedule.Where(x => x.id == job.id).FirstOrDefault();
                var side2 = master.JobShift;
                var differ = from t1 in side1
                             join t2 in side2
                             on t1.id equals t2.id
                             into m
                             from x in m.DefaultIfEmpty()
                             select x;
                return Ok(differ);
            }
            return NotFound();
        }

        //دریافت شیفت کاری یک روز خاص
        [HttpGet]
        [Route("api/Job/GetTodyJobSchedule")]
        public IHttpActionResult GetTodayJobSchedule(string now=null,int duration=0)
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

            var result = SipDataEntity.JobSchedule.Where(x => x.JobDate == nowdate ).FirstOrDefault();
            if(result!=null)
               return Ok(result);
            return NotFound();
        }

        [HttpGet]
        [Route("api/Job/GetMadadakrSchedule")]
        public IHttpActionResult GetMadadkarSchedule(int madadkarId,string now = null)
        {
            DateTime nowdate = new DateTime();
            if (now != null)
                nowdate = DateTime.ParseExact(now, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            else
            {
                nowdate = DateTime.Now;
                nowdate = DateTime.ParseExact(nowdate.ToShortDateString(), "yyyy-MM-dd", CultureInfo.InvariantCulture);
            }

            var result = (from job in SipDataEntity.JobSchedule
                         from shift in job.JobShift
                         from persons in shift.ShiftPersons
                         where persons.MadadkarId == madadkarId && job.JobDate == nowdate
                         select job).FirstOrDefault();

            return Ok(result);
        }

        //دریافت شیفت کاری امروز
        [HttpGet]
        [Route("api/Job/GetTodyShift")]
        public IHttpActionResult GetTodayShift(int daytoadd)
        {
            DateTime nowdate = new DateTime();
            nowdate = DateTime.Now.AddDays(daytoadd);
            string my = nowdate.ToString( "yyyy-MM-dd", CultureInfo.InvariantCulture);
            nowdate = DateTime.ParseExact(my,"yyyy-MM-dd", CultureInfo.InvariantCulture);

            var result = SipDataEntity.JobSchedule.Where(x => x.JobDate == nowdate).FirstOrDefault();
            

            return Ok(result);
        }



        //دریافت شیفت های کاری یک بازه
        [HttpGet]
        [Route("api/Job/GetJobScheduleByRange")]
        public IHttpActionResult GetJobScheduleByRange(string StartDate,string EndDate)
        {
            DateTime start = DateTime.ParseExact(StartDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime end = DateTime.ParseExact(EndDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            IList<JobSchedule> result = SipDataEntity.JobSchedule.Where(x => x.JobDate >= start && x.JobDate <= end).ToList();
            return Ok(result);
        }

        //ثبت ساعت ورود مددکار
        [HttpPost]
        [Route("api/Job/AddEntranceTime")]
        public IHttpActionResult addEntrance(int ShiftPersonId,int JobShiftId)
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

        //ثبت خروج مددکار
        [HttpPost]
        [Route("api/Job/AddExitTime")]
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


        //دریافت اطلاعات یک شیفت برای ادمین
        [HttpPost]
        [Route("api/Job/GetShiftById")]
        public IHttpActionResult GetShiftById(int ShiftId)
        {
            JobShift result = SipDataEntity.JobShift.Where(x => x.id == ShiftId).FirstOrDefault();
            //var result2 = result.ShiftPersons;
            var s = result.id;
            if (result != null)
                return Ok(result);
            return NotFound();
        }
        
        //چک کردن تعداد شیفت های مددکار در ماه جاری
        [HttpPost]
        [Route("api/Job/GetMadadkarShiftsCount")]
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
                end_date = pc.ToDateTime(shamsi_year, shamsi_month, 30,23,59,59,00);
            }
            else
            {
                end_date = pc.ToDateTime(shamsi_year, shamsi_month, 31, 23, 59, 59, 0);

            }
            start_date= pc.ToDateTime(shamsi_year, shamsi_month, 1, 0, 0, 0, 0);

            var result = SipDataEntity.JobShiftsView.Where(x=>x.MadadkarId==madadkar_id && x.JobDate>=start_date && x.JobDate<=end_date).ToList();
            var allowed = SipDataEntity.settings.FirstOrDefault(x => x.setting_name == "allowed_shifts_in_month");
            MadadkarShiftsInfo info = new MadadkarShiftsInfo() {
                AllowedShiftsInMonth = Int32.Parse(allowed.setting_value ?? "10"),
                ShiftListInMonth = result
            };

            return Ok(info);
        }

        class MadadkarShiftsInfo
        {
            public int AllowedShiftsInMonth { get; set; }
            public ICollection<JobShiftsView> ShiftListInMonth { get; set; }
        }


        //رزرو شیفت کاری برا ی مددکار
        [HttpPost]
        [Route("api/Job/AddShiftForMadadkar1")]
        public IHttpActionResult AddShiftForMadadkar(int shiftid,int madadkarId)
        {
           if(DateTime.Now.Hour>=12 && DateTime.Now.Hour <= 21)
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

        //گرفتن ساعت مجاز
        [HttpPost]
        [Route("api/Job/GetClock1")]
        public IHttpActionResult getClock()
        {
            if (DateTime.Now.Hour>=12 && DateTime.Now.Hour <= 21)
            {
                return Ok(DateTime.Now);
            }
            return NotFound();
        }

        //حذف شیفت کاری برای مددکار
        [HttpPost]
        [Route("api/Job/RemoveShiftForMadadkar1")]
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
    }

    
    public class HamiPhone
    {
        public string PrePhoneNumber { get; set; }
        public string NewPhoneNumber { get; set; }
        public int HamiId { get; set; }
    }
    internal class MadadkarModel
    {
        public string MadadkarName { get; set; }
        public int MadadkarId { get; set; }
        public string SipWsUrl { get; set; }
        public string SipUrl { get; set; }
        public string SipDisplayname { get; set; }
        public string SipPassword { get; set; }
        public string SipExtention { get; set; }
    }
}
