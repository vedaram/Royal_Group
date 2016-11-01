var masterPage = (function($, w, d) {
	
	var 
		main, _initMenu,
		_toggleMenu;

	var 
		$currently_open = '';

	_toggleMenu = function () {

		var 
			$this = $(this),
			menu = $this.data('labelledby');
		
		$currently_open = $('.menu.open');

		if (menu === '#home') {
			_cacheMenuItem('', '#home');
			window.location.href = '../System/HomePage.aspx';
		}
		else {
			if (menu === '#' + $currently_open.attr('id')) {
				$(menu).removeClass('open');
				$this.removeClass('menu-active');
			}
			else {
				$('#' + $currently_open.attr('id')).removeClass('open');
				
				$currently_open.removeClass('open');
				$('.menu-active').removeClass('menu-active');

				$(menu).addClass('open');
				$this.addClass('menu-active');
			}
		}
	};

	_cacheMenuItem = function (menu_item, parent) {
		
		sessvars.current_screen = {
        	'current_menu_item': menu_item,
        	'current_section': parent
        };
	};

	_setActiveMenuItem = function () {

		if (sessvars.current_screen) { 
			$(sessvars.current_screen.current_section).addClass('active');
		}
		else {
			$('#home').addClass('active');
		}
	};

	_initMenu = function () {

		var 
			current_item_parent = '',
			current_item = '';

		$('.menu-level-0 li.menu-item').click(_toggleMenu);
		$('.menu-level-1 li.menu-item').click(function(event) {
			current_item = $(event.target).attr('id');
			current_item_parent = $(event.target).closest('.menu').data('parent');
			_cacheMenuItem(current_item, current_item_parent);
		});

		_setActiveMenuItem();
	};

	_menu = function () {
		if (sessvars.TAMS_ENV.user_details.user_name == "admin") {
			$(".sidebar > .menu").show();
		}
		else {
			$(".sidebar > .menu").empty().show().append(sessvars.TAMS_ENV.menu);
		}
	};

	main = function () {
		//_menu();
	};

	return {
		'main': main
	};

})(jQuery, window, document);

$(function () {
	masterPage.main();
});