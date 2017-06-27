$("#ddlid").select2();
$("#ddlidtag").select2();
$("#ddltype").select2();
$("#ddlstatus").select2();
$("#select-tour").select2();

$("#select-customer").select2();
$("#insert-country-visa").select2();
$("#insert-status-visa").select2();
$("#insert-type-visa").select2();

$("#select-staff").select2();
$("#insert-country-visastaff").select2();
$("#insert-status-visastaff").select2();
$("#insert-type-visastaff").select2();

selectRowTable();

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

function selectRowTable() {
    $("table#tableDictionary").delegate("tr", "click", function (e) {
        $('tr').not(this).removeClass('oneselected');
        $(this).toggleClass('oneselected');
        console.log($(this).find("input[name='VisaType']").val());
        if ($(this).find("input[name='VisaType']").val() == "1") {
            var dataPost = {
                id: $(this).find("input[name='VisaId']").val()
            };
            $.ajax({
                type: "POST",
                url: '/ListVisaManage/VisaProcedureList',
                data: JSON.stringify(dataPost),
                contentType: "application/json; charset=utf-8",
                dataType: "html",
                success: function (data) {
                    $("#thongtinchitiet").html(data);
                }
            });
        }
        else {
            $("#thongtinchitiet").html("<div class='row'><strong>Visa nhân viên không có checklist thủ tục.</strong></div>");
        }
        
    });
}

function filter() {
    var dataPost = {
        id: $("#ddlid").val(),
        idtag: $("#ddlidtag").val(),
        type: $("#ddltype").val(),
        status: $("#ddlstatus").val(),
        startDate: $("#txtstart").val(),
        endDate: $("#txtend").val(),
        personal: $("#ddlpersonal").val()
    };
    $.ajax({
        type: "POST",
        url: '/ListVisaManage/FilterCustomerStaff',
        data: JSON.stringify(dataPost),
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (data) {
            $("#data-visa").html(data);

            $('.dataTable').DataTable({
                order: [],
                columnDefs: [{ orderable: false, targets: [0] }]
            });

            $(".dataTable").dataTable().columnFilter({
                sPlaceHolder: "head:after",
                //aoColumns: [null,
                //            { type: "text" },
                //            { type: "text" },
                //            { type: "text" },
                //            { type: "text" },
                //            { type: "text" },
                //            { type: "text" },
                //            { type: "text" },
                //            { type: "text" }]
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
                { type: "text" }]
            });
            // kéo dài kích thước cột
            colResize();
            selectRowTable();
        }
    });
}

$("#ddlid").change(function () {
    filter();
    $("#txtid").val($("#ddlid").val());
});

$("#ddlidtag").change(function () {
    filter();
    $("#txtidtag").val($("#ddlidtag").val());
});

$("#ddltype").change(function () {
    filter();
    $("#txttype").val($("#ddltype").val());
});

$("#ddlstatus").change(function () {
    filter();
    $("#txtstatus").val($("#ddlstatus").val());
});

$("#ddlpersonal").change(function () {
    filter();
    $("#txtpersonal").val($("#ddlpersonal").val());
});

$("#txtstart").change(function () {
    filter();
    $("#txtstartDate").val($("#txtstart").val());
});

$("#txtend").change(function () {
    filter();
    $("#txtendDate").val($("#txtend").val());
});

/** cập nhật thủ tục visa **/
function updateProcedure(id) {
    var dataPost = {
        id: id,
        check: $("#ckProcedure" + id).is(':checked') ? 1 : 0,
        visaId: $("#VisaID").val()
    };
    $.ajax({
        type: "POST",
        url: '/ListVisaManage/UpdateVisaProcedure',
        data: JSON.stringify(dataPost),
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (data) {
        }
    });
}

/** check visa **/
$("#insert-visa-customer").change(function () {
    var dataPost = {
        text: $("#insert-visa-customer").val()
    };
    $.ajax({
        type: "POST",
        url: '/CustomersManage/CheckVisa',
        data: JSON.stringify(dataPost),
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (data) {
            if (data == "1") { // trùng
                if (!confirm("Dữ liệu trùng lắp! Bạn có muốn lưu lại không?")) {
                    $("#insert-visa-customer").val('');
                    $("#insert-visa-customer").focus();
                }
            }
        }
    });
});

/** check visa **/
$("#insert-visa-staff").change(function () {
    var dataPost = {
        text: $("#insert-visa-staff").val()
    };
    $.ajax({
        type: "POST",
        url: '/StaffManage/CheckVisa',
        data: JSON.stringify(dataPost),
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (data) {
            if (data == "1") { // trùng
                if (!confirm("Dữ liệu trùng lắp! Bạn có muốn lưu lại không?")) {
                    $("#insert-visa-staff").val('');
                    $("#insert-visa-staff").focus();
                }
            }
        }
    });
});

/** sửa Visa **/
function editVisa(id, iscus){
    var dataPost = {
        id: id,
        iscus: iscus
    };
    $.ajax({
        type: 'POST',
        url: '/ListVisaManage/Edit',
        data: JSON.stringify(dataPost),
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (data) {
            $("#data-edit-visa").html(data);
            $("#edit-country-visa").select2();
            $("#edit-status-visa").select2();
            $("#edit-type-visa").select2();
            $("#modal-edit-visa").modal("show");
        }
    })
}

/** xóa Visa **/
function deleteVisa(id, iscus) {
    var dataPost = {
        id: id,
        iscus: iscus
    };
    $.ajax({
        type: 'POST',
        url: '/ListVisaManage/DeleteVisa',
        data: JSON.stringify(dataPost),
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (data) {
            $("#row" + id + iscus).hide();
        }
    })
}

function OnFailureListVisa() {
    $("#modal-edit-visa").modal('hide');
    alert("Lỗi!");
    window.location.href = '/ListVisaManage';
}

function OnSuccessListVisa() {
    $("#modal-edit-visa").modal('hide');
    window.location.href = '/ListVisaManage';
}