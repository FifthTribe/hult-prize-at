$(function () {

    $('.submit-btn-top').on('click', function (e) {
        e.preventDefault();

        // This form
        var form = $(this).parents('.contact');

        // valid bool
        var validForm = validateForm(form);

        if (validForm) {

            $.ajax({
                type: "POST",
                url: form.attr('action'),
                data: form.serialize(),
                success: function (data) {

                    if (data == "success") {
                        alert("Thank you for submitting your information.");

                    }
                    // Reset the form
                    $('input', form).val('');
                },
            });

        }

    });


    $('.answer').hide();
    $('.question-wrap').each(function () {
        var q = $(this);
        $('.question', q).on('click', function () {
            $('.answer').slideUp();
            if ($(this).hasClass('active')) {
                $(this).removeClass('active');
            } else {
                $('.answer', q).slideDown();
                $(this).addClass('active');
            }
        });
    });


    // Fancybox Vasil added
    $(".various").fancybox({
        maxWidth: 780,
        maxHeight: 400,
        fitToView: false,
        width: '70%',
        height: '70%',
        autoSize: false,
        closeClick: false,
        openEffect: 'none',
        closeEffect: 'none',
        onComplete: formInit()
    });

    // More JS vasil added for scroll:
    $('a[href*=#]:not([href=#])').click(function () {
        if (location.pathname.replace(/^\//, '') == this.pathname.replace(/^\//, '') && location.hostname == this.hostname) {
            var target = $(this.hash);
            target = target.length ? target : $('[name=' + this.hash.slice(1) + ']');
            if (target.length) {
                $('html,body').animate({
                    scrollTop: target.offset().top
                }, 1000);
                return false;
            }
        }
    });

    // Scroll for schools
    $('#schools.scroll').mCustomScrollbar({
        axis: "y"
    });

    // Scroll for judges and sponsors
    $('#tab-judges, #tab-sponsors').mCustomScrollbar({
        setHeight:500,
    });

    // Sponsors and Judges Tabs
    $('#sponsors-judges-tabs a').click(function (e) {
        e.preventDefault()
        $(this).tab('show')
    })

    $('#popup-video').on('show.bs.modal', function (e) {
      $('.popup-video-iframe').attr('src', 'https://www.youtube.com/embed/11dyGlSK29I');
    }).on('hide.bs.modal', function (e) {
      $('.popup-video-iframe').attr('src', '');
    }).modal('show');

});

function validateEmail(email) {
    var re = /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
    return re.test(email);
}

function formInit() {
    $('.submit-btn-popup').off('click');
    $('.submit-btn-popup').on('click', function (e) {
        e.preventDefault();


        // This form
        var form = $(this).parents('.contact');

        // valid bool
        var validForm = validateForm(form);

        if (validForm) {

            $.ajax({
                type: "POST",
                url: 'process.php',
                data: form.serialize(),
                success: function (data) {

                    if (data == "success") {
                        alert("Thank you for submitting your information.");

                    }
                    // Reset the form
                    $('input', form).val('');
                },
            });

        }

    });

}

function validateForm(form) {

    var validForm = true;

    // Validate each field
    $("input", form).each(function () {

        var val = $(this).val();

        if (val == '') {
            alert('You did not complete the form.');
            validForm = false;
            return false;
        } else {

            // email validation
            if ($(this).hasClass('email')) {

                var validEmail = validateEmail(val);

                if (!validEmail) {
                    alert('Please enter a valid email address.');
                    validForm = false;
                    return false;
                }
            }

        }

    });

    return validForm;

}
