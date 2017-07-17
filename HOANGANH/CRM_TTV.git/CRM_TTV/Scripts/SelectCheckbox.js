$(document).ready(function () {
    $("#btnSave").attr("disabled", "disabled");
    CheckSelect();
});

function SelectAllCb() {
    var isChecked = $('#allcb').is(':checked');
    if (isChecked) {
        $("input:checkbox[name='cb']").prop("checked", true);

        $("#btnEdit").removeAttr("disabled", "disabled");
        $("#btnRemove").attr("disabled", "disabled");
        $("#btnUpdate").removeAttr("disabled", "disabled");
        $("#btnPassword").removeAttr("disabled", "disabled");
        $("#btnLock").removeAttr("disabled", "disabled");
        $("#btnUnLock").removeAttr("disabled", "disabled");
    }
    else {
        $("input:checkbox[name='cb']").prop("checked", false);
        $("#btnEdit").removeAttr("disabled", "disabled");
        $("#btnRemove").removeAttr("disabled", "disabled");
        $("#btnUpdate").removeAttr("disabled", "disabled");
        $("#btnPassword").removeAttr("disabled", "disabled");
        $("#btnLock").removeAttr("disabled", "disabled");
        $("#btnUnLock").removeAttr("disabled", "disabled");
    }
    CheckSelect();
}

function CheckSelect() {
    var cb = 0;
    $('tbody tr td').find("input:checkbox[name='cb']").each(function () {
        if (this.checked) { cb = cb + 1; }
    });
    if (cb == 0) {
        $("#btnEdit").attr("disabled", "disabled");
        $("#btnRemove").attr("disabled", "disabled");
        $("#btnUpdate").attr("disabled", "disabled");
        $("#btnPassword").attr("disabled", "disabled");
        $("#btnLock").attr("disabled", "disabled");
        $("#btnUnLock").attr("disabled", "disabled");

    } else if (cb == 1) {
        $("#btnEdit").removeAttr("disabled", "disabled");
        $("#btnRemove").removeAttr("disabled", "disabled");
        $("#btnSave").removeAttr("disabled", "disabled");
        $("#btnUpdate").removeAttr("disabled", "disabled");
        $("#btnPassword").removeAttr("disabled", "disabled");
        $("#btnLock").removeAttr("disabled", "disabled");
        $("#btnUnLock").removeAttr("disabled", "disabled");
    }
    else {
        $("#btnEdit").attr("disabled", "disabled");
        $("#btnRemove").removeAttr("disabled", "disabled");
        $("#btnSave").attr("disabled", "disabled");
        $("#btnUpdate").attr("disabled", "disabled");
        $("#btnPassword").attr("disabled", "disabled");
        $("#btnLock").attr("disabled", "disabled");
        $("#btnUnLock").attr("disabled", "disabled");
    }
}