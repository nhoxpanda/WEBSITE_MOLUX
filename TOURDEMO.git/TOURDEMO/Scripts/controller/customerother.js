CKEDITOR.replace("insert-note-quotation");
CKEDITOR.replace("insert-program-note");
CKEDITOR.replace("insert-note-contract");
CKEDITOR.replace("insert-note-ticket");
CKEDITOR.replace("insert-request-tour");
CKEDITOR.replace("insert-noteflight-tour");
//CKEDITOR.replace("edit-note-lichhen");
//CKEDITOR.replace("edit-note-lienhe");
function OnSuccessAppointment() {
    $("#modal-insert-appointment").modal("hide");
    $("#modal-edit-appointment").modal("hide");
    $('#formLichHen').trigger("reset");
    $("#modal-insert-contacthistory").modal("hide");
    $("#modal-edit-contacthistory").modal("hide");
    $('form').trigger("reset");
    CKupdate();
}

function OnFailureAppointment() {
    alert("Lỗi, vui lòng kiểm tra lại!");
    $("#modal-insert-appointment").modal("hide");
    $("#modal-edit-appointment").modal("hide");
    $('#formLichHen').trigger("reset");
    $("#modal-insert-contacthistory").modal("hide");
    $("#modal-edit-contacthistory").modal("hide");
    $('form').trigger("reset");
    CKupdate();
}

/** xóa lịch hẹn **/
function deleteAppointment(id) {
    if (confirm('Bạn thực sự muốn xóa mục này ?')) {
        var dataPost = { id: id };
        $.ajax({
            type: "POST",
            url: '/CustomerOtherTab/DeleteAppointment',
            data: JSON.stringify(dataPost),
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            success: function (data) {
                alert("Xóa dữ liệu thành công!!!");
                $("#lichhen").html(data);
            }
        });
    }
}

/** xóa lịch sử liên hệ **/
function deleteContactHistory(id) {
    if (confirm('Bạn thực sự muốn xóa mục này ?')) {
        var dataPost = { id: id };
        $.ajax({
            type: "POST",
            url: '/CustomerOtherTab/DeleteContactHistory',
            data: JSON.stringify(dataPost),
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            success: function (data) {
                alert("Xóa dữ liệu thành công!!!");
                $("#lichsulienhe").html(data);
            }
        });
    }
}

/** xóa người liên hệ **/
function deleteContact(id) {
    if (confirm('Bạn thực sự muốn xóa mục này ?')) {
        var dataPost = { id: id };
        $.ajax({
            type: "POST",
            url: '/CustomerOtherTab/DeleteContact',
            data: JSON.stringify(dataPost),
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            success: function (data) {
                alert("Xóa dữ liệu thành công!!!");
                $("#nguoilienhe").html(data);
            }
        });
    }
}

/* upload file báo giá */
$('#insert-file-quotation').change(function () {
    var data = new FormData();
    var totalFiles = document.getElementById("insert-file-quotation").files.length;
    for (var i = 0; i < totalFiles; i++) {
        var file = document.getElementById("insert-file-quotation").files[i];
        data.append('FileNameQuotation', file);
    }

    var ajaxRequest = $.ajax({
        type: "POST",
        url: 'CustomerOtherTab/UploadMultipleFileQuotation',
        contentType: false,
        processData: false,
        data: data
    });

    ajaxRequest.done(function (xhr, textStatus) {
        // Onsuccess
    });
});

/**** upload file hợp đồng ****/
$('#insert-file-contract').change(function () {
    var data = new FormData();
    data.append('FileNameContract', $('#insert-file-contract')[0].files[0]);

    var ajaxRequest = $.ajax({
        type: "POST",
        url: 'CustomerOtherTab/UploadFileContract',
        contentType: false,
        processData: false,
        data: data
    });

    ajaxRequest.done(function (xhr, textStatus) {
        // Onsuccess
    });
});

/**** upload file chương trình ****/
$('#insert-file-program').change(function () {
    var data = new FormData();
    data.append('FileNameProgram', $('#insert-file-program')[0].files[0]);

    var ajaxRequest = $.ajax({
        type: "POST",
        url: 'CustomerOtherTab/UploadFileProgram',
        contentType: false,
        processData: false,
        data: data
    });

    ajaxRequest.done(function (xhr, textStatus) {
        // Onsuccess
    });
});

