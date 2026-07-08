#if FMOD_INSTALLED

using System;
using System.Runtime.InteropServices;
using DG.Tweening;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace Workshop.Scaffolding.Nature.Scripts.Audio
{
    public class TimelineInfo   
    {
        public int    CurrentMusicBar;
        public int    CurrentBeat;
        public float  Tempo;
        public string LastMarker;
    }
    
    public static class TimelineBeatService
    {
        private static TimelineInfo _timelineInfo;
        private static GCHandle _timelineInfoHandle;
        private static EVENT_CALLBACK _beatCallback;

        private static EventReference _eventRef;
        private static EventInstance _eventInst;
        
        private static Tween  _pollTween;
        private static int    _lastBar  = -1;
        private static int    _lastBeat = -1;
        private static string _lastMarker = string.Empty;
        
        public static event Action<int, int> OnBeat;
        public static event Action<string>   OnMarker;
        
        // Static classes' set fields persist across editor runs, this ensures those get cleared.
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStaticState()
        {
            // Only runs at actual domain/session start - safe to force-free defensively here,
            // since no native callback can still be pending against a prior session's handle.
            if (_timelineInfoHandle.IsAllocated) _timelineInfoHandle.Free();
            
            ClearData();

            Application.quitting -= Stop;
            Application.quitting += Stop;
        }

        public static void Start(EventReference eventRef, Vector3 position)
        {
            _timelineInfo = new TimelineInfo();
            _eventRef = eventRef;

            // Create delegate object & store it so it won't get GC'd.
            _beatCallback = BeatEventCallback;
            
            _eventInst = RuntimeManager.CreateInstance(_eventRef);
            
            // Pin the class that will store the data modified during the callback.
            _timelineInfoHandle = GCHandle.Alloc(_timelineInfo);
            
            _eventInst.setUserData(GCHandle.ToIntPtr(_timelineInfoHandle));
            _eventInst.setCallback(_beatCallback, EVENT_CALLBACK_TYPE.TIMELINE_BEAT | EVENT_CALLBACK_TYPE.TIMELINE_MARKER | EVENT_CALLBACK_TYPE.DESTROYED);
            _eventInst.set3DAttributes(position.To3DAttributes());
            _eventInst.start();
            
            StartPolling();
        }

        private static void StartPolling()
        {
            // This runs every rendered frame & calls `Poll`
            _pollTween = DOVirtual.Float(0f, 1f, 0.1f, _ => Poll())
                .SetLoops(-1, LoopType.Restart)
                .SetUpdate(true);
        }

        private static void Poll()
        {
            if (_timelineInfo == null) return;
            
            var bar  = _timelineInfo.CurrentMusicBar;
            var beat = _timelineInfo.CurrentBeat;
            
            // If bar & beat haven't changed, don't call event.
            if (bar == _lastBar && beat == _lastBeat) return;
            _lastBar  = bar;
            _lastBeat = beat;
            
            OnBeat?.Invoke(bar, beat);
            
            // If marker hasn't changed, don't call event.
            if (string.IsNullOrEmpty(_timelineInfo.LastMarker)) return;
            var marker = _timelineInfo.LastMarker;
            
            if (marker.Equals(_lastMarker)) return;
            _lastMarker = marker;
            
            OnMarker?.Invoke(marker);
        }

        public static void Stop()
        {
            _eventInst.stop(STOP_MODE.ALLOWFADEOUT);
            _eventInst.release(); // DESTROYED callback frees the handle once FMOD actually tears the instance down.
            ClearData();
        }

        // This does not run on the Unity main thread; it's not advisable to drive any game code form here (hence, the polling).
        [AOT.MonoPInvokeCallback(typeof(EVENT_CALLBACK))]
        private static RESULT BeatEventCallback(
            EVENT_CALLBACK_TYPE type, 
            IntPtr              instancePtr, 
            IntPtr              parameters)
        {
            var inst = new EventInstance(instancePtr);

            // Get user data.
            var result = inst.getUserData(out var timelineInfoPtr);
            if (result != RESULT.OK)
            {
                UnityEngine.Debug.LogError($"[Workshop] [TimelineBeatService] callback error: {result}");
                return result;
            }
            if (timelineInfoPtr != IntPtr.Zero)
            {
                // Get object to store data.
                var timelineHandle = GCHandle.FromIntPtr(timelineInfoPtr);
                var timelineInfo   = (TimelineInfo)timelineHandle.Target;

                switch (type)
                {
                    case EVENT_CALLBACK_TYPE.TIMELINE_BEAT:
                    {
                        var param = (TIMELINE_BEAT_PROPERTIES)Marshal.PtrToStructure(parameters, typeof(TIMELINE_BEAT_PROPERTIES));
                        timelineInfo.CurrentMusicBar = param.bar;
                        timelineInfo.CurrentBeat     = param.beat;
                        timelineInfo.Tempo           = param.tempo;
                        break;
                    }
                    case EVENT_CALLBACK_TYPE.TIMELINE_MARKER:
                    {
                        var param = (TIMELINE_MARKER_PROPERTIES)Marshal.PtrToStructure(parameters, typeof(TIMELINE_MARKER_PROPERTIES));
                        timelineInfo.LastMarker = param.name;
                        break;
                    }
                    case EVENT_CALLBACK_TYPE.DESTROYED:
                        timelineHandle.Free();
                        break;
                }
            }
            return RESULT.OK;
        }
        
        private static void ClearData()
        {
            _pollTween?.Kill();
            _pollTween    = null;
            _lastBar      = -1;
            _lastBeat     = -1;
            _lastMarker   = string.Empty;
            _timelineInfo = null;
            _eventInst    = default;
            _eventRef     = default;
            _beatCallback = null;
        }
    }
}

#endif