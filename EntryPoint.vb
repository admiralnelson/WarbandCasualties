Imports HarmonyLib
Imports TaleWorlds.MountAndBlade

Namespace Global.WarbandCasualties
    Public Class EntryPoint
        Inherits MBSubModuleBase
        Protected Overrides Sub OnSubModuleLoad()
            MyBase.OnSubModuleLoad()
            Dim har As New Harmony("org.calradia.admiralnelson.warbandkillfeed.vbnet")
            har.PatchAll()
        End Sub
    End Class

End Namespace