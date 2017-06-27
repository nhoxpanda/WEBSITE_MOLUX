CKEDITOR.replace("insert-note-company");
CKEDITOR.replace("insert-note-personal");
$("#insert-ngayhen-lichhen").val(moment(new Date()).format("YYYY-MM-DD") + "T08:30");
$("#ddlKyBaoCao").select2();

/*** visa-passport ***/
$("#country-insert-cmnd").select2();
$("#country-insert-passport").select2();

/*** doanh nghiệp ***/
$("#ddlCustomerType").select2();
$("#ddlNguonDen").select2();
$("#ddlNhomKH").select2();
$("#ddlKyHopDong").select2();
$("#insert-address-company").select2();
$("#customer-nhomkh-company").select2();
$("#customer-nguonden-company").select2();
$("#edit-customer-company").select2();
$("#customer-quanly-company").select2();
$("#insert-detail-company").select2({
    tags: true
});

/*** cá nhân ***/
$("#insert-address-personal").select2();
$("#customer-nghenghiep-personal").select2();
$("#customer-nguonden-personal").select2();
$("#customer-nhomkh-personal").select2();
$("#customer-quydanh").select2();
$("#customer-quanly-personal").select2();
$("#customer-company").select2();
$("#insert-nganhnghe-other").select2();
$("#insert-address-othercompany").select2();
$("#insert-detail-personal").select2({
    tags: true
});

/*** người liên hệ ***/
$("#customer-contact").select2();
$("#customer-quydanh-contact").select2();
$("#insert-address-contact").select2();
$("#country-insert-profilevisa").select2();
$("#countryvisa1").select2();

var table = $('.dataTable').DataTable({
    order: [],
    columnDefs: [{ orderable: false, targets: [0] }]
});

$(".dataTable").dataTable().columnFilter({
    sPlaceHolder: "head:after",
    aoColumns: [null,//1
                { type: "text" },//2
                { type: "text" },//3
                { type: "text" },//4
                { type: "text" },//5
                { type: "text" },
                { type: "text" },
                { type: "text" },//6
                { type: "text" },//7
                { type: "text" },//8
                { type: "text" },//9
                { type: "text" },//10
                { type: "text" },//11
                { type: "text" },//12
                { type: "text" },//13
                { type: "text" },//14
                { type: "text" },//15
                { type: "text" },//16
                { type: "text" },//17
                { type: "text" },//18
                { type: "text" },//19
                { type: "text" },//20
                { type: "text" }]//21
});

$('a.toggle-vis').on('click', function (e) {
    e.preventDefault();
    if ($(this).hasClass('selected')) {
        $(this).removeClass('selected');
    }
    else {
        $(this).addClass('selected');
    }
    // Get the column API object
    var column = table.column($(this).attr('data-column'));
    // Toggle the visibility
    column.visible(!column.visible());
});

function radCompanyClick() {
    $('#detail-company').show(); $('#detail-personal').hide();
}

function radPersonalClick() {
    $('#detail-company').hide(); $('#detail-personal').show();
}

///*** duplicate form visa ***/
$(function () {
    $('#btnAdd').click(function () {
        var num = $('.clonedInput').length, // how many "duplicatable" input fields we currently have
            newNum = new Number(num + 1),      // the numeric ID of the new input field being added
            newElem = $('#entry' + num).clone().attr('id', 'entry' + newNum).fadeIn('slow'); // create the new element via clone(), and manipulate it's ID using newNum value
        // manipulate the name/id values of the input inside the new element

        newElem.find('.visacard').attr('name', 'VisaNumber' + newNum).attr('id', 'VisaNumber' + newNum).val('');
        newElem.find('.ngaycapvisa').attr('id', 'ngaycapvisa' + newNum).attr('name', 'CreatedDateVisa' + newNum).val('');
        newElem.find('.ngayhethanvisa').attr('id', 'ngayhethanvisa' + newNum).attr('name', 'ExpiredDateVisa' + newNum).val('');
        newElem.find('.countryvisa').attr('id', 'countryvisa' + newNum).attr('name', 'TagsId' + newNum).val('');

        // insert the new element after the last "duplicatable" input field
        $('#entry' + num).after(newElem);
        $("#countryvisa" + newNum).select2();

        for (var i = 1; i < newNum; i++) {
            $("#entry" + newNum + " #select2-countryvisa" + i + "-container").parent().parent().parent().remove();
        }

        /** check visa **/
        $("#VisaNumber" + newNum).change(function () {
            var dataPost = {
                text: $("#VisaNumber" + newNum).val()
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
                            $("#VisaNumber" + newNum).val('');
                            $("#VisaNumber" + newNum).focus();
                        }
                    }
                }
            });
        });

        // enable the "remove" button
        $('#btnDel').attr('disabled', false);

    });
    $('#btnDel').click(function () {
        // confirmation
        var num = $('.clonedInput').length;
        // how many "duplicatable" input fields we currently have
        $('#entry' + num).slideUp('slow', function () {
            $(this).remove();
            // if only one element remains, disable the "remove" button
            if (num - 1 === 1)
                $('#btnDel').attr('disabled', true);
            // enable the "add" button
            $('#btnAdd').attr('disabled', false).prop('value', "add section");
        });
        return false;

        $('#btnAdd').attr('disabled', false);
    });
    $('#btnDel').attr('disabled', true);
});

