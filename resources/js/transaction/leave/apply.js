var LeaveApplication = (function($, w, d) {

    var 
		main, _init, _initButtons, _initOther,
		_getData, _success, _failure, _success1,
		_renderTable, _renderDropdown,
		_processLeaveTypesData,
		_processAvailableLeaveData,
		_saveLeave, _processSaveLeave, _validate,
		_doExport, _processExport,
		_doImport, _processImport, _onemployeeidchange, _processEmployeeData;

    var 
		$half_day = $('#half_day'),
		$to_date = $('#to_date'),
		$employee_id = $('#employee_id'),
		$page_actions = $('.page-actions'),
		$leaves_available_table = $('#leavesAvailableTable'),
		$leaves_available_table_parent = $leaves_available_table.parent(),
		$save_form = $('#saveLeaveForm'),
		$save_button = $('#saveLeaveButton');
    $export_button = $('#exportExcelButton'),
		$import_box = $("#importTemplate"),
		$import_form = $("#importTemplateForm"),
		$file_upload = $("#import_leaves"),
		$import_button = $("#importTemplateButton"),
		$import_dialog = $("#importResultDialog"),
		$import_result = $("#importResult"),
		$leave_confirmation_dialog = $('#checkSandwichDialog'),
		$leave_confirmation_button = $('#confirmSandwichButton');

    var 
		page_name = 'apply.aspx',
		available_leaves_data_URL = page_name + '/GetAvailableLeaves',
		leave_types_data_URL = page_name + '/GetLeaveType',
		employee_data_URL = page_name + '/ValidateEmployeeId',
		page = { file_name: "" };

    /******************************************************************************************************************/

    _processImport = function(data, additional) {
        var 
			status = data.status;

        if (status == "success") {
            $file_upload.val("");
            $import_result.val(data.return_data);
            $import_dialog.modal("show");
        }
        else {
            SAXAlert.show({ type: "error", message: data.return_data });
        }

        $import_button.button("reset");
        SAXLoader.closeBlockingLoader();
    };

    _doImport = function() {
        var 
            ajax_options = {
                'url': page_name + '/DoImport',
                'data': { file_name: page.file_name },
                'callback': _processImport,
                'additional': {}
            };

        if ($file_upload.val() == '') {
            SAXAlert.show({ type: 'error', message: 'Please select a file for import' });
            return false;
        }

        $import_result.val('Beginning import of Shift Roster ...');

        $import_button.button('loading');

        SAXLoader.showBlockingLoader();
        SAXHTTP.ajax(ajax_options);
    }

    _processUpload = function(data, additional) {

        var 
            result = JSON.parse(data),
            status = result.status;

        if (status === 'success') {
            $import_button.removeAttr('disabled');
            page.file_name = result.return_data;
        }
        else {
            SAXAlert.show({ 'type': status, 'message': result.return_data });
        }

        SAXLoader.closeBlockingLoader();
    };

    _doUpload = function(that) {

        var uploaded_files = $(that).get(0).files,
            form_data = new FormData(),
            ajax_options = {},
            i = 0,
            employee_code = sessvars.TAMS_ENV.user_details.user_name,
            now = new Date(),
            now = Date.parse(now),
            file_name = employee_code + "-" + now + "-" + uploaded_files[0].name,
            file_extension = file_name.slice((file_name.lastIndexOf(".") - 1 >>> 0) + 2).toLowerCase();

        if (file_extension === 'xls' || file_extension === 'xlsx') {

            form_data.append('file_name', uploaded_files[0]);
            form_data.append('filename', file_name);

            ajax_options = {
                'url': 'FileUpload.ashx',
                'type': 'POST',
                'data': form_data,
                'contentType': false,
                'processData': false,
                'success': _processUpload,
                'error': function() {
                    SAXLoader.closeBlockingLoader();
                    SAXAlert.show({ 'type': 'error', 'message': 'An error occurred while uploading the file. Please try again. If the error persists, please contact Support.' });
                }
            };

            SAXLoader.showBlockingLoader();
            $.ajax(ajax_options);
        }
        else {
            SAXAlert.show({ 'type': 'error', 'message': 'Extension of the file uploaded is incorrect. Please try again.' });
            return false;
        }
    };

    /******************************************************************************************************************/

    _processExport = function(data, additional) {

        var 
            status = data.status,
            type_of_export = additional.type_of_export;

        switch (status) {

            case 'success':
                SAXAlert.showAlertBox({ 'type': status, 'url': SAXUtils.getApplicationURL() + data.return_data });
                break;
            case 'info':
                SAXAlert.show({ 'type': status, 'message': data.return_data });
                break;
            case 'error':
                SAXAlert.show({ 'type': status, 'message': data.return_data });
                break;
        }

        $export_button.button('reset');
        SAXLoader.closeBlockingLoader();
    };

    _doExport = function() {

        var 
            ajax_options = {},
            type_of_export = $(event.target).data('role');

        ajax_options = {
            'url': page_name + '/DoExport',
            'data': {},
            'callback': _processExport,
            'additional': {
                'type_of_export': type_of_export
            }
        };

        $export_button.button('loading');
        SAXLoader.showBlockingLoader();
        SAXHTTP.ajax(ajax_options);
    };

    /******************************************************************************************************************/

    _processSaveLeave = function(data, additional) {

        var 
			status = data.status,
			message = data.return_data;

        if (status === 'success') {
            $save_form[0].reset();
            $employee_id.val(sessvars.TAMS_ENV.user_details.user_name);
            _onemployeeidchange();
        }

        // close the loader
        SAXLoader.closeBlockingLoader();
        // close the modal
        $leave_confirmation_dialog.modal('hide');
        // show the message
        SAXAlert.show({ 'type': status, 'message': message });
        // resetting the button states
        $leave_confirmation_button.button('reset');
    };

    _validate = function() {

        var 
			data = SAXForms.get($save_form),
			from_date = data['from_date'],
			to_date = data['to_date'],
			employee_id = data['employee_id'],
			leave_type = data['leave_type'],
			half_day = data['half_day'];


        if (from_date === '') {
            SAXAlert.show({ 'type': 'error', 'message': 'Please enter a From Date.' });
            return false;
        }

        if (half_day === 0) {

            if (to_date === '') {
                SAXAlert.show({ 'type': 'error', 'message': 'Please enter a To Date.' });
                return false;
            }
        }

        if (moment(from_date) > moment(to_date)) {
            SAXAlert.show({ 'type': 'error', 'message': 'From Date cannot be greater than To Date.' });
            return false;
        }

        if (employee_id === '') {
            SAXAlert.show({ 'type': 'error', 'message': 'Please enter an Employee ID.' });
            return false;
        }

        if (leave_type === 'select') {
            SAXAlert.show({ 'type': 'error', 'message': 'Please select a Leave Type' });
            return false;
        }

        return true;
    };

    _saveLeave = function() {

        var 
			ajax_options = {},
			data = SAXForms.get($save_form);

        ajax_options = {
            'url': page_name + '/SubmitLeave',
            'data': { current: JSON.stringify(data) },
            'callback': _processSaveLeave,
            'additional': {}
        }

        $leave_confirmation_button.button('loading');
        SAXLoader.showBlockingLoader();
        SAXHTTP.ajax(ajax_options);
    };

    _processSandwichSaveLeave = function(data, additional) {

        var 
			status = data.status,
			message = data.return_data;

        if (status === 'confirm-leave-deduction') {
            $leave_confirmation_dialog.find('.modal-body').html('<p>' + message + '</p>');
            $leave_confirmation_dialog.modal('show');
        } else if (status === 'success') 
        {
             SAXAlert.show({ 'type': 'success', 'message': message });
             $save_form[0].reset();
             $employee_id.val(sessvars.TAMS_ENV.user_details.user_name);
             _onemployeeidchange();     
            
        } else {
            SAXAlert.show({ 'type': 'error', 'message': message });
        }

        SAXLoader.closeBlockingLoader();
        $save_button.button('reset');
    };

    _CheckSandwich = function() {

        var 
			ajax_options = {},
			data = SAXForms.get($save_form);

        if (_validate()) {

            ajax_options = {
                'url': page_name + '/CheckForSandwich',
                'data': { current: JSON.stringify(data) },
                'callback': _processSandwichSaveLeave,
                'additional': {}
            }

            $save_button.button('loading');
            SAXLoader.showBlockingLoader();
            SAXHTTP.ajax(ajax_options);
        }
    };

    /******************************************************************************************************************/

    _processLeaveTypesData = function(data) {

        var status = data.status,
            results = {};

        if (status === 'success') {

            results = JSON.parse(data.return_data);
            _renderDropdown(results);
        }
        else {
            SAXAlert.show({ 'type': 'error', 'message': data.return_data });
        }
    };

    _processAvailableLeaveData = function(data) {

        var status = data.status,
            results = {};

        if (status === 'success') {

            results = JSON.parse(data.return_data);
            _renderTable(results);
        }
        else {
            SAXAlert.show({ 'type': 'error', 'message': data.return_data });
        }
    };

    /******************************************************************************************************************/

    _renderDropdown = function(data) {

        var select_HTML = '<option value="select">Select Leave Type</option>',
            data_length = data.length,
            counter = 0,
            $element = $('#leave_type'),
            $parent = $element.parent();

        for (counter = 0; counter < data_length; counter += 1) {

            select_HTML += '<option value="' + data[counter]['LeaveCode'] + '">' + data[counter]['LeaveName'] + '</option>';
        }

        $element.empty().append(select_HTML);
    };

    _renderTable = function(data) {

        var data_length = data.length,
            table_HTML = '',
            counter = 0;

        for (counter = 0; counter < data_length; counter += 1) {

            table_HTML += '<tr id="' + data[counter]['Leavetype'] + '" >' +
                            '<td>' + data[counter]['Leavetype'] + '</td>' +
                            '<td>' + data[counter]['Leave_balance'] + '</td>' +
                        '</tr>';
        }

        $leaves_available_table.detach();
        $leaves_available_table.find('tbody').empty().append(table_HTML);
        $leaves_available_table_parent.prepend($leaves_available_table);
    };

    /******************************************************************************************************************/

    _failure = function() {

        SAXAlert.show({ 'type': 'error', 'message': 'An error occurred while loading data for the page. Please try again. If the error persists, please contact Support.' });
        SAXLoader.close();
    };

    _success = function(data1, data2) {
        _processAvailableLeaveData(data1[0].d);
        _processLeaveTypesData(data2[0].d);


        SAXLoader.close();

    }


    _processEmployeeData = function(data) {
        var 
			status = data.status,
			message = data.return_data;


        if (status === 'success') {
            _getData();
        }
        else {
            SAXAlert.show({ 'type': 'error', 'message': data.return_data });
            $save_form[0].reset();
            $employee_id.val(sessvars.TAMS_ENV.user_details.user_name);
        }
    };

    _getData = function() {

        var 
			employee_id = $employee_id.val(),
			promise1 = {},
			promise2 = {};

        if (employee_id == '') {
            employee_id = sessvars.TAMS_ENV.user_details.user_name;
        }

        promise1 = $.ajax({ url: available_leaves_data_URL, type: "POST", contentType: 'application/json; charset=utf-8', dataType: 'json', data: JSON.stringify({ employee_id: employee_id }) });
        promise2 = $.ajax({ url: leave_types_data_URL, type: "POST", contentType: 'application/json; charset=utf-8', dataType: 'json', data: JSON.stringify({ employee_id: employee_id }) });

        SAXLoader.show();
        $.when(promise1, promise2).then(_success, _failure);
    };

    /******************************************************************************************************************/

    _initOther = function() {

        var 
			current_user_details = sessvars.TAMS_ENV.user_details,
			employee_id = current_user_details.user_name,
			access_level = current_user_details.user_access_level;

        $employee_id.val(employee_id);

        if (access_level != 2) {
            $page_actions.removeClass('hide');

        }

        $('.datepicker').Zebra_DatePicker({
            format: 'd-M-Y'
        });

        $employee_id.change(function() {
            _onemployeeidchange();
        });

        $half_day.change(function() {
            $(this).is(':checked') ? $to_date.prop('disabled', true) : $to_date.prop('disabled', false);
        });

        $file_upload.change(function(event) {
            event.preventDefault();
            _doUpload(this);
        });
    };



    _initButtons = function() {

        var role = '',
			button_actions = {
			    'leave/validate': function() {
			        _CheckSandwich();
			    },
			    'leave/submit': function() {
			        _saveLeave();
			    },
			    'toggle-import': function() {
			        $import_box.slideToggle();
			    },
			    'leave/export': function() {
			        _doExport();
			    },
			    'import-excel': function() {
			        _doImport();
			    }
			};

        $('body').on('click', '[data-control="button"]', function(event) {
            role = $(event.target).data('role');
            button_actions[role].call(this, event);
        });

    };

    _onemployeeidchange = function() {
        var 
			ajax_options = {},
			data = SAXForms.get($save_form)
        employee_id = data['employee_id'];

        $.ajax({
            type: "POST",
            url: employee_data_URL,
            data: JSON.stringify({ employee_id: employee_id }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: successFunc,
            error: _failure
        });

        function successFunc(data) {
            _processEmployeeData(data.d);
        }


    };

    _init = function() {

        available_leaves_model_class = SAXModel.extend({
            'idAttribute': 'Leavetype'
        });
        available_leaves_collection_class = SAXCollection.extend({
            'baseModel': available_leaves_model_class
        });
        available_leaves = new available_leaves_collection_class([]);
    };

    main = function() {

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
	LeaveApplication.main();
});