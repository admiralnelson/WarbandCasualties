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
    Public ReadOnly DamageReceivedStr As New TextObject("{=admiralnelson_received_damage}Received {amount} damage")
    Public ReadOnly CouchedDamageStr As New TextObject("{=admiralnelson_delivered_couch_damage}Delivered {amount} couched lance damage!")
    Public ReadOnly FriendlyFireStr As New TextObject("{=admiralnelson_friendly_fire}Friendly Fire!")
    Public ReadOnly HeadShotStr As New TextObject("{=admiralnelson_friendly_fire}Headshot!")
    Public ReadOnly ChargedMountStr As New TextObject("{=admiralnelson_charged_mount}Mount charged for {amount} damage")
    Public ReadOnly SpeedBonusStr As New TextObject("{=admiralnelson_speed_bonus}Relative movement speed: {amount} m/s")
    Public ReadOnly HitDistanceStr As New TextObject("{=admiralnelson_hit_distance}Distance: {amount} m")
End Module
Public Module Colours
    Public ReadOnly ColourUnconciousOurSide = Color.ConvertStringToColor("#FFA862FF")
    Public ReadOnly ColourDeathOurSide = Color.ConvertStringToColor("#AF6353FF")
    Public ReadOnly ColourUnconciousAllySide = Color.ConvertStringToColor("#EEA4FFFF")
    Public ReadOnly ColourDeathAllySide = Color.ConvertStringToColor("#F11CB5FF")
    Public ReadOnly ColourKill = Color.ConvertStringToColor("#9AD7B4FF")
    Public ReadOnly ColourRed = Color.ConvertStringToColor("#FF0000FF")
    Public ReadOnly ColourGold = Color.ConvertStringToColor("#FFD700FF")
    Public ReadOnly ColourDamage = Color.ConvertStringToColor("#FFAAAAFF")
    Public Function GetColour(theAgent As Agent, isDead As Boolean) As Color
        If isDead Then
            If theAgent.Team.IsPlayerTeam Then
                Return ColourDeathOurSide
            End If
            If theAgent.Team.IsPlayerAlly Then
                Return ColourDeathAllySide
            End If
        Else
            If theAgent.Team.IsPlayerTeam Then
                Return ColourUnconciousOurSide
            End If
            If theAgent.Team.IsPlayerAlly Then
                Return ColourUnconciousAllySide
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
            Return False
        End Function
    End Class
    <HarmonyPatch(GetType(SPKillFeedVM), "OnPersonalDamage")>
    Public Class OnPersonalDamage
        Public Shared Function Prefix(totalDamage As Integer, isVictimAgentMount As Boolean, isFriendlyFire As Boolean, victimAgentName As String) As Boolean
            Return False
        End Function
    End Class


    <HarmonyPatch(GetType(CombatLogManager), "GenerateCombatLog")>
    Public Class GenerateCombatLog
        Public Shared Function Prefix(logData As CombatLogData) As Boolean
            If logData.TotalDamage <= 0 Then Return False
            If logData.IsVictimAgentMount Then Return False
            'this log is generated from bot POV, that why we check against human
            If IsAttackingHuman(logData) Then
                Dim str = DamageReceivedStr.SetTextVariable("amount", logData.TotalDamage)
                Print(str.ToString(), ColourDamage)
                Return False
            End If
            If logData.IsAttackerAgentMount Then
                If logData.TotalDamage > 0 Then
                    Dim str = ChargedMountStr.SetTextVariable("amount", logData.TotalDamage)
                    Print(str.ToString(), Color.White)
                End If
            Else
                If logData.TotalDamage > 0 Then
                    Dim damstr = DamageDeliveredStr.SetTextVariable("amount", logData.TotalDamage)
                    Print(damstr.ToString(), Color.White)
                End If
                If logData.HitSpeed > 0.0001 AndAlso Not logData.IsRangedAttack Then
                    Dim str = SpeedBonusStr.SetTextVariable("amount", logData.HitSpeed.ToString("N3"))
                    Print(str.ToString(), Color.White)
                ElseIf logData.IsRangedAttack Then
                    Dim str = HitDistanceStr.SetTextVariable("amount", logData.HitSpeed.ToString("N3"))
                    Print(str.ToString(), Color.White)
                    If logData.IsFatalDamage Then
                        Print(HeadShotStr.ToString(), ColourGold)
                    End If
                End If
            End If

            Return False
        End Function
    End Class


End Class