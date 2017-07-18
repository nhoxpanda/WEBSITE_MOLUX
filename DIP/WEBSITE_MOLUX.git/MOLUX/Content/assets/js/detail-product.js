(function () {
    // store the slider in a local variable
    var $window = $(window),
        flexslider = { vars: {} };

    // tiny helper function to add breakpoints
    function getGridSize() {
        return (window.innerWidth < 600) ? 2 :
               (window.innerWidth < 900) ? 3 : 5;
    }

    $(function () {
        SyntaxHighlighter.all();
    });

    $window.load(function () {
        $('#flexslider1').flexslider({
            animation: "slide",
            animationLoop: true,
            itemWidth: 208,
            //itemMargin: 5,
            minItems: getGridSize(), // use function to pull in initial value
            maxItems: getGridSize() // use function to pull in initial value
        });

        $('#flexslider2').flexslider({
            animation: "slide",
            animationLoop: true,
            itemWidth: 208,
            //itemMargin: 5,
            minItems: getGridSize(), // use function to pull in initial value
            maxItems: getGridSize() // use function to pull in initial value
        });
    });

    // check grid size on resize event
    $window.resize(function () {
        var gridSize = getGridSize();

        flexslider.vars.minItems = gridSize;
        flexslider.vars.maxItems = gridSize;
    });
}());

$('#bxsliderProduct').bxSlider({
    mode: 'vertical',
    slideMargin: 5,
    minSlides: 3,
    maxSlides: 3,
    auto: true,
});

(function ($) {
    $(document).ready(function () {
        $('.xzoom, .xzoom-gallery').xzoom({ tint: '#FFDD00', Xoffset: 5 });
        //Integration with hammer.js
        var isTouchSupported = 'ontouchstart' in window;
        if (isTouchSupported) {
            //If touch device
            $('.xzoom').each(function () {
                var xzoom = $(this).data('xzoom');
                xzoom.eventunbind();
            });
            $('.xzoom').each(function () {
                var xzoom = $(this).data('xzoom');
                $(this).hammer().on("tap", function (event) {
                    event.pageX = event.gesture.center.pageX;
                    event.pageY = event.gesture.center.pageY;
                    var s = 1, ls;

                    xzoom.eventmove = function (element) {
                        element.hammer().on('drag', function (event) {
                            event.pageX = event.gesture.center.pageX;
                            event.pageY = event.gesture.center.pageY;
                            xzoom.movezoom(event);
                            event.gesture.preventDefault();
                        });
                    }
                    var counter = 0;
                    xzoom.eventclick = function (element) {
                        element.hammer().on('tap', function () {
                            counter++;
                            if (counter == 1) setTimeout(openmagnific, 300);
                            event.gesture.preventDefault();
                        });
                    }
                    function openmagnific() {
                        if (counter == 2) {
                            xzoom.closezoom();
                            var gallery = xzoom.gallery().cgallery;
                            var i, images = new Array();
                            for (i in gallery) {
                                images[i] = { src: gallery[i] };
                            }
                            $.magnificPopup.open({ items: images, type: 'image', gallery: { enabled: true } });
                        } else {
                            xzoom.closezoom();
                        }
                        counter = 0;
                    }
                    xzoom.openzoom(event);
                });
            });
        } else {
            //Integration with magnific popup plugin
            $('#xzoom-magnific').bind('click', function (event) {
                var xzoom = $(this).data('xzoom');
                xzoom.closezoom();
                var gallery = xzoom.gallery().cgallery;
                var i, images = new Array();
                for (i in gallery) {
                    images[i] = { src: gallery[i] };
                }
                $.magnificPopup.open({ items: images, type: 'image', gallery: { enabled: true } });
                event.preventDefault();
            });
        }
    });
})(jQuery);