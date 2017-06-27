var getTrID = 0;

function OpenAddMembercard() {
    $("#modaltitle").html("Thêm mới Membercard");
    $("#modalaction").text("Ok");
    $("#modalbody").empty();
    $("#modalbody").append(
                "<div class='form-group'>"
                            + "<label class='control-label intalicText'>Tên loại Membercard <span class='text-red' style='color:red;'>*</span></label>"
                            + "<input class='form-control ' placeholder='Nhập tên loại' value='' id='nameMembercard' />"
                            + "<span class='existMessage' id='nameError'></span>"
                + "</div>"
                + "<div class='form-group'>"
                            + "<label class='control-label intalicText'>Giá trị nhỏ nhất <span class='text-red' style='color:red;'>*</span></label>"
                             + "<input class='form-control ' placeholder='Nhập min - chỉ nhập số' value='' id='minMembercard' onkeypress = 'return isNumberKey(event);' />"
                + "</div>"
                + "<div class='form-group'>"
                            + "<label class='control-label intalicText'>Giá trị lớn nhất <span class='text-red' style='color:red;'>*</span></label>"
                             + "<input class='form-control ' placeholder='Nhập max - chỉ nhập số' value='' id='maxMembercard' onkeypress = 'return isNumberKey(event);' />"
                             + "<span class='existMessage' id='maxError'></span>"
                + "</div>"
                //+ "<span class='existMessage' id='addError'></span>"
                + "<div class='form-group'>"
                            + "<label class='control-label intalicText'>Phần % giảm <span class='text-red' style='color:red;'>*</span></label>"
                             + "<input class='form-control ' placeholder='Ví dụ: 1,8' value='' id='percentMembercard' />"
                             + "<span class='existMessage' id='percentError'></span>"
                + "</div>"
                + "<span class='existMessage' id='addError'></span>"
                            );
    
    $("#minMembercard").on("input", function () {
        checkCanAdd();
    });
    $("#maxMembercard").on("input", function () {
        checkCanAdd();
    });
    $("#mymodalCustom").modal("show");
    $("#modalaction").on("click", function () {
        var nameMembercard = $("#nameMembercard").val();
        var minMembercard = Number($("#minMembercard").val());
        var maxMembercard = Number($("#maxMembercard").val());
        var _canAdd = checkCanAdd();
        var _checkName = checkName();
        if (_canAdd && _checkName) {
            $.ajax({
                url: "/MemberCardManage/AddMemberCard",
                data: { ID: 0, _name: nameMembercard, _min: minMembercard, _max: maxMembercard },
                type: "POST",
                success: function (data) {
                    if (data.result) {
                        $("#addError").text("");
                        $("#mymodalCustom").modal("hide");
                        window.location.href = "/MemberCardManage/Index";
                    }
                    else {
                        $("#addError").text("Có lỗi xảy ra!");
                    }
                }
            });
        }
    });
};

