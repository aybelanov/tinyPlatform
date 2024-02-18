$(document).ready(function () {
  const tour = new Shepherd.Tour(AdminTourCommonTourOptions);

  //'Polls add' step
  tour.addStep({
    title: AdminTourDataProvider.localized_data.PollsAddTitle,
    text: AdminTourDataProvider.localized_data.PollsAddText,
    attachTo: {
      element: '#tour-addpolls-btns',
      on: 'bottom'
    },
    buttons: [AdminTourBackButton, AdminTourNextButton]
  });


  //'Polls table' step
  tour.addStep({
    title: AdminTourDataProvider.localized_data.PollsTableTitle,
    text: AdminTourDataProvider.localized_data.PollsTableText,
    attachTo: {
      element: '#tour-polls-table',
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

  //'Polls edit' step
  tour.addStep({
    title: AdminTourDataProvider.localized_data.PollsEditTitle,
    text: AdminTourDataProvider.localized_data.PollsEditText,
    attachTo: {
      element: '.button-column .btn.btn-default',
      on: 'bottom'
    },
    scrollTo: true,
    scrollToHandler: function (e) {
      $('html, body').animate({
        scrollTop: $(e).offset().top - 5
      }, 500);
    },
    buttons: [AdminTourBackButton, AdminTourDoneButton]
  });

  tour.start();
});