$("#ddlKyBaoCao").select2();
$("#insert-manager-tour").select2();
$("#insert-start-place").select2();
$("#insert-destination-place").select2();
$("#insert-type-tour").select2();
$("#insert-guide-tour").select2();
$("#insert-startplace-tourguide").select2();
$("#insert-permission-tour").select2();
$("#insert-customer-tour").select2();
$("#insert-task-type").select2();
$("#insert-department-tasktour").select2();
$("#insert-staff-tasktour").select2();
$("#insert-priority-task").select2();
$("#insert-timetype-task").select2();
$("#update-type-tour").select2();
$("#insert-guide-tour1").select2();
$("#insert-status-tour").select2();
$("#insert-nhiemvu-ngaynhac").val(moment(new Date()).format("YYYY-MM-DD") + "T08:30");

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
                { type: "text" },
                { type: "text" },
                { type: "text" },
                { type: "text" }]
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

///***** cập nhật tour *****/
$("#btnEdit").click(function () {
    var dataPost = {
        id: $("input[type='checkbox']:checked").val()
    };

    $.ajax({
        type: "POST",
        url: '/TourBiddingManage/TourInfomation',
        data: JSON.stringify(dataPost),
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (data) {
            if (data == 1) {
                alert('Bạn không được chỉnh sửa tour này!');
            }
            else {
                $("#info-data-tour").html(data);
                $("#edit-manager-tour").select2();
                $("#edit-customer-tour").select2();
                $("#edit-start-place").select2();
                $("#edit-destination-place").select2();
                $("#edit-type-tour").select2();
                $("#edit-guide-tour").select2();
                $("#edit-startplace-tourguide").select2();
                $("#edit-status-tour").select2();
                $("#edit-permission-tour").select2();
                CKEDITOR.replace("edit-note-tour");
                CKEDITOR.replace("edit-noteflight-tour");
                $("#modal-edit-tour").modal("show");
            }
        }
    })
});

///***** xóa tour *****/
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

