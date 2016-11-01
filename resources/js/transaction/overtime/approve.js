var OvertimeApproval = (function($, w, d) {

    var 
        main, _initButtons, _initDialogs, _initOther,
        _getData, _success, _failure,
        _getOverTimeData, _processOverTimeData, _renderOverTimeTable,
        _loadMoreOverTime,
        _doAction, _processAction, _commentDialog,
        _filterData, _resetFilters, _filterDataOnValidate;

    var 
        comments_dialog_class, comments_dialog;

    var 
        $over_time_table = $('#overTimeApprovalTable'),
        $over_time_table_parent = $over_time_table.parent(),
        $over_time_no_data = $('#otNoData'),
        $over_time_pagination = $('#otPagination').parent(),
        $comments_dialog = $('#approvalCommentDialog'),
        $comments = $('#approvalComment'),
        $filters_box = $('#overTimeApprovalFilters'),
        $filters_form = $('#overTimeApprovalFiltersForm'),
        $action_button = $('#approveOverTimeButton');


    var 
        selected_action = 0,
        over_time_page_number = 1,
        is_filter = false,
        over_time_no_data_HTML = '<p><span class="text-orange fa fa-frown-o"></span> <strong>No OverTime data found.</strong></p>',
        page_name = 'approve.aspx',
        over_time_data_URL = page_name + '/getOvertimeData',
        company_data_URL = page_name + '/GetCompanyData',
        department_data_URL = page_name + '/GetDepartmentData',
        designation_data_URL = page_name + '/GetDesignationData';

    var 
        status_map = {
            1: "Submitted",
            2: "Approved",
            3: "Declined",
            4: "Cancelled"
        };



    /******************************************************************************************************************/

    _resetFilters = function() {

        is_filter = false;
        over_time_page_number = 1;

        $filters_box.slideToggle();

        $filters_form[0].reset();
        $over_time_table.find('tbody').empty();

        //SAXLoader.show();

        //_getOverTimeData();

    };

    _validateFilters = function() {

        var 
            data = SAXForms.get($filters_form);

        //         if (data['filter_by'] == '0') {
        //            SAXAlert.show({'type': 'error', 'message': 'Please select a OT Date.'});
        //            return false;
        //        }
        //       
        //        if (data['filter_hours'] == '' ) {
        //            SAXAlert.show({'type': 'error', 'message': 'Please select a OT Hours.'});
        //            return false;
        //        }
        //  
        if (data['filter_CompanyCode'] == 'select') {
            SAXAlert.show({ 'type': 'error', 'message': 'Please select a Company.' });
            return false;
        }
        if (data['employee_id'] == '') {
            SAXAlert.show({ 'type': 'error', 'message': 'Please select a Employee.' });
            return false;
        }
        //if (data['employee_name'] == '') {
        //    SAXAlert.show({ 'type': 'error', 'message': 'Please select a Employee.' });
        //    return false;
        //}
        if (data['filter_from'] == '') {
            SAXAlert.show({ 'type': 'error', 'message': 'Please select From Date.' });
            return false;
        }
        if (data['filter_to'] == '') {
            SAXAlert.show({ 'type': 'error', 'message': 'Please select To Date.' });
            return false;
        }
        

        return true;
    };

    _filterData = function() {

        if (_validateFilters()) {
            //


            var fromDateValue = $('#filter_to').val();
            //alert(fromDateValue);
            var afterFormat = moment(fromDateValue, "DD-MMM-YYYY");
            // alert(afterFormat.format("dddd"));
            if (afterFormat.format("dddd") != 'Friday') {
                
                var message = 'you choosed other than friday, Do you want to continue YES/NO?';
                var $OT_confirmation_dialog = $('#checkFridayDailog'), $ooo_confirmation_button = $('#confirmshift');
                $OT_confirmation_dialog.find('.modal-body').html('<p>' + message + '</p>');
                $OT_confirmation_dialog.modal('show');
            }
            else {

                is_filter = true;
                over_time_page_number = 1;
                SAXLoader.show();

                $filters_box.slideToggle();

                $over_time_table.find('tbody').empty();

                _getOverTimeData();

            }




        }
    };

    _filterDataOnValidate = function() {

        if (true) {
            //
            var $OT_confirmation_dialog = $('#checkFridayDailog');
            $OT_confirmation_dialog.modal('hide');

           


            is_filter = true;
            over_time_page_number = 1;
            SAXLoader.show();

            $filters_box.slideToggle();

            $over_time_table.find('tbody').empty();

            _getOverTimeData();
        }
    };















    /******************************************************************************************************************/

    _loadMoreOverTime = function() {

        over_time_page_number += 1;
        SAXLoader.show();
        _getOverTimeData();
    };

    /******************************************************************************************************************/

    _processAction = function(data, additional) {

        var i = 0,
                selected_employees = additional.selected_employees,
                selected_employees_length = additional.selected_employees.length;

        selected_action = 0;

        for (i = 0; i < selected_employees_length; i += 1) {
            $('#' + selected_employees[i]).remove();
        }

        comments_dialog.close();
        SAXAlert.show({ 'type': data.status, 'message': data.return_data });
        $action_button.button('reset');
    };

    _doAction = function() {

        var 
            selected_rows = [], selected_rows_length = 0,
            selected_rows_temp = [],
            ajax_options = {},
            comments = $comments.val();

        selected_rows_temp = $over_time_table.find('tbody input:checked');
        selected_rows_temp_length = selected_rows_temp.length;

        if (selected_rows_temp_length === 0) {
            SAXAlert.show({ 'type': 'error', 'message': 'Please select atleast one row.' });
            return false;
        }

        for (var i = 0; i < selected_rows_temp_length; i++) {
            selected_rows.push($(selected_rows_temp[i]).val());
        };

        ajax_options = {
            'url': page_name + '/DoAction',
            'data': { action: selected_action, comments: comments, selected_rows: JSON.stringify(selected_rows) },
            'callback': _processAction,
            'additional': {
                'selected_employees': selected_rows
            }
        };

        $action_button.button('loading');
        SAXHTTP.ajax(ajax_options);

    };

    /*******************************************************************************************************************/

    _renderDepartmentDropdown = function(data) {
        var 
           select_HTML = '';

        select_HTML = '<option value="select">Select Department</option>',
        data_length = data.length,
        counter = 0,
        $element = $('#filter_DepartmentCode'),
        $parent = $element.parent();
        if (data_length > 0) {
            for (counter = 0; counter < data_length; counter += 1) {

                select_HTML += '<option value="' + data[counter]['DeptCode'] + '">' + data[counter]['deptName'] + '</option>';
            }
        }
        else {
            select_HTML = '<option value="select">' + no_data + '</option>';
        }

        $element.append(select_HTML);

    };

    _renderDesignationDropdown = function(data) {
        var 
           select_HTML = '';

        select_HTML = '<option value="select">Select Designation</option>',
        data_length = data.length,
        counter = 0,
        $element = $('#filter_DesignationCode'),
        $parent = $element.parent();
        if (data_length > 0) {
            for (counter = 0; counter < data_length; counter += 1) {

                select_HTML += '<option value="' + data[counter]['desigcode'] + '">' + data[counter]['designame'] + '</option>';
            }
        }
        else {
            select_HTML = '<option value="select">' + no_data + '</option>';
        }

        $element.append(select_HTML);

    };

    _processDepartmentData = function(data) {

        var status = data.status,
            results = {};

        if (status === 'success') {

            results = JSON.parse(data.return_data);
            // resetting department drop down value ...
            $('#filter_DepartmentCode').empty();
            _renderDepartmentDropdown(results);

            results = {};
        }
        else {
            SAXAlert.show({ 'type': 'error', 'message': data.return_data });
        }

        SAXLoader.closeBlockingLoader();
        var company_code = $('#filter_CompanyCode');

        appendDesignationData(company_code.val());
    };

    _processDesignationData = function(data) {

        var status = data.status,
            results = {};

        if (status === 'success') {

            results = JSON.parse(data.return_data);
            // resetting department drop down value ...
            $('#filter_DesignationCode').empty();
            _renderDesignationDropdown(results);

            results = {};
        }
        else {
            SAXAlert.show({ 'type': 'error', 'message': data.return_data });
        }

        SAXLoader.closeBlockingLoader();
    };

    _loadDepartmentData = function(company_code) {

        var 
            ajax_options = {
                'url': page_name + '/GetDepartmentData',
                'data': { company_code: company_code },
                'callback': _processDepartmentData,
                'additional': {}
            }

        SAXLoader.showBlockingLoader();
        SAXHTTP.ajax(ajax_options);
    };

    /******************************************************************************************************************/

    function appendDesignationData(company_code) {

        var 
            ajax_options = {
                'url': page_name + '/GetDesignationData',
                'data': { company_code: company_code },
                'callback': _processDesignationData,
                'additional': {}
            }

        SAXLoader.showBlockingLoader();
        SAXHTTP.ajax(ajax_options);
    }


    /******************************************************************************************************************/

    _renderOverTimeTable = function(data) {

        var 
            data_length = data.length,
            table_HTML = '',
            counter = 0, modifiedby, Status, TimingFrom, TimingTo, RoundedHours,
            WithinLegalRequirementTime, AboveLegalRequirementTime,
            ShortageWorkingHours, WithinLegalStatus, AboveLegalStatus;
        var totalRoundedHours = '';
        var TotalWLR = '', TotalALR = '', TotalSWH = ''
        var counter1 = 0;
        var empid_txt = data[0]['employeeId'];
        $('#employee_lable').text('Employee Id : ' + empid_txt);

        for (counter = 0; counter < data_length; counter += 1) {
            var weekDay = true, weekOff = false;
            //modifiedby = data[counter]['modifiedby'] == null ? '' : data[counter]['modifiedby'];
            //Status = data[counter]['Status'] == null ? '' : data[counter]['Status'];
            TimingFrom = data[counter]['TimingFrom'] == null ? '' : moment(data[counter]['TimingFrom'], 'HH:mm').format("HH:mm");
            TimingTo = data[counter]['TimingTo'] == null ? '' : moment(data[counter]['TimingTo'], 'HH:mm').format("HH:mm");
            RoundedHours = data[counter]['RoundedHours'] == null ? '' : moment(data[counter]['RoundedHours'], 'HH:mm').format("HH:mm");
            WithinLegalRequirementTime = data[counter]['WithinLegalRequirementTime'] == null ? '00:00' : moment(data[counter]['WithinLegalRequirementTime'], 'HH:mm').format("HH:mm");
            AboveLegalRequirementTime = data[counter]['AboveLegalRequirementTime'] == null ? '00:00' : moment(data[counter]['AboveLegalRequirementTime'], 'HH:mm').format("HH:mm");
            ShortageWorkingHours = data[counter]['ShortageWorkingHours'] == null ? '00:00' : moment(data[counter]['ShortageWorkingHours'], 'HH:mm').format("HH:mm");
            // WithinLegalStatus = data[counter]['WithinLegalStatus'] == 'NO' ? WithinLegalStatus.html('&#10003;') : data[counter]['ShortageWorkingHours'];
            var emp_id = data[counter]['employeeId'];
            var day = moment(data[counter]['WeekDate']).format("DD-MMM-YYYY");
            var chkBoxId = emp_id + '_' + day;
            var dropDownId = emp_id + '_' + day + '_select';
            var dropDownIdForLegal = emp_id + '_' + day + '_select_legal';
            counter1++;
            table_HTML += '<tr  id="' + data[counter]['Weekdays'] + '" >' +
                        '<td style="width:30px;"><input type="checkbox" id="' + chkBoxId + '" name="' + chkBoxId + '" value="' + chkBoxId + '" checked class="hide"></td>' +
                        '<td style="width:97px;">' + data[counter]['Weekdays'] + '</td>' +
                        '<td style="width:103px;">' + moment(data[counter]['WeekDate']).format("DD-MMM-YYYY") + '</td>' +
                        '<td style="width:81px;">' + TimingFrom + '</td>' +
                        '<td style="width:81px;" >' + TimingTo + '</td>' +
                        '<td style="width:81px;" >' + RoundedHours + '</td>' +
                        '<td style="width:116px;" >' + WithinLegalRequirementTime + '</td>' +
                        '<td  style="width:90px;"><select class="form-control" id="' + dropDownId + '" name="' + dropDownId + '" ><option value="1">yes</option><option value="0">No</option></select></td>' +
                        '<td style="width:154px;">' + AboveLegalRequirementTime + '</td>' +
                        '<td style="width:91px;"><select class="form-control" id="' + dropDownIdForLegal + '" name="' + dropDownIdForLegal + '" ><option value="1">yes</option><option value="0">No</option></select></td>' +
                        '<td>' + ShortageWorkingHours + '</td>'
            '</tr>';



            if (counter1 == 1) {
                totalRoundedHours = RoundedHours;
                TotalWLR = WithinLegalRequirementTime;
                TotalALR = AboveLegalRequirementTime;
                TotalSWH = ShortageWorkingHours;
            }
            else {
                totalRoundedHours = totalRoundedHours + '||' + RoundedHours;
                TotalWLR = TotalWLR + '||' + WithinLegalRequirementTime;
                TotalALR = TotalALR + '||' + AboveLegalRequirementTime;
                TotalSWH = TotalSWH + '||' + ShortageWorkingHours;
            }

            //alert(totalRoundedHours);
            if (data[counter]['Weekdays'] == 'Friday') {
                weekOff = true;
                var totalTimeInMinutes = concatTime(totalRoundedHours);
                var afterTotalRound = toTime(totalTimeInMinutes);

                var TotalWLRInMinutes = concatTime(TotalWLR);
                var afterTotalWLRRound = toTime(TotalWLRInMinutes);

                var TotalALRInMinutes = concatTime(TotalALR);
                var afterTotalALRRound = toTime(TotalALRInMinutes);

                var TotalSWHInMinutes = concatTime(TotalSWH);
                var afterTotalSWHRound = toTime(TotalSWHInMinutes);

                table_HTML += '<tr style="background-color:#fafafa;"  id="' + data[counter]['Weekdays'] + '" >' +
                '<td style="width:30px;">Total </td>' +
                '<td style="width:80px;"> </td>' +
                '<td> </td>' +
                '<td> </td>' +
                '<td> </td>' +
                '<td>' + afterTotalRound + '</td>' +
                '<td>' + afterTotalWLRRound + '</td>' +
                '<td> </td>' +
                '<td>' + afterTotalALRRound + '</td>' +
                '<td> </td>' +
                '<td>' + afterTotalSWHRound + '</td>' +
                '</tr>';

                totalRoundedHours = '';
                TotalWLRInMinutes = '';
                TotalALRInMinutes = '';
                TotalSWHInMinutes = '';

                counter1 = 0;
            }

        }

        if (weekOff == false && weekDay == true) {
            var totalTimeInMinutes = concatTime(totalRoundedHours);
            var afterTotalRound = toTime(totalTimeInMinutes);

            var TotalWLRInMinutes = concatTime(TotalWLR);
            var afterTotalWLRRound = toTime(TotalWLRInMinutes);

            var TotalALRInMinutes = concatTime(TotalALR);
            var afterTotalALRRound = toTime(TotalALRInMinutes);

            var TotalSWHInMinutes = concatTime(TotalSWH);
            var afterTotalSWHRound = toTime(TotalSWHInMinutes);

            table_HTML += '<tr style="background-color:#fafafa;">' +
                '<td style="width:30px;">Total</td>' +
                '<td style="width:80px;"> </td>' +
                '<td> </td>' +
                '<td> </td>' +
                '<td> </td>' +
                '<td>' + afterTotalRound + '</td>' +
                '<td>' + afterTotalWLRRound + '</td>' +
                '<td> </td>' +
                '<td>' + afterTotalALRRound + '</td>' +
                '<td> </td>' +
                '<td>' + afterTotalSWHRound + '</td>' +
                '</tr>';

            totalRoundedHours = '';
            TotalWLRInMinutes = '';
            TotalALRInMinutes = '';
            TotalSWHInMinutes = '';
        }

        $over_time_table.detach();

        $over_time_table.find('tbody').append(table_HTML);
        $over_time_table_parent.prepend($over_time_table);
    };

    _processOverTimeData = function(data, additional) {

        var 
            status = data.status,
            results = {},
            results_length = 0,
            is_no_data = $over_time_no_data.hasClass('hide'),
            is_pagination = $over_time_pagination.hasClass('hide');

        if (status === 'success') {

            results = JSON.parse(data.return_data);
            results_length = results.length;

            if (results_length > 0) {
                if (!is_no_data) {
                    $over_time_no_data.empty();
                    $over_time_no_data.addClass('hide');
                }

                _renderOverTimeTable(results);

                results_length === 30 ? $over_time_pagination.removeClass('hide') : $over_time_pagination.addClass('hide');
            }
            else {

                if (is_no_data) {
                    $over_time_no_data.html(over_time_no_data_HTML);
                    $over_time_no_data.removeClass('hide');
                }

                if (is_pagination) $over_time_pagination.addClass('hide');
            }
        }
        else {

            SAXAlert.show({ 'type': status, 'message': data.return_data });
        }

        if (additional && additional.loading) SAXLoader.close();
    };

    _getOverTimeData = function() {

        var 
            filters_data = SAXForms.get($filters_form),
            ajax_options = {
                'url': over_time_data_URL,
                'data': { page_number: over_time_page_number, is_filter: is_filter, filters: JSON.stringify(filters_data) },
                'callback': _processOverTimeData,
                'additional': {
                    'loading': true
                }
            }

        SAXHTTP.ajax(ajax_options);
    };

    _renderCompanyDropdown = function(data) {

        var 
            select_HTML = '<option value="select">Select a Company</option>',
            data_length = data.length,
            counter = 0,
            $element = $('#filter_CompanyCode'),
            $parent = $element.parent();

        for (counter = 0; counter < data_length; counter += 1) {

            select_HTML += '<option value="' + data[counter]['CompanyCode'] + '">' + data[counter]['CompanyName'] + '</option>';
        }

        $element.append(select_HTML);
    };

    _processCompanyData = function(data) {

        var status = data.status,
            results = {};

        if (status === 'success') {

            results = JSON.parse(data.return_data);
            _renderCompanyDropdown(results);
        }
        else {
            SAXAlert.show({ 'type': 'error', 'message': data.return_data });
        }
    };

    /******************************************************************************************************************/

    _failure = function() {

        SAXLoader.close();
        SAXAlert.show({ 'type': 'error', 'message': 'An error occurred while loading data for the page. Please try again. If the error persists, please contact Support.' });
    };

    _success = function(data1, data2) {
        _processOverTimeData(data1[0].d);
        _processCompanyData(data2[0].d);
        SAXLoader.close();
    };

    _getData = function() {

        var 
            data1 = { page_number: over_time_page_number, is_filter: is_filter, filters: JSON.stringify({}) },
            promise1 = $.ajax({ url: over_time_data_URL, type: "POST", contentType: 'application/json; charset=utf-8', dataType: 'json', data: JSON.stringify(data1) });
        promise2 = $.ajax({ url: company_data_URL, type: "POST", contentType: 'application/json; charset=utf-8', dataType: 'json', data: JSON.stringify() });

        SAXLoader.show();
        $.when(promise1, promise2).then(_success, _failure);
    };

    _initOther = function() {

        $('.date-picker').Zebra_DatePicker({
            format: 'd-M-Y'
        });

        $('.time-picker').timepicki({
            show_meridian: false,
            overflow_minutes: true,
            min_hour_value: 0,
            max_hour_value: 23
        });

        $('#CheckAll').change(function() {
            var is_checked = $(this).is(':checked'),
                checkboxes = $over_time_table.find('tbody input[type="checkbox"]');
            is_checked ? $(checkboxes).prop('checked', true) : $(checkboxes).prop('checked', false);
        });

        $('#filter_CompanyCode').change(function() {

           // _loadDepartmentData($(this).val());
            $('#employee_id').focus();
            // _loadDesignationData($(this).val());
        });

    };

    _initDialogs = function() {

        comments_dialog_class = SAXDialog.extend({
            'element': $comments_dialog
        });
        comments_dialog = new comments_dialog_class();

        $comments_dialog.on('hidden.bs.modal', function() {
            $comments.val('');
        })
    };

    _initButtons = function() {

        var role = '',
            button_actions = {
                'load-more-ot-data': function() {
                    _loadMoreOverTime();
                },
                'toggle-filters': function() {
                    $filters_box.slideToggle();
                },
                'filter-data': function() {
                    _filterData();
                }, //
                'Ot/filter': function() {
                    _filterDataOnValidate();
                },
                'reset-filters': function() {
                    _resetFilters();
                },
                'action-button-click': function(event) {
                    selected_action = $(event.target).data('operation');
                    comments_dialog.open();
                },
                'confirm-approval': function() {
                    _doAction();
                }
                ,
                'data/back': function() {

                    resetPage();
                }
                ,
                'data/approve_final': function() {

                    saveFinalApprovedData();
                }
            };

        $('body').on('click', '[data-control="button"]', function(event) {
            role = $(event.target).data('role');
            button_actions[role].call(this, event);
        });
    };

    main = function() {

        _getData();
        _initButtons();
        _initDialogs();


        _initOther();
    };

    return {
        'main': main
    };

})(jQuery, window, document);

