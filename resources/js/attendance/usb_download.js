var USBDownloadAndProcess = (function ($, w, d) {

    var
        main, _initButtons, _initOther,
        _renderDropDown,
        _getDeviceLocationData, _processDeviceLocationData,
        _doUpload, _processUpload,
        _doImport, _processImport;

    var
        page_name = 'usb_download.aspx',
        file_name = '';

    var
        $file = $('#file'),
        $device = $('#device'),
        $import_button = $('#importLogsButton'),
        $import_result = $('#usbDownloadAndProcessResult');

    /******************************************************************************************************************/

    _processUpload = function (data, additional) {

        var result = JSON.parse(data),
            status = result.status,
            message = result.return_data;

        if (status === 'success') {
            $import_button.removeAttr('disabled');
            file_name = message; 
        }
        else {
            SAXAlert.show({'type': status, 'message': message});
        }

        SAXLoader.closeBlockingLoader();
    };

    _doUpload = function (that) {

        var uploaded_files = $(that).get(0).files,
            form_data = new FormData(),
            ajax_options = {},
            i = 0,
            employee_code = sessvars.TAMS_ENV.user_details.user_name,
            now = new Date(),
            now = Date.parse(now),
            file_name = employee_code + "-" + now + "-" + uploaded_files[0].name,
            file_extension = file_name.slice((file_name.lastIndexOf(".") - 1 >>> 0) + 2).toLowerCase();


        if (file_extension === 'kq' || file_extension === 'dat') {

            form_data.append('file_name', uploaded_files[0]);
            form_data.append('filename', file_name);
            
            ajax_options = {
                'url': 'fileupload.ashx',
                'type': 'POST',
                'data': form_data,
                'contentType': false,
                'processData': false,
                'success': _processUpload,
                'error': function () { 
                    SAXLoader.closeBlockingLoader();
                    SAXAlert.show({'type': 'error', 'message': 'An error occurred while uploading the file. Please try again. If the error persists, please contact Support.'});
                }
            };

            SAXLoader.showBlockingLoader();
            $.ajax(ajax_options);   
        }
        else {
            SAXAlert.show({'type': 'error', 'message': 'Extension of the file uploaded is incorrect. Please try again.'});
            return false;
        }
    };

    /******************************************************************************************************************/

    _processImport = function (data, additional) {

        var
            status = data.status,
            message = data.return_data;

        if (status === 'success') {
            $import_result.val(message);
        }
        else {
            SAXAlert.show({'type': status, 'message': message});
        }

        $import_button.button('reset');
        SAXLoader.closeBlockingLoader();
    };

    _doImport = function () {

        var
            device_id = $device.val(),
            ajax_options = {
                'url': page_name + '/DoImport',
                'data': {file_name: file_name, device_id: device_id},
                'callback': _processImport,
                'additional': {}
            };

        $import_result.val('Starting download of data ...');

        $import_button.button('loading');

        SAXLoader.showBlockingLoader();
        SAXHTTP.ajax(ajax_options);
    };

    /******************************************************************************************************************/

    _processDeviceLocationData = function (data, additional) {

        var 
            status = data.status,
            results = {};

        if (status === 'success') {
            results = JSON.parse(data.return_data);
            _renderDropDown(results);
            $device.prop('disabled', false);
        }
        else {
            SAXAlert.show({'type': status, 'message': data.return_data});
        }

        SAXLoader.close();
    };

    _getDeviceLocationData = function () {

        var 
            ajax_options = {
                'url': page_name + '/GetDeviceLocationData',
                'data': {},
                'callback': _processDeviceLocationData,
                'additional': {}
            };

        SAXLoader.show();
        SAXHTTP.ajax(ajax_options);
    };

    /******************************************************************************************************************/

    _renderDropDown = function (data) {

        var select_HTML = '<option value="select">Select Device Location</option>',
            data_length = data.length,
            counter = 0;

        for (counter = 0; counter < data_length; counter += 1) {
            select_HTML += '<option value="' + data[counter]['deviceid'] + '">' + data[counter]['devicelocation'] + '</option>';
        }

        $device.empty().append(select_HTML);
    };

    /******************************************************************************************************************/

    _initOther = function () {
        $file.change(function (event) {
            event.preventDefault();
            _doUpload(this);
        });
    };

    _initButtons = function () {

        var 
            role = '',
            button_actions = {
                'import-logs': function (event) {
                    _doImport();
                }
            };

        $('body').on('click', '[data-control="button"]', function (event) {
            role = $(event.target).data('role');
            button_actions[role].call(this, event);
        });
    };

    main = function () {

        $device.select2({
            placeholder: "select a device"
        });

        _getDeviceLocationData();
        _initButtons();
        _initOther();
    };

    return {
        'main': main
    };

})(jQuery, window, document);

$(function () {
    USBDownloadAndProcess.main();
});