/** chọn từng dòng để hiển thị thông tin chi tiết dưới các tab **/
$("table#tableDictionary").delegate("tr", "click", function (e) {
    $('tr').not(this).removeClass('oneselected');
    $(this).toggleClass('oneselected');
    var tab = $(".tab-content").find('.active').data("id");
    var dataPost = { id: $(this).find("input[type='checkbox']").val() };
    switch (tab) {
        case 'thongtinchitiet':
            $.ajax({
                type: "POST",
                url: '/CustomerTabInfo/InfoThongTinChiTiet',
                data: JSON.stringify(dataPost),
                contentType: "application/json; charset=utf-8",
                dataType: "html",
                success: function (data) {
                    $("#thongtinchitiet").html(data);
                }
            });
            break;
        case 'lichhen':
            $.ajax({
                type: "POST",
                url: '/CustomerTabInfo/InfoLichHen',
                data: JSON.stringify(dataPost),
                contentType: "application/json; charset=utf-8",
                dataType: "html",
                success: function (data) {
                    $("#lichhen").html(data);
                }
            });
            break;
        case 'tourtuyen':
            $.ajax({
                type: "POST",
                url: '/CustomerTabInfo/InfoTourTuyen',
                data: JSON.stringify(dataPost),
                contentType: "application/json; charset=utf-8",
                dataType: "html",
                success: function (data) {
                    $("#tourtuyen").html(data);
                }
            });
            break;
        case 'nguoilienhe':
            $.ajax({
                type: "POST",
                url: '/CustomerTabInfo/InfoNguoiLienHe',
                data: JSON.stringify(dataPost),
                contentType: "application/json; charset=utf-8",
                dataType: "html",
                success: function (data) {
                    $("#nguoilienhe").html(data);
                }
            });
            break;
        case 'visa':
            $.ajax({
                type: "POST",
                url: '/CustomerTabInfo/InfoVisa',
                data: JSON.stringify(dataPost),
                contentType: "application/json; charset=utf-8",
                dataType: "html",
                success: function (data) {
                    $("#visa").html(data);
                }
            });
            break;
        case 'hosolienquan':
            $.ajax({
                type: "POST",
                url: '/CustomerTabInfo/InfoHoSoLienQuan',
                data: JSON.stringify(dataPost),
                contentType: "application/json; charset=utf-8",
                dataType: "html",
                success: function (data) {
                    $("#hosolienquan").html(data);
                }
            });
            break;
        case 'phanhoikhachhang':
            $.ajax({
                type: "POST",
                url: '/CustomerTabInfo/InfoPhanHoiKhachHang',
                data: JSON.stringify(dataPost),
                contentType: "application/json; charset=utf-8",
                dataType: "html",
                success: function (data) {
                    $("#phanhoikhachhang").html(data);
                }
            });
            break;
        case 'email':
            $.ajax({
                type: "POST",
                url: '/CustomerTabInfo/InfoEmail',
                data: JSON.stringify(dataPost),
                contentType: "application/json; charset=utf-8",
                dataType: "html",
                success: function (data) {
                    $("#email").html(data);
                }
            });
            break;
        case 'lichsulienhe':
            $.ajax({
                type: "POST",
                url: '/CustomerTabInfo/InfoLichSuLienHe',
                data: JSON.stringify(dataPost),
                contentType: "application/json; charset=utf-8",
                dataType: "html",
                success: function (data) {
                    $("#lichsulienhe").html(data);
                }
            });
            break;
        case 'capnhatthaydoi':
            $.ajax({
                type: "POST",
                url: '/CustomerTabInfo/InfoCapNhatThayDoi',
                data: JSON.stringify(dataPost),
                contentType: "application/json; charset=utf-8",
                dataType: "html",
                success: function (data) {
                    $("#capnhatthaydoi").html(data);
                }
            });
            break;
        case 'hopdong':
            $.ajax({
                type: "POST",
                url: '/CustomerTabInfo/InfoHopDong',
                data: JSON.stringify(dataPost),
                contentType: "application/json; charset=utf-8",
                dataType: "html",
                success: function (data) {
                    $("#hopdong").html(data);
                }
            });
            break;
        case 'chuongtrinh':
            $.ajax({
                type: "POST",
                url: '/CustomerTabInfo/InfoChuongTrinh',
                data: JSON.stringify(dataPost),
                contentType: "application/json; charset=utf-8",
                dataType: "html",
                success: function (data) {
                    $("#chuongtrinh").html(data);
                }
            });
            break;
        case 'baogia':
            $.ajax({
                type: "POST",
                url: '/CustomerTabInfo/InfoBaoGia',
                data: JSON.stringify(dataPost),
                contentType: "application/json; charset=utf-8",
                dataType: "html",
                success: function (data) {
                    $("#baogia").html(data);
                }
            });
            break;
        case 'codeve':
            $.ajax({
                type: "POST",
                url: '/CustomerTabInfo/InfoCodeVe',
                data: JSON.stringify(dataPost),
                contentType: "application/json; charset=utf-8",
                dataType: "html",
                success: function (data) {
                    $("#codeve").html(data);
                }
            });
            break;
        case 'thethanhvien':
            $.ajax({
                type: "POST",
                url: '/CustomerTabInfo/InfoTheThanhVien',
                data: JSON.stringify(dataPost),
                contentType: "application/json; charset=utf-8",
                dataType: "html",
                success: function (data) {
                    $("#thethanhvien").html(data);
                }
            });
            break;
    }
});

/** click chọn từng tab -> hiển thị thông tin **/
$("#tabthongtinchitiet").click(function () {
    if ($("table#tableDictionary").find('tr.oneselected').length === 0) {
        alert("Vui lòng chọn 1 khách hàng!");
    }
    else {
        var dataPost = { id: $("table#tableDictionary").find('tr.oneselected').find("input[type='checkbox']").val() };
        $.ajax({
            type: "POST",
            url: '/CustomerTabInfo/InfoThongTinChiTiet',
            data: JSON.stringify(dataPost),
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            success: function (data) {
                $("#thongtinchitiet").html(data);
            }
        });
    }
});

$("#tablichhen").click(function () {
    if ($("table#tableDictionary").find('tr.oneselected').length === 0) {
        alert("Vui lòng chọn 1 khách hàng!");
    }
    else {
        var dataPost = { id: $("table#tableDictionary").find('tr.oneselected').find("input[type='checkbox']").val() };
        $.ajax({
            type: "POST",
            url: '/CustomerTabInfo/InfoLichHen',
            data: JSON.stringify(dataPost),
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            success: function (data) {
                $("#lichhen").html(data);
            }
        });
    }
});

$("#tabtourtuyen").click(function () {
    if ($("table#tableDictionary").find('tr.oneselected').length === 0) {
        alert("Vui lòng chọn 1 khách hàng!");
    }
    else {
        var dataPost = { id: $("table#tableDictionary").find('tr.oneselected').find("input[type='checkbox']").val() };
        $.ajax({
            type: "POST",
            url: '/CustomerTabInfo/InfoTourTuyen',
            data: JSON.stringify(dataPost),
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            success: function (data) {
                $("#tourtuyen").html(data);
            }
        });
    }
});

$("#tabvisa").click(function () {
    if ($("table#tableDictionary").find('tr.oneselected').length === 0) {
        alert("Vui lòng chọn 1 khách hàng!");
    }
    else {
        var dataPost = { id: $("table#tableDictionary").find('tr.oneselected').find("input[type='checkbox']").val() };
        $.ajax({
            type: "POST",
            url: '/CustomerTabInfo/InfoVisa',
            data: JSON.stringify(dataPost),
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            success: function (data) {
                $("#visa").html(data);
            }
        });
    }
});

$("#tabhosolienquan").click(function () {
    if ($("table#tableDictionary").find('tr.oneselected').length === 0) {
        alert("Vui lòng chọn 1 khách hàng!");
    }
    else {
        var dataPost = { id: $("table#tableDictionary").find('tr.oneselected').find("input[type='checkbox']").val() };
        $.ajax({
            type: "POST",
            url: '/CustomerTabInfo/InfoHoSoLienQuan',
            data: JSON.stringify(dataPost),
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            success: function (data) {
                $("#hosolienquan").html(data);
            }
        });
    }
});

$("#tabemail").click(function () {
    if ($("table#tableDictionary").find('tr.oneselected').length === 0) {
        alert("Vui lòng chọn 1 khách hàng!");
    }
    else {
        var dataPost = { id: $("table#tableDictionary").find('tr.oneselected').find("input[type='checkbox']").val() };
        $.ajax({
            type: "POST",
            url: '/CustomerTabInfo/InfoEmail',
            data: JSON.stringify(dataPost),
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            success: function (data) {
                $("#email").html(data);
            }
        });
    }
});