/** chọn từng dòng để hiển thị thông tin chi tiết dưới các tab **/
$("table#tableDictionary").delegate("tr", "click", function (e) {
    $('tr').not(this).removeClass('oneselected');
    $(this).toggleClass('oneselected');
    var tab = $(".tab-content").find('.active').data("id");
    var dataPost = { id: $(this).find("input[type='checkbox']").val() };
    switch (tab) {
        case 'chitiettour':
            $.ajax({
                type: "POST",
                url: '/TourTabInfo/InfoChiTietTour',
                data: JSON.stringify(dataPost),
                contentType: "application/json; charset=utf-8",
                dataType: "html",
                success: function (data) {
                    $("#chitiettour").html(data);
                }
            });
            break;
        case 'huongdanvien':
            $.ajax({
                type: "POST",
                url: '/TourTabInfo/InfoHuongDanVien',
                data: JSON.stringify(dataPost),
                contentType: "application/json; charset=utf-8",
                dataType: "html",
                success: function (data) {
                    $("#huongdanvien").html(data);
                }
            });
            break;
        case 'lichhen':
            $.ajax({
                type: "POST",
                url: '/TourTabInfo/InfoLichHen',
                data: JSON.stringify(dataPost),
                contentType: "application/json; charset=utf-8",
                dataType: "html",
                success: function (data) {
                    $("#lichhen").html(data);
                }
            });
            break;
        case 'dichvu':
            $.ajax({
                type: "POST",
                url: '/TourTabInfo/InfoDichVu',
                data: JSON.stringify(dataPost),
                contentType: "application/json; charset=utf-8",
                dataType: "html",
                success: function (data) {
                    $("#dichvu").html(data);
                }
            });
            break;
        case 'khachhang':
            $.ajax({
                type: "POST",
                url: '/TourTabInfo/InfoKhachHang',
                data: JSON.stringify(dataPost),
                contentType: "application/json; charset=utf-8",
                dataType: "html",
                success: function (data) {
                    $("#khachhang").html(data);
                }
            });
            break;
        case 'visa':
            $.ajax({
                type: "POST",
                url: '/TourTabInfo/InfoVisa',
                data: JSON.stringify(dataPost),
                contentType: "application/json; charset=utf-8",
                dataType: "html",
                success: function (data) {
                    $("#visa").html(data);
                    ////////
                    $("#btnUpdateVisa").click(function () {
                        var items = '';
                        $("input.cbVisa:checked").each(function () {
                            items += $(this).val() + ',';
                        })
                        $("#listVisaId").val(items);
                        CKEDITOR.replace("edit-note-visa");
                        $("#modal-update-visa").modal('show');
                    })
                }
            });
            break;
        case 'tailieumau':
            $.ajax({
                type: "POST",
                url: '/TourTabInfo/InfoTaiLieuMau',
                data: JSON.stringify(dataPost),
                contentType: "application/json; charset=utf-8",
                dataType: "html",
                success: function (data) {
                    $("#tailieumau").html(data);
                }
            });
            break;
        case 'nhiemvu':
            $.ajax({
                type: "POST",
                url: '/TourTabInfo/InfoNhiemVu',
                data: JSON.stringify(dataPost),
                contentType: "application/json; charset=utf-8",
                dataType: "html",
                success: function (data) {
                    $("#nhiemvu").html(data);
                }
            });
            break;
        case 'chuongtrinh':
            $.ajax({
                type: "POST",
                url: '/TourTabInfo/InfoChuongTrinh',
                data: JSON.stringify(dataPost),
                contentType: "application/json; charset=utf-8",
                dataType: "html",
                success: function (data) {
                    $("#chuongtrinh").html(data);
                }
            });
            break;
        case 'hopdong':
            $.ajax({
                type: "POST",
                url: '/TourTabInfo/InfoHopDong',
                data: JSON.stringify(dataPost),
                contentType: "application/json; charset=utf-8",
                dataType: "html",
                success: function (data) {
                    $("#hopdong").html(data);
                }
            });
            break;
        case 'lichsulienhe':
            $.ajax({
                type: "POST",
                url: '/TourTabInfo/InfoLichSuLienHe',
                data: JSON.stringify(dataPost),
                contentType: "application/json; charset=utf-8",
                dataType: "html",
                success: function (data) {
                    $("#lichsulienhe").html(data);
                }
            });
            break;
        case 'viettourbaogia':
            $.ajax({
                type: "POST",
                url: '/TourTabInfo/InfoViettourBaoGia',
                data: JSON.stringify(dataPost),
                contentType: "application/json; charset=utf-8",
                dataType: "html",
                success: function (data) {
                    $("#viettourbaogia").html(data);
                }
            });
            break;
        case 'congnokh':
            $.ajax({
                type: "POST",
                url: '/TourTabInfo/InfoCongNoKH',
                data: JSON.stringify(dataPost),
                contentType: "application/json; charset=utf-8",
                dataType: "html",
                success: function (data) {
                    $("#congnokh").html(data);
                }
            });
            break;
        case 'congnodoitac':
            $.ajax({
                type: "POST",
                url: '/TourTabInfo/InfoCongNoDoiTac',
                data: JSON.stringify(dataPost),
                contentType: "application/json; charset=utf-8",
                dataType: "html",
                success: function (data) {
                    $("#congnodoitac").html(data);
                }
            });
            break;
        case 'danhgia':
            $.ajax({
                type: "POST",
                url: '/TourTabInfo/InfoDanhGia',
                data: JSON.stringify(dataPost),
                contentType: "application/json; charset=utf-8",
                dataType: "html",
                success: function (data) {
                    $("#danhgia").html(data);
                }
            });
            break;
        case 'capnhatthaydoi':
            $.ajax({
                type: "POST",
                url: '/TourTabInfo/InfoCapNhatThayDoi',
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
$("#tabchitiettour").click(function () {
    if ($("table#tableDictionary").find('tr.oneselected').length === 0) {
        alert("Vui lòng chọn 1 tour!");
    }
    else {
        var dataPost = { id: $("table#tableDictionary").find('tr.oneselected').find("input[type='checkbox']").val() };
        $.ajax({
            type: "POST",
            url: '/TourTabInfo/InfoChiTietTour',
            data: JSON.stringify(dataPost),
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            success: function (data) {
                $("#chitiettour").html(data);
            }
        });
    }
});

$("#tablichhen").click(function () {
    if ($("table#tableDictionary").find('tr.oneselected').length === 0) {
        alert("Vui lòng chọn 1 tour!");
    }
    else {
        var dataPost = { id: $("table#tableDictionary").find('tr.oneselected').find("input[type='checkbox']").val() };
        $.ajax({
            type: "POST",
            url: '/TourTabInfo/InfoLichHen',
            data: JSON.stringify(dataPost),
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            success: function (data) {
                $("#lichhen").html(data);
            }
        });
    }
});

$("#tabhuongdanvien").click(function () {
    if ($("table#tableDictionary").find('tr.oneselected').length === 0) {
        alert("Vui lòng chọn 1 tour!");
    }
    else {
        var dataPost = { id: $("table#tableDictionary").find('tr.oneselected').find("input[type='checkbox']").val() };
        $.ajax({
            type: "POST",
            url: '/TourTabInfo/InfoHuongDanVien',
            data: JSON.stringify(dataPost),
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            success: function (data) {
                $("#huongdanvien").html(data);
            }
        });
    }
});

$("#tabkhachhang").click(function () {
    if ($("table#tableDictionary").find('tr.oneselected').length === 0) {
        alert("Vui lòng chọn 1 tour!");
    }
    else {
        var dataPost = { id: $("table#tableDictionary").find('tr.oneselected').find("input[type='checkbox']").val() };
        $.ajax({
            type: "POST",
            url: '/TourTabInfo/InfoKhachHang',
            data: JSON.stringify(dataPost),
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            success: function (data) {
                $("#khachhang").html(data);
            }
        });
    }
});

$("#tabvisa").click(function () {
    if ($("table#tableDictionary").find('tr.oneselected').length === 0) {
        alert("Vui lòng chọn 1 tour!");
    }
    else {
        var dataPost = { id: $("table#tableDictionary").find('tr.oneselected').find("input[type='checkbox']").val() };
        $.ajax({
            type: "POST",
            url: '/TourTabInfo/InfoVisa',
            data: JSON.stringify(dataPost),
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            success: function (data) {
                $("#visa").html(data);
                ///////////
                $("#btnUpdateVisa").click(function () {
                    var items = '';
                    $("input.cbVisa:checked").each(function () {
                        items += $(this).val() + ',';
                    })
                    $("#listVisaId").val(items);
                    $("#modal-update-visa").modal('show')
                })
            }
        });
    }
});

$("#tabtailieumau").click(function () {
    if ($("table#tableDictionary").find('tr.oneselected').length === 0) {
        alert("Vui lòng chọn 1 tour!");
    }
    else {
        var dataPost = { id: $("table#tableDictionary").find('tr.oneselected').find("input[type='checkbox']").val() };
        $.ajax({
            type: "POST",
            url: '/TourTabInfo/InfoTaiLieuMau',
            data: JSON.stringify(dataPost),
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            success: function (data) {
                $("#tailieumau").html(data);
            }
        });
    }
});

$("#tabchuongtrinh").click(function () {
    if ($("table#tableDictionary").find('tr.oneselected').length === 0) {
        alert("Vui lòng chọn 1 tour!");
    }
    else {
        var dataPost = { id: $("table#tableDictionary").find('tr.oneselected').find("input[type='checkbox']").val() };
        $.ajax({
            type: "POST",
            url: '/TourTabInfo/InfoChuongTrinh',
            data: JSON.stringify(dataPost),
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            success: function (data) {
                $("#chuongtrinh").html(data);
            }
        });
    }
});

$("#tabhopdong").click(function () {
    if ($("table#tableDictionary").find('tr.oneselected').length === 0) {
        alert("Vui lòng chọn 1 tour!");
    }
    else {
        var dataPost = { id: $("table#tableDictionary").find('tr.oneselected').find("input[type='checkbox']").val() };
        $.ajax({
            type: "POST",
            url: '/TourTabInfo/InfoHopDong',
            data: JSON.stringify(dataPost),
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            success: function (data) {
                $("#hopdong").html(data);
            }
        });
    }
});

$("#tablichsulienhe").click(function () {
    if ($("table#tableDictionary").find('tr.oneselected').length === 0) {
        alert("Vui lòng chọn 1 tour!");
    }
    else {
        var dataPost = { id: $("table#tableDictionary").find('tr.oneselected').find("input[type='checkbox']").val() };
        $.ajax({
            type: "POST",
            url: '/TourTabInfo/InfoLichSuLienHe',
            data: JSON.stringify(dataPost),
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            success: function (data) {
                $("#lichsulienhe").html(data);
            }
        });
    }
});

$("#tabdichvu").click(function () {
    if ($("table#tableDictionary").find('tr.oneselected').length === 0) {
        alert("Vui lòng chọn 1 tour!");
    }
    else {
        var dataPost = { id: $("table#tableDictionary").find('tr.oneselected').find("input[type='checkbox']").val() };
        $.ajax({
            type: "POST",
            url: '/TourTabInfo/InfoDichVu',
            data: JSON.stringify(dataPost),
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            success: function (data) {
                $("#dichvu").html(data);
            }
        });
    }
});

$("#tabnhiemvu").click(function () {
    if ($("table#tableDictionary").find('tr.oneselected').length === 0) {
        alert("Vui lòng chọn 1 tour!");
    }
    else {
        var dataPost = { id: $("table#tableDictionary").find('tr.oneselected').find("input[type='checkbox']").val() };
        $.ajax({
            type: "POST",
            url: '/TourTabInfo/InfoNhiemVu',
            data: JSON.stringify(dataPost),
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            success: function (data) {
                $("#nhiemvu").html(data);
            }
        });
    }
});

$("#tabdanhgia").click(function () {
    if ($("table#tableDictionary").find('tr.oneselected').length === 0) {
        alert("Vui lòng chọn 1 tour!");
    }
    else {
        var dataPost = { id: $("table#tableDictionary").find('tr.oneselected').find("input[type='checkbox']").val() };
        $.ajax({
            type: "POST",
            url: '/TourTabInfo/InfoDanhGia',
            data: JSON.stringify(dataPost),
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            success: function (data) {
                $("#danhgia").html(data);
            }
        });
    }
});

$("#tabviettourbaogia").click(function () {
    if ($("table#tableDictionary").find('tr.oneselected').length === 0) {
        alert("Vui lòng chọn 1 tour!");
    }
    else {
        var dataPost = { id: $("table#tableDictionary").find('tr.oneselected').find("input[type='checkbox']").val() };
        $.ajax({
            type: "POST",
            url: '/TourTabInfo/InfoViettourBaoGia',
            data: JSON.stringify(dataPost),
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            success: function (data) {
                $("#viettourbaogia").html(data);
            }
        });
    }
});

$("#tabcongnokh").click(function () {
    if ($("table#tableDictionary").find('tr.oneselected').length === 0) {
        alert("Vui lòng chọn 1 tour!");
    }
    else {
        var dataPost = { id: $("table#tableDictionary").find('tr.oneselected').find("input[type='checkbox']").val() };
        $.ajax({
            type: "POST",
            url: '/TourTabInfo/InfoCongNoKH',
            data: JSON.stringify(dataPost),
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            success: function (data) {
                $("#congnokh").html(data);
            }
        });
    }
});

$("#tabcongnodoitac").click(function () {
    if ($("table#tableDictionary").find('tr.oneselected').length === 0) {
        alert("Vui lòng chọn 1 tour!");
    }
    else {
        var dataPost = { id: $("table#tableDictionary").find('tr.oneselected').find("input[type='checkbox']").val() };
        $.ajax({
            type: "POST",
            url: '/TourTabInfo/InfoCongNoDoiTac',
            data: JSON.stringify(dataPost),
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            success: function (data) {
                $("#congnodoitac").html(data);
            }
        });
    }
});

$("#tabcapnhatthaydoi").click(function () {
    if ($("table#tableDictionary").find('tr.oneselected').length === 0) {
        alert("Vui lòng chọn 1 tour!");
    }
    else {
        var dataPost = { id: $("table#tableDictionary").find('tr.oneselected').find("input[type='checkbox']").val() };
        $.ajax({
            type: "POST",
            url: '/TourTabInfo/InfoCapNhatThayDoi',
            data: JSON.stringify(dataPost),
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            success: function (data) {
                $("#capnhatthaydoi").html(data);
            }
        });
    }
});

/** filter loại tour **/
$("#select-type-tour").change(function () {
    var dataPost = {
        id: $("#select-type-tour").val(),
        sltu: $("#txtSLTu").val(),
        slden: $("#txtSLDen").val(),
        tungay: $("#txtStartDate").val(),
        denngay: $("#txtEndDate").val()
    };
    $.ajax({
        type: "POST",
        url: '/TourBiddingManage/FilterTour',
        data: JSON.stringify(dataPost),
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (data) {
            $("#data-tour").html(data);

            loadTabInfo();

            /****************/
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
                            { type: "text" },
                            { type: "text" },
                            { type: "text" },
                            { type: "text" },
                            { type: "text" }]
            });
            // kéo dài kích thước cột
            colResize();
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
        }
    });
});

/**  filter ngày và só lượng **/
$("#btnSearch").click(function () {
    var dataPost = {
        id: $("#select-type-tour").val(),
        sltu: $("#txtSLTu").val(),
        slden: $("#txtSLDen").val(),
        tungay: $("#txtStartDate").val(),
        denngay: $("#txtEndDate").val(),
        codinh: $('input#chkTourFixed').is(':checked') ? 1 : 0
    };
    $.ajax({
        type: "POST",
        url: '/TourBiddingManage/FilterTour',
        data: JSON.stringify(dataPost),
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (data) {
            $("#data-tour").html(data);

            loadTabInfo();

            /****************/
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
                            { type: "text" },
                            { type: "text" },
                            { type: "text" },
                            { type: "text" },
                            { type: "text" }]
            });
            // kéo dài kích thước cột
            colResize();
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

            /****************/
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
        }
    });
});

