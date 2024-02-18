$(document).ready(function () {
  const tour = new Shepherd.Tour(AdminTourCommonTourOptions);

  AdminTourDoneButton.action = function () { window.location = '/Admin/User/List' };

  //'Configure common device settings
  tour.addStep({
    title: AdminTourDataProvider.localized_data.DeviceSettingsCommonTitle,
    text: AdminTourDataProvider.localized_data.DeviceSettingsCommonText,
    attachTo: {
      element: '#tour-devicesettings-common',
      on: 'auto'
    },
    buttons: [AdminTourBackButton, AdminTourNextButton]
  });

  //'Configure device account settings
  tour.addStep({
    title: AdminTourDataProvider.localized_data.DeviceSettingsAccountTitle,
    text: AdminTourDataProvider.localized_data.DeviceSettingsAccountText,
    attachTo: {
      element: '#tour-devicesettings-account',
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

  //'Configure device password
  tour.addStep({
    title: AdminTourDataProvider.localized_data.DeviceSettingsSecurityTitle,
    text: AdminTourDataProvider.localized_data.DeviceSettingsSecurityText,
    attachTo: {
      element: '#tour-devicesettings-security',
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
    buttons: [AdminTourBackButton, AdminTourNextButton]
  });

  //'Learm Users/Devices' step
  tour.addStep({
    title: AdminTourDataProvider.localized_data.LearnUsersDevicesTitle,
    text: AdminTourDataProvider.localized_data.LearnUsersDevicesText,
    buttons: [AdminTourBackButton, AdminTourDoneButton]
  });

  tour.start();
});