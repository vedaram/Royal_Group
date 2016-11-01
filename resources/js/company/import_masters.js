$(function () {
	
	var ImportMasterView = (function ($, w, d) {

		var
			upload_elements = {
				employee_file_import: $("#employee_file_upload"),
				company_file_import: $("#company_file_upload"),
				transaction_file_import: $("#transaction_file_upload")
			},
			upload_results = {
				employee: $("#importEmployeeResult"),
				company: $("#importCompanyResult"),
				employeetransaction: $("#importEmployeeTransactionResult")
			},
			uploads = {
				file: ""
			},
			buttons = {
				company: $("#importCompanyButton"),
				employee: $("#importEmployeeButton"),
				transaction: $("#importTransactionButton")
			},
			button_events = {
				"import/company": _companyImport,
				"import/employee": _employeeImport,
				"import/transaction": _transactionImport
			};

			function _employeeImport(event) {

				if (upload_elements.employee_file_import.val() == "") {
					SAXAlert.show({type: "error", message: "Please select a file for upload"});
					return false;
				}
				// disable the button to avoid multiple clicks
				buttons.employee.button("loading");
				// clear the results box
				upload_results.employee.val("");
				SAXHTTP.AJAX(
						"import_masters.aspx/DoEmployeeImport",
						{file_name: uploads.file}
					)
					.done(function (data) {
						upload_results.employee.val(data.d.return_data);
					})
					.fail(function () {
						SAXAlert.show({type: "error", message: "An error occurred while import Employee Master. Please try again."});
					})
					.always(function () { 
						buttons.employee.button("reset");
						upload_elements.employee_file_import.val("");
					})
			}

			function _companyImport(event) {

				if (upload_elements.company_file_import.val() == "") {
					SAXAlert.show({type: "error", message: "Please select a file for upload"});
					return false;
				}
				// disable the button to avoid multiple clicks
				buttons.company.button("loading");
				// clear the results box
				upload_results.company.val("");
				SAXHTTP.AJAX(
						"import_masters.aspx/DoCompanyImport",
						{file_name: uploads.file}
					)
					.done(function (data) {
						upload_results.company.val(data.d.return_data);
					})
					.fail(function () {
						SAXAlert.show({type: "error", message: "An error occurred while import Company Master. Please try again."});
					})
					.always(function () {
						buttons.company.button("reset");
						upload_elements.company_file_import.val("")
					});
			}
			
			function _transactionImport(event) {

				if (upload_elements.transaction_file_import.val() == "") {
					SAXAlert.show({type: "error", message: "Please select a file for upload"});
					return false;
				}
				// disable the button to avoid multiple clicks
				buttons.transaction.button("loading");
				// clear the results box
				upload_results.employeetransaction.val("");
				SAXHTTP.AJAX(
						"import_masters.aspx/DoImport",
						{file_name: uploads.file}
					)
					.done(function (data) {
						upload_results.employeetransaction.val(data.d.return_data);
					})
					.fail(function () {
						SAXAlert.show({type: "error", message: "An error occurred while import Company Master. Please try again."});
					})
					.always(function () {
						buttons.transaction.button("reset");
						upload_elements.transaction_file_import.val("")
					});
			}

			function _doUpload(event) {

				var 
					uploaded_files = $(event.target).get(0).files,
		            form_data = new FormData(),
		            employee_code = sessvars.TAMS_ENV.user_details.user_name,
		            now = new Date(),
		            now = Date.parse(now),
		            file_name = employee_code + "-" + now + "-" + uploaded_files[0].name,
		            file_extension = file_name.slice((file_name.lastIndexOf(".") - 1 >>> 0) + 2).toLowerCase();

            	if (file_extension === 'xls' || file_extension === 'xlsx') {

		            form_data.append('file_name', uploaded_files[0]);
		            form_data.append('filename', file_name);

					$.ajax({
						'url': 'fileupload.ashx',
		                'type': 'POST',
		                'data': form_data,
		                'contentType': false,
		                'processData': false,
		                success: function (data) {
		                	var results = JSON.parse(data);
		                	uploads.file = results.return_data; 
		                }
					})
					.fail(function () { SAXAlert.show({type: "error", message: "An error occurred while uploading the file. Please try again."}) })
				}
			}

			function _initOther() {
				upload_elements.employee_file_import.change(_doUpload);
				upload_elements.company_file_import.change(_doUpload);
				upload_elements.transaction_file_import.change(_doUpload);
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
		};

	}) (jQuery, window, document);

	// INITIAL PAGE LOAD
	ImportMasterView.init();
});