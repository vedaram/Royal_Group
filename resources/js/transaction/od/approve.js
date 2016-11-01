var ODLeavesApproval = (function ($, w, d) {

    var 
        main, _initButtons, _initDialogs, _initOther,
        _getData, _success, _failure,
        _getODLeavesData, _processODLeavesData, _renderODLeavesTable,
        _loadMoreODLeaves,
        _doAction, _processAction, _commentDialog,
        _filterData, _resetFilters,_deleteRow;

    var
        comments_dialog_class, comments_dialog;

    var 
        $od_leave_table = $('#odLeaveApprovalTable'),
        $od_leave_table_parent = $od_leave_table.parent(),
        $od_leaves_no_data = $('#odNoData'),
        $od_leaves_pagination = $('#odPagination').parent(),
        $comments_dialog = $('#approvalCommentDialog'),
        $comments = $('#approvalComment'),
        $filters_box = $('#odLeavesApprovalFilters'),
        $filters_form = $('#odLeavesApprovalFiltersForm'),
        $action_button = $('#approveLeaveButton');

    var 
        selected_action = 0,        
        od_leave_page_number = 1,
        is_filter = false,
        od_leaves_no_data_HTML = '<p><span class="text-orange fa fa-frown-o"></span> <strong>No OD Leaves data found.</strong></p>',
        page_name = 'approve.aspx',
        od_leave_data_URL = page_name + '/GetODLeavesData', 
        company_data_URL = page_name + '/GetCompanyData';

    var 
        status_map = {
            1: "Submitted",
            2: "Approved",
            3: "Declined",
            4: "Cancelled"
        };

    /******************************************************************************************************************/   

    _resetFilters = function () {

        is_filter = false;
        od_leave_page_number=1;

        $filters_form[0].reset();
        $od_leave_table.find('tbody').empty();        

        SAXLoader.show();

        _getODLeavesData();
        
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
            od_leave_page_number = 1;
            
            SAXLoader.show();

            $od_leave_table.find('tbody').empty();
            
            _getODLeavesData();    

            $filters_box.slideToggle();       
        }
    };

    /******************************************************************************************************************/   
 
    _loadMoreODLeaves = function () {

        od_leave_page_number += 1;
        SAXLoader.show();
        _getODLeavesData();
    };

    /******************************************************************************************************************/   

    _processAction = function (data, additional) {

        selected_action = 0;
        _deleteRow(additional.select_rows);
        comments_dialog.close();
        SAXAlert.show({'type': data.status, 'message': data.return_data});
        $action_button.button('reset');
    };
    
    _deleteRow=function(select_rows){
    
     for (var i =0; i < select_rows.length; i++) {
            
            $('#odLeaveApprovalTable tr#' + select_rows[i]).remove();
        }
    }
    
    _doAction = function () {

        var 
            selected_rows = [], selected_rows_length = 0,
            selected_rows_temp = [],
            ajax_options = {},
            comments = $comments.val();

        selected_rows_temp = $od_leave_table.find('tbody input:checked');
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
            'data': {action: selected_action, comments: comments, selected_rows: JSON.stringify(selected_rows)},
            'callback': _processAction,
            'additional': {
            "select_rows":selected_rows
            
            }
        };

        $action_button.button('loading');
        SAXHTTP.ajax(ajax_options);
    };

    
    /******************************************************************************************************************/   

    _renderODLeavesTable = function (data) {

        var 
            data_length = data.length, 
            table_HTML = '',
            counter = 0, remarks, half_day_status;

        for ( counter = 0; counter < data_length; counter += 1) {

            remarks = data[counter]['Remarks'] == null ? '' : data[counter]['Remarks'];
            half_day_status = data[counter]['hl_status'] == 0 ? 'No' : 'Yes';

            table_HTML += '<tr id="' + data[counter]['Leave_id'] + '" >' +
                            '<td><input type="checkbox" id="' + data[counter]['Leave_id'] + '" value="' + data[counter]['Leave_id'] + '"/></td>' +
                            '<td>' + data[counter]['Emp_Code'] + '</td>' +
                            '<td>' + data[counter]['Emp_Name'] + '</td>' +
                            '<td>' + data[counter]['LeaveName'] + '</td>' +
                            '<td>' + moment(data[counter]['FromDate']).format("DD-MMM-YYYY") + '</td>' +
                            '<td>' + moment(data[counter]['ToDate']).format("DD-MMM-YYYY") + '</td>' +
                            '<td>' + half_day_status + '</td>' +
                            '<td>' + remarks + '</td>' +
                            '<td>' + data[counter]['Status'] + '</td>' +
                        '</tr>' ;
        }

        $od_leave_table.detach();
        $od_leave_table.find('tbody').append(table_HTML);
        $od_leave_table_parent.prepend($od_leave_table);  
    };

    _processODLeavesData = function (data, additional) {

        var 
            status = data.status,
            results = {},
            results_length = 0,
            is_no_data = $od_leaves_no_data.hasClass('hide'),
            is_pagination = $od_leaves_pagination.hasClass('hide');

        if (status === 'success') {

            results = JSON.parse(data.return_data);
            results_length = results.length;

            if (results_length > 0) {
                if (!is_no_data) {
                    $od_leaves_no_data.empty();
                    $od_leaves_no_data.addClass('hide');
                }

                _renderODLeavesTable(results);

                results_length === 30 ? $od_leaves_pagination.removeClass('hide') : $od_leaves_pagination.addClass('hide');
            }
            else {

                if (is_no_data) {
                    $od_leaves_no_data.html(od_leaves_no_data_HTML);
                    $od_leaves_no_data.removeClass('hide');
                }

                if (is_pagination) $od_leaves_pagination.addClass('hide');
            }
        }
        else {

            SAXAlert.show({'type': status, 'message': data.return_data});
        }

        if (additional && additional.loading) SAXLoader.close();
    };

    _getODLeavesData = function () {

        var 
            filters_data = SAXForms.get($filters_form),
            ajax_options = {
                'url': od_leave_data_URL,
                'data': {page_number: od_leave_page_number, is_filter: is_filter, filters: JSON.stringify(filters_data)},
                'callback': _processODLeavesData,
                'additional': {
                    'loading': true
                }
            }

        SAXHTTP.ajax(ajax_options);
    };

    

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

        _processODLeavesData(data1[0].d);        
        _processCompanyData(data2[0].d);
        SAXLoader.close();
    };

    _getData = function () {

        var 
            data1 = {page_number: od_leave_page_number, is_filter: is_filter, filters: JSON.stringify({})},
            
            promise1 = $.ajax({url: od_leave_data_URL, type: "POST", contentType: 'application/json; charset=utf-8', dataType: 'json', data: JSON.stringify(data1) }),            
            promise2 = $.ajax({url: company_data_URL, type: "POST", contentType: 'application/json; charset=utf-8', dataType: 'json', data: JSON.stringify() });

        SAXLoader.show();
        $.when(promise1, promise2).then(_success, _failure);
    };

    _initOther = function () {

        $('.date-picker').Zebra_DatePicker({
            format: 'd-M-Y'
        });

        $('#CheckAll').change(function() {
            var is_checked = $(this).is(':checked'),
                checkboxes = $od_leave_table.find('tbody input[type="checkbox"]');
            is_checked ? $(checkboxes).prop('checked', true) : $(checkboxes).prop('checked', false);
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
                'load-more-od-data': function () {
                    _loadMoreODLeaves();                    
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
    ODLeavesApproval.main();
});