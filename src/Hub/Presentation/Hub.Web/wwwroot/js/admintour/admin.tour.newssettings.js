$(document).ready(function () {
  const tour = new Shepherd.Tour(AdminTourCommonTourOptions);

  AdminTourNextPageButton.action = function () { window.location = '/Admin/News/NewsItems?showtour=True' };


  //'News common settigs'
  tour.addStep({
    title: AdminTourDataProvider.localized_data.NewsCommonSettingsTitle,
    text: AdminTourDataProvider.localized_data.NewsCommonSettingsText,
    attachTo: {
      element: '#newsettings-common',
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

  //'News comment settigs'
  tour.addStep({
    title: AdminTourDataProvider.localized_data.NewsSettingsCommentsTitle,
    text: AdminTourDataProvider.localized_data.NewsSettingsCommentsText,
    attachTo: {
      element: '#newsettings-comments',
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