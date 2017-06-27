using CRM.Core;
using CRM.Enum;
using CRM.Infrastructure;
using TOURDEMO.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TOURDEMO.Utilities
{
    //hello
    public static class LoadData
    {
        private static DataContext _db = new DataContext();

        public static List<SelectListItem> ListHeadQuarterItem()
        {
            var listHeadQuarter = _db.tbl_Headquater.Where(x => x.IsDelete == false).Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.HeadquarterName }).ToList();
            return listHeadQuarter;
        }
        public static tbl_Headquater GetHeadquaterByID(int id)
        {
            var headQuarter = _db.tbl_Headquater.Where(x => x.IsDelete == false && x.Id == id).FirstOrDefault();
            return headQuarter;
        }
        
        public static string LocationTags(string tagsId)
        {
            string kq = "";
            if (tagsId != null)
            {
                var array = tagsId.Split(',');
                for (int i = array.Count() - 1; i >= 0; i--)
                {
                    if (array[i] != "")
                    {
                        if (i == 0)
                        {
                            kq += _db.tbl_Tags.Find(Convert.ToInt32(array[i])).Tag;
                        }
                        else
                        {
                            kq += _db.tbl_Tags.Find(Convert.ToInt32(array[i])).Tag + ", ";
                        }
                    }
                }
            }

            return kq;
        }
        public static tbl_Staff GetStaffbyId(int _staffID)
        {
            var staff = _db.tbl_Staff.Where(x => x.Id == _staffID && x.IsDelete == false).FirstOrDefault();
            return staff;
        }
        public static TimeWorkingDayOff GetTimeWorkingDayOffByStaffID(int _staffID)
        {
            var staff = _db.tbl_Staff.Where(x => x.Id == _staffID && x.IsDelete == false).FirstOrDefault();
            if (staff == null)
            {
                return new TimeWorkingDayOff();
            }
            int datediffDays = 0;
            if (staff.IsLock)
            {
                datediffDays = Convert.ToInt32((staff.EndWork - staff.StartWork).Value.TotalDays);
            }
            else
            {
                datediffDays = Convert.ToInt32((DateTime.Now - staff.StartWork).Value.TotalDays);
            }
            var years = datediffDays / 365;
            var months = (datediffDays % 365) / 30;
            var days = (datediffDays % 365) % 30;   
            TimeWorkingDayOff staffTime = new TimeWorkingDayOff(years)
            {
                Days = days,
                Months = months,
                Years = years
            };
            return staffTime;
        }
        public static int GetTimeChangeSalary(int idStaffSalaryDetail)
        {
            int _time = 0;
            var _staffSalaryDetail = _db.tbl_StaffSalaryDetail.Where(x => x.Id == idStaffSalaryDetail).FirstOrDefault();
            var year = _staffSalaryDetail.ApplyDate.Year;
            DateTime firstDate = new DateTime(year, 1, 1);
            DateTime lastDate = new DateTime(year, 12, 31,23,59,59);
            var _listStaffSalaryDetail = _db.tbl_StaffSalaryDetail.Where(x => x.StaffSalaryId == _staffSalaryDetail.StaffSalaryId && x.ApplyDate >= firstDate && x.ApplyDate <= lastDate).OrderBy(p => p.ApplyDate).ToList();   
            for(int i=0;i< _listStaffSalaryDetail.Count; i++)
            {
                if(_staffSalaryDetail.ApplyDate== _listStaffSalaryDetail[i].ApplyDate)
                {
                    _time = (i + 1);
                }
            }
            return _time;
        }
        public static void ResetDayOff()
        {

        }

        /// <summary>
        /// load phòng ban
        /// </summary>
        /// <param name="tagsId"></param>
        /// <returns></returns>
        public static string Department(string staffId)
        {
            string kq = _db.tbl_Staff.Find(Convert.ToInt32(staffId)).tbl_DictionaryDepartment.Name;
            return kq;
        }

        /// <summary>
        /// load tên nhân viên
        /// </summary>
        /// <param name="staffId"></param>
        /// <returns></returns>
        public static string LoadStaff(string staffId)
        {
            string kq = "";
            var staff = _db.tbl_Staff;
            if (staffId != null)
            {
                var array = staffId.Split(',');
                for (int i = array.Count() - 1; i >= 0; i--)
                {
                    if (array[i] != "")
                    {
                        if (i == 0)
                        {
                            kq += staff.Find(Convert.ToInt32(array[i])).FullName;
                        }
                        else
                        {
                            kq += staff.Find(Convert.ToInt32(array[i])).FullName + ", ";
                        }
                    }
                }
            }

            return kq;
        }

        /// <summary>
        /// nhân viên thực hiện nhiệm vụ
        /// </summary>
        /// <param name="tagsId"></param>
        /// <returns></returns>
        public static string StaffTask(string staffId)
        {
            if (staffId != "")
            {
                var item = _db.tbl_Staff.Find(Convert.ToInt32(staffId));
                if (item != null)
                { return item.FullName; }
                else
                { return ""; }
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// lấy ra tên các nhân viên
        /// </summary>
        /// <param name="tagsId"></param>
        /// <returns></returns>
        public static string StaffPermission(string staffId)
        {
            string kq = "";
            var array = staffId.Split(',');
            for (int i = array.Count() - 1; i >= 0; i--)
            {
                if (array[i] != "")
                {
                    if (i == 0)
                    {
                        kq += _db.tbl_Staff.Find(Convert.ToInt32(array[i])).FullName;
                    }
                    else
                    {
                        kq += _db.tbl_Staff.Find(Convert.ToInt32(array[i])).FullName + ", ";
                    }
                }
            }

            return kq;
        }

        /// <summary>
        /// các tag vị trí địa lý
        /// </summary>
        /// <returns></returns>
        public static List<TagsViewModel> DropdownlistLocation()
        {
            var model = CacheLayer.Get<List<TagsViewModel>>("tagLocationList");
            if (model == null)
            {
                model = _db.tbl_Tags.Where(p => p.IsDelete == false).Select(p => new TagsViewModel
                {
                    Id = p.Id,
                    Tags = p.Tag
                }).ToList();
                CacheLayer.Add<List<TagsViewModel>>(model, "tagLocationList", 1);
            }

            return model;
        }

        /// <summary>
        /// danh sách quốc gia
        /// </summary>
        /// <returns></returns>
        public static List<TagsViewModel> DropdownlistCountry()
        {
            var model = CacheLayer.Get<List<TagsViewModel>>("tagCountryList");
            if (model == null)
            {
                model = _db.tbl_Tags.Where(p => p.TypeTag == 3).Where(p => p.IsDelete == false).Select(p => new TagsViewModel
                {
                    Id = p.Id,
                    Tags = p.Tag
                }).ToList();
                CacheLayer.Add<List<TagsViewModel>>(model, "tagCountryList", 1);
            }

            return model;
        }

        /// <summary>
        /// danh sách tỉnh thành
        /// </summary>
        /// <returns></returns>
        public static List<TagsViewModel> ProvinceList()
        {
            var model = CacheLayer.Get<List<TagsViewModel>>("ProvinceList");
            if (model == null)
            {
                model = _db.tbl_Tags.Where(p => p.IsDelete == false && p.TypeTag == 5)
                            .Select(p => new TagsViewModel
                            {
                                Id = p.Id,
                                Tags = p.Tag
                            }).ToList();
                CacheLayer.Add<List<TagsViewModel>>(model, "ProvinceList", 1);
            }

            return model;
        }

        /// <summary>
        /// danh sách tỉnh thành, quốc gia
        /// </summary>
        /// <returns></returns>
        public static List<TagsViewModel> TinhThanhQuocGia()
        {
            var model = CacheLayer.Get<List<TagsViewModel>>("TinhThanhQuocGia");
            if (model == null)
            {
                model = _db.tbl_Tags.Where(p => p.IsDelete == false && (p.TypeTag == 3 || p.TypeTag == 5))
                            .Select(p => new TagsViewModel
                            {
                                Id = p.Id,
                                Tags = p.Tag
                            }).ToList();
                CacheLayer.Add<List<TagsViewModel>>(model, "TinhThanhQuocGia", 1);
            }

            return model;
        }

        /// <summary>
        /// danh sách tất cả các đối tác
        /// </summary>
        /// <returns></returns>
        public static List<tbl_Partner> PartnerAllList()
        {
            //var model = CacheLayer.Get<List<tbl_Partner>>("partnerAllList");
            //if (model == null)
            //{
            var model = _db.tbl_Partner.AsEnumerable().Where(p => p.IsDelete == false).Select(p => new tbl_Partner
            {
                Id = p.Id,
                Name = p.Name,
                Code = p.Code
            }).ToList();
            //    CacheLayer.Add<List<tbl_Partner>>(model, "partnerAllList", 1);
            //}

            return model;
        }

        /// <summary>
        /// danh sách đối tác
        /// </summary>
        /// <returns></returns>
        public static List<tbl_Partner> PartnerList(int id)
        {
            var model = CacheLayer.Get<List<tbl_Partner>>("partnerList" + id);
            if (model == null)
            {
                model = _db.tbl_Partner.AsEnumerable().Where(p => p.IsDelete == false).Where(p => p.DictionaryId == id).Select(p => new tbl_Partner
                {
                    Id = p.Id,
                    Name = p.Name,
                    Code = p.Code
                }).ToList();
                CacheLayer.Add<List<tbl_Partner>>(model, "partnerList" + id, 1);
            }

            return model;
        }

        /// <summary>
        /// danh sách loại tài liệu
        /// </summary>
        /// <returns></returns>
        public static List<DictionaryViewModel> DocumentTypeList()
        {
            var model = CacheLayer.Get<List<DictionaryViewModel>>("documentTypeList");
            if (model == null)
            {
                model = _db.tbl_Dictionary.Where(p => p.DictionaryCategoryId == 1).Where(p => p.IsDelete == false).Select(p => new DictionaryViewModel
                {
                    Id = p.Id,
                    Name = p.Name
                }).ToList();
                CacheLayer.Add<List<DictionaryViewModel>>(model, "documentTypeList", 1);
            }

            return model;
        }

        /// <summary>
        /// danh sách chuyến bay
        /// </summary>
        /// <returns></returns>
        public static List<DictionaryViewModel> FlightList()
        {
            var model = CacheLayer.Get<List<DictionaryViewModel>>("flightList");
            if (model == null)
            {
                model = _db.tbl_Dictionary.Where(p => p.DictionaryCategoryId == 25).Where(p => p.IsDelete == false).Select(p => new DictionaryViewModel
                {
                    Id = p.Id,
                    Name = p.Name
                }).ToList();
                CacheLayer.Add<List<DictionaryViewModel>>(model, "flightList", 1);
            }

            return model;
        }

        /// <summary>
        /// danh sách loại tour
        /// </summary>
        /// <returns></returns>
        public static List<DictionaryViewModel> TourTypeList()
        {
            var model = CacheLayer.Get<List<DictionaryViewModel>>("tourTypeList");
            if (model == null)
            {
                model = _db.tbl_Dictionary.Where(p => p.DictionaryCategoryId == 19).Where(p => p.IsDelete == false).Select(p => new DictionaryViewModel
                {
                    Id = p.Id,
                    Name = p.Name
                }).OrderByDescending(p => p.Id).ToList();
                CacheLayer.Add<List<DictionaryViewModel>>(model, "tourTypeList", 1);
            }

            return model;
        }

        /// <summary>
        /// danh sách loại nhiệm vụ
        /// </summary>
        /// <returns></returns>
        public static List<DictionaryViewModel> TaskTypeList()
        {
            var model = CacheLayer.Get<List<DictionaryViewModel>>("taskTypeList");
            if (model == null)
            {
                model = _db.tbl_Dictionary.Where(p => p.DictionaryCategoryId == 21).Where(p => p.IsDelete == false).Select(p => new DictionaryViewModel
                {
                    Id = p.Id,
                    Name = p.Name
                }).ToList();
                CacheLayer.Add<List<DictionaryViewModel>>(model, "taskTypeList", 1);
            }

            return model;
        }

        /// <summary>
        /// danh sách trạng thái nhiệm vụ
        /// </summary>
        /// <returns></returns>
        public static List<DictionaryViewModel> TaskStatusList()
        {
            var model = CacheLayer.Get<List<DictionaryViewModel>>("taskStatusList");
            if (model == null)
            {
                model = _db.tbl_Dictionary.Where(p => p.DictionaryCategoryId == 22).Where(p => p.IsDelete == false).Select(p => new DictionaryViewModel
                {
                    Id = p.Id,
                    Name = p.Name
                }).ToList();
                CacheLayer.Add<List<DictionaryViewModel>>(model, "taskStatusList", 1);
            }

            return model;
        }

        /// <summary>
        /// danh sách độ ưu tiên nhiệm vụ
        /// </summary>
        /// <returns></returns>
        public static List<DictionaryViewModel> TaskPriorityList()
        {
            var model = CacheLayer.Get<List<DictionaryViewModel>>("taskPriorityList");
            if (model == null)
            {
                model = _db.tbl_Dictionary.Where(p => p.DictionaryCategoryId == 23).Where(p => p.IsDelete == false).Select(p => new DictionaryViewModel
                {
                    Id = p.Id,
                    Name = p.Name
                }).ToList();
                CacheLayer.Add<List<DictionaryViewModel>>(model, "taskPriorityList", 1);
            }

            return model;
        }

        /// <summary>
        /// danh sách loại lịch hẹn
        /// </summary>
        /// <returns></returns>
        public static List<DictionaryViewModel> AppointmentTypeList()
        {
            var model = CacheLayer.Get<List<DictionaryViewModel>>("appointmentTypeList");
            if (model == null)
            {
                model = _db.tbl_Dictionary.Where(p => p.DictionaryCategoryId == 20).Where(p => p.IsDelete == false).Select(p => new DictionaryViewModel
                {
                    Id = p.Id,
                    Name = p.Name
                }).ToList();
                CacheLayer.Add<List<DictionaryViewModel>>(model, "appointmentTypeList", 1);
            }

            return model;
        }

        /// <summary>
        /// danh sách tình trạng visa
        /// </summary>
        /// <returns></returns>
        public static List<DictionaryViewModel> VisaStatusList()
        {
            var model = CacheLayer.Get<List<DictionaryViewModel>>("visaStatusList");
            if (model == null)
            {
                model = _db.tbl_Dictionary.Where(p => p.DictionaryCategoryId == 14).Where(p => p.IsDelete == false).Select(p => new DictionaryViewModel
                {
                    Id = p.Id,
                    Name = p.Name
                }).ToList();
                CacheLayer.Add<List<DictionaryViewModel>>(model, "visaStatusList", 1);
            }

            return model;
        }

        /// <summary>
        /// loại visa
        /// </summary>
        /// <returns></returns>
        public static List<DictionaryViewModel> VisaTypeList()
        {
            var model = CacheLayer.Get<List<DictionaryViewModel>>("visaTypeList");
            if (model == null)
            {
                model = _db.tbl_Dictionary.AsEnumerable().Where(p => p.DictionaryCategoryId == 15).Where(p => p.IsDelete == false).Select(p => new DictionaryViewModel
                {
                    Id = p.Id,
                    Name = p.Name
                }).ToList();
                CacheLayer.Add<List<DictionaryViewModel>>(model, "visaTypeList", 1);
            }

            return model;
        }

        /// <summary>
        /// nhiệm vụ
        /// </summary>
        /// <returns></returns>
        public static List<tbl_Task> TaskList()
        {
            var model = CacheLayer.Get<List<tbl_Task>>("taskList");
            if (model == null)
            {
                model = _db.tbl_Task.AsEnumerable().Where(p => p.IsDelete == false).Select(p => new tbl_Task
                {
                    Id = p.Id,
                    Name = p.Name,
                    Code = p.Code
                }).ToList();
                CacheLayer.Add<List<tbl_Task>>(model, "taskList", 1);
            }

            return model;
        }

        /// <summary>
        /// chương trình
        /// </summary>
        /// <returns></returns>
        public static List<tbl_Program> ProgramList()
        {
            var model = CacheLayer.Get<List<tbl_Program>>("programList");
            if (model == null)
            {
                model = _db.tbl_Program.AsEnumerable().Where(p => p.IsDelete == false).Select(p => new tbl_Program
                {
                    Id = p.Id,
                    Name = p.Name,
                    Code = p.Code
                }).ToList();
                CacheLayer.Add<List<tbl_Program>>(model, "programList", 1);
            }

            return model;
        }

        /// <summary>
        /// danh sách khách hàng
        /// </summary>
        /// <returns></returns>
        public static List<tbl_Customer> CustomerList()
        {
            var customer = CacheLayer.Get<List<tbl_Customer>>("customerList");
            if (customer == null)
            {
                customer = _db.tbl_Customer.AsEnumerable().Where(p => p.IsDelete == false && p.IsTemp == false)
                    .Select(p => new tbl_Customer
                    {
                        Id = p.Id,
                        FullName = p.FullName,
                        Code = p.Code
                    }).ToList();
                CacheLayer.Add<List<tbl_Customer>>(customer, "customerList", 1);
            }

            return customer;
        }
        public static List<tbl_Partner> PartnerList()
        {
            var customer = CacheLayer.Get<List<tbl_Partner>>("partnerList");
            if (customer == null)
            {
                customer = _db.tbl_Partner.AsEnumerable().Where(p => p.IsDelete == false)
                    .Select(p => new tbl_Partner
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Code = p.Code
                    }).ToList();
                CacheLayer.Add<List<tbl_Partner>>(customer, "partnerList", 1);
            }

            return customer;
        }

        /// <summary>
        /// danh sách hợp đồng
        /// </summary>
        /// <returns></returns>
        public static List<tbl_Contract> ContractList()
        {
            var contract = CacheLayer.Get<List<tbl_Contract>>("contractlist");
            if (contract == null)
            {
                contract = _db.tbl_Contract.AsEnumerable().Where(p => p.IsDelete == false)
                    .Select(p => new tbl_Contract
                    {
                        Id = p.Id,
                        Name = p.Code,
                        Code = p.Code
                    }).ToList();
                CacheLayer.Add<List<tbl_Contract>>(contract, "contractlist", 1);
            }

            return contract;
        }

        /// <summary>
        /// danh sách nhân viên
        /// </summary>
        /// <returns></returns>
        public static List<tbl_Staff> StaffList()
        {
            var staff = CacheLayer.Get<List<tbl_Staff>>("staffList");
            if (staff == null)
            {
                staff = _db.tbl_Staff.AsEnumerable().Where(p => p.IsDelete == false)
                    .Select(p => new tbl_Staff
                    {
                        Id = p.Id,
                        FullName = p.FullName,
                        Code = p.Code,
                        Birthday = p.Birthday,
                        Address = p.Address,
                        PositionId = p.PositionId,
                        DepartmentId = p.DepartmentId,
                        StaffGroupId = p.StaffGroupId
                    }).ToList();
                CacheLayer.Add<List<tbl_Staff>>(staff, "staffList", 1);
            }

            return staff;
        }

        /// <summary>
        /// nguồn đến
        /// </summary>
        /// <returns></returns>
        public static List<tbl_Dictionary> OriginList()
        {
            var model = CacheLayer.Get<List<tbl_Dictionary>>("originList");
            if (model == null)
            {
                model = _db.tbl_Dictionary.Where(p => p.DictionaryCategoryId == 4 && p.IsDelete == false).ToList();
                CacheLayer.Add<List<tbl_Dictionary>>(model, "originList", 1);
            }

            return model;
        }

        /// <summary>
        /// ngành nghề
        /// </summary>
        /// <returns></returns>
        public static List<tbl_Dictionary> CareerList()
        {
            var model = CacheLayer.Get<List<tbl_Dictionary>>("careerList");
            if (model == null)
            {
                model = _db.tbl_Dictionary.Where(p => p.DictionaryCategoryId == 2).Where(p => p.IsDelete == false).ToList();
                CacheLayer.Add<List<tbl_Dictionary>>(model, "careerList", 1);
            }

            return model;
        }

        /// <summary>
        /// nhóm khách hàng
        /// </summary>
        /// <returns></returns>
        public static List<tbl_Dictionary> CustomerGroupList()
        {
            var model = CacheLayer.Get<List<tbl_Dictionary>>("customerGroupList");
            if (model == null)
            {
                model = _db.tbl_Dictionary.Where(p => p.DictionaryCategoryId == 3).Where(p => p.IsDelete == false).ToList();
                CacheLayer.Add<List<tbl_Dictionary>>(model, "customerGroupList", 1);
            }

            return model;
        }

        /// <summary>
        /// trạng thái xử lý
        /// </summary>
        /// <returns></returns>
        public static List<tbl_Dictionary> StatusProcessList()
        {
            var model = CacheLayer.Get<List<tbl_Dictionary>>("statusProcessList");
            if (model == null)
            {
                model = _db.tbl_Dictionary.Where(p => p.DictionaryCategoryId == 17).Where(p => p.IsDelete == false).ToList();
                CacheLayer.Add<List<tbl_Dictionary>>(model, "statusProcessList", 1);
            }

            return model;
        }

        /// <summary>
        /// tình trạng tour
        /// </summary>
        /// <returns></returns>
        public static List<tbl_Dictionary> StatusTourList()
        {
            var model = CacheLayer.Get<List<tbl_Dictionary>>("StatusTourList");
            if (model == null)
            {
                model = _db.tbl_Dictionary.Where(p => p.DictionaryCategoryId == 29).Where(p => p.IsDelete == false).ToList();
                CacheLayer.Add<List<tbl_Dictionary>>(model, "StatusTourList", 1);
            }

            return model;
        }

        /// <summary>
        /// vị trí
        /// </summary>
        /// <returns></returns>
        public static List<tbl_Dictionary> PositionList()
        {
            var model = CacheLayer.Get<List<tbl_Dictionary>>("positionList");
            if (model == null)
            {
                model = _db.tbl_Dictionary.Where(p => p.DictionaryCategoryId == 5).Where(p => p.IsDelete == false).ToList();
                CacheLayer.Add<List<tbl_Dictionary>>(model, "positionList", 1);
            }

            return model;
        }

        /// <summary>
        /// phòng ban
        /// </summary>
        /// <returns></returns>
        public static List<tbl_Dictionary> DepartmentList()
        {
            var model = CacheLayer.Get<List<tbl_Dictionary>>("departmentList");
            if (model == null)
            {
                model = _db.tbl_Dictionary.Where(p => p.DictionaryCategoryId == 6).Where(p => p.IsDelete == false).ToList();
                CacheLayer.Add<List<tbl_Dictionary>>(model, "departmentList", 1);
            }

            return model;
        }

        /// <summary>
        /// danh xưng
        /// </summary>
        /// <returns></returns>
        public static List<tbl_Dictionary> NameTypeList()
        {
            var model = CacheLayer.Get<List<tbl_Dictionary>>("nameTypeList");
            if (model == null)
            {
                model = _db.tbl_Dictionary.Where(p => p.DictionaryCategoryId == 7).Where(p => p.IsDelete == false).ToList();
                CacheLayer.Add<List<tbl_Dictionary>>(model, "nameTypeList", 1);
            }

            return model;
        }

        /// <summary>
        /// tình trạng hợp đồng
        /// </summary>
        /// <returns></returns>
        public static List<tbl_Dictionary> StatusContractList()
        {
            var model = CacheLayer.Get<List<tbl_Dictionary>>("statusContractList");
            if (model == null)
            {
                model = _db.tbl_Dictionary.Where(p => p.DictionaryCategoryId == 18 && p.IsDelete == false).OrderBy(p => p.Name).ToList();
                CacheLayer.Add<List<tbl_Dictionary>>(model, "statusContractList", 1);
            }

            return model;
        }

        /// <summary>
        /// bằng cấp
        /// </summary>
        /// <returns></returns>
        public static List<tbl_Dictionary> CertificateList()
        {
            var model = CacheLayer.Get<List<tbl_Dictionary>>("certificateList");
            if (model == null)
            {
                model = _db.tbl_Dictionary.Where(p => p.DictionaryCategoryId == 12).Where(p => p.IsDelete == false).ToList();
                CacheLayer.Add<List<tbl_Dictionary>>(model, "certificateList", 1);
            }

            return model;
        }

        /// <summary>
        /// dân tộc
        /// </summary>
        /// <returns></returns>
        public static List<tbl_Dictionary> NationList()
        {
            var model = CacheLayer.Get<List<tbl_Dictionary>>("nationList");
            if (model == null)
            {
                model = _db.tbl_Dictionary.Where(p => p.DictionaryCategoryId == 10).Where(p => p.IsDelete == false).ToList();
                CacheLayer.Add<List<tbl_Dictionary>>(model, "nationList", 1);
            }

            return model;
        }

        /// <summary>
        /// tôn giáo
        /// </summary>
        /// <returns></returns>
        public static List<tbl_Dictionary> ReligionList()
        {
            var model = CacheLayer.Get<List<tbl_Dictionary>>("religionList");
            if (model == null)
            {
                model = _db.tbl_Dictionary.Where(p => p.DictionaryCategoryId == 11).Where(p => p.IsDelete == false).ToList();
                CacheLayer.Add<List<tbl_Dictionary>>(model, "religionList", 1);
            }

            return model;
        }

        /// <summary>
        /// nhóm nhân viên
        /// </summary>
        /// <returns></returns>
        public static List<tbl_Dictionary> StaffGroupList()
        {
            var model = CacheLayer.Get<List<tbl_Dictionary>>("staffGroupList");
            if (model == null)
            {
                model = _db.tbl_Dictionary.Where(p => p.DictionaryCategoryId == 16).Where(p => p.IsDelete == false).ToList();
                CacheLayer.Add<List<tbl_Dictionary>>(model, "staffGroupList", 1);
            }

            return model;
        }

        /// <summary>
        /// loại tiền
        /// </summary>
        /// <returns></returns>
        public static List<tbl_Dictionary> CurrencyList()
        {
            var model = CacheLayer.Get<List<tbl_Dictionary>>("currencyList");
            if (model == null)
            {
                model = _db.tbl_Dictionary.Where(p => p.DictionaryCategoryId == 24).Where(p => p.IsDelete == false).ToList();
                CacheLayer.Add<List<tbl_Dictionary>>(model, "currencyList", 1);
            }

            return model;
        }

        /// <summary>
        /// danh sách trụ sở chi nhánh
        /// </summary>
        /// <returns></returns>
        public static List<tbl_Headquater> HeadquarterList()
        {
            var model = CacheLayer.Get<List<tbl_Headquater>>("headquarterList");
            if (model == null)
            {
                model = _db.tbl_Headquater.AsEnumerable().Where(p => p.IsDelete == false).Select(p => new tbl_Headquater { Id = p.Id, ShortName = p.ShortName }).ToList();
                CacheLayer.Add<List<tbl_Headquater>>(model, "headquarterList", 1);
            }

            return model;
        }

        /// <summary>
        /// dịch vụ
        /// </summary>
        /// <returns></returns>
        public static List<tbl_Dictionary> ServiceList()
        {
            var model = CacheLayer.Get<List<tbl_Dictionary>>("serviceList");
            if (model == null)
            {
                model = _db.tbl_Dictionary.AsEnumerable().Where(p => p.IsDelete == false).Where(p => p.DictionaryCategoryId == 13).Select(p => new tbl_Dictionary { Id = p.Id, Name = p.Name }).ToList();
                CacheLayer.Add<List<tbl_Dictionary>>(model, "serviceList", 1);
            }

            return model;
        }

        /// <summary>
        /// loại visa
        /// </summary>
        /// <returns></returns>
        public static List<tbl_Dictionary> CategoryVisaList()
        {
            var model = CacheLayer.Get<List<tbl_Dictionary>>("categoryVisaList");
            if (model == null)
            {
                model = _db.tbl_Dictionary.AsEnumerable().Where(p => p.IsDelete == false).Where(p => p.DictionaryCategoryId == 15).Select(p => new tbl_Dictionary { Id = p.Id, Name = p.Name }).ToList();
                CacheLayer.Add<List<tbl_Dictionary>>(model, "categoryVisaList", 1);
            }

            return model;
        }

        /// <summary>
        /// tour
        /// </summary>
        /// <returns></returns>
        public static List<tbl_Tour> TourList()
        {
            var model = CacheLayer.Get<List<tbl_Tour>>("tourList");
            if (model == null)
            {
                model = _db.tbl_Tour.AsEnumerable().Where(p => p.IsDelete == false)
                    .Select(p => new tbl_Tour { Id = p.Id, Name = p.Name, Code = p.Code }).ToList();
                CacheLayer.Add<List<tbl_Tour>>(model, "tourList", 1);
            }

            return model;
        }

        /// <summary>
        /// tour
        /// </summary>
        /// <returns></returns>
        public static List<tbl_Quotation> QuotationList()
        {
            var model = CacheLayer.Get<List<tbl_Quotation>>("quotationList");
            if (model == null)
            {
                model = _db.tbl_Quotation.AsEnumerable().Where(p => p.IsDelete == false)
                    .Select(p => new tbl_Quotation { Id = p.Id, Code = p.Code }).ToList();
                CacheLayer.Add<List<tbl_Quotation>>(model, "quotationList", 1);
            }

            return model;
        }

        /// <summary>
        /// new code staff
        /// </summary>
        /// <returns></returns>
        public static string NewCodeStaff()
        {
            var staf = _db.tbl_Staff.AsEnumerable().Last();
            string num = staf.Code.Substring(2);
            int codenum = Int32.Parse(num);
            codenum++;
            string newcode = "NV" + codenum.ToString("D4");
            return newcode;
        }

        /// <summary>
        /// new code customer Personal
        /// </summary>
        /// <returns></returns>
        public static string NewCodeCustomerPersonal()
        {
            _db = new DataContext();
            var staf = _db.tbl_Customer.AsEnumerable().Where(c => c.CustomerType == CustomerType.Personal && c.IsTemp == false).Last();
            string num = staf.Code.Substring(3);
            string codechar = staf.Code.Substring(2, 1);
            string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            int i = alphabet.IndexOf(codechar);
            int codenum = Int32.Parse(num);
            codenum++;
            if (codenum == 100000)
            {
                codenum = 1;
                codechar = alphabet[i++].ToString();
            }
            string newcode = "KH" + codechar + codenum.ToString("D5");
            return newcode;
        }

        /// <summary>
        /// danh sách đối tác khách sạn
        /// </summary>
        /// <returns></returns>
        public static List<tbl_Partner> HotelList()
        {
            var model = CacheLayer.Get<List<tbl_Partner>>("hotelList");
            if (model == null)
            {
                model = _db.tbl_Partner.AsEnumerable().Where(p => p.IsDelete == false).Where(p => p.DictionaryId == 1048)
                    .Select(p => new tbl_Partner { Id = p.Id, Name = p.Name, Code = p.Code }).ToList();
                CacheLayer.Add<List<tbl_Partner>>(model, "hotelList", 1);
            }

            return model;
        }

        

        /// <summary>
        /// danh sách đối tác nhà hàng
        /// </summary>
        /// <returns></returns>
        public static List<tbl_Partner> RestaurantList()
        {
            var model = CacheLayer.Get<List<tbl_Partner>>("restaurantList");
            if (model == null)
            {
                model = _db.tbl_Partner.AsEnumerable().Where(p => p.IsDelete == false).Where(p => p.DictionaryId == 1047)
                    .Select(p => new tbl_Partner { Id = p.Id, Name = p.Name, Code = p.Code }).ToList();
                CacheLayer.Add<List<tbl_Partner>>(model, "restaurantList", 1);
            }

            return model;
        }

        /// <summary>
        /// danh sách đối tác vé máy bay
        /// </summary>
        /// <returns></returns>
        public static List<tbl_Partner> PlaneList()
        {
            var model = CacheLayer.Get<List<tbl_Partner>>("planeList");
            if (model == null)
            {
                model = _db.tbl_Partner.AsEnumerable().Where(p => p.IsDelete == false).Where(p => p.DictionaryId == 1049)
                    .Select(p => new tbl_Partner { Id = p.Id, Name = p.Name, Code = p.Code }).ToList();
                CacheLayer.Add<List<tbl_Partner>>(model, "planeList", 1);
            }

            return model;
        }
        /// <summary>
        /// danh sách tiêu chí đánh giá
        /// </summary>
        /// <returns></returns>
        public static List<tbl_EvaluationCriteria> EvaluationCriteriaList()
        {
            var model = CacheLayer.Get<List<tbl_EvaluationCriteria>>("EvaluationCriteriaList");
            if (model == null)
            {
                model = _db.tbl_EvaluationCriteria.AsEnumerable().Where(p => p.IsDelete == false)
                    .Select(p => new tbl_EvaluationCriteria { Id = p.Id, Name = p.Name }).ToList();
                CacheLayer.Add<List<tbl_EvaluationCriteria>>(model, "EvaluationCriteriaList", 1);
            }

            return model;
        }
        /// <summary>
        /// danh sách vùng miền
        /// </summary>
        /// <returns></returns>
        public static List<TagsViewModel> AreaList()
        {
            var model = CacheLayer.Get<List<TagsViewModel>>("AreaList");
            if (model == null)
            {
                model = _db.tbl_Tags.Where(p => p.ParentId == 11 && p.TypeTag == 4 && p.IsDelete == false)
                    .Select(p => new TagsViewModel
                    {
                        Id = p.Id,
                        Tags = p.Tag
                    }).ToList();
                CacheLayer.Add<List<TagsViewModel>>(model, "AreaList", 1);
            }

            return model;
        }
        /// <summary>
        /// danh sách đối tác vận chuyển
        /// </summary>
        /// <returns></returns>
        public static List<tbl_Partner> TransportList()
        {
            var model = CacheLayer.Get<List<tbl_Partner>>("transportList");
            if (model == null)
            {
                model = _db.tbl_Partner.AsEnumerable().Where(p => p.IsDelete == false).Where(p => p.DictionaryId == 1050).Select(p => new tbl_Partner { Id = p.Id, Name = p.Name, Code = p.Code }).ToList();
                CacheLayer.Add<List<tbl_Partner>>(model, "transportList", 1);
            }

            return model;
        }

        /// <summary>
        /// danh sách đối tác sự kiện
        /// </summary>
        /// <returns></returns>
        public static List<tbl_Partner> EventList()
        {
            var model = CacheLayer.Get<List<tbl_Partner>>("eventList");
            if (model == null)
            {
                model = _db.tbl_Partner.AsEnumerable().Where(p => p.IsDelete == false).Where(p => p.DictionaryId == 1051).Select(p => new tbl_Partner { Id = p.Id, Name = p.Name, Code = p.Code }).ToList();
                CacheLayer.Add<List<tbl_Partner>>(model, "eventList", 1);
            }

            return model;
        }

        public static tbl_Partner getPartnerByID(int id)
        {
            var model = _db.tbl_Partner.AsEnumerable().Where(p => p.Id == id && p.IsDelete == false).FirstOrDefault();
            return model; 
        }


        /// <summary>
        /// danh sách đối tác landtour
        /// </summary>
        /// <returns></returns>
        public static List<tbl_Partner> LandtourList()
        {
            var model = CacheLayer.Get<List<tbl_Partner>>("landtourList");
            if (model == null)
            {
                model = _db.tbl_Partner.AsEnumerable().Where(p => p.IsDelete == false).Where(p => p.DictionaryId == 1224)
                    .Select(p => new tbl_Partner { Id = p.Id, Name = p.Name, Code = p.Code }).ToList();
                CacheLayer.Add<List<tbl_Partner>>(model, "landtourList", 1);
            }

            return model;
        }

        /// <summary>
        /// danh sách các đối tác khác
        /// </summary>
        /// <returns></returns>
        public static List<tbl_Partner> OtherList()
        {
            var model = CacheLayer.Get<List<tbl_Partner>>("otherList");
            if (model == null)
            {
                model = _db.tbl_Partner.AsEnumerable().Where(p => p.IsDelete == false).Where(p => p.DictionaryId == 1052).Select(p => new tbl_Partner { Id = p.Id, Name = p.Name, Code = p.Code }).ToList();
                CacheLayer.Add<List<tbl_Partner>>(model, "otherList", 1);
            }

            return model;
        }

        /// <summary>
        /// danh sách các tag theo type
        /// </summary>
        /// <returns></returns>
        public static List<tbl_Tags> LoadTagsByType(int id)
        {
            var model = CacheLayer.Get<List<tbl_Tags>>("loadTagsByType" + id);
            if (model == null)
            {
                model = _db.tbl_Tags.AsEnumerable().Where(c => c.IsDelete == false && c.TypeTag == id).Select(p => new tbl_Tags { Id = p.Id, Tag = p.Tag }).ToList();
                CacheLayer.Add<List<tbl_Tags>>(model, "loadTagsByType" + id, 1);
            }
            return model;
        }

        /// <summary>
        /// danh sách các function
        /// </summary>
        /// <returns></returns>
        public static List<tbl_Function> FunctionList()
        {
            var model = CacheLayer.Get<List<tbl_Function>>("functionList");
            if (model == null)
            {
                model = _db.tbl_Function.AsEnumerable().Where(c => c.IsDelete == false).Select(p => new tbl_Function { Id = p.Id, Name = p.Name }).ToList();
                CacheLayer.Add<List<tbl_Function>>(model, "functionList", 1);
            }
            return model;
        }

        /// <summary>
        /// danh sách module vs form
        /// </summary>
        /// <returns></returns>
        public static List<tbl_Module> ModuleFormList()
        {
            var model = CacheLayer.Get<List<tbl_Module>>("moduleformlist");
            if (model == null)
            {
                model = _db.tbl_Module.AsEnumerable().Select(p => new tbl_Module
                {
                    Id = p.Id,
                    Name = p.Name
                }).ToList();
                foreach (var item in model)
                {
                    item.tbl_Form = _db.tbl_Form.AsEnumerable().Where(p => p.IsDelete == false).Where(c => c.ModuleId == item.Id).Select(c => new tbl_Form
                    {
                        Id = c.Id,
                        Name = c.Name
                    }).ToList();
                }
                CacheLayer.Add<List<tbl_Module>>(model, "moduleformlist", 1);
            }
            return model;
        }

        /// <summary>
        /// danh sách quyền truy cập
        /// </summary>
        /// <returns></returns>
        public static List<tbl_ShowDataBy> ShowDataByList()
        {
            var model = CacheLayer.Get<List<tbl_ShowDataBy>>("showdatabylist");
            if (model == null)
            {
                model = _db.tbl_ShowDataBy.AsEnumerable().ToList();
                CacheLayer.Add<List<tbl_ShowDataBy>>(model, "showdatabylist", 1);
            }
            return model;
        }

        /// <summary>
        /// ....
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool TourUpdate(int id)
        {
            _db = new DataContext();
            return _db.tbl_Tour.AsEnumerable().Where(c => c.Id == id).Select(c => c.IsUpdate).Single();
        }

        /// <summary>
        /// danh sách Công ty 
        /// </summary>
        /// <returns></returns>
        public static List<tbl_Customer> CompanyList()
        {
            var model = CacheLayer.Get<List<tbl_Customer>>("companylist");
            if (model == null)
            {
                model = _db.tbl_Customer.AsEnumerable().Where(p => p.IsDelete == false && p.CustomerType == CustomerType.Organization)
                    .Select(p => new tbl_Customer
                    {
                        Id = p.Id,
                        FullName = p.FullName,
                        Code = p.Code
                    }).ToList();
                CacheLayer.Add<List<tbl_Customer>>(model, "companylist", 1);
            }
            return model;
        }

        /// <summary>
        /// danh sách khách hàng cá nhân
        /// </summary>
        /// <returns></returns>
        public static List<tbl_Customer> PersonalList()
        {
            var model = CacheLayer.Get<List<tbl_Customer>>("personalList");
            if (model == null)
            {
                model = _db.tbl_Customer.AsEnumerable().Where(p => p.IsDelete == false && p.CustomerType == CustomerType.Personal && p.IsTemp == false)
                    .Select(p => new tbl_Customer
                    {
                        Id = p.Id,
                        FullName = p.FullName,
                        Code = p.Code
                    }).ToList();
                CacheLayer.Add<List<tbl_Customer>>(model, "personalList", 1);
            }
            return model;
        }

        /// <summary>
        /// load hướng dẫn viên
        /// </summary>
        /// <param name="tourId"></param>
        /// <returns></returns>
        public static string StaffPermission(int tourId)
        {
            string kq = "";
            var items = _db.tbl_TourGuide.Where(p => p.TourId == tourId).ToList();
            if (items.Count() > 0)
            {
                foreach (var i in items)
                {
                    kq += i.tbl_Staff.FullName + "<br/>";
                }
            }
            return kq;
        }

        /// <summary>
        /// danh sách thẻ thành viên
        /// </summary>
        /// <returns></returns>
        public static List<DictionaryViewModel> MemberCardList()
        {
            var model = CacheLayer.Get<List<DictionaryViewModel>>("MemberCardList");
            if (model == null)
            {
                model = _db.tbl_MemberCard.Where(p => p.IsDelete == false)
                    .Select(p => new DictionaryViewModel
                    {
                        Id = p.Id,
                        Name = p.Name + "(" + p.MinValue + " - " + p.MaxValue + ")"
                    }).ToList();
                CacheLayer.Add<List<DictionaryViewModel>>(model, "MemberCardList", 1);
            }

            return model;
        }


        /// <summary>
        /// danh sách loại vé
        /// </summary>
        /// <returns></returns>
        public static List<DictionaryViewModel> TicketTypeList()
        {
            var model = CacheLayer.Get<List<DictionaryViewModel>>("TicketTypeList");
            if (model == null)
            {
                model = _db.tbl_Dictionary.Where(p => p.DictionaryCategoryId == 27).Where(p => p.IsDelete == false).Select(p => new DictionaryViewModel
                {
                    Id = p.Id,
                    Name = p.Name
                }).ToList();
                CacheLayer.Add<List<DictionaryViewModel>>(model, "TicketTypeList", 1);
            }

            return model;
        }

        /// <summary>
        /// danh sách tình trạng vé
        /// </summary>
        /// <returns></returns>
        public static List<DictionaryViewModel> TicketStatusList()
        {
            var model = CacheLayer.Get<List<DictionaryViewModel>>("TicketStatusList");
            if (model == null)
            {
                model = _db.tbl_Dictionary.Where(p => p.DictionaryCategoryId == 28).Where(p => p.IsDelete == false).Select(p => new DictionaryViewModel
                {
                    Id = p.Id,
                    Name = p.Name
                }).ToList();
                CacheLayer.Add<List<DictionaryViewModel>>(model, "TicketStatusList", 1);
            }

            return model;
        }

        /// <summary>
        /// danh sách phân loại mail
        /// </summary>
        /// <returns></returns>
        public static List<tbl_Dictionary> MailCategory()
        {
            var model = CacheLayer.Get<List<tbl_Dictionary>>("mailCategory");
            if (model == null)
            {
                model = _db.tbl_MailCategory.AsEnumerable().Where(p => p.IsDelete == false)
                    .Select(p => new tbl_Dictionary { Id = p.Id, Name = p.CateName }).ToList();
                CacheLayer.Add<List<tbl_Dictionary>>(model, "mailCategory", 1);
            }

            return model;
        }

        /// <summary>
        /// danh sách mail config 
        /// </summary>
        /// <returns></returns>
        public static List<tbl_Dictionary> MailConfigList()
        {
            var model = CacheLayer.Get<List<tbl_Dictionary>>("mailConfigList");
            if (model == null)
            {
                model = _db.tbl_MailConfig.AsEnumerable().Where(p => p.IsDelete == false)
                    .Select(p => new tbl_Dictionary { Id = p.Id, Name = p.Email }).ToList();
                CacheLayer.Add<List<tbl_Dictionary>>(model, "mailConfigList", 1);
            }

            return model;
        }

        /// <summary>
        /// danh sách mail receive 
        /// </summary>
        /// <returns></returns>
        public static List<tbl_MailReceives> MailReceiveList()
        {
            var model = CacheLayer.Get<List<tbl_MailReceives>>("mailReceiveList");
            if (model == null)
            {
                model = _db.tbl_MailReceives.AsEnumerable().Where(p => p.IsDelete == false).ToList();
                CacheLayer.Add<List<tbl_MailReceives>>(model, "mailReceiveList", 1);
            }

            return model;
        }

        /// <summary>
        /// danh sách kỳ báo cáo
        /// </summary>
        private static string[] strSource = new string[] {
                "-- Tất cả --", "Hôm nay", "Tuần này", "Đầu tuần đến hiện tại", "Tháng này", "Đầu tháng đến hiện tại",
                "Quý này", "Đầu quý đến hiện tại", "Năm này", "Đầu năm đến hiện tại", "Tháng 1",
                "Tháng 2", "Tháng 3", "Tháng 4", "Tháng 5", "Tháng 6", "Tháng 7", "Tháng 8", "Tháng 9",
                "Tháng 10", "Tháng 11", "Tháng 12", "Quý I", "Quý II", "Quý III", "Quý IV", "Tuần trước",
                "Tháng trước", "Quý trước", "Năm trước", "Tuần sau", "Bốn tuần sau", "Tháng sau", "Quý sau",
                "Năm sau"
            };

        public static List<string> KyBaoCaoList()
        {
            var list = new List<string>();
            foreach (string str in strSource)
            {
                list.Add(str);
            }

            return list;
        }

        private static int FirstMonth(int month)
        {
            if (month <= 3)
                return 1;
            else if (month <= 6)
                return 4;
            else if (month <= 9)
                return 7;
            else
                return 10;
        }

        public static string GetDate(int index)
        {
            DateTime dateNow = DateTime.Now;
            DateTime dateFrom = new DateTime();
            DateTime dateTo = new DateTime();
            switch (index)
            {
                case 0: //Tu chon
                    dateFrom = dateNow.AddYears(-2);
                    dateTo = new DateTime(dateNow.Year, 12, 31);
                    break;
                case 1: //Ngay nay
                    dateFrom = dateNow;
                    dateTo = dateNow;
                    break;
                case 2: //Tuan nay
                    dateFrom = dateNow.AddDays(1 - (int)dateNow.DayOfWeek);
                    dateTo = dateNow.AddDays(7 - (int)dateNow.DayOfWeek);
                    break;
                case 3: //Dau tuan den hien tai
                    dateFrom = dateNow.AddDays(1 - (int)dateNow.DayOfWeek);
                    dateTo = dateNow;
                    break;
                case 4: //Thang nay
                    dateFrom = new DateTime(dateNow.Year, dateNow.Month, 1);
                    dateTo = new DateTime(dateNow.Year, dateNow.Month, 1).AddMonths(1).AddDays(-1);
                    break;
                case 5: //Dau thang den hien tai
                    dateFrom = new DateTime(dateNow.Year, dateNow.Month, 1);
                    dateTo = dateNow;
                    break;
                case 6: //Quy nay
                    dateFrom = new DateTime(dateNow.Year, FirstMonth(dateNow.Month), 1);
                    dateTo = new DateTime(dateNow.Year,
                        FirstMonth(dateNow.Month) + 2, 1).AddMonths(1).AddDays(-1);
                    break;
                case 7: //Dau quy den hien tai
                    dateFrom = new DateTime(dateNow.Year, FirstMonth(dateNow.Month), 1);
                    dateTo = dateNow;
                    break;
                case 8: //Nam nay
                    dateFrom = new DateTime(dateNow.Year, 1, 1);
                    dateTo = new DateTime(dateNow.Year, 12, 31);
                    break;
                case 9: //Dau nam den hien tai
                    dateFrom = new DateTime(dateNow.Year, 1, 1);
                    dateTo = dateNow;
                    break;
                case 10: //Thang 1
                    dateFrom = new DateTime(dateNow.Year, 1, 1);
                    dateTo = new DateTime(dateNow.Year, 2, 1).AddDays(-1);
                    break;
                case 11: //Thang 2
                    dateFrom = new DateTime(dateNow.Year, 2, 1);
                    dateTo = new DateTime(dateNow.Year, 3, 1).AddDays(-1);
                    break;
                case 12: //Thang 3
                    dateFrom = new DateTime(dateNow.Year, 3, 1);
                    dateTo = new DateTime(dateNow.Year, 4, 1).AddDays(-1);
                    break;
                case 13: //Thang 4
                    dateFrom = new DateTime(dateNow.Year, 4, 1);
                    dateTo = new DateTime(dateNow.Year, 5, 1).AddDays(-1);
                    break;
                case 14: //Thang 5
                    dateFrom = new DateTime(dateNow.Year, 5, 1);
                    dateTo = new DateTime(dateNow.Year, 6, 1).AddDays(-1);
                    break;
                case 15: //Thang 6
                    dateFrom = new DateTime(dateNow.Year, 6, 1);
                    dateTo = new DateTime(dateNow.Year, 7, 1).AddDays(-1);
                    break;
                case 16: //Thang 7
                    dateFrom = new DateTime(dateNow.Year, 7, 1);
                    dateTo = new DateTime(dateNow.Year, 8, 1).AddDays(-1);
                    break;
                case 17: //Thang 8
                    dateFrom = new DateTime(dateNow.Year, 8, 1);
                    dateTo = new DateTime(dateNow.Year, 9, 1).AddDays(-1);
                    break;
                case 18: //Thang 9
                    dateFrom = new DateTime(dateNow.Year, 9, 1);
                    dateTo = new DateTime(dateNow.Year, 10, 1).AddDays(-1);
                    break;
                case 19: //Thang 10
                    dateFrom = new DateTime(dateNow.Year, 10, 1);
                    dateTo = new DateTime(dateNow.Year, 11, 1).AddDays(-1);
                    break;
                case 20: //Thang 11
                    dateFrom = new DateTime(dateNow.Year, 11, 1);
                    dateTo = new DateTime(dateNow.Year, 12, 1).AddDays(-1);
                    break;
                case 21: //Thang 12
                    dateFrom = new DateTime(dateNow.Year, 12, 1);
                    dateTo = new DateTime(dateNow.Year, 12, 31);
                    break;
                case 22: //Quy I
                    dateFrom = new DateTime(dateNow.Year, 1, 1);
                    dateTo = new DateTime(dateNow.Year, 4, 1).AddDays(-1);
                    break;
                case 23: //Quy II
                    dateFrom = new DateTime(dateNow.Year, 4, 1);
                    dateTo = new DateTime(dateNow.Year, 7, 1).AddDays(-1);
                    break;
                case 24: //Quy III
                    dateFrom = new DateTime(dateNow.Year, 7, 1);
                    dateTo = new DateTime(dateNow.Year, 10, 1).AddDays(-1);
                    break;
                case 25: //Quy IV
                    dateFrom = new DateTime(dateNow.Year, 10, 1);
                    dateTo = new DateTime(dateNow.Year, 12, 31);
                    break;
                case 26: //Tuan truoc
                    dateFrom = dateNow.AddDays(-(int)dateNow.DayOfWeek - 6);
                    dateTo = dateNow.AddDays(-(int)dateNow.DayOfWeek);
                    break;
                case 27: //Thang truoc
                    dateFrom = new DateTime(dateNow.Year, dateNow.Month, 1).AddMonths(-1);
                    dateTo = new DateTime(dateNow.Year, dateNow.Month, 1).AddDays(-1);
                    break;
                case 28: //Quy truoc
                    dateFrom = new DateTime(dateNow.Year, FirstMonth(dateNow.Month), 1).AddMonths(-3);
                    dateTo = new DateTime(dateNow.Year, FirstMonth(dateNow.Month), 1).AddDays(-1);
                    break;
                case 29: //Nam truoc
                    dateFrom = new DateTime(dateNow.Year - 1, 1, 1);
                    dateTo = new DateTime(dateNow.Year - 1, 12, 31);
                    break;
                case 30: //Tuan sau
                    dateFrom = dateNow.AddDays(8 - (int)dateNow.DayOfWeek);
                    dateTo = dateNow.AddDays(14 - (int)dateNow.DayOfWeek);
                    break;
                case 31: //Bon tuan sau
                    dateFrom = dateNow;
                    dateTo = dateNow.AddDays(28);
                    break;
                case 32: //Thang sau
                    dateFrom = new DateTime(dateNow.Year, dateNow.Month, 1).AddMonths(1);
                    dateTo = new DateTime(dateNow.Year, dateNow.Month, 1).AddMonths(2).AddDays(-1);
                    break;
                case 33: //Quy sau
                    DateTime QuySau = new DateTime(dateNow.Year, FirstMonth(dateNow.Month) + 3, 1);
                    dateFrom = QuySau;
                    dateTo = QuySau.AddMonths(3).AddDays(-1);
                    break;
                case 34: //Nam sau
                    dateFrom = new DateTime(dateNow.Year + 1, 1, 1);
                    dateTo = new DateTime(dateNow.Year + 1, 12, 31);
                    break;
            }

            string kq = @"<div class='col-md-3'><label>Từ ngày</label><input type='date' value='" + dateFrom.ToString("yyyy-MM-dd") + "' id='txtStartDate' class='form-control' name='tungay' required />" +
                    "</div><div class='col-md-3'><label>Đến ngày</label><input type='date' value='" + dateTo.ToString("yyyy-MM-dd") + "'id='txtEndDate' class='form-control' name='denngay' required />" +
                    "</div>";

            return kq;
        }

        public static string GetDateModal(int index)
        {
            DateTime dateNow = DateTime.Now;
            DateTime dateFrom = new DateTime();
            DateTime dateTo = new DateTime();
            switch (index)
            {
                case 0: //Tu chon
                    dateFrom = dateNow.AddYears(-2);
                    dateTo = new DateTime(dateNow.Year, 12, 31);
                    break;
                case 1: //Ngay nay
                    dateFrom = dateNow;
                    dateTo = dateNow;
                    break;
                case 2: //Tuan nay
                    dateFrom = dateNow.AddDays(1 - (int)dateNow.DayOfWeek);
                    dateTo = dateNow.AddDays(7 - (int)dateNow.DayOfWeek);
                    break;
                case 3: //Dau tuan den hien tai
                    dateFrom = dateNow.AddDays(1 - (int)dateNow.DayOfWeek);
                    dateTo = dateNow;
                    break;
                case 4: //Thang nay
                    dateFrom = new DateTime(dateNow.Year, dateNow.Month, 1);
                    dateTo = new DateTime(dateNow.Year, dateNow.Month, 1).AddMonths(1).AddDays(-1);
                    break;
                case 5: //Dau thang den hien tai
                    dateFrom = new DateTime(dateNow.Year, dateNow.Month, 1);
                    dateTo = dateNow;
                    break;
                case 6: //Quy nay
                    dateFrom = new DateTime(dateNow.Year, FirstMonth(dateNow.Month), 1);
                    dateTo = new DateTime(dateNow.Year,
                        FirstMonth(dateNow.Month) + 2, 1).AddMonths(1).AddDays(-1);
                    break;
                case 7: //Dau quy den hien tai
                    dateFrom = new DateTime(dateNow.Year, FirstMonth(dateNow.Month), 1);
                    dateTo = dateNow;
                    break;
                case 8: //Nam nay
                    dateFrom = new DateTime(dateNow.Year, 1, 1);
                    dateTo = new DateTime(dateNow.Year, 12, 31);
                    break;
                case 9: //Dau nam den hien tai
                    dateFrom = new DateTime(dateNow.Year, 1, 1);
                    dateTo = dateNow;
                    break;
                case 10: //Thang 1
                    dateFrom = new DateTime(dateNow.Year, 1, 1);
                    dateTo = new DateTime(dateNow.Year, 2, 1).AddDays(-1);
                    break;
                case 11: //Thang 2
                    dateFrom = new DateTime(dateNow.Year, 2, 1);
                    dateTo = new DateTime(dateNow.Year, 3, 1).AddDays(-1);
                    break;
                case 12: //Thang 3
                    dateFrom = new DateTime(dateNow.Year, 3, 1);
                    dateTo = new DateTime(dateNow.Year, 4, 1).AddDays(-1);
                    break;
                case 13: //Thang 4
                    dateFrom = new DateTime(dateNow.Year, 4, 1);
                    dateTo = new DateTime(dateNow.Year, 5, 1).AddDays(-1);
                    break;
                case 14: //Thang 5
                    dateFrom = new DateTime(dateNow.Year, 5, 1);
                    dateTo = new DateTime(dateNow.Year, 6, 1).AddDays(-1);
                    break;
                case 15: //Thang 6
                    dateFrom = new DateTime(dateNow.Year, 6, 1);
                    dateTo = new DateTime(dateNow.Year, 7, 1).AddDays(-1);
                    break;
                case 16: //Thang 7
                    dateFrom = new DateTime(dateNow.Year, 7, 1);
                    dateTo = new DateTime(dateNow.Year, 8, 1).AddDays(-1);
                    break;
                case 17: //Thang 8
                    dateFrom = new DateTime(dateNow.Year, 8, 1);
                    dateTo = new DateTime(dateNow.Year, 9, 1).AddDays(-1);
                    break;
                case 18: //Thang 9
                    dateFrom = new DateTime(dateNow.Year, 9, 1);
                    dateTo = new DateTime(dateNow.Year, 10, 1).AddDays(-1);
                    break;
                case 19: //Thang 10
                    dateFrom = new DateTime(dateNow.Year, 10, 1);
                    dateTo = new DateTime(dateNow.Year, 11, 1).AddDays(-1);
                    break;
                case 20: //Thang 11
                    dateFrom = new DateTime(dateNow.Year, 11, 1);
                    dateTo = new DateTime(dateNow.Year, 12, 1).AddDays(-1);
                    break;
                case 21: //Thang 12
                    dateFrom = new DateTime(dateNow.Year, 12, 1);
                    dateTo = new DateTime(dateNow.Year, 12, 31);
                    break;
                case 22: //Quy I
                    dateFrom = new DateTime(dateNow.Year, 1, 1);
                    dateTo = new DateTime(dateNow.Year, 4, 1).AddDays(-1);
                    break;
                case 23: //Quy II
                    dateFrom = new DateTime(dateNow.Year, 4, 1);
                    dateTo = new DateTime(dateNow.Year, 7, 1).AddDays(-1);
                    break;
                case 24: //Quy III
                    dateFrom = new DateTime(dateNow.Year, 7, 1);
                    dateTo = new DateTime(dateNow.Year, 10, 1).AddDays(-1);
                    break;
                case 25: //Quy IV
                    dateFrom = new DateTime(dateNow.Year, 10, 1);
                    dateTo = new DateTime(dateNow.Year, 12, 31);
                    break;
                case 26: //Tuan truoc
                    dateFrom = dateNow.AddDays(-(int)dateNow.DayOfWeek - 6);
                    dateTo = dateNow.AddDays(-(int)dateNow.DayOfWeek);
                    break;
                case 27: //Thang truoc
                    dateFrom = new DateTime(dateNow.Year, dateNow.Month, 1).AddMonths(-1);
                    dateTo = new DateTime(dateNow.Year, dateNow.Month, 1).AddDays(-1);
                    break;
                case 28: //Quy truoc
                    dateFrom = new DateTime(dateNow.Year, FirstMonth(dateNow.Month), 1).AddMonths(-3);
                    dateTo = new DateTime(dateNow.Year, FirstMonth(dateNow.Month), 1).AddDays(-1);
                    break;
                case 29: //Nam truoc
                    dateFrom = new DateTime(dateNow.Year - 1, 1, 1);
                    dateTo = new DateTime(dateNow.Year - 1, 12, 31);
                    break;
                case 30: //Tuan sau
                    dateFrom = dateNow.AddDays(8 - (int)dateNow.DayOfWeek);
                    dateTo = dateNow.AddDays(14 - (int)dateNow.DayOfWeek);
                    break;
                case 31: //Bon tuan sau
                    dateFrom = dateNow;
                    dateTo = dateNow.AddDays(28);
                    break;
                case 32: //Thang sau
                    dateFrom = new DateTime(dateNow.Year, dateNow.Month, 1).AddMonths(1);
                    dateTo = new DateTime(dateNow.Year, dateNow.Month, 1).AddMonths(2).AddDays(-1);
                    break;
                case 33: //Quy sau
                    DateTime QuySau = new DateTime(dateNow.Year, FirstMonth(dateNow.Month) + 3, 1);
                    dateFrom = QuySau;
                    dateTo = QuySau.AddMonths(3).AddDays(-1);
                    break;
                case 34: //Nam sau
                    dateFrom = new DateTime(dateNow.Year + 1, 1, 1);
                    dateTo = new DateTime(dateNow.Year + 1, 12, 31);
                    break;
            }

            string kq = @"<div class='form-group'>"
                        + "<label class='control-label col-lg-2 col-md-2'>Từ ngày</label>"
                        + "<div class='col-lg-10 col-md-10'>"
                        + "<input type='date' id='txtTuNgay' value='" + dateFrom.ToString("yyyy-MM-dd") + "' class='form-control' name='tungay' required />"
                        + "</div>"
                        + "</div>"
                        + "<div class='form-group'>"
                        + "<label class='control-label col-lg-2 col-md-2'>Đến ngày</label>"
                        + "<div class='col-lg-10 col-md-10'>"
                        + "<input type='date' id='txtDenNgay' value='" + dateTo.ToString("yyyy-MM-dd") + "' class='form-control' name='denngay' required />"
                        + "</div>"
                        + "</div>";

            return kq;
        }

        public static int GetModule(int formid)
        {
            return _db.tbl_Form.Find(formid).ModuleId ?? 0;
        }

        /// <summary>
        /// Tình trạng phiếu thu chi
        /// </summary>
        /// <returns></returns>
        public static List<tbl_Dictionary> BillStatusList()
        {
            var model = CacheLayer.Get<List<tbl_Dictionary>>("BillStatusList");
            if (model == null)
            {
                model = _db.tbl_Dictionary.Where(p => p.DictionaryCategoryId == 30).Where(p => p.IsDelete == false).ToList();
                CacheLayer.Add<List<tbl_Dictionary>>(model, "BillStatusList", 1);
            }

            return model;
        }

        /// <summary>
        /// hình thức thanh toán
        /// </summary>
        /// <returns></returns>
        public static List<tbl_Dictionary> PaymentMethodList()
        {
            var model = CacheLayer.Get<List<tbl_Dictionary>>("PaymentMethodList");
            if (model == null)
            {
                model = _db.tbl_Dictionary.Where(p => p.DictionaryCategoryId == 31).Where(p => p.IsDelete == false).ToList();
                CacheLayer.Add<List<tbl_Dictionary>>(model, "PaymentMethodList", 1);
            }

            return model;
        }
    }
}