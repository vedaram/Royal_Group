var SAXPagination = (function ($, w, d) {

    var 
        showPagination = function ($element) {
            $element.removeClass("hide");
        },  
        hidePagination = function ($element) {
            $element.addClass("hide");
        },
        togglePagination = function ($pagination, data_length) {
            switch (data_length) {
            case 30:
                $pagination.removeClass("hide");
                break;
            default:
                $pagination.addClass("hide");
                break;
            }
        };

    return {
        show: showPagination,
        hide: hidePagination,
        toggle: togglePagination
    };

}) (jQuery, window, document);

var SAXHTTP = (function ($, w, d){

    var ajax = function(options) {

        $.ajax({
            method: 'POST',
            url: options.url,
            contentType: 'application/json; charset=utf-8',
            data: JSON.stringify( options.data ),
            dataType: 'json',
            'global': options.global,
            success: function (data) {
                
                if (options.callback) {
                    options.callback( data.d, options.additional);
                }
            },
            error: function(data) {

                SAXAlert.show('error', 'An error occurred while performing this operation. Please try again. If the error persists, please contact Support.');
            }
        });

    },
    AJAX = function(url, data) {
        return $.ajax(url, {method: 'POST', contentType: 'application/json; charset=utf-8', data: JSON.stringify(data), dataType: 'json'});
    };

    return {
        'ajax': ajax,
        'AJAX': AJAX
    };

})(jQuery, window, document);

var SAXAlert = (function ($, w, d){

    var 
    _closeAlert = function () {
        $('body div.alert').remove();
    },
    showAlert = function (options) {

        var alert_HTML = '<div class="alert alert-' + options.type + ' ">' +
                            options.message +
                        '</div>';

        $('body').append( alert_HTML );

        w.setTimeout(_closeAlert, 5000);
    },
    _closeAlertBox = function () {

        $('.alert-box .close').click(function() {
            $(this).parent().remove();
        });
    },
    showAlertBox = function (options) {

        var $body = $('body'),
            alert_box_HTML = '<div class="alert-' + options.type + ' alert-box">' +
                                '<a href="javascript:void(0);" class="close pull-right" data-control="close" data-dismiss="modal" data-target="#alertBox">X</a>' +
                                '<div class="alert-header">' +
                                    '<p>Your Excel Export has been generated successfully!.</p>' +
                                '</div>' +
                                '<div class="alert-body">' +
                                    '<p>Please click the link below to download your Export.</p>' +
                                    '<p><a href="' + options.url + '" target="_blank" class="btn btn-grey">Download Export</a></p>' +
                                '</div>' +
                            '</div>';

        $body.append(alert_box_HTML);

        _closeAlertBox();
    },
    renderAlert = function (data) {
        var alert_HTML = '<div class="alert alert-' + options.type + ' ">' +
                            options.message +
                        '</div>';

        $('body').append( alert_HTML );

        w.setTimeout(_closeAlert, 5000);
    };

    return {
        "render": renderAlert,
        'showAlert': showAlert,
        'show': showAlert,
        'showAlertBox': showAlertBox
    };

})(jQuery, window, document);

var SAXLoader = (function($, w, d) {

    var 
        $body = $('body'), 
        alert;

    var 
        showLoader = function () {

            var HTML = '<div class="loading" id="loading">' + 
                        '</div>';

            alert = $(HTML).appendTo($body);
        },
        closeLoader = function () {
            alert.remove();
        },
        showBlockingLoader = function () {

            var 
                loader_HTML = '<div class="loading" id="blockingLoader">' +
                        '</div>',
                blocker_HTML = '<div class="blocker"></div>';
            // Append the blocker DIV to the body.
            // This will add a semi-transparent layer on top of the viewport, thereby blocking any user interactions.
            blocker = $(blocker_HTML).appendTo($body);
            alert = $(loader_HTML).appendTo($body);
        },
        closeBlockingLoader = function () {

            blocker.remove();
            alert.remove();
        };

    return {
        'show': showBlockingLoader,
        'close': closeBlockingLoader,
        'showBlockingLoader': showBlockingLoader,
        'closeBlockingLoader': closeBlockingLoader
    };

})(jQuery, window, document);

