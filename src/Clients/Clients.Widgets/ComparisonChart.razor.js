'use strict';

import './js/widgets.js';
import Chart from './lib/chart.js/auto/auto.js';
import './lib/chartjs-adapter-luxon/dist/chartjs-adapter-luxon.esm.js';

export function initChart(container, objRef, emptyText, culture, theme, showCfg) {
   if (!container || !objRef)
      return;

   const cfg = getInitChartConfig();
   cfg.options.plugins.title.text = emptyText;
   cfg.options.locale = culture;
   setTheme(cfg, theme);
   window.tinyWidgets[container.id] = {
      container: container,
      objectref: objRef,
      chart: new Chart(container, cfg)
   };
   setInitCheckBoxStates(container.id, showCfg);
   addCheckBoxesEvents(container.id);

   function getInitChartConfig() {
      const chartInitConfig = {
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
                     // //For a category axis, the val is the index so the lookup via getLabelForValue is needed
                     //callback: function (val, index) {
                     //  // Hide every 2nd tick label
                     //  return index % 1 === 0 ? this.getLabelForValue(val) : '';
                     //}
                  },
                  grid: {},
                  type: 'time',
                  time: {
                     tooltipFormat: '',//'yyyy.MM.DD HH:mm:ss',
                     displayFormats: {
                        //hour: 'HH'
                     }
                  },
                  title: {
                     display: false,
                     text: 'Date'
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
                     drawOnChartArea: false, // only want the grid lines for one axis to show up
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
                  display: true,
                  text: ''
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
            locale: 'en'
         }
      };

      return chartInitConfig;
   }

}

export function clearChart(containerId, emptyText, culture, theme, showCfg) {
   return new Promise(function (resolve) {
      const widget = window.tinyWidgets[containerId];
      if (widget) {
         setControlBlockVisible(widget.container.id, 'series1', null, false);
         setControlBlockVisible(widget.container.id, 'series2', null, false);
         var cfg = getInitChartConfig();
         cfg.options.plugins.title.text = emptyText;
         cfg.options.locale = culture;
         setTheme(cfg, theme);
         widget.chart.destroy();
         widget.chart = new Chart(widget.container, cfg);
         setInitCheckBoxStates(widget.container.id, showCfg);
      }
      resolve();
   });
}