$("#tabsms").click(function () {
    if ($("table#tableDictionary").find('tr.oneselected').length === 0) {
        alert("Vui lòng chọn 1 khách hàng!");
    }
    else {
        var dataPost = { id: $("table#tableDictionary").find('tr.oneselected').find("input[type='checkbox']").val() };
        $.ajax({
            type: "POST",
            url: '/CustomerTabInfo/InfoSMS',
            data: JSON.stringify(dataPost),
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            success: function (data) {
                $("#sms").html(data);
            }
        });
    }
});

$("#tablichsulienhe").click(function () {
    if ($("table#tableDictionary").find('tr.oneselected').length === 0) {
        alert("Vui lòng chọn 1 khách hàng!");
    }
    else {
        var dataPost = { id: $("table#tableDictionary").find('tr.oneselected').find("input[type='checkbox']").val() };
        $.ajax({
            type: "POST",
            url: '/CustomerTabInfo/InfoLichSuLienHe',
            data: JSON.stringify(dataPost),
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            success: function (data) {
                $("#lichsulienhe").html(data);
            }
        });
    }
});

$("#tabcapnhatthaydoi").click(function () {
    if ($("table#tableDictionary").find('tr.oneselected').length === 0) {
        alert("Vui lòng chọn 1 khách hàng!");
    }
    else {
        var dataPost = { id: $("table#tableDictionary").find('tr.oneselected').find("input[type='checkbox']").val() };
        $.ajax({
            type: "POST",
            url: '/CustomerTabInfo/InfoCapNhatThayDoi',
            data: JSON.stringify(dataPost),
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            success: function (data) {
                $("#capnhatthaydoi").html(data);
            }
        });
    }
});

$("#tabphanhoikhachhang").click(function () {
    if ($("table#tableDictionary").find('tr.oneselected').length === 0) {
        alert("Vui lòng chọn 1 khách hàng!");
    }
    else {
        var dataPost = { id: $("table#tableDictionary").find('tr.oneselected').find("input[type='checkbox']").val() };
        $.ajax({
            type: "POST",
            url: '/CustomerTabInfo/InfoPhanHoiKhachHang',
            data: JSON.stringify(dataPost),
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            success: function (data) {
                $("#phanhoikhachhang").html(data);
            }
        });
    }
});

$("#tabnguoilienhe").click(function () {
    if ($("table#tableDictionary").find('tr.oneselected').length === 0) {
        alert("Vui lòng chọn 1 khách hàng!");
    }
    else {
        var dataPost = { id: $("table#tableDictionary").find('tr.oneselected').find("input[type='checkbox']").val() };
        $.ajax({
            type: "POST",
            url: '/CustomerTabInfo/InfoNguoiLienHe',
            data: JSON.stringify(dataPost),
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            success: function (data) {
                $("#nguoilienhe").html(data);
            }
        });
    }
});

$("#tabhopdong").click(function () {
    if ($("table#tableDictionary").find('tr.oneselected').length === 0) {
        alert("Vui lòng chọn 1 khách hàng!");
    }
    else {
        var dataPost = { id: $("table#tableDictionary").find('tr.oneselected').find("input[type='checkbox']").val() };
        $.ajax({
            type: "POST",
            url: '/CustomerTabInfo/InfoHopDong',
            data: JSON.stringify(dataPost),
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            success: function (data) {
                $("#hopdong").html(data);
            }
        });
    }
});

$("#tabchuongtrinh").click(function () {
    if ($("table#tableDictionary").find('tr.oneselected').length === 0) {
        alert("Vui lòng chọn 1 khách hàng!");
    }
    else {
        var dataPost = { id: $("table#tableDictionary").find('tr.oneselected').find("input[type='checkbox']").val() };
        $.ajax({
            type: "POST",
            url: '/CustomerTabInfo/InfoChuongTrinh',
            data: JSON.stringify(dataPost),
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            success: function (data) {
                $("#chuongtrinh").html(data);
            }
        });
    }
});

$("#tabbaogia").click(function () {
    if ($("table#tableDictionary").find('tr.oneselected').length === 0) {
        alert("Vui lòng chọn 1 khách hàng!");
    }
    else {
        var dataPost = { id: $("table#tableDictionary").find('tr.oneselected').find("input[type='checkbox']").val() };
        $.ajax({
            type: "POST",
            url: '/CustomerTabInfo/InfoBaoGia',
            data: JSON.stringify(dataPost),
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            success: function (data) {
                $("#baogia").html(data);
            }
        });
    }
});

$("#tabcodeve").click(function () {
    if ($("table#tableDictionary").find('tr.oneselected').length === 0) {
        alert("Vui lòng chọn 1 khách hàng!");
    }
    else {
        var dataPost = { id: $("table#tableDictionary").find('tr.oneselected').find("input[type='checkbox']").val() };
        $.ajax({
            type: "POST",
            url: '/CustomerTabInfo/InfoCodeVe',
            data: JSON.stringify(dataPost),
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            success: function (data) {
                $("#codeve").html(data);
            }
        });
    }
});

$("#tabthethanhvien").click(function () {
    if ($("table#tableDictionary").find('tr.oneselected').length === 0) {
        alert("Vui lòng chọn 1 khách hàng!");
    }
    else {
        var dataPost = { id: $("table#tableDictionary").find('tr.oneselected').find("input[type='checkbox']").val() };
        $.ajax({
            type: "POST",
            url: '/CustomerTabInfo/InfoTheThanhVien',
            data: JSON.stringify(dataPost),
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            success: function (data) {
                $("#thethanhvien").html(data);
            }
        });
    }
});

/** xóa tài liệu **/
function deleteDocument(id) {
    if (confirm('Bạn thực sự muốn xóa mục này ?')) {
        var dataPost = { id: id };
        $.ajax({
            type: "POST",
            url: '/CustomersManage/DeleteDocument',
            data: JSON.stringify(dataPost),
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            success: function (data) {
                alert("Xóa dữ liệu thành công!!!");
                $("#hosolienquan").html(data);
            }
        });
    }
}

