Attribute VB_Name = "RESULTATS"
Option Explicit
Sub RESULTATS()
'*******************************
' RESULTATS_V1.0.0_du_07_09_2023
'*******************************
On Error GoTo err_RESULTATS

'VARIABLE Erreur
Dim NomModule As String
Dim p As Double
Dim NoLigneEnErreur As Double
Dim MessageErreur As String
Dim LibelleFamille As String
Dim LibelleActe As String

' déclaration des ONGLETS utilisés
Dim shAFFICHAGE As Worksheet
Dim shAFFICHAGE_2 As Worksheet
Dim shErreurs As Worksheet
Dim shResultats As Worksheet
Dim shDEMOGRAPHIE As Worksheet
Dim shCollege As Worksheet
Dim shPrestations As Worksheet
Dim shProvisions As Worksheet
Dim shCotisations As Worksheet

' déclaration des noms des ONGLETS
Dim nomFeuille_AFFICHAGE As String
Dim nomFeuille_AFFICHAGE_2 As String
Dim nomFeuille_ERREURS As String
Dim nomFeuille_RESULTATS As String
Dim nomFeuille_DEMOGRAPHIE As String
Dim nomFeuille_COLLEGE As String
Dim nomFeuille_PRESTATIONS As String
Dim nomFeuille_PROVISIONS As String
Dim nomFeuille_COTISATIONS As String

nomFeuille_AFFICHAGE = "AFFICHAGE"
nomFeuille_AFFICHAGE_2 = "AFFICHAGE-2"
nomFeuille_ERREURS = "Erreurs"
nomFeuille_RESULTATS = "Résultats"

' controle à traiter
Dim TYPE_CONTROLE As String
Dim PREV_1AN As String
Dim SANTE_1AN As String
Dim SANTE_2ANS As String

'chargement nom du contrôle
PREV_1AN = "PREV_1AN"
SANTE_1AN = "SANTE_1AN"
SANTE_2ANS = "SANTE_2ANS"

' Chargement des ONGLETS GENERAUX
NomModule = "RESULTAT"
LibelleFamille = " Ouverture des Onglets"
Set shAFFICHAGE = Worksheets(nomFeuille_AFFICHAGE)
Set shAFFICHAGE_2 = Worksheets(nomFeuille_AFFICHAGE_2)
Set shErreurs = Worksheets(nomFeuille_ERREURS)
Set shResultats = Worksheets(nomFeuille_RESULTATS)

' chargement TYPE DE CONTROLE
TYPE_CONTROLE = shAFFICHAGE.Cells(1, 20) ' TYPE DE CONTROLE

' Chargement des ONGLETS SPECIFIQUES PREV SANTE
Select Case TYPE_CONTROLE

Case PREV_1AN  ' modèle PREV_1AN
    '  chargement des noms des ONGLETS PREV
    nomFeuille_PRESTATIONS = "DATA PREV"
    nomFeuille_PROVISIONS = "DATA PROV"
    nomFeuille_COTISATIONS = "DATA COT"
    Set shPrestations = Worksheets(nomFeuille_PRESTATIONS)
    Set shProvisions = Worksheets(nomFeuille_PROVISIONS)
    Set shCotisations = Worksheets(nomFeuille_COTISATIONS)

Case SANTE_1AN, SANTE_2ANS  ' modèle SANTE_1AN ou ' modèle SANTE_2ANS
    '  chargement des noms des ONGLETS SANTE
    nomFeuille_DEMOGRAPHIE = "DATA DEMO"
    nomFeuille_COLLEGE = "COLLEGE"
    nomFeuille_PRESTATIONS = "DATA PREST"
    nomFeuille_PROVISIONS = "DATA PROV"
    nomFeuille_COTISATIONS = "DATA COT"
    Set shDEMOGRAPHIE = Worksheets(nomFeuille_DEMOGRAPHIE)
    Set shCollege = Worksheets(nomFeuille_COLLEGE)
    Set shPrestations = Worksheets(nomFeuille_PRESTATIONS)
    Set shProvisions = Worksheets(nomFeuille_PROVISIONS)
    Set shCotisations = Worksheets(nomFeuille_COTISATIONS)

Case Else 'NON TRAITE
    Exit Sub ' fin du traitement

End Select

LibelleFamille = ""
Dim college(100) As String
Dim Ligne As Double
Dim LigneAnnee2 As Double
Dim LigneAnnee1 As Double
Dim ANNEE2 As String
Dim ANNEE1 As String
Dim LibelleFrais As String
Dim Selection_FRAIS As String
Selection_FRAIS = "H:J"  ' selection des frais dans AFFICHAGE-2

Dim bValue As Boolean
   
' colonnes dans RESULTATS
Dim Col_ANNEE As Double  ' année
Dim Col_TITRE As Double  ' Colonne prestations
Dim Col_PRESTA As Double  ' Colonne prestations
Dim Col_PROV As Double  ' Colonne provisions
Dim Col_CB As Double  ' Colonne cotisations Brutes
Dim Col_TAUX_GESTION As Double  ' Colonne taux frais de gestion
Dim Col_TAUX_TAXES As Double  ' Colonne taux taxes
Dim Col_CHARGEMENTS As Double  ' Colonne chargements
Dim Col_CN As Double  ' Colonne cotisations nettes
Dim Col_RATIO As Double  ' Colonne ratio
Dim Col_GAINS_PERTES As Double  ' Colonne gains pertes
Dim Col_TAUX_TAXE_COVID As Double  ' Colonne taxe covid

Col_ANNEE = 3 ' année
Col_TITRE = 5 ' titre
Col_PRESTA = 7 ' colonne prestations
Col_PROV = 9 ' colonne provisions
Col_CB = 11 ' colonne cotisations Brutes
Col_TAUX_GESTION = 13 ' colonne taux frais de gestion
Col_TAUX_TAXES = 14 ' colonne taux taxes
Col_CHARGEMENTS = 15 ' colonne chargements
Col_CN = 17 ' colonne cotisations nettes
Col_RATIO = 19 ' colonne ratio
Col_GAINS_PERTES = 21 ' colonne gains pertes
Col_TAUX_TAXE_COVID = 23 ' colonne taxe covid

