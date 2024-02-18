'use strict';

import './js/widgets.js';
import Chart from './lib/chart.js/auto/auto.js';
import './lib/chartjs-adapter-luxon/dist/chartjs-adapter-luxon.esm.js';


export function initChart(container, objRef, emptyText, culture, theme) {
   if (!container || !objRef)
      return;

   window.tinyWidgets[container.id] = {
      container: container,
      objectref: objRef,
      initialData: [],
      chart: new Chart(container, getInitConfig(emptyText, culture, theme))
   };

   function getInitConfig(emptyText, culture, theme) {
      emptyText = emptyText || '';
      culture = culture || 'en';
      theme = theme || 'white';
      const config = {
         type: 'line',
         data: {
            labels: [],
            datasets: [{
               data: []
            }]
         },
         options: {
            scales: {
               x: {
                  ticks: {
                     maxRotation: 0,
                     minRotation: 0,
                     padding: 0,
                     autoSkip: false,
                     autoSkipPadding: 10,
                     source: 'auto',
                     major: {
                        enabled: true
                     },
                  },
                  grid: {},
                  type: 'time',
                  time: {
                     //tooltipFormat: 'yyyy.MM.DD HH:mm:ss'
                  },
                  title: {
                     display: false,
                     text: 'timeline'
                  }
               },
               y: {
                  type: 'linear',
                  title: {
                     display: false,
                     text: 'value'
                  },
                  ticks: {},
                  grid: {}
               },
               y2: {
                  type: 'linear',
                  display: false,
                  position: 'right',
                  ticks: {

                  },
                  title: {
                     display: false,
                     text: 'value2'
                  },
                  // grid line settings
                  grid: {
                     drawOnChartArea: false,
                  },
               }
            },
            interaction: {
               intersect: false,
               mode: 'nearest',
               axis: 'xy'
            },
            stacked: true,
            plugins: {
               title: {
                  display: false,
                  text: emptyText
               },
               legend: {
                  display: false,
                  position: 'top',
                  labels: {}
               },
            },
            datasets: {
               line: {}
            },
            elements: {
               point: {}
            },
            //spanGaps: false//1000 * 60 * 60 * 24 * 1,// one day
            animation: true,
            responsive: true,
            maintainAspectRatio: false,
            locale: culture
         }
      };
      setTheme(config, theme);
      return config;
   }

}

export function drawChart(containerId, cfg, xValues, yValues) {
   return new Promise(function (resolve) {
      const widget = window.tinyWidgets[containerId];
      if (widget) {
         const config = JSON.parse(cfg);
         const chart = widget.chart;
         widget.initialData = getSeries(xValues, yValues);
         const series = config.isCumulative ? getCumulative(widget.initialData) : widget.initialData;
         chart.options.scales.y.title.display = true;
         chart.options.scales.y.title.text = config.seriesName;
         chart.data.datasets = [];
         chart.data.datasets.push({
            id: 'recordcount',
            label: config.seriesName,
            data: series,
            cubicInterpolationMode: 'monotone',
            tension: 0.4,
            radius: 3,
            hidden: false,
            borderColor: config.seriesColor,
            backgroundColor: config.areaColor,
            spanGaps: false,
            yAxisID: 'y',
            fill: config.isCumulative ? 'origin' : false
         });

         chart.update();
      }
      resolve();
   });
}

export function clearChart(containerId) {
   const widget = window.tinyWidgets[containerId];
   if (widget) {
      const dataset = widget.chart.data.datasets[0];
      widget.initialData = [];
      dataset.data = [];
      widget.chart.options.scales.y.title.display = false;
      widget.chart.update();
   }
}

export function updateChart(containerId, isCumulative) {
   const widget = window.tinyWidgets[containerId];
   if (widget) {
      const dataset = widget.chart.data.datasets[0];
      dataset.data = isCumulative ? getCumulative(widget.initialData) : widget.initialData;
      dataset.fill = isCumulative ? 'origin' : false;
      widget.chart.update();
   }
}

function getSeries(xValues, yValues) {
   const result = [];
   const length = xValues.length;
   let i = 0;
   for (i = 0; i < length; i++) {
      result.push({
         x: xValues[i],
         y: yValues[i]
      });
   }
   return result;
}

function getCumulative(series) {
   const res = [];
   if (series.length > 0) {
      res.push({
         x: series[0].x,
         y: series[0].y
      });
      const length = series.length;
      let i = 0;
      for (i = 1; i < length; i++) {
         res.push({
            x: series[i].x,
            y: series[i].y + res[i - 1].y
         });
      }
   }
   return res;
}


function setTheme(cfg, theme) {
   if (theme === 'dark') {
      cfg.options.plugins.title.color =
         cfg.options.plugins.legend.labels.color =
         cfg.options.scales.x.ticks.color =
         cfg.options.scales.x.title.color =
         cfg.options.scales.y.ticks.color =
         cfg.options.scales.y.title.color =
         cfg.options.scales.y2.ticks.color =
         cfg.options.scales.y2.title.color = 'white';
      cfg.options.scales.x.grid.color =
         cfg.options.scales.y.grid.color = '#45565e';
   }
}

export function destroyChart(container) {
   const id = container.id;
   if (window.tinyWidgets[id]) {
      if (window.tinyWidgets[id].objectref) {
         window.tinyWidgets[id].objectref.dispose();
      }
      delete window.tinyWidgets[id].objectref;
      if (window.tinyWidgets[id].chart) {
         window.tinyWidgets[id].chart.destroy();
      }
      delete window.tinyWidgets[id].chart;
      delete window.tinyWidgets[id].container;
      delete window.tinyWidgets[id].initialData;
      delete window.tinyWidgets[id];
   }
}