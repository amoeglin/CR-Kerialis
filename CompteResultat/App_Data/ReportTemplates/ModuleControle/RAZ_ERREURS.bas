Attribute VB_Name = "RAZ_ERREURS"
Sub RAZ_ERREURS()

'VARIABLE
Dim NomModule As String
Dim bValue As Boolean
Dim shErreurs As Worksheet

On Error GoTo err_RAZ_ERREURS

'*** RAZ des lignes dans la feuille Erreurs  ***
Set shErreurs = Worksheets("Erreurs")
NomModule = "RAZ_ERREURS"
bValue = RAZ_shErreurs(shErreurs)
    
GoTo FIN_RAZ_ERREURS:
               
err_RAZ_ERREURS:

  Set shErreurs = Worksheets("Erreurs")
  Application.Cursor = xlDefault
  
 ' recherche du dernier numéro de la ligne <> "" pour permettre d'ajouter un message à la ligne suivante
    p = 1
    While shErreurs.Cells(p, 1) <> ""
    p = p + 1
    Wend
    p = p - 1
    NoLigneEnErreur = p - 1
  
  MsgBox "Erreur dans RAZ_ERREURS() : " & Err.Number & vbLf & Err.Description, vbCritical
  
  ' affichage du message d'erreur dans la feuille "Erreurs"
  MessageErreur = "RAZ_ERREURS() : " & Err.Number & " - " & Err.Description

  NoLigneEnErreur = NoLigneEnErreur + 1
  'shErreurs.Range("A1").Offset(NoLigneEnErreur, 0).Value = shResultats.Range("C2").Offset(NoLigneEnErreur, 0).Value
  shErreurs.Range("A1").Offset(NoLigneEnErreur, 0).value = NoLigneEnErreur
  shErreurs.Range("B1").Offset(NoLigneEnErreur, 0).value = NomModule
  shErreurs.Range("C1").Offset(NoLigneEnErreur, 0).value = LibelleFamille
  shErreurs.Range("D1").Offset(NoLigneEnErreur, 0).value = LibelleActe
  shErreurs.Range("E1").Offset(NoLigneEnErreur, 0).Font.Color = vbRed
  shErreurs.Range("E1").Offset(NoLigneEnErreur, 0).value = "Erreur : " & Err.Number & " - " & Err.Description
    
  'shResultats.Protect PROTECT_PASSWORD
  'Resume Next
    
FIN_RAZ_ERREURS:

End Sub


Function RAZ_shErreurs(shErreurs As Worksheet) As Boolean

' RAZ des lignes dans la feuille Erreurs
    
    shErreurs.Select
   ' recherche du dernier numéro de la ligne <> "" pour permettre de supprimer toutes les lignes
    p = 1 ' à partir de la 1 ème ligne
    While Cells(p, 1) <> ""
    p = p + 1
    Wend
    
    RangeDelete = 1 & ":" & p ' chargement des no de lignes à supprimer
    Rows(RangeDelete).Select
    Selection.Delete Shift:=xlDown
    
    ' titre
    Cells(1, 1) = "Numéro chronologique"
    Cells(1, 2) = "Traitement concerné"
    Cells(1, 3) = "Libellé Famille"
    Cells(1, 4) = "Libellé Acte"
    Cells(1, 5) = "Libellé de l'erreur"

End Function



