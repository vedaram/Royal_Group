var Authentication = (function($, w, d) {

    var main, _ajax,
		_checkLicenseValidity, _processLicenseValidity,
		_showLicensePopup,
		_getOtherData, _success, _failure,
		_processSessionData, _setupAdminSession, _setupEmployeeSession,
		_processMenu,
		_homePage;

    var $license_status = $('#licenseStatus'),
	$session_status = $('#sessionStatus'),
	$menu_status = $('#menuStatus');

    function _setupMenu(data) {

        var 
			app_url = w.location.protocol + '//' + w.location.host + '/',
			menu_HTML = '',
			employee_permissions, emp_perms_grouped;

        if (w.location.pathname.split('/')[1] != "authentication") {
            app_url += w.location.pathname.split('/')[1] + '/';
        }

        menu_HTML = '<li><a href="' + app_url + 'ui/profile/home.aspx"><p class="menu-text"><span class="fa fa-home menu-icon"></span> HOME</p></a></li>';

        data = JSON.parse(data);

        if (data.length > 0) {
            employee_permissions = EmployeePermissions.process(data[0]["PERMISSIONS"]);
            emp_perms_grouped = EmployeePermissions.group(employee_permissions);

            if (_.where(emp_perms_grouped["device_management"], { value: "1" }).length > 0) {
                menu_HTML += '<li>';
                menu_HTML += "<a href=\"javascript:void(0);\"><p class=\"menu-text\"><span class=\"fa fa-cog menu-icon\"></span> DEVICE</p></a>";
                menu_HTML += "<div class=\"menu-popout\">";

                if (_.where(emp_perms_grouped["device_management"], { sub_section: "gprs", value: "1" }).length > 0) {
                    menu_HTML += '<h4 class="menu-heading">GPRS DEVICE MANAGEMENT</h4>' +
										'<ul class="menu">';

                    if (employee_permissions["device_location"]["value"] == 1)
                        menu_HTML += '<li><a href="' + app_url + 'ui/device/gprs/information.aspx">Device Information</a></li>';

                    if (employee_permissions["template_upload"]["value"] == 1)
                        menu_HTML += '<li><a href="' + app_url + 'ui/device/gprs/upload.aspx">Template Upload</a></li>';

                    if (employee_permissions["template_upload_status"]["value"] == 1)
                        menu_HTML += '<li><a href="' + app_url + 'ui/device/gprs/upload_status.aspx">Template Upload Status</a></li>';

                    menu_HTML += '</ul>';
                }

                if (_.where(emp_perms_grouped["device_management"], { sub_section: "lan", value: "1" }).length > 0) {
                    menu_HTML += '<h4 class="menu-heading">LAN DEVICE MANAGEMENT</h4>' +
										'<ul class="menu">';

                    if (employee_permissions["lan_device_information"]["value"] == 1)
                        menu_HTML += '<li><a href="' + app_url + 'ui/device/lan/information.aspx">Device Information</a></li>';

                    if (employee_permissions["lan_template_upload"]["value"] == 1)
                        menu_HTML += '<li><a href="' + app_url + 'ui/device/lan/upload.aspx">Template Upload</a></li>';

                    if (employee_permissions["lan_template_download"]["value"] == 1)
                        menu_HTML += '<li><a href="' + app_url + 'ui/device/lan/download.aspx">Template Download</a></li>';

                    if (employee_permissions["lan_enroll_card"]["value"] == 1)
                        menu_HTML += '<li><a href="' + app_url + 'ui/device/lan/enroll.aspx">Enroll Card</a></li>';

                    menu_HTML += '</ul>';
                }

                menu_HTML += '</div>';
                menu_HTML += '</li>';
            }

            if (_.where(emp_perms_grouped["company_management"], { value: "1" }).length > 0) {
                menu_HTML += '<li>';
                menu_HTML += '<a href="javascript:void(0);"><p class="menu-text"><span class="fa fa-building-o menu-icon"></span> COMPANY</p></a>';
                menu_HTML += '<div class="menu-popout">';

                if (_.where(emp_perms_grouped["company_management"], { sub_section: "masters", value: "1" }).length > 0) {
                    menu_HTML += '<h4 class="menu-heading">MASTERS</h4><ul class="menu">';

                    if (employee_permissions["company_master"]["value"] == 1)
                        menu_HTML += '<li id="company-master-menu-item"><a href="' + app_url + 'ui/company/masters/company.aspx">Company Master</a></li>';

                    if (employee_permissions["holiday_group_master"]["value"] == 1)
                        menu_HTML += '<li id="holiday-group-master-menu-item"><a href="' + app_url + 'ui/company/masters/holiday_group_master.aspx">Holiday Group Master</a></li>';

                    if (employee_permissions["holiday_master"]["value"] == 1)
                        menu_HTML += '<li id="holiday-master-menu-item"><a href="' + app_url + 'ui/company/masters/holiday.aspx">Holiday Master</a></li>';

                    if (employee_permissions["holiday_list"]["value"] == 1)
                        menu_HTML += '<li id="holiday-list-menu-item"><a href="' + app_url + 'ui/company/masters/holiday_list.aspx">Holiday List</a></li>';

                    if (employee_permissions["branch_master"]["value"] == 1)
                        menu_HTML += '<li id="holiday-master-menu-item"><a href="' + app_url + 'ui/company/masters/branch.aspx">Branch Master</a></li>';

                    if (employee_permissions["department_master"]["value"] == 1)
                        menu_HTML += '<li id="holiday-master-menu-item"><a href="' + app_url + 'ui/company/masters/department.aspx">Department Master</a></li>';

                    if (employee_permissions["designation_master"]["value"] == 1)
                        menu_HTML += '<li id="holiday-master-menu-item"><a href="' + app_url + 'ui/company/masters/designation.aspx">Designation Master</a></li>';

                    if (employee_permissions["employee_category"]["value"] == 1)
                        menu_HTML += '<li id="holiday-master-menu-item"><a href="' + app_url + 'ui/company/masters/employee_category.aspx">Employee Category Master</a></li>';

                    if (employee_permissions["leave_master"]["value"] == 1)
                        menu_HTML += '<li id="holiday-master-menu-item"><a href="' + app_url + 'ui/company/masters/leave.aspx">Leave Master</a></li>';

                    if (employee_permissions["shift_master"]["value"] == 1)
                        menu_HTML += '<li id="holiday-master-menu-item"><a href="' + app_url + 'ui/company/masters/shift.aspx">Shift Master</a></li>';

                    if (employee_permissions["employee_master"]["value"] == 1)
                        menu_HTML += '<li id="holiday-master-menu-item"><a href="' + app_url + 'ui/company/masters/employee.aspx">Employee Master</a></li>';

                    if (employee_permissions["ot_eligibility"]["value"] == 1)
                        menu_HTML += '<li id="holiday-master-menu-item"><a href="' + app_url + 'ui/company/masters/ot_eligibility.aspx">OT Eligibility Master</a></li>';

                    if (employee_permissions["change_manager"]["value"] == 1)
                        menu_HTML += '<li id="holiday-master-menu-item"><a href="' + app_url + 'ui/company/masters/change_manager.aspx">Change Manager</a></li>';

                    if (employee_permissions["delegation_assignment"]["value"] == 1)
                        menu_HTML += '<li id="holiday-master-menu-item"><a href="' + app_url + 'ui/company/masters/delegation_assignment.aspx">Delegation Assignment</a></li>';

                    if (employee_permissions["branch_mapping"]["value"] == 1)
                        menu_HTML += '<li id="holiday-master-menu-item"><a href="' + app_url + 'ui/company/masters/branch_mapping.aspx">Branch Mapping</a></li>';

                    menu_HTML += '</ul>';
                }

                if (_.where(emp_perms_grouped["company_management"], { sub_section: "configuration", value: "1" }).length > 0) {

                    menu_HTML += '<h4 class="menu-heading">CONFIGURATION</h4><ul class="menu">';

                    if (employee_permissions["shift_setting"]["value"] == 1)
                        menu_HTML += '<li id="shift-settings-menu-item"><a href="' + app_url + 'ui/company/configuration/shift_setting.aspx">Shift Setting</a></li>';

                    if (employee_permissions["change_company_logo"]["value"] == 1)
                        menu_HTML += '<li id="change-company-logo-menu-item"><a href="' + app_url + 'ui/company/configuration/change_company_logo.aspx">Change Company Logo</a></li>';

                    if (employee_permissions["ramadan_history"]["value"] == 1)
                        menu_HTML += '<li id="ramadan-history-menu-item"><a href="' + app_url + 'ui/company/configuration/ramadan_history.aspx">Ramadan History</a></li>';
                    
                    menu_HTML += '</ul>';
                }

                menu_HTML += '</div>';
                menu_HTML += '</li>';
            }

            if (_.where(emp_perms_grouped["attendance_management"], { value: "1" }).length > 0) {
                menu_HTML += '<li>';
                menu_HTML += '<a href="javascript:void(0);"><p class="menu-text"><span class="fa fa-check menu-icon"></span> ATTENDANCE</p></a>';
                menu_HTML += '<div class="menu-popout">';

                if (_.where(emp_perms_grouped["attendance_management"], { sub_section: "attendance", value: "1" }).length > 0) {
                    menu_HTML += '<h4 class="menu-heading">ATTENDANCE</h4>' +
							'<ul class="menu" style="margin-top: 140px;">';

                    if (employee_permissions["process_data"]["value"] == 1)
                        menu_HTML += '<li><a href="' + app_url + 'ui/attendance/process_data.aspx">Process Data</a></li>';

                    if (employee_permissions["usb_download"]["value"] == 1)
                        menu_HTML += '<li><a href="' + app_url + 'ui/attendance/usb_download.aspx">USB Downlaod & Process</a></li>';

                    if (employee_permissions["unprocessed_data"]["value"] == 1)
                        menu_HTML += '<li><a href="' + app_url + 'ui/attendance/unprocessed_data.aspx">Unprocess Data</a></li>';

                    if (employee_permissions["reprocess"]["value"] == 1)
                        menu_HTML += '<li><a href="' + app_url + 'ui/attendance/reprocess.aspx">Reprocess</a></li>';

                    menu_HTML += '</ul>';
                }

                menu_HTML += '</div>';
                menu_HTML += '</li>';
            }

            if (_.where(emp_perms_grouped["transaction_management"], { value: "1" }).length > 0) {
                menu_HTML += '<li>';
                menu_HTML += "<a href=\"javascript:void(0);\"><p class=\"menu-text\"><span class=\"fa fa-exchange menu-icon\"></span> TRANSACTION</p></a>";
                menu_HTML += "<div class=\"menu-popout\">";

                if (_.where(emp_perms_grouped["transaction_management"], { sub_section: "leave", value: "1" }).length > 0) {
                    menu_HTML += '<h4 class="menu-heading">LEAVE</h4>' +
							'<ul class="menu">';

                    if (employee_permissions["leave_available"]["value"] == 1)
                        menu_HTML += '<li><a href="' + app_url + 'ui/transaction/leave/available.aspx">Leave Available</a></li>';

                    if (employee_permissions["leave_application"]["value"] == 1)
                        menu_HTML += '<li><a href="' + app_url + 'ui/transaction/leave/apply.aspx">Leave Application</a></li>';

                    if (employee_permissions["leave_approval"]["value"] == 1)
                        menu_HTML += '<li><a href="' + app_url + 'ui/transaction/leave/approve.aspx">Leave Approval</a></li>';

                    if (employee_permissions["leave_details"]["value"] == 1)
                        menu_HTML += '<li><a href="' + app_url + 'ui/transaction/leave/details.aspx">Leave Details</a></li>';

                    if (employee_permissions["od_leave_application"]["value"] == 1)
                        menu_HTML += '<li><a href="' + app_url + 'ui/transaction/od/apply.aspx">OD Leave Application</a></li>';

                    if (employee_permissions["od_leave_approval"]["value"] == 1)
                        menu_HTML += '<li><a href="' + app_url + 'ui/transaction/od/approve.aspx">OD Leave Approval</a></li>';

                    if (employee_permissions["od_leave_details"]["value"] == 1)
                        menu_HTML += '<li><a href="' + app_url + 'ui/transaction/od/details.aspx">OD Leave Details</a></li>';

                    if (employee_permissions["leave_assign"]["value"] == 1)
                        menu_HTML += '<li><a href="' + app_url + 'ui/transaction/leave/assign.aspx">Leave Assign</a></li>';

                    menu_HTML += '</ul>';
                }

                if (_.where(emp_perms_grouped["transaction_management"], { sub_section: "manual_punch", value: "1" }).length > 0) {
                    menu_HTML += '<h4 class="menu-heading">MANUAL PUNCH</h4>' +
							'<ul class="menu">';

                    if (employee_permissions["manual_punch_application"]["value"] == 1)
                        menu_HTML += '<li><a href="' + app_url + 'ui/transaction/manual/apply.aspx">Manual Punch Application</a></li>';

                    if (employee_permissions["manual_punch_details"]["value"] == 1)
                        menu_HTML += '<li><a href="' + app_url + 'ui/transaction/manual/details.aspx">Manual Punch Details</a></li>';

                    if (employee_permissions["manual_punch_approval"]["value"] == 1)
                        menu_HTML += '<li><a href="' + app_url + 'ui/transaction/manual/approve.aspx">Manual Punch Approve</a></li>';

                    menu_HTML += '</ul>';
                }

                if (_.where(emp_perms_grouped["transaction_management"], { sub_section: "overtime", value: "1" }).length > 0) {

                    menu_HTML += '<h4 class="menu-heading">OVERTIME</h4>' +
							'<ul class="menu">';

                    if (employee_permissions["overtime_application"]["value"] == 1)
                        menu_HTML += '<li><a href="' + app_url + 'ui/transaction/overtime/apply.aspx">Overtime Application</a></li>';

                    if (employee_permissions["overtime_details"]["value"] == 1)
                        menu_HTML += '<li><a href="' + app_url + 'ui/transaction/overtime/details.aspx">Overtime Details</a></li>';

                    if (employee_permissions["overtime_approval"]["value"] == 1)
                        menu_HTML += '<li><a href="' + app_url + 'ui/transaction/overtime/approve.aspx">Overtime Approve</a></li>';

                    menu_HTML += '</ul>';
                }

                if (_.where(emp_perms_grouped["transaction_management"], { sub_section: "compoff", value: "1" }).length > 0) {
                    menu_HTML += '<h4 class="menu-heading">COMPENSATORY OFF</h4>' +
							'<ul class="menu">';

                    if (employee_permissions["compoff_application"]["value"] == 1)
                        menu_HTML += '<li><a href="' + app_url + 'ui/transaction/compoff/apply.aspx">Comp Off Application</a></li>';

                    if (employee_permissions["compoff_approval"]["value"] == 1)
                        menu_HTML += '<li><a href="' + app_url + 'ui/transaction/compoff/approve.aspx">Comp Off Approve</a></li>';

                    if (employee_permissions["compoff_details"]["value"] == 1)
                        menu_HTML += '<li><a href="' + app_url + 'ui/transaction/compoff/details.aspx">Comp Off Details</a></li>';

                    if (employee_permissions["compoff_validity"]["value"] == 1)
                        menu_HTML += '<li><a href="' + app_url + 'ui/transaction/compoff/validity.aspx">Comp Off Validity</a></li>';

                    menu_HTML += '</ul>';
                }

                if (_.where(emp_perms_grouped["transaction_management"], { sub_section: "shift", value: "1" }).length > 0) {
                    menu_HTML += '<h4 class="menu-heading">SHIFT ROSTER</h4>' +
							'<ul class="menu">';

                    if (employee_permissions["shift_roster"]["value"] == 1)
                        menu_HTML += '<li><a href="' + app_url + 'ui/transaction/shift/roster.aspx">Shift Roster</a></li>';

                    if (employee_permissions["shift_roster_import"]["value"] == 1)
                        menu_HTML += '<li><a href="' + app_url + 'ui/transaction/shift/import.aspx">Shift Roster Import</a></li>';

                    menu_HTML += '</ul>';
                }


//                if (_.where(emp_perms_grouped["transaction_management"], { sub_section: "employeetransaction", value: "1" }).length > 0) {
//                    menu_HTML += '<h4 class="menu-heading">EMPLOYEE TRANSACTION</h4><ul class="menu">';
//                     							

//                    if (employee_permissions["employee_transaction_data"]["value"] == 1)
//                        menu_HTML += '<li><a href="' + app_url + 'ui/transaction/employee/employee_data_transaction.aspx">EMPLOYEE TRANSACTION</a></li>';


//                    menu_HTML += '</ul>';
//                }
                if (_.where(emp_perms_grouped["transaction_management"], { sub_section: "outofoffice", value: "1" }).length > 0)
                {
                    menu_HTML += '<h4 class="menu-heading">OUT OF OFFICE</h4>' +
                      '<ul class="menu">';

                    if (employee_permissions["outofoffice_apply"]["value"] == 1)
                        menu_HTML += '<li><a href="' + app_url + 'ui/transaction/outofoffice/apply.aspx">OOO Application</a></li>';

                    if (employee_permissions["outofoffice_approve"]["value"] == 1)
                        menu_HTML += '<li><a href="' + app_url + 'ui/transaction/outofoffice/approve.aspx">OOO Approval</a></li>';

                    if (employee_permissions["outofoffice_details"]["value"] == 1)
                        menu_HTML += '<li><a href="' + app_url + 'ui/transaction/outofoffice/details.aspx">OOO Details</a></li>';

                    menu_HTML += '</ul>';
                }

                menu_HTML += '</div>';
                menu_HTML += '</li>';
            }

            if (_.where(emp_perms_grouped["user_management"], { value: "1" }).length > 0) {
                menu_HTML += '<li>';
                menu_HTML += "<a href=\"javascript:void(0);\"><p class=\"menu-text\"><span class=\"fa fa-user menu-icon\"></span> USER</p></a>";
                menu_HTML += "<div class=\"menu-popout\">";

                if (_.where(emp_perms_grouped["user_management"], { sub_section: "user", value: "1" }).length > 0) {
                    menu_HTML += '<ul class="menu" style="margin-top: 260px;">';

                    if (employee_permissions["accounts"]["value"] == 1)
                        menu_HTML += '<li><a href="' + app_url + 'ui/user/account.aspx">Account</a></li>';

                    if (employee_permissions["access_permissions"]["value"] == 1)
                        menu_HTML += '<li><a href="' + app_url + 'ui/user/permissions.aspx">Access Permissions</a></li>';

                    menu_HTML += '</ul>';
                }

                menu_HTML += '</div>';
                menu_HTML += '</li>';
            }

            if (_.where(emp_perms_grouped["reports"], { value: "1" }).length > 0) {
                menu_HTML += '<li>';
                menu_HTML += "<a href=\"javascript:void(0);\"><p class=\"menu-text\"><span class=\"fa fa-bar-chart menu-icon\"></span> REPORTS</p></a>";
                menu_HTML += "<div class=\"menu-popout\">";

                if (_.where(emp_perms_grouped["reports"], { sub_section: "daily", value: "1" }).length > 0) {
                    menu_HTML += '<h4 class="menu-heading">DAILY REPORTS</h4><ul class="menu">';

                    if (employee_permissions["performance_report"]["value"] == 1)
                        menu_HTML += '<li><a href="' + app_url + 'ui/reporting/daily/performance_report.aspx">Daily Performance Report</a></li>';

                    if (employee_permissions["attendance_report"]["value"] == 1)
                        menu_HTML += '<li><a href="' + app_url + 'ui/reporting/daily/attendance_report.aspx">Daily Attendance Report</a></li>';

                    if (employee_permissions["payroll_report"]["value"] == 1)
                        menu_HTML += '<li><a href="' + app_url + 'ui/reporting/daily/payroll_report.aspx">Daily Payroll Report</a></li>';

                    if (employee_permissions["punch_report"]["value"] == 1)
                        menu_HTML += '<li><a href="' + app_url + 'ui/reporting/daily/punch_report.aspx">Daily Punch Report</a></li>';
                    if (employee_permissions["late_comers_report"]["value"] == 1)
                        menu_HTML += '<li><a href="' + app_url + 'ui/reporting/daily/late_comers_report.aspx">Daily Late Comers Report</a></li>';
                    if (employee_permissions["missing_swipe_report"]["value"] == 1)
                        menu_HTML += '<li><a href="' + app_url + 'ui/reporting/daily/missing_swipe_report.aspx">Daily Missing Swipe Report</a></li>';
                    if (employee_permissions["performance_in_out_report"]["value"] == 1)
                        menu_HTML += '<li><a href="' + app_url + 'ui/reporting/daily/performance_in_out_report.aspx">Daily Performance In Out Report</a></li>';



                    menu_HTML += '</ul>';
                }

                if (_.where(emp_perms_grouped["reports"], { sub_section: "monthly", value: "1" }).length > 0) {
                    menu_HTML += '<h4 class="menu-heading">MONTHLY REPORTS</h4><ul class="menu">';

                    if (employee_permissions["detailed_report"]["value"] == 1)
                        menu_HTML += '<li><a href="' + app_url + 'ui/reporting/monthly/detailed_report.aspx">Detailed Monthly Report</a></li>';

                    if (employee_permissions["overtime_report"]["value"] == 1)
                        menu_HTML += '<li><a href="' + app_url + 'ui/reporting/monthly/overtime_report.aspx">Monthly Overtime Report</a></li>';

                    if (employee_permissions["muster_roll_report"]["value"] == 1)
                        menu_HTML += '<li><a href="' + app_url + 'ui/reporting/monthly/muster_roll_report.aspx">Muster Roll Report</a></li>';

                    if (employee_permissions["payroll_link_report"]["value"] == 1)
                        menu_HTML += '<li><a href="' + app_url + 'ui/reporting/monthly/payroll_link_report.aspx">Payroll Link Report</a></li>';

//                    if (employee_permissions["leave_report"]["value"] == 1)
//                        menu_HTML += '<li><a href="' + app_url + 'ui/reporting/monthly/leave_report.aspx">Leave Report</a></li>';

                    menu_HTML += '</ul>';
                }

                if (_.where(emp_perms_grouped["reports"], { sub_section: "employee", value: "1" }).length > 0) {
                    menu_HTML += '<h4 class="menu-heading">EMPLOYEE REPORTS</h4><ul class="menu">';

                    if (employee_permissions["leave_card"]["value"] == 1)
                        menu_HTML += '<li><a href="' + app_url + 'ui/reporting/employee/leave_card.aspx">Leave Card</a></li>';

                    if (employee_permissions["leave_register"]["value"] == 1)
                        menu_HTML += '<li><a href="' + app_url + 'ui/reporting/employee/leave_register.aspx">Leave Register</a></li>';


                    menu_HTML += '</ul>';
                }

                menu_HTML += '</div>';
                menu_HTML += '</li>';
            }
        }

        sessvars.TAMS_ENV.menu = menu_HTML;
    }

    _processMenu = function(data) {

        var status = data.status;

        if (status === 'success') {

            _setupMenu(data.return_data);
            $menu_status.addClass('text-green');
        }
        else {
            $menu_status.addClass('text-red');
        }

        $menu_status.html(status.toUpperCase());
    };

    _setupAdminSession = function(data) {

        sessvars.TAMS_ENV.user_details = {
            'user_name': data['username'],
            'employee_id': data['employee_id'],
            'user_access_level': data['access_level'],
            'display_picture': data['emp_photo']
        };
    };

    _setupEmployeeSession = function(data) {

        sessvars.TAMS_ENV.user_details = {
            'user_name': data['username'],
            'employee_id': data['empid'],
            'user_access_level': data['access'],
            'employee_name': data['emp_name'],
            'employee_email': data['emp_email'],
            'department_id': data['emp_department'],
            'department_name': data['deptname'],
            'designation_id': data['emp_designation'],
            'designation_name': data['designame'],
            'display_picture': data['emp_photo'],
            'employee_category': data['emp_employee_category']
        };
    };

    _processSessionData = function(data) {

        var status = data.status,
			employee_details = {};

        sessvars.TAMS_ENV = {};

        if (status === 'success') {

            employee_details = JSON.parse(data.return_data);
            if (employee_details['employee_id'] === 'admin') {
                _setupAdminSession(employee_details);
            }
            else {
                _setupEmployeeSession(employee_details[0]);
            }

            $session_status.addClass('text-green');
        }
        else {
            $session_status.addClass('text-red');
        }

        $session_status.text(status.toUpperCase());
    };

    _homePage = function() {

        sessvars.current_screen = {
            'current_menu_item': '',
            'current_section': '#home'
        };

        window.location.href = "../ui/profile/home.aspx";
    };

    _failure = function() {
        alert('An error occurred while loading user data. Please refresh the page and try again. If the error persists, please contact Support.');
    };

    _success = function(data1, data2) {

        _processSessionData(data1[0].d);
        if (sessvars.TAMS_ENV.user_details.user_name != "admin") {
            _processMenu(data2[0].d);
        }

        _homePage();
    };

    _getOtherData = function() {

        var url = [
            'Authentication.aspx/loadSessionData',
            'Authentication.aspx/loadMenuStructure'
        ], data = {};

        var promise1 = $.ajax({ url: url[0], type: "POST", contentType: 'application/json; charset=utf-8', dataType: 'json', data: JSON.stringify({}) }),
        	promise2 = $.ajax({ url: url[1], type: "POST", contentType: 'application/json; charset=utf-8', dataType: 'json', data: JSON.stringify({}) });

        $.when(promise1, promise2).then(_success, _failure);
    };

    _showLicensePopup = function() {

    };

    _processLicenseValidity = function(data, additional) {

        var status = data.status;

        if (status === 'success') {
            _getOtherData();
            $license_status.addClass('text-green');
        }
        else {
            _showLicensePopup();
            $license_status.addClass('text-red')
        }

        $license_status.text(status.toUpperCase());
    };

    _checkLicenseValidity = function() {

        var ajax_options = {};

        var ajax_options = {
            'url': 'Authentication.aspx/checkLicenseValidity',
            'data': {},
            'callback': _processLicenseValidity,
            'additional': {}
        };

        _ajax(ajax_options);
    };

    _ajax = function(options) {

        $.ajax({
            method: 'POST',
            url: options.url,
            contentType: 'application/json; charset=utf-8',
            data: JSON.stringify(options.data),
            dataType: 'json',
            success: function(data) {

                if (options.callback) {
                    options.callback(data.d, options.additional);
                }
            },
            error: function(data) {
                alert('An error occurred while performing this operation. Please try again. If the error persists, please contact Support.');
            }
        });

    };

    main = function() {
        //_checkLicenseValidity();
        _getOtherData();
    };

    return {
        'main': main
    };

})(jQuery, window, document);

$(function() {
    Authentication.main();
});
