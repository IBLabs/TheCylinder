using UnityEngine;

[RequireComponent(typeof(CodeGenerator))]
public class RelayJoinCodeRevealer : MonoBehaviour
{
    public void OnRelayJoinCodeReceived(string code)
    {
        CodeGenerator codeGenerator = GetComponent<CodeGenerator>();
        codeGenerator.OnRelayJoinCodeReceived(code);
    }
}
