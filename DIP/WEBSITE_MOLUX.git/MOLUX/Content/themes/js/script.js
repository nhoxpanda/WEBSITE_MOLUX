new mlPushMenu( document.getElementById( 'mp-menu' ), document.getElementById( 'bg_menu' ), {
	type : 'cover'
});

$(document).ready(function(){
    $('#mb-search').on('click',function(){
        $('.mb-search-box').show();
        $('.mb-search-box input[type=text]').focus();
    });
    $('.mb-search-box .header_text .fl').on('click',function(){
        $('.mb-search-box').hide();
    });
    $('.mb-search-box .search_btn').on('click',function(){
        $('.mb-search-box form').submit();
    });
});
$(".dropdown dt a").click(function() {
    $(".dropdown dd ul").toggle();
});
$(".dropdown dd ul li a").click(function() {
    var text = $(this).html();
    $(".dropdown dt a span").html(text);
    $(".dropdown dd ul").hide();
    var get = $(this).find(".value_selec").html();
    var l = document.createElement("a");
    l.href = window.location.href;
    var link = l.origin+l.pathname+'?setlocation='+get;
    window.location.href = link;
    console.log(link);
});
$(document).bind('click', function(e) {
    var $clicked = $(e.target);
    if (! $clicked.parents().hasClass("dropdown"))
        $(".dropdown dd ul").hide();
});
function go_link_on_select(name_select){
    var domain = location.host;
    $(name_select).on('change',function(){
        var link = $(name_select+' option:selected').attr('data-link');
        window.location = link;
    });
}
function get_thumb_vimeo(url) {
          var firstPart = url.split('?')[0].split("/");
          var vid = firstPart[firstPart.length - 1];
          var vimeoVideoID = vid;
            $.getJSON('http://www.vimeo.com/api/v2/video/' + vimeoVideoID + '.json?callback=?', {format: "json"}, function(data) {
                     var link = data[0].thumbnail_large;
                     return vimeoVideoID;
                     alert(link);
            });

            
        };
