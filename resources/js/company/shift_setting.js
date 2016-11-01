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
				"shift_setting.aspx/GetCompanyData", {}
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

	/******************************************************************************************************************/

	var ShiftSetting = (function ($, w, d) {

		var
			buttons = {
				save: $("#saveButton")
			},
			forms = {
				save: $("#saveForm")
			},
			mode = "",
			$company = $("#company"),
			$break_type = $("#break_type"),
			$is_ramadan = $("#is_ramadan"),
			$break_deduction_required = $("#break_deduction_required");

			function _validate(data) {

				if (data["company"] == "select") {
					SAXAlert.show({type: "error", message: "Please select a company."});
					return false;
				}

				if (data["max_holiday_count"] == "") {
					SAXAlert.show({type: "error", message: "Please enter a value for max holidays"});
					return false;
				}

				if (data["max_holiday_count"] != "" && !$.isNumeric(data["max_holiday_count"])) {
					SAXAlert.show({type: "error", message: "Please enter a numeric value for Max Holiday Count"});
					return false;
				}

				if (data["is_ramadan"] == 1) {

					if (data["ramadan_from_date"] == "" && data["ramadan_to_date"] == "") {
						SAXAlert.show({type: "error", message: "Please enter a Ramadan From & To Date"});
						return false;
					}

					if (moment(data["ramadan_to_date"]).unix() < moment(data["ramadan_from_date"]).unix()) {
						SAXAlert.show({type: "error", message: "From Date cannot be greater than To Date"});
						return false;
					}
				}

				if (data["break_deduction_required"] == 1 && (data["min_work_hours_for_deduction"] == "" && data["break_deduction"] == "")) {
					SAXAlert.show({type: "error", message: "Please enter the Min Work Hours for Deduction and Break Deduction (in Minutes)"});
					return false;
				}
				if(data["cutoff_time"] == ""){
				    SAXAlert.show({type: "error", message: "Please enter Weekly Cut off Time"});
					return false;
				}

				if (data["break_deduction_required"] == 1 && data["break_deduction"] != "" && !$.isNumeric(data["break_deduction"])) {
					SAXAlert.show({type: "error", message: "Please enter a numeric value for Break Deduction (in Minutes)"});
					return false;
				}

				if (data["from_day"] != "" && !$.isNumeric(data["from_day"])) {
					SAXAlert.show({type: "error", message: "Please enter a numeric value for From Day"});
					return false;
				}

				if (data["to_day"] != "" && !$.isNumeric(data["to_day"])) {
					SAXAlert.show({type: "error", message: "Please enter a numeric value for To Day"});
					return false;
				}

				if (data["block_day"] != "" && !$.isNumeric(data["block_day"])) {
					SAXAlert.show({type: "error", message: "Please enter a numeric value for Block Day"});
					return false;
				}

				return true;
			}

		function saveShiftSetting() {
			
			var
				data = SAXForms.get(forms.save);

			data["company"] = $company.val();
			data["mode"] = mode;

			if (_validate(data)) {
				buttons.save.button("loading");

				SAXHTTP.AJAX(
						"shift_setting.aspx/SaveShiftSetting",
						{current: JSON.stringify(data)}
					)
					.done(function (data) {
						SAXAlert.show({type: data.d.status, message: data.d.return_data})
						forms.save[0].reset();
						$company.val("select");
					})
					.fail(function (data) {
						SAXAlert.show({type: "error", message: data.d.return_data})
					})
					.always(function () {
						buttons.save.button("reset");
					})
			}
		}

			function _toggleSettings(data) {

				if (data["break_type"] == 0) {
					$break_type.trigger("change");
				}

				if (data["is_ramadan"] == 1) {
					$is_ramadan.prop("checked", true);
					$is_ramadan.trigger("change");
				}

				if (data["break_deduction_required"] == 1) {
					$break_deduction_required.prop("checked", true);
					$break_deduction_required.trigger("change");
				}
			}

			function _formatDates(data) {
				// format ramadan dates

				if (data["ramadan_from_date"] != "01/01/1900" && data["ramadan_from_date"] != null)
					data["ramadan_from_date"] = moment(data["ramadan_from_date"], "DD/MM/YYYY").format("DD-MMM-YYYY");
				else
					data["ramadan_from_date"] = "";

				if (data["ramadan_to_date"] != "01/01/1900" && data["ramadan_to_date"] != null)
					data["ramadan_to_date"] = moment(data["ramadan_to_date"], "DD/MM/YYYY").format("DD-MMM-YYYY");
				else
					data["ramadan_to_date"] = "";

				if (data["is_auto_shift"] == null || data["is_auto_shift"] == "")
					data["is_auto_shift"] = 0;

				return data;
			}

		function getShiftSetting(event) {

			if ($(event.target).val() != "select") {
				SAXLoader.show();

				SAXHTTP.AJAX(
						"shift_setting.aspx/GetShiftSetting",
						{company_code: $(event.target).val()}
					)
					.done(function (data) {
						var results = JSON.parse(data.d.return_data);

						if (results.length > 0) {

							results = results[0];

							results = _formatDates(results);
							SAXForms.set(forms.save, results);
							_toggleSettings(results);

							mode = "U";
						}
						else {
							mode = "I";
						}
					})
					.fail(function (data) {
						SAXAlert.show({type: "error", message: data.d.return_data})
					})
					.always(SAXLoader.close);

			} else {
				forms.save[0].reset();
			}
		}

		return {
			get: getShiftSetting,
			save: saveShiftSetting
		};

	}) (jQuery, window, document);

	/******************************************************************************************************************/

	var ShiftSettingMasterView = (function ($, w, d) {

		var 		
			button_events = {
				"shift-setting/save": ShiftSetting.save,
			},
			$company = $("#company"),
			$break_type = $("#break_type"),
			$break_hours = $("#break_hours"),
			$is_ramadan = $("#is_ramadan"),
			$ramadan_from_date = $("#ramadan_from_date"),
			$ramadan_to_date = $("#ramadan_to_date"),
			$break_deduction_required = $("#break_deduction_required"),
			$min_work_hours_for_deduction = $("#min_work_hours_for_deduction"),
			$cutoff_time = $("#cutoff_time"),
			$break_deduction = $("#break_deduction");

		/* buttons */
			function _buttonHandler(event) {
				var role = $(event.target).data('role');
				button_events[role].call(this, event);
			}

		function _initButtons() {
			$(document).on("click", "[data-control=\"button\"]", _buttonHandler);
		}

		/* other */

			function _toggleBreakType(event) {
				if($(event.target).val() == 0) {
					$break_hours.prop("disabled", false)
				}
				else {
					$break_hours.prop("disabled", true);
					$break_hours.val("00:00");
				}
			}

			function _toggleRamadan(event) {
				if ($(event.target).is(":checked")) {
					$ramadan_from_date.prop("disabled", false);
					$ramadan_to_date.prop("disabled", false);
				} else {
					$ramadan_from_date.prop("disabled", true).val("");
					$ramadan_to_date.prop("disabled", true).val("");
				}
			}

			function _toggleBreakDeduction() {
				if ($(event.target).is(":checked")) {
					$break_deduction.prop("disabled", false);
					$min_work_hours_for_deduction.prop("disabled", false);
				} else {
					$break_deduction.prop("disabled", true);
					$min_work_hours_for_deduction.prop("disabled", true);
				}
			}

		function _initOther() {

			$(".datepicker").Zebra_DatePicker({
				format: 'd-M-Y'
			});

			$(".timepicker").timepicki({
				show_meridian:false,
				min_hour_value:0,
				max_hour_value:23,
	            start_time: ["00", "00"]
			});

			$company.change(ShiftSetting.get);

			$break_type.change(_toggleBreakType);

			$is_ramadan.change(_toggleRamadan);

			$break_deduction_required.change(_toggleBreakDeduction);
		}

		function initialize() {
			_initButtons();
			_initOther();
		}

		return {
			init: initialize
		};

	}) (jQuery, window, document);

	/******************************************************************************************************************/

	// INITIAL PAGE LOAD
	SAXLoader.show();

	ShiftSettingMasterView.init();

	Company.get()
		.always(function () {
			SAXLoader.close();
		});
});