$(function () {
    OvertimeApproval.main();

});

function concatTime(time) 
{
    var splitArray = time.split('||');
    var hours = 0, minutes = 0, totalMinutes=0;
    totalMinutes = Number(totalMinutes);
    for (var i = 0; i < splitArray.length; i++) 
    {
            if (splitArray[i]=='') {
               // splitArray[i] = Number(splitArray[i]);
            splitArray[i] = '00:00';
            //alert(splitArray[i]);
        }
          var timeArray = splitArray[i].split(':');
          hours = parseInt(Number(timeArray[0]));
          minutes = parseInt(Number(timeArray[1]));
          //alert(hours + ':' + minutes);
          //alert(Number((Number(hours) * 60)) + Number(minutes));
         totalMinutes = Number(totalMinutes) + Number((Number(hours) * 60)) + Number(minutes);

     }
     
    return totalMinutes;
}
function toTime(number) {
    number = Number(number);
    var hours = 0, minutes=0;
    hours = Math.floor(number / 60);
    minutes = number % 60;
    var result = hours < 10 ? '0' + hours: hours;
    return  result + ":" + (minutes <= 9 ? "0" : "") + minutes;
    
}

function saveApprovedData() 
{
   
    var $table = $('#overTimeApprovalTable');
    var $filterFromDate = $('#filter_from');
    var filterFromDateValue =$filterFromDate.val();
    var $filterToDate = $('#filter_to');
    var filterToDateValue = $filterToDate.val();
    var empid_txt = $('#employee_id').val();
   // $('#employee_lable').text('Employee Id : ' + empid_txt);
    //employee_lable

    var list_elements = {
        table: $('#overTimeApprovalTable')

    };
    var $over_time_table = $('#overTimeApprovalTable');
    var table_body = list_elements.table.find("tbody");

    //  selected_rows_temp = $over_time_table.find('tbody input:checked');
    var
	            selected_employees = $over_time_table.find('tbody input:checked');
    var   employees = [], employeesDropdownVal = [],
       selected_employees_length = selected_employees.length;

    if (selected_employees_length == 0) 
    {
        SAXAlert.show({ type: "error", message: "Please select one or more employees" });
    }
    else
{
    var return_data='{"OverTime":[';

    for (var i = 0; i < selected_employees_length; i++) {

        var EmpID, PDate, WLR, ALR;
       
        
        var selectedId = selected_employees[i]+'_select';

        var dropdownId = $(selected_employees[i]).val() + '_select'; //_select_legal

        var dropdownvalue = $('#' + dropdownId + '').val();

        var dropdownIdForLegal = $(selected_employees[i]).val() + '_select_legal';

        var dropdownvalueForLegal = $('#' + dropdownIdForLegal + '').val();
        
        var chkboxId=$(selected_employees[i]).val();
        var splitEmployeeData=chkboxId.split('_');
        EmpID=splitEmployeeData[0];
        PDate = splitEmployeeData[1];
        WLR = dropdownvalue;
        ALR = dropdownvalueForLegal;
       
        return_data += '{"EmployeeID":"'+EmpID+'","OTDate":"'+PDate+ '", "WLR":"'+WLR+'","ALR":"'+ALR +'"},';

    }
    
    return_data+=']}';

    // alert(return_data);
    //var page_name = 'approve.aspx';
    //    var 
    //        ajax_options = {
    //        'url': page_name + '/DoContinue',
    //        'data': { current: return_data}
                
    //        }

    //  SAXLoader.showBlockingLoader();
    //  var result=SAXHTTP.ajax(ajax_options);
   // alert(filterFromDateValue + ':' + filterToDateValue)
    SAXHTTP.AJAX(
          "approve.aspx/DoContinue",
          { current: return_data, filterFromDate: filterFromDateValue, filterToDate: filterToDateValue, empid: empid_txt }
          //{employees: JSON.stringify(final_data), date: date.format("MM-YYYY")}
      ).done(function (data)
      {

          var status = data.d.status;
          if (status == "success")
          {
              // alert(status);
              var otFirstTable = $("#Ot_Approval_Table_First");
              var otSecondTable = $("#Ot_Approval_Table_Second");
              var filterBtn = $("#filterBtn");


              otFirstTable.addClass("hide");
              filterBtn.addClass("hide");
              otSecondTable.removeClass("hide");

              //fetching data from Table
              SAXHTTP.AJAX(
                     "approve.aspx/DoContinueForOt2",
                     {}
                     //{employees: JSON.stringify(final_data), date: date.format("MM-YYYY")}
                 ).done(function (data)
                 {

                     var status = data.d.status;
                     if (status == "success")
                     {
                         //appending data to table
                         //console.log("data : " + data.d.return_data);
                         var results = JSON.parse(data.d.return_data);
                         var results_length = results.length;
                         renderDataTable(results);
                     }

                 })
                    .fail(function () {
                        SAXAlert.show({ type: "error", message: "An error occurred while Approving first part of the OT. Please try again." });
                    });
             

          }
          else
          {
              SAXAlert.show({ type: "error", message: "An error occurred while storing data to database." });
          }
          // });
      })
        .fail(function () {
    SAXAlert.show({type: "error", message: "An error occurred while Approving first part of the OT. Please try again."});
});
}
     
}