var SAXModel = function (data, options) {

    this.changed = false;
    this.id = 0;
    this.data = {};

    this.set(data, options);
}
_.extend(SAXModel.prototype, {

    idAttribute: 'id',
    get: function (key) {
        return this.data[key];
    },
    set: function (key, value, options) {

        if (key === null) return;

        this.changed = false;

        this._older = _.clone(this.data);

        var older = this._older;

        if (typeof key === 'object') {
            this.data = key;
            options = value;
        } else {
            this.data[key] = value;
        }

        var current = _.clone(this.data);

        if (!_.isEmpty(older) && !_.isEqual(older, current)) this.changed = true;
        
        this.id = this.get(this.idAttribute);
    },
    toJSON: function () {
        return _.clone(this.data);
    },
    hasChanged: function () {
        return this.changed;
    },
    modelID: function () {
        return this.id;
    },
    reset: function () {
        this.changed = false;
        this.id = 0;
        this.data = {};
    }
});

var SAXCollection = function (models, options) {

    this.modelMap = {};

    this.reset();
    this.set(models, options);
};
_.extend(SAXCollection.prototype, {

    baseModel: SAXModel,
    get: function (modelID) {
        return this.modelMap[modelID];
    },
    set: function (models, options) {

        if (models === null) return null;

        var singular = !_.isArray(models);

        var models = singular ? [models] : models,
            models_length = models.length,
            i = 0,
            model = null;

        for (i = 0; i < models_length; i += 1) {
            model = models[i];

            var existing = this.get(model[this.id]);
            
            if (existing) {
                this.modelMap[ existing.idAttribute ] = _.extend(this.modelMap[ existing.idAttribute ], model);
            }
            else {
                model = new this.baseModel(model, options);
                this.modelMap[model.get(model.idAttribute)] = model;
                models[i] = model;
            }
        }

        return singular ? models[0] : models;
    },
    unset: function (modelID) {
        delete this.modelMap[modelID];
        return;
    },
    reset: function () {
        this.modelMap = {};
    },
    toJSON: function () {
        return _.map(this.modelMap, function(model){ return model.toJSON();} );
    }

});

var SAXDialog = function () {

    this.dialogID = _.unique();
    this._bindClose();
};

_.extend(SAXDialog.prototype, {
    // Close button in the dialog, is handled by the Bootstrap JS.
    element: '',
    model: SAXModel,
    operation: '',
    open: function (model, operation) {

        this.element.modal('show');

        if (model instanceof SAXModel) {
            this.model = model;
            this.operation = operation ? operation : '';
        } else {
            this.operation = model;
            this.model = {};
        }
    },
    close: function () {
        this.element.modal('hide');
    },
    get: function () {
        return this.model;
    },
    op: function () {
        return this.operation;
    },
    _bindClose: function() {
        $(this.element).on('hidden.bs.modal', function() {
            this.model = 'SAXModel';
        });
    }

});

/* Helper functions */

// Helper function to correctly set up the prototype chain for subclasses. This has been taken from Backbone.js
// Similar to `goog.inherits`, but uses a hash of prototype properties and
// class properties to be extended.
var extend = function(protoProps, staticProps) {
    var parent = this;
    var child;

    // The constructor function for the new subclass is either defined by you
    // (the "constructor" property in your `extend` definition), or defaulted
    // by us to simply call the parent constructor.
    if (protoProps && _.has(protoProps, 'constructor')) {
      child = protoProps.constructor;
    } else {
      child = function(){ return parent.apply(this, arguments); };
    }

    // Add static properties to the constructor function, if supplied.
    _.extend(child, parent, staticProps);

    // Set the prototype chain to inherit from `parent`, without calling
    // `parent` constructor function.
    var Surrogate = function(){ this.constructor = child; };
    Surrogate.prototype = parent.prototype;
    child.prototype = new Surrogate;

    // Add prototype properties (instance properties) to the subclass,
    // if supplied.
    if (protoProps) _.extend(child.prototype, protoProps);

    // Set a convenience property in case the parent's prototype is needed
    // later.
    child.__super__ = parent.prototype;

    return child;
};

SAXModel.extend = SAXCollection.extend = SAXDialog.extend = extend;

