$("#edit-mailconfig").select2();

$("#btnAddReceiveList").click(function () {
    var dataPost = { id: $("#Id").val() };
    $.ajax({
        type: "POST",
        data: JSON.stringify(dataPost),
        url: '/MailAutoSend/LoadListReceiveUpdate',
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (data) {
            $("#listReceive").html(data);

            $('.dataTable2').dataTable({
                order: [],
                columnDefs: [{ orderable: false, targets: [0] }]
            });

            $(".dataTable2").dataTable().columnFilter({
                sPlaceHolder: "head:after",
                aoColumns: [null,
                            { type: "text" },
                            { type: "text" },
                            { type: "text" },
                            { type: "text" }]
            });

            $("#tableReveice").on("change", ".cbItem", function () {
                var ItemID = $(this).val();
                var currentlistItemID = $("#listItemIdAdd").val();
                var stringBranchID = "";
                if ($(this).prop('checked')) {
                    currentlistItemID += ItemID + ",";
                    $("#listItemIdAdd").val(currentlistItemID);
                } else {
                    $("#listItemIdAdd").val(currentlistItemID.replace(ItemID + ",", ""));
                }
            });

            $("#btnLuuDS").on("click", function () {
                if ($("#listItemIdAdd").val() == "") {
                    alert("Vui lòng chọn danh sách nhận mail!");
                    return false;
                }
                var $form = $("#formReceive");
                var options = {
                    url: $form.attr("action"),
                    type: $form.attr("method"),
                    data: $form.serialize(),
                };
                $.ajax(options).done(function (data) {
                    $("#data-receive").html(data);
                    $("#modal-add-receive").modal("hide");
                });
            });

            $("#modal-add-receive").modal('show');
        }
    });
})

function removeFromList(stt, id) {
    var dataPost = {
        id: id,
        stt: stt
    };
    $.ajax({
        type: "POST",
        url: "/MailAutoSend/DeleteReceiveInUpdate",
        data: JSON.stringify(dataPost),
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (data) {
            $("#item-" + id).hide();
        }
    })
}