function show_more_text(number_char,box_text){
    
    var showChar = number_char;  
    var ellipsestext = "...";
    var moretext = "[ + Xem thêm + ]";
    var lesstext = "[ - Ẩn bớt - ]";
    

    $(box_text).each(function() {
        var content = $(this).html();
 
        if(content.length > showChar) {
 
            var c = content.substr(0, showChar);
            var h = content.substr(showChar, content.length - showChar);
 
            var html = c + '<span class="moreellipses">' + ellipsestext+ '&nbsp;</span><span class="morecontent"><span>' + h + '</span>&nbsp;&nbsp;<a href="" class="morelink">' + moretext + '</a></span>';
 
            $(this).html(html);
        }
 
    });
 
    $('.morelink').click(function(){
        if($(this).hasClass("less")) {
            $(this).removeClass("less");
            $(this).html(moretext);
        } else {
            $(this).addClass("less");
            $(this).html(lesstext);
        }
        $(this).parent().prev().toggle();
        $(this).prev().toggle();
        return false;
});
}
var Youtube = (function () {
    'use strict';

    var video, results;

    var getThumb = function (url, size) {
        if (url === null) {
            return '';
        }
        size    = (size === null) ? 'big' : size;
        results = url.match('[\\?&]v=([^&#]*)');
        video   = (results === null) ? url : results[1];

        if (size === 'small') {
            return 'http://img.youtube.com/vi/' + video + '/2.jpg';
        }
        return 'http://img.youtube.com/vi/' + video + '/0.jpg';
    };

    return {
        thumb: getThumb
    };
}());
function box_auto_height(name_box) {

  var elementHeights = $(name_box).map(function() {
    return $(this).height();
  }).get();
  var maxHeight = Math.max.apply(null, elementHeights);
  $(name_box).height(maxHeight);
};
function getIdYoutube(html) {
        var pattern1 = /(?:http?s?:\/\/)?(?:www\.)?(?:vimeo\.com)\/?(.+)/g;
        var pattern2 = /(?:http?s?:\/\/)?(?:www\.)?(?:youtube\.com|youtu\.be)\/(?:watch\?v=)?(.+)/g;
        var pattern3 = /([-a-zA-Z0-9@:%_\+.~#?&//=]{2,256}\.[a-z]{2,4}\b(\/[-a-zA-Z0-9@:%_\+.~#?&//=]*)?(?:jpg|jpeg|gif|png))/gi;
        
        if(pattern1.test(html)){
           var replacement = '<iframe src="https://player.vimeo.com/video/$1?autoplay=1" frameborder="0" webkitallowfullscreen mozallowfullscreen allowfullscreen></iframe>';
           
           var html = html.replace(pattern1, replacement);
        }
           
        
        if(pattern2.test(html)){
              var replacement = '<iframe src="https://www.youtube.com/embed/$1?autoplay=1" frameborder="0" allowfullscreen webkitallowfullscreen mozallowfullscreen></iframe>';
              var html = html.replace(pattern2, replacement);
        } 
        
        
        if(pattern3.test(html)){
            var replacement = '<a href="$1" target="_blank"><img class="sml" src="$1" /></a><br />';
            var html = html.replace(pattern3, replacement);
        } 
        return html;
}
function closebPopup(el) {
    var bPopup = $('#'+el+'').bPopup();
    bPopup.close();
}
function popupSignup() {
    closebPopup('popup-login');
    $('#popup-signup').bPopup({
        speed: 450,
        transition: 'slideDown',
        zIndex:99999
    });
}
function popupLogin() {
    closebPopup('popup-signup');
    $('#popup-login').bPopup({
        speed: 450,
        transition: 'slideDown',
        zIndex:99999
    });
}
$(document).ready(function(){
    $('#subscribe-btn').click(function(){
        var email = $('#subscribe-email').val();
        if(email == '') {
            alert('Vui lòng nhập email đăng ký');
            $('#subscribe-email').focus();
        }else if(!isemail(email)) {
            alert('Vui lòng nhập email đăng ký hợp lệ');
            $('#subscribe-email').focus();
        }else {
            var dataPost = { str: $('#subscribe-email').val() };
            $.ajax({
                type: "POST",
                url: '/Home/Subscribe',
                data: JSON.stringify(dataPost),
                contentType: "application/json; charset=utf-8",
                dataType: "html",
                success: function (data) {
                    alert("Đăng ký thành công! Cảm ơn bạn!");
                }
            });
        }
    });
    $('.intro_cate_more').on('click',function(){
        var cno = $(this);
        var el = $('.intro_cate_content'),
        curHeight = el.height(),
        autoHeight = el.css('height', 'auto').height();
        el.height(curHeight).animate({height: autoHeight}, 500);
        cno.hide();
    });
    $('.intro_cate_close').on('click',function(){
        var el = $('.intro_cate_content');
        el.animate({height: '32px'}, 500);
        $('.intro_cate_more').show();
        var offset = el.offset();
        window.scrollTo(offset.top, 0);
    });
});
$(document).ready(function(){
    box_auto_height('.pro_con');
    box_auto_height('.phukien_con');
    box_auto_height('.ser_con');
    box_auto_height('.name_pro');
    box_auto_height('.nam_ser');
    box_auto_height('.name_pk');
    show_more_text(350,"#box_more_text");
    $(".title_pk_sing").on('click',function(){
       $("html,body").animate({scrollTop:$(".coment_web").offset().top},'slow') 
    });
});
    $('#slider_video li').on('click', function () {
        var embed = getIdYoutube($(this).attr('data-iframe'));
        $('#video').html(embed);
    });
    $('#slider_video_home li').on('click', function () {
        var embed = getIdYoutube($(this).attr('data-iframe'));
        $('#video').html(embed);
    });
   
function addCommas(nStr)
{
    nStr += '';
    x = nStr.split('.');
    x1 = x[0];
    x2 = x.length > 1 ? '.' + x[1] : '';
    var rgx = /(\d+)(\d{3})/;
    while (rgx.test(x1)) {
        x1 = x1.replace(rgx, '$1' + '.' + '$2');
    }
    return x1 + x2;
}
$(document).ready(function(){
$('#selectUl li:not(":first")').addClass('unselected');
$('#selectUl').hover(
    function(){
        $(this).find('li').click(
            function(){
                $('.unselected').removeClass('unselected');
                $(this).siblings('li').addClass('unselected');
                var index = $(this).index();
                $('select[name=size]')
                    .find('option:eq(' + index + ')')
                    .attr('selected',true);
            });
    },
    function(){
    });
});
$(document).ready(function() {
var ch = $('body').find('#slider_home').length;
if(ch == 1){
$("#slider_home").owlCarousel({
    slideSpeed : 1000,
    paginationSpeed : 1000,
    singleItem:true,
    lazyLoad : true,
    autoPlay : 4000,
    transitionStyle : "goDown"

  }); 
}
});
                            $('.start').on('click',function(){
                                    $(this).prevAll().addClass('fa-star');
                                    $(this).prevAll().removeClass('fa-star-o');
                                    $(this).addClass('fa-star');
                                    $(this).removeClass('fa-star-o');
                                    $(this).nextAll().removeClass('fa-star');
                                    $(this).nextAll().addClass('fa-star-o');
                            });
                            $('.btn_coment').on('click',function(){
                                $('.box_rank').animate({
                                    opacity:'1',
                                    height:'135px'
                                },600);
                                $('.box_rank').css('display','block');
                                $('.btn_coment').css('display','none');
                                $('.end').css('display','block');
                            });
                            $('.end').on('click',function(){
                                $('.box_rank').animate({
                                    opacity:'0',
                                    height:'0'
                                },600,'swing',function(){
                                    $('.box_rank').css('display','none');
                                });
                                $('.btn_coment').css('display','block');
                                $('.end').css('display','none');
                            });
                            $(document).on('click','.rep_rank',function(){
                                var a = $(this).attr('data-id-coment');
                                $('.rep_'+a).animate({
                                    opacity:'1',
                                    height:'85px',
                                    margin: '10px 0'
                                },600);
                                $('.box_rep_ranking').css('display','block');
                            });
                            $(document).on('click','.close_rep_rank',function(){
                                var a = $(this).attr('id-box');
                                $('.rep_'+a).animate({
                                    opacity:'0',
                                    height:'0',
                                    margin: '0'
                                },600,'swing',function(){
                                    $('.rep_'+a).css('display','none');
                                });
                            });  
$('.btn_dow').on('click',function(){
    $('html, body').animate({scrollTop:550},800);
});
$('#btn_con').on('click',function(){
    var numberReg =  /^[0-9]+$/;
    var emailReg = /^([\w-\.]+@([\w-]+\.)+[\w-]{2,4})?$/;
    var nameReg = /^[A-Za-z]+$/;
    var name = $('#name_contac').val();
    var phone = $('#phone_contac').val();
    var email = $('#email_contac').val();
    var title = $('#title_contac').val();
    var detail = $('#detail_contac').val();
    if(name == ""){
        var myAlert = new cAlert("Họ tên không được để trống !", "danger", "", 5);
        myAlert.alert()
        $('#name_contac').focus();
        return false;
    }else if(email == ""){
        var myAlert = new cAlert('Email không được để trống !','danger','',5);
        myAlert.alert();
        $("#email_contac").focus();
        return false;
    }else if(!emailReg.test(email)){
        var myAlert = new cAlert('Email bạn nhập sai định dạng !','danger','',5);
        myAlert.alert();
        $("#email_contac").focus();
        return false;
    }else if(phone == ""){
        var myAlert = new cAlert("Số điện thoại không được để trống !", "danger", "", 5);
        myAlert.alert()
        $("#phone_contac").focus();
        return false;
    }else if(!numberReg.test(phone)){
        var myAlert = new cAlert('Số điện thoại bạn nhập sai định dạng !','danger','',5);
        myAlert.alert();
        $("#phone_contac").focus();
        return false;
    }else if(phone.length < 10){
        var myAlert = new cAlert('Số điện thoại ít nhất phải là 10 số !','danger','',5);
        myAlert.alert();
        $("#phone_contac").focus();
        return false;
    }else if(title == ""){
        var myAlert = new cAlert('Tiêu đề của liên hệ không được để trống !','danger','',5);
        myAlert.alert();
        $("#title_contac").focus();
        return false;
    }else if(detail == ""){
        var myAlert = new cAlert('Nội dung của liên hệ không được để trống !','danger','',5);
        myAlert.alert();
        $("#detail_contac").focus();
        return false;
    }
});
$(document).ready(function(){
var count1 = 0;
$(".btn_show p").on('click',function(){
   var h = $('.box').height();
    count1++;
    var isEven = function(someNumber) {
        return (someNumber % 2 === 0) ? true : false;
        };
        if (isEven(count1) === false) {
            $('.detail_pk').animate({
                height: h+30
            }, 1000);
            $(this).html('<i class="fa fa-long-arrow-up" aria-hidden="true"></i>Ẩn bớt chi tiết bài viết<i class="fa fa-long-arrow-up" aria-hidden="true"></i>');
        } else if (isEven(count1) === true) {
            $('.detail_pk').animate({
                height: "400"
            }, 1000);
            $(this).html('<i class="fa fa-long-arrow-down" aria-hidden="true"></i>Xem thêm chi tiết bài viết<i class="fa fa-long-arrow-down" aria-hidden="true"></i>');
        }
});
});
var loa = $('html,body').find('#slider_pro').length;
if(loa > 0){
 $("#slider_pro").lightSlider({
        gallery:true,
        item:1,
        loop:true,
        thumbItem:5,
        slideMargin:0,
        enableDrag: false,
        controls:true,
        autoHeight : true,
        speed:600,
        keyPress:true,
        freeMove:true,
        enableDrag:true,
        enableTouch:true,
        currentPagerPosition:'middle',
        onSliderLoad: function(el) {
            el.lightGallery({
                selector: '#slider_pro .lslide',
                download:false
            });
        }   
    });
    //fix box mua ngay
    var yah = $('.right_pro').outerHeight();
    var yahWidth = $('.right_pro').width();
    var yahoffset = $('#box_detail').offset();
    var startFix = yahoffset.top;
    $(window).scroll(function() {
        var yaBox = $('#box_detail').outerHeight();
        var endFix = startFix + yaBox - yah;
        var fromTop = $(this).scrollTop();
        if(fromTop > startFix) {
            if(fromTop > endFix) {
                var stopFix = yaBox - yah;
                $('.tomtat_pro').css({'position':'absolute','top':stopFix});
            }else {
                $('.tomtat_pro').css({'position':'fixed','top':'0px','width':yahWidth});
            }
        }else {
            $('.tomtat_pro').css({'position':'relative'});
        }
    });
}