/** cập nhật tài liệu **/
function updateDocument(id) {
    var dataPost = { id: id };
    $.ajax({
        type: "POST",
        url: '/CustomersManage/EditInfoDocument',
        data: JSON.stringify(dataPost),
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (data) {
            $("#info-data-doc").html(data);
            $("#edit-tag-document").select2();
            $("#edit-document-type").select2();
            CKEDITOR.replace("edit-document-note");
            $("#modal-edit-document").modal("show");
            /**** update in tab file của khách hàng ****/
            $("#btnUpdateFile").click(function () {
                var $this = $(this);
                var $form = $("#frmUpdateFile");
                var $parent = $form.parent();
                var options = {
                    url: $form.attr("action"),
                    type: $form.attr("method"),
                    data: $form.serialize()
                };

                $.ajax(options).done(function (data) {
                    $("#modal-edit-document").modal("hide");
                    alert("Lưu dữ liệu thành công!");
                    $("#hosolienquan").html(data);
                });
                return false;
            });

            /** upload file **/
            $("#edit-file").change(function () {
                var data = new FormData();
                data.append('FileName', $('#edit-file')[0].files[0]);
                var ajaxRequest = $.ajax({
                    type: "POST",
                    url: 'CustomersManage/UploadFile',
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

/**** insert in tab visa của khách hàng ****/
$("#btnInsertVisa").click(function () {
    var $this = $(this);
    var $form = $("#frmInsertVisa");
    var $parent = $form.parent();
    var options = {
        url: $form.attr("action"),
        type: $form.attr("method"),
        data: $form.serialize()
    };

    $.ajax(options).done(function (data) {
        $("#modal-insert-visa").modal("hide");
        alert("Lưu dữ liệu thành công!");
        $("#visa").html(data);
    });
    return false;
})

/** thêm mới visa **/
function btnCreateVisa() {
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
                $("#insert-country-visa").select2();
                $("#insert-type-visa").select2();
                $("#insert-status-visa").select2();
                $("#modal-insert-visa").modal("show");
                checkCodeVisa();
            }
        });
    }
}

/** xóa visa **/
function deleteVisa(id) {
    if (confirm('Bạn thực sự muốn xóa mục này ?')) {
        var dataPost = { id: id };
        $.ajax({
            type: "POST",
            url: '/CustomersManage/DeleteVisa',
            data: JSON.stringify(dataPost),
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            success: function (data) {
                alert("Xóa dữ liệu thành công!!!");
                $("#visa").html(data);
            }
        });
    }
}

/** cập nhật visa **/
function updateVisa(id) {
    var dataPost = { id: id };
    $.ajax({
        type: "POST",
        url: '/CustomersManage/EditInfoVisa',
        data: JSON.stringify(dataPost),
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (data) {
            $("#info-data-visa").html(data);
            $("#edit-country-visa").select2();
            $("#edit-type-visa").select2();
            $("#edit-status-visa").select2();
            //$("#edit-createdate-visa").datepicker();
            //$("#edit-expiredate-visa").datepicker();
            $("#modal-edit-visa").modal("show");

            /**** update in tab visa của khách hàng ****/
            $("#btnUpdateVisa").click(function () {
                var $this = $(this);
                var $form = $("#frmUpdateVisa");
                var $parent = $form.parent();
                var options = {
                    url: $form.attr("action"),
                    type: $form.attr("method"),
                    data: $form.serialize()
                };

                $.ajax(options).done(function (data) {
                    $("#modal-edit-visa").modal("hide");
                    alert("Lưu dữ liệu thành công!");
                    $("#visa").html(data);
                });
                return false;
            })

            checkCodeVisa();
        }
    });
}

///****** Chuyển quyền quản lý ******/
$("#btnManage").click(function () {
    var dataPost = {
        id: $("input[type='checkbox']:checked").val()
    };
    $.ajax({
        type: "POST",
        url: '/CustomersManage/ChangeStaffManage',
        data: JSON.stringify(dataPost),
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (data) {
            $("#info-data-customer").html(data);
            $("#customer-change-manager").select2();
            $("#modal-change-manager").modal("show");
        }
    });
});

///****** Sửa thông tin ******/
$("#btnEdit").click(function () {
    var dataPost = {
        id: $("input[type='checkbox']:checked").val()
    };

    $.ajax({
        type: "POST",
        url: '/CustomersManage/CustomerInfomation',
        data: JSON.stringify(dataPost),
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (data) {
            $("#info-data-customer").html(data);

            /*** visa-passport ***/
            $("#country-edit-cmnd").select2();
            $("#country-edit-passport").select2();

            /*** doanh nghiệp ***/
            $("#edit-address-company").select2();
            $("#edit-nhomkh-company").select2();
            $("#edit-nguonden-company").select2();
            $("#edit-quanly-company").select2();
            $("#edit-company").select2();
            $("#edit-customer-career").select2();

            /*** cá nhân ***/
            $("#edit-personal-quydanh").select2();
            $("#edit-address-personal").select2();
            $("#edit-nghenghiep-personal").select2();
            $("#edit-quanly-personal").select2();
            $("#edit-nhomkh-personal").select2();
            $("#edit-nguonden-personal").select2();
            $("#customer-company-edit").select2();
            $("#edit-nganhnghe-other").select2();
            $("#edit-address-othercompany").select2();

            ///*** modal ***/
            CKEDITOR.replace("edit-note-company");
            CKEDITOR.replace("edit-note-personal");
            $("#modal-edit-customer").modal("show");

            ///*** duplicate form visa (edit) ***/
            $(function () {
                $('#btnAddE').click(function () {
                    var num = $('.clonedInputE').length, // how many "duplicatable" input fields we currently have
                        newNum = new Number(num + 1),      // the numeric ID of the new input field being added
                        newElem = $('#entryE' + num).clone().attr('id', 'entryE' + newNum).fadeIn('slow'); // create the new element via clone(), and manipulate it's ID using newNum value
                    // manipulate the name/id values of the input inside the new element

                    newElem.find('.visacardE').attr('name', 'VisaNumber' + newNum).val('');
                    newElem.find('.ngaycapvisaE').attr('id', 'ngaycapvisaE' + newNum).attr('name', 'CreatedDateVisa' + newNum).val('');
                    newElem.find('.ngayhethanvisaE').attr('id', 'ngayhethanvisaE' + newNum).attr('name', 'ExpiredDateVisa' + newNum).val('');
                    newElem.find('.countryvisaE').attr('id', 'countryvisaE' + newNum).attr('name', 'TagsId' + newNum).val('');

                    // insert the new element after the last "duplicatable" input field
                    $('#entryE' + num).after(newElem);
                    //$("#ngaycapvisaE" + newNum).datepicker();
                    //$("#ngayhethanvisaE" + newNum).datepicker();
                    $("#countryvisaE" + newNum).select2();

                    for (var i = 1; i < newNum; i++) {
                        $("#entryE" + newNum + " #select2-countryvisaE" + i + "-container").parent().parent().parent().remove();
                    }

                    // enable the "remove" button
                    $('#btnDelE').attr('disabled', false);

                });

                $('#btnDelE').click(function () {
                    // confirmation
                    var num = $('.clonedInputE').length;
                    // how many "duplicatable" input fields we currently have
                    $('#entryE' + num).slideUp('slow', function () {
                        $(this).remove();
                        // if only one element remains, disable the "remove" button
                        if (num - 1 === 1)
                            $('#btnDelE').attr('disabled', true);
                        // enable the "add" button
                        $('#btnAddE').attr('disabled', false).prop('value', "add section");
                    });
                    return false;

                    $('#btnAddE').attr('disabled', false);
                });


                $('#btnDelE').attr('disabled', true);
            });
        }
    });
});

/** success ajax form **/
function OnSuccessCustomerFile() {
    $("#modal-insert-document").modal("hide");
    $("#modal-edit-document").modal("hide");
    $('form').trigger("reset");
    CKupdate();
}

/** failure ajax form **/
function OnFailureCustomerFile() {
    alert("Lỗi, vui lòng kiểm tra lại!");
    $("#modal-insert-document").modal("hide");
    $("#modal-edit-document").modal("hide");
    $('form').trigger("reset");
    CKupdate();
}

$("#btnExport").click(function () {
    $("#exportForm").submit();
});

$("#check-customer-company").change(function () {
    if ($(this).is(":checked"))
        $('#customer-company').attr('disabled', false);
    else
        $('#customer-company').attr('disabled', true);
})

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

/****** filter khách hàng ******/
$("#btnSearch").click(function () {
    filterCustomer();
})
function filterCustomer() {
    var dataPost = {
        id: $("#ddlCustomerType").val(),
        nguon: $("#ddlNguonDen").val(),
        nhom: $("#ddlNhomKH").val(),
        kyhopdong: $("#ddlKyHopDong").val(),
        tungay: $("#txtStartDate").val(),
        denngay: $("#txtEndDate").val()
    };
    $.ajax({
        type: "POST",
        url: '/CustomersManage/FilterCustomerList',
        data: JSON.stringify(dataPost),
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (data) {
            $("#data-customer").html(data);
            var table = $('.dataTable').DataTable({
                order: [],
                columnDefs: [{ orderable: false, targets: [0] }]
            });
            $(".dataTable").dataTable().columnFilter({
                sPlaceHolder: "head:after",
                aoColumns: [null,//1
                { type: "text" },//2
                { type: "text" },//3
                { type: "text" },//4
                { type: "text" },//5
                { type: "text" },
                { type: "text" },
                { type: "text" },//6
                { type: "text" },//7
                { type: "text" },//8
                { type: "text" },//9
                { type: "text" },//10
                { type: "text" },//11
                { type: "text" },//12
                { type: "text" },//13
                { type: "text" },//14
                { type: "text" },//15
                { type: "text" },//16
                { type: "text" },//17
                { type: "text" },//18
                { type: "text" },//19
                { type: "text" },//20
                { type: "text" }]//21
            });
            $('a.toggle-vis').on('click', function (e) {
                e.preventDefault();
                if ($(this).hasClass('selected')) {
                    $(this).removeClass('selected');
                }
                else {
                    $(this).addClass('selected');
                }
                // Get the column API object
                var column = table.column($(this).attr('data-column'));
                // Toggle the visibility
                column.visible(!column.visible());
            });
            colResize();
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

            ///// select table - tab info /////

            /** chọn từng dòng để hiển thị thông tin chi tiết dưới các tab **/
            $("table#tableDictionary").delegate("tr", "click", function (e) {
                $('tr').not(this).removeClass('oneselected');
                $(this).toggleClass('oneselected');
                var tab = $(".tab-content").find('.active').data("id");
                var dataPost = { id: $(this).find("input[type='checkbox']").val() };
                switch (tab) {
                    case 'thongtinchitiet':
                        $.ajax({
                            type: "POST",
                            url: '/CustomerTabInfo/InfoThongTinChiTiet',
                            data: JSON.stringify(dataPost),
                            contentType: "application/json; charset=utf-8",
                            dataType: "html",
                            success: function (data) {
                                $("#thongtinchitiet").html(data);
                            }
                        });
                        break;
                    case 'lichhen':
                        $.ajax({
                            type: "POST",
                            url: '/CustomerTabInfo/InfoLichHen',
                            data: JSON.stringify(dataPost),
                            contentType: "application/json; charset=utf-8",
                            dataType: "html",
                            success: function (data) {
                                $("#lichhen").html(data);
                            }
                        });
                        break;
                    case 'tourtuyen':
                        $.ajax({
                            type: "POST",
                            url: '/CustomerTabInfo/InfoTourTuyen',
                            data: JSON.stringify(dataPost),
                            contentType: "application/json; charset=utf-8",
                            dataType: "html",
                            success: function (data) {
                                $("#tourtuyen").html(data);
                            }
                        });
                        break;
                    case 'nguoilienhe':
                        $.ajax({
                            type: "POST",
                            url: '/CustomerTabInfo/InfoNguoiLienHe',
                            data: JSON.stringify(dataPost),
                            contentType: "application/json; charset=utf-8",
                            dataType: "html",
                            success: function (data) {
                                $("#nguoilienhe").html(data);
                            }
                        });
                        break;
                    case 'visa':
                        $.ajax({
                            type: "POST",
                            url: '/CustomerTabInfo/InfoVisa',
                            data: JSON.stringify(dataPost),
                            contentType: "application/json; charset=utf-8",
                            dataType: "html",
                            success: function (data) {
                                $("#visa").html(data);
                            }
                        });
                        break;
                    case 'hosolienquan':
                        $.ajax({
                            type: "POST",
                            url: '/CustomerTabInfo/InfoHoSoLienQuan',
                            data: JSON.stringify(dataPost),
                            contentType: "application/json; charset=utf-8",
                            dataType: "html",
                            success: function (data) {
                                $("#hosolienquan").html(data);
                            }
                        });
                        break;
                    case 'phanhoikhachhang':
                        $.ajax({
                            type: "POST",
                            url: '/CustomerTabInfo/InfoPhanHoiKhachHang',
                            data: JSON.stringify(dataPost),
                            contentType: "application/json; charset=utf-8",
                            dataType: "html",
                            success: function (data) {
                                $("#phanhoikhachhang").html(data);
                            }
                        });
                        break;
                    case 'email':
                        $.ajax({
                            type: "POST",
                            url: '/CustomerTabInfo/InfoEmail',
                            data: JSON.stringify(dataPost),
                            contentType: "application/json; charset=utf-8",
                            dataType: "html",
                            success: function (data) {
                                $("#email").html(data);
                            }
                        });
                        break;
                    case 'sms':
                        $.ajax({
                            type: "POST",
                            url: '/CustomerTabInfo/InfoSMS',
                            data: JSON.stringify(dataPost),
                            contentType: "application/json; charset=utf-8",
                            dataType: "html",
                            success: function (data) {
                                $("#sms").html(data);
                            }
                        });
                        break;
                    case 'lichsulienhe':
                        $.ajax({
                            type: "POST",
                            url: '/CustomerTabInfo/InfoLichSuLienHe',
                            data: JSON.stringify(dataPost),
                            contentType: "application/json; charset=utf-8",
                            dataType: "html",
                            success: function (data) {
                                $("#lichsulienhe").html(data);
                            }
                        });
                        break;
                    case 'capnhatthaydoi':
                        $.ajax({
                            type: "POST",
                            url: '/CustomerTabInfo/InfoCapNhatThayDoi',
                            data: JSON.stringify(dataPost),
                            contentType: "application/json; charset=utf-8",
                            dataType: "html",
                            success: function (data) {
                                $("#capnhatthaydoi").html(data);
                            }
                        });
                        break;
                }
            });

            /** click chọn từng tab -> hiển thị thông tin **/
            $("#tabthongtinchitiet").click(function () {
                if ($("table#tableDictionary").find('tr.oneselected').length === 0) {
                    alert("Vui lòng chọn 1 khách hàng!");
                }
                else {
                    var dataPost = { id: $("table#tableDictionary").find('tr.oneselected').find("input[type='checkbox']").val() };
                    $.ajax({
                        type: "POST",
                        url: '/CustomerTabInfo/InfoThongTinChiTiet',
                        data: JSON.stringify(dataPost),
                        contentType: "application/json; charset=utf-8",
                        dataType: "html",
                        success: function (data) {
                            $("#thongtinchitiet").html(data);
                        }
                    });
                }
            });

            $("#tablichhen").click(function () {
                if ($("table#tableDictionary").find('tr.oneselected').length === 0) {
                    alert("Vui lòng chọn 1 khách hàng!");
                }
                else {
                    var dataPost = { id: $("table#tableDictionary").find('tr.oneselected').find("input[type='checkbox']").val() };
                    $.ajax({
                        type: "POST",
                        url: '/CustomerTabInfo/InfoLichHen',
                        data: JSON.stringify(dataPost),
                        contentType: "application/json; charset=utf-8",
                        dataType: "html",
                        success: function (data) {
                            $("#lichhen").html(data);
                        }
                    });
                }
            });

            $("#tabtourtuyen").click(function () {
                if ($("table#tableDictionary").find('tr.oneselected').length === 0) {
                    alert("Vui lòng chọn 1 khách hàng!");
                }
                else {
                    var dataPost = { id: $("table#tableDictionary").find('tr.oneselected').find("input[type='checkbox']").val() };
                    $.ajax({
                        type: "POST",
                        url: '/CustomerTabInfo/InfoTourTuyen',
                        data: JSON.stringify(dataPost),
                        contentType: "application/json; charset=utf-8",
                        dataType: "html",
                        success: function (data) {
                            $("#tourtuyen").html(data);
                        }
                    });
                }
            });

            $("#tabvisa").click(function () {
                if ($("table#tableDictionary").find('tr.oneselected').length === 0) {
                    alert("Vui lòng chọn 1 khách hàng!");
                }
                else {
                    var dataPost = { id: $("table#tableDictionary").find('tr.oneselected').find("input[type='checkbox']").val() };
                    $.ajax({
                        type: "POST",
                        url: '/CustomerTabInfo/InfoVisa',
                        data: JSON.stringify(dataPost),
                        contentType: "application/json; charset=utf-8",
                        dataType: "html",
                        success: function (data) {
                            $("#visa").html(data);
                        }
                    });
                }
            });

            $("#tabhosolienquan").click(function () {
                if ($("table#tableDictionary").find('tr.oneselected').length === 0) {
                    alert("Vui lòng chọn 1 khách hàng!");
                }
                else {
                    var dataPost = { id: $("table#tableDictionary").find('tr.oneselected').find("input[type='checkbox']").val() };
                    $.ajax({
                        type: "POST",
                        url: '/CustomerTabInfo/InfoHoSoLienQuan',
                        data: JSON.stringify(dataPost),
                        contentType: "application/json; charset=utf-8",
                        dataType: "html",
                        success: function (data) {
                            $("#hosolienquan").html(data);
                        }
                    });
                }
            });

            $("#tabemail").click(function () {
                if ($("table#tableDictionary").find('tr.oneselected').length === 0) {
                    alert("Vui lòng chọn 1 khách hàng!");
                }
                else {
                    var dataPost = { id: $("table#tableDictionary").find('tr.oneselected').find("input[type='checkbox']").val() };
                    $.ajax({
                        type: "POST",
                        url: '/CustomerTabInfo/InfoEmail',
                        data: JSON.stringify(dataPost),
                        contentType: "application/json; charset=utf-8",
                        dataType: "html",
                        success: function (data) {
                            $("#email").html(data);
                        }
                    });
                }
            });

            $("#tabsms").click(function () {
                if ($("table#tableDictionary").find('tr.oneselected').length === 0) {
                    alert("Vui lòng chọn 1 khách hàng!");
                }
                else {
                    var dataPost = { id: $("table#tableDictionary").find('tr.oneselected').find("input[type='checkbox']").val() };
                    $.ajax({
                        type: "POST",
                        url: '/CustomerTabInfo/InfoSMS',
                        data: JSON.stringify(dataPost),
                        contentType: "application/json; charset=utf-8",
                        dataType: "html",
                        success: function (data) {
                            $("#sms").html(data);
                        }
                    });
                }
            });

            $("#tablichsulienhe").click(function () {
                if ($("table#tableDictionary").find('tr.oneselected').length === 0) {
                    alert("Vui lòng chọn 1 khách hàng!");
                }
                else {
                    var dataPost = { id: $("table#tableDictionary").find('tr.oneselected').find("input[type='checkbox']").val() };
                    $.ajax({
                        type: "POST",
                        url: '/CustomerTabInfo/InfoLichSuLienHe',
                        data: JSON.stringify(dataPost),
                        contentType: "application/json; charset=utf-8",
                        dataType: "html",
                        success: function (data) {
                            $("#lichsulienhe").html(data);
                        }
                    });
                }
            });

            $("#tabcapnhatthaydoi").click(function () {
                if ($("table#tableDictionary").find('tr.oneselected').length === 0) {
                    alert("Vui lòng chọn 1 khách hàng!");
                }
                else {
                    var dataPost = { id: $("table#tableDictionary").find('tr.oneselected').find("input[type='checkbox']").val() };
                    $.ajax({
                        type: "POST",
                        url: '/CustomerTabInfo/InfoCapNhatThayDoi',
                        data: JSON.stringify(dataPost),
                        contentType: "application/json; charset=utf-8",
                        dataType: "html",
                        success: function (data) {
                            $("#capnhatthaydoi").html(data);
                        }
                    });
                }
            });

            $("#tabphanhoikhachhang").click(function () {
                if ($("table#tableDictionary").find('tr.oneselected').length === 0) {
                    alert("Vui lòng chọn 1 khách hàng!");
                }
                else {
                    var dataPost = { id: $("table#tableDictionary").find('tr.oneselected').find("input[type='checkbox']").val() };
                    $.ajax({
                        type: "POST",
                        url: '/CustomerTabInfo/InfoPhanHoiKhachHang',
                        data: JSON.stringify(dataPost),
                        contentType: "application/json; charset=utf-8",
                        dataType: "html",
                        success: function (data) {
                            $("#phanhoikhachhang").html(data);
                        }
                    });
                }
            });

            $("#tabnguoilienhe").click(function () {
                if ($("table#tableDictionary").find('tr.oneselected').length === 0) {
                    alert("Vui lòng chọn 1 khách hàng!");
                }
                else {
                    var dataPost = { id: $("table#tableDictionary").find('tr.oneselected').find("input[type='checkbox']").val() };
                    $.ajax({
                        type: "POST",
                        url: '/CustomerTabInfo/InfoNguoiLienHe',
                        data: JSON.stringify(dataPost),
                        contentType: "application/json; charset=utf-8",
                        dataType: "html",
                        success: function (data) {
                            $("#nguoilienhe").html(data);
                        }
                    });
                }
            });
        }
    });
}

$("#ddlCustomerType").change(function () {
    $("#loaikh").val($("#ddlCustomerType").val());
    $("#loaikhoffline").val($("#ddlCustomerType").val());
})

$("#ddlNguonDen").change(function () {
    $("#nguonden").val($("#ddlNguonDen").val());
    $("#nguondenoffline").val($("#ddlNguonDen").val());
})

$("#ddlNhomKH").change(function () {
    $("#nhomkh").val($("#ddlNhomKH").val());
    $("#nhomkhoffline").val($("#ddlNhomKH").val());
})

$("#ddlKyHopDong").change(function () {
    $("#kyhopdong").val($("#ddlKyHopDong").val());
    $("#kyhopdongoffline").val($("#ddlKyHopDong").val());
})

$("#txtStartDate").change(function () {
    $("#tungay").val($("#txtStartDate").val());
    $("#tungayoffline").val($("#txtStartDate").val());
})

$("#txtEndDate").change(function () {
    $("#denngay").val($("#txtEndDate").val());
    $("#denngayoffline").val($("#txtEndDate").val());
})

/** check tên khách hàng doanh nghiệp **/
$("#insert-detail-company").change(function () {

    $("#insert-company-name").val($("#insert-detail-company :selected").text());

    var dataPost = {
        text: $("#insert-detail-company").val()
    };
    $.ajax({
        type: "POST",
        url: '/CustomersManage/CheckFullname',
        data: JSON.stringify(dataPost),
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (data) {
            if (data == "1") { // trùng
                if (!confirm("Dữ liệu trùng lắp! Bạn có muốn lưu lại không?")) {
                    $("#insert-detail-company").select2('val', '0');
                    $("#insert-detail-company").focus();
                }
            }
        }
    });
});

/** check tên khách hàng cá nhân **/
$("#insert-detail-personal").change(function () {

    $("#insert-personal-name").val($("#insert-detail-personal :selected").text());

    var dataPost = {
        text: $("#insert-detail-personal").val()
    };
    $.ajax({
        type: "POST",
        url: '/CustomersManage/CheckFullname',
        data: JSON.stringify(dataPost),
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (data) {
            if (data == "1") { // trùng
                if (!confirm("Dữ liệu trùng lắp! Bạn có muốn lưu lại không?")) {
                    $("#insert-detail-personal").select2('val', '0');
                    $("#insert-detail-personal").focus();
                }
            }
        }
    });
});

/** check tên điện thoại **/
$("#dienthoai-cty").change(function () {
    var dataPost = {
        text: $("#dienthoai-cty").val()
    };
    $.ajax({
        type: "POST",
        url: '/CustomersManage/CheckPhone',
        data: JSON.stringify(dataPost),
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (data) {
            if (data == "1") { // trùng
                if (!confirm("Dữ liệu trùng lắp! Bạn có muốn lưu lại không?")) {
                    $("#dienthoai-cty").val('');
                    $("#dienthoai-cty").focus();
                }
            }
        }
    });
});

$("#didong-cty").change(function () {
    var dataPost = {
        text: $("#didong-cty").val()
    };
    $.ajax({
        type: "POST",
        url: '/CustomersManage/CheckPhone',
        data: JSON.stringify(dataPost),
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (data) {
            if (data == "1") { // trùng
                if (!confirm("Dữ liệu trùng lắp! Bạn có muốn lưu lại không?")) {
                    $("#didong-cty").val('');
                    $("#didong-cty").focus();
                }
            }
        }
    });
});

$("#dienthoai-canhan").change(function () {
    var dataPost = {
        text: $("#dienthoai-canhan").val()
    };
    $.ajax({
        type: "POST",
        url: '/CustomersManage/CheckPhone',
        data: JSON.stringify(dataPost),
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (data) {
            if (data == "1") { // trùng
                if (!confirm("Dữ liệu trùng lắp! Bạn có muốn lưu lại không?")) {
                    $("#dienthoai-canhan").val('');
                    $("#dienthoai-canhan").focus();
                }
            }
        }
    });
});

$("#didong-canhan").change(function () {
    var dataPost = {
        text: $("#didong-canhan").val()
    };
    $.ajax({
        type: "POST",
        url: '/CustomersManage/CheckPhone',
        data: JSON.stringify(dataPost),
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (data) {
            if (data == "1") { // trùng
                if (!confirm("Dữ liệu trùng lắp! Bạn có muốn lưu lại không?")) {
                    $("#didong-canhan").val('');
                    $("#didong-canhan").focus();
                }
            }
        }
    });
});

/** check email**/
$("#mailcanhan-cty").change(function () {
    var dataPost = {
        text: $("#mailcanhan-cty").val()
    };
    $.ajax({
        type: "POST",
        url: '/CustomersManage/CheckEmail',
        data: JSON.stringify(dataPost),
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (data) {
            if (data == "1") { // trùng
                if (!confirm("Dữ liệu trùng lắp! Bạn có muốn lưu lại không?")) {
                    $("#mailcanhan-cty").val('');
                    $("#mailcanhan-cty").focus();
                }
            }
        }
    });
});

$("#mailcongty-cty").change(function () {
    var dataPost = {
        text: $("#mailcongty-cty").val()
    };
    $.ajax({
        type: "POST",
        url: '/CustomersManage/CheckEmail',
        data: JSON.stringify(dataPost),
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (data) {
            if (data == "1") { // trùng
                if (!confirm("Dữ liệu trùng lắp! Bạn có muốn lưu lại không?")) {
                    $("#mailcongty-cty").val('');
                    $("#mailcongty-cty").focus();
                }
            }
        }
    });
});

$("#email-canhan").change(function () {
    var dataPost = {
        text: $("#email-canhan").val()
    };
    $.ajax({
        type: "POST",
        url: '/CustomersManage/CheckEmail',
        data: JSON.stringify(dataPost),
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (data) {
            if (data == "1") { // trùng
                if (!confirm("Dữ liệu trùng lắp! Bạn có muốn lưu lại không?")) {
                    $("#email-canhan").val('');
                    $("#email-canhan").focus();
                }
            }
        }
    });
});

/** check cmnd **/
$("#so-cmnd").change(function () {
    var dataPost = {
        text: $("#so-cmnd").val()
    };
    $.ajax({
        type: "POST",
        url: '/CustomersManage/CheckCMND',
        data: JSON.stringify(dataPost),
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (data) {
            if (data == "1") { // trùng
                if (!confirm("Dữ liệu trùng lắp! Bạn có muốn lưu lại không?")) {
                    $("#so-cmnd").val('');
                    $("#so-cmnd").focus();
                }
            }
        }
    });
});

/** check passport **/
$("#so-passport").change(function () {
    var dataPost = {
        text: $("#so-passport").val()
    };
    $.ajax({
        type: "POST",
        url: '/CustomersManage/CheckPassport',
        data: JSON.stringify(dataPost),
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (data) {
            if (data == "1") { // trùng
                if (!confirm("Dữ liệu trùng lắp! Bạn có muốn lưu lại không?")) {
                    $("#so-passport").val('');
                    $("#so-passport").focus();
                }
            }
        }
    });
});

/** check visa **/
$("#VisaNumber1").change(function () {
    var dataPost = {
        text: $("#VisaNumber1").val()
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
                    $("#VisaNumber1").val('');
                    $("#VisaNumber1").focus();
                }
            }
        }
    });
});

