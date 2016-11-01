var ReprocessData = (function($, w, d) {

    var 
        main, _initButtons, _initOther,
        _renderDropdown,
        _getCompanyData, _processCompanyData,
        _getOtherData, _processOtherData,
        _renderTable,
        _filterData, _processFilterData, _validateFilters,
        _loadMoreData,
        _doReprocess, _processReprocess;

    var 
        $company = $('#filter_company'),
        $branch = $('#filter_branch'),
        $department = $('#filter_department'),
        $designation = $('#filter_designation'),
        $filters_form = $('#reprocessDataFiltersForm'),
        $filters_button = $('#filterDataButton'),
        $reprocess_form = $('#reprocessDataForm'),
        $reprocess_button = $('#reprocessDataButton'),
        $reprocess_container = $('#reprocessContainer'),
        $table = $('#reprocessDataTable'),
        $table_parent = $table.parent(),
        $no_data = $('#noData'),
        $pagination = $('#pagination').parent();

    var 
        page_number = 1,
        page_name = 'reprocess.aspx',
        no_data_HTML = '';

    _processReprocess = function(data, additional) {

        var 
            status = data.status
        message = data.return_data;

        $reprocess_button.button('reset');
        SAXLoader.closeBlockingLoader();
        SAXAlert.show({ 'type': status, 'message': message });
    };

    _doReprocess = function() {

        var ajax_options = {},
            selected_employees = $table.find('tbody input:checked'),
            employees = [],
            selected_employees_length = selected_employees.length,
            data = SAXForms.get($reprocess_form),
            from_date, to_date;

        if (selected_employees_length === 0) {
            SAXAlert.show({ 'type': 'error', 'message': 'Please select at least one Employee.' });
            return false;
        }

        if (data['from_date'] === '' || data['to_date'] === '') {
            SAXAlert.show({ 'type': 'error', 'message': 'Please select From & To date.' });
            return false;
        }

        if (moment(data["from_date"], "DD-MM-YYYY").unix() > moment(data["to_date"], "DD-MM-YYYY").unix()) {
            SAXAlert.show({ type: "error", message: "From Date cannot be greater than To Date" });
            return false;
        }

        for (var i = 0; i < selected_employees_length; i++) {
            employees.push($(selected_employees[i]).val());
        };

        ajax_options = {
            'url': page_name + '/DoReprocess',
            'data': { employees: JSON.stringify(employees), from_date: data['from_date'], to_date: data['to_date'] },
            'callback': _processReprocess,
            'additional': {
                'data': employees
            }
        };

        $reprocess_button.button('loading');

        SAXLoader.showBlockingLoader();
        SAXHTTP.ajax(ajax_options);
    };

    /******************************************************************************************************************/

    _loadMoreData = function() {

        page_number += 1;
        _filterData();
    }

    _renderTable = function(data) {

        var data_length = data.length,
            table_HTML = '',
            counter = 0;

        for (counter = 0; counter < data_length; counter += 1) {

            table_HTML += '<tr id="' + data[counter]['emp_code'] + '" class="case" >' +
                            '<td><input type="checkbox" id="' + data[counter]['emp_code'] + '" value="' + data[counter]['emp_code'] + '" /></td>' +
                            '<td>' + data[counter]['emp_code'] + '</td>' +
                            '<td>' + data[counter]['emp_name'] + '</td>' +
                        '</tr>';
        }

        $table.detach();
        $table.find('tbody').append(table_HTML);
        $table_parent.prepend($table);
    };

    _processFilterData = function(data, additional) {

        var 
            status = data.status,
            results = {},
            results_length = 0,
            is_no_data = $no_data.hasClass('hide'),
            is_pagination = $no_data.hasClass('hide');

        if (status === 'success') {

            results = JSON.parse(data.return_data);
            results_length = results.length;

            if (results_length > 0) {

                if (!is_no_data) {
                    $no_data.empty();
                    $no_data.addClass('hide');
                }

                _renderTable(results);
                $reprocess_container.removeClass('hide');
                results_length === 30 ? $pagination.removeClass('hide') : $pagination.addClass('hide');
            }
            else {

                if (is_no_data) {
                    $no_data.html(no_data_HTML);
                    $no_data.removeClass('hide');
                }

                $reprocess_container.addClass('hide');

                if (!is_pagination) $pagination.addClass('hide');
            }

        }
        else {
            SAXAlert.show({ 'type': 'error', 'message': data.return_data });
        }

        SAXLoader.closeBlockingLoader();
    };

    _validateFilters = function() {

        var 
            data = SAXForms.get($filters_form);

        if (data.filter_company == "select") {
            SAXAlert.show({ 'type': 'error', 'message': 'Please select a Company.' });
            return false;
        }

        if (data.filter_keyword != "" && data.filter_by == 0) {
            SAXAlert.show({ 'type': 'error', 'message': 'Please select a Filter By option.' });
            return false;
        }

        if (data.filter_by != 0 && data.filter_keyword == "") {
            SAXAlert.show({ 'type': 'error', 'message': 'Please enter a keyword.' });
            return false;
        }

        return true;
    };

    _filterData = function() {

        var 
            data = SAXForms.get($filters_form),
            ajax_options = {};

        if (_validateFilters()) {

            ajax_options = {
                'url': page_name + '/GetReprocessData',
                'data': { page_number: page_number, filters: JSON.stringify(data) },
                'callback': _processFilterData,
                'additional': {}
            };

            SAXLoader.showBlockingLoader();
            SAXHTTP.ajax(ajax_options);
        }
    };

    /******************************************************************************************************************/

    _processOtherData = function(data, additional) {

        var 
            status = data.status,
            results = {},
            branch_data = {},
            department_data = {},
            designation_data = {};

        if (status === 'success') {

            results = JSON.parse(data.return_data);

            // Rendering the results in a dropdown.            
            branch_data = results['branch'];
            _renderDropdown($branch, branch_data, 'BranchCode', 'BranchName', 'All Branches', 'No data found');
            // Enable the dropdown after rendering the data
            $branch.prop('disabled', false);

            // Rendering the results in a dropdown.
            department_data = results['department'];
            _renderDropdown($department, department_data, 'DeptCode', 'DeptName', 'All Departments', 'No data found');
            // Enable the dropdown after rendering the data
            $department.prop('disabled', false);

            // Rendering the results in a dropdown.
            designation_data = results['designation'];
            _renderDropdown($designation, designation_data, 'DesigCode', 'DesigName', 'All Designations', 'No data found');
            // Enable the dropdown after rendering the data
            $designation.prop('disabled', false);
        }
        else {
            SAXAlert.show({ 'type': 'error', 'message': data.return_data });
        }

        SAXLoader.closeBlockingLoader();
    };

    _getOtherData = function(company_code) {

        var 
            ajax_options = {
                'url': page_name + '/GetOtherData',
                'data': { company_code: company_code },
                'callback': _processOtherData,
                'additional': {}
            };

        SAXLoader.showBlockingLoader();
        SAXHTTP.ajax(ajax_options);
    };

    /******************************************************************************************************************/

    _processCompanyData = function(data, additional) {

        var 
            status = data.status,
            results = {};

        if (status === 'success') {
            results = JSON.parse(data.return_data);
            _renderDropdown($company, results, 'CompanyCode', 'CompanyName', 'Select a Company', 'No data found');
        }
        else {
            SAXAlert.show({ 'type': 'error', 'message': data.return_data });
        }

        SAXLoader.closeBlockingLoader();
    };

    _getCompanyData = function() {

        var 
            ajax_options = {
                'url': page_name + '/GetCompanyData',
                'data': {},
                'callback': _processCompanyData,
                'additional': {}
            };

        SAXLoader.showBlockingLoader();
        SAXHTTP.ajax(ajax_options);
    };

    /******************************************************************************************************************/

    _renderDropdown = function($element, data, key, value, default_text, no_data) {

        var 
            select_HTML = '',
            data_length = data.length,
            counter = 0,
            $parent = $element.parent();

        if (data_length > 0) {

            select_HTML = '<option value="select">' + default_text + '</option>';

            for (counter = 0; counter < data_length; counter += 1) {
                select_HTML += '<option value="' + data[counter][key] + '">' + data[counter][value] + '</option>';
            }
        }
        else {
            select_HTML = '<option value="select">No data found</option>';
        }

        $element.detach();
        $element.empty().append(select_HTML);
        $parent.append($element);
        $element.prop('disabled', false);
    };

    /******************************************************************************************************************/

    _initOther = function() {

        $('.date-picker').Zebra_DatePicker({
            'format': 'd-m-Y'
        });

        $company.change(function() {
            _getOtherData($(this).val());
        });

        $('[data-toggle="popover"]').popover({
            html: true
        })
    };

    _initButtons = function() {

        var 
            role = '',
            button_actions = {
                'reprocess-data': function() {
                    _doReprocess();
                },
                'filter-data': function() {
                    /* clear the contents of the table as we are assuming the user has selected new filter. */
                    $table.find('tbody').empty();
                    // reset the page number based on the above assumption.
                    page_number = 1;
                    _filterData();
                },
                'load-more-data': function() {
                    _loadMoreData();
                }
            };

        $('body').on('click', '[data-control="button"]', function(event) {
            event.preventDefault();
            role = $(event.target).data('role');
            button_actions[role].call(this, event);
        });

        $("#checkall").change(function() {
            var is_checked = $(this).is(':checked');
            checkboxes = $table.find('tbody input[type="checkbox"]');
            is_checked ? $(checkboxes).prop('checked', true) : $(checkboxes).prop('checked', false);
        });

        $('body').on('click', 'tr.case', function() {
            var checkboxes = $table.find('tbody input:checked');
            $(".case").length == checkboxes.length ? $("#checkall").prop("checked", true) : $("#checkall").prop("checked", false);
        });
    };

    $("#filter_company").on('change', function() {
        $("#filterDataButton").click(function() {
            $("#checkall").prop("checked", false);
        });
    });

    main = function() {
        _getCompanyData();
        _initButtons();
        _initOther();
    };
    

    return {
        'main': main
    };

})(jQuery, window, document);

$(function () {
    ReprocessData.main();
}); 