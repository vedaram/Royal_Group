var LeavesApproval = (function ($, w, d) {

    var 
        main, _initButtons, _initDialogs, _initOther,
        _getData, _success, _failure,
        _getNormalLeavesData, _processNormalLeavesData, _renderNormalLeavesTable,
        _getLWPLeavesData, _processLWPLeavesData, _renderLWPLeavesTable,
        _loadMoreNormalLeaves, _loadMoreLWPLeaves,
        _doAction, _processAction, _commentDialog,
        _filterData, _resetFilters,_deleteRow;

    var
        comments_dialog_class, comments_dialog;

    var 
        $normal_leave_table = $('#leaveApprovalTable'),
        $normal_leave_table_parent = $normal_leave_table.parent(),
        $normal_leaves_no_data = $('#normalNoData'),
        $normal_leaves_pagination = $('#normalPagination').parent(),
        $lwp_leave_table = $('#lwpApprovalTable'),
        $lwp_leave_table_parent = $lwp_leave_table.parent(),
        $lwp_leaves_no_data = $('#lwpNoData'),
        $lwp_leaves_pagination = $('#lwpPagination').parent(),
        $comments_dialog = $('#approvalCommentDialog'),
        $comments = $('#approvalComment'),
        $filters_box = $('#leavesApprovalFilters'),
        $filters_form = $('#leavesApprovalFiltersForm'),
        $tablist = $('.tablist'),
        $action_button = $('#approveLeaveButton');

    var 
        selected_action = 0,
        normal_leave_page_number = 1,
        lwp_leave_page_number = 1,
        is_filter = false,
        lwp_leaves_no_data_HTML = '<p><span class="text-orange fa fa-frown-o"></span> <strong>No LWP Leaves data found.</strong></p>',
        normal_leaves_no_data_HTML = '<p><span class="text-orange fa fa-frown-o"></span> <strong>No Normal Leaves data found.</strong></p>',
        page_name = 'approve.aspx',
        normal_leave_data_URL = page_name + '/GetNormalLeavesData',
        lwp_leave_data_URL = page_name + '/GetLWPLeavesData',
        leave_type_data_URL = page_name + '/GetLeaveTypeData',
        company_data_URL = page_name + '/GetCompanyData';

    var 
        status_map = {
            1: "Submitted",
            2: "Approved",
            3: "Declined",
            4: "Cancelled",
            5: "Submitted"
        };

    /******************************************************************************************************************/   

    _resetFilters = function () {

        is_filter = false;
        normal_leave_page_number = 1;
        lwp_leave_page_number = 1;

        $filters_form[0].reset();
        $normal_leave_table.find('tbody').empty();
        $lwp_leave_table.find('tbody').empty();

        SAXLoader.show();

        _getNormalLeavesData();
        _getLWPLeavesData();
        $filters_box.slideToggle();
    };

    _validateFilters = function () {

        var 
            data = SAXForms.get($filters_form);

        if (data['filter_from'] != '' && data['filter_to'] == '') {
            SAXAlert.show({'type': 'error', 'message': 'Please select a To Date.'});
            return false;
        }

        if (data['filter_to'] != '' && data['filter_from'] == '') {
            SAXAlert.show({'type': 'error', 'message': 'Please select a From Date.'});
            return false;
        }

        if (moment(data['filter_from']) > moment(data['filter_to']) ) {
            SAXAlert.show({'type': 'error', 'message': 'From Date cannot be greater than To Date.'});
            return false;
        }

        if (data['filter_by'] != 0) {

            if (data['filter_keyword'] == '') {
                SAXAlert.show({'type': 'error', 'message': 'Please enter a keyword.'});
                return false;
            }
        }

        if (data['filter_keyword'] != '') {

            if (data['filter_by'] == 0) {
                SAXAlert.show({'type': 'error', 'message': 'Please select a Filter By condition.'});
                return false;
            }
        }

        return true;
    };

    _filterData = function () {

        if (_validateFilters()) {

            is_filter = true;
            normal_leave_page_number = 1;
            lwp_leave_page_number = 1;
            
            SAXLoader.show();

            $normal_leave_table.find('tbody').empty();
            $lwp_leave_table.find('tbody').empty();

            _getNormalLeavesData();
            _getLWPLeavesData();

            $filters_box.slideToggle();
        }
    };

    /******************************************************************************************************************/   

    _loadMoreLWPLeaves = function () {

        lwp_leave_page_number += 1;
        SAXLoader.show();
        _getLWPLeavesData();
    };

    _loadMoreNormalLeaves = function () {

        normal_leave_page_number += 1;
        SAXLoader.show();
        _getNormalLeavesData();
    };

    /******************************************************************************************************************/   

    _processAction = function (data, additional) {

        selected_action = 0;
        _deleteRow(additional.selected_tab, additional.selected_rows);
        comments_dialog.close();
        SAXAlert.show({'type': data.status, 'message': data.return_data});
        $action_button.button('reset');
    };
    
    _deleteRow=function(selected_tab, selected_rows){
       for (var i =0; i < selected_rows.length; i++) {
       
       if(selected_tab=='leave'){
            
            $('#leaveApprovalTable tr#' + selected_rows[i]).remove();
          }
       if(selected_tab=='lwp'){
           
            $('#lwpApprovalTable tr#' + selected_rows[i]).remove();
          }
       }
    
    };
   
    _doAction = function () {

        var 
            selected_rows = [], selected_rows_length = 0,
            selected_rows_temp = [],
            selected_tab = $tablist.find('li.active a'),
            selected_tab_id = selected_tab.data('id'),
            selected_tab_target = selected_tab.data('target'),
            ajax_options = {},
            comments = $comments.val();

        selected_rows_temp = $(selected_tab_target + ' tbody').find('input:checked');
        selected_rows_temp_length = selected_rows_temp.length;

        if (selected_rows_temp_length === 0) {
            SAXAlert.show({'type': 'error', 'message': 'Please select atleast one row.'});
            return false;
        }

        for (var i = 0; i < selected_rows_temp_length; i++) {
            selected_rows.push( $(selected_rows_temp[i]).val());
        };

        ajax_options = {
            'url': page_name + '/DoAction',
            'data': {selected_tab: selected_tab_id, action: selected_action, comments: comments, selected_rows: JSON.stringify(selected_rows)},
            'callback': _processAction,
            'additional': {
                "selected_tab": selected_tab_id,
                "selected_rows": selected_rows
            }
        };

        $action_button.button('loading');
        SAXHTTP.ajax(ajax_options);
    };

    /******************************************************************************************************************/   

    _renderLWPLeavesTable = function (data) {

        var 
            data_length = data.length, 
            table_HTML = '',
            counter = 0, remarks, half_day_status;

        for ( counter = 0; counter < data_length; counter += 1) {

            remarks = data[counter]['ReasonForLeave'] == null ? '' : data[counter]['ReasonForLeave'];
            half_day_status = data[counter]['hl_status'] == 0 ? 'No' : 'Yes';

            table_HTML += '<tr id="' + data[counter]['leave_id'] + '" >' +
                            '<td><input type="checkbox" id="' + data[counter]['leave_id'] + '"  value="' + data[counter]['leave_id'] + '"/></td>' +
                            '<td>' + data[counter]['empid'] + '</td>' +
                            '<td>' + data[counter]['emp_name'] + '</td>' +
                            '<td>' + data[counter]['leavename'] + '</td>' +
                            '<td>' + moment(data[counter]['From']).format("DD-MMM-YYYY") + '</td>' +
                            '<td>' + moment(data[counter]['To']).format("DD-MMM-YYYY") + '</td>' +
                            '<td>' + half_day_status + '</td>' +
                            '<td>' +  remarks + '</td>' +
                            '<td>' + status_map[data[counter]['flag']] + '</td>' +
                        '</tr>' ;
        }

        $lwp_leave_table.detach();
        $lwp_leave_table.find('tbody').append(table_HTML);
        $lwp_leave_table_parent.prepend($lwp_leave_table);  
    };

    _processLWPLeavesData = function (data, additional) {

        var 
            status = data.status,
            results = {},
            results_length = 0,
            is_no_data = $lwp_leaves_no_data.hasClass('hide'),
            is_pagination = $lwp_leaves_pagination.hasClass('hide');
              
              
        if (status === 'success') {

            results = JSON.parse(data.return_data);
            results_length = results.length;

            if (results_length > 0) {
                if (!is_no_data) {
                    $lwp_leaves_no_data.empty();
                    $lwp_leaves_no_data.addClass('hide');
                }

                _renderLWPLeavesTable(results);

                results_length === 30 ? $lwp_leaves_pagination.removeClass('hide') : $lwp_leaves_pagination.addClass('hide');
            }
            else {

                if (is_no_data) {
                    $lwp_leaves_no_data.html(lwp_leaves_no_data_HTML);
                    $lwp_leaves_no_data.removeClass('hide');
                }

                if (is_pagination) $lwp_leaves_pagination.addClass('hide');
            }
        }
        else {

            SAXAlert.show({'type': status, 'message': data.return_data});
        }

        if (additional && additional.loading) SAXLoader.close();
    };

    _getLWPLeavesData = function () {

        var 
            filters_data = SAXForms.get($filters_form),
            ajax_options = {
                'url': lwp_leave_data_URL,
                'data': {page_number: lwp_leave_page_number, is_filter: is_filter, filters: JSON.stringify(filters_data)},
                'callback': _processLWPLeavesData,
                'additional': {
                    'loading': true
                }
            }

        SAXHTTP.ajax(ajax_options);
    };

    /******************************************************************************************************************/   

    _renderNormalLeavesTable = function (data) {

        var 
            data_length = data.length, 
            table_HTML = '',
            counter = 0, remarks, half_day_status,approvedby;

        for ( counter = 0; counter < data_length; counter += 1) {

            remarks = data[counter]['ReasonForLeave'] == null ? '' : data[counter]['ReasonForLeave'];
            approvedby=data[counter]['ApprovedbyName'] == null ? '' : data[counter]['ApprovedbyName'];
            half_day_status = data[counter]['hl_status'] == 0 ? 'No' : 'Yes';

            table_HTML += '<tr id="' + data[counter]['leave_id'] + '" >' +
                            '<td><input type="checkbox" id="' + data[counter]['leave_id'] + '" value="' + data[counter]['leave_id'] + '"/></td>' +
                            '<td>' + data[counter]['empid'] + '</td>' +
                            '<td>' + data[counter]['emp_name'] + '</td>' +
                            '<td>' + data[counter]['leavename'] + '</td>' +
                            '<td>' + moment(data[counter]['From']).format("DD-MMM-YYYY") + '</td>' +
                            '<td>' + moment(data[counter]['To']).format("DD-MMM-YYYY") + '</td>' +
                            '<td>' + half_day_status + '</td>' +
                            '<td>' +  remarks + '</td>' +
                            '<td>' + status_map[data[counter]['flag']] + '</td>' +
                             '<td>' + approvedby + '</td>' +
                            
                        '</tr>' ;
        }

        $normal_leave_table.detach();
        $normal_leave_table.find('tbody').append(table_HTML);
        $normal_leave_table_parent.prepend($normal_leave_table);  
    };

    _processNormalLeavesData = function (data, additional) {

        var 
            status = data.status,
            results = {},
            results_length = 0,
            is_no_data = $normal_leaves_no_data.hasClass('hide'),
            is_pagination = $normal_leaves_pagination.hasClass('hide');

        if (status === 'success') {

            results = JSON.parse(data.return_data);
            results_length = results.length;

            if (results_length > 0) {
                if (!is_no_data) {
                    $normal_leaves_no_data.empty();
                    $normal_leaves_no_data.addClass('hide');
                }

                _renderNormalLeavesTable(results);

                results_length === 30 ? $normal_leaves_pagination.removeClass('hide') : $normal_leaves_pagination.addClass('hide');
            }
            else {

                if (is_no_data) {
                    $normal_leaves_no_data.html(normal_leaves_no_data_HTML);
                    $normal_leaves_no_data.removeClass('hide');
                }

                if (is_pagination) $normal_leaves_pagination.addClass('hide');
            }
        }
        else {

            SAXAlert.show({'type': status, 'message': data.return_data});
        }

        if (additional && additional.loading) SAXLoader.close();
    };

    _getNormalLeavesData = function () {

        var 
            filters_data = SAXForms.get($filters_form),
            ajax_options = {
                'url': normal_leave_data_URL,
                'data': {page_number: normal_leave_page_number, is_filter: is_filter, filters: JSON.stringify(filters_data)},
                'callback': _processNormalLeavesData,
                'additional': {
                    'loading': true
                }
            }

        SAXHTTP.ajax(ajax_options);
    };

    /******************************************************************************************************************/    

    _renderLeaveTypeDropdown = function (data) {

        var 
            select_HTML = '<option value="select">Select Leave Type</option>',
            data_length = data.length,
            counter = 0,
            $element = $('#filter_LeaveType'),
            $parent = $element.parent();

        for (counter = 0; counter < data_length; counter += 1) {
            
            select_HTML += '<option value="' + data[counter]['LeaveName'] + '">' + data[counter]['LeaveName'] + ' (' + data[counter]['LeaveCode'] + ')</option>';
        }

        $element.append(select_HTML);
    };

    _processLeaveTypeData = function (data) {

        var status = data.status,
            results = {};

        if (status === 'success') { 

            results = JSON.parse(data.return_data);
            _renderLeaveTypeDropdown(results);
        }
        else {
            SAXAlert.show({ 'type': 'error', 'message': data.return_data});
        }

        SAXLoader.closeBlockingLoader();
    };

    _loadLeaveTypesData = function (company_code) {

        var 
            ajax_options = {
                'url': page_name + '/GetLeaveTypeData',
                'data': {company_code: company_code},
                'callback': _processLeaveTypeData,
                'additional': {}
            }

        SAXLoader.showBlockingLoader();
        SAXHTTP.ajax(ajax_options);
    }

    /******************************************************************************************************************/

    _renderCompanyDropdown = function (data) {

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

    _processCompanyData = function (data) {

        var status = data.status,
            results = {};

        if (status === 'success') { 

            results = JSON.parse(data.return_data);
            _renderCompanyDropdown(results);
        }
        else {
            SAXAlert.show({ 'type': 'error', 'message': data.return_data});
        }
    };

    /******************************************************************************************************************/

    _failure = function () {

        SAXLoader.close();
        SAXAlert.show({'type': 'error', 'message': 'An error occurred while loading data for the page. Please try again. If the error persists, please contact Support.'});
    };

    _success = function (data1, data2, data3) {

        _processNormalLeavesData(data1[0].d);
        _processLWPLeavesData(data2[0].d);
        _processCompanyData(data3[0].d);

        SAXLoader.close();
    };

    _getData = function () {

        var 
            data1 = {page_number: normal_leave_page_number, is_filter: is_filter, filters: JSON.stringify({})},
            data2 = {page_number: lwp_leave_page_number, is_filter: is_filter, filters: JSON.stringify({})},
            promise1 = $.ajax({url: normal_leave_data_URL, type: "POST", contentType: 'application/json; charset=utf-8', dataType: 'json', data: JSON.stringify(data1) }),
            promise2 = $.ajax({url: lwp_leave_data_URL, type: "POST", contentType: 'application/json; charset=utf-8', dataType: 'json', data: JSON.stringify(data2) }),
            promise3 = $.ajax({url: company_data_URL, type: "POST", contentType: 'application/json; charset=utf-8', dataType: 'json', data: JSON.stringify() });

        SAXLoader.show();
        $.when(promise1, promise2, promise3).then(_success, _failure);
    };

    _initOther = function () {

        $('.date-picker').Zebra_DatePicker({
            format: 'd-M-Y'
        });

        $('#normalCheckAll').change(function() {
            var is_checked = $(this).is(':checked'),
                checkboxes = $normal_leave_table.find('tbody input[type="checkbox"]');
            is_checked ? $(checkboxes).prop('checked', true) : $(checkboxes).prop('checked', false);
        });

        $('#lwpCheckAll').change(function() {
            var is_checked = $(this).is(':checked'),
                checkboxes = $lwp_leave_table.find('tbody input[type="checkbox"]');
            is_checked ? $(checkboxes).prop('checked', true) : $(checkboxes).prop('checked', false);
        });

        $('#filter_CompanyCode').change(function() {
            _loadLeaveTypesData($(this).val());
        });
    };

    _initDialogs = function () {

        comments_dialog_class = SAXDialog.extend({
            'element': $comments_dialog
        });
        comments_dialog = new comments_dialog_class();

        $comments_dialog.on('hidden.bs.modal', function() {
            $comments.val('');
        })
    };

    _initButtons = function () {

        var role = '',
            button_actions = {
                'load-more-lwp-data': function () {
                    _loadMoreLWPLeaves();
                },
                'load-more-normal-data': function () {
                    _loadMoreNormalLeaves();
                },
                'toggle-filters': function () {
                    $filters_box.slideToggle();
                },
                'filter-data': function () {
                    _filterData();
                },
                'reset-filters': function () {
                    _resetFilters();
                },
                'action-button-click': function (event) {
                    selected_action = $(event.target).data('operation');
                    comments_dialog.open();
                },
                'confirm-approval': function () {
                    _doAction();
                }
            };

        $('body').on('click', '[data-control="button"]', function(event) {
            role = $(event.target).data('role');
            button_actions[role].call(this, event);
        });
    };

    main = function () {

        _getData();
        _initButtons();
        _initDialogs();

        _initOther();
    };

    return {
        'main': main
    };

})(jQuery, window, document);

$(function() {
    LeavesApproval.main();
});