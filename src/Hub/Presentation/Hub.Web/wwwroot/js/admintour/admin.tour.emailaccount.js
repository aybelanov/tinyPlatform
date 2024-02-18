$(document).ready(function () {
  const tour = new Shepherd.Tour(AdminTourCommonTourOptions);

  //'Email address' step
  tour.addStep({
    title: AdminTourDataProvider.localized_data.EmailAccountEmailAddressTitle,
    text: AdminTourDataProvider.localized_data.EmailAccountEmailAddressText,
    attachTo: {
      element: '#email-area',
      on: 'auto'
    },
    buttons: [AdminTourNextButton]
  });

  //'Send test email' step
  tour.addStep({
    title: AdminTourDataProvider.localized_data.EmailAccountTestEmailTitle,
    text: AdminTourDataProvider.localized_data.EmailAccountTestEmailText,
    attachTo: {
      element: '#test-email-area',
      on: 'auto'
    },
    buttons: [AdminTourBackButton, AdminTourDoneButton]
  });

  tour.start();
})