using UnityEditor;

namespace Assets.Editor
{
    [CustomEditor(typeof(BaseTower))]
    class BaseTowerInspector : UnityEditor.Editor
    {
        BaseTower instance;
        PropertyField[] fields;


        public void OnEnable()
        {
            instance = target as BaseTower;
            fields = ExposeProperties.GetProperties(instance);
        }

        public override void OnInspectorGUI()
        {

            if (instance == null)
                return;

            this.DrawDefaultInspector();

            ExposeProperties.Expose(fields);

        }

    }
}
