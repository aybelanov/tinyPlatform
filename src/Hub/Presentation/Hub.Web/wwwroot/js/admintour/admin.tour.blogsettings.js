$(document).ready(function () {
  const tour = new Shepherd.Tour(AdminTourCommonTourOptions);

  AdminTourNextPageButton.action = function () { window.location = '/Admin/Blog/BlogPosts?showtour=True' };

  //'Blog common settigs'
  tour.addStep({
    title: AdminTourDataProvider.localized_data.BlogCommonSettingsTitle,
    text: AdminTourDataProvider.localized_data.BlogCommonSettingsText,
    attachTo: {
      element: '#blogsettings-common',
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

  //'Blog comment settigs'
  tour.addStep({
    title: AdminTourDataProvider.localized_data.BlogSettingsCommentsTitle,
    text: AdminTourDataProvider.localized_data.BlogSettingsCommentsText,
    attachTo: {
      element: '#blogsettings-comments',
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