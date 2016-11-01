var CompOffDetails = (function($, w, d) {

    var
        main, _initOther, _initButtons,
        _getData, _success, _failure,
        _getCompOffData, _processCompOffData,
        _loadMoreCompOffData,
        _filterData, _resetFilters;


    var 
        $Comp_Off_table = $('#CompoffTable'),
        $Comp_Off_table_parent = $Comp_Off_table.parent(),
        $Comp_Off_no_data = $('#CompoffNoData'),
        $Comp_Off_pagination = $('#CompoffPagination').parent(),

        $filters_box = $('#CompoffDetailsFilters'),
        $filters_form = $('#CompoffFiltersForm');


    var 
        selected_action = 0,
        Comp_Off_page_number = 1,
        is_filter = false,

        Comp_Off_no_data_HTML = '<p><span class="text-orange fa fa-frown-o"></span> <strong>No Comp Off Details data found.</strong></p>',
        page_name = 'details.aspx',
        Comp_Off_data_URL = page_name + '/GetCompOffData';

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
        Comp_Off_page_number = 1;

        $filters_form[0].reset();
        $Comp_Off_table.find('tbody').empty();
        $filters_box.slideToggle();
        SAXLoader.show();
        _getCompOffData();

    };

    _validateFilters = function() {

        var 
            data = SAXForms.get($filters_form);

        if (data['filter_indate'] != '' && data['filter_outdate'] == '') {
            SAXAlert.show({ 'type': 'error', 'message': 'Please select a To Date.' });
            return false;
        }

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
            Comp_Off_page_number = 1;

            SAXLoader.show();

            $Comp_Off_table.find('tbody').empty();
            $filters_box.slideToggle();
            _getCompOffData();
        }
    };

    /******************************************************************************************************************/

    _loadMoreCompOffData = function() {

        Comp_Off_page_number += 1;
        SAXLoader.show();
        _getCompOffData();
    };

    /******************************************************************************************************************/

    _renderCompOffTable = function(data) {

        var 
            data_length = data.length,
            table_HTML = '',
            counter = 0, ApprovedByName;

        for (counter = 0; counter < data_length; counter += 1) {

            ApprovedByName = data[counter]['ApprovedByName'] == null ? '' : data[counter]['ApprovedByName'];

            table_HTML += '<tr id="' + data[counter]['compoff_id'] + '" >' +
                            '<td>' + data[counter]['EmpID'] + '</td>' +
                            '<td>' + data[counter]['Emp_Name'] + '</td>' +
                            '<td>' + moment(data[counter]['fromdate']).format("DD-MMM-YYYY") + '</td>' +
                            '<td>' + moment(data[counter]['todate']).format("DD-MMM-YYYY") + '</td>' +
                             '<td>' + status_map[data[counter]['Flag']] + '</td>' +
                              '<td>' + ApprovedByName + '</td>' +
                        '</tr>';
        }

        $Comp_Off_table.detach();
        $Comp_Off_table.find('tbody').append(table_HTML);
        $Comp_Off_table_parent.prepend($Comp_Off_table);
    };

    _processCompOffData = function(data, additional) {

        var 
            status = data.status,
            results = {},
            results_length = 0,
            is_no_data = $Comp_Off_no_data.hasClass('hide'),
            is_pagination = $Comp_Off_pagination.hasClass('hide');

        if (status === 'success') {

            results = JSON.parse(data.return_data);
            results_length = results.length;

            if (results_length > 0) {
                if (!is_no_data) {
                    $Comp_Off_no_data.empty();
                    $Comp_Off_no_data.addClass('hide');
                }

                _renderCompOffTable(results);

                results_length === 30 ? $Comp_Off_pagination.removeClass('hide') : $Comp_Off_pagination.addClass('hide');
            }
            else {

                if (is_no_data) {
                    $Comp_Off_no_data.html(Comp_Off_no_data_HTML);
                    $Comp_Off_no_data.removeClass('hide');
                }

                if (is_pagination) $Comp_Off_pagination.addClass('hide');
            }
        }
        else {

            SAXAlert.show({ 'type': status, 'message': data.return_data });
        }

        if (additional && additional.loading) SAXLoader.close();
    };

    _getCompOffData = function() {

        var 
            filters_data = SAXForms.get($filters_form),
            ajax_options = {
                'url': Comp_Off_data_URL,
                'data': { page_number: Comp_Off_page_number, is_filter: is_filter, filters: JSON.stringify(filters_data) },
                'callback': _processCompOffData,
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
        _processCompOffData(data1.d);
        SAXLoader.close();
    };

    _initButtons = function() {

        var role = '',
            button_actions = {

                'toggle-filters': function() {
                    $filters_box.slideToggle();
                },
                 'filter-data': function() {
                    _filterData();
                },                
                'reset-filters': function() {
                    _resetFilters();
                },                
                'load-more-Comp-off-data': function() {
                _loadMoreCompOffData();
                }
            };

        $('body').on('click', '[data-control="button"]', function(event) {
            role = $(event.target).data('role');
            button_actions[role].call(this, event);
        });
    };
    
    _getData = function() {

        var 
            data1 = { page_number: Comp_Off_page_number, is_filter: is_filter, filters: JSON.stringify({}) },
            promise1 = $.ajax({ url: Comp_Off_data_URL, type: "POST", contentType: 'application/json; charset=utf-8', dataType: 'json', data: JSON.stringify(data1) });

        SAXLoader.show();
        $.when(promise1).then(_success, _failure);
    };

    _initOther = function() {

        $('.datepicker').Zebra_DatePicker({
            format: 'd-M-Y'
        });

        $('#CompoffCheckAll').change(function() {
            var is_checked = $(this).is(':checked'),
                checkboxes = $Comp_Off_table.find('tbody input[type="checkbox"]');
            is_checked ? $(checkboxes).prop('checked', true) : $(checkboxes).prop('checked', false);
        });

    };

    main = function() {

        _getData();
        _initButtons();
        _initOther();
    }

    return {
        'main': main
    };

})(jQuery, window, document);

$(function() {
CompOffDetails.main();
});