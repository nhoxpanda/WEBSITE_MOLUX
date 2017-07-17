using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRM_TTV.Models
{
    public class Paging
    {
        public static string Pagination(int totalRow, int? pageCurent, int? size)
        {
            int sumPage = (Int32)(totalRow / size);

            //if (sumPage <= 1)
            //    return "";

            if ((totalRow % size) <= 4 && (totalRow % size) != 0) { sumPage++; }
            //int start = pageCurent == sumPage ? sumPage < 6 ? 1 : (Int32)pageCurent - 5 : pageCurent > 3 ? (Int32)pageCurent - 2 : 1;
            int start = pageCurent == sumPage ? (Int32)pageCurent - 5 : pageCurent > 3 ? (Int32)pageCurent - 2 : 1;
            start = start < 1 ? 1 : start;
             
            string html = @"<div class='row paddingtop10'>
                                <div class='col-md-6 col-sm-6'>
                                    <label>
                                        Hiển thị
                                        <select class='form-control input-sm input-xsmall input-inline' onchange='viewSizeInfo(this.value)' id='page-row'>";
                                            if (size != 10 && size != 30 && size != 50)
                                                html += string.Format("<option value='{0}'>{0}</option>",size);
                                            html += @"<option value = '10' > 10 </option>
                                            <option value='30'>30</option>
                                            <option value = '50' > 50 </option>
                                        </select> trong " + totalRow + @" kết quả
                                    </label>
                                </div>
                                <div class='col-md-6 col-sm-6'>
                                    <div class='dataTables_paginate paging_bootstrap_full_number pull-right'>
                                        <ul class='pagination' style='visibility: visible;'>
                                            <li ><a title='First' onclick='viewPageInfo(1)'><i class='fa fa-angle-double-left'></i></a></li>";
                                        
                                                    for (int i = start; i <= sumPage; i++)
                                                    {
                                                        if (sumPage <= 6)
                                                        {
                                                            html += string.Format("<li id='page-{0}'><a onclick='viewPageInfo({0})'>{0}</a></li>", i);
                                                        }
                                                        else
                                                        {
                                                            if (i <= pageCurent + 3 || i <= 6)
                                                            {
                                                                html += string.Format("<li id='page-{0}'><a onclick='viewPageInfo({0})'>{0}</a></li>", i);
                                                            }
                                                        }
                                                    }
                                                    if (start > sumPage)
                                                    {
                                                        if (pageCurent == 1)
                                                            html += string.Format("<li id='page-{0}'><a>{1}</a></li>", pageCurent, "1");
                                                        else
                                                            html += string.Format("<li id='page-{0}'><a>{1}</a></li>", pageCurent, "OVER");
                                                    }
                                                        
                                            html += @"<li id='page-"+ sumPage + "'><a title='Last' onclick='viewPageInfo(" + sumPage + @")'><i class='fa fa-angle-double-right'></i></a></li>
                                        </ul>
                                    </div>
                                </div>
                            </div>";
            return html;
        }
    }
}