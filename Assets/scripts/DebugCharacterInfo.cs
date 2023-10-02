using UnityEngine;

public class DebugCharacterInfo : MonoBehaviour
{
    private Vector3 initialPosition;

    private void Start()
    {
        // �������� ��������� ������� ��'���� ��� �����
        initialPosition = transform.position;
    }

    private void Update()
    {
        // ��������� ������� ������� � ����������
        if (transform.position != initialPosition)
        {
            Debug.Log("Character position changed by another script: " + GetCallerScriptName());
        }
    }

    private string GetCallerScriptName()
    {
        // �������� ��'� �������, ���� �������� ��� �����.
        System.Diagnostics.StackTrace stackTrace = new System.Diagnostics.StackTrace();
        System.Diagnostics.StackFrame callerFrame = stackTrace.GetFrame(2); // �������� �����, �� ���� �� ��� ��� �� ���������.

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
