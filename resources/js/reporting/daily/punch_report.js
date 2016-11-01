$(function () {

	var Company = (function ($, w, d) {

		function _renderDropdown(data) {
	        var select_HTML = "",
	        	data = JSON.parse(data.d.return_data)
	            data_length = data.length,
	            counter = 0,
	            $element = $('#company'),
	            $parent = $element.parent();

	        for (counter = 0; counter < data_length; counter += 1) {
	            select_HTML += '<option value="' + data[counter]['company_code'] + '">' + data[counter]['company_name'] + '</option>';
	        }

	        $element.append(select_HTML);
		}
		
		function getCompanyData() {
			return SAXHTTP.AJAX(
				"punch_report.aspx/GetCompanyData", {}
			)
			.done(_renderDropdown)
			.fail(function () {
				SAXAlert.show({type: "error", message: "An error occurred while loading Company data. Please try again."})
			});
		};

		return {
			get: getCompanyData
		};

	}) (jQuery, window, document);

	/************************************************************************************************************************************************/

	var OtherData = (function ($, w, d) {

		var
			$branch = $("#branch"),
			$department = $("#department"),
			$shift = $("#shift");
			$category = $("#category");

		function _render($element, data, key, value, default_data, no_data) {
			var 
				data_length = data.length,
	            select_HTML = '',
	            counter = 0;

            if (default_data != '')
        		select_HTML = '<option value="select">' + default_data + '</option>';

	        if (data_length > 0) {
	            for (counter = 0; counter < data_length; counter += 1) {
	                select_HTML += '<option value="' + data[counter][key] + '">' + data[counter][value] + '</option>';
	            }
	        }
	        else {
	            select_HTML = '<option value="select">' + no_data + '</option>';
	        }
	        // empty any previous data present in the dropdown
	        $element.empty();
	        // append the HTML created in the above steps
	        $element.append(select_HTML);
	        // remove the disabled state of the element
	        $element.prop("disabled", false);
		}

		function getOtherData(company_code) {

			return SAXHTTP.AJAX(
					"punch_report.aspx/GetOtherData",
					{company_code: company_code}
				)
				.done(function (data) {

					var results = JSON.parse(data.d.return_data);
					_render($department, results.department, 'department_name', 'department_name', 'Select Department', 'No Departments found');
		            _render($branch, results.branch, 'branch_name', 'branch_name', '', 'No Branches found');
		            _render($shift, results.shift, 'shift_name', 'shift_name', 'Select Shift', 'No Shift found');
		            _render($category, results.category, 'category_name', 'category_name', 'Select category', 'No category found');
				})
				.fail(function () {
					SAXAlert.show({type: "error", message: "An error occurred while loading Company data. Please try again."});
				});
		}

		return {
			get: getOtherData
		};

	}) (jQuery, window, document);	

	/************************************************************************************************************************************************/

	var Report = (function ($, w, d) {

		var
			$form = $("#filterForm"),
			$company = $("#company"),
			$branch = $("#branch"),
			$button = $("#exportExcelButton");

		function _validateFilters(data) {

	        if (data.from_date == "" || data.to_date == "") {
	            SAXAlert.show({'type': 'error', 'message': 'Please select a From & To Date.'});
	            return false;
	        }

	        if (moment(data.from_date, "DD-MMM-YYYY").unix() > moment(data.to_date, "DD-MMM-YYYY").unix()) {
	            SAXAlert.show({'type': 'error', 'message': 'From Date cannot be greater than the To Date.'});
	            return false;
	        }

	        if (data.company == "select") {
	            SAXAlert.show({'type': 'error', 'message': 'Please select a Company.'});
	            return false;
	        }

	        if (data.employee_id != '' && !SAXValidation.code(data.employee_id)) {
	            SAXAlert.show({'type': 'error', 'message': 'Please enter a valid Employee ID.'});
	            return false;
	        }

	        if (data.employee_name != '' && !SAXValidation.name(data.employee_name)) {
	            SAXAlert.show({'type': 'error', 'message': 'Please enter a valid Employee Name.'});
	            return false;
	        }

	        return true;
	    }

        function _processExport(data) {
        
	        var 
	            status = data.d.status;

	        switch (status) {

	        case 'success':
	            SAXAlert.showAlertBox({'type': status, 'url': SAXUtils.getApplicationURL() + data.d.return_data});
	            break;
	        case 'info':
	            SAXAlert.show({'type': status, 'message': data.d.return_data});
	            break;
	        case 'error':
	            SAXAlert.show({'type': status, 'message': data.d.return_data});
	            break;
	        }
	    }

	    	function _getSelectedBranches() {
	    		var selected_branches = [];

		        $.each($branch.find("option:selected"), function() {
		            selected_branches.push($(this).val());
		        });

		        return selected_branches;
	    	}

		function doExport() {

			var
				filters_data = SAXForms.get($form),
				branches = [];

			if (_validateFilters(filters_data)) {
				// disable button to avoid double click
				$button.button("loading");
				// append company name to selected filters.
				filters_data['company_name'] = $company.find('option:selected').text();
				branches = _getSelectedBranches();

				SAXHTTP.AJAX(
						"punch_report.aspx/DoExport",
						{filters: JSON.stringify(filters_data), branches: JSON.stringify(branches)}
					)
				.done(function (data) {
					_processExport(data);
				})
				.fail(function (data) {
					SAXAlert.show({type: "error", message: "An error occurred while exporting data. Please try again."});
				})
				.always(function () {
					$button.button("reset");
				});
			}
		}

		return {
			export: doExport
		};

	}) (jQuery, window, document);

	/************************************************************************************************************************************************/

	var ReportMasterView = (function ($, w, d) {

		var
			$company = $("#company"),
			$branch = $("#branch")
			button_events = {
				"export/excel": Report.export,
				"filter/branch/reset": _clearBranch
			};

		/* other */
		function _clearBranch() {
			$branch.val("");
		}
		function _initOther() {
			$(".datepicker").Zebra_DatePicker({
				format: 'd-M-Y'
			});

			$company.change(function() {
				OtherData.get($(this).val());
			});
		}

		/* buttons */
			function _buttonHandler(event) {
				var role = $(event.target).data('role');
				button_events[role].call(this, event);
			}

		function _initButtons() {
			$(document).on("click", "[data-control=\"button\"]", _buttonHandler);
		}

		function initialize() {
			_initButtons();
			_initOther();
		}

		return {
			init: initialize
		}

	}) (jQuery, window, document);

	/************************************************************************************************************************************************/
	
	// INITIAL PAGE LOAD
	SAXLoader.show();

	// init master view
	ReportMasterView.init();

	Company.get()
		.done(function () {
			$("#company").prop("disabled", false);
		})
		.always(function() { SAXLoader.close(); });
});