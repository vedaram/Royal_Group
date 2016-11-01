var TemplateDownload = (function($, w, d) {

    var 
        main, _init, _initButtons, _initDialogs, _initOther,
        _getDeviceData, _processDeviceData, _renderDeviceTable,
        _getEnrollmentData, _processEnrollmentData, _renderEnrollmentTable,
        _downloadTemplate, _processDownloadTemplate,
        _deleteTemplate, _processDeleteTemplate, _deleteDialog;

    var
        delete_dialog_class, delete_dialog;

    var 
        device_data_model_class,
        device_data_collection_class, device_data;

    var 
        $device_table            = $('#deviceTable'),
        $device_table_parent     = $device_table.parent(),
        $enrollment_table        = $('#enrollmentTable'),
        $enrollment_table_parent = $enrollment_table.parent(),
        $pagination              = $('#pagination').parent(),
        $no_device_data          = $('#noDeviceData'),
        $no_enrollment_data      = $('#noEnrollmentData'),
        $delete_dialog           = $('#deleteDialog'),
        $device_tab              = $('#deviceTab'),
        $enrollment_tab          = $('#enrollmentTab'),
        $device_tab_content      = $('#deviceTabOption'),
        $enrollment_tab_content  = $('#enrollmentTabOption'),
        $download_button         = $('#downloadTemplateButton');
        $delete_button           = $('#deleteEnrollmentButton');

    var page_number = 1,
        button_actions = {},
        selected_device = 0,
        no_device_data_HTML = '<p><span class="text-orange fa fa-frown-o"></span> <strong>No Device data found.</strong></p>',
        no_enrollment_data_HTML = '<p><span class="text-orange fa fa-frown-o"></span> <strong>No Enrollment data found.</strong></p>';

    /******************************************************************************************************************/

    button_actions = {
        'download-template': function () {
            _downloadTemplates();
        },
        'delete-template': function (event) {
            _deleteDialog(event);
        },
        'confirm-delete-enrollments': function () {
            _deleteTemplate();
        },
        'choose-device': function () {
            _displayDeviceTab();
        },
        'load-more-device-data': function() {
            _loadMoreData();
        }
    }

    /******************************************************************************************************************/

    _loadMoreData = function () {

        page_number += 1;
        _getDeviceData();
    };

    /******************************************************************************************************************/

    _displayDeviceTab = function () {

        $enrollment_tab.removeClass('active');
        $enrollment_tab_content.removeClass('active');

        $device_tab.addClass('active');
        $device_tab_content.addClass('active');
        
        $enrollment_table.find('tbody').empty();
    };

    /******************************************************************************************************************/

    _processDownloadTemplate = function (data, additional) {

        var employees = $('#enrollmentTable tbody').find('input:checked');

        if (data.status === 'success')
            employees.prop('checked', false);

        $download_button.button('reset');
        SAXAlert.show({'type': data.status, 'message': data.return_data});
    };

    _downloadTemplates = function () {

        var ajax_options = {},
            employees = [],
            selected_employees = $('#enrollmentTable tbody').find('input:checked'),
            selected_employees_length = selected_employees.length;

        if (selected_employees_length === 0) {
            SAXAlert.show({'type': 'error', 'message': 'Please select at least one Enrollment.'});
            return false;
        }

        for (var i = 0; i < selected_employees_length; i++) { 
            employees.push( $(selected_employees[i]).val());
        };

        ajax_options = {
            'url': 'download.aspx/downloadTemplates',
            'data': {device: JSON.stringify(device_data.get(selected_device).toJSON()), employees: JSON.stringify(employees)},
            'callback': _processDownloadTemplate,
            'additional': {}
        };

        $download_button.button('loading');
        SAXHTTP.ajax(ajax_options);
    };

    /******************************************************************************************************************/

    _processDeleteTemplate = function (data, additional) {

        var employees = $('#enrollmentTable tbody').find('input:checked');

        if (data.status === 'success') {
            // clearing the checked employees
            employees.prop('checked', false);

            // remove the existing enrollment data rendered in the table.
            $enrollment_table.find("tbody").empty();

            // hiding the enrollment tab
            $enrollment_tab.removeClass('active');
            $enrollment_tab_content.removeClass('active');

            // display the device tab
            $device_tab.addClass('active');
            $device_tab_content.addClass('active');
        }

        $delete_button.button('reset');
        $delete_dialog.modal("hide");
        SAXAlert.show({'type': data.status, 'message': data.return_data});
    };

    _deleteTemplate = function () {

        var ajax_options = {},
            employees = [],
            selected_employees = $('#enrollmentTable tbody').find('input:checked'),
            selected_employees_length = selected_employees.length;

        if (selected_employees_length.length === 0) {
            SAXAlert.show({'type': 'error', 'message': 'Please select at least one Enrollment.'});
            return false;
        }

        for (var i = 0; i < selected_employees_length; i++) { 
            employees.push( $(selected_employees[i]).val());
        };

        ajax_options = {
            'url': 'download.aspx/deleteEnrollment',
            'data': {device: JSON.stringify(device_data.get(selected_device).toJSON()), employees: JSON.stringify(employees)},
            'callback': _processDeleteTemplate,
            'additional': {}
        };

        $delete_button.button('loading');
        SAXHTTP.ajax(ajax_options);
    };

    _deleteDialog = function () {
        delete_dialog.open();
    };

    /******************************************************************************************************************/

    _renderEnrollmentTable = function (data) {

        var data_length = data.length, 
            table_HTML = '',
            counter = 0;

        for ( counter = 0; counter < data_length; counter += 1) {

            table_HTML += '<tr id="' + data[counter]['EnrollId'] + '" >' +
                            '<td><input type="checkbox" value="' + data[counter]['EnrollId'] + '" id="' + data[counter]['EnrollId'] + '" ></td>' +
                            '<td>' + data[counter]['EnrollId'] + '</td>' +
                            '<td>' + data[counter]['Employeename'] + '</td>' +
                        '</tr>' ;
        }

        $enrollment_table.detach();
        $enrollment_table.find('tbody').append(table_HTML);
        $enrollment_table_parent.prepend($enrollment_table);
    };

    _processEnrollmentData = function (data, additional) {

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

                $device_tab.removeClass('active');
                $device_tab_content.removeClass('active');
                $enrollment_tab.addClass('active');
                $enrollment_tab_content.addClass('active');
            }
            else {

                if (is_no_data) {
                    $no_enrollment_data.html(no_enrollment_data_HTML);
                    $no_enrollment_data.removeClass('hide');
                }

                SAXAlert.show({type: "info", message: "No enrollments found on device."});
            }
        }
        else {

            SAXAlert.show({'type': 'error', 'message': data.return_data});
        }

        SAXLoader.close();
    };

    _getEnrollmentData = function (device_id) {

        var ajax_options = {},
            data = device_data.get(device_id).toJSON();

        selected_device = device_data.get(device_id).get('deviceid');

        ajax_options = {
            'url': 'download.aspx/getEnrollmentData',
            'data': {current: JSON.stringify(data)},
            'callback': _processEnrollmentData,
            'additional': {}
        };

        SAXLoader.show();
        SAXHTTP.ajax(ajax_options);
    };

    /******************************************************************************************************************/

    _renderDeviceTable = function (data, additional) {

        var data_length = data.length, 
            table_HTML = '',
            device_status = '',
            counter = 0;

        for ( counter = 0; counter < data_length; counter += 1) {

            device_status = data[counter]['status'] == "connected" ? '<span class="text-green fa fa-circle"></span>' : '<span class="text-red fa fa-circle"></span>'

            table_HTML += '<tr id="' + data[counter]['deviceid'] + '" >' +
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

        var 
            status         = data.status,
            results        = {},
            results_length = 0,
            is_no_data     = $no_device_data.hasClass('hide'),
            is_pagination  = $pagination.hasClass('hide');

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

                results_length === 30 ? $pagination.removeClass('hide') : $pagination.addClass('hide');
            }
            else {

                if (is_no_data) {
                    $no_device_data.html(no_device_data_HTML);
                    $no_device_data.removeClass('hide');
                }

                if (is_pagination) $pagination.addClass('hide');
            }
        }
        else {

            SAXAlert.show({'type': 'error', 'message': data.return_data});
        }

        SAXLoader.close();
    };

    _getDeviceData = function () {

        var ajax_options = {};

        ajax_options = {
            'url': 'download.aspx/getDeviceData',
            'data': {page_number: page_number},
            'callback': _processDeviceData,
            'additional': {}
        };

        SAXLoader.show();
        SAXHTTP.ajax(ajax_options);
    };

    /******************************************************************************************************************/

    _initOther = function () {

        var device_id = 0;

        $('body').on('click', '#deviceTable tbody tr', function(event) {
            device_id = $(event.target).parent().attr('id');
            _getEnrollmentData(device_id);
        }); 

        $("#chk_all").click(function() {
            var is_checked = $(this).is(':checked'),
                checkboxes = $enrollment_table.find('tbody input[type="checkbox"]');
            is_checked ? $(checkboxes).prop('checked', true) : $(checkboxes).prop('checked', false);
        });
    };

    _initDialogs = function () {

        delete_dialog_class = SAXDialog.extend({
            'element': $delete_dialog
        });
        delete_dialog = new delete_dialog_class();
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
    };

    main = function () {

        _getDeviceData();

        _init();
        _initButtons();
        _initDialogs();

        _initOther();
    };

    return {
        'main': main
    };

})(jQuery, window, document);

$(function () {
    TemplateDownload.main();
});