var table = $('.dataTable').DataTable({
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
                { type: "text" },
                { type: "text" },
                { type: "text" },
                { type: "text" },
                { type: "text" },
                { type: "text" }]
});

CKEDITOR.replace("insert-note");
$("#insert-type").select2();
$("#insert-status").select2();
$("#insert-start").select2();
$("#insert-end").select2();
$("#insert-currency").select2();
$("#insert-customer").select2();

$("#btnEdit").click(function () {
    var dataPost = {
        id: $("input[type='checkbox']:checked").val()
    };

    $.ajax({
        type: "POST",
        url: '/TicketManage/EditInfoTicket',
        data: JSON.stringify(dataPost),
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (data) {
            $("#data-ticket").html(data);
            CKEDITOR.replace("edit-note");
            $("#edit-type").select2();
            $("#edit-status").select2();
            $("#edit-start").select2();
            $("#edit-end").select2();
            $("#edit-customer").select2();
            $("#edit-currency").select2();
            $("#modal-edit-ticket").modal("show");

            $("#edit-customer").change(function () {
                var dataPost = {
                    id: $("#edit-customer").val()
                };

                $.ajax({
                    type: "POST",
                    url: '/TicketManage/LoadCustomer',
                    data: JSON.stringify(dataPost),
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (data) {
                        $("#edit-phone").val(data.phone);
                        $("#edit-skyteam").val(data.skyteam);
                    }
                });
            })
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
        if (data == 1) {
            alert('Đã xóa!');
            window.location.href = '/TicketManage';
        }
        else {
            alert('Lỗi, vui lòng xem lại!');
        }
    });
}

/** chọn từng dòng để hiển thị thông tin chi tiết dưới các tab **/
$("table#tableDictionary").delegate("tr", "click", function () {
    var dataPost = { id: $(this).find("input[type='checkbox']").val() };
    $.ajax({
        type: "POST",
        url: '/TicketManage/InfoNote',
        data: JSON.stringify(dataPost),
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (data) {
            $("#tab1").html(data);
        }
    });
});

$("#insert-customer").change(function () {
    var dataPost = {
        id: $("#insert-customer").val()
    };

    $.ajax({
        type: "POST",
        url: '/TicketManage/LoadCustomer',
        data: JSON.stringify(dataPost),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            $("#insert-phone").val(data.phone);
            $("#insert-skyteam").val(data.skyteam);
        }
    });
})

checkCodeTicket();

function checkCodeTicket() {
    var dataPost = { code: $("#CodeTicket").val() };
    $.ajax({
        type: "POST",
        url: '/CustomerOtherTab/CheckCodeTicket',
        data: JSON.stringify(dataPost),
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (data) {
            if (data == 0) {
                $("#CodeTicket").val('');
                $("#CodeTicket").focus();
                alert('Trùng Vé máy bay');
            }
        }
    });
}