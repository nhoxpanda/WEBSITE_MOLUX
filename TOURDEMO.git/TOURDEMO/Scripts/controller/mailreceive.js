$('.dataTable').DataTable({
    order: [],
    columnDefs: [{ orderable: false, targets: [0] }]
});

$(".dataTable").dataTable().columnFilter({
    sPlaceHolder: "head:after",
    aoColumns: [null,
                { type: "text" },
                { type: "text" },
                { type: "text" },
                { type: "text" }]
});

CKEDITOR.replace("insert-noidung");

/** Xoa du lieu **/
$("#btnRemove").on("click", function () {
    if ($("#listItemId").val() == "") {
        alert("Vui lòng chọn mục cần xóa !");
        return false;
    }
    var $this = $(this);
    var $tableWrapper = $("#tableDictionary-Wrapper");
    var $table = $("#tableDictionary");

    DeleteSelectedItem($this, $tableWrapper, $table, function (data) { });
    return false;
});

$("#tableDictionary").on("change", ".cbItem", function () {
    var ItemID = $(this).val();
    var currentlistItemID = $("#listItemId").val();
    var stringBranchID = "";
    if ($(this).prop('checked')) {
        currentlistItemID += ItemID + ",";
        $("#listItemId").val(currentlistItemID);
    } else {
        $("#listItemId").val(currentlistItemID.replace(ItemID + ",", ""));
    }
});

$("#tableDictionary").on("change", "#allcb", function () {
    var $this = $(this);
    $("#listItemId").val('');
    var currentlistItemID = $("#listItemId").val();
    var ItemID = "";
    if ($this.prop("checked")) {
        $(".cbItem").each(function () {
            ItemID = $(this).val();
            if ($(this).parent().hasClass('text-danger') == false) {
                $(this).prop("checked", true);
                currentlistItemID += ItemID + ",";
                $("#listItemId").val(currentlistItemID);
            }
        });
    } else {
        $(".cbItem").prop("checked", false);
        $("#listItemId").val("");
    }
});

