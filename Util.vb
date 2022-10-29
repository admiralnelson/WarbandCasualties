Imports TaleWorlds.Library
Imports TaleWorlds.MountAndBlade

Public Module Util
    Public Sub Print(s As String, Optional colour As Color = Nothing)
        If IsNothing(colour) Then colour = Color.White
        InformationManager.DisplayMessage(New InformationMessage(s, colour))
    End Sub

    Public Function IsAttackingHuman(combatlog As CombatLogData) As Boolean
        If Not combatlog.IsVictimAgentHuman Then
            Return combatlog.DoesVictimAgentHaveRiderAgent AndAlso combatlog.IsVictimAgentRiderAgentMine
        End If
        Return combatlog.IsVictimAgentMine
    End Function
End Module
