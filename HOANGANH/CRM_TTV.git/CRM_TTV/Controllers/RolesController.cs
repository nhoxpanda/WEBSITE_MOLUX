using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CRM_TTV;
using CRM_TTV.Models;
using Newtonsoft.Json;

namespace CRM_TTV.Controllers
{

    public class RolesController : Controller
    {
        private CRM_TTVEntities db = new CRM_TTVEntities();

        public async Task<ActionResult> test()
        {
            return View();
        }
        // GET: Roles
        public async Task<ActionResult> Index(string order, Int32? size = 10, Int32? page = 1)
        {
            //var tbRoles = db.tbRoles.Include(t => t.tbCompany).Include(t => t.tbRoleType);
            //return View(await tbRoles.ToListAsync());
            if ((Int32)size < 0)
                size = 10;
            if ((Int32)page < 0)
                page = 1;

            if (TempData["search"] != null) //trả về kết quả tìm kiếm
            {
                if (TempData["searchModel"] != null) //diss mẹ phải chơi kiểu này nó mới lưu dc vcl chưa biết fix
                    TempData["sModel"] = TempData["searchModel"];

                IEnumerable<object> search = TempData.ContainsKey("search") ? TempData["search"] as IEnumerable<tbCompany> : null;
                ViewBag.Paging = Paging.Pagination(search.Count(), page, size);
                TempData["speaker"] = new speaker { type = 2, title = "Thành công...!!!", content = "Đã tìm thấy " + search.Count() + " dòng dữ liệu...!!!" };
                return View(search);
            }

            int take = (Int32)size;
            int skip = 0;
            if (page > 0)
                skip = ((Int32)page - 1) * take;
            //var ghghgh = db.tbRoles.OrderBy("name asc");

            string[] orderBy = null;
            if (!string.IsNullOrEmpty(order))
            {
                // trả về kết quả xắp xếp theo yêu cầu của client
                orderBy = order.Split('-');
                //var tbRoles = db.tbRoles.OrderBy(orderBy[0] + " " + orderBy[1]).Skip(skip).Take(take).Include(t => t.tbCompany).Include(t => t.tbRoleType);

                var tbRoles = db.tbRoles;
                ViewBag.Paging = Paging.Pagination(db.tbRoles.Count(), page, size);
                return View(await tbRoles.ToListAsync());
            }
            else
            {
                //trả về danh sách mặc định từ csdl và xắp xếp theo ID giảm dần
                var tbRoles = db.tbRoles.OrderByDescending(x => x.roleID).Skip(skip).Take(take).Include(t => t.tbCompany).Include(t => t.tbRoleType);
                ViewBag.Paging = Paging.Pagination(db.tbRoles.Count(), page, size);
                return View(await tbRoles.ToListAsync());
            }
        }

        // GET: Roles/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbRole tbRole = await db.tbRoles.FindAsync(id);
            if (tbRole == null)
            {
                return HttpNotFound();
            }
            return PartialView(tbRole);
        }

        // GET: Roles/Create
        public ActionResult Create()
        {
            ViewBag.idCompany = new SelectList(db.tbCompanies, "idCompany", "name");
            ViewBag.idRoleType = new SelectList(db.tbRoleTypes, "idRoleType", "name");
            return PartialView();
        }
        public ActionResult AuthorizationRole()
        {
            return PartialView();
        }

        [HttpPost]
        public async Task<ActionResult> AuthorizationRole(int[] ids, int idRole, string role)
        {
            if (idRole == -1)
            {
                foreach (var id in ids)
                {
                    tbRole tbRole = await db.tbRoles.FindAsync(id);
                    tbRole.role = role.ToString();
                }
            }
            else
            {
                foreach (var id in ids)
                {
                    tbUser tbUser = await db.tbUsers.FindAsync(id);
                    tbUser.userRole = null;
                    //Kiểm tra nếu quyền dc chọn = với quyền của Role thì set role ID
                    //ngược lại thì set quyền đặc biệt cho user
                    if (role == db.tbRoles.FirstOrDefault(x => x.roleID == idRole).role)
                        tbUser.roleID = idRole;
                    else
                        tbUser.userRole = role.ToString();
                }
            }

            await db.SaveChangesAsync();
            TempData["speaker"] = new speaker { type = 1, title = "Success!", content = "Phân quyền thành công...!!!" };
            return Json("Phân quyền thành công...!!!");
        }

