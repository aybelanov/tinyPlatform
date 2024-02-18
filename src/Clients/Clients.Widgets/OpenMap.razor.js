import './js/widgets.js';
import Map from './lib/ol/Map.js';
import Feature from './lib/ol/Feature.js';
import OSM from './lib/ol/source/OSM.js';
import BingMaps from './lib/ol/source/BingMaps.js';
import TileLayer from './lib/ol/layer/Tile.js';
import View from './lib/ol/View.js';
import { fromLonLat } from './lib/ol/proj.js';
import { FullScreen, ScaleLine, defaults as defaultControls } from './lib/ol/control.js';
import { containsCoordinate } from './lib/ol/extent.js';
import Point from './lib/ol/geom/Point.js';
import LineString from './lib/ol/geom/LineString.js';
import VectorSource from './lib/ol/source/Vector.js';
import Overlay from './lib/ol/Overlay.js';
import { Circle, Fill, RegularShape, Icon, Stroke, Style, Text } from './lib/ol/style.js';
//import { Modify, Select, defaults as defaultInteractions } from './lib/ol/interaction.js';
import { getVectorContext } from './lib/ol/render.js';
import VectorLayer from './lib/ol/layer/Vector.js';
import { addCss } from './js/common.widgets.js';
addCss('_content/Clients.Widgets/lib/ol/ol.css');

window.tinyWidgets = window.tinyWidgets || {};

