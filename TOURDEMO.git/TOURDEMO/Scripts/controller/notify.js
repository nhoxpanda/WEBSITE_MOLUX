$(document).ready(function () {
    var hubProxy = $.connection.clockHub;

    hubProxy.client.setTime = function (time) {
        var dataPost = { time: time };
        $.ajax({
            type: "POST",
            url: '/AppointmentManage/Notification',
            data: JSON.stringify(dataPost),
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            success: function (data) {
                if (data != 0) {
                    $("#notification").html(data);
                    // Let's check if the browser supports notifications
                    if (!("Notification" in window)) {
                        alert("This browser does not support desktop notification");
                    }

                        // Let's check whether notification permissions have already been granted
                    else if (Notification.permission === "granted") {
                        // If it's okay let's create a notification
                        var options = {
                            body: $('#body-notification').text(),
                            icon: 'http://TOURDEMO.dip.vn/Images/logo.gif'
                        }
                        var notification = new Notification($("#title-notification").text(), options);
                    }

                        // Otherwise, we need to ask the user for permission
                    else if (Notification.permission !== 'denied') {
                        Notification.requestPermission(function (permission) {
                            // If the user accepts, let's create a notification
                            if (permission === "granted") {
                                var options = {
                                    body: $('#body-notification').text(),
                                    icon: 'http://TOURDEMO.dip.vn/Images/logo.gif'
                                }
                                var notification = new Notification($("#title-notification").text(), options);
                            }
                        });
                    }
                }
            }
        });
    };

    $.connection.hub.start().done(function () {
        setInterval(function () {
            hubProxy.server.getTime();
        }, 30000);
    });
});

//////////////////

$(document).ready(function () {
    var hubProxy = $.connection.clockHub;

    hubProxy.client.setTime = function (time) {
        var dataPost = { time: time };
        $.ajax({
            type: "POST",
            url: '/TaskManage/Notification',
            data: JSON.stringify(dataPost),
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            success: function (data) {
                if (data != 0) {
                    $("#task").html(data);
                    // Let's check if the browser supports notifications
                    if (!("Notification" in window)) {
                        alert("This browser does not support desktop notification");
                    }

                        // Let's check whether notification permissions have already been granted
                    else if (Notification.permission === "granted") {
                        // If it's okay let's create a notification
                        var options = {
                            body: $('#body-task').text(),
                            icon: 'http://TOURDEMO.dip.vn/Images/logo.gif'
                        }
                        var notification = new Notification($("#title-notification").text(), options);
                    }

                        // Otherwise, we need to ask the user for permission
                    else if (Notification.permission !== 'denied') {
                        Notification.requestPermission(function (permission) {
                            // If the user accepts, let's create a notification
                            if (permission === "granted") {
                                var options = {
                                    body: $('#body-task').text(),
                                    icon: 'http://TOURDEMO.dip.vn/Images/logo.gif'
                                }
                                var notification = new Notification($("#title-task").text(), options);
                            }
                        });
                    }
                }
            }
        });
    };

    $.connection.hub.start().done(function () {
        setInterval(function () {
            hubProxy.server.getTime();
        }, 30000);
    });
});