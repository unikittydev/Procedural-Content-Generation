using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PCG
{
    public abstract class ScriptableStructObject<T> : ScriptableObject where T : unmanaged
    {
        [SerializeField] private T _value;

        public T value
        {
            get => _value;
            set
            {
                #if UNITY_EDITOR
                Undo.RecordObject(this, $"Set: {nameof(T)}");
                #endif

                _value = value;
                
                #if UNITY_EDITOR
                EditorUtility.SetDirty(this);
                #endif
            }
        }
    }
}