function checkCodeVisa() {
    var dataPost = { code: $("#insert-visa-customer").val() };
    $.ajax({
        type: "POST",
        url: '/CustomerOtherTab/CheckCodeVisa',
        data: JSON.stringify(dataPost),
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (data) {
            if (data == 0) {
                $("#insert-visa-customer").val('');
                $("#insert-visa-customer").focus();
                alert('Trùng số thẻ visa');
            }
        }
    });
}

////////// kỳ báo cáo
$("#ddlKyBaoCao").change(function () {
    var dataPost = {
        id: $("#ddlKyBaoCao").val()
    };
    $.ajax({
        type: "POST",
        url: '/ReportsManage/GetStartEndDate',
        data: JSON.stringify(dataPost),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            $("#startEndDate").html(data);
            $("#tungay").val($("#txtStartDate").val());
            $("#denngay").val($("#txtEndDate").val());
            $("#txtStartDate").change(function () {
                $("#tungay").val($("#txtStartDate").val());
                $("#tungayoffline").val($("#txtStartDate").val());
            })

            $("#txtEndDate").change(function () {
                $("#denngay").val($("#txtEndDate").val());
                $("#denngayoffline").val($("#txtEndDate").val());
            })
        }
    });
})

function OnFailureMemberCard() {
    $("#modal-update-card").modal("hide");
    $("#modal-update-point").modal("hide");
    alert("Lỗi, vui lòng xem lại!");
}

