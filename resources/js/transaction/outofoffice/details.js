var OutOffOfficeDetails = (function($, w, d) {

    var 
        main, _initButtons, _initDialogs, _initOther,
        _getData, _success, _failure,
        _getOutOffOfficeData, _processOutOffOfficeData, _renderOutOffOfficeTable,
        _loadMoreOutOffOfficeData,
        _doAction, _processAction, _commentDialog,
        _filterData, _resetFilters;

    var 
        comments_dialog_class, comments_dialog;

    var 
        $Out_Off_Office_table = $('#OutOffOfficeTable'),
        $Out_Off_Office_table_parent = $Out_Off_Office_table.parent(),
        $Out_Off_Office_no_data = $('#OutOffOfficeNoData'),
        $Out_Off_Office_pagination = $('#OutOffOfficePagination').parent(),

        $comments_dialog = $('#approvalCommentDialog'),
        $comments = $('#approvalComment'),
        $filters_box = $('#OutOffOfficeApprovalFilters'),
        $filters_form = $('#OutOffOfficeFiltersForm'),

        $action_button = $('#ApproveOutOffOfficeButton');

    var 
        selected_action = 0,
        Out_Off_Office_page_number = 1,
        is_filter = false,

        Out_Off_Office_no_data_HTML = '<p><span class="text-orange fa fa-frown-o"></span> <strong>No Out Off Office data found.</strong></p>',
        page_name = 'details.aspx',
        Out_Off_Office_data_URL = page_name + '/GetOutOffOfficeData';

    var 
        status_map = {
            1: "Submitted",
            2: "Approved",
            3: "Rejected"
        };

    /******************************************************************************************************************/

    _resetFilters = function() {

        is_filter = false;
        Out_Off_Office_page_number = 1;

        $filters_form[0].reset();
        $Out_Off_Office_table.find('tbody').empty();

        SAXLoader.show();

        $filters_box.slideToggle();
        _getOutOffOfficeData();

    };

    _validateFilters = function() {

        var 
            data = SAXForms.get($filters_form);

//        if (data['filter_indate'] != '' && data['filter_outdate'] == '') {
//            SAXAlert.show({ 'type': 'error', 'message': 'Please select a To Date.' });
//            return false;
//        }

        if (data['filter_outdate'] != '' && data['filter_indate'] == '') {
            SAXAlert.show({ 'type': 'error', 'message': 'Please select a From Date.' });
            return false;
        }

        if (moment(data['filter_indate']) > moment(data['filter_outdate'])) {
            SAXAlert.show({ 'type': 'error', 'message': 'From Date cannot be greater than To Date.' });
            return false;
        }

        if (data['filter_by'] != 0) {

            if (data['filter_keyword'] == '') {
                SAXAlert.show({ 'type': 'error', 'message': 'Please enter a keyword.' });
                return false;
            }
        }

        if (data['filter_keyword'] != '') {

            if (data['filter_by'] == 0) {
                SAXAlert.show({ 'type': 'error', 'message': 'Please select a Filter By condition.' });
                return false;
            }
        }

        return true;
    };

    _filterData = function() {

        if (_validateFilters()) {
            is_filter = true;
            Out_Off_Office_page_number = 1;

            SAXLoader.show();

            $Out_Off_Office_table.find('tbody').empty();
            $filters_box.slideToggle();

            _getOutOffOfficeData();
        }
    };

    /******************************************************************************************************************/

    _loadMoreOutOffOfficeData = function() {

        Out_Off_Office_page_number += 1;
        SAXLoader.show();
        _getOutOffOfficeData();
    };


    /******************************************************************************************************************/

    _renderOutOffOfficeTable = function(data) {

    var row_colors = {
        init: '#4D4D4D',
        red: '#c0392b'
    };
    var row_style = {
        normal: 'normal',
        bold: 'bold'
    };

    var 
            data_length = data.length,
            table_HTML = '',
            row_class = row_colors.init,
           row_class_style = row_style.normal,
        counter = 0, remarks, Modifiedby, Manager_Remark;
    var user_access_level = sessvars.TAMS_ENV.user_details.user_access_level;

    if (user_access_level == 1 || user_access_level == 2) {
        hideTd();
    }

    for (counter = 0; counter < data_length; counter += 1) {

        remarks = data[counter]['Reason'] == null ? '' : data[counter]['Reason'];
        Manager_Remark = data[counter]['Manager_Remark'] == null ? '' : data[counter]['Manager_Remark'];

        row_class = data[counter]['OOO_Type'] == 'Personal' && data[counter]['TotalHours'] > '03:00' ? row_colors.red : row_colors.init;

        row_class_style = data[counter]['Status'] == '1' ? row_style.bold : row_style.normal;

        table_HTML += '<tr style="color:' + row_class + '; font-weight:' + row_class_style + ' " id="' + data[counter]['OOO_ID'] + '" >' +
                            
                            '<td>' + data[counter]['Emp_ID'] + '</td>' +
                            '<td>' + data[counter]['Emp_Name'] + '</td>' +
                            '<td>' + data[counter]['OOO_Type'] + '</td>' +
                            '<td>' + moment(data[counter]['FromDate']).format("DD-MMM-YYYY") + '</td>' +
                            '<td>' + data[counter]['FromTime'] + '</td>' +
                            '<td>' + moment(data[counter]['ToDate']).format("DD-MMM-YYYY") + '</td>' +
                            '<td>' + data[counter]['ToTime'] + '</td>' +
                            '<td>' + data[counter]['Hours'] + '</td>' +
                            '<td>' + data[counter]['TotalHours'] + '</td>';

        if (user_access_level == 3 || user_access_level == 0) {
            table_HTML += '<td>' + data[counter]['Manager_ID'] + '</td>' +
                            '<td>' + Manager_Remark + '</td>';
        }

        table_HTML += '<td>' + remarks + '</td>' +
                            '<td>' + status_map[data[counter]['Status']] + '</td>' +
                        '</tr>';
    }

        $Out_Off_Office_table.detach();
        $Out_Off_Office_table.find('tbody').append(table_HTML);
        $Out_Off_Office_table_parent.prepend($Out_Off_Office_table);
    };

    _processOutOffOfficeData = function(data, additional) {

        var 
            status = data.status,
            results = {},
            results_length = 0,
            is_no_data = $Out_Off_Office_no_data.hasClass('hide'),
            is_pagination = $Out_Off_Office_pagination.hasClass('hide');

        if (status === 'success') {

            results = JSON.parse(data.return_data);
            results_length = results.length;

            if (results_length > 0) {
                if (!is_no_data) {
                    $Out_Off_Office_no_data.empty();
                    $Out_Off_Office_no_data.addClass('hide');
                }

                _renderOutOffOfficeTable(results);

                results_length === 30 ? $Out_Off_Office_pagination.removeClass('hide') : $Out_Off_Office_pagination.addClass('hide');
            }
            else {

                if (is_no_data) {
                    $Out_Off_Office_no_data.html(Out_Off_Office_no_data_HTML);
                    $Out_Off_Office_no_data.removeClass('hide');
                }

                if (is_pagination) $Out_Off_Office_pagination.addClass('hide');
            }
        }
        else {

            SAXAlert.show({ 'type': status, 'message': data.return_data });
        }

        if (additional && additional.loading) SAXLoader.close();
    };

    _getOutOffOfficeData = function() {

        var 
            filters_data = SAXForms.get($filters_form),
            ajax_options = {
                'url': Out_Off_Office_data_URL,
                'data': { page_number: Out_Off_Office_page_number, is_filter: is_filter, filters: JSON.stringify(filters_data) },
                'callback': _processOutOffOfficeData,
                'additional': {
                    'loading': true
                }
            }

        SAXHTTP.ajax(ajax_options);
    };

    /******************************************************************************************************************/

    _failure = function() {

        SAXLoader.close();
        SAXAlert.show({ 'type': 'error', 'message': 'An error occurred while loading data for the page. Please try again. If the error persists, please contact Support.' });
    };

    _success = function(data1) {
        _processOutOffOfficeData(data1.d);
        SAXLoader.close();
    };

    _getData = function() {

        var 
            data1 = { page_number: Out_Off_Office_page_number, is_filter: is_filter, filters: JSON.stringify({}) },
            promise1 = $.ajax({ url: Out_Off_Office_data_URL, type: "POST", contentType: 'application/json; charset=utf-8', dataType: 'json', data: JSON.stringify(data1) });

        SAXLoader.show();
        $.when(promise1).then(_success, _failure);
    };

    _initOther = function() {

        $('.date-picker').Zebra_DatePicker({
            format: 'M-Y'
        });

    };

//    _initDialogs = function() {

//        comments_dialog_class = SAXDialog.extend({
//            'element': $comments_dialog
//        });
//        comments_dialog = new comments_dialog_class();

//        $comments_dialog.on('hidden.bs.modal', function() {
//            $comments.val('');
//        })
//    };

    _initButtons = function() {

        var role = '',
            button_actions = {

                'load-more-Out_Off_Office-data': function() {
                    _loadMoreOutOffOfficeData();
                },
                'toggle-filters': function() {
                    $filters_box.slideToggle();
                },
                'filter-data': function() {
                    _filterData();
                },
                'reset-filters': function() {
                    _resetFilters();
                }
//                ,
//                'action-button-click': function(event) {
//                    selected_action = $(event.target).data('operation');
//                    comments_dialog.open();
//                }
            };

        $('body').on('click', '[data-control="button"]', function(event) {
            role = $(event.target).data('role');
            button_actions[role].call(this, event);
        });
    };

    main = function() {

        _getData();
        _initButtons();
        //_initDialogs();

        _initOther();
    }

    return {
        'main': main
    };

})(jQuery, window, document);

$(function() {
    OutOffOfficeDetails.main();
});

function hideTd() {

    $('#HRC1').hide();
    $('#HRC2').hide();
}
