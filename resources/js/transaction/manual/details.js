var ManualPunchDetails = (function ($, w, d) {

    var 
        main, _initButtons,  _initOther,
        _getData, _success, _failure,
        _getManualPunchData, _processManualPunchData, _renderManualPunchTable,
        _loadMoreManualPunchData,       
        _filterData, _resetFilters;

    var
        comments_dialog_class, comments_dialog;

    var 
        $Manual_Punch_table = $('#ManualPunchTable'),
        $Manual_Punch_table_parent = $Manual_Punch_table.parent(),
        $Manual_Punch_no_data = $('#ManuaPunchNoData'),
        $Manual_Punch_pagination = $('#ManualPunchPagination').parent(), 
        $comments_dialog = $('#approvalCommentDialog'),
        $comments = $('#approvalComment'),
        $filters_box = $('#ManualpunchApprovalFilters'),
        $filters_form = $('#ManualpunchFiltersForm');  

    var 
        selected_action = 0,
        Manual_punch_page_number = 1,
        is_filter = false,
        
        Manual_punch_no_data_HTML = '<p><span class="text-orange fa fa-frown-o"></span> <strong>No Manual Punch data found.</strong></p>',
        page_name = 'details.aspx',
        Manual_punch_data_URL = page_name + '/GetManualPunchData';
                
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
        Manual_punch_page_number = 1;
        
        $filters_form[0].reset();
        $Manual_Punch_table.find('tbody').empty();
        
        SAXLoader.show();

        $filters_box.slideToggle();
        
        _getManualPunchData();
        
    };

    _validateFilters = function () {

        var 
            data = SAXForms.get($filters_form);

        if (data['filter_indate'] != '' && data['filter_outdate'] == '') {
            SAXAlert.show({'type': 'error', 'message': 'Please select a To Date.'});
            return false;
        }

        if (data['filter_outdate'] != '' && data['filter_indate'] == '') {
            SAXAlert.show({'type': 'error', 'message': 'Please select a From Date.'});
            return false;
        }

        if (moment(data['filter_indate']) > moment(data['filter_outdate']) ) {
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
            Manual_punch_page_number = 1;
                        
            SAXLoader.show();

            $Manual_Punch_table.find('tbody').empty();

            $filters_box.slideToggle();
            
            _getManualPunchData();
          }
    };

    /******************************************************************************************************************/   

    _loadMoreManualPunchData = function () {

        Manual_punch_page_number += 1;
        SAXLoader.show();
        _getManualPunchData();
    };
   
    /******************************************************************************************************************/   

    _renderManualPunchTable = function (data) {

        var 
            data_length = data.length, 
            table_HTML = '',
            counter = 0, remarks, Modifiedby;

        for ( counter = 0; counter < data_length; counter += 1) {

            remarks = data[counter]['ReasonForManualPunch'] == null ? '' : data[counter]['ReasonForManualPunch'];
            Manager_remark = data[counter]['Manager_remark'] == null ? '' : data[counter]['Manager_remark'];
            Hr_remark = data[counter]['Hr_remark'] == null ? '' : data[counter]['Hr_remark'];
            Modifiedby = data[counter]['Modifiedby'] == null ? '' : data[counter]['Modifiedby'];
            
            table_HTML += '<tr id="' + data[counter]['id'] + '" >' +
                           
                            '<td>' + data[counter]['Empcode'] + '</td>' +
                            '<td>' + data[counter]['Emp_Name'] + '</td>' +                            
                            '<td>' + moment(data[counter]['WorkDate']).format("DD-MMM-YYYY") + '</td>' +
                            '<td>' + moment(data[counter]['outdate']).format("DD-MMM-YYYY") + '</td>' +
                            '<td>' + moment(data[counter]['InPunch']).format("HH:mm:ss") + '</td>' +
                            '<td>' + moment(data[counter]['OutPunch']).format("HH:mm:ss") + '</td>' +                            
                            '<td>' + remarks + '</td>' +
                            '<td>' + Manager_remark + '</td>' +
                            '<td>' + Hr_remark + '</td>' +
                            //'<td>' + status_map[data[counter]['approve']] + '</td>' +
                            '<td>' + data[counter]['Leave_Status_text'] + '</td>' +
                            '<td>' + Modifiedby + '</td>' +
                        '</tr>' ;
        }

        $Manual_Punch_table.detach();
        $Manual_Punch_table.find('tbody').append(table_HTML);
        $Manual_Punch_table_parent.prepend($Manual_Punch_table);  
    };

    _processManualPunchData = function (data, additional) {

        var 
            status = data.status,
            results = {},
            results_length = 0,
            is_no_data = $Manual_Punch_no_data.hasClass('hide'),
            is_pagination = $Manual_Punch_pagination.hasClass('hide');

        if (status === 'success') {

            results = JSON.parse(data.return_data);
            results_length = results.length;

            if (results_length > 0) {
                if (!is_no_data) {
                    $Manual_Punch_no_data.empty();
                    $Manual_Punch_no_data.addClass('hide');
                }

                _renderManualPunchTable(results);

                results_length === 30 ? $Manual_Punch_pagination.removeClass('hide') : $Manual_Punch_pagination.addClass('hide');
            }
            else {

                if (is_no_data) {
                    $Manual_Punch_no_data.html(Manual_punch_no_data_HTML);
                    $Manual_Punch_no_data.removeClass('hide');
                }

                if (is_pagination) $Manual_Punch_pagination.addClass('hide');
            }
        }
        else {

            SAXAlert.show({'type': status, 'message': data.return_data});
        }

        if (additional && additional.loading) SAXLoader.close();
    };

    _getManualPunchData = function () {

        var 
            filters_data = SAXForms.get($filters_form),
            ajax_options = {
                'url': Manual_punch_data_URL,
                'data': {page_number: Manual_punch_page_number, is_filter: is_filter, filters: JSON.stringify(filters_data)},
                'callback': _processManualPunchData,
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
        _processManualPunchData(data1.d);
        SAXLoader.close();
    };

    _getData = function () {

        var 
            data1 = {page_number: Manual_punch_page_number, is_filter: is_filter, filters: JSON.stringify({})},            
            promise1 = $.ajax({url: Manual_punch_data_URL, type: "POST", contentType: 'application/json; charset=utf-8', dataType: 'json', data: JSON.stringify(data1) });
            
        SAXLoader.show();
        $.when(promise1).then(_success, _failure);
    };

    _initOther = function () {

        $('.datepicker').Zebra_DatePicker({
            format: 'd-M-Y'
        });
        
    };

    
    _initButtons = function () {

        var role = '',
            button_actions = {
               
                'load-more-manual-punch-data': function () {
                    _loadMoreManualPunchData();
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
    }

    return {
        'main': main
    };

})(jQuery, window, document);

$(function() {
    ManualPunchDetails.main();
});