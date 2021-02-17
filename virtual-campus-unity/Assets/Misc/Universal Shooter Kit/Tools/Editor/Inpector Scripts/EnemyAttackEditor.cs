using UnityEngine;
using UnityEditor;

namespace GercStudio.USK.Scripts
{

    [CustomEditor(typeof(EnemyAttack))]
    public class EnemyAttackEditor : Editor
    {

        public EnemyAttack script;

        public void Awake()
        {
            script = (EnemyAttack) target;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            serializedObject.ApplyModifiedProperties();
       
        }
    }

}


