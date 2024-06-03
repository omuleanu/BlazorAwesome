//::beginawef
var awef = function ($) {
    var entityMap = {
        "&": "&amp;",
        "<": "&lt;",
        '"': '&quot;',
        "'": '&#39;',
        ">": '&gt;'
    };

    var idnr = 1;
    var storage = {};

    function isNull(val) {
        return val === undefined || val === null;
    }

    function loop(arr, f) {
        if (arr) {
            for (var i = 0, ln = len(arr); i < ln; i++) {
                var col = arr[i];
                if (f(col, i) === false) {
                    break;
                };
            }
        }
    }

    function len(o) {
        return !o ? 0 : o.length;
    }

    function isNullOrEmp(val) {
        return awef.isNull(val) || len(val.toString()) === 0;
    }

    function strEquals(x, y) {
        if (isNull(x) || isNull(y)) {
            return x === y;
        }

        return x.toString() === y.toString();
    }

    function select(list, func) {
        var res = [];
        loop(list, function (el, i) {
            res.push(func(el, i));
        });

        return res;
    }

    function data(elm, name, obj) {
        var id = getOrAddIdFor(elm);
        var so = storage[id] || {};
        storage[id] = so;

        if (obj) {
            so[name] = obj;
            return id;
        } else {
            if (name) return so[name];
            return so;
        }
    }

    function getOrAddIdFor(elm) {
        elm = $(elm);
        var id = elm.attr('aweid');
        if (!id) {
            id = 'awe-' + (idnr++);
            elm.attr('aweid', id);
        }

        return id;
    }

    function remd(elm) {
        var id = getOrAddIdFor(elm);
        delete storage[id];
    }

    function trigger(elm, ename) {
        if (len(elm)) {
            var ev = new CustomEvent(ename, { bubbles: true });
            elm[0].dispatchEvent(ev);
        }
    }

    function getFunc(fdecl) {//, context
        if (typeof fdecl != 'string') {
            return fdecl;
        }

        if (!fdecl) return;

        var fname = fdecl;
        var fargs = null;

        var parts = getDeclFuncParts(fdecl);

        if (parts.length > 1) {
            fname = parts[0];
            fargs = getArgs(parts[1]);
        }

        var context = window;
        var namespaces = fname.split(".");
        var func = namespaces.pop();
        for (var i = 0; i < namespaces.length; i++) {
            context = context[namespaces[i]];
        }

        var res;

        if (func == 'null') { res = null; }
        else if (func == 'undefined') { res = undefined; }
        else {
            res = context[func];
            if (!res) {
                throw "could not find " + func + " " + fdecl;
            }

            if (fargs) {
                var r1 = res.apply(window, fargs);
                return r1;
            }
        }

        return res;
    }

    function getDeclFuncParts(str) {
        var prnti = str.indexOf('(');
        if (prnti < 0) return [str];
        return [str.substring(0, prnti), str.substring(prnti)];
    }

    function getArgs(str) {
        function isNumeric(n) {
            return !isNaN(parseFloat(n));
        }

        var i = 1;

        var mode = 0;
        var argendc = '';
        var argval = '';
        var resargs = [];
        var commas = 0;
        while (i < str.length) {
            var c = str[i];
            i++;

            if (c == ' ' && mode === 0) continue;

            if ((c == ')' || c == ',') && mode === 0) {

                if (c == ',') {
                    commas++;
                }

                if (argval.length) {
                    if (isNumeric(argval)) {
                        resargs.push(parseFloat(argval));
                    }
                    else {
                        resargs.push(getFunc(argval));
                    }

                    argval = '';
                }

                if (c == ')') break;

                if (commas > resargs.length) {
                    resargs.push(undefined);
                }

                continue;
            }

            if (mode == 'strArg') {

                if (c == argendc) {
                    resargs.push(argval);
                    argval = '';
                    mode = 0;
                    continue;
                }
            }
            else {
                if (c == "'" || c == '"') {
                    mode = 'strArg';
                    argendc = c;
                    continue;
                }
            }

            argval += c;

        }

        return resargs;
    }

    return {
        gfunc: getFunc, // (ex eval)
        setCookie: function (name, val) {
            document.cookie = name + '=' + val;
        },
        trigger: trigger,
        data: data,
        remd: remd,
        storage: storage,
        outerh: function (o, b) {
            return o.outerHeight(!!b);
        },
        outerw: function (o, b) {
            return o.outerWidth(!!b);
        },
        seq: strEquals,
        len: len,
        sval: function (val) {
            if (Array.isArray(val)) {
                val = JSON.stringify(val);
            }

            return val;
        },
        // from json obj to namevalue array
        serlObj: function (jobj) {
            var res = [];

            for (var key in jobj) {
                if (!Array.isArray(jobj[key]))
                    res.push({ name: key, value: jobj[key] });
                else res = res.concat(awef.serlArr(jobj[key], key));
            }

            return res;
        },
        vcont: function (v, vals) {
            for (var i = 0; i < len(vals); i++) {
                if (strEquals(v, vals[i])) {
                    return 1;
                }
            }

            return 0;
        },
        scont: function (str, src) {
            return isNullOrEmp(src) || str.toLowerCase().indexOf(src.toLowerCase()) > -1;
        },
        loop: loop,
        isNotNull: function (val) {
            return !awef.isNull(val);
        },
        isNull: isNull,
        isNullOrEmp: isNullOrEmp,
        encode: function (str) {
            return String(str).replace(/[&<>"']/g, function (s) {
                return entityMap[s];
            });
        },
        //arr = [1,2,3] k="hi" -> {name:hi, value: 1} ...
        serlArr: function (arr, k) {
            var res = [];
            if (isNull(arr)) return res;

            if (!Array.isArray(arr)) arr = [arr];

            loop(arr,
                function (v) {
                    res.push({ name: k, value: v });
                });

            return res;
        },
        select: select,
        where: function (list, func) {
            var res = [];
            loop(list,
                function (el) {
                    if (func(el)) {
                        res.push(el);
                    }
                });

            return res;
        },
        logErr: console && console.error || function () { },
        tchange: function (elm) {
            return elm.trigger('change');
        },
        tfocus: function (elm) {
            return elm.trigger('focus');
        }
    };
}(jQuery);

