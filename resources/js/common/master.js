var Menu = (function($, w, d) {
	
	_cacheMenuItem = function () {
		sessvars.current_screen = {
        	'current_menu_item': menu_item,
        	'current_section': parent
        };
	};

})(jQuery, window, document);

$(docuemnt).ready(function () {
	Master.main();
});