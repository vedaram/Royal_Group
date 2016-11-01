
function setEnrollIdValue() {

    document.getElementById("enroll_id").value = document.getElementById("employee_code").value;

}


$(function() {

    var loadFlag = '0';

    var Company = (function($, w, d) {

        function _renderDropdown(data) {
            var select_HTML = "<option value=\"select\">Select Company</option>",
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
				"manage_employee.aspx/GetCompanyData", {}
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

    /************************************************************************************************************************************************/

    OtherData = (function($, w, d) {

        var 
			$branch_code = $('#branch_code'),
	        $department_code = $('#department_code'),
	        $designation_code = $('#designation_code'),
	        $shift_code = $("#shift_code"),
	        $manager_id = $('#manager_id'),
	        $employee_category_code = $("#employee_category_code");

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
            //  alert(select_HTML);
        }

        function getOtherData(company_code) {

            return SAXHTTP.AJAX(
					"manage_employee.aspx/GetOtherData",
					{ company_code: company_code }
				)
				.done(function(data) {

				    var results = JSON.parse(data.d.return_data);
				    _render($department_code, results.department, 'department_code', 'department_name', 'Select Department', 'No Departments found');
				    _render($branch_code, results.branch, 'branch_code', 'branch_name', 'Select Branch', 'No Branches found');
				    _render($designation_code, results.designation, 'designation_code', 'designation_name', 'Select Designation', 'No Designations found');
				    _render($shift_code, results.shift, 'shift_code', 'shift_desc', 'Select Shift', 'No Shifts found');
				    _render($manager_id, results.manager, 'employee_code', 'employee_name', 'Select Manager', 'No Managers found');
				    _render($employee_category_code, results.employee_category, 'employee_category_code', 'employee_category_name', 'Select Employee Category', 'No Employee Categories found');
				})
				.fail(function() {
				    SAXAlert.show({ type: "error", message: "An error occurred while loading Company data. Please try again." });
				});
        } /***************************************************************************/
        function getmanagerlist(branch_code) {

            var $manager_id = $("#manager_id");
            return SAXHTTP.AJAX(
					"../../../ui/company/masters/manage_employee.aspx/getmanagerlist",
					{ branch_code: branch_code }
				)
				.done(function(data) {
				    var results = JSON.parse(data.d.return_data);
				    _render($manager_id, results.manager, 'employee_code', 'employee_name', 'Select Manager', 'No Managers found');
				})
				.fail(function() {

				    SAXAlert.show({ type: "error", message: "An error occurred while loading  Branch  data. Please try again." });
				});


        }
        /*******************************************************************************/
        return {
            get: getOtherData,
            getmanagerlist: getmanagerlist

        };

    })(jQuery, window, document);


    /************************************************************************************************************************************************/

    var EmployeeView = (function($, w, d) {


        var 
			forms = {
			    save: $("#saveForm")
			},
            dialogs = {
                save: $("#EmployeeDataTransactionForm")
            },
			buttons = {
			    save: $("#saveButton")
			},
			button_events = {
			    "employee/save": saveEmployee
			},
			page = {
			    mode: "",
			    enroll_id: ""
			},
			$company_code = $("#company_code"),
			$date_of_leaving = $("#date_of_leaving"),
			$employee_status = $("#employee_status"),
		    $gender = $("#gender")
        $work_hours_day = $("#work_hours_day")
        $work_hours_week = $("#work_hours_week")
        $work_hours_month = $("#work_hours_month");
        $flagData = false;
        $branch_code = "";
        $empcode = "";
        visitCount = 0;
        shiftSelectionOptions = '';
        managerSelectionOptions = '';
        hasValidationErrors = false;


        function _validate(data) {

            if (data.employee_code == "") {
                SAXAlert.show({ type: "error", message: "Please enter a Employee Code." });
                return false;
            }

            if (data.employee_code != "" && !SAXValidation.employeeCode(data.employee_code)) {
                SAXAlert.show({ type: "error", message: "Please enter a valid Employee Code" });
                return false;
            }

            if (data.date_of_joining == "") {
                SAXAlert.show({ type: "error", message: "Please enter a Date of Joining" });
                return false;
            }

            if (data.employee_name == "") {
                SAXAlert.show({ type: "error", message: "Please enter a Employee Name" });
                return false;
            }

            if (data.employee_name != "" && !SAXValidation.name(data.employee_name)) {
                SAXAlert.show({ type: "error", message: "Please enter a valid Employee Name" });
                return false;
            }

            if (data.company_code == "select") {
                SAXAlert.show({ type: "error", message: "Please select a Company" });
                return false;
            }

            //if(data.date_of_birth == "") {
            //	SAXAlert.show({type: "error", message: "Please enter a Date of Birth"});
            //	return false;						
            //}

            //				if (data.date_of_birth != "" && data.date_of_joining != "") {
            //					if(moment(data.date_of_birth).unix() > moment(data.date_of_joining).unix()) {
            //						SAXAlert.show({type: "error", message: "Date of Joining cannot be less than Date of Birth"});
            //						return false;	
            //					}
            //                }

            if (data.date_of_birth != "" && data.date_of_joining != "") {
                if (moment(data.date_of_birth).unix() >= moment(data.date_of_joining).unix()) {
                    SAXAlert.show({ type: "error", message: "Date of Joining cannot be equal or less than Date of Birth" });
                    return false;
                }
            }

            if (data.enroll_id == "") {
                SAXAlert.show({ type: "error", message: "Please enter a Enrollment ID/ Card Number" });
                return false;
            }

            if (data.employee_category_code == "select") {
                SAXAlert.show({ type: "error", message: "Please select a Employee Category" });
                return false;
            }

            if (data.email_address != "" && !SAXValidation.email(data.email_address)) {
                SAXAlert.show({ type: "error", message: "Please enter a valid Email Address" });
                return false;
            }

            if (data.phone_number != "" && !SAXValidation.isNumber(data.phone_number)) {
                SAXAlert.show({ type: "error", message: "Phone Number should be numeric" });
                return false;
            }

            if (data.emergency_contact_number != "" && !SAXValidation.isNumber(data.emergency_contact_number)) {
                SAXAlert.show({ type: "error", message: "Emergency Contact Number should be numeric" });
                return false;
            }

            if ((data.employee_status == 2 || data.employee_status == 3 || data.employee_status == 4) && data.date_of_leaving == "") {
                SAXAlert.show({ type: "error", message: "Please enter a Date of Leaving" });
                return false;
            }

            return true;
        }

        function saveEmployee(event) {
            event.preventDefault();
            var 
				form_data = SAXForms.get(forms.save),
				operation = "AddEmployee";
            //  alert('saving employee details');
            if (_validate(form_data)) {
                // alert(page.mode);
                // disable the button to avoid multiple clicks
                //  buttons.save.button("loading");

                if (page.mode == "edit") operation = "EditEmployee";
                //    alert('about call ajax after validation ok');
                SAXHTTP.AJAX(
						"manage_employee.aspx/" + operation,
						{ current: JSON.stringify(form_data), previous_enroll_id: page.enroll_id }
					)
				.done(function(data) {
				    SAXAlert.show({ type: data.d.status, message: data.d.return_data });
				    page.enroll_id = form_data["enroll_id"];
				    if (data.d.status == "success" && page.mode == "add")
				        forms.save[0].reset();
				})
				.fail(function() {
				    SAXAlert.show({ type: "error", message: "An error occurred while saving Employee details. Please try again." });
				})
				.always(function() {
				    buttons.save.button("reset");
				});
            }
        }

        function _formatData(data) {

            var 
                date_of_birth = data["date_of_birth"],
                date_of_joining = data["date_of_joining"],
                date_of_leaving = data["date_of_leaving"],
                visa_expiry = data["visa_expiry_date"],
                passport_expiry = data["passport_expiry"],
                religion = data["religion"],
                gender = data["gender"].toLowerCase();




            if (data["IsAutoShiftEligible"] == 1) {
                document.getElementById("auto_checked").checked = true;
            }
            else {
                document.getElementById("auto_checked").checked = false;
            }
            if (data["ot_eligibility"] == 1) {
                document.getElementById("overtime_checked").checked = true;
            }
            else {
                document.getElementById("overtime_checked").checked = false;
            }
            if (data["Ramadan_Eligibility"] == 1) {
                document.getElementById("ramadan_checked").checked = true;
            }
            else {
                document.getElementById("ramadan_checked").checked = false;
            }
            if (data["PunchException_Eligibility"] == 1) {
                document.getElementById("punch_exception_checked").checked = true;
            }
            else {
                document.getElementById("punch_exception_checked").checked = false;
            }
            //data["date_of_birth"] = date_of_birth != null ? moment(date_of_birth, "DD/MM/YYYY").format("DD-MMM-YYYY") : "";

            data["date_of_birth"] = "";
            if (date_of_birth != "01/01/1900" && date_of_birth != "" && date_of_birth != null && date_of_birth != "null") {
                data["date_of_birth"] = moment(date_of_birth, "DD/MM/YYYY").format("DD-MMM-YYYY");
            }
            data["date_of_joining"] = date_of_joining != null ? moment(date_of_joining, "DD/MM/YYYY").format("DD-MMM-YYYY") : "";

            data["date_of_leaving"] = "";
            if (date_of_leaving != "01/01/1900" && date_of_leaving != "" && date_of_leaving != null && date_of_leaving != "null") {
                data["date_of_leaving"] = moment(date_of_leaving, "DD/MM/YYYY").format("DD-MMM-YYYY");
            }

            data["passport_expiry"] = "";
            if (passport_expiry != "01/01/1900" && passport_expiry != "" && passport_expiry != null) {
                data["passport_expiry"] = moment(passport_expiry, "DD/MM/YYYY").format("DD-MMM-YYYY");
            }

            data["visa_expiry"] = "";
            if (visa_expiry != "01/01/1900" && visa_expiry != "" && visa_expiry != null) {
                data["visa_expiry"] = moment(visa_expiry, "DD/MM/YYYY").format("DD-MMM-YYYY");
            }

            data["gender"] = gender;
            data["employee_religion"] = religion;
            var $maternity_break_hours_checked = $('#maternity');

            //if (data["gender"] == "male") {
            //    $maternity_break_hours_checked.hide();

            //}
            //else {
            //    $maternity_break_hours_checked.show();
            //}
            if (data["IsManager"] == 1) {
                data["user_type"] = 1;
            }
            else if (data["IsHR"] == 1) {
                data["user_type"] = 3;
            }
            else if (data["IsAdmin"] == 0) {
                data["user_type"] = 0;
            }
            else {
                data["user_type"] = 2;
            }

            return data;
        }
        function getEmployeeDetails(employee_code) {

            _showEmpDetailsTab();

            //  alert('displaying page for '+employee_code);

            // alert("emp code in getEmpDetail "+employee_code);
            $empcode = employee_code;

            return SAXHTTP.AJAX(
				"manage_employee.aspx/GetEmployeeDetails", { employee_code: employee_code }
			)
			.done(function(data) {

			    data = JSON.parse(data.d.return_data);
			    OtherData.get(data[0]["company_code"])
					.done(function() {
					    data[0] = _formatData(data[0]);
					    SAXForms.set(forms.save, data[0]);
					    SAXForms.disable(["employee_code"]);
					    page.enroll_id = data[0]["enroll_id"];
					    //  $branch_code=data[0]["branch_code"];
					    // getEmployeeTransactionData(employee_code, data[0]["company_code"],data[0]["branch_code"] );
					});
			    OtherData.getmanagerlist(data[0]["branch_code"]).done(function() {
			        SAXForms.set(forms.save, data[0]);

			    });

			})

			.fail(function() {
			    SAXAlert.show({ type: "error", message: "An error occurred while loading Employee data. Please try again." })
			});
        }

        // employee transaction data code begin

        function getEmployeeTransactionData(employee_code, company_code, branch_code) {

            empcode = employee_code;
            //  alert('getting edt data for employee '+ empcode);

            SAXLoader.showBlockingLoader();

            getShiftData(company_code);


            var $branch_code = $('#filter_branch');

            return SAXHTTP.AJAX(
                    "manage_employee.aspx/GetEmployeeTransactionData",
                    { employee_code: employee_code, company_code: company_code, branch_code: branch_code }
                )
                .done(function(data) {

                    var results = JSON.parse(data.d.return_data);

                    //   debugger;

                    _renderforEmployee(results);
                    //   _updateEmployeeTransactionData(results);

                })
                .fail(function() {
                    SAXAlert.show({ type: "error", message: "An error occurred while loading Company data. Please try again." });
                });
        }

        function getShiftData(company_code) {

            return SAXHTTP.AJAX(
                    "manage_employee.aspx/GetOtherData",
                    { company_code: company_code }
                )
                .done(function(data) {

                    var results = JSON.parse(data.d.return_data);
                    _renderShiftOptions(results.shift, 'shift_code', 'shift_desc', 'Select Shift', 'No Shifts found');
                    _renderManagerOptions(results.manager, 'employee_code', 'employee_name', 'Select Manager', 'No Managers found');
                })
                .fail(function() {
                    SAXAlert.show({ type: "error", message: "An error occurred while loading Company data. Please try again." });
                });
        }


        function _renderShiftOptions(data, key, value, default_data, no_data) {
            var 
				data_length = data.length,
				shiftOptions = '',
	            select_HTML = '<option value="select" disabled selected="selected">' + default_data + '</option>',
	            counter = 0;

            if (data_length > 0) {
                for (counter = 0; counter < data_length; counter += 1) {
                    select_HTML += '<option value="' + data[counter][key] + '">' + data[counter][value] + '</option>';
                }
            }
            else {
                select_HTML = '<option value="select">' + no_data + '</option>';
            }
            shiftSelectionOptions = select_HTML;
        }

        function _renderManagerOptions(data, key, value, default_data, no_data) {
            var 
                data_length = data.length,
                shiftOptions = '',
                select_HTML = '<option value="select" disabled selected="selected">' + default_data + '</option>',
                counter = 0;

            if (data_length > 0) {
                for (counter = 0; counter < data_length; counter += 1) {
                    select_HTML += '<option value="' + data[counter][key] + '">' + data[counter][value] + '</option>';
                }
            }
            else {
                select_HTML = '<option value="select">' + no_data + '</option>';
            }
            managerSelectionOptions = select_HTML;
        }

        deleteTransaction = function(e) {
            //  alert("delete >> " + e.currentTarget.id);
            var remId = e.currentTarget.id;
            var tdDiv = remId.split("_")[2];
            var txnId = remId.split("_")[0];

            switch (txnId) {
                case 'shift':
                    if ($('#shift_statusflag_' + tdDiv).val() == 'I') {
                        $('#shift_statusflag_' + tdDiv).val('X');
                        $("#shift_" + tdDiv).hide();
                        //  $("#shift_"+tdDiv).remove();
                        //  $('#shift_statusflag_'+tdDiv).val('X');
                        //  txnData.shiftData.splice(tdDiv,1);
                    } else {
                        $("#shift_" + tdDiv).hide();
                        $('#shift_statusflag_' + tdDiv).val('D');
                    }
                    break;
                case 'ot':
                    if ($('#ot_statusflag_' + tdDiv).val() == 'I') {
                        $('#ot_statusflag_' + tdDiv).val('X');
                        $("#ot_" + tdDiv).hide();
                        //  $('#ot_statusflag_'+tdDiv).val('X');
                        //  txnData.otData.splice(tdDiv,1);
                    } else {
                        $("#ot_" + tdDiv).hide();
                        $('#ot_statusflag_' + tdDiv).val('D');
                    }
                    break;
                case 'ramadan':
                    if ($('#ramadan_statusflag_' + tdDiv).val() == 'I') {
                        $('#ramadan_statusflag_' + tdDiv).val('X');
                        $("#ramadan_" + tdDiv).hide();
                        //  $('#ramadan_statusflag_'+tdDiv).val('X');
                        //  txnData.ramadanData.splice(tdDiv,1);
                    } else {
                        $("#ramadan_" + tdDiv).hide();
                        $('#ramadan_statusflag_' + tdDiv).val('D');
                    }
                    break;
                case 'punchexception':
                    if ($('#punchexception_statusflag_' + tdDiv).val() == 'I') {
                        $('#punchexception_statusflag_' + tdDiv).val('X');
                        $("#punchexception_" + tdDiv).hide();
                        // $('#punchexception_statusflag_'+tdDiv).val('X');
                        // txnData.punchexceptionData.splice(tdDiv,1);
                    } else {
                        $("#punchexception_" + tdDiv).hide();
                        $('#punchexception_statusflag_' + tdDiv).val('D');
                    }
                    break;

                case 'workhourperday':
                    if ($('#workhourperday_statusflag_' + tdDiv).val() == 'I') {
                        $('#workhourperday_statusflag_' + tdDiv).val('X');
                        $("#workhourperday_" + tdDiv).hide();
                        //  $('#workhourperday_statusflag_'+tdDiv).val('X');
                        //  txnData.workhourperday.splice(tdDiv,1);
                    } else {
                        $("#workhourperday_" + tdDiv).hide();
                        $('#workhourperday_statusflag_' + tdDiv).val('D');
                    }
                    break;

                case 'workhourperweek':
                    if ($('#workhourperweek_statusflag_' + tdDiv).val() == 'I') {
                        $('#workhourperweek_statusflag_' + tdDiv).val('X');
                        $("#workhourperweek_" + tdDiv).hide();
                        //   $('#workhourperweek_statusflag_'+tdDiv).val('X');
                        //  txnData.workhourperweek.splice(tdDiv,1);
                    } else {
                        $("#workhourperweek_" + tdDiv).hide();
                        $('#workhourperweek_statusflag_' + tdDiv).val('D');
                    }
                    break;

                case 'workhourpermonth':
                    if ($('#workhourpermonth_statusflag_' + tdDiv).val() == 'I') {
                        $('#workhourpermonth_statusflag_' + tdDiv).val('X');
                        $("#workhourpermonth_" + tdDiv).hide();
                        //  $('#workhourpermonth_statusflag_'+tdDiv).val('X');
                        //  txnData.workhourpermonth.splice(tdDiv,1);
                    } else {
                        $("#workhourpermonth_" + tdDiv).hide();
                        $('#workhourpermonth_statusflag_' + tdDiv).val('D');
                    }
                    break;

                case 'maternity':

                    if ($('#maternity_statusflag_' + tdDiv).val() == 'I') {
                        $('#maternity_statusflag_' + tdDiv).val('X');
                        $("#maternity_" + tdDiv).hide();
                        // $('#maternity_statusflag_'+tdDiv).val('X');
                        //   txnData.maternityData.splice(tdDiv,1);
                    } else {
                        $("#maternity_" + tdDiv).hide();
                        $('#maternity_statusflag_' + tdDiv).val('D');
                    }
                    break;

                case 'termination':
                    if ($('#termination_statusflag_' + tdDiv).val() == 'I') {
                        $('#termination_statusflag_' + tdDiv).val('X');
                        $("#termination_" + tdDiv).hide();
                        //  $('#termination_statusflag_'+tdDiv).val('X');
                        //   txnData.terminationData.splice(tdDiv,1);
                    } else {
                        $("#termination_" + tdDiv).hide();
                        $('#termination_statusflag_' + tdDiv).val('D');
                    }
                    break;

                case 'manager':
                    if ($('#manager_statusflag_' + tdDiv).val() == 'I') {
                        $('#manager_statusflag_' + tdDiv).val('X');
                        $("#manager_" + tdDiv).hide();
                        //  $('#manager_statusflag_'+tdDiv).val('X');
                        //   txnData.manager.splice(tdDiv,1);
                    } else {
                        $("#manager_" + tdDiv).hide();
                        $('#manager_statusflag_' + tdDiv).val('D');
                    }
                    break;
            }

        }

        editTransaction = function(e) {
            //  alert("delete >> " + e.currentTarget.id);
            var remId = e.currentTarget.id;
            var tdDiv = remId.split("_")[2];
            var txnId = remId.split("_")[0];

            // alert('edt function invoked');

            //  $("#shift_from_date_0").removeAttr("disabled");
            //$("#shift_from_date_0").prop("disabled",false);

            //    debugger;
            // alert($("#shift_id_"+tdDiv));
            switch (txnId) {
                case 'shift':
                    $("#shift_from_date_" + tdDiv).prop("disabled", false);
                    $("#shift_to_date_" + tdDiv).prop("disabled", false);
                    $("#shift_name_" + tdDiv).prop("disabled", false);
                    $('#shift_statusflag_' + tdDiv).val('M');
                    break;
                case 'ot':
                    $("#ot_from_date_" + tdDiv).prop("disabled", false);
                    $("#ot_to_date_" + tdDiv).prop("disabled", false);
                    $('#ot_statusflag_' + tdDiv).val('M');
                    break;
                case 'ramadan':
                    $("#ramadan_from_date_" + tdDiv).prop("disabled", false);
                    $("#ramadan_to_date_" + tdDiv).prop("disabled", false);
                    $('#ramadan_statusflag_' + tdDiv).val('M');
                    break;
                case 'punchexception':
                    $("#punchexception_from_date_" + tdDiv).prop("disabled", false);
                    $("#punchexception_to_date_" + tdDiv).prop("disabled", false);
                    $('#punchexception_statusflag_' + tdDiv).val('M');
                    break;
                case 'workhourperday':
                    $("#workhourperday_from_date_" + tdDiv).prop("disabled", false);
                    $("#workhourperday_to_date_" + tdDiv).prop("disabled", false);
                    $("#workhourperday_hours_" + tdDiv).prop("disabled", false);
                    $('#workhourperday_statusflag_' + tdDiv).val('M');
                    break;
                case 'workhourperweek':
                    $("#workhourperweek_from_date_" + tdDiv).prop("disabled", false);
                    $("#workhourperweek_to_date_" + tdDiv).prop("disabled", false);
                    $("#workhourperweek_hours_" + tdDiv).prop("disabled", false);
                    $('#workhourperweek_statusflag_' + tdDiv).val('M');
                    break;
                case 'workhourpermonth':
                    $("#workhourpermonth_from_date_" + tdDiv).prop("disabled", false);
                    $("#workhourpermonth_to_date_" + tdDiv).prop("disabled", false);
                    $("#workhourpermonth_hours_" + tdDiv).prop("disabled", false);
                    $('#workhourpermonth_statusflag_' + tdDiv).val('M');
                    break;
                case 'maternity':

                    $("#maternity_from_date_" + tdDiv).prop("disabled", false);
                    //   $("#shift_to_date_"+tdDiv).prop("disabled",false);
                    $('#maternity_statusflag_' + tdDiv).val('M');
                    break;
                case 'termination':
                    $("#termination_from_date_" + tdDiv).prop("disabled", false);
                    //   $("#termination_to_date_"+tdDiv).prop("disabled",false);
                    $('#termination_statusflag_' + tdDiv).val('M');
                    break;
                case 'manager':
                    $("#manager_from_date_" + tdDiv).prop("disabled", false);
                    $("#manager_to_date_" + tdDiv).prop("disabled", false);
                    $("#manager_emp_id_" + tdDiv).prop("disabled", false);
                    $('#manager_statusflag_' + tdDiv).val('M');
                    break;
            }

            $(".myDate").removeClass('hasDatepicker')
                 .removeData('datepicker')
                .unbind()
                 .datepicker({ dateFormat: 'dd-M-yy' });

        }

        function getCurrentTableSize(tableName) {

            var rowCount = $("#" + tableName + " td").closest("tr").length;
            //    alert('number of rows in table '+ tableName+' is '+rowCount)
            return rowCount;
        }


        addTransaction = function(e) {
            //  alert("emp code  "+select_HTML);
            //  getOtherData(empcode);

            var remId = e.currentTarget.id;
            var tdDiv = remId.split("_")[0];
            var rowNum = 0;
            var newRow = '';
            var newRec = {};

            switch (tdDiv) {
                case 'shift':
                    $('#shift tr:last').remove();
                    //  rowNum=txnData.shiftData.length;
                    rowNum = getCurrentTableSize('shift');
                    newRow = '';
                    newRec = {};
                    var newshiftfromdate = 'shift_from_date_' + rowNum;
                    var newshifttodate = 'shift_to_date_' + rowNum;
                    var newshiftstatusflag = 'shift_statusflag_' + rowNum;
                    var shiftname = 'shift_name_' + rowNum;
                    var newshiftid = 'shift_id_' + rowNum;
                    // alert('adding row for '+emp-val);
                    //  debugger;
                    //shiftOptions=getOtherData(10)

                    //   $(".myDate").datepicker("destroy");

                    newRow = "<tr id='shift_" + rowNum + "'>" +
                                    "<td><input id=" + newshiftid + " type='hidden' value=''> </td>" +
                                    "<div class='col-3'>   <td><input id=" + newshiftfromdate + " class='myDate form-control'  type='text' name='fromdate' size=12 ></td>  </div>" +
                                    "<div class='col-3'>   <td><input id=" + newshifttodate + " class='myDate form-control' type='text' name='todate' size=12></td>  </div>" +
                                    "<div class='col-3'>   <td>" +
                                    "<select class='form-control' id='" + shiftname + "' name='shift_select_data'>" +
                                        shiftSelectionOptions +
                                    "</select></td>" +
                                    "<div class='col-3'>" +
                                    "<td ><label id=shift_del_" + rowNum + " class='fa fa-trash-o action-icon' data-role='txn-del' " +
                                    "data-control='button'></label></td>" +
                                    "<td> <input id=" + newshiftstatusflag + " type='hidden' value='I'></td></div>" +
                                    "</tr>";


                    $('#shift tbody').append(newRow);

                    /*
                    $(".myDate").removeClass('hasDatepicker')
                    .removeData('datepicker')
                    .unbind()
                    .datepicker({ dateFormat: 'dd-M-yy' }); */

                    $('#shift tbody').append("<tr><div class='col-2'><td></td></div>" +
                                    "<div class='col-3'>   <td> </td>  </div>" +
                                    "<div class='col-3'>   <td> </td>  </div>" +
                                    "<div clawss='col-3'>   <td> </td>  </div>" +
                                    "<div class='col-1'><td><label id=shift_add_" + rowNum + " class='fa fa-plus' data-role='txn-add' " + "data-control='button'></label></td></div></tr>");

                    //    txnData.shiftData.push(newRec);
                    break;

                case 'ot':
                    $('#ot tr:last').remove();
                    rowNum = getCurrentTableSize('ot');
                    // rowNum=txnData.ot.length;
                    newRow = '';
                    newRec = {};
                    var newotfromdate = 'ot_from_date_' + rowNum;
                    var newottodate = 'ot_to_date_' + rowNum;
                    var newotstatusflag = 'ot_statusflag_' + rowNum;
                    var newotid = 'ot_id_' + rowNum;
                    newRow = "<tr id='ot_" + rowNum + "'>" +
                                    "<td><input id=" + newotid + " type='hidden' value=''> </td>" +
                                    "<div class='col-6'>   <td><input id=" + newotfromdate + " class='myDate form-control' type='text' name='fromdate' size=12> </td>  </div>" +
                                    "<div class='col-3'>   <td><input id=" + newottodate + " class='myDate form-control' type='text' name='todate' size=12> </td>  </div>" +
                                    "<div class='col-3'><td></td>" +
                                    "<td ></label><label id=ot_del_" + rowNum + " class='fa fa-trash-o action-icon' data-role='txn-del' " +
                                    "data-control='button'></label></td>" +
                                    "<td> <input id=" + newotstatusflag + " type='hidden' value='I'></td></div>" +
                                    "</tr>";
                    $('#ot tbody').append(newRow);

                    $('#ot tbody').append("<tr><div class='col-2'><td></td></div>" +
                                    "<div class='col-3'>   <td> </td>  </div>" +
                                    "<div class='col-3'>   <td> </td>  </div>" +
                                     "<div class='col-3'>   <td> </td>  </div>" +
                                    "<div class='col-1'><td><label id=ot_add_" + rowNum + " class='fa fa-plus' data-role='txn-add' " + "data-control='button'></label></td></div></tr>");

                    //    txnData.ot.push(newRec);
                    break;

                case 'ramadan':
                    $('#ramadan tr:last').remove();
                    rowNum = getCurrentTableSize('ramadan');
                    //rowNum=txnData.ramadan.length;
                    newRow = '';
                    newRec = {};
                    var newramadanfromdate = 'ramadan_from_date_' + rowNum;
                    var newramadantodate = 'ramadan_to_date_' + rowNum;
                    var newramadanstatusflag = 'ramadan_statusflag_' + rowNum;
                    var newramadanid = 'ramadan_id_' + rowNum;
                    newRow = "<tr id='ramadan_" + rowNum + "'>" +
                                    "<td><input id=" + newramadanid + " type='hidden' value=''> </td>" +
                                    "<div class='col-6'>   <td><input id=" + newramadanfromdate + " class='myDate form-control' type='text' name='fromdate' size=12> </td>  </div>" +
                                    "<div class='col-3'>   <td><input id=" + newramadantodate + " class='myDate form-control' type='text' name='todate' size=12> </td>  </div>" +
                                    "<div class='col-3'><td></td>" +
                                    "<td ></label><label id=ramadan_del_" + rowNum + " class='fa fa-trash-o action-icon' data-role='txn-del' " +
                                    "data-control='button'></label></td>" +
                                    "<td> <input id=" + newramadanstatusflag + " type='hidden' value='I'></td></div>" +
                                    "</tr>";
                    $('#ramadan tbody').append(newRow);

                    $('#ramadan tbody').append("<tr><div class='col-2'><td></td></div>" +
                                    "<div class='col-3'>   <td> </td>  </div>" +
                                    "<div class='col-3'>   <td> </td>  </div>" +
                                     "<div class='col-3'>   <td> </td>  </div>" +
                                    "<div class='col-1'><td><label id=ramadan_add_" + rowNum + " class='fa fa-plus' data-role='txn-add' " + "data-control='button'></label></td></div></tr>");

                    //   txnData.ramadan.push(newRec);
                    break;


                case 'punchexception':
                    $('#punchexception tr:last').remove();
                    rowNum = getCurrentTableSize('punchexception');
                    //rowNum=txnData.punchexception.length;
                    newRow = '';
                    newRec = {};
                    var newpunchexceptionfromdate = 'punchexception_from_date_' + rowNum;
                    var newpunchexceptiontodate = 'punchexception_to_date_' + rowNum;
                    var newpunchexceptionstatusflag = 'punchexception_statusflag_' + rowNum;
                    var newpunchexceptionid = 'punchexception_id_' + rowNum;
                    newRow = "<tr id='punchexception_" + rowNum + "'>" +
                                    "<td><input id=" + newpunchexceptionid + " type='hidden' value=''> </td>" +
                                    "<div class='col-6'>   <td><input id=" + newpunchexceptionfromdate + " class='myDate form-control' type='text' name='fromdate' size=12> </td>  </div>" +
                                    "<div class='col-3'>   <td><input id=" + newpunchexceptiontodate + " class='myDate form-control' type='text' name='todate' size=12> </td>  </div>" +
                                    "<div class='col-3'><td></td>" +
                                    "<td ></label><label id=punchexception_del_" + rowNum + " class='fa fa-trash-o action-icon' data-role='txn-del' " +
                                    "data-control='button'></label></td>" +
                                    "<td> <input id=" + newpunchexceptionstatusflag + " type='hidden' value='I'></td></div>" +
                                    "</tr>";
                    $('#punchexception tbody').append(newRow);

                    $('#punchexception tbody').append("<tr><div class='col-2'><td></td></div>" +
                                    "<div class='col-3'>   <td> </td>  </div>" +
                                    "<div class='col-3'>   <td> </td>  </div>" +
                                     "<div class='col-3'>   <td> </td>  </div>" +
                                    "<div class='col-1'><td><label id=punchexception_add_" + rowNum + " class='fa fa-plus' data-role='txn-add' " + "data-control='button'></label></td></div></tr>");

                    //    txnData.punchexception.push(newRec);
                    break;

                case 'workhourperday':
                    $('#workhourperday tr:last').remove();
                    rowNum = getCurrentTableSize('workhourperday');
                    //rowNum=txnData.workhourperday.length;
                    newRow = '';
                    newRec = {};
                    var newworkhourperdayfromdate = 'workhourperday_from_date_' + rowNum;
                    var newworkhourperdaytodate = 'workhourperday_to_date_' + rowNum;
                    var newworkhourperdaystatusflag = 'workhourperday_statusflag_' + rowNum;
                    var newworkhourperdayhours = 'workhourperday_hours_' + rowNum;
                    var newworkhourperdayid = 'workhourperday_id_' + rowNum;
                    newRow = "<tr id='workhourperday_" + rowNum + "'>" +
                                    "<td><input id=" + newworkhourperdayid + " type='hidden' value=''> </td>" +
                                    "<div class='col-6'>   <td><input id=" + newworkhourperdayfromdate + " class='myDate form-control' type='text' name='fromdate' size=12> </td>  </div>" +
                                    "<div class='col-3'>   <td><input id=" + newworkhourperdaytodate + " class='myDate form-control' type='text' name='todate' size=12> </td>  </div>" +
                                    "<div class='col-3'>   <td>" +
                    //  "<select class='form-control' id='workhourperweek_select_data' name='workhourperweek_select_data'>"+
                    //      "<option value='select'>Select</option>"+
                    //  "</select></td>"+ 
                                    "<input type=text class='form-control' id=" + newworkhourperdayhours + " ></td>" +
                                    "<div class='col-3'>" +
                                    "<td ><label id=workhourperday_del_" + rowNum + " class='fa fa-trash-o action-icon' data-role='txn-del' " +
                                    "data-control='button'></label></td>" +
                                    "<td> <input id=" + newworkhourperdaystatusflag + " type='hidden' value='I'></td></div>" +
                                    "</tr>";
                    $('#workhourperday tbody').append(newRow);

                    $('#workhourperday tbody').append("<tr><div class='col-2'><td></td></div>" +
                                    "<div class='col-3'>   <td> </td>  </div>" +
                                    "<div class='col-3'>   <td> </td>  </div>" +
                                    "<div clawss='col-3'>   <td> </td>  </div>" +
                                    "<div class='col-1'><td><label id=workhourperday_add_" + rowNum + " class='fa fa-plus' data-role='txn-add' " + "data-control='button'></label></td></div></tr>");

                    //   txnData.workhourperday.push(newRec);
                    break;

                case 'workhourperweek':
                    $('#workhourperweek tr:last').remove();
                    rowNum = getCurrentTableSize('workhourperweek');
                    //   rowNum=txnData.workhourperweek.length;
                    newRow = '';
                    newRec = {};
                    var newworkhourperweekfromdate = 'workhourperweek_from_date_' + rowNum;
                    var newworkhourperweektodate = 'workhourperweek_to_date_' + rowNum;
                    var newworkhourperweekstatusflag = 'workhourperweek_statusflag_' + rowNum;
                    var newworkhourperweekhours = 'workhourperweek_hours_' + rowNum;
                    var newworkhourperweekid = 'workhourperweek_id_' + rowNum;
                    newRow = "<tr id='workhourperweek_" + rowNum + "'>" +
                                    "<td><input id=" + newworkhourperweekid + " type='hidden' value=''> </td>" +
                                    "<div class='col-6'>   <td><input id=" + newworkhourperweekfromdate + " class='myDate form-control' type='text' name='fromdate' size=12> </td>  </div>" +
                                    "<div class='col-3'>   <td><input id=" + newworkhourperweektodate + " class='myDate form-control' type='text' name='todate' size=12> </td>  </div>" +
                                    "<div class='col-3'>   <td>" +
                                    "<input type=text class='form-control' id=" + newworkhourperweekhours + " ></td>" +
                                    "<div class='col-3'>" +
                                    "<td ><label id=workhourperweek_del_" + rowNum + " class='fa fa-trash-o action-icon' data-role='txn-del' " +
                                    "data-control='button'></label></td>" +
                                    "<td> <input id=" + newworkhourperweekstatusflag + " type='hidden' value='I'></td></div>" +
                                    "</tr>";
                    $('#workhourperweek tbody').append(newRow);

                    $('#workhourperweek tbody').append("<tr><div class='col-2'><td></td></div>" +
                                    "<div class='col-3'>   <td> </td>  </div>" +
                                    "<div class='col-3'>   <td> </td>  </div>" +
                                    "<div clawss='col-3'>   <td> </td>  </div>" +
                                    "<div class='col-1'><td><label id=workhourperweek_add_" + rowNum + " class='fa fa-plus' data-role='txn-add' " + "data-control='button'></label></td></div></tr>");

                    //    txnData.workhourperweek.push(newRec);
                    break;

                case 'workhourpermonth':
                    $('#workhourpermonth tr:last').remove();
                    rowNum = getCurrentTableSize('workhourpermonth');
                    // rowNum=txnData.workhourpermonth.length;
                    newRow = '';
                    newRec = {};
                    var newworkhourpermonthfromdate = 'workhourpermonth_from_date_' + rowNum;
                    var newworkhourpermonthtodate = 'workhourpermonth_to_date_' + rowNum;
                    var newworkhourpermonthstatusflag = 'workhourpermonth_statusflag_' + rowNum;
                    var newworkhourpermonthhours = 'workhourpermonth_hours_' + rowNum;
                    var newworkhourpermonthid = 'workhourpermonth_id_' + rowNum;
                    newRow = "<tr id='workhourpermonth_" + rowNum + "'>" +
                                    "<td><input id=" + newworkhourpermonthid + " type='hidden' value=''> </td>" +
                                    "<div class='col-6'>   <td><input id=" + newworkhourpermonthfromdate + " class='myDate form-control' type='text' name='fromdate' size=12> </td>  </div>" +
                                    "<div class='col-3'>   <td><input id=" + newworkhourpermonthtodate + " class='myDate form-control' type='text' name='todate' size=12> </td>  </div>" +
                                    "<div class='col-3'>   <td>" +
                                     "<input type=text class='form-control' id=" + newworkhourpermonthhours + " ></td>" +
                                    "<div class='col-3'>" +
                                    "<td ><label id=workhourpermonth_del_" + rowNum + " class='fa fa-trash-o action-icon' data-role='txn-del' " +
                                    "data-control='button'></label></td>" +
                                    "<td> <input id=" + newworkhourpermonthstatusflag + " type='hidden' value='I'></td></div>" +
                                    "</tr>";
                    $('#workhourpermonth tbody').append(newRow);

                    $('#workhourpermonth tbody').append("<tr><div class='col-2'><td></td></div>" +
                                    "<div class='col-3'>   <td> </td>  </div>" +
                                    "<div class='col-3'>   <td> </td>  </div>" +
                                    "<div clawss='col-3'>   <td> </td>  </div>" +
                                    "<div class='col-1'><td><label id=workhourpermonth_add_" + rowNum + " class='fa fa-plus' data-role='txn-add' " + "data-control='button'></label></td></div></tr>");

                    //    txnData.workhourpermonth.push(newRec);
                    break;

                case 'maternity':

                    $('#Maternity tr:last').remove();
                    rowNum = getCurrentTableSize('Maternity');
                    //rowNum=txnData.maternity.length;
                    newRow = '';
                    newRec = {};
                    var newmaternityfromdate = 'maternity_from_date_' + rowNum;
                    //var new='maternity_to_date_'+rowNum;
                    var newmaternitystatusflag = 'maternity_statusflag_' + rowNum;
                    var newmaternityid = 'maternity_id_' + rowNum;
                    newRow = "<tr id='maternity_" + rowNum + "'>" +
                                    "<td><input id=" + newmaternityid + " type='hidden' value=''> </td>" +
                                    "<div class='col-6'>   <td><input id=" + newmaternityfromdate + " class='myDate form-control' type='text' name='fromdate' size=12> </td>  </div>" +
                    // "<div class='col-3'>   <td><input id="+newmaternitytodate+" class='myDate form-control' type='text' name='todate' size=12> </td>  </div>" +
                                    "<div class='col-3'><td></td><td></td>" +
                                    "<td ></label><label id=maternity_del_" + rowNum + " class='fa fa-trash-o action-icon' data-role='txn-del' " +
                                    "data-control='button'></label></td>" +
                                    "<td> <input id=" + newmaternitystatusflag + " type='hidden' value='I'></td></div>" +
                                    "</tr>";
                    $('#Maternity tbody').append(newRow);

                    $('#Maternity tbody').append("<tr><div class='col-2'><td></td></div>" +
                                    "<div class='col-3'>   <td> </td>  </div>" +
                                    "<div class='col-3'>   <td> </td>  </div>" +
                                    "<div class='col-3'>   <td> </td>  </div>" +
                                    "<div class='col-1'><td><label id=maternity_add_" + rowNum + " class='fa fa-plus' data-role='txn-add' " + "data-control='button'></label></td></div></tr>");

                    //     txnData.maternity.push(newRec);
                    break;

                case 'termination':
                    $('#termination tr:last').remove();
                    rowNum = getCurrentTableSize('termination');
                    // rowNum=txnData.termination.length;
                    newRow = '';
                    newRec = {};
                    var newterminationfromdate = 'termination_from_date_' + rowNum;
                    //   var newterminationtodate='termination_to_date_'+rowNum;
                    var newterminationstatusflag = 'termination_statusflag_' + rowNum;
                    var newterminationid = 'termination_id_' + rowNum;
                    newRow = "<tr id='termination_" + rowNum + "'>" +
                                    "<td><input id=" + newterminationid + " type='hidden' value=''> </td>" +
                                    "<div class='col-6'>   <td><input id=" + newterminationfromdate + " class='myDate form-control' type='text' name='fromdate' size=12> </td>  </div>" +
                    //   "<div class='col-3'>   <td><input id="+newterminationtodate+" class='myDate form-control' type='text' name='todate' size=12> </td>  </div>" +
                                    "<div class='col-3'><td></td><td></td>" +
                                    "<td ></label><label id=termination_del_" + rowNum + " class='fa fa-trash-o action-icon' data-role='txn-del' " +
                                    "data-control='button'></label></td>" +
                                    "<td> <input id=" + newterminationstatusflag + " type='hidden' value='I'></td></div>" +
                                    "</tr>";
                    $('#termination tbody').append(newRow);

                    $('#termination tbody').append("<tr><div class='col-2'><td></td></div>" +
                                    "<div class='col-3'>   <td> </td>  </div>" +
                                    "<div class='col-3'>   <td> </td>  </div>" +
                                     "<div class='col-3'>   <td> </td>  </div>" +
                                    "<div class='col-1'><td><label id=termination_add_" + rowNum + " class='fa fa-plus' data-role='txn-add' " + "data-control='button'></label></td></div></tr>");

                    //   txnData.termination.push(newRec);
                    break;

                case 'manager':
                    $('#manager tr:last').remove();
                    rowNum = getCurrentTableSize('manager');

                    // rowNum=txnData.manager.length;
                    newRow = '';
                    newRec = {};
                    var newmanagerfromdate = 'manager_from_date_' + rowNum;
                    var newmanagertodate = 'manager_to_date_' + rowNum;
                    var newmanagerstatusflag = 'manager_statusflag_' + rowNum;
                    var managerid = 'manager_emp_id_' + rowNum;
                    var newmanagerid = 'manager_recid_' + rowNum;
                    newRow = "<tr id='manager_" + rowNum + "'>" +
                                    "<td><input id=" + newmanagerid + " type='hidden' value=''> </td>" +
                                    "<div class='col-6'>   <td><input id=" + newmanagerfromdate + " class='myDate form-control' type='text' name='fromdate' size=12> </td>  </div>" +
                                    "<div class='col-3'>   <td><input id=" + newmanagertodate + " class='myDate form-control' type='text' name='todate' size=12> </td>  </div>" +
                                    "<div class='col-3'>   <td>" +
                                    "<select class='form-control' id='" + managerid + "' name='manager_select_data'>" +
                                        managerSelectionOptions +
                                    "</select></td>" +
                    // "<input type=text  id="+managerid+"></td>"+
                                    "<div class='col-3'>" +
                                    "<td ><label id=manager_del_" + rowNum + " class='fa fa-trash-o action-icon' data-role='txn-del' " +
                                    "data-control='button'></label></td>" +
                                    "<td> <input id=" + newmanagerstatusflag + " type='hidden' value='I'></td></div>" +
                                    "</tr>";
                    $('#manager tbody').append(newRow);

                    $('#manager tbody').append("<tr><div class='col-2'><td></td></div>" +
                                    "<div class='col-3'>   <td> </td>  </div>" +
                                    "<div class='col-3'>   <td> </td>  </div>" +
                                    "<div clawss='col-3'>   <td> </td>  </div>" +
                                    "<div class='col-1'><td><label id=manager_add_" + rowNum + " class='fa fa-plus' data-role='txn-add' " + "data-control='button'></label></td></div></tr>");

                    //   txnData.manager.push(newRec);
                    break;

            }
            $(".myDate").removeClass('hasDatepicker')
                             .removeData('datepicker')
                            .unbind()
                             .datepicker({ dateFormat: 'dd-M-yy' });
        };


        saveEmployeeData = function(txnData) {

            var todaysDate = $.datepicker.formatDate('dd-M-yy', new Date());
            var status_flag = '';
            var errorMessage = '';

            var fromDate = '';
            var toDate = '';

            //add new shift record to transaction data table to be saved - begin
            status_flag = '';
            var newShiftRec = {};
            hasValidationErrors = false;
            //alert('save called');
            var shiftRowCount = getCurrentTableSize('shift') - 1;

            var shiftRecCount = 0;

            for (var i = 0; i < shiftRowCount; i++) {
                // alert(txnData.shift.length);
                status_flag = $('#shift_statusflag_' + i).val();
                if (status_flag == 'N') shiftRecCount++;
                if (status_flag == 'D') {
                    txnData.shiftData[shiftRecCount].StatusFlag = status_flag;
                    shiftRecCount++;
                } else if (status_flag == 'I' || status_flag == 'M') {

                    newShiftRec = {};
                    if (status_flag == 'I') {
                        newShiftRec['id'] = 99;
                    } else {
                        newShiftRec['id'] = $('#shift_id_' + i).val();
                    }
                    newShiftRec['transactiontype'] = 1;
                    newShiftRec['fromdate'] = $('#shift_from_date_' + i).val();
                    newShiftRec['todate'] = $('#shift_to_date_' + i).val();
                    newShiftRec['transactiondata'] = $('#shift_name_' + i).val();

                    fromDate = Date.parse(newShiftRec['fromdate']);
                    toDate = Date.parse(newShiftRec['todate']);
                    today = Date.parse(todaysDate);


                    if (newShiftRec['fromdate'] == '' || newShiftRec['todate'] == '') {

                        SAXAlert.show({ type: "error", message: "From / To Date can not be blank" });
                        hasValidationErrors = true;
                        return;
                    } else if (toDate < fromDate) {

                        SAXAlert.show({ type: "error", message: "To Date can not be earlier than From Date" });
                        hasValidationErrors = true;
                        return;

                    } else if (toDate < today) {

                        SAXAlert.show({ type: "error", message: "To Date can not be earlier than Current Date" });
                        hasValidationErrors = true;
                        return;
                    } if (newShiftRec['transactiondata'] == 'select' || newShiftRec['transactiondata'] == null) {

                        SAXAlert.show({ type: "error", message: "Please select a Shift" });
                        hasValidationErrors = true;
                        return;
                    } else {
                        newShiftRec['StatusFlag'] = status_flag;
                        txnData.shiftData[shiftRecCount] = newShiftRec;
                        shiftRecCount++;
                    }
                }

            }

            //  alert('flag is '+hasValidationErrors); 
            // alert(newShiftRec); 
            // alert(JSON.stringify(txnData, null, 4));
            console.log('printing txnData for shift');
            for (var i = 0; i < txnData.shiftData.length; i++) {
                console.log('id-> ' + txnData.shiftData[i].id);
                console.log('fromdate-> ' + txnData.shiftData[i].fromdate);
                console.log('todate-> ' + txnData.shiftData[i].todate);
                console.log('StatusFlag-> ' + txnData.shiftData[i].StatusFlag);
            }
            //add new shift record to transaction data table to be saved - end


            //add new ot record to transaction data table to be saved - begin
            status_flag = '';
            var newotRec = {};

            var otRowCount = getCurrentTableSize('ot') - 1;

            var otRecCount = 0;

            for (var i = 0; i < otRowCount; i++) {
                status_flag = $('#ot_statusflag_' + i).val();
                if (status_flag == 'N') otRecCount++;
                if (status_flag == 'D') {
                    txnData.ot[otRecCount].StatusFlag = status_flag;
                    otRecCount++;
                } else if (status_flag == 'I' || status_flag == 'M') {
                    newotRec = {};
                    if (status_flag == 'I') {
                        newotRec['id'] = 99;
                    } else {
                        newotRec['id'] = $('#ot_id_' + i).val();
                    }

                    newotRec['transactiontype'] = 2;
                    newotRec['fromdate'] = $('#ot_from_date_' + i).val();
                    newotRec['todate'] = $('#ot_to_date_' + i).val();
                    newotRec['transactiondata'] = 1;
                    newotRec['StatusFlag'] = status_flag;

                    fromDate = Date.parse(newotRec['fromdate']);
                    toDate = Date.parse(newotRec['todate']);
                    today = Date.parse(todaysDate);


                    if (newotRec['fromdate'] == '' || newotRec['todate'] == '') {

                        SAXAlert.show({ type: "error", message: "From / To Date can not be blank" });
                        hasValidationErrors = true;
                        return;
                    } else if (toDate < fromDate) {
                        SAXAlert.show({ type: "error", message: "To Date can not be earlier than From Date" });
                        hasValidationErrors = true;
                        return;

                    } else if (toDate < today) {

                        SAXAlert.show({ type: "error", message: "To Date can not be earlier than Current Date" });
                        hasValidationErrors = true;
                        return;
                    } else {
                        txnData.ot[otRecCount] = newotRec;
                        otRecCount++;
                    }
                }
            }
            // alert(JSON.stringify(txnData, null, 4));
            console.log('printing txnData for OT');
            for (var i = 0; i < txnData.ot.length; i++) {
                console.log('id-> ' + txnData.ot[i].id);
                console.log('fromdate-> ' + txnData.ot[i].fromdate);
                console.log('todate-> ' + txnData.ot[i].todate);
                console.log('StatusFlag-> ' + txnData.ot[i].StatusFlag);
            }
            //add new ot record to transaction data table to be saved - end

            //add new ramadan record to transaction data table to be saved - begin
            status_flag = '';
            var newramadanRec = {};

            var ramadanRowCount = getCurrentTableSize('ramadan') - 1;

            var ramadanRecCount = 0;

            for (var i = 0; i < ramadanRowCount; i++) {
                status_flag = $('#ramadan_statusflag_' + i).val();
                if (status_flag == 'N') ramadanRecCount++;
                if (status_flag == 'D') {
                    txnData.ramadan[ramadanRecCount].StatusFlag = status_flag;
                    ramadanRecCount++;
                } else if (status_flag == 'I' || status_flag == 'M') {
                    newramadanRec = {};
                    if (status_flag == 'I') {
                        newramadanRec['id'] = 99;
                    } else {
                        newramadanRec['id'] = $('#ramadan_id_' + i).val();
                    }
                    newramadanRec['transactiontype'] = 3;
                    newramadanRec['fromdate'] = $('#ramadan_from_date_' + i).val();
                    newramadanRec['todate'] = $('#ramadan_to_date_' + i).val();
                    newramadanRec['transactiondata'] = 1;
                    newramadanRec['StatusFlag'] = status_flag;

                    fromDate = Date.parse(newramadanRec['fromdate']);
                    toDate = Date.parse(newramadanRec['todate']);
                    today = Date.parse(todaysDate);

                    if (newramadanRec['fromdate'] == '' || newramadanRec['todate'] == '') {

                        SAXAlert.show({ type: "error", message: "From / To Date can not be blank" });
                        hasValidationErrors = true;
                        return;
                    } else if (toDate < fromDate) {
                        SAXAlert.show({ type: "error", message: "To Date can not be earlier than From Date" });
                        hasValidationErrors = true;
                        return;

                    } else if (toDate < today) {

                        SAXAlert.show({ type: "error", message: "To Date can not be earlier than Current Date" });
                        hasValidationErrors = true;
                        return;
                    } else {
                        txnData.ramadan[ramadanRecCount] = newramadanRec;
                        ramadanRecCount++;
                    }

                }
            }
            // alert(JSON.stringify(txnData, null, 4));
            console.log('printing txnData for Ramadan');
            for (var i = 0; i < txnData.ramadan.length; i++) {
                console.log('id-> ' + txnData.ramadan[i].id);
                console.log('fromdate-> ' + txnData.ramadan[i].fromdate);
                console.log('todate-> ' + txnData.ramadan[i].todate);
                console.log('StatusFlag-> ' + txnData.ramadan[i].StatusFlag);
            }
            //add new ramadan record to transaction data table to be saved - end

            //add new punchexception record to transaction data table to be saved - begin
            status_flag = '';
            var newpunchexceptionRec = {};

            var punchexceptionRowCount = getCurrentTableSize('punchexception') - 1;

            var punchexceptionRecCount = 0;

            for (var i = 0; i < punchexceptionRowCount; i++) {
                status_flag = $('#punchexception_statusflag_' + i).val();
                if (status_flag == 'N') punchexceptionRecCount++;
                if (status_flag == 'D') {
                    txnData.punchexception[punchexceptionRecCount].StatusFlag = status_flag;
                    punchexceptionRecCount++;
                } else if (status_flag == 'I' || status_flag == 'M') {
                    newpunchexceptionRec = {};
                    if (status_flag == 'I') {
                        newpunchexceptionRec['id'] = 99;
                    } else {
                        newpunchexceptionRec['id'] = $('#punchexception_id_' + i).val();
                    }

                    newpunchexceptionRec['transactiontype'] = 4;
                    newpunchexceptionRec['fromdate'] = $('#punchexception_from_date_' + i).val();
                    newpunchexceptionRec['todate'] = $('#punchexception_to_date_' + i).val();
                    newpunchexceptionRec['transactiondata'] = 1;
                    newpunchexceptionRec['StatusFlag'] = status_flag;
                    fromDate = Date.parse(newpunchexceptionRec['fromdate']);
                    toDate = Date.parse(newpunchexceptionRec['todate']);
                    today = Date.parse(todaysDate);
                    if (newpunchexceptionRec['fromdate'] == '' || newpunchexceptionRec['todate'] == '') {

                        SAXAlert.show({ type: "error", message: "From / To Date can not be blank" });
                        hasValidationErrors = true;
                        return;
                    } else if (toDate < fromDate) {
                        SAXAlert.show({ type: "error", message: "To Date can not be earlier than From Date" });
                        hasValidationErrors = true;
                        return;

                    } else if (toDate < today) {

                        SAXAlert.show({ type: "error", message: "To Date can not be earlier than Current Date" });
                        hasValidationErrors = true;
                        return;
                    } else {
                        txnData.punchexception[punchexceptionRecCount] = newpunchexceptionRec;
                        punchexceptionRecCount++;
                    }

                }

            }
            // alert(JSON.stringify(txnData, null, 4));
            console.log('printing txnData for punch');
            for (var i = 0; i < txnData.punchexception.length; i++) {

                console.log('id-> ' + txnData.punchexception[i].id);
                console.log('fromdate-> ' + txnData.punchexception[i].fromdate);
                console.log('todate-> ' + txnData.punchexception[i].todate);
                console.log('StatusFlag-> ' + txnData.punchexception[i].StatusFlag);
            }
            //add new punchexception record to transaction data table to be saved - end


            //add new workhourperday record to transaction data table to be saved - begin
            status_flag = '';
            var newworkhourperdayRec = {};

            var workhourperdayRowCount = getCurrentTableSize('workhourperday') - 1;

            var workhourperdayRecCount = 0;

            for (var i = 0; i < workhourperdayRowCount; i++) {
                status_flag = $('#workhourperday_statusflag_' + i).val();
                if (status_flag == 'N') workhourperdayRecCount++;
                if (status_flag == 'D') {
                    txnData.workhourperday[i].StatusFlag = status_flag;
                    workhourperdayRecCount++;
                } else if (status_flag == 'I' || status_flag == 'M') {
                    newworkhourperdayRec = {};
                    if (status_flag == 'I') {
                        newworkhourperdayRec['id'] = 99;
                    } else {
                        newworkhourperdayRec['id'] = $('#workhourperday_id_' + i).val();
                    }
                    newworkhourperdayRec['transactiontype'] = 6;
                    newworkhourperdayRec['fromdate'] = $('#workhourperday_from_date_' + i).val();
                    newworkhourperdayRec['todate'] = $('#workhourperday_to_date_' + i).val();
                    newworkhourperdayRec['transactiondata'] = $('#workhourperday_hours_' + i).val();
                    newworkhourperdayRec['StatusFlag'] = status_flag;

                    fromDate = Date.parse(newworkhourperdayRec['fromdate']);
                    toDate = Date.parse(newworkhourperdayRec['todate']);
                    today = Date.parse(todaysDate);

                    if (newworkhourperdayRec['fromdate'] == '' || newworkhourperdayRec['todate'] == '') {

                        SAXAlert.show({ type: "error", message: "From / To Date can not be blank" });
                        hasValidationErrors = true;
                        return;
                    } else if (toDate < fromDate) {
                        SAXAlert.show({ type: "error", message: "To Date can not be earlier than From Date" });
                        hasValidationErrors = true;
                        return;

                    } else if (toDate < today) {

                        SAXAlert.show({ type: "error", message: "To Date can not be earlier than Current Date" });
                        hasValidationErrors = true;
                        return;

                    } else if (newworkhourperdayRec['transactiondata'] > 24) {
                        SAXAlert.show({ type: "error", message: "Work hours per day can not be more than 24" });
                        hasValidationErrors = true;
                        return;
                    } else {
                        txnData.workhourperday[workhourperdayRecCount] = newworkhourperdayRec;
                        workhourperdayRecCount++;
                    }

                }
            }
            // alert(JSON.stringify(txnData, null, 4));
            console.log('printing wh day Data');
            for (var i = 0; i < txnData.workhourperday.length; i++) {
                console.log('id-> ' + txnData.workhourperday[i].id);
                console.log('fromdate-> ' + txnData.workhourperday[i].fromdate);
                console.log('todate-> ' + txnData.workhourperday[i].todate);
                console.log('StatusFlag-> ' + txnData.workhourperday[i].StatusFlag);
            }
            //add new workhourperday record to transaction data table to be saved - end


            //add new workhourperweek record to transaction data table to be saved - begin
            status_flag = '';
            var newworkhourperweekRec = {};
            var workhourperweekRowCount = getCurrentTableSize('workhourperweek') - 1;

            var workhourperweekRecCount = 0;

            for (var i = 0; i < workhourperweekRowCount; i++) {
                status_flag = $('#workhourperweek_statusflag_' + i).val();
                if (status_flag == 'N') workhourperweekRecCount++;
                if (status_flag == 'D') {
                    txnData.workhourperweek[i].StatusFlag = status_flag;
                    workhourperweekRecCount++;
                } else if (status_flag == 'I' || status_flag == 'M') {
                    newworkhourperweekRec = {};
                    if (status_flag == 'I') {
                        newworkhourperweekRec['id'] = 99;
                    } else {
                        newworkhourperweekRec['id'] = $('#workhourperweek_id_' + i).val();
                    }
                    newworkhourperweekRec['transactiontype'] = 7;
                    newworkhourperweekRec['fromdate'] = $('#workhourperweek_from_date_' + i).val();
                    newworkhourperweekRec['todate'] = $('#workhourperweek_to_date_' + i).val();
                    newworkhourperweekRec['transactiondata'] = $('#workhourperweek_hours_' + i).val();
                    newworkhourperweekRec['StatusFlag'] = status_flag;

                    fromDate = Date.parse(newworkhourperweekRec['fromdate']);
                    toDate = Date.parse(newworkhourperweekRec['todate']);
                    today = Date.parse(todaysDate);

                    if (newworkhourperweekRec['fromdate'] == '' || newworkhourperweekRec['todate'] == '') {

                        SAXAlert.show({ type: "error", message: "From / To Date can not be blank" });
                        hasValidationErrors = true;
                        return;
                    } else if (toDate < fromDate) {
                        SAXAlert.show({ type: "error", message: "To Date can not be earlier than From Date" });
                        hasValidationErrors = true;
                        return;

                    } else if (toDate < today) {

                        SAXAlert.show({ type: "error", message: "To Date can not be earlier than Current Date" });
                        hasValidationErrors = true;
                        return;
                    } else if (newworkhourperweekRec['transactiondata'] > 168) {
                        SAXAlert.show({ type: "error", message: "Work hours per week can not be more than 168" });
                        hasValidationErrors = true;
                        return;
                    } else {
                        txnData.workhourperweek[workhourperweekRecCount] = newworkhourperweekRec;
                        workhourperweekRecCount++;
                    }

                }
            }
            // alert(JSON.stringify(txnData, null, 4));
            console.log(' printing wh week Data');
            for (var i = 0; i < txnData.workhourperweek.length; i++) {
                console.log('id-> ' + txnData.workhourperweek[i].id);
                console.log('fromdate-> ' + txnData.workhourperweek[i].fromdate);
                console.log('todate-> ' + txnData.workhourperweek[i].todate);
                console.log('StatusFlag-> ' + txnData.workhourperweek[i].StatusFlag);
            }
            //add new workhourperweek record to transaction data table to be saved - end

            //add new workhourpermonth record to transaction data table to be saved - begin
            status_flag = '';
            var newworkhourpermonthRec = {};

            var workhourpermonthRowCount = getCurrentTableSize('workhourpermonth') - 1;

            var workhourpermonthRecCount = 0;

            for (var i = 0; i < workhourpermonthRowCount; i++) {
                status_flag = $('#workhourpermonth_statusflag_' + i).val();
                if (status_flag == 'N') workhourpermonthRecCount++;
                if (status_flag == 'D') {
                    txnData.workhourpermonth[i].StatusFlag = status_flag;
                    workhourpermonthRecCount++;
                } else if (status_flag == 'I' || status_flag == 'M') {
                    newworkhourpermonthRec = {};
                    if (status_flag == 'I') {
                        newworkhourpermonthRec['id'] = 99;
                    } else {
                        newworkhourpermonthRec['id'] = $('#workhourpermonth_id_' + i).val();
                    }
                    newworkhourpermonthRec['transactiontype'] = 8;
                    newworkhourpermonthRec['fromdate'] = $('#workhourpermonth_from_date_' + i).val();
                    newworkhourpermonthRec['todate'] = $('#workhourpermonth_to_date_' + i).val();
                    newworkhourpermonthRec['transactiondata'] = $('#workhourpermonth_hours_' + i).val();
                    newworkhourpermonthRec['StatusFlag'] = status_flag;

                    fromDate = Date.parse(newworkhourpermonthRec['fromdate']);
                    toDate = Date.parse(newworkhourpermonthRec['todate']);
                    today = Date.parse(todaysDate);

                    if (newworkhourpermonthRec['fromdate'] == '' || newworkhourpermonthRec['todate'] == '') {

                        SAXAlert.show({ type: "error", message: "From / To Date can not be blank" });
                        hasValidationErrors = true;
                        return;
                    } else if (toDate < fromDate) {
                        SAXAlert.show({ type: "error", message: "To Date can not be earlier than From Date" });
                        hasValidationErrors = true;
                        return;

                    } else if (toDate < today) {

                        SAXAlert.show({ type: "error", message: "To Date can not be earlier than Current Date" });
                        hasValidationErrors = true;
                        return;
                    } else if (newworkhourpermonthRec['transactiondata'] > 744) {
                        SAXAlert.show({ type: "error", message: "Work hours per month can not be more than 744" });
                        hasValidationErrors = true;
                        return;
                    } else {
                        txnData.workhourpermonth[workhourpermonthRecCount] = newworkhourpermonthRec;
                        workhourpermonthRecCount++;
                    }

                }
            }
            // alert(JSON.stringify(txnData, null, 4));
            console.log('printing wh month Data');
            for (var i = 0; i < txnData.workhourpermonth.length; i++) {
                console.log('id-> ' + txnData.workhourpermonth[i].id);
                console.log('fromdate-> ' + txnData.workhourpermonth[i].fromdate);
                console.log('todate-> ' + txnData.workhourpermonth[i].todate);
                console.log('StatusFlag-> ' + txnData.workhourpermonth[i].StatusFlag);
            }
            //add new workhourpermonth record to transaction data table to be saved - end


            //add new maternity record to transaction data table to be saved - begin
            status_flag = '';
            var newmaternityRec = {};

            var maternityRowCount = getCurrentTableSize('Maternity') - 1;

            var maternityRecCount = 0;

            for (var i = 0; i < maternityRowCount; i++) {


                status_flag = $('#maternity_statusflag_' + i).val();

                if (status_flag == 'N') maternityRecCount++;

                if (status_flag == 'D') {
                    txnData.maternity[maternityRecCount].StatusFlag = status_flag;
                    maternityRecCount++;
                    //   alert(maternityRecCount);
                } else if (status_flag == 'I' || status_flag == 'M') {
                    newmaternityRec = {};
                    if (status_flag == 'I') {
                        newmaternityRec['id'] = 99;
                    } else {
                        newmaternityRec['id'] = $('#maternity_id_' + i).val();
                    }
                    newmaternityRec['transactiontype'] = 5;
                    newmaternityRec['fromdate'] = $('#maternity_from_date_' + i).val();
                    newmaternityRec['todate'] = $('#maternity_from_date_' + i).val();
                    newmaternityRec['transactiondata'] = 1;
                    newmaternityRec['StatusFlag'] = status_flag;
                    if (newmaternityRec['fromdate'] == '') {

                        SAXAlert.show({ type: "error", message: "Child date of birth can not be blank" });
                        hasValidationErrors = true;
                        return;
                    } else {
                        txnData.maternity[maternityRecCount] = newmaternityRec;
                        maternityRecCount++;
                    }
                }
            }
            // alert(JSON.stringify(txnData, null, 4));
            console.log('printing txnData for maternity');
            for (var i = 0; i < txnData.maternity.length; i++) {
                console.log('id-> ' + txnData.maternity[i].id);
                console.log('fromdate-> ' + txnData.maternity[i].fromdate);
                console.log('todate-> ' + txnData.maternity[i].todate);
                console.log('StatusFlag-> ' + txnData.maternity[i].StatusFlag);
            }
            //add new maternity record to transaction data table to be saved - end

            //add new termination record to transaction data table to be saved - begin
            status_flag = '';
            var newterminationRec = {};

            var terminationRowCount = getCurrentTableSize('termination') - 1;

            var terminationRecCount = 0;

            for (var i = 0; i < terminationRowCount; i++) {
                status_flag = $('#termination_statusflag_' + i).val();
                if (status_flag == 'N') terminationRecCount++;
                if (status_flag == 'D') {
                    txnData.termination[terminationRecCount].StatusFlag = status_flag;
                    terminationRecCount++;
                } else if (status_flag == 'I' || status_flag == 'M') {
                    newterminationRec = {};
                    if (status_flag == 'I') {
                        newterminationRec['id'] = 99;
                    } else {
                        newterminationRec['id'] = $('#termination_id_' + i).val();
                    }
                    newterminationRec['transactiontype'] = 9;
                    newterminationRec['fromdate'] = $('#termination_from_date_' + i).val();
                    newterminationRec['todate'] = $('#termination_from_date_' + i).val();
                    newterminationRec['transactiondata'] = 1;
                    newterminationRec['StatusFlag'] = status_flag;

                    if (newterminationRec['fromdate'] == '') {

                        SAXAlert.show({ type: "error", message: "Termination date can not be blank" });
                        hasValidationErrors = true;
                        return;
                    } else {
                        txnData.termination[terminationRecCount] = newterminationRec;
                        terminationRecCount++;
                    }
                    txnData.termination[i] = newterminationRec;
                }
            }
            // alert(JSON.stringify(txnData, null, 4));
            console.log('printing txnData for termination');
            for (var i = 0; i < txnData.termination.length; i++) {
                console.log('id-> ' + txnData.termination[i].id);
                console.log('fromdate-> ' + txnData.termination[i].fromdate);
                console.log('todate-> ' + txnData.termination[i].todate);
                console.log('StatusFlag-> ' + txnData.termination[i].StatusFlag);
            }
            //add new termination record to transaction data table to be saved - end

            //add new manager record to transaction data table to be saved - begin
            status_flag = '';
            var newmanagerRec = {};

            var managerRowCount = getCurrentTableSize('manager') - 1;

            var managerRecCount = 0;

            for (var i = 0; i < managerRowCount; i++) {
                status_flag = $('#manager_statusflag_' + i).val();
                if (status_flag == 'N') managerRecCount++;
                if (status_flag == 'D') {
                    txnData.manager[managerRecCount].StatusFlag = status_flag;
                    managerRecCount++;
                } else if (status_flag == 'I' || status_flag == 'M') {
                    newmanagerRec = {};
                    if (status_flag == 'I') {
                        newmanagerRec['id'] = 99;
                    } else {
                        newmanagerRec['id'] = $('#manager_id_' + i).val();
                    }
                    newmanagerRec['transactiontype'] = 10;
                    newmanagerRec['fromdate'] = $('#manager_from_date_' + i).val();
                    newmanagerRec['todate'] = $('#manager_to_date_' + i).val();
                    newmanagerRec['transactiondata'] = $('#manager_emp_id_' + i).val();
                    newmanagerRec['StatusFlag'] = status_flag;

                    fromDate = Date.parse(newmanagerRec['fromdate']);
                    toDate = Date.parse(newmanagerRec['todate']);
                    today = Date.parse(todaysDate);
                    if (newmanagerRec['fromdate'] == '' || newmanagerRec['todate'] == '') {

                        SAXAlert.show({ type: "error", message: "From / To Date can not be blank" });
                        hasValidationErrors = true;
                        return;
                    } else if (toDate < fromDate) {

                        SAXAlert.show({ type: "error", message: "To Date can not be earlier than From Date" });
                        hasValidationErrors = true;
                        return;

                    } else if (toDate < today) {

                        SAXAlert.show({ type: "error", message: "To Date can not be earlier than Current Date" });
                        hasValidationErrors = true;
                        return;
                    } if (newmanagerRec['transactiondata'] == 'select' || newmanagerRec['transactiondata'] == null) {

                        SAXAlert.show({ type: "error", message: "Please select a Manager" });
                        hasValidationErrors = true;
                        return;
                    } else {
                        txnData.manager[managerRecCount] = newmanagerRec;
                        managerRecCount++;
                    }


                }
            }
            //alert(JSON.stringify(txnData, null, 4));
            console.log('printing txnData for manager');
            for (var i = 0; i < txnData.manager.length; i++) {
                console.log('id-> ' + txnData.manager[i].id);
                console.log('fromdate-> ' + txnData.manager[i].fromdate);
                console.log('todate-> ' + txnData.manager[i].todate);
                console.log('StatusFlag-> ' + txnData.manager[i].StatusFlag);
            }
            //add new manager record to transaction data table to be saved - end

            //alert('Employee Transaction data saved');

            // debugger;

            if (!hasValidationErrors) {

                console.log('calling SAXHTTP AJAX method');



                SAXHTTP.AJAX(
    						"manage_employee.aspx/addEmployeeTransaction",
    						{ current: JSON.stringify(txnData) }
    					)
                       .done(function(data) {

                           // alert('returned from SAX call with status '+ data.d.status);

                           //  SAXAlert.show({ type: "success", message: "Employee Transaction data saved." });

                           if (data.d.status == "success") {
                               //   window.location.reload();
                               SAXAlert.show({ type: "success", message: "Employee Transaction data saved." });
                               // $('#saveForm')[0].reset();
                               dialogs.save.modal("hide");
                           }
                           if (data.d.status == "error") {
                               SAXAlert.show({ type: "error", message: "An error occurred while saving Employee details. Please try again." });
                               //  $('#saveForm')[0].reset();
                               //  $('#empTransaction').hide();
                               // dialogs.save.modal("hide");
                               event.preventDefault();
                           }


                       })
    				.fail(function() {
    				    SAXAlert.show({ type: "error", message: "An error occurred while saving Employee details. Please try again." });
    				    // dialogs.save.modal("show");
    				    event.preventDefault();
    				})



            }

            /*
            if (hasValidationErrors) {
             
            event.preventDefault();
            } 
            return false; */

        }


        _showEmpDetailsTab = function() {

            //   alert('details invoked');
            $("#empDataTransactionTabOption").removeClass('active'); //empDataTransactionTabOption
            $("#edtTab").removeClass('active'); //edtTab

            $("#empDetailsTabOption").addClass('active'); //empDetailsTabOption
            $("#empDetailsTab").addClass('active'); //empDetailsTab
        };

        _showEmpDataTransactionsTab = function() {
            //   alert('edt invoked');
            $("#empDetailsTabOption").removeClass('active'); //empDetailsTabOption
            $("#empDetailsTab").removeClass('active'); //empDetailsTab

            $("#empDataTransactionTabOption").addClass('active'); //empDataTransactionTabOption
            $("#edtTab").addClass('active'); //edtTab
        };

        function _displayErrorMessage(errormessage) {

            SAXAlert.show({ type: "error", message: errormessage });
            return false;
        }




        function _renderforEmployee(data) {

            hasValidationErrors = false;

            txnData = data;

            //   alert('in rendering edt rec');

            $("#shift").find("tr:gt(0)").remove();

            var shiftrec = '';

            for (i = 0; i < data.shiftData.length; i++) {
                id = data.shiftData[i].id;
                shiftFromDate = new Date(data.shiftData[i].fromdate);

                shiftToDate = new Date(data.shiftData[i].todate);
                shiftname = data.shiftData[i].shift_desc;
                shiftcode = data.shiftData[i].transactiondata;
                // alert(shiftname);
                shiftStatus = data.shiftData[i].StatusFlag;

                shiftrec = '';

                shiftrec += '<tr id="shift_' + i + '">';
                shiftrec += '<td><input id=shift_id_' + i + ' type="hidden" value="' + id + '"</td>';

                if (shiftFromDate != null) {

                    shiftrec += '<td ><input id=shift_from_date_' + i + ' type="text" class="myDate form-control" size=15  disabled value=' + moment(shiftFromDate).format("DD-MMM-YYYY") + ">";

                    //  $("#shift_from_date_"+i).prop("disabled",true);
                } else {

                    shiftrec += '<td>'
                }
                shiftrec += '</td>';

                if (shiftToDate != null) {
                    // $("#shift_To_date").val(moment(shiftToDate).format("DD-MMM-YYYY"));
                    shiftrec += '<td ><input id=shift_to_date_' + i + ' type="text" class="myDate form-control" size=15  disabled value=' + moment(shiftToDate).format("DD-MMM-YYYY") + ">";

                } else {
                    // $("#shift_To_date").val("");
                    shiftrec += '<td>'
                }
                shiftrec += '</td>';

                if (shiftname != null) {
                    shiftrec += '<td> <select class="form-control" id="shift_name_' + i + '" name="shift_select_data">"' +
                                            shiftSelectionOptions +
                                        "</select></td>";
                }

                shiftrec += "<td ><label id=shift_del_" + i + " class='fa fa-trash-o action-icon' data-role='txn-del' " +
                        "data-control='button'></label></td>" +
                       "<td><label id=shift_edit_" + i + " class='fa fa-pencil action-icon' data-role='txn-edit' " +
                        "data-control='button'></label></td>";

                if (shiftStatus != null) {
                    // $("#shift_To_date").val(moment(shiftToDate).format("DD-MMM-YYYY"));
                    shiftrec += '<td> <input id=shift_statusflag_' + i + ' type="hidden" value="' + shiftStatus + '">';
                } else {
                    // $("#shift_To_date").val("");
                    shiftrec += '<td>'
                }
                shiftrec += '</td>';

                shiftrec += '</tr>';
                $('#shift tbody').append(shiftrec);
                $("#shift_name_" + i).val(shiftcode);
                $("#shift_name_" + i).prop("disabled", true);

            }

            $('#shift tbody').append("<tr><td></td><td></td><td></td><td></td></div>" +
                                        "<td><label id=shift_add_" + i + " class='fa fa-plus' data-role='txn-add' " + "data-control='button'></label></td></tr>");
            //end of rendering shift data


            // begin rendering of OT data
            $("#ot").find("tr:gt(0)").remove();
            //   $('#ot tbody').empty();
            var otrec = '';

            for (i = 0; i < data.ot.length; i++) {
                id = data.ot[i].id;
                otFromDate = new Date(data.ot[i].fromdate);

                otToDate = new Date(data.ot[i].todate);

                otStatus = data.ot[i].StatusFlag;
                otrec = '';

                otrec += '<tr id="ot_' + i + '">';
                otrec += '<td><input id=ot_id_' + i + ' type="hidden" value="' + id + '"</td>';

                if (otFromDate != null) {

                    otrec += '<td ><input id=ot_from_date_' + i + ' type="text" class="myDate form-control" size=15  disabled value=' + moment(otFromDate).format("DD-MMM-YYYY") + ">";

                } else {

                    otrec += '<td>'
                }
                otrec += '</td>';

                if (otToDate != null) {
                    // $("#ot_To_date").val(moment(otToDate).format("DD-MMM-YYYY"));
                    otrec += '<td ><input id=ot_to_date_' + i + ' type="text" class="myDate form-control" size=15  disabled value=' + moment(otToDate).format("DD-MMM-YYYY") + ">";

                } else {
                    // $("#ot_To_date").val("");
                    otrec += '<td>'
                }
                otrec += '</td><td></td>';


                otrec += "<td ><label id=ot_del_" + i + " class='fa fa-trash-o action-icon' data-role='txn-del' " +
                        "data-control='button'></label></td>" +
                        "<td><label id=ot_edit_" + i + " class='fa fa-pencil action-icon' data-role='txn-edit' " +
                        "data-control='button'></label></td>";

                if (otStatus != null) {
                    // $("#ot_To_date").val(moment(otToDate).format("DD-MMM-YYYY"));
                    otrec += '<td> <input id=ot_statusflag_' + i + ' type="hidden" value="' + otStatus + '">';
                } else {
                    // $("#ot_To_date").val("");
                    otrec += '<td>'
                }
                otrec += '</td>';
                otrec += '</tr>';
                $('#ot tbody').append(otrec);
            }

            $('#ot tbody').append("<tr><div class='col-2'><td></td></div>" +
                                        "<div class='col-3'>   <td> </td>  </div>" +
                                        "<div class='col-3'>   <td> </td>  </div>" +
                                         "<div class='col-3'>   <td> </td>  </div>" +
                                        "<div class='col-1'><td><label id=ot_add_" + i + " class='fa fa-plus' data-role='txn-add' " + "data-control='button'></label></td></div></tr>");
            //end of rendering OT data

            // begin rendering of ramadan data
            $("#ramadan").find("tr:gt(0)").remove();
            //    $('#ramadan tbody').empty();
            var ramadanrec = '';

            for (i = 0; i < data.ramadan.length; i++) {
                id = data.ramadan[i].id;
                ramadanFromDate = new Date(data.ramadan[i].fromdate);

                ramadanToDate = new Date(data.ramadan[i].todate);

                ramadanStatus = data.ramadan[i].StatusFlag;
                ramadanrec = '';
                ramadanrec += '<tr id="ramadan_' + i + '">';
                ramadanrec += '<td><input id=ramadan_id_' + i + ' type="hidden" value="' + id + '"</td>';

                if (ramadanFromDate != null) {

                    ramadanrec += '<td ><input id=ramadan_from_date_' + i + ' type="text" class="myDate form-control" size=15  disabled value=' + moment(ramadanFromDate).format("DD-MMM-YYYY") + ">";

                } else {

                    ramadanrec += '<td>'
                }
                ramadanrec += '</td>';

                if (ramadanToDate != null) {
                    // $("#ramadan_To_date").val(moment(ramadanToDate).format("DD-MMM-YYYY"));
                    ramadanrec += '<td ><input id=ramadan_to_date_' + i + ' type="text" class="myDate form-control" size=15  disabled value=' + moment(ramadanToDate).format("DD-MMM-YYYY") + ">";

                } else {
                    // $("#ramadan_To_date").val("");
                    ramadanrec += '<td>'
                }
                ramadanrec += '</td><td></td>';


                ramadanrec += "<td ><label id=ramadan_del_" + i + " class='fa fa-trash-o action-icon' data-role='txn-del' " +
                        "data-control='button'></label></td>" +
                        "<td><label id=ramadan_edit_" + i + " class='fa fa-pencil action-icon' data-role='txn-edit' " +
                        "data-control='button'></label></td>";

                if (ramadanStatus != null) {
                    // $("#ramadan_To_date").val(moment(ramadanToDate).format("DD-MMM-YYYY"));
                    ramadanrec += '<td> <input id=ramadan_statusflag_' + i + ' type="hidden" value="' + ramadanStatus + '">';
                } else {
                    // $("#ramadan_To_date").val("");
                    ramadanrec += '<td>'
                }
                ramadanrec += '</td>';
                ramadanrec += '</tr>';
                $('#ramadan tbody').append(ramadanrec);
            }

            $('#ramadan tbody').append("<tr><div class='col-2'><td></td></div>" +
                                        "<div class='col-3'>   <td> </td>  </div>" +
                                        "<div class='col-3'>   <td> </td>  </div>" +
                                          "<div class='col-3'>   <td> </td>  </div>" +
                                        "<div class='col-1'><td><label id=ramadan_add_" + i + " class='fa fa-plus' data-role='txn-add' " + "data-control='button'></label></td></div></tr>");
            //end of rendering ramadan data

            // begin rendering of punchexception data

            $("#punchexception").find("tr:gt(0)").remove();

            //$('#punchexception tbody').empty();
            var punchexceptionrec = '';

            for (i = 0; i < data.punchexception.length; i++) {
                id = data.punchexception[i].id;
                punchexceptionFromDate = new Date(data.punchexception[i].fromdate);

                punchexceptionToDate = new Date(data.punchexception[i].todate);

                punchexceptionStatus = data.punchexception[i].StatusFlag;
                punchexceptionrec = '';

                punchexceptionrec += '<tr id="punchexception_' + i + '">';
                punchexceptionrec += '<td><input id=punchexception_id_' + i + ' type="hidden" value="' + id + '"</td>';

                if (punchexceptionFromDate != null) {

                    punchexceptionrec += '<td ><input id=punchexception_from_date_' + i + ' type="text" class="myDate form-control" size=15  disabled value=' + moment(punchexceptionFromDate).format("DD-MMM-YYYY") + ">";

                } else {

                    punchexceptionrec += '<td>'
                }
                punchexceptionrec += '</td>';

                if (punchexceptionToDate != null) {
                    // $("#punchexception_To_date").val(moment(punchexceptionToDate).format("DD-MMM-YYYY"));
                    punchexceptionrec += '<td ><input id=punchexception_to_date_' + i + ' type="text" class="myDate form-control" size=15  disabled value=' + moment(punchexceptionToDate).format("DD-MMM-YYYY") + ">";

                } else {
                    // $("#punchexception_To_date").val("");
                    punchexceptionrec += '<td>'
                }
                punchexceptionrec += '</td><td></td>';


                punchexceptionrec += "<td ><label id=punchexception_del_" + i + " class='fa fa-trash-o action-icon' data-role='txn-del' " +
                        "data-control='button'></label></td>" +
                        "<td><label id=punchexception_edit_" + i + " class='fa fa-pencil action-icon' data-role='txn-edit' " +
                        "data-control='button'></label></td>";

                if (punchexceptionStatus != null) {
                    // $("#punchexception_To_date").val(moment(punchexceptionToDate).format("DD-MMM-YYYY"));
                    punchexceptionrec += '<td> <input id=punchexception_statusflag_' + i + ' type="hidden" value="' + punchexceptionStatus + '">';
                } else {
                    // $("#punchexception_To_date").val("");
                    punchexceptionrec += '<td>'
                }
                punchexceptionrec += '</td>';
                punchexceptionrec += '</tr>';
                $('#punchexception tbody').append(punchexceptionrec);
            }

            $('#punchexception tbody').append("<tr><div class='col-2'><td></td></div>" +
                                        "<div class='col-3'>   <td> </td>  </div>" +
                                        "<div class='col-3'>   <td> </td>  </div>" +
                                          "<div class='col-3'>   <td> </td>  </div>" +
                                        "<div class='col-1'><td><label id=punchexception_add_" + i + " class='fa fa-plus' data-role='txn-add' " + "data-control='button'></label></td></div></tr>");
            //end of rendering punchexception data

            // begin rendering of workhourperday data

            $("#workhourperday").find("tr:gt(0)").remove();
            // $('#workhourperday tbody').empty();
            var workhourperdayrec = '';


            for (i = 0; i < data.workhourperday.length; i++) {
                id = data.workhourperday[i].id;
                workhourperdayFromDate = new Date(data.workhourperday[i].fromdate);

                workhourperdayToDate = new Date(data.workhourperday[i].todate);
                workhourperdayhours = data.workhourperday[i].transactiondata;
                workhourperdayStatus = data.workhourperday[i].StatusFlag;
                workhourperdayrec = '';

                workhourperdayrec += '<tr id="workhourperday_' + i + '">';
                workhourperdayrec += '<td><input id=workhourperday_id_' + i + ' type="hidden" value="' + id + '"</td>';

                if (workhourperdayFromDate != null) {

                    workhourperdayrec += '<td ><input id=workhourperday_from_date_' + i + ' type="text" class="myDate form-control" size=15  disabled value=' + moment(workhourperdayFromDate).format("DD-MMM-YYYY") + ">";

                } else {

                    workhourperdayrec += '<td>'
                }
                workhourperdayrec += '</td>';

                if (workhourperdayToDate != null) {
                    // $("#workhourperday_To_date").val(moment(workhourperdayToDate).format("DD-MMM-YYYY"));
                    workhourperdayrec += '<td ><input id=workhourperday_to_date_' + i + ' type="text" class="myDate form-control" size=15  disabled value=' + moment(workhourperdayToDate).format("DD-MMM-YYYY") + ">";

                } else {
                    // $("#workhourperday_To_date").val("");
                    workhourperdayrec += '<td>'
                }
                workhourperdayrec += '</td>';

                if (workhourperdayhours != null) {
                    // $("#workhourperday_To_date").val(moment(workhourperdayToDate).format("DD-MMM-YYYY"));
                    workhourperdayrec += '<td> <input id=workhourperday_hours_' + i + ' type="text" class="text-blue" disabled value=' + workhourperdayhours + ">";
                } else {
                    // $("#workhourperday_To_date").val("");
                    workhourperdayrec += '<td>'
                }

                workhourperdayrec += "<td ><label id=workhourperday_del_" + i + " class='fa fa-trash-o action-icon' data-role='txn-del' " +
                        "data-control='button'></label></td>" +
                        "<td><label id=workhourperday_edit_" + i + " class='fa fa-pencil action-icon' data-role='txn-edit' " +
                        "data-control='button'></label></td>";

                if (workhourperdayStatus != null) {
                    // $("#workhourperday_To_date").val(moment(workhourperdayToDate).format("DD-MMM-YYYY"));
                    workhourperdayrec += '<td> <input id=workhourperday_statusflag_' + i + ' type="hidden" value="' + workhourperdayStatus + '">';
                } else {
                    // $("#workhourperday_To_date").val("");
                    workhourperdayrec += '<td>'
                }
                workhourperdayrec += '</td>';

                workhourperdayrec += '</tr>';
                $('#workhourperday tbody').append(workhourperdayrec);
            }

            $('#workhourperday tbody').append("<tr><div class='col-2'><td></td></div>" +
                                        "<div class='col-3'>   <td> </td>  </div>" +
                                        "<div class='col-3'>   <td> </td>  </div>" +
                                        "<div clawss='col-3'>   <td> </td>  </div>" +
                                        "<div class='col-1'><td><label id=workhourperday_add_" + i + " class='fa fa-plus' data-role='txn-add' " + "data-control='button'></label></td></div></tr>");
            //end of rendering workhourperday data

            // begin rendering of workhourperweek data
            $("#workhourperweek").find("tr:gt(0)").remove();
            //$('#workhourperweek tbody').empty();
            var workhourperweekrec = '';

            for (i = 0; i < data.workhourperweek.length; i++) {
                id = data.workhourperweek[i].id;
                workhourperweekFromDate = new Date(data.workhourperweek[i].fromdate);

                workhourperweekToDate = new Date(data.workhourperweek[i].todate);
                workhourperweekhours = data.workhourperweek[i].transactiondata;
                workhourperweekStatus = data.workhourperweek[i].StatusFlag;
                workhourperweekrec = '';

                workhourperweekrec += '<tr id="workhourperweek_' + i + '">';
                workhourperweekrec += '<td><input id=workhourperweek_id_' + i + ' type="hidden" value="' + id + '"</td>';

                if (workhourperweekFromDate != null) {

                    workhourperweekrec += '<td ><input id=workhourperweek_from_date_' + i + ' type="text" class="myDate form-control" size=15  disabled value=' + moment(workhourperweekFromDate).format("DD-MMM-YYYY") + ">";

                } else {

                    workhourperweekrec += '<td>'
                }
                workhourperweekrec += '</td>';

                if (workhourperweekToDate != null) {
                    // $("#workhourperweek_To_date").val(moment(workhourperweekToDate).format("DD-MMM-YYYY"));
                    workhourperweekrec += '<td ><input id=workhourperweek_to_date_' + i + ' type="text" class="myDate form-control" size=15  disabled value=' + moment(workhourperweekToDate).format("DD-MMM-YYYY") + ">";


                } else {
                    // $("#workhourperweek_To_date").val("");
                    workhourperweekrec += '<td>'
                }
                workhourperweekrec += '</td>';

                if (workhourperweekhours != null) {
                    // $("#workhourperweek_To_date").val(moment(workhourperweekToDate).format("DD-MMM-YYYY"));
                    workhourperweekrec += '<td> <input id=workhourperweek_hours_' + i + ' type="text" class="text-blue" disabled value=' + workhourperweekhours + ">";
                } else {
                    // $("#workhourperweek_To_date").val("");
                    workhourperweekrec += '<td>'
                }

                workhourperweekrec += "<td ><label id=workhourperweek_del_" + i + " class='fa fa-trash-o action-icon' data-role='txn-del' " +
                        "data-control='button'></label></td>" +
                        "<td><label id=workhourperweek_edit_" + i + " class='fa fa-pencil action-icon' data-role='txn-edit' " +
                        "data-control='button'></label></td>";

                if (workhourperweekStatus != null) {
                    // $("#workhourperweek_To_date").val(moment(workhourperweekToDate).format("DD-MMM-YYYY"));
                    workhourperweekrec += '<td> <input id=workhourperweek_statusflag_' + i + ' type="hidden" value="' + workhourperweekStatus + '">';
                } else {
                    // $("#workhourperweek_To_date").val("");
                    workhourperweekrec += '<td>'
                }
                workhourperweekrec += '</td>';

                workhourperweekrec += '</tr>';
                $('#workhourperweek tbody').append(workhourperweekrec);
            }

            $('#workhourperweek tbody').append("<tr><div class='col-2'><td></td></div>" +
                                        "<div class='col-3'>   <td> </td>  </div>" +
                                        "<div class='col-3'>   <td> </td>  </div>" +
                                        "<div clawss='col-3'>   <td> </td>  </div>" +
                                        "<div class='col-1'><td><label id=workhourperweek_add_" + i + " class='fa fa-plus' data-role='txn-add' " + "data-control='button'></label></td></div></tr>");
            //end of rendering workhourperweek data

            // begin rendering of workhourpermonth data
            $("#workhourpermonth").find("tr:gt(0)").remove();
            //$('#workhourpermonth tbody').empty();
            var workhourpermonthrec = '';

            for (i = 0; i < data.workhourpermonth.length; i++) {
                id = data.workhourpermonth[i].id;
                workhourpermonthFromDate = new Date(data.workhourpermonth[i].fromdate);

                workhourpermonthToDate = new Date(data.workhourpermonth[i].todate);
                workhourpermonthhours = data.workhourpermonth[i].transactiondata;
                workhourpermonthStatus = data.workhourpermonth[i].StatusFlag;
                workhourpermonthrec = '';

                workhourpermonthrec += '<tr id="workhourpermonth_' + i + '">';
                workhourpermonthrec += '<td><input id=workhourpermonth_id_' + i + ' type="hidden" value="' + id + '"</td>';

                if (workhourpermonthFromDate != null) {

                    workhourpermonthrec += '<td ><input id=workhourpermonth_from_date_' + i + ' type="text" class="myDate form-control" size=15  disabled value=' + moment(workhourpermonthFromDate).format("DD-MMM-YYYY") + ">";

                } else {

                    workhourpermonthrec += '<td>'
                }
                workhourpermonthrec += '</td>';

                if (workhourpermonthToDate != null) {
                    // $("#workhourpermonth_To_date").val(moment(workhourpermonthToDate).format("DD-MMM-YYYY"));
                    workhourpermonthrec += '<td ><input id=workhourpermonth_to_date_' + i + ' type="text" class="myDate form-control" size=15  disabled value=' + moment(workhourpermonthToDate).format("DD-MMM-YYYY") + ">";

                } else {
                    // $("#workhourpermonth_To_date").val("");
                    workhourpermonthrec += '<td>'
                }
                workhourpermonthrec += '</td>';

                if (workhourpermonthhours != null) {
                    // $("#workhourpermonth_To_date").val(moment(workhourpermonthToDate).format("DD-MMM-YYYY"));
                    workhourpermonthrec += '<td> <input id=workhourpermonth_hours_' + i + ' type="text" class="text-blue" disabled value=' + workhourpermonthhours + ">";
                } else {
                    // $("#workhourpermonth_To_date").val("");
                    workhourpermonthrec += '<td>'
                }

                workhourpermonthrec += "<td ><label id=workhourpermonth_del_" + i + " class='fa fa-trash-o action-icon' data-role='txn-del' " +
                        "data-control='button'></label></td>" +
                        "<td><label id=workhourpermonth_edit_" + i + " class='fa fa-pencil action-icon' data-role='txn-edit' " +
                        "data-control='button'></label></td>";

                if (workhourpermonthStatus != null) {
                    // $("#workhourpermonth_To_date").val(moment(workhourpermonthToDate).format("DD-MMM-YYYY"));
                    workhourpermonthrec += '<td> <input id=workhourpermonth_statusflag_' + i + ' type="hidden" value="' + workhourpermonthStatus + '">';
                } else {
                    // $("#workhourpermonth_To_date").val("");
                    workhourpermonthrec += '<td>'
                }
                workhourpermonthrec += '</td>';

                workhourpermonthrec += '</tr>';
                $('#workhourpermonth tbody').append(workhourpermonthrec);
            }

            $('#workhourpermonth tbody').append("<tr><div class='col-2'><td></td></div>" +
                                        "<div class='col-3'>   <td> </td>  </div>" +
                                        "<div class='col-3'>   <td> </td>  </div>" +
                                        "<div clawss='col-3'>   <td> </td>  </div>" +
                                        "<div class='col-1'><td><label id=workhourpermonth_add_" + i + " class='fa fa-plus' data-role='txn-add' " + "data-control='button'></label></td></div></tr>");
            //end of rendering workhourpermonth data

            // begin rendering of maternity data
            $("#Maternity").find("tr:gt(0)").remove();
            // $('#Maternity tbody').empty();
            var id = $('#gender').val();
            if (id != 'male') {
                var maternityrec = '';

                for (i = 0; i < data.maternity.length; i++) {
                    id = data.maternity[i].id;
                    maternityFromDate = new Date(data.maternity[i].fromdate);

                    //    maternityToDate = new Date(data.maternity[i].todate);

                    maternityStatus = data.maternity[i].StatusFlag;
                    maternityrec = '';
                    maternityrec += '<tr id="maternity_' + i + '">';
                    maternityrec += '<td><input id=maternity_id_' + i + ' type="hidden" value="' + id + '"</td>';

                    if (maternityFromDate != null) {

                        maternityrec += '<td ><input id=maternity_from_date_' + i + ' type="text" class="myDate form-control" size=15  disabled value=' + moment(maternityFromDate).format("DD-MMM-YYYY") + ">";

                    } else {

                        maternityrec += '<td>'
                    }
                    maternityrec += '</td><td></td><td></td>';

                    maternityrec += "<td ><label id=maternity_del_" + i + " class='fa fa-trash-o action-icon' data-role='txn-del' " +
                        "data-control='button'></label></td>" +
                        "<td><label id=maternity_edit_" + i + " class='fa fa-pencil action-icon' data-role='txn-edit' " +
                        "data-control='button'></label></td>";

                    if (maternityStatus != null) {
                        // $("#maternity_To_date").val(moment(maternityToDate).format("DD-MMM-YYYY"));
                        maternityrec += '<td> <input id=maternity_statusflag_' + i + ' type="hidden" value="' + maternityStatus + '">';
                    } else {
                        // $("#maternity_To_date").val("");
                        maternityrec += '<td>'
                    }
                    maternityrec += '</td>';
                    maternityrec += '</tr>';
                    $('#Maternity tbody').append(maternityrec);
                }

                // $('#Maternity tbody').append("<tr><div class='col-2'><td></td></div>" +
                //                         "<div class='col-3'>   <td> </td>  </div>" +
                //                         "<div class='col-3'>   <td> </td>  </div>" +
                //                          "<div class='col-3'>   <td> </td>  </div>" +
                //                         "<div class='col-1'><td><label id=maternity_add_" + i + " class='fa fa-plus' data-role='txn-add' " + "data-control='button'></label></td></div></tr>");

                $('#Maternity tbody').append("<tr><td></td></div>" +
                                        "<td> </td>" +
                                        "<td> </td>" +
                                         "<td> </td>" +
                                        "<td><label id=maternity_add_" + i + " class='fa fa-plus' data-role='txn-add' " + "data-control='button'></label></td></tr>");

            }
            
           else
            {

                $('#Maternity').hide();
            }
            //end of rendering maternity data

            // begin rendering of termination data
            $("#termination").find("tr:gt(0)").remove();
            //$('#termination tbody').empty();
            var terminationrec = '';


            for (i = 0; i < data.termination.length; i++) {
                id = data.termination[i].id;
                terminationFromDate = new Date(data.termination[i].fromdate);

                //  terminationToDate = new Date(data.termination[i].todate);

                terminationStatus = data.termination[i].StatusFlag;
                terminationrec = '';
                terminationrec += '<tr id="termination_' + i + '">';
                terminationrec += '<td><input id=termination_id_' + i + ' type="hidden" value="' + id + '"</td>';

                if (terminationFromDate != null) {

                    terminationrec += '<td ><input id=termination_from_date_' + i + ' type="text" class="myDate form-control" size=15  disabled value=' + moment(terminationFromDate).format("DD-MMM-YYYY") + ">";

                } else {

                    terminationrec += '<td>'
                }
                terminationrec += '</td><td></td><td></td>';

                terminationrec += "<td ><label id=termination_del_" + i + " class='fa fa-trash-o action-icon' data-role='txn-del' " +
                        "data-control='button'></label></td>" +
                        "<td><label id=termination_edit_" + i + " class='fa fa-pencil action-icon' data-role='txn-edit' " +
                        "data-control='button'></label></td>";

                if (terminationStatus != null) {
                    // $("#termination_To_date").val(moment(terminationToDate).format("DD-MMM-YYYY"));
                    terminationrec += '<td> <input id=termination_statusflag_' + i + ' type="hidden" value="' + terminationStatus + '">';
                } else {
                    // $("#termination_To_date").val("");
                    terminationrec += '<td>'
                }
                terminationrec += '</td>';
                terminationrec += '</tr>';
                $('#termination tbody').append(terminationrec);
            }

            $('#termination tbody').append("<tr><div class='col-2'><td></td></div>" +
                                        "<div class='col-3'>   <td> </td>  </div>" +
                                        "<div class='col-3'>   <td> </td>  </div>" +
                                          "<div class='col-3'>   <td> </td>  </div>" +
                                        "<div class='col-1'><td><label id=termination_add_" + i + " class='fa fa-plus' data-role='txn-add' " + "data-control='button'></label></td></div></tr>");
            //end of rendering termination data

            // begin rendering of manager data
            $("#manager").find("tr:gt(0)").remove();
            //$('#manager tbody').empty();
            var managerrec = '';

            for (i = 0; i < data.manager.length; i++) {
                id = data.manager[i].id;
                managerFromDate = new Date(data.manager[i].fromdate);

                managerToDate = new Date(data.manager[i].todate);
                managerempid = data.manager[i].transactiondata;
                managerStatus = data.manager[i].StatusFlag;
                managerrec = '';
                managerrec += '<tr id="manager_' + i + '">';
                managerrec += '<td><input id=manager_recid_' + i + ' type="hidden" value="' + id + '"</td>';
                if (managerFromDate != null) {

                    managerrec += '<td ><input id=manager_from_date_' + i + ' type="text" class="myDate form-control" size=15  disabled value=' + moment(managerFromDate).format("DD-MMM-YYYY") + ">";

                } else {

                    managerrec += '<td>'
                }
                managerrec += '</td>';

                if (managerToDate != null) {
                    // $("#manager_To_date").val(moment(managerToDate).format("DD-MMM-YYYY"));
                    managerrec += '<td ><input id=manager_to_date_' + i + ' type="text" class="myDate form-control" size=15  disabled value=' + moment(managerToDate).format("DD-MMM-YYYY") + ">";

                } else {
                    // $("#manager_To_date").val("");
                    managerrec += '<td>'
                }
                managerrec += '</td>';

                if (managerempid != null) {
                    // $("#manager_To_date").val(moment(managerToDate).format("DD-MMM-YYYY"));
                    managerrec += '<td> <input id=manager_emp_id_' + i + ' type="text" class="text-blue" disabled value=' + managerempid + ">";

                } else {
                    // $("#manager_To_date").val("");
                    managerrec += '<td>'
                }

                managerrec += "<td ><label id=manager_del_" + i + " class='fa fa-trash-o action-icon' data-role='txn-del' " +
                        "data-control='button'></label></td>" +
                        "<td><label id=manager_edit_" + i + " class='fa fa-pencil action-icon' data-role='txn-edit' " +
                        "data-control='button'></label></td>";

                if (managerStatus != null) {
                    // $("#manager_To_date").val(moment(managerToDate).format("DD-MMM-YYYY"));
                    managerrec += '<td> <input id=manager_statusflag_' + i + ' type="hidden" value="' + managerStatus + '">';
                } else {
                    // $("#manager_To_date").val("");
                    managerrec += '<td>'

                }
                managerrec += '</td>';

                managerrec += '</tr>';
                $('#manager tbody').append(managerrec);
            }

            $('#manager tbody').append("<tr><div class='col-2'><td></td></div>" +
                                        "<div class='col-3'>   <td> </td>  </div>" +
                                        "<div class='col-3'>   <td> </td>  </div>" +
                                        "<div clawss='col-3'>   <td> </td>  </div>" +
                                        "<div class='col-1'><td><label id=manager_add_" + i + " class='fa fa-plus' data-role='txn-add' " + "data-control='button'></label></td></div></tr>");
            //end of rendering manager data


            // select_HTML = '<option value="select">Select Shift</option>';

            SAXLoader.closeBlockingLoader();


        }

        // employee data transaction code end

        /* other */


        function _initOther() {

            var $maternity_break_hours_checked = $('#maternity');

            $(".mydate").datepicker({
                changeMonth: true, //this option for allowing user to select month
                changeYear: true
            });

            $(".datepicker").datepicker({
                dateFormat: "dd-M-yy"
            });


            // Get branch, department, shift, designation, category data based on the company selected.
            $company_code.change(function() {

                OtherData.get($(this).val());
                // $("#manager_id").val('');
            });
            // reset the date of leaving if status is changed to Active.
            $employee_status.change(function() {
                console.log("Hola");
                if ($(this).val() == "1") {
                    $date_of_leaving.val("");
                }


            });

            $gender.change(function() {
                // setEnrollIdValue();
                console.log($(this).val());
                if ($(this).val() == "male") {

                }
                else {

                    //  $maternity_break_hours_checked.show();
                }
            });
            $("#branch_code").change(function() {
                OtherData.getmanagerlist($(this).val());

            });
        }


        function _initButtons() {

            //   alert('called init buttons');

            var role = '',
            button_actions =
            {
                'employee-save': function(event) {
                    //     alert('abt to save')
                    saveEmployeeData(txnData);
                    //  _showEmpDataTransactionsTab();
                    event.preventDefault();
                    if (!hasValidationErrors) {
                        _showEmpDetailsTab();
                    }

                    // getEmployeeTransactionData($empcode, $company_code.val(), $branch_code);

                    //  SAXLoader.showBlockingLoader();
                    //  return false;
                },
                'employee/save': function(event) {
                    saveEmployee(event);
                    loadFlag = '1';
                },
                'txn-del': function(event) {
                    deleteTransaction(event);
                },
                'txn-edit': function(event) {
                    editTransaction(event);
                },
                'txn-add': function(event) {
                    //  alert('calling add transaction twice ***');
                    addTransaction(event);
                },
                "filters/data": function(event) {

                    getOtherData(emp_code.val());
                    //  getEmployeeTransactionData(emp_code.val(), company_code.val(), branch_code.val());
                }
            };
            $('body').on('click', '[data-control="button"]', function(event) {

                //     alert('came here');

                role = $(event.target).data('role');

                //     alert('came here for '+role);

                button_actions[role].call(this, event);
                event.preventDefault();
                //  _showEmpDataTransactionsTab();
            });

            $('body').on('click', '[data-control="toggle"]', function(event) {

                role = $(event.target).data('target');
                loadFlag = '1';
                //   alert($empcode +','+ $company_code.val()+',' + $branch_code)
                //     alert(role);

                getEmployeeTransactionData($empcode, $company_code.val(), $branch_code);
                // alert('111111');
                //   getEmployeeTransactionData("10", "RGH-10", "10-07");
                //   event.preventDefault();
                //_showEmpDataTransactionsTab();
            });

        }



        function initialize(mode) {

            var 
                hash = document.location.hash,
                hash_parts = hash.split("/");

            // alert(hash_parts[1]);

            _initButtons();
            _initOther();

            hasValidationErrors = false;

            //  getEmployeeDetails(hash_parts[2]);
            //  _showEmpDetailsTab();


            if ($("#empDetailsTabOption").hasClass("active")) {
                _showEmpDetailsTab();

            } else {
                _showEmpDataTransactionsTab();
            }

            page.mode = mode;
            var $maternity_break_hours_checked = $('#maternity');
        }


        main = function(mode) {

            initialize(mode);
        };

        return {
            'init': initialize,
            'get': getEmployeeDetails
        };

    })(jQuery, window, document);


    //$(function () {
    // alert($("#empDetailsTabOption").hasClass("active"));
    //  alert($("#tabname").val());
    // EmployeeView.main();


    // INITIAL PAGE LOAD
    SAXLoader.show();
    // init page components
    var 
        hash = document.location.hash,
        hash_parts = hash.split("/");
    Company.get()
        .done(function() {
            var mode = "add";
            if (hash_parts.length > 2) {
                EmployeeView.get(hash_parts[2]);
                mode = "edit";
            }
            if ( hash_parts[2] == '')
            {mode = "add"}
            EmployeeView.init(mode);
        })
        .always(SAXLoader.close);


});

