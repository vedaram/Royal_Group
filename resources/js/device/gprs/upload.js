$(function () {
	
	var Company = (function ($, w, d) {

		function _renderDropdown(data) {
	        var select_HTML = "",
	        	data = JSON.parse(data.d.return_data)
	            data_length = data.length,
	            counter = 0,
	            $element = $('#filter_company'),
	            $parent = $element.parent();

	        for (counter = 0; counter < data_length; counter += 1) {
	            select_HTML += '<option value="' + data[counter]['company_code'] + '">' + data[counter]['company_name'] + '</option>';
	        }

	        $element.append(select_HTML);
		}
		
		function getCompanyData() {
			return SAXHTTP.AJAX(
				"upload.aspx/GetCompanyData", {}
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
			$branch_code           = $('#filter_branch'),
	        $department_code       = $('#filter_department');

		function _render($element, data, key, value, default_data, no_data) {
			var 
				data_length = data.length,
	            select_HTML = '<option value="select">' + default_data + '</option>',
	            counter = 0;

	        if (data_length > 0) {
	            for (counter = 0; counter < data_length; counter += 1) {
	                select_HTML += '<option value="' + data[counter][key] + '">' + data[counter][value] + '</option>';
	            }
	        }
	        else {
	            select_HTML = '<option value="select">' + no_data + '</option>';
	        }
	        $element.empty();
	        $element.append(select_HTML);
		}

		function getOtherData(company_code) {

			return SAXHTTP.AJAX(
					"upload.aspx/GetOtherData",
					{company_code: company_code}
				)
				.done(function (data) {

					var results = JSON.parse(data.d.return_data);
					_render($department_code, results.department, 'department_code', 'department_name', 'Select Department', 'No Departments found');
		            _render($branch_code, results.branch, 'branch_code', 'branch_name', 'Select Branch', 'No Branches found');
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

	var TemplateUpload = (function ($, w, d) {

		var 
			buttons = {
				save: $("#saveButton"),
				delete: $("#deleteButton"),
				reboot: $("#rebootButton")
			},
			list_elements = {
				device: $("#deviceTable"),
				enrollment: $("#dataTable")
			};

		function rebootDevice() {
			var devices = _getDevices();

			if (devices.length == 0) {
				SAXAlert.show({type: "error", message: "Please select at least one device."});
				return false;
			}

			buttons.save.button("loading");
			buttons.delete.button("loading");
			buttons.reboot.button("loading");

			SAXHTTP.AJAX(
					"upload.aspx/RebootDevice",
					{devices: JSON.stringify(devices)}
				)
			.done(function (data) {
				SAXAlert.show({type: data.d.status, message: data.d.return_data});

				list_elements.device.find("tbody input:checked").prop("checked", false);
			})
			.fail(function () {
				SAXAlert.show({type: "error", message: "An error occurred while performing this operation. Please try again."});
			})
			.always(function () {
				buttons.save.button("reset");
				buttons.delete.button("reset");
				buttons.reboot.button("reset");
			})
		}

			function _getEnrollments() {
				var selected_enrollments = list_elements.enrollment.find("tbody input:checked"),
					selected_enrollments_length = selected_enrollments.length,
					final = [],
					i = 0;

				for ( i = 0; i < selected_enrollments_length; i += 1 ) {
					final.push($(selected_enrollments[i]).val());
				}

				return final;
			}
			function _getDevices() {
				var selected_devices = list_elements.device.find("tbody input:checked"),
					selected_devices_length = selected_devices.length,
					final = [],
					i = 0;

				for ( i = 0; i < selected_devices_length; i += 1 ) {
					final.push($(selected_devices[i]).val());
				}

				return final;
			}

		function deleteEnrollments() {
			var 
				enrollments = _getEnrollments(),
				devices = _getDevices();

			if (enrollments.length == 0) {
				SAXAlert.show({type: "error", message: "Please select at least one enrollment"});
				return false;
			}

			if (devices.length == 0) {
				SAXAlert.show({type: "error", message: "Please select at least one device"});
				return false;	
			}

			buttons.save.button("loading");
			buttons.delete.button("loading");
			buttons.reboot.button("loading");

			SAXHTTP.AJAX(
					"upload.aspx/DeleteEnrollments",
					{enrollments: JSON.stringify(enrollments), devices: JSON.stringify(devices)}
				)
			.done(function (data) {
				SAXAlert.show({type: data.d.status, message: data.d.return_data})
			})
			.fail(function () {
				SAXAlert.show({type: "error", message: "An error occurred while deleting enrollments. Please try again."});
			})
			.always(function () {
				buttons.save.button("reset");
				buttons.delete.button("reset");
				buttons.reboot.button("reset");
			});
		}

		function saveEnrollments() {

			var 
				enrollments = _getEnrollments(),
				devices = _getDevices();

			if (enrollments.length == 0) {
				SAXAlert.show({type: "error", message: "Please select at least one enrollment"});
				return false;
			}

			if (devices.length == 0) {
				SAXAlert.show({type: "error", message: "Please select at least one device"});
				return false;	
			}

			buttons.save.button("loading");
			buttons.delete.button("loading");
			buttons.reboot.button("loading");

			SAXHTTP.AJAX(
					"upload.aspx/SaveEnrollments",
					{enrollments: JSON.stringify(enrollments), devices: JSON.stringify(devices)}
				)
			.done(function (data) {
				SAXAlert.show({type: data.d.status, message: data.d.return_data});

				//reset the checkboxes in both tables
				list_elements.enrollment.find("tbody input:checked").prop("checked", false);
				list_elements.device.find("tbody input:checked").prop("checked", false);
			})
			.fail(function () {
				SAXAlert.show({type: "error", message: "An error occurred while saving enrollments. Please try again."});
			})
			.always(function () {
				buttons.save.button("reset");
				buttons.delete.button("reset");
				buttons.reboot.button("reset");
			});
		}

		return {
			save: saveEnrollments,
			delete: deleteEnrollments,
			reboot: rebootDevice
		};

	}) (jQuery, window, document);

	/************************************************************************************************************************************************/

	var DeviceListView = (function ($, w, d) {

		var
			device_data = {},
			$filter_device = $("#filter_device"),
			list_elements = {
				table: $('#deviceTable'),
				message: $('#noDeviceData')
			};

		function filterData() {
			var
				device_id = $.trim($filter_device.val());

			table_body = list_elements.table.find("tbody tr");

			if (device_id != "") {
				table_body.not("tr#" + device_id).addClass("hide");
			}
			else {
				table_body.removeClass("hide");
			}
		}

			function _getHTML(data) {

		        var 
		        	data_length = data.length,
		            table_HTML = '',
		            counter = 0;

		        for ( counter = 0; counter < data_length; counter += 1) {

		            table_HTML += '<tr id="' + data[counter]['device_id'] + '" >' +
		                            '<td><input type="checkbox" value="' + data[counter]['device_id'] + '" ></td>' +
		                            '<td>' + data[counter]['device_id'] + '</td>' +
		                            '<td>' + data[counter]['device_location'] + '</td>' +
		                        '</tr>' ;
		        }

		        return table_HTML;
			}

			function _render(results) { 

				var 
					table_body, 
					data = JSON.parse(results.d.return_data),
					data_length = data.length;

				list_elements.message.children().length > 0 ? list_elements.message.empty() : 0;

				if (data_length > 0) {
					
					table_body = list_elements.table.find("tbody");

					// get the HTML and append to the table.
					table_HTML = _getHTML(data);
					table_body.append(table_HTML);

					device_data = data; 
				}
				else { 
					list_elements.message.append("<h3>No Device data found</h3>");
				}
			}

		function getDeviceData() {

			return SAXHTTP.AJAX(
					"upload.aspx/GetDeviceData",
					{}
				)
				.done(_render)
				.fail(function () {
					SAXAlert.show({type: "error", message: "An error occurred while loading data. Please try again."});
				})
				.always();
		}

		return {
			get: getDeviceData,
			filter: filterData
		};

	}) (jQuery, window, document);

	/************************************************************************************************************************************************/

	var EnrollmentListView = (function ($, w, d) {

		var
			page_number = 1,
			is_filter = false,
			forms = {
				filter: $('#filterForm')
			},
			buttons = {
				pagination: $('#paginationButton'),
			},
			dialogs = {
				filter: $("#enrollmentFilters")
			},
			list_elements = {
				table: $('#dataTable'),
				message: $('#noData')
			};

		function resetFilters() {
	        is_filter = false;
	        page_number = 1;

	        forms.filter[0].reset();
	        SAXLoader.show();

	        list_elements.table.find('tbody').empty();

	        getEnrollmentData()
				.always(function() {
					SAXLoader.close();
					dialogs.filter.slideToggle();
				});
		}

			function _validateFilters() {

		        var data = SAXForms.get(forms.filter);

		        if (data.filter_company_code == "select") {
		            SAXAlert.show({'type': 'error', 'message': 'Please select a Company.'});
		            return false;
		        }

		        if (data.filter_keyword != "" && data.filter_by == 0) {
		            SAXAlert.show({'type': 'error', 'message': 'Please select a Filter By option.'});
		            return false;
		        }

		        if (data.filter_by != 0 && data.filter_keyword == "") {
		            SAXAlert.show({'type': 'error', 'message': 'Please enter a keyword.'});
		            return false;
		        }

		        return true;
		    }

		function filterData() {

	        if (_validateFilters()) {
	            is_filter = true;
	            page_number = 1;

	            SAXLoader.show();

	            list_elements.table.find('tbody').empty();

	            getEnrollmentData()
				.always(function() {
					SAXLoader.close();
					dialogs.filter.slideToggle();
				});
	        }
		}

		/* pagination function */
		function loadMoreData() {
			SAXLoader.show();
			// disable pagination button to avoid multiple clicks
			buttons.pagination.button("loading"); 
			
			page_number += 1;

			getEnrollmentData()
				.always(function () { 
					SAXLoader.close(); 
					buttons.pagination.button("reset"); 
				});
		}

			function _getHTML(data) {

		        var 
		        	data_length = data.length,
		            table_HTML = '',
		            counter = 0;

		        for ( counter = 0; counter < data_length; counter += 1) {

		            table_HTML += '<tr id="' + data[counter]['enroll_id'] + '" >' +
		                            '<td><input type="checkbox" value="' + data[counter]['enroll_id'] + '" ></td>' +
		                            '<td>' + data[counter]['enroll_id'] + '</td>' +
		                            '<td>' + data[counter]['employee_name'] + '</td>' +
		                        '</tr>' ;
		        }

		        return table_HTML;
			}

			function _render(results) { 

				var 
					table_body, 
					data = JSON.parse(results.d.return_data),
					data_length = data.length;

				list_elements.message.children().length > 0 ? list_elements.message.empty() : 0;

				if (data_length > 0) {

					table_body = list_elements.table.find("tbody");

					// get the HTML and append to the table.
					table_HTML = _getHTML(data);
					table_body.append(table_HTML);

					// hiding the pagination button
					table_body.children().length < page_number*30 ? buttons.pagination.hide() : buttons.pagination.show();
				}
				else { 
					list_elements.message.append("<h3>No Enrollment data found</h3>");
				}
			}

		function getEnrollmentData() {

			return SAXHTTP.AJAX(
					"upload.aspx/GetEnrollmentData",
					{page_number: page_number, is_filter: is_filter, filters: JSON.stringify(SAXForms.get(forms.filter))}
				)
				.done(_render)
				.fail(function () {
					SAXAlert.show({type: "error", message: "An error occurred while loading data. Please try again."});
				})
				.always();
		}

		return {
			get: getEnrollmentData,
			more: loadMoreData,
			filter: filterData,
			reset: resetFilters
		};

	}) (jQuery, window, document);

	/************************************************************************************************************************************************/

	var TemplateUploadMasterView = (function ($, w, d) {

		var 		
			$company = $('#filter_company'),
			$enrollment_table = $('#dataTable'),
			$device_table = $("#deviceTable"),
			$enrollment_filters = $("#enrollmentFilters"),
			button_events = {
				"templates/save": TemplateUpload.save,
				"templates/delete": TemplateUpload.delete,
				"device/reboot": TemplateUpload.reboot,
				"device/filter": DeviceListView.filter,
				"filters/data": EnrollmentListView.filter,
				"filters/reset": EnrollmentListView.reset,
				"enrollment/more": EnrollmentListView.more,
				"enrollment/filters": function () {$enrollment_filters.slideToggle();}
			},
			model_class, collection_class, collection;

		/* buttons */
			function _buttonHandler(event) {
				var role = $(event.target).data('role');
				button_events[role].call(this, event);
			}

		function _initButtons() {
			$(document).on("click", "[data-control=\"button\"]", _buttonHandler);
		}

		function _initOther() {
			$company.change(function () {
				var company_code = $(this).val();
				SAXLoader.showBlockingLoader();
				$.when(OtherData.get(company_code))
					.then(SAXLoader.closeBlockingLoader, null)
			});

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
		}

		function initialize() {

			var content_height = $(document).height() - 290;

			$("#templateUpload .card-body").css({"max-height": content_height, "overflow-y": "auto"});

			_initButtons();
			_initOther();
		}

		return {
			init: initialize
		};

	}) (jQuery, window, document);

	/************************************************************************************************************************************************/

	// INITIAL PAGE LOAD
	SAXLoader.show();

	TemplateUploadMasterView.init();

	Company.get()
		.done(function () {

			EnrollmentListView.get()
				.done(function () {

					DeviceListView.get()
						.always(SAXLoader.close);
				})
				.fail(SAXLoader.close);
		})
		.fail(SAXLoader.close);
});