using CRM.Core;
using CRM.Enum;
using CRM.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TOURDEMO.Utilities;

namespace TOURDEMO.Controllers
{
    public class DemoController : BaseController
    {
        //
        // GET: /Demo/

        #region Init

        private IGenericRepository<tbl_Customer> _customerRepository;
        private IGenericRepository<tbl_Partner> _partnerRepository;
        private IGenericRepository<tbl_Tags> _tagsRepository;
        private IGenericRepository<tbl_CustomerContact> _customerContactRepository;
        private IGenericRepository<tbl_CustomerVisa> _customerVisaRepository;
        private IGenericRepository<tbl_CustomerContactVisa> _customerContactVisaRepository;

        private IGenericRepository<tbl_Dictionary> _dictionaryRepository;
        private IGenericRepository<tbl_DocumentFile> _documentFileRepository;
        private IGenericRepository<tbl_UpdateHistory> _updateHistoryRepository;
        private IGenericRepository<tbl_ContactHistory> _contactHistoryRepository;
        private IGenericRepository<tbl_AppointmentHistory> _appointmentHistoryRepository;
        private IGenericRepository<tbl_Staff> _staffRepository;
        private DataContext _db;

        public DemoController(
            IGenericRepository<tbl_Partner> partnerRepository,
            IGenericRepository<tbl_Customer> customerRepository,
            IGenericRepository<tbl_Tags> tagsRepository,
            IGenericRepository<tbl_CustomerContact> customerContactRepository,
            IGenericRepository<tbl_CustomerVisa> customerVisaRepository,

            IGenericRepository<tbl_Dictionary> dictionaryRepository,
            IGenericRepository<tbl_CustomerContactVisa> customerContactVisaRepository,
            IGenericRepository<tbl_DocumentFile> documentFileRepository,
            IGenericRepository<tbl_UpdateHistory> updateHistoryRepository,
            IGenericRepository<tbl_ContactHistory> contactHistoryRepository,
            IGenericRepository<tbl_AppointmentHistory> appointmentHistoryRepository,
            IGenericRepository<tbl_Staff> staffRepository,
            IBaseRepository baseRepository)
            : base(baseRepository)
        {
            this._partnerRepository = partnerRepository;
            this._customerRepository = customerRepository;
            this._customerContactRepository = customerContactRepository;
            this._tagsRepository = tagsRepository;
            this._customerVisaRepository = customerVisaRepository;
            this._customerContactVisaRepository = customerContactVisaRepository;

            this._dictionaryRepository = dictionaryRepository;
            this._documentFileRepository = documentFileRepository;
            this._contactHistoryRepository = contactHistoryRepository;
            this._appointmentHistoryRepository = appointmentHistoryRepository;
            this._updateHistoryRepository = updateHistoryRepository;
            this._staffRepository = staffRepository;
            _db = new DataContext();
        }

        #endregion

        /// <summary>
        /// //////
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ReadFileOnline()
        {
            return View();
        }

        public async Task<ActionResult> DemoInsert()
        {
            var other = new tbl_Customer
                        {
                            Address = "tb",
                            CompanyEmail = "",
                            CreatedDate = DateTime.Now,
                            Director = "Vinh",
                            Phone = "",
                            FullName = "tnhh",
                            IsDelete = false,
                            IsTemp = true,
                            ModifiedDate = DateTime.Now,
                            ParentId = 0,
                            StaffId = clsPermission.GetUser().StaffID,
                            StaffManager = clsPermission.GetUser().StaffID,
                            TagsId = "22",
                            SubscribeSMS = true,
                            SubscribeEmail = true,
                            Code = "a",
                            CareerId = Convert.ToInt32(11),
                            CustomerType = CustomerType.Organization,
                            NameTypeId = 47,
                            PassportTagId = 11,
                            IdentityTagId = 11,

                        };
            await _customerRepository.Create(other);

            return View();
        }

    }
}
