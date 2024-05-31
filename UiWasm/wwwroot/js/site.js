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

    return {
        setCookie: setCookie,
        changeTheme: changeTheme,
        getLSVal: getVal,
        setLSVal: setVal
    };
}();