﻿$('.dataTable').DataTable({
    order: [],
    columnDefs: [{ orderable: false, targets: [0] }]
});

$(".dataTable").dataTable().columnFilter({
    sPlaceHolder: "head:after",
    aoColumns: [null,
                { type: "text" },
                { type: "text" },
                { type: "text" },
                { type: "text" },
                { type: "text" },
                { type: "text" },
                { type: "text" },
                { type: "text" }
                
    ]
});

$("#insert-address-company").select2();
$("#insert-tag-document").select2();
$("#insert-document-type").select2();

//CKEDITOR.replace("insert-company-note");
//CKEDITOR.replace("insert-document-note");

$("#btnEdit").click(function () {
    var dataPost = {
        id: $("input[type='checkbox']:checked").val()
    };

    $.ajax({
        type: "POST",
        url: '/AirlineManage/AirlineInformation',
        data: JSON.stringify(dataPost),
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (data) {
            $("#info-data-airline").html(data);
            $("#modal-edit-airline").modal("show");
        }
    });
});

/** Xoa du lieu **/

$("#btnRemove").on("click", function () {
    if ($("#listItemId").val() == "") {
        alert("Vui lòng chọn mục cần xóa !");
        return false;
    }
    var $this = $(this);
    var $tableWrapper = $("#tableDictionary-Wrapper");
    var $table = $("#tableDictionary");

    DeleteSelectedItem($this, $tableWrapper, $table, function (data) {

    });
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

$("#btnExport").click(function () {
    $("#exportForm").submit();
});

/** button thêm tài liệu của công ty **/
function btnDocumentFile() {
    //var dataPost = {
    //    id: $("input[type='checkbox']:checked").val()
    //};

    //$.ajax({
    //    type: "POST",
    //    url: '/CompanyManage/GetIdCompany',
    //    data: JSON.stringify(dataPost),
    //    contentType: "application/json; charset=utf-8",
    //    dataType: "html",
    //    success: function (data) {
    //        $("#modal-insert-document").modal("show");
    //    }
    //});
}

//$("table#tableDictionary").delegate("tr", "click", function () {
//    var dataPost = { id: $(this).find("input[type='checkbox']").val() };
//    $.ajax({
//        type: "POST",
//        url: '/CompanyManage/CompanyDocumentList',
//        data: JSON.stringify(dataPost),
//        contentType: "application/json; charset=utf-8",
//        dataType: "html",
//        success: function (data) {
//            $("#table-document").html(data);
//        }
//    });
//});

/** xóa tài liệu **/
function deleteDocument(id) {
    //var dataPost = { id: id };
    //$.ajax({
    //    type: "POST",
    //    url: '/CompanyManage/DeleteDocument',
    //    data: JSON.stringify(dataPost),
    //    contentType: "application/json; charset=utf-8",
    //    dataType: "html",
    //    success: function (data) {
    //        alert(data.Message);
    //    }
    //});
}
$("table#tableDictionary").delegate("tr", "click", function () {
    var dataPost = { id: $(this).find("input[type='checkbox']").val() };
    $('tr').not(this).removeClass('oneselected');
    $(this).toggleClass('oneselected');
    $.ajax({
        type: "POST",
        url: '/AirlineManage/Detail',
        data: JSON.stringify(dataPost),
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (data) {
            $("#ticket").html(data);
        }
    });
});


function btnDeleteTicket(id) {
    if (!confirm("File upload trước đó cũng sẽ bị xóa? Bạn thực sự muốn xóa những mục đã chọn ? ")) {
        return false;
    }
    $.ajax({
        type: "POST",
        url: '/AirlineManage/DeleteTicket',
        data: { IdTicket: id },
        success: function (data) {
            alert(data.Message);
        }
    });
}

function btnCreateTicket() {
    if ($("table#tableDictionary").find('tr.oneselected').length === 0) {
        alert("Vui lòng chọn 1 hãng hàng không!");
    }
    else {
        var dataPost = { id: $("table#tableDictionary").find('tr.oneselected').find("input[type='checkbox']").val() };
        $.ajax({
            type: "POST",
            url: '/AirlineManage/GetIdAirline',
            data: JSON.stringify(dataPost),
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            success: function (data) {
                //$("#insert-country-visa").select2();
                //$("#insert-type-visa").select2();
                //$("#insert-status-visa").select2();
                $("#modal-insert-ticket").modal("show");
                //checkCodeVisa();
            }
        });
    }
}




$("#btnInsertTicket").click(function () {
    var $this = $(this);
    var $form = $("#frmInsertTicket");
    var $parent = $form.parent();
    var options = {
        url: $form.attr("action"),
        type: $form.attr("method"),
        data: $form.serialize()
    };

    $.ajax(options).done(function (data) {
        $("#modal-insert-ticket").modal("hide");
        alert("Lưu dữ liệu thành công!");
        //$("#ticket").html(data);
    });
    return false;
})

/** cập nhật tài liệu **/
function updateDocument(id) {
    //var dataPost = { id: id };
    //$.ajax({
    //    type: "POST",
    //    url: '/CompanyManage/EditInfoDocument',
    //    data: JSON.stringify(dataPost),
    //    contentType: "application/json; charset=utf-8",
    //    dataType: "html",
    //    success: function (data) {
    //        $("#info-data-doc").html(data);
    //        $("#edit-tag-document").select2();
    //        $("#edit-document-type").select2();
    //        CKEDITOR.replace("edit-document-note");
    //        $("#modal-edit-document").modal("show");
    //    }
    //});
}

/** file import **/
$('#fileImport').change(function () {
    var data = new FormData();
    data.append('FileName', $('#fileImport')[0].files[0]);

    var ajaxRequest = $.ajax({
        type: "POST",
        url: '/CandidateManage/ImportFile',
        contentType: false,
        processData: false,
        data: data
    });

    ajaxRequest.done(function (xhr, textStatus) {
        // Onsuccess
    });
    ajaxRequest.success(function (data) {
        $("#listItemIdI").val("");
        $("#import-data-candidate").html(data);
    })
});


$('#file-insert-ticket').change(function () {
    var data = new FormData();
    data.append('FileNameQuotation', $('#file-insert-ticket')[0].files[0]);

    var ajaxRequest = $.ajax({
        type: "POST",
        url: 'AirlineManage/UploadFileQuotation',
        contentType: false,
        processData: false,
        data: data
    });

    ajaxRequest.done(function (xhr, textStatus) {
        // Onsuccess
    });
});