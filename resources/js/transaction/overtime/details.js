var OvertimeDetails = (function ($, w, d) {

    var 
        main, _initButtons, _initOther,
        _getData, _success, _failure,
        _getOverTimeData, _processOverTimeData, _renderOverTimeTable,
        _loadMoreOverTime,        
        _filterData, _resetFilters;

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
        page_name = 'details.aspx',
        over_time_data_URL = page_name + '/getOvertimeData';
        
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
        over_time_page_number=1;

        $filters_form[0].reset();
        $over_time_table.find('tbody').empty();        

        SAXLoader.show();
        $filters_box.slideToggle();
        _getOverTimeData();
        
    };

    _validateFilters = function () {

        var 
            data = SAXForms.get($filters_form);
        
//         if (data['filter_date'] == '') {
//            SAXAlert.show({'type': 'error', 'message': 'Please select a OT Date.'});
//            return false;
//        }
//       
//        if (data['filter_hours'] == '' ) {
//            SAXAlert.show({'type': 'error', 'message': 'Please select a OT Hours.'});
//            return false;
//        }
        
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
            over_time_page_number = 1;
            
            SAXLoader.show();
            $filters_box.slideToggle();
            $over_time_table.find('tbody').empty();
            
            _getOverTimeData();           
        }
    };

    /******************************************************************************************************************/   
 
    _loadMoreOverTime = function () {

        over_time_page_number += 1;
        SAXLoader.show();
        _getOverTimeData();
    };

   
    /******************************************************************************************************************/   

    _renderOverTimeTable = function (data) {

        var 
            data_length = data.length, 
            table_HTML = '',
            counter = 0,modifiedby;

        for ( counter = 0; counter < data_length; counter += 1) {

            modifiedby = data[counter]['modifiedby'] == null ? '' : data[counter]['modifiedby'];

            table_HTML += '<tr id="' + data[counter]['Overtimeid'] + '" >' +
                           
                            '<td>' + data[counter]['EmpID'] + '</td>' +
                            '<td>' + data[counter]['Emp_Name'] + '</td>' +
                            '<td>' + moment(data[counter]['OTDate']).format("DD-MMM-YYYY") + '</td>' +
                            '<td>' + moment(data[counter]['OtHrs'], 'HH:mm').format("HH:mm") + '</td>' +
                            '<td>' + status_map[data[counter]['Approval']] + '</td>' +
                            '<td>' + modifiedby + '</td>' +
                        '</tr>';
        }

        $over_time_table.detach();
        $over_time_table.find('tbody').append(table_HTML);
        $over_time_table_parent.prepend($over_time_table);  
    };

    _processOverTimeData = function (data, additional) {

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

            SAXAlert.show({'type': status, 'message': data.return_data});
        }

        if (additional && additional.loading) SAXLoader.close();
    };

    _getOverTimeData = function () {

        var 
            filters_data = SAXForms.get($filters_form),
            ajax_options = {
                'url': over_time_data_URL,
                'data': {page_number: over_time_page_number, is_filter: is_filter, filters: JSON.stringify(filters_data)},
                'callback': _processOverTimeData,
                'additional': {
                    'loading': true
                }
            }

        SAXHTTP.ajax(ajax_options);
    };
    
    /******************************************************************************************************************/

    _failure = function () {

        SAXLoader.close();
        SAXAlert.show({'type': 'error', 'message': 'An error occurred while loading data for the page. Please try again. If the error persists, please contact Support.'});
    };

    _success = function (data1) {
        _processOverTimeData(data1.d);  
        SAXLoader.close();
    };

    _getData = function () {

        var 
            data1 = {page_number: over_time_page_number, is_filter: is_filter, filters: JSON.stringify({})},            
            promise1 = $.ajax({url: over_time_data_URL, type: "POST", contentType: 'application/json; charset=utf-8', dataType: 'json', data: JSON.stringify(data1) });

        SAXLoader.show();
        $.when(promise1).then(_success, _failure);
    };

    _initOther = function () {

        $('.date-picker').Zebra_DatePicker({
            format: 'd-M-Y'
        });
        
        $('.time-picker').timepicki({
            show_meridian: false,
            overflow_minutes: true,
            min_hour_value: 0,
            max_hour_value: 23
        });      
       
    };
   
    _initButtons = function () {

        var role = '',
            button_actions = {
                'load-more-ot-data': function () {
                    _loadMoreOverTime();                    
                },                
                'toggle-filters': function () {
                    $filters_box.slideToggle();
                },
                'filter-data': function () {
                    _filterData();
                },
                'reset-filters': function () {
                    _resetFilters();
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
       

        _initOther();
    };

    return {
        'main': main
    };

})(jQuery, window, document);

$(function() {
    OvertimeDetails.main();
});