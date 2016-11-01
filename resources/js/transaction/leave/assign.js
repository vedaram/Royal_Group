var LeaveAssign = (function ($, w, d) {

	var
		main, _init, _initButtons, _initDialog, _initOther,
		_getData, _success, _failure,
		_getEmployeeData, _processEmployeeData, _renderTable,
		_renderDropdown,
		_processCompanyData,
		_getOtherData, _processOtherData,
		_getLeavesForEmployee, _processLeavesForEmployee, _populateLeaveTable,
		_saveLeavesForEmployee, _processLeaveSave, _getLeaveDataFromTable,
		_loadMoreData,
		_filterData, _resetFilters, _validateFilters,
		_doImport, _processImport,
		_doUpload, _processUpload,
		_showEmployeeTab, _showLeaveTab,
		_fillSelectedEmployeeDetails;

	var
		employee_data_model_class = {},
		employee_data_collection_class = {},
		employee_data = {};

	var 
		import_dialog_class, import_dialog;

    var 
		$employee_tab_option        = $('#employeeTabOption'),
		$employee_tab               = $('#employeeTab'),
		$employee_table             = $('#employeeTable'),
		$employee_table_parent      = $employee_table.parent(),
		$no_data                    = $('#noData'),
		$pagination                 = $('#pagination').parent(),
		$filters_box                = $('#employeeFilters'),
		$filters_form               = $('#employeeFiltersForm'),
		$leave_tab_option           = $('#leaveTabOption'),
		$leave_tab                  = $('#leaveTab'),
		$leave_table                = $('#leaveTable'),
		$leave_table_parent         = $leave_table.parent(),
		$import_box                 = $('#importLeavesBox'),
		$import_form                = $('#importLeavesForm'),
		$import_button              = $('#importLeavesButton'),
		$import_dialog              = $('#importResultDialog'),
		$import_result              = $('#importResult'),
		$update_button              = $('#updateLeavesButton'),
		$company                    = $('#filter_company'),
		$branch                     = $('#filter_branch'),
		$category                   = $('#filter_employee_category'),
		$department                 = $('#filter_department'),
		$designation                = $('#filter_designation'),
		$shift                      = $('#filter_shift'),
		$file_upload                = $('#file_upload'),
		$selected_employee_id       = $('#selected_employee_id'),
		$selected_employee_name     = $('#selected_employee_name'),
		$selected_employee_category = $('#selected_employee_category');


	var
		page_name = 'assign.aspx',
		company_data_URL = page_name + '/GetCompanyData',
		employee_data_URL = page_name + '/GetEmployeeData',
		no_data_HTML = '<p><span class="text-orange fa fa-frown-o"></span> <strong>No Employee data found.</strong></p>'
		page_number = 1,
		is_filter = false,
		selected_employee = 0,
		file_name = '';

	/******************************************************************************************************************/

	_loadMoreData = function () {

        page_number += 1;
        SAXLoader.showBlockingLoader();
        _getEmployeeData();
	};

 	/******************************************************************************************************************/

 	_showEmployeeTab = function () {

 		$leave_tab_option.removeClass('active');
 		$leave_tab.removeClass('active');

 		$employee_tab_option.addClass('active');
 		$employee_tab.addClass('active');

 		selected_employee = 0;
 	};

 	_showLeaveTab = function () {

 		$employee_tab_option.removeClass('active');
 		$employee_tab.removeClass('active');

 		$leave_tab_option.addClass('active');
 		$leave_tab.addClass('active');
 	};

 	/******************************************************************************************************************/
    
    _processImport = function (data, additional) {

        var
            status = data.status,
            message = data.return_data;

        if (status === 'success') {
            $file_upload.val("");
            $import_result.val(message);
            $import_dialog.modal("show");
        }
        else {
            SAXAlert.show({'type': status, 'message': message});
        }

        $import_button.button('reset');
        SAXLoader.closeBlockingLoader();
    };

 	_doImport = function () {

 		var
 			ajax_options = {
 				'url': page_name + '/DoImport',
 				'data': {file_name: file_name},
 				'callback': _processImport,
 				'additional': {}
 			};
        
         if ($file_upload.val() == '') {
            SAXAlert.show({ type: 'error', message: 'Please select a file for import' });
            return false;
        }
        
        $import_result.val('Beginning import of Leaves ...');

        $import_button.button('loading');
        
		SAXLoader.showBlockingLoader();
		SAXHTTP.ajax(ajax_options);
 	};

 	/******************************************************************************************************************/

    _resetFilters = function () {

        is_filter = false;
        page_number = 1;

        $filters_form[0].reset();
        $employee_table.find('tbody').empty();

        SAXLoader.showBlockingLoader();
        _getEmployeeData();
    };

    _validateFilters = function () {

        var data = SAXForms.get($filters_form);

        if(data.filter_company == "select") {
            SAXAlert.show({'type': 'error', 'message': 'Please select a Company.'});
            return false;
        }

        return true;
    }

    _filterData = function () {

        if (_validateFilters()) {
            is_filter = true;
            page_number = 1;
            
            SAXLoader.showBlockingLoader();

            $employee_table.find('tbody').empty();
            _getEmployeeData();
        }
    };

	/******************************************************************************************************************/

	_processLeaveSave = function (data, additional) {

		var
			status = data.status,
			message = '';

		if (status === 'success') {
			selected_employee = 0;
			message = JSON.parse(data.return_data);
		}
		else {
			message = data.return_data;
		}
		
		$update_button.button('reset');
		SAXLoader.closeBlockingLoader();
		SAXAlert.show({'type': status, 'message': message});	
	};

	_getLeaveDataFromTable = function () {

		var 
			leave_name, leave_code,
			max_leave, leave_applied, leave_balance, 
			$row, final_data = [];

		$leave_table.find('tbody tr').each(function (i, row) {

			$row = $(row),
			leave_code = $row.find('td:nth-child(1)').text(),
			leave_name = $row.find('td:nth-child(2)').text(), 
			max_leave = $.trim($row.find('td:nth-child(3) input').val()),
			leave_applied = $.trim($row.find('td:nth-child(4) input').val()),
			leave_balance = $.trim($row.find('td:nth-child(5) input').val());

			if (max_leave === '' || leave_applied === '' ) {
				SAXAlert.show({'type': 'error', 'message': 'Please enter a value for Max Leave and Leaves Applied.'});
				return false;
			}

			if (isNaN(max_leave) || isNaN(leave_applied)) {
				SAXAlert.show({'type': 'error', 'message': 'Please enter a numeric value for Max Leaves & Leaves Applied'});
				return false;
			}

			final_data.push({ leave_code: leave_code, leave_name: leave_name, max_leave: max_leave, leave_applied: leave_applied, leave_balance: leave_balance});
		});

		return final_data
	};

	_saveLeavesForEmployee = function () {

		var
			ajax_options = {},
			leave_data;

		leave_data = _getLeaveDataFromTable();

		if (leave_data.length > 0) {

			ajax_options = {
				'url': page_name + '/DoAction',
				'data': {employee_id: selected_employee, leave_data: JSON.stringify(leave_data)},
				'callback': _processLeaveSave,
				'additional': {}
			};

			$update_button.button('loading');
			SAXLoader.showBlockingLoader();
			SAXHTTP.ajax(ajax_options);
		}
	};

 	/******************************************************************************************************************/

 	_populateLeaveTable = function (data) {

        var data_length = data.length, 
            table_HTML = '',
            counter = 0,
            max_leave, leaves_applied, leaves_balance;

        for ( counter = 0; counter < data_length; counter += 1) {

        	max_leave = data[counter]['Max_leaves'] == null ? 0 : +data[counter]["Max_leaves"];
        	leaves_applied = data[counter]['Leaves_applied'] == null ? 0 : +data[counter]['Leaves_applied'];
        	leaves_balance = data[counter]['Leave_balance'] == null ? 0 : +data[counter]['Leave_balance'];

            table_HTML += '<tr id="' + data[counter]['LeaveCode'] + '" data-value="' + data[counter]['LeaveCode'] + '" >' +
                            '<td>' + data[counter]['LeaveCode'] + '</td>' +
                            '<td>' + data[counter]['LeaveName'] + '</td>' +
                            '<td><input type="text" class="form-control" value="' + max_leave + '" data-type="max-leave" /></td>' +
                            '<td><input type="text" class="form-control" value="' + leaves_applied + '" data-type="leaves-applied" /></td>' +
                            '<td><input type="text" class="form-control" value="' + leaves_balance + '" disabled /></td>' +
                        '</tr>' ;
        }

        $leave_table.detach();
        $leave_table.find('tbody').empty().append(table_HTML);
        $leave_table_parent.prepend($leave_table);
 	};

 	_fillSelectedEmployeeDetails = function (employee_id) {

 		var 
 			selected_employee_details = employee_data.get(employee_id).toJSON(),
 			selected_employee_id = selected_employee_details['EmployeeID'],
 			selected_employee_name = selected_employee_details['EmployeeName'],
 			selected_employee_category = selected_employee_details['EmployeeCategory'];

		$selected_employee_id.val(selected_employee_id);
		$selected_employee_name.val(selected_employee_name);
		$selected_employee_category.val(selected_employee_category);
 	};

 	_processLeavesForEmployee = function (data, additional) {

 		var
 			status = data.status,
 			results = {};

		if (status === 'success') {

			results = JSON.parse(data.return_data); 

			selected_employee = additional.employee_id;

			_populateLeaveTable(results);

			_fillSelectedEmployeeDetails(selected_employee);
			_showLeaveTab();
		}
		else {
			SAXAlert.show({'type': status, 'message': data.return_data});
		}

		SAXLoader.closeBlockingLoader();
 	};
 
 	_getLeavesForEmployee = function (employee_id) {

 		var 
 			employee_category = employee_data.get(employee_id).toJSON()["EmployeeCategory"],
 			ajax_options = {
 				'url': page_name + '/GetLeavesForEmployee',
 				'data': {employee_id: employee_id, employee_category: employee_category},
 				'callback': _processLeavesForEmployee,
 				'additional': {
 					'employee_id': employee_id,
 					'employee_category': employee_category
 				}
 			};

		SAXLoader.showBlockingLoader();
		SAXHTTP.ajax(ajax_options);
 	};

 	/******************************************************************************************************************/

 	_renderTable = function (data) {

        var data_length = data.length, 
            table_HTML = '',
            counter = 0;

        for ( counter = 0; counter < data_length; counter += 1) {

            table_HTML += '<tr id="' + data[counter]['EmployeeID'] + '" data-value="' + data[counter]['EmployeeID'] + '" >' +
                            '<td>' + data[counter]['EmployeeID'] + '</td>' +
                            '<td>' + data[counter]['EmployeeName'] + '</td>' +
                        '</tr>' ;
        }

        $employee_table.detach();
        $employee_table.find('tbody').append(table_HTML);
        $employee_table_parent.prepend($employee_table);
 	}

 	_processEmployeeData = function (data, additional) {

 		var status = data.status,
            results = {},
            results_length = 0,
            is_no_data = $no_data.hasClass('hide'),
            is_pagination = $no_data.hasClass('hide');

        if (status === 'success') {

            results = JSON.parse(data.return_data);
            results_length = results.length;

            if (results_length > 0) {

                if (!is_no_data) {
                    $no_data.empty();
                    $no_data.addClass('hide');
                }
                // Pass data to be rendered in the UI.
                _renderTable(results);
                employee_data.set(results);
                results_length === 30 ? $pagination.removeClass('hide') : $pagination.addClass('hide');
            }
            else {

                if (is_no_data) {
                    $no_data.html(no_data_HTML);
                    $no_data.removeClass('hide');
                }

                if (!is_pagination) $pagination.addClass('hide');
            }

        } 
        else {

            SAXAlert.show({'type': 'error', 'message': data.return_data});
        }

        if (additional && additional.loading) SAXLoader.closeBlockingLoader();
 	};

	_getEmployeeData = function () {

		var
			filters = SAXForms.get($filters_form);
			ajax_options = {
				'url': page_name + '/GetEmployeeData',
				'data': {page_number: page_number, is_filter: is_filter, filters: JSON.stringify(filters)},
				'callback': _processEmployeeData,
				'additional': {
					'loading': true
				}
			};

		SAXHTTP.ajax(ajax_options);
	};

	/******************************************************************************************************************/

	_processOtherData = function (data, additional) {

        var status = data.status,
            results = {},
            branch_data = {},
            department_data = {},
            shift_data = {};

        if (status === 'success') {

            results = JSON.parse(data.return_data);
            
            // Rendering the results in a dropdown.            
            branch_data = results['branch'];
            _renderDropdown($branch, branch_data, 'BranchCode', 'BranchName', 'All Branches'); 
            // Enable the dropdown after rendering the data
            $branch.prop('disabled', false);

            // Rendering the results in a dropdown.
            department_data = results['department'];
            _renderDropdown($department, department_data, 'DeptCode', 'DeptName', 'All Departments');
            // Enable the dropdown after rendering the data
            $department.prop('disabled', false);

            // Rendering the results in a dropdown.
            shift_data = results['shift'];
            _renderDropdown($shift, shift_data, 'Shift_Code', 'Shift_Desc', 'All Shifts');
            // Enable the dropdown after rendering the data
            $shift.prop('disabled', false);

            // Rendering the results in a dropdown.
            category_data = results['category'];
            _renderDropdown($category, category_data, 'EmpCategoryCode', 'EmpCategoryName', 'All Employee Categories');
            // Enable the dropdown after rendering the data
            $category.prop('disabled', false);

            // Rendering the results in a dropdown.
            designation_data = results['designation'];
            _renderDropdown($designation, designation_data, 'DesigCode', 'DesigName', 'All Designation');
            // Enable the dropdown after rendering the data
            $designation.prop('disabled', false);
        }
        else {
            SAXAlert.show({'type': 'error', 'message': data.return_data});
        }

        SAXLoader.closeBlockingLoader();
	};

	_getOtherData = function (company_code) {

		var
			ajax_options = {
				'url': page_name + '/GetOtherData',
				'data': {company_code: company_code},
				'callback': _processOtherData,
				'additional': {}
			};

		SAXLoader.showBlockingLoader();
		SAXHTTP.ajax(ajax_options);
	};

	/******************************************************************************************************************/

	_processCompanyData = function (data) {

        var status = data.status,
            results = {};

        if (status === 'success') { 

            results = JSON.parse(data.return_data);
            _renderDropdown($company, results, 'CompanyCode', 'CompanyName', 'Select a Company', 'No data found');
            $company.prop('disabled', false);
        }
        else {
            SAXAlert.show({ 'type': 'error', 'message': data.return_data});
        }    
	};

	/******************************************************************************************************************/

	_renderDropdown = function ($element, data, key, value, default_text, no_data) {

        var 
            data_length = data.length,
            select_HTML = '',
            counter = 0 ;

        if (data.length === 0) {
            select_HTML = '<option value="select">' + no_data + '</option>';
        }
        else {

            select_HTML = '<option value="select">' + default_text + '</option>';

            for (counter = 0; counter < data_length; counter += 1) {
                select_HTML += '<option value="' + data[counter][key] + '">' + data[counter][value] + '</option>';
            }
        }

        $element.empty().append(select_HTML); 
	};

	/******************************************************************************************************************/

	_failure = function () {

		SAXLoader.closeBlockingLoader();
		SAXAlert.show({'type': 'error', 'message': 'An error occurred while loading data for the page. Please try again. If the error persists, please contact Support.'});
	};

	_success = function (data1, data2) {

		_processCompanyData(data1[0].d);
		_processEmployeeData(data2[0].d);

		SAXLoader.closeBlockingLoader();
	};

	_getData = function () {

        var filters = SAXForms.get($filters_form),
            data2 = {page_number: page_number, is_filter: is_filter, filters: JSON.stringify(filters)},
            promise1 = $.ajax({url: company_data_URL, type: "POST", contentType: 'application/json; charset=utf-8', dataType: 'json', data: JSON.stringify({}) }),
            promise2 = $.ajax({url: employee_data_URL, type: "POST", contentType: 'application/json; charset=utf-8', dataType: 'json', data: JSON.stringify(data2) });

        SAXLoader.showBlockingLoader();
        $.when(promise1, promise2).then(_success, _failure);
	};

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


        if (file_extension === 'xlsx' || file_extension === 'xls') {

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

	_initOther = function () {

		var employee_id = 0;

		$employee_table.on('click', 'tbody tr', function () {
			employee_id = $(this).data('value');
			_getLeavesForEmployee(employee_id);
		});

		$company.change(function () {
			_getOtherData($(this).val());
		});
               
		$file_upload.change(function (event) {
			event.preventDefault();
            _doUpload(this);
		});
		
		$(document).on("change","[data-type=\"leaves-applied\"]", function (event) {
           $this = $(this);
           var leaves_applied = $this.val();
           var parent_row = $this.closest("tr");
           var max_leaves = parent_row.find(":nth-child(3) input").val();
           var balance = max_leaves - leaves_applied;
           parent_row.find(":nth-child(5) input").val(balance);
        });

       $(document).on("change", "[data-type=\"max-leave\"]", function (event) {
           $this = $(this);
           var max_leaves = $this.val();
           var parent_row = $this.closest("tr");
           var leaves_applied = parent_row.find(":nth-child(4) input").val();
           var balance = max_leaves - leaves_applied;
           parent_row.find(":nth-child(5) input").val(balance);
          });
        };
	
	

	_initDialog = function () {

		import_dialog_class = SAXDialog.extend({
			'element': $import_dialog
		});
		import_dialog = new import_dialog_class();
	};
	
	_initButtons = function () {

		var 
			role = '',
			button_actions = {
				'toggle-filters': function () {
					$filters_box.slideToggle();
				},
				'import-leaves': function () {
					_doImport();
				},
				'toggle-import': function () {
					$import_box.slideToggle();
				},
				'filter-data': function () {
					_filterData();
				},
				'reset-filters': function () {
					_resetFilters();
				},
				'load-more-data': function () {
					_loadMoreData();
				},
				'update-leave': function () {
					_saveLeavesForEmployee();
				},
				'select-employee': function () {
					_showEmployeeTab();
				}
			};

		$('body').on('click', '[data-control="button"]', function (event) {
			event.preventDefault();
			role = $(event.target).data('role');
			button_actions[role].call(this, event);
		});
	};

	_init = function () {

		employee_data_model_class = SAXModel.extend({
			'idAttribute': 'EmployeeID'
		});
		employee_data_collection_class = SAXCollection.extend({
			'baseModel': employee_data_model_class
		});
		employee_data = new employee_data_collection_class([]);
	};

	main = function () {

		_getData();

		_init();
		_initButtons();
		_initDialog();

		_initOther();
	};

	return {
		'main': main
	};

})(jQuery, window, document);

$(function () {
	LeaveAssign.main();
});