function DeleteSelectedItem(selector, tableWrapper, table, callBack) {
    if (!confirm("Bạn thực sự muốn xóa những mục đã chọn ?")) {
        return false;
    }
    var $form = selector.next("form");
    var options = {
        url: $form.attr("action"),
        type: $form.attr("method"),
        data: $form.serialize(),
    };

    tableWrapper.append("<div class='layer'></div>");

    $.ajax(options).done(function (data) {
        tableWrapper.find(".layer").remove();
        if (data.Succeed) {
            alert(data.Message);
            if (data.IsPartialView) {
                table.replaceWith(data.View);
            }
            else {
                if (data.RedirectTo != null && data.RedirectTo != "") {
                    window.location.href = data.RedirectTo;
                }
            }
        }
        else {
            alert(data.Message);
        }
    });
}
/** Cập nhật dữ liệu **/
$("#btnEdit").click(function () {
    var dataPost = {
        id: $("input[type='checkbox']:checked").val()
    };
    $.ajax({
        type: "POST",
        url: '/MailReceive/Edit',
        data: JSON.stringify(dataPost),
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (data) {
            $("#info-data").html(data);
            CKEDITOR.replace("edit-noidung");
            $("#modal-edit-mailreceive").modal('show');
        }
    });
})
/** chọn từng dòng để hiển thị thông tin chi tiết dưới các tab **/
$("table#tableDictionary").delegate("tr", "click", function () {
    $('tr').not(this).removeClass('oneselected');
    $(this).toggleClass('oneselected');
    var dataPost = { id: $(this).find("input[type='checkbox']").val() };
    $.ajax({
        type: "POST",
        url: '/MailReceive/TabInfoMailReceive',
        data: JSON.stringify(dataPost),
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (data) {
            $("#tab1").html(data);
        }
    });
});
/** xóa kh,nv từ danh sách **/
function removeFromList(id) {
    var dataPost = { id: id };
    $.ajax({
        type: "POST",
        url: '/MailReceive/RemoveFromList',
        data: JSON.stringify(dataPost),
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (data) {
            $("#item-" + id).hide();
        }
    });
}
/** chọn khách hàng hoặc nhân viên nhận mail **/
function btnChoose() {
    if ($("table#tableDictionary").find('tr.oneselected').length === 0) {
        alert("Vui lòng chọn 1 danh sách nhận!");
    }
    else {
        var dataPost = { id: $("table#tableDictionary").find('tr.oneselected').find("input[type='checkbox']").val() };
        $.ajax({
            type: "POST",
            url: '/MailReceive/GetIdReceive',
            data: JSON.stringify(dataPost),
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            success: function (data) {
                $("#info-choose").html('<div id="modal-choose" class="modal fade" role="dialog">' +
                  '<div class="modal-dialog">' +
                    '<div class="modal-content">' +
                      '<div class="modal-header">' +
                        '<button type="button" class="close" data-dismiss="modal">&times;</button>' +
                        '<h4 class="modal-title">Chọn</h4>' +
                      '</div>' +
                      '<div class="modal-body">' +
                        '<p><a style="cursor: pointer" onclick="btnAddStaff()"><i class="fa fa-user"></i>&nbsp;Nhân viên</a></p>' +
                        '<p><a style="cursor: pointer" onclick="btnAddCustomer()" ><i class="fa fa-user"></i>&nbsp;Khách hàng</a></p>' +
                        '<p><a style="cursor: pointer" onclick="btnAddImport()" ><i class="fa fa-user"></i>&nbsp;Khác</a></p>' +
                      '</div>' +
                    '</div>' +
                  '</div>' +
                '</div>');
                $("#modal-choose").modal("show");
            }
        });
    }
}
/** add danh sách nhân viên **/
function btnAddStaff() {
    $("#modal-choose").modal("hide");
    var dataPost = {
        id: $("table#tableDictionary").find('tr.oneselected').find("input[type='checkbox']").val()
    };
    $.ajax({
        type: "POST",
        url: '/MailReceiveList/AddStaff',
        data: JSON.stringify(dataPost),
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (data) {
            $("#info-data-add").html(data);
            $('.dataTable2').dataTable({
                order: [],
                columnDefs: [{ orderable: false, targets: [0] }]
            });
            $(".dataTable2").dataTable().columnFilter({
                sPlaceHolder: "head:after",
                aoColumns: [null,
                            { type: "text" },
                            { type: "text" },
                            { type: "text" },
                            { type: "text" },
                            { type: "text" },
                            { type: "text" },
                            { type: "text" }]
            });
            $("#tableStaff").on("change", ".checkAddStaff", function () {
                var ItemID = $(this).val();
                var currentlistItemID = $("#listItemIdAddStaff").val();
                var stringBranchID = "";
                if ($(this).prop('checked')) {
                    currentlistItemID += ItemID + ",";
                    $("#listItemIdAddStaff").val(currentlistItemID);
                } else {
                    $("#listItemIdAddStaff").val(currentlistItemID.replace(ItemID + ",", ""));
                }
            });
            $("#btnLuuAddStaff").on("click", function () {
                if ($("#listItemIdAddStaff").val() == "") {
                    alert("Vui lòng chọn nhân viên nhận mail !");
                    return false;
                }
                var $form = $("#formAddStaff");
                var options = {
                    url: $form.attr("action"),
                    type: $form.attr("method"),
                    data: $form.serialize(),
                };
                $.ajax(options).done(function (data) {
                    if (data.Succeed) {
                        alert(data.Message);
                        if (data.RedirectTo != null && data.RedirectTo != "") {
                            window.location.href = data.RedirectTo;
                        }
                    }
                    else {
                        alert(data.Message);
                    }
                });
            });
            $("#modal-add-staff").modal("show");

            $("#tableStaff").on("change", "#allstaff", function () {
                var $this = $(this);
                $("#listItemIdAddStaff").val('');
                var currentlistItemID = $("#listItemIdAddStaff").val();
                var ItemID = "";
                if ($this.prop("checked")) {
                    $(".checkAddStaff").each(function () {
                        ItemID = $(this).val();
                        if ($(this).parent().hasClass('text-danger') == false) {
                            $(this).prop("checked", true);
                            currentlistItemID += ItemID + ",";
                            $("#listItemIdAddStaff").val(currentlistItemID);
                        }
                    });
                } else {
                    $(".checkAddStaff").prop("checked", false);
                    $("#listItemIdAddStaff").val("");
                }
            });
        }
    });
}
/** add danh sách khách hàng **/
function btnAddCustomer() {
    $("#modal-choose").modal("hide");
    var dataPost = {
        id: $("table#tableDictionary").find('tr.oneselected').find("input[type='checkbox']").val()
    };
    $.ajax({
        type: "POST",
        url: '/MailReceiveList/AddCustomer',
        data: JSON.stringify(dataPost),
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (data) {
            $("#info-data-add").html(data);
            $('.dataTable3').dataTable({
                order: [],
                columnDefs: [{ orderable: false, targets: [0] }]
            });
            $(".dataTable3").dataTable().columnFilter({
                sPlaceHolder: "head:after",
                aoColumns: [null,
                            { type: "text" },
                            { type: "text" },
                            { type: "text" },
                            { type: "text" },
                            { type: "text" },
                            { type: "text" },
                            { type: "text" }]
            });
            $("#tableCustomer").on("change", ".checkAddCustomer", function () {
                var ItemID = $(this).val();
                var currentlistItemID = $("#listItemIdAddCus").val();
                var stringBranchID = "";
                if ($(this).prop('checked')) {
                    currentlistItemID += ItemID + ",";
                    $("#listItemIdAddCus").val(currentlistItemID);
                } else {
                    $("#listItemIdAddCus").val(currentlistItemID.replace(ItemID + ",", ""));
                }
            });
            $("#btnLuuAddCustomer").on("click", function () {
                if ($("#listItemIdAddCus").val() == "") {
                    alert("Vui lòng chọn khách hàng nhận mail !");
                    return false;
                }
                var $form = $("#formAddCustomer");
                var options = {
                    url: $form.attr("action"),
                    type: $form.attr("method"),
                    data: $form.serialize(),
                };
                $.ajax(options).done(function (data) {
                    if (data.Succeed) {
                        alert(data.Message);
                        if (data.RedirectTo != null && data.RedirectTo != "") {
                            window.location.href = data.RedirectTo;
                        }
                    }
                    else {
                        alert(data.Message);
                    }
                });
            });
            $("#modal-add-customer").modal("show");

            $("#tableCustomer").on("change", "#allcustomer", function () {
                var $this = $(this);
                $("#listItemIdAddCus").val('');
                var currentlistItemID = $("#listItemIdAddCus").val();
                var ItemID = "";
                if ($this.prop("checked")) {
                    $(".checkAddCustomer").each(function () {
                        ItemID = $(this).val();
                        if ($(this).parent().hasClass('text-danger') == false) {
                            $(this).prop("checked", true);
                            currentlistItemID += ItemID + ",";
                            $("#listItemIdAddCus").val(currentlistItemID);
                        }
                        
                    });
                } else {
                    $(".checkAddCustomer").prop("checked", false);
                    $("#listItemIdAddCus").val("");
                }
            });
        }
    });
}
/** add danh sách import **/
function btnAddImport() {
    $("#modal-choose").modal("hide");
    var dataPost = {
        id: $("table#tableDictionary").find('tr.oneselected').find("input[type='checkbox']").val()
    };
    $.ajax({
        type: "POST",
        url: '/MailReceiveList/AddImport',
        data: JSON.stringify(dataPost),
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (data) {
            $("#info-data-add").html(data);
            $('.dataTable4').dataTable({
                order: [],
                columnDefs: [{ orderable: false, targets: [0] }]
            });
            $(".dataTable4").dataTable().columnFilter({
                sPlaceHolder: "head:after",
                aoColumns: [null,
                            { type: "text" },
                            { type: "text" },
                            { type: "text" }]
            });
            $("#tableImport").on("change", ".checkAddImport", function () {
                var ItemID = $(this).val();
                var currentlistItemID = $("#listItemIdAddImport").val();
                var stringBranchID = "";
                if ($(this).prop('checked')) {
                    currentlistItemID += ItemID + ",";
                    $("#listItemIdAddImport").val(currentlistItemID);
                } else {
                    $("#listItemIdAddImport").val(currentlistItemID.replace(ItemID + ",", ""));
                }
            });
            $("#btnLuuAddImport").on("click", function () {
                if ($("#listItemIdAddImport").val() == "") {
                    alert("Vui lòng chọn người nhận mail !");
                    return false;
                }
                var $form = $("#formAddImport");
                var options = {
                    url: $form.attr("action"),
                    type: $form.attr("method"),
                    data: $form.serialize(),
                };
                $.ajax(options).done(function (data) {
                    if (data.Succeed) {
                        alert(data.Message);
                        if (data.RedirectTo != null && data.RedirectTo != "") {
                            window.location.href = data.RedirectTo;
                        }
                    }
                    else {
                        alert(data.Message);
                    }
                });
            });
            $("#modal-add-import").modal("show");

            $("#tableImport").on("change", "#allimport", function () {
                var $this = $(this);
                $("#listItemIdAddImport").val('');
                var currentlistItemID = $("#listItemIdAddImport").val();
                var ItemID = "";
                if ($this.prop("checked")) {
                    $(".checkAddImport").each(function () {
                        ItemID = $(this).val();
                        if ($(this).parent().hasClass('text-danger') == false) {
                            $(this).prop("checked", true);
                            currentlistItemID += ItemID + ",";
                            $("#listItemIdAddImport").val(currentlistItemID);
                        }

                    });
                } else {
                    $(".checkAddImport").prop("checked", false);
                    $("#listItemIdAddImport").val("");
                }
            });
        }
    });
}