' MONTANTS dans RESULTATS
Dim TITRE As String  '  titre
Dim PRESTA As Double  '  prestations
Dim PROV As Double  '  provisions
Dim CB As Double  '  cotisations Brutes
Dim TAUX_GESTION As Double  '  taux frais de gestion
Dim TAUX_TAXES As Double  '  taux taxes
Dim CHARGEMENTS As Double  '  chargements
Dim CN As Double  '  cotisations nettes
Dim RATIO As Double  '  ratio
Dim GAINS_PERTES As Double  '  gains pertes
Dim TAUX_TAXE_COVID As Double  '  taxe covid
  
' chargement des ANNEES avec onglet AFFICHAGE
shAFFICHAGE.Select
Dim Date_arrete As Date
Dim Date_fin As Date
Dim Date_debut As Date
Date_arrete = Cells(2, 15)
Date_debut = Cells(2, 13)
Date_fin = Cells(2, 14)

ANNEE1 = Year(Date_debut)
ANNEE2 = Year(Date_fin)

If ANNEE2 = ANNEE1 Then
ANNEE1 = ""
Else
End If


Select Case TYPE_CONTROLE

Case PREV_1AN  ' modèle PREV_1AN

    'RAZ et MAJ ligne ANNEE2 - "INCAPACITE"
    TITRE = "INCAPACITE"
    LigneAnnee2 = 17  ' numéro de ligne pour l'Année 1
    LibelleFamille = "RAZ et MAJ ligne ANNEE2 modèle SANTE_1AN - " & TITRE
    bValue = RAZ_Ligne(shResultats, LigneAnnee2, Col_ANNEE, Col_TITRE, Col_PRESTA, Col_PROV, Col_CB, Col_TAUX_GESTION, Col_TAUX_TAXES, Col_CHARGEMENTS, Col_CN, Col_RATIO, Col_GAINS_PERTES, Col_TAUX_TAXE_COVID)
    bValue = MAJ_Ligne_PREV(shResultats, shCotisations, shPrestations, shProvisions, shAFFICHAGE, shAFFICHAGE_2, LigneAnnee2, Col_ANNEE, ANNEE2, Col_TITRE, TITRE, Col_PRESTA, Col_PROV, Col_CB, Col_TAUX_GESTION, Col_TAUX_TAXES, Col_CHARGEMENTS, Col_CN, Col_RATIO, Col_GAINS_PERTES, Col_TAUX_TAXE_COVID)
    LibelleFamille = ""

    'RAZ et MAJ ligne ANNEE2 - "INVALIDITE"
    TITRE = "INVALIDITE"
    LigneAnnee2 = 19  ' numéro de ligne pour l'Année 1
    LibelleFamille = "RAZ et MAJ ligne ANNEE2 modèle SANTE_1AN - " & TITRE
    bValue = RAZ_Ligne(shResultats, LigneAnnee2, Col_ANNEE, Col_TITRE, Col_PRESTA, Col_PROV, Col_CB, Col_TAUX_GESTION, Col_TAUX_TAXES, Col_CHARGEMENTS, Col_CN, Col_RATIO, Col_GAINS_PERTES, Col_TAUX_TAXE_COVID)
    bValue = MAJ_Ligne_PREV(shResultats, shCotisations, shPrestations, shProvisions, shAFFICHAGE, shAFFICHAGE_2, LigneAnnee2, Col_ANNEE, ANNEE2, Col_TITRE, TITRE, Col_PRESTA, Col_PROV, Col_CB, Col_TAUX_GESTION, Col_TAUX_TAXES, Col_CHARGEMENTS, Col_CN, Col_RATIO, Col_GAINS_PERTES, Col_TAUX_TAXE_COVID)
    LibelleFamille = ""

    'RAZ et MAJ ligne ANNEE2 - "DECES"
    TITRE = "DECES"
    LigneAnnee2 = 21  ' numéro de ligne pour l'Année 1
    LibelleFamille = "RAZ et MAJ ligne ANNEE2 modèle SANTE_1AN - " & TITRE
    bValue = RAZ_Ligne(shResultats, LigneAnnee2, Col_ANNEE, Col_TITRE, Col_PRESTA, Col_PROV, Col_CB, Col_TAUX_GESTION, Col_TAUX_TAXES, Col_CHARGEMENTS, Col_CN, Col_RATIO, Col_GAINS_PERTES, Col_TAUX_TAXE_COVID)
    bValue = MAJ_Ligne_PREV(shResultats, shCotisations, shPrestations, shProvisions, shAFFICHAGE, shAFFICHAGE_2, LigneAnnee2, Col_ANNEE, ANNEE2, Col_TITRE, TITRE, Col_PRESTA, Col_PROV, Col_CB, Col_TAUX_GESTION, Col_TAUX_TAXES, Col_CHARGEMENTS, Col_CN, Col_RATIO, Col_GAINS_PERTES, Col_TAUX_TAXE_COVID)
    LibelleFamille = ""

    'RAZ et MAJ ligne ANNEE2 - "TOTAL"
    TITRE = "TOTAL"
    LigneAnnee2 = 23  ' numéro de ligne pour l'Année 1
    LibelleFamille = "RAZ et MAJ ligne ANNEE2 modèle SANTE_1AN - " & TITRE
    bValue = RAZ_Ligne(shResultats, LigneAnnee2, Col_ANNEE, Col_TITRE, Col_PRESTA, Col_PROV, Col_CB, Col_TAUX_GESTION, Col_TAUX_TAXES, Col_CHARGEMENTS, Col_CN, Col_RATIO, Col_GAINS_PERTES, Col_TAUX_TAXE_COVID)
    bValue = MAJ_Ligne_TOTAL_PREV(shResultats, shCotisations, shPrestations, shProvisions, shAFFICHAGE, shAFFICHAGE_2, LigneAnnee2, Col_ANNEE, ANNEE2, Col_TITRE, TITRE, Col_PRESTA, Col_PROV, Col_CB, Col_TAUX_GESTION, Col_TAUX_TAXES, Col_CHARGEMENTS, Col_CN, Col_RATIO, Col_GAINS_PERTES, Col_TAUX_TAXE_COVID)
    LibelleFamille = ""