var SAXForms = (function ( $, w, d ){

    /*
        TODO: 
            - validation logic using a JSON object (currently defined on a per page basis)
            - Data attribute for form elements to get additional data from the element. Example: In the select tag, we can get the text and the value of the selected option.
    */

    var _tagName = function ( field ) {

        return field.tagName.toLowerCase()
    },
    _getValue = function ( field, type ) {

        var value = '';

        switch ( type ) {

        case 'input':
            var type_of_input = $(field).attr('type');
            switch (type_of_input) {
            case 'checkbox':
            case 'radio':
                value = $(field).is(':checked') ? 1 : 0;
                break;
            case 'text':
                value = $.trim( $(field).val() );
                break;
            case 'hidden':
                value = $.trim( $(field).val() );
                break;
            }
            break;
        case 'textarea':
            value = $.trim( $(field).val() );
            break;
        case 'select':
            value = $.trim( $(field).find('option:selected').val() );
            break;
        }

        return value;
    },
    get = function ( form ) {
        
        var fields = form.find('input, textarea, select'),
            form_data = {},
            field_name = '';

        $.each(fields, function() {
            
            field_name = $(this).attr('name');

            form_data[field_name] = _getValue( this, _tagName(this) );
        });

        return form_data;
    }
    _setByInputType = function (field, data) {

        var type = $(field).attr('type'),
            id = $(field).attr('id'),
            value = data[id];
        
        switch (type) {

        case 'checkbox':
            if (value === 1 || value === true || value === 'true' || value === '1')
                $(field).prop('checked', true); 
            break;
        case 'radio':
                var name = $(field).attr('name');
                    radio_element = '#' + name + '_' + value;
                $(radio_element).prop('checked', false);
            break;
        case 'text':
            $(field).val(value);
            break;
        case 'hidden':
            $(field).val(value)
            break;
        }
    },
    _setByTagName = function (field, data) {

        var tagName = _tagName(field),
            id = '',
            value = '';

        switch (tagName) {
        case 'input': 
            _setByInputType(field, data);
            break;
        case 'textarea': 
        case 'select':
            id = $(field).attr('id');
            value = data[id];
            if (value != "" || value != undefined || value != "undefined") {
                $(field).val(value);
            }
            break;
        }
    },
    set = function ( form, data ) {

        var fields = form.find('input, textarea, select'),
            tagName = '',
            $field;

        $.each(fields, function() { 
            _setByTagName(this, data);
        });

    },
    enable = function (fields) {

        var counter = 0,
            fields_length = fields.length;

        fields = !(fields instanceof Array) ? [fields] : fields;

        for (counter = 0; counter < fields_length; counter += 1) {
            $('#' + fields[counter]).removeAttr('disabled', '');
        }
    },
    disable = function (fields) {

        var counter = 0,
            fields_length = fields.length;

        fields = !(fields instanceof Array) ? [fields] : fields;

        for (counter = 0; counter < fields_length; counter += 1) {
            $('#' + fields[counter]).attr('disabled', 'disabled');
        }
    };

    return {
        'get': get,
        'set': set,
        'enable': enable,
        'disable': disable
    }

})( jQuery, window, document );

var SAXUtils = (function ($, w, d) {

    var
        getApplicationURL = function () {

            var 
                application_URL = w.location.protocol + '//' + w.location.host,
                application_folder = w.location.pathname.split('/')[1];

            if (application_folder == "ui") {
                application_folder = "";
            } else {
                application_folder = "/" + w.location.pathname.split('/')[1];
            }

            switch (w.location.hostname) {

            case 'localhost':
                application_URL += application_folder + '/exports/data/';
                break;
            default:
                application_URL += application_folder + '/exports/data/';
                break;
            }

            return application_URL;
        };

    return {
        'getApplicationURL': getApplicationURL
    };

})(jQuery, window, document);

var SAXValidation = (function ( $, w, d ) {

    /*
        TODO:
            - Add validations for min and max length
            - Add validations for isNumeric and isCharacter
    */

    function isValidName(data) {
        var re = /^[a-zA-Z0-9_\- ]*$/;

        return re.test(data);
    }

    function isValidCode(data) {
        var re= /^[a-zA-Z0-9_\-]*$/;
        
        return re.test(data);
    }

    function isValidEmployeeCode(data) {
        var re= /^[a-zA-Z0-9]*$/;
        
        return re.test(data);
    }

    function isValidURL(data) {
        var re = /^(http(?:s)?\:\/\/[a-zA-Z0-9\-]+(?:\.[a-zA-Z0-9\-]+)*\.[a-zA-Z]{2,6}(?:\/?|(?:\/[\w\-]+)*)(?:\/?|\/\w+\.[a-zA-Z]{2,4}(?:\?[\w]+\=[\w\-]+)?)?(?:\&[\w]+\=[\w\-]+)*)$/;
        
        return re.test(data);
    }

    function isValidEmail(data) {
         var re = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
        return re.test(data);
    }

    function isNumber(data) {
        return $.isNumeric(data)
    }

    function isValidIP(data) {
        var re = /^(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$/;
        return re.test(data);
    }

    return {
        'code': isValidCode,
        'name': isValidName,
        'url': isValidURL,
        'email': isValidEmail,
        'isNumber': isNumber,
        'ipAddress': isValidIP,
        'employeeCode': isValidEmployeeCode
    };
    
})( jQuery, window, document);