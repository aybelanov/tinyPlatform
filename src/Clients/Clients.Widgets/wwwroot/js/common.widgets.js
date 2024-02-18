export function addScript(url, onloadcallback, onerrorcallback, type) {
   if (!scriptExists(url)) {
      const script = document.createElement('script');
      script.async = true;
      script.defer = true;
      script.type = type || script.type;
      script.src = url;
      script.onload = function (ev) {
         if (onloadcallback) onloadcallback(ev);
      };
      script.onerror = function (error) {
         if (onerrorcallback) onerrorcallback(error);
      };
      document.head.appendChild(script);
      return true;
   }
   else {
      if (onloadcallback) {
         onloadcallback(null);
      }
      return false;
   }
}

export function addCss(url, onloadcallback, onerrorcallback) {
   if (!cssExists(url)) {
      const link = document.createElement('link');
      link.type = 'text/css';
      link.rel = 'stylesheet';
      link.href = url;
      link.onload = function (ev) {
         if (onloadcallback) onloadcallback(ev);
      };
      link.onerror = function (error) {
         if (onerrorcallback) onerrorcallback(error);
      };
      document.head.appendChild(link);
   } else {
      if (onloadcallback) onloadcallback(null);
   }
}

function scriptExists(url) {
   return document.querySelectorAll(`script[src="${url}"]`).length > 0;
}

function cssExists(url) {
   return document.querySelectorAll(`link[href="${url}"]`).length > 0;
}