Case SANTE_1AN  ' modèle SANTE_1AN
    
    ' MAJ FAMILLE COLLEGE dans les onglets DEMOGRAPHIE PRESTATIONS PROVISIONS COTISATIONS
    LibelleFamille = "MAJ_FAMILLE_COLLEGE modèle SANTE_1AN"
    bValue = MAJ_FAMILLE_COLLEGE(shCollege, shDEMOGRAPHIE, shPrestations, shProvisions, shCotisations)
    LibelleFamille = ""

    'RAZ et MAJ ligne ANNEE2
    LibelleFamille = "RAZ et MAJ ligne ANNEE2 modèle SANTE_1AN"
    LigneAnnee2 = 20  ' numéro de ligne pour l'Année 1
    bValue = RAZ_Ligne(shResultats, LigneAnnee2, Col_ANNEE, Col_TITRE, Col_PRESTA, Col_PROV, Col_CB, Col_TAUX_GESTION, Col_TAUX_TAXES, Col_CHARGEMENTS, Col_CN, Col_RATIO, Col_GAINS_PERTES, Col_TAUX_TAXE_COVID)
    bValue = MAJ_Ligne_SANTE(shResultats, shCotisations, shPrestations, shProvisions, shAFFICHAGE, shAFFICHAGE_2, LigneAnnee2, Col_ANNEE, ANNEE2, Col_TITRE, "GLOBAL", Col_PRESTA, Col_PROV, Col_CB, Col_TAUX_GESTION, Col_TAUX_TAXES, Col_CHARGEMENTS, Col_CN, Col_RATIO, Col_GAINS_PERTES, Col_TAUX_TAXE_COVID)
    LibelleFamille = ""
    
Case SANTE_2ANS ' modèle SANTE_2ANS

    ' MAJ FAMILLE COLLEGE dans les onglets DEMOGRAPHIE PRESTATIONS PROVISIONS COTISATIONS
    LibelleFamille = "MAJ_FAMILLE_COLLEGE modèle SANTE_2ANS"
    bValue = MAJ_FAMILLE_COLLEGE(shCollege, shDEMOGRAPHIE, shPrestations, shProvisions, shCotisations)
    LibelleFamille = ""
    
    'RAZ et MAJ ligne ANNEE2 et ANNEE1
    LibelleFamille = "RAZ et MAJ ligne ANNEE2 et ANNEE1 modèle SANTE_2ANS"
    LibelleFamille = ""
    LigneAnnee2 = 20  ' numéro de ligne pour l'Année 1
    LigneAnnee1 = 25  ' numéro de ligne pour l'Année 2
    bValue = RAZ_Ligne(shResultats, LigneAnnee2, Col_ANNEE, Col_TITRE, Col_PRESTA, Col_PROV, Col_CB, Col_TAUX_GESTION, Col_TAUX_TAXES, Col_CHARGEMENTS, Col_CN, Col_RATIO, Col_GAINS_PERTES, Col_TAUX_TAXE_COVID)
    bValue = RAZ_Ligne(shResultats, LigneAnnee1, Col_ANNEE, Col_TITRE, Col_PRESTA, Col_PROV, Col_CB, Col_TAUX_GESTION, Col_TAUX_TAXES, Col_CHARGEMENTS, Col_CN, Col_RATIO, Col_GAINS_PERTES, Col_TAUX_TAXE_COVID)
    bValue = MAJ_Ligne_SANTE(shResultats, shCotisations, shPrestations, shProvisions, shAFFICHAGE, shAFFICHAGE_2, LigneAnnee2, Col_ANNEE, ANNEE2, Col_TITRE, "GLOBAL", Col_PRESTA, Col_PROV, Col_CB, Col_TAUX_GESTION, Col_TAUX_TAXES, Col_CHARGEMENTS, Col_CN, Col_RATIO, Col_GAINS_PERTES, Col_TAUX_TAXE_COVID)
    If ANNEE1 <> "" Then
    bValue = MAJ_Ligne_SANTE(shResultats, shCotisations, shPrestations, shProvisions, shAFFICHAGE, shAFFICHAGE_2, LigneAnnee1, Col_ANNEE, ANNEE1, Col_TITRE, "GLOBAL", Col_PRESTA, Col_PROV, Col_CB, Col_TAUX_GESTION, Col_TAUX_TAXES, Col_CHARGEMENTS, Col_CN, Col_RATIO, Col_GAINS_PERTES, Col_TAUX_TAXE_COVID)
    End If
    
Case Else 'NON TRAITE
    ' pas de traitement

End Select
              
               
GoTo FIN_CONTROLE:
               
err_RESULTATS:

  Set shErreurs = Worksheets("Erreurs")
  Application.Cursor = xlDefault
  
 ' recherche du dernier numéro de la ligne <> "" pour permettre d'ajouter un message à la ligne suivante
    p = 1
    While shErreurs.Cells(p, 1) <> ""
    p = p + 1
    Wend
    p = p - 1
    NoLigneEnErreur = p - 1
  
  MsgBox "Erreur dans RESULTAT() : " & Err.Number & vbLf & Err.Description, vbCritical
  
  ' affichage du message d'erreur dans la feuille "Erreurs"
  'MessageErreur = "CONTROLE() : " & Err.Number & " - " & Err.Description
  MessageErreur = NomModule & " - " & LibelleFamille
  MsgBox MessageErreur
    
  NoLigneEnErreur = NoLigneEnErreur + 1
  'shErreurs.Range("A1").Offset(NoLigneEnErreur).Value = shResultats.Range("C2").Offset(NoLigneEnErreur).Value
  shErreurs.Range("A1").Offset(NoLigneEnErreur).value = NoLigneEnErreur
  shErreurs.Range("B1").Offset(NoLigneEnErreur).value = NomModule
  shErreurs.Range("C1").Offset(NoLigneEnErreur).value = LibelleFamille
  shErreurs.Range("D1").Offset(NoLigneEnErreur).value = LibelleActe
  shErreurs.Range("E1").Offset(NoLigneEnErreur).Font.Color = vbRed
  shErreurs.Range("E1").Offset(NoLigneEnErreur).value = "Erreur : " & Err.Number & " - " & Err.Description
    
  'shResultats.Protect PROTECT_PASSWORD
  
  'Resume Nex
    
