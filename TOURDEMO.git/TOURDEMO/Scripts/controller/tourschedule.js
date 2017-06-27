$('.dataTable').DataTable({
    order: [],
    columnDefs: [{ orderable: false, targets: [0] }]
});

$(".dataTable").dataTable().columnFilter({
    sPlaceHolder: "head:after",
    aoColumns: [null,
                { type: "text" },
                null,
                null,
                { type: "text" },
                { type: "text" }]
});

$("#type-tour").select2();
$("#leader-tour").select2();
$("#guide-tour").select2();

$("#type-tour").change(function () {
    var dataPost = {
        id: $("#type-tour").val(),
        start: $("#start-date").val(),
        end: $("#end-date").val(),
        leader: $("#leader-tour").val(),
        guide: $("#guide-tour").val()
    };
    $.ajax({
        type: "POST",
        url: '/TourSchedule/JsonCalendar',
        data: JSON.stringify(dataPost),
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (data) {
            var jdata = jQuery.parseJSON(data);
            $('#calendar').fullCalendar('removeEvents');
            $('#calendar').fullCalendar('addEventSource', jdata);
        }
    })
    $.ajax({
        type: "POST",
        url: '/TourSchedule/TourScheduleFilter',
        data: JSON.stringify(dataPost),
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (data) {
            $("#dangluoi").html(data);
            $('.dataTable').DataTable({
                order: [],
                columnDefs: [{ orderable: false, targets: [0] }]
            });
            $(".dataTable").dataTable().columnFilter({
                sPlaceHolder: "head:after",
                aoColumns: [null,
                            { type: "text" },
                            null,
                           null,
                           { type: "text" },
                { type: "text" }]
            });
            // kéo dài kích thước cột
            colResize();
        }
    })
});

$("#leader-tour").change(function () {
    var dataPost = {
        id: $("#type-tour").val(),
        start: $("#start-date").val(),
        end: $("#end-date").val(),
        leader: $("#leader-tour").val(),
        guide: $("#guide-tour").val()
    };
    $.ajax({
        type: "POST",
        url: '/TourSchedule/JsonCalendar',
        data: JSON.stringify(dataPost),
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (data) {
            var jdata = jQuery.parseJSON(data);
            $('#calendar').fullCalendar('removeEvents');
            $('#calendar').fullCalendar('addEventSource', jdata);
        }
    })
    $.ajax({
        type: "POST",
        url: '/TourSchedule/TourScheduleFilter',
        data: JSON.stringify(dataPost),
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (data) {
            $("#dangluoi").html(data);
            $('.dataTable').DataTable({
                order: [],
                columnDefs: [{ orderable: false, targets: [0] }]
            });
            $(".dataTable").dataTable().columnFilter({
                sPlaceHolder: "head:after",
                aoColumns: [null,
                            { type: "text" },
                            null,
                           null,
                           { type: "text" },
                { type: "text" }]
            });
            // kéo dài kích thước cột
            colResize();
        }
    })
});

$("#guide-tour").change(function () {
    var dataPost = {
        id: $("#type-tour").val(),
        start: $("#start-date").val(),
        end: $("#end-date").val(),
        leader: $("#leader-tour").val(),
        guide: $("#guide-tour").val()
    };
    $.ajax({
        type: "POST",
        url: '/TourSchedule/JsonCalendar',
        data: JSON.stringify(dataPost),
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (data) {
            var jdata = jQuery.parseJSON(data);
            $('#calendar').fullCalendar('removeEvents');
            $('#calendar').fullCalendar('addEventSource', jdata);
        }
    })
    $.ajax({
        type: "POST",
        url: '/TourSchedule/TourScheduleFilter',
        data: JSON.stringify(dataPost),
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (data) {
            $("#dangluoi").html(data);

            $('.dataTable').DataTable({
                order: [],
                columnDefs: [{ orderable: false, targets: [0] }]
            });

            $(".dataTable").dataTable().columnFilter({
                sPlaceHolder: "head:after",
                aoColumns: [null,
                            { type: "text" },
                            null,
                           null,
                           { type: "text" },
                { type: "text" }]
            });
            // kéo dài kích thước cột
            colResize();
        }
    })
});

$("#end-date").change(function () {
    var dataPost = {
        id: $("#type-tour").val(),
        start: $("#start-date").val(),
        end: $("#end-date").val(),
        leader: $("#leader-tour").val(),
        guide: $("#guide-tour").val()
    };
    $.ajax({
        type: "POST",
        url: '/TourSchedule/JsonCalendar',
        data: JSON.stringify(dataPost),
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (data) {
            var jdata = jQuery.parseJSON(data);
            $('#calendar').fullCalendar('removeEvents');
            $('#calendar').fullCalendar('addEventSource', jdata);
        }
    })
    $.ajax({
        type: "POST",
        url: '/TourSchedule/TourScheduleFilter',
        data: JSON.stringify(dataPost),
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (data) {
            $("#dangluoi").html(data);

            $('.dataTable').DataTable({
                order: [],
                columnDefs: [{ orderable: false, targets: [0] }]
            });

            $(".dataTable").dataTable().columnFilter({
                sPlaceHolder: "head:after",
                aoColumns: [null,
                            { type: "text" },
                            null,
                           null,
                           { type: "text" },
                { type: "text" }]
            });
            // kéo dài kích thước cột
            colResize();
        }
    })
});

$(document).ready(function () {
    $('#calendar').fullCalendar({
        header: {
            left: 'prev,next today',
            center: 'title',
            right: 'month,agendaWeek,agendaDay'
        },
        lang: 'vi',
        defaultDate: new Date(),
        businessHours: false, // display business hours
        editable: true,
        defaultView: 'agendaWeek',
        dayClick: function (date, jsEvent, view) {
            $(".fc-highlight").removeClass("fc-highlight")
            $(jsEvent.toElement).addClass("fc-highlight")
        },
        eventRender: function (event, element) {
            element.bind('dblclick', function () {
                //edit
                var dataPost = {
                    id: event.id
                };
                $.ajax({
                    type: "POST",
                    url: '/TourSchedule/EditScheduleTour',
                    data: JSON.stringify(dataPost),
                    contentType: "application/json; charset=utf-8",
                    dataType: "html",
                    success: function (data) {
                        $("#info-data-tourschedule").html(data);
                        $("#modal-edit-tourschedule").modal("show");
                    }
                });//end edit
            });
        },
        events: '/TourSchedule/JsonCalendarDefault',
    });
});

$("#tabdanglich").click(function () {
    $("#tabThongTinChiTiet").hide();
})

$("#tabdangluoi").click(function () {
    $("#tabThongTinChiTiet").show();
})

function OnFailureScheduleTour() {
    $('form').trigger("reset");
    CKupdate();
    alert("Lỗi!");
    $("#modal-edit-tourschedule").modal("hide");
    $("#type-tour").change();
}

function OnSuccessScheduleTour() {
    $('form').trigger("reset");
    CKupdate();
    alert("Đã lưu!");
    $("#modal-edit-tourschedule").modal("hide");
    $("#type-tour").change();
}