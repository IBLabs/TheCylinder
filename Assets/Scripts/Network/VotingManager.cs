using Unity.Netcode;
using UnityEngine;

public class VotingManager : NetworkBehaviour
{
    private NetworkVariable<int> votesForOptionA = new NetworkVariable<int>(0);
    private NetworkVariable<int> votesForOptionB = new NetworkVariable<int>(0);

    // Called by the host to send a new question to all clients
    public void SendQuestion(string question, string optionA, string optionB)
    {
        if (IsServer)
        {
            ResetVotes();
            SendQuestionToClientsClientRpc(question, optionA, optionB);
        }
    }

    private void ResetVotes()
    {
        votesForOptionA.Value = 0;
        votesForOptionB.Value = 0;
    }

    [ClientRpc]
    void SendQuestionToClientsClientRpc(string question, string optionA, string optionB)
    {
        Debug.Log($"{question}\n1. {optionA}\n2. {optionB}");
        // Implement UI here to show the options and allow voting
    }

    public void SubmitVote(int option)
    {
        if (IsClient)
        {
            SubmitVoteServerRpc(option);
        }
    }

    [ServerRpc]
    void SubmitVoteServerRpc(int option)
    {
        if (option == 1)
            votesForOptionA.Value += 1;
        else if (option == 2)
            votesForOptionB.Value += 1;
    }

    // Optionally, the host can call this method to finalize the voting and announce the result
    public void EndVoting()
    {
        if (IsServer)
        {
            AnnounceResults();
        }
    }

    private void AnnounceResults()
    {
        Debug.Log($"Voting Results:\nOption 1: {votesForOptionA.Value} votes\nOption 2: {votesForOptionB.Value} votes");
        // Implement additional logic to handle what happens after the results are announced
    }
}