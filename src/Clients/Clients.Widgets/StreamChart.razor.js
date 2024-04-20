import './js/widgets.js';
import Chart from './lib/chart.js/auto/auto.js';
import './lib/chartjs-adapter-luxon/dist/chartjs-adapter-luxon.esm.js';
import ChartStreaming from './lib/chartjs-plugin-streaming/dist/chartjs-plugin-streaming.esm.js';

Chart.register(ChartStreaming);

export function initChart(container, objRef, emptyText, culture, theme, chartType, measureUnit, minValue, maxValue, duration, delay, smooth, markers) {
   if (!container || !objRef)
      return;

   window.tinyWidgets[container.id] = {
      container: container,
      objectref: objRef,
      chart: new Chart(container, getInitConfig(emptyText, culture, theme, chartType, measureUnit, minValue, maxValue, duration, delay, smooth))
   };

   function getInitConfig(emptyText, culture, theme, chartType, measureUnit, minValue, maxValue, duration, delay, smooth) {
      emptyText = emptyText || '';
      culture = culture || 'en';
      theme = theme || 'white';
      const config = {
         type: 'line',
         data: {
            labels: [],
            datasets: [{
               data: [],
               cubicInterpolationMode: smooth ? 'monotone' : 'default',
               fill: chartType === 'Area' ? "start" : false
            }]
         },
         options: {
            scales: {
               x: {
                  type: 'realtime',
                  // Change options only for THIS AXIS
                  realtime: {
                     duration: duration * 1000,
                     refresh: 1000,
                     delay: delay,
                     frameRate: 20,
                     //onRefresh: chart => { }
                  },
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
                  min: minValue,
                  max: maxValue,
                  title: {
                     display: true,
                     text: measureUnit
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
               // Change options for ALL axes of THIS CHART
               streaming: {
                  duration: duration
               },
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
               point: {
                  pointStyle: markers ? markers : false
               }
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

export function onReceive(containerId, moments, values) {
   const widget = window.tinyWidgets[containerId];
   if (widget) {
      const length = values.length; let i = 0;
      for (i = 0; i < length; i++) {
         widget.chart.data.datasets[0].data.push({
            x: moments[i],
            y: values[i]
         });
      }
      
      //// update chart datasets keeping the current animation
      //if (widget.chart) {
      //   widget.chart.update('quiet');
      //}
   }
}

export function clearChart(containerId) {
   const widget = window.tinyWidgets[containerId];
   if (widget) {
      widget.chart.data.datasets[0].dataset.data = [];
      widget.chart.options.scales.y.title.display = false;
      widget.chart.update();
   }
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
         //Chart.unregister(ChartStreaming);
         ChartStreaming[0].destroy(window.tinyWidgets[id].chart);
         window.tinyWidgets[id].chart.destroy();
      }
      delete window.tinyWidgets[id].chart;
      delete window.tinyWidgets[id].container;
      delete window.tinyWidgets[id];
   }
}