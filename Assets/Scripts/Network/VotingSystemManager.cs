using UnityEngine;
using Unity.Netcode;

public struct VotingQuestion
{
    string question;
    string[] answers;

    public VotingQuestion(string question, string[] answers)
    {
        this.question = question;
        this.answers = answers;
    }
}

public class VotingSystemManager : NetworkBehaviour
{
    // public void PresentQuestion()
    // {
    //     if (IsServer)
    //     {
    //         VotingQuestion question = new VotingQuestion("What is the best color?", new string[] { "Red", "Green", "Blue" });

    //         PresentQuestionClientRpc(question);
    //     }
    // }

    // [ClientRpc]
    // private void PresentQuestionClientRpc(VotingQuestion question)
    // {
    //     // Present the question to the client
    // }

    // public void SubmitVote(int answerIndex)
    // {
    //     if (IsClient)
    //     {
    //         SubmitVoteServerRpc(answerIndex);
    //     }
    // }

    // [ServerRpc]
    // private void SubmitVoteServerRpc(int answerIndex)
    // {
    //     // Submit the vote
    // }
}