$(function () {

    // Navigation affix
    $('#nav').affix({
        offset: {
            top: $('#banner').height()
        }
    });

    // On Window Resize
    $(window).resize(function () {

        // Fix Banner
        //fixBanner();

    });

    // No idea what this does, Mossab needs to answer this
    $('.wide').css('width', '16%');

    // navigation click actions
    $('.scroll-link').on('click', function (event) {
        event.preventDefault();
        var sectionID = $(this).attr("data-id");
        scrollToID('#' + sectionID, 500);
    });

    // When user hits register button, show registration section
    $('.register').on('click', function (event) {
        var bodyDiv = $("#body");
        var registerDiv = $("#register");
        if (!registerDiv.is(":visible")) {
            bodyDiv.hide();
            registerDiv.show();
            $("body").css('background', '#ffffff');

            event.preventDefault();
            var sectionID = $(this).attr("data-id");
            scrollToID('#' + sectionID, 500);
        }
    });

    // When user clicks to play clinton video
    /*$('#clinton-video').on('click', function (event) {
        var video = document.getElementById("clinton-video");
        if (video.paused) {
            video.play();
        } else {
            video.pause();
        }
    });*/

    // Phone international checkbox
    $('.is-international').on('click', function (event) {
        var isInternational = $(this).is(':checked');

        var id = $(this).attr('id');
        var memberNumber = id.substring(0, 7);

        //international fields
        var countryCodeString = "#" + memberNumber + 'PhoneCountryCodeCol';
        var intFieldString = "#" + memberNumber + 'PhoneInternationalCol';

        //add them to a list
        var internationalFields = [countryCodeString, intFieldString];

        //north american fields
        var areaCodeString = "#" + memberNumber + 'PhoneAreaCodeCol';
        var firstThreeString = "#" + memberNumber + 'PhonefirstthreeCol';
        var lastFourString = "#" + memberNumber + 'PhonelastfourCol';

        //add them to a list
        var americanFields = [areaCodeString, firstThreeString, lastFourString];

        //reset the phone extension regardless
        $('[name="' + memberNumber + 'PhoneExtension"]').val('');

        // It's international
        if (isInternational) {
            // Hide the north american fields
            americanFields.forEach(function (entry) {
                $(entry).hide().addClass('no-display');
            });

            // Show international fields
            internationalFields.forEach(function (entry) {
                $(entry).show().removeClass('no-display');
                $(entry).find("input[type=text]").val('');
            });
        } else {
            // Hide the international fields
            internationalFields.forEach(function (entry) {
                $(entry).hide().addClass('no-display');
            });

            // Show american fields
            americanFields.forEach(function (entry) {
                $(entry).show().removeClass('no-display');
                $(entry).find("input[type=text]").val('');
            });
        }
    });
    
    $('.is-member-included').on('click', function (event) {
        var on = $(this).is(':checked');

        var id = $(this).attr('id');
        var memberNumber = id.substring(0, 7);

        toggleMemberRegistration(memberNumber, on);
    });

    $('#remove-announcement').on('click', function (event) {
        $('#announcement').hide().addClass('no-display');
    });

    /*(var lis = $('.item');
    var liLength = lis.length;
    if (liLength > 0) {
        var width = 100 / liLength;
        $('.item').css('width', width.toString() + '%');
    }*/

    /*var image = $('#image-banner');
    if (image) {
        image.onload = function () {
            fixBanner();
        };
    }*/



    // Form validation
    $('form').each(function () {

        // Get this form
        var form = $(this);

        // When user submits this form
        form.on('submit', function () {

            // Start off as valid
            var valid = true;

            // Reset classes on form fields
            $('.form-group').removeClass('show-alert');
            $('.form-group .has-error').removeClass('has-error');

            // Check each field
            $('.form-group', form).each(function () {

                // Get current field
                var field = $(this);

                // Check numeric values of fields that need to be a number
                $('.only-numbers', field).each(function () {

                    // Get the value
                    var num = $(this).val();

                    // Check if it's not anumber
                    if (isNaN(num)) {

                        // It's not a number set bool to false so form is not submitted
                        valid = false;

                        // Show help info to tell them why
                        field.addClass('show-alert');

                        // Add error css
                        $(this).parent().addClass('has-error');
                    }
                });

                // Check all the min length fields
                $('.minlength', field).each(function () {

                    // Get the value
                    var value = $(this).val();

                    // Get the max length
                    var maxLength = $(this).attr('maxlength');

                    // Check to see if we should check it
                    if (value.length > 0 || $(this).hasClass('required')) {

                        // Check the length to see if it's correct
                        if (value.length != maxLength) {

                            // Length doesn't match
                            field.addClass('show-alert');

                            // Add error css
                            $(this).parent().addClass('has-error');

                            // Not valid
                            valid = false;
                        }
                    }

                });

                // Check all the required fields for this form
                $('.required', field).each(function () {

                    // Get the value
                    var value = $(this).val();

                    // Check if it exists
                    if (value == null || value == '') {

                        // Add error css
                        $(this).parent().addClass('has-error');

                        // Set boolean
                        valid = false;
                    }

                });

                // Check passowrd
                $('.password', field).each(function () {

                    // Get Password
                    var pw = $(this).val();

                    // Check password
                    pwValid = analyzePassword(pw);

                    // Set valid boolean
                    if (!pwValid) {
                        valid = false;

                        // Add error css
                        $(this).parent().addClass('has-error');

                        // Show help info to tell them why
                        field.addClass('show-alert');

                    }

                });

                // Check email
                $('.email', field).each(function () {

                    // Get Password
                    var email = $(this).val();

                    // Check password
                    emailValid = validateEmail(email);

                    // Set valid boolean
                    if (!emailValid) {
                        valid = false;

                        // Add error css
                        $(this).parent().addClass('has-error');

                        // Show help info to tell them why
                        field.addClass('show-alert');

                    }

                });

            });


            // Check for http:// on website, if not add it
            $('.website', form).each(function () {

                // Get the site
                var site = $(this).val();

                // CHeck if they actually entered a site
                if (site.length > 0) {

                    // Check for http
                    if (site.indexOf("http") == -1) {
                        $(this).val('http://' + site);
                    }
                }


            });

            // If invalid prevent submission
            if (valid != true) {

                // Focus on the first error
                $(".has-error:first input").focus();

                return false;
            }


        });

    });

    // Registration form submission
    $('#registrationForm').on('submit', function (e) {
        e.preventDefault();

        // Submit the registration form data
        $.ajax({
            type: 'POST',
            data: $(this).serialize(),
            url: $(this).attr('action'),
            success: function (data) {

                // clear any errors
                $('#form-container .alert').remove();

                // go to top of form where success message is
                scrollToID('#form-container', 500);

                // Check if it is successful based on the result object that is returned
                if (data.Success == true)
                {

                    successHtml = '<div class="alert alert-success">Thank you for registering your team.</div>';
                    $('#registrationForm').before(successHtml);

                    // reset fields
                    $('#registrationForm').find("input.form-control, textarea").val("");

                }
                else
                {
                    errorHtml = '<div class="alert alert-danger">' + data.ErrorDisplayMessage + '</div>';
                    $('#registrationForm').before(errorHtml);
                }
            }
        });

        return false;
    });

    // When user opens video popup
    $('#video-popup').on('shown.bs.modal', function (e) {
      $('#video-popup .modal-body iframe').attr('src', '//www.youtube.com/embed/-CcVYBECYw0?rel=0&amp;showinfo=0');
    })

    // When user closes video popup
    $('#video-popup').on('hidden.bs.modal', function (e) {
        $('#video-popup .modal-body iframe').attr('src', '');
    })

    // Get article info
    $('.press-article').each(function () {

        // Get this article
        var article = $(this);

        // Get the url
        var url = $(this).attr('data-url');
        if (url) {
          $.ajax({
            url: '/meta?url=' + url,
            type: "GET",
            async: true
          }).done(function (data) {

            // add the description to the dom
            $('.article-description a', article).before(data[0]);

            if (data[1] != null && data[1] != 'No Image Found') {
              // add the image to the dom
              $('.article-image a', article).html('<img src="' + data[1] + '" border="0" alt="" />');
            } else {
              $('.article-image a', article).html('<img src="/images/press.png" border="0" alt="" />');
            }

          })
        }


    });

    // Long judge bios
    $('.judge-bio').readmore();

});

function fixBanner() {
    var pageBreak = $(".page-break");

    var banner = $(".banner-rectangle");
    banner.css('padding-left', pageBreak.css('margin-left'));

    if (document.getElementById("image-banner") != null) {
        //alert('called');
        var imgBanner = $('#image-banner');
        var pixNum = pageBreak.css('margin-left').slice(0, -2) - 120;
        var pix = pixNum.toString() + 'px';
        imgBanner.css('padding-left', pix);
        banner.css('height', imgBanner.css('height'));
        banner.css('width', imgBanner.css('width'));
    }
}

function scrollToID(id, speed) {
    var offSet = 50;
    var targetOffset = $(id).offset().top - offSet;
    var mainNav = $('#main-nav');
    $('html,body').animate({ scrollTop: targetOffset }, speed);
    if (mainNav.hasClass("open")) {
        mainNav.css("height", "1px").removeClass("in").addClass("collapse");
        mainNav.removeClass("open");
    }
}

if (typeof console === "undefined") {
    console = {
        log: function () {
        }
    };
}


// Function to validate email
function validateEmail(email) {
    var re = /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
    return re.test(email);
}