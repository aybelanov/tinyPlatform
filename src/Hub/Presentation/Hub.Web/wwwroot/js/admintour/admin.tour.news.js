$(document).ready(function () {
  const tour = new Shepherd.Tour(AdminTourCommonTourOptions);

  AdminTourNextPageButton.action = function () { window.location = '/Admin/Setting/Forum?showtour=True' };

  //'Basic/Advanced mode' step
  tour.addStep({
    title: AdminTourDataProvider.localized_data.NewsListTitle,
    text: AdminTourDataProvider.localized_data.NewsListText,
    attachTo: {
      element: '#tour-news-list',
      on: 'top'
    },
    scrollTo: true,
    scrollToHandler: function (e) {
      $('html, body').animate({
        scrollTop: $(e).offset().top - 5
      }, 500);
    },
    buttons: [AdminTourNextButton]
  });

  //'Blogpost add new' sper
  tour.addStep({
    title: AdminTourDataProvider.localized_data.NewsAddNewTitle,
    text: AdminTourDataProvider.localized_data.NewsAddNewText,
    attachTo: {
      element: '#tour-news-addnew',
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

  //'View comment' step
  tour.addStep({
    title: AdminTourDataProvider.localized_data.NewsCommentTitle,
    text: AdminTourDataProvider.localized_data.NewsCommentText,
    attachTo: {
      element: '.btn.btn-default.comments',
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

  //'Edit post' step
  tour.addStep({
    title: AdminTourDataProvider.localized_data.NewsEditTitle,
    text: AdminTourDataProvider.localized_data.NewsEditText,
    attachTo: {
      element: '.button-column.edit .btn.btn-default',
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