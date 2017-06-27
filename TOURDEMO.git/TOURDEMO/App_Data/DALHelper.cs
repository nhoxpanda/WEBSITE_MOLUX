using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using CRM.Core;
using Dapper;

namespace TOURDEMO.App_Data
{
    public static class DALHelper
    {
        private static IDbConnection _db = new SqlConnection("Data Source=103.27.239.106;Initial Catalog=Web_TOURDEMO;user id=Web_TOURDEMO_login;password=SIKU%$^#HJGF;Integrated Security=false;MultipleActiveResultSets=True");

        public static List<tbl_Customer> GetAllCustomer()
        {
            List<tbl_Customer> empList = _db.Query<tbl_Customer>("SELECT * FROM tbl_Customer WHERE IsDelete = 0").ToList();
            return empList;
        }

        public static tbl_Customer FindCustomer(int? id)
        {
            string query = "SELECT * FROM tbl_Customer WHERE Id = " + id + "";
            return _db.Query<tbl_Customer>(query).SingleOrDefault();
        }

        public static List<tbl_Tour> GetAllTour()
        {
            List<tbl_Tour> tourList = _db.Query<tbl_Tour>("SELECT * FROM tbl_Tour WHERE IsDelete = 0").ToList();
            return tourList;
        }

        public static tbl_Tour FindTour(int? id)
        {
            string query = "SELECT * FROM tbl_Tour WHERE Id = " + id + "";
            return _db.Query<tbl_Tour>(query).SingleOrDefault();
        }

        public static tbl_Staff FindStaff(int? id)
        {
            string query = "SELECT * FROM tbl_Staff WHERE Id = " + id + "";
            return _db.Query<tbl_Staff>(query).SingleOrDefault();
        }

        public static tbl_Dictionary FindDictionary(int? id)
        {
            string query = "SELECT * FROM tbl_Dictionary WHERE Id = " + id + "";
            return _db.Query<tbl_Dictionary>(query).SingleOrDefault();
        }
    }
}