export function initMap(container, objRef, cfg) {
   if (!container || !objRef)
      return;

   /** mouse tootip info*/
   let pointerTooltip;
   /** mouse tooltip marker*/
   const pointerTooltipMarker = { point: null, line: null };
   /** player tootip info*/
   let playerTooltip;
   /** player tooltip marker*/
   const playerTooltipMarker = { point: null };
   /** is route animating*/
   let isAnimating;
   /** last animating time (for marker move)*/
   let lastTime;
   /** played distance*/
   let distance = 0;
   /** player marker*/
   let playMarker;
   /** current route*/
   let route;
   /**player marker position*/
   let position;
   /** player play/pause button's toggle flag*/
   let playPauseToggle = true;

   // initial view
   cfg.view = cfg.view || {};
   cfg.view.layerType = cfg.view.layerType || 'OSM';
   cfg.view.culture = cfg.view.culture || 'en-US';
   cfg.view.culture = cfg.view.culture.toLowerCase();

   const bingLayerTypes = [
      'Aerial',
      'AerialWithLabelsOnDemand',
      'CanvasDark',
      'CanvasLight',
      'CanvasGray',
      'OrdnanceSurvey',
      'RoadOnDemand'
   ];

   const layers = [
      new TileLayer({
         name: 'OSM',
         source: new OSM(),
         visible: cfg.view.layerType === 'OSM'
      })
   ];
   let i, lenght;
   for (i = 0, lenght = bingLayerTypes.length; i < lenght; ++i) {
      layers.push(
         new TileLayer({
            name: bingLayerTypes[i],
            visible: bingLayerTypes[i] === cfg.view.layerType,
            preload: Infinity,
            source: new BingMaps({
               key: cfg.apiKey,
               culture: cfg.view.culture,
               imagerySet: bingLayerTypes[i],
               placeholderTiles: false
            }),
         })
      );
   }

   let vectorLayer = new VectorLayer({
      name: 'mapObjects',
      source: new VectorSource({ features: [] }),
      visible: true,
      style: getFeatureStyle
   });

   layers.push(vectorLayer);

   const map = new Map({
      target: container.id,
      controls: defaultControls().extend([
         new FullScreen({
            source: 'wrapper-' + container.id,
         }),
         new ScaleLine({
            units: 'metric',
            bar: true,
            steps: 4,
            text: true,
            minWidth: 140,
         })
      ]),
      layers: layers,
      view: new View({
         center: fromLonLat([cfg.view.center.lon, cfg.view.center.lat], 'EPSG:3857'),
         zoom: cfg.view.zoom,
         maxZoom: 20
      }),
   });

   const widget =
   window.tinyWidgets[container.id] = {
      container: container,
      objectref: objRef,
      olmap: map,
      options: prepareOptions(),
      controls: {},
      disposed: false,
      showPath,
      updateOnlinePath,
      changeLayer,
      showObjects,
      showAllOnMap,
      startTrackAnimation,
      pauseTrackAnimation,
      stopTrackAnimation,
      dispose
   };

   createTooltipInfo();   
   createPlayTooltipInfo();
   map.on('click', clickHandler);
   map.on('pointermove', pointerViewHandler);

   // prepare animatiion scrollbar
   if (cfg.scrollbarId) {
      const el = document.getElementById(cfg.scrollbarId);
      widget.controls.scrollbar = {
         element: el,
         min: Number(el.min),
         range: Number(el.max) - Number(el.min)
      };
      el.addEventListener("change", scrollBarOnChangeHandler);
      el.addEventListener("input", scrollBarOnInputHandler);
   }

   // prepare play button
   if (cfg.playButtonId) {
      const el = document.getElementById(cfg.playButtonId);
      widget.controls.playButton = el;
      el.addEventListener("click", onPlayButtonClick);
   }

   // prepare stop button
   if (cfg.stopButtonId) {
      const el = document.getElementById(cfg.stopButtonId);
      widget.controls.stopButton = el;
      el.addEventListener("click", onStopButtonClick);
   }

   // prepare fit (show all objects) button
   if (cfg.fitButtonId) {
      const el = document.getElementById(cfg.fitButtonId);
      widget.controls.fitButton = el;
      el.addEventListener("click", onFitButtonClick);
   }

   // map's fit button handler
   function onFitButtonClick(evt) {
      showAllOnMap();
   }

   // player's stop button event handler
   function onStopButtonClick(evt) {
      stopTrackAnimation();
   }

   // player's play button event handler
   function onPlayButtonClick(evt) {
      if (playPauseToggle) {
         startTrackAnimation();
      } else {
         pauseTrackAnimation();
      }
   }

   // sets player to play station
   function setPalyerToPlay() {
      if (widget.controls.playButton) {
         widget.controls.playButton.getElementsByTagName('i')[0].innerText = 'play_arrow';
      }
      playPauseToggle = true;
   }

   // sets palyer to pause station
   function setPalyerToPause() {
      if (widget.controls.playButton) {
         widget.controls.playButton.getElementsByTagName('i')[0].innerText = 'pause';
      }
      playPauseToggle = false;
   }

   // scrollbar event handlers
   function scrollBarOnInputHandler(evt) {
      if (playMarker) {
         vectorLayer.un('postrender', playFeature);
         const _distance = Number(evt.target.value) / widget.controls.scrollbar.range;
         distance = distance > 1 ? 2 - _distance : _distance;
         const currentCoordinate = route.getCoordinateAt(distance > 1 ? 2 - distance : distance);
         position.setCoordinates(currentCoordinate);
         pauseTrackAnimation();
      }
   }
   function scrollBarOnChangeHandler(evt) {
      //if (playMarker) {
      //  distance = Number(evt.target.value) / (Number(evt.target.max) - Number(evt.target.min));
      //  const currentCoordinate = route.getCoordinateAt(distance > 1 ? 2 - distance : distance);
      //  position.setCoordinates(currentCoordinate);
      //  pauseTrackAnimation();
      //}
   }

   // prepare widget option
   function prepareOptions() {
      const colors = {
         'blue1': 'rgba(0, 0, 130, 0.9)',
         'blue2': 'rgba(0, 99, 255, 0.9)',
         'blue3': 'rgba(0, 160, 255, 0.9)',
         'green1': 'rgba(0, 123, 0, 0.9)',
         'green2': 'rgba(0, 172, 0, 0.9)',
         'green3': 'rgba(15, 255, 0, 0.9)',
         'yellow1': 'rgba(255, 255, 0, 0.9)',
         'yellow2': 'rgba(255, 200, 0, 0.9)',
         'yellow3': 'rgba(255, 125, 0, 0.9)',
         'red1': 'rgba(255, 60, 0, 0.9)',
         'red2': 'rgba(255, 125, 0, 0.9 )',
         'red3': 'rgba(255, 0, 0, 0.9)'
      }

      const fills = {}, strokes = {}, segmentStyles = {}, arrowStyles = {};
      for (const prop in colors) {
         fills[prop] = new Fill({ color: colors[prop] });
         strokes[prop] = new Stroke({ color: colors[prop], width: 4 });
         segmentStyles[prop] = new Style({
            fill: fills[prop],
            stroke: strokes[prop]
         });
         arrowStyles[prop] = new Style({
            image: new RegularShape({
               points: 3,
               radius: 8,
               fill: fills[prop],
               rotateWithView: true,
               scale: [1, 1.5]
            })
         });
      }

      const tooltipStyle = new Style({
         stroke: new Stroke({
            color: 'rgba(191,191,191,0.9)',
            width: 1,
         }),
         image: new Circle({
            radius: 5,
            fill: new Fill({ color: 'rgba(255,255,255, 1)' }),
            stroke: new Stroke({
               color: 'rgba(191,191,191,1)',
               width: 2,
            }),
         }),
         zIndex: 9999
      });

      const playMarkerStyle = new Style({
         image: new Circle({
            radius: 5,
            fill: new Fill({ color: 'rgba(255,255,255, 1)' }),
            stroke: new Stroke({
               color: 'rgba(191,191,191,1)',
               width: 3,
            }),
         }),
         zIndex: 9999
      });

      const options = {
         colors,
         fills,
         strokes,
         segmentStyles,
         arrowStyles,
         tooltipStyle,
         playMarkerStyle,
         getColorBySpeed: function (speed) {

            let color = null;

            if (speed < 10) color = 'blue1';
            else if (speed >= 10 & speed < 20) color = 'blue2';
            else if (speed >= 20 & speed < 30) color = 'blue3';
            else if (speed >= 30 & speed < 40) color = 'green1';
            else if (speed >= 40 & speed < 50) color = 'green2';
            else if (speed >= 50 & speed < 60) color = 'green3';
            else if (speed >= 60 & speed < 70) color = 'yellow1';
            else if (speed >= 70 & speed < 80) color = 'yellow2';
            else if (speed >= 80 & speed < 90) color = 'yellow3';
            else if (speed >= 90 & speed < 100) color = 'red1';
            else if (speed >= 100 & speed < 110) color = 'red2';
            else if (speed >= 110) color = 'red3';

            return color;
         },
         playerSpeed: 1,
         scrollbar: null
      }

      return options;
   };

   // feature style func
   function getFeatureStyle(feature) {

      const type = feature.get('type');
      let style = null;

      if (type === 'routeSegment') {
         //line
         const firstCoordinate = feature.getGeometry().getFirstCoordinate();
         //const speed = firstCoordinate[3]; 
         const color = widget.options.getColorBySpeed(firstCoordinate[3]);
         style = [widget.options.segmentStyles[color]];

         // arrow as line end's style
         const zoom = map.getView().getZoom();
         if (zoom >= 15) {
            const start = firstCoordinate;
            const end = feature.getGeometry().getLastCoordinate();
            const startPixel = map.getPixelFromCoordinate(start);
            const endPixel = map.getPixelFromCoordinate(end);
            const distPixel = ((endPixel[1] - startPixel[1]) ** 2 + (endPixel[0] - startPixel[0]) ** 2) ** (1 / 2);
            const segmentId = feature.getId();
            if (distPixel > 100 || segmentId % 5 === 0) {
               const arrowStyle = widget.options.arrowStyles[color];
               const dx = end[0] - start[0];
               const dy = end[1] - start[1];
               const rotation = Math.atan2(dx, dy);
               arrowStyle.setGeometry(new Point(end));
               const arrow = arrowStyle.getImage();
               arrow.setDisplacement([0, -arrow.getRadius()]);
               arrow.setRotation(rotation);
               style.push(arrowStyle);
            }
         }
      }

      else if (type === 'onlineSegment') {
         //line
         const firstCoordinate = feature.getGeometry().getFirstCoordinate();
         const color = widget.options.getColorBySpeed(firstCoordinate[3]);
         style = [widget.options.segmentStyles[color]];

         // arrow as line end's style for last segment
         if (feature.isLast) {
            const start = firstCoordinate;
            const end = feature.getGeometry().getLastCoordinate();
            const arrowStyle = widget.options.arrowStyles[color];
            const dx = end[0] - start[0];
            const dy = end[1] - start[1];
            const rotation = Math.atan2(dx, dy);
            arrowStyle.setGeometry(new Point(end));
            const arrow = arrowStyle.getImage();
            arrow.setDisplacement([0, -arrow.getRadius()]);
            arrow.setRotation(rotation);
            style.push(arrowStyle);
         }
      }

      else if (type === 'onlinePoint') {
         //line
         const coordinate = feature.getGeometry().getFirstCoordinate();
         const color = widget.options.getColorBySpeed(coordinate[3]);
         const arrowStyle = widget.options.arrowStyles[color];
         arrowStyle.setGeometry(new Point(coordinate));
         const arrow = arrowStyle.getImage();
         arrow.setDisplacement([0, -arrow.getRadius()]);
         const rotation = coordinate[4] > 180 ? coordinate[4] - 360 : coordinate[4];
         arrow.setRotation(rotation * Math.PI / 180);
         style = [arrowStyle];
      }

      else if (type === 'deviceIcon') {
         const data = feature.get('data');
         style = new Style({
            image: new Icon({
               anchor: [0.5, 1],
               src: data.icon,
            }),
            text: new Text({
               text: data.name,
               font: 'bold 16px Calibri,sans-serif',
               fill: new Fill({
                  color: 'white',
               }),
               stroke: new Stroke({
                  color: data.textColor,
                  width: 2,
               }),
               offsetX: 18,
               offsetY: -22,
               textAlign: 'left',
               textBaseline: 'middle'
            }),
         });
      }

      else if (type === 'startMarker') {
         style = new Style({
            image: new Icon({
               anchor: [0.25, 0.9],
               src: 'img/flag_start.svg',
            }),
         });
      }

      else if (type === 'endMarker') {
         style = new Style({
            image: new Icon({
               anchor: [0.5, 0.9],
               src: 'img/mobile_green.svg',
            }),
         });
      }

      else if (type === 'playMarker') {
         style = widget.options.playMarkerStyle;
      }

      return style;
   };

   // creates mouse tooltip info
   function createTooltipInfo() {
      //if (tooltipInfoElement) {
      //   tooltipInfoElement.parentNode.removeChild(tooltipInfoElement);
      //}
      const doomElement = document.createElement('div');
      doomElement.className = 'ol-tooltip ol-tooltip-measure';
      //tooltipInfoElement.style.display = 'none';
      pointerTooltip = new Overlay({
         element: doomElement,
         offset: [0, -15],
         positioning: 'bottom-center',
         stopEvent: false,
         insertFirst: false,
      });
      pointerTooltip.setPosition(undefined);
      map.addOverlay(pointerTooltip);
   }

   // creates player tooltip info
   function createPlayTooltipInfo() {
      const doomElement = document.createElement('div');
      doomElement.className = 'ol-tooltip ol-tooltip-measure';
      playerTooltip = new Overlay({
         element: doomElement,
         offset: [0, -15],
         positioning: 'bottom-center',
         stopEvent: false,
         insertFirst: false,
      });
      playerTooltip.setPosition(undefined);
      map.addOverlay(playerTooltip);
   }

   // mouse pointer type event hsndler
   function pointerViewHandler(evt) {
      const map = widget.olmap;
      map.getTargetElement().style.cursor = map.hasFeatureAtPixel(evt.pixel) ? 'pointer' : '';
   };

   // mouse tooltip event handler
   function pointerTooltipHandler(evt) {
      if (evt.dragging) return;
      if (route) {
         const coord = map.getEventCoordinate(evt.originalEvent);
         const closestFeature = vectorLayer.getSource().getClosestFeatureToCoordinate(coord);

         if (closestFeature === null) { //|| closestFeature.get('type') !== 'routeSegment') {
            pointerTooltipMarker.point = null;
            pointerTooltipMarker.line = null;
            pointerTooltip.setPosition(undefined);
         } else {
            const geometry = closestFeature.getGeometry();
            const closestPoint = geometry.getClosestPoint(coord);
            const mousePixel = map.getPixelFromCoordinate(coord);
            const pointixel = map.getPixelFromCoordinate(closestPoint);
            const distPixel = ((pointixel[1] - mousePixel[1]) ** 2 + (pointixel[0] - mousePixel[0]) ** 2) ** (1 / 2);

            if (distPixel <= 50) {
               if (pointerTooltipMarker.point === null) {
                  pointerTooltipMarker.point = new Point(closestPoint);
               } else {
                  pointerTooltipMarker.point.setCoordinates(closestPoint);
               }
               const coordinates = [coord, [closestPoint[0], closestPoint[1]]];
               if (pointerTooltipMarker.line === null) {
                  pointerTooltipMarker.line = new LineString(coordinates);
               } else {
                  pointerTooltipMarker.line.setCoordinates(coordinates);
               }
               pointerTooltip.setPosition(closestPoint);
               const firstCoordinate = geometry.getFirstCoordinate();
               const speed = firstCoordinate[3];
               const date =  new Date(firstCoordinate[2]).toLocaleString(cfg.view.culture);
               pointerTooltip.getElement().innerHTML = `<span>${date}</span><br/><span>${speed} km/h</span>`;
            } else {
               pointerTooltipMarker.point = null;
               pointerTooltipMarker.line = null;
               pointerTooltip.setPosition(undefined);
            }
         }
         map.render();
      }
   };

   // mouse tooltip render event handler
   function pointerTooltipRender(evt) {
      if (route && (pointerTooltipMarker.point !== null || pointerTooltipMarker.line !== null)) {
         const vectorContext = getVectorContext(evt);
         vectorContext.setStyle(widget.options.tooltipStyle);

         if (pointerTooltipMarker.point !== null) 
            vectorContext.drawGeometry(pointerTooltipMarker.point);
         
         if (pointerTooltipMarker.line !== null) 
            vectorContext.drawGeometry(pointerTooltipMarker.line);
      }
   }

   // mouse map out event handler
   function mouseMapOut() {
      pointerTooltipMarker.point = null;
      pointerTooltipMarker.line = null;
      pointerTooltip.setPosition(undefined);
      map.render();
   }

   // marker click event handler
   function clickHandler(evt) {
      const feature = map.forEachFeatureAtPixel(evt.pixel, function (feature) {
         return feature;
      });
      if (!feature) {
         return;
      }
      if (feature.get('type') === 'deviceIcon') {
         const link = feature.get('data').link;
         if (link) {
            const dotNetHelper = widget.objectref;
            dotNetHelper.invokeMethodAsync("MapLink", link);
            //location.href = link;
         }
      }
   };

   // converts inbound data to markers 
   function getMarkers(data) {
      const mapObjects = [];
      if (data) {
         let i = 0, length;
         for (i = 0, length = data[0].length; i < length; i++) {
            mapObjects.push({
               id: data[0][i],
               icon: data[1][i],
               name: data[2][i],
               content: data[3][i],
               point: [data[4][i], data[5][i]],
               visible: data[6][i],
               textColor: data[7][i],
               link: data[8][i],
            });
         }
      }
      return mapObjects;
   };

   // changes background map layer type: osm, sat, hybrid and etc.
   function changeLayer(newLayer) {
      try {
         widget.olmap.getLayers().getArray()
            .filter(layer => layer.get('name') !== 'mapObjects')
            .forEach(layer => layer
               .setVisible(layer.get('name') === newLayer));
      }
      catch { }
   }

   // clear widget objects and events
   function clearWidget() {
      container.removeEventListener('mouseout', mouseMapOut);
      map.un('pointermove', pointerTooltipHandler);
      vectorLayer.un('postrender', pointerTooltipRender);
      vectorLayer.un('postrender', playFeature);
      route = null;
      distance = 0;
      playMarker = null;
      position = null;
      pointerTooltipMarker.point = null;
      pointerTooltipMarker.line = null;
      playerTooltipMarker.point = null;
      pointerTooltip.setPosition(undefined);
      playerTooltip.setPosition(undefined);
      if (widget.controls.scrollbar) {
         widget.controls.scrollbar.element.value = widget.controls.scrollbar.min;
      }
      setPalyerToPlay();
      const source = new VectorSource({ features: [] });
      const objectLayer = map.getLayers().getArray().filter(layer => layer.get('name') === 'mapObjects')[0];
      objectLayer.setSource(source);
      objectLayer.changed();
   }

   // shows track (path) on the map
   function showPath(data) {
      clearWidget();
      if (data[0].length > 0) {
         const coordinates = [];
         for (let i = 0, length = data[0].length; i < length; i++) {
            coordinates.push([data[1][i], data[2][i], data[0][i], data[3][i]]);
         }

         let m = 0; const segmentFeatures = [];
         route = new LineString(coordinates).transform('EPSG:4326', 'EPSG:3857');
         route.forEachSegment((start, end) => {
            const lineString = new LineString([start, end]);
            const segmentFeature = new Feature({
               type: 'routeSegment',
               geometry: lineString,
            });
            segmentFeature.setId(m);
            segmentFeatures.push(segmentFeature);
            m++;
         });

         if (coordinates.length > 1) {
            const startMarker = new Feature({ type: 'startMarker', geometry: new Point(route.getFirstCoordinate()) });
            segmentFeatures.push(startMarker);

            // player feature 
            playMarker = new Feature({
               type: 'playMarker'
               //geometry: new Point(route.getFirstCoordinate()),
            });
            //playMarker.setStyle(widget.options.playMarkerStyle);
            segmentFeatures.push(playMarker);
            position = startMarker.getGeometry().clone();
         }

         const endMarker = new Feature({ type: 'endMarker', geometry: new Point(coordinates[coordinates.length - 1]).transform('EPSG:4326', 'EPSG:3857') });
         segmentFeatures.push(endMarker);

         container.addEventListener('mouseout', mouseMapOut);
         map.on('pointermove', pointerTooltipHandler);
         vectorLayer.on('postrender', pointerTooltipRender);

         const source = new VectorSource({ features: segmentFeatures });
         vectorLayer.setSource(source);
         vectorLayer.changed();
      }
   }

   // online tracking
   function updateOnlinePath(data) {
      container.removeEventListener('mouseout', mouseMapOut);
      map.un('pointermove', pointerTooltipHandler);
      vectorLayer.un('postrender', pointerTooltipRender);

      if (data[0].length > 0) {

         const coordinates = [];
         for (let i = 0, length = data[0].length; i < length; i++) {
            coordinates.push([data[1][i], data[2][i], data[0][i], data[3][i], data[4][i]]);
         }

         const segmentFeatures = [];
         route = new LineString(coordinates).transform('EPSG:4326', 'EPSG:3857');

         // only point
         if (coordinates.length < 2) {
            const onlyPointFeature = new Feature({
               type: 'onlinePoint',
               geometry: new Point(route.getFirstCoordinate())//(coordinates[0].transform('EPSG:4326', 'EPSG:3857'))
            });
            segmentFeatures.push(onlyPointFeature);
         }
         // track
         else {
            let m = 0; 
            route.forEachSegment((start, end) => {
               const segmentFeature = new Feature({
                  type: 'onlineSegment',
                  geometry: new LineString([start, end]),
               });
               segmentFeature.setId(m);
               segmentFeatures.push(segmentFeature);
               m++;
            });

            segmentFeatures[segmentFeatures.length - 1].isLast = true;
         }

         container.addEventListener('mouseout', mouseMapOut);
         map.on('pointermove', pointerTooltipHandler);
         vectorLayer.on('postrender', pointerTooltipRender);

         const source = new VectorSource({ features: segmentFeatures });
         vectorLayer.setSource(source);
         vectorLayer.changed();

         const isInViewport = containsCoordinate(map.getView().calculateExtent(), route.getLastCoordinate());
         if (!isInViewport) {
            //const size = map.getSize();
            //map.getView().centerOn(route.getLastCoordinate(), map.getSize(), [size[0] / 2, size[1] / 2]);
            const view = map.getView();
            const zoom = view.getZoom();
            const extent = new Point(route.getLastCoordinate()).getExtent();
            view.fit(extent, { maxZoom: zoom, minZoom: zoom, duration: 600 });
         };
      }
   }

   // shows map objects: markers, baloons and etc.
   function showObjects(data) {
      const mapObjects = getMarkers(data);
      const features = [];
      let i, length = mapObjects.length;
      for (i = 0; i < length; i++) {
         const point = new Point(fromLonLat(mapObjects[i].point, 'EPSG:3857'));
         const feature = new Feature({
            type: 'deviceIcon',
            geometry: point,
            data: mapObjects[i]
         });
         feature.setId(i);
         features.push(feature);
      }

      const source = new VectorSource({ features: features });
      //const objectLayer = widget.olmap.getLayers().getArray().filter(layer => layer.get('name') === 'mapObjects')[0];
      vectorLayer.setSource(source);
      vectorLayer.changed();
   }

   // shows all map objects on the map window
   function showAllOnMap() {
      const map = widget.olmap;
      const view = map.getView();
      const layer = map.getLayers().getArray().filter(layer => layer.get('name') === 'mapObjects')[0];
      const source = layer.getSource();
      const extent = source.getExtent();
      var isNoData = extent.filter((x) => x === Infinity || x === -Infinity).length === 4;
      if (!isNoData) {
         view.fit(extent, { padding: [50, 50, 50, 50], duration: 1000 });
      }
   }

   // start track player amimation
   function startTrackAnimation() {
      if (playMarker && route) {
         isAnimating = true;
         setPalyerToPause();
         lastTime = Date.now();
         vectorLayer.on('postrender', playFeature);
         // hide playMarker and trigger map render through change event
         pointerTooltip.setPosition(undefined);
         if (playMarker.getGeometry()) playMarker.setGeometry(null);
         else map.render();           
      }
   }

   // stop track player amimation
   function stopTrackAnimation() {
      if (playMarker && route) {
         isAnimating = false;
         setPalyerToPlay();
         vectorLayer.un('postrender', playFeature);
         distance = 0;
         position.setCoordinates(route.getFirstCoordinate());
         playerTooltip.setPosition(undefined);
         playMarker.setGeometry(null);
         if (widget.controls.scrollbar) {
            widget.controls.scrollbar.element.value =
               widget.controls.scrollbar.min;
         }
      }
   }

   // pause track player amimation
   function pauseTrackAnimation() {
      if (playMarker && route) {
         isAnimating = false;
         setPalyerToPlay();
         vectorLayer.un('postrender', playFeature);
         playMarker.setGeometry(position);
         setPlayerTooltipPosition(position.getCoordinates());
      }
   }

   // sets player tooltip position
   function setPlayerTooltipPosition(coordinates) {
      playerTooltip.setPosition(coordinates);
      if (coordinates) {
         const speed = Number(coordinates[3]).toFixed();
         const moment = new Date(coordinates[2]).toLocaleString(cfg.view.culture);
         playerTooltip.getElement().innerHTML = `<span>${moment}</span><br/><span>${speed} km/h</span>`;
      } else {
         playerTooltip.getElement().innerHTML = '';
      }
   }

   // animation render event handler
   function playFeature(event) {
      if (route) {
         const speed = widget.options.playerSpeed;
         const time = event.frameState.time;
         const elapsedTime = time - lastTime;
         distance = (distance + (speed * elapsedTime) / 1e6) % 2;
         lastTime = time;
         const currentCoordinate = route.getCoordinateAt(distance > 1 ? 2 - distance : distance);
         position.setCoordinates(currentCoordinate);
         const vectorContext = getVectorContext(event);
         vectorContext.setStyle(widget.options.playMarkerStyle);
         vectorContext.drawGeometry(position);
         setPlayerTooltipPosition(currentCoordinate);
         //playMarker.setGeometry(position);
         if (widget.controls.scrollbar) {
            widget.controls.scrollbar.element.value =
               widget.controls.scrollbar.min +
               widget.controls.scrollbar.range *
               (distance > 1 ? 2 - distance : distance);
         }
         map.render();
      }
   }

   // disposes widget
   function dispose() {
      map.un('click', clickHandler);
      map.un('pointermove', pointerViewHandler);
      map.un('pointermove', pointerTooltipHandler);
      vectorLayer.un('postrender', pointerTooltipRender);
      vectorLayer.un('postrender', playFeature);
      container.removeEventListener('mouseout', mouseMapOut);
      if (widget.controls.scrollbar) {
         widget.controls.scrollbar.element.removeEventListener("change", scrollBarOnChangeHandler)
         widget.controls.scrollbar.element.removeEventListener("input", scrollBarOnInputHandler)
      }
      if (widget.controls.playButton) {
         widget.controls.playButton.removeEventListener("click", onPlayButtonClick);
      }
      if (widget.controls.stopButton) {
         widget.controls.stopButton.removeEventListener("click", onStopButtonClick);
      }
      if (widget.controls.fitButton) {
         widget.controls.fitButton.removeEventListener("click", onFitButtonClick);
      }
      map.setTarget(null);
      widget.objectref.dispose();
      widget.objectref = null;
      widget.container = null;
      widget.olmap = null;
      widget.options = null;
      widget.controls = null;
      delete widget.objectref;
      delete widget.container;
      delete widget.options;
      delete widget.controls;
      delete widget.olmap;
      widget.disposed = true;
   }
}

