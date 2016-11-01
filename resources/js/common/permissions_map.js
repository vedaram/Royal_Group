var EmployeePermissions = (function($, w, d) {

    // Add new sections to this array
    var sections = {
        "device_management": {
            key: "device_management",
            display: "DEVICE MANAGEMENT"
        },
        "company_management": {
            key: "company_management",
            display: "COMPANY MANAGEMENT"
        },
        "attendance_management": {
            key: "attendance_management",
            display: "ATTENDANCE MANAGEMENT"
        },
        "transaction_management": {
            key: "transaction_management",
            display: "TRANSACTION MANAGEMENT"
        },
        "user_management": {
            key: "user_management",
            display: "USER MANAGEMENT"
        },
        "reports": {
            key: "reports",
            display: "REPORTING"
        }
    };

    // NOTE: VERY IMPORTANT
    // new pages to be added at the end of the map. Use the format given in the map for each page.
    var perms_map = {
        "device_location": {
            section: "device_management",
            sub_section: "gprs",
            display: "GPRS Device Information",
            key: "device_location",
            value: 0
        },
        "template_upload": {
            section: "device_management",
            sub_section: "gprs",
            display: "GPRS Template Upload",
            key: "template_upload",
            value: 0
        },
        "template_upload_status": {
            section: "device_management",
            sub_section: "gprs",
            display: "GPRS Template Upload Status",
            key: "template_upload_status",
            value: 0
        },
        "company_master": {
            section: "company_management",
            sub_section: "masters",
            display: "Company Master",
            key: "company_master",
            value: 0
        },
        "holiday_group_master": {
            section: "company_management",
            sub_section: "masters",
            display: "Holiday Group Master",
            key: "holiday_group_master",
            value: 0
        },
        "holiday_master": {
            section: "company_management",
            sub_section: "masters",
            display: "Holiday Master",
            key: "holiday_master",
            value: 0
        },
        "holiday_list": {
            section: "company_management",
            sub_section: "masters",
            display: "Holiday List",
            key: "holiday_list",
            value: 0
        },
        "branch_master": {
            section: "company_management",
            sub_section: "masters",
            display: "Branch Master",
            key: "branch_master",
            value: 0
        },
        "department_master": {
            section: "company_management",
            sub_section: "masters",
            display: "Department Master",
            key: "department_master",
            value: 0
        },
        "designation_master": {
            section: "company_management",
            sub_section: "masters",
            display: "Designation Master",
            key: "designation_master",
            value: 0
        },
        "employee_category": {
            section: "company_management",
            sub_section: "masters",
            display: "Employee Category",
            key: "employee_category",
            value: 0
        },
        "leave_master": {
            section: "company_management",
            sub_section: "masters",
            display: "Leave Master",
            key: "leave_master",
            value: 0
        },
        "shift_master": {
            section: "company_management",
            sub_section: "masters",
            display: "Shift Master",
            key: "shift_master",
            value: 0
        },
        "employee_master": {
            section: "company_management",
            sub_section: "masters",
            display: "Employee Master",
            key: "employee_master",
            value: 0
        },
        "ot_eligibility": {
            section: "company_management",
            sub_section: "masters",
            display: "OT Eligibility",
            key: "ot_eligibility",
            value: 0
        },
        "change_manager": {
            section: "company_management",
            sub_section: "masters",
            display: "Change Manager",
            key: "change_manager",
            value: 0
        },
        "delegation_assignment": {
            section: "company_management",
            sub_section: "masters",
            display: "Delegation Assignment",
            key: "delegation_assignment",
            value: 0
        },
        "branch_mapping": {
            section: "company_management",
            sub_section: "masters",
            display: "Branch Mapping to HR",
            key: "branch_mapping",
            value: 0
        },
        "usb_download": {
            section: "attendance_management",
            sub_section: "attendance",
            display: "USB Download & Process",
            key: "usb_download",
            value: 0
        },
        "unprocessed_data": {
            section: "attendance_management",
            sub_section: "attendance",
            display: "Unprocessed Data",
            key: "unprocessed_data",
            value: 0
        },
        "reprocess": {
            section: "attendance_management",
            sub_section: "attendance",
            display: "Reprocess",
            key: "reprocess",
            value: 0
        },
        "leave_available": {
            section: "transaction_management",
            sub_section: "leave",
            display: "Leave Available",
            key: "leave_available",
            value: 0
        },
        "leave_application": {
            section: "transaction_management",
            sub_section: "leave",
            display: "Leave Application",
            key: "leave_application",
            value: 0
        },
        "leave_approval": {
            section: "transaction_management",
            sub_section: "leave",
            display: "Leave Approval",
            key: "leave_approval",
            value: 0
        },
        "leave_details": {
            section: "transaction_management",
            sub_section: "leave",
            display: "Leave Details",
            key: "leave_details",
            value: 0
        },
        "od_leave_application": {
            section: "transaction_management",
            sub_section: "leave",
            display: "OD Leave Application",
            key: "od_leave_application",
            value: 0
        },
        "od_leave_approval": {
            section: "transaction_management",
            sub_section: "leave",
            display: "OD Leave Approval",
            key: "od_leave_approval",
            value: 0
        },
        "od_leave_details": {
            section: "transaction_management",
            sub_section: "leave",
            display: "OD Leave Details",
            key: "od_leave_details",
            value: 0
        },
        "leave_assign": {
            section: "transaction_management",
            sub_section: "leave",
            display: "Leave Assign",
            key: "leave_assign",
            value: 0
        },
        "manual_punch_application": {
            section: "transaction_management",
            sub_section: "manual_punch",
            display: "Manual Punch Application",
            key: "manual_punch_application",
            value: 0
        },
        "manual_punch_details": {
            section: "transaction_management",
            sub_section: "manual_punch",
            display: "Manual Punch Details",
            key: "manual_punch_details",
            value: 0
        },
        "manual_punch_approval": {
            section: "transaction_management",
            sub_section: "manual_punch",
            display: "Manual Punch Approval",
            key: "manual_punch_approval",
            value: 0
        },
        "overtime_application": {
            section: "transaction_management",
            sub_section: "overtime",
            display: "Overtime Application",
            key: "overtime_application",
            value: 0
        },
        "overtime_details": {
            section: "transaction_management",
            sub_section: "overtime",
            display: "Overtime Details",
            key: "overtime_details",
            value: 0
        },
        "overtime_approval": {
            section: "transaction_management",
            sub_section: "overtime",
            display: "Overtime Approval",
            key: "overtime_approval",
            value: 0
        },
        "compoff_application": {
            section: "transaction_management",
            sub_section: "compoff",
            display: "Comp Off Application",
            key: "compoff_application",
            value: 0
        },
        "compoff_approval": {
            section: "transaction_management",
            sub_section: "compoff",
            display: "Comp Off Approval",
            key: "compoff_approval",
            value: 0
        },
        "compoff_validity": {
            section: "transaction_management",
            sub_section: "compoff",
            display: "Comp Off Validity",
            key: "compoff_validity",
            value: 0
        },
        "shift_roster": {
            section: "transaction_management",
            sub_section: "shift",
            display: "Shift Roster",
            key: "shift_roster",
            value: 0
        },
        "shift_roster_import": {
            section: "transaction_management",
            sub_section: "shift",
            display: "Shift Roster Import",
            key: "shift_roster_import",
            value: 0
        },
        "accounts": {
            section: "user_management",
            sub_section: "user",
            display: "Accounts",
            key: "accounts",
            value: 0
        },
        "access_permissions": {
            section: "user_management",
            sub_section: "user",
            display: "Access Permissions",
            key: "access_permissions",
            value: 0
        },
        "performance_report": {
            section: "reports",
            sub_section: "daily",
            display: "Daily Performance Report",
            key: "performance_report",
            value: 0
        },
        "attendance_report": {
            section: "reports",
            sub_section: "daily",
            display: "Daily Attendance Report",
            key: "attendance_report",
            value: 0
        },
        "detailed_report": {
            section: "reports",
            sub_section: "monthly",
            display: "Detailed Monthly Report",
            key: "detailed_report",
            value: 0
        },
        "overtime_report": {
            section: "reports",
            sub_section: "monthly",
            display: "Monthly Overtime Report",
            key: "overtime_report",
            value: 0
        },
        "muster_roll_report": {
            section: "reports",
            sub_section: "monthly",
            display: "Muster Roll Report",
            key: "muster_roll_report",
            value: 0
        },
        "payroll_link_report": {
            section: "reports",
            sub_section: "monthly",
            display: "Payroll Link Report",
            key: "payroll_link_report",
            value: 0
        },
        "compoff_details": {
            section: "transaction_management",
            sub_section: "compoff",
            display: "Comp Off Details",
            key: "compoff_details",
            value: 0
        },
        "shift_setting": {
            section: "company_management",
            sub_section: "configuration",
            display: "Shift Settings",
            key: "shift_setting",
            value: 0
        },
        "lan_device_information": {
            section: "device_management",
            sub_section: "lan",
            display: "LAN Device Information",
            key: "lan_device_information",
            value: 0
        },
        "lan_template_upload": {
            section: "device_management",
            sub_section: "lan",
            display: "LAN Template Upload",
            key: "lan_template_upload",
            value: 0
        },
        "lan_template_download": {
            section: "device_management",
            sub_section: "lan",
            display: "LAN Template Download",
            key: "lan_template_download",
            value: 0
        },
        "lan_enroll_card": {
            section: "device_management",
            sub_section: "lan",
            display: "LAN Enroll Card",
            key: "lan_enroll_card",
            value: 0
        },
        "process_data": {
            section: "attendance_management",
            sub_section: "attendance",
            display: "Process Data",
            key: "process_data",
            value: 0
        },
        "payroll_report": {
            section: "reports",
            sub_section: "daily",
            display: "Daily Payroll Report",
            key: "payroll_report",
            value: 0
        },
        "punch_report": {
            section: "reports",
            sub_section: "daily",
            display: "Daily Employee Punch Report",
            key: "punch_report",
            value: 0
        },
        "late_comers_report": {
            section: "reports",
            sub_section: "daily",
            display: "Late Comers Report",
            key: "late_comers_report",
            value: 0
        },
        "missing_swipe_report": {
            section: "reports",
            sub_section: "daily",
            display: "Missing Swipe Report",
            key: "missing_swipe_report",
            value: 0
        },
        "performance_in_out_report": {
            section: "reports",
            sub_section: "daily",
            display: "Performance In Out Report",
            key: "performance_in_out_report",
            value: 0
        },
        "change_company_logo": {
            section: "company_management",
            sub_section: "configuration",
            display: "Change Company Logo",
            key: "change_company_logo",
            value: 0
        },       
        "leave_card": {
            section: "reports",
            sub_section: "employee",
            display: "Leave Card",
            key: "leave_card",
            value: 0
        },
        "leave_register": {
            section: "reports",
            sub_section: "employee",
            display: "Leave Register",
            key: "leave_register",
            value: 0
        },
//        "leave_report": {
//            section: "reports",
//            sub_section: "monthly",
//            display: "Leave Report",
//            key: "leave_report",
//            value: 0
//        },
        "ramadan_history": {
            section:"company_management",
            sub_section: "configuration",
            display: "Ramadan History",
            key: "ramadan_history",
            value: 0
        },
//        "employee_transaction_data": {
//            section: "transaction_management",
//            sub_section: "employeetransaction",
//            display: "Employee Transaction",
//            key: "employee_transaction_data",
//            value: 0
//        },

        "outofoffice_apply": {
            section: "transaction_management",
            sub_section: "outofoffice",
            display: "OOO Application",
            key: "outofoffice_apply",
            value: 0
        },
        "outofoffice_approve": {
            section: "transaction_management",
            sub_section: "outofoffice",
            display: "OOO Approval",
            key: "outofoffice_approve",
            value: 0
        },
        "outofoffice_details": {
            section: "transaction_management",
            sub_section: "outofoffice",
            display: "OOO Details",
            key: "outofoffice_details",
            value: 0
        }

        
        
    };

    // END OF PERMISSIONS MAP

    var 
		selected_perms = {},
		unselected_perms = {};

    function getSelected() {
        return selected_perms;
    }

    function getUnselected() {
        return unselected_perms;
    }

    function getSections() {
        return sections;
    }

    function getDefaultPermissions() {
        return $.extend(true, {}, perms_map);
    }

    function groupBySection(perms) {
        var final = {}, i;

        for (i in sections) {
            final[i] = _.where(perms, { section: i });
        }

        return final;
    }

    function processPermissions(section) {

        var current_perms = section.split(""),
			perms = {};

        $.extend(true, perms, perms_map);

        var i = 0, current_value = "";

        $.each(perms, function(key, value) {

            if (current_perms[i]) {
                perms[key]["value"] = current_perms[i];
            }
            i++;
        });

        return perms;
    }

    return {
        process: processPermissions,
        sections: getSections,
        group: groupBySection,
        defaultMap: getDefaultPermissions
    };

})(jQuery, window, document);