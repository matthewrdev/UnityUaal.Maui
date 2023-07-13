using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor.Timeline
{
    internal interface ITimelinePlaybackControls
    {
        void Play();
        void Pause();
        void PreviousFrame();
        void NextFrame();
        void GoToFirstFrame();
        void GoToLastFrame();
        void SetCurrentTime(double time, TimelinePlaybackControls.Context context);
        void SetCurrentFrame(int frame, TimelinePlaybackControls.Context context);
        double GetCurrentTime(TimelinePlaybackControls.Context context);
        int GetCurrentFrame(TimelinePlaybackControls.Context context);
    }
    /// <summary>
    /// Use the TimelinePlaybackControls to manage the Timeline window's playback state, playhead location, and play range.
    /// </summary>
    public sealed class TimelinePlaybackControls
    {
        TimelineWindow.TimelinePlaybackControlsImpl m_Impl;

        internal TimelinePlaybackControls(IWindowStateProvider stateProvider)
        {
            m_Impl = new TimelineWindow.TimelinePlaybackControlsImpl(stateProvider);
        }

        /// <summary>
        /// Use Context to specify whether the time is based on local time or global time.
        /// </summary>
        public enum Context
        {
            /// <summary>
            /// Time is relative to the current Timeline
            /// </summary>
            Local,
            /// <summary>
            /// Time is relative to the main Timeline
            /// </summary>
            Global
        }

        /// <summary>
        /// Starts playback.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">The Window associated with this instance has been destroyed.</exception>
        public void Play() { m_Impl.Play(); }

        /// <summary>
        /// Pauses playback.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">The Window associated with this instance has been destroyed.</exception>
        public void Pause() { m_Impl.Pause(); }

        /// <summary>
        /// Moves the playhead to the previous frame.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">The Window associated with this instance has been destroyed.</exception>
        public void PreviousFrame() { m_Impl.PreviousFrame(); }

        /// <summary>
        /// Moves the playhead to the next frame.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">The Window associated with this instance has been destroyed.</exception>
        public void NextFrame() { m_Impl.NextFrame(); }

        /// <summary>
        /// Moves the playhead to the first frame.
        /// </summary>
        /// <exception cref="System.InvalidOperationException"> The Window associated with this instance has been destroyed.</exception>
        public void GoToFirstFrame() { m_Impl.GoToFirstFrame(); }

        /// <summary>
        /// Moves the playhead to the last frame.
        /// </summary>
        /// <exception cref="System.InvalidOperationException"> The Window associated with this instance has been destroyed.</exception>
        public void GoToLastFrame() { m_Impl.GoToLastFrame(); }

        /// <summary>
        /// Moves the playhead to a specific time.
        /// </summary>
        /// <param name="time">The time in seconds.</param>
        /// <param name="context">
        /// Use Context with a Sub-Timeline to specify whether the specified time is relative to the Sub-Timeline or the main Timeline.
        /// If the Timeline is not a Sub-Timeline, the context uses local time regardless of the specified context.
        /// </param>
        /// Use <see cref="Context.Local"/>, the default, to move the playhead relative to the Sub-Timeline or Timeline.
        /// Use <see cref="Context.Global"/> to move the playhead relative to the main Timeline.
        /// <exception cref="System.InvalidOperationException">The Window associated with this instance has been destroyed.</exception>
        /// <exception cref="System.ArgumentException">The context is invalid.</exception>
        public void SetCurrentTime(double time, Context context = Context.Local) { m_Impl.SetCurrentTime(time, context); }

        /// <summary>
        /// Moves the playhead to a specific frame.
        /// </summary>
        /// <param name="frame">The frame to move to.</param>
        /// <param name="context">
        /// Use Context with a Sub-Timeline to specify whether the specified frame is relative to the Sub-Timeline or the main Timeline.
        /// If the Timeline is not a Sub-Timeline, the context uses local time regardless of the specified context.
        /// </param>
        /// Use <see cref="Context.Local"/>, the default, to move the playhead relative to the Sub-Timeine.
        /// Use <see cref="Context.Global"/> to move the playhead relative to the main Timeline.
        /// <exception cref="System.InvalidOperationException">The Window associated with this instance has been destroyed.</exception>
        /// <exception cref="System.ArgumentException">The context is invalid.</exception>
        public void SetCurrentFrame(int frame, Context context = Context.Local) { m_Impl.SetCurrentFrame(frame, context); }

        /// <summary>
        /// Retrieves the location of the timeline playhead in seconds.
        /// </summary>
        /// <param name="context">
        /// Use Context with a Sub-Timeline to specify whether the returned value is relative to the Sub-Timeline or the main Timeline.
        /// If the Timeline is not a Sub-Timeline, the context uses local time regardless of the specified context.
        /// </param>
        /// Use <see cref="Context.Local"/>, the default, to retrieve the playhead location relative to the Sub-Timeline.
        /// Use <see cref="Context.Global"/> to retrive the location relative to the main Timeline.
        /// <exception cref="System.InvalidOperationException">The Window associated with this instance has been destroyed.</exception>
        /// <exception cref="System.ArgumentException">The context is invalid.</exception>
        /// <returns>The playhead location in seconds.</returns>
        public double GetCurrentTime(Context context = Context.Local)
        {
            return m_Impl.GetCurrentTime(context);
        }

        /// <summary>
        /// Retrieves the location of the timeline playhead in frames.
        /// </summary>
        /// <param name="context">
        /// Use Context with a Sub-Timeline to specify whether the returned value is relative to the Sub-Timeline or the main Timeline.
        /// If the Timeline is not a Sub-Timeline, the context uses local time regardless of the specified context.
        /// </param>
        /// Use <see cref="Context.Local"/>, the default, to retrieve the playhead location relative to the Sub-Timeline.
        /// Use <see cref="Context.Global"/> to retrive the playhead location relative to the main Timeline.
        /// <exception cref="System.InvalidOperationException">The Window associated with this instance has been destroyed.</exception>
        /// <exception cref="System.ArgumentException">The context is invalid.</exception>
        /// <returns>The playhead location in frames.</returns>
        public int GetCurrentFrame(Context context = Context.Local)
        {
            return m_Impl.GetCurrentFrame(context);
        }
    }
}
