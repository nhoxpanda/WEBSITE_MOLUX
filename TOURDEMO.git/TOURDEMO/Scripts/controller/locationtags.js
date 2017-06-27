function create(id, type) {
    $("#modalTag").html("<div class='modal fade' id='modal-insert-tag' role='document' aria-hidden='true'>"
    + "<div class='modal-dialog'>"
        + "<div class='modal-content'>"
            + "<div class='modal-header'>"
                + "<button type='button' class='close' data-dismiss='modal' aria-hidden='true'></button>"
                + "<h4 class='modal-title'>Địa lý</h4>"
            + "</div>"
            + "<div class='modal-body'>"
                + "<div class='form-horizontal'>"
                    + "<div class='row'>"
                        + "<div class='col-md-12'>"
                            + "<div class='form-group'>"
                                + "<label class='control-label col-lg-3 col-md-3'>Tên</label>"
                                + "<div class='col-lg-5 col-md-5'>"
                                    + "<input type='text' class='form-control' required='required' name='name' id='name' />"
                                + "</div>"
                            + "</div>"
                            + "<div class='form-group'>"
                                + "<label class='control-label col-lg-3 col-md-3'></label>"
                                + "<div class='col-lg-5 col-md-5'>"
                                    + "<a href='#' onclick='createSave(" + id + "," + type + ")' class='btn btn-primary'>&nbsp;Lưu</a>"
                                + "</div>"
                            + "</div>"
                        + "</div>"
                    + "</div>"
                + "</div>"
            + "</div>"
        + "</div>"
    + "</div>"
+ "</div>");
    $("#modal-insert-tag").modal("show");
}

function createSave(id, type) {
    var dataPost = {
        id: id,
        type: type,
        name: $("#name").val()
    };
    $.ajax({
        type: "POST",
        url: '/LocationTagsManage/Create',
        data: JSON.stringify(dataPost),
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (data) {
            $("#modal-insert-tag").modal("hide");
            alert("Đã lưu");
            window.location.href = "/LocationTagsManage";
        }
    });
}

function update(id) {
    var dataPost = {
        id: id
    };
    $.ajax({
        type: "POST",
        url: '/LocationTagsManage/Edit',
        data: JSON.stringify(dataPost),
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (data) {
            $("#modalTag").html(data);
            $("#modal-edit-tag").modal("show");
        }
    });
}

function removeItem(id) {
    var dataPost = {
        id: id
    };
    $.ajax({
        type: "POST",
        url: '/LocationTagsManage/Delete',
        data: JSON.stringify(dataPost),
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (data) {
            if (data == "1")
            {
                $("#row" + id).hide();
            }
            else {
                alert("Không xóa được!")
            }
        }
    });
}