/** thêm mới tour **/
function btnCreateTour() {
    if ($("table#tableDictionary").find('tr.oneselected').length === 0) {
        alert("Vui lòng chọn 1 khách hàng!");
    }
    else {
        var dataPost = { id: $("table#tableDictionary").find('tr.oneselected').find("input[type='checkbox']").val() };

        $.ajax({
            type: "POST",
            url: '/CustomersManage/GetIdCustomer',
            data: JSON.stringify(dataPost),
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            success: function (data) {
                $("#insert-start-place").select2();
                $("#insert-destination-place").select2();
                $("#insert-type-tour").select2();
                $("#insert-status-tour").select2();
                $("#insert-manager-tour").select2();
                $("#insert-permission-tour").select2();
                $("#modal-insert-tour").modal("show");
                checkCodeTour();
            }
        });
    }
}

/** thêm hdv **/
function insertTourGuide(id) {
    var dataPost = { id: id };
    $.ajax({
        type: "POST",
        url: '/CustomerOtherTab/GetIdTour',
        data: JSON.stringify(dataPost),
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (data) {
            $("#insert-guide").select2();
            $("#modal-insert-guide").modal("show");

            $("#insert-check").click(function () {
                if (this.checked) {
                    $("#insert-guide").removeAttr("disabled", "disabled");
                }
                else {
                    $("#insert-guide").attr("disabled", "disabled");
                }
            });

            $("#insert-guide").change(function () {
                $.getJSON('/CustomerOtherTab/LoadStaffInfo/' + $('#insert-guide').val(), function (data) {
                    console.log(data.Birthday);
                    $("#SingleStaff_FullName").val(data.FullName);
                    $("#SingleStaff_Birthday").val(data.Birthday);
                    $("#SingleStaff_CodeGuide").val(data.CodeGuide);
                });
            });
        }
    });
}

function listTourGuide(id) {
    var dataPost = { id: id };
    $.ajax({
        type: "POST",
        url: '/CustomerTabInfo/ListTourGuide',
        data: JSON.stringify(dataPost),
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (data) {
            $("#info-data-guide").html(data);
            $("#modal-list-guide").modal("show");
        }
    });
}

/** thêm mới hợp đồng **/
function btnCreateContract() {
    if ($("table#tableDictionary").find('tr.oneselected').length === 0) {
        alert("Vui lòng chọn 1 khách hàng!");
    }
    else {
        var dataPost = { id: $("table#tableDictionary").find('tr.oneselected').find("input[type='checkbox']").val() };

        $.ajax({
            type: "POST",
            url: '/CustomersManage/GetIdCustomer',
            data: JSON.stringify(dataPost),
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            success: function (data) {
                    $("#insert-tag-contract").select2();
                    $("#insert-currency-dukien").select2();
                    $("#insert-currency-contract").select2();
                    $("#insert-permission-contract").select2();
                    $("#insert-status-contract").select2();
                    $("#insert-tour-contract").select2();
                    $("#modal-insert-contract").modal("show");
                    checkCodeContract();
            }
        });
    }
}

/** thêm mới chương trình **/
function btnCreateProgram() {
    if ($("table#tableDictionary").find('tr.oneselected').length === 0) {
        alert("Vui lòng chọn 1 khách hàng!");
    }
    else {
        var dataPost = { id: $("table#tableDictionary").find('tr.oneselected').find("input[type='checkbox']").val() };

        $.ajax({
            type: "POST",
            url: '/CustomersManage/GetIdCustomer',
            data: JSON.stringify(dataPost),
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            success: function (data) {
                $("#insert-tag-program").select2();
                $("#insert-tour-program").select2();
                $("#insert-tour-status").select2();
                $("#insert-name-program").select2({ tags: true });
                $("#modal-insert-program").modal("show");
            }
        });
    }
}