FIN_CONTROLE:
               
               
End Sub


Function RAZ_Ligne(shResultats As Worksheet, LigneAnnee As Double, Col_ANNEE As Double, Col_TITRE As Double, Col_PRESTA As Double, Col_PROV As Double, Col_CB As Double, Col_TAUX_GESTION As Double, Col_TAUX_TAXES As Double, Col_CHARGEMENTS As Double, Col_CN As Double, Col_RATIO As Double, Col_GAINS_PERTES As Double, Col_TAUX_TAXE_COVID As Double)
    
 shResultats.Select
 
shResultats.Cells(LigneAnnee, Col_ANNEE) = ""
shResultats.Cells(LigneAnnee, Col_TITRE) = ""
shResultats.Cells(LigneAnnee, Col_PRESTA) = ""
shResultats.Cells(LigneAnnee, Col_PROV) = ""
shResultats.Cells(LigneAnnee, Col_CB) = ""
shResultats.Cells(LigneAnnee, Col_TAUX_GESTION) = ""
shResultats.Cells(LigneAnnee, Col_TAUX_TAXES) = ""
shResultats.Cells(LigneAnnee, Col_CHARGEMENTS) = ""
shResultats.Cells(LigneAnnee, Col_CN) = ""
shResultats.Cells(LigneAnnee, Col_RATIO) = ""
shResultats.Cells(LigneAnnee, Col_GAINS_PERTES) = ""
shResultats.Cells(LigneAnnee, Col_TAUX_TAXE_COVID) = ""

End Function


Function MAJ_Ligne_SANTE(shResultats As Worksheet, shCotisations As Worksheet, shPrestations As Worksheet, shProvisions As Worksheet, shAFFICHAGE, shAFFICHAGE_2 As Worksheet, LigneAnnee As Double, Col_ANNEE As Double, ANNEE As String, Col_TITRE As Double, TITRE As String, Col_PRESTA As Double, Col_PROV As Double, Col_CB As Double, Col_TAUX_GESTION As Double, Col_TAUX_TAXES As Double, Col_CHARGEMENTS As Double, Col_CN As Double, Col_RATIO As Double, Col_GAINS_PERTES As Double, Col_TAUX_TAXE_COVID As Double)

Dim PRESTA As Double
Dim PROV As Double
Dim CB As Double
Dim TAUX_GESTION As Double
Dim TAUX_TAXES As Double
Dim CHARGEMENTS As Double
Dim CN As Double
Dim RATIO As Double
Dim GAINS_PERTES As Double
Dim TAUX_TAXE_COVID As Double

' "DATA PREST"
Dim FPREST As String
Dim PRESTATION As String
Dim PRESTATIONANNEE As String
Dim PRESTATIONCOL As String
FPREST = "DATA PREST"
PRESTATION = "L:L"
PRESTATIONANNEE = "D:D"
PRESTATIONCOL = "R:R"

'"DATA COT"
Dim FCOT As String
Dim COTISATION_NETTE As String
Dim COTISATION_BRUTE As String
Dim COTISATIONANNEE As String
Dim COTISATIONCOL As String
FCOT = "DATA COT"
COTISATION_NETTE = "F:F"
COTISATION_BRUTE = "H:H"
COTISATIONANNEE = "E:E"
COTISATIONCOL = "G:G"

' "DATA PROV"
Dim FPROV As String
Dim PROVISION As String
Dim PROVISIONANNEE As String
Dim PROVISIONCOL As String
FPROV = "DATA PROV"
PROVISION = "G:G"
PROVISIONANNEE = "D:D"
PROVISIONCOL = "H:H"

Dim LibelleFrais As String

'shResultats.Select

'RAZ
shResultats.Cells(LigneAnnee, Col_ANNEE) = ""
shResultats.Cells(LigneAnnee, Col_TITRE) = ""
shResultats.Cells(LigneAnnee, Col_PRESTA) = ""
shResultats.Cells(LigneAnnee, Col_PROV) = ""
shResultats.Cells(LigneAnnee, Col_CB) = ""
shResultats.Cells(LigneAnnee, Col_TAUX_GESTION) = ""
shResultats.Cells(LigneAnnee, Col_TAUX_TAXES) = ""
shResultats.Cells(LigneAnnee, Col_CHARGEMENTS) = ""
shResultats.Cells(LigneAnnee, Col_CN) = ""
shResultats.Cells(LigneAnnee, Col_RATIO) = ""
shResultats.Cells(LigneAnnee, Col_GAINS_PERTES) = ""
shResultats.Cells(LigneAnnee, Col_TAUX_TAXE_COVID) = ""

'CALCUL
' cotisations Brutes
CB = Application.WorksheetFunction.SumIfs(shCotisations.Range(COTISATION_BRUTE), shCotisations.Range(COTISATIONANNEE), ANNEE, shCotisations.Range(COTISATIONCOL), "ACTIFS")
CB = Round(CB, 2)

' cotisations nettes
CN = Application.WorksheetFunction.SumIfs(shCotisations.Range(COTISATION_NETTE), shCotisations.Range(COTISATIONANNEE), ANNEE, shCotisations.Range(COTISATIONCOL), "ACTIFS")
CN = Round(CN, 2)

