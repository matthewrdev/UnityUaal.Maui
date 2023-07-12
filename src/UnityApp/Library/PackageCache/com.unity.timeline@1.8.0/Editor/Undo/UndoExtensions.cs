using System;
using System.Collections.Generic;
using UnityEditor.Timeline.Actions;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using Object = UnityEngine.Object;

namespace UnityEditor.Timeline
{
    /// <summary>
    /// Provides methods that record the state of a timeline, and its components, prior to modification.
    /// </summary>
    /// <remarks>
    /// The methods in this class are not required when adding or deleting tracks, clips, or markers.
    /// Use methods in the UnityEngine.Timeline namespace, such as <see cref="UnityEngine.Timeline.TimelineAsset.CreateTrack"/>
    /// or <see cref="UnityEngine.Timeline.TrackAsset.CreateDefaultClip"/>, to apply the appropriate
    /// Undo calls when using the Editor.
    /// </remarks>
    public static class UndoExtensions
    {
        /// <summary>
        /// Records changes to all items contained in an action context.
        /// </summary>
        /// <param name="context">The action context to record into the Undo system.</param>
        /// <param name="undoTitle">The title of the action that appears in the undo history. For example, this title is shown in the Undo menu.</param>
        public static void RegisterContext(ActionContext context, string undoTitle)
        {
            using (var undo = new UndoScope(undoTitle))
            {
                undo.Add(context.tracks);
                undo.Add(context.clips, true);
                undo.Add(context.markers);
            }
        }

        /// <summary>
        /// Records changes to timeline asset properties.
        /// This method does not record changes to tracks or clips.
        /// </summary>
        /// <param name="asset">The timeline asset being modified.</param>
        /// <param name="undoTitle">The title of the action that appears in the undo history. For example, this title is shown in the Undo menu.</param>
        public static void RegisterTimeline(TimelineAsset asset, string undoTitle)
        {
            using (var undo = new UndoScope(undoTitle))
                undo.AddObject(asset);
        }

        /// <summary>
        /// Records all timeline changes including changes to tracks, clips, and markers.
        /// </summary>
        /// <param name="asset">The timeline asset being modified.</param>
        /// <param name="undoTitle">The title of the action that appears in the undo history. For example, this title is shown in the Undo menu.</param>
        public static void RegisterCompleteTimeline(TimelineAsset asset, string undoTitle)
        {
            if (asset == null)
                return;

            using (var undo = new UndoScope(undoTitle))
            {
                undo.AddObject(asset);
                undo.Add(asset.flattenedTracks);
                foreach (var t in asset.flattenedTracks)
                {
                    undo.Add(t.GetClips(), true);
                    undo.Add(t.GetMarkers());
                }
            }
        }

        /// <summary>
        /// Records changes to tracks and clips but not to markers nor PlayableAssets attached to clips.
        /// </summary>
        /// <param name="asset">The timeline track being modified.</param>
        /// <param name="undoTitle">The title of the action that appears in the undo history. For example, this title is shown in the Undo menu.</param>
        public static void RegisterTrack(TrackAsset asset, string undoTitle)
        {
            using (var undo = new UndoScope(undoTitle))
                undo.AddObject(asset);
        }

        /// <summary>
        /// Records changes to tracks. This includes changes
        /// to clips on these tracks, but not changes to markers nor PlayableAssets attached to clips.
        /// </summary>
        /// <param name="tracks">The timeline track being modified.</param>
        /// <param name="undoTitle">The title of the action that appears in the undo history. For example, this title is shown in the Undo menu.</param>
        public static void RegisterTracks(IEnumerable<TrackAsset> tracks, string undoTitle)
        {
            using (var undo = new UndoScope(undoTitle))
                undo.Add(tracks);
        }

        /// <summary>
        /// Records changes to a clip.
        /// </summary>
        /// <param name="clip">The timeline clip being modified.</param>
        /// <param name="undoTitle">The title of the action that appears in the undo history. For example, this title is shown in the Undo menu.</param>
        /// <param name="includePlayableAsset">Set this value to true to record changes to the attached playable asset.</param>
        public static void RegisterClip(TimelineClip clip, string undoTitle, bool includePlayableAsset = true)
        {
            using (var undo = new UndoScope(undoTitle))
            {
                undo.AddClip(clip, includePlayableAsset);
            }
        }

        /// <summary>
        /// Records changes to a PlayableAsset.
        /// </summary>
        /// <param name="asset">The PlayableAsset being modified.</param>
        /// <param name="undoTitle">The title of the action that appears in the undo history. For example, this title is shown in the Undo menu.</param>
        public static void RegisterPlayableAsset(PlayableAsset asset, string undoTitle)
        {
            using (var undo = new UndoScope(undoTitle))
                undo.AddObject(asset);
        }

        /// <summary>
        /// Records changes to clips.
        /// </summary>
        /// <param name="clips">The timeline clips being modified.</param>
        /// <param name="name">The title of the action that appears in the undo history. For example, this title is shown in the Undo menu.</param>
        /// <param name="includePlayableAssets">Set this value to true to also record changes to attached playable assets.</param>
        public static void RegisterClips(IEnumerable<TimelineClip> clips, string name, bool includePlayableAssets = true)
        {
            using (var undo = new UndoScope(name))
                undo.Add(clips, includePlayableAssets);
        }

        /// <summary>
        /// Records changes to a timeline marker.
        /// </summary>
        /// <param name="marker">The timeline marker being modified.</param>
        /// <param name="undoTitle">The title of the action that appears in the undo history. For example, this title is shown in the Undo menu.</param>
        public static void RegisterMarker(IMarker marker, string undoTitle)
        {
            using (var undo = new UndoScope(undoTitle))
            {
                if (marker is Object o)
                    undo.AddObject(o);
                else if (marker != null)
                    undo.AddObject(marker.parent);
            }
        }

        /// <summary>
        /// Records changes to timeline markers.
        /// </summary>
        /// <param name="markers">The timeline markers being modified.</param>
        /// <param name="undoTitle">The title of the action that appears in the undo history. For example, this title is shown in the Undo menu.</param>
        public static void RegisterMarkers(IEnumerable<IMarker> markers, string undoTitle)
        {
            using (var undo = new UndoScope(undoTitle))
                undo.Add(markers);
        }

        /// <summary>
        /// This class provides an object which prevents the creation of undos for all Timeline operations. Undos are restored when the object is disposed.
        /// </summary>
        /// <remarks>
        /// Use this class to procedurally create or modify TimelineAssets when undos are not needed.
        /// If multiple DisableTimelineUndoScope instances are created, undos are only restored after all instances are disposed.
        ///
        /// It is recommended to use this object within a using scope.
        /// </remarks>
        /// <example>
        /// <code>
        /// var timelineAsset = new TimelineAsset();
        /// using (new DisableTimelineUndoScope())
        /// {
        ///     //Creates a track without generating an undo
        ///     timelineAsset.CreateTrack<ActivationTrack>();
        /// }
        /// </code>
        /// </example>
        internal sealed class DisableTimelineUndoScope : IDisposable
        {
            TimelineUndo.DisableUndoScope m_DisableScope;

            /// <summary>
            /// Creates a new DisableTimelineUndoScope object which prevents undos from being created by Timeline operations.
            /// </summary>
            public DisableTimelineUndoScope()
            {
                m_DisableScope = new TimelineUndo.DisableUndoScope();
            }

            /// <summary>
            /// Disposes the DisableTimelineUndoScope object.
            /// </summary>
            public void Dispose()
            {
                m_DisableScope.Dispose();
            }
        }
    }
}
