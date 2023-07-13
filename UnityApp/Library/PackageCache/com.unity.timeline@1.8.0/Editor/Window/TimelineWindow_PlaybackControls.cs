using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Timeline;

namespace UnityEditor.Timeline
{
    partial class TimelineWindow
    {
        /// <summary>
        /// Internal class that implements TimelinePlaybackControls
        /// </summary>
        internal class TimelinePlaybackControlsImpl : ITimelinePlaybackControls
        {
            public TimelinePlaybackControlsImpl(IWindowStateProvider window)
            {
                if (window == null)
                    throw new ArgumentNullException(nameof(window),
                        "TimelineNavigator cannot be used with a null window");
                m_Window = window;
            }

            public void Play()
            {
                windowState.SetPlaying(true);
            }

            public void Pause()
            {
                windowState.SetPlaying(false);
            }

            public void PreviousFrame()
            {
                windowState.editSequence.frame--;
            }

            public void NextFrame()
            {
                windowState.editSequence.frame++;
            }

            public void GoToFirstFrame()
            {
                windowState.editSequence.time = 0;
            }

            public void GoToLastFrame()
            {
                windowState.editSequence.time = windowState.editSequence.duration;
            }

            public void SetCurrentTime(double time, TimelinePlaybackControls.Context context)
            {
                ISequenceState targetSequenceState = GetTargetSequenceState(context);
                targetSequenceState.time = time;
            }

            public void SetCurrentFrame(int frame, TimelinePlaybackControls.Context context)
            {
                ISequenceState targetSequenceState = GetTargetSequenceState(context);
                targetSequenceState.frame = frame;
            }

            public double GetCurrentTime(TimelinePlaybackControls.Context context)
            {
                ISequenceState targetSequenceState = GetTargetSequenceState(context);
                return targetSequenceState.time;
            }

            public int GetCurrentFrame(TimelinePlaybackControls.Context context)
            {
                ISequenceState targetSequenceState = GetTargetSequenceState(context);
                return targetSequenceState.frame;
            }

            ISequenceState GetTargetSequenceState(TimelinePlaybackControls.Context context)
            {
                switch (context)
                {
                    case TimelinePlaybackControls.Context.Global:
                        return windowState.masterSequence;
                    case TimelinePlaybackControls.Context.Local:
                        return windowState.editSequence;
                    default:
                        throw new ArgumentException("Unknown Context", nameof(context));
                }
            }

            IWindowState windowState
            {
                get
                {
                    if (m_Window == null || m_Window.windowState == null)
                        throw new InvalidOperationException("The Window associated to this instance has been destroyed");
                    return m_Window.windowState;
                }
            }

            readonly IWindowStateProvider m_Window;
        }

        public override TimelinePlaybackControls playbackControls => new TimelinePlaybackControls(this);
    }
}
