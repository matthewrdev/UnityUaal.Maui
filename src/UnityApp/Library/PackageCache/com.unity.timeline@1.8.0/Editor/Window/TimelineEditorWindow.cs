using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace UnityEditor.Timeline
{
    /// <summary>
    /// Base class of the TimelineWindow.
    /// </summary>
    public abstract class TimelineEditorWindow : EditorWindow
    {
        /// <summary>
        /// Use this interface to navigate between Timelines and Sub-Timelines. (RO)
        /// </summary>
        public abstract TimelineNavigator navigator { get; }

        /// <summary>
        /// Use this interface to control the playback behaviour of the Timeline window. (RO)
        /// </summary>
        public abstract TimelinePlaybackControls playbackControls { get; }
        /// <summary>
        /// Retrieves and sets the Timeline Window lock state. When disabled (false), the window focus follows the Unity selection.
        /// </summary>
        /// <remarks>When the lock state transitions from true to false, the focused timeline is synchronized with the Unity selection.</remarks>>
        public abstract bool locked { get; set; }
        /// <summary>
        /// Sets which TimelineAsset is shown in the TimelineWindow.
        /// </summary>
        /// <param name="sequence">The TimelineAsset to show. Specify a null to clear the TimelineWindow.</param>
        /// <remarks>When you call this method, the TimelineWindow is placed in asset edit mode. This mode does not support all features. For example, bindings are not available and the timeline cannot be evaluated.
        /// You can use this method when the TimelineWindow is locked.</remarks>
        public abstract void SetTimeline(TimelineAsset sequence);
        /// <summary>
        /// Sets which TimelineAsset is shown in the TimelineWindow based on the PlayableDirector.
        /// </summary>
        /// <param name="director">The PlayableDirector associated with the TimelineAsset to show in the TimelineWindow. Specify a null to clear the TimelineWindow.</param>
        /// <remarks>You can use this method when the TimelineWindow is locked.</remarks>
        public abstract void SetTimeline(PlayableDirector director);
        /// <summary>
        /// Clears the TimelineAsset that is shown in the TimelineWindow.
        /// </summary>
        /// <remarks>You can use this method when the TimelineWindow is locked.</remarks>>
        public abstract void ClearTimeline();
    }
}
