using System.Collections.Generic;
using UnityEngine;

namespace Util
{
    public abstract class ObjectPool<T> : MonoBehaviour where T : Component
    {
        public GameObject pooledObject;
        public int pooledAmount = 5;
        public bool willGrow = true;

        List<T> pooledObjects = new List<T>();

        private void Awake()
        {
            for (var i = 0; i < pooledAmount; i++)
            {
                var obj = AddChild(gameObject, pooledObject); 
                obj.SetActive(false);
                pooledObjects.Add(obj.GetComponent<T>());
            }
        }

        public T GetPooledObject()
        {
            foreach (var t in pooledObjects)
            {
                if (!t.gameObject.activeInHierarchy)
                {
                    return (T)t;
                }
            }

            if (willGrow)
            {
                GameObject obj = AddChild(gameObject, pooledObject);
                var newT = obj.GetComponent<T>();
                pooledObjects.Add(newT);
                return newT;
            }
            return null;
        }


        GameObject AddChild(GameObject parent, GameObject prefab)
        {
            var go = GameObject.Instantiate(prefab) as GameObject;
#if UNITY_EDITOR
            UnityEditor.Undo.RegisterCreatedObjectUndo(go, "Create Object");
#endif
            if (go != null && parent != null)
            {
                var t = go.transform;
                t.parent = parent.transform;
                t.localPosition = Vector3.zero;
                t.localRotation = Quaternion.identity;
                t.localScale = Vector3.one;
                go.layer = parent.layer;
            }
            return go;
        }
    }
}