/** thêm mới báo giá **/
function btnCreateQuotation() {
    if ($("table#tableDictionary").find('tr.oneselected').length === 0) {
        alert("Vui lòng chọn 1 khách hàng!");
    }
    else {
        var dataPost = { id: $("table#tableDictionary").find('tr.oneselected').find("input[type='checkbox']").val() };

        $.ajax({
            type: "POST",
            url: '/CustomersManage/GetIdCustomer',
            data: JSON.stringify(dataPost),
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            success: function (data) {
                $("#insert-country-quotation").select2();
                $("#insert-tour-quotation").select2();
                $("#insert-currency-quotation").select2();
                $("#insert-staff-quotation").select2();
                $("#modal-insert-quotation").modal("show");
            }
        });
    }
}

/** thêm mới Vé máy bay **/
function btnCreateTicket() {
    if ($("table#tableDictionary").find('tr.oneselected').length === 0) {
        alert("Vui lòng chọn 1 khách hàng!");
    }
    else {
        var dataPost = { id: $("table#tableDictionary").find('tr.oneselected').find("input[type='checkbox']").val() };

        $.ajax({
            type: "POST",
            url: '/CustomersManage/GetIdCustomer',
            data: JSON.stringify(dataPost),
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            success: function (data) {
                $("#insert-status-ticket").select2();
                $("#insert-start-ticket").select2();
                $("#insert-end-ticket").select2();
                $("#insert-currency-ticket").select2();
                $("#insert-tour-ticket").select2();
                $("#insert-type-ticket").select2();
                $("#modal-insert-ticket").modal("show");
                checkCodeTicket();
            }
        });
    }
}

/** xóa tour **/
function deleteTour(id) {
    if (confirm('Bạn thực sự muốn xóa mục này ?')) {
        var dataPost = { id: id };
        $.ajax({
            type: "POST",
            url: '/CustomerOtherTab/DeleteTour',
            data: JSON.stringify(dataPost),
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            success: function (data) {
                alert("Xóa dữ liệu thành công!!!");
                $("#tourtuyen").html(data);
            }
        });
    }
}

/** xóa hợp đồng **/
function deleteContract(id) {
    if (confirm('Bạn thực sự muốn xóa mục này ?')) {
        var dataPost = { id: id };
        $.ajax({
            type: "POST",
            url: '/CustomerOtherTab/DeleteContract',
            data: JSON.stringify(dataPost),
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            success: function (data) {
                alert("Xóa dữ liệu thành công!!!");
                $("#hopdong").html(data);
            }
        });
    }
}

/** xóa chương trình **/
function deleteProgram(id) {
    if (confirm('Bạn thực sự muốn xóa mục này ?')) {
        var dataPost = { id: id };
        $.ajax({
            type: "POST",
            url: '/CustomerOtherTab/DeleteProgram',
            data: JSON.stringify(dataPost),
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            success: function (data) {
                alert("Xóa dữ liệu thành công!!!");
                $("#chuongtrinh").html(data);
            }
        });
    }
}

/** xóa báo giá **/
function deleteQuotation(id) {
    if (confirm('Bạn thực sự muốn xóa mục này ?')) {
        var dataPost = { id: id };
        $.ajax({
            type: "POST",
            url: '/CustomerOtherTab/DeleteQuotation',
            data: JSON.stringify(dataPost),
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            success: function (data) {
                alert("Xóa dữ liệu thành công!!!");
                $("#baogia").html(data);
            }
        });
    }
}

/** xóa Vé máy bay **/
function deleteTicket(id) {
    if (confirm('Bạn thực sự muốn xóa mục này ?')) {
        var dataPost = { id: id };
        $.ajax({
            type: "POST",
            url: '/CustomerOtherTab/DeleteTicket',
            data: JSON.stringify(dataPost),
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            success: function (data) {
                alert("Xóa dữ liệu thành công!!!");
                $("#codeve").html(data);
            }
        });
    }
}

/** sửa tour **/
function updateTour(id) {
    var dataPost = { id: id };
    $.ajax({
        type: "POST",
        url: '/CustomerOtherTab/EditTour',
        data: JSON.stringify(dataPost),
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (data) {
            if (data == 1) {
                alert('Bạn không được chỉnh sửa tour này!');
            }
            else {
                $("#info-data-tour").html(data);
                $("#edit-start-place").select2();
                $("#edit-destination-place").select2();
                $("#edit-type-tour").select2();
                $("#edit-status-tour").select2();
                $("#edit-manager-tour").select2();
                $("#edit-permission-tour").select2();
                CKEDITOR.replace("edit-request-tour");
                CKEDITOR.replace("edit-noteflight-tour");
                $("#modal-edit-tour").modal("show");
                checkCodeTour();
            }
        }
    });
}

