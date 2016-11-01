$(function () {

	var Employee = (function ($, w, d) {

		var 
			page_elements = { 
				employee_code: $("#employee_code"),
				employee_name: $("#employee_name"),
				department_name: $("#department_name"),
				designation_name: $("#designation_name")
			};

		function renderEmployeeData(data, data_length) {
			page_elements.employee_code.val(data[0]["employee_code"]);
			page_elements.employee_name.val(data[0]["employee_name"]);
			page_elements.department_name.val(data[0]["department_name"]);
			page_elements.designation_name.val(data[0]["designation_name"]);
		}

		function getEmployeeData() {

			var deferred = $.Deferred();

			SAXHTTP.AJAX(
				"update.aspx/GetEmployeeData",
				{}
			)
			.done(function (data) {
				var data = JSON.parse(data.d.return_data);
				deferred.resolve(data, data.length);
			}).fail(function () {
				deferred.reject();
			});

			return deferred.promise();
		}

		return {
			get: getEmployeeData,
			render: renderEmployeeData
		};

	}) (jQuery, window, document);

	var Profile = (function ($, w, d) {

		var
			forms = {
				save: $("#saveForm")
			},
			buttons = {
				upload: $("#uploadButton"),
				save: $("#saveButton")
			},
			$display_pic = $("#display_pic");

		function uploadImage(that) {

        	var 
        		uploaded_files = $(that).get(0).files,
	            form_data = new FormData(),
	            ajax_options = {},
	            date = new Date(),
	            employee_code = sessvars.TAMS_ENV.user_details.user_name,
	            file_extension = uploaded_files[0].name.slice((uploaded_files[0].name.lastIndexOf(".") - 1 >>> 0) + 2).toLowerCase(),
	            file_name = employee_code + ".png";


	        if (file_extension === 'png' || file_extension === 'jpg') {

	            form_data.append('file_name', uploaded_files[0]);
	            form_data.append('filename', file_name);
	            
	            ajax_options = {
	                'url': 'fileupload.ashx',
	                'type': 'POST',
	                'data': form_data,
	                'contentType': false,
	                'processData': false,
	                'success': function (data) { 
	                	$("#display_pic").remove();
	                	window.setTimeout(function() {
	                		$("#imageContainer").append("<img src='../../uploads/profiles/" + JSON.parse(data).return_data + "?" + date.getTime() + "' id=\"display_pic\">");
	                		SAXLoader.closeBlockingLoader();
	                	}, "3000");
	                },
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
		}

		function _validate(data) {

			if (data.current_password == "") {
				SAXAlert.show({type: "error", message: "Please enter your current password"});
				return false;
			}

			if (data.new_password == "") {
				SAXAlert.show({type: "error", message: "Please enter your new password"});
				return false;	
			}

			if (data.confirm_password == "")  {
				SAXAlert.show({type: "error", message: "Please confirm your new password"});
				return false;
			}

			if (data.new_password != data.confirm_password) {
				SAXAlert.show({type: "error", message: "Please ensure your new password and confirm password are the same"});
				return false;
			}

			return true;
		}

		function savePassword() {
			var
				data = SAXForms.get(forms.save);

			buttons.save.button("loading");

			if (_validate(data)) {
				SAXHTTP.AJAX(
						"update.aspx/SavePassword",
						{current: JSON.stringify(data)}
					)
				.done(function (data) {
					SAXAlert.show({type: data.d.status, message: data.d.return_data});

					if (data.d.status == "success")
						forms.save[0].reset();
				})
				.fail(function () {
					SAXAlert.show({type: "error", message: "An error occurred while performing this operation. Please try again."});
				})
				.always(function () {
					buttons.save.button("reset");
				});
			}
		}

		return {
			save: savePassword,
			upload: uploadImage
		};

	}) (jQuery, window, document);

	var UpdateView = (function ($, w, d) {

		var 
			button_events = {
				"profile/upload": Profile.upload,
				"profile/save": Profile.save
			};

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

			$("#file").change(function () { Profile.upload(this) });
		}

		return {
			init: initialize
		};

	}) (jQuery, window, document);
	
	// INITIAL PAGE LOAD

	UpdateView.init();

	if (sessvars.TAMS_ENV.user_details.user_name != "admin") {
		SAXLoader.show();
		Employee.get()
			.done(Employee.render)
			.fail(function () {
				SAXAlert.show({type: "error", message: "An error occurred while loading page data. Please try again."});
			})
			.always(function() {
				SAXLoader.close();
			});
	} else {
		$("#profileDetails").hide();
	}
});