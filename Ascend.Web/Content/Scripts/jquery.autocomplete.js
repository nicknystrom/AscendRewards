/**
*    Json key/value autocomplete for jQuery 
*    Provides a transparent way to have key/value autocomplete
*    Copyright (C) 2008 Ziadin Givan www.CodeAssembly.com  
*
*    This program is free software: you can redistribute it and/or modify
*    it under the terms of the GNU Lesser General Public License as published by
*    the Free Software Foundation, either version 3 of the License, or
*    (at your option) any later version.
*
*    This program is distributed in the hope that it will be useful,
*    but WITHOUT ANY WARRANTY; without even the implied warranty of
*    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
*    GNU General Public License for more details.
*
*    You should have received a copy of the GNU Lesser General Public License
*    along with this program.  If not, see http://www.gnu.org/licenses/
*    
*    Examples 
*	 $("input#example").autocomplete("autocomplete.php");//using default parameters
*	 $("input#example").autocomplete("autocomplete.php",{minChars:3,timeout:3000,validSelection:false,parameters:{'myparam':'myvalue'},before : function(input,text) {},after : function(input,text) {}});
*    minChars = Minimum characters the input must have for the ajax request to be made
*	 timeOut = Number of miliseconds passed after user entered text to make the ajax request   
*    validSelection = If set to true then will invalidate (set to empty) the value field if the text is not selected (or modified) from the list of items.
*    parameters = Custom parameters to be passed,
*    valueInput = the input field that holds the value of the selected list item (optional)
*    width = width of the dropdown list, defaults to the width of the textbox (optional)
*    onSelect = function(value, text)
*    onInvalidate = function()
*/
jQuery.fn.autocomplete = function(url, settings) {
    return this.each(function() {

        settings = jQuery.extend(
		{
		    minChars: 1,
		    timeout: 1000,
		    validSelection: true,
		    parameters: {},
		    valueInput: null,
		    width: 0,
		    onSelect: null,
		    onInvalidate: null
		}, settings);

        var textInput = $(this);
        var valueInput = settings.valueInput;
        if (!valueInput) {
            valueInput = textInput.after('<input type="hidden" name="' + textInput.attr("name") + '"/>')
                                  .attr("name", textInput.attr("name") + "_text")
                                  .next();
        }

        //create the ul that will hold the text and values
        valueInput.after('<ul class="autocomplete" />');
        var list = valueInput.next().css({
            //top: textInput.offset().top + textInput.outerHeight(),
            //left: textInput.offset().left,
            width: (settings.width == 0 ? textInput.width() : settings.width)
        });
        var oldText = '';
        var typingTimeout;
        var size = 0;
        var selected = 0;

        function invalidate() {
            if (settings.validSelection) valueInput.val('');
            if (settings.onInvalidate) settings.onInvalidate();
        }

        function selectItem(li) {
            var t = $(li).text();
            var v = $(li).attr('val');
            valueInput.val(v);
            textInput.val(t);
            if (settings.onSelect) settings.onSelect(v, t);
        }

        function getData(text) {
            window.clearInterval(typingTimeout);
            if (text == oldText) {
                list.show();
            }
            else if (settings.minChars != null && text.length >= settings.minChars) {
                clear();
                textInput.addClass('autocomplete-loading');
                settings.parameters.q = text;
                $.getJSON(url, settings.parameters, function(data) {
                    var items = '';
                    if (data) {
                        for (key in data) {
                            items += '<li val="' + key + '">' + data[key].replace(new RegExp("(" + text + ")", "i"), "<strong>$1</strong>") + '</li>';
                        }
                        list.html(items);
                        size = list.children().length;
                        //on mouse hover over elements set selected class and on click set the selected value and close list
                        list.show().children()
				              .hover(function() { $(this).addClass("selected").siblings().removeClass("selected"); },
				                     function() { $(this).removeClass("selected") })
				              .click(function() { selectItem(this); clear(); });
                    }
                    textInput.removeClass('autocomplete-loading');
                });
                oldText = text;
            }
        }

        function clear() {
            list.hide();
            selected = 0;
        }

        $(window).click(clear);
        textInput.keydown(function(e) {
            window.clearInterval(typingTimeout); screen
            if (e.which == 27)//escape
            {
                clear();
            }
            else if (e.which == 46 || e.which == 8) //delete and backspace
            {
                clear();
                invalidate();
                typingTimeout = window.setTimeout(function() { getData(textInput.val()) }, settings.timeout);
            }
            else if (e.which == 13) //enter 
            {
                if (list.css("display") == "none") //if the list is not visible then make a new request, otherwise hide the list
                {
                    getData(textInput.val());
                }
                else {
                    clear();
                }
                e.preventDefault();
                return false;
            }
            else if (e.which == 40 || e.which == 38) {
                if (list.css('display') != 'none') {
                    switch (e.which) {
                        case 40:
                            selected = selected >= size - 1 ? 0 : selected + 1; break;
                        case 38:
                            selected = selected <= 0 ? size - 1 : selected - 1; break;
                        default: break;
                    }
                    selectItem(list.children().removeClass('selected').eq(selected).addClass('selected')[0]);
                }
            }
            else if (e.which == 9) {
                clear();
            }
            else if (e.which == 37 || e.which == 39) {
                // ignore left/right
            }
            else {
                invalidate();
                typingTimeout = window.setTimeout(function() { getData(textInput.val()) }, settings.timeout);
            }
        });
    });
};