/** popup insert lịch đi tour **/
$("#btnAddSchedule").click(function () {
    var dataPost = { id: $("table#tableDictionary").find("input[type='checkbox']:checked").val() };
    $.ajax({
        type: "POST",
        url: '/TourBiddingManage/GetIdTour',
        data: JSON.stringify(dataPost),
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (data) {
            $("#modal-insert-tourschedule").modal('show');

            $('#insert-partner-tour').change(function () {
                $.getJSON('/TourService/LoadPartner/' + $('#insert-partner-tour').val(), function (data) {
                    $('#Address').val(data.Address);
                });
            });

        }
    });
})

/** popup insert task tour **/
$("#btnAddTask").click(function () {
    var dataPost = { id: $("table#tableDictionary").find("input[type='checkbox']:checked").val() };
    $.ajax({
        type: "POST",
        url: '/TourBiddingManage/GetIdTour',
        data: JSON.stringify(dataPost),
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (data) {
            $("#modal-insert-tourtask").modal('show');
            CKEDITOR.replace("insert-schedule-tour");
        }
    });
})

/** popup update status tour **/
$("#btnUpdateStatus").click(function () {
    var dataPost = { id: $("table#tableDictionary").find("input[type='checkbox']:checked").val() };
    $.ajax({
        type: "POST",
        url: '/TourBiddingManage/GetInfoStatus',
        data: JSON.stringify(dataPost),
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (data) {
            $("#info-status").html(data);
            $("#update-status").select2();
            $("#modal-update-status").modal('show');
        }
    });
})

