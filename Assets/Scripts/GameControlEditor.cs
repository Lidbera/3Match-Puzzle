using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GameControl))]
public class GameControlEditor : Editor
{
    protected override void OnHeaderGUI()
    {
        base.OnHeaderGUI();
    }

    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();

        GameControl control = (GameControl)target;

        GUILayout.BeginHorizontal(GUILayout.MaxWidth(50));
        GUILayout.Label("x, y: ");
        control.tsize.x = EditorGUILayout.IntField(control.tsize.x);
        control.tsize.y = EditorGUILayout.IntField(control.tsize.y);
        if (GUILayout.Button("맵 크기 변경"))
        {
            control.size = control.tsize;
        }
        GUILayout.EndHorizontal();

        if (control.size.x > 0 && control.size.y > 0)
        {
            int temp = 0;
            for (int j = control.size.y - 1; j >= 0; j--)
            {
                GUILayout.BeginHorizontal(GUILayout.MaxWidth(100));
                for (int i = 0; i < control.size.x; i++)
                {
                    control.tmap[temp] = EditorGUILayout.Toggle(control.tmap[temp]);
                    temp++;
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.Label("체크된 부분은 장애물");
        }

        GUILayout.BeginHorizontal(GUILayout.MaxWidth(50));
        if (GUILayout.Button("장애물 초기화"))
        {
            for(int i = 0; i < 100; i++)
            {
                control.tmap[i] = false;
            }
        }
        if (GUILayout.Button("맵 초기화"))
        {
            control.tsize = new Coord(0, 0);
            control.size = new Coord(0, 0);
        }
        GUILayout.EndHorizontal();
    }
}
