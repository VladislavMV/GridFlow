using System.Collections;
using System.Collections.Generic;
using UnityEditor; // Необхідно для роботи з редактором [00:06:19]
using UnityEngine;

// Атрибут вказує, що цей редактор працює для AbstractDungeonGenerator та всіх його спадкоємців [00:06:35]
[CustomEditor(typeof(AbstractDungeonGenerator), true)]
public class RandomDungeonGeneratorEditor : Editor
{
    AbstractDungeonGenerator generator;

    private void Awake()
    {
        // Отримуємо посилання на скрипт, який зараз відображається в інспекторі [00:07:27]
        generator = (AbstractDungeonGenerator)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI(); // Малює стандартні поля (Iterations, Walk Length тощо) [00:08:14]

        // Створюємо кнопку в інспекторі [00:08:21]
        if (GUILayout.Button("Create Dungeon"))
        {
            generator.GenerateDungeon();
        }
    }
}