' TAUX_GESTION frais de gestion
LibelleFrais = "FRAIS"
TAUX_GESTION = Frais(shAFFICHAGE_2, ANNEE, LibelleFrais)

' TAUX_TAXE_COVID taxe covid
LibelleFrais = "TAXE_COVID"
TAUX_TAXE_COVID = Frais(shAFFICHAGE_2, ANNEE, LibelleFrais)

' TAUX_TAXES taxe
LibelleFrais = "TCA_CMU"
TAUX_TAXES = Frais(shAFFICHAGE_2, ANNEE, LibelleFrais)

' CHARGEMENTS chargements
If CB <> 0 Then
    CHARGEMENTS = Round(1 - CN / CB, 4)
Else
    CHARGEMENTS = 0
End If

' PRESTA prestations
PRESTA = Application.WorksheetFunction.SumIfs(shPrestations.Range(PRESTATION), shPrestations.Range(PRESTATIONANNEE), ANNEE, shPrestations.Range(PRESTATIONCOL), "ACTIFS")
PRESTA = Round(PRESTA, 2)

'PROV provisions
PROV = Application.WorksheetFunction.SumIfs(shProvisions.Range(PROVISION), shProvisions.Range(PROVISIONANNEE), ANNEE, shProvisions.Range(PROVISIONCOL), "ACTIFS")
PROV = Round(PROV, 2)

' RATIO ratio
If CN <> 0 Then
RATIO = Round((PRESTA + PROV) / CN, 4)
Else
RATIO = 0
End If

' gains / pertes
GAINS_PERTES = CN - (PRESTA + PROV)


' AFFICHAGE RESULTATS
shResultats.Cells(LigneAnnee, Col_ANNEE) = ANNEE
shResultats.Cells(LigneAnnee, Col_TITRE) = TITRE
shResultats.Cells(LigneAnnee, Col_PRESTA) = PRESTA
shResultats.Cells(LigneAnnee, Col_PROV) = PROV
shResultats.Cells(LigneAnnee, Col_CB) = CB
shResultats.Cells(LigneAnnee, Col_TAUX_GESTION) = TAUX_GESTION
shResultats.Cells(LigneAnnee, Col_TAUX_TAXES) = TAUX_TAXES
shResultats.Cells(LigneAnnee, Col_CHARGEMENTS) = CHARGEMENTS
shResultats.Cells(LigneAnnee, Col_CN) = CN
shResultats.Cells(LigneAnnee, Col_RATIO) = RATIO
shResultats.Cells(LigneAnnee, Col_GAINS_PERTES) = GAINS_PERTES
shResultats.Cells(LigneAnnee, Col_TAUX_TAXE_COVID) = TAUX_TAXE_COVID

If TAUX_TAXE_COVID <> 0 Then
    Cells(LigneAnnee, Col_TAUX_TAXE_COVID) = "Taux Taxe COVID = " & TAUX_TAXE_COVID
Else
    Cells(LigneAnnee, Col_TAUX_TAXE_COVID) = ""
End If


End Function

Function MAJ_Ligne_PREV(shResultats As Worksheet, shCotisations As Worksheet, shPrestations As Worksheet, shProvisions As Worksheet, shAFFICHAGE As Worksheet, shAFFICHAGE_2 As Worksheet, LigneAnnee As Double, Col_ANNEE As Double, ANNEE As String, Col_TITRE As Double, TITRE As String, Col_PRESTA As Double, Col_PROV As Double, Col_CB As Double, Col_TAUX_GESTION As Double, Col_TAUX_TAXES As Double, Col_CHARGEMENTS As Double, Col_CN As Double, Col_RATIO As Double, Col_GAINS_PERTES As Double, Col_TAUX_TAXE_COVID As Double)

Dim PRESTA As Double
Dim PROV As Double
Dim CB As Double
Dim TAUX_GESTION As Double
Dim TAUX_TAXES As Double
Dim CHARGEMENTS As Double
Dim CN As Double
Dim RATIO As Double
Dim GAINS_PERTES As Double
Dim TAUX_TAXE_COVID As Double

' "DATA PREST"
Dim FPREST As String
Dim PRESTATION As String
Dim PRESTATIONANNEE As String
Dim PRESTATION_GARANTIE As String
FPREST = "DATA PREST"
PRESTATION = "P:P"
PRESTATIONANNEE = "E:E"
PRESTATION_GARANTIE = "AH:AH"

'"DATA COT"
Dim FCOT As String
Dim COTISATION_NETTE As String
Dim COTISATION_BRUTE As String
Dim COTISATIONANNEE As String
Dim COTISATION_GARANTIE As String
FCOT = "DATA COT"
COTISATION_NETTE = "F:F"
COTISATION_BRUTE = "G:G"
COTISATIONANNEE = "E:E"
COTISATION_GARANTIE = "H:H"

' "DATA PROV"
Dim FPROV As String
Dim PROVISION As String
Dim PROVISIONANNEE As String
Dim PROVISION_GARANTIE As String
FPROV = "DATA PROV"
PROVISION = "Q:Q"
PROVISIONANNEE = "E:E"
PROVISION_GARANTIE = "F:F"

Dim LibelleFrais As String
Dim TypeCompte As String

'shResultats.Select

'RAZ
shResultats.Cells(LigneAnnee, Col_ANNEE) = ""
shResultats.Cells(LigneAnnee, Col_TITRE) = ""
shResultats.Cells(LigneAnnee, Col_PRESTA) = ""
shResultats.Cells(LigneAnnee, Col_PROV) = ""
shResultats.Cells(LigneAnnee, Col_CB) = ""
shResultats.Cells(LigneAnnee, Col_TAUX_GESTION) = ""
shResultats.Cells(LigneAnnee, Col_TAUX_TAXES) = ""
shResultats.Cells(LigneAnnee, Col_CHARGEMENTS) = ""
shResultats.Cells(LigneAnnee, Col_CN) = ""
shResultats.Cells(LigneAnnee, Col_RATIO) = ""
shResultats.Cells(LigneAnnee, Col_GAINS_PERTES) = ""
shResultats.Cells(LigneAnnee, Col_TAUX_TAXE_COVID) = ""

