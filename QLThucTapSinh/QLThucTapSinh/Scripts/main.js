

(function ($) {

    $(window).on('load', function () {
        $(".loader").fadeOut();
        $("#preloder").delay(200).fadeOut("slow");

        /*------------------
            Gallery filter
        --------------------*/
        $('.gallery-controls li').on('click', function () {
            $('.gallery-controls li').removeClass('active');
            $(this).addClass('active');
        });
        if ($('.gallery-filter').length > 0) {
            var containerEl = document.querySelector('.gallery-filter');
            var mixer = mixitup(containerEl);
        }

    });

    /*------------------
        Background Set
    --------------------*/
    $('.set-bg').each(function () {
        var bg = $(this).data('setbg');
        $(this).css('background-image', 'url(' + bg + ')');
    });

    /*------------------
		Navigation
	--------------------*/
    $(".mobile-menu").slicknav({
        prependTo: '#mobile-menu-wrap',
        allowParentLinks: true
    });

    /*------------------
		Menu Hover
	--------------------*/
    $(".header-section .nav-menu .mainmenu ul li").on('mousehover', function () {
        $(this).addClass('active');
    });
    $(".header-section .nav-menu .mainmenu ul li").on('mouseleave', function () {
        $('.header-section .nav-menu .mainmenu ul li').removeClass('active');
    });

    /*------------------------
		Class Slider
    ----------------------- */
    $(".classes-slider").owlCarousel({
        items: 3,
        dots: true,
        autoplay: true,
        loop: true,
        smartSpeed: 1200,
        responsive: {
            0: {
                items: 1,
            },
            768: {
                items: 3,
            },
            992: {
                items: 3,
            }
        }
    });

    /*------------------------
		Testimonial Slider
    ----------------------- */
    $(".testimonial-slider").owlCarousel({
        items: 1,
        dots: false,
        autoplay: true,
        loop: true,
        smartSpeed: 1200,
        nav: true,
        navText: ["<i class='fa fa-angle-left'></i>", "<i class='fa fa-angle-right'></i>"]
    });

    /*------------------
        Magnific Popup
    --------------------*/
    $('.video-popup').magnificPopup({
        type: 'iframe'
    });

    /*------------------
        About Counter Up
    --------------------*/
    $('.count').each(function () {
        $(this).prop('Counter', 0).animate({
            Counter: $(this).text()
        }, {
                duration: 4000,
                easing: 'swing',
                step: function (now) {
                    $(this).text(Math.ceil(now));
                }
            });
    });

    /*------------------
       Schedule Filter
    --------------------*/


})(jQuery);


$(document).ready(function () {
    $.ajax({
        type: 'GET',
        //url: '@Url.Action("checkEDate", "Home")',
        url: '/Home/checkEDate',
    });
});
$(document).ready(function () {
    $('#form-login1').toggle("sidepanel");
    $('.dangnhap2').click(function () {
        $('#form-login1').toggle("sidepanel");
        $(".navbar-nav").animate({ marginRight: "328px" }, 1000);

    });
});
$(document).ready(function () {
    $('.closebtn').click(function () {
        $('#form-login1').toggle("sidepanel");
        $(".navbar-nav").animate({ marginRight: "0" }, 1000);
    });
});
$(document).ready(function () {
    $(window).scroll(function (event) {
        var pos_body = $('html,body').scrollTop();
        if (pos_body > 20) {
            $('.header-section').addClass('co-dinh-menu');
        }
        else {
            $('.header-section').removeClass('co-dinh-menu');
        }
    });
    $('.dangnhap3').click(function () {
        $('html,body').animate({ scrollTop: 0 }, 1400)

    });
});