function OnSuccessMemberCard() {
    $("#modal-update-card").modal("hide");
    $("#modal-update-point").modal("hide");
    alert("Đã lưu!");
}

$("#btnUpdateCard").click(function () {
    var dataPost = {
        id: $("input[type='checkbox']:checked").val()
    };
    $.ajax({
        type: "POST",
        url: '/MemberCardHistoryManage/EditCard',
        data: JSON.stringify(dataPost),
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (data) {
            $("#info-data-customer").html(data);
            $("#update-membercard").select2();
            $("#modal-update-card").modal("show");
        }
    });
});

$("#btnUpdatePoint").click(function () {
    var dataPost = {
        id: $("input[type='checkbox']:checked").val()
    };
    $.ajax({
        type: "POST",
        url: '/MemberCardHistoryManage/EditPoint',
        data: JSON.stringify(dataPost),
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (data) {
            $("#info-data-customer").html(data);
            $("#update-membercard").select2();
            $("#modal-update-point").modal("show");
        }
    });
});
function isNumberKey(evt) {
    var charCode = (evt.which) ? evt.which : evt.keyCode
    return !(charCode > 31 && (charCode < 48 || charCode > 57));
}
function checkValidCustomerPersonal() {
    var errorText = "Không nhập kí tự đặc biệt và khoảng trắng!";
    var span = $("#_customerPersonalSpan");
    var input = $("#_customerPersonalCode");
    var regex = /^[a-zA-Z0-9]+$/;
    var checkname = CheckValidInput(input, regex, span, errorText);
    var inputVal = $('#_customerPersonalCode').val().trim();
    if (checkname) {
        $.ajax({
            type: "POST",
            url: '/CustomersManage/checkCodeCustomer',
            data: { code: inputVal },
            success: function (data) {
                if (data.check) {
                    span.text("Mã khách hàng đã tồn tại!");
                    input.removeClass("valid").addClass("invalid");
                    return false;
                }
                else {
                    span.text("");
                    input.removeClass("invalid").addClass("valid");
                    return true;
                }
            }
        });
    }
    else {
        return false;
    }
}