'TypeDeCompte SURVENANCE COMPTABLE
TypeCompte = UCase(shAFFICHAGE.Cells(2, 19))


Select Case TypeCompte

'*****************
'CALCUL SURVENANCE
'*****************
Case "SURVENANCE"

' cotisations Brutes
CB = Application.WorksheetFunction.SumIfs(shCotisations.Range(COTISATION_BRUTE), shCotisations.Range(COTISATIONANNEE), ANNEE, shCotisations.Range(COTISATION_GARANTIE), TITRE)
CB = Round(CB, 2)

' cotisations nettes
CN = Application.WorksheetFunction.SumIfs(shCotisations.Range(COTISATION_NETTE), shCotisations.Range(COTISATIONANNEE), ANNEE, shCotisations.Range(COTISATION_GARANTIE), TITRE)
CN = Round(CN, 2)

' TAUX_GESTION frais de gestion
LibelleFrais = TITRE
TAUX_GESTION = Frais(shAFFICHAGE_2, ANNEE, LibelleFrais)

' TAUX_TAXE_COVID taxe covid
'LibelleFrais = "TAXE_COVID"
'TAUX_TAXE_COVID = Frais(shAFFICHAGE_2, ANNEE, LibelleFrais)

' TAUX_TAXES taxe
'LibelleFrais = "TCA_CMU"
'TAUX_TAXES = Frais(shAFFICHAGE_2, ANNEE, LibelleFrais)

' CHARGEMENTS chargements
If CB <> 0 Then
    CHARGEMENTS = Round(1 - CN / CB, 4)
Else
    CHARGEMENTS = 0
End If

' PRESTA prestations
PRESTA = Application.WorksheetFunction.SumIfs(shPrestations.Range(PRESTATION), shPrestations.Range(PRESTATIONANNEE), ANNEE, shPrestations.Range(PRESTATION_GARANTIE), TITRE)
PRESTA = Round(PRESTA, 2)

'PROV provisions
PROV = Application.WorksheetFunction.SumIfs(shProvisions.Range(PROVISION), shProvisions.Range(PROVISIONANNEE), ANNEE, shProvisions.Range(PRESTATION_GARANTIE), TITRE)
PROV = Round(PROV, 2)

' RATIO ratio
If CN <> 0 Then
RATIO = Round((PRESTA + PROV) / CN, 4)
Else
RATIO = 0
End If

' gains / pertes
GAINS_PERTES = CN - (PRESTA + PROV)


Case "COMPTABLE"
'*****************
'CALCUL COMPTABLE
'*****************

' cotisations Brutes
CB = Application.WorksheetFunction.SumIfs(shCotisations.Range(COTISATION_BRUTE), shCotisations.Range(COTISATION_GARANTIE), TITRE)
CB = Round(CB, 2)

' cotisations nettes
CN = Application.WorksheetFunction.SumIfs(shCotisations.Range(COTISATION_NETTE), shCotisations.Range(COTISATION_GARANTIE), TITRE)
CN = Round(CN, 2)

' TAUX_GESTION frais de gestion
LibelleFrais = TITRE
TAUX_GESTION = Frais(shAFFICHAGE_2, ANNEE, LibelleFrais)

' TAUX_TAXE_COVID taxe covid
'LibelleFrais = "TAXE_COVID"
'TAUX_TAXE_COVID = Frais(shAFFICHAGE_2, ANNEE, LibelleFrais)

' TAUX_TAXES taxe
'LibelleFrais = "TCA_CMU"
'TAUX_TAXES = Frais(shAFFICHAGE_2, ANNEE, LibelleFrais)

' CHARGEMENTS chargements
If CB <> 0 Then
    CHARGEMENTS = Round(1 - CN / CB, 4)
Else
    CHARGEMENTS = 0
End If

' PRESTA prestations
PRESTA = Application.WorksheetFunction.SumIfs(shPrestations.Range(PRESTATION), shPrestations.Range(PRESTATION_GARANTIE), TITRE)
PRESTA = Round(PRESTA, 2)

'PROV provisions
PROV = Application.WorksheetFunction.SumIfs(shProvisions.Range(PROVISION), shProvisions.Range(PRESTATION_GARANTIE), TITRE)
PROV = Round(PROV, 2)

' RATIO ratio
If CN <> 0 Then
RATIO = Round((PRESTA + PROV) / CN, 4)
Else
RATIO = 0
End If

' gains / pertes
GAINS_PERTES = CN - (PRESTA + PROV)



Case Else 'NON TRAITE
    ' pas de traitement

      MsgBox "Erreur dans RESULTAT() : 'TypeCompte dans onglet AFFICHAGE <> DE COMPTABLE OU SURVENANCE " & Err.Number & vbLf & Err.Description, vbCritical
    
    
End Select
 


' AFFICHAGE RESULTATS
shResultats.Cells(LigneAnnee, Col_ANNEE) = ANNEE
shResultats.Cells(LigneAnnee, Col_TITRE) = TITRE
shResultats.Cells(LigneAnnee, Col_PRESTA) = PRESTA
shResultats.Cells(LigneAnnee, Col_PROV) = PROV
shResultats.Cells(LigneAnnee, Col_CB) = CB
shResultats.Cells(LigneAnnee, Col_TAUX_GESTION) = TAUX_GESTION
shResultats.Cells(LigneAnnee, Col_TAUX_TAXES) = TAUX_TAXES
shResultats.Cells(LigneAnnee, Col_CHARGEMENTS) = CHARGEMENTS
shResultats.Cells(LigneAnnee, Col_CN) = CN
shResultats.Cells(LigneAnnee, Col_RATIO) = RATIO
shResultats.Cells(LigneAnnee, Col_GAINS_PERTES) = GAINS_PERTES
shResultats.Cells(LigneAnnee, Col_TAUX_TAXE_COVID) = TAUX_TAXE_COVID

