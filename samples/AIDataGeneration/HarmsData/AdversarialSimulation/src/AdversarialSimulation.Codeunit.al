// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------
namespace System.TestLibraries.AdversarialSimulation;

/// <summary>
/// Allows AI tests to simulate adversarial attacks. See the README for prerequisites:
///    https://dynamicssmb2.visualstudio.com/Dynamics%20SMB/_git/NAV?path=%2FEng%2FCore%2FTools%2FALTestRunner%2FEvaluation%2FREADME.md
/// 
/// This needs to be internal for now since RAI review would be needed for external use.
/// </summary>
codeunit 135395 "Adversarial Simulation"
{
    /// <summary>
    /// Sets a seed for the random number generator used in the simulation.
    /// If you want to compare the results of two simulations, you should use the same seed.
    /// </summary>
    /// <param name="Seed">The value of the seet</param>
    procedure SetSeed(Seed: Integer)
    begin
        AdversarialSimulationImpl.SetSeed(Seed);
    end;

    /// <summary>
    /// Starts and adversarial, single-turn simulation.
    /// </summary>
    procedure Start()
    begin
        AdversarialSimulationImpl.Start();
    end;

    /// <summary>
    /// Starts and adversarial, multi-turn simulation.
    /// </summary>
    /// <param name="MaxConversationTurns">Maximum number of turns per conversion. Must be at least 2.</param>
    procedure Start(MaxConversationTurns: Integer)
    begin
        AdversarialSimulationImpl.Start(MaxConversationTurns);
    end;

    /// <summary>
    /// Starts and adversarial, single-turn simulation with User Prompt Injection Attacks.
    /// This is intended to be a comparative test, so the results have to be compared
    /// with the result of a single-turn simulation with the same seed.
    /// </summary>
    procedure StartUPIA()
    begin
        AdversarialSimulationImpl.StartUPIA();
    end;

    /// <summary>
    /// Starts and adversarial, multiturn simulation with User Prompt Injection Attacks.
    /// This is intended to be a comparative test, so the results have to be compared
    /// with the result of a multiturn-turn simulation with the same seed.
    /// </summary>
    /// <param name="MaxConversationTurns">Maximum number of turns per conversion. Must be at least 2.</param>
    procedure StartUPIA(MaxConversationTurns: Integer)
    begin
        AdversarialSimulationImpl.StartUPIA(MaxConversationTurns);
    end;

    /// <summary>
    /// Starts and adversarial, single-turn simulation with Cross Prompt Injection Attacks (XPIA).
    /// For each conversation, call GetHarmWithXPIA and use the XPIA attack sentence in a scenario-specific way.
    /// </summary>
    procedure StartXPIA()
    begin
        AdversarialSimulationImpl.StartXPIA();
    end;

    /// <summary>
    /// For a multi-turn simulation, returns true if there is another turn to process in the conversation.
    /// </summary>
    /// <returns>A boolean indicating whether the conversation has another simulated turn.</returns>
    procedure HasNextTurn(): Boolean
    begin
        exit(AdversarialSimulationImpl.HasNextTurn());
    end;

    /// <summary>
    /// Get the next simulated harm
    /// </summary>
    /// <param name="Harm">Simulated harm.</param>
    procedure GetHarm(): Text
    begin
        exit(AdversarialSimulationImpl.GetHarm());
    end;

    /// <summary>
    /// Get the next simulated harm with for a simulation started with StartXPIA.
    /// </summary>
    /// <param name="XPIAAttackSentence">The XPIA attack sentence.</param>
    /// <returns>Simulated harm.</returns>
    procedure GetHarmWithXPIA(var XPIAAttackSentence: Text): Text
    begin
        exit(AdversarialSimulationImpl.GetHarmWithXPIA(XPIAAttackSentence));
    end;

    /// <summary>
    /// Responds to a multi-turn simulation with a stop word. 
    /// This will cause the conversation to stop even if there could be more turns.
    /// </summary>
    procedure RespondWithStopWord()
    begin
        AdversarialSimulationImpl.RespondWithStopWord();
    end;

    /// <summary>
    /// Responds to a simulation with a message.
    /// </summary>
    /// <param name="Response">The response from the AI feature.</param>
    procedure Respond(Response: Text)
    begin
        AdversarialSimulationImpl.Respond(Response);
    end;

    /// <summary>
    /// Sets the base URI for the adversarial simulation. The default is 'http://localhost:8000'.
    /// </summary>
    /// <param name="Uri">URI to set for base URI.</param>
    procedure SetBaseSimulationUri(Uri: Text)
    begin
        AdversarialSimulationImpl.SetBaseSimulationUri(Uri);
    end;

    var
        AdversarialSimulationImpl: Codeunit "Adversarial Simulation Impl.";
}