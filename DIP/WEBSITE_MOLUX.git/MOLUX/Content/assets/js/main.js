
$(document).ready(function () {

    //init plugins

    $("#owl-slide").owlCarousel({
        lazyLoad: !0,
        navigation: !0,
        autoPlay: 8000,
        stopOnHover: !0,
        slideSpeed: 50,
        paginationSpeed: 400,
        singleItem: !0
    });

    //$('#logo-slider').owlCarousel({
    //    lazyLoad: !0,
    //    navigation: true,
    //    autoPlay: 4000,
    //    stopOnHover: !0,
    //    slideSpeed: 50,
    //    paginationSpeed: 400,
    //    pagination: false,
    //    margin: 10,
    //    nav: true,
    //    dots: false,
    //    nav: true,
    //    navText: ["<i class='fa fa-angel-left'></i>", "<i class='fa fa-angel-right'></i>"],
    //    responsive: {
    //        0: {
    //            items: 3
    //        },
    //        600: {
    //            items: 6
    //        },
    //        1000: {
    //            items: 9
    //        }
    //    }
    //})

    //menu mobile
    var pathname = window.location.href;
    //menu category
    var hiddenToggler = $("#hidden-toggler");
    var hidden = $('#hidden');

    if (pathname == 'http://molux.dip.vn/' || pathname == 'http://localhost:59360/') {
        $("#hidden").show();
    }
    else {
        $("#hidden").slideUp();

        hiddenToggler.on({
            mouseenter: function () {
                hidden.stop().slideDown(200);
            },
            mouseleave: function () {
                hidden.stop().slideUp(200);
            }
        });

        hidden.on({
            mouseenter: function () {
                hidden.stop().slideDown(200);
            },
            mouseleave: function () {
                hidden.stop().slideUp(200);
            }
        });
    }

    $(".flexy-menu").flexymenu({
        speed: 400,
        type: "vertical",
        indicator: !1
    });

    //slider in category
    $("#owl-slide-product").owlCarousel({
        items: 2,
        lazyLoad: true,
        loop: true,
        autoPlay: true,
        navigation: true
    });

   $("#btnSubscribe").click(function () {
            var dataPost = { email: $("#txtSubscribeEmail").val() };
            $.ajax({
                type: "POST",
                url: '/Home/SubscribeEmail',
                data: JSON.stringify(dataPost),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    $.notify("Đăng ký nhận bản tin thành công! Cảm ơn quý khách!", { animationType: "scale", color: "#fff", background: "#00C907", icon: "check" });
                }
            });
        })
})
