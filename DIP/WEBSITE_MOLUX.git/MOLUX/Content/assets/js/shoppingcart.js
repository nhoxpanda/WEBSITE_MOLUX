function removeFromCart(id) {
    var dataPost = { id: id };
    $.ajax({
        type: "POST",
        url: '/ShoppingCart/RemoveFromCart',
        data: JSON.stringify(dataPost),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            $("#countTotal").text(data.CartCount);
            $("#cartTotal").text(data.StrCartTotal);
            $("#item" + id).fadeOut();
            $.notify("Đã xóa sản phẩm này khỏi giỏ hàng!", { animationType: "scale", color: "#fff", background: "#00C907", icon: "check" });
        }
    });
}
function updateQuantity(id) {
    var dataPost = {
        id: id,
        qty: $("#quantity" + id).val()
    };
    $.ajax({
        type: "POST",
        url: '/ShoppingCart/UpdateQuantity',
        data: JSON.stringify(dataPost),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            $("#cartTotal").text(data.StrCartTotal);
            console.log(parseFloat($("#price" + id).text().replace(".", "").replace("₫", "")));
            // tính thành tiền
            var dataPost1 = {
                price: $("#price" + id).text(),
                qty: $("#quantity" + id).val()
            };
            $.ajax({
                type: "POST",
                url: '/ShoppingCart/CalculatorPriceQuantity',
                data: JSON.stringify(dataPost1),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (rs) {
                    $("#total" + id).text(rs);
                }
            });
        }
    });
}
$("#ddlCountry").change(function () {
    $.getJSON('/Orders/GetCityList?code=' + $('#ddlCountry').val(), function (data) {
        var items = '<option>-- Tỉnh/TP --</option>';
        $.each(data, function (i, gd) {
            items += "<option value='" + gd.Value + "'>" + gd.Text + "</option>";
        });
        $('#ddlCity').html(items);
    });
})

function OnSuccess() {
    $.notify("Chúc mừng bạn đã đặt hàng thành công! Vui lòng vào mail để kiểm tra lại đơn hàng!", { animationType: "scale", color: "#fff", background: "#00C907", icon: "check" });
    setTimeout(function () { window.location.href = '/'; }, 4000);
}

function OnFailure() {
    $.notify("Lỗi! Vui lòng xem lại!", { animationType: "scale", color: "#fff", background: "#FF0000", icon: "close" });
}