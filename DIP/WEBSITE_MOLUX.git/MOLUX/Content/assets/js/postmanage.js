$("#menu_selected").change(function () {
    var id = Number($('#menu_selected option:selected').val());
    $.ajax({
        url: "/PostManage/LoadCate",
        data: { ID: id },
        type: "GET",
        success: function (data) {
            var arr = $.parseJSON(JSON.stringify(data.listCateItem));
            $("#cate_selected").empty();
            $("#cate_selected").append($("<option></option>").val("0").html("Tất cả"));
            $.each(arr, function (i, obj) {
                $("#cate_selected").append($("<option></option>").val(obj.Value).html(obj.Text));
            });
            var t = $('#postTable').DataTable();
            t.clear().draw();
            $.ajax({
                url: "/PostManage/LoadPostByMenuID",
                data: { IDMenu: id },
                type: "GET",
                success: function (data2) {
                    var arr2 = $.parseJSON(JSON.stringify(data2.listPost));

                    var counter = 1;
                    $.each(arr2, function (x, obj2) {
                        var html = '<button class="btn btn-xs green dropdown-toggle" type="button" onclick="EditPost("' + obj2.post.Id + ')">'
                            + 'Edit'
                            + '</button>'
                        var html2 = '';
                        if (obj2.post.IsShow === true) {
                            html2 = '<button id="btnVisible" class="btn btn-xs green dropdown-toggle" type="button" onclick="changeIsShow(' + obj2.post.Id + '">'
                                + 'Ẩn'
                                + '</button>'
                        }
                        else {
                            html2 = '<button id="btnVisible" class="btn btn-xs green dropdown-toggle" type="button" onclick="changeIsShow(' + obj2.post.Id + '">'
                                        + 'Hiển thị'
                                        + '</button>'
                        }
                        var html3 = '<button class="btn btn-xs red dropdown-toggle" type="button" onclick="DeletePost(' + obj2.post.Id + ');">'
                                + 'Delete'
                                + '</button>'
                        var action = html + html2 + html3;
                        t.row.add([
                                   counter,
                                   obj2.post.Title,
                                   obj2.post.Description,
                                   obj2.ChuyenMuc,
                                   obj2.post.MetaTitle,
                                   obj2.post.MetaDesc,
                                   obj2.post.IsShow,
                                   action
                        ]).draw(false);
                        counter++;
                    });
                }
            });
        }
    });
});
$("#cate_selected").change(function () {
    var idMenu = Number($('#menu_selected option:selected').val());
    var idCate = Number($('#cate_selected option:selected').val());
    $.ajax({
        url: "/PostManage/LoadPostByMenuID",
        data: { IDMenu: idMenu, IDCate: idCate },
        type: "GET",
        success: function (data2) {
            var t = $('#postTable').DataTable();
            t.clear().draw();
            var arr2 = $.parseJSON(JSON.stringify(data2.listPost));
            var counter = 1;
            $.each(arr2, function (x, obj2) {
                console.log(obj2);
                var html = '<button class="btn btn-xs green dropdown-toggle" type="button" onclick="EditPost("' + obj2.post.Id + ')">'
                + 'Edit'
                + '</button>'
                var html2 = '';

                if (obj2.post.IsShow === true) {
                    html2 = '<button id="btnVisible" class="btn btn-xs green dropdown-toggle" type="button" onclick="changeIsShow(' + obj2.post.Id + '">'
                        + 'Ẩn'
                        + '</button>'
                }
                else {
                    html2 = '<button id="btnVisible" class="btn btn-xs green dropdown-toggle" type="button" onclick="changeIsShow(' + obj2.post.Id + '">'
                                + 'Hiển thị'
                                + '</button>'
                }
                var html3 = '<button class="btn btn-xs red dropdown-toggle" type="button" onclick="DeletePost(' + obj2.post.Id + ');">'
                        + 'Delete'
                        + '</button>'
                var action = html + html2 + html3;
                t.row.add([
                           counter,
                           obj2.post.Title,
                           obj2.post.Description,
                           obj2.ChuyenMuc,
                           obj2.post.MetaTitle,
                           obj2.post.MetaDesc,
                           obj2.post.IsShow,
                           action
                ]).draw(false);
                counter++;
            });
        }
    });
});

function deletePost(id){
    var dataPost = { id: id };
    $.ajax({
        type: "POST",
        url: '/Admin/PostManage/DeletePost',
        data: JSON.stringify(dataPost),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            $("#post" + id).fadeOut();
        }
    });
}