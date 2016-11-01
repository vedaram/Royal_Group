var ManualPunchApplication = (function() {

    var 
		main, _initOther,
		_saveManualPunch,_changeInpunch, _processSaveManualPunch,
		_doUpload, _processUpload, _processEmployeeData,
		_doImport, _processImport, _onemployeeidchange;

    var 
        $import_box = $("#importLeavesBox"),
		$file_upload = $('#file_upload'),
		$import_manual_punch_button = $('#manualPunchImportButton'),
		$import_results = $('#importManualPunchResults');

    var 
		$employee_id = $('#employee_id'),
		$in_punch_date = $('#punch_in_date'),
		$in_punch_time = $('#punch_in_time'),
		$out_punch_date = $('#punch_out_date'),
		$out_punch_time = $('#punch_out_time'),
		$break_out_date = $('#break_out_date'),
		$break_out_time = $('#break_out_time'),
		$break_in_date = $('#break_in_date'),
		$break_in_time = $('#break_in_time'),
		$save_form = $('#saveManualPunchForm'),
		$modal_body = $('#resultDialog .modal-body'),
		$result_dialog = $('#resultDialog'),
        $saveManualPunchForm = $('#saveManualPunchForm');

    var 
		page_name = 'apply.aspx',
		page = { file_name: "" };


    _processSaveManualPunch = function(data, additional) {

        var 
			status = data.status;

        if (status === 'success') {
            $save_form[0].reset();
              location.reload();
            _setEmployeeID();
        }

        SAXLoader.closeBlockingLoader();
        SAXAlert.show({ 'type': status, 'message': data.return_data });
    };

    _validate = function() {

        var 
			data = SAXForms.get($saveManualPunchForm),
			punch_in_date = data['punch_in_date'],
		    punch_in_time = data['punch_in_time'],
		    punch_out_date = data['punch_out_date'],
		    punch_out_time = data['punch_out_time'];
		    punchStatus=data['status'];
		   
		    //status
        if (punchStatus == 'P') {
            SAXAlert.show({ 'type': 'error', 'message': 'Cant submit Manualpunch for Status Present.' });
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


        return true;
    };
    
    
    _validateInpunch = function() {

        var 
			data = SAXForms.get($saveManualPunchForm),
			punch_in_date = data['punch_in_date'],
		    punch_in_time = data['punch_in_time'],
		    punch_out_date = data['punch_out_date'],
		    punch_out_time = data['punch_out_time'];
		    punchStatus=data['status'];
		    
		    //status

       
         if (punch_in_date === '') {
            SAXAlert.show({ 'type': 'error', 'message': 'Please enter a punch in date.' });
            return false;
        }


        if (punch_in_time === '') {
            SAXAlert.show({ 'type': 'error', 'message': 'Please enter a punch in time.' });
            return false;
        }

        if (punch_out_date != '') {
            SAXAlert.show({ 'type': 'error', 'message': 'We cant move inpunch because outpunch already recorded by device' });
            return false;
        }
        if (punch_out_time !='') {
            SAXAlert.show({ 'type': 'error', 'message': 'We cant move inpunch because outpunch already recorded by device' });
            return false;
        }

        return true;
    };

    _saveManualPunch = function() {

        var 
			ajax_options = {},
			data = SAXForms.get($saveManualPunchForm);

        if (_validate()) {

            ajax_options = {
                'url': page_name + '/SaveManualPunch',
                'data': { current: JSON.stringify(data) },
                'callback': _processSaveManualPunch,
                'additional': {}
            };

            SAXLoader.showBlockingLoader();
            SAXHTTP.ajax(ajax_options);
        }

    };
    //function for Changing from inpunch as outpunch
    _changeInpunch= function() 
    {
           if (_validateInpunch()) 
           {
            var in_punch_date = $('#punch_in_date'),
		        in_punch_time = $('#punch_in_time'),
		        out_punch_date = $('#punch_out_date'),
		        out_punch_time = $('#punch_out_time');
                
             //setting values from inpunch;
             out_punch_date.val(in_punch_date.val());
             out_punch_time.val(in_punch_time.val());
             //making outpuch values readonly;
             out_punch_date.prop('disabled',true);
             event.preventDefault();
             out_punch_time.prop('disabled',true);
             event.preventDefault();
              //making inpunch values readonly false;
             in_punch_date.prop('disabled',false);
             event.preventDefault();
             in_punch_time.prop('disabled',false);
             event.preventDefault();
           }
   

    };
    
    
    _insertPunchDetails = function(data) {

        var 
            result = JSON.parse(data);
              var status = $('#status');
              var moveInpunchBtn= $('#move_inpunch');
             
 
        if (result[0]["In_Punch"] != null) {
             $in_punch_date.val(moment(result[0]["In_Punch"]).format("DD-MMM-YYYY"));
             $in_punch_time.val(moment(result[0]["In_Punch"]).format("HH:mm"));
            //disabling time textbox If textbox values are there 
             $in_punch_date.prop('disabled',true);
             event.preventDefault();
             $in_punch_time.prop('disabled',true);
             event.preventDefault();
             //move_inpunch
             moveInpunchBtn.show();
        }

        if (result[0]["Out_Punch"] != null) {
            $out_punch_date.val(moment(result[0]["Out_Punch"]).format("DD-MMM-YYYY"));
            $out_punch_time.val(moment(result[0]["Out_Punch"]).format("HH:mm"));
                  //disabling time textbox If textbox values are there 
             $out_punch_date.prop('disabled',true);
             event.preventDefault();
             $out_punch_time.prop('disabled',true);
             event.preventDefault();
        }

        if (result[0]["BreakIn"] != null) {
            $break_in_date.val(moment(result[0]["BreakIn"]).format("DD-MMM-YYYY"));
            $break_in_time.val(moment(result[0]["BreakIn"]).format("HH:mm"));
        }

        if (result[0]["BreakOut"] != null) {
            $break_out_date.val(moment(result[0]["BreakOut"]).format("DD-MMM-YYYY"));
            $break_out_time.val(moment(result[0]["BreakOut"]).format("HH:mm"));
        }
        status.val(result[0]["status"]);
         if (result[0]["In_Punch"] != null && result[0]["status"]=="MS") 
         {
           moveInpunchBtn.show();
         }else
         {
            moveInpunchBtn.hide();
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

        $break_in_date.val('');
        $break_in_time.val('');

        $break_out_date.val('');
        $break_out_time.val('');
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

    _setEmployeeID = function() {

        var employee_id = sessvars.TAMS_ENV.user_details.user_name;
        $employee_id.val(employee_id);
    };


    /******************************************************************************************************************/

    _processImport = function(data, additional) {

        var 
            status = data.status,
            message = data.return_data;

        if (status === 'success') {
            $import_results.val(message);
            $file_upload.val('');
            $modal_body.empty().append("<textarea class=\"form-control\" style=\"height: 250px;\" readonly>" + message + "</textarea>");
            $result_dialog.modal("show");
        }
        else {
            SAXAlert.show({ 'type': status, 'message': message });
        }

        $import_manual_punch_button.button('reset');
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

        $import_results.val('Beginning Manual Punch import ...');

        $import_manual_punch_button.button('loading');

        SAXLoader.showBlockingLoader();
        SAXHTTP.ajax(ajax_options);
    };
    /******************************************************************************************************************/

    _processUpload = function(data, additional) {

        var 
            result = JSON.parse(data),
            status = result.status;

        if (status === 'success') {
            $import_manual_punch_button.removeAttr('disabled');
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

    _processEmployeeCheck = function(data, additional) {

        var 
            status = data.status;

        if (status == "success") {
            $employee_id.val(additional.employee_id);
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
                employee_id: employee_id
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
                _getPunchDetailsForEmployee($(this).val())
            }
        });

        $('.timepicker').timepicki({
            show_meridian: false,
            min_hour_value: 0,
            max_hour_value: 23
        });

        var 
        user_access_level = sessvars.TAMS_ENV.user_details.user_access_level;

        if (user_access_level != "2") {
            $("#downloadTemplateButton").removeClass("hide");
            $("#importToggleButton").removeClass("hide");
        }
        $employee_id.change(_onEmployeeIDChange);

        $file_upload.change(function(event) {
            event.preventDefault();
            _doUpload(this);
        });
    };

    _initButtons = function() {

        var role = '',
			button_actions = 
			{
			    'move-inpunch': function() 
			    {
			        _changeInpunch();
			    },
			    'save-leave': function() 
			    {
			        _saveManualPunch();
			    },
			    'import-manual-punch': function() {
			        _doImport();
			    },
			    "import/toggle": function() {
			        $import_box.slideToggle();
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
            //$save_form[0].reset();
            _setEmployeeID();

        }
        else {
            SAXAlert.show({ 'type': 'error', 'message': data.return_data });
            $save_form[0].reset();
            $employee_id.val(sessvars.TAMS_ENV.user_details.user_name);
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
    ManualPunchApplication.main();
});