        // POST: Roles/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "roleID,idCompany,idRoleType,name,role,sort,note")] tbRole tbRole)
        {
            if (ModelState.IsValid)
            {
                db.tbRoles.Add(tbRole);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.idCompany = new SelectList(db.tbCompanies, "idCompany", "name", tbRole.idCompany);
            ViewBag.idRoleType = new SelectList(db.tbRoleTypes, "idRoleType", "name", tbRole.idRoleType);

            TempData["speaker"] = new speaker { type = 2, title = "Error!", content = "Đã có lỗi xẩy ra...!!!" };
            return RedirectToAction("Index");
        }

        // GET: Roles/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbRole tbRole = await db.tbRoles.FindAsync(id);
            if (tbRole == null)
            {
                return HttpNotFound();
            }
            ViewBag.idCompany = new SelectList(db.tbCompanies, "idCompany", "name", tbRole.idCompany);
            ViewBag.idRoleType = new SelectList(db.tbRoleTypes, "idRoleType", "name", tbRole.idRoleType);
            TempData["speaker"] = new speaker { type = 1, title = "Success!", content = "Cập nhật thành công...!!!" };

            return PartialView(tbRole);
        }

        // POST: Roles/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "roleID,idCompany,idRoleType,name,role,sort,note")] tbRole tbRole)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tbRole).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.idCompany = new SelectList(db.tbCompanies, "idCompany", "name", tbRole.idCompany);
            ViewBag.idRoleType = new SelectList(db.tbRoleTypes, "idRoleType", "name", tbRole.idRoleType);
            return PartialView(tbRole);
        }

        // GET: Roles/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbRole tbRole = await db.tbRoles.FindAsync(id);
            if (tbRole == null)
            {
                return HttpNotFound();
            }
            return View(tbRole);
        }

        // POST: Roles/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int[] ids)
        {
            foreach (var id in ids)
            {
                tbRole tbRole = await db.tbRoles.FindAsync(id);
                db.tbRoles.Remove(tbRole);
            }
            await db.SaveChangesAsync();
            TempData["speaker"] = new speaker { type = 3, title = "Success!", content = "Xóa thành công " + ids.Count() + " dòng dữ liệu...!!!" };
            return Json("Deleted successfully!");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public string GetRoleType()
        {
            List<roles.Roles> category = new List<roles.Roles>();
            category = db.tbRoles.OrderBy(x => x.sort).Select(m => new roles.Roles()
            {
                ID = m.roleID,
                name = m.name,
                roles = m.role
            }).ToList();
            return JsonConvert.SerializeObject(category);
        }

        /// <summary>
        /// trả về quyền của 1 role or của 1 user
        /// Dùng để check các checkbox trên freeViews
        /// </summary>
        public string GetRole(string id)
        {
            if (id.Contains("user_"))
            {
                var item = db.tbUsers.FirstOrDefault(x => "user_" + x.userID == id);
                if (item != null)
                    return string.IsNullOrEmpty(item.userRole) ? db.tbRoles.FirstOrDefault(z => z.roleID == item.roleID).role : item.userRole;
            }
            else
                return db.tbRoles.FirstOrDefault(x => x.roleID.ToString() == id).role;
            return null;
        }

        /// <summary>
        /// trả về chuổi json chứa menu form và các action mà user có quyền truy vấn
        /// dùng để load menu và check quyền truy cập các buttom action khi user click
        /// </summary>
            // GET: api/role/{user_5 |1}
        public string UserMenuRole(string id)
        {
            //TẠO RA THƯ VIỆN MENU
            Dictionary<int, MenuFreeView> dictMenu = db.tbMenuAndForms.Where(x => x.del == false && x.type != 0).OrderBy(x => x.sort)
                .Select(m => new MenuFreeView
                {
                    idDict = m.menuID,
                    id = m.type == 1 ? "M-" + m.menuID : "F-" + m.menuID,
                    ParentId = m.parentID == null ? 0 : (int)m.parentID,
                    text = m.name,
                    icon = m.icon,
                    redirect = m.redirect,
                }).ToDictionary(m => m.idDict);

            //Lấy quyền của user
            string listRole = "";
            if (id.Contains("user_"))
            {
                var item = db.tbUsers.FirstOrDefault(x => "user_" + x.userID == id);
                if (item != null)
                {
                    //if (!string.IsNullOrEmpty(item.menuRole))
                    //    return item.menuRole;

                    listRole = item.userRole == null ? db.tbRoles.FirstOrDefault(z => z.roleID == item.roleID).role : item.userRole;
                }
            }
            else
            {
                var item = db.tbRoles.FirstOrDefault(x => x.roleID.ToString() == id);
                if (item != null)
                    listRole = item.role == null ? null : item.role;
            }

            //Xóa các menu mà user ko có quyền
            foreach (var kvp in dictMenu.ToList())
            {
                MenuFreeView item = kvp.Value;
                if (!listRole.Contains(item.id))
                    dictMenu.Remove(kvp.Key);
            }

            //tạo menu với quyền của user
            List<MenuFreeView> rootMenuFree = new List<MenuFreeView>();
            foreach (var kvp in dictMenu)
            {
                List<MenuFreeView> menu = rootMenuFree;
                MenuFreeView item = kvp.Value;
                if (item.ParentId > 0 && dictMenu.ContainsKey(item.ParentId))
                {
                    try
                    {
                        menu = dictMenu[item.ParentId].children;
                    }
                    catch
                    { }
                }
                menu.Add(item);
            }

            return JsonConvert.SerializeObject(rootMenuFree, Formatting.Indented);
        }


        /// <summary>
        /// trả về json tất cả các menu form và action
        /// dành để load tree view
        /// khi mới vào là load thằng này lên chưa có check gì hết
        /// </summary>
        [HttpGet]
        public string FreeViewAll(bool? isMenu)
        {
            isMenu = false;

            //lấy tất cả menu, form và action
            Dictionary<int, MenuFreeView> dictMenu = db.tbMenuAndForms.Where(x => x.del == false).OrderBy(x => x.sort)
                .Select(m => new MenuFreeView
                {
                    idDict = m.menuID,
                    id = m.type == 1 ? "M-" + m.menuID : m.type == 2 ? "F-" + m.menuID : "F-" + m.parentID + "-A-" + m.idAction,
                    ParentId = m.parentID == null ? 0 : (int)m.parentID,
                    text = m.name,
                    icon = m.icon,
                    redirect = m.redirect,
                }).ToDictionary(m => m.idDict);


            List<MenuFreeView> rootMenuFree = new List<MenuFreeView>();
            foreach (var kvp in dictMenu)
            {
                List<MenuFreeView> menu = rootMenuFree;
                MenuFreeView item = kvp.Value;
                if (item.ParentId > 0 && dictMenu.ContainsKey(item.ParentId))
                {
                    try
                    {
                        menu = dictMenu[item.ParentId].children;
                    }
                    catch
                    { }
                }
                menu.Add(item);
            }

            return JsonConvert.SerializeObject(rootMenuFree);

        }

        ////THẰNG NÀY CÓ THỂ ĐÉO SÀI
        //[HttpGet]
        ////[Route("Roles/FreeView/{id:int}")]
        //public string FreeView(int? id)
        //{
        //    List<string> listRole = new List<string>();

        //    var roles = db.tbRoles.FirstOrDefault(x => x.roleID == id);
        //    if (roles != null)
        //        listRole = roles.role == null ? null : roles.role.Split(',').ToList();

        //    List<roles.roleObject> categories = new List<roles.roleObject>();
        //    var listMenu = db.tbMenuAndForms.Where(x => x.del == false && x.parentID == null).OrderBy(x => x.sort);
        //    foreach (var item in listMenu)
        //    {
        //        roles.roleObject rol = new roles.roleObject();
        //        rol.id = "M-" + item.menuID;
        //        rol.text = item.name;
        //        rol.icon = item.icon;
        //        rol.redirect = item.redirect;
        //        rol.state = new roles.State { opened = false, selected = (listRole != null ? listRole.Contains("M-" + item.menuID) : false) };
        //        if (item.isForm)
        //        {
        //            if (item.jsonData != null)
        //                GetChildren(item.jsonData, item.menuID, listRole);
        //            else
        //                rol.children = null;
        //        }
        //        else
        //            GetChildren(item.menuID, listRole);

        //        categories.Add(rol);
        //    }

        //    return JsonConvert.SerializeObject(categories);
        //}

        public List<roles.roleObject> GetAction(string jsonAction, int parentId, List<string> listRole)
        {
            return null;
            List<roles.State> comments = JsonConvert.DeserializeObject<List<roles.State>>(jsonAction);
            var res = comments.Select(c => new roles.roleObject
            {
                id = "F-" + parentId + "-A-" + c.actionID,
                text = db.tbActions.FirstOrDefault(x => x.actionID == c.actionID).name,
                state = listRole.Contains("F-" + parentId + "-A-" + c.actionID) ? new roles.State { actionID = c.actionID, urlRequest = c.urlRequest, selected = true } : null,
                children = null
            }).ToList().Where(z => z.state == null ? false : z.state.selected).ToList();

            return res;
        }

        public List<MenuFreeView> GetAction(string jsonAction, int parentId)
        {

            List<MenuFreeView.State> comments = JsonConvert.DeserializeObject<List<MenuFreeView.State>>(jsonAction);
            return comments.Select(c => new MenuFreeView
            {
                id = "F-" + parentId + "-A-" + c.actionID,
                ParentId = parentId,
                text = "",
                redirect = c.urlRequest,
                children = null
            }).ToList();
        }


        //public List<roles.roleObject> GetChildRole(int parentId, List<string> listRole)
        //{
        //    if (listRole != null)
        //    {
        //        return db.tbMenuAndForms
        //        .Where(m => m.parentID == parentId && m.del == false).ToList()
        //        .Select(m => new roles.roleObject
        //        {
        //            id = m.isForm ? "F-" + m.menuID : "M-" + m.menuID,
        //            text = m.name,
        //            icon = m.icon,
        //            redirect = m.redirect,
        //            state = listRole.Contains(m.isForm ? "F-" + m.menuID : "M-" + m.menuID) ? new roles.State { selected = true } : null,
        //            children = m.isForm ? m.jsonData != null ? GetAction(m.jsonData, m.menuID, listRole) : null : GetChildRole(m.menuID, listRole)
        //        }).ToList().Where(z => z.state == null ? true : z.state.selected).ToList().Where(x => (x.children != null ? x.children.Count() > 0 : true) || listRole.Contains(x.id)).ToList();
        //    }
        //    return null;
        //}
        //public List<roles.roleObject> GetChildren(int parentId, List<string> listRole)
        //{
        //    if (listRole != null)
        //    {
        //        return db.tbMenuAndForms
        //            .Where(m => m.parentID == parentId && m.del == false).ToList()
        //            .Select(m => new roles.roleObject
        //            {
        //                id = m.isForm ? "F-" + m.menuID : "M-" + m.menuID,
        //                text = m.name,
        //                icon = m.icon,
        //                redirect = m.redirect,
        //                state = new roles.State { selected = listRole.Contains(m.isForm ? "F-" + m.menuID : "M-" + m.menuID) },
        //                children = m.isForm ? m.jsonData != null ? GetChildren(m.jsonData, m.menuID, listRole) : null : GetChildren(m.menuID, listRole)
        //            }).ToList();
        //    }
        //    else
        //    {
        //        return db.tbMenuAndForms
        //            .Where(m => m.parentID == parentId && m.del == false).ToList()
        //            .Select(m => new roles.roleObject
        //            {
        //                id = m.isForm ? "F-" + m.menuID : "M-" + m.menuID,
        //                text = m.name,
        //                icon = m.icon,
        //                redirect = m.redirect,
        //                children = m.isForm ? m.jsonData != null ? GetChildren(m.jsonData, m.menuID, listRole) : null : GetChildren(m.menuID, listRole)
        //            }).ToList();
        //    }
        //}

        //public List<roles.roleObject> GetChildren(string jsonAction, int parentId, List<string> listRole)
        //{
        //    List<roles.State> comments = JsonConvert.DeserializeObject<List<roles.State>>(jsonAction);
        //    return comments.Select(c => new roles.roleObject
        //    {
        //        id = "F-" + parentId + "-A-" + c.actionID,
        //        text = db.tbActions.FirstOrDefault(x => x.actionID == c.actionID).name,
        //        state = new roles.State { actionID = c.actionID, urlRequest = c.urlRequest, selected = listRole == null ? false : listRole.Contains("F-" + parentId + "-A-" + c.actionID) },
        //        children = null
        //    }).ToList();
        //}
    }
}
