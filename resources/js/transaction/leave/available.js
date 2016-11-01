var LeavesAvailabe = (function ($, w, d) {

    var 
        main, _initButtons, _initDialogs, _initOther,
        _getData, _success, _failure,
        _getLeavesAvailableData, _renderLeaveAvailable,
        _doAction, _processAction, _commentDialog,
        _doExport, _processExport;

    var
        comments_dialog_class, comments_dialog;

    var 
        $leave_available_table = $('#leavesAvailableTable')
        $leave_available_table_parent = $leave_available_table.parent(),
        $leave_available_no_data = $('#leaveAvailableNoData'),
        $leave_available_pagination = $('#leaveAvailablePagination').parent(),                
        $action_button = $('#LeaveExportButton'),
        $export_button=$('#exportButton');

    var         
        leave_available_page_number = 1,        
        leave_available_no_data_HTML = '<p><span class="text-orange fa fa-frown-o"></span> <strong>No Leaves data found.</strong></p>',        
        page_name = 'available.aspx',
        leave_available_data_URL = page_name + '/GetLeavesAvailable';       

    /******************************************************************************************************************/   
    _processExport = function (data, additional) {

        var 
            status = data.status,
            type_of_export = additional.type_of_export;

        switch (status) {

        case 'success':
            SAXAlert.showAlertBox({'type': status, 'url': SAXUtils.getApplicationURL() + data.return_data});
            break;
        case 'info':
            SAXAlert.show({'type': status, 'message': data.return_data});
            break;
        case 'error':
            SAXAlert.show({'type': status, 'message': data.return_data});
            break;
        }
        
        $export_button.button('reset');
        SAXLoader.closeBlockingLoader();
	};    
    
    _doExport = function (event) {
    
     var 
            ajax_options = {},
            type_of_export = $(event.target).data('role');
        
        ajax_options = {
            'url': page_name + '/DoExport',
            'data': {},
            'callback': _processExport,
            'additional': {
            'type_of_export': type_of_export
            }
        };
        
        $export_button.button('loading');
        SAXLoader.showBlockingLoader();
        SAXHTTP.ajax(ajax_options);
    
    };
    
    /******************************************************************************************************************/   

    _loadMoreLeavesAvailabe=function(){
        leave_available_page_number+=1;
        SAXLoader.show();
        _getLeavesAvailableData();
    };
    
    
    /******************************************************************************************************************/   

    _processAction = function (data, additional) {

        selected_action = 0;

        comments_dialog.close();
        SAXAlert.show({'type': data.status, 'message': data.return_data});
        $action_button.button('reset');
    };

   

    /******************************************************************************************************************/   

    _renderLeaveAvailable = function (data) {

        var 
            data_length = data.length, 
            table_HTML = '',
            counter = 0;

        for ( counter = 0; counter < data_length; counter += 1) {
            
            table_HTML += '<tr id="' + data[counter]['row'] + '" >' +
                            '<td>' + data[counter]['Emp_Code'] + '</td>' +
                            '<td>' + data[counter]['Emp_Name'] + '</td>' +
                            '<td>' + data[counter]['LeaveName'] + '</td>' +
                            '<td>' + data[counter]['Max_leaves'] + '</td>' +
                            '<td>' + data[counter]['Leaves_applied'] + '</td>' +
                            '<td>' + data[counter]['Leave_balance'] + '</td>' +
                        '</tr>' ;
        }

        $leave_available_table.detach();
        $leave_available_table.find('tbody').append(table_HTML);
        $leave_available_table_parent.prepend($leave_available_table);  
    };

    _processLeavesAvailable = function (data, additional) {

        var 
            status = data.status,
            results = {},
            results_length = 0,
            is_no_data = $leave_available_no_data.hasClass('hide'),
            is_pagination = $leave_available_pagination.hasClass('hide');

        if (status === 'success') {

            results = JSON.parse(data.return_data); 
            results_length = results.length;

            if (results_length > 0) {
                if (!is_no_data) {
                    $leave_available_no_data.empty();
                    $leave_available_no_data.addClass('hide');
                }

                _renderLeaveAvailable(results);

                results_length === 30 ? $leave_available_pagination.removeClass('hide') : $leave_available_pagination.addClass('hide');
            }
            else {

                if (is_no_data) {
                    $leave_available_no_data.html(leave_available_no_data_HTML);
                    $leave_available_no_data.removeClass('hide');
                }

                if (is_pagination) $leave_available_pagination.addClass('hide');
            }
        }
        else {

            SAXAlert.show({'type': status, 'message': data.return_data});
        }

        if (additional && additional.loading) SAXLoader.close();
    };

    
    _getLeavesAvailableData = function()
    {
     var 
           
            ajax_options = {
                'url': leave_available_data_URL,
                'data': {page_number: leave_available_page_number},
                'callback': _processLeavesAvailable,
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

        _processLeavesAvailable(data1.d);

        SAXLoader.close();
    };

    _getData = function () {

        var 
            data1 = {page_number: leave_available_page_number},            
            promise1 = $.ajax({url: leave_available_data_URL, type: "POST", contentType: 'application/json; charset=utf-8', dataType: 'json', data: JSON.stringify(data1) });
            
        SAXLoader.show();
        $.when(promise1).then(_success, _failure);
    };    


    _initButtons = function () {

        var role = '',
            button_actions = {
                'load-more-leaves-available-data': function () {                    
                    _loadMoreLeavesAvailabe();                    
                },
                "leave-available/export": function (event) {
                    _doExport(event);
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
        
    };

    return {
        'main': main
    };

})(jQuery, window, document);

$(function() {
    LeavesAvailabe.main();
});