If TAUX_TAXE_COVID <> 0 Then
    Cells(LigneAnnee, Col_TAUX_TAXE_COVID) = "Taux Taxe COVID = " & TAUX_TAXE_COVID
Else
    Cells(LigneAnnee, Col_TAUX_TAXE_COVID) = ""
End If


End Function



Function MAJ_Ligne_TOTAL_PREV(shResultats As Worksheet, shCotisations As Worksheet, shPrestations As Worksheet, shProvisions As Worksheet, shAFFICHAGE As Worksheet, shAFFICHAGE_2 As Worksheet, LigneAnnee As Double, Col_ANNEE As Double, ANNEE As String, Col_TITRE As Double, TITRE As String, Col_PRESTA As Double, Col_PROV As Double, Col_CB As Double, Col_TAUX_GESTION As Double, Col_TAUX_TAXES As Double, Col_CHARGEMENTS As Double, Col_CN As Double, Col_RATIO As Double, Col_GAINS_PERTES As Double, Col_TAUX_TAXE_COVID As Double)

Dim PRESTA As Double
Dim PROV As Double
Dim CB As Double
Dim TAUX_GESTION As Double
Dim TAUX_TAXES As Double
Dim CHARGEMENTS As Double
Dim CN As Double
Dim RATIO As Double
Dim GAINS_PERTES As Double
Dim TAUX_TAXE_COVID As Double

' "DATA PREST"
Dim FPREST As String
Dim PRESTATION As String
Dim PRESTATIONANNEE As String
Dim PRESTATION_GARANTIE As String
FPREST = "DATA PREST"
PRESTATION = "P:P"
PRESTATIONANNEE = "E:E"
PRESTATION_GARANTIE = "AH:AH"

'"DATA COT"
Dim FCOT As String
Dim COTISATION_NETTE As String
Dim COTISATION_BRUTE As String
Dim COTISATIONANNEE As String
Dim COTISATION_GARANTIE As String
FCOT = "DATA COT"
COTISATION_NETTE = "F:F"
COTISATION_BRUTE = "G:G"
COTISATIONANNEE = "E:E"
COTISATION_GARANTIE = "H:H"

' "DATA PROV"
Dim FPROV As String
Dim PROVISION As String
Dim PROVISIONANNEE As String
Dim PROVISION_GARANTIE As String
FPROV = "DATA PROV"
PROVISION = "Q:Q"
PROVISIONANNEE = "E:E"
PROVISION_GARANTIE = "F:F"

Dim LibelleFrais As String
Dim TypeCompte As String

'shResultats.Select

'RAZ
shResultats.Cells(LigneAnnee, Col_ANNEE) = ""
shResultats.Cells(LigneAnnee, Col_TITRE) = ""
shResultats.Cells(LigneAnnee, Col_PRESTA) = ""
shResultats.Cells(LigneAnnee, Col_PROV) = ""
shResultats.Cells(LigneAnnee, Col_CB) = ""
shResultats.Cells(LigneAnnee, Col_TAUX_GESTION) = ""
shResultats.Cells(LigneAnnee, Col_TAUX_TAXES) = ""
shResultats.Cells(LigneAnnee, Col_CHARGEMENTS) = ""
shResultats.Cells(LigneAnnee, Col_CN) = ""
shResultats.Cells(LigneAnnee, Col_RATIO) = ""
shResultats.Cells(LigneAnnee, Col_GAINS_PERTES) = ""
shResultats.Cells(LigneAnnee, Col_TAUX_TAXE_COVID) = ""

'TypeDeCompte SURVENANCE COMPTABLE
TypeCompte = UCase(shAFFICHAGE.Cells(2, 19))


Select Case TypeCompte

'*****************
'CALCUL SURVENANCE
'*****************
Case "SURVENANCE"

' cotisations Brutes
CB = Application.WorksheetFunction.SumIfs(shCotisations.Range(COTISATION_BRUTE), shCotisations.Range(COTISATIONANNEE), ANNEE)
CB = Round(CB, 2)

' cotisations nettes
CN = Application.WorksheetFunction.SumIfs(shCotisations.Range(COTISATION_NETTE), shCotisations.Range(COTISATIONANNEE), ANNEE)
CN = Round(CN, 2)

' TAUX_GESTION frais de gestion
LibelleFrais = TITRE
TAUX_GESTION = Frais(shAFFICHAGE_2, ANNEE, LibelleFrais)

' TAUX_TAXE_COVID taxe covid
'LibelleFrais = "TAXE_COVID"
'TAUX_TAXE_COVID = Frais(shAFFICHAGE_2, ANNEE, LibelleFrais)

' TAUX_TAXES taxe
'LibelleFrais = "TCA_CMU"
'TAUX_TAXES = Frais(shAFFICHAGE_2, ANNEE, LibelleFrais)

' CHARGEMENTS chargements
If CB <> 0 Then
    CHARGEMENTS = Round(1 - CN / CB, 4)
Else
    CHARGEMENTS = 0
End If

' PRESTA prestations
PRESTA = Application.WorksheetFunction.SumIfs(shPrestations.Range(PRESTATION), shPrestations.Range(PRESTATIONANNEE), ANNEE)
PRESTA = Round(PRESTA, 2)

'PROV provisions
PROV = Application.WorksheetFunction.SumIfs(shProvisions.Range(PROVISION), shProvisions.Range(PROVISIONANNEE), ANNEE)
PROV = Round(PROV, 2)

' RATIO ratio
If CN <> 0 Then
RATIO = Round((PRESTA + PROV) / CN, 4)
Else
RATIO = 0
End If

' gains / pertes
GAINS_PERTES = CN - (PRESTA + PROV)


Case "COMPTABLE"
'*****************
'CALCUL COMPTABLE
'*****************

