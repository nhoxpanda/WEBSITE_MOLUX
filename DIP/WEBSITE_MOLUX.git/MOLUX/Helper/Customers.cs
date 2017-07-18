using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MOLUX.Models;

namespace MOLUX.Helper
{
    public static class Customers
    {
        private static BMSMoluxHongKongEntities _db = new BMSMoluxHongKongEntities();

        public static string GenerateCode()
        {
            var staf = _db.Customer.AsEnumerable().Where(p => p.Code.Contains("KHWEB/")).ToList();
            if (staf.Count() > 0 && staf.Last() != null)
            {
                string num = staf.Last().Code.Substring(6);
                int codenum = Int32.Parse(num);
                codenum++;
                string newcode = "KHWEB/" + codenum.ToString("D5");
                return newcode;
            }
            else
            {
                return "KHWEB/00001";
            }
        }

        public static string GenerateOrderCode()
        {
            var staf = _db.Sales_Order_Master.AsEnumerable().Where(p => p.Code.Contains("DHB/")).ToList();
            if (staf.Count() > 0 && staf.Last() != null)
            {
                string num = staf.Last().Code.Substring(4);
                int codenum = Int32.Parse(num);
                codenum++;
                string newcode = "DHB/" + codenum.ToString("D6");
                return newcode;
            }
            else
            {
                return "DHB/000001";
            }
        }

        public static List<Country> CountryList()
        {
            return _db.Country.ToList();
        }

        public static List<City> CityList(string code)
        {
            if (!string.IsNullOrEmpty(code))
            {
                return _db.City.Where(p => p.Country_Code == code).ToList();
            }
            else
            {
                return _db.City.ToList();
            }
        }

        public static string GetCityName(string code)
        {
            var check = _db.City.FirstOrDefault(p => p.Code == code);
            if (check != null)
            {
                return ", " + check.Name + ", ";
            }
            else
            {
                return ", ";
            }
        }

        public static string GetCountryName(string code)
        {
            var check = _db.Country.FirstOrDefault(p => p.Code == code);
            if (check != null)
            {
                return ", " + check.Name;
            }
            else
            {
                return "";
            }
        }

        public static string GetPaymentName(string code)
        {
            var check = _db.Payment_Method.FirstOrDefault(p => p.Code == code);
            if (check != null)
            {
                return check.Name;
            }
            else
            {
                return "";
            }
        }

        public static Customer GetCustomer(string code)
        {
            var check = _db.Customer.FirstOrDefault(p => p.Code == code);
            if (check != null)
            {
                return check;
            }
            else
            {
                return null;
            }
        }

