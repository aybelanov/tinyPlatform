'use strict';

import './js/widgets.js';
import { SVG } from './lib/svgdotjs/svg.js/dist/svg.esm.js';

export async function initScheme(elementId, objRef, theme, culture, imgUrl, cfg) {
   if (!elementId || !objRef)
      return;

   try {
      const response = await fetch(imgUrl, { cache: "default" });
      let content = await response.text();
      const parser = new DOMParser();
      const old = document.querySelector('svg');
      const svg = parser.parseFromString(content, 'image/svg+xml').querySelector('svg');
      svg.id = elementId;
      svg.style = old.style;
      svg.classList = old.classList;
      svg.setAttribute('height', old.getAttribute('height'));
      svg.setAttribute('width', old.getAttribute('width'));
      const innerScript = svg.querySelector('script');
      svg.removeChild(innerScript);
      const wrapper = document.getElementById(`wrapper-${elementId}`)
      wrapper.replaceChild(svg, old);

      window.tinyWidgets[elementId] = {
         container: svg,
         objectref: objRef,
         svgjs: SVG(svg),
         theme: theme,
         culture: culture
      };

      // !Important! Be careful! Do load a livescheme only from trusted issuer and after js-code checking! 
      const jscode = innerScript.innerHTML.replace('<![CDATA[', '').replace(']]>', '');
      const script = document.createElement('script');
      script.innerHTML = jscode;
      svg.appendChild(script);

      window.tinyWidgets[elementId].initSvg(cfg);
   }
   catch (error) {
      console.log(error);
      return;
   }
}


export function updateScheme(containerId, data) {
   const widget = window.tinyWidgets[containerId];
   if (widget && widget.updateScheme) {
      if (data.length > 0 && data[0].length > 0) {
         const records = []
         for (let i = 0; i < data[0].length; i++) {
            records.push({
               eventtimestamp: data[0][i],
               value: data[1][i],
               jsonvalue: data[2][i],
               metadata: data[3][i]
            });
         }

         widget.updateScheme(records);
      }
   }
}

export function dispose(containerId) {
   const id = containerId;
   if (window.tinyWidgets[id]) {
      window.tinyWidgets[id].destroy();
      if (window.tinyWidgets[id].objectref) {
         window.tinyWidgets[id].objectref.dispose();
      }
      delete window.tinyWidgets[id].objectref;
      if (window.tinyWidgets[id].svgjs) {
         window.tinyWidgets[id].svgjs.clear();
      }
      delete window.tinyWidgets[id].svg;
      delete window.tinyWidgets[id].container;
      delete window.tinyWidgets[id];
   }
}