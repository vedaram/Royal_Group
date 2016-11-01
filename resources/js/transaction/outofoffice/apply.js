var OutOfOfficeApplication = (function() {

    var 
		main, _initOther, outOfOfficeTypes,
		_saveOutOfOffice, _processOutOfOffice,
		_processEmployeeData, _onemployeeidchange;

    var 
		$employee_id = $('#employee_id'),
		$employee_name = $('#employee_name'),
		$in_punch_date = $('#in_date'),
		$in_punch_time = $('#in_time'),
		$out_punch_date = $('#out_date'),
		$out_punch_time = $('#out_time'),
        $out_of_office = $('#outofoffice'),
        $outOfOfficeTypes = $('#outofoffice'),
        $totalDates = $('#total_days'),
        $totalHours = $('#total_hour'),
        $save_form = $('#saveoutofofficeForm'),
        $save_button = $('#saveoutofofficeButton');
    $modal_body = $('#resultDialog .modal-body'),
		$result_dialog = $('#resultDialog'),
        $saveOutOfOfficeForm = $('#saveoutofofficeForm'),
        $ooo_confirmation_dialog = $('#checkOOODialog'),
		$ooo_confirmation_button = $('#confirmOOOButton');

    var 
		page_name = 'apply.aspx',
		page = { file_name: "" };

    /**************************************************************************************************************************************************/
    _processOOOHours = function(data, additional) {

        var 
			status = data.status,
			message = data.return_data;

        if (status === 'confirm-personal-ooo_hours') {
            $ooo_confirmation_dialog.find('.modal-body').html('<p>' + message + '</p>');
            $ooo_confirmation_dialog.modal('show');
        } else if (status === 'success') {
            SAXAlert.show({ 'type': 'success', 'message': message });
            $save_form[0].reset();
            _setEmployeeID();
        } else {
            SAXAlert.show({ 'type': 'error', 'message': message });
        }

        SAXLoader.closeBlockingLoader();
        $save_button.button('reset');
    };

    _CheckPOOOHours = function() {

        var 
			ajax_options = {},
			data = SAXForms.get($saveOutOfOfficeForm);

        if (_validate()) {

            ajax_options = {
                'url': page_name + '/CheckPOOOHours',
                'data': { current: JSON.stringify(data) },
                'callback': _processOOOHours,
                'additional': {}
            }

            $save_button.button('loading');
            SAXLoader.showBlockingLoader();
            SAXHTTP.ajax(ajax_options);
        }
    };

    /**************************************************************************************************************************************************/
    _processOutOfOffice = function(data, additional) {

        var 
			status = data.status;

        if (status === 'success') {
            //$save_form.reset();
            _setEmployeeID();
        }
        // close the loader
        $ooo_confirmation_dialog.modal('hide');
        // close the modal
        SAXLoader.closeBlockingLoader();
        // show the message
        SAXAlert.show({ 'type': status, 'message': data.return_data });
        // resetting the button states
        $ooo_confirmation_button.button('reset');
    };

    _validate = function() {

        var 
			data = SAXForms.get($saveOutOfOfficeForm),
			punch_in_date = data['in_date'],
		    punch_in_time = data['in_time'],
		    punch_out_date = data['out_date'],
		    punch_out_time = data['out_time'],
		    out_of_office = data['outofoffice'],
		    total_hours = data['total_hour'];

        if (out_of_office === "select") {
            SAXAlert.show({ 'type': 'error', 'message': 'Please select out of office.' });
            return false;
        }

        if (punch_in_date === '') {
            SAXAlert.show({ 'type': 'error', 'message': 'Please enter a punch in date.' });
            return false;
        }

        if (punch_in_time === '') {
            SAXAlert.show({ 'type': 'error', 'message': 'Please enter a punch in time.' });
            return false;
        }

        if (punch_out_date === '') {
            SAXAlert.show({ 'type': 'error', 'message': 'Please enter a punch out date.' });
            return false;
        }

        if (punch_out_time === '') {
            SAXAlert.show({ 'type': 'error', 'message': 'Please enter a punch out time.' });
            return false;
        }

        if (total_hours < 15) {
            SAXAlert.show({ 'type': 'error', 'message': 'Minimum out off office hours has to be more than 15 minutes.' });
            return false;
        }

        return true;
    };

    _saveOutOfOffice = function() {

        var 
			ajax_options = {},
			data = SAXForms.get($saveOutOfOfficeForm);

        if (_validate()) {

            ajax_options = {
                'url': page_name + '/SaveOutOfOffice',
                'data': { current: JSON.stringify(data) },
                'callback': _processOutOfOffice,
                'additional': {}
            };

            $ooo_confirmation_button.button('loading');
            SAXLoader.showBlockingLoader();
            SAXHTTP.ajax(ajax_options);
        }

    };

    _insertPunchDetails = function(data) {

        var 
            result = JSON.parse(data);

        if (result[0]["In_Punch"] != null) {
            $in_punch_date.val(moment(result[0]["In_Punch"]).format("DD-MMM-YYYY"));
            $in_punch_time.val(moment(result[0]["In_Punch"]).format("HH:mm"));
        }

        if (result[0]["Out_Punch"] != null) {
            $out_punch_date.val(moment(result[0]["Out_Punch"]).format("DD-MMM-YYYY"));
            $out_punch_time.val(moment(result[0]["Out_Punch"]).format("HH:mm"));
        }

        return result;
    };

    _processPunchDetails = function(data, additional) {

        var 
			status = data.status,
			result = {},
        temp_data;

        if (status === 'success') {
            // beautifying data
            _insertPunchDetails(data.return_data);
        }
        else {
            SAXAlert.show({ 'type': 'error', 'message': data.return_data });
        }

        SAXLoader.closeBlockingLoader();
    };

    _clearPunchDetails = function() {

        $in_punch_time.val('');

        $out_punch_date.val('');
        $out_punch_time.val('');

    };

    _getPunchDetailsForEmployee = function(date) {

        var 
		  employee_code = $employee_id.val();

        ajax_options = {
            'url': page_name + '/GetPunchDetailsForEmployee',
            'data': { employee_code: employee_code, date: date },
            'callback': _processPunchDetails,
            'additional': {}
        };

        _clearPunchDetails();

        SAXLoader.showBlockingLoader();
        SAXHTTP.ajax(ajax_options);
    };


    /******************************************************************************************************************/

    //    _setTotalDays = function(date) {

    //        var total_days;
    //        var a = moment($out_punch_date.val());
    //        var b = moment($in_punch_date.val());
    //        total_days = a.diff(b, 'days') + 1;

    //        $totalDates.val(total_days);

    //    };

    _setToDate = function(date) {
        var 
        selected_type = $outOfOfficeTypes.val();

        if (selected_type != 3) {
            $out_punch_date.val($in_punch_date.val());
            $totalDates.val(1);
        }
        else {
            $out_punch_date.Zebra_DatePicker({
                format: 'd-M-Y',
                direction: [$in_punch_date.val(), false]
                //                onSelect: function() {
                //                    _setTotalDays($(this).val())
                //                }
            });

        }

    };

    /******************************************************************************************************************/

    _setEmployeeID = function() {

        var employee_id = sessvars.TAMS_ENV.user_details.user_name;
        var employee_name = sessvars.TAMS_ENV.user_details.employee_name;
        $employee_id.val(employee_id);
        if (employee_id == 'admin') {
            employee_name = 'admin';
        }
        $employee_name.val(employee_name);

    };


    _processEmployeeCheck = function(data, additional) {

        var 
            status = data.status;

        if (status == "success") {

            employee_data = data.return_data.split(',');

            $employee_id.val(employee_data[0]);
            $employee_name.val(employee_data[1]);
        }
        else {
            SAXAlert.show({ type: "error", message: data.return_data });
            // set the field back to the logged in Employee ID.
            _setEmployeeID();
        }

        SAXLoader.closeBlockingLoader();
    };

    _onEmployeeIDChange = function(event) {

        var 
			ajax_options = {},
			employee_id = $(event.target).val();

        ajax_options = {
            'url': page_name + '/ValidateEmployeeId',
            'data': { employee_id: employee_id },
            'callback': _processEmployeeCheck,
            'additional': {
        }
    };

    SAXLoader.showBlockingLoader();
    SAXHTTP.ajax(ajax_options);
};

_initOther = function() {

    $('.datepicker').Zebra_DatePicker({
        format: 'd-M-Y'
    });

    $in_punch_date.Zebra_DatePicker({
        format: 'd-M-Y',
        onSelect: function() {
            _setToDate($(this).val())
        }
    });

    $out_punch_date.Zebra_DatePicker({
        format: 'd-M-Y'

    });

    $('.timepicker').timepicki({
        show_meridian: false,
        min_hour_value: 0,
        max_hour_value: 23
    });

    $('.datepicker').change(function() {

        var 
            inDate = $in_punch_date.val() + ' ' + $in_punch_time.val(),
            outDate = $out_punch_date.val() + ' ' + $out_punch_time.val();

        in_time = moment(inDate, 'DD-MMM-YYYY HH:mm');
        out_time = moment(outDate, 'DD-MMM-YYYY HH:mm');
        console.log(in_time, out_time);
        if ($.trim(in_time) != '' && $.trim(out_time) != '' && in_time < out_time) {
            $totalHours.val(out_time.diff(in_time, 'minutes'));
            $totalDates.val(out_time.diff(in_time, 'days'));
        }
    });

    $('.timepicker').change(function() {

        var 
            inDate = $in_punch_date.val() + ' ' + $in_punch_time.val(),
            outDate = $out_punch_date.val() + ' ' + $out_punch_time.val();

        in_time = moment(inDate, 'DD-MMM-YYYY HH:mm');
        out_time = moment(outDate, 'DD-MMM-YYYY HH:mm');

        if ($.trim(in_time) != '' && $.trim(out_time) != '' && in_time < out_time) {
            $totalHours.val(out_time.diff(in_time, 'minutes'));
            $totalDates.val(out_time.diff(in_time, 'days'));
        }
    });


    $out_of_office.change(function() {
        var 
        selected_type = $outOfOfficeTypes.val();

        $in_punch_date.val('');
        $out_punch_date.val('');
        $totalDates.val('');

        if (selected_type == 3) {
            $out_punch_date.prop('disabled', false);
        }
        else {
            $out_punch_date.prop('disabled', true);
            $out_punch_date.val('');
        }
    });

    $employee_id.change(_onEmployeeIDChange);

};

_initButtons = function() {

    var role = '',
			button_actions = {
			    'ooo/validate': function() {
			        _CheckPOOOHours();
			    },
			    'ooo/submit': function() {
			        _saveOutOfOffice();
			    }
			};

    $('body').on('click', '[data-control="button"]', function(event) {
        role = $(event.target).data('role');
        button_actions[role].call(this, event);
    });

};

_processEmployeeData = function(data) {
    var 
			status = data.status,
			message = data.return_data;


    if (status === 'success') {

        _setEmployeeID();

    }
    else {
        SAXAlert.show({ 'type': 'error', 'message': data.return_data });
        $save_form[0].reset();
        $employee_id.val(sessvars.TAMS_ENV.user_details.user_name);
        $employee_name.val(sessvars.TAMS_ENV.user_details.employee_name);
    }
};

main = function() {
    // set the Employee ID to same as the logged in user.
    _setEmployeeID();
    _initButtons();
    _initOther();
};

return {
    'main': main
};

})(jQuery, window, document);

$(function() {
    OutOfOfficeApplication.main();
});