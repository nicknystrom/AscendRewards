
function normalize() {
    $('input[type="text"].date').datepicker({ showAnim: '' });
    $(':button:contains("Delete")').button({ text: false, icons: { primary: 'ui-icon-trash'} });
    $(':button:not(.no-button), :submit:not(.no-button)').button();
 }

 $(function () {
     // no longer supporting ie
     // otherwise ie aggresively caches things
     // $.ajaxSetup({ cache: false });

     // drive the editor for string[]
     $('ul.strings :text').live('keyup', function () {
         var strings = $(this).closest('ul.strings');
         if (strings.find('input:last').val().length > 0) {
             strings.append('<li><input type="text" name="' + strings.find(':input:first').attr('name') + '" /></li>');
         }
     });

     // collapse button that hides/shows the left side menu
     $('#collapse').button({ icons: { primary: 'ui-icon-circle-minus' }, text: false })
                   .click(function () {
                       $('#content').css('left', $('#navigation').is(':visible') ? '20px' : '290px');
                       $('#navigation').toggle('fast');
                   });

     // folding create panels (auto-shown if errors)
     $('input[type="button"].dashboard-create').click(function () {
         $('fieldset.dashboard-create').slideToggle();
     });

     // configure the tenant quick navigator
     try {
         $('#tenant').selectmenu({
             select: function (event, ui) {
                 window.location = $(this).val();
             }
         });
     }
     catch (ex) {
     }

     // configure left side menu tabs
     $('#menu-tabs .menu-navigation a, #controller, #entity').button();
     $('#menu-tabs').tabs({ cookie: { expires: 180, path: '/'} });
     $('#admin-edit-tabs').tabs();

     // build navigation bar user search
     $('#menu-user-search-go').button({ text: false, icons: { primary: 'ui-icon-play'} });
     $('#menu-user-search').autocomplete({
         source: '/admin/user/search',
         minLength: 1
     });
     $('#menu-user-search-go').click(function () {
         window.location = '/admin/user/' + $('#menu-user-search').val();
     });

     // apply transforms to standard html
     normalize();
     $('#page').ajaxComplete(normalize);

     // the first submit button in the actions panel submits the first form in the content panel..
     // lets us put the submit button outside the <form>
     $('#actions > input[type=submit]:first').click(function () {
         $('#content > form:first').submit();
     });
     $('#actions > ul > li > a').button();

     // setup notifications
     $('.notification').hover(
      function () { $(this).addClass('notification-hover'); },
      function () { $(this).removeClass('notification-hover'); }
   );
     $('.notification').click(function () {
         $(this).remove();
     });

     // hook up 'create' buttons on index pages.. centralized for convenience
     $('.create-button').click(function () {
         $($(this).attr('href')).dialog({
             title: $(this).text(),
             modal: true
         });
     });

     // style up the tables on index pages
     $('.index-table').dataTable({

         bJQueryUI: true,
         bPaginate: false,
         bLengthChange: false,
         bProcessing: false,
         bStateSave: true,

         aaSorting: [[0, 'asc']],
         sDom: 'tfi',

         fnInitComplete: function () {
             // move info and filter divs to #actions
             $('.dataTables_info, .dataTables_filter').prependTo('#actions');
         }

     });

 });