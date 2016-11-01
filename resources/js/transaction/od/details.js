var ODLeavesDetails = (function ($, w, d) {

    var 
        main, _initButtons, _initDialogs, _initOther,
        _getData, _success, _failure,
        _getODLeavesData, _processODLeavesData, _renderODLeavesTable,
        _loadMoreODLeaves, _filterData, _resetFilters;

    var
        comments_dialog_class, comments_dialog;

    var 
        $od_leave_table = $('#odLeaveDetailsTable'),
        $od_leave_table_parent = $od_leave_table.parent(),
        $od_leaves_no_data = $('#odNoData'),
        $od_leaves_pagination = $('#odPagination').parent(),        
        $filters_box = $('#odLeavesDetailsFilters'),
        $filters_form = $('#odLeavesDetailsFiltersForm');
        
    var             
        od_leave_page_number = 1,
        is_filter = false,        
        od_leaves_no_data_HTML = '<p><span class="text-orange fa fa-frown-o"></span> <strong>No OD Leaves data found.</strong></p>',        
        page_name = 'details.aspx',
        od_leave_data_URL = page_name + '/GetODLeavesData',        
        leave_type_data_URL = page_name + '/GetLeaveTypeData',
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

    _renderODLeavesTable = function (data) {

        var 
            data_length = data.length, 
            table_HTML = '',
            counter = 0, Remarks, ApprovedbyName;

        for ( counter = 0; counter < data_length; counter += 1) {
            ApprovedbyName = data[counter]['ApprovedbyName'] == null ? '' : data[counter]['ApprovedbyName'];        
            Remarks = data[counter]['Remarks'] == null ? '' : data[counter]['Remarks'];        
            
            table_HTML += '<tr id="' + data[counter]['Leave_id'] + '" >' +                           
                            '<td>' + data[counter]['Emp_Code'] + '</td>' +
                            '<td>' + data[counter]['Emp_Name'] + '</td>' +
                            '<td>' + data[counter]['LeaveName'] + '</td>' +
                            '<td>' + moment(data[counter]['FromDate']).format("DD-MMM-YYYY") + '</td>' +
                            '<td>' + moment(data[counter]['ToDate']).format("DD-MMM-YYYY") + '</td>' +  
                            '<td>' + Remarks + '</td>' +
                            '<td>' + data[counter]['Status'] + '</td>' +
                            '<td>' + ApprovedbyName + '</td>' +
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

    _success = function (data1, data2) {

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
    ODLeavesDetails.main();
});