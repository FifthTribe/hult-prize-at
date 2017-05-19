// Fifth Tribe
// Amir Sahib
// 2014

$(function () {

    var loadingAnimationHTML = '<div class="loading-wrap"><div class="loading"><div class="dot"></div><div class="dot2"></div></div></div>';

    // Date and time pickers for add event form
    $('#add-event-form').each(function () {

        // init the datepickers
        $('.form_datetime').datetimepicker({
            weekStart: 1,
            todayBtn: 1,
            autoclose: 1,
            todayHighlight: 1,
            startView: 2,
            forceParse: 0,
            showMeridian: 1
        });

    });

    // When user clicks on edit judge, get judge info and throw up modal with the edit judge form
    $('body').on('click', '.edit-judge', function (e) {
        e.preventDefault();

        // Get the judge id
        var judgeId = $(this).attr('data-id');

        // Check if it exists
        if (judgeId) {

            // Handlebars fun
            var source = $("#edit-judge-template").html();
            var template = Handlebars.compile(source);

            // Get judge info json
            $.post('/school/judge?judgeId=' + judgeId, function (data) {

                // Load html form with data into modal
                $('#edit-judge .modal-body').html(template(data));

                // Show the modal
                $('#edit-judge').modal('show');

            });
        }

    });

    // When user clicks on edit staff, get staff info and throw up modal with the edit judge form
    $('body').on('click', '.edit-staff', function (e) {
        e.preventDefault();

        // Get the judge id
        var staffId = $(this).attr('data-id');

        // Check if it exists
        if (staffId) {

            // Handlebars fun
            var source = $("#edit-staff-template").html();
            var template = Handlebars.compile(source);

            // Get judge info json
            $.post('/school/staff?staffId=' + staffId, function (data) {

                // Load html form with data into modal
                $('#edit-staff .modal-body').html(template(data));

                // Show the modal
                $('#edit-staff').modal('show');


            });
        }

    });

    // When user clicks on edit sponsor, get sponsor info and throw up modal with the edit sponsor form
    $('body').on('click', '.edit-sponsor', function (e) {
        e.preventDefault();

        // Get the sponsor id
        var sponsorId = $(this).attr('data-id');

        // Check if it exists
        if (sponsorId) {

            // Handlebars fun
            var source = $("#edit-sponsor-template").html();
            var template = Handlebars.compile(source);

            // Get sponsor info json
            $.post('/school/sponsor?sponsorId=' + sponsorId, function (data) {

                // Load html form with data into modal
                $('#edit-sponsor .modal-body').html(template(data));

                // Show the modal
                $('#edit-sponsor').modal('show');

            });
        }

    });

    // When user clicks on edit press, get press info and throw up modal with the edit press form
    $('body').on('click', '.edit-press', function (e) {
        e.preventDefault();

        // Get the sponsor id
        var pressId = $(this).attr('data-id');

        // Check if it exists
        if (pressId) {

            // Handlebars fun
            var source = $("#edit-press-template").html();
            var template = Handlebars.compile(source);

            // Get sponsor info json
            $.post('/school/press?pressId=' + pressId, function (data) {

                // Load html form with data into modal
                $('#edit-press .modal-body').html(template(data));

                // Show the modal
                $('#edit-press').modal('show');

            });
        }

    });



    // Activate/Deactivate school page
    $('#activation').on('click', function (e) {
        e.preventDefault();

        // Get the button the user clicked on
        var button = $(this);

        // Status variable for activate/deactivate status
        var status;

        // Check whether the user is activating or deactivating
        if (button.hasClass('active')) {

            // User is activating
            button.text('Deactivate');
            button.addClass("btn-danger").removeClass("btn-success");
            status = true;
        }
        else
        {
            
            // User is deactivating
            button.text('Activate');
            button.removeClass("btn-danger").addClass("btn-success");
            status = false;
        }

        // Toggle active class
        button.toggleClass('active');

        // Get the school id
        var orgId = button.attr('data-id');

        // Activate or deactivate the page
        $.post('/school/activatepage?activate=' + status + '&orgId=' + orgId);

    });

    // Toggle a section of the school page on or off
    $('.is-visible').on('click', function (event) {
        // boolean show or hide
        var show = $(this).is(':checked');

        // which section to apply the toggle to
        var section = $(this).attr('id');

        // the school id of the school this is being applied to
        var id = $(this).attr('name');

        // toggle section
        $.post('/school/togglesection?section=' + section + '&on=' + show + '&id=' + id);
    });


    // When user clicks on edit event, get judge info and throw up modal with the edit judge form
    $('body').on('click', '.edit-event', function (e) {
        e.preventDefault();

        // Get the event id
        var eventId = $(this).attr('data-id');

        // Check if it exists
        if (eventId) {

            // Handlebars fun
            var source = $("#edit-event-template").html();
            var template = Handlebars.compile(source);

            // Get judge info json
            $.post('/school/event?eventId=' + eventId, function (data) {

                // Load html form with data into modal
                $('#edit-event .modal-body').html('').html(template(data));

                // Show the modal
                $('#edit-event').modal('show');

                // Init datepicker
                $('#edit-event .form_datetime').datetimepicker({
                    weekStart: 1,
                    todayBtn: 1,
                    autoclose: 1,
                    todayHighlight: 1,
                    startView: 2,
                    forceParse: 0,
                    showMeridian: 1
                });

            });
        }

    });

    $('.popover-dismiss').popover({
        trigger: 'focus'
    })

    // Image Upload
    $('.image-upload').each(function () {

        // Get this field
        var fileUploadField = $(this);

        // Get this form
        var fileUploadForm = $(this).parents('form');

        // When user selects a file
        fileUploadField.on('change', function () {

            // Get the file name
            var fileName = $(this).val();

            // Validate to make sure it is an image
            if (fileName.split('.').pop() == 'png' || fileName.split('.').pop() == 'PNG' || fileName.split('.').pop() == 'jpg' || fileName.split('.').pop() == 'JPG' || fileName.split('.').pop() == 'jpeg' || fileName.split('.').pop() == 'JPEG') {

                // Submit the form
                fileUploadForm.submit();

            } else {

                // Not Valid
                alert('Only files in the .jpg format are accepted.');
                fileUploadField.val('');
            }

        });


    });

    // Announcements
    $('.update-announcement-status span').on('click', function () {

        // Check if it was clicked
        $(this).parent().toggleClass('active');

    });

    // Team Details
    $('.team-details-btn').on('click', function (e) {

        // Get the ID
        registrationId = $(this).attr('data-id');

        // Handlebars fun
        var source = $("#team-details-template").html();
        var template = Handlebars.compile(source);

        // Get the registration details
        // Get the json list of requirements
        $.ajax({
            type: 'POST',
            url: '/registration/details?registrationId=' + registrationId,
            dataType: 'json',
            cache: false, // 'cache: false' must be present for IE 7 & 8
            success: function (data) {

                // Send the data to the html via handlebars template
                $('#team-details .modal-body').html(template(data));

            }
        });

    });

    // When user updates position of a judge/staff
    $('#judges').on('submit', '.judge-order', function (e) {
        e.preventDefault();

        // Get the form
        var form = $(this);

        // Get Handlebars template ready to update list of judges
        var source = $("#judges-template").html();
        var template = Handlebars.compile(source);

        // Add loading animation
        $('#judges').html(loadingAnimationHTML);

        // Submit form via ajax
        $.ajax({
            type: form.attr('method'),
            url: form.attr('action'),
            dataType: 'json',
            data: form.serialize(),
            cache: false, // 'cache: false' must be present for IE 7 & 8
            success: function (data) {

                $('#judges').html(template(data));

            }
        });

    });


    // When user updates position of a judge/staff
    $('#staff').on('submit', '.judge-order', function (e) {
        e.preventDefault();

        // Get the form
        var form = $(this);

        // Get Handlebars template ready to update list of judges
        var source = $("#staff-template").html();
        var template = Handlebars.compile(source);

        // Add loading animation
        $('#staff').html(loadingAnimationHTML);

        // Submit form via ajax
        $.ajax({
            type: form.attr('method'),
            url: form.attr('action'),
            dataType: 'json',
            data: form.serialize(),
            cache: false, // 'cache: false' must be present for IE 7 & 8
            success: function (data) {

                $('#staff').html(template(data));

            }
        });

    });

    // When user updates position of a sponsor
    $('#sponsors').on('submit', '.judge-order', function (e) {
        e.preventDefault();

        // Get the form
        var form = $(this);

        // Get Handlebars template ready to update list of sponsors
        var source = $("#sponsors-template").html();
        var template = Handlebars.compile(source);

        // Add loading animation
        $('#sponsors').html(loadingAnimationHTML);

        // Submit form via ajax
        $.ajax({
            type: form.attr('method'),
            url: form.attr('action'),
            dataType: 'json',
            data: form.serialize(),
            cache: false, // 'cache: false' must be present for IE 7 & 8
            success: function (data) {

                $('#sponsors').html(template(data));

            }
        });

    });

    // When user updates position of a sponsor
    $('#press').on('submit', '.judge-order', function (e) {
        e.preventDefault();

        // Get the form
        var form = $(this);

        // Get Handlebars template ready to update list of sponsors
        var source = $("#press-template").html();
        var template = Handlebars.compile(source);

        // Add loading animation
        $('#press').html(loadingAnimationHTML);

        // Submit form via ajax
        $.ajax({
            type: form.attr('method'),
            url: form.attr('action'),
            dataType: 'json',
            data: form.serialize(),
            cache: false, // 'cache: false' must be present for IE 7 & 8
            success: function (data) {

                $('#press').html(template(data));

                getPressInfo($('#press'));

            }
        });

    });

    // Find all the tables that need to have the sortable functionality
    $('.table-sortable').each(function () {

        // Make table sortable
        $(this).tablesorter();

    });

    // When user clicks preview to preview the school page
    $('body').on('click','.btn-preview-url', function (e) {
        e.preventDefault();

        // Get the url
        var url = $(this).attr('href');

        // Get the height of the browser and set it to that.
        $('#preview-url').css('height', $(window).height() + 'px');

        // Put it in an iframe in the popup div
        $('.preview-url-iframe').html('<iframe src="' + url + '" style="height:' + $('#preview-url').height()+'px' + '"></iframe>');

        // Show the div
        $('#preview-url').addClass('active');

    });

    // When user clicks the close button on the preview popup
    $('body').on('click', '.btn-close-preview', function (e) {
        e.preventDefault();

        // clear html inside the popup
        $('.preview-url-iframe').html('');

        // Hide the div
        $('#preview-url').removeClass('active');


    });

    // Get Press info
    getPressInfo();

    $('.table-datatable').each(function () {
      $(this).DataTable({
        dom: 'Bfrtip',
        buttons: [
            'copy', 'csv', 'excel', 'pdf', 'print'
        ]
      });
    });

});

