
/**************************************************************************************/

function getbranchAndCategory(company_code, branch_code, category_code) {


    $company_code = $("#company_code");
    return SAXHTTP.AJAX(
            "manage_shift.aspx/GetCompanyCategory", { company_code: company_code }
        )
        .done(function(data) {

            var results = JSON.parse(data.d.return_data);
            _renderBranchAndCategory($employee_branch_name, results.branch, 'branch_code', 'branch_name', 'Select Branch', 'No Branches found');
            if (branch_code == "" || branch_code == null) {
                document.getElementById('branch').value = "select";
            }
            else {
                document.getElementById('branch').value = branch_code;
            }
            _renderBranchAndCategory($employee_category_code, results.employee_category, 'employee_category_code', 'employee_category_name', 'Select Employee Category', 'No Employee Categories found');
            if (category_code == "" || category_code == null) {
                document.getElementById('category').value = "select";
            }
            else {
                document.getElementById('category').value = category_code;
            }

        })
        .fail(function() {
            SAXAlert.show({ type: "error", message: "An error occurred while loading Company data. Please try again." });
        });
}

function _renderBranchAndCategory($element, data, key, value, default_data, no_data) {
    var data_length = data.length,
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

$company_code = $("#company_code");
$(function() {

    var Company = (function($, w, d) {

        function _renderDropdown(data) {
            var select_HTML = "",
	        	data = JSON.parse(data.d.return_data)
            data_length = data.length,
	            counter = 0,
	            $element = $('#company_code'),
	            $parent = $element.parent();

            for (counter = 0; counter < data_length; counter += 1) {
                select_HTML += '<option value="' + data[counter]['company_code'] + '">' + data[counter]['company_name'] + '</option>';
            }

            $element.append(select_HTML);
        }

        function getCompanyData() {
            return SAXHTTP.AJAX(
				"manage_shift.aspx/GetCompanyData", {}
			)
			.done(_renderDropdown)
			.fail(function() {
			    SAXAlert.show({ type: "error", message: "An error occurred while loading Company data. Please try again." })
			});
        };

        return {
            get: getCompanyData
        };

    })(jQuery, window, document);


    /***********************************************************************************************************************************************/

    OtherData = (function($, w, d) {
        $employee_category_code = $("#category");
        $employee_branch_name = $("#branch");

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

            $company_code = $("#company_code");
            return SAXHTTP.AJAX(


                    	"manage_shift.aspx/GetCompanyCategory", { company_code: company_code }
				)
				.done(function(data) {

				    var results = JSON.parse(data.d.return_data);

				    _render($employee_branch_name, results.branch, 'branch_code', 'branch_name', 'Select Branch', 'No Branches found');
				    _render($employee_category_code, results.employee_category, 'employee_category_code', 'employee_category_name', 'Select Employee Category', 'No Employee Categories found');
				})
				.fail(function() {
				    SAXAlert.show({ type: "error", message: "An error occurred while loading Company data. Please try again." });
				});
        }

        return {
            get: getOtherData
        };

    })(jQuery, window, document);


    /************************************************************************************************************************************************/

    var ShiftView = (function($, w, d) {

        var 
			forms = {
			    save: $("#saveForm")
			},
			buttons = {
			    save: $("#saveButton")
			},
			button_events = {
			    "shift/save": saveShift
			},
			page = {
			    mode: ""
			},
			checkboxes = {
			    normal_shift: $("#check_if_normal_shift"),
			    night_shift: $("#status_night_shift"),
			    overtime: $("#overtime"),
			    half_day: $("#status_half_day"),
			    is_active: $("#is_active"),
			    auto_checked: $("#auto_checked")
			    //
			},
			checkbox_elements = {
			    normal_shift: $('.normal-shift-disable'),
			    overtime: $('.overtime-disable'),
			    night_shift: $('.night-shift-disable'),
			    is_active: $('.is_active'),
			    auto_checked: $('.auto_checked')

			};

        function _validate(data) {

            var 
	        		cutoff_start_time = moment(data["in_grace"], "HH:mm"),
	        		cutoff_start_time_hour = cutoff_start_time.hour(), cutoff_start_time_minutes = cutoff_start_time.minutes();

            var 
        			cutoff_break_out = moment(data["break_out_grace"], "HH:mm"),
        			cutoff_break_out_hour = cutoff_break_out.hour(), cutoff_break_out_minutes = cutoff_break_out.minutes();

            var 
    				cutoff_break_in = moment(data["break_in_grace"], "HH:mm"),
    				cutoff_break_in_hour = cutoff_break_in.hour(), cutoff_break_in_minutes = cutoff_break_in.minutes();

            var 
					cutoff_end_time = moment(data["out_grace"], "HH:mm"),
					cutoff_end_time_hour = cutoff_end_time.hour(), cutoff_end_time_minutes = cutoff_end_time.minutes();

            if (data.company_code == "select") {
                SAXAlert.show({ type: "error", message: "Please select a Company" });
                return false;
            }

            if (data.shift_code == "") {
                SAXAlert.show({ type: "error", message: "Please enter a Shift Code" });
                return false;
            }
            if (data.shift_code != "" && !SAXValidation.code(data.shift_code)) {
                SAXAlert.show({ type: "error", message: "Please enter a valid Shift Code" });
                return false;
            }

            if (data.shift_desc == "") {
                SAXAlert.show({ type: "error", message: "Please enter a Shift Name" });
                return false;
            }
            if (data.shift_desc != "" && !SAXValidation.name(data.shift_desc)) {
                SAXAlert.show({ type: "error", message: "Please enter a valid Shift Name" });
                return false;
            }
            if (data.branch_code == "select") {
                SAXAlert.show({ type: "error", message: "Please select a Branch" });
                return false;
            }

            if (data.in_time == "") {
                SAXAlert.show({ type: "error", message: "Please enter a Start Time" });
                return false;
            }
            if (data.out_time == "") {
                SAXAlert.show({ type: "error", message: "Please enter a End Time" });
                return false;
            }
            if (data.check_if_normal_shift == 0 && data.break_out == "") {
                SAXAlert.show({ type: "error", message: "Please enter a Break Out Time" });
                return false;
            }
            if (data.check_if_normal_shift == 0 && data.break_in == "") {
                SAXAlert.show({ type: "error", message: "Please enter a Break In Time" });
                return false;
            }
            if (data.check_if_normal_shift == 0 && data.in_grace == "") {
                SAXAlert.show({ type: "error", message: "Please enter a Cut Off Start Time" });
                return false;
            }
            if (data.check_if_normal_shift == 0 && data.break_out_grace == "") {
                SAXAlert.show({ type: "error", message: "Please enter a Cut Off Break Out Time" });
                return false;
            }
            if (data.check_if_normal_shift == 0 && data.break_in_grace == "") {
                SAXAlert.show({ type: "error", message: "Please enter a Cut Off Break In Time" });
                return false;
            }
            if (data.check_if_normal_shift == 0 && data.out_grace == "") {
                SAXAlert.show({ type: "error", message: "Please enter a Cut Off End Time" });
                return false;
            }

            if (moment(data["in_time"], "HH:mm").unix() == moment(data["out_time"], "HH:mm").unix()) {
                SAXAlert.show({ type: "error", message: "In & Out Time cannot be the same" });
                return false;
            }
            if (data["status_night_shift"] == 0) {
                    SAXAlert.show({ type: "error", message: "Please Check The  Shift Cutoff Check Box" });
                    return false;
                }
                if (data["max_shift_end_cut_off_time"] == "") {
                    SAXAlert.show({ type: "error", message: "Shift Cutoff Cannot Be Empty" });
                    return false;}
            //  if ((moment(data["in_time"], "HH:mm").unix() <= moment(data["out_time"], "HH:mm").unix()) && data["status_night_shift"] == 1) {
            //      SAXAlert.show({ type: "error", message: "No Cutoff For Other Than Night" });
            //      return false;
            //  }
            //  if ((moment(data["in_time"], "HH:mm").unix() <= moment(data["out_time"], "HH:mm").unix()) && data["status_night_shift"] == 1 && data["max_shift_end_cut_off_time"] != "") {
            //      SAXAlert.show({ type: "error", message: "No Cutoff For Other Than Night" });
            //      return false;
            //  }
            // if (moment(data["in_time"], "HH:mm").unix() > moment(data["out_time"], "HH:mm").unix()) {

            //     if (data["status_night_shift"] == 0) {
            //         SAXAlert.show({ type: "error", message: "Please Check The Night Shift Check Box" });
            //         return false;
            //     }
            //     else if (data["max_shift_end_cut_off_time"] == "") {
            //         SAXAlert.show({ type: "error", message: "Night Shift Cutoff Cannot Be Empty" });
            //         return false;
            //     }
            // }

            if (data["check_if_normal_shift"] == 0 && data["status_night_shift"] == 0) {

                if (moment(data["in_time"], "HH:mm").add(cutoff_start_time_hour, "h").add(cutoff_start_time_minutes, "m") > moment(data["break_out"], "HH:mm").add(-cutoff_break_out_hour, "h").add(-cutoff_break_out_minutes, "m")) {
                    SAXAlert.show({ type: "error", message: "Cutoffs Of Start Time & Break Out Are Overlapping" });
                    return false;
                }
                if (moment(data["break_out"], "HH:mm").add(cutoff_break_out_hour, "h").add(cutoff_break_out_minutes, "m") > moment(data["break_in"], "HH:mm").add(-cutoff_break_in_hour, "h").add(-cutoff_break_in_minutes, "m")) {
                    SAXAlert.show({ type: "error", message: "Cutoffs of Break Out & Break In time are overlapping" });
                    return false;
                }
                if (moment(data["break_in"], "HH:mm").add(cutoff_break_in_hour, "h").add(cutoff_break_in_minutes, "m") > moment(data["out_time"], "HH:mm").add(-cutoff_end_time_hour, "h").add(-cutoff_end_time_minutes, "m")) {
                    SAXAlert.show({ type: "error", message: "Cutoffs of Break In & End time are overlapping" })
                    return false;
                }
            }

            if (data["check_if_normal_shift"] == 0 && data["status_night_shift"] == 1) {

                var 
    					break_out_time = "", break_in_time = "", out_time = "";

                if (moment(data["in_time"], "HH:mm").unix() > moment(data["break_out"], "HH:mm").unix()) {
                    break_out_time = moment(data["break_out"]).add(1, 'd');
                }
                if (moment(data["break_out"], "HH:mm").unix() > moment(data["break_in"], "HH:mm").unix()) {
                    break_in_time = moment(data["break_in_time"]).add(1, "d");
                }
                if (moment(data["in_time"], "HH:mm").unix() > moment(data["out_time"], "HH:mm").unix()) {
                    out_time = moment(data["out_time"]).add(1, "d");
                }


                if (moment(data["in_time"], "HH:mm").add(cutoff_start_time_hour, "h").add(cutoff_start_time_minutes, "m") > moment(data["break_out"], "HH:mm").add(-cutoff_break_out_hour, "h").add(-cutoff_break_out_minutes, "m")) {
                    SAXAlert.show({ type: "error", message: "Cutoffs Of Start Time & Break Out Are Overlapping" });
                    return false;
                }
                if (moment(data["break_out"], "HH:mm").add(cutoff_break_out_hour, "h").add(cutoff_break_out_minutes, "m") > moment(data["break_in"], "HH:mm").add(-cutoff_break_in_hour, "h").add(-cutoff_break_in_minutes, "m")) {
                    SAXAlert.show({ type: "error", message: "Cutoffs of Break Out & Break In time are overlapping" });
                    return false;
                }
                if (moment(data["break_in"], "HH:mm").add(cutoff_break_in_hour, "h").add(cutoff_break_in_minutes, "m") > moment(data["out_time"], "HH:mm").add(-cutoff_end_time_hour, "h").add(-cutoff_end_time_minutes, "m")) {
                    SAXAlert.show({ type: "error", message: "Cutoffs of Break In & End time are overlapping" })
                    return false;
                }
            }

            if (data["grace_in"] != "" && !$.isNumeric(data["grace_in"])) {
                SAXAlert.show({ type: "error", message: "Please enter a numeric value for Grace In Time" });
                return false;
            }
            if (data["grace_out"] != "" && !$.isNumeric(data["grace_out"])) {
                SAXAlert.show({ type: "error", message: "Please enter a numeric value for Grace Out Time" });
                return false;
            }

            if (data["grace_break_out"] != "" && !$.isNumeric(data["grace_break_out"])) {
                SAXAlert.show({ type: "error", message: "Please enter a numeric value for Grace Break Out Time" });
                return false;
            }

            if (data["grace_break_in"] != "" && !$.isNumeric(data["grace_break_in"])) {
                SAXAlert.show({ type: "error", message: "Please enter a numeric value for Grace Break In Time" });
                return false;
            }

            if (data.overtime == 1) {
                if (data.min_overtime == "") {
                    SAXAlert.show({ type: "error", message: "Please enter a Min Overtime" });
                    return false;
                }
                if (data.max_overtime == "") {
                    SAXAlert.show({ type: "error", message: "Please enter a Max Overtime" });
                    return false;
                }
                if (moment(data.max_overtime, "HH:mm").unix() < moment(data.min_overtime, "HH:mm").unix()) {
                    SAXAlert.show({ type: "error", message: "Max Overtime should be greater than Min Overtime" });
                    return false;
                }
            }
            if (data.status_half_day == 1) {
                if (data.half_day == "select") {
                    SAXAlert.show({ type: "error", message: "Please select a Half Day" });
                    return false;
                }
                if (data.start_time_half_day == "") {
                    SAXAlert.show({ type: "error", message: "Please enter Half Day start time" });
                    return false;
                }
                if (data.end_time_half_day == "") {
                    SAXAlert.show({ type: "error", message: "Please enter Half Day end time" });
                    return false;
                }
                if ((data.half_day == data.weekly_off1) && data.half_day != "select") {
                    SAXAlert.show({ type: "error", message: "Half Day and Weekly Off 1 cannot be the same" });
                    return false;
                }
                if ((data.half_day == data.weekly_off2) && data.half_day != "select") {
                    SAXAlert.show({ type: "error", message: "Half Day and Weekly Off 2 cannot be the same" });
                    return false;
                }
            }
            if ((data.weekly_off1 != "select" || data.weekly_off2 != "select")) {
                if (data.weekly_off1 == data.weekly_off2) {
                    SAXAlert.show({ type: "error", message: "Both Weekly Off's cannot be the same" });
                    return false;
                }
            }

            return true;
        }

        function saveShift(event) {

            var 
				form_data = SAXForms.get(forms.save),
				operation = "AddShift";
            
            if (_validate(form_data)) {
                // disable the button to avoid multiple clicks
                buttons.save.button("loading");

                if (page.mode == "edit") operation = "EditShift";

                SAXHTTP.AJAX(
						"manage_shift.aspx/" + operation,
						{ current: JSON.stringify(form_data) }
					)
				.done(function(data) {
				    SAXAlert.show({ type: data.d.status, message: data.d.return_data });

				    if (page.mode != "edit") {
				        forms.save[0].reset();
				        buttons.save.button("reset");
				        checkboxes.normal_shift.prop("checked", true);
				        checkboxes.normal_shift.trigger("change");

				        checkboxes.night_shift.prop("checked", false);
				        checkboxes.night_shift.trigger("change");

				        checkboxes.overtime.prop("checked", false);
				        checkboxes.overtime.trigger("change");

				        checkboxes.half_day.prop("checked", false);
				        checkboxes.half_day.trigger("change");

				        checkboxes.is_active.prop("checked", false);
				        checkboxes.is_active.trigger("change");
				    }
				})
				.fail(function() {
				    SAXAlert.show({ type: "error", message: "An error occurred while saving Shift details. Please try again." });
				})
				.always(function() {
				    buttons.save.button("reset");
				});
            }
        }
        function _setDateTimeFormats(data) {
            data["in_time"] = moment(data["in_time"]).format("HH:mm");
            data["break_out"] = moment(data["break_out"], "HH:mm:ss").format("HH:mm");
            data["break_in"] = moment(data["break_in"], "HH:mm:ss").format("HH:mm");
            data["out_time"] = moment(data["out_time"]).format("HH:mm");

            data["in_grace"] = moment(data["in_grace"], "HH:mm:ss").format("HH:mm");
            data["break_out_grace"] = moment(data["break_out_grace"], "HH:mm:ss").format("HH:mm");
            data["break_in_grace"] = moment(data["break_in_grace"], "HH:mm:ss").format("HH:mm");
            data["out_grace"] = moment(data["out_grace"], "HH:mm:ss").format("HH:mm");

            if (data["max_shift_end_cut_off_time"] != "")
                data["max_shift_end_cut_off_time"] = moment(data["max_shift_end_cut_off_time"], "HH:mm").format("HH:mm");
            else
                data["max_shift_end_cut_off_time"] = "";

            data["min_overtime"] = moment(data["min_overtime"], "HH:mm:ss").format("HH:mm");
            data["max_overtime"] = moment(data["max_overtime"], "HH:mm:ss").format("HH:mm");

            data["start_time_half_day"] = moment(data["start_time_half_day"]).format("HH:mm");
            data["end_time_half_day"] = moment(data["end_time_half_day"]).format("HH:mm");

            if (data["weekly_off1"] == "")
                data["weekly_off1"] = "select";
            if (data["weekly_off2"] == "")
                data["weekly_off2"] = "select";

            data["ramadan_in_time"] = moment(data["ramadan_in_time"]).format("HH:mm");
            data["ramadan_out_time"] = moment(data["ramadan_out_time"]).format("HH:mm");

            return data;
        }
        function _triggerChangeEvents(data) {

            if (data["check_if_normal_shift"] == 0)
                checkboxes.normal_shift.trigger("click");

            if (data["max_shift_end_cut_off_time"] != "") {
                checkboxes.night_shift.trigger("click");
            }

            if (data["min_overtime"] != "00:00") {
                checkboxes.overtime.trigger("click");
            }

            if (data["half_day"] != "") {
                checkboxes.half_day.trigger("click");
            }



            getbranchAndCategory(data["company_code"], data["branch"], data["category"]);
            document.getElementById('shift_code').value = data["shift_code"];

            if (data["auto_checked"] == 1) {
                document.getElementById("auto_checked").checked = true;
            }

        }

        function getShiftDetails(shift_code) {
            return SAXHTTP.AJAX(
				"manage_shift.aspx/GetShiftDetails", { shift_code: shift_code }
			)
			.done(function(data) {
			    data = JSON.parse(data.d.return_data);
			    data[0] = _setDateTimeFormats(data[0]);
			    SAXForms.set(forms.save, data[0]);
			    _triggerChangeEvents(data[0]);
			    SAXForms.disable(["company_code", "shift_code", "shift_desc"]);
			})
			.fail(function() {
			    SAXAlert.show({ type: "error", message: "An error occurred while loading Company data. Please try again." })
			});
        }

        /* other */
        function _resetNormalShift() {
            checkbox_elements.normal_shift.val("");
        }
        function _toggleNormalShift(event) {
            if ($(event.target).is(':checked')) {
                checkbox_elements.normal_shift.prop('disabled', true);
                _resetNormalShift();
            }
            else {
                checkbox_elements.normal_shift.prop('disabled', false);
            }
        }

        function _resetOvertime() {
            checkbox_elements.overtime.val("");
        }
        function _toggleOvertime(event) {
            if ($(event.target).is(':checked'))
                checkbox_elements.overtime.prop('disabled', false);
            else {
                checkbox_elements.overtime.prop('disabled', true);
                _resetOvertime();
            }
        }

        function _resetHalfDay() {
            checkbox_elements.half_day.val("");
            $("#half_day").val("select");
        }
        function _toggleHalfDay(event) {
            if ($(event.target).is(':checked'))
                checkbox_elements.half_day.prop('disabled', false);
            else {
                checkbox_elements.half_day.prop('disabled', true);
                _resetHalfDay();
            }
        }

        function _resetNightShift() {
            checkbox_elements.night_shift.val("");
        }
        function _toggleNightShift(event) {
            if ($(event.target).is(':checked'))
                checkbox_elements.night_shift.prop('disabled', false);
            else {
                checkbox_elements.night_shift.prop('disabled', true);
                _resetNightShift();
            }
        }

        function _CheckShiftCodeAssigned(shift_code) {
            return SAXHTTP.AJAX(
					"manage_shift.aspx/CheckShiftCodeAssigned", { shift_code: shift_code }
				)
		    .done(function(data) {
		        data = JSON.parse(data.d.return_data);
		        if (data > 0) {
		            SAXAlert.show({ type: "error", message: "This shift is already assigned to some employee's. So you can't de-active this shift." });
		            $("#is_active").prop('checked', true);
		        }
		    })

        }

        function _toggleIsActive(event) {
            if ($(event.target).is(':checked') == false) {
                var shift_code = $("#shift_code").val(); 
                _CheckShiftCodeAssigned(shift_code);
            }
        }

        function _initOther() {

            $company_code.change(function() {
            
            var is_active = $("#is_active");
            is_active.prop("checked", true);
            
                OtherData.get($(this).val());
            });

            $(".timepicker").timepicki({
                show_meridian: false,
                min_hour_value: 0,
                max_hour_value: 23,
                start_time: ["00", "00"]
            });



            checkboxes.normal_shift.change(_toggleNormalShift);

            checkboxes.overtime.change(_toggleOvertime);

            checkboxes.half_day.change(_toggleHalfDay);

            checkboxes.night_shift.change(_toggleNightShift);

            checkboxes.is_active.change(_toggleIsActive);

            //Adding stuffs

            // Get branch, department, shift, designation, category data based on the company selected.

        }

        /* buttons */
        function _buttonHandler(event) {
            var role = $(event.target).data('role');
            button_events[role].call(this, event);           
        }
        function _initButtons() {
            $(document).on("click", "[data-control=\"button\"]", _buttonHandler);
        
        }

        function initialize(mode) {
            _initButtons();
            _initOther();
            page.mode = mode;
        }

        return {
            init: initialize,
            get: getShiftDetails,
            save: saveShift
        };

    })(jQuery, window, document);

    SAXLoader.show();

    // INIT PAGE
    var 
		hash = document.location.hash,
		hash_parts = hash.split("/");

    Company.get()
		.done(function() {
		    var mode = "add";
		    if (hash_parts.length > 2) {
		        ShiftView.get(hash_parts[2]);

		        mode = "edit";
		    }

		    ShiftView.init(mode);
		})
		.always(SAXLoader.close);
});