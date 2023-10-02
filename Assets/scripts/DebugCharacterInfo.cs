using UnityEngine;

public class DebugCharacterInfo : MonoBehaviour
{
    private Vector3 initialPosition;

    private void Start()
    {
        // Зберігаємо початкову позицію об'єкта при старті
        initialPosition = transform.position;
    }

    private void Update()
    {
        // Порівнюємо поточну позицію з початковою
        if (transform.position != initialPosition)
        {
            Debug.Log("Character position changed by another script: " + GetCallerScriptName());
        }
    }

    private string GetCallerScriptName()
    {
        // Отримуємо ім'я скрипта, який викликав цей метод.
        System.Diagnostics.StackTrace stackTrace = new System.Diagnostics.StackTrace();
        System.Diagnostics.StackFrame callerFrame = stackTrace.GetFrame(2); // Вибираємо фрейм, що вище на два рівні від поточного.

        if (callerFrame != null)
        {
            System.Type callerScriptType = callerFrame.GetMethod().DeclaringType;
            if (callerScriptType != null)
            {
                return callerScriptType.Name;
            }
        }

        return "Unknown";
    }
}
