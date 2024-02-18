$(document).ready(function () {
  const tour = new Shepherd.Tour(AdminTourCommonTourOptions);

  AdminTourNextPageButton.action = function () { window.location = '/Admin/Setting/News?showtour=True' };

  //'Blog post list' step
  tour.addStep({
    title: AdminTourDataProvider.localized_data.BlogPostsListTitle,
    text: AdminTourDataProvider.localized_data.BlogPostsListText,
    attachTo: {
      element: '#tour-blogpost-list',
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
    title: AdminTourDataProvider.localized_data.BlogPostsAddNewTitle,
    text: AdminTourDataProvider.localized_data.BlogPostsAddNewText,
    attachTo: {
      element: '#tour-blogpost-addnew',
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
    title: AdminTourDataProvider.localized_data.BlogPostsCommentTitle,
    text: AdminTourDataProvider.localized_data.BlogPostsCommentText,
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
    title: AdminTourDataProvider.localized_data.BlogPostsEditTitle,
    text: AdminTourDataProvider.localized_data.BlogPostsEditText,
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