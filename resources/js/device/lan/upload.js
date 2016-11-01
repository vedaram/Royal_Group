var TemplateUpload = (function($, w ,d) {

    var 
        main, _init, _initButtons, _initOther,
        _getData, _success, _failure,
        _getDeviceData, _processDeviceData, _renderDeviceTable,
        _processEnrollmentData, _renderEnrollmentTable,
        _uploadTemplate, _processUploadTemplate,
        _chooseEnrollment, _chooseDevice,
        _loadMoreData;

    var 
        device_data_model_class,
        device_data_collection_class, device_data,
        enrollment_data_model_class,
        enrollment_data_collection_class, enrollment_data;

    var 
        $device_table            = $('#deviceTable'),
        $device_table_parent     = $device_table.parent(),
        $no_device_data          = $('#noDeviceData'),
        $device_tab              = $('#deviceTab'),
        $device_tab_content      = $('#deviceTabOption'),
        $enrollment_table        = $('#enrollmentTable'),
        $enrollment_table_parent = $enrollment_table.parent(),
        $no_enrollment_data      = $('#noEnrollmentData'),
        $enrollment_tab          = $('#enrollmentTab'),
        $enrollment_tab_content  = $('#enrollmentTabOption'),
        $upload_button           = $('#uploadTemplateButton');

    var 
        page_number = 1,
        button_actions = {},
        no_device_data_HTML = '<p><span class="text-orange fa fa-frown-o"></span> <strong>No Device data found.</strong></p>',
        no_enrollment_data_HTML = '<p><span class="text-orange fa fa-frown-o"></span> <strong>No Enrollment data found.</strong></p>';

    /******************************************************************************************************************/

    button_actions = {
        'load-more-data': function () {
            _loadMoreData();
        },
        'upload-template': function () {
            _uploadTemplate();
        },
        'choose-device': function () {
            _chooseDevice();
        },
        'choose-enrollment': function () {
            _chooseEnrollment();
        }
    };

    /******************************************************************************************************************/

    _chooseEnrollment = function () {

        $device_tab.removeClass('active');
        $device_tab_content.removeClass('active');

        $enrollment_tab.addClass('active');
        $enrollment_tab_content.addClass('active');
    };

    _chooseDevice = function () {

        var selected_employees = $('#enrollmentTable tbody').find('input:checked');

        if (selected_employees.length == 0) {
            SAXAlert.show({'type': 'error', 'message': 'Please select atleast one Enrollment.'});
            return false;
        }

        $enrollment_tab.removeClass('active');
        $enrollment_tab_content.removeClass('active');

        $device_tab.addClass('active');
        $device_tab_content.addClass('active');
    };

    /******************************************************************************************************************/

    _processUploadTemplate = function (data, additional) {

        var devices = $('#deviceTable tbody').find('input:checked');

        if (data.status === 'success')
            devices.prop('checked', false);

        $upload_button.button('reset');
        SAXAlert.show({'type': data.status, 'message': data.return_data});
    };

    _uploadTemplate = function () {

        var ajax_options = {},
            employees = [], devices = [],
            selected_employees = $('#enrollmentTable tbody').find('input:checked'),
            selected_employees_length = selected_employees.length,
            selected_devices = $('#deviceTable tbody').find('input:checked'),
            selected_devices_length = selected_devices.length;

        var row_id, current_model;

        if (selected_devices_length == 0) {
            SAXAlert.show({'type': 'error', 'message': 'Please select atleast one Device.'});
            return false;
        }

        for (var i = 0; i < selected_employees_length; i++) { 
            row_id = $(selected_employees[i]).val(); 
            current_model = enrollment_data.get(row_id);
            employees.push(current_model.toJSON());
        };

        for (var i = 0; i < selected_devices_length; i++) { 
            row_id = $(selected_devices[i]).val();
            current_model = device_data.get(row_id);
            devices.push(current_model.toJSON());
        };

        ajax_options = {
            'url': 'upload.aspx/uploadTemplates',
            'data': {employees: JSON.stringify(employees), devices: JSON.stringify(devices)},
            'callback': _processUploadTemplate,
            'additional': {}
        };

        $upload_button.button('loading');
        SAXHTTP.ajax(ajax_options);
    };

    /******************************************************************************************************************/

    _renderEnrollmentTable = function (data) {

        var data_length = data.length, 
            table_HTML = '',
            counter = 0;

        for ( counter = 0; counter < data_length; counter += 1) {

            table_HTML += '<tr id="' + data[counter]['Enrollid'] + '" >' +
                            '<td><input type="checkbox" id="' + data[counter]['Enrollid'] + '" value="' + data[counter]['Enrollid'] + '" ></td>' +
                            '<td>' + data[counter]['Enrollid'] + '</td>' +
                            '<td>' + data[counter]['fingerp1'] + '</td>' +
                            '<td>' + data[counter]['fingerp2'] + '</td>' +
                            '<td>' + data[counter]['Cardid'] + '</td>' +
                            '<td>' + data[counter]['pin'] + '</td>' +
                            '<td>' + data[counter]['Empid'] + '</td>' +
                            '<td>' + data[counter]['Name'] + '</td>' +
                        '</tr>' ;
        }

        $enrollment_table.detach();
        $enrollment_table.find('tbody').append(table_HTML);
        $enrollment_table_parent.prepend($enrollment_table);

    };

    _processEnrollmentData = function (data) {

        var 
            status         = data.status,
            results        = {},
            results_length = 0,
            is_no_data     = $no_enrollment_data.hasClass('hide');

        if (status === 'success') {

            results = JSON.parse(data.return_data);
            results_length = results.length;

            if (results_length > 0) {
                if (!is_no_data) {
                    $no_enrollment_data.empty();
                    $no_enrollment_data.addClass('hide');
                }

                _renderEnrollmentTable(results);

                enrollment_data.set(results);
            }
            else {

                if (is_no_data) {
                    $no_enrollment_data.html(no_enrollment_data_HTML);
                    $no_enrollment_data.removeClass('hide');
                }
            }
        }
        else {

            SAXAlert.show({'type': 'error', 'message': data.return_data});
        }
    };

    /******************************************************************************************************************/

    _renderDeviceTable = function (data) {

        var 
            data_length   = data.length, 
            table_HTML    = '',
            device_status = '',
            counter       = 0;

        for ( counter = 0; counter < data_length; counter += 1) {

            device_status = data[counter]['status'] == "connected" ? '<span class="text-green fa fa-circle"></span>' : '<span class="text-red fa fa-circle"></span>'

            table_HTML += '<tr id="' + data[counter]['deviceid'] + '" >' +
                            '<td><input type="checkbox" id="' + data[counter]['deviceid'] + '" value="' + data[counter]['deviceid'] + '"></td>' +
                            '<td>' + data[counter]['deviceid'] + '</td>' +
                            '<td>' + data[counter]['devicename'] + '</td>' +
                            '<td>' + data[counter]['communation'] + '</td>' +
                            '<td>' + data[counter]['deviceip'] + '</td>' +
                            '<td>' + data[counter]['devicemodel'] + '</td>' +
                            '<td>' + device_status + '</td>' +
                        '</tr>' ;
        }

        $device_table.detach();
        $device_table.find('tbody').append(table_HTML);
        $device_table_parent.append($device_table);
    };

    _processDeviceData = function (data, additional) {

        var status = data.status,
            results = {},
            results_length = 0,
            is_no_data = $no_device_data.hasClass('hide');

        if (status === 'success') {

            results = JSON.parse(data.return_data);
            results_length = results.length;

            if (results_length > 0) {
                if (!is_no_data) {
                    $no_device_data.empty();
                    $no_device_data.addClass('hide');
                }

                _renderDeviceTable(results);
                device_data.set(results);
            }
            else {

                if (is_no_data) {
                    $no_device_data.html(no_device_data_HTML);
                    $no_device_data.removeClass('hide');
                }
            }
        }
        else {

            SAXAlert.show({'type': 'error', 'message': data.return_data});
        }

        if (additional && additional.loading) SAXLoader.close();
    };

    _getDeviceData = function () {

        var ajax_options = {};

        ajax_options = {
            'url': 'upload.aspx/getDeviceData',
            'data': {page_number: page_number},
            'callback': _processDeviceData,
            'additional': {
                'loading': true
            }
        };

        SAXHTTP.ajax(ajax_options);
    };

    /******************************************************************************************************************/

    _failure = function () {

        SAXLoader.close();
        SAXAlert.show({'type': 'error', 'message': 'An error occurred while loading data for this page. Please try again. If the error persists, please contact Support.'});
    };

    _success = function (data1, data2) {

        _processDeviceData(data1[0].d);
        _processEnrollmentData(data2[0].d);
        SAXLoader.close();
    };

    _getData = function () {

        var device_data_URL = 'upload.aspx/getDeviceData',
            enrollment_data_URL = 'upload.aspx/getEnrollmentData',
            promise1 = $.ajax({url: device_data_URL, type: "POST", contentType: 'application/json; charset=utf-8', dataType: 'json', data: JSON.stringify({page_number: page_number}) }),
            promise2 = $.ajax({url: enrollment_data_URL, type: "POST", contentType: 'application/json; charset=utf-8', dataType: 'json', data: JSON.stringify({}) });

        SAXLoader.show();

        $.when(promise1, promise2).then(_success, _failure);
    };

    /******************************************************************************************************************/

    _initOther = function () {
        $('#device_check_all').change(function() {
            var is_checked = $(this).is(':checked'),
                checkboxes = $device_table.find('tbody input[type="checkbox"]');
            is_checked ? $(checkboxes).prop('checked', true) : $(checkboxes).prop('checked', false);
        });

        $('#enrollment_check_all').change(function() {
            var is_checked = $(this).is(':checked'),
                checkboxes = $enrollment_table.find('tbody input[type="checkbox"]');
            is_checked ? $(checkboxes).prop('checked', true) : $(checkboxes).prop('checked', false);
        });
    };

    _initButtons = function () {

        var role = '';

        $('body').on('click', '[data-control="button"]', function (event) {
            role = $(event.target).data('role');
            button_actions[role].call(this, event);
        });
    };

    _init = function () {

        device_data_model_class = SAXModel.extend({
            'idAttribute': 'deviceid'
        });
        device_data_collection_class = SAXCollection.extend({
            'baseModel': device_data_model_class
        });
        device_data = new device_data_collection_class([]);

        enrollment_data_model_class = SAXModel.extend({
            'idAttribute': 'Enrollid'
        });
        enrollment_data_collection_class = SAXCollection.extend({
            'baseModel': enrollment_data_model_class
        });
        enrollment_data = new enrollment_data_collection_class([]);
    };

    main = function () {

        _getData();

        _init();
        _initButtons();

        _initOther();
    };

    return {
        'main': main
    };

})(jQuery, window, document);

$(function() {
    TemplateUpload.main();
});