$('#insert-department-tasktour').change(function () {
    $.getJSON('/TourBiddingManage/LoadPermission/' + $('#insert-department-tasktour').val(), function (data) {
        var items = '<option>-- Chọn nhân viên thực hiện --</option>';
        $.each(data, function (i, ward) {
            items += "<option value='" + ward.Value + "'>" + ward.Text + "</option>";
        });
        $('#insert-staff-tasktour').html(items);
    });
});

/** popup update type tour **/
$("#btnUpdateType").click(function () {
    var dataPost = { id: $("table#tableDictionary").find('tr.oneselected').find("input[type='checkbox']").val() };
    $.ajax({
        type: "POST",
        url: '/TourBiddingManage/GetIdTour',
        data: JSON.stringify(dataPost),
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (data) {
            $.ajax({
                type: "POST",
                url: '/TourBiddingManage/UpdateTypeTour',
                data: JSON.stringify(dataPost),
                contentType: "application/json; charset=utf-8",
                dataType: "html",
                success: function () {
                    alert('Đã chuyển!');
                }
            });
        }
    });
})

function OnFailureTour() {
    alert("Lỗi...!");
    $("#modal-insert-tourtask").modal('hide');
    $("#modal-insert-tourschedule").modal('hide');
    $("#modal-edit-tourtask").modal('hide');
    $("#modal-update-status").modal('hide');
    $("#modal-visa-update").modal('hide');
    CKupdate();

    /** chọn từng dòng để hiển thị thông tin chi tiết dưới các tab **/
    $("table#tableDictionary").delegate("tr", "click", function (e) {
        $('tr').not(this).removeClass('oneselected');
        $(this).toggleClass('oneselected');
        var tab = $(".tab-content").find('.active').data("id");
        var dataPost = { id: $(this).find("input[type='checkbox']").val() };
        switch (tab) {
            case 'chitiettour':
                $.ajax({
                    type: "POST",
                    url: '/TourTabInfo/InfoChiTietTour',
                    data: JSON.stringify(dataPost),
                    contentType: "application/json; charset=utf-8",
                    dataType: "html",
                    success: function (data) {
                        $("#chitiettour").html(data);
                    }
                });
                break;
            case 'lichhen':
                $.ajax({
                    type: "POST",
                    url: '/TourTabInfo/InfoLichHen',
                    data: JSON.stringify(dataPost),
                    contentType: "application/json; charset=utf-8",
                    dataType: "html",
                    success: function (data) {
                        $("#lichhen").html(data);
                    }
                });
                break;
            case 'dichvu':
                $.ajax({
                    type: "POST",
                    url: '/TourTabInfo/InfoDichVu',
                    data: JSON.stringify(dataPost),
                    contentType: "application/json; charset=utf-8",
                    dataType: "html",
                    success: function (data) {
                        $("#dichvu").html(data);
                    }
                });
                break;
            case 'khachhang':
                $.ajax({
                    type: "POST",
                    url: '/TourTabInfo/InfoKhachHang',
                    data: JSON.stringify(dataPost),
                    contentType: "application/json; charset=utf-8",
                    dataType: "html",
                    success: function (data) {
                        $("#khachhang").html(data);
                    }
                });
                break;
            case 'visa':
                $.ajax({
                    type: "POST",
                    url: '/TourTabInfo/InfoVisa',
                    data: JSON.stringify(dataPost),
                    contentType: "application/json; charset=utf-8",
                    dataType: "html",
                    success: function (data) {
                        $("#visa").html(data);
                        ////////
                        $("#btnUpdateVisa").click(function () {
                            var items = '';
                            $("input.cbVisa:checked").each(function () {
                                items += $(this).val() + ',';
                            })
                            $("#listVisaId").val(items);
                            CKEDITOR.replace("edit-note-visa");
                            $("#modal-update-visa").modal('show');
                        })
                    }
                });
                break;
            case 'tailieumau':
                $.ajax({
                    type: "POST",
                    url: '/TourTabInfo/InfoTaiLieuMau',
                    data: JSON.stringify(dataPost),
                    contentType: "application/json; charset=utf-8",
                    dataType: "html",
                    success: function (data) {
                        $("#tailieumau").html(data);
                    }
                });
                break;
            case 'nhiemvu':
                $.ajax({
                    type: "POST",
                    url: '/TourTabInfo/InfoNhiemVu',
                    data: JSON.stringify(dataPost),
                    contentType: "application/json; charset=utf-8",
                    dataType: "html",
                    success: function (data) {
                        $("#nhiemvu").html(data);
                    }
                });
                break;
            case 'chuongtrinh':
                $.ajax({
                    type: "POST",
                    url: '/TourTabInfo/InfoChuongTrinh',
                    data: JSON.stringify(dataPost),
                    contentType: "application/json; charset=utf-8",
                    dataType: "html",
                    success: function (data) {
                        $("#chuongtrinh").html(data);
                    }
                });
                break;
            case 'hopdong':
                $.ajax({
                    type: "POST",
                    url: '/TourTabInfo/InfoHopDong',
                    data: JSON.stringify(dataPost),
                    contentType: "application/json; charset=utf-8",
                    dataType: "html",
                    success: function (data) {
                        $("#hopdong").html(data);
                    }
                });
                break;
            case 'lichsulienhe':
                $.ajax({
                    type: "POST",
                    url: '/TourTabInfo/InfoLichSuLienHe',
                    data: JSON.stringify(dataPost),
                    contentType: "application/json; charset=utf-8",
                    dataType: "html",
                    success: function (data) {
                        $("#lichsulienhe").html(data);
                    }
                });
                break;
            case 'viettourbaogia':
                $.ajax({
                    type: "POST",
                    url: '/TourTabInfo/InfoViettourBaoGia',
                    data: JSON.stringify(dataPost),
                    contentType: "application/json; charset=utf-8",
                    dataType: "html",
                    success: function (data) {
                        $("#viettourbaogia").html(data);
                    }
                });
                break;
            case 'congnokh':
                $.ajax({
                    type: "POST",
                    url: '/TourTabInfo/InfoCongNoKH',
                    data: JSON.stringify(dataPost),
                    contentType: "application/json; charset=utf-8",
                    dataType: "html",
                    success: function (data) {
                        $("#congnokh").html(data);
                    }
                });
                break;
            case 'congnodoitac':
                $.ajax({
                    type: "POST",
                    url: '/TourTabInfo/InfoCongNoDoiTac',
                    data: JSON.stringify(dataPost),
                    contentType: "application/json; charset=utf-8",
                    dataType: "html",
                    success: function (data) {
                        $("#congnodoitac").html(data);
                    }
                });
                break;
            case 'danhgia':
                $.ajax({
                    type: "POST",
                    url: '/TourTabInfo/InfoDanhGia',
                    data: JSON.stringify(dataPost),
                    contentType: "application/json; charset=utf-8",
                    dataType: "html",
                    success: function (data) {
                        $("#danhgia").html(data);
                    }
                });
                break;
        }
    });
}

