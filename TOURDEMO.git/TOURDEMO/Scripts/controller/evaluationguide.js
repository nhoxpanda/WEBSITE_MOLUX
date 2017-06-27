$(".dataTable").dataTable().columnFilter({
    colReorder: true,
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
                { type: "text" }]
});

$("#search-guide").select2();
$("#search-tour").select2();
$("#insert-guide").select2();
$("#insert-tour").select2();
$("#insert-area").select2();
$("#insert-province").select2();
$("#insert-evaluation1").select2();
xoadulieu();

$("#btnEdit").click(function () {
    var dataPost = {
        id: $("input[type='checkbox']:checked").val()
    };
    $.ajax({
        type: "POST",
        url: '/EvaluationGuideManage/Edit',
        data: JSON.stringify(dataPost),
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (data) {
            $("#edit-evaluation").html(data);
            $("#edit-guide").select2();
            $("#edit-tour").select2();
            $("#edit-area").select2();
            $("#edit-province").select2();
            $("#edit-evaluation-1").select2();
            EditDuplicate();
        }
    });
});

$("#btnSearch").click(function () {
    var dataPost = {
        guide: $("#search-guide").val(),
        tour: $("#search-tour").val()
    };
    $.ajax({
        type: "POST",
        url: '/EvaluationGuideManage/Search',
        data: JSON.stringify(dataPost),
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (data) {
            $("#data-evaluation").html(data);
            xoadulieu();
            $(".dataTable").dataTable().columnFilter({
                colReorder: true,
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
                            { type: "text" }]
            });
        }
    });
});

/** Xoa du lieu **/
function xoadulieu() {
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
    $("table#tableDictionary").delegate("tr", "click", function () {
        var dataPost = { id: $(this).find("input[type='checkbox']").val() };
        $.ajax({
            type: "POST",
            url: '/EvaluationGuideManage/Detail',
            data: JSON.stringify(dataPost),
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            success: function (data) {
                $("#tab1").html(data);
            }
        });
    });
}

$("#insert-area").change(function () {
    $.getJSON('/EvaluationGuideManage/ProvinceList?id=' + $('#insert-area').val(), function (data) {
        var items = '<option value=0>Chọn tỉnh thành</option>';
        $.each(data, function (i, gd) {
            items += "<option value='" + gd.Value + "'>" + gd.Text + "</option>";
        });
        $('#insert-provice').html(items);
        $('#insert-provice').select2();
    });
})

///*** duplicate form add service partner ***/
$('#btnAddD').click(function () {
    var num = $('.clonedInput').length, // how many "duplicatable" input fields we currently have
        newNum = new Number(num + 1),      // the numeric ID of the new input field being added
        newElem = $('#entry' + num).clone().attr('id', 'entry' + newNum).fadeIn('slow'); // create the new element via clone(), and manipulate it's ID using newNum value
    // manipulate the name/id values of the input inside the new element

    newElem.find('.insert-title').attr('id', 'insert-title' + newNum).text('Tiêu chí đánh giá ' + newNum);
    newElem.find('.insert-evaluation').attr('id', 'insert-evaluation' + newNum).attr('name', 'EvaluationCriteriaId' + newNum).val('');
    newElem.find('.insert-point').attr('id', 'insert-point' + newNum).attr('name', 'EvaluationPoint' + newNum).val('');
    newElem.find('.insert-note').attr('id', 'insert-note' + newNum).attr('name', 'EvaluationNote' + newNum);

    // insert the new element after the last "duplicatable" input field
    $('#entry' + num).after(newElem);
    $("#insert-evaluation" + newNum).select2();

    for (var i = 1; i < newNum; i++) {
        $("#entry" + newNum + " #select2-insert-evaluation" + i + "-container").parent().parent().parent().remove();
    }

    // enable the "remove" button
    $('#btnDel').attr('disabled', false);

    // count service
    $("#NumberEvaluation").val(newNum);
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
        $('#btnAddD').attr('disabled', false).prop('value', "add section");
    });
    return false;

    $('#btnAddD').attr('disabled', false);
    // count service
    $("#NumberEvaluation").val(num);
});

///*** duplicate form add service partner ***/
function EditDuplicate() {
    $('#btnAddEdit').click(function () {
        var num = $('.clonedInputEdit').length, // how many "duplicatable" input fields we currently have
            newNum = new Number(num + 1),      // the numeric ID of the new input field being added
            newElem = $('#entryEdit' + num).clone().attr('id', 'entryEdit' + newNum).fadeIn('slow'); // create the new element via clone(), and manipulate it's ID using newNum value
        // manipulate the name/id values of the input inside the new element

        newElem.find('.edit-title').attr('id', 'edit-title' + newNum).text('Tiêu chí đánh giá ' + newNum);
        newElem.find('.edit-evaluation').attr('id', 'edit-evaluation' + newNum).attr('name', 'EditEvaluationCriteriaId' + newNum).val('');
        newElem.find('.edit-point').attr('id', 'edit-point' + newNum).attr('name', 'EditEvaluationPoint' + newNum).val('');
        newElem.find('.edit-note').attr('id', 'edit-note' + newNum).attr('name', 'EditEvaluationNote' + newNum);

        // insert the new element after the last "duplicatable" input field
        $('#entryEdit' + num).after(newElem);
        $("#edit-evaluation" + newNum).select2();

        for (var i = 1; i < newNum; i++) {
            $("#entryEdit" + newNum + " #select2-edit-evaluation" + i + "-container").parent().parent().parent().remove();
        }

        // enable the "remove" button
        $('#btnDelEdit').attr('disabled', false);

        // count service
        $("#NumberEvaluationEdit").val(newNum);
    });

    $('#btnDelEdit').click(function () {
        // confirmation
        var num = $('.clonedInputEdit').length;
        // how many "duplicatable" input fields we currently have
        $('#entryEdit' + num).slideUp('slow', function () {
            $(this).remove();
            // if only one element remains, disable the "remove" button
            if (num - 1 === 1)
                $('#btnDelEdit').attr('disabled', true);
            // enable the "add" button
            $('#btnAddEdit').attr('disabled', false).prop('value', "add section");
        });
        return false;

        $('#btnAddEdit').attr('disabled', false);
        // count service
        $("#NumberEvaluationEdit").val(num);
    });
}
