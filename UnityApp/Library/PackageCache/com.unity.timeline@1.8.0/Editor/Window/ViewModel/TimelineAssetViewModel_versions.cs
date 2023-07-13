using UnityEngine;
using UnityEngine.Serialization;

namespace UnityEditor.Timeline
{
    partial class TimelineAssetViewModel
    {
        //used for file format versioning purposes
        [SerializeField] int m_Version;

#pragma warning disable 649
        [FormerlySerializedAs("timeAreaPlayRange")]
        [SerializeField]
        Vector2 deprecated_timeAreaPlayRange;
#pragma warning restore 649


        enum Versions
        {
            Default = 0,
            PlayRangeAsStruct = 1,
            Current = PlayRangeAsStruct
        }

        void UpgradeIfNecessary()
        {
            if (m_Version < (int)Versions.PlayRangeAsStruct)
                Upgrade_PlayRangeAsStruct();
            m_Version = (int)Versions.Current;
        }

        void Upgrade_PlayRangeAsStruct()
        {
            m_TimeAreaPlayRange = new PlayRange(deprecated_timeAreaPlayRange.x, deprecated_timeAreaPlayRange.y);
        }
    }
}