/*rendering table*/

function renderDataTable(data) {

    var
        data_length = data.length,
        table_HTML = '',
        counter = 0, modifiedby, Status, TimingFrom, TimingTo, RoundedHours,
        WithinLegalRequirementTime, AboveLegalRequirementTime,
        ShortageWorkingHours, WithinLegalStatus, AboveLegalStatus;
    var totalRoundedHours = '';
    var TotalWLR = '', TotalALR = '', TotalSWH = ''
    var TotalWeekly_Mandatory_Hours = '', TotalActualWeekly_Mandatory_Hours = '', TotalRejectedOT = '', TotalWithin = '', TotalAbove = '', TotalshortageCompletedHrs = '', TotalRegulaOtTable = '';
    var TotalNightOtTable = '',TotalWithinWo='',TotalAboveWo='',TotalPublicOT='',TotalWoCO='';
    var counter1 = 0;
    var  OT_Table2= $('#overTimeApprovalTable2');
    $over_time_table = $('#overTimeApprovalTable2');
    $over_time_table_parent = $over_time_table.parent();
 
    for (counter = 0; counter < data_length; counter += 1)
    {
        var emp_id = data[counter]['EmployeeID'];
        var EmployeeName = data[counter]['EmployeeName'];
        var WeekDates = data[counter]['WeekDates'];
        var WeeklyMandatoryHours = toTime(data[counter]['WeeklyMandatoryHours']);
        var ActualWeeklyRoundedHours = toTime(data[counter]['ActualWeeklyRoundedHours']);
        var RejectedOT = toTime(data[counter]['RejectedOT']);
        var WithinLegalLimits =toTime( data[counter]['WithinLegalLimits']);
        var AboveLegalLimits =toTime(data[counter]['AboveLegalLimits']);
        var ShortageofCompletedWorkingHours = toTime(data[counter]['ShortageofCompletedWorkingHours']);
        var TotalRegularOT = toTime(data[counter]['TotalRegularOT']);
        var TotalNightOT = toTime(data[counter]['TotalNightOT']);

        var WithinLegal = toTime(data[counter]['WithinLegal']);
        var AboveLegal = toTime(data[counter]['AboveLegal']);
        var PublicHolidayOvertime = data[counter]['PublicHolidayOvertime'];
        
        var WeekendcompensatewithCompOffDays = data[counter]['WeekendcompensatewithCompOffDays'];
        if (counter == 0)
        {
            TotalWeekly_Mandatory_Hours = WeeklyMandatoryHours;
            TotalActualWeekly_Mandatory_Hours = ActualWeeklyRoundedHours;
            TotalRejectedOT = RejectedOT;
            TotalWithin = WithinLegalLimits;
            TotalAbove = AboveLegalLimits;
            TotalshortageCompletedHrs = ShortageofCompletedWorkingHours;
            TotalRegulaOtTable = TotalRegularOT;
            TotalNightOtTable = TotalNightOT;

            TotalWithinWo = WithinLegal;
            TotalAboveWo = AboveLegal;
            TotalPublicOT = toTime(PublicHolidayOvertime);
            TotalWoCO = WeekendcompensatewithCompOffDays;
        }
        else
        {
            TotalWeekly_Mandatory_Hours = TotalWeekly_Mandatory_Hours + '||' + WeeklyMandatoryHours;
            TotalActualWeekly_Mandatory_Hours = TotalActualWeekly_Mandatory_Hours + '||' + ActualWeeklyRoundedHours;
            TotalRejectedOT = TotalRejectedOT + '||' + RejectedOT;
            TotalWithin = TotalWithin + '||' + WithinLegalLimits;
            TotalAbove = TotalAbove + '||' + AboveLegalLimits;
            TotalshortageCompletedHrs = TotalshortageCompletedHrs + '||' + ShortageofCompletedWorkingHours;
            TotalRegulaOtTable = TotalRegulaOtTable + '||' + TotalRegularOT;
            TotalNightOtTable = TotalNightOtTable + '||' + TotalNightOT;

            TotalWithinWo = TotalWithinWo + '||' + WithinLegal;
            TotalAboveWo = TotalAboveWo + '||' + AboveLegal;
            TotalPublicOT = TotalPublicOT + '||' + toTime(PublicHolidayOvertime);
            TotalWoCO = TotalWoCO + '||' + WeekendcompensatewithCompOffDays;
        }
        
       var PublicHolidayscompensatewithCompOffDays =data[counter]['PublicHolidayscompensatewithCompOffDays'];
        var weekDatesId = counter + '_txt';
        var WOPTxtId = counter + '_WOP_txt';
        var HPTxtId = counter + '_HP_txt';
        var WOP = counter + '_WOP_select';
        var HP = counter + '_HP_select';
        var chkBoxId = counter;
        
        table_HTML += '<tr>'+
                        '<td><input type="text" id=' + weekDatesId + ' value=' + WeekDates + ' style="width:80px;border:none" readonly></td>' +
                        '<td>' + WeeklyMandatoryHours + '</td>' +
                        '<td>' + ActualWeeklyRoundedHours + '</td>' +
                        '<td>' + RejectedOT + '</td>' +
                        '<td>' + WithinLegalLimits + '</td>' +
                        '<td>' + AboveLegalLimits + '</td>' +
                        '<td>' + ShortageofCompletedWorkingHours + '</td>' +
                        '<td>' + TotalRegularOT + '</td>' +
                        '<td>' + TotalNightOT + '</td>' +
                        '<td>' + WithinLegal + '</td>' +
                        '<td>' + AboveLegal + '</td>' +
                        '<td>' + toTime(PublicHolidayOvertime) + '</td>' +
        //'<td>' + WeekendcompensatewithCompOffDays + '</td>' +
                         '<td><input type="text" id=' + WOPTxtId + ' value=' + WeekendcompensatewithCompOffDays + ' style="width:10px;border:none" readonly></td>' +
                        '<td><select class="form-control" id="' + WOP + '" name="' + WOP + '" ><option value="1">yes</option><option value="0">No</option></select></td>' +
        //'<td>' + PublicHolidayscompensatewithCompOffDays + '</td>' +
                        '<td><input type="text" id=' + HPTxtId + ' value=' + PublicHolidayscompensatewithCompOffDays + ' style="width:10px;border:none" readonly></td>' +
                        '<td><select class="form-control" id="' + HP + '" name="' + HP  + '" ><option value="1">yes</option><option value="0">No</option></select></td>' +
                       '<td><input type="checkbox" id="' + chkBoxId + '" name="' + chkBoxId + '" value="' + chkBoxId + '" checked class="hide"></td>' +
                      '</tr>';        

    }
    var TotalWeekly_Mandatory_HoursInMinutes = concatTime(TotalWeekly_Mandatory_Hours);
    var afterTotalWeekly_Mandatory_Hours = toTime(TotalWeekly_Mandatory_HoursInMinutes);

    var TotalActuallyWeekly_Mandatory_HoursInMinutes = concatTime(TotalActualWeekly_Mandatory_Hours);
    var afterTotalActuallyWeekly_Mandatory_Hours = toTime(TotalActuallyWeekly_Mandatory_HoursInMinutes);

    var TotalRejectedOTHoursInMinutes = concatTime(TotalRejectedOT);
    var afterTotalRejectedOTHoursInMinutes = toTime(TotalRejectedOTHoursInMinutes);


    var TotalWithinInMinutes = concatTime(TotalWithin);
    var afterTotalWithinInMinutes = toTime(TotalWithinInMinutes);

    var TotalAboveinInMinutes = concatTime(TotalAbove);
    var afterTotalTotalAbove = toTime(TotalAboveinInMinutes);

    var TotalshortageCompletedHrsInMinutes = concatTime(TotalshortageCompletedHrs);
    var afterTotalTotalshortageCompletedHrsInMinutes = toTime(TotalshortageCompletedHrsInMinutes);


    var TotalRegulaOtTableInMinutes = concatTime(TotalRegulaOtTable);
    var afterTotalRegulaOtTable = toTime(TotalRegulaOtTableInMinutes);

    var TotalNightOtTableInMinutes = concatTime(TotalNightOtTable);
    var afterTotalNightOtTable = toTime(TotalNightOtTableInMinutes);

    var TotalWithinWoInMinutes = concatTime(TotalWithinWo);
    var afterTotalWithinWo = toTime(TotalWithinWoInMinutes);

    var TotalAboveWoInMinutes = concatTime(TotalAboveWo);
    var afterTotalAboveWo = toTime(TotalAboveWoInMinutes);

    var TotalPublicOTInMinutes = concatTime(TotalPublicOT);
    var afterTotalPublicOT = toTime(TotalPublicOTInMinutes);
 
   // var TotalTotalWoCO = concatTime(TotalWoCO);
    //var afterTotalTotalWoCO = toTime(TotalTotalWoCO);
    

    table_HTML += '<tr style="background-color:#2980b9;color:white;">' +
                  '<td>Total </td>' +
                  '<td>' + afterTotalWeekly_Mandatory_Hours + '</td>' +
                        '<td>'+afterTotalActuallyWeekly_Mandatory_Hours+'</td>' +
                        '<td>'+ afterTotalRejectedOTHoursInMinutes + '</td>' +
                        '<td>' + afterTotalWithinInMinutes + ' </td>' +
                        '<td>'+afterTotalTotalAbove+' </td>' +
                        '<td>'+afterTotalTotalshortageCompletedHrsInMinutes+' </td>' +
                        '<td>'+afterTotalRegulaOtTable+' </td>' +
                        '<td>'+afterTotalNightOtTable+' </td>' +
                        '<td>'+afterTotalWithinWo+' </td>' +
                        '<td>'+afterTotalAboveWo+' </td>' +
                        '<td>' + afterTotalPublicOT + ' </td>' +
        //'<td>' + WeekendcompensatewithCompOffDays + '</td>' +
                         '<td></td>' +
                        '<td> </td>' +
        //'<td>' + PublicHolidayscompensatewithCompOffDays + '</td>' +
                        '<td> </td>' +
                        '<td> </td>' +
                        
                      '</tr>';

    $over_time_table.detach();
    $over_time_table.find('tbody').append(table_HTML);
    $over_time_table_parent.prepend($over_time_table);
}



