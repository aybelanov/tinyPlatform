'use strict';

import './js/widgets.js';
import { addCss, addScript } from './js/common.widgets.js';
addCss('_content/Clients.Widgets/lib/video.js/dist/video-js.min.css');

window.tinyWidgets = window.tinyWidgets || {};

export function initPlayer(container, objRef, culture, theme, tokenKey, endpoint, isLive) {
   if (!container || !objRef)
      return;

   return new Promise(function (resolve) {
      addScript('_content/Clients.Widgets/lib/video.js/dist/video.js', function () {
         addScript(`_content/Clients.Widgets/lib/video.js/dist/lang/${culture}.js`, function () {

            let playListLink;

            window.HELP_IMPROVE_VIDEOJS = false;            
            const playerOptions = {
               liveui: isLive,
               language: culture,
               preload: 'none',
               playbackRates: isLive ? [] : [0.5, 1, 1.5, 2, 3, 5, 10],
               responsitive: true,
               //children: isLive ? ['controlBar'] : null
               //children: [
               //   'bigPlayButton',
               //   'controlBar'
               //]
            };

            const widget =
               window.tinyWidgets[container.id] = {
                  container: container,
                  objectref: objRef,
                  player: buildPlayer(),
                  play,
                  destroy,
                  stop,
                  updatePlaylist
               };

            function addJwt() {
               const playerXhrRequestHook = (options) => {
                  if (options.uri.includes(endpoint)) {
                     options.beforeSend = function (xhr) {
                        xhr.setRequestHeader('Authorization', 'Bearer ' + JSON.parse(sessionStorage.getItem('oidc.user:' + (tokenKey || '')))['access_token']);
                     };
                  }
                  else {
                     options.uri = playListLink;
                  }
                  return options;
               };
               //widget.player.tech().vhs.xhr.onRequest(playerXhrRequestHook);
               videojs.Vhs.xhr.onRequest(playerXhrRequestHook);
            }
            widget.player.on('xhr-hooks-ready', addJwt);

            function createPlaylist(segments, targetDuration) {
               let playList = '#EXTM3U\n';
               playList += '#EXT-X-VERSION:3\n';
               playList += '#EXT-X-INDEPENDENT-SEGMENTS\n';
               playList += `#EXT-X-TARGETDURATION:${targetDuration}\n`;
               playList += '#EXT-X-MEDIA-SEQUENCE:0\n';
               playList += '#EXT-X-PLAYLIST-TYPE:VOD\n';

               let i = 0; const length = segments[0].length;
               for (i = 0; i < length; i++) {
                  // if the segment sequence has discontinuties
                  // it is the requred tag (to avoid media error code 3)
                  playList += '#EXT-X-DISCONTINUITY\n';
                  playList += `#EXTINF:${segments[0][i]},\n`;
                  playList += endpoint + '/' + segments[1][i] + '.ts\n';
               }

               if (!isLive) {
                  playList += '#EXT-X-ENDLIST\n';
               }

               return playList;
            }

            function updatePlaylist(segments, targetDuration) {
               const playList = createPlaylist(segments, targetDuration);
               const playListAsBlob = new Blob([playList], { type: 'application/x-mpegURL' });
               const _temp = playListLink;
               playListLink = URL.createObjectURL(playListAsBlob);
               URL.revokeObjectURL(_temp);
            }

            function play(segments, targetDuration) {
               const playList = createPlaylist(segments, targetDuration);
               const playListAsBlob = new Blob([playList], { type: 'application/x-mpegURL' });
               playListLink = URL.createObjectURL(playListAsBlob);
               widget.player.src({
                  type: 'application/x-mpegURL',
                  src: playListLink
                  //src: `data:application/x-mpegURL;base64,${btoa(playList)}`
               });
               widget.player.play();
            }

            function stop() {
               widget.player.src(undefined);
               widget.player.reset();
            }

            function buildPlayer() {
               const player = videojs(container, playerOptions, function () { resolve(); });
               return player;
            }

            function destroy() {
               widget.player.off('xhr-hooks-ready', addJwt);
               URL.revokeObjectURL(playListLink);
            }
         });
      });
   });
}

export function play(containerId, segments, targetDuration) {
   if (window.tinyWidgets[containerId]) {
      window.tinyWidgets[containerId].play(segments, targetDuration);
   }
}

export function updatePlaylist(containerId, segments, targetDuration) {
   if (window.tinyWidgets[containerId]) {
      window.tinyWidgets[containerId].updatePlaylist(segments, targetDuration);
   }
}

export function stop(containerId) {
   if (window.tinyWidgets[containerId]) {
      window.tinyWidgets[containerId].stop();
   }
}

export function pause(containerId) {
   if (window.tinyWidgets[containerId]) {
      window.tinyWidgets[containerId].player.pause();
   }
}

export function destroyPlayer(container) {
   const id = container.id;
   if (window.tinyWidgets[id]) {
      window.tinyWidgets[id].destroy();
      if (window.tinyWidgets[id].objectref) {
         window.tinyWidgets[id].objectref.dispose();
      }
      delete window.tinyWidgets[id].objectref;
      delete window.tinyWidgets[id].container;
      if (window.tinyWidgets[id].player) {
         window.tinyWidgets[id].player.dispose();
      }
      delete window.tinyWidgets[id].player;
      delete window.tinyWidgets[id];
   }
}