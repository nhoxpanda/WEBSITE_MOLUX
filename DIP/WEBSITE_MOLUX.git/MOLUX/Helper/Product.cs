using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MOLUX.Models;

namespace MOLUX.Helper
{
    public static class Product
    {
        private static BMSMoluxHongKongEntities _db = new BMSMoluxHongKongEntities();

        public static List<web_getTop5ItemLevel1_Result> ProductLevel1(int id)
        {
            return _db.web_getTop5ItemLevel1(id).ToList();
        }
        public static List<Linh_getRowId_Result> GetRowID(int id)
        {
            return _db.Linh_getRowId(id).ToList();
        }
        public static List<Linh_ProductLever1_Result> Linh_ProductLevel1(int id)
        {
            return _db.Linh_ProductLever1(id).ToList();
        }
        public static List<web_getTop5ItemLevel2_Result> ProductLevel2(int id)
        {
            return _db.web_getTop5ItemLevel2(id).ToList();
        }

        public static List<web_getTop5ItemLevel3_Result> ProductLevel3(string code)
        {
            return _db.web_getTop5ItemLevel3(code).ToList();
        }

        public static Item GetProductById(int id)
        {
            return _db.Item.Find(id);
        }

        public static List<Payment_Method> OrderPayment()
        {
            return _db.Payment_Method.ToList();
        }

        public static Manufacturer GetManufacturerByCode(string code)
        {
            var model =  _db.Manufacturer.FirstOrDefault(p => p.Code == code);
            return model;
        }
    }
}