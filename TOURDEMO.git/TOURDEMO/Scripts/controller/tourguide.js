$("#edit-phongban").select2();

$('.dataTable').DataTable({
    order: [],
    columnDefs: [{ orderable: false, targets: [0] }]
});

$(".dataTable").dataTable().columnFilter({
    sPlaceHolder: "head:after",
    aoColumns: [null,
                { type: "text" },
                null,
                null]
});

$("#type-tour").select2();
$("#guide-tour").select2();

$("#type-tour").change(function () {
    var dataPost = {
        id: $("#type-tour").val(),
        start: $("#start-date").val(),
        end: $("#end-date").val(),
        guide: $("#guide-tour").val()
    };

    $.ajax({
        type: "POST",
        url: '/TourGuideSchedule/JsonCalendar',
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
        url: '/TourGuideSchedule/TourGuideFilter',
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
                           null]
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
        guide: $("#guide-tour").val()
    };

    $.ajax({
        type: "POST",
        url: '/TourGuideSchedule/JsonCalendar',
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
        url: '/TourGuideSchedule/TourGuideFilter',
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
                           null]
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
        guide: $("#guide-tour").val()
    };
    $.ajax({
        type: "POST",
        url: '/TourGuideSchedule/JsonCalendar',
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
        url: '/TourGuideSchedule/TourGuideFilter',
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
                           null]
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
                    url: '/TourGuideSchedule/EditTourGuide',
                    data: JSON.stringify(dataPost),
                    contentType: "application/json; charset=utf-8",
                    dataType: "html",
                    success: function (data) {
                        $("#info-data-tourguide").html(data);
                        $("#edit-staff").select2();
                        $("#edit-phongban").change(function () {
                            $.getJSON('/TourGuideSchedule/FilterDepartment/' + $('#edit-phongban').val(), function (data) {
                                var items = "";
                                $.each(data, function (i, ward) {
                                    items += "<option value='" + ward.Value + "'>" + ward.Text + "</option>";
                                });
                                $('#edit-staff').html(items);
                                
                            });
                        })
                        $("#edit-phongban").select2();
                        $("#modal-edit-tourguide").modal("show");
                    }
                });//end edit
            });
        },
        events: '/TourGuideSchedule/JsonCalendarDefault',
    });
});

$("#tabdanglich").click(function () {
    $("#tabThongTinChiTiet").hide();
})

$("#tabdangluoi").click(function () {
    $("#tabThongTinChiTiet").show();
})

function OnFailureTourGuide() {
    alert("Lỗi!");
    $("#modal-edit-tourguide").modal("hide");
    $("#type-tour").change();
    $('form').trigger("reset");
    CKupdate();
}

function OnSuccessTourGuide() {
    alert("Đã lưu!");
    $("#modal-edit-tourguide").modal("hide");
    $("#type-tour").change();
    $('form').trigger("reset");
    CKupdate();
}