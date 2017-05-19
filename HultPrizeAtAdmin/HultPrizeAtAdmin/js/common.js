// Fifth Tribe
// Amir Sahib
// 2014
// Common JS

$(function () {

    // Activate all the tooltips
    $(".has-tooltip").tooltip();

    // Forms with confirm password
    $('.confirm-password-form').each(function () {

        // Check Password strength
        "use strict";
        var options = {};
        options.ui = {
            container: "#pwd-container",
            showVerdictsInsideProgressBar: true,
            viewports: {
                progress: ".pwstrength_viewport_progress"
            }
        };
        options.common = {
            minChar: 8,
            usernameField: "email"
        };
        options.rules = {
            activated: {
                wordLowercase: true,
                wordUppercase: true,
                wordOneNumber: true,
                wordOneSpecialChar: true
            }
        }
        $('.real-password').pwstrength(options);

        // Password confirmation check
        $('#confirmpassword').keyup(function () {

            if ($(this).val() != $('.real-password').val()) {
                $('.password-confirm-message').text('Password does not match.').addClass('label-warning').removeClass('label-success');
            } else {
                $('.password-confirm-message').text('Password matches.').removeClass('label-warning').addClass('label-success');
            }
        });

        // When user is changing country, if not USA remove state dropdown
        $('.country-field select').on('change', function (e) {

            // Get the country
            var country = $(this).val();

            // Check if it's usa
            if (country == 'United States') {

                // Update US State label text
                $('.states-dropdown label').text('State*');

                // Hide the state other field
                $('.states-dropdown input').removeClass('required').hide().val('').attr('name', '').attr('type', 'hidden');

                // US States now required
                $('.states-dropdown select').addClass('required').val('').show().attr('name', 'state');

            } else {

                // Outside US, so remove us state dropdown, us zip code
                $('.states-dropdown select').removeClass('required').hide().attr('name', '').val('');

                // Show the state other field
                $('.states-dropdown input').show().addClass('required').attr('name', 'state').attr('type', 'text').val('');

                // Update state label text
                $('.states-dropdown label').text('State/Region/Province* (International Address)');

            }

        });

    });

    // Check each international number field
    $('.has-international-option').each(function () {

        // Get this field
        var internationalField = $(this);

        // Get the next field which is country code
        var countryCode = internationalField.next('.country-code');

        $('.is-international', internationalField).change(function () {
            var isInternational = $(this).is(':checked');
            $('.is-international-field', internationalField).val(isInternational);

            // Reset phone extension text
            $('[name="phoneExtension"]').val('');

            // It's international
            if (isInternational == true) {

                // Hide the north american fields
                $('.north-american', internationalField).hide();

                // North American fields no longer required
                $('.north-american input', internationalField).val('').removeClass('required');

                // Show international fields
                $('.international', internationalField).show().removeClass('hide').attr("value", "");

                // Reset the international text field text
                $('.international :text', internationalField).val('');

                // Show the country code field
                countryCode.show().removeClass('hide');

                // Reset the text of the country code
                countryCode.find("input[type=text]").val('');

                // International fields are now required
                $('.international input', internationalField).addClass('required');

            } else {

                // Show the north american fields
                $('.north-american', internationalField).show().removeClass('hide').attr("value", "");

                // Reset the north american text fields text
                $('.north-american :text', internationalField).val('');

                // North American fields are now required
                $('.north-american input', internationalField).val('').addClass('required');

                // Hide international fields
                $('.international, .country-code', internationalField).hide();
                countryCode.hide();

                // International fields no longer required
                $('.international input', internationalField).val('').removeClass('required');

            }

        });

    });

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


    // Number only fields
    $('.number-only').on('focusout', function () {

        // Remove any non-numbers
        $(this).val($(this).val().replace(/[A-Za-z$-.,]/g, ""));
    });

    // Number only fields
    $('.number-comma-only').on('focusout', function () {

        // Remove any non-numbers
        $(this).val($(this).val().replace(/[A-Za-z$-]/g, ""));
    });

});

// Function to validate email
function validateEmail(email) {
    var re = /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
    return re.test(email);
}