$('#OtCheckAll').change(function () {
 
    var $over_time_table = $('#overTimeApprovalTable');
    var is_checked = $(this).is(':checked'),
        checkboxes = $over_time_table.find('tbody input[type="checkbox"]');
    is_checked ? $(checkboxes).prop('checked', true) : $(checkboxes).prop('checked', false);
});

$('#employee_id').blur(function() {

    setFromDateForOt();
});

$('#filter_to').change(function() {
    //filter_to
     
    var fromDateValue = $('#filter_to').val();
   // alert(fromDateValue);
    var afterFormat = moment(fromDateValue, "DD-MMM-YYYY");
    alert(afterFormat.format("dddd"));
    if (afterFormat.format("dddd") == 'Friday') {
        alert('you choosed friday');
    }

});




function saveFinalApprovedData() {
   // alert('called');
    var $table = $('#overTimeApprovalTable2');
   

    var list_elements = {
    table: $('#overTimeApprovalTable2')

    };
    var $over_time_table = $('#overTimeApprovalTable2');
    var table_body = list_elements.table.find("tbody");

    //  selected_rows_temp = $over_time_table.find('tbody input:checked');
    var 
	            selected_employees = $over_time_table.find('tbody input:checked');
    var employees = [], employeesDropdownVal = [],
       selected_employees_length = selected_employees.length;

    if (selected_employees_length == 0) {
        SAXAlert.show({ type: "error", message: "Please select one or more employees" });
    }
    else {

        var return_data = '{"OverTime":[';
       var TotalValuesSelected = '';
       for (var i = 0; i<selected_employees_length; i++) 
        {
         var rowValues = '';


         var chkboxValue = $(selected_employees[i]).val();
        // alert(chkboxValue);
            var dateTxt = chkboxValue + '_txt';
            var date_txt_Value = $('#' + dateTxt + '').val();

           // alert(date_txt_Value);
            var WOP_txt_id = chkboxValue + '_WOP_txt';
            var WOP_txt_Value = $('#' + WOP_txt_id + '').val();

            var WOP_dropDown_Id = chkboxValue + '_WOP_select';
            var WOP_dropDown_Value = $('#' + WOP_dropDown_Id + '').val();
             


            var HP_txt_id = chkboxValue + '_HP_txt';
            var HP_txt_Value = $('#' + HP_txt_id + '').val();
         
            
            var HP_dropDown_Id = chkboxValue + '_HP_select';
            var HP_dropDown_Value = $('#' + HP_dropDown_Id + '').val();

            var rowNumber = Number(chkboxValue) + 1;


            return_data += '{"ID":"' + rowNumber + '", "WOPV":"' + WOP_txt_Value + '","WOC":"' + WOP_dropDown_Value + '", "HPV":"' + HP_txt_Value + '","HPC":"' + HP_dropDown_Value + '"},';
            rowValues = rowNumber + ':' + WOP_txt_Value + ':' + WOP_dropDown_Value + ':' + HP_txt_Value + ':' + HP_dropDown_Value;
            return_data += '';
            if (i == 0)  

             {
                 TotalValuesSelected = rowValues;
             }
             else {
                 TotalValuesSelected =TotalValuesSelected+'||'+'\n'+rowValues;
             }

         }
         return_data += ']}';

        // alert(return_data);
       
        SAXHTTP.AJAX(
          "approve.aspx/DoFinalContinue",
          { current: return_data }
        //{employees: JSON.stringify(final_data), date: date.format("MM-YYYY")}
      ).done(function(data) {

          var status = data.d.status;
          if (status == "success") 
          {
              SAXAlert.show({ type: "success", message: "Overtime approved sucessfully." });
              location.reload();
          }
          else {
              SAXAlert.show({ type: "error", message: "An error occurred while storing data to database." });
          }
          
      })
        .fail(function() {
            SAXAlert.show({ type: "error", message: "An error occurred while Approving first part of the OT. Please try again." });
        });
    }

}

