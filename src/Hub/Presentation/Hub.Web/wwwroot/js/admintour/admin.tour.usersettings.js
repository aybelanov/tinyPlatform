$(document).ready(function () {
  const tour = new Shepherd.Tour(AdminTourCommonTourOptions);

  AdminTourNextPageButton.action = function () { window.location = '/Admin/Setting/Device?showtour=True' };

  //'Configure common user settings
  tour.addStep({
    title: AdminTourDataProvider.localized_data.UserSettingsCommonTitle,
    text: AdminTourDataProvider.localized_data.UserSettingsCommonText,
    attachTo: {
      element: '#tour-usersettings-common',
      on: 'auto'
    },
    buttons: [AdminTourNextButton]
  });

  //'Configure user account settings
  tour.addStep({
    title: AdminTourDataProvider.localized_data.UserSettingsAccountTitle,
    text: AdminTourDataProvider.localized_data.UserSettingsAccountText,
    attachTo: {
      element: '#tour-usersettings-account',
      on: 'auto'
    },
    scrollTo: true,
    scrollToHandler: function (e) {
      $('html, body').animate({
        scrollTop: $(e).offset().top - 100
      }, 500);
    },
    buttons: [AdminTourBackButton, AdminTourNextButton]
  });

  //'Configure user password
  tour.addStep({
    title: AdminTourDataProvider.localized_data.UserSettingsSecurityTitle,
    text: AdminTourDataProvider.localized_data.UserSettingsSecurityText,
    attachTo: {
      element: '#tour-usersettings-security',
      on: 'auto'
    },
    scrollTo: true,
    scrollToHandler: function (e) {
      $('html, body').animate({
        scrollTop: $(e).offset().top - 100
      }, 500);
    },
    buttons: [AdminTourBackButton, AdminTourNextButton]
  });


  //'Don't forget to save' step
  tour.addStep({
    title: AdminTourDataProvider.localized_data.ConfigureHubSaveTitle,
    text: AdminTourDataProvider.localized_data.ConfigureHubSaveText,
    attachTo: {
      element: '.float-right button[name="save"]',
      on: 'auto'
    },
    buttons: [AdminTourBackButton, AdminTourNextPageButton]
  });

  tour.start();
});