function getPressInfo() {
    // Show press info
    $('.press-article').each(function () {

        // Get this article
        var article = $(this);

        // Get the url
        var url = $(this).attr('data-url');
        if (url) {
          $.ajax({
            url: '/School/PressUrlMeta?url=' + url,
            type: "GET",
            async: true
          }).done(function (data) {


            // add the description to the dom
            $('.press-description', article).text(data[0]);

            if (data[1] != null && data[1] != 'No Image Found') {
              // add the image to the dom
              $('.press-image', article).attr('src', data[1]);
            } else {
              $('.press-image', article).html('<img src="http://www.hultprizeat.com/images/press.png" border="0" alt="" />');
            }

          }).fail(function (jqXHR, textStatus, errorThrown) {
            //console.log("AJAX ERROR:", textStatus, errorThrown);
          });
        }





    });
}

Handlebars.registerHelper('if', function (conditional, options) {
    if (conditional) {
        return options.fn(this);
    }
});

Handlebars.registerHelper('ifelse', function (conditional, options) {
    if (conditional) {
        return options.fn(this);
    } else {
        return options.inverse(this);
    }
});

// Helper that checks int boolean if it is false
Handlebars.registerHelper('ifintboolfalse', function (conditional,options) {
    if (conditional == 0) {
        return options.fn(this);
    }
});

