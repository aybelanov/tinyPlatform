$(document).ready(function () {
  const tour = new Shepherd.Tour(AdminTourCommonTourOptions);

  AdminTourNextPageButton.action = function () { window.location = '/Admin/Topic/List?showtour=True' };

  //'Footer items' step
  tour.addStep({
    title: AdminTourDataProvider.localized_data.PersonalizeFooterItemsTitle,
    text: AdminTourDataProvider.localized_data.PersonalizeFooterItemsText,
    attachTo: {
      element: '#generalcommon-footeritems',
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

  //'Social media' step
  tour.addStep({
    title: AdminTourDataProvider.localized_data.PersonalizeSocialMediaTitle,
    text: AdminTourDataProvider.localized_data.PersonalizeSocialMediaText,
    attachTo: {
      element: '#generalcommon-socialmedia',//'#tour-socialmedia-settings',
      on: 'auto'
    },
    scrollTo: true,
    scrollToHandler: function (e) {
      $('html, body').animate({
        scrollTop: $(e).offset().top - 5 //- ($(window).height() - $(e).outerHeight() - 20)
      }, 500);
    },
    buttons: [AdminTourBackButton, AdminTourNextButton]
  });

  //'Captcha' step
  tour.addStep({
    title: AdminTourDataProvider.localized_data.PersonalizeCaptchaTitle,
    text: AdminTourDataProvider.localized_data.PersonalizeCaptchaText,
    attachTo: {
      element: '#generalcommon-captcha',
      on: 'auto'
    },
    scrollTo: true,
    scrollToHandler: function (e) {
      $('html, body').animate({
        scrollTop: $(e).offset().top - 5
      }, 500);
    },
    buttons: [AdminTourBackButton, AdminTourNextButton]
  });

  //'Choose a theme' step
  tour.addStep({
    title: AdminTourDataProvider.localized_data.PersonalizeDesignTitle,
    text: AdminTourDataProvider.localized_data.PersonalizeDesignText,
    attachTo: {
      element: '#tour-design-settings',
      on: 'auto'
    },
    scrollTo: true,
    scrollToHandler: function (e) {
      $('html, body').animate({
        scrollTop: $(e).offset().top - 75
      }, 500);
    },
    buttons: [AdminTourBackButton, AdminTourNextButton]
  });

  //'Change icons' step
  tour.addStep({
    title: AdminTourDataProvider.localized_data.PersonalizeIconsTitle,
    text: AdminTourDataProvider.localized_data.PersonalizeIconsText,
    attachTo: {
      element: '#generalcommon-favicon',
      on: 'auto'
    },
    scrollTo: true,
    scrollToHandler: function (e) {
      $('html, body').animate({
        scrollTop: $(e).offset().top - 5
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
})