//export {awef};
//::endawef
//::beginawe
// Copyright (c) Ribor
/* eslint-disable */
//import * as jQuery from 'jquery';
//import {awef} from './awef.js';
var awe = function ($) {
    var $win = $(window);
    var $document = $(document);
    var $doc = $document;
    var $window = $(window);
    var loop = awef.loop, qsel = awef.select, encode = awef.encode, isNull = awef.isNull, isNotNull = awef.isNotNull, isEmp = awef.isNullOrEmp, gfunc = awef.gfunc;
    var len = awef.len, outerh = awef.outerh, outerw = awef.outerw, logError = awef.logErr;
    var tchange = awef.tchange, tfocus = awef.tfocus;
    var adata = awef.data;

    var minZindex = 1;
    var popupDraggable = 1;
    var popSpace = 50;
    var popHorizSpace = 10;
    var popTopSpace = 0;
    var clickOutSpace = 35;
    var hpSpace = popSpace / 2;
    var hpHorizSpace = popHorizSpace / 2;    
    var maxDropdownHeight = 420;
    var menuMinh = 200;

    var keyEnter = 13;
    var keyRight = 39;
    var keyLeft = 37;
    var keySpace = 32;
    var keyUp = 38;
    var keyDown = 40;
    var keyEsc = 27;
    var keyTab = 9;
    var keyBackspace = 8;
    var keyShift = 16;
    var keyCtrl = 17;
    // down and up arrow, enter, esc, shift //, left arrow, right arrow
    var controlKeys = [keyUp, keyDown, keyEnter, keyEsc, keyShift];

    var idnw = 16;
    var loadingIco = '<div class="awe-loading"><span></span></div>';
    var loadingStr = '<span class="awe-ldgcnt">' + loadingIco + '</span>';
    var sdisabled = "disabled";
    var schange = "change";
    var smousemove = 'mousemove';
    var sfocus = 'focus';
    var sfocusin = sfocus + "in";
    var sfocusout = sfocus + "out";
    var sawepch = "awepch";
    var sclick = "click";
    var sawebeginload = "awebeginload";
    var saweload = "aweload";
    var sconid = "__aweconid";
    var nbsp = "<div class='awe-empty'>&nbsp;</div>";
    var nul = null;
    var touchstartmousedown = 'touchstart mousedown';
    var touchstartmousedownNpd = 'touchstart.awenpd mousedown';
    var ws = ' ';
    var sawerowcls = '.awe-row';

    var sawedisabled = 'awe-' + sdisabled;
    var saweselected = "awe-selected";
    var sawechanging = "awe-changing";
    var sawehighlight = 'awe-highlight';
    var sawedragging = "awe-dragging";
    var sawehasval = "awe-hasval";
    var sawegroupable = "awe-groupable";
    var sawenonselect = 'awe-nonselect';
    var saweselectedclass = '.' + saweselected;
    var namedInput = ":input[name]";
    var lookupSearchFormat = '<input type="text" name="search" class="awe-searchtxt awe-txt" placeholder="{1}..."/><button type="button" class="awe-searchbtn awe-btn">{0}</button>';
    var hiddenInputFormat = "<input type='hidden' name='{0}' value='{1}' />";
    var colidn = '<col class="awe-idn"/>';
    var estr = "";
    var dot = ".";


    var se = '';
    var sddpOutClEv = 'click.ddp mousedown.ddp touchstart.ddp';
    var skeyup = 'keyup';
    var skeydown = 'keydown';
    var sposition = 'position';
    var sheight = 'height';
    var sminw = 'min-width';
    var szindex = 'z-index';
    var swidth = 'width';
    var smaxh = 'max-height';
    var sselected = 'selected';
    var svalc = '.o-v';

    var max = Math.max, min = Math.min, round = Math.round;

    var kvIdOpener = {};
    var storage = {};    

    var isMobile = function () { return awe.isMobile(); };

    //#region core sync

    var popId = 1;
    function dapi(o, newapi) {
        if (newapi) return o.data('api', newapi);
        return o.data('api');
    }

    var emptyFunc = function () { };
    var prevDef = function (e) { return awe.prevDef(e); };

    function addClass(o, s) {
        return o.addClass(s);
    }

    function flash(o, func) {
        addClass(o, sawechanging);
        o.one('animationend', function () {
            removeClass(o, sawechanging);
            func && func();
        });
    }

    function tabbable(o) {
        return o.find('input,button,select,textarea,a[href],[tabindex]').filter(':visible:not([disabled]):not([tabindex="-1"])');
    }

    function isDirectionRtl(element) {
        var e = element[0];
        var result = nul;

        if (window.getComputedStyle) {
            result = window.getComputedStyle(e).direction;
        } else if (e.currentStyle) {
            result = e.currentStyle.direction;
        }

        return result === "rtl";
    }

    function tclick(elm) {
        elm.trigger(sclick);
    }

    function removeClass(o, s) {
        return o.removeClass(s);
    }

    function trg(e) {
        return $(e.target);
    }

    function istrg(e, sel) {
        return trg(e).closest(sel).length;
    }

    function addDestrFunc(o, destrFunc, type, handler, id) {
        o.desf = o.desf || [];
        o.desf.push({ f: destrFunc, type: type, h: handler, id: id });
    }

    function unbind(o, handler, id) {
        loop(o.desf, function (v, i) {
            if ((id && v.id == id) || (v.h === handler)) {
                o.desf.splice(i, 1);
                v.f();
                return false;
            }
        });
    }

    function bind(o, tag, event, selector, handler) {
        tag.on(event, selector, handler);
        if (!handler) handler = selector;
        addDestrFunc(o, function () {
            tag.off(event, handler);
        }, 'ev', handler);
    }

    function bind2(opt) {
        var handler = opt.handler;
        var event = opt.event;
        var el = opt.el;
        var id = opt.id;
        var o = opt.o;

        if (id && o.desf) {
            unbind(o, handler, id);
        }

        el.on(event, opt.selector, handler);

        addDestrFunc(o, function () {
            el.off(event, handler);
        }, 'ev', handler, id);
    }

    function regDragDrop(dragFromCont, dragFilter, dropContainers, dropFuncs, dragClass,
        hideSource, dropContHoverFuncs, endFunc, resetHover, scrollCnts, wrapFunc, createDragHandleFunc, cancelFunc, nrdh,
        moveFunc, hoverFunc, dropFunc, handleSel, getScrollCont, toFunc) {

        var dragging = 0;
        var dragBase;
        var keepxy;
        resetHover = resetHover || emptyFunc;
        endFunc = endFunc || emptyFunc;
        wrapFunc = wrapFunc || function (cx) { return cx.drgo.clone(); };
        scrollCnts = scrollCnts || [];

        createDragHandleFunc = createDragHandleFunc || function (cx) {
            var dragObj = cx.drgo;
            var dw = dragObj.width();
            var dh = dragObj.height();
            var clss = 'awe-drag ' + safeToStr(dragClass);

            var res = addClass(wrapFunc(cx), clss).hide();

            if (isDirectionRtl(dragFromCont)) {
                dragBase = $('<div style="direction:rtl"/>').appendTo($('body')).append(res);
            } else {
                res.appendTo($('body'));
            }

            res.width(dw).height(dh);
            keepxy = 1;

            return res;
        };

        function onTouchStart(touchStartEvent) {
            var scrollTimer;
            var istouch = touchStartEvent.type !== 'mousedown';
            var amove;
            var handleX;
            var handleY;
            var lasti;

            var startCoords = istouch ? touchStartEvent.originalEvent.touches[0] : touchStartEvent;
            var dragObj = dragFilter ? $(touchStartEvent.target).closest(dragFilter) : dragFromCont;

            var x = startCoords.pageX;
            var y = startCoords.pageY;

            var dxmid = x - dragObj.offset().left;
            var dymid = y - dragObj.offset().top;

            var context = {
                drgo: dragObj,
                from: dragFromCont,
                sel: dragFilter,
                istch: istouch,
                x: x,
                y: y,
                e: touchStartEvent
            };

            if (cancelFunc && cancelFunc(context)) {
                return true;
            }

            if (dragging) return true;

            dragging = 1;

            var moveHappened, allowMove;
            setTimeout(function () { allowMove = 1; }, 300);

            // needed here, otherwise ipad scrolls, android chrome tries to refresh page when dragging down
            // if (istouch) text getting selected on ff and edge without preventing touchStartEvent
            prevDef(touchStartEvent);

            var dropVertx = [];

            if (toFunc) {
                dropContainers = toFunc();
            }

            // fill dropVertx array
            loop(dropContainers, function (dc) {
                if (len(dc)) {
                    var vert = {};
                    vert.Ys = dc.offset().top;
                    vert.Ye = vert.Ys + outerh(dc);
                    vert.Xs = dc.offset().left;
                    vert.Xe = vert.Xs + outerw(dc);

                    dropVertx.push(vert);
                } else {
                    dropVertx.push(nul);
                }
            });

            // check for window cont and set scroll width an scroll height
            loop(scrollCnts, function (sc) {
                if (sc.c.is($window)) {
                    sc.w = 1;
                    var sbw = getScrollBarWidth();
                    var maxScrollHeight = $document.height() - $window.height();
                    var maxScrollWidth = $document.width() - $window.width();

                    if (maxScrollHeight) maxScrollHeight += sbw;
                    if (maxScrollWidth) maxScrollWidth += sbw;

                    sc.sh = maxScrollHeight;
                    sc.sw = maxScrollWidth;
                }
            });

            //var context = { drgo: dragObj, from: dragFromCont, sel: dragFilter };
            var dragHandle = createDragHandleFunc(context);
            context.drgh = dragHandle;

            if (!keepxy) {
                dxmid = startCoords.pageX - dragHandle.offset().left;
                dymid = startCoords.pageY - dragHandle.offset().top;
            }

            var isFixed = dragHandle.css('position') === 'fixed';
            var counter = 0;
            function touchMoveHandler(moveEvent) {
                prevDef(touchStartEvent);

                if (counter > 3 || istouch && allowMove) {
                    var curramove = {};
                    amove = curramove;
                    clearInterval(scrollTimer);

                    moveHappened = 1;
                    addClass(dragHandle, sawedragging);

                    if (hideSource) {
                        dragObj.hide();
                    }

                    var moveCoords = istouch ? moveEvent.originalEvent.touches[0] : moveEvent;
                    x = moveCoords.pageX;
                    y = moveCoords.pageY;
                    context.x = x;
                    context.y = y;

                    var hhres;
                    var handleHover = function () {

                        handleX = x - dxmid;
                        handleY = y - dymid;

                        if (isFixed) {
                            var $win = $(window);
                            handleY -= $win.scrollTop();
                            if (handleY > $win.height())
                                handleY = $win.height() - 20;
                        }

                        if (handleY < 0) handleY = 0;

                        //move handle
                        dragHandle && dragHandle.css({ left: handleX, top: handleY }).show();
                        moveFunc && moveFunc();

                        var resi;
                        for (var i = 0; i < len(dropVertx); i++) {
                            var val = dropVertx[i];
                            if (val && y > val.Ys && y < val.Ye && x > val.Xs && x < val.Xe) {
                                resi = i;
                                break;
                            }
                        }

                        if (lasti !== resi) {
                            resetHover(context);
                        }

                        if (resi >= 0) {
                            if (hoverFunc) {
                                context.cont = dropContainers[resi];
                                hhres = hoverFunc(context); //dropContainers[resi], dragObj, x, y, dragHandle
                            } else if (dropContHoverFuncs) {
                                dropContHoverFuncs[resi] && dropContHoverFuncs[resi](context); //dragObj, x, y, dragHandle
                            }
                        }

                        lasti = resi;
                    };

                    handleHover();

                    var currsconts = scrollCnts;

                    if (getScrollCont) {
                        currsconts = (getScrollCont(hhres) || []).concat(currsconts);
                    }

                    var checkScrollContainers = function (cnts, skip) {
                        var isScrolling;

                        loop(cnts, function (val, i) {
                            if (skip >= i) {
                                return true;
                            }

                            var scrollDistance = 30;
                            var cont = val.c;
                            var ctrlCont = val.p || cont;

                            var isWindow = val.w;
                            var step, osv;

                            var y1 = isWindow ? getScroll($document, 1) : cont.offset().top;
                            var y4 = y1 + outerh(cont);

                            var y2 = y1 + scrollDistance;
                            var y3 = y4 - scrollDistance;

                            var x1 = isWindow ? getScroll($document) : cont.offset().left;
                            var x4 = x1 + outerw(cont);

                            var x2 = x1 + scrollDistance;
                            var x3 = x4 - scrollDistance;
                            var scrollVal = getScroll(cont, val.y);

                            if (val.y) {
                                isScrolling = x > x1 && x < x4 && (scroll(y, y1, y2, 1, 1) || scroll(y, y3, y4, 0, 1));
                            } else if (val.x) {
                                isScrolling = y > y1 && y < y4 && (scroll(x, x1, x2, 1) || scroll(x, x3, x4));
                            }

                            // stop loop, scroll in progress
                            if (isScrolling) return false;

                            function scroll(c, c1, c2, up, vert) {
                                if (c > c1 && c < c2) {
                                    step = round((up ? c2 - c : c - c1) / scrollDistance * 15);

                                    var scrollLimit = vert ? val.sh : val.sw;
                                    if (up) step *= -1;

                                    var scrollNow = function () {
                                        if (amove && (!isWindow || (up ? scrollVal > Math.abs(step) : scrollVal + step < scrollLimit))) {

                                            scrollVal += step;

                                            setScroll(ctrlCont, scrollVal, vert);

                                            var nsv = getScroll(ctrlCont, vert);

                                            if (isWindow) {
                                                if (vert) y += step;
                                                else x += step;
                                            }

                                            handleHover();

                                            setScroll(ctrlCont, nsv, vert);

                                            if (nsv === osv) {
                                                clearInterval(scrollTimer);
                                                // go check next scrollCont
                                                checkScrollContainers(cnts, i);
                                            } else {
                                                startScroll();
                                            }

                                            osv = nsv;
                                        }
                                    };

                                    var startScroll = function () {
                                        if (amove === curramove) {
                                            setTimeout(scrollNow, 10);
                                        }
                                    };

                                    startScroll();

                                    return 1;
                                }
                            }
                        });
                    };

                    checkScrollContainers(currsconts);

                } else {
                    counter++;
                }
            }

            $document
                .on(istouch ? 'touchmove.awenpd' : smousemove, touchMoveHandler)
                .one(istouch ? 'touchend.awenpd' : 'mouseup', function () {
                    $document.off('touchmove.awenpd ' + smousemove, touchMoveHandler);
                    clearInterval(scrollTimer);
                    amove = nul;

                    // to keep the class a bit more after drag complete, for click event to see
                    setTimeout(function () {
                        removeClass(dragHandle, sawedragging);
                        dragging = 0;
                    }, 1);

                    !nrdh && dragHandle && dragHandle.remove();
                    dragBase && dragBase.remove();

                    if (isNotNull(lasti)) {
                        if (dropFunc) {
                            context.cont = dropContainers[lasti];
                            dropFunc(context); //dropContainers[lasti], dragObj, x, y
                        } else {
                            dropFuncs[lasti] && dropFuncs[lasti](context);//dragObj, x, y
                        }
                    }

                    resetHover(context);
                    endFunc(context);

                    if (hideSource && !context.noSrcShow) dragObj.show();

                    if (!moveHappened && istouch) {
                        $(touchStartEvent.target).trigger(sclick);
                    }
                });
        }

        dragFromCont.on(touchstartmousedown, handleSel || dragFilter, onTouchStart);

        return function () {
            dragFromCont.off(touchstartmousedown, onTouchStart);
        };
    }

    function numeric(o, noFocusState) {
        var editor = o.d;
        var step = o.stp;
        var sep = o.sep;

        var op = 1;
        var mdIncInterval;
        var mdStartIncTimeout;
        var mouseup = 1;
        var lastclicktype;
        var incUsed;

        editor.keydown(getNumericKeyDown(o, editor, sep, inc));
        editor.keydown(function (e) {
            if (incUsed && e.which === 13) {
                awef.trigger(editor, schange);
            }
        });

        if (o.v) {
            editor.on('input', function () {
                enforceEditorNumber(editor);
            });
        }

        var changeTimeout;

        function inc(oop) {
            incUsed = 1;
            oop = oop || op;

            if (o.inc) {
                o.inc(oop);
            } else {
                var val = getNumericValStr(step * oop, 0, o.v);
                editor.val(val).keyup();
                awef.trigger(editor, 'input');

                clearTimeout(changeTimeout);
                changeTimeout = setTimeout(function () {
                    awef.trigger(editor, schange);
                }, 250);
            }
        }

        function enforceEditorNumber() {
            var sval = getFloatStr(editor.val());
            var nval = Number(sval);

            if (isNaN(nval) || sval.indexOf('x') > -1) {

                nval = Number(getFloatStr(o.v.val()));
                editor.val(nval);
            }

            return nval;
        }

        function getNumericValStr(addVal, fractLen1, input, init) {
            var val = getFloatStr(input.val());
            var floatVal = input === o.v ? Number(val) : enforceEditorNumber();

            var fractLen = fractLen1 || max(getFractLen(val), getFractLen(step.toString()));
            if (init && fractLen < o.dec) {
                fractLen = o.dec;
            }

            if (!init && isNull(o.dec)) fractLen = 0;

            if (addVal) floatVal += addVal;

            // on render you need to show the real value
            if (!init) {
                if (isNotNull(o.min)) floatVal = max(floatVal, o.min);
                if (isNotNull(o.max)) floatVal = min(floatVal, o.max);
            }

            return floatVal.toFixed(fractLen).replace(dot, sep);

            function getFractLen(pval) {
                var sepPosition = pval.indexOf(dot) + 1;
                return sepPosition ? len(pval) - sepPosition : 0;
            }
        }

        function getFloatStr(str) {
            var pval = str.replace(',', dot);
            if (pval === dot || pval === estr) pval = '0';
            return pval;
        }

        function startInc() {
            if (!mouseup) {
                mdIncInterval = setInterval(inc, 50);
            }
        }

        function incAndStartTime(etype, noFocusOnSpin) {
            // can be touchstart or mousedown
            if (lastclicktype === etype || !lastclicktype) {
                lastclicktype = etype;
                mouseup = 0;

                setTimeout(function () {
                    if (!editor.is(':focus') && !noFocusOnSpin) { tfocus(editor); }
                    inc();
                });

                if (!(mdStartIncTimeout + mdIncInterval)) {
                    mdStartIncTimeout = setTimeout(startInc, 200);
                    $document.one('mouseup touchend.awenpd touchcancel',
                        function () {
                            mouseup = 1;

                            clearTimeout(mdStartIncTimeout);
                            clearInterval(mdIncInterval);
                            mdStartIncTimeout = 0;
                            mdIncInterval = 0;

                            if (!editor.is(':focus') && !noFocusOnSpin) { tfocus(editor); }
                        });
                }
            } else {
                lastclicktype = estr;
            }
        }

        // spin buttons
        if (o.ss) {
            o.f.find('.awe-spinup')
                .on(touchstartmousedownNpd, function (e) {
                    op = 1;
                    incAndStartTime(e.type, o.noFocusOnSpin);
                });

            o.f.find('.awe-spindown')
                .on(touchstartmousedownNpd, function (e) {
                    op = -1;
                    incAndStartTime(e.type, o.noFocusOnSpin);
                });
        }

        if (noFocusState) return;

        var focusreg;
        var chVal = o.v.val();
        var api = o.api;
        api.foh = emptyFunc;
        api.render = function () {
            var sval = o.v.val() ? getNumericValStr(0, 0, o.v, 1) : estr;

            editor.val(o.ff(sval));
        };

        // display change
        api.dch = function () {
            var val = editor.val() ? getNumericValStr(0, o.dec, editor) : estr;
            val = trimFloatZeroesDot(val);
            o.v.val(val);

            if (val !== chVal) {
                chVal = val;
                ach(o, { awe: 1 });
            }

            incUsed = 0;

            function trimFloatZeroesDot(str) {
                if (str.indexOf(sep) === -1) return str;
                var i;
                for (i = len(str) - 1; i > 0; i--) {
                    if (str[i] === '0') {
                        /*empty*/
                    } else {
                        break;
                    }
                }

                if (str[i] === sep) i--;

                var res = str.substring(0, i + 1);
                return res;
            }
        };

        api.fih = function () {
            if (!focusreg) {
                focusreg = 1;
                editor.val(o.v.val());
                $document.on(sfocusin + ws + sclick, focusoutHandler);
            }
        };

        function focusoutHandler(e) {
            if (!istrg(e, o.f) && !$(':focus').is(editor)) {
                $document.off(sfocusin + ws + sclick, focusoutHandler);
                focusreg = 0;
                tchange(editor);
                var sval = o.v.val() ? getNumericValStr(0, 0, o.v, 1) : estr;
                editor.val(o.ff(sval));
            }
        }
    }

    function popupCreateOpenLoad(o, midFunc) {
        var scon = o.scon;
        var popup = o.d;

        midFunc && midFunc(popup); // for popupForm reg submit, default buttons

        // create open load popup
        loop(o.btns, function (btn) {
            btn.action = gfunc(btn.action);
        });

        makePopup(o);

        var api = dapi(popup);
        function loadContent(content, donotset) {
            if (!dapi(popup)) return; // popup was removed before loading its content
            if (!donotset) scon.html(content);
            api.ld = 1;
            api.lay && api.lay();
            
            focusfirst(o);
            if (o.load) gfunc(o.load).call(o);
        }

        dapi(popup).open(o);

        if (o.content) {
            loadContent(o.content);
        }
        else if (o.url || o.dataFunc || o.setCont) {
            var sd = makeParams(o);

            var $loadStr = $(loadingStr);
            scon.html($loadStr);

            popup.trigger(sawebeginload, [sd]);

            getUrlOrFunc2({
                o: o,
                type: 'get',
                url: o.url,
                func: o.dataFunc || o.setCont,
                data: sd,
                success: function (content) {
                    loadContent(content, o.setCont);
                },
                complete: function () {
                    $loadStr.remove();
                    popup.trigger(saweload);
                }
            });
        }

        return popup;
    }

    function destroyExPopup(id) {
        if (id) {
            var popup = $('#' + id);
            if (len(popup)) {
                dapi(popup).destroy();
            }
        }
    }

    function makePopup(o) {
        var pp = o;
        var popup = pp.d;

        !pp.div && destroyExPopup(pp.id);

        //popup.data('o', o);

        if (pp.g) {
            $('.awe-popup[aweg=' + pp.g + ']').each(function () {
                var api = dapi($(this));
                if (api)
                    api.destroy();
            });
            popup.attr('aweg', pp.g);
        }

        addClass(popup, 'awe-popup');

        if (pp.id) popup.attr('id', pp.id);

        awe.popup(o);

        addDestrFunc(o, function () {
            pp.api && pp.api.destroy();
        });
    }

    function getNumericKeyDown(o, editor, sep, upDownFunc) {
        return function (e) {
            var key = e.which;
            var ek = e.key;
            var el = editor[0];
            var val = editor.val();
            var sel = getTextSelection(el);
            var selection = val.slice(sel.s, sel.e);
            var minus = '-';
            // up arrow
            if (key === 38) {
                upDownFunc(1);
            }
            // down arrow
            else if (key === 40) {
                upDownFunc(-1);
                // 188 = , ; 190 = . 110 = numpad .
            } else if (key === 188 && sep == ',' || (key === 190 || key === 110) && sep == '.') {
                prevDef(e);
                if (isNotNull(o.dec) && (val.indexOf(sep) === -1 || selection.indexOf(sep) > -1)) {
                    val = val.slice(0, sel.s) + sep + val.slice(sel.e);
                    editor.val(val);
                    setCaretPosition(el, sel.s + 1);
                }
            }
            // minus 
            else if (ek === minus) {
                if (val.indexOf(minus) > -1 && selection.indexOf(minus) === -1 || sel.s > 0 || o.min >= 0) {
                    prevDef(e);
                }
            }
            else {
                awe.pnn(e, []);
            }
        };
    }

    function getScrollBarWidth(grid) {
        var inner = document.createElement('p');
        inner.style.width = "100%";
        inner.style.height = "200px";

        var pouter = document.createElement('div');
        var outer = document.createElement('div');

        if (grid) {
            pouter.className = grid.attr('class');
        }

        var os = outer.style;
        os.position = "absolute";
        os.top = "0px";
        os.left = "0px";
        os.visibility = "hidden";
        os.width = "200px";
        os.height = "150px";
        os.overflow = "hidden";

        outer.appendChild(inner);
        pouter.appendChild(outer);

        document.body.appendChild(pouter);
        var w1 = inner.offsetWidth;
        os.overflow = 'scroll';
        var w2 = inner.offsetWidth;
        if (w1 === w2) w2 = outer.clientWidth;
        document.body.removeChild(pouter);

        return w1 - w2;
    }

    function regColumnsResize(o) {
        var resizing;
        // small div, resize cursor on hover, mousedown will start resize
        var resizeHandle;
        var gridHeadtbody = o.hrow;
        var colgroups = o.colgroups;
        var hcon = o.hcon;

        bind2({
            o: o,
            el: gridHeadtbody,
            event: smousemove,
            selector: 'td:not(.awe-idn)',
            handler: handler,
        });

        function handler(emove) {
            var isRtl = o.rtl;
            var columns = o.columns;
            var hovTd = trg(emove).closest('td');
            var resColIndex = hovTd.data('i');
            var left = hovTd.offset().left;
            var cellWidth = outerw(hovTd);
            var hx = left - 5;

            if (emove.pageX > cellWidth / 2 + left || hovTd.hasClass('o-fzrb')) {
                hx += cellWidth;
                if (resColIndex === len(columns) - 1 && !isRtl) hx -= 5;
                if (isRtl) resColIndex--;
            } else {
                if (!isRtl) resColIndex--;
            }

            var col = columns[resColIndex];
            if (isNull(col) || !columns[resColIndex].R) return;

            if (!resizeHandle) {
                resizeHandle = $('<div class="awe-resh"/>').appendTo("body");

                // remove resize handle when not resizing or cursor outside  grid header
                var handleMouseMove = function (ev) {
                    var target = $(ev.target);
                    if (target.is(resizeHandle) || len(target.closest('.awe-header')) || resizing) {
                        /* empty */
                    } else {
                        destr();
                    }
                };

                $document.on(smousemove, handleMouseMove);

                // start resizing
                resizeHandle.on('mousedown', function (e) {
                    prevDef(e);
                    columns = o.columns; // blazor on click to sort o.columns is updated

                    var resColIndex = $(this).width(20).data('i');
                    resizing = 1;

                    var rescolg = colgroups.find('[data-i=' + resColIndex + ']');
                    var x0 = e.pageX;
                    var othersChanged = 0;
                    var otherWidthlessColumns = 0;
                    var resizeColumn = columns[resColIndex];
                    var prevcolw = resizeColumn.W;
                    var gridColsMinWidth = idnw * (o.th + o.gl);
                    var resColAbsWidth = outerw(gridHeadtbody.find('[data-i=' + resColIndex + ']:not(.awe-hgc)'));
                    var resColMinResWidth = resizeColumn.Mw || o.cw;

                    // get grid width
                    loop(columns, function (col, colIndex) {
                        if (isColHid(o, col)) return;
                        gridColsMinWidth += col.needw;

                        if (colIndex !== resColIndex && !col.W) {
                            otherWidthlessColumns = 1;
                        }
                    });

                    // for first column add idn cols w
                    var colspanw = !resColIndex ? o.th * idnw : 0;

                    if (!prevcolw) {
                        prevcolw = resColAbsWidth;
                        gridColsMinWidth += prevcolw - (resColMinResWidth + o.th * idnw);
                    }
                    else {
                        prevcolw += colspanw;
                    }

                    var conw = hcon.width();

                    var mousemoveh = function (ev) {
                        if (ev.pageX < $document.width() - 10) {
                            resizeHandle.css({ left: ev.pageX - 10, top: ev.pageY - 10 });
                        }

                        var x1 = ev.pageX;
                        var delta = x1 - x0;

                        if (isRtl) {
                            delta = delta * -1;
                        }

                        var newColumnWidth;
                        if (!otherWidthlessColumns) {
                            var othersaw = conw - (resColAbsWidth + delta);
                            var otherspxw = gridColsMinWidth - prevcolw;
                            if (othersaw < otherspxw) othersaw = otherspxw;
                            var k = otherspxw / othersaw || 1;
                            newColumnWidth = (resColAbsWidth + delta) * k;
                        } else {
                            newColumnWidth = prevcolw + delta;
                        }

                        if (newColumnWidth < 1) newColumnWidth = 1;

                        var coef = 1;
                        if (newColumnWidth < resColMinResWidth) {
                            coef = resColMinResWidth / newColumnWidth;
                            newColumnWidth = newColumnWidth * coef;
                        }

                        newColumnWidth -= colspanw;

                        resizeColumn.W = newColumnWidth;

                        var idns = idnw * (o.th + o.gl);
                        var predictedWidth = (gridColsMinWidth - (idns + (prevcolw - colspanw))) * coef + newColumnWidth + idns;

                        if (!otherWidthlessColumns && (coef > 1 || othersChanged) && conw >= predictedWidth + 5) {
                            loop(columns, function (column, colIndex) {
                                if (colIndex !== resColIndex && columns[colIndex].R && !isColHid(o, column)) {
                                    if (column.W) {
                                        if (!othersChanged) {
                                            column.ciw = column.W;
                                        }

                                        var ncolw = column.ciw * coef;

                                        column.W = ncolw;
                                    }
                                }
                            });

                            othersChanged = 1;
                            if (coef <= 1) othersChanged = 0;
                        }

                        setColumnsDimensions(o);
                        rescolg.trigger('awecolresize');
                    };

                    $document.on(smousemove, mousemoveh);
                    $document.one('mouseup', function () {
                        resizing = 0;
                        $document.off(smousemove, mousemoveh);
                        resizeHandle.width(10);

                        o.persist && o.persist();

                        rescolg.trigger('awecolresizeend');
                    });
                });
            }

            resizeHandle.data('i', resColIndex)
                .css({ left: hx, top: hovTd.offset().top, height: outerh(hovTd) });

            function destr() {
                $document.off(smousemove, handleMouseMove);
                resizeHandle && resizeHandle.remove();
                resizeHandle = nul;
            }
        }
    }

    function regColumnsGroupReorder(opt) {
        var reh, currenti, canGroup, canReorder;
        var $body = $('body');

        var gbar = opt.gbar;
        var hcon = opt.hcon;
        var hrow = opt.hrow;
        var isRtl = opt.rtl;

        function wrap(context) {
            var dragObj = context.drgo;

            reh = opt.v.find('.awe-reh').clone().appendTo($body);
            canGroup = dragObj.hasClass(sawegroupable);
            canReorder = dragObj.hasClass('awe-rer');

            currenti = dragObj.data("i");
            return $('<table/>').append($('<tr/>').append(dragObj.clone()));
        }

        function onEnd() {
            reh.remove();
        }

        function onResetHover() {
            removeClass(gbar, 'awe-highlight');
            reh.hide();
        }

        function onHeaderHover(context) {
            var x = context.x;

            if (!canReorder) return;

            // determine hovered column using just x
            var hoveri = -1;
            var hoverCol;
            var colOffset;
            var left;

            var rb = hrow.find('.o-fzrb:first');
            var rbi, rbleft = null;
            if (rb.length) {
                rbleft = rb.offset().left;
                rbi = rb.data('i');
            }

            let headerCols = hrow.find('.awe-hc')
                .get()
                .sort(function (a, b) {
                    return $(a).data('i') - $(b).data('i');
                });

            loop(headerCols, function (val) {
                var col = $(val);
                colOffset = col.offset();
                left = colOffset.left;

                if (rbleft) {
                    if (col.data('i') === rbi) {
                        rbleft = null;
                    }
                    else if (left + outerw(col) - 1 > rbleft && !isRtl || isRtl && left - outerw(col) < rbleft) { // -1 or rbleft will be .5 > than its previous column
                        // column under frozen column
                        return true;
                    }
                }

                if (x > left && x < left + outerw(col)) {
                    hoverCol = col;
                    hoveri = hoverCol.data('i');
                    return false;
                }
            });

            if (hoverCol) {
                left -= 5;

                if (currenti < hoveri && !isRtl || isRtl && currenti > hoveri) {
                    left += outerw(hoverCol);
                }

                if (currenti !== hoveri && hoverCol.hasClass('awe-rer')) {
                    reh.css({ left: left, top: colOffset.top + 1, height: outerh(hoverCol) + 1 })
                        .show();
                    return hoveri;
                } else {
                    onResetHover();
                }
            } else {
                onResetHover();
            }
        } // end func onHeaderhover

        function onHeaderDrop(context) {
            if (canReorder) {
                var hoveri = onHeaderHover(context);
                if (isNotNull(hoveri)) {
                    opt.hdrop({
                        hoveri: hoveri,
                        currenti: currenti
                    });
                }
            }
        }

        function onGroupHover() {
            if (canGroup) {
                addClass(gbar, 'awe-highlight');
            }
        }

        function onGroupDrop(context) {
            if (canGroup) {
                opt.gdrop(context);
            }
        }

        function hconDragCancel() {
            if (hcon.css('overflow') === 'hidden') return emptyFunc;
            return dragCancelFunc(hcon);
        }

        return regDragDrop(hrow, '.' + sawegroupable + ', .awe-rer', [gbar, hrow], [onGroupDrop, onHeaderDrop], 'awe-dcol', nul,
            [onGroupHover, onHeaderHover], onEnd, onResetHover,
            [{ c: hcon, x: 1, p: opt.syncon ? opt.cont : hcon }, { c: $window, x: 1 }], wrap, nul, hconDragCancel());
    }

    function mainGridLay(o) {
        var gdiv = o.v;
        var content = o.cont;
        var gridColumnsHeader = o.header;
        var frozenRowsContent = o.gfc;

        // to set correct height
        if (!gdiv.is(':visible')) return;

        var isRtl = isDirectionRtl(gdiv);
        addClass(gdiv, isRtl ? 'awe-rtl' : 'awe-ltr');

        o.rtl = isRtl;

        // get scrollbar side
        var d = $("<div></div>");
        d.css("overflow-y", "scroll");
        d.css('position', 'relative');
        d.append('<p/>');
        gdiv.append(d);
        var x = d.find('p').position().left;
        d.remove();

        var sides = ["left", "right"];

        if (x < 3) {
            sides.reverse();
        }

        var sbw = content.width() - content.children().width();
        var paddingValue = (sbw > 0 ? sbw : getScrollBarWidth(gdiv)) + 'px';
        var spadding = 'padding-';
        var padding = spadding + sides[0];
        var rempadding = spadding + sides[1];

        frozenRowsContent && frozenRowsContent.css('margin-bottom', '-' + paddingValue);

        var contentHeight = o.conth;
        var gheight = o.h;

        var scrollable = gheight || contentHeight || sbw;

        if (!scrollable) paddingValue = estr;

        gdiv.css(sheight, gheight || estr);

        if (contentHeight) {
            content.parent().css(sheight, o.contenth);
        }

        // set padding depending on vertical scrollbar side
        if (len(gridColumnsHeader)) {
            gridColumnsHeader.css(padding, paddingValue)
                .css(rempadding, estr);

            if (!scrollable)
                gridColumnsHeader.css('padding-inline', 0);
        }

        frozenRowsContent && frozenRowsContent.css(padding, paddingValue)
            .css(rempadding, estr);

        // if there is height we need to set a scrollbar
        if (scrollable) {
            content.css('overflow-y', 'scroll');
        } else {
            content.css('overflow-y', estr);
            if (o.mh) {
                gdiv.css('min-height', o.mh);
            }
        }
    }

    function regSyncHorizScroll(opt) {
        var content = opt.cont;
        var hcon = opt.hcon;
        var fztablc = opt.fztablc;

        // for columns drag and drop scroll
        // no infin loop, probably because scroll is not handled if scroll value hasn't changed
        hcon.on('scroll', function () {
            var sl = getScroll(hcon);
            setScroll(content, sl);
            fztablc && setScroll(fztablc, sl);
        });

        content.on('scroll', function () {
            var sl = getScroll(content);
            setScroll(hcon, sl);
            fztablc && setScroll(fztablc, sl);
        });

        fztablc && fztablc.on('scroll', function () {
            var sl = getScroll(fztablc);
            setScroll(hcon, sl);
            setScroll(content, sl);
        });
    }

    // calc cols with based on available with and MinWidth, Grow properties
    // set column needw, awidth
    function calcColsWidths(o) {
        // awidth - calculated width, based on MinWidth or Grow the column will use instead of W
        // needw - min width the column will occupy

        var content = o.cont;

        var freeColumns = []; // cols without width defined
        var hasMinWidth;

        var contentWidth = content.prop("clientWidth"); // available width for minwidth and nowidth/nogrow defined columns
        var growTotal = 0;
        var remainWForGrow = contentWidth;

        loop(o.columns, function (col) {
            col.awidth = 0; // remove value added on previous initLayout calls
            if (isColHid(o, col)) return;

            growTotal += col.Grow || (col.W ? 0 : 1);

            // ignore minWidth smaller than cw
            //if (col.Mw < o.cw) col.Mw = 0; // can't resize back to initial w

            // min w the col will take
            var needw = col.W || col.Mw || o.cw;
            col.needw = needw;

            remainWForGrow -= needw;

            if (col.W) {
                contentWidth -= col.W;
            } else {
                if (col.Mw) {
                    hasMinWidth = 1;
                }

                freeColumns.push(col);
            }
        });


        var indentWidth = (o.gl + o.th) * idnw;

        remainWForGrow -= indentWidth;

        o.colGrowTotal = growTotal;
        o.remainWForGrow = remainWForGrow;

        // set col.awidth for columns with Grow if there is space
        if (remainWForGrow > 20 && len(freeColumns) > 1) {
            var freeColumnsAfterGrow = [];
            loop(freeColumns, function (col) {
                if (col.Grow > 0) {
                    col.awidth = Math.floor(col.needw + (col.Grow * remainWForGrow) / growTotal);
                    contentWidth -= col.awidth;
                } else {
                    freeColumnsAfterGrow.push(col);
                }
            });

            freeColumns = freeColumnsAfterGrow;
        }

        // set awidth to cols with MinWidth greater than available
        if (hasMinWidth) {
            // remove group indentation columns width
            contentWidth -= indentWidth;

            // sort by mw val desc
            freeColumns.sort(function (x, y) { return y.needw - x.needw; });

            loop(freeColumns,
                function (col) {
                    if (col.needw > o.cw && col.needw > contentWidth / len(freeColumns)) {
                        col.awidth = col.needw;
                        contentWidth -= col.needw;
                    } else {
                        return false;
                    }
                });
        }

        return indentWidth;
    }

    function isColHid(o, col) {
        return !o.sgc && col.Gd || col.Hid;
    }

    function open(name, openCfg, event) {
        //{cfg}, event        
        if (typeof name != 'string') {
            event = openCfg;
            openCfg = name;
        }
        // name, {cfg}, event
        else {
            openCfg = openCfg || {};
            openCfg.id = name;
        }

        if (openCfg.div || !openCfg.id) {
            var aid = $(openCfg.div).data('aid');
            if (!aid) {
                aid = openCfg.id || ('awep' + popId++);
            }

            openCfg.id = aid;
            $(openCfg.div).data('aid', aid);
        }

        var cfg = openCfg; // current cfg

        if (openCfg.id) {
            var initCfg = storage[openCfg.id + '-ip'];

            if (isNotNull(initCfg)) {
                cfg = $.extend(true, {}, initCfg, openCfg);
                cfg.desf = []; // rem prev desf which is for -ip cfg obj
            }

            var exo = storage[cfg.id];
            if (exo && exo.loadOnce) {
                cfg.cx = exo.cx;
                cfg.d = exo.d;
            } else {
                addDestrFunc(cfg, function () {
                    delete storage[cfg.id];

                    awe.destroyCont(cfg.d);
                });
            }

            storage[cfg.id] = cfg;
        }

        var popup = cfg.d || (cfg.div ? $(cfg.div).show() : createPopupDiv());
        cfg.d = popup;

        popup.data('o', cfg);
        adata(popup, 'o', cfg);
        cfg.scon = popup.find('.awe-scon');

        if (cfg.type === "pf" || cfg.type === 'popupForm') {

            if (isNull(cfg.closeOnSuccess)) {
                cfg.closeOnSuccess = 1;
            }

            return awe.pf(event, cfg);
        } else {
            return awe.op(event, cfg);
        }
    }

    function op(e, o) {
        prevDef(e);
        o.e = e;

        if (o.cx) {
            o.cx.api.open(o);
        }
        else {
            popupCreateOpenLoad(o);
        }

        if (!o.noret) return o.d;
    }

    function safeToStr(v) {
        return isNull(v) ? estr : v.toString();
    }

    function getScroll($o, vert) {
        return vert ? $o.scrollTop() : $o.scrollLeft();
    }

    function setScroll($o, val, vert) {
        if (vert) $o.scrollTop(val);
        else $o.scrollLeft(val);
    }

    function getTextSelection(inputBox) {
        if ("selectionStart" in inputBox) {
            return {
                s: inputBox.selectionStart,
                e: inputBox.selectionEnd
            };
        }

        //and now, the blinkered IE way
        var bookmark = document.selection.createRange().getBookmark();
        var selection = inputBox.createTextRange();
        selection.moveToBookmark(bookmark);

        var before = inputBox.createTextRange();
        before.collapse(true);
        before.setEndPoint("EndToStart", selection);

        var beforeLength = len(before.text);
        var selLength = len(selection.text);

        return {
            s: beforeLength,
            e: beforeLength + selLength
        };
    }

    function preventNonNumbers(ev, oths) {
        var ekey = ev.key;
        if (ev.ctrlKey || oths.indexOf(ev.which) + 1 || !isNaN(Number(ekey)) || len(ekey) > 1)
            return;

        prevDef(ev);
    }

    function setColumnsDimensions(o) {
        var indentWidth = (o.gl + o.th) * idnw;
        var contentWidth = indentWidth;

        calcColsWidths(o);

        loop(o.columns, function (col, i) {
            if (isColHid(o, col)) return;
            var w = col.W || col.awidth || 0;
            o.colgroups.find('col[data-i=' + i + ']').css('width', w ? w : estr);

            contentWidth += w || col.needw;
        });

        o.colswrap.css('min-width', contentWidth + 'px');
        o.gtableswraps.css('min-width', contentWidth + 'px');
    }
    //#endregion core sync

    //#region awem sync
    function initWave() {
        if ($doc.data('awewave')) return;
        $doc.data('awewave', 1);
        $(function () {
            $doc.on('mousedown touchstart.awenpd', '.awe-btn, .awe-tab-btn, .o-wavs, .awe-sortable.awe-hc, .awe-sortable.awe-col, .o-chk', function (e) {
                var time = 700;
                var btn = $(this);
                if (btn.is(':disabled') || btn.closest('.awe-disabled').length || btn.closest('.nowave').length) return;

                var wc = $('<div class="o-wavc" tabindex="-1"/>');
                var wav = $('<div class="o-wav" tabindex="-1"/>');
                wc.append(wav);

                if (isMobile()) {
                    wc.addClass('o-mobl');
                }

                var csize;
                var istouch = e.type !== 'mousedown';
                var isCol = 0;

                if (istouch) {
                    btn.data('awewvtch', 1);
                    setTimeout(function () { btn.data('awewvtch', 0); }, 330);
                }

                if (btn.data('awewvtch') && !istouch) return;

                var startCoords = istouch ? e.originalEvent.touches[0] : e;

                var x = startCoords.pageX - btn.offset().left;
                var y = startCoords.pageY - btn.offset().top;
                var w = outerw(btn);
                var h = outerh(btn);

                if (btn.is('.o-chk')) {
                    time = 350;
                    csize = w = h = 57;
                    x = y = csize / 2;
                    var marg = csize / 2 - outerw(btn) / 2;
                    wc.css('top', -marg);
                    wc.css('left', -marg);
                }

                // ff col click
                if (btn.is('.awe-sortable .awe-col')) {
                    btn = btn.parent();
                    isCol = 1;
                }

                mouseup();

                if (btn.closest('.awe-groupable').length) {
                    var uprls;
                    var moved;
                    $doc
                        .one('mouseup touchend',
                            function () {
                                uprls = 1;
                            })
                        .one('mousemove touchmove', function () {
                            moved = 1;
                        });

                    setTimeout(function () {
                        if (!uprls && moved) {
                            wc.remove();
                        }
                    },
                        200);
                }

                function mouseup() {
                    wc.width(w).height(isCol ? '100%' : h);
                    var size = Math.max(w, h);

                    wav.css('left', x);
                    wav.css('top', y);
                    wav.width(20).height(20);

                    btn.append(wc);
                    size = csize || Math.max(size * 2, 100);
                    wav.width(size).height(size);

                    wav.css('opacity', 0);
                    setTimeout(function () {
                        wc.remove();
                    }, time);
                }
            });
        });
    }

    function ifnul(val, dval) {
        return isNull(val) ? dval : val;
    }

    function ypos(o) {
        return o.offset().top;
    }

    function scrollTopBy(con, val) {
        con.scrollTop(con.scrollTop() + val);
    }

    function contains(key, keys) {
        return keys.indexOf(key) > -1;
    }

    function outerhn(sel, m) {
        return sel.length ? outerh(sel, m) : 0;
    }

    function which(e) {
        return e.which;
    }

    function isPosFixed(elm) {
        if (!elm || !elm.length || elm.is('body')) return 0;

        if (elm.css('position') === 'fixed') {
            return 1;
        }

        return isPosFixed(elm.parent());
    }

    function getZIndex(el) {
        var val = el.css(szindex);
        if (val && val > 0) return val;
        var parent = el.parent();
        return parent && !parent.is($('body')) ? getZIndex(parent) : 0;
    }

    function calcZIndex(zIndex, el) {
        if (zIndex < minZindex) zIndex = minZindex;
        var zi = getZIndex(el);
        if (zi && zi > zIndex) {
            zIndex = zi;
        }

        return zIndex;
    }

    function slist(cont, opt) {
        var itemsel = opt.sel;
        var onenter = opt.enter;
        var focuscls = opt.fcls || sfocus;
        var selcls = opt.sc || sselected;
        var afs = opt.afs;

        function visf(sel) {
            return sel.filter(':visible').first();
        }

        function focus(item) {
            remFocus();
            item.addClass(focuscls);
        }

        function remFocus() {
            cont.find('.' + focuscls).removeClass(focuscls);
        }

        function scrllTo(to, cangomid, noWinScroll) {
            if (!to.length || !to.is(':visible')) return;

            var ty = ypos(to);
            var th = outerh(to);
            var conh = cont.height();
            var miny = ypos(cont);
            var maxy = miny + conh - th;

            var scrCont = cont;
            var winmax = $win.height() + $doc.scrollTop() - th;
            var winmin = $doc.scrollTop();

            if (!noWinScroll) {
                if (maxy > winmax && winmax < ty) {
                    maxy = winmax;
                    scrCont = $win;
                }

                if (miny < winmin && winmin > ty) {
                    miny = winmin;
                    scrCont = $win;
                }
            }

            var delta = ty < miny ? ty - miny : ty > maxy ? ty - maxy : 0;

            // +1 for ie and ff 
            if (cangomid && delta > th + 1 && scrCont !== $win) {
                delta += conh / 2;
            }

            scrollTopBy(scrCont, delta);
        }

        function scrollToFocused(cangomid, noWinScroll) {
            scrllTo(cont.find('.' + focuscls), cangomid, noWinScroll);
        }

        function autofocus($itemToFocus, noWinScroll) {
            if ($itemToFocus) {
                focus($itemToFocus);
            } else {
                var $selected = cont.find('.' + selcls + ':visible');
                if ($selected.length === 1) {
                    focus($selected);
                } else {
                    while (1) {
                        if (afs) {
                            var safs = visf(cont.find(afs));
                            if (safs.length) {
                                focus(safs);
                                break;
                            }
                        }

                        focus(visf(cont.find(itemsel)));
                        break;
                    }
                }
            }

            scrollToFocused(1, noWinScroll);
        }

        function handleMoveSelectKeys(e) {
            var key = which(e);

            var focused = gfocus();

            var select = function (item, f) {
                if (!focused.length) {
                    autofocus();
                }
                else if (item.length) {
                    focus(item);
                    scrollToFocused();
                } else if (f) {
                    f();
                }
            };

            if (contains(key, controlKeys)) {
                if (key === keyDown) {
                    prevDef(e);
                    select(visf(focused.nextAll(itemsel)), opt.botf);
                } else if (key === keyUp) {
                    prevDef(e);
                    select(visf(focused.prevAll(itemsel)), opt.topf);
                } else if (key === keyEnter) {
                    if (onenter) {
                        onenter(e, focused);
                    }
                    else {
                        var alink = focused.find('a');
                        if (len(alink)) {
                            prevDef(e);
                            tclick(alink[0]);
                        }
                        else {
                            if (len(focused)) {
                                prevDef(e);
                                tclick(focused);
                            }
                        }
                    }
                }

                return 1;
            }

            return 0;
        }

        function gfocus() {
            return cont.find('.' + focuscls);
        }

        return {
            gfocus: gfocus,
            focus: focus,
            scrollToFocused: scrollToFocused,
            scrollTo: scrllTo,
            keyh: handleMoveSelectKeys,
            autofocus: autofocus,
            remf: remFocus
        };
    }

    function dragAndDrop(opt) {
        var dropContainers = [];
        var dropFuncs = [];
        var dropHoverFuncs = [];

        loop(opt.to, function (val) {
            dropContainers.push(val.c);
            dropHoverFuncs.push(val.hover);
            dropFuncs.push(val.drop);
        });

        var dropConts = opt.dropConts;

        if (opt.dropSel || dropConts) {
            opt.tof = function () {
                if (!opt.dropSel)
                    return dropConts;

                if (!dropConts)
                    return $(opt.dropSel).map(function (i, el) { return $(el); }).get();

                var res = [];
                loop(dropConts, function (dc) {
                    res = res.concat(dc.find(opt.dropSel).map(function (i, el) { return $(el); }).get());
                });

                return res;
            };
        }

        var cancelFunc = opt.cancel || function (cx) {
            var dragItm = $(cx.e.target).closest('.o-dragItm');
            if (len(dragItm) && !dragItm.is(cx.sel)) return true;
        };

        return regDragDrop(opt.from, opt.sel, dropContainers, dropFuncs, opt.dragClass, opt.hide, dropHoverFuncs, opt.end,
            opt.reshov, opt.scroll, opt.wrap, opt.ch, cancelFunc, opt.kdh, opt.move, opt.hover, opt.drop, opt.handle, opt.gscroll, opt.tof);
    }

    function dragReor(opt) {
        var placeh;
        var plhclss = opt.plh || 'o-plh';
        var sel = opt.sel;
        var handle = opt.handle;
        var lastItemHovered;
        var fromCont = opt.from;
        var previ;
        var ondrop = opt.ondrop || opt.onDrop;
        var noPlaceh = opt.noPlh;
        // new obj func, obj to be dropped
        var dropElmFunc = opt.newo;
        var showPlacehOnHover;
        var justmoved;
        var stickyPlh = opt.splh;
        var hovHighl = !opt.noHovHighl;

        // func when hov cont but not any item
        var hovec = opt.hovec;

        var swrap = opt.swrap;

        // func on start hover
        var shov = opt.shov;

        //if (stickyPlh) {
        //    hovec = hovec || emptyFunc;
        //}

        // used with sticky placeholder,
        // when hovering a bigger cont/window, but dropping on a predefined cont
        var getHoverOnCont = opt.gcon || function (cx) {
            return cx.cont;
        };

        function wrap(cx) {
            var clone = swrap && swrap(cx);
            var dragObj = cx.drgo;
            previ = dragObj.index();
            cx.previ = previ;
            placeh = dragObj.clone().addClass(plhclss);
            cx.plh = placeh;

            showPlacehOnHover = 1;
            justmoved = 0;
            placeh.hide();
            dragObj.after(placeh);

            return clone || dragObj.clone();
        }

        function reshov(cx) {
            if (placeh && !stickyPlh) {
                placeh.detach();
            }

            var cont = cx.itmCont || cx.cont;

            if (cont) {
                removeClass(cont, 'o-dragForb ' + sawehighlight);
            }

            opt.reshov && opt.reshov(cx);

            lastItemHovered = null;
        }

        // executed when hovering opt.to
        // returns the cont parameter used in gscroll
        function hoverFunc(cx) {
            shov && shov(cx);

            //var hoverf = opt.hover;
            //if (hoverf) {
            //    return hoverf(cx);
            //}

            placeh = cx.plh;
            var cont = getHoverOnCont(cx), x = cx.x, y = cx.y;

            cx.itmCont = cont;

            if (hovHighl) {
                addClass(cont, sawehighlight);
            }

            if (showPlacehOnHover) {
                placeh.show();
                showPlacehOnHover = 0;
            }

            var sogroup = 'data-ogroup';

            var contGroups = cont.attr(sogroup);
            cx.cancel = 0;
            if (contGroups) {
                contGroups = contGroups.split(',');
                var itmGroup = cx.drgo.attr(sogroup);
                if (!contains(itmGroup, contGroups)) {
                    cx.cancel = 1;
                    addClass(cont, 'o-dragForb');
                }
            }

            if (opt.noReorder || noPlaceh) {
                if (!noPlaceh) { cont.append(placeh); }
                return cont;
            }

            /*if (opt.chkhov && !opt.chkhov(cx)) return cont;*/

            var itemHovered = 0;
            var elms = cont.find(sel + ':visible').get();

            // check still hovering last
            if (isNotNull(lastItemHovered)) {
                var ofs = lastItemHovered.offset();
                var lx = ofs.left;
                var ly = ofs.top;

                if (ly + outerh(lastItemHovered) > y &&
                    ly < y &&
                    lx + outerw(lastItemHovered) > x
                    && lx < x) {
                    return cont;
                }
            }

            var elmsLen = elms.length;
            var cof = cont.offset();
            var minDist;
            var isAbove = y < cof.top;
            var outside =
                isAbove || x < cof.left ||
                y > cof.top + outerh(cont) || x > cof.left + outerw(cont);

            if (outside && !stickyPlh) {
                return cont;
            }

            for (var i = 0; i < elmsLen; i++) {
                var item = $(elms[i]);
                var iof = item.offset();
                var iow = outerw(item);
                var ioh = outerh(item);

                var ix = iof.left + iow;
                var iy = iof.top + ioh;

                if (isAbove) {
                    var distance = Math.abs(x - (ix - iow / 2)) + Math.abs(y - (iy - ioh / 2));

                    if (!i || distance < minDist) {
                        minDist = distance;
                        lastItemHovered = item;
                        itemHovered = item;
                    }
                } else {
                    if (y < iy && x < ix) {
                        lastItemHovered = item;
                        itemHovered = item;
                        break;
                    }
                }
            }

            if (!itemHovered && outside) {
                for (var j = elmsLen - 1; j >= 0; j--) {
                    var itm = $(elms[j]);
                    var jof = itm.offset();
                    var jx = jof.left;
                    var jy = jof.top;

                    if (x > jx && y > jy) {
                        lastItemHovered = itm;
                        itemHovered = itm;
                        break;
                    }
                }
            }

            if (justmoved) {
                if (!itemHovered) {
                    justmoved = 0;
                } else if (justmoved.is(itemHovered)) {
                    return cont;
                }
            }

            var st = $win.scrollTop();

            if (itemHovered) {
                justmoved = itemHovered;

                var pi = placeh.index();

                if (!len(placeh.closest(cx.cont))) {
                    // hovering different cont
                    pi = -1;
                }

                if (itemHovered.index() < pi || pi === -1) {
                    itemHovered.before(placeh);
                } else {
                    itemHovered.after(placeh);
                }
            } else {
                if (hovec) {
                    hovec(cx);
                } else {
                    if (!placeh.parent().is(cont)) {
                        cont.append(placeh);
                    }
                }
            }

            // chrome page jump
            if (st !== $win.scrollTop()) {
                $win.scrollTop(st);
            }

            return cont;
        }

        function dropFunc(cx) {
            if (cx.cancel) return;
            var dragObj = cx.drgo;
            var dropf = opt.drop || opt.dropFunc;
            if (dropf) return dropf(cx);
            var nobj = dragObj;
            if (dropElmFunc) {
                nobj = dropElmFunc(cx);
            }

            cx.dropElm = nobj;
            //cx.placeh = placeh;

            if (placeh.closest('body').length) {
                if (opt.onFakeDrop) {
                    cx.noSrcShow = 1;
                }
                else {
                    placeh.after(nobj).remove();
                }
            }

            nobj.trigger('awedrop', { from: fromCont, previ: previ });
            opt.onFakeDrop && opt.onFakeDrop(cx);
            ondrop && ondrop(cx);
        }

        // get additional containers to scroll on drag
        function gscroll(cont) {
            if (cont) {
                var scrollCont = cont.closest('.o-scrollCont');
                if (len(scrollCont)) {
                    return [{ c: scrollCont, y: 1 }];
                }

                return [{ c: cont, y: 1 }];
            }
        }

        function end(cx) {
            //cx.cont null when dragstart without dragmove
            if (cx.cont && stickyPlh && len(placeh.closest('body'))) {
                dropFunc(cx);
            }

            placeh.remove();
            placeh = null;
        }

        return dragAndDrop({
            from: opt.from,
            sel: sel,
            handle: handle,
            dropSel: opt.to,
            dragClass: opt.dragClass,
            tof: opt.tof,
            dropConts: opt.dropConts,
            wrap: wrap,
            hover: hoverFunc,
            drop: dropFunc,
            reshov: reshov,
            cancel: opt.cancel,
            gscroll: opt.gscroll || gscroll,
            end: end,
            hide: ifnul(opt.hide, 1), // hide dragObj until dropped
            scroll: opt.scroll || [{ c: $win, y: 1 }]
        });
    }

    function isChildPopup(it, myId) {
        var pop1 = it.closest('.o-pu');
        if (pop1.data('pid') === myId) {
            return 1;
        }

        var pid, mclick = 0;
        if (it.is('.o-pmodal')) {
            mclick = 1;
        }

        if (pop1.length) {
            pid = pop1.data('i');
        }

        if (pid) {
            if (pid === myId && !mclick) return 1;

            var popener = kvIdOpener[pid];
            if (popener)
                return isChildPopup(popener, myId);
        }
    }

    function dropdownPopup(o) {
        var outsideClickClose = readTag(o, "Occ", o.outClickClose);
        var isDropDown = readTag(o, "Dd", o.dropdown);
        var showHeader = readTag(o, "Sh", !isDropDown);
        var toggle = readTag(o, "Tg");

        var pp = o; // popup properties
        var popup = pp.d; // popup div
        var itmoved, header, $opener, openerId, fls, popt, top;
        var host = $('body');

        pp.id = pp.id || se;
        o.cx = {};
        var cx = o.cx;
        var api = function () { };
        o.cx.api = api;

        var modal;
        var wrap;

        var pmccls = ' o-pmc o-pu ' + (pp.popupClass || se);
        if (isDropDown) pmccls += ' o-ddp';

        if (o.pwrp) {
            wrap = popup.closest('.o-pwrap');
            modal = wrap.find('.o-pmodal');
            popup.closest('.o-pu').addClass(pmccls);
        }
        else {
            wrap = $(rdiv('o-pwrap', rdiv(pmccls, se, 'tabindex="-1"'))).hide();
            modal = $(rdiv('o-pmodal o-pu', se, 'tabindex="-1" data-i="' + pp.id + '"'))
        }

        wrap.find('.o-pu').first().attr('data-i', pp.id);

        var $dropdownPopup = wrap.find('.o-pmc');

        if (!o.pwrp) {
            header = $(rdiv('o-phdr', rdiv('o-ptitl', pp.title || snbsp) + sclosespan));
            $dropdownPopup.prepend(header);
            $dropdownPopup.append(popup);
        } else {
            header = $dropdownPopup.find('.o-phdr');
        }

        var btns = pp.btns;

        var sopener = o.opener;

        pp.mlh = 0;

        !o.menu && popup.addClass('o-pc');

        if (!isDropDown) {
            popup.addClass('o-fpp');
        }

        if (isNull(pp.minw)) {
            popup.css(sminw, pp.minw);
        }

        if (o.rtl) {
            $dropdownPopup.addClass('awe-rtl').css('direction', 'rtl');
        }


        modal.on(skeyup, closeOnEsc);

        $dropdownPopup.on(skeydown,
            function (e) {
                if (e.keyCode === keyTab) {
                    var tabbables = tabbable($dropdownPopup),
                        first = tabbables.first(),
                        last = tabbables.last();
                    var tg = trg(e);
                    if (tg.is(last) && !e.shiftKey) {
                        tfocus(first);
                        return false;
                    } else if (tg.is(first) && e.shiftKey) {
                        tfocus(last);
                        return false;
                    }
                }
            });

        var isFixed;
        var zIndex = minZindex;

        function layPopup(isResize, canShrink) {

            var initSt = popup.scrollTop();

            if (pp.nolay) return;

            if (isResize) {
                // reset position changed by dragging popup
                itmoved = 0;
            }

            if (!cx.isOpen) return;

            var winavh = $win.height() - popSpace;
            var winavw = $win.width() - popHorizSpace;

            if (top) {
                winavh -= popTopSpace;
            }

            modal.css(szindex, zIndex);
            $dropdownPopup.css('overflow-y', 'auto');
            if (zIndex) {
                $dropdownPopup.css(szindex, zIndex);
            }

            popup.css(swidth, se);
            popup.css(sheight, se);
            popup.css(smaxh, se);

            var oapi = o.api || {};

            if (oapi.rlay) {
                oapi.rlay();
            }

            var capHeight = o.f ? outerhn(o.f.find('.awe-openbtn:first'), 1) : 0;

            fls = pp.fullscreen;
            top = pp.top;

            if (openerId && !$opener.closest(document).length) {
                $opener = $('#' + openerId);
            }

            var height = pp.dh || pp.height;

            if (!height && !o.menu) {
                height = Math.max(350, outerhn($dropdownPopup));
            }

            var maxph = 0;

            var popoutw = outerw($dropdownPopup) - outerw(popup);
            var popouth = outerh($dropdownPopup) - outerh(popup);

            if (o.lkp) {
                height = pp.dh || maxLookupDropdownHeight;
                maxph = pp.dh || maxLookupDropdownHeight;
            }

            var minw = popt.minw || 0;
            if (o.menu) {
                maxph = maxDropdownHeight;

                if (len($opener) && !popt.xy) {
                    minw = Math.max(outerw($opener), minw);
                }

                popup.css(sminw, minw);

                canShrink = false;
            }

            var limw = winavw;
            if (pp.maxw) {
                popup.css('max-width', pp.maxw);
                limw = pp.maxw;
            }

            if (pp.width) {
                if (!isDropDown || !pp.srv) {
                    minw = Math.min(pp.width, Math.min(limw, winavw)) - popoutw;
                    popup.css(sminw, minw);
                }
            }
            else {
                // set minw based on opener
                if (len($opener)) {
                    popup.css(sminw, outerw($opener));
                }
            }

            var minh = height;
            if (!isDropDown || pp.hws) {
                if (pp.height) {
                    minh = pp.height;
                    if (height < minh) height = minh;
                    if (maxph < minh) maxph = minh;
                    popup.css('min-height', Math.min(pp.height, winavh) - popouth);
                }
            }

            function chkfulls(ph) {
                var pw = outerw($dropdownPopup);
                var h = outerh($dropdownPopup);
                var wlim = 25, hlim = 70;

                if (pp.af || pp.menu) {
                    wlim = 200;
                    hlim = 300;

                    h = Math.max(ph, h);
                }

                if (isNotNull(pp.wlim)) {
                    wlim = pp.wlim;
                }

                var hcondit = pw > winavw - wlim && h > winavh - hlim;

                if (!o.lkp) {
                    hcondit = hcondit && h * .7 > winavh - h;
                }

                if (hcondit) {
                    fls = 1;
                }

                if (fls) {
                    var avh = winavh - popouth - (showHeader || btns ? 0 : clickOutSpace);
                    if (o.lkp) {
                        o.avh = avh;
                        o.nph = popouth;
                    }

                    popup.css(swidth, winavw - popoutw);

                    // set max-height for dropdown popups (menu)
                    //popup.css(o.menu ? smaxh : sheight, avh);                    
                    popup.css(pp.fullscreen ? sheight : smaxh, avh);
                }

                if (fls || pp.modal) {
                    modal.show();
                } else {
                    modal.hide();
                }

                return fls;
            }

            function setMaxHeight(poph, maxh) {
                var avh = maxh - popouth;

                popup.css(smaxh, avh);

                if (o.lkp) {
                    avh = poph - popouth;

                    popup.css(sheight, avh);

                    o.avh = avh;
                    o.nph = popouth;
                }
            }

            function setHeight(poph, maxh, valign) {
                if (valign === 'top') {
                    popup.css(sheight, poph);
                } else {
                    var h = maxh;
                    if (valign !== 'fls') {
                        h = Math.min(h, maxDropdownHeight); // maxpoph
                    }

                    h && popup.css(smaxh, h);
                }
            }

            layDropdownPopup2(o,
                $dropdownPopup,
                isFixed,
                capHeight,
                isDropDown ? $opener : null,
                o.menu ? setHeight : setMaxHeight,
                itmoved,
                canShrink,
                chkfulls,
                o.menu ? menuMinh : minh, //minh,
                height,
                maxph,
                popup,
                popt,
                top);

            popup.scrollTop(initSt);
            popup.trigger('awepos');
        }

        function outClickClose(e) {
            var shouldClose;
            if (isNotNull(outsideClickClose)) {
                shouldClose = outsideClickClose;
            } else {
                shouldClose = $opener && isDropDown; //closePopOnOutClick ||
            }

            if (shouldClose) {
                var tg = trg(e);
                if (!len(tg.closest($doc))) return;// for change year dd, or rem click.ddp

                if (!isChildPopup(tg, pp.id)) {
                    if (!tg.closest($opener).length) {
                        api.close({ nofocus: 1, initialEvent: e });
                    }
                }
            } else {
                //$doc.off(sddpOutClEv, outClickClose);
                unbind(o, outClickClose);
            }
        }

        function loadHandler() {
            layPopup();
        }

        $dropdownPopup.on(saweload + ' ' + sawebeginload, loadHandler);

        function resizeHandler() {
            layPopup(1, 1);
        }

        bind(o, $win, 'resize awedomlay', resizeHandler);
        //$win.on('resize awedomlay', resizeHandler);

        api.lay = resizeHandler;

        api.open = function (opt) {
            popt = opt || {};
            var e = popt.e;
            if (toggle) {
                if (cx.isOpen) {
                    return api.close();
                }
            }

            sopener = popt.opener || sopener;

            if (sopener) {
                $opener = $(sopener);
            } else {
                if (e && e.target) {
                    $opener = trg(e);
                    var btn = $opener.closest('button');
                    if (btn.length) $opener = btn;
                }

                if (o.f && o.f.closest('.awe-field').length) {
                    $opener = o.f;
                }

                if ($opener && !$opener.is(':visible')) {//|| p.f
                    $opener = null;
                }
            }

            isFixed = 1;

            if ($opener) {
                openerId = $opener.attr('id');
                var parPop = $opener.closest('.o-pmc');

                if (parPop.length) {
                    zIndex = parPop.css(szindex);
                    if (parPop.css(sposition) !== 'fixed') {
                        isFixed = 0;
                    }
                } else {
                    isFixed = isPosFixed($opener);
                    zIndex = calcZIndex(zIndex, $opener);
                }
            }

            if (!isDropDown) {
                isFixed = 1;
                header.show();
            } else {
                itmoved = 0;
            }

            if (showHeader) {
                header.show();
            } else {
                header.hide();
            }

            if (!o.pwrp) {
                o.modal && host.append(modal);
            }

            host.append(wrap);
            wrap.show();
            cx.isOpen = 1;

            layPopup(0, isDropDown);

            kvIdOpener[pp.id] = $opener;

            setTimeout(function () {
                if (!cx.isOpen) return;
                bind(o, $doc, sddpOutClEv, outClickClose);
            }, 100);

            focusFirstDesk(o);

            popup.trigger('aweopen');
        };        

        api.close = function (opt) {
            opt = opt || {};

            var nofocus = opt.nofocus || o.noCloseFocus;

            popup.trigger('awebfclose', opt);
            o.beforeClose && o.beforeClose(opt);

            if (opt.noclose && !opt.force) return; // noclose can be set by awebfclose handler

            cx.isOpen = 0;
            itmoved = 0;
            wrap.hide();
            if (modal) modal.hide();

            if (pp.close) {
                gfunc(pp.close)();
            }

            popup.trigger('aweclose', { out: nofocus });

            unbind(o, outClickClose);

            if (!(pp.loadOnce || pp.pwrp)) {

                if (pp.div) {
                    wrap.after(popup);
                }

                awe.destroyCont(wrap);
                wrap.remove();

                if (modal) modal.remove();
            }

            if (!pp.loadOnce) {
                awe.destroy(o);
            }

            if (!nofocus) {
                if (o.fcs) {
                    tfocus(o.fcs);
                }
                else if ($opener && len($opener)) {

                    tfocus($opener);
                    var tofoc = $opener.find('.o-tofoc');
                    if (len(tofoc)) {
                        tfocus(tofoc);
                    }
                    else {
                        tfocus(awe.tabbable($opener).first());
                    }
                }
            }

            return true;
        };

        api.destroy = function () {
            awe.destroyCont(wrap);
            api.close({ force: 1, nofocus: 1 });
            !pp.pwrp && wrap.remove();
            if (modal) modal.remove();
            $win.off('resize awedomlay', resizeHandler);
            awe.destroy(o);
        };

        popup.data('api', api);
        header.find('.o-cls').on(sclick, api.close);

        function getDragPopup() {
            itmoved = 1;
            return $dropdownPopup;
        }

        if (!isDropDown && popupDraggable && pp.draggable != false) {
            dragAndDrop({
                from: header,
                ch: getDragPopup,
                kdh: 1,
                cancel: function (cx) {
                    return fls || len($(cx.e.target).closest('.o-cls'));
                }
            });
        }

        addFooter(btns, $dropdownPopup, popup, 'o-pbtns');

        function closeOnEsc(e) {
            if (which(e) === keyEsc) {

                if (!popup.data('esc')) {
                    api.close({ esc: 1 });
                }

                popup.data('esc', null);
            }
        }

        $dropdownPopup.on(skeyup, closeOnEsc);

        return wrap;
    }

    function addFooter(btns, cont, popup, fclass) {
        // add btns if any
        var btnsbar = cont.find('.o-pbtns');
        if (len(btnsbar)) {
            cont.append(btnsbar);
        }
        else if (len(btns)) {
            var footer = $('<div/>').addClass(fclass);

            loop(btns, function (el) {
                var cls = !el.ok ? 'awe-sbtn' : 'awe-okbtn';
                var btn = $(rbtn('awe-btn ' + cls + ' o-pbtn', el.text));

                if (el.tag) {
                    var tag = el.tag;
                    if (tag.K)
                        loop(tag.K, function (key, indx) {
                            btn.attr(key, tag.V[indx]);
                        });
                }

                btn.on(sclick, function () { el.action.call(popup); });
                footer.append(btn);
            });

            cont.append(footer);
        }
    }

    function readTag(o, prop, nullVal, opt) {
        var res = nullVal;
        var tag = o.tag || o.Tag;

        if (tag) {
            if (isNotNull(tag[prop])) {
                res = tag[prop];
            } else {
                var lname = toLowerFirst(prop);
                if (isNotNull(tag[lname])) {
                    res = tag[lname];
                }
                else if (opt && isNotNull(opt[lname])) {
                    return opt[lname];
                }
            }
        }

        return res;
    }

    function layDropdownPopup2(o, pop, isFixed, capHeight, opener, setHeight, keepPos, canShrink, chkfulls, minh, popuph, maxph, popup, popt, postop, minbotd) {
        var scrolltop = $win.scrollTop();
        var marg = o.marg || 0;

        var opTop, opOutHeight = 0, opLeft, opOutWidth = 0;
        popt = popt || {};
        var xy = popt.xy;
        var right = popt.right;

        if (xy) {
            opener = 1;
            opTop = xy.y + scrolltop;
            opLeft = xy.x + $win.scrollLeft();
        } else if (opener) {
            opTop = opener.offset().top;
            opOutHeight = outerh(opener);
            opLeft = opener.offset().left;
            opOutWidth = outerw(opener);
        }

        if (!keepPos) {
            pop.css('left', 0);
            pop.css('top', 0);
        }

        var winh = $win.height();
        var winw = $win.width();

        var maxw = popt.maxw || winw - popHorizSpace;
        var mnw = Math.min(outerw(pop), maxw);

        pop.css('min-height', se);
        pop.css(sheight, se);
        pop.css('max-width', maxw);


        pop.css(sminw, canShrink ? se : mnw);

        pop.css(sposition, se);

        var toppos;
        var left;

        var topd = scrolltop;
        var topCapHeight;

        if (opener) {
            topd = opTop;
            capHeight = capHeight || opOutHeight;

            topCapHeight = capHeight;

            if (right) {
                capHeight = 0;
            }
        }

        // handle opener overflow
        var botoverflow = topd - (winh + scrolltop);
        if (botoverflow > 0) {
            topd -= botoverflow;
        }

        var topoverflow = scrolltop - (topd + topCapHeight);

        if (topoverflow > 0) {
            topd += topoverflow;
        }

        var top = topd - scrolltop;
        var bot = winh - (top + capHeight);

        // adjust height
        var poph = popuph || outerh(pop);

        if (!o.maxph) o.maxph = poph;
        else if (o.maxph > poph) poph = o.maxph;
        else o.maxph = poph;

        var autofls = chkfulls(poph);

        var valign = 'bot';
        var fullsclass = 'o-fls';
        if (autofls) {
            isFixed = 1;
            addClass(pop, fullsclass);
        } else {
            removeClass(pop, fullsclass);
            var maxh = 0;
            if (opener) {
                var stop = top - hpSpace;
                var sbot = bot - hpSpace;
                maxh = sbot;

                if (minh > poph) minh = poph;

                if (sbot > poph || minbotd && sbot > minbotd) {
                    valign = 'bot';
                } else if (stop > sbot) {
                    valign = 'top';
                    if (poph > stop) {
                        poph = stop;
                    }

                    maxh = stop;
                } else {
                    poph = sbot;
                }

                if (poph < minh) {
                    maxh = poph = minh;
                }

                if (maxph && poph > maxph) {
                    poph = maxph;
                }

                if (poph > winh - popSpace) {
                    maxh = poph = winh - popSpace;
                }
            } else {
                maxh = winh - popSpace;
                if (postop) {
                    maxh -= popTopSpace;
                }
            }

            setHeight(poph, maxh, valign);
        }

        if (popup) {
            popup.trigger('aweresize');
        }

        if (isFixed) {
            topd = top;
            pop.css(sposition, 'fixed');
        }

        var w = outerw(pop);
        var h = outerh(pop);
        if (o.avh) h = o.avh + o.nph;


        if (opener) {
            left = opLeft + (right ? opOutWidth : 0);
            var lspace = winw - (left + w);
            if (lspace < 0) {
                if (right) {
                    if (w < opLeft) {
                        left = opLeft - w;
                    }
                }
                else if (opOutWidth < w) {
                    left -= w - opOutWidth;
                }

                if (left + w > winw) {
                    left -= left + w - winw;
                }

                if (left < hpHorizSpace) {
                    left = hpHorizSpace;
                }
            }

            if (autofls) {
                toppos = hpSpace;
                left = hpHorizSpace;
            } else if (bot < h + hpSpace && (top > h + hpSpace || top > bot)) {
                // top
                toppos = topd - h - marg;
                if (right) toppos += topCapHeight;

                if (top < h) {
                    toppos = topd - top;
                    if (h + hpSpace < winh) toppos += hpSpace;
                }
            } else {
                // bot
                toppos = topd + capHeight + marg;

                if (bot < h + hpSpace) {

                    toppos -= h - bot;

                    if (h + hpSpace < winh) toppos -= hpSpace;
                }
            }
        } else {
            // no opener, center popup
            if (autofls) {
                toppos = hpSpace / 2;
            }
            else if (postop) {
                toppos = hpSpace + popTopSpace;
            } else {
                var diff = winh - h;
                toppos = diff / 2;
                if (diff > 200) toppos -= 45;
            }

            left = Math.max((winw - outerw(pop)) / 2, 0);
        }

        if (!keepPos || autofls) {
            pop.css('left', left);
            pop.css('top', Math.floor(toppos));
        }

        addRemClass(pop, function () { return pop.width() < 90; }, 'o-small');
    }

    function addRemClass(pop, cond, cssClass) {
        if (cond()) {
            addClass(pop, cssClass);
        } else {
            removeClass(pop, cssClass);
        }
    }

    function focusFirstDesk(o) {
        if (isMobile() || o.noFocusFirst) return;
        tfocus(awe.tabbable(o.d).first());
    }

    //#endregion awem sync

    //#region blazor

    function openBz(opt) {
        opt.close = function () {
            var popup = opt.d;
            if (popup && len(popup.closest($doc))) {
                opt.objRef.invokeMethodAsync("SetClosed");
            }
        }

        return open(opt);
    }

    function openOnHover(opt) {
        var cont = $(opt.cont);
        var api = opt.objRef;
        hovOpen({
            hover: cont,
            open: function () {
                var res = api.invokeMethodAsync("Open");
                return res;
            },
            close: function (popup) {
                if (popup) {
                    var o = adata(popup, 'o');
                    if (o) {
                        awe.close(o);
                    }
                };
            }
        });
    }

    function dtpKeyNavCon(opt) {
        var isOpen = opt.isOpen;
        var o = opt.o;
        var input = opt.input;
        var btn = opt.btn;
        var cont;
        var shov = 'o-hov';
        var shovc = '.' + shov;
        var nxtcls = '.o-mnxt';
        var prvcls = '.o-mprv';
        if (opt.rtl) {
            var c = nxtcls;
            nxtcls = prvcls;
            prvcls = c;
        }

        bind(o, input, skeyup, keyuph);
        bind(o, input, skeydown, keyDownBld(1));

        bind(o, btn, skeyup, keyuph);
        bind(o, btn, skeydown, keyDownBld());

        function keyDownBld(isInp) {
            return function (e) {
                cont = opt.getPopup();
                var key = which(e);

                if (keynav(key)) {
                    prevDef(e);
                }

                if (!isInp) return;

                if (!isOpen()) {
                    if (key === keyDown || key === keyUp) {
                        opt.open(e); //openDtp(e);
                    }
                } else {
                    if (key === keyEnter) {
                        prevDef(e); // stop form post
                    }
                }

                // / / . . - -
                awe.pnn(e, [191, 111, 190, 110, 189, 109]);

                //kval = input.val();
            };
        }

        function keyuph(e) {
            var k = which(e);

            if (isOpen()) {
                if (k === keyEnter) {
                    tclick(getHov());
                } else if (k === keyEsc) {//!inline && 
                    opt.close(); 
                    e.stopPropagation();
                }                
            }
        }

        function keynav(key) {
            var res = 0;
            if (isOpen()) {
                if (key === keyDown) {
                    moveHov(1);
                    res = 1;
                }
                else if (key === keyUp) {
                    moveHov(-1);
                    res = 1;
                }
            }

            if (res) cont.addClass('o-nhov');
            return res;
        }

        function getHov() {
            var h = cont.find(shovc);
            if (!h.length) h = cont.find('.o-enb:hover');
            if (!h.length) h = cont.find('.o-enb.o-selday');
            if (!h.length) h = cont.find('.o-enb.o-tday');
            if (!h.length) h = cont.find('.o-enb:first');

            return h;
        }

        function getTbMonth() {
            return cont.find('.o-tb').attr('data-month');
        }

        var waitRender;
        function moveHov(dir) {
            if (waitRender) return;
            var pivot = getHov();
            var sel = '.o-enb';
            if (cont.find(nxtcls).is(':enabled')) {
                sel = '.o-mnth:first ' + sel;
            }

            var list = cont.find(sel);

            var indx = list.index(pivot) + dir;
            var n = list.eq(indx);

            if (!n.length || indx < 0) {
                n = 0;
                var cls = dir > 0 ? nxtcls : prvcls;
                var fl = dir > 0 ? 'first' : 'last';
                var nbtn = cont.find(cls);

                if (nbtn.is(':enabled')) {
                    var m = getTbMonth();
                    tclick(cont.find(cls));

                    var intc = 0;
                    waitRender = 1;
                    var int1 = setInterval(function () {
                        intc++;
                        if (getTbMonth() != m || intc > 30) {
                            clearInterval(int1);
                            waitRender = 0;
                            if (intc > 30) return;
                            n = cont.find('.o-mnth:first .o-enb:' + fl);
                            focusn();
                        }
                    }, 10);
                }
            }
            else {
                focusn();
            }

            function focusn() {
                if (n && n.length) {
                    cont.find(shovc).removeClass(shov);
                    n.addClass(shov);
                }
            }
        }
    }

    function olist(opt) {
        var itemscont = $(opt.div);
        var cont = itemscont.parent();
        var opener = $(opt.opener);

        if (opt.focus) {
            lfocus(itemscont, opt);
            return;
        }

        var mnits = itemscont.find('.o-mnits');

        //:not(.o-cmbi) to prevent from auto focusing the combo item
        var slistc = slist(itemscont, { sel: svalc + ', .o-nod', afs: svalc + '.o-ditm:not(.o-cmbi)', botf: botf, topf: topf, fcls: opt.fcls, sc: opt.scls });

        if (opt.hovFocus) {
            cont.on(smousemove,
                svalc + ', .o-nod',
                function () {
                    slistc.focus($(this));
                    //dmo.prntRf && dmo.prntRf(); // refocus parent
                });
        }

        cont.on(skeydown, slistc.keyh);

        var o = adata(itemscont, 'o') || {};
        adata(itemscont, 'o', o);

        //bind(o, opener, skeydown, openerKeyDown);
        bind2({ o: o, handler: openerKeyDown, event: skeydown, el: opener, id: skeydown });

        function openerKeyDown(e) {
            if (cont.is(':visible'))
                slistc.keyh(e);
        }

        adata(itemscont, 'olst', slistc);

        function botf() {
            var st = itemscont.scrollTop();
            var sth = itemscont.height();
            var h = mnits.height();
            if (sth + st < h) {
                itemscont.scrollTop(st + 25);
            }
        }

        function topf() {
            var st = itemscont.scrollTop();
            if (st > 0) {
                itemscont.scrollTop(st - 25);
            }
        }
    }

    function lfocus(itemscont, opt) {
        var slc = adata(itemscont, 'olst');
        if (!slc) return;
        var index = opt.index;
        if (isNotNull(index)) {
            slc.autofocus(itemscont.find('.o-itm[data-index=' + index + ']'), 1);
        } else {
            slc.autofocus(0, 1);
        }
    }

    // filter jquery object to only first level child, avoid nested grid elements
    function mine(jqobj, g) {
        return jqobj.filter(function (_, elm) {
            return $(elm).closest('.awe-grid').is(g);
        });
    }

    function gfindMine(sel, gdiv) {
        return mine(gdiv.find(sel), gdiv);
    }

    function regGGO(opt) {
        opt.v = $(opt.gdiv);
        opt.cont = $(opt.cont);
        opt.header = $(opt.header);

        var gdiv = opt.v;
        var gbar = gdiv.find('.awe-groupbar').first();
        var hcon = gfindMine('.awe-hcon', gdiv);

        opt.hcon = hcon;
        opt.hrow = hcon.find('.awe-hrow');
        opt.gtableswraps = gfindMine('.awe-tablw', gdiv);
        opt.colswrap = gfindMine('.awe-colw', gdiv);
        opt.colgroups = gfindMine('colgroup', gdiv);
        opt.syncon = 1;

        if (opt.onRowClick) {
            bind(opt, gdiv, 'click', '.awe-row[data-k]', function (e) {
                var trg = $(e.target);
                if (!trg.closest('.awe-grid').is(gdiv)) return;
                if (trg.closest('button, .awe-field').length) return;
                var k = trg.closest('.awe-row').data('k');

                opt.objRef.invokeMethodAsync("RowClick", k.toString());
            });
        }

        // drag columns grouping and reordering
        regColumnsGroupReorder({
            v: gdiv,
            gbar: gbar,
            hcon: hcon,
            hrow: opt.hrow,
            hdrop: function (hopt) {
                opt.objRef.invokeMethodAsync("Reorder", hopt.currenti, hopt.hoveri);
            },
            gdrop: function (cx) {
                opt.objRef.invokeMethodAsync("Group", cx.drgo.data('i'));
            }
        });

        mainGridLay(opt);
        setColumnsDimensions(opt);

        opt.persist = function () {
            // save column width after resize;
            loop(opt.columns,
                function (col) {
                    col.W = Math.round(col.W);
                });

            opt.objRef.invokeMethodAsync("Persist", opt.columns);
        };

        if (opt.fzr || opt.fzl) {
            opt.fzopt = { left: opt.fzl, right: opt.fzr };
            awem.gfzcols(opt);
        }

        regColumnsResize(opt);

        // for zoom  in scrollbar width adjust
        bind(opt,
            $window,
            'resize awedomlay',
            function () {
                if (!gdiv || !gdiv.closest($('body')).length) {
                    awe.destroy(opt);
                }

                mainGridLay(opt);
                setColumnsDimensions(opt);
            });

        regSyncHorizScroll(opt);

        return adata(gdiv, 'opt', opt); // returns aweid
    }

    function gmod(opt) {
        // get storage data by opt.aweid
        var copt = awef.storage[opt.aweid].opt;

        if (opt.columns) {
            copt.columns = opt.columns;
        }

        copt.v.trigger('awerender');

        // helps chrome keep header and content cells aligned (they get misaligned even though colgroups html is the same)
        mainGridLay(copt);

        setColumnsDimensions(copt);
    }

    function onStopInp(opt) {
        var txt = $(opt.inp).add(opt.inp2);
        var timer;
        txt.on('input', function (e) {
            clearTimeout(timer);
            timer = setTimeout(function () {
                opt.objRef.invokeMethodAsync("Search", txt.val())
            }, 250);
        });
    }

    function dtpbz(opt) {
        var field = $(opt.field);
        var input = field.find('.awe-val');
        var btn = field.find('.awe-btn');

        // set popup on open call
        if (opt.popup) {
            var popup = $(opt.popup);
            var o = adata(field, 'opt');
            o.popup = popup;
            return;
        }

        adata(field, 'opt', opt);

        //input.on(skeydown, inpkeyd);

        function isOpen() {
            return getPopup().closest(document).length;
        }

        function openDtp() {
            tclick(input);
        }

        function getPopup() {
            var o = adata(field, 'opt');
            return $(o.popup);
        }

        dtpKeyNavCon({
            getPopup: getPopup, isOpen: isOpen, open: openDtp, o: opt,
            input: input,
            btn: btn,
            close: function () {
                awe.close({ div: getPopup() });
            }
        });
    }

    function multis(opt) {
        var cont = $(opt.cont);

        function applyCaption() {
            var input = cont.find('input');
            var caption = cont.find('.o-cptn');

            if (input.is(':focus') || len(cont.find('.o-mlti'))) {
                caption.hide();
            }
            else {
                caption.show();
            }
        }

        applyCaption();

        if (opt.apl) return;

        cont.on('focusin keyup', 'input', function () {
            applyCaption();
            var input = $(this);
            var w = Math.min(
                Math.max((input.val().length + 2) * 10, 25),
                cont.width());
            input.width(w);
        });

        cont.on('focusout', 'input', function () {
            $(this).val('').change().width(10);
            applyCaption();
        });

        dragReor({
            from: cont,
            sel: '.o-mlti:not([disabled])',
            tof: function () {
                return [$('body')];
            },
            gcon: function () { return cont; },
            plh: 'awe-hl',
            splh: 1,
            noHovHighl: 1,
            dropFunc: function (cx) {
                var drgo = cx.drgo;
                var from = cx.previ;
                var to = cx.plh.index();
                cx.plh.remove();
                drgo.show();
                if (from < to) to--;
                opt.objRef.invokeMethodAsync("MoveVal", from, to);
            }
        });
    }

    function submitb(event) {
        var f = $(event.target).closest('.o-pu').find('form');
        awef.trigger(f, 'submit')
    }

    function inlfcs(opt) {
        var gdiv = $(opt.gdiv);
        var key = opt.key;
        var row = gdiv.find('.awe-row[data-k=' + key + ']');
        tfocus(tabbable(row).first());        
    }

    function flashRow(opt) {
        var gdiv = $(opt.gdiv);
        var key = opt.key;
        var row = gdiv.find('.awe-row[data-k=' + key + ']');
        flash(row);
    }

    function ldngbz(opt) {
        var div = $(opt.div);
        setTimeout(function () {
            addClass(div, 'on');
            div.show();
        }, opt.delay ? 150 : 1);
    }

    function numspn(o) {
        o.d = $(o.elm);
        o.f = o.d.closest('.awe-field');
        o.inc = function (op) {
            o.oref.invokeMethodAsync("Inc", op);
        };

        awe.numeric(o, 1);
    }
    //#endregion

    function hovOpen(opt) {
        var timer, wopener, aopener, popup, isLoading;
        var sev = smousemove + ' touchstart.awenpd';
        opt.hover.on(sev, opt.hsel, onHover);

        function onHover() {
            if (isLoading) return;

            var copener = $(this);

            if (!popup || popup.is(':hidden')) {
                aopener = null;
            }

            if (!(copener.is(aopener) || copener.is(wopener))) {
                resett();
                $document.on(sev, docMove);

                var topen = opt.topen || 120;

                timer = setTimeout(function () {
                    opt.close(popup);

                    aopener = copener;
                    wopener = null;

                    // open
                    isLoading = 1;
                    Promise.resolve(opt.open(copener)).then(function (value) {
                        popup = $(value);
                        isLoading = 0;
                    });
                }, aopener ? topen : topen / 2);

                $document.on(sev, docMove);
                wopener = copener;
            } else {
                if (copener.is(aopener) && wopener) {
                    resett();
                }
            }
        }

        var lstTrg;
        function docMove(e) {
            if (isLoading) return;
            var tg = trg(e);
            if (tg.is(lstTrg)) return;
            lstTrg = tg;

            var chk = opt.hover;
            if (opt.hsel) {
                chk = chk.find(opt.hsel);
            }

            if (istrg(e, chk) || !popup) {
                resett();
                return;
            }

            resett();

            if (!istrg(e, popup) || opt.hclose) {
                if (opt.shclose && !opt.shclose(e)) {
                    resett();
                } else {
                    timer = setTimeout(function () {
                        opt.close(popup);
                        $document.off(sev, docMove);
                    }, opt.tclose || 300);
                }
            }
        }

        function resett() {
            clearTimeout(timer);
            timer = null;
            wopener = null;
        }

        return {
            destroy: function () {
                opt.hover.off(sev, onHover);
            }
        };
    }

    function setCaretPosition(elem, caretPos) {
        if (elem.createTextRange) {
            var range = elem.createTextRange();
            range.move('character', caretPos);
            range.select();
        } else if (elem.selectionStart) {
            tfocus(elem);
            elem.setSelectionRange(caretPos, caretPos);
        }
    }

    function remDrag(remApi) {
        remApi && remApi.apply();
    }

    initWave();

    return {
        version: '1.2.10',
        pnn: preventNonNumbers,
        openOnHover: openOnHover,
        tabbable: tabbable,
        popup: dropdownPopup,
        multis: multis,
        olist: olist,
        onStopInp: onStopInp,
        dtpbz: dtpbz,
        submitb: submitb,
        inlfcs: inlfcs,
        gmod: gmod,
        rdd: regDragDrop,
        ldngbz: ldngbz,
        flash: flash,
        flashRow: flashRow,
        regGGO: regGGO,
        numspn: numspn,
        numeric: numeric,
        close: function (o) {
            var div = o.div;
            var id = $(o.div).data('aid');
            var o = adata(o.div, 'o');
            //o = storage[id];           

            if (o) {
                o.cx.api.close();
            }
        },
        open: open,
        openBz: openBz,
        op: op,
        rowDragWrap: function (cx) {
            drgo = cx.drgo;

            if (drgo.closest('.awe-itc').is('tbody')) {

                return $('<div/>')
                    .prop('class', drgo.closest('.awe-grid').prop('class'))
                    .append($('<table/>')
                        .append(drgo.closest('table').find('colgroup').clone())
                        .append(drgo.clone()));
            }

        },
        remDrag: remDrag,
        dragBz: function (opt) {
            if (opt.dragRef) {
                remDrag(opt.dragRef);
            }

            var modifyDom = opt.chDom;

            var toSelector = opt.to || opt.from;

            var sel = opt.sel;

            var dragOpt = {
                from: opt.fromCont ? $(opt.fromCont) : $(opt.from),
                to: toSelector,
                sel: sel,
                noReorder: opt.noReorder,
                hide: !opt.noHide,
                noPlh: opt.noPlh,
                noHovHighl: opt.noHovHighl,
                handle: opt.handle,
                splh: opt.splh,
                plh: opt.plhCls,
                swrap: opt.swrap ? gfunc(opt.swrap) : nul
            };            

            var dropConts = opt.dropConts;

            if (dropConts) {
                dragOpt.dropConts = awef.select(dropConts, function (elm) { return $(elm); });
            }

            if (modifyDom) {
                dragOpt.onDrop = onDrop;

            }
            else {
                dragOpt.onFakeDrop = onDrop;
            }

            return { apply: dragReor(dragOpt) }; // apply: dragDestrApi

            function selParIndex(de, sel) {
                var mysel = sel;
                if (!modifyDom) mysel += ':visible'; // skip hidden dragObj
                return de.parent().find(mysel).index(de);
            }

            function onDrop(cx) {

                var de = cx.plh;

                if (modifyDom) {
                    de = cx.dropElm;
                }

                var fromcontK = cx.drgo.closest('.o-dragCont').attr('data-k');
                var promise = move(
                    de.attr('data-k'),
                    selParIndex(de, sel),
                    cx.cont.attr('data-k'),
                    fromcontK);

                $.when(promise).done(function () {
                    if (!modifyDom) de.remove();
                    cx.drgo.show();
                });
            }

            function move(itmKey, index, contKey, fromContKey) {
                if (!opt.method) return;
                return opt.objRef.invokeMethodAsync(opt.method, { itmKey: itmKey, index: index, contKey: contKey, fromContKey: fromContKey });
            }
        },
        dragReor: dragReor,
        destrElm: function (elm) {
            var div = $(elm);
            var datas = adata(div);
            awef.remd(div);

            for (var d in datas) {
                awe.destroy(datas[d]);
            }
        },
        destroy: function (o) {
            if (o && o.desf) {
                loop(o.desf, function (ds) {
                    ds.f();
                });

                o.desf = [];
            }
        },
        destroyCont: function (div) {
            div = $(div);
            function desf() {
                awe.destroy($(this).data('o'));
            }

            div.find('.awe-grid,.awe-val').each(desf);

            awe.destrElm(div);
            div.find('[aweid]').each(function (_, elm) {
                awe.destrElm(elm);
            })
        },
        isMobile: function () {
            if (navigator) {
                var uad = navigator.userAgentData;
                if (uad && uad.mobile) return true;

                return (/Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent));
            }

            return false;
        },
        prevDef: function (e) {
            if (e) {
                if (e.preventDefault) {
                    if (e.cancelable !== false) {
                        e.preventDefault();
                    }
                }
                else {
                    e.returnValue = false;
                }
            }
        }
    };
}(jQuery);

//export {awe};
//::endawe