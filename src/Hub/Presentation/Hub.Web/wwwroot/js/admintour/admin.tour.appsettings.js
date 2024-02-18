$(document).ready(function () {
  const tour = new Shepherd.Tour(AdminTourCommonTourOptions);

  AdminTourNextPageButton.action = function () { window.location = '/Admin/Setting/UserUser?showtour=True' };

  //'Welcome' step
  tour.addStep({
    title: AdminTourDataProvider.localized_data.PersonalizeStoreIntroTitle,
    text: AdminTourDataProvider.localized_data.PersonalizeStoreIntroText,
    buttons: [AdminTourNextButton]
  });

  //'Basic/Advanced mode' step
  tour.addStep({
    title: AdminTourDataProvider.localized_data.PersonalizeStoreBasicAdvancedTitle,
    text: AdminTourDataProvider.localized_data.PersonalizeStoreBasicAdvancedText,
    attachTo: {
      element: '.onoffswitch',
      on: 'auto'
    },
    buttons: [AdminTourBackButton, AdminTourNextButton]
  });

  //'Configure hub hosting
  tour.addStep({
    title: AdminTourDataProvider.localized_data.ConfigureHubHostingTitle,
    text: AdminTourDataProvider.localized_data.ConfigureHubHostingText,
    attachTo: {
      element: '#hosting-area',
      on: 'auto'
    },
    buttons: [AdminTourBackButton, AdminTourNextButton]
  });

  //'Configure hub proxy
  tour.addStep({
    title: AdminTourDataProvider.localized_data.ConfigureHubProxyTitle,
    text: AdminTourDataProvider.localized_data.ConfigureHubProxyText,
    attachTo: {
      element: '#proxy-area',
      on: 'auto'
    },
    buttons: [AdminTourBackButton, AdminTourNextButton]
  });

  //'Configure ssl
  tour.addStep({
    title: AdminTourDataProvider.localized_data.ConfigureHubSecurityTitle,
    text: AdminTourDataProvider.localized_data.ConfigureHubSecurityText,
    attachTo: {
      element: '#security-area',
      on: 'auto'
    },
    buttons: [AdminTourBackButton, AdminTourNextButton]
  });

  //'Don't forget to save' step
  tour.addStep({
    title: AdminTourDataProvider.localized_data.ConfigureHubSaveTitle,
    text: AdminTourDataProvider.localized_data.ConfigureHubSaveText,
    attachTo: {
      element: '#tour-appsettings-save',
      on: 'auto'
    },
    buttons: [AdminTourBackButton, AdminTourNextPageButton]
  });

  tour.start();

  if (!$('#HostingConfigModel_UseProxy').is(":checked"))
    $('#HostingConfigModel_UseProxy').click();
});