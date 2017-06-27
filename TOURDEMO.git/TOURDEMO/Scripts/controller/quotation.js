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
    console.log(column);
    // Toggle the visibility
    column.visible(!column.visible());
});

CKEDITOR.replace("insert-note-quotation");
$("#insert-code-tour").select2();
$("#insert-code-customer").select2();
$("#insert-code-country").select2();
$("#insert-staff-quotation").select2();
$("#insert-tags").select2();
$("#insert-currency").select2();

$("#btnEdit").click(function () {
    var dataPost = {
        id: $("input[type='checkbox']:checked").val()
    };

    $.ajax({
        type: "POST",
        url: '/QuotationManage/EditInfoQuotation',
        data: JSON.stringify(dataPost),
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (data) {
            $("#data-quotation").html(data);
            CKEDITOR.replace("edit-note-quotation");
            $("#edit-code-tour").select2();
            $("#edit-code-customer").select2();
            $("#edit-code-country").select2();
            $("#edit-tags").select2();
            $("#edit-staff-quotation").select2();
            $("#edit-currency").select2();
            $('#tonggiatribaogiaedit').number(true, 0);
            $("#modal-edit-quotation").modal("show");
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
function isNumberKey(evt) {
    var charCode = (evt.which) ? evt.which : evt.keyCode
    return !(charCode > 31 && (charCode < 48 || charCode > 57));
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

function OnSuccessCustomer() {
    $("#modal-insert-customer").modal("hide");
    $("#insert-code-customer").select2();
  
}