/** sửa hợp đồng **/
function updateContract(id, docId) {
    var dataPost = {
        id: id,
        docId: docId
    };
    $.ajax({
        type: "POST",
        url: '/CustomerOtherTab/EditContract',
        data: JSON.stringify(dataPost),
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (data) {
            if (data == 1) {
                alert('Bạn không được chỉnh sửa hợp đồng này!');
            }
            else {
                $("#info-data-contract").html(data);
                $("#edit-tag-contract").select2();
                $("#edit-currency-dukien").select2();
                $("#edit-currency-contract").select2();
                $("#edit-permission-contract").select2();
                $("#edit-status-contract").select2();
                $("#edit-tour-contract").select2();
                CKEDITOR.replace("edit-note-contract");
                $("#modal-edit-contract").modal("show");
                $('#edit-file-contract').change(function () {
                    var data = new FormData();
                    data.append('FileNameContract', $('#edit-file-contract')[0].files[0]);

                    var ajaxRequest = $.ajax({
                        type: "POST",
                        url: 'CustomerOtherTab/UploadFileContract',
                        contentType: false,
                        processData: false,
                        data: data
                    });

                    ajaxRequest.done(function (xhr, textStatus) {
                        // Onsuccess
                    });
                });
                checkCodeContract();
            }
        }
    });
}

/** sửa chương trình **/
function updateProgram(id) {
    var dataPost = { id: id };
    $.ajax({
        type: "POST",
        url: '/CustomerOtherTab/EditProgram',
        data: JSON.stringify(dataPost),
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (data) {
            if (data == 1) {
                alert('Bạn không được chỉnh sửa chương trình này!');
            }
            else {
                $("#info-data-program").html(data);
                $("#edit-tag-program").select2();
                $("#edit-tour-program").select2();
                $("#edit-tour-status").select2();
                CKEDITOR.replace("edit-program-note");
                $("#modal-edit-program").modal("show");

                $('#edit-file-program').change(function () {
                    var data = new FormData();
                    data.append('FileNameProgram', $('#edit-file-program')[0].files[0]);

                    var ajaxRequest = $.ajax({
                        type: "POST",
                        url: 'CustomerOtherTab/UploadFileProgram',
                        contentType: false,
                        processData: false,
                        data: data
                    });

                    ajaxRequest.done(function (xhr, textStatus) {
                        // Onsuccess
                    });
                });
            }
        }
    });
}

/** sửa báo giá **/
function updateQuotation(id) {
    var dataPost = { id: id };
    $.ajax({
        type: "POST",
        url: '/CustomerOtherTab/EditQuotation',
        data: JSON.stringify(dataPost),
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (data) {
            $("#info-data-quotation").html(data);
            $("#edit-country-quotation").select2();
            $("#edit-tour-quotation").select2();
            $("#edit-currency-quotation").select2();
            $("#edit-staff-quotation").select2();
            CKEDITOR.replace("edit-note-quotation");
            $("#modal-edit-quotation").modal("show");

            /** upload báo giá **/
            $('#edit-file-quotation').change(function () {
                var data = new FormData();
                data.append('FileNameQuotation', $('#edit-file-quotation')[0].files[0]);

                var ajaxRequest = $.ajax({
                    type: "POST",
                    url: 'CustomerOtherTab/UploadFileQuotation',
                    contentType: false,
                    processData: false,
                    data: data
                });

                ajaxRequest.done(function (xhr, textStatus) {
                    // Onsuccess
                });
            });
        }
    });
}

