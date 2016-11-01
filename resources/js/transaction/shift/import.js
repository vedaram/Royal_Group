$(function () {

	var Shift = (function ($, w, d) {

		var
			buttons = {
				import: $("#importShiftButton"),
				export: $("#shiftExportButton")
			},
			page_elements = {
				file_upload: $("#file_upload"),
				import_result: $("#importShiftResult")
			};

		function importShiftRoster() {

			var	file_name = ShiftImportMasterView.file();

			buttons.import.button("loading");
			page_elements.import_result.val('Beginning import of Shift Roster ...');

			SAXHTTP.AJAX(
					"import.aspx/DoImport",
					{file_name: file_name}
				)
				.done(function (data) {
					var 
						message = data.d.return_data;
					page_elements.import_result.val(message);
            		page_elements.file_upload.val("");
				})
				.fail(function (data) {
					SAXAlert.show({type: "error", message: data.d.return_data})
				})
				.always(function () {
					buttons.import.button("reset");
				});
		}

			_processExport = function(data) {

		        var 
		            status = data.d.status;

		        switch (status) {
		            case 'success':
		                SAXAlert.showAlertBox({ 'type': status, 'url': SAXUtils.getApplicationURL() + data.d.return_data });
		                break;
		            case 'info':
		                SAXAlert.show({ 'type': status, 'message': data.d.return_data });
		                break;
		            case 'error':
		                SAXAlert.show({ 'type': status, 'message': data.d.return_data });
		                break;
		        }
		    }

		function exportShift() {

			buttons.export.button("loading");

			SAXHTTP.AJAX(
					"import.aspx/DoExport",
					{}
				)
				.done(_processExport)
				.fail(function (data) {
					SAXAlert.show({type: "error", message: data.d.return_data});
				})
				.always(function () {
					buttons.export.button("reset");
				});
		}

		return {
			export: exportShift,
			import: importShiftRoster
		}

	}) (jQuery, window, document);
	
	var ShiftImportMasterView = (function ($, w, d) {

		var 
			uploads = {
				file: ""
			},
			page_elements = {
				file: $("#file_upload")
			},
			button_events = {
				"shift/export": Shift.export,
				"shift/import": Shift.import
			};

		function getFileName() {
			return uploads.file;
		}

		/* other */
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

            		SAXLoader.showBlockingLoader();

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
					.always(function () { SAXLoader.closeBlockingLoader(); });
				}
			}
		function _initOther() {
			page_elements.file.change(_doUpload);
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
			init: initialize,
			file: getFileName
		};

	}) (jQuery, window, document);

	// INITIAL PAGE LOAD
	ShiftImportMasterView.init();

});