function CheckMobilePer() {
    var inputLength = $("#didong-canhan").val().length;
    var input = $("#didong-canhan");
    var span = $("#_customerPerMobileSpan");
    if (inputLength < 10) {
        span.text("Vui lòng nhập 10 - 11 kí tự!");
        input.removeClass("valid").addClass("invalid");
        return false;
    }
    else {
        span.text("");
        input.removeClass("invalid").addClass("valid");
        return true;
    }
}
function CheckPhonePer() {
    var inputLength = $("#dienthoai-canhan").val().length;
    var input = $("#dienthoai-canhan");
    var span = $("#_customerPerPhoneSpan");
    if (inputLength < 10) {
        span.text("Vui lòng nhập 10 - 11 kí tự!");
        input.removeClass("valid").addClass("invalid");
        return false;
    }
    else {
        span.text("");
        input.removeClass("invalid").addClass("valid");
        return true;
    }
}
function CheckMobileCom() {
    var inputLength = $("#didong-cty").val().length;
    var input = $("#didong-cty");
    var span = $("#_customerComMobileSpan");
    if (inputLength < 10) {
        span.text("Vui lòng nhập 10 - 11 kí tự!");
        input.removeClass("valid").addClass("invalid");
        return false;
    }
    else {
        span.text("");
        input.removeClass("invalid").addClass("valid");
        return true;
    }
}
function CheckPhoneCom() {
    var inputLength = $("#dienthoai-cty").val().length;
    var input = $("#dienthoai-cty");
    var span = $("#_customerComPhoneSpan");
    if (inputLength < 10) {
        span.text("Vui lòng nhập 10 - 11 kí tự!");
        input.removeClass("valid").addClass("invalid");
        return false;
    }
    else {
        span.text("");
        input.removeClass("invalid").addClass("valid");
        return true;
    }
}

