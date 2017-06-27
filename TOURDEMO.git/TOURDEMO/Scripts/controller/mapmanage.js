$("#partner-services").select2();
$("#partner-tag").select2();

loadMarker(0, "", "");

function clickChangeService() {
    $('#test1').gmap3('destroy').remove();
    $("#menuMap").append('<div id="test1" class="gmap3"></div>');
    loadMarker($("#partner-services").val(), $("#partner-name").val(), $("#partner-tag").val());
    $('#panTo').empty();
}

function clickChangeTag() {
    $('#test1').gmap3('destroy').remove();
    $("#menuMap").append('<div id="test1" class="gmap3"></div>');
    loadMarker($("#partner-services").val(), $("#partner-name").val(), $("#partner-tag").val());
    $('#panTo').empty();
}

function keyChangeName() {
    $('#test1').gmap3('destroy').remove();
    $("#menuMap").append('<div id="test1" class="gmap3"></div>');
    loadMarker($("#partner-services").val(), $("#partner-name").val(), $("#partner-tag").val());
    $('#panTo').empty();
}

function loadMarker(idService, name, idTags) {
    $('#test1').gmap3({
        map: {
            options: {
                center: [10.8230989, 106.6296638],
                zoom: 12
            },
            callback: function (map) {
                $('#test1').gmap3({
                    map: {
                        onces: {
                            bounds_changed: function (map) {

                                $.getJSON('/MapManage/LoadMarker',
                                    {
                                        idService: idService,
                                        name: name,
                                        idTags: idTags
                                    },
                                    function (data) {
                                        $.each(data, function (key, val) {
                                            $('#test1').gmap3({
                                                marker: {
                                                    options: {
                                                        icon: '/Images/Icon/' + val.Icon + ''
                                                    },
                                                    latLng: [val.xMap, val.yMap],
                                                    callback: function (marker) {
                                                        var infowindow = new google.maps.InfoWindow();
                                                        var $button = $('<p id="button-' + val.Id + '"><img style="width: 20px" src="/Images/Icon/' + val.Icon + '" />&nbsp;' + val.Name + ' </p>');
                                                        $button
                                                          .click(function () {
                                                              $('#test1').gmap3("get").panTo(marker.position);
                                                              marker.setAnimation(google.maps.Animation.BOUNCE);
                                                              setTimeout(function () { marker.setAnimation(google.maps.Animation.STOP) }, 2000);
                                                              infowindow.setContent("<div id='boxinfo'><h5 class='title' style='text-align: center'>" + val.Name + "</h5><p style='margin: 0 !important; text-align: center'>" + val.AddressMap + "</p></div>");
                                                              infowindow.open(map, marker);
                                                              setTimeout(function () { infowindow.close(); }, 4000);
                                                          })
                                                          .css('cursor', 'pointer');
                                                        $('#panTo').append($button);
                                                    },
                                                    events: {
                                                        click: function (marker, event, context) {
                                                            var infowindow = new google.maps.InfoWindow();
                                                            infowindow.setContent("<div id='boxinfo'><h5 class='title' style='text-align: center'>" + val.Name + "</h5><p style='margin: 0 !important; text-align: center'>" + val.AddressMap + "</p></div>");
                                                            infowindow.open(map, marker);
                                                            setTimeout(function () { infowindow.close(); }, 4000);
                                                        }
                                                    }
                                                },
                                            });
                                        });
                                    });
                            }
                        }
                    }
                });
            }
        }
    });
};