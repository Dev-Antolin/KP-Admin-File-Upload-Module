Imports ML.Plugin.Authentication

Module Module1
    Public username As String = UserHandle.Current.CurrentUser.Username
    Public branchcode As String = UserHandle.Current.CurrentUser.BranchCode
    Public zonecode As Integer = UserHandle.Current.CurrentUser.ZoneCode
    Public branchname As String = UserHandle.Current.CurrentUser.BranchName
    Public stationcode As String = UserHandle.Current.CurrentUser.StationCode
    Public vfile As Double
    'Public usernameFname As String = UserHandle.Current.CurrentUser.userFirstName
    'Public usernameLname As String = UserHandle.Current.CurrentUser.userLastName
    'Public userFullname As String = usernameFname + " " + usernameLname
    Public operatorID As String = UserHandle.Current.CurrentUser.Username
    Public stationID As String = UserHandle.Current.CurrentUser.Stationno
    Public PartnersNameIdentifier As String
    Public partnersSession As String
    Public listResp As List(Of String)
    Public RespMsg As String
    Public MLbatchNumber As String
    Public Intresp As String
    Public NameOfPartners As String
End Module