// Helper that checks int boolean if it is true
Handlebars.registerHelper('ifintbooltrue', function (conditional, options) {
    if (conditional != 0) {
        return options.fn(this);
    }
});


// Helper to convert a c# date to a js date and then using moment js to format it
Handlebars.registerHelper('convertdate', function (options) {

    // Get the raw date from backend
    var cSharpDateTime = options.fn(this);

    // convert to js date obkect
    var d = new Date(parseInt(cSharpDateTime.substr(6)));

    // Use moment js to nicely format it for display
    return moment(d).format("DD MMMM GGGG - hh:mm a");
});

// This helper checks to whether or not to show the left arrow for moving a judge/staff
Handlebars.registerHelper('dontshowleftarrow', function (displayOrder, options) {

    // Check if it's not the first one
    if (displayOrder != 1) {

        // show the left arrow
        return options.fn(this);
    }

});

// This helper checks to whether or not to show the right arrow for moving a judge/staff
Handlebars.registerHelper('dontshowrightarrow', function (displayOrder, count, options) {

    // Check if it's not the last one
    if (displayOrder != count) {

        // show the right arrow
        return options.fn(this);
    }

});

// This helper checks if an image was uploaded
Handlebars.registerHelper('checkifimageuploaded', function (imageUploaded, options) {

    // Check if it's not the last one
    if (imageUploaded == 1) {

        // show the image
        return options.fn(this);
    }

});

// Helper to convert a c# date to a js date for the hidden input on page load and then using moment js to format it
Handlebars.registerHelper('convertdateinput', function (options) {

    // Get the raw date from backend
    var cSharpDateTime = options.fn(this);

    // convert to js date obkect
    var d = new Date(parseInt(cSharpDateTime.substr(6)));

    // Use moment js to nicely format it for display 2014-09-15 08:57:24
    return moment(d).format("YYYY-MM-DD hh:mm:ss A");
});
