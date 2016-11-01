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
				"change_company_logo.aspx/GetCompanyData", {}
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

	
	// INITIAL PAGE LOAD
	SAXLoader.show();

	// init master view
	//ReportMasterView.init();
	
	Company.get()
		.done(function () {
			$("#company").prop("disabled", false);
		})
		.always(function() { SAXLoader.close(); });
});


//
 
 
 /*********************************************************************************************************/
 
//Upload logo stuff

$(function () {
 
var Profile = (function ($, w, d) {

		var
			forms = {
				save: $("#saveForm")
			},
			buttons = {
				upload: $("#logo_update"),
				save: $("#logo_save")
			},
			$display_pic = $("#display_pic");

		function uploadImage(that) {
		
    var selector = document.getElementById('company');
    var value = selector[selector.selectedIndex].value;
        	var 
        		uploaded_files = $(that).get(0).files,
	            form_data = new FormData(),
	            ajax_options = {},
	            date = new Date(),
	            
	            file_extension = uploaded_files[0].name.slice((uploaded_files[0].name.lastIndexOf(".") - 1 >>> 0) + 2).toLowerCase(),
	            file_name = value + ".png";

                    //alert(file_extension);
	        if (file_extension === 'png' || file_extension === 'jpg') {

	            form_data.append('file_name', uploaded_files[0]);
	            form_data.append('filename', file_name);
	            //alert(form_data);
	            ajax_options = {
	                'url': 'fileupload.ashx',
	                'type': 'POST',
	                'data': form_data,
	                'contentType': false,
	                'processData': false,
	                'success': function (data) { 
	                	
	                	window.setTimeout(function() {
	                	// alert(JSON.parse(data).return_data);
	                	SAXAlert.show({'type': 'success', 'message': 'Company Logo updated successfully'});
//	                		$("#imageContainer").append("<img src='../../uploads/profiles/" + JSON.parse(data).return_data + "?" + date.getTime());
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

 
		return {
			//save: savePassword,
			upload: uploadImage
		};

	}) (jQuery, window, document);

	var UpdateView = (function ($, w, d) {

		var 
			button_events = {
				"logo/upload": Profile.upload
				
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
			
			$("#logo_update").click(function () { alert(1); });
			
		}

		return {
			init: initialize
		};

	}) (jQuery, window, document);
	
	// INITIAL PAGE LOAD

	UpdateView.init();
	//Company.init();

	if (sessvars.TAMS_ENV.user_details.user_name != "admin") {
		//SAXLoader.show();
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
 