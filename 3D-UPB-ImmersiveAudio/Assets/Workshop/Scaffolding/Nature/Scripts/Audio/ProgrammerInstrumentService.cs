#if FMOD_INSTALLED

using System.Runtime.InteropServices;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Workshop.Scaffolding.Nature.Scripts.Audio
{
    public static class ProgrammerInstrumentService
    {
        // ---------- Sound data ----------

        /// <summary>
        /// Carries the raw PCM data needed to create an FMOD Sound inside the callback.
        /// Passed via GCHandle through the unmanaged callback boundary.
        /// </summary>
        private class SoundData
        {
            public readonly float[] Samples;
            public readonly int     Channels;
            public readonly int     Frequency;

            public SoundData(AudioClip clip)
            {
                Samples   = new float[clip.samples * clip.channels];
                Channels  = clip.channels;
                Frequency = clip.frequency;
                clip.GetData(Samples, 0);
            }
        }

        // ---------- Static callback ----------

        // Must be static for IL2CPP / AOT compatibility.
        private static readonly EVENT_CALLBACK Callback = ProgrammerSoundCallback;

        [AOT.MonoPInvokeCallback(typeof(EVENT_CALLBACK))]
        private static RESULT ProgrammerSoundCallback(
            EVENT_CALLBACK_TYPE type,
            System.IntPtr       eventPtr,
            System.IntPtr       parameterPtr)
        {
            var inst = new EventInstance(eventPtr);
            if (!inst.isValid()) return RESULT.OK;

            switch (type)
            {
                // FMOD needs to create the internal data for this sound.
                case EVENT_CALLBACK_TYPE.CREATE_PROGRAMMER_SOUND:
                {
                    inst.getUserData(out var userData);
                    if (userData == System.IntPtr.Zero) return RESULT.ERR_INVALID_PARAM;

                    var handle = GCHandle.FromIntPtr(userData);
                    if (handle.Target is not SoundData data) return RESULT.ERR_INVALID_PARAM;

                    var sound = CreateSound(data);
                    if (!sound.hasHandle()) return RESULT.ERR_INVALID_PARAM;

                    var props = (PROGRAMMER_SOUND_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(PROGRAMMER_SOUND_PROPERTIES));
                    props.sound         = sound.handle;
                    props.subsoundIndex = -1;
                    Marshal.StructureToPtr(props, parameterPtr, false);
                    break;
                }

                // FMOD is done with the sound — release it.
                case EVENT_CALLBACK_TYPE.DESTROY_PROGRAMMER_SOUND:
                {
                    var props = (PROGRAMMER_SOUND_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(PROGRAMMER_SOUND_PROPERTIES));
                    var sound = new Sound(props.sound);
                    if (sound.hasHandle()) sound.release();
                    break;
                }

                // Event instance destroyed — free the GCHandle.
                case EVENT_CALLBACK_TYPE.DESTROYED:
                {
                    inst.getUserData(out var userData);
                    if (userData == System.IntPtr.Zero) break;

                    inst.setUserData(System.IntPtr.Zero);
                    var handle = GCHandle.FromIntPtr(userData);
                    if (handle.IsAllocated) handle.Free();
                    break;
                }
            }

            return RESULT.OK;
        }

        // ---------- Public API ----------

        /// <summary>
        /// Plays a Unity AudioClip through an FMOD Programmer Instrument event.
        /// Fire-and-forget — no cleanup needed by the caller.
        /// </summary>
        public static void SetupAndStartWithAudioClip(EventInstance eventInst, AudioClip clip)
        {
            if (clip == null)
            {
                Debug.LogError("[ProgrammerInstrumentService] AudioClip is null.");
                return;
            }

            // Pack clip data and pass it through the callback boundary via GCHandle.
            var handle = GCHandle.Alloc(new SoundData(clip));
            eventInst.setUserData(GCHandle.ToIntPtr(handle));
            eventInst.setCallback(
                Callback,
                EVENT_CALLBACK_TYPE.CREATE_PROGRAMMER_SOUND  |
                EVENT_CALLBACK_TYPE.DESTROY_PROGRAMMER_SOUND |
                EVENT_CALLBACK_TYPE.DESTROYED);

            eventInst.start();
            eventInst.release(); // Safe for one-shots: FMOD keeps the instance alive until playback finishes.
        }

        // ---------- Internal: sound creation ----------

        private static Sound CreateSound(SoundData data)
        {
            var byteLen = data.Samples.Length * sizeof(float);

            var exinfo = new CREATESOUNDEXINFO
            {
                cbsize           = Marshal.SizeOf<CREATESOUNDEXINFO>(),
                length           = (uint)byteLen,
                format           = SOUND_FORMAT.PCMFLOAT,
                numchannels      = data.Channels,
                defaultfrequency = data.Frequency
            };

            // Pin only during createSound — MODE.OPENMEMORY means FMOD copies the buffer.
            // Safe to unpin immediately after.
            var tempHandle = GCHandle.Alloc(data.Samples, GCHandleType.Pinned);
            RESULT result;
            Sound  sound;
            try
            {
                result = RuntimeManager.CoreSystem.createSound(
                    tempHandle.AddrOfPinnedObject(),
                    MODE.OPENMEMORY | MODE.OPENRAW | MODE.LOOP_OFF,
                    ref exinfo,
                    out sound);
            }
            finally
            {
                tempHandle.Free();
            }
            if (result != RESULT.OK)
            {
                Debug.LogError($"[ProgrammerInstrumentService] createSound failed: {result}");
                sound.clearHandle();
            }
            return sound;
        }
    }
}

#endif