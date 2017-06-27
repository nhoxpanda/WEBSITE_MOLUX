
$("#ddlKyBaoCao").select2();
$("#ddlNhanVien").select2();
checkExport();

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

$("#btnSearch").click(function () {
    $("#listItemId").val('');
    //
    var dataPost = {
        start: $("#txtStartDate").val(),
        end: $("#txtEndDate").val(),
        staff: $("#ddlNhanVien").val()
    };
    $.ajax({
        type: "POST",
        url: '/AppointmentCustomerReport/FilterDate',
        data: JSON.stringify(dataPost),
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (data) {
            $("#data-tour").html(data);
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
                            { type: "text" }]
            });
            // kéo dài kích thước cột
            colResize();
            checkExport();
        }
    });
})

////////// kỳ báo cáo
$("#ddlKyBaoCao").change(function () {
    var dataPost = {
        id: $("#ddlKyBaoCao").val()
    };
    $.ajax({
        type: "POST",
        url: '/AppointmentCustomerReport/GetStartEndDate',
        data: JSON.stringify(dataPost),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            $("#startEndDate").html(data);
        }
    });
})

/// check

function checkExport() {
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
}