export function startTrackAnimation(containerId) {
   if (window.tinyWidgets[containerId]) {
      window.tinyWidgets[containerId].startTrackAnimation();
   }
}

export function stopTrackAnimation(containerId) {
   if (window.tinyWidgets[containerId]) {
      window.tinyWidgets[containerId].stopTrackAnimation();
   }
}

export function pauseTrackAnimation(containerId) {
   if (window.tinyWidgets[containerId]) {
      window.tinyWidgets[containerId].pauseTrackAnimation();
   }
}

export function changeLayer(containerId, newLayer) {
   if (window.tinyWidgets[containerId]) {
      window.tinyWidgets[containerId].changeLayer(newLayer);
   }
}

export function showPath(containerId, data) {
   if (window.tinyWidgets[containerId]) {
      window.tinyWidgets[containerId].showPath(data);
   }
}

export function updateOnlinePath(containerId, data) {
   if (window.tinyWidgets[containerId]) {
      window.tinyWidgets[containerId].updateOnlinePath(data);
   }
}

export function showObjects(containerId, data) {
   if (window.tinyWidgets[containerId]) {
      window.tinyWidgets[containerId].showObjects(data);
   }
}

export function showAllOnMap(containerId) {
   if (window.tinyWidgets[containerId]) {
      window.tinyWidgets[containerId].showAllOnMap();
   }
}

export function setPlayerSpeed(containerId, value) {
   if (window.tinyWidgets[containerId]) {
      window.tinyWidgets[containerId].options.playerSpeed = value;
   }
}

export function destroyOlMap(container) {
   const id = container.id;
   if (window.tinyWidgets[id]) {
      window.tinyWidgets[id].dispose();
      delete window.tinyWidgets[id];
   }
}