export function drawChart(id, metaData, series1, series2) {
   return new Promise(function (resove) {
      if (window.tinyWidgets[id]) {
         const chart = window.tinyWidgets[id].chart;
         const meta = JSON.parse(metaData);
         chart.type = meta.type;
         //chart.options.plugins.legend.display = true;
         //chart.options.plugins.legend.position = 'top';
         if (meta.title) {
            chart.options.plugins.title.display = true;
            chart.options.plugins.title.text = meta.title;
         }
         else {
            chart.options.plugins.title.display = false;
         }

         if (meta.titleX) {
            chart.options.scales.x.title.display = true;
            chart.options.scales.x.title.text = meta.titleX;
         } else {
            chart.options.scales.x.title.display = false;
         }

         if (meta.titleY) {
            chart.options.scales.y.title.display = true;
            chart.options.scales.y.title.text = meta.titleY;
         } else {
            chart.options.scales.y.title.display = false;
         }

         chart.data.datasets = [];
         setControlBlockVisible(id, 'series1', meta.seriesName, true);
         const series = getSeries(series1[0], series1[1]);
         if (series.length > 0) {
            setCheckBoxesVisible(id, 'series1', true);
            chart.data.datasets.push({
               id: 'avg1',
               label: meta.seriesName,
               data: series,
               cubicInterpolationMode: 'monotone',
               tension: 0.1,
               radius: 3,
               hidden: !getCheckBox(id, 'avg1').checked,
               borderColor: 'rgb(54, 162, 235)',
               backgroundColor: 'rgb(54, 162, 235, 0.5)',
               spanGaps: false,
               yAxisID: 'y'
            }, {
               id: 'min1',
               label: meta.seriesName + ' (min)',
               data: getSeries(series1[0], series1[2]),
               cubicInterpolationMode: 'monotone',
               hidden: true,
               tension: 0.1,
               radius: 3,
               hidden: !getCheckBox(id, 'min1').checked,
               borderColor: 'rgb(54, 162, 235)',
               backgroundColor: 'rgb(54, 162, 235, 0.5)',
               spanGaps: false,
               yAxisID: 'y'
            }, {
               id: 'max1',
               label: meta.seriesName + ' (max)',
               data: getSeries(series1[0], series1[3]),
               cubicInterpolationMode: 'monotone',
               hidden: true,
               tension: 0.1,
               radius: 3,
               hidden: !getCheckBox(id, 'max1').checked,
               borderColor: 'rgb(54, 162, 235)',
               backgroundColor: 'rgb(54, 162, 235, 0.5)',
               spanGaps: false,
               yAxisID: 'y'
            });
         } else {
            setCheckBoxesVisible(id, 'series1', false);
         }

         if (meta.seriesName2 != null) {
            setControlBlockVisible(id, 'series2', meta.seriesName2, true);
            chart.options.scales.y2.display = true;
            chart.options.scales.y2.title.display = true;
            chart.options.scales.y2.title.text = meta.titleY2;
            const series = getSeries(series2[0], series2[1]);
            if (series.length > 0) {
               setCheckBoxesVisible(id, 'series2', true);
               chart.data.datasets.push({
                  id: 'avg2',
                  label: meta.seriesName2,
                  cubicInterpolationMode: 'monotone',
                  data: series,
                  tension: 0.1,
                  radius: 3,
                  hidden: !getCheckBox(id, 'avg2').checked,
                  borderColor: 'rgb(255, 99, 132)',
                  backgroundColor: 'rgb(255, 99, 132, 0.5)',
                  spanGaps: false,
                  yAxisID: 'y2'
               }, {
                  id: 'min2',
                  label: meta.seriesName2 + ' (min)',
                  data: getSeries(series2[0], series2[2]),
                  cubicInterpolationMode: 'monotone',
                  tension: 0.1,
                  radius: 3,
                  hidden: !getCheckBox(id, 'min2').checked,
                  borderColor: 'rgb(255, 99, 132)',
                  backgroundColor: 'rgb(255, 99, 132, 0.5)',
                  spanGaps: false,
                  yAxisID: 'y2'
               }, {
                  id: 'max2',
                  label: meta.seriesName2 + ' (max)',
                  data: getSeries(series2[0], series2[3]),
                  cubicInterpolationMode: 'monotone',
                  tension: 0.1,
                  radius: 3,
                  hidden: !getCheckBox(id, 'max2').checked,
                  borderColor: 'rgb(255, 99, 132)',
                  backgroundColor: 'rgb(255, 99, 132, 0.5)',
                  spanGaps: false,
                  yAxisID: 'y2'
               });

            } else {
               setCheckBoxesVisible(id, 'series2', false);
            }
         } else {
            chart.options.scales.y2.display = false;
            chart.options.scales.y2.title.display = false;
         }

         chart.update();
      }
      resove();
   });
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


function getSeries(dataX, dataY) {
   const series = [];
   if (dataX && dataY) {
      for (let i = 0; i < dataX.length; i++) {
         const point = { x: dataX[i] * 1000, y: dataY[i] };
         series.push(point);
      }
   }
   return series;
}

function addCheckBoxesEvents(wrapperId) {
   const checkBoxes = document.querySelectorAll('[data-chart-id="' + `${wrapperId}` + '"] input[type=checkbox]');
   for (let i = 0; i < checkBoxes.length; i++) {
      checkBoxes[i].addEventListener('change', checkBoxHandler);
   }
}

function removeCheckBoxesEvents(wrapperId) {
   var checkBoxes = document.querySelectorAll('[data-chart-id="' + `${wrapperId}` + '"] input[type=checkbox]');
   for (let i = 0; i < checkBoxes.length; i++) {
      checkBoxes[i].removeEventListener('change', checkBoxHandler);
   }
}

function checkBoxHandler(event) {
   const id = event.currentTarget.parentElement.parentElement.parentElement.parentElement.dataset.chartId;
   const chart = window.tinyWidgets[id].chart;
   const datasets = chart.data.datasets;
   const set = datasets.filter((s) => s.id === event.currentTarget.name);
   if (set[0]) {
      set[0].hidden = !event.currentTarget.checked;
   }
   chart.update();
}

function setControlBlockVisible(id, blockName, seriesName, state) {
   const controlWrapper = document.querySelector('[data-chart-id="' + `${id}` + '"] [data-control-wrapper="' + `${blockName}` + '"]');
   const name = controlWrapper.querySelector('span');
   if (state) {
      controlWrapper.style.display = 'block';
      name.innerHTML = seriesName + ': ';
   } else {
      controlWrapper.style.cssText = 'display:none!important';
      name.innerHTML = '';
   }
}

function setCheckBoxesVisible(id, series, isVisible) {
   const divIsEmpty = document.querySelector('[data-chart-id="' + `${id}` + '"] [data-is-empty="' + `${series}` + '"]');
   const divNotEmpty = document.querySelector('[data-chart-id="' + `${id}` + '"] [data-not-empty="' + `${series}` + '"]');
   if (isVisible) {
      divNotEmpty.style.display = 'block';
      divIsEmpty.style.cssText = 'display:none!important';
   } else {
      divNotEmpty.style.cssText = 'display:none!important';
      divIsEmpty.style.display = 'block';
   }
}

function setInitCheckBoxStates(wrapperId, showCfg) {
   if (showCfg) {
      const cfg = JSON.parse(showCfg);
      getCheckBox(wrapperId, 'avg1').checked = !cfg.hideY;
      getCheckBox(wrapperId, 'min1').checked = !cfg.hideMinY;
      getCheckBox(wrapperId, 'max1').checked = !cfg.hideMaxY;
      getCheckBox(wrapperId, 'avg2').checked = !cfg.hideY2;
      getCheckBox(wrapperId, 'min2').checked = !cfg.hideMinY2;
      getCheckBox(wrapperId, 'max2').checked = !cfg.hideMaxY2;
   }
   else {
      getCheckBox(wrapperId, 'avg1').checked = true;
      getCheckBox(wrapperId, 'min1').checked = false;
      getCheckBox(wrapperId, 'max1').checked = false;
      getCheckBox(wrapperId, 'avg2').checked = true;
      getCheckBox(wrapperId, 'min2').checked = false;
      getCheckBox(wrapperId, 'max2').checked = false;
   }
}

function getCheckBox(wrapperId, name) {
   return document.querySelector('[data-chart-id="' + `${wrapperId}` + '"] input[type=checkbox][name=' + `${name}` + ']');
}

export function destroyChart(container) {
   return new Promise(function (resolve) {
      const id = container.id;
      removeCheckBoxesEvents(id);
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
         delete window.tinyWidgets[id];
      }
      resolve();
   });
}
