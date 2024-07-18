var site = function () {
    var win = $(window);
    var body = $('body');
    var bwidth = 1050;

    function menuToggle() {
        var header = $('#header');
        var aside = $('#sideMenuCont');
        var itsc = aside.find('.o-itsc');

        var hide = $('#demoMenu').is(':visible');
        var btn = $('#btnMenuToggle');
        var modalMenuClass = 'fullMenuModalOpen';

        setMenuItemsSize();

        if (hide) {
            aside.hide();
            body.removeClass(modalMenuClass);
            btn.removeClass('on').trigger('awedomlay');
        } else {
            if (win.width() < bwidth) {
                body.addClass(modalMenuClass);
                aside.show();
                win.on('resize', modalMenuWinRes);
            }
            else {
                aside.css('display', '');
            }

            btn.addClass('on').trigger('awedomlay');
        }

        function modalMenuWinRes() {
            if (win.width() >= bwidth || !body.hasClass(modalMenuClass)) {
                if (body.hasClass(modalMenuClass)) {
                    menuToggle();
                    menuToggle();
                }

                win.off('resize', modalMenuWinRes);
            }
        }

        function setMenuItemsSize() {
            itsc.css('height', 'calc(100vh - ' + (header.height() + 75) + 'px)');
        }
    }

    function changeTheme(theme) {
        $('#theme').attr('href', '_content/Omu.BlazorAwesome/css/theme/' + theme + '/BlazorAwe.css');
        setVal("awetheme", theme);
        setCookie('awetheme', theme);
        var body = $('body');
        body.removeClass(body.attr('theme')).addClass(theme).attr('theme', theme);
    }

    function getVal(key) {
        return localStorage.getItem(key);
    }

    function setVal(key, val) {
        localStorage.setItem(key, val);
    }

    function setCookie(name, val) {
        document.cookie = name + '=' + val;
    }

    // side menu
    $(document).on('click', '#btnMenuToggle', menuToggle)
        .on('click touchstart', '#menuModal', menuToggle);

    var theme = getVal("awetheme");
    theme && changeTheme(theme);

    var loadedCache = {};
    async function loadSourceCode(url) {
        if (loadedCache[url]) {
            return loadedCache[url];
        }

        var res = await fetch(url);
        if (!res.ok) {
            return 'error loading source from github';
        }

        const raw = await res.text();
        loadedCache[url] = raw;
        return raw;
    }

    function setShowCodeState(hidden) {
        var btn = $('.shcode');
        var codeWrap = btn.parent().next();
        if (hidden) {
            btn.html("show code");
            btn.addClass('codeHidden');
            codeWrap.hide();

            $('#showCodeCont').empty();
            $('.codePath').empty();
            history = [];
            return;
        }


        btn.html("hide code");
        codeWrap.show();
    }

    var history = [];

    function renderHistory() {
        var res = '';
        for (let i = 0; i < history.length; i++) {

            if (i) res += '\\';
            res += `<button type="button" class="awe-btn histbtn" data-i="${i}">${history[i].caption}</button>`;
        }

        return res;
    }

    $(document).on('click', '.histbtn', async function () {
        var i = $(this).data('i');
        var hitm = history[i];

        history = history.slice(0, i + 1);
        await loadCode(hitm);
    });

    async function loadCode(hitm) {
        var codeCont = $('#showCodeCont').html('loading...');
        var source = await loadSourceCode(hitm.url);
        var parsedCode = strUtil.getCode(source);

        $('.codeHistory').html(renderHistory());
        codeCont.html(parsedCode).removeClass('prettyprinted');
        $('.codePath').html(hitm.name);

        site.prettyPrint();
    }

    $(document).on('click', '.shcode', async function (e) {
        var btn = $(this);
        btn.toggleClass("codeHidden");
        var hidden = btn.hasClass("codeHidden");

        setShowCodeState(hidden);

        if (hidden) {
            return;
        }

        var page = $(this).attr('page');
        page = !page ? 'Index' : page + 'Page';

        var cname = page;
        var name = `/Pages/${page}.razor`;
        var url = `https://raw.githubusercontent.com/omuleanu/BlazorAwesome/main/UiWasm/${name}`;

        var hitm = { url: url, name: name, caption: cname };
        history.push(hitm);
        await loadCode(hitm);
    });

    $(document).on('click',
        '.funcLink',
        async function (e) {
            e.preventDefault();
            var o = $(this);
            var cname = o.data('name');
            var name = "PageComps/" + cname + ".razor";

            var url = `https://raw.githubusercontent.com/omuleanu/BlazorAwesome/main/UiWasm/${name}`;
            var hitm = { url: url, name: name, caption: cname };
            history.push(hitm);
            await loadCode(hitm);
        });

    function pageChange() {
        setShowCodeState(1);
    }

    return {
        prettyPrint: function () {
            try {
                window.PR && PR.prettyPrint();
            } catch (ex) {
                //ignore
            }
        },
        pageChange: pageChange,
        loadSourceCode: loadSourceCode,
        setCookie: setCookie,
        changeTheme: changeTheme,
        getLSVal: getVal,
        setLSVal: setVal
    };
}();



