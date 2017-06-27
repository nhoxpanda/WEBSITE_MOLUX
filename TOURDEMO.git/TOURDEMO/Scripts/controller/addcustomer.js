
/*** visa-passport ***/
$("#country-insert-cmnd").select2();
$("#country-insert-passport").select2();

/*** doanh nghiệp ***/
$("#ddlCustomerType").select2();
$("#ddlNguonDen").select2();
$("#ddlNhomKH").select2();
$("#ddlKyHopDong").select2();
$("#insert-province-company").select2();
$("#insert-district-company").select2();
$("#insert-ward-company").select2();
$("#customer-nhomkh-company").select2();
$("#customer-nguonden-company").select2();
$("#edit-customer-company").select2();
$("#customer-quanly-company").select2();
$("#insert-detail-company").select2({
    tags: true
});

/*** cá nhân ***/
$("#insert-province-personal").select2();
$("#insert-district-personal").select2();
$("#insert-ward-personal").select2();
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

//$('a.toggle-vis').on('click', function (e) {
//    e.preventDefault();
//    if ($(this).hasClass('selected')) {
//        $(this).removeClass('selected');
//    }
//    else {
//        $(this).addClass('selected');
//    }
//    // Get the column API object
//    var column = table.column($(this).attr('data-column'));
//    // Toggle the visibility
//    column.visible(!column.visible());
//});

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
                        //if (!confirm("Dữ liệu trùng lắp! Bạn có muốn lưu lại không?")) {
                        //    $("#VisaNumber" + newNum).val('');
                        //    $("#VisaNumber" + newNum).focus();
                        //}
                        alert("Dữ liệu trùng lắp! Vui lòng nhập lại!");
                        $("#VisaNumber" + newNum).val('');
                        $("#VisaNumber" + newNum).focus();
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

$("#check-customer-company").change(function () {
    if ($(this).is(":checked"))
        $('#customer-company').attr('disabled', false);
    else
        $('#customer-company').attr('disabled', true);
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
                alert("Dữ liệu trùng lắp! Vui lòng nhập lại!");
                    $("#insert-detail-company").select2('val', '0');
                    $("#insert-detail-company").focus();
                
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
                alert("Dữ liệu trùng lắp! Vui lòng nhập lại!");
                    $("#insert-detail-personal").select2('val', '0');
                    $("#insert-detail-personal").focus();
                
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
                alert("Dữ liệu trùng lắp! Vui lòng nhập lại!");
                    $("#dienthoai-cty").val('');
                    $("#dienthoai-cty").focus();
                
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
                alert("Dữ liệu trùng lắp! Vui lòng nhập lại!");
                    $("#didong-cty").val('');
                    $("#didong-cty").focus();
                
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
                alert("Dữ liệu trùng lắp! Vui lòng nhập lại!");
                    $("#dienthoai-canhan").val('');
                    $("#dienthoai-canhan").focus();
                
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
                alert("Dữ liệu trùng lắp! Vui lòng nhập lại!");
                    $("#didong-canhan").val('');
                    $("#didong-canhan").focus();
                
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
                alert("Dữ liệu trùng lắp! Vui lòng nhập lại!");
                    $("#mailcanhan-cty").val('');
                    $("#mailcanhan-cty").focus();
                
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
                alert("Dữ liệu trùng lắp! Vui lòng nhập lại!");
                    $("#mailcongty-cty").val('');
                    $("#mailcongty-cty").focus();
                
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
                alert("Dữ liệu trùng lắp! Vui lòng nhập lại!");
                    $("#email-canhan").val('');
                    $("#email-canhan").focus();
                
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
                alert("Dữ liệu trùng lắp! Vui lòng nhập lại!");
                    $("#so-cmnd").val('');
                    $("#so-cmnd").focus();
                
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
                alert("Dữ liệu trùng lắp! Vui lòng nhập lại!");
                    $("#so-passport").val('');
                    $("#so-passport").focus();
                
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
                alert("Dữ liệu trùng lắp! Vui lòng nhập lại!");
                    $("#VisaNumber1").val('');
                    $("#VisaNumber1").focus();
                
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

// tỉnh thành, quận huyện, phường xã
$("#insert-province-company").change(function () {
    $.getJSON('/Home/DistrictList?id=' + $('#insert-province-company').val(), function (data) {
        var items = '<option>Quận/Huyện</option>';
        $.each(data, function (i, gd) {
            items += "<option value='" + gd.Value + "'>" + gd.Text + "</option>";
        });
        $('#insert-district-company').html(items);
    });
})

$("#insert-district-company").change(function () {
    $.getJSON('/Home/WardList?id=' + $('#insert-district-company').val(), function (data) {
        var items = '<option>Phường/Xã</option>';
        $.each(data, function (i, gd) {
            items += "<option value='" + gd.Value + "'>" + gd.Text + "</option>";
        });
        $('#insert-ward-company').html(items);
    });
})

//

$("#insert-province-personal").change(function () {
    $.getJSON('/Home/DistrictList?id=' + $('#insert-province-personal').val(), function (data) {
        var items = '<option>Quận/Huyện</option>';
        $.each(data, function (i, gd) {
            items += "<option value='" + gd.Value + "'>" + gd.Text + "</option>";
        });
        $('#insert-district-personal').html(items);
    });
})

$("#insert-district-personal").change(function () {
    $.getJSON('/Home/WardList?id=' + $('#insert-district-personal').val(), function (data) {
        var items = '<option>Phường/Xã</option>';
        $.each(data, function (i, gd) {
            items += "<option value='" + gd.Value + "'>" + gd.Text + "</option>";
        });
        $('#insert-ward-personal').html(items);
    });
})