function checkValidCustomerCompany() {
    var errorText = "Không nhập kí tự đặc biệt và khoảng trắng!";
    var span = $("#_customerCompanySpan");
    var input = $("#_customerCompanyCode");
    var regex = /^[a-zA-Z0-9]+$/;
    var checkname = CheckValidInput(input, regex, span, errorText);
    var inputVal = $('#_customerCompanyCode').val().trim();
    if (checkname) {
        $.ajax({
            type: "POST",
            url: '/CustomersManage/checkCodeCustomer',
            data: { code: inputVal },
            success: function (data) {
                if (data.check) {
                    span.text("Mã khách hàng đã tồn tại!");
                    input.removeClass("valid").addClass("invalid");
                    return false;
                }
                else {
                    span.text("");
                    input.removeClass("invalid").addClass("valid");
                    return true;
                }
            }
        });
    }
    else {
        return false;
    }
}



//$('#_customerCompanyCode').on('input', function () {
    
//});

function CheckValidInput(input, regex, span, errorText) {
    var is_name = regex.test(input.val());
    if (input.val().trim().length > 0) {
        if (is_name) {
            input.removeClass("invalid").addClass("valid");
            span.text("");
            return true;
        }
        else {
            input.removeClass("valid").addClass("invalid");
            span.text(errorText);
            return false;
        }
    }
    else {
        input.removeClass("valid").addClass("invalid");
        span.text("Không được bỏ trống");
        return false;
    }
}