function setFromDateForOt() 
{

    var empid_txt = $('#employee_id').val();
    var companyCode = $('#filter_CompanyCode').val();
    $('#filter_to').prop('disabled', true);
     
    if (empid_txt!='')
{
    SAXHTTP.AJAX(
          "approve.aspx/getFromDate",
          { EmployeeId: empid_txt, companyCode: companyCode }

      ).done(function(data) {

          var status = data.d.status;
          if (status == "success") 
          {
              //SAXAlert.show({ type: "success", message: "Overtime approved sucessfully." });
              var returnDate = data.d.return_data;
             
              // alert(returnDate);
              if (returnDate!='')
              {
                  var fromDate = moment(returnDate).format("DD-MMM-YYYY")
                  $('#filter_from').val(fromDate);
              }
              else
              {
                  SAXAlert.show({ type: "error", message: "There is no pending approval for overtime." });
              }
             
              //making readonly
              $('#filter_from').prop('disabled', true);
              
               var fromDateValue= $('#filter_from').val();
               //setting to date > from date
               $('#filter_to').prop('disabled', false);
               $('#filter_to').Zebra_DatePicker({
                format: 'd-M-Y',
                direction: [fromDateValue, false]
            });

          }
          else if (status == "error") {
              SAXAlert.show({ type: "error", message: "An error occurred while fetching data from database" });
          }
          else if (status == "failure") {
          SAXAlert.show({ type: "error", message: data.d.return_data });
          }
          

      })
        .fail(function() {
            SAXAlert.show({ type: "error", message: "An error occurred while parsing data  " });
        });
    
    }
    else
    {
        SAXAlert.show({ type: "error", message: "Please Enter Employee Id" });

    }
}


function resetPage() {
    var otFirstTable = $("#Ot_Approval_Table_First");
    var otSecondTable = $("#Ot_Approval_Table_Second");
    var filterBtn = $("#filterBtn");


    otFirstTable.removeClass("hide");
    filterBtn.removeClass("hide");
    otSecondTable.addClass("hide");
    $('#overTimeApprovalTable2 tbody').empty();
}

