$(document).ready(function () {
  const tour = new Shepherd.Tour(AdminTourCommonTourOptions);

  AdminTourNextPageButton.action = function () { window.location = '/Admin/Poll/List?showtour=True' };

  //'Forum add' step
  tour.addStep({
    title: AdminTourDataProvider.localized_data.ForumAddTitle,
    text: AdminTourDataProvider.localized_data.ForumAddText,
    attachTo: {
      element: '#tour-addforums-btns',
      on: 'auto'
    },
    buttons: [AdminTourNextButton]
  });

  //'Forum subtable arrow control' step
  tour.addStep({
    title: AdminTourDataProvider.localized_data.ForumSubTableTitle,
    text: AdminTourDataProvider.localized_data.ForumSubTableText,
    attachTo: {
      element: '.fas.fa-caret-right',
      on: 'bottom'
    },
    scrollTo: true,
    scrollToHandler: function (e) {
      $('html, body').animate({
        scrollTop: $(e).offset().top - 5
      }, 500);
    },
    buttons: [AdminTourBackButton, AdminTourNextButton]
  });


  //'Forum table' step
  var subTableAlreadyShown = false;
  tour.addStep({
    title: AdminTourDataProvider.localized_data.ForumTableTitle,
    text: AdminTourDataProvider.localized_data.ForumTableText,
    attachTo: {
      element: '#tour-forums-table',
      on: 'auto'
    },
    scrollTo: true,
    scrollToHandler: function (e) {
      $('html, body').animate({
        scrollTop: $(e).offset().top - 5
      }, 500);
    },
    when: {
      show() {
        if (!subTableAlreadyShown) {
          $('.child-control').click();
          subTableAlreadyShown = true;
        }
      }
    },
    buttons: [AdminTourBackButton, AdminTourNextButton]
  });

  //'Forum edit' step
  tour.addStep({
    title: AdminTourDataProvider.localized_data.ForumEditTitle,
    text: AdminTourDataProvider.localized_data.ForumEditText,
    attachTo: {
      element: '.button-column .btn.btn-default',
      on: 'auto'
    },
    scrollTo: true,
    scrollToHandler: function (e) {
      $('html, body').animate({
        scrollTop: $(e).offset().top - 5
      }, 500);
    },
    buttons: [AdminTourBackButton, AdminTourNextPageButton]
  });
  tour.start();
});