$(document).ready(function () {
  const tour = new Shepherd.Tour(AdminTourCommonTourOptions);

  AdminTourNextPageButton.action = function () { window.location = '/Admin/Forum/List?showtour=True' };

  //'Forum common settigs'
  tour.addStep({
    title: AdminTourDataProvider.localized_data.ForumSettingsCommonTitle,
    text: AdminTourDataProvider.localized_data.ForumSettingsCommonText,
    attachTo: {
      element: '#forumsettings-common',
      on: 'auto'
    },
    scrollTo: true,
    scrollToHandler: function (e) {
      $('html, body').animate({
        scrollTop: $(e).offset().top - 5
      }, 500);
    },
    buttons: [AdminTourNextButton]
  });

  //'Forum permission settigs'
  tour.addStep({
    title: AdminTourDataProvider.localized_data.ForumSettingsPermissionsTitle,
    text: AdminTourDataProvider.localized_data.ForumSettingsPermissionsText,
    attachTo: {
      element: '#forumsettings-permissions',
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