function OpenEditMembercard() {
    $("#modaltitle").html("Sửa Membercard");
    $("#modalaction").text("Ok");
    $("#modalbody").empty();
    $.ajax({
        url: "/MemberCardManage/LoadMBCardByID",
        data: { ID: getTrID },
        type: "POST",
        success: function (data) {
            if (data.result) {
                $("#modalbody").append(
                            "<div class='form-group'>"
                                        + "<label class='control-label intalicText'>Tên loại Membercard <span class='text-red' style='color:red;'>*</span></label>"
                                        + "<input class='form-control ' placeholder='Nhập tên loại' value='" + data._name + "' id='nameMembercard' />"
                                        + "<span class='existMessage' id='nameError'></span>"
                            + "</div>"
                            + "<div class='form-group'>"
                                        + "<label class='control-label intalicText'>Giá trị nhỏ nhất <span class='text-red' style='color:red;'>*</span></label>"
                                         + "<input class='form-control ' placeholder='Nhập min - chỉ nhập số' value='" + data._min + "' id='minMembercard' onkeypress = 'return isNumberKey(event);' />"
                            + "</div>"
                            + "<div class='form-group'>"
                                        + "<label class='control-label intalicText'>Giá trị lớn nhất <span class='text-red' style='color:red;'>*</span></label>"
                                         + "<input class='form-control ' placeholder='Nhập max - chỉ nhập số' value='" + data._max + "' id='maxMembercard' onkeypress = 'return isNumberKey(event);' />"
                                         + "<span class='existMessage' id='maxError'></span>"
                            + "</div>"
                            + "<div class='form-group'>"
                            + "<label class='control-label intalicText'>Phần % giảm <span class='text-red' style='color:red;'>*</span></label>"
                            + "<input class='form-control ' placeholder='Ví dụ: 1,8' value='" + data._percent + "' id='percentMembercard' />"
                            + "<span class='existMessage' id='percentError'></span>"
                            + "</div>"
                            + "<span class='existMessage' id='addError'></span>"
                                        );

                $("#minMembercard").on("input", function () {
                    checkCanAdd();
                });
                $("#maxMembercard").on("input", function () {
                    checkCanAdd();
                });
                $("#mymodalCustom").modal("show");
                $("#modalaction").on("click", function () {
                    var nameMembercard = $("#nameMembercard").val();
                    var minMembercard = Number($("#minMembercard").val());
                    var maxMembercard = Number($("#maxMembercard").val());
                    var percentMembercard = $("#percentMembercard").val();
                    var _canAdd = checkCanAdd();
                    var _checkName = checkName();
                    if (_canAdd && _checkName) {
                        $.ajax({
                            url: "/MemberCardManage/AddMemberCard",
                            data: { ID: getTrID, _name: nameMembercard, _min: minMembercard, _max: maxMembercard, _percent: percentMembercard },
                            type: "POST",
                            success: function (data) {
                                if (data.result) {
                                    $("#addError").text("");
                                    $("#mymodalCustom").modal("hide");
                                    window.location.href = "/MemberCardManage/Index";
                                }
                                else {
                                    $("#addError").text("Có lỗi xảy ra!");
                                }
                            }
                        });
                    }
                });
            }
            else {
                $("#modalbody").append(
                           "<div class='form-group'>"
                                       + "<label class='control-label'>Có lỗi xảy ra</label>"
                                       
                           + "</div>"
                           );
            }
        }
    });
};

function checkCanAdd() {
    var check=false;
    var minMembercard = Number($("#minMembercard").val());
    var maxMembercard = Number($("#maxMembercard").val());
    if (maxMembercard == 0 && minMembercard == 0) {
        $("#maxError").text("Vui lòng nhập giá trị!");
        check = false;
    }
    else
    {
        if (minMembercard >= maxMembercard) {
            check = false;
            $("#maxError").text("Giá trị min phải nhỏ hơn max!");
        }
        else {
            check = true;
            $("#maxError").text("");
        }
    }
    
    return check;
}
function checkName() {
    var check = false;
    var nameMembercard = $("#nameMembercard").val().length;
    if (nameMembercard > 0) {
        $("#nameError").text("");
        check = true;
    }
    else {
        $("#nameError").text("Vui lòng nhập tên!");
        check = false;
    }
    return check;
}
function isNumberKey(evt) {
    var charCode = (evt.which) ? evt.which : evt.keyCode
    return !(charCode > 31 && (charCode < 48 || charCode > 57));
}

function updateRow2Colum() {
    $('tbody tr td').find("input:checkbox").each(function () {
        if (this.checked) {
            getTrID = $(this).closest("tr").attr("value");
            OpenEditMembercard();
        }
    });
   
}

$("#btnRemove").on("click", function () {
    if ($("#listItemId").val() == "") {
        alert("Vui lòng chọn mục cần xóa !");
        return false;
    }
    var $this = $(this);
    var $tableWrapper = $("#tableMemberCard-Wrapper");
    var $table = $("#tableMemberCard");

    DeleteSelectedItem($this, $tableWrapper, $table, function (data) {

    });
    return false;
});

$("#tableMemberCard").on("change", ".cbItem", function () {
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

$("#tableMemberCard").on("change", "#allcb", function () {
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