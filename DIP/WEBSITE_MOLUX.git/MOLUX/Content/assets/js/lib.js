(function (t, e, i) {
    function n(i, n, o) {
        var r = e.createElement(i);
        return n && (r.id = Z + n),
        o && (r.style.cssText = o),
        t(r)
    }
    function o() {
        return i.innerHeight ? i.innerHeight : t(i).height()
    }
    function r(e, i) {
        i !== Object(i) && (i = {}),
        this.cache = {},
        this.el = e,
        this.value = function (e) {
            var n;
            return void 0 === this.cache[e] && (n = t(this.el).attr("data-cbox-" + e),
            void 0 !== n ? this.cache[e] = n : void 0 !== i[e] ? this.cache[e] = i[e] : void 0 !== X[e] && (this.cache[e] = X[e])),
            this.cache[e]
        }
        ,
        this.get = function (e) {
            var i = this.value(e);
            return t.isFunction(i) ? i.call(this.el, this) : i
        }
    }
    function h(t) {
        var e = W.length
          , i = (A + t) % e;
        return 0 > i ? e + i : i
    }
    function a(t, e) {
        return Math.round((/%/.test(t) ? ("x" === e ? E.width() : o()) / 100 : 1) * parseInt(t, 10))
    }
    function s(t, e) {
        return t.get("photo") || t.get("photoRegex").test(e)
    }
    function l(t, e) {
        return t.get("retinaUrl") && i.devicePixelRatio > 1 ? e.replace(t.get("photoRegex"), t.get("retinaSuffix")) : e
    }
    function d(t) {
        "contains" in x[0] && !x[0].contains(t.target) && t.target !== v[0] && (t.stopPropagation(),
        x.focus())
    }
    function c(t) {
        c.str !== t && (x.add(v).removeClass(c.str).addClass(t),
        c.str = t)
    }
    function g(e) {
        A = 0,
        e && e !== !1 && "nofollow" !== e ? (W = t("." + te).filter(function () {
            var i = t.data(this, Y)
              , n = new r(this, i);
            return n.get("rel") === e
        }),
        A = W.index(_.el),
        -1 === A && (W = W.add(_.el),
        A = W.length - 1)) : W = t(_.el)
    }
    function u(i) {
        t(e).trigger(i),
        ae.triggerHandler(i)
    }
    function f(i) {
        var o;
        if (!G) {
            if (o = t(i).data(Y),
            _ = new r(i, o),
            g(_.get("rel")),
            !U) {
                U = $ = !0,
                c(_.get("className")),
                x.css({
                    visibility: "hidden",
                    display: "block",
                    opacity: ""
                }),
                I = n(se, "LoadedContent", "width:0; height:0; overflow:hidden; visibility:hidden"),
                b.css({
                    width: "",
                    height: ""
                }).append(I),
                j = T.height() + k.height() + b.outerHeight(!0) - b.height(),
                D = C.width() + H.width() + b.outerWidth(!0) - b.width(),
                N = I.outerHeight(!0),
                z = I.outerWidth(!0);
                var h = a(_.get("initialWidth"), "x")
                  , s = a(_.get("initialHeight"), "y")
                  , l = _.get("maxWidth")
                  , f = _.get("maxHeight");
                _.w = Math.max((l !== !1 ? Math.min(h, a(l, "x")) : h) - z - D, 0),
                _.h = Math.max((f !== !1 ? Math.min(s, a(f, "y")) : s) - N - j, 0),
                I.css({
                    width: "",
                    height: _.h
                }),
                J.position(),
                u(ee),
                _.get("onOpen"),
                O.add(F).hide(),
                x.focus(),
                _.get("trapFocus") && e.addEventListener && (e.addEventListener("focus", d, !0),
                ae.one(re, function () {
                    e.removeEventListener("focus", d, !0)
                })),
                _.get("returnFocus") && ae.one(re, function () {
                    t(_.el).focus()
                })
            }
            var p = parseFloat(_.get("opacity"));
            v.css({
                opacity: p === p ? p : "",
                cursor: _.get("overlayClose") ? "pointer" : "",
                visibility: "visible"
            }).show(),
            _.get("closeButton") ? B.html(_.get("close")).appendTo(b) : B.appendTo("<div/>"),
            w()
        }
    }
    function p() {
        x || (V = !1,
        E = t(i),
        x = n(se).attr({
            id: Y,
            "class": t.support.opacity === !1 ? Z + "IE" : "",
            role: "dialog",
            tabindex: "-1"
        }).hide(),
        v = n(se, "Overlay").hide(),
        y = n(se, "Wrapper"),
        b = n(se, "Content").append(F = n(se, "Title"), P = t('').attr({
            id: Z + "Previous"
        }), K = t('').attr({
            id: Z + "Next"
        }), S = t('').attr({
            id: Z + "Slideshow"
        }), L),
        B = t('<button type="button"/>').attr({
            id: Z + "Close"
        }),
        y.append(n(se).append(n(se, "TopLeft"), T = n(se, "TopCenter"), n(se, "TopRight")), n(se, !1, "clear:left").append(C = n(se, "MiddleLeft"), b, H = n(se, "MiddleRight")), n(se, !1, "clear:left").append(n(se, "BottomLeft"), k = n(se, "BottomCenter"), n(se, "BottomRight"))).find("div div").css({
            "float": "left"
        }),
        M = n(se, !1, "position:absolute; width:9999px; visibility:hidden; display:none; max-width:none;"),
        O = K.add(P).add(R).add(S)),
        e.body && !x.parent().length && t(e.body).append(v, x.append(y, ''))
    }
    function m() {
        function i(t) {
            t.which > 1 || t.shiftKey || t.altKey || t.metaKey || t.ctrlKey || (t.preventDefault(),
            f(this))
        }
        return x ? (V || (V = !0,
        K.click(function () {
            J.next()
        }),
        P.click(function () {
            J.prev()
        }),
        B.click(function () {
            J.close()
        }),
        v.click(function () {
            _.get("overlayClose") && J.close()
        }),
        t(e).bind("keydown." + Z, function (t) {
            var e = t.keyCode;
            U && _.get("escKey") && 27 === e && (t.preventDefault(),
            J.close()),
            U && _.get("arrowKey") && W[1] && !t.altKey && (37 === e ? (t.preventDefault(),
            P.click()) : 39 === e && (t.preventDefault(),
            K.click()))
        }),
        t.isFunction(t.fn.on) ? t(e).on("click." + Z, "." + te, i) : t("." + te).live("click." + Z, i)),
        !0) : !1
    }
    function w() {
        var e, o, r, h = J.prep, d = ++le;
        if ($ = !0,
        q = !1,
        u(he),
        u(ie),
        _.get("onLoad"),
        _.h = _.get("height") ? a(_.get("height"), "y") - N - j : _.get("innerHeight") && a(_.get("innerHeight"), "y"),
        _.w = _.get("width") ? a(_.get("width"), "x") - z - D : _.get("innerWidth") && a(_.get("innerWidth"), "x"),
        _.mw = _.w,
        _.mh = _.h,
        _.get("maxWidth") && (_.mw = a(_.get("maxWidth"), "x") - z - D,
        _.mw = _.w && _.w < _.mw ? _.w : _.mw),
        _.get("maxHeight") && (_.mh = a(_.get("maxHeight"), "y") - N - j,
        _.mh = _.h && _.h < _.mh ? _.h : _.mh),
        e = _.get("href"),
        Q = setTimeout(function () {
            L.show()
        }, 100),
        _.get("inline")) {
            var c = t(e).eq(0);
            r = t("<div>").hide().insertBefore(c),
            ae.one(he, function () {
                r.replaceWith(c)
            }),
            h(c)
        } else
            _.get("iframe") ? h(" ") : _.get("html") ? h(_.get("html")) : s(_, e) ? (e = l(_, e),
            q = _.get("createImg"),
            t(q).addClass(Z + "Photo").bind("error." + Z, function () {
                h(n(se, "Error").html(_.get("imgError")))
            }).one("load", function () {
                d === le && setTimeout(function () {
                    var e;
                    _.get("retinaImage") && i.devicePixelRatio > 1 && (q.height = q.height / i.devicePixelRatio,
                    q.width = q.width / i.devicePixelRatio),
                    _.get("scalePhotos") && (o = function () {
                        q.height -= q.height * e,
                        q.width -= q.width * e
                    }
                    ,
                    _.mw && q.width > _.mw && (e = (q.width - _.mw) / q.width,
                    o()),
                    _.mh && q.height > _.mh && (e = (q.height - _.mh) / q.height,
                    o())),
                    _.h && (q.style.marginTop = Math.max(_.mh - q.height, 0) / 2 + "px"),
                    W[1] && (_.get("loop") || W[A + 1]) && (q.style.cursor = "pointer",
                    t(q).bind("click." + Z, function () {
                        J.next()
                    })),
                    q.style.width = q.width + "px",
                    q.style.height = q.height + "px",
                    h(q)
                }, 1)
            }),
            q.src = e) : e && M.load(e, _.get("data"), function (e, i) {
                d === le && h("error" === i ? n(se, "Error").html(_.get("xhrError")) : t(this).contents())
            })
    }
    var v, x, y, b, T, C, H, k, W, E, I, M, L, F, R, S, K, P, B, O, _, j, D, N, z, A, q, U, $, G, Q, J, V, X = {
        html: !1,
        photo: !1,
        iframe: !1,
        inline: !1,
        transition: "elastic",
        speed: 300,
        fadeOut: 300,
        width: !1,
        initialWidth: "600",
        innerWidth: !1,
        maxWidth: !1,
        height: !1,
        initialHeight: "450",
        innerHeight: !1,
        maxHeight: !1,
        scalePhotos: !0,
        scrolling: !0,
        opacity: .9,
        preloading: !0,
        className: !1,
        overlayClose: !0,
        escKey: !0,
        arrowKey: !0,
        top: !1,
        bottom: !1,
        left: !1,
        right: !1,
        fixed: !1,
        data: void 0,
        closeButton: !0,
        fastIframe: !0,
        open: !1,
        reposition: !0,
        loop: !0,
        slideshow: !1,
        slideshowAuto: !0,
        slideshowSpeed: 2500,
        slideshowStart: "start slideshow",
        slideshowStop: "stop slideshow",
        photoRegex: /\.(gif|png|jp(e|g|eg)|bmp|ico|webp|jxr|svg)((#|\?).*)?$/i,
        retinaImage: !1,
        retinaUrl: !1,
        retinaSuffix: "@2x.$1",
        current: "image {current} of {total}",
        previous: "previous",
        next: "next",
        close: "close",
        xhrError: "This content failed to load.",
        imgError: "This image failed to load.",
        returnFocus: !0,
        trapFocus: !0,
        onOpen: !1,
        onLoad: !1,
        onComplete: !1,
        onCleanup: !1,
        onClosed: !1,
        rel: function () {
            return this.rel
        },
        href: function () {
            return t(this).attr("href")
        },
        title: function () {
            return this.title
        },
        createImg: function () {
            var e = new Image
              , i = t(this).data("cbox-img-attrs");
            return "object" == typeof i && t.each(i, function (t, i) {
                e[t] = i
            }),
            e
        },
        createIframe: function () {
            var i = e.createElement("iframe")
              , n = t(this).data("cbox-iframe-attrs");
            return "object" == typeof n && t.each(n, function (t, e) {
                i[t] = e
            }),
            "frameBorder" in i && (i.frameBorder = 0),
            "allowTransparency" in i && (i.allowTransparency = "true"),
            i.name = (new Date).getTime(),
            i.allowFullscreen = !0,
            i
        }
    }, Y = "colorbox", Z = "cbox", te = Z + "Element", ee = Z + "_open", ie = Z + "_load", ne = Z + "_complete", oe = Z + "_cleanup", re = Z + "_closed", he = Z + "_purge", ae = t("<a/>"), se = "div", le = 0, de = {}, ce = function () {
        function t() {
            clearTimeout(h)
        }
        function e() {
            (_.get("loop") || W[A + 1]) && (t(),
            h = setTimeout(J.next, _.get("slideshowSpeed")))
        }
        function i() {
            S.html(_.get("slideshowStop")).unbind(s).one(s, n),
            ae.bind(ne, e).bind(ie, t),
            x.removeClass(a + "off").addClass(a + "on")
        }
        function n() {
            t(),
            ae.unbind(ne, e).unbind(ie, t),
            S.html(_.get("slideshowStart")).unbind(s).one(s, function () {
                J.next(),
                i()
            }),
            x.removeClass(a + "on").addClass(a + "off")
        }
        function o() {
            r = !1,
            S.hide(),
            t(),
            ae.unbind(ne, e).unbind(ie, t),
            x.removeClass(a + "off " + a + "on")
        }
        var r, h, a = Z + "Slideshow_", s = "click." + Z;
        return function () {
            r ? _.get("slideshow") || (ae.unbind(oe, o),
            o()) : _.get("slideshow") && W[1] && (r = !0,
            ae.one(oe, o),
            _.get("slideshowAuto") ? i() : n(),
            S.show())
        }
    }();
    t[Y] || (t(p),
    J = t.fn[Y] = t[Y] = function (e, i) {
        var n, o = this;
        return e = e || {},
        t.isFunction(o) && (o = t("<a/>"),
        e.open = !0),
        o[0] ? (p(),
        m() && (i && (e.onComplete = i),
        o.each(function () {
            var i = t.data(this, Y) || {};
            t.data(this, Y, t.extend(i, e))
        }).addClass(te),
        n = new r(o[0], e),
        n.get("open") && f(o[0])),
        o) : o
    }
    ,
    J.position = function (e, i) {
        function n() {
            T[0].style.width = k[0].style.width = b[0].style.width = parseInt(x[0].style.width, 10) - D + "px",
            b[0].style.height = C[0].style.height = H[0].style.height = parseInt(x[0].style.height, 10) - j + "px"
        }
        var r, h, s, l = 0, d = 0, c = x.offset();
        if (E.unbind("resize." + Z),
        x.css({
            top: -9e4,
            left: -9e4
        }),
        h = E.scrollTop(),
        s = E.scrollLeft(),
        _.get("fixed") ? (c.top -= h,
        c.left -= s,
        x.css({
            position: "fixed"
        })) : (l = h,
        d = s,
        x.css({
            position: "absolute"
        })),
        d += _.get("right") !== !1 ? Math.max(E.width() - _.w - z - D - a(_.get("right"), "x"), 0) : _.get("left") !== !1 ? a(_.get("left"), "x") : Math.round(Math.max(E.width() - _.w - z - D, 0) / 2),
        l += _.get("bottom") !== !1 ? Math.max(o() - _.h - N - j - a(_.get("bottom"), "y"), 0) : _.get("top") !== !1 ? a(_.get("top"), "y") : Math.round(Math.max(o() - _.h - N - j, 0) / 2),
        x.css({
            top: c.top,
            left: c.left,
            visibility: "visible"
        }),
        y[0].style.width = y[0].style.height = "9999px",
        r = {
            width: _.w + z + D,
            height: _.h + N + j,
            top: l,
            left: d
        },
        e) {
            var g = 0;
            t.each(r, function (t) {
                return r[t] !== de[t] ? (g = e,
                void 0) : void 0
            }),
            e = g
        }
        de = r,
        e || x.css(r),
        x.dequeue().animate(r, {
            duration: e || 0,
            complete: function () {
                n(),
                $ = !1,
                y[0].style.width = _.w + z + D + "px",
                y[0].style.height = _.h + N + j + "px",
                _.get("reposition") && setTimeout(function () {
                    E.bind("resize." + Z, J.position)
                }, 1),
                t.isFunction(i) && i()
            },
            step: n
        })
    }
    ,
    J.resize = function (t) {
        var e;
        U && (t = t || {},
        t.width && (_.w = a(t.width, "x") - z - D),
        t.innerWidth && (_.w = a(t.innerWidth, "x")),
        I.css({
            width: _.w
        }),
        t.height && (_.h = a(t.height, "y") - N - j),
        t.innerHeight && (_.h = a(t.innerHeight, "y")),
        t.innerHeight || t.height || (e = I.scrollTop(),
        I.css({
            height: "auto"
        }),
        _.h = I.height()),
        I.css({
            height: _.h
        }),
        e && I.scrollTop(e),
        J.position("none" === _.get("transition") ? 0 : _.get("speed")))
    }
    ,
    J.prep = function (i) {
        function o() {
            return _.w = _.w || I.width(),
            _.w = _.mw && _.mw < _.w ? _.mw : _.w,
            _.w
        }
        function a() {
            return _.h = _.h || I.height(),
            _.h = _.mh && _.mh < _.h ? _.mh : _.h,
            _.h
        }
        if (U) {
            var d, g = "none" === _.get("transition") ? 0 : _.get("speed");
            I.remove(),
            I = n(se, "LoadedContent").append(i),
            I.hide().appendTo(M.show()).css({
                width: o(),
                overflow: _.get("scrolling") ? "auto" : "hidden"
            }).css({
                height: a()
            }).prependTo(b),
            M.hide(),
            t(q).css({
                "float": "none"
            }),
            c(_.get("className")),
            d = function () {
                function i() {
                    t.support.opacity === !1 && x[0].style.removeAttribute("filter")
                }
                var n, o, a = W.length;
                U && (o = function () {
                    clearTimeout(Q),
                    L.hide(),
                    u(ne),
                    _.get("onComplete")
                }
                ,
                F.html(_.get("title")).show(),
                I.show(),
                a > 1 ? ("string" == typeof _.get("current") && R.html(_.get("current").replace("{current}", A + 1).replace("{total}", a)).show(),
                K[_.get("loop") || a - 1 > A ? "show" : "hide"]().html(_.get("next")),
                P[_.get("loop") || A ? "show" : "hide"]().html(_.get("previous")),
                ce(),
                _.get("preloading") && t.each([h(-1), h(1)], function () {
                    var i, n = W[this], o = new r(n, t.data(n, Y)), h = o.get("href");
                    h && s(o, h) && (h = l(o, h),
                    i = e.createElement("img"),
                    i.src = h)
                })) : O.hide(),
                _.get("iframe") ? (n = _.get("createIframe"),
                _.get("scrolling") || (n.scrolling = "no"),
                t(n).attr({
                    src: _.get("href"),
                    "class": Z + "Iframe"
                }).one("load", o).appendTo(I),
                ae.one(he, function () {
                    n.src = "//about:blank"
                }),
                _.get("fastIframe") && t(n).trigger("load")) : o(),
                "fade" === _.get("transition") ? x.fadeTo(g, 1, i) : i())
            }
            ,
            "fade" === _.get("transition") ? x.fadeTo(g, 0, function () {
                J.position(0, d)
            }) : J.position(g, d)
        }
    }
    ,
    J.next = function () {
        !$ && W[1] && (_.get("loop") || W[A + 1]) && (A = h(1),
        f(W[A]))
    }
    ,
    J.prev = function () {
        !$ && W[1] && (_.get("loop") || A) && (A = h(-1),
        f(W[A]))
    }
    ,
    J.close = function () {
        U && !G && (G = !0,
        U = !1,
        u(oe),
        _.get("onCleanup"),
        E.unbind("." + Z),
        v.fadeTo(_.get("fadeOut") || 0, 0),
        x.stop().fadeTo(_.get("fadeOut") || 0, 0, function () {
            x.hide(),
            v.hide(),
            u(he),
            I.remove(),
            setTimeout(function () {
                G = !1,
                u(re),
                _.get("onClosed")
            }, 1)
        }))
    }
    ,
    J.remove = function () {
        x && (x.stop(),
        t[Y].close(),
        x.stop(!1, !0).remove(),
        v.remove(),
        G = !1,
        x = null,
        t("." + te).removeData(Y).removeClass(te),
        t(e).unbind("click." + Z).unbind("keydown." + Z))
    }
    ,
    J.element = function () {
        return t(_.el)
    }
    ,
    J.settings = X)
})(jQuery, document, window);
jQuery.fn.flexymenu = function (i) {
    function e() {
        window.innerWidth <= 767 ? (d(),
        t(),
        1 == h && (u(),
        h = !1)) : (s(),
        n(),
        "horizontal" == l.type && "right" == l.align && 0 == h && (u(),
        h = !0))
    }
    function n() {
        navigator.userAgent.match(/Mobi/i) || window.navigator.msMaxTouchPoints > 0 ? (jQuery(o).find("a").on("click touchstart", function (i) {
            i.stopPropagation(),
            i.preventDefault(),
            window.location.href = jQuery(this).attr("href"),
            jQuery(this).parent("li").siblings("li").find("ul").stop(!0, !0).fadeOut(l.speed),
            "none" == jQuery(this).siblings("ul").css("display") ? jQuery(this).siblings("ul").stop(!0, !0).fadeIn(l.speed) : (jQuery(this).siblings("ul").stop(!0, !0).fadeOut(l.speed),
            jQuery(this).siblings("ul").find("ul").stop(!0, !0).fadeOut(l.speed))
        }),
        1 == l.hideClickOut && jQuery(document).bind("click.menu touchstart.menu", function (i) {
            0 == jQuery(i.target).closest(o).length && jQuery(o).find("ul").fadeOut(l.speed)
        })) : jQuery(o).find("li").bind("mouseenter", function () {
            jQuery(this).children("ul").stop(!0, !0).fadeIn(l.speed)
        }).bind("mouseleave", function () {
            jQuery(this).children("ul").stop(!0, !0).fadeOut(l.speed)
        })
    }
    function t() {
        jQuery(o).find("li:not(.showhide)").each(function () {
            jQuery(this).children("ul").length > 0 && jQuery(this).children("a").on("click", function () {
                "none" == jQuery(this).siblings("ul").css("display") ? jQuery(this).siblings("ul").slideDown(l.speed) : jQuery(this).siblings("ul").slideUp(l.speed)
            })
        })
    }
    function d() {
        jQuery(o).children("li:not(.showhide)").hide(0),
        jQuery(o).children("li.showhide").show(0).bind("click", function () {
            jQuery(o).children("li").is(":hidden") ? jQuery(o).children("li").slideDown(l.speed) : (jQuery(o).children("li:not(.showhide)").slideUp(l.speed),
            jQuery(o).children("li.showhide").show(0))
        })
    }
    function s() {
        jQuery(o).children("li").show(0),
        jQuery(o).children("li.showhide").hide(0)
    }
    function u() {
        jQuery(o).children("li").addClass("right");
        var i = jQuery(o).children("li");
        jQuery(o).children("li:not(.showhide)").detach();
        for (var e = i.length; e >= 1; e--)
            jQuery(o).append(i[e])
    }
    function r() {
        jQuery(o).find("li, a").unbind(),
        jQuery(document).unbind("click.menu touchstart.menu"),
        jQuery(o).find("ul").hide(0)
    }
    var l = {
        speed: 300,
        type: "horizontal",
        align: "left",
        indicator: !1,
        hideClickOut: !0
    };
    jQuery.extend(l, i);
    var h = !1
      , o = jQuery(this)
      , c = window.innerWidth;
    "vertical" == l.type && (jQuery(o).addClass("vertical"),
    "right" == l.align && jQuery(o).addClass("right")),
    1 == l.indicator && jQuery(o).find("li").each(function () {
        jQuery(this).children("ul").length > 0 && jQuery(this).append("<span class='indicator'>+</span>")
    }),
    jQuery(o).prepend("<li class='showhide'><span class='icon'><em></em><em></em><em></em></span></li>"),
    e(),
    jQuery(window).resize(function () {
        767 >= c && window.innerWidth > 767 && (r(),
        s(),
        n(),
        "horizontal" == l.type && "right" == l.align && 0 == h && (u(),
        h = !0)),
        c > 767 && window.innerWidth <= 767 && (r(),
        d(),
        t(),
        1 == h && (u(),
        h = !1)),
        c = window.innerWidth
    })
}
;
$(document).ready(function () {
    console.log(window.location.href);
    var e = window.location.href;
    "http://localhost:59360/" == e || "" == e || "http://molux.dip.vn/" == e ? $("#hidden").slideDown() : $("#hidden").slideUp(),
    $(".flexy-menu").flexymenu({
        speed: 400,
        type: "vertical",
        indicator: !1
    })
});
!function (e) {
    "use strict";
    function t() {
        E(!0)
    }
    var a = {};
    e.respond = a,
    a.update = function () { }
    ;
    var n = []
      , r = function () {
          var t = !1;
          try {
              t = new e.XMLHttpRequest
          } catch (a) {
              t = new e.ActiveXObject("Microsoft.XMLHTTP")
          }
          return function () {
              return t
          }
      }()
      , s = function (e, t) {
          var a = r();
          a && (a.open("GET", e, !0),
          a.onreadystatechange = function () {
              4 !== a.readyState || 200 !== a.status && 304 !== a.status || t(a.responseText)
          }
          ,
          4 !== a.readyState && a.send(null))
      }
      , i = function (e) {
          return e.replace(a.regex.minmaxwh, "").match(a.regex.other)
      };
    if (a.ajax = s,
    a.queue = n,
    a.unsupportedmq = i,
    a.regex = {
        media: /@media[^\{]+\{([^\{\}]*\{[^\}\{]*\})+/gi,
        keyframes: /@(?:\-(?:o|moz|webkit)\-)?keyframes[^\{]+\{(?:[^\{\}]*\{[^\}\{]*\})+[^\}]*\}/gi,
        comments: /\/\*[^*]*\*+([^\/][^*]*\*+)*\//gi,
        urls: /(url\()['"]?([^\/\)'"][^:\)'"]+)['"]?(\))/g,
        findStyles: /@media *([^\{]+)\{([\S\s]+?)$/,
        only: /(only\s+)?([a-zA-Z]+)\s?/,
        minw: /\(\s*min\-width\s*:\s*(\s*[0-9\.]+)(px|em)\s*\)/,
        maxw: /\(\s*max\-width\s*:\s*(\s*[0-9\.]+)(px|em)\s*\)/,
        minmaxwh: /\(\s*m(in|ax)\-(height|width)\s*:\s*(\s*[0-9\.]+)(px|em)\s*\)/gi,
        other: /\([^\)]*\)/g
    },
    a.mediaQueriesSupported = e.matchMedia && null !== e.matchMedia("only all") && e.matchMedia("only all").matches,
    !a.mediaQueriesSupported) {
        var o, l, m, h = e.document, d = h.documentElement, u = [], c = [], p = [], f = {}, g = 30, x = h.getElementsByTagName("head")[0] || d, y = h.getElementsByTagName("base")[0], v = x.getElementsByTagName("link"), w = function () {
            var e, t = h.createElement("div"), a = h.body, n = d.style.fontSize, r = a && a.style.fontSize, s = !1;
            return t.style.cssText = "position:absolute;font-size:1em;width:1em",
            a || (a = s = h.createElement("body"),
            a.style.background = "none"),
            d.style.fontSize = "100%",
            a.style.fontSize = "100%",
            a.appendChild(t),
            s && d.insertBefore(a, d.firstChild),
            e = t.offsetWidth,
            s ? d.removeChild(a) : a.removeChild(t),
            d.style.fontSize = n,
            r && (a.style.fontSize = r),
            e = m = parseFloat(e)
        }, E = function (t) {
            var a = "clientWidth"
              , n = d[a]
              , r = "CSS1Compat" === h.compatMode && n || h.body[a] || n
              , s = {}
              , i = v[v.length - 1]
              , f = (new Date).getTime();
            if (t && o && g > f - o)
                return e.clearTimeout(l),
                void (l = e.setTimeout(E, g));
            o = f;
            for (var y in u)
                if (u.hasOwnProperty(y)) {
                    var S = u[y]
                      , T = S.minw
                      , $ = S.maxw
                      , z = null === T
                      , b = null === $
                      , C = "em";
                    T && (T = parseFloat(T) * (T.indexOf(C) > -1 ? m || w() : 1)),
                    $ && ($ = parseFloat($) * ($.indexOf(C) > -1 ? m || w() : 1)),
                    S.hasquery && (z && b || !(z || r >= T) || !(b || $ >= r)) || (s[S.media] || (s[S.media] = []),
                    s[S.media].push(c[S.rules]))
                }
            for (var R in p)
                p.hasOwnProperty(R) && p[R] && p[R].parentNode === x && x.removeChild(p[R]);
            p.length = 0;
            for (var O in s)
                if (s.hasOwnProperty(O)) {
                    var M = h.createElement("style")
                      , k = s[O].join("\n");
                    M.type = "text/css",
                    M.media = O,
                    x.insertBefore(M, i.nextSibling),
                    M.styleSheet ? M.styleSheet.cssText = k : M.appendChild(h.createTextNode(k)),
                    p.push(M)
                }
        }, S = function (e, t, n) {
            var r = e.replace(a.regex.comments, "").replace(a.regex.keyframes, "").match(a.regex.media)
              , s = r && r.length || 0;
            t = t.substring(0, t.lastIndexOf("/"));
            var o = function (e) {
                return e.replace(a.regex.urls, "$1" + t + "$2$3")
            }
              , l = !s && n;
            t.length && (t += "/"),
            l && (s = 1);
            for (var m = 0; s > m; m++) {
                var h, d, p, f;
                l ? (h = n,
                c.push(o(e))) : (h = r[m].match(a.regex.findStyles) && RegExp.$1,
                c.push(RegExp.$2 && o(RegExp.$2))),
                p = h.split(","),
                f = p.length;
                for (var g = 0; f > g; g++)
                    d = p[g],
                    i(d) || u.push({
                        media: d.split("(")[0].match(a.regex.only) && RegExp.$2 || "all",
                        rules: c.length - 1,
                        hasquery: d.indexOf("(") > -1,
                        minw: d.match(a.regex.minw) && parseFloat(RegExp.$1) + (RegExp.$2 || ""),
                        maxw: d.match(a.regex.maxw) && parseFloat(RegExp.$1) + (RegExp.$2 || "")
                    })
            }
            E()
        }, T = function () {
            if (n.length) {
                var t = n.shift();
                s(t.href, function (a) {
                    S(a, t.href, t.media),
                    f[t.href] = !0,
                    e.setTimeout(function () {
                        T()
                    }, 0)
                })
            }
        }, $ = function () {
            for (var t = 0; t < v.length; t++) {
                var a = v[t]
                  , r = a.href
                  , s = a.media
                  , i = a.rel && "stylesheet" === a.rel.toLowerCase();
                r && i && !f[r] && (a.styleSheet && a.styleSheet.rawCssText ? (S(a.styleSheet.rawCssText, r, s),
                f[r] = !0) : (!/^([a-zA-Z:]*\/\/)/.test(r) && !y || r.replace(RegExp.$1, "").split("/")[0] === e.location.host) && ("//" === r.substring(0, 2) && (r = e.location.protocol + r),
                n.push({
                    href: r,
                    media: s
                })))
            }
            T()
        };
        $(),
        a.update = $,
        a.getEmValue = w,
        e.addEventListener ? e.addEventListener("resize", t, !1) : e.attachEvent && e.attachEvent("onresize", t)
    }
}(this);
function getXmlHttpRequestObject() {
    return window.XMLHttpRequest ? new XMLHttpRequest : window.ActiveXObject ? new ActiveXObject("Microsoft.XMLHTTP") : void alert("Your Browser Sucks!\nIt's about time to upgrade don't you think?")
}
function searchSuggest(e) {
    var t = window.event ? e.keyCode : e.which;
    if (40 == t || 38 == t)
        ;
    else if (4 == searchReq.readyState || 0 == searchReq.readyState) {
        var i = escape(document.getElementById("txtSearch").value);
        strOriginal = i,
        searchReq.open("GET", "/Result.aspx?search=" + i, !0),
        searchReq.onreadystatechange = handleSearchSuggest,
        searchReq.send(null)
    }
}
function handleSearchSuggest() {
    if (4 == searchReq.readyState) {
        var e = document.getElementById("search_suggest");
        e.innerHTML = "";
        var t = searchReq.responseText.split("~");
        if (t.length > 1)
            for (i = 0; i < t.length - 1; i++) {
                maxDivId = i,
                currentDivId = -1;
                var r = "<div ";
                r += "id=div" + i,
                r += "  ",
                r += 'class="suggest_link">' + t[i] + "</div>",
                e.innerHTML += r,
                e.style.visibility = "visible"
            }
        else
            e.style.visibility = "hidden"
    }
}
function suggestOver(e) {
    e.className = "suggest_link_over"
}
function searchRedirect() {
    window.location.href = "/ket-qua-tim-kiem?keyword="+document.getElementById("txtSearch").value+"&sort=0&page=1";
}
var maxDivId, currentDivId, strOriginal, urlroot = "/";
searchReq = getXmlHttpRequestObject();
"function" !== typeof Object.create && (Object.create = function (f) {
    function g() { }
    g.prototype = f;
    return new g
}
);
(function (f, g, k) {
    var l = {
        init: function (a, b) {
            this.$elem = f(b);
            this.options = f.extend({}, f.fn.owlCarousel.options, this.$elem.data(), a);
            this.userOptions = a;
            this.loadContent()
        },
        loadContent: function () {
            function a(a) {
                var d, e = "";
                if ("function" === typeof b.options.jsonSuccess)
                    b.options.jsonSuccess.apply(this, [a]);
                else {
                    for (d in a.owl)
                        a.owl.hasOwnProperty(d) && (e += a.owl[d].item);
                    b.$elem.html(e)
                }
                b.logIn()
            }
            var b = this, e;
            "function" === typeof b.options.beforeInit && b.options.beforeInit.apply(this, [b.$elem]);
            "string" === typeof b.options.jsonPath ? (e = b.options.jsonPath,
            f.getJSON(e, a)) : b.logIn()
        },
        logIn: function () {
            this.$elem.data("owl-originalStyles", this.$elem.attr("style"));
            this.$elem.data("owl-originalClasses", this.$elem.attr("class"));
            this.$elem.css({
                opacity: 0
            });
            this.orignalItems = this.options.items;
            this.checkBrowser();
            this.wrapperWidth = 0;
            this.checkVisible = null;
            this.setVars()
        },
        setVars: function () {
            if (0 === this.$elem.children().length)
                return !1;
            this.baseClass();
            this.eventTypes();
            this.$userItems = this.$elem.children();
            this.itemsAmount = this.$userItems.length;
            this.wrapItems();
            this.$owlItems = this.$elem.find(".owl-item");
            this.$owlWrapper = this.$elem.find(".owl-wrapper");
            this.playDirection = "►";
            this.prevItem = 0;
            this.prevArr = [0];
            this.currentItem = 0;
            this.customEvents();
            this.onStartup()
        },
        onStartup: function () {
            this.updateItems();
            this.calculateAll();
            this.buildControls();
            this.updateControls();
            this.response();
            this.moveEvents();
            this.stopOnHover();
            this.owlStatus();
            !1 !== this.options.transitionStyle && this.transitionTypes(this.options.transitionStyle);
            !0 === this.options.autoPlay && (this.options.autoPlay = 5E3);
            this.play();
            this.$elem.find(".owl-wrapper").css("display", "block");
            this.$elem.is(":visible") ? this.$elem.css("opacity", 1) : this.watchVisibility();
            this.onstartup = !1;
            this.eachMoveUpdate();
            "function" === typeof this.options.afterInit && this.options.afterInit.apply(this, [this.$elem])
        },
        eachMoveUpdate: function () {
            !0 === this.options.lazyLoad && this.lazyLoad();
            !0 === this.options.autoHeight && this.autoHeight();
            this.onVisibleItems();
            "function" === typeof this.options.afterAction && this.options.afterAction.apply(this, [this.$elem])
        },
        updateVars: function () {
            "function" === typeof this.options.beforeUpdate && this.options.beforeUpdate.apply(this, [this.$elem]);
            this.watchVisibility();
            this.updateItems();
            this.calculateAll();
            this.updatePosition();
            this.updateControls();
            this.eachMoveUpdate();
            "function" === typeof this.options.afterUpdate && this.options.afterUpdate.apply(this, [this.$elem])
        },
        reload: function () {
            var a = this;
            g.setTimeout(function () {
                a.updateVars()
            }, 0)
        },
        watchVisibility: function () {
            var a = this;
            if (!1 === a.$elem.is(":visible"))
                a.$elem.css({
                    opacity: 0
                }),
                g.clearInterval(a.autoPlayInterval),
                g.clearInterval(a.checkVisible);
            else
                return !1;
            a.checkVisible = g.setInterval(function () {
                a.$elem.is(":visible") && (a.reload(),
                a.$elem.animate({
                    opacity: 1
                }, 200),
                g.clearInterval(a.checkVisible))
            }, 500)
        },
        wrapItems: function () {
            this.$userItems.wrapAll('<div class="owl-wrapper">').wrap('<div class="owl-item"></div>');
            this.$elem.find(".owl-wrapper").wrap('<div class="owl-wrapper-outer">');
            this.wrapperOuter = this.$elem.find(".owl-wrapper-outer");
            this.$elem.css("display", "block")
        },
        baseClass: function () {
            var a = this.$elem.hasClass(this.options.baseClass)
              , b = this.$elem.hasClass(this.options.theme);
            a || this.$elem.addClass(this.options.baseClass);
            b || this.$elem.addClass(this.options.theme)
        },
        updateItems: function () {
            var a, b;
            if (!1 === this.options.responsive)
                return !1;
            if (!0 === this.options.singleItem)
                return this.options.items = this.orignalItems = 1,
                this.options.itemsCustom = !1,
                this.options.itemsDesktop = !1,
                this.options.itemsDesktopSmall = !1,
                this.options.itemsTablet = !1,
                this.options.itemsTabletSmall = !1,
                this.options.itemsMobile = !1;
            a = f(this.options.responsiveBaseWidth).width();
            a > (this.options.itemsDesktop[0] || this.orignalItems) && (this.options.items = this.orignalItems);
            if (!1 !== this.options.itemsCustom)
                for (this.options.itemsCustom.sort(function (a, b) {
                    return a[0] - b[0]
                }),
                b = 0; b < this.options.itemsCustom.length; b += 1)
                    this.options.itemsCustom[b][0] <= a && (this.options.items = this.options.itemsCustom[b][1]);
            else
                a <= this.options.itemsDesktop[0] && !1 !== this.options.itemsDesktop && (this.options.items = this.options.itemsDesktop[1]),
                a <= this.options.itemsDesktopSmall[0] && !1 !== this.options.itemsDesktopSmall && (this.options.items = this.options.itemsDesktopSmall[1]),
                a <= this.options.itemsTablet[0] && !1 !== this.options.itemsTablet && (this.options.items = this.options.itemsTablet[1]),
                a <= this.options.itemsTabletSmall[0] && !1 !== this.options.itemsTabletSmall && (this.options.items = this.options.itemsTabletSmall[1]),
                a <= this.options.itemsMobile[0] && !1 !== this.options.itemsMobile && (this.options.items = this.options.itemsMobile[1]);
            this.options.items > this.itemsAmount && !0 === this.options.itemsScaleUp && (this.options.items = this.itemsAmount)
        },
        response: function () {
            var a = this, b, e;
            if (!0 !== a.options.responsive)
                return !1;
            e = f(g).width();
            a.resizer = function () {
                f(g).width() !== e && (!1 !== a.options.autoPlay && g.clearInterval(a.autoPlayInterval),
                g.clearTimeout(b),
                b = g.setTimeout(function () {
                    e = f(g).width();
                    a.updateVars()
                }, a.options.responsiveRefreshRate))
            }
            ;
            f(g).resize(a.resizer)
        },
        updatePosition: function () {
            this.jumpTo(this.currentItem);
            !1 !== this.options.autoPlay && this.checkAp()
        },
        appendItemsSizes: function () {
            var a = this
              , b = 0
              , e = a.itemsAmount - a.options.items;
            a.$owlItems.each(function (c) {
                var d = f(this);
                d.css({
                    width: a.itemWidth
                }).data("owl-item", Number(c));
                if (0 === c % a.options.items || c === e)
                    c > e || (b += 1);
                d.data("owl-roundPages", b)
            })
        },
        appendWrapperSizes: function () {
            this.$owlWrapper.css({
                width: this.$owlItems.length * this.itemWidth * 2,
                left: 0
            });
            this.appendItemsSizes()
        },
        calculateAll: function () {
            this.calculateWidth();
            this.appendWrapperSizes();
            this.loops();
            this.max()
        },
        calculateWidth: function () {
            this.itemWidth = Math.round(this.$elem.width() / this.options.items)
        },
        max: function () {
            var a = -1 * (this.itemsAmount * this.itemWidth - this.options.items * this.itemWidth);
            this.options.items > this.itemsAmount ? this.maximumPixels = a = this.maximumItem = 0 : (this.maximumItem = this.itemsAmount - this.options.items,
            this.maximumPixels = a);
            return a
        },
        min: function () {
            return 0
        },
        loops: function () {
            var a = 0, b = 0, e, c;
            this.positionsInArray = [0];
            this.pagesInArray = [];
            for (e = 0; e < this.itemsAmount; e += 1)
                b += this.itemWidth,
                this.positionsInArray.push(-b),
                !0 === this.options.scrollPerPage && (c = f(this.$owlItems[e]),
                c = c.data("owl-roundPages"),
                c !== a && (this.pagesInArray[a] = this.positionsInArray[e],
                a = c))
        },
        buildControls: function () {
            if (!0 === this.options.navigation || !0 === this.options.pagination)
                this.owlControls = f('<div class="owl-controls"/>').toggleClass("clickable", !this.browser.isTouch).appendTo(this.$elem);
            !0 === this.options.pagination && this.buildPagination();
            !0 === this.options.navigation && this.buildButtons()
        },
        buildButtons: function () {
            var a = this
              , b = f('<div class="owl-buttons"/>');
            a.owlControls.append(b);
            a.buttonPrev = f("<div/>", {
                "class": "owl-prev",
                html: a.options.navigationText[0] || ""
            });
            a.buttonNext = f("<div/>", {
                "class": "owl-next",
                html: a.options.navigationText[1] || ""
            });
            b.append(a.buttonPrev).append(a.buttonNext);
            b.on("touchstart.owlControls mousedown.owlControls", 'div[class^="owl"]', function (a) {
                a.preventDefault()
            });
            b.on("touchend.owlControls mouseup.owlControls", 'div[class^="owl"]', function (b) {
                b.preventDefault();
                f(this).hasClass("owl-next") ? a.next() : a.prev()
            })
        },
        buildPagination: function () {
            var a = this;
            a.paginationWrapper = f('<div class="owl-pagination"/>');
            a.owlControls.append(a.paginationWrapper);
            a.paginationWrapper.on("touchend.owlControls mouseup.owlControls", ".owl-page", function (b) {
                b.preventDefault();
                Number(f(this).data("owl-page")) !== a.currentItem && a.goTo(Number(f(this).data("owl-page")), !0)
            })
        },
        updatePagination: function () {
            var a, b, e, c, d, g;
            if (!1 === this.options.pagination)
                return !1;
            this.paginationWrapper.html("");
            a = 0;
            b = this.itemsAmount - this.itemsAmount % this.options.items;
            for (c = 0; c < this.itemsAmount; c += 1)
                0 === c % this.options.items && (a += 1,
                b === c && (e = this.itemsAmount - this.options.items),
                d = f("<div/>", {
                    "class": "owl-page"
                }),
                g = f("<span></span>", {
                    text: !0 === this.options.paginationNumbers ? a : "",
                    "class": !0 === this.options.paginationNumbers ? "owl-numbers" : ""
                }),
                d.append(g),
                d.data("owl-page", b === c ? e : c),
                d.data("owl-roundPages", a),
                this.paginationWrapper.append(d));
            this.checkPagination()
        },
        checkPagination: function () {
            var a = this;
            if (!1 === a.options.pagination)
                return !1;
            a.paginationWrapper.find(".owl-page").each(function () {
                f(this).data("owl-roundPages") === f(a.$owlItems[a.currentItem]).data("owl-roundPages") && (a.paginationWrapper.find(".owl-page").removeClass("active"),
                f(this).addClass("active"))
            })
        },
        checkNavigation: function () {
            if (!1 === this.options.navigation)
                return !1;
            !1 === this.options.rewindNav && (0 === this.currentItem && 0 === this.maximumItem ? (this.buttonPrev.addClass("disabled"),
            this.buttonNext.addClass("disabled")) : 0 === this.currentItem && 0 !== this.maximumItem ? (this.buttonPrev.addClass("disabled"),
            this.buttonNext.removeClass("disabled")) : this.currentItem === this.maximumItem ? (this.buttonPrev.removeClass("disabled"),
            this.buttonNext.addClass("disabled")) : 0 !== this.currentItem && this.currentItem !== this.maximumItem && (this.buttonPrev.removeClass("disabled"),
            this.buttonNext.removeClass("disabled")))
        },
        updateControls: function () {
            this.updatePagination();
            this.checkNavigation();
            this.owlControls && (this.options.items >= this.itemsAmount ? this.owlControls.hide() : this.owlControls.show())
        },
        destroyControls: function () {
            this.owlControls && this.owlControls.remove()
        },
        next: function (a) {
            if (this.isTransition)
                return !1;
            this.currentItem += !0 === this.options.scrollPerPage ? this.options.items : 1;
            if (this.currentItem > this.maximumItem + (!0 === this.options.scrollPerPage ? this.options.items - 1 : 0))
                if (!0 === this.options.rewindNav)
                    this.currentItem = 0,
                    a = "rewind";
                else
                    return this.currentItem = this.maximumItem,
                    !1;
            this.goTo(this.currentItem, a)
        },
        prev: function (a) {
            if (this.isTransition)
                return !1;
            this.currentItem = !0 === this.options.scrollPerPage && 0 < this.currentItem && this.currentItem < this.options.items ? 0 : this.currentItem - (!0 === this.options.scrollPerPage ? this.options.items : 1);
            if (0 > this.currentItem)
                if (!0 === this.options.rewindNav)
                    this.currentItem = this.maximumItem,
                    a = "rewind";
                else
                    return this.currentItem = 0,
                    !1;
            this.goTo(this.currentItem, a)
        },
        goTo: function (a, b, e) {
            var c = this;
            if (c.isTransition)
                return !1;
            "function" === typeof c.options.beforeMove && c.options.beforeMove.apply(this, [c.$elem]);
            a >= c.maximumItem ? a = c.maximumItem : 0 >= a && (a = 0);
            c.currentItem = c.owl.currentItem = a;
            if (!1 !== c.options.transitionStyle && "drag" !== e && 1 === c.options.items && !0 === c.browser.support3d)
                return c.swapSpeed(0),
                !0 === c.browser.support3d ? c.transition3d(c.positionsInArray[a]) : c.css2slide(c.positionsInArray[a], 1),
                c.afterGo(),
                c.singleItemTransition(),
                !1;
            a = c.positionsInArray[a];
            !0 === c.browser.support3d ? (c.isCss3Finish = !1,
            !0 === b ? (c.swapSpeed("paginationSpeed"),
            g.setTimeout(function () {
                c.isCss3Finish = !0
            }, c.options.paginationSpeed)) : "rewind" === b ? (c.swapSpeed(c.options.rewindSpeed),
            g.setTimeout(function () {
                c.isCss3Finish = !0
            }, c.options.rewindSpeed)) : (c.swapSpeed("slideSpeed"),
            g.setTimeout(function () {
                c.isCss3Finish = !0
            }, c.options.slideSpeed)),
            c.transition3d(a)) : !0 === b ? c.css2slide(a, c.options.paginationSpeed) : "rewind" === b ? c.css2slide(a, c.options.rewindSpeed) : c.css2slide(a, c.options.slideSpeed);
            c.afterGo()
        },
        jumpTo: function (a) {
            "function" === typeof this.options.beforeMove && this.options.beforeMove.apply(this, [this.$elem]);
            a >= this.maximumItem || -1 === a ? a = this.maximumItem : 0 >= a && (a = 0);
            this.swapSpeed(0);
            !0 === this.browser.support3d ? this.transition3d(this.positionsInArray[a]) : this.css2slide(this.positionsInArray[a], 1);
            this.currentItem = this.owl.currentItem = a;
            this.afterGo()
        },
        afterGo: function () {
            this.prevArr.push(this.currentItem);
            this.prevItem = this.owl.prevItem = this.prevArr[this.prevArr.length - 2];
            this.prevArr.shift(0);
            this.prevItem !== this.currentItem && (this.checkPagination(),
            this.checkNavigation(),
            this.eachMoveUpdate(),
            !1 !== this.options.autoPlay && this.checkAp());
            "function" === typeof this.options.afterMove && this.prevItem !== this.currentItem && this.options.afterMove.apply(this, [this.$elem])
        },
        stop: function () {
            this.apStatus = "stop";
            g.clearInterval(this.autoPlayInterval)
        },
        checkAp: function () {
            "stop" !== this.apStatus && this.play()
        },
        play: function () {
            var a = this;
            a.apStatus = "play";
            if (!1 === a.options.autoPlay)
                return !1;
            g.clearInterval(a.autoPlayInterval);
            a.autoPlayInterval = g.setInterval(function () {
                a.next(!0)
            }, a.options.autoPlay)
        },
        swapSpeed: function (a) {
            "slideSpeed" === a ? this.$owlWrapper.css(this.addCssSpeed(this.options.slideSpeed)) : "paginationSpeed" === a ? this.$owlWrapper.css(this.addCssSpeed(this.options.paginationSpeed)) : "string" !== typeof a && this.$owlWrapper.css(this.addCssSpeed(a))
        },
        addCssSpeed: function (a) {
            return {
                "-webkit-transition": "all " + a + "ms ease",
                "-moz-transition": "all " + a + "ms ease",
                "-o-transition": "all " + a + "ms ease",
                transition: "all " + a + "ms ease"
            }
        },
        removeTransition: function () {
            return {
                "-webkit-transition": "",
                "-moz-transition": "",
                "-o-transition": "",
                transition: ""
            }
        },
        doTranslate: function (a) {
            return {
                "-webkit-transform": "translate3d(" + a + "px, 0px, 0px)",
                "-moz-transform": "translate3d(" + a + "px, 0px, 0px)",
                "-o-transform": "translate3d(" + a + "px, 0px, 0px)",
                "-ms-transform": "translate3d(" + a + "px, 0px, 0px)",
                transform: "translate3d(" + a + "px, 0px,0px)"
            }
        },
        transition3d: function (a) {
            this.$owlWrapper.css(this.doTranslate(a))
        },
        css2move: function (a) {
            this.$owlWrapper.css({
                left: a
            })
        },
        css2slide: function (a, b) {
            var e = this;
            e.isCssFinish = !1;
            e.$owlWrapper.stop(!0, !0).animate({
                left: a
            }, {
                duration: b || e.options.slideSpeed,
                complete: function () {
                    e.isCssFinish = !0
                }
            })
        },
        checkBrowser: function () {
            var a = k.createElement("div");
            a.style.cssText = "  -moz-transform:translate3d(0px, 0px, 0px); -ms-transform:translate3d(0px, 0px, 0px); -o-transform:translate3d(0px, 0px, 0px); -webkit-transform:translate3d(0px, 0px, 0px); transform:translate3d(0px, 0px, 0px)";
            a = a.style.cssText.match(/translate3d\(0px, 0px, 0px\)/g);
            this.browser = {
                support3d: null !== a && 1 === a.length,
                isTouch: "ontouchstart" in g || g.navigator.msMaxTouchPoints
            }
        },
        moveEvents: function () {
            if (!1 !== this.options.mouseDrag || !1 !== this.options.touchDrag)
                this.gestures(),
                this.disabledEvents()
        },
        eventTypes: function () {
            var a = ["s", "e", "x"];
            this.ev_types = {};
            !0 === this.options.mouseDrag && !0 === this.options.touchDrag ? a = ["touchstart.owl mousedown.owl", "touchmove.owl mousemove.owl", "touchend.owl touchcancel.owl mouseup.owl"] : !1 === this.options.mouseDrag && !0 === this.options.touchDrag ? a = ["touchstart.owl", "touchmove.owl", "touchend.owl touchcancel.owl"] : !0 === this.options.mouseDrag && !1 === this.options.touchDrag && (a = ["mousedown.owl", "mousemove.owl", "mouseup.owl"]);
            this.ev_types.start = a[0];
            this.ev_types.move = a[1];
            this.ev_types.end = a[2]
        },
        disabledEvents: function () {
            this.$elem.on("dragstart.owl", function (a) {
                a.preventDefault()
            });
            this.$elem.on("mousedown.disableTextSelect", function (a) {
                return f(a.target).is("input, textarea, select, option")
            })
        },
        gestures: function () {
            function a(a) {
                if (void 0 !== a.touches)
                    return {
                        x: a.touches[0].pageX,
                        y: a.touches[0].pageY
                    };
                if (void 0 === a.touches) {
                    if (void 0 !== a.pageX)
                        return {
                            x: a.pageX,
                            y: a.pageY
                        };
                    if (void 0 === a.pageX)
                        return {
                            x: a.clientX,
                            y: a.clientY
                        }
                }
            }
            function b(a) {
                "on" === a ? (f(k).on(d.ev_types.move, e),
                f(k).on(d.ev_types.end, c)) : "off" === a && (f(k).off(d.ev_types.move),
                f(k).off(d.ev_types.end))
            }
            function e(b) {
                b = b.originalEvent || b || g.event;
                d.newPosX = a(b).x - h.offsetX;
                d.newPosY = a(b).y - h.offsetY;
                d.newRelativeX = d.newPosX - h.relativePos;
                "function" === typeof d.options.startDragging && !0 !== h.dragging && 0 !== d.newRelativeX && (h.dragging = !0,
                d.options.startDragging.apply(d, [d.$elem]));
                (8 < d.newRelativeX || -8 > d.newRelativeX) && !0 === d.browser.isTouch && (void 0 !== b.preventDefault ? b.preventDefault() : b.returnValue = !1,
                h.sliding = !0);
                (10 < d.newPosY || -10 > d.newPosY) && !1 === h.sliding && f(k).off("touchmove.owl");
                d.newPosX = Math.max(Math.min(d.newPosX, d.newRelativeX / 5), d.maximumPixels + d.newRelativeX / 5);
                !0 === d.browser.support3d ? d.transition3d(d.newPosX) : d.css2move(d.newPosX)
            }
            function c(a) {
                a = a.originalEvent || a || g.event;
                var c;
                a.target = a.target || a.srcElement;
                h.dragging = !1;
                !0 !== d.browser.isTouch && d.$owlWrapper.removeClass("grabbing");
                d.dragDirection = 0 > d.newRelativeX ? d.owl.dragDirection = "left" : d.owl.dragDirection = "right";
                0 !== d.newRelativeX && (c = d.getNewPosition(),
                d.goTo(c, !1, "drag"),
                h.targetElement === a.target && !0 !== d.browser.isTouch && (f(a.target).on("click.disable", function (a) {
                    a.stopImmediatePropagation();
                    a.stopPropagation();
                    a.preventDefault();
                    f(a.target).off("click.disable")
                }),
                a = f._data(a.target, "events").click,
                c = a.pop(),
                a.splice(0, 0, c)));
                b("off")
            }
            var d = this
              , h = {
                  offsetX: 0,
                  offsetY: 0,
                  baseElWidth: 0,
                  relativePos: 0,
                  position: null,
                  minSwipe: null,
                  maxSwipe: null,
                  sliding: null,
                  dargging: null,
                  targetElement: null
              };
            d.isCssFinish = !0;
            d.$elem.on(d.ev_types.start, ".owl-wrapper", function (c) {
                c = c.originalEvent || c || g.event;
                var e;
                if (3 === c.which)
                    return !1;
                if (!(d.itemsAmount <= d.options.items)) {
                    if (!1 === d.isCssFinish && !d.options.dragBeforeAnimFinish || !1 === d.isCss3Finish && !d.options.dragBeforeAnimFinish)
                        return !1;
                    !1 !== d.options.autoPlay && g.clearInterval(d.autoPlayInterval);
                    !0 === d.browser.isTouch || d.$owlWrapper.hasClass("grabbing") || d.$owlWrapper.addClass("grabbing");
                    d.newPosX = 0;
                    d.newRelativeX = 0;
                    f(this).css(d.removeTransition());
                    e = f(this).position();
                    h.relativePos = e.left;
                    h.offsetX = a(c).x - e.left;
                    h.offsetY = a(c).y - e.top;
                    b("on");
                    h.sliding = !1;
                    h.targetElement = c.target || c.srcElement
                }
            })
        },
        getNewPosition: function () {
            var a = this.closestItem();
            a > this.maximumItem ? a = this.currentItem = this.maximumItem : 0 <= this.newPosX && (this.currentItem = a = 0);
            return a
        },
        closestItem: function () {
            var a = this
              , b = !0 === a.options.scrollPerPage ? a.pagesInArray : a.positionsInArray
              , e = a.newPosX
              , c = null;
            f.each(b, function (d, g) {
                e - a.itemWidth / 20 > b[d + 1] && e - a.itemWidth / 20 < g && "left" === a.moveDirection() ? (c = g,
                a.currentItem = !0 === a.options.scrollPerPage ? f.inArray(c, a.positionsInArray) : d) : e + a.itemWidth / 20 < g && e + a.itemWidth / 20 > (b[d + 1] || b[d] - a.itemWidth) && "right" === a.moveDirection() && (!0 === a.options.scrollPerPage ? (c = b[d + 1] || b[b.length - 1],
                a.currentItem = f.inArray(c, a.positionsInArray)) : (c = b[d + 1],
                a.currentItem = d + 1))
            });
            return a.currentItem
        },
        moveDirection: function () {
            var a;
            0 > this.newRelativeX ? (a = "right",
            this.playDirection = "next") : (a = "left",
            this.playDirection = "◄");
            return a
        },
        customEvents: function () {
            var a = this;
            a.$elem.on("owl.next", function () {
                a.next()
            });
            a.$elem.on("owl.prev", function () {
                a.prev()
            });
            a.$elem.on("owl.play", function (b, e) {
                a.options.autoPlay = e;
                a.play();
                a.hoverStatus = "play"
            });
            a.$elem.on("owl.stop", function () {
                a.stop();
                a.hoverStatus = "stop"
            });
            a.$elem.on("owl.goTo", function (b, e) {
                a.goTo(e)
            });
            a.$elem.on("owl.jumpTo", function (b, e) {
                a.jumpTo(e)
            })
        },
        stopOnHover: function () {
            var a = this;
            !0 === a.options.stopOnHover && !0 !== a.browser.isTouch && !1 !== a.options.autoPlay && (a.$elem.on("mouseover", function () {
                a.stop()
            }),
            a.$elem.on("mouseout", function () {
                "stop" !== a.hoverStatus && a.play()
            }))
        },
        lazyLoad: function () {
            var a, b, e, c, d;
            if (!1 === this.options.lazyLoad)
                return !1;
            for (a = 0; a < this.itemsAmount; a += 1)
                b = f(this.$owlItems[a]),
                "loaded" !== b.data("owl-loaded") && (e = b.data("owl-item"),
                c = b.find(".lazyOwl"),
                "string" !== typeof c.data("src") ? b.data("owl-loaded", "loaded") : (void 0 === b.data("owl-loaded") && (c.hide(),
                b.addClass("loading").data("owl-loaded", "checked")),
                (d = !0 === this.options.lazyFollow ? e >= this.currentItem : !0) && e < this.currentItem + this.options.items && c.length && this.lazyPreload(b, c)))
        },
        lazyPreload: function (a, b) {
            function e() {
                a.data("owl-loaded", "loaded").removeClass("loading");
                b.removeAttr("data-src");
                "fade" === d.options.lazyEffect ? b.fadeIn(400) : b.show();
                "function" === typeof d.options.afterLazyLoad && d.options.afterLazyLoad.apply(this, [d.$elem])
            }
            function c() {
                f += 1;
                d.completeImg(b.get(0)) || !0 === k ? e() : 100 >= f ? g.setTimeout(c, 100) : e()
            }
            var d = this, f = 0, k;
            "DIV" === b.prop("tagName") ? (b.css("background-image", "url(" + b.data("src") + ")"),
            k = !0) : b[0].src = b.data("src");
            c()
        },
        autoHeight: function () {
            function a() {
                var a = f(e.$owlItems[e.currentItem]).height();
                e.wrapperOuter.css("height", a + "px");
                e.wrapperOuter.hasClass("autoHeight") || g.setTimeout(function () {
                    e.wrapperOuter.addClass("autoHeight")
                }, 0)
            }
            function b() {
                d += 1;
                e.completeImg(c.get(0)) ? a() : 100 >= d ? g.setTimeout(b, 100) : e.wrapperOuter.css("height", "")
            }
            var e = this, c = f(e.$owlItems[e.currentItem]).find("img"), d;
            void 0 !== c.get(0) ? (d = 0,
            b()) : a()
        },
        completeImg: function (a) {
            return !a.complete || "undefined" !== typeof a.naturalWidth && 0 === a.naturalWidth ? !1 : !0
        },
        onVisibleItems: function () {
            var a;
            !0 === this.options.addClassActive && this.$owlItems.removeClass("active");
            this.visibleItems = [];
            for (a = this.currentItem; a < this.currentItem + this.options.items; a += 1)
                this.visibleItems.push(a),
                !0 === this.options.addClassActive && f(this.$owlItems[a]).addClass("active");
            this.owl.visibleItems = this.visibleItems
        },
        transitionTypes: function (a) {
            this.outClass = "owl-" + a + "-out";
            this.inClass = "owl-" + a + "-in"
        },
        singleItemTransition: function () {
            var a = this
              , b = a.outClass
              , e = a.inClass
              , c = a.$owlItems.eq(a.currentItem)
              , d = a.$owlItems.eq(a.prevItem)
              , f = Math.abs(a.positionsInArray[a.currentItem]) + a.positionsInArray[a.prevItem]
              , g = Math.abs(a.positionsInArray[a.currentItem]) + a.itemWidth / 2;
            a.isTransition = !0;
            a.$owlWrapper.addClass("owl-origin").css({
                "-webkit-transform-origin": g + "px",
                "-moz-perspective-origin": g + "px",
                "perspective-origin": g + "px"
            });
            d.css({
                position: "relative",
                left: f + "px"
            }).addClass(b).on("webkitAnimationEnd oAnimationEnd MSAnimationEnd animationend", function () {
                a.endPrev = !0;
                d.off("webkitAnimationEnd oAnimationEnd MSAnimationEnd animationend");
                a.clearTransStyle(d, b)
            });
            c.addClass(e).on("webkitAnimationEnd oAnimationEnd MSAnimationEnd animationend", function () {
                a.endCurrent = !0;
                c.off("webkitAnimationEnd oAnimationEnd MSAnimationEnd animationend");
                a.clearTransStyle(c, e)
            })
        },
        clearTransStyle: function (a, b) {
            a.css({
                position: "",
                left: ""
            }).removeClass(b);
            this.endPrev && this.endCurrent && (this.$owlWrapper.removeClass("owl-origin"),
            this.isTransition = this.endCurrent = this.endPrev = !1)
        },
        owlStatus: function () {
            this.owl = {
                userOptions: this.userOptions,
                baseElement: this.$elem,
                userItems: this.$userItems,
                owlItems: this.$owlItems,
                currentItem: this.currentItem,
                prevItem: this.prevItem,
                visibleItems: this.visibleItems,
                isTouch: this.browser.isTouch,
                browser: this.browser,
                dragDirection: this.dragDirection
            }
        },
        clearEvents: function () {
            this.$elem.off(".owl owl mousedown.disableTextSelect");
            f(k).off(".owl owl");
            f(g).off("resize", this.resizer)
        },
        unWrap: function () {
            0 !== this.$elem.children().length && (this.$owlWrapper.unwrap(),
            this.$userItems.unwrap().unwrap(),
            this.owlControls && this.owlControls.remove());
            this.clearEvents();
            this.$elem.attr("style", this.$elem.data("owl-originalStyles") || "").attr("class", this.$elem.data("owl-originalClasses"))
        },
        destroy: function () {
            this.stop();
            g.clearInterval(this.checkVisible);
            this.unWrap();
            this.$elem.removeData()
        },
        reinit: function (a) {
            a = f.extend({}, this.userOptions, a);
            this.unWrap();
            this.init(a, this.$elem)
        },
        addItem: function (a, b) {
            var e;
            if (!a)
                return !1;
            if (0 === this.$elem.children().length)
                return this.$elem.append(a),
                this.setVars(),
                !1;
            this.unWrap();
            e = void 0 === b || -1 === b ? -1 : b;
            e >= this.$userItems.length || -1 === e ? this.$userItems.eq(-1).after(a) : this.$userItems.eq(e).before(a);
            this.setVars()
        },
        removeItem: function (a) {
            if (0 === this.$elem.children().length)
                return !1;
            a = void 0 === a || -1 === a ? -1 : a;
            this.unWrap();
            this.$userItems.eq(a).remove();
            this.setVars()
        }
    };
    f.fn.owlCarousel = function (a) {
        return this.each(function () {
            if (!0 === f(this).data("owl-init"))
                return !1;
            f(this).data("owl-init", !0);
            var b = Object.create(l);
            b.init(a, this);
            f.data(this, "owlCarousel", b)
        })
    }
    ;
    f.fn.owlCarousel.options = {
        items: 5,
        itemsCustom: !1,
        itemsDesktop: [1199, 4],
        itemsDesktopSmall: [979, 3],
        itemsTablet: [768, 2],
        itemsTabletSmall: !1,
        itemsMobile: [479, 1],
        singleItem: !1,
        itemsScaleUp: !1,
        slideSpeed: 200,
        paginationSpeed: 800,
        rewindSpeed: 1E3,
        autoPlay: !1,
        stopOnHover: !1,
        navigation: !1,
        navigationText: ["◄", "►"],
        rewindNav: !0,
        scrollPerPage: !1,
        pagination: !0,
        paginationNumbers: !1,
        responsive: !0,
        responsiveRefreshRate: 200,
        responsiveBaseWidth: g,
        baseClass: "owl-carousel",
        theme: "owl-theme",
        lazyLoad: !1,
        lazyFollow: !0,
        lazyEffect: "fade",
        autoHeight: !1,
        jsonPath: !1,
        jsonSuccess: !1,
        dragBeforeAnimFinish: !0,
        mouseDrag: !0,
        touchDrag: !0,
        addClassActive: !1,
        transitionStyle: !1,
        beforeUpdate: !1,
        afterUpdate: !1,
        beforeInit: !1,
        afterInit: !1,
        beforeMove: !1,
        afterMove: !1,
        afterAction: !1,
        startDragging: !1,
        afterLazyLoad: !1
    }
})(jQuery, window, document);
if ("undefined" == typeof jQuery)
    throw new Error("Bootstrap's JavaScript requires jQuery");
+function (a) {
    "use strict";
    var b = a.fn.jquery.split(" ")[0].split(".");
    if (b[0] < 2 && b[1] < 9 || 1 == b[0] && 9 == b[1] && b[2] < 1 || b[0] > 2)
        throw new Error("Bootstrap's JavaScript requires jQuery version 1.9.1 or higher, but lower than version 3")
}(jQuery),
+function (a) {
    "use strict";
    function b() {
        var a = document.createElement("bootstrap")
          , b = {
              WebkitTransition: "webkitTransitionEnd",
              MozTransition: "transitionend",
              OTransition: "oTransitionEnd otransitionend",
              transition: "transitionend"
          };
        for (var c in b)
            if (void 0 !== a.style[c])
                return {
                    end: b[c]
                };
        return !1
    }
    a.fn.emulateTransitionEnd = function (b) {
        var c = !1
          , d = this;
        a(this).one("bsTransitionEnd", function () {
            c = !0
        });
        var e = function () {
            c || a(d).trigger(a.support.transition.end)
        };
        return setTimeout(e, b),
        this
    }
    ,
    a(function () {
        a.support.transition = b(),
        a.support.transition && (a.event.special.bsTransitionEnd = {
            bindType: a.support.transition.end,
            delegateType: a.support.transition.end,
            handle: function (b) {
                return a(b.target).is(this) ? b.handleObj.handler.apply(this, arguments) : void 0
            }
        })
    })
}(jQuery),
+function (a) {
    "use strict";
    function b(b) {
        return this.each(function () {
            var c = a(this)
              , e = c.data("bs.alert");
            e || c.data("bs.alert", e = new d(this)),
            "string" == typeof b && e[b].call(c)
        })
    }
    var c = '[data-dismiss="alert"]'
      , d = function (b) {
          a(b).on("click", c, this.close)
      };
    d.VERSION = "3.3.6",
    d.TRANSITION_DURATION = 150,
    d.prototype.close = function (b) {
        function c() {
            g.detach().trigger("closed.bs.alert").remove()
        }
        var e = a(this)
          , f = e.attr("data-target");
        f || (f = e.attr("href"),
        f = f && f.replace(/.*(?=#[^\s]*$)/, ""));
        var g = a(f);
        b && b.preventDefault(),
        g.length || (g = e.closest(".alert")),
        g.trigger(b = a.Event("close.bs.alert")),
        b.isDefaultPrevented() || (g.removeClass("in"),
        a.support.transition && g.hasClass("fade") ? g.one("bsTransitionEnd", c).emulateTransitionEnd(d.TRANSITION_DURATION) : c())
    }
    ;
    var e = a.fn.alert;
    a.fn.alert = b,
    a.fn.alert.Constructor = d,
    a.fn.alert.noConflict = function () {
        return a.fn.alert = e,
        this
    }
    ,
    a(document).on("click.bs.alert.data-api", c, d.prototype.close)
}(jQuery),
+function (a) {
    "use strict";
    function b(b) {
        return this.each(function () {
            var d = a(this)
              , e = d.data("bs.button")
              , f = "object" == typeof b && b;
            e || d.data("bs.button", e = new c(this, f)),
            "toggle" == b ? e.toggle() : b && e.setState(b)
        })
    }
    var c = function (b, d) {
        this.$element = a(b),
        this.options = a.extend({}, c.DEFAULTS, d),
        this.isLoading = !1
    };
    c.VERSION = "3.3.6",
    c.DEFAULTS = {
        loadingText: "loading..."
    },
    c.prototype.setState = function (b) {
        var c = "disabled"
          , d = this.$element
          , e = d.is("input") ? "val" : "html"
          , f = d.data();
        b += "Text",
        null == f.resetText && d.data("resetText", d[e]()),
        setTimeout(a.proxy(function () {
            d[e](null == f[b] ? this.options[b] : f[b]),
            "loadingText" == b ? (this.isLoading = !0,
            d.addClass(c).attr(c, c)) : this.isLoading && (this.isLoading = !1,
            d.removeClass(c).removeAttr(c))
        }, this), 0)
    }
    ,
    c.prototype.toggle = function () {
        var a = !0
          , b = this.$element.closest('[data-toggle="buttons"]');
        if (b.length) {
            var c = this.$element.find("input");
            "radio" == c.prop("type") ? (c.prop("checked") && (a = !1),
            b.find(".active").removeClass("active"),
            this.$element.addClass("active")) : "checkbox" == c.prop("type") && (c.prop("checked") !== this.$element.hasClass("active") && (a = !1),
            this.$element.toggleClass("active")),
            c.prop("checked", this.$element.hasClass("active")),
            a && c.trigger("change")
        } else
            this.$element.attr("aria-pressed", !this.$element.hasClass("active")),
            this.$element.toggleClass("active")
    }
    ;
    var d = a.fn.button;
    a.fn.button = b,
    a.fn.button.Constructor = c,
    a.fn.button.noConflict = function () {
        return a.fn.button = d,
        this
    }
    ,
    a(document).on("click.bs.button.data-api", '[data-toggle^="button"]', function (c) {
        var d = a(c.target);
        d.hasClass("btn") || (d = d.closest(".btn")),
        b.call(d, "toggle"),
        a(c.target).is('input[type="radio"]') || a(c.target).is('input[type="checkbox"]') || c.preventDefault()
    }).on("focus.bs.button.data-api blur.bs.button.data-api", '[data-toggle^="button"]', function (b) {
        a(b.target).closest(".btn").toggleClass("focus", /^focus(in)?$/.test(b.type))
    })
}(jQuery),
+function (a) {
    "use strict";
    function b(b) {
        return this.each(function () {
            var d = a(this)
              , e = d.data("bs.carousel")
              , f = a.extend({}, c.DEFAULTS, d.data(), "object" == typeof b && b)
              , g = "string" == typeof b ? b : f.slide;
            e || d.data("bs.carousel", e = new c(this, f)),
            "number" == typeof b ? e.to(b) : g ? e[g]() : f.interval && e.pause().cycle()
        })
    }
    var c = function (b, c) {
        this.$element = a(b),
        this.$indicators = this.$element.find(".carousel-indicators"),
        this.options = c,
        this.paused = null,
        this.sliding = null,
        this.interval = null,
        this.$active = null,
        this.$items = null,
        this.options.keyboard && this.$element.on("keydown.bs.carousel", a.proxy(this.keydown, this)),
        "hover" == this.options.pause && !("ontouchstart" in document.documentElement) && this.$element.on("mouseenter.bs.carousel", a.proxy(this.pause, this)).on("mouseleave.bs.carousel", a.proxy(this.cycle, this))
    };
    c.VERSION = "3.3.6",
    c.TRANSITION_DURATION = 600,
    c.DEFAULTS = {
        interval: 5e3,
        pause: "hover",
        wrap: !0,
        keyboard: !0
    },
    c.prototype.keydown = function (a) {
        if (!/input|textarea/i.test(a.target.tagName)) {
            switch (a.which) {
                case 37:
                    this.prev();
                    break;
                case 39:
                    this.next();
                    break;
                default:
                    return
            }
            a.preventDefault()
        }
    }
    ,
    c.prototype.cycle = function (b) {
        return b || (this.paused = !1),
        this.interval && clearInterval(this.interval),
        this.options.interval && !this.paused && (this.interval = setInterval(a.proxy(this.next, this), this.options.interval)),
        this
    }
    ,
    c.prototype.getItemIndex = function (a) {
        return this.$items = a.parent().children(".item"),
        this.$items.index(a || this.$active)
    }
    ,
    c.prototype.getItemForDirection = function (a, b) {
        var c = this.getItemIndex(b)
          , d = "prev" == a && 0 === c || "next" == a && c == this.$items.length - 1;
        if (d && !this.options.wrap)
            return b;
        var e = "prev" == a ? -1 : 1
          , f = (c + e) % this.$items.length;
        return this.$items.eq(f)
    }
    ,
    c.prototype.to = function (a) {
        var b = this
          , c = this.getItemIndex(this.$active = this.$element.find(".item.active"));
        return a > this.$items.length - 1 || 0 > a ? void 0 : this.sliding ? this.$element.one("slid.bs.carousel", function () {
            b.to(a)
        }) : c == a ? this.pause().cycle() : this.slide(a > c ? "next" : "prev", this.$items.eq(a))
    }
    ,
    c.prototype.pause = function (b) {
        return b || (this.paused = !0),
        this.$element.find(".next, .prev").length && a.support.transition && (this.$element.trigger(a.support.transition.end),
        this.cycle(!0)),
        this.interval = clearInterval(this.interval),
        this
    }
    ,
    c.prototype.next = function () {
        return this.sliding ? void 0 : this.slide("next")
    }
    ,
    c.prototype.prev = function () {
        return this.sliding ? void 0 : this.slide("prev")
    }
    ,
    c.prototype.slide = function (b, d) {
        var e = this.$element.find(".item.active")
          , f = d || this.getItemForDirection(b, e)
          , g = this.interval
          , h = "next" == b ? "left" : "right"
          , i = this;
        if (f.hasClass("active"))
            return this.sliding = !1;
        var j = f[0]
          , k = a.Event("slide.bs.carousel", {
              relatedTarget: j,
              direction: h
          });
        if (this.$element.trigger(k),
        !k.isDefaultPrevented()) {
            if (this.sliding = !0,
            g && this.pause(),
            this.$indicators.length) {
                this.$indicators.find(".active").removeClass("active");
                var l = a(this.$indicators.children()[this.getItemIndex(f)]);
                l && l.addClass("active")
            }
            var m = a.Event("slid.bs.carousel", {
                relatedTarget: j,
                direction: h
            });
            return a.support.transition && this.$element.hasClass("slide") ? (f.addClass(b),
            f[0].offsetWidth,
            e.addClass(h),
            f.addClass(h),
            e.one("bsTransitionEnd", function () {
                f.removeClass([b, h].join(" ")).addClass("active"),
                e.removeClass(["active", h].join(" ")),
                i.sliding = !1,
                setTimeout(function () {
                    i.$element.trigger(m)
                }, 0)
            }).emulateTransitionEnd(c.TRANSITION_DURATION)) : (e.removeClass("active"),
            f.addClass("active"),
            this.sliding = !1,
            this.$element.trigger(m)),
            g && this.cycle(),
            this
        }
    }
    ;
    var d = a.fn.carousel;
    a.fn.carousel = b,
    a.fn.carousel.Constructor = c,
    a.fn.carousel.noConflict = function () {
        return a.fn.carousel = d,
        this
    }
    ;
    var e = function (c) {
        var d, e = a(this), f = a(e.attr("data-target") || (d = e.attr("href")) && d.replace(/.*(?=#[^\s]+$)/, ""));
        if (f.hasClass("carousel")) {
            var g = a.extend({}, f.data(), e.data())
              , h = e.attr("data-slide-to");
            h && (g.interval = !1),
            b.call(f, g),
            h && f.data("bs.carousel").to(h),
            c.preventDefault()
        }
    };
    a(document).on("click.bs.carousel.data-api", "[data-slide]", e).on("click.bs.carousel.data-api", "[data-slide-to]", e),
    a(window).on("load", function () {
        a('[data-ride="carousel"]').each(function () {
            var c = a(this);
            b.call(c, c.data())
        })
    })
}(jQuery),
+function (a) {
    "use strict";
    function b(b) {
        var c, d = b.attr("data-target") || (c = b.attr("href")) && c.replace(/.*(?=#[^\s]+$)/, "");
        return a(d)
    }
    function c(b) {
        return this.each(function () {
            var c = a(this)
              , e = c.data("bs.collapse")
              , f = a.extend({}, d.DEFAULTS, c.data(), "object" == typeof b && b);
            !e && f.toggle && /show|hide/.test(b) && (f.toggle = !1),
            e || c.data("bs.collapse", e = new d(this, f)),
            "string" == typeof b && e[b]()
        })
    }
    var d = function (b, c) {
        this.$element = a(b),
        this.options = a.extend({}, d.DEFAULTS, c),
        this.$trigger = a('[data-toggle="collapse"][href="#' + b.id + '"],[data-toggle="collapse"][data-target="#' + b.id + '"]'),
        this.transitioning = null,
        this.options.parent ? this.$parent = this.getParent() : this.addAriaAndCollapsedClass(this.$element, this.$trigger),
        this.options.toggle && this.toggle()
    };
    d.VERSION = "3.3.6",
    d.TRANSITION_DURATION = 350,
    d.DEFAULTS = {
        toggle: !0
    },
    d.prototype.dimension = function () {
        var a = this.$element.hasClass("width");
        return a ? "width" : "height"
    }
    ,
    d.prototype.show = function () {
        if (!this.transitioning && !this.$element.hasClass("in")) {
            var b, e = this.$parent && this.$parent.children(".panel").children(".in, .collapsing");
            if (!(e && e.length && (b = e.data("bs.collapse"),
            b && b.transitioning))) {
                var f = a.Event("show.bs.collapse");
                if (this.$element.trigger(f),
                !f.isDefaultPrevented()) {
                    e && e.length && (c.call(e, "hide"),
                    b || e.data("bs.collapse", null));
                    var g = this.dimension();
                    this.$element.removeClass("collapse").addClass("collapsing")[g](0).attr("aria-expanded", !0),
                    this.$trigger.removeClass("collapsed").attr("aria-expanded", !0),
                    this.transitioning = 1;
                    var h = function () {
                        this.$element.removeClass("collapsing").addClass("collapse in")[g](""),
                        this.transitioning = 0,
                        this.$element.trigger("shown.bs.collapse")
                    };
                    if (!a.support.transition)
                        return h.call(this);
                    var i = a.camelCase(["scroll", g].join("-"));
                    this.$element.one("bsTransitionEnd", a.proxy(h, this)).emulateTransitionEnd(d.TRANSITION_DURATION)[g](this.$element[0][i])
                }
            }
        }
    }
    ,
    d.prototype.hide = function () {
        if (!this.transitioning && this.$element.hasClass("in")) {
            var b = a.Event("hide.bs.collapse");
            if (this.$element.trigger(b),
            !b.isDefaultPrevented()) {
                var c = this.dimension();
                this.$element[c](this.$element[c]())[0].offsetHeight,
                this.$element.addClass("collapsing").removeClass("collapse in").attr("aria-expanded", !1),
                this.$trigger.addClass("collapsed").attr("aria-expanded", !1),
                this.transitioning = 1;
                var e = function () {
                    this.transitioning = 0,
                    this.$element.removeClass("collapsing").addClass("collapse").trigger("hidden.bs.collapse")
                };
                return a.support.transition ? void this.$element[c](0).one("bsTransitionEnd", a.proxy(e, this)).emulateTransitionEnd(d.TRANSITION_DURATION) : e.call(this)
            }
        }
    }
    ,
    d.prototype.toggle = function () {
        this[this.$element.hasClass("in") ? "hide" : "show"]()
    }
    ,
    d.prototype.getParent = function () {
        return a(this.options.parent).find('[data-toggle="collapse"][data-parent="' + this.options.parent + '"]').each(a.proxy(function (c, d) {
            var e = a(d);
            this.addAriaAndCollapsedClass(b(e), e)
        }, this)).end()
    }
    ,
    d.prototype.addAriaAndCollapsedClass = function (a, b) {
        var c = a.hasClass("in");
        a.attr("aria-expanded", c),
        b.toggleClass("collapsed", !c).attr("aria-expanded", c)
    }
    ;
    var e = a.fn.collapse;
    a.fn.collapse = c,
    a.fn.collapse.Constructor = d,
    a.fn.collapse.noConflict = function () {
        return a.fn.collapse = e,
        this
    }
    ,
    a(document).on("click.bs.collapse.data-api", '[data-toggle="collapse"]', function (d) {
        var e = a(this);
        e.attr("data-target") || d.preventDefault();
        var f = b(e)
          , g = f.data("bs.collapse")
          , h = g ? "toggle" : e.data();
        c.call(f, h)
    })
}(jQuery),
+function (a) {
    "use strict";
    function b(b) {
        var c = b.attr("data-target");
        c || (c = b.attr("href"),
        c = c && /#[A-Za-z]/.test(c) && c.replace(/.*(?=#[^\s]*$)/, ""));
        var d = c && a(c);
        return d && d.length ? d : b.parent()
    }
    function c(c) {
        c && 3 === c.which || (a(e).remove(),
        a(f).each(function () {
            var d = a(this)
              , e = b(d)
              , f = {
                  relatedTarget: this
              };
            e.hasClass("open") && (c && "click" == c.type && /input|textarea/i.test(c.target.tagName) && a.contains(e[0], c.target) || (e.trigger(c = a.Event("hide.bs.dropdown", f)),
            c.isDefaultPrevented() || (d.attr("aria-expanded", "false"),
            e.removeClass("open").trigger(a.Event("hidden.bs.dropdown", f)))))
        }))
    }
    function d(b) {
        return this.each(function () {
            var c = a(this)
              , d = c.data("bs.dropdown");
            d || c.data("bs.dropdown", d = new g(this)),
            "string" == typeof b && d[b].call(c)
        })
    }
    var e = ".dropdown-backdrop"
      , f = '[data-toggle="dropdown"]'
      , g = function (b) {
          a(b).on("click.bs.dropdown", this.toggle)
      };
    g.VERSION = "3.3.6",
    g.prototype.toggle = function (d) {
        var e = a(this);
        if (!e.is(".disabled, :disabled")) {
            var f = b(e)
              , g = f.hasClass("open");
            if (c(),
            !g) {
                "ontouchstart" in document.documentElement && !f.closest(".navbar-nav").length && a(document.createElement("div")).addClass("dropdown-backdrop").insertAfter(a(this)).on("click", c);
                var h = {
                    relatedTarget: this
                };
                if (f.trigger(d = a.Event("show.bs.dropdown", h)),
                d.isDefaultPrevented())
                    return;
                e.trigger("focus").attr("aria-expanded", "true"),
                f.toggleClass("open").trigger(a.Event("shown.bs.dropdown", h))
            }
            return !1
        }
    }
    ,
    g.prototype.keydown = function (c) {
        if (/(38|40|27|32)/.test(c.which) && !/input|textarea/i.test(c.target.tagName)) {
            var d = a(this);
            if (c.preventDefault(),
            c.stopPropagation(),
            !d.is(".disabled, :disabled")) {
                var e = b(d)
                  , g = e.hasClass("open");
                if (!g && 27 != c.which || g && 27 == c.which)
                    return 27 == c.which && e.find(f).trigger("focus"),
                    d.trigger("click");
                var h = " li:not(.disabled):visible a"
                  , i = e.find(".dropdown-menu" + h);
                if (i.length) {
                    var j = i.index(c.target);
                    38 == c.which && j > 0 && j--,
                    40 == c.which && j < i.length - 1 && j++,
                    ~j || (j = 0),
                    i.eq(j).trigger("focus")
                }
            }
        }
    }
    ;
    var h = a.fn.dropdown;
    a.fn.dropdown = d,
    a.fn.dropdown.Constructor = g,
    a.fn.dropdown.noConflict = function () {
        return a.fn.dropdown = h,
        this
    }
    ,
    a(document).on("click.bs.dropdown.data-api", c).on("click.bs.dropdown.data-api", ".dropdown form", function (a) {
        a.stopPropagation()
    }).on("click.bs.dropdown.data-api", f, g.prototype.toggle).on("keydown.bs.dropdown.data-api", f, g.prototype.keydown).on("keydown.bs.dropdown.data-api", ".dropdown-menu", g.prototype.keydown)
}(jQuery),
+function (a) {
    "use strict";
    function b(b, d) {
        return this.each(function () {
            var e = a(this)
              , f = e.data("bs.modal")
              , g = a.extend({}, c.DEFAULTS, e.data(), "object" == typeof b && b);
            f || e.data("bs.modal", f = new c(this, g)),
            "string" == typeof b ? f[b](d) : g.show && f.show(d)
        })
    }
    var c = function (b, c) {
        this.options = c,
        this.$body = a(document.body),
        this.$element = a(b),
        this.$dialog = this.$element.find(".modal-dialog"),
        this.$backdrop = null,
        this.isShown = null,
        this.originalBodyPad = null,
        this.scrollbarWidth = 0,
        this.ignoreBackdropClick = !1,
        this.options.remote && this.$element.find(".modal-content").load(this.options.remote, a.proxy(function () {
            this.$element.trigger("loaded.bs.modal")
        }, this))
    };
    c.VERSION = "3.3.6",
    c.TRANSITION_DURATION = 300,
    c.BACKDROP_TRANSITION_DURATION = 150,
    c.DEFAULTS = {
        backdrop: !0,
        keyboard: !0,
        show: !0
    },
    c.prototype.toggle = function (a) {
        return this.isShown ? this.hide() : this.show(a)
    }
    ,
    c.prototype.show = function (b) {
        var d = this
          , e = a.Event("show.bs.modal", {
              relatedTarget: b
          });
        this.$element.trigger(e),
        this.isShown || e.isDefaultPrevented() || (this.isShown = !0,
        this.checkScrollbar(),
        this.setScrollbar(),
        this.$body.addClass("modal-open"),
        this.escape(),
        this.resize(),
        this.$element.on("click.dismiss.bs.modal", '[data-dismiss="modal"]', a.proxy(this.hide, this)),
        this.$dialog.on("mousedown.dismiss.bs.modal", function () {
            d.$element.one("mouseup.dismiss.bs.modal", function (b) {
                a(b.target).is(d.$element) && (d.ignoreBackdropClick = !0)
            })
        }),
        this.backdrop(function () {
            var e = a.support.transition && d.$element.hasClass("fade");
            d.$element.parent().length || d.$element.appendTo(d.$body),
            d.$element.show().scrollTop(0),
            d.adjustDialog(),
            e && d.$element[0].offsetWidth,
            d.$element.addClass("in"),
            d.enforceFocus();
            var f = a.Event("shown.bs.modal", {
                relatedTarget: b
            });
            e ? d.$dialog.one("bsTransitionEnd", function () {
                d.$element.trigger("focus").trigger(f)
            }).emulateTransitionEnd(c.TRANSITION_DURATION) : d.$element.trigger("focus").trigger(f)
        }))
    }
    ,
    c.prototype.hide = function (b) {
        b && b.preventDefault(),
        b = a.Event("hide.bs.modal"),
        this.$element.trigger(b),
        this.isShown && !b.isDefaultPrevented() && (this.isShown = !1,
        this.escape(),
        this.resize(),
        a(document).off("focusin.bs.modal"),
        this.$element.removeClass("in").off("click.dismiss.bs.modal").off("mouseup.dismiss.bs.modal"),
        this.$dialog.off("mousedown.dismiss.bs.modal"),
        a.support.transition && this.$element.hasClass("fade") ? this.$element.one("bsTransitionEnd", a.proxy(this.hideModal, this)).emulateTransitionEnd(c.TRANSITION_DURATION) : this.hideModal())
    }
    ,
    c.prototype.enforceFocus = function () {
        a(document).off("focusin.bs.modal").on("focusin.bs.modal", a.proxy(function (a) {
            this.$element[0] === a.target || this.$element.has(a.target).length || this.$element.trigger("focus")
        }, this))
    }
    ,
    c.prototype.escape = function () {
        this.isShown && this.options.keyboard ? this.$element.on("keydown.dismiss.bs.modal", a.proxy(function (a) {
            27 == a.which && this.hide()
        }, this)) : this.isShown || this.$element.off("keydown.dismiss.bs.modal")
    }
    ,
    c.prototype.resize = function () {
        this.isShown ? a(window).on("resize.bs.modal", a.proxy(this.handleUpdate, this)) : a(window).off("resize.bs.modal")
    }
    ,
    c.prototype.hideModal = function () {
        var a = this;
        this.$element.hide(),
        this.backdrop(function () {
            a.$body.removeClass("modal-open"),
            a.resetAdjustments(),
            a.resetScrollbar(),
            a.$element.trigger("hidden.bs.modal")
        })
    }
    ,
    c.prototype.removeBackdrop = function () {
        this.$backdrop && this.$backdrop.remove(),
        this.$backdrop = null
    }
    ,
    c.prototype.backdrop = function (b) {
        var d = this
          , e = this.$element.hasClass("fade") ? "fade" : "";
        if (this.isShown && this.options.backdrop) {
            var f = a.support.transition && e;
            if (this.$backdrop = a(document.createElement("div")).addClass("modal-backdrop " + e).appendTo(this.$body),
            this.$element.on("click.dismiss.bs.modal", a.proxy(function (a) {
                return this.ignoreBackdropClick ? void (this.ignoreBackdropClick = !1) : void (a.target === a.currentTarget && ("static" == this.options.backdrop ? this.$element[0].focus() : this.hide()))
            }, this)),
            f && this.$backdrop[0].offsetWidth,
            this.$backdrop.addClass("in"),
            !b)
                return;
            f ? this.$backdrop.one("bsTransitionEnd", b).emulateTransitionEnd(c.BACKDROP_TRANSITION_DURATION) : b()
        } else if (!this.isShown && this.$backdrop) {
            this.$backdrop.removeClass("in");
            var g = function () {
                d.removeBackdrop(),
                b && b()
            };
            a.support.transition && this.$element.hasClass("fade") ? this.$backdrop.one("bsTransitionEnd", g).emulateTransitionEnd(c.BACKDROP_TRANSITION_DURATION) : g()
        } else
            b && b()
    }
    ,
    c.prototype.handleUpdate = function () {
        this.adjustDialog()
    }
    ,
    c.prototype.adjustDialog = function () {
        var a = this.$element[0].scrollHeight > document.documentElement.clientHeight;
        this.$element.css({
            paddingLeft: !this.bodyIsOverflowing && a ? this.scrollbarWidth : "",
            paddingRight: this.bodyIsOverflowing && !a ? this.scrollbarWidth : ""
        })
    }
    ,
    c.prototype.resetAdjustments = function () {
        this.$element.css({
            paddingLeft: "",
            paddingRight: ""
        })
    }
    ,
    c.prototype.checkScrollbar = function () {
        var a = window.innerWidth;
        if (!a) {
            var b = document.documentElement.getBoundingClientRect();
            a = b.right - Math.abs(b.left)
        }
        this.bodyIsOverflowing = document.body.clientWidth < a,
        this.scrollbarWidth = this.measureScrollbar()
    }
    ,
    c.prototype.setScrollbar = function () {
        var a = parseInt(this.$body.css("padding-right") || 0, 10);
        this.originalBodyPad = document.body.style.paddingRight || "",
        this.bodyIsOverflowing && this.$body.css("padding-right", a + this.scrollbarWidth)
    }
    ,
    c.prototype.resetScrollbar = function () {
        this.$body.css("padding-right", this.originalBodyPad)
    }
    ,
    c.prototype.measureScrollbar = function () {
        var a = document.createElement("div");
        a.className = "modal-scrollbar-measure",
        this.$body.append(a);
        var b = a.offsetWidth - a.clientWidth;
        return this.$body[0].removeChild(a),
        b
    }
    ;
    var d = a.fn.modal;
    a.fn.modal = b,
    a.fn.modal.Constructor = c,
    a.fn.modal.noConflict = function () {
        return a.fn.modal = d,
        this
    }
    ,
    a(document).on("click.bs.modal.data-api", '[data-toggle="modal"]', function (c) {
        var d = a(this)
          , e = d.attr("href")
          , f = a(d.attr("data-target") || e && e.replace(/.*(?=#[^\s]+$)/, ""))
          , g = f.data("bs.modal") ? "toggle" : a.extend({
              remote: !/#/.test(e) && e
          }, f.data(), d.data());
        d.is("a") && c.preventDefault(),
        f.one("show.bs.modal", function (a) {
            a.isDefaultPrevented() || f.one("hidden.bs.modal", function () {
                d.is(":visible") && d.trigger("focus")
            })
        }),
        b.call(f, g, this)
    })
}(jQuery),
+function (a) {
    "use strict";
    function b(b) {
        return this.each(function () {
            var d = a(this)
              , e = d.data("bs.tooltip")
              , f = "object" == typeof b && b;
            (e || !/destroy|hide/.test(b)) && (e || d.data("bs.tooltip", e = new c(this, f)),
            "string" == typeof b && e[b]())
        })
    }
    var c = function (a, b) {
        this.type = null,
        this.options = null,
        this.enabled = null,
        this.timeout = null,
        this.hoverState = null,
        this.$element = null,
        this.inState = null,
        this.init("tooltip", a, b)
    };
    c.VERSION = "3.3.6",
    c.TRANSITION_DURATION = 150,
    c.DEFAULTS = {
        animation: !0,
        placement: "top",
        selector: !1,
        template: '<div class="tooltip" role="tooltip"><div class="tooltip-arrow"></div><div class="tooltip-inner"></div></div>',
        trigger: "hover focus",
        title: "",
        delay: 0,
        html: !1,
        container: !1,
        viewport: {
            selector: "body",
            padding: 0
        }
    },
    c.prototype.init = function (b, c, d) {
        if (this.enabled = !0,
        this.type = b,
        this.$element = a(c),
        this.options = this.getOptions(d),
        this.$viewport = this.options.viewport && a(a.isFunction(this.options.viewport) ? this.options.viewport.call(this, this.$element) : this.options.viewport.selector || this.options.viewport),
        this.inState = {
            click: !1,
            hover: !1,
            focus: !1
        },
        this.$element[0] instanceof document.constructor && !this.options.selector)
            throw new Error("`selector` option must be specified when initializing " + this.type + " on the window.document object!");
        for (var e = this.options.trigger.split(" "), f = e.length; f--;) {
            var g = e[f];
            if ("click" == g)
                this.$element.on("click." + this.type, this.options.selector, a.proxy(this.toggle, this));
            else if ("manual" != g) {
                var h = "hover" == g ? "mouseenter" : "focusin"
                  , i = "hover" == g ? "mouseleave" : "focusout";
                this.$element.on(h + "." + this.type, this.options.selector, a.proxy(this.enter, this)),
                this.$element.on(i + "." + this.type, this.options.selector, a.proxy(this.leave, this))
            }
        }
        this.options.selector ? this._options = a.extend({}, this.options, {
            trigger: "manual",
            selector: ""
        }) : this.fixTitle()
    }
    ,
    c.prototype.getDefaults = function () {
        return c.DEFAULTS
    }
    ,
    c.prototype.getOptions = function (b) {
        return b = a.extend({}, this.getDefaults(), this.$element.data(), b),
        b.delay && "number" == typeof b.delay && (b.delay = {
            show: b.delay,
            hide: b.delay
        }),
        b
    }
    ,
    c.prototype.getDelegateOptions = function () {
        var b = {}
          , c = this.getDefaults();
        return this._options && a.each(this._options, function (a, d) {
            c[a] != d && (b[a] = d)
        }),
        b
    }
    ,
    c.prototype.enter = function (b) {
        var c = b instanceof this.constructor ? b : a(b.currentTarget).data("bs." + this.type);
        return c || (c = new this.constructor(b.currentTarget, this.getDelegateOptions()),
        a(b.currentTarget).data("bs." + this.type, c)),
        b instanceof a.Event && (c.inState["focusin" == b.type ? "focus" : "hover"] = !0),
        c.tip().hasClass("in") || "in" == c.hoverState ? void (c.hoverState = "in") : (clearTimeout(c.timeout),
        c.hoverState = "in",
        c.options.delay && c.options.delay.show ? void (c.timeout = setTimeout(function () {
            "in" == c.hoverState && c.show()
        }, c.options.delay.show)) : c.show())
    }
    ,
    c.prototype.isInStateTrue = function () {
        for (var a in this.inState)
            if (this.inState[a])
                return !0;
        return !1
    }
    ,
    c.prototype.leave = function (b) {
        var c = b instanceof this.constructor ? b : a(b.currentTarget).data("bs." + this.type);
        return c || (c = new this.constructor(b.currentTarget, this.getDelegateOptions()),
        a(b.currentTarget).data("bs." + this.type, c)),
        b instanceof a.Event && (c.inState["focusout" == b.type ? "focus" : "hover"] = !1),
        c.isInStateTrue() ? void 0 : (clearTimeout(c.timeout),
        c.hoverState = "out",
        c.options.delay && c.options.delay.hide ? void (c.timeout = setTimeout(function () {
            "out" == c.hoverState && c.hide()
        }, c.options.delay.hide)) : c.hide())
    }
    ,
    c.prototype.show = function () {
        var b = a.Event("show.bs." + this.type);
        if (this.hasContent() && this.enabled) {
            this.$element.trigger(b);
            var d = a.contains(this.$element[0].ownerDocument.documentElement, this.$element[0]);
            if (b.isDefaultPrevented() || !d)
                return;
            var e = this
              , f = this.tip()
              , g = this.getUID(this.type);
            this.setContent(),
            f.attr("id", g),
            this.$element.attr("aria-describedby", g),
            this.options.animation && f.addClass("fade");
            var h = "function" == typeof this.options.placement ? this.options.placement.call(this, f[0], this.$element[0]) : this.options.placement
              , i = /\s?auto?\s?/i
              , j = i.test(h);
            j && (h = h.replace(i, "") || "top"),
            f.detach().css({
                top: 0,
                left: 0,
                display: "block"
            }).addClass(h).data("bs." + this.type, this),
            this.options.container ? f.appendTo(this.options.container) : f.insertAfter(this.$element),
            this.$element.trigger("inserted.bs." + this.type);
            var k = this.getPosition()
              , l = f[0].offsetWidth
              , m = f[0].offsetHeight;
            if (j) {
                var n = h
                  , o = this.getPosition(this.$viewport);
                h = "bottom" == h && k.bottom + m > o.bottom ? "top" : "top" == h && k.top - m < o.top ? "bottom" : "right" == h && k.right + l > o.width ? "left" : "left" == h && k.left - l < o.left ? "right" : h,
                f.removeClass(n).addClass(h)
            }
            var p = this.getCalculatedOffset(h, k, l, m);
            this.applyPlacement(p, h);
            var q = function () {
                var a = e.hoverState;
                e.$element.trigger("shown.bs." + e.type),
                e.hoverState = null,
                "out" == a && e.leave(e)
            };
            a.support.transition && this.$tip.hasClass("fade") ? f.one("bsTransitionEnd", q).emulateTransitionEnd(c.TRANSITION_DURATION) : q()
        }
    }
    ,
    c.prototype.applyPlacement = function (b, c) {
        var d = this.tip()
          , e = d[0].offsetWidth
          , f = d[0].offsetHeight
          , g = parseInt(d.css("margin-top"), 10)
          , h = parseInt(d.css("margin-left"), 10);
        isNaN(g) && (g = 0),
        isNaN(h) && (h = 0),
        b.top += g,
        b.left += h,
        a.offset.setOffset(d[0], a.extend({
            using: function (a) {
                d.css({
                    top: Math.round(a.top),
                    left: Math.round(a.left)
                })
            }
        }, b), 0),
        d.addClass("in");
        var i = d[0].offsetWidth
          , j = d[0].offsetHeight;
        "top" == c && j != f && (b.top = b.top + f - j);
        var k = this.getViewportAdjustedDelta(c, b, i, j);
        k.left ? b.left += k.left : b.top += k.top;
        var l = /top|bottom/.test(c)
          , m = l ? 2 * k.left - e + i : 2 * k.top - f + j
          , n = l ? "offsetWidth" : "offsetHeight";
        d.offset(b),
        this.replaceArrow(m, d[0][n], l)
    }
    ,
    c.prototype.replaceArrow = function (a, b, c) {
        this.arrow().css(c ? "left" : "top", 50 * (1 - a / b) + "%").css(c ? "top" : "left", "")
    }
    ,
    c.prototype.setContent = function () {
        var a = this.tip()
          , b = this.getTitle();
        a.find(".tooltip-inner")[this.options.html ? "html" : "text"](b),
        a.removeClass("fade in top bottom left right")
    }
    ,
    c.prototype.hide = function (b) {
        function d() {
            "in" != e.hoverState && f.detach(),
            e.$element.removeAttr("aria-describedby").trigger("hidden.bs." + e.type),
            b && b()
        }
        var e = this
          , f = a(this.$tip)
          , g = a.Event("hide.bs." + this.type);
        return this.$element.trigger(g),
        g.isDefaultPrevented() ? void 0 : (f.removeClass("in"),
        a.support.transition && f.hasClass("fade") ? f.one("bsTransitionEnd", d).emulateTransitionEnd(c.TRANSITION_DURATION) : d(),
        this.hoverState = null,
        this)
    }
    ,
    c.prototype.fixTitle = function () {
        var a = this.$element;
        (a.attr("title") || "string" != typeof a.attr("data-original-title")) && a.attr("data-original-title", a.attr("title") || "").attr("title", "")
    }
    ,
    c.prototype.hasContent = function () {
        return this.getTitle()
    }
    ,
    c.prototype.getPosition = function (b) {
        b = b || this.$element;
        var c = b[0]
          , d = "BODY" == c.tagName
          , e = c.getBoundingClientRect();
        null == e.width && (e = a.extend({}, e, {
            width: e.right - e.left,
            height: e.bottom - e.top
        }));
        var f = d ? {
            top: 0,
            left: 0
        } : b.offset()
          , g = {
              scroll: d ? document.documentElement.scrollTop || document.body.scrollTop : b.scrollTop()
          }
          , h = d ? {
              width: a(window).width(),
              height: a(window).height()
          } : null;
        return a.extend({}, e, g, h, f)
    }
    ,
    c.prototype.getCalculatedOffset = function (a, b, c, d) {
        return "bottom" == a ? {
            top: b.top + b.height,
            left: b.left + b.width / 2 - c / 2
        } : "top" == a ? {
            top: b.top - d,
            left: b.left + b.width / 2 - c / 2
        } : "left" == a ? {
            top: b.top + b.height / 2 - d / 2,
            left: b.left - c
        } : {
            top: b.top + b.height / 2 - d / 2,
            left: b.left + b.width
        }
    }
    ,
    c.prototype.getViewportAdjustedDelta = function (a, b, c, d) {
        var e = {
            top: 0,
            left: 0
        };
        if (!this.$viewport)
            return e;
        var f = this.options.viewport && this.options.viewport.padding || 0
          , g = this.getPosition(this.$viewport);
        if (/right|left/.test(a)) {
            var h = b.top - f - g.scroll
              , i = b.top + f - g.scroll + d;
            h < g.top ? e.top = g.top - h : i > g.top + g.height && (e.top = g.top + g.height - i)
        } else {
            var j = b.left - f
              , k = b.left + f + c;
            j < g.left ? e.left = g.left - j : k > g.right && (e.left = g.left + g.width - k)
        }
        return e
    }
    ,
    c.prototype.getTitle = function () {
        var a, b = this.$element, c = this.options;
        return a = b.attr("data-original-title") || ("function" == typeof c.title ? c.title.call(b[0]) : c.title)
    }
    ,
    c.prototype.getUID = function (a) {
        do
            a += ~~(1e6 * Math.random());
        while (document.getElementById(a)); return a
    }
    ,
    c.prototype.tip = function () {
        if (!this.$tip && (this.$tip = a(this.options.template),
        1 != this.$tip.length))
            throw new Error(this.type + " `template` option must consist of exactly 1 top-level element!");
        return this.$tip
    }
    ,
    c.prototype.arrow = function () {
        return this.$arrow = this.$arrow || this.tip().find(".tooltip-arrow")
    }
    ,
    c.prototype.enable = function () {
        this.enabled = !0
    }
    ,
    c.prototype.disable = function () {
        this.enabled = !1
    }
    ,
    c.prototype.toggleEnabled = function () {
        this.enabled = !this.enabled
    }
    ,
    c.prototype.toggle = function (b) {
        var c = this;
        b && (c = a(b.currentTarget).data("bs." + this.type),
        c || (c = new this.constructor(b.currentTarget, this.getDelegateOptions()),
        a(b.currentTarget).data("bs." + this.type, c))),
        b ? (c.inState.click = !c.inState.click,
        c.isInStateTrue() ? c.enter(c) : c.leave(c)) : c.tip().hasClass("in") ? c.leave(c) : c.enter(c)
    }
    ,
    c.prototype.destroy = function () {
        var a = this;
        clearTimeout(this.timeout),
        this.hide(function () {
            a.$element.off("." + a.type).removeData("bs." + a.type),
            a.$tip && a.$tip.detach(),
            a.$tip = null,
            a.$arrow = null,
            a.$viewport = null
        })
    }
    ;
    var d = a.fn.tooltip;
    a.fn.tooltip = b,
    a.fn.tooltip.Constructor = c,
    a.fn.tooltip.noConflict = function () {
        return a.fn.tooltip = d,
        this
    }
}(jQuery),
+function (a) {
    "use strict";
    function b(b) {
        return this.each(function () {
            var d = a(this)
              , e = d.data("bs.popover")
              , f = "object" == typeof b && b;
            (e || !/destroy|hide/.test(b)) && (e || d.data("bs.popover", e = new c(this, f)),
            "string" == typeof b && e[b]())
        })
    }
    var c = function (a, b) {
        this.init("popover", a, b)
    };
    if (!a.fn.tooltip)
        throw new Error("Popover requires tooltip.js");
    c.VERSION = "3.3.6",
    c.DEFAULTS = a.extend({}, a.fn.tooltip.Constructor.DEFAULTS, {
        placement: "top",
        trigger: "click",
        content: "",
        template: '<div class="popover" role="tooltip"><div class="arrow"></div><h3 class="popover-title"></h3><div class="popover-content"></div></div>'
    }),
    c.prototype = a.extend({}, a.fn.tooltip.Constructor.prototype),
    c.prototype.constructor = c,
    c.prototype.getDefaults = function () {
        return c.DEFAULTS
    }
    ,
    c.prototype.setContent = function () {
        var a = this.tip()
          , b = this.getTitle()
          , c = this.getContent();
        a.find(".popover-title")[this.options.html ? "html" : "text"](b),
        a.find(".popover-content").children().detach().end()[this.options.html ? "string" == typeof c ? "html" : "append" : "text"](c),
        a.removeClass("fade top bottom left right in"),
        a.find(".popover-title").html() || a.find(".popover-title").hide()
    }
    ,
    c.prototype.hasContent = function () {
        return this.getTitle() || this.getContent()
    }
    ,
    c.prototype.getContent = function () {
        var a = this.$element
          , b = this.options;
        return a.attr("data-content") || ("function" == typeof b.content ? b.content.call(a[0]) : b.content)
    }
    ,
    c.prototype.arrow = function () {
        return this.$arrow = this.$arrow || this.tip().find(".arrow")
    }
    ;
    var d = a.fn.popover;
    a.fn.popover = b,
    a.fn.popover.Constructor = c,
    a.fn.popover.noConflict = function () {
        return a.fn.popover = d,
        this
    }
}(jQuery),
+function (a) {
    "use strict";
    function b(c, d) {
        this.$body = a(document.body),
        this.$scrollElement = a(a(c).is(document.body) ? window : c),
        this.options = a.extend({}, b.DEFAULTS, d),
        this.selector = (this.options.target || "") + " .nav li > a",
        this.offsets = [],
        this.targets = [],
        this.activeTarget = null,
        this.scrollHeight = 0,
        this.$scrollElement.on("scroll.bs.scrollspy", a.proxy(this.process, this)),
        this.refresh(),
        this.process()
    }
    function c(c) {
        return this.each(function () {
            var d = a(this)
              , e = d.data("bs.scrollspy")
              , f = "object" == typeof c && c;
            e || d.data("bs.scrollspy", e = new b(this, f)),
            "string" == typeof c && e[c]()
        })
    }
    b.VERSION = "3.3.6",
    b.DEFAULTS = {
        offset: 10
    },
    b.prototype.getScrollHeight = function () {
        return this.$scrollElement[0].scrollHeight || Math.max(this.$body[0].scrollHeight, document.documentElement.scrollHeight)
    }
    ,
    b.prototype.refresh = function () {
        var b = this
          , c = "offset"
          , d = 0;
        this.offsets = [],
        this.targets = [],
        this.scrollHeight = this.getScrollHeight(),
        a.isWindow(this.$scrollElement[0]) || (c = "position",
        d = this.$scrollElement.scrollTop()),
        this.$body.find(this.selector).map(function () {
            var b = a(this)
              , e = b.data("target") || b.attr("href")
              , f = /^#./.test(e) && a(e);
            return f && f.length && f.is(":visible") && [[f[c]().top + d, e]] || null
        }).sort(function (a, b) {
            return a[0] - b[0]
        }).each(function () {
            b.offsets.push(this[0]),
            b.targets.push(this[1])
        })
    }
    ,
    b.prototype.process = function () {
        var a, b = this.$scrollElement.scrollTop() + this.options.offset, c = this.getScrollHeight(), d = this.options.offset + c - this.$scrollElement.height(), e = this.offsets, f = this.targets, g = this.activeTarget;
        if (this.scrollHeight != c && this.refresh(),
        b >= d)
            return g != (a = f[f.length - 1]) && this.activate(a);
        if (g && b < e[0])
            return this.activeTarget = null,
            this.clear();
        for (a = e.length; a--;)
            g != f[a] && b >= e[a] && (void 0 === e[a + 1] || b < e[a + 1]) && this.activate(f[a])
    }
    ,
    b.prototype.activate = function (b) {
        this.activeTarget = b,
        this.clear();
        var c = this.selector + '[data-target="' + b + '"],' + this.selector + '[href="' + b + '"]'
          , d = a(c).parents("li").addClass("active");
        d.parent(".dropdown-menu").length && (d = d.closest("li.dropdown").addClass("active")),
        d.trigger("activate.bs.scrollspy")
    }
    ,
    b.prototype.clear = function () {
        a(this.selector).parentsUntil(this.options.target, ".active").removeClass("active")
    }
    ;
    var d = a.fn.scrollspy;
    a.fn.scrollspy = c,
    a.fn.scrollspy.Constructor = b,
    a.fn.scrollspy.noConflict = function () {
        return a.fn.scrollspy = d,
        this
    }
    ,
    a(window).on("load.bs.scrollspy.data-api", function () {
        a('[data-spy="scroll"]').each(function () {
            var b = a(this);
            c.call(b, b.data())
        })
    })
}(jQuery),
+function (a) {
    "use strict";
    function b(b) {
        return this.each(function () {
            var d = a(this)
              , e = d.data("bs.tab");
            e || d.data("bs.tab", e = new c(this)),
            "string" == typeof b && e[b]()
        })
    }
    var c = function (b) {
        this.element = a(b)
    };
    c.VERSION = "3.3.6",
    c.TRANSITION_DURATION = 150,
    c.prototype.show = function () {
        var b = this.element
          , c = b.closest("ul:not(.dropdown-menu)")
          , d = b.data("target");
        if (d || (d = b.attr("href"),
        d = d && d.replace(/.*(?=#[^\s]*$)/, "")),
        !b.parent("li").hasClass("active")) {
            var e = c.find(".active:last a")
              , f = a.Event("hide.bs.tab", {
                  relatedTarget: b[0]
              })
              , g = a.Event("show.bs.tab", {
                  relatedTarget: e[0]
              });
            if (e.trigger(f),
            b.trigger(g),
            !g.isDefaultPrevented() && !f.isDefaultPrevented()) {
                var h = a(d);
                this.activate(b.closest("li"), c),
                this.activate(h, h.parent(), function () {
                    e.trigger({
                        type: "hidden.bs.tab",
                        relatedTarget: b[0]
                    }),
                    b.trigger({
                        type: "shown.bs.tab",
                        relatedTarget: e[0]
                    })
                })
            }
        }
    }
    ,
    c.prototype.activate = function (b, d, e) {
        function f() {
            g.removeClass("active").find("> .dropdown-menu > .active").removeClass("active").end().find('[data-toggle="tab"]').attr("aria-expanded", !1),
            b.addClass("active").find('[data-toggle="tab"]').attr("aria-expanded", !0),
            h ? (b[0].offsetWidth,
            b.addClass("in")) : b.removeClass("fade"),
            b.parent(".dropdown-menu").length && b.closest("li.dropdown").addClass("active").end().find('[data-toggle="tab"]').attr("aria-expanded", !0),
            e && e()
        }
        var g = d.find("> .active")
          , h = e && a.support.transition && (g.length && g.hasClass("fade") || !!d.find("> .fade").length);
        g.length && h ? g.one("bsTransitionEnd", f).emulateTransitionEnd(c.TRANSITION_DURATION) : f(),
        g.removeClass("in")
    }
    ;
    var d = a.fn.tab;
    a.fn.tab = b,
    a.fn.tab.Constructor = c,
    a.fn.tab.noConflict = function () {
        return a.fn.tab = d,
        this
    }
    ;
    var e = function (c) {
        c.preventDefault(),
        b.call(a(this), "show")
    };
    a(document).on("click.bs.tab.data-api", '[data-toggle="tab"]', e).on("click.bs.tab.data-api", '[data-toggle="pill"]', e)
}(jQuery),
+function (a) {
    "use strict";
    function b(b) {
        return this.each(function () {
            var d = a(this)
              , e = d.data("bs.affix")
              , f = "object" == typeof b && b;
            e || d.data("bs.affix", e = new c(this, f)),
            "string" == typeof b && e[b]()
        })
    }
    var c = function (b, d) {
        this.options = a.extend({}, c.DEFAULTS, d),
        this.$target = a(this.options.target).on("scroll.bs.affix.data-api", a.proxy(this.checkPosition, this)).on("click.bs.affix.data-api", a.proxy(this.checkPositionWithEventLoop, this)),
        this.$element = a(b),
        this.affixed = null,
        this.unpin = null,
        this.pinnedOffset = null,
        this.checkPosition()
    };
    c.VERSION = "3.3.6",
    c.RESET = "affix affix-top affix-bottom",
    c.DEFAULTS = {
        offset: 0,
        target: window
    },
    c.prototype.getState = function (a, b, c, d) {
        var e = this.$target.scrollTop()
          , f = this.$element.offset()
          , g = this.$target.height();
        if (null != c && "top" == this.affixed)
            return c > e ? "top" : !1;
        if ("bottom" == this.affixed)
            return null != c ? e + this.unpin <= f.top ? !1 : "bottom" : a - d >= e + g ? !1 : "bottom";
        var h = null == this.affixed
          , i = h ? e : f.top
          , j = h ? g : b;
        return null != c && c >= e ? "top" : null != d && i + j >= a - d ? "bottom" : !1
    }
    ,
    c.prototype.getPinnedOffset = function () {
        if (this.pinnedOffset)
            return this.pinnedOffset;
        this.$element.removeClass(c.RESET).addClass("affix");
        var a = this.$target.scrollTop()
          , b = this.$element.offset();
        return this.pinnedOffset = b.top - a
    }
    ,
    c.prototype.checkPositionWithEventLoop = function () {
        setTimeout(a.proxy(this.checkPosition, this), 1)
    }
    ,
    c.prototype.checkPosition = function () {
        if (this.$element.is(":visible")) {
            var b = this.$element.height()
              , d = this.options.offset
              , e = d.top
              , f = d.bottom
              , g = Math.max(a(document).height(), a(document.body).height());
            "object" != typeof d && (f = e = d),
            "function" == typeof e && (e = d.top(this.$element)),
            "function" == typeof f && (f = d.bottom(this.$element));
            var h = this.getState(g, b, e, f);
            if (this.affixed != h) {
                null != this.unpin && this.$element.css("top", "");
                var i = "affix" + (h ? "-" + h : "")
                  , j = a.Event(i + ".bs.affix");
                if (this.$element.trigger(j),
                j.isDefaultPrevented())
                    return;
                this.affixed = h,
                this.unpin = "bottom" == h ? this.getPinnedOffset() : null,
                this.$element.removeClass(c.RESET).addClass(i).trigger(i.replace("affix", "affixed") + ".bs.affix")
            }
            "bottom" == h && this.$element.offset({
                top: g - b - f
            })
        }
    }
    ;
    var d = a.fn.affix;
    a.fn.affix = b,
    a.fn.affix.Constructor = c,
    a.fn.affix.noConflict = function () {
        return a.fn.affix = d,
        this
    }
    ,
    a(window).on("load", function () {
        a('[data-spy="affix"]').each(function () {
            var c = a(this)
              , d = c.data();
            d.offset = d.offset || {},
            null != d.offsetBottom && (d.offset.bottom = d.offsetBottom),
            null != d.offsetTop && (d.offset.top = d.offsetTop),
            b.call(c, d)
        })
    })
}(jQuery);
!function (a, b, c, d) {
    var e = a(b);
    a.fn.lazyload = function (f) {
        function g() {
            var b = 0;
            i.each(function () {
                var c = a(this);
                if (!j.skip_invisible || c.is(":visible"))
                    if (a.abovethetop(this, j) || a.leftofbegin(this, j))
                        ;
                    else if (a.belowthefold(this, j) || a.rightoffold(this, j)) {
                        if (++b > j.failure_limit)
                            return !1
                    } else
                        c.trigger("appear"),
                        b = 0
            })
        }
        var h, i = this, j = {
            threshold: 0,
            failure_limit: 0,
            event: "scroll",
            effect: "show",
            container: b,
            data_attribute: "original",
            skip_invisible: !0,
            appear: null,
            load: null,
            placeholder: "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsQAAA7EAZUrDhsAAAANSURBVBhXYzh8+PB/AAffA0nNPuCLAAAAAElFTkSuQmCC"
        };
        return f && (d !== f.failurelimit && (f.failure_limit = f.failurelimit,
        delete f.failurelimit),
        d !== f.effectspeed && (f.effect_speed = f.effectspeed,
        delete f.effectspeed),
        a.extend(j, f)),
        h = j.container === d || j.container === b ? e : a(j.container),
        0 === j.event.indexOf("scroll") && h.bind(j.event, function () {
            return g()
        }),
        this.each(function () {
            var b = this
              , c = a(b);
            b.loaded = !1,
            (c.attr("src") === d || c.attr("src") === !1) && c.is("img") && c.attr("src", j.placeholder),
            c.one("appear", function () {
                if (!this.loaded) {
                    if (j.appear) {
                        var d = i.length;
                        j.appear.call(b, d, j)
                    }
                    a("<img />").bind("load", function () {
                        var d = c.attr("data-" + j.data_attribute);
                        c.hide(),
                        c.is("img") ? c.attr("src", d) : c.css("background-image", "url('" + d + "')"),
                        c[j.effect](j.effect_speed),
                        b.loaded = !0;
                        var e = a.grep(i, function (a) {
                            return !a.loaded
                        });
                        if (i = a(e),
                        j.load) {
                            var f = i.length;
                            j.load.call(b, f, j)
                        }
                    }).attr("src", c.attr("data-" + j.data_attribute))
                }
            }),
            0 !== j.event.indexOf("scroll") && c.bind(j.event, function () {
                b.loaded || c.trigger("appear")
            })
        }),
        e.bind("resize", function () {
            g()
        }),
        /(?:iphone|ipod|ipad).*os 5/gi.test(navigator.appVersion) && e.bind("pageshow", function (b) {
            b.originalEvent && b.originalEvent.persisted && i.each(function () {
                a(this).trigger("appear")
            })
        }),
        a(c).ready(function () {
            g()
        }),
        this
    }
    ,
    a.belowthefold = function (c, f) {
        var g;
        return g = f.container === d || f.container === b ? (b.innerHeight ? b.innerHeight : e.height()) + e.scrollTop() : a(f.container).offset().top + a(f.container).height(),
        g <= a(c).offset().top - f.threshold
    }
    ,
    a.rightoffold = function (c, f) {
        var g;
        return g = f.container === d || f.container === b ? e.width() + e.scrollLeft() : a(f.container).offset().left + a(f.container).width(),
        g <= a(c).offset().left - f.threshold
    }
    ,
    a.abovethetop = function (c, f) {
        var g;
        return g = f.container === d || f.container === b ? e.scrollTop() : a(f.container).offset().top,
        g >= a(c).offset().top + f.threshold + a(c).height()
    }
    ,
    a.leftofbegin = function (c, f) {
        var g;
        return g = f.container === d || f.container === b ? e.scrollLeft() : a(f.container).offset().left,
        g >= a(c).offset().left + f.threshold + a(c).width()
    }
    ,
    a.inviewport = function (b, c) {
        return !(a.rightoffold(b, c) || a.leftofbegin(b, c) || a.belowthefold(b, c) || a.abovethetop(b, c))
    }
    ,
    a.extend(a.expr[":"], {
        "below-the-fold": function (b) {
            return a.belowthefold(b, {
                threshold: 0
            })
        },
        "above-the-top": function (b) {
            return !a.belowthefold(b, {
                threshold: 0
            })
        },
        "right-of-screen": function (b) {
            return a.rightoffold(b, {
                threshold: 0
            })
        },
        "left-of-screen": function (b) {
            return !a.rightoffold(b, {
                threshold: 0
            })
        },
        "in-viewport": function (b) {
            return a.inviewport(b, {
                threshold: 0
            })
        },
        "above-the-fold": function (b) {
            return !a.belowthefold(b, {
                threshold: 0
            })
        },
        "right-of-fold": function (b) {
            return a.rightoffold(b, {
                threshold: 0
            })
        },
        "left-of-fold": function (b) {
            return !a.rightoffold(b, {
                threshold: 0
            })
        }
    })
}(jQuery, window, document);
