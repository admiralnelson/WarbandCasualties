Imports HarmonyLib
Imports TaleWorlds.Core
Imports TaleWorlds.Library
Imports TaleWorlds.Localization
Imports TaleWorlds.MountAndBlade
Imports TaleWorlds.MountAndBlade.ViewModelCollection.HUD.KillFeed

Public Module StringConstants
    Public ReadOnly KnockedUnconciouBysStr As New TextObject("{=admiralnelson_knocked_unconscious}{agent1} knocked unconscious by {agent2}")
    Public ReadOnly KilledByStr As New TextObject("{=admiralnelson_killed_by}{agent1} killed by {agent2}")
    Public ReadOnly DamageDeliveredStr As New TextObject("{=admiralnelson_delivered_damage}Delivered {amount} damage")
    Public ReadOnly CouchedDamageStr As New TextObject("{=admiralnelson_delivered_couch_damage}Delivered {amount} couched lance damage!")
    Public ReadOnly FriendlyFireStr As New TextObject("{=admiralnelson_friendly_fire}Friendly Fire!")
    Public ReadOnly HeadShotStr As New TextObject("{=admiralnelson_friendly_fire}Headshot!")
End Module
Public Module Colours
    Public ReadOnly ColourUnconciousOurSide = Color.ConvertStringToColor("#FFA862FF")
    Public ReadOnly ColourDeathOurSide = Color.ConvertStringToColor("#9AD7B4FF")
    Public ReadOnly ColourUnconciousAllySide = Color.ConvertStringToColor("#EEA4FFFF")
    Public ReadOnly ColourDeathAllySide = Color.ConvertStringToColor("#F11CB5FF")
    Public ReadOnly ColourKill = Color.ConvertStringToColor("#9AD7B4FF")
    Public ReadOnly ColourRed = Color.ConvertStringToColor("#FF0000FF")
    Public ReadOnly ColourGold = Color.ConvertStringToColor("#FFD700FF")
    Public Function GetColour(theAgent As Agent, isDead As Boolean) As Color
        If isDead Then
            If theAgent.Team.IsPlayerAlly Then
                Return ColourDeathAllySide
            End If
            If theAgent.Team.IsPlayerTeam Then
                Return ColourDeathOurSide
            End If
        Else
            If theAgent.Team.IsPlayerAlly Then
                Return ColourUnconciousAllySide
            End If
            If theAgent.Team.IsPlayerTeam Then
                Return ColourUnconciousOurSide
            End If
        End If
        Return ColourKill
    End Function
End Module
Public Class WarbandFeedPatch

    <HarmonyPatch(GetType(SPKillFeedVM), "OnAgentRemoved")>
    Public Class OnAgentRemoved
        Public Shared Function Prefix(ByRef affectedAgent As Agent, ByRef affectorAgent As Agent, isHeadshot As Boolean) As Boolean
            If affectedAgent.State = AgentState.Killed Then
                Dim str = KilledByStr.SetTextVariable("agent1", affectedAgent.Name) _
                                     .SetTextVariable("agent2", affectorAgent.Name)
                Print(str.ToString(), GetColour(affectedAgent, affectedAgent.State = AgentState.Killed))
                Return False
            End If
            If affectedAgent.State = AgentState.Unconscious Then
                Dim str = KnockedUnconciouBysStr.SetTextVariable("agent1", affectedAgent.Name) _
                                                .SetTextVariable("agent2", affectorAgent.Name)
                Print(str.ToString(), GetColour(affectedAgent, affectedAgent.State = AgentState.Killed))
                Return False
            End If
            Return False
        End Function
    End Class

    <HarmonyPatch(GetType(SPKillFeedVM), "OnPersonalKill")>
    Public Class OnPersonallKill
        Public Shared Function Prefix(damageAmount As Integer, isMountDamage As Boolean, isFriendlyFire As Boolean, isHeadshot As Boolean, killedAgentName As String, isUnconscious As Boolean) As Boolean
            If isFriendlyFire Then
                Print(FriendlyFireStr.ToString(), ColourRed)
                Return False
            End If
            If isHeadshot Then
                Print(HeadShotStr.ToString(), ColourGold)
            End If
            Return False
        End Function
    End Class

End Class