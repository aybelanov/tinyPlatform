window.tinyWidgets = window.tinyWidgets || {};

var appjs = {
   getCulture: () => window.localStorage['DashboardCulture'],
   setCulture: value => window.localStorage['DashboardCulture'] = value,
   getLanguage: () => navigator.language || navigator.userLanguage,
   getTheme: () => {
      let result = window.localStorage['DashboardTheme'];
      if (!result) {
         result = 'standard';
         window.localStorage['DashboardTheme'] = result;
      }
      return result;
   },
   setTheme: value => window.localStorage['DashboardTheme'] = value,
   getTimeZoneOffset: () => (new Date()).getTimezoneOffset(),
   getSettings: key => window.localStorage.getItem(key),
   saveSettings: (key, value) => window.localStorage.setItem(key, value),
   removeSettings: key => window.localStorage.removeItem(key),
   clearSettings: () => window.localStorage.clear(),
   getElementSize: element => {
      if (element) return element.getBoundingClientRect();
      else return null;
   }
};

(() => {
   const link = document.createElement('link');
   link.type = 'text/css';
   link.rel = 'stylesheet';
   const theme = appjs.getTheme();
   link.href = '_content/Radzen.Blazor/css/' + theme + '.css';
   document.head.appendChild(link);
})();