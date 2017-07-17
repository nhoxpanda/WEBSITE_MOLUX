$(document).ready(function () {
    $("table.data-demo").delegate("tr", "click", function () {
        var value = $(this).children('td:first').text();
        $("#valueID").text(value);
    });
    
    $("#btnFirstPrev").addClass("disabled");
    $("#btnPrev").addClass("disabled");
});


//function checkFirstLastPage(i) {
//    if (i == 1) {
//        $("#btnFirstPrev").addClass("disabled");
//        $("#btnPrev").addClass("disabled");
//        $("#btnLastNext").removeClass("disabled");
//        $("#btnNext").removeClass("disabled");
//    }
//    else if (i == 5) {
//        $("#btnFirstPrev").removeClass("disabled");
//        $("#btnPrev").removeClass("disabled");
//        $("#btnLastNext").addClass("disabled");
//        $("#btnNext").addClass("disabled");
//    }
//    else {
//        $("#btnFirstPrev").removeClass("disabled");
//        $("#btnPrev").removeClass("disabled");
//        $("#btnLastNext").removeClass("disabled");
//        $("#btnNext").removeClass("disabled");
//    }
//}

//function clickpageData(i) {
//    loadDataJSON("http://" + "api.ilinks.xyz/api/log/108/" + i + "/" + $("#sort-data option:selected").val());
//    $("ul.pagination li.active").removeClass("active");
//    $("#page" + i).addClass("active");
//    checkFirstLastPage(i);
//}

function clickbtnLuuSua() {
    $("#loading").fadeIn();
    setTimeout(function () { $("#loading").fadeOut(); }, 5000);
    $("#modal-edit-customer").modal('hide');
}

function clickbtnLuuThem() {
    $("#loading").fadeIn();
    setTimeout(function () { $("#loading").fadeOut(); }, 5000);
    $("#modal-insert-customer").modal('hide');
}

function clickSortASC(i) {
    var x = $("#sortasc" + i).parent().attr('id');
    //alert('ASC: ' + x);
    var uri;
    if ($(location).attr('href').indexOf('?') > 1) {
        uri = $(location).attr('href') + '&order=' + x + '-ASC';
    }
    else {
        uri = $(location).attr('href') + '?order=' + x + '-ASC';
    }
}

function clickSortDESC(i) {
    var x = $("#sortdesc" + i).parent().attr('id');
    //alert('DESC: ' + x);
    var uri;
    if ($(location).attr('href').indexOf('?') > 1) {
        uri = $(location).attr('href') + '&order=' + x + '-DESC';
    }
    else {
        uri = $(location).attr('href') + '?order=' + x + '-DESC';
    }
}

//$("#btnFirstPrev a").click(function () {
//    clickpageData(1);
//});

//$("#btnPrev a").click(function () {
//    var i = $("ul.pagination li.active").prev().find("a").text();
//    clickpageData(i);
//});

//$("#btnLastNext a").click(function () {
//    var i = $("ul.pagination li.active").last().find("a").text();
//    clickpageData(i);
//});

//$("#btnNext a").click(function () {
//    var i = $("ul.pagination li.active").next().find("a").text();
//    clickpageData(i);
//});