' cotisations Brutes
CB = Application.WorksheetFunction.Sum(shCotisations.Range(COTISATION_BRUTE))
CB = Round(CB, 2)

' cotisations nettes
CN = Application.WorksheetFunction.Sum(shCotisations.Range(COTISATION_NETTE))
CN = Round(CN, 2)

' TAUX_GESTION frais de gestion
LibelleFrais = TITRE
TAUX_GESTION = Frais(shAFFICHAGE_2, ANNEE, LibelleFrais)

' TAUX_TAXE_COVID taxe covid
'LibelleFrais = "TAXE_COVID"
'TAUX_TAXE_COVID = Frais(shAFFICHAGE_2, ANNEE, LibelleFrais)

' TAUX_TAXES taxe
'LibelleFrais = "TCA_CMU"
'TAUX_TAXES = Frais(shAFFICHAGE_2, ANNEE, LibelleFrais)

' CHARGEMENTS chargements
If CB <> 0 Then
    CHARGEMENTS = Round(1 - CN / CB, 4)
Else
    CHARGEMENTS = 0
End If

' PRESTA prestations
PRESTA = Application.WorksheetFunction.Sum(shPrestations.Range(PRESTATION))
PRESTA = Round(PRESTA, 2)

'PROV provisions
PROV = Application.WorksheetFunction.Sum(shProvisions.Range(PROVISION))
PROV = Round(PROV, 2)

' RATIO ratio
If CN <> 0 Then
RATIO = Round((PRESTA + PROV) / CN, 4)
Else
RATIO = 0
End If

' gains / pertes
GAINS_PERTES = CN - (PRESTA + PROV)



Case Else 'NON TRAITE
    ' pas de traitement

      MsgBox "Erreur dans RESULTAT() : 'TypeCompte dans onglet AFFICHAGE <> DE COMPTABLE OU SURVENANCE " & Err.Number & vbLf & Err.Description, vbCritical
    
End Select
 
' AFFICHAGE RESULTATS
shResultats.Cells(LigneAnnee, Col_ANNEE) = ANNEE
shResultats.Cells(LigneAnnee, Col_TITRE) = TITRE
shResultats.Cells(LigneAnnee, Col_PRESTA) = PRESTA
shResultats.Cells(LigneAnnee, Col_PROV) = PROV
shResultats.Cells(LigneAnnee, Col_CB) = CB
shResultats.Cells(LigneAnnee, Col_TAUX_GESTION) = TAUX_GESTION
shResultats.Cells(LigneAnnee, Col_TAUX_TAXES) = TAUX_TAXES
shResultats.Cells(LigneAnnee, Col_CHARGEMENTS) = CHARGEMENTS
shResultats.Cells(LigneAnnee, Col_CN) = CN
shResultats.Cells(LigneAnnee, Col_RATIO) = RATIO
shResultats.Cells(LigneAnnee, Col_GAINS_PERTES) = GAINS_PERTES
shResultats.Cells(LigneAnnee, Col_TAUX_TAXE_COVID) = TAUX_TAXE_COVID

If TAUX_TAXE_COVID <> 0 Then
    Cells(LigneAnnee, Col_TAUX_TAXE_COVID) = "Taux Taxe COVID = " & TAUX_TAXE_COVID
Else
    Cells(LigneAnnee, Col_TAUX_TAXE_COVID) = ""
End If


End Function



Function MAJ_FAMILLE_COLLEGE(shCollege As Worksheet, shDEMOGRAPHIE As Worksheet, shPrestations As Worksheet, shProvisions As Worksheet, shCotisations As Worksheet) As Double

Dim i As Double
Dim j As Double
Dim FAMCOL As String

' chargement DEMOGRAPHIE colonne "FAMILLE COLLEGE" avec la feuille College
shDEMOGRAPHIE.Select
Range("J:J").ClearContents
Cells(1, 10) = "FAMILLE COLLEGE"
FAMCOL = "B:C" ' dans feuille college
j = 2
While Cells(j, 1) <> ""
Cells(j, 10) = Application.VLookup(Cells(j, 9), shCollege.Range(FAMCOL), 2, False)
j = j + 1
Wend
   
' chargement PRESTATIONS colonne "FAMILLE COLLEGE" avec la feuille College
shPrestations.Select
Range("R:R").ClearContents
Cells(1, 18) = "FAMILLE COLLEGE"
FAMCOL = "B:C" ' dans feuille college
j = 2
While Cells(j, 1) <> ""
Cells(j, 18) = Application.VLookup(Cells(j, 3), shCollege.Range(FAMCOL), 2, False)
j = j + 1
Wend
        
' chargement PROVISIONS colonne "FAMILLE COLLEGE" avec la feuille College
shProvisions.Select
Range("H:H").ClearContents
Cells(1, 8) = "FAMILLE COLLEGE"
FAMCOL = "B:C" ' dans feuille college
j = 2
While Cells(j, 1) <> ""
Cells(j, 8) = Application.VLookup(Cells(j, 3), shCollege.Range(FAMCOL), 2, False)
j = j + 1
Wend

' chargement COTISATIONS colonne "FAMILLE COLLEGE" avec la feuille College
shCotisations.Select
Range("G:G").ClearContents
Cells(1, 7) = "FAMILLE COLLEGE"
FAMCOL = "B:C" ' dans feuille college
j = 2
While Cells(j, 1) <> ""
Cells(j, 7) = Application.VLookup(Cells(j, 4), shCollege.Range(FAMCOL), 2, False)
j = j + 1
Wend


End Function

Function CumulFrais(shDonnees As Worksheet, ANNEE As String, LibelleFrais As String) As Double
' les frais en colonne J doivent être numérique pour que la somme fonctionne
' sinon la somme retournée =0 si la colonne J est alphanumérique)
' CumulFrais
CumulFrais = 0
CumulFrais = Application.WorksheetFunction.SumIfs(shDonnees.Range("J:J"), shDonnees.Range("H:H"), ANNEE, shDonnees.Range("I:I"), LibelleFrais)
End Function