var strUtil = function () {
    var loop = awef.loop;
    function replAll(str, f, r) {
        return str.split(f).join(r);
    }

    function setLinks(str, owner) {
        var fxwords = [];
        var cons = [
            'GridInlineEditDemo',
            'GridNestCrud',
            'HomeEditors',
            'RemoteSearchNoCache',
            'DinnersDetailsGridButton',
            'DinnerForm',
            'RestaurantDinnersInlineEdit',
            'GridFilterRow',
            'DragDropAB',
            'DragDropGroups',
            'DragCardsAndItems',
            'DragAndDropCount',
            'DragAndDropHandle'];

        var res = '';
        for (var i = 0; i < str.length; i++) {
            var handled = false;
            loop(cons, function (con) {
                if (isNextStr(str, i, con)) {
                    var endi = indexOfAny(str, [' ', ',', '/', '>'], i + con.length);
                    if (endi < 0) endi = str.length;
                    var fname = str.substring(i, endi);

                    res += '<a href="#" class="funcLink" data-name="' + fname + '">' + fname + '</a>';

                    i = endi - 1;
                    handled = true;
                    return false;
                }
            });

            if (handled) continue;

            loop(fxwords, function (word) {
                if (isNextStr(str, i, word)) {
                    var endi = i + word.length;
                    var fname = str.substring(i, endi);
                    res += '<a href="#" class="funcLink" data-name="' + fname + '">' + fname + '</a>';
                    i = endi;
                }
            });

            res += str[i];
        }

        return res;
    }

    function isNextStr(source, startIndex, nextStr) {
        if (source.length >= startIndex + nextStr.length) {
            return source.substr(startIndex, nextStr.length) === nextStr;
        }

        return false;
    }

    function indexOfAny(str, chars, startAtIndx) {
        var index = -1;

        for (var i = startAtIndx; i < str.length; i++) {
            var match = awef.where(chars, function (c) {
                return c === str[i];
            });

            if (match.length) {
                index = i;
                break;
            }
        }

        return index;
    }

    return {
        getCode: function (str) {
            var lines = str.split('\n');

            lines = strUtil.remStartSpace(lines);

            str = lines.join('\n');

            str = strUtil.sanitize(str);
            str = setLinks(str);

            return str;
        },
        indexOfAny: indexOfAny,
        fromTo: function (str, fromi, toi) {
            return str.substring(fromi, toi);
        },
        sanitize: function (str) {
            str = replAll(str, '\r', '');
            str = replAll(str, '&', '&amp;');
            str = replAll(str, '\"', '&quot;');
            str = replAll(str, '\'', '&#39;');
            str = replAll(str, '<', '&lt;');
            str = replAll(str, '>', '&gt;');
            str = replAll(str, '\n', '<br/>');
            return str;
        },
        remStartSpace: function (lines) {
            var res = lines;
            var minws = null;
            awef.loop(res, function (line) {
                if (line.length === 0
                    || line.length === 1 && line[0].charCodeAt() === 13) return true;

                var lmin = 0;
                awef.loop(line, function (ch) {
                    if (ch === ' ') lmin++;
                    else return false;
                });

                if (awef.isNull(minws) || lmin < minws) minws = lmin;
            });


            if (awef.isNotNull(minws)) {
                for (var i = 0; i < res.length; i++) {
                    var line = res[i];
                    if (line.length !== 0) {
                        res[i] = line.substring(minws);
                    }
                }
            }

            return res;
        }
    };
}();