        public static string BodyEmail(int id)
        {
            int i = 1;
            var order = _db.Sales_Order_Master.Find(id);
            var orderDetail = _db.Sales_Order_Detail.Where(p => p.Sales_Order_Code == order.Code).ToList();
            var customer = _db.Customer.FirstOrDefault(p => p.Code == order.Customer_Code);
            string detail = "";
            foreach (var item in orderDetail)
            {
                detail += "<tr>" +
                            "<td style='padding: 10px; border: 1px solid #ccc;'>" + i + "</td>" +
                            "<td style='padding: 10px; border: 1px solid #ccc;'>" + _db.Item.FirstOrDefault(p => p.Code == item.Item_Code).Name + "</td>" +
                            "<td style='text-align: right; padding: 10px; border: 1px solid #ccc;'>" + item.Qty + "</td>" +
                            "<td style='text-align: right; padding: 10px; border: 1px solid #ccc;'>" + string.Format("{0:0,0₫}", item.Rate).Replace(",", ".") + "</td>" +
                            "<td style='text-align: right;padding: 10px; border: 1px solid #ccc;'>" + string.Format("{0:0,0₫}", item.Amount).Replace(",", ".") + "</td>" +
                        "</tr>";
                i++;
            }

            string body = @"<table style='width: 100%'>
                <tr>
                    <th colspan='4' style='text-align: center; border: 1px solid #ccc; padding: 10px; background-color: #FFDD00'>
                        THÔNG TIN ĐƠN HÀNG
                    </th>
                </tr>
                <tr>
                    <th style='text-align: left; padding: 10px; border: 1px solid #ccc;'>Mã đơn hàng</th>
                    <td style='padding: 10px; border: 1px solid #ccc;'>" + order.Code + @"</td>
                    <th style='text-align: left; padding: 10px; border: 1px solid #ccc;'>Mã KH</th>
                    <td style='padding: 10px; border: 1px solid #ccc;'>" + order.Customer_Code + @"</td>
                </tr>
                <tr>
                    <th style='text-align: left; padding: 10px; border: 1px solid #ccc;'>Ngày đặt hàng</th>
                    <td style='padding: 10px; border: 1px solid #ccc;'>" + string.Format("{0:dd/MM/yyyy}", order.Created_Date) + @"</td>
                    <th style='text-align: left; padding: 10px; border: 1px solid #ccc;'>Họ tên</th>
                    <td style='padding: 10px; border: 1px solid #ccc;'>" + customer.Name + @"</td>
                </tr>
                <tr>
                    <th style='text-align: left; padding: 10px; border: 1px solid #ccc;'>Ngày giao hàng</th>
                    <td style='padding: 10px; border: 1px solid #ccc;'>" + string.Format("{0:dd/MM/yyyy}", order.Ship_Date) + @"</td>
                    <th style='text-align: left; padding: 10px; border: 1px solid #ccc;'>Điện thoại</th>
                    <td style='padding: 10px; border: 1px solid #ccc;'>" + order.Bill_Phone + @"</td>
                </tr>
                <tr>
                    <th style='text-align: left; padding: 10px; border: 1px solid #ccc;'>Hình thức thanh toán</th>
                    <td style='padding: 10px; border: 1px solid #ccc;'>" + _db.Payment_Method.FirstOrDefault(p => p.Code == order.Payment_Method_Code).Name + @"</td>
                    <th style='text-align: left; padding: 10px; border: 1px solid #ccc;'>Email</th>
                    <td style='padding: 10px; border: 1px solid #ccc;'>" + order.Bill_Email + @"</td>
                </tr>
                <tr>
                    <td style='padding: 10px; border: 1px solid #ccc;'></td>
                    <td style='padding: 10px; border: 1px solid #ccc;'></td>
                    <th style='text-align: left; padding: 10px; border: 1px solid #ccc;'>Địa chỉ</th>
                    <td colspan='3' style='padding: 10px; border: 1px solid #ccc;'>" + order.Code + Customers.GetCityName(customer.City_Code) + Customers.GetCountryName(customer.Country_Code) + @"</td>
                </tr>
                <tr>
                    <th style='text-align: left; padding: 10px; border: 1px solid #ccc;'>Ghi chú</th>
                    <td colspan='3' style='padding: 10px; border: 1px solid #ccc;'>" + order.Sale_Note + @"</td>
                </tr>
            </table>
            <table style='width: 100%'>
                <caption style='padding: 10px; background-color: #FFDD00'><strong>DANH SÁCH SẢN PHẨM</strong></caption>
                <thead>
                    <tr>
                        <th style='text-align: center; padding: 10px; border: 1px solid #ccc;'>STT</th>
                        <th style='text-align: center; padding: 10px; border: 1px solid #ccc;'>SẢN PHẨM</th>
                        <th style='text-align: center; padding: 10px; border: 1px solid #ccc;'>SỐ LƯỢNG</th>
                        <th style='text-align: center; padding: 10px; border: 1px solid #ccc;'>GIÁ BÁN</th>
                        <th style='text-align: center; padding: 10px; border: 1px solid #ccc;'>THÀNH TIỀN</th>
                    </tr>
                </thead>
                <tbody>
                    " + detail +
                    @"<tr>
                        <td colspan='4' style='text-align: right; padding: 10px; border: 1px solid #ccc;'><strong>Tổng cộng:</strong></td>
                        <td style='text-align: right; padding: 10px; border: 1px solid #ccc;'><strong>" + string.Format("{0:0,0₫}", orderDetail.Sum(p => p.Amount)).Replace(",", ".") + @"</strong></td>
                    </tr>
                </tbody>
            </table>";
            body += "<p>User và pass đăng nhập <a title='quản lý đơn hàng' href='/quan-ly-don-hang'>Quản lý đơn hàng</a>: </p>" +
                    "<p>Username: " + order.Bill_Email + "</p>" +
                    "<p>Password: " + order.Bill_Phone + "</p>";
            return body;
        }
    }
}