/** sửa Vé máy bay **/
function updateTicket(id) {
    var dataPost = { id: id };
    $.ajax({
        type: "POST",
        url: '/CustomerOtherTab/EditTicket',
        data: JSON.stringify(dataPost),
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (data) {
            $("#info-data-ticket").html(data);
            $("#edit-status-ticket").select2();
            $("#edit-start-ticket").select2();
            $("#edit-end-ticket").select2();
            $("#edit-currency-ticket").select2();
            $("#edit-tour-ticket").select2();
            $("#edit-type-ticket").select2();
            CKEDITOR.replace("edit-note-ticket");
            $("#modal-edit-ticket").modal("show");
            CKupdate();
            checkCodeTicket();
        }
    });
}

function OnFailureCustomer() {
    alert("Lỗi!");
    $("#modal-insert-quotation").modal("hide");
    $("#modal-edit-quotation").modal("hide");
    $("#modal-change-manager").modal("hide");
    $("#modal-insert-contract").modal("hide");
    $("#modal-edit-contract").modal("hide");
    $("#modal-insert-program").modal("hide");
    $("#modal-edit-program").modal("hide");
    $("#modal-insert-tour").modal("hide");
    $("#modal-edit-tour").modal("hide");
    $("#modal-insert-ticket").modal("hide");
    $("#modal-edit-ticket").modal("hide");
    $('form').trigger("reset");
    CKupdate();
}

function OnSuccessCustomer() {
    $("#modal-insert-quotation").modal("hide");
    $("#modal-edit-quotation").modal("hide");
    $("#modal-insert-contract").modal("hide");
    $("#modal-edit-contract").modal("hide");
    $("#modal-insert-program").modal("hide");
    $("#modal-change-manager").modal("hide");
    $("#modal-edit-program").modal("hide");
    $("#modal-insert-tour").modal("hide");
    $("#modal-edit-tour").modal("hide");
    $("#modal-insert-ticket").modal("hide");
    $("#modal-edit-ticket").modal("hide");
    $('form').trigger("reset");
    alert("Đã lưu!");
    CKupdate();
}

function checkCodeTour() {
    var dataPost = { code: $("#CodeTour").val() };
    $.ajax({
        type: "POST",
        url: '/TourManage/CheckCodeTour',
        data: JSON.stringify(dataPost),
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (data) {
            if (data == 0) {
                $("#CodeTour").val('');
                $("#CodeTour").focus();
                alert('Trùng code tour');
            }
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

$('#insert-name-program').change(function () {
    var dataPost = { id: $("#insert-name-program").val() };

    $.ajax({
        type: "POST",
        url: '/CustomerOtherTab/LoadProgram',
        data: JSON.stringify(dataPost),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            if (data != "0") {
                $("#insert-code-program").val(data.Code);
                $('#insert-tour-status').val(data.StatusId).select2('destroy').select2();
                $('#insert-tour-program').val(data.TourId).select2('destroy').select2();
            }
        }
    });
});

function checkCodeContract() {
    var dataPost = { code: $("#CodeContract").val() };
    $.ajax({
        type: "POST",
        url: '/CustomerOtherTab/CheckCodeContract',
        data: JSON.stringify(dataPost),
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (data) {
            if (data == 0) {
                $("#CodeContract").val('');
                $("#CodeContract").focus();
                alert('Trùng code hợp đồng');
            }
        }
    });
}

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

/* upload file nhân viên */
$('#insert-file-staff').change(function () {
    var data = new FormData();
    var totalFiles = document.getElementById("insert-file-staff").files.length;
    for (var i = 0; i < totalFiles; i++) {
        var file = document.getElementById("insert-file-staff").files[i];
        data.append('FileStaff', file);
    }

    var ajaxRequest = $.ajax({
        type: "POST",
        url: 'CustomerOtherTab/UploadMultipleFileStaff',
        contentType: false,
        processData: false,
        data: data
    });

    ajaxRequest.done(function (xhr, textStatus) {
        // Onsuccess
    });
});

/**** upload image nhân viên ****/
$('#insert-image-staff').change(function () {
    var data = new FormData();
    data.append('Image', $('#insert-image-staff')[0].files[0]);

    var ajaxRequest = $.ajax({
        type: "POST",
        url: 'CustomerOtherTab/UploadImage',
        contentType: false,
        processData: false,
        data: data
    });

    ajaxRequest.done(function (xhr, textStatus) {
        // Onsuccess
    });
});