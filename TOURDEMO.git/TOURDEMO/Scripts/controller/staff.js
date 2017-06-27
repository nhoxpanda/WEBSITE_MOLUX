CKEDITOR.replace("insert-note-schedule");
$("#insert-nametype").select2();
$("#insert-address").select2();
$("#insert-position").select2();
$("#insert-staffgroup").select2();
$("#insert-headquarter").select2();
$("#insert-placeidentity").select2();
$("#insert-department").select2();
$("#insert-birthplace").select2();
$("#insert-placepassport").select2();
$("#insert-nation").select2();
$("#insert-religion").select2();
$("#insert-marriage").select2();
$("#insert-certificate").select2();
$("#countryvisa1").select2();

$("#insert-task-type").select2();
$("#insert-department-task").select2();
$("#insert-timetype-task").select2();
$("#insert-tour-task").select2();
$("#insert-priority-task").select2();
CKEDITOR.replace("insert-note-task");
$("#insert-ngayhen-lichhen").val(moment(new Date()).format("YYYY-MM-DD") + "T08:30");
$("#insert-nhiemvu-ngaynhac").val(moment(new Date()).format("YYYY-MM-DD") + "T08:30");

var table = $('.dataTable').DataTable({
    order: [],
    columnDefs: [{ orderable: false, targets: [0] }],
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
                { type: "text" },
                null]
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

///*** duplicate form visa ***/
$(function () {
    $('#btnAdd').click(function () {
        var num = $('.clonedInput').length, // how many "duplicatable" input fields we currently have
            newNum = new Number(num + 1),      // the numeric ID of the new input field being added
            newElem = $('#entry' + num).clone().attr('id', 'entry' + newNum).fadeIn('slow'); // create the new element via clone(), and manipulate it's ID using newNum value
        // manipulate the name/id values of the input inside the new element

        newElem.find('.visacard').attr('name', 'VisaNumber' + newNum).val('');
        newElem.find('.ngaycapvisa').attr('id', 'ngaycapvisa' + newNum).attr('name', 'CreatedDateVisa' + newNum).val('');
        newElem.find('.ngayhethanvisa').attr('id', 'ngayhethanvisa' + newNum).attr('name', 'ExpiredDateVisa' + newNum).val('');
        newElem.find('.countryvisa').attr('id', 'countryvisa' + newNum).attr('name', 'TagsId' + newNum).val('');

        // insert the new element after the last "duplicatable" input field
        $('#entry' + num).after(newElem);
        //$("#ngaycapvisa" + newNum).datepicker();
        //$("#ngayhethanvisa" + newNum).datepicker();
        $("#countryvisa" + newNum).select2();

        for (var i = 1; i < newNum; i++) {
            $("#entry" + newNum + " #select2-countryvisa" + i + "-container").parent().parent().parent().remove();
        }

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


///****** cập nhật nhân viên ******/
$("#btnEdit").click(function () {
    var dataPost = {
        id: $("input[name='cb']:checked").val()
    };

    $.ajax({
        type: "POST",
        url: '/StaffManage/StaffInformation',
        data: JSON.stringify(dataPost),
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (data) {
            $(".info-data-staff").html(data);
            $("#edit-nametype").select2();
            $("#edit-address").select2();
            $("#edit-position").select2();
            $("#edit-staffgroup").select2();
            $("#edit-headquarter").select2();
            $("#edit-placeidentity").select2();
            $("#edit-department").select2();
            //$("#edit-identity").datepicker();
            $("#edit-birthplace").select2();
            //$("#edit-birthday").datepicker();
            //$("#edit-createpassport").datepicker();
            //$("#edit-expiredPassport").datepicker();
            $("#edit-placepassport").select2();
            $("#edit-nation").select2();
            $("#edit-religion").select2();
            $("#edit-marriage").select2();
            $("#edit-certificate").select2();

            ///*** modal ***/
            $("#modal-edit-staff").modal("show");

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

            $("#edit-check").click(function () {
                if (this.checked) {
                    $("#edit-code-guide").removeAttr("disabled", "disabled");
                }
                else {
                    $("#edit-code-guide").attr("disabled", "disabled");
                }
            });
        }
    });
});

///*** lock nhan vien ****/
$("#btnLock").click(function () {
    if (!confirm("Bạn thực sự muốn khóa nhân viên đã chọn ?")) {
        return false;
    }
    var dataPost = {
        id: $("input[name='cb']:checked").val()
    };

    $.ajax({
        type: "POST",
        url: '/StaffManage/LockStaff',
        data: JSON.stringify(dataPost),
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (data) {
            alert("Đã khóa tài khoản này!");
        }
    });
})

///*** unlock nhan vien ****/
$("#btnUnLock").click(function () {
    if (!confirm("Bạn thực sự muốn mở khóa nhân viên đã chọn ?")) {
        return false;
    }
    var dataPost = {
        id: $("input[name='cb']:checked").val()
    };

    $.ajax({
        type: "POST",
        url: '/StaffManage/UnlockStaff',
        data: JSON.stringify(dataPost),
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (data) {
            alert("Đã mở khóa tài khoản này!");
        }
    });
})

///***** xóa nhân viên *****/
$("#btnRemove").on("click", function () {
    if ($("#listItemId").val() == "") {
        alert("Vui lòng chọn mục cần xóa !");
        return false;
    }
    var $this = $(this);
    var $tableWrapper = $("#tableDictionary-Wrapper");
    var $table = $("#tableDictionary");

    DeleteSelectedItem($this, $tableWrapper, $table, function (data) {});
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
        case 'thongtinchitiet':
            console.log(tab);
            $.ajax({
                type: "POST",
                url: '/StaffTabInfo/InfoThongTinChiTiet',
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
                url: '/StaffTabInfo/InfoLichHen',
                data: JSON.stringify(dataPost),
                contentType: "application/json; charset=utf-8",
                dataType: "html",
                success: function (data) {
                    $("#lichhen").html(data);
                }
            });
            break;
        case 'nhiemvu':
            $.ajax({
                type: "POST",
                url: '/StaffTabInfo/InfoNhiemVu',
                data: JSON.stringify(dataPost),
                contentType: "application/json; charset=utf-8",
                dataType: "html",
                success: function (data) {
                    $("#nhiemvu").html(data);
                }
            });
            break;
        case 'thautour':
            $.ajax({
                type: "POST",
                url: '/StaffTabInfo/InfoThauTour',
                data: JSON.stringify(dataPost),
                contentType: "application/json; charset=utf-8",
                dataType: "html",
                success: function (data) {
                    $("#thautour").html(data);
                }
            });
            break;
        case 'visa':
            $.ajax({
                type: "POST",
                url: '/StaffTabInfo/InfoVisa',
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
                url: '/StaffTabInfo/InfoHoSoLienQuan',
                data: JSON.stringify(dataPost),
                contentType: "application/json; charset=utf-8",
                dataType: "html",
                success: function (data) {
                    $("#hosolienquan").html(data);
                }
            });
            break;
        case 'lichsulienhe':
            $.ajax({
                type: "POST",
                url: '/StaffTabInfo/InfoLichSuLienHe',
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
                url: '/StaffTabInfo/InfoCapNhatThayDoi',
                data: JSON.stringify(dataPost),
                contentType: "application/json; charset=utf-8",
                dataType: "html",
                success: function (data) {
                    $("#capnhatthaydoi").html(data);
                }
            });
            break;
        case 'khachhang':
            $.ajax({
                type: "POST",
                url: '/StaffTabInfo/InfoKhachHang',
                data: JSON.stringify(dataPost),
                contentType: "application/json; charset=utf-8",
                dataType: "html",
                success: function (data) {
                    $("#khachhang").html(data);
                }
            });
            break;
        case 'lichsuditour':
            $.ajax({
                type: "POST",
                url: '/StaffTabInfo/InfoLichSuDiTour',
                data: JSON.stringify(dataPost),
                contentType: "application/json; charset=utf-8",
                dataType: "html",
                success: function (data) {
                    $("#lichsuditour").html(data);
                }
            });
            break;
        case 'chuongtrinh':
            $.ajax({
                type: "POST",
                url: '/StaffTabInfo/InfoChuongTrinh',
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
                url: '/StaffTabInfo/InfoHopDong',
                data: JSON.stringify(dataPost),
                contentType: "application/json; charset=utf-8",
                dataType: "html",
                success: function (data) {
                    $("#hopdong").html(data);
                }
            });
            break;
        case 'visakh':
            $.ajax({
                type: "POST",
                url: '/StaffTabInfo/InfoVisaKH',
                data: JSON.stringify(dataPost),
                contentType: "application/json; charset=utf-8",
                dataType: "html",
                success: function (data) {
                    $("#visakh").html(data);
                }
            });
            break;
        case 'ticket':
            $.ajax({
                type: "POST",
                url: '/StaffTabInfo/InfoCodeVe',
                data: JSON.stringify(dataPost),
                contentType: "application/json; charset=utf-8",
                dataType: "html",
                success: function (data) {
                    $("#ticket").html(data);
                }
            });
            break;
        case 'salary':
            $.ajax({
                type: "POST",
                url: '/StaffTabInfo/InfoLuong',
                data: JSON.stringify(dataPost),
                contentType: "application/json; charset=utf-8",
                dataType: "html",
                success: function (data) {
                    $("#salary").html(data);
                }
            });
            break;
        case 'khenthuongkyluat':
            $.ajax({
                type: "POST",
                url: '/StaffTabInfo/InfoKhenThuongKyLuat',
                data: JSON.stringify(dataPost),
                contentType: "application/json; charset=utf-8",
                dataType: "html",
                success: function (data) {
                    $("#khenthuongkyluat").html(data);
                }
            });
            break;
        case 'ngaynghiphep':
            $.ajax({
                type: "POST",
                url: '/StaffTabInfo/InfoNgayNghiPhep',
                data: JSON.stringify(dataPost),
                contentType: "application/json; charset=utf-8",
                dataType: "html",
                success: function (data) {
                    $("#ngaynghiphep").html(data);
                }
            });
            break;
    }
});

/** click chọn từng tab -> hiển thị thông tin **/
$("#tabthongtinchitiet").click(function () {
    if ($("table#tableDictionary").find('tr.oneselected').length === 0) {
        alert("Vui lòng chọn 1 nhân viên!");
    }
    else {
        var dataPost = { id: $("table#tableDictionary").find('tr.oneselected').find("input[type='checkbox']").val() };
        $.ajax({
            type: "POST",
            url: '/StaffTabInfo/InfoThongTinChiTiet',
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
        alert("Vui lòng chọn 1 nhân viên!");
    }
    else {
        var dataPost = { id: $("table#tableDictionary").find('tr.oneselected').find("input[type='checkbox']").val() };
        $.ajax({
            type: "POST",
            url: '/StaffTabInfo/InfoLichHen',
            data: JSON.stringify(dataPost),
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            success: function (data) {
                $("#lichhen").html(data);
            }
        });
    }
});

$("#tabnhiemvu").click(function () {
    if ($("table#tableDictionary").find('tr.oneselected').length === 0) {
        alert("Vui lòng chọn 1 nhân viên!");
    }
    else {
        var dataPost = { id: $("table#tableDictionary").find('tr.oneselected').find("input[type='checkbox']").val() };
        $.ajax({
            type: "POST",
            url: '/StaffTabInfo/InfoNhiemVu',
            data: JSON.stringify(dataPost),
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            success: function (data) {
                $("#nhiemvu").html(data);
            }
        });
    }
});

$("#tablichsulienhe").click(function () {
    if ($("table#tableDictionary").find('tr.oneselected').length === 0) {
        alert("Vui lòng chọn 1 nhân viên!");
    }
    else {
        var dataPost = { id: $("table#tableDictionary").find('tr.oneselected').find("input[type='checkbox']").val() };
        $.ajax({
            type: "POST",
            url: '/StaffTabInfo/InfoLichSuLienHe',
            data: JSON.stringify(dataPost),
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            success: function (data) {
                $("#lichsulienhe").html(data);
            }
        });
    }
});

$("#tabthautour").click(function () {
    if ($("table#tableDictionary").find('tr.oneselected').length === 0) {
        alert("Vui lòng chọn 1 nhân viên!");
    }
    else {
        var dataPost = { id: $("table#tableDictionary").find('tr.oneselected').find("input[type='checkbox']").val() };
        $.ajax({
            type: "POST",
            url: '/StaffTabInfo/InfoThauTour',
            data: JSON.stringify(dataPost),
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            success: function (data) {
                $("#thautour").html(data);
            }
        });
    }
});

$("#tabhosolienquan").click(function () {
    if ($("table#tableDictionary").find('tr.oneselected').length === 0) {
        alert("Vui lòng chọn 1 nhân viên!");
    }
    else {
        var dataPost = { id: $("table#tableDictionary").find('tr.oneselected').find("input[type='checkbox']").val() };
        $.ajax({
            type: "POST",
            url: '/StaffTabInfo/InfoHoSoLienQuan',
            data: JSON.stringify(dataPost),
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            success: function (data) {
                $("#hosolienquan").html(data);
            }
        });
    }
});

$("#tabkhachhang").click(function () {
    if ($("table#tableDictionary").find('tr.oneselected').length === 0) {
        alert("Vui lòng chọn 1 nhân viên!");
    }
    else {
        var dataPost = { id: $("table#tableDictionary").find('tr.oneselected').find("input[type='checkbox']").val() };
        $.ajax({
            type: "POST",
            url: '/StaffTabInfo/InfoKhachHang',
            data: JSON.stringify(dataPost),
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            success: function (data) {
                $("#khachhang").html(data);
            }
        });
    }
});

$("#tablichsuditour").click(function () {
    if ($("table#tableDictionary").find('tr.oneselected').length === 0) {
        alert("Vui lòng chọn 1 nhân viên!");
    }
    else {
        var dataPost = { id: $("table#tableDictionary").find('tr.oneselected').find("input[type='checkbox']").val() };
        $.ajax({
            type: "POST",
            url: '/StaffTabInfo/InfoLichSuDiTour',
            data: JSON.stringify(dataPost),
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            success: function (data) {
                $("#lichsuditour").html(data);
            }
        });
    }
});

$("#tabvisa").click(function () {
    if ($("table#tableDictionary").find('tr.oneselected').length === 0) {
        alert("Vui lòng chọn 1 nhân viên!");
    }
    else {
        var dataPost = { id: $("table#tableDictionary").find('tr.oneselected').find("input[type='checkbox']").val() };
        $.ajax({
            type: "POST",
            url: '/StaffTabInfo/InfoVisa',
            data: JSON.stringify(dataPost),
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            success: function (data) {
                $("#visa").html(data);
            }
        });
    }
});

$("#tabcapnhatthaydoi").click(function () {
    if ($("table#tableDictionary").find('tr.oneselected').length === 0) {
        alert("Vui lòng chọn 1 nhân viên!");
    }
    else {
        var dataPost = { id: $("table#tableDictionary").find('tr.oneselected').find("input[type='checkbox']").val() };
        $.ajax({
            type: "POST",
            url: '/StaffTabInfo/InfoCapNhatThayDoi',
            data: JSON.stringify(dataPost),
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            success: function (data) {
                $("#capnhatthaydoi").html(data);
            }
        });
    }
});

$("#tabhopdong").click(function () {
    if ($("table#tableDictionary").find('tr.oneselected').length === 0) {
        alert("Vui lòng chọn 1 nhân viên!");
    }
    else {
        var dataPost = { id: $("table#tableDictionary").find('tr.oneselected').find("input[type='checkbox']").val() };
        $.ajax({
            type: "POST",
            url: '/StaffTabInfo/InfoHopDong',
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
        alert("Vui lòng chọn 1 nhân viên!");
    }
    else {
        var dataPost = { id: $("table#tableDictionary").find('tr.oneselected').find("input[type='checkbox']").val() };
        $.ajax({
            type: "POST",
            url: '/StaffTabInfo/InfoChuongTrinh',
            data: JSON.stringify(dataPost),
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            success: function (data) {
                $("#chuongtrinh").html(data);
            }
        });
    }
});

$("#tabticket").click(function () {
    if ($("table#tableDictionary").find('tr.oneselected').length === 0) {
        alert("Vui lòng chọn 1 nhân viên!");
    }
    else {
        var dataPost = { id: $("table#tableDictionary").find('tr.oneselected').find("input[type='checkbox']").val() };
        $.ajax({
            type: "POST",
            url: '/StaffTabInfo/InfoCodeVe',
            data: JSON.stringify(dataPost),
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            success: function (data) {
                $("#ticket").html(data);
            }
        });
    }
});

$("#tabsalary").click(function () {
    if ($("table#tableDictionary").find('tr.oneselected').length === 0) {
        alert("Vui lòng chọn 1 nhân viên!");
    }
    else {
        var dataPost = { id: $("table#tableDictionary").find('tr.oneselected').find("input[type='checkbox']").val() };
        $.ajax({
            type: "POST",
            url: '/StaffTabInfo/InfoLuong',
            data: JSON.stringify(dataPost),
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            success: function (data) {
                $("#salary").html(data);
            }
        });
    }
});

$("#tabvisakh").click(function () {
    if ($("table#tableDictionary").find('tr.oneselected').length === 0) {
        alert("Vui lòng chọn 1 nhân viên!");
    }
    else {
        var dataPost = { id: $("table#tableDictionary").find('tr.oneselected').find("input[type='checkbox']").val() };
        $.ajax({
            type: "POST",
            url: '/StaffTabInfo/InfoVisaKH',
            data: JSON.stringify(dataPost),
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            success: function (data) {
                $("#visakh").html(data);
            }
        });
    }
});

$("#tabngaynghiphep").click(function () {
    if ($("table#tableDictionary").find('tr.oneselected').length === 0) {
        alert("Vui lòng chọn 1 nhân viên!");
    }
    else {
        var dataPost = { id: $("table#tableDictionary").find('tr.oneselected').find("input[type='checkbox']").val() };
        $.ajax({
            type: "POST",
            url: '/StaffTabInfo/InfoNgayNghiPhep',
            data: JSON.stringify(dataPost),
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            success: function (data) {
                $("#ngaynghiphep").html(data);
            }
        });
    }
});

$("#tabkhenthuongkyluat").click(function () {
    if ($("table#tableDictionary").find('tr.oneselected').length === 0) {
        alert("Vui lòng chọn 1 nhân viên!");
    }
    else {
        var dataPost = { id: $("table#tableDictionary").find('tr.oneselected').find("input[type='checkbox']").val() };
        $.ajax({
            type: "POST",
            url: '/StaffTabInfo/InfoKhenThuongKyLuat',
            data: JSON.stringify(dataPost),
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            success: function (data) {
                $("#khenthuongkyluat").html(data);
            }
        });
    }
});

/** upload file **/
$('#FileName').change(function () {
    var data = new FormData();
    data.append('FileName', $('#FileName')[0].files[0]);
    var ajaxRequest = $.ajax({
        type: "POST",
        url: 'StaffManage/UploadFile',
        contentType: false,
        processData: false,
        data: data
    });

    ajaxRequest.done(function (xhr, textStatus) {
        // Onsuccess
    });
});

/** xóa tài liệu **/
function deleteDocument(id) {
    if (confirm('Bạn thực sự muốn xóa mục này ?')) {
        var dataPost = { id: id };
        $.ajax({
            type: "POST",
            url: '/StaffManage/DeleteDocument',
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
        url: '/StaffManage/EditInfoDocument',
        data: JSON.stringify(dataPost),
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (data) {
            $("#info-data-doc").html(data);
            $("#edit-tag-document").select2();
            $("#edit-document-type").select2();
            CKEDITOR.replace("edit-document-note");
            $("#modal-edit-document").modal("show");

            /**** update in tab file của nhân viên ****/
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
                    url: 'StaffManage/UploadFile',
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

/** success **/
function OnSuccessCustomerFile() {
    $("#modal-insert-document").modal("hide");
    $('form').trigger("reset");
    CKupdate();
    $("#modal-edit-document").modal("hide");
}

/** failure **/
function OnFailureCustomerFile() {
    $('form').trigger("reset");
    CKupdate();
    alert("Lỗi, vui lòng kiểm tra lại!");
    $("#modal-insert-document").modal("hide");
    $("#modal-edit-document").modal("hide");
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

/** xóa visa **/
function deleteVisa(id) {
    if (confirm('Bạn thực sự muốn xóa mục này ?')) {
        var dataPost = { id: id };
        $.ajax({
            type: "POST",
            url: '/StaffManage/DeleteVisa',
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
        url: '/StaffManage/EditInfoVisa',
        data: JSON.stringify(dataPost),
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (data) {
            $("#info-data-visa").html(data);
            $("#edit-country-visa").select2();
            $("#edit-type-visa").select2();
            $("#edit-status-visa").select2();
            //$("#edit-createdatevisa").datepicker();
            //$("#edit-expiredatevisa").datepicker();
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
            });
        }
    });
}

$("#btnExport").click(function () {
    $("#exportForm").submit();
});

$("#btnAddTask").click(function () {
    var dataPost = { id: $("table#tableDictionary").find("input[type='checkbox']:checked").val() };
    $.ajax({
        type: "POST",
        url: '/StaffManage/GetIdStaff',
        data: JSON.stringify(dataPost),
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (data) {
            $("#modal-insert-stafftask").modal('show');
        }
    });
})

function OnFailureStaff() {
    alert("Lỗi...!");
    $("#modal-insert-stafftask").modal('hide');
    $("#modal-edit-stafftask").modal('hide');
    $('form').trigger("reset");
    CKupdate();
}

function OnSuccessStaff() {
    alert("Đã lưu!");
    $("#modal-insert-stafftask").modal('hide');
    $("#modal-edit-stafftask").modal('hide');
    $('form').trigger("reset");
    CKupdate();
}

$("#insert-check").click(function () {
    if (this.checked) {
        $("#insert-code-guide").removeAttr("disabled", "disabled");
    }
    else {
        $("#insert-code-guide").attr("disabled", "disabled");
    }
});

// check code staff
/** check visa **/
$("#insert-code-staff").change(function () {
    var dataPost = {
        text: $("#insert-code-staff").val()
    };
    $.ajax({
        type: "POST",
        url: '/StaffManage/CheckCode',
        data: JSON.stringify(dataPost),
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (data) {
            if (data == "1") { // trùng
                alert('Trùng code! Vui lòng nhập lại!');
                $("#insert-code-staff").val('');
                $("#insert-code-staff").focus();
            }
        }
    });
});