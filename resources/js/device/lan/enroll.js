var EnrollCard = (function($, w, d) {

    var main, _init, _initButtons, _initDialogs, _initOther,
        _getEnrolledCards, _processEnrolledCards, _renderTable,
        _getEmployeeDetails, _processEmployeeDetails,
        _saveEnrollment, _processEnrollmentSave, _validate,
        _processNewEnrollment,
        _processEnrollmentEdit, _editDialog,
        _deleteEnrollment, _processEnrollmentDelete, _deleteDialog,
        _loadMoreData;

    var save_dialog_class, save_dialog,
        delete_dialog_class, delete_dialog;

    var enrolled_cards_model_class, 
        enrolled_cards_collection_class, enrolled_cards;

     var 
        $table         = $('#enrollmentTable'),
        $table_parent  = $table.parent(),
        $save_dialog   = $('#saveDialog'),
        $save_button   = $('#saveEnrollmentButton'),
        $save_form     = $('#saveForm'),
        $delete_dialog = $('#deleteDialog'),
        $delete_button = $('#deleteEnrollmentButton'),
        $no_data       = $('#noData'),
        $pagination    = $('#pagination').parent(),
        $loading       = $('#loading'),
        $list_view     = $("#listView");

    var 
        no_data_HTML   = '<p><span class="fa fa-frown-o text-orange"></span> <strong>No Enrollment data found.</strong></p>',
        page_number    = 1,
        button_actions = {},
        fields         = [];

    /******************************************************************************************************************/

    button_actions = {
        'enrollment/add': function () {
            save_dialog.open('add');
        },
        'enrollment/edit': function (event) {
            _editDialog(event);
        },
        'enrollment/save': function () {
            _saveEnrollment();
        },
        'enrollment/delete': function (event) {
            _deleteDialog(event);
        },
        'enrollment/confirm-delete': function () {
            _deleteEnrollment();
        },
        'enrollment/more': function () {
            _loadMoreData();
        }
    };

    /******************************************************************************************************************/

    _loadMoreData = function () {

        page_number += 1;
        SAXLoader.show();
        _getEnrolledCards();
    };

    /******************************************************************************************************************/

    _deleteDialog = function (event) {

        var $element = $(event.target),
            parent = $element.closest('tr'),
            row_id = parent.attr('id'),
            current_model = enrolled_cards.get(row_id);

        delete_dialog.open(current_model, 'delete');
    };

    _processEnrollmentDelete = function (data, additional) {

        var status = data.status,
            message = data.return_data,
            current_model = delete_dialog.get(),
            model_id = current_model.modelID();

        if (status === 'success') {

            $('tr#' + model_id).remove();
            enrolled_cards.unset(model_id);

            delete_dialog.close();
        }

        $delete_button.button('reset');
        SAXAlert.show({'type': status, 'message': message});
    };

    _deleteEnrollment = function () {

        var ajax_options = {},
            current_model = delete_dialog.get(),
            model_data = current_model.toJSON();

        ajax_options = {
            'url': 'enroll.aspx/deleteEnrollment',
            'data': {current: JSON.stringify(model_data)},
            'callback': _processEnrollmentDelete,
            'additional': {}
        };

        $delete_button.button('loading');
        SAXHTTP.ajax(ajax_options);
    }

    /******************************************************************************************************************/

    _editDialog = function (event) {

        var $element = $(event.target),
            parent = $element.closest('tr'),
            row_id = parent.attr('id'),
            current_model = enrolled_cards.get(row_id);

        SAXForms.disable(fields);
        SAXForms.set($save_form, current_model.toJSON());
        save_dialog.open(current_model, 'edit');
    };

    _processEnrollmentEdit = function (data, additional) {

        var status = data.status,
            message = data.return_data,
            current_model = save_dialog.get(),
            model_id = current_model.modelID();

        if (status === 'success') {

            current_model.set(additional.data);

            $('tr#' + model_id).remove();
            _renderTable([additional.data]);

            save_dialog.close();
        }

        $save_button.button('reset');
        SAXAlert.show({'type': status, 'message': message});
    }

    /******************************************************************************************************************/

    _processNewEnrollment = function (data, additional) {

        var status = data.status,
            message = data.return_data,
            is_no_data = $no_data.hasClass('hide');

        if (status === 'success') {

            if (!is_no_data) {
                $no_data.empty();
                $no_data.addClass('hide');
                $list_view.show();
            }

            _renderTable([additional.data]);
            enrolled_cards.set(additional.data);

            save_dialog.close();
        }

        $save_button.button('reset');
        SAXAlert.show({'type': status, 'message': message});
    };

    /******************************************************************************************************************/
    
    _validate = function () {

        var data = SAXForms.get($save_form);

        if (data.Enrollid === '') {
            SAXAlert.show({'type': 'error', 'message': 'Please enter a Enrollment ID.'});
            return false;
        }

        if (data.cardid === '') {
            SAXAlert.show({'type': 'error', 'message': 'Please enter a Card ID.'});
            return false;
        }

        if (!$.isNumeric(data.cardid)) {
            SAXAlert.show({type: "error", message: "Please enter a numeric value for Card ID."});
            return false;
        }

        if (data.pin != "" && !$.isNumeric(data.pin)) {
            SAXAlert.show({type: "error", message: "Please enter a numeric value for PIN number."});
            return false;
        }

        if (data.pin != "" && data.pin.length < 4) {
            SAXAlert.show({'type': 'error', 'message': 'PIN Number cannot be less than 4 digits. Please try again.'});
            return false;
        }

        return true;
    };

    _processEnrollmentSave = function (data, additional) {

        var operation = save_dialog.op();

        switch (operation) {
        case 'add':
            _processNewEnrollment(data, additional);
            break;
        case 'edit':
            _processEnrollmentEdit(data, additional);
            break;
        }
    };

    _saveEnrollment = function () {

        var operation = save_dialog.op(),
            data = SAXForms.get($save_form),
            ajax_options = {};

        if (_validate()) {

            ajax_options = {
                'callback': _processEnrollmentSave,
                'additional': {
                    'data': data
                }
            };

            switch (operation) {
            case 'add':
                ajax_options.url = 'enroll.aspx/addEnrollment';
                ajax_options.data = {current: JSON.stringify(data)};
                break;
            case 'edit':
                ajax_options.url = 'enroll.aspx/editEnrollment';
                ajax_options.data = {current: JSON.stringify(data)};
                break;
            }

            $save_button.button('loading');
            SAXHTTP.ajax(ajax_options);
        }
    };

    /******************************************************************************************************************/  

    _processEmployeeDetails = function (data, additional) {

        var status = data.status,
            results = {};

        if (status === 'success') {

            results = JSON.parse(data.return_data);

            if (results.length > 0) {
                // binding the values to the fields.
                $('#Empid').val(results[0].Emp_Code);
                $('#Name').val(results[0].Emp_Name);
            }

        } else {
            SAXAlert.show({'type': 'error', 'message': data.return_data});
        }

        SAXLoader.close();
    };

    _getEmployeeDetails = function (enrollid) {

        var ajax_options = {};

        ajax_options = {
            'url': 'enroll.aspx/getEmployeeDetails',
            'data': {enroll_id: enrollid},
            'callback': _processEmployeeDetails,
            'additional': {}
        };

        SAXLoader.show();
        SAXHTTP.ajax(ajax_options);
    }  

    /******************************************************************************************************************/

    _renderTable = function (data) {

        var data_length = data.length, 
            table_HTML = '',
            counter = 0;

        for ( counter = 0; counter < data_length; counter += 1) {

            table_HTML += '<tr id="' + data[counter]['Enrollid'] + '" >' +
                            '<td>' + data[counter]['Enrollid'] + '</td>' +
                            '<td>' + data[counter]['cardid'] + '</td>' +
                            '<td>' + data[counter]['pin'] + '</td>' +
                            '<td>' + data[counter]['Empid'] + '</td>' +
                            '<td>' + data[counter]['Name'] + '</td>' +
                            '<td>' + 
                                '<span class="fa fa-pencil text-orange action-icon" data-control="button" data-role="enrollment/edit"></span>' +
                                '<span class="fa fa-trash-o text-red action-icon" data-control="button" data-role="enrollment/delete"></span>' +
                            '</td>' +
                        '</tr>' ;
        }

        $table.detach();
        $table.find('tbody').append( table_HTML );
        $table_parent.prepend( $table );

    };

    _processEnrolledCards = function (data, additional) {

        var status = data.status,
            results = {},
            results_length = 0,
            is_no_data = $no_data.hasClass('hide'),
            is_pagination = $pagination.hasClass('hide');

        if (status === 'success') {

            results = JSON.parse(data.return_data);
            results_length = results.length;

            if (results_length > 0) {

                $list_view.show();

                if (!is_no_data) {
                    $no_data.empty();
                    $no_data.addClass('hide');
                }

                _renderTable(results);
                enrolled_cards.set(results);

                results_length === 30 ? $pagination.removeClass('hide') : $pagination.addClass('hide');
            }
            else {

                $list_view.hide();

                if (is_no_data) {
                    $no_data.html(no_data_HTML);
                    $no_data.removeClass('hide');
                }

                if (is_pagination) $pagination.addClass('hide');
            }
        }
        else {

            SAXAlert.show({'type': 'error', 'message': data.return_data});
        }

        SAXLoader.close();
    };

    _getEnrolledCards = function () {

        var ajax_options = {};

        ajax_options = {
            'url': 'enroll.aspx/getEnrolledCards',
            'data': {page_number: page_number},
            'callback': _processEnrolledCards,
            'additional': {}
        };

        SAXHTTP.ajax(ajax_options);
    };

    /******************************************************************************************************************/

    _initOther = function () {

        $('#Enrollid').on('change', function() {
            var enrollid = $(this).val();
            _getEmployeeDetails(enrollid);
        });
    };

    _initDialogs = function () {

        save_dialog_class = SAXDialog.extend({
            'element': $save_dialog
        });
        save_dialog = new save_dialog_class();

        delete_dialog_class = SAXDialog.extend({
            'element': $delete_dialog
        });
        delete_dialog = new delete_dialog_class();

        $save_dialog.on('hidden.bs.modal', function() {
            SAXForms.enable(fields);
            $save_form[0].reset();
        });
    };

    _initButtons = function () {

        var role = '';

        $('body').on('click', '[data-control="button"]', function(event) {
            role = $(event.target).data('role');
            button_actions[role].call(this, event);
        });
    };

    _init  = function () {

        enrolled_cards_model_class = SAXModel.extend({
            'idAttribute': 'Enrollid'
        });

        enrolled_cards_collection_class = SAXCollection.extend({
            'baseModel': enrolled_cards_model_class
        });
        enrolled_cards = new enrolled_cards_collection_class([]);
    };

    main = function () {

        SAXLoader.show();

        _getEnrolledCards();

        _init();
        _initButtons();
        _initDialogs();

        _initOther();
    };

    return {
        'main': main
    };

})(jQuery, window, document);

$(function() {
    EnrollCard.main();
});