function OnSuccessTour() {
    $("#modal-insert-tourtask").modal('hide');
    $("#modal-insert-tourschedule").modal('hide');
    $("#modal-edit-tourtask").modal('hide');
    $("#modal-update-status").modal('hide');
    $("#modal-visa-update").modal('hide');
    alert("Đã lưu!");
    $('form').trigger("reset");
    CKupdate();

    /** chọn từng dòng để hiển thị thông tin chi tiết dưới các tab **/
    $("table#tableDictionary").delegate("tr", "click", function (e) {
        $('tr').not(this).removeClass('oneselected');
        $(this).toggleClass('oneselected');
        var tab = $(".tab-content").find('.active').data("id");
        var dataPost = { id: $(this).find("input[type='checkbox']").val() };
        switch (tab) {
            case 'chitiettour':
                $.ajax({
                    type: "POST",
                    url: '/TourTabInfo/InfoChiTietTour',
                    data: JSON.stringify(dataPost),
                    contentType: "application/json; charset=utf-8",
                    dataType: "html",
                    success: function (data) {
                        $("#chitiettour").html(data);
                    }
                });
                break;
            case 'lichhen':
                $.ajax({
                    type: "POST",
                    url: '/TourTabInfo/InfoLichHen',
                    data: JSON.stringify(dataPost),
                    contentType: "application/json; charset=utf-8",
                    dataType: "html",
                    success: function (data) {
                        $("#lichhen").html(data);
                    }
                });
                break;
            case 'dichvu':
                $.ajax({
                    type: "POST",
                    url: '/TourTabInfo/InfoDichVu',
                    data: JSON.stringify(dataPost),
                    contentType: "application/json; charset=utf-8",
                    dataType: "html",
                    success: function (data) {
                        $("#dichvu").html(data);
                    }
                });
                break;
            case 'khachhang':
                $.ajax({
                    type: "POST",
                    url: '/TourTabInfo/InfoKhachHang',
                    data: JSON.stringify(dataPost),
                    contentType: "application/json; charset=utf-8",
                    dataType: "html",
                    success: function (data) {
                        $("#khachhang").html(data);
                    }
                });
                break;
            case 'visa':
                $.ajax({
                    type: "POST",
                    url: '/TourTabInfo/InfoVisa',
                    data: JSON.stringify(dataPost),
                    contentType: "application/json; charset=utf-8",
                    dataType: "html",
                    success: function (data) {
                        $("#visa").html(data);
                        ////////
                        $("#btnUpdateVisa").click(function () {
                            var items = '';
                            $("input.cbVisa:checked").each(function () {
                                items += $(this).val() + ',';
                            })
                            $("#listVisaId").val(items);
                            CKEDITOR.replace("edit-note-visa");
                            $("#modal-update-visa").modal('show');
                        })
                    }
                });
                break;
            case 'tailieumau':
                $.ajax({
                    type: "POST",
                    url: '/TourTabInfo/InfoTaiLieuMau',
                    data: JSON.stringify(dataPost),
                    contentType: "application/json; charset=utf-8",
                    dataType: "html",
                    success: function (data) {
                        $("#tailieumau").html(data);
                    }
                });
                break;
            case 'nhiemvu':
                $.ajax({
                    type: "POST",
                    url: '/TourTabInfo/InfoNhiemVu',
                    data: JSON.stringify(dataPost),
                    contentType: "application/json; charset=utf-8",
                    dataType: "html",
                    success: function (data) {
                        $("#nhiemvu").html(data);
                    }
                });
                break;
            case 'chuongtrinh':
                $.ajax({
                    type: "POST",
                    url: '/TourTabInfo/InfoChuongTrinh',
                    data: JSON.stringify(dataPost),
                    contentType: "application/json; charset=utf-8",
                    dataType: "html",
                    success: function (data) {
                        $("#chuongtrinh").html(data);
                    }
                });
                break;
            case 'hopdong':
                $.ajax({
                    type: "POST",
                    url: '/TourTabInfo/InfoHopDong',
                    data: JSON.stringify(dataPost),
                    contentType: "application/json; charset=utf-8",
                    dataType: "html",
                    success: function (data) {
                        $("#hopdong").html(data);
                    }
                });
                break;
            case 'lichsulienhe':
                $.ajax({
                    type: "POST",
                    url: '/TourTabInfo/InfoLichSuLienHe',
                    data: JSON.stringify(dataPost),
                    contentType: "application/json; charset=utf-8",
                    dataType: "html",
                    success: function (data) {
                        $("#lichsulienhe").html(data);
                    }
                });
                break;
            case 'viettourbaogia':
                $.ajax({
                    type: "POST",
                    url: '/TourTabInfo/InfoViettourBaoGia',
                    data: JSON.stringify(dataPost),
                    contentType: "application/json; charset=utf-8",
                    dataType: "html",
                    success: function (data) {
                        $("#viettourbaogia").html(data);
                    }
                });
                break;
            case 'congnokh':
                $.ajax({
                    type: "POST",
                    url: '/TourTabInfo/InfoCongNoKH',
                    data: JSON.stringify(dataPost),
                    contentType: "application/json; charset=utf-8",
                    dataType: "html",
                    success: function (data) {
                        $("#congnokh").html(data);
                    }
                });
                break;
            case 'congnodoitac':
                $.ajax({
                    type: "POST",
                    url: '/TourTabInfo/InfoCongNoDoiTac',
                    data: JSON.stringify(dataPost),
                    contentType: "application/json; charset=utf-8",
                    dataType: "html",
                    success: function (data) {
                        $("#congnodoitac").html(data);
                    }
                });
                break;
            case 'danhgia':
                $.ajax({
                    type: "POST",
                    url: '/TourTabInfo/InfoDanhGia',
                    data: JSON.stringify(dataPost),
                    contentType: "application/json; charset=utf-8",
                    dataType: "html",
                    success: function (data) {
                        $("#danhgia").html(data);
                    }
                });
                break;
        }
    });
}

function OnFailureScheduleTour() {
    alert("Lỗi...!");
    $("#modal-insert-tourschedule").modal('hide');
    CKupdate();
}

function OnSuccessScheduleTour() {
    alert("Đã lưu!");
    $("#modal-insert-tourschedule").modal('hide');
    $('form').trigger("reset");
    CKupdate();
}

function OnFailure() {
    alert("Cập nhật thất bại!");
    $("#modal-update-visa").modal('hide');
    $("#modal-update-note").modal('hide');
    $('form').trigger("reset");
    CKupdate();
}

function OnSuccess() {
    $("#modal-update-visa").modal('hide');
    $("#modal-update-note").modal('hide');
    alert("Cập nhật thành công!");
    $('form').trigger("reset");
    CKupdate();
}

function loadTabInfo() {
    /** chọn từng dòng để hiển thị thông tin chi tiết dưới các tab **/
    $("table#tableDictionary").delegate("tr", "click", function (e) {
        $('tr').not(this).removeClass('oneselected');
        $(this).toggleClass('oneselected');
        var tab = $(".tab-content").find('.active').data("id");
        var dataPost = { id: $(this).find("input[type='checkbox']").val() };
        switch (tab) {
            case 'chitiettour':
                $.ajax({
                    type: "POST",
                    url: '/TourTabInfo/InfoChiTietTour',
                    data: JSON.stringify(dataPost),
                    contentType: "application/json; charset=utf-8",
                    dataType: "html",
                    success: function (data) {
                        $("#chitiettour").html(data);
                    }
                });
                break;
            case 'lichhen':
                $.ajax({
                    type: "POST",
                    url: '/TourTabInfo/InfoLichHen',
                    data: JSON.stringify(dataPost),
                    contentType: "application/json; charset=utf-8",
                    dataType: "html",
                    success: function (data) {
                        $("#lichhen").html(data);
                    }
                });
                break;
            case 'dichvu':
                $.ajax({
                    type: "POST",
                    url: '/TourTabInfo/InfoDichVu',
                    data: JSON.stringify(dataPost),
                    contentType: "application/json; charset=utf-8",
                    dataType: "html",
                    success: function (data) {
                        $("#dichvu").html(data);
                    }
                });
                break;
            case 'khachhang':
                $.ajax({
                    type: "POST",
                    url: '/TourTabInfo/InfoKhachHang',
                    data: JSON.stringify(dataPost),
                    contentType: "application/json; charset=utf-8",
                    dataType: "html",
                    success: function (data) {
                        $("#khachhang").html(data);
                    }
                });
                break;
            case 'visa':
                $.ajax({
                    type: "POST",
                    url: '/TourTabInfo/InfoVisa',
                    data: JSON.stringify(dataPost),
                    contentType: "application/json; charset=utf-8",
                    dataType: "html",
                    success: function (data) {
                        $("#visa").html(data);
                        ////////
                        $("#btnUpdateVisa").click(function () {
                            var items = '';
                            $("input.cbVisa:checked").each(function () {
                                items += $(this).val() + ',';
                            })
                            $("#listVisaId").val(items);
                            $("#modal-update-visa").modal('show')
                        })
                    }
                });
                break;
            case 'tailieumau':
                $.ajax({
                    type: "POST",
                    url: '/TourTabInfo/InfoTaiLieuMau',
                    data: JSON.stringify(dataPost),
                    contentType: "application/json; charset=utf-8",
                    dataType: "html",
                    success: function (data) {
                        $("#tailieumau").html(data);
                    }
                });
                break;
            case 'nhiemvu':
                $.ajax({
                    type: "POST",
                    url: '/TourTabInfo/InfoNhiemVu',
                    data: JSON.stringify(dataPost),
                    contentType: "application/json; charset=utf-8",
                    dataType: "html",
                    success: function (data) {
                        $("#nhiemvu").html(data);
                    }
                });
                break;
            case 'chuongtrinh':
                $.ajax({
                    type: "POST",
                    url: '/TourTabInfo/InfoChuongTrinh',
                    data: JSON.stringify(dataPost),
                    contentType: "application/json; charset=utf-8",
                    dataType: "html",
                    success: function (data) {
                        $("#chuongtrinh").html(data);
                    }
                });
                break;
            case 'hopdong':
                $.ajax({
                    type: "POST",
                    url: '/TourTabInfo/InfoHopDong',
                    data: JSON.stringify(dataPost),
                    contentType: "application/json; charset=utf-8",
                    dataType: "html",
                    success: function (data) {
                        $("#hopdong").html(data);
                    }
                });
                break;
            case 'lichsulienhe':
                $.ajax({
                    type: "POST",
                    url: '/TourTabInfo/InfoLichSuLienHe',
                    data: JSON.stringify(dataPost),
                    contentType: "application/json; charset=utf-8",
                    dataType: "html",
                    success: function (data) {
                        $("#lichsulienhe").html(data);
                    }
                });
                break;
            case 'viettourbaogia':
                $.ajax({
                    type: "POST",
                    url: '/TourTabInfo/InfoViettourBaoGia',
                    data: JSON.stringify(dataPost),
                    contentType: "application/json; charset=utf-8",
                    dataType: "html",
                    success: function (data) {
                        $("#viettourbaogia").html(data);
                    }
                });
                break;
            case 'congnokh':
                $.ajax({
                    type: "POST",
                    url: '/TourTabInfo/InfoCongNoKH',
                    data: JSON.stringify(dataPost),
                    contentType: "application/json; charset=utf-8",
                    dataType: "html",
                    success: function (data) {
                        $("#congnokh").html(data);
                    }
                });
                break;
            case 'congnodoitac':
                $.ajax({
                    type: "POST",
                    url: '/TourTabInfo/InfoCongNoDoiTac',
                    data: JSON.stringify(dataPost),
                    contentType: "application/json; charset=utf-8",
                    dataType: "html",
                    success: function (data) {
                        $("#congnodoitac").html(data);
                    }
                });
                break;
            case 'danhgia':
                $.ajax({
                    type: "POST",
                    url: '/TourTabInfo/InfoDanhGia',
                    data: JSON.stringify(dataPost),
                    contentType: "application/json; charset=utf-8",
                    dataType: "html",
                    success: function (data) {
                        $("#danhgia").html(data);
                    }
                });
                break;
        }
    });

    /** click chọn từng tab -> hiển thị thông tin **/
    $("#tabchitiettour").click(function () {
        if ($("table#tableDictionary").find('tr.oneselected').length === 0) {
            alert("Vui lòng chọn 1 tour!");
        }
        else {
            var dataPost = { id: $("table#tableDictionary").find('tr.oneselected').find("input[type='checkbox']").val() };
            $.ajax({
                type: "POST",
                url: '/TourTabInfo/InfoChiTietTour',
                data: JSON.stringify(dataPost),
                contentType: "application/json; charset=utf-8",
                dataType: "html",
                success: function (data) {
                    $("#chitiettour").html(data);
                }
            });
        }
    });

    $("#tablichhen").click(function () {
        if ($("table#tableDictionary").find('tr.oneselected').length === 0) {
            alert("Vui lòng chọn 1 tour!");
        }
        else {
            var dataPost = { id: $("table#tableDictionary").find('tr.oneselected').find("input[type='checkbox']").val() };
            $.ajax({
                type: "POST",
                url: '/TourTabInfo/InfoLichHen',
                data: JSON.stringify(dataPost),
                contentType: "application/json; charset=utf-8",
                dataType: "html",
                success: function (data) {
                    $("#lichhen").html(data);
                }
            });
        }
    });

    $("#tabkhachhang").click(function () {
        if ($("table#tableDictionary").find('tr.oneselected').length === 0) {
            alert("Vui lòng chọn 1 tour!");
        }
        else {
            var dataPost = { id: $("table#tableDictionary").find('tr.oneselected').find("input[type='checkbox']").val() };
            $.ajax({
                type: "POST",
                url: '/TourTabInfo/InfoKhachHang',
                data: JSON.stringify(dataPost),
                contentType: "application/json; charset=utf-8",
                dataType: "html",
                success: function (data) {
                    $("#khachhang").html(data);
                }
            });
        }
    });

    $("#tabvisa").click(function () {
        if ($("table#tableDictionary").find('tr.oneselected').length === 0) {
            alert("Vui lòng chọn 1 tour!");
        }
        else {
            var dataPost = { id: $("table#tableDictionary").find('tr.oneselected').find("input[type='checkbox']").val() };
            $.ajax({
                type: "POST",
                url: '/TourTabInfo/InfoVisa',
                data: JSON.stringify(dataPost),
                contentType: "application/json; charset=utf-8",
                dataType: "html",
                success: function (data) {
                    $("#visa").html(data);
                    ///////////////////
                    $("#btnUpdateVisa").click(function () {
                        var items = '';
                        $("input.cbVisa:checked").each(function () {
                            items += $(this).val() + ',';
                        })
                        $("#listVisaId").val(items);
                        $("#modal-update-visa").modal('show')
                    })
                }
            });
        }
    });

    $("#tabtailieumau").click(function () {
        if ($("table#tableDictionary").find('tr.oneselected').length === 0) {
            alert("Vui lòng chọn 1 tour!");
        }
        else {
            var dataPost = { id: $("table#tableDictionary").find('tr.oneselected').find("input[type='checkbox']").val() };
            $.ajax({
                type: "POST",
                url: '/TourTabInfo/InfoTaiLieuMau',
                data: JSON.stringify(dataPost),
                contentType: "application/json; charset=utf-8",
                dataType: "html",
                success: function (data) {
                    $("#tailieumau").html(data);
                }
            });
        }
    });

    $("#tabchuongtrinh").click(function () {
        if ($("table#tableDictionary").find('tr.oneselected').length === 0) {
            alert("Vui lòng chọn 1 tour!");
        }
        else {
            var dataPost = { id: $("table#tableDictionary").find('tr.oneselected').find("input[type='checkbox']").val() };
            $.ajax({
                type: "POST",
                url: '/TourTabInfo/InfoChuongTrinh',
                data: JSON.stringify(dataPost),
                contentType: "application/json; charset=utf-8",
                dataType: "html",
                success: function (data) {
                    $("#chuongtrinh").html(data);
                }
            });
        }
    });

    $("#tabhopdong").click(function () {
        if ($("table#tableDictionary").find('tr.oneselected').length === 0) {
            alert("Vui lòng chọn 1 tour!");
        }
        else {
            var dataPost = { id: $("table#tableDictionary").find('tr.oneselected').find("input[type='checkbox']").val() };
            $.ajax({
                type: "POST",
                url: '/TourTabInfo/InfoHopDong',
                data: JSON.stringify(dataPost),
                contentType: "application/json; charset=utf-8",
                dataType: "html",
                success: function (data) {
                    $("#hopdong").html(data);
                }
            });
        }
    });

    $("#tablichsulienhe").click(function () {
        if ($("table#tableDictionary").find('tr.oneselected').length === 0) {
            alert("Vui lòng chọn 1 tour!");
        }
        else {
            var dataPost = { id: $("table#tableDictionary").find('tr.oneselected').find("input[type='checkbox']").val() };
            $.ajax({
                type: "POST",
                url: '/TourTabInfo/InfoLichSuLienHe',
                data: JSON.stringify(dataPost),
                contentType: "application/json; charset=utf-8",
                dataType: "html",
                success: function (data) {
                    $("#lichsulienhe").html(data);
                }
            });
        }
    });

    $("#tabdichvu").click(function () {
        if ($("table#tableDictionary").find('tr.oneselected').length === 0) {
            alert("Vui lòng chọn 1 tour!");
        }
        else {
            var dataPost = { id: $("table#tableDictionary").find('tr.oneselected').find("input[type='checkbox']").val() };
            $.ajax({
                type: "POST",
                url: '/TourTabInfo/InfoDichVu',
                data: JSON.stringify(dataPost),
                contentType: "application/json; charset=utf-8",
                dataType: "html",
                success: function (data) {
                    $("#dichvu").html(data);
                }
            });
        }
    });

    $("#tabnhiemvu").click(function () {
        if ($("table#tableDictionary").find('tr.oneselected').length === 0) {
            alert("Vui lòng chọn 1 tour!");
        }
        else {
            var dataPost = { id: $("table#tableDictionary").find('tr.oneselected').find("input[type='checkbox']").val() };
            $.ajax({
                type: "POST",
                url: '/TourTabInfo/InfoNhiemVu',
                data: JSON.stringify(dataPost),
                contentType: "application/json; charset=utf-8",
                dataType: "html",
                success: function (data) {
                    $("#nhiemvu").html(data);
                }
            });
        }
    });

    $("#tabdanhgia").click(function () {
        if ($("table#tableDictionary").find('tr.oneselected').length === 0) {
            alert("Vui lòng chọn 1 tour!");
        }
        else {
            var dataPost = { id: $("table#tableDictionary").find('tr.oneselected').find("input[type='checkbox']").val() };
            $.ajax({
                type: "POST",
                url: '/TourTabInfo/InfoDanhGia',
                data: JSON.stringify(dataPost),
                contentType: "application/json; charset=utf-8",
                dataType: "html",
                success: function (data) {
                    $("#danhgia").html(data);
                }
            });
        }
    });

    $("#tabviettourbaogia").click(function () {
        if ($("table#tableDictionary").find('tr.oneselected').length === 0) {
            alert("Vui lòng chọn 1 tour!");
        }
        else {
            var dataPost = { id: $("table#tableDictionary").find('tr.oneselected').find("input[type='checkbox']").val() };
            $.ajax({
                type: "POST",
                url: '/TourTabInfo/InfoViettourBaoGia',
                data: JSON.stringify(dataPost),
                contentType: "application/json; charset=utf-8",
                dataType: "html",
                success: function (data) {
                    $("#viettourbaogia").html(data);
                }
            });
        }
    });

    $("#tabcongnokh").click(function () {
        if ($("table#tableDictionary").find('tr.oneselected').length === 0) {
            alert("Vui lòng chọn 1 tour!");
        }
        else {
            var dataPost = { id: $("table#tableDictionary").find('tr.oneselected').find("input[type='checkbox']").val() };
            $.ajax({
                type: "POST",
                url: '/TourTabInfo/InfoCongNoKH',
                data: JSON.stringify(dataPost),
                contentType: "application/json; charset=utf-8",
                dataType: "html",
                success: function (data) {
                    $("#congnokh").html(data);
                }
            });
        }
    });

    $("#tabcongnodoitac").click(function () {
        if ($("table#tableDictionary").find('tr.oneselected').length === 0) {
            alert("Vui lòng chọn 1 tour!");
        }
        else {
            var dataPost = { id: $("table#tableDictionary").find('tr.oneselected').find("input[type='checkbox']").val() };
            $.ajax({
                type: "POST",
                url: '/TourTabInfo/InfoCongNoDoiTac',
                data: JSON.stringify(dataPost),
                contentType: "application/json; charset=utf-8",
                dataType: "html",
                success: function (data) {
                    $("#congnodoitac").html(data);
                }
            });
        }
    });
}

function deleteVisa(id) {
    var dataPost = { id: id };
    $.ajax({
        type: "POST",
        url: '/TourOtherTab/DeleteVisa',
        data: JSON.stringify(dataPost),
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (data) {
            alert("Đã xóa!");
            $("#visa").html(data);
        }
    });
}

function updateVisa(id) {
    var dataPost = { id: id };
    $.ajax({
        type: "POST",
        url: '/TourOtherTab/EditVisa',
        data: JSON.stringify(dataPost),
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (data) {
            $("#info-data-visa").html(data);
            $("#update-visa-country").select2();
            $("#update-visa-status").select2();
            $("#update-visa-type").select2();
            $("#modal-visa-update").modal("show");
        }
    });
}

$(function () {
    $('#btnAddGuide').click(function () {
        var num = $('.TourGuide').length, // how many "duplicatable" input fields we currently have
            newNum = new Number(num + 1),      // the numeric ID of the new input field being added
            newElem = $('#TourGuide' + num).clone().attr('id', 'TourGuide' + newNum).fadeIn('slow'); // create the new element via clone(), and manipulate it's ID using newNum value
        // manipulate the name/id values of the input inside the new element

        newElem.find('.insert-guide-tour').attr('name', 'StaffTourGuide' + newNum).attr('id', 'insert-guide-tour' + newNum);
        newElem.find('.insert-start-tour').attr('name', 'StartDateTourGuide' + newNum).val('');
        newElem.find('.insert-end-tour').attr('name', 'EndDateTourGuide' + newNum).val('');
        newElem.find('.insert-customer-tour').attr('id', 'insert-customer-tour' + newNum).val('');

        // insert the new element after the last "duplicatable" input field
        $('#TourGuide' + num).after(newElem);
        $("#insert-guide-tour" + newNum).select2();
        $("#insert-customer-tour" + newNum).text("Nhân viên " + newNum);

        for (var i = 1; i < newNum; i++) {
            $("#TourGuide" + newNum + " #select2-insert-guide-tour" + i + "-container").parent().parent().parent().remove();
        }

        // enable the "remove" button
        $('#btnDeleteGuide').attr('disabled', false);

        // count service
        $("#countGuide").val(newNum);
    });

    $('#btnDeleteGuide').click(function () {
        // confirmation
        var num = $('.TourGuide').length;
        // how many "duplicatable" input fields we currently have
        $('#TourGuide' + num).slideUp('slow', function () {
            $(this).remove();
            // if only one element remains, disable the "remove" button
            if (num - 1 === 1)
                $('#btnDeleteGuide').attr('disabled', true);
            // enable the "add" button
            $('#btnAddGuide').attr('disabled', false).prop('value', "add section");
        });
        return false;

        $('#btnAddGuide').attr('disabled', false);
        // count service
        $("#countGuide").val(num);
    });
    //$('#btnDel').attr('disabled', true);
});

function checkCodeTour() {
    var dataPost = { code: $("#CodeTour").val() };
    $.ajax({
        type: "POST",
        url: '/TourBiddingManage/CheckCodeTour',
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
        }
    });
})