Attribute VB_Name = "CONTROLE"
Option Explicit
Public PgBar As New ProgressBar

'******************************
' CONTROLE_V4.0.0_du_28_09_2023
'*****************************'
Sub CONTROLE()

On Error GoTo err_CONTROLE

'VARIABLE
Dim Bloc As Integer
Dim TotalBloc As Integer
Dim bAffichageProgressbarre As Boolean

' type de Compte
Dim bTypeCompte As Boolean
Dim TypeCompteComptable As String
Dim TypeCompteSurvenance As String

' controle à traiter
Dim TYPE_CONTROLE As String
Dim SYNTHESE_PREV As String
Dim GLOBAL_PREV As String
Dim PREV_1AN As String
Dim SYNTHESE_SANTE As String
Dim GLOBAL_SANTE As String
Dim SANTE_1AN As String
Dim SANTE_2ANS As String

'chargement nom du contrôle
SYNTHESE_PREV = "SYNTHESE_PREV"
GLOBAL_PREV = "GLOBAL_PREV"
PREV_1AN = "PREV_1AN"
SYNTHESE_SANTE = "SYNTHESE_SANTE"
GLOBAL_SANTE = "GLOBAL_SANTE"
SANTE_1AN = "SANTE_1AN"
SANTE_2ANS = "SANTE_2ANS"

Dim PeriodeDebutAFFICHAGE As String
Dim periodeFinAFFICHAGE As String
Dim dateDebutAffichage As Date
Dim dateFinAffichage As Date
Dim AnneeDebutAffichage As Integer
Dim AnneeFinAffichage As Integer
Dim NbAnneeATraiter As Integer
Dim NbAnneeLue As Integer
Dim AnneeTraitee As Integer

Dim NomModule As String
Dim p As Double
Dim NoLigneEnErreur As Double
Dim MessageErreur As String
Dim LibelleFamille As String
Dim LibelleActe As String

' déclaration des ONGLETS utilisés
Dim shResultats_SYNTHESE As Worksheet
Dim shResultats_CONTROLE As Worksheet
Dim shResultats_DATA_CUMUL As Worksheet
Dim shResultats_RESULTATS As Worksheet
Dim shResultats_AFFICHAGE As Worksheet
Dim shResultats_AFFICHAGE_2 As Worksheet
Dim shErreurs As Worksheet

' déclaration des noms des ONGLETS
Dim nomFeuille_SYNTHESE As String
Dim nomFeuille_CONTROLE As String
Dim nomFeuille_DATA_CUMUL As String
Dim nomFeuille_AFFICHAGE As String
Dim nomFeuille_AFFICHAGE_2 As String
Dim nomFeuille_RESULTATS As String
Dim nomFeuille_ERREURS As String

'  chargement des noms des ONGLETS
nomFeuille_SYNTHESE = "SYNTHESE"
nomFeuille_CONTROLE = "CONTROLE"
nomFeuille_DATA_CUMUL = "DATA_CUMUL"
nomFeuille_AFFICHAGE = "AFFICHAGE"
nomFeuille_AFFICHAGE_2 = "AFFICHAGE-2"
nomFeuille_RESULTATS = "Résultats"
nomFeuille_ERREURS = "Erreurs"

' Chargement des ONGLETS
NomModule = "Sélection des feuilles pour le CONTROLE"
Set shResultats_CONTROLE = Worksheets(nomFeuille_CONTROLE)
Set shResultats_AFFICHAGE = Worksheets(nomFeuille_AFFICHAGE)
Set shResultats_AFFICHAGE_2 = Worksheets(nomFeuille_AFFICHAGE_2)
'Set shResultats_SYNTHESE = Worksheets(nomFeuille_SYNTHESE) ou Worksheets(nomFeuille_DATA_CUMUL) ' activation ultérieure en fonction du type de controle
'Set shResultats_DATA_CUMUL = Worksheets(nomFeuille_DATA_CUMUL) ' activation ultérieure en fonction du type de controle
'Set shResultats_RESULTATS = Worksheets(nomFeuille_RESULTATS) ' activation ultérieure en fonction du type de controle


' tableaux mémoire à charger
Dim Tableau_SYNTHESE(50, 10) ' avec les lignes TOTAL dans SYNTHESE
Dim Tableau_CONTROLE(50, 10) ' avec toutes les lignes CONTROLE
Dim Tableau_FUSION(100, 10) ' FUSION DES 2 TABLEAUX SYNTHESE+CONTROLE
Dim Tableau_ANNEE(100) ' TABLEAUX DES ANNEES SANS DOUBLONS pour contruire le tableau ECARTS
Dim AnneeTableauDejaPresente As Boolean
Dim PresenceAnnee As Boolean
Dim AjoutAnnee As Boolean
Dim PresenceECART As Boolean
Dim Tableau_ECART(100, 10) ' TABLEAUX DES ECARTS (SYNTHESE-CONTROLE)

'année   Prestations Provisions  Cotisations brutes  Chargements Cotisations nettes  Ratio combiné   Gains/Pertes
Dim wAnnee As Double
Dim wPrestations As Double
Dim wProvisions As Double
Dim wCotBrutes As Double
Dim wChargements As Double
Dim wCotNettes As Double
Dim wRatio As Double
Dim wGainsPertes As Double


Dim i As Integer ' no ligne tableau
Dim k As Integer ' no ligne tableau
Dim iMax As Integer ' no ligne tableau maximum
iMax = 50
Dim iMaxFusion As Integer ' no ligne tableau maximum
iMaxFusion = 100
Dim iSYNTHESE As Integer
Dim iCONTROLE As Integer

Dim DerniereLigneFusion As Integer
Dim j As Integer ' no colonne tableau
Dim jMax As Integer ' no colonne tableau maximum
jMax = 10

' ligne avec Libelle ECART
Dim Libelle As String
Dim LibelleEcart As String
Dim LibelleAucunEcart As String
Dim Couleur As String
Dim CouleurRouge As String
Dim CouleurVerte As String
Dim CouleurCaractere As String
Dim CouleurCaractereRouge As String
Dim CouleurCaractereNoir As String

LibelleEcart = " E C A R T"
LibelleAucunEcart = "A U C U N    E C A R T"
CouleurVerte = 5296274
CouleurRouge = 255
'CaractereRouge = -16776961
CouleurCaractereNoir = -16777216
CouleurCaractereRouge = 255
Dim Zone1 As String ' zone à selectionner
Dim Zone2 As String ' zone à selectionner

Dim SourceSYNTHESE As String ' nom de la source  = "SYNTHESE"
Dim SourceCONTROLE As String ' nom de la source  = "CONTROLE"
Dim SourceECART As String ' nom de la source  = "ECART"

Dim Ligne_Depart_SYNTHESE As Integer
Dim Ligne_Final_SYNTHESE As Integer ' ligne avec total
Dim Ligne_SYNTHESE As Integer
Dim Ligne_SYNTHESE_ANNEE1 As Integer
Dim Ligne_SYNTHESE_ANNEE2 As Integer
Dim Ligne_titre_AFFICHAGE_SYNTHESE As Integer  ' titre tableau SYNTHESE affiché dans onglet CONTROLE
Dim Ligne_AFFICHAGE_SYNTHESE As Integer ' ligne tableau SYNTHESE affiché dans onglet CONTROLE
Dim Ligne_titre_AFFICHAGE_ECART As Integer  ' titre tableau ECART affiché dans onglet CONTROLE
Dim Ligne_AFFICHAGE_ECART As Integer ' ligne tableau ECART affiché dans onglet CONTROLE

Dim Ligne_Depart_CONTROLE As Integer
Dim Ligne_Titre_CONTROLE As Integer
Dim Ligne_Rubriques_CONTROLE As Integer
Dim Ligne_Fin_CONTROLE As Integer
Dim Ligne_CONTROLE As Integer
Dim Presence_CONTROLE_ANNEE As Boolean ' présence année dans onglet CONTROLE

Dim Ligne_AFFICHAGE_MESSAGE_ECART As String ' ligne avec message "E C A R T" ou "A U C U N    E C A R T"
Dim Presence_Ligne_AFFICHAGE_MESSAGE_ECART As String
Dim Libelle_TOTAL As String ' valeur RECHERCHE pour identifier la ligne TOTAL

Dim COL_SYNTHESE_SURVENANCE As String  ' colonne de la SURVENANCE dans la SYNTHESE
Dim COL_SYNTHESE_GARANTIES As String
Dim COL_SYNTHESE_PRESTATIONS As String
Dim COL_SYNTHESE_PROVISIONS As String
Dim COL_SYNTHESE_COTISATIONS_BRUTES As String
Dim COL_SYNTHESE_CHARGEMENTS As String
Dim COL_SYNTHESE_COTISATIONS_NETTES As String
Dim COL_SYNTHESE_RATIO As String
Dim COL_SYNTHESE_GAINS_PERTES As String

Dim COL_CONTROLE_SURVENANCE As String  ' COLONNE de la SURVENANCE dans la SYNTHESE ou CONTROLE
Dim COL_CONTROLE_GARANTIES As String
Dim COL_CONTROLE_PRESTATIONS As String
Dim COL_CONTROLE_PROVISIONS As String
Dim COL_CONTROLE_COTISATIONS_BRUTES As String
Dim COL_CONTROLE_CHARGEMENTS As String
Dim COL_CONTROLE_COTISATIONS_NETTES As String
Dim COL_CONTROLE_RATIO As String
Dim COL_CONTROLE_GAINS_PERTES As String

Dim Selection_PRESTATIONS As String
Dim Selection_PROVISIONS As String
Dim Selection_COTISATIONS_BRUTES As String
Dim Selection_COTISATIONS_NETTES As String
Dim Selection_GAINS_PERTES As String
Dim Selection_ANNEE As String


' taux frais de Sante
Dim COL_TITRE_FRAIS_SANTE As String
Dim COL_CONTROLE_TAUX_GESTION As String
Dim COL_CONTROLE_TAUX_TAXE_COVID As String
Dim COL_CONTROLE_TAUX_TAXES As String

Dim LIB_CONTROLE_TAUX_GESTION As String
Dim LIB_CONTROLE_TAUX_TAXE_COVID As String
Dim LIB_CONTROLE_TAUX_TAXES As String

'Dim ANNEE As Double
Dim ANNEE As String
Dim FraisGestion As Double

'***********************************************************************
'0.0) chargement TypeDeCompte = COMPTABLE OU SURVENANCE ET ANNEE_COMPTABLE
'***********************************************************************

'Les années sont forcées à zéro dans les tableaux

For i = 1 To iMax
 Tableau_SYNTHESE(i, 2) = 0 ' année = 0
 Tableau_CONTROLE(i, 2) = 0 ' année = 0
Next i

For i = 1 To iMaxFusion
 Tableau_FUSION(i, 2) = 0 ' année = 0
 Tableau_ANNEE(i) = 0 ' année = 0
 Tableau_ECART(i, 2) = 0 ' année = 0
Next i


'TypeDeCompte
TypeCompteComptable = "Comptable"
TypeCompteSurvenance = "Survenance"
If shResultats_AFFICHAGE.Cells(2, 19) = TypeCompteComptable Then
    bTypeCompte = True 'Comptable
Else
    bTypeCompte = False 'Survenance
End If

'***********************************************************************
'0.1) chargement TYPE DE CONTROLE et PERIODE
'***********************************************************************

TYPE_CONTROLE = shResultats_AFFICHAGE.Cells(1, 20) ' TYPE DE CONTROLE

' chargement des péridodes à traiter
NbAnneeLue = 0 ' nb lue lue dans SYNTHESE
NbAnneeATraiter = 0 ' pas de chargement des données SYNTHESE

PeriodeDebutAFFICHAGE = shResultats_AFFICHAGE.Cells(2, 13)
periodeFinAFFICHAGE = shResultats_AFFICHAGE.Cells(2, 14)

If PeriodeDebutAFFICHAGE <> "" And PeriodeDebutAFFICHAGE <> "" Then
    dateDebutAffichage = PeriodeDebutAFFICHAGE
    dateFinAffichage = periodeFinAFFICHAGE
    AnneeDebutAffichage = Year(dateDebutAffichage)
    AnneeFinAffichage = Year(dateFinAffichage)
    NbAnneeATraiter = AnneeFinAffichage - AnneeDebutAffichage + 1
End If


'********************************************************************************************************
'1 a) CHARGEMENT DU TABLEAU SYNTHESE PREV des toutes lignes TOTALES
'********************************************************************************************************

Select Case TYPE_CONTROLE

Case SYNTHESE_PREV   ' modèle SYNTHESE_PREV

        Set shResultats_SYNTHESE = Worksheets(nomFeuille_SYNTHESE)
        SourceSYNTHESE = shResultats_SYNTHESE.Name
        
        Ligne_Depart_SYNTHESE = 5  ' début du tableu synthese
        Libelle_TOTAL = "TOTAL" ' valeur RECHERCHE pour identifier la ligne TOTAL
        COL_SYNTHESE_SURVENANCE = "C" ' COL_SYNTHESE de la SURVENANCE dans le SYNTHESE_PREV
        COL_SYNTHESE_GARANTIES = "D"
        COL_SYNTHESE_PRESTATIONS = "E"
        COL_SYNTHESE_PROVISIONS = "F"
        COL_SYNTHESE_COTISATIONS_BRUTES = "G"
        COL_SYNTHESE_CHARGEMENTS = "H"
        COL_SYNTHESE_COTISATIONS_NETTES = "I"
        COL_SYNTHESE_RATIO = "J"
        COL_SYNTHESE_GAINS_PERTES = "K"

'CHARGEMENT des valeurs lues dans LE TABLEAU SYNTHESE
i = 1
j = 0
Ligne_SYNTHESE = Ligne_Depart_SYNTHESE

If NbAnneeATraiter <> 0 Then ' aucun chargement des données SYNTHESE à faire
    While NbAnneeLue < NbAnneeATraiter
        If shResultats_SYNTHESE.Range(COL_SYNTHESE_GARANTIES & Ligne_SYNTHESE) = Libelle_TOTAL Then
            
            wAnnee = shResultats_SYNTHESE.Range(COL_SYNTHESE_SURVENANCE & Ligne_SYNTHESE) ' année
            wPrestations = shResultats_SYNTHESE.Range(COL_SYNTHESE_PRESTATIONS & Ligne_SYNTHESE) ' prestations
            wProvisions = shResultats_SYNTHESE.Range(COL_SYNTHESE_PROVISIONS & Ligne_SYNTHESE) ' provisions
            wCotBrutes = shResultats_SYNTHESE.Range(COL_SYNTHESE_COTISATIONS_BRUTES & Ligne_SYNTHESE) ' cotisations brutes
            wChargements = shResultats_SYNTHESE.Range(COL_SYNTHESE_CHARGEMENTS & Ligne_SYNTHESE) ' chargements
            wCotNettes = shResultats_SYNTHESE.Range(COL_SYNTHESE_COTISATIONS_NETTES & Ligne_SYNTHESE) ' cotisations nettes
            wRatio = shResultats_SYNTHESE.Range(COL_SYNTHESE_RATIO & Ligne_SYNTHESE) ' ratio
            wGainsPertes = shResultats_SYNTHESE.Range(COL_SYNTHESE_GAINS_PERTES & Ligne_SYNTHESE) ' gains/pertes
            
            Tableau_SYNTHESE(i, 1) = SourceSYNTHESE  '
            Tableau_SYNTHESE(i, 2) = Round(wAnnee, 0) ' année
            Tableau_SYNTHESE(i, 3) = Round(wPrestations, 2) ' prestations
            Tableau_SYNTHESE(i, 4) = Round(wProvisions, 2) ' provisions
            Tableau_SYNTHESE(i, 5) = Round(wCotBrutes, 2) ' cotisations brutes
            Tableau_SYNTHESE(i, 6) = Round(wChargements, 4) ' chargements
            Tableau_SYNTHESE(i, 7) = Round(wCotNettes, 2) ' cotisations nettes
            Tableau_SYNTHESE(i, 8) = Round(wRatio, 4) ' ratio
            Tableau_SYNTHESE(i, 9) = Round(wGainsPertes, 2) ' gains/pertes
            Tableau_SYNTHESE(i, 10) = 0 ' écart forcé à 0
             
            NbAnneeLue = NbAnneeLue + 1
            i = i + 1
            If i > iMax Then             ' controle de iMax
                LibelleFamille = "ATTENTION AUGMENTER le nombre de lignes i pour le Tableau_Synthese, la limite iMax est atteinte =  " & iMax
            GoTo err_CONTROLE:
            Else
            End If
        Else
        End If
            Ligne_SYNTHESE = Ligne_SYNTHESE + 1     ' ligne suivante
            If Ligne_SYNTHESE > 100 Then     ' controle du nb de lignes
                LibelleFamille = "ATTENTION RECHERCHE des cumuls ANNEE est INTERROMPUE dans l'ONGLET " & SourceSYNTHESE & "..il y a déjà " & Ligne_SYNTHESE & " lignes analysées sans ANNEE "
                GoTo err_CONTROLE:
            Else
            End If
    Wend
Else
End If

'********************************************************************************************************
'1 b) CHARGEMENT DU TABLEAU SYNTHESE SANTE
'********************************************************************************************************

Case SYNTHESE_SANTE ' modèle SYNTHESE_SANTE
    
        Set shResultats_SYNTHESE = Worksheets(nomFeuille_SYNTHESE)
        SourceSYNTHESE = shResultats_SYNTHESE.Name
         
         Ligne_Depart_SYNTHESE = 5  ' début du tableu synthese
        
        Libelle_TOTAL = "" ' non utilisé
        COL_SYNTHESE_SURVENANCE = "B" ' COL_SYNTHESE de la SURVENANCE dans le SYNTHESE_SANTE"
        COL_SYNTHESE_GARANTIES = "" ' non utilisé
        COL_SYNTHESE_PRESTATIONS = "C"
        COL_SYNTHESE_PROVISIONS = "D"
        COL_SYNTHESE_COTISATIONS_BRUTES = "E"
        COL_SYNTHESE_CHARGEMENTS = "F"
        COL_SYNTHESE_COTISATIONS_NETTES = "G"
        COL_SYNTHESE_RATIO = "H"
        COL_SYNTHESE_GAINS_PERTES = "I"

'CHARGEMENT des valeurs lues dans LE TABLEAU SYNTHESE
i = 1
j = 0
'Dim variable As Double
Ligne_SYNTHESE = Ligne_Depart_SYNTHESE

If NbAnneeATraiter <> 0 Then ' aucun chargement des données SYNTHESE à faire
    While NbAnneeLue < NbAnneeATraiter
        If shResultats_SYNTHESE.Range(COL_SYNTHESE_SURVENANCE & Ligne_SYNTHESE) <> "" Then
            
            wAnnee = shResultats_SYNTHESE.Range(COL_SYNTHESE_SURVENANCE & Ligne_SYNTHESE) ' année
            wPrestations = shResultats_SYNTHESE.Range(COL_SYNTHESE_PRESTATIONS & Ligne_SYNTHESE) ' prestations
            wProvisions = shResultats_SYNTHESE.Range(COL_SYNTHESE_PROVISIONS & Ligne_SYNTHESE) ' provisions
            wCotBrutes = shResultats_SYNTHESE.Range(COL_SYNTHESE_COTISATIONS_BRUTES & Ligne_SYNTHESE) ' cotisations brutes
            wChargements = shResultats_SYNTHESE.Range(COL_SYNTHESE_CHARGEMENTS & Ligne_SYNTHESE) ' chargements
            wCotNettes = shResultats_SYNTHESE.Range(COL_SYNTHESE_COTISATIONS_NETTES & Ligne_SYNTHESE) ' cotisations nettes
            wRatio = shResultats_SYNTHESE.Range(COL_SYNTHESE_RATIO & Ligne_SYNTHESE) ' ratio
            wGainsPertes = shResultats_SYNTHESE.Range(COL_SYNTHESE_GAINS_PERTES & Ligne_SYNTHESE) ' gains/pertes
            
            Tableau_SYNTHESE(i, 1) = SourceSYNTHESE  '
            Tableau_SYNTHESE(i, 2) = Round(wAnnee, 0) ' année
            Tableau_SYNTHESE(i, 3) = Round(wPrestations, 2) ' prestations
            Tableau_SYNTHESE(i, 4) = Round(wProvisions, 2) ' provisions
            Tableau_SYNTHESE(i, 5) = Round(wCotBrutes, 2) ' cotisations brutes
            Tableau_SYNTHESE(i, 6) = Round(wChargements, 4) ' chargements
            Tableau_SYNTHESE(i, 7) = Round(wCotNettes, 2) ' cotisations nettes
            Tableau_SYNTHESE(i, 8) = Round(wRatio, 4) ' ratio
            Tableau_SYNTHESE(i, 9) = Round(wGainsPertes, 2) ' gains/pertes
            Tableau_SYNTHESE(i, 10) = 0 ' écart forcé à 0
            
            NbAnneeLue = NbAnneeLue + 1
            i = i + 1
            If i > iMax Then             ' controle de iMax
                LibelleFamille = "ATTENTION AUGMENTER le nombre de lignes i pour le Tableau_Synthese, la limite iMax est atteinte =  " & iMax
            GoTo err_CONTROLE:
            Else
            End If
        Else
        End If
            Ligne_SYNTHESE = Ligne_SYNTHESE + 1     ' ligne suivante
            If Ligne_SYNTHESE > 100 Then     ' controle du nb de lignes
                LibelleFamille = "ATTENTION RECHERCHE des cumuls ANNEE est INTERROMPUE dans l'ONGLET " & SourceSYNTHESE & "..il y a déjà " & Ligne_SYNTHESE & " lignes analysées sans ANNEE "
                GoTo err_CONTROLE:
            Else
            End If
    Wend
Else
End If


'********************************************************************************************************
'1 C) CHARGEMENT DU TABLEAU GLOBAL_PREV
'********************************************************************************************************

Case GLOBAL_PREV ' modèle GLOBAL_PREV
        
        Set shResultats_DATA_CUMUL = Worksheets(nomFeuille_DATA_CUMUL)
        SourceSYNTHESE = shResultats_DATA_CUMUL.Name  ' onglet DATA_CUMUL
        
        Ligne_Depart_SYNTHESE = 0  ' ' non utilisé
        Libelle_TOTAL = "" ' non utilisé
        COL_SYNTHESE_SURVENANCE = "D" ' année de Survenance
        COL_SYNTHESE_GARANTIES = "" ' non utilisé
        COL_SYNTHESE_PRESTATIONS = "F"
        COL_SYNTHESE_PROVISIONS = "J"
        COL_SYNTHESE_COTISATIONS_BRUTES = "K"
        COL_SYNTHESE_CHARGEMENTS = "" ' à recalculer
        COL_SYNTHESE_COTISATIONS_NETTES = "M"
        COL_SYNTHESE_RATIO = "" ' à recalculer
        COL_SYNTHESE_GAINS_PERTES = "O"
        
        ' selection des rubriques
        Selection_PRESTATIONS = COL_SYNTHESE_PRESTATIONS & ":" & COL_SYNTHESE_PRESTATIONS
        Selection_PROVISIONS = COL_SYNTHESE_PROVISIONS & ":" & COL_SYNTHESE_PROVISIONS
        Selection_COTISATIONS_BRUTES = COL_SYNTHESE_COTISATIONS_BRUTES & ":" & COL_SYNTHESE_COTISATIONS_BRUTES
        Selection_COTISATIONS_NETTES = COL_SYNTHESE_COTISATIONS_NETTES & ":" & COL_SYNTHESE_COTISATIONS_NETTES
        Selection_PRESTATIONS = COL_SYNTHESE_PRESTATIONS & ":" & COL_SYNTHESE_PRESTATIONS
        Selection_GAINS_PERTES = COL_SYNTHESE_GAINS_PERTES & ":" & COL_SYNTHESE_GAINS_PERTES
        Selection_ANNEE = COL_SYNTHESE_SURVENANCE & ":" & COL_SYNTHESE_SURVENANCE
        
'CHARGEMENT des valeurs lues dans LE TABLEAU SYNTHESE
i = 1
j = 0

'ProgressBarre
Bloc = 0
TotalBloc = 12
PgBar.SetProgressValue 0: Bloc = 0
PgBar.Show
DoEvents

' ProgressBarre
Bloc = Bloc + 1
'bAffichageProgressbarre = AffichageProgressbarre(Bloc, TotalBloc)

If NbAnneeATraiter <> 0 Then ' aucun chargement des données SYNTHESE à faire

    For AnneeTraitee = AnneeDebutAffichage To AnneeFinAffichage
    
            wAnnee = AnneeTraitee ' année
            wPrestations = CumulMontant(shResultats_DATA_CUMUL, Selection_PRESTATIONS, Selection_ANNEE, AnneeTraitee, bTypeCompte)  ' prestations
            wProvisions = CumulMontant(shResultats_DATA_CUMUL, Selection_PROVISIONS, Selection_ANNEE, AnneeTraitee, bTypeCompte) ' provisions
            wCotBrutes = CumulMontant(shResultats_DATA_CUMUL, Selection_COTISATIONS_BRUTES, Selection_ANNEE, AnneeTraitee, bTypeCompte) ' cotisations brutes
            wCotNettes = CumulMontant(shResultats_DATA_CUMUL, Selection_COTISATIONS_NETTES, Selection_ANNEE, AnneeTraitee, bTypeCompte) ' cotisations nettes
            wGainsPertes = CumulMontant(shResultats_DATA_CUMUL, Selection_GAINS_PERTES, Selection_ANNEE, AnneeTraitee, bTypeCompte) ' gains/pertes
            
            Tableau_SYNTHESE(i, 1) = SourceSYNTHESE  '
            Tableau_SYNTHESE(i, 2) = AnneeTraitee ' année
            Tableau_SYNTHESE(i, 3) = wPrestations ' prestations
            Tableau_SYNTHESE(i, 4) = wProvisions ' provisions
            Tableau_SYNTHESE(i, 5) = wCotBrutes ' cotisations brutes
            
            If wCotBrutes <> 0 Then 'chargements
            wChargements = Round(1 - wCotNettes / wCotBrutes, 4) '= (1 - CotNette / CotBrute) ' chargements
            Tableau_SYNTHESE(i, 6) = wChargements
            End If
            
            Tableau_SYNTHESE(i, 7) = wCotNettes ' cotisations nettes
            
            If wCotNettes <> 0 Then ' ratio
            wRatio = Round((wPrestations + wProvisions) / wCotNettes, 4) ' ratio
            Tableau_SYNTHESE(i, 8) = wRatio
            End If
            
            Tableau_SYNTHESE(i, 9) = wGainsPertes ' gains/pertes
            Tableau_SYNTHESE(i, 10) = 0 ' écart forcé à 0
            
            
            NbAnneeLue = NbAnneeLue + 1
            
            i = i + 1
            If i > iMax Then             ' controle de iMax
                LibelleFamille = "ATTENTION AUGMENTER le nombre de lignes i pour le Tableau_Synthese, la limite iMax est atteinte =  " & iMax
            GoTo err_CONTROLE:
            Else
            End If
            
    Next AnneeTraitee
    
Else
End If

'********************************************************************************************************
'1 D) CHARGEMENT DU TABLEAU GLOBAL_SANTE
'********************************************************************************************************

Case GLOBAL_SANTE ' modèle GLOBAL_SANTE
        
        Set shResultats_DATA_CUMUL = Worksheets(nomFeuille_DATA_CUMUL)
        SourceSYNTHESE = shResultats_DATA_CUMUL.Name  ' onglet DATA_CUMUL
        
        Ligne_Depart_SYNTHESE = 0  ' ' non utilisé
        Libelle_TOTAL = "" ' non utilisé
        COL_SYNTHESE_SURVENANCE = "D" ' année de Survenance
        COL_SYNTHESE_GARANTIES = "" ' non utilisé
        COL_SYNTHESE_PRESTATIONS = "E"
        COL_SYNTHESE_PROVISIONS = "F"
        COL_SYNTHESE_COTISATIONS_BRUTES = "G"
        COL_SYNTHESE_CHARGEMENTS = "" ' à recalculer
        COL_SYNTHESE_COTISATIONS_NETTES = "I"
        COL_SYNTHESE_RATIO = "" ' à recalculer
        COL_SYNTHESE_GAINS_PERTES = "K"
        
        ' selection des rubriques
        Selection_PRESTATIONS = COL_SYNTHESE_PRESTATIONS & ":" & COL_SYNTHESE_PRESTATIONS
        Selection_PROVISIONS = COL_SYNTHESE_PROVISIONS & ":" & COL_SYNTHESE_PROVISIONS
        Selection_COTISATIONS_BRUTES = COL_SYNTHESE_COTISATIONS_BRUTES & ":" & COL_SYNTHESE_COTISATIONS_BRUTES
        Selection_COTISATIONS_NETTES = COL_SYNTHESE_COTISATIONS_NETTES & ":" & COL_SYNTHESE_COTISATIONS_NETTES
        Selection_PRESTATIONS = COL_SYNTHESE_PRESTATIONS & ":" & COL_SYNTHESE_PRESTATIONS
        Selection_GAINS_PERTES = COL_SYNTHESE_GAINS_PERTES & ":" & COL_SYNTHESE_GAINS_PERTES
        Selection_ANNEE = COL_SYNTHESE_SURVENANCE & ":" & COL_SYNTHESE_SURVENANCE
        
'CHARGEMENT des valeurs lues dans LE TABLEAU SYNTHESE
i = 1
j = 0

'ProgressBarre
Bloc = 0
TotalBloc = 12
PgBar.SetProgressValue 0: Bloc = 0
PgBar.Show
DoEvents

' ProgressBarre
Bloc = Bloc + 1
'bAffichageProgressbarre = AffichageProgressbarre(Bloc, TotalBloc)

If NbAnneeATraiter <> 0 Then ' aucun chargement des données SYNTHESE à faire

    For AnneeTraitee = AnneeDebutAffichage To AnneeFinAffichage
            
            wAnnee = AnneeTraitee ' année
            wPrestations = CumulMontant(shResultats_DATA_CUMUL, Selection_PRESTATIONS, Selection_ANNEE, AnneeTraitee, bTypeCompte)  ' prestations
            wProvisions = CumulMontant(shResultats_DATA_CUMUL, Selection_PROVISIONS, Selection_ANNEE, AnneeTraitee, bTypeCompte) ' provisions
            wCotBrutes = CumulMontant(shResultats_DATA_CUMUL, Selection_COTISATIONS_BRUTES, Selection_ANNEE, AnneeTraitee, bTypeCompte) ' cotisations brutes
            wCotNettes = CumulMontant(shResultats_DATA_CUMUL, Selection_COTISATIONS_NETTES, Selection_ANNEE, AnneeTraitee, bTypeCompte) ' cotisations nettes
            wGainsPertes = CumulMontant(shResultats_DATA_CUMUL, Selection_GAINS_PERTES, Selection_ANNEE, AnneeTraitee, bTypeCompte) ' gains/pertes
            
            Tableau_SYNTHESE(i, 1) = SourceSYNTHESE  '
            Tableau_SYNTHESE(i, 2) = AnneeTraitee ' année
            Tableau_SYNTHESE(i, 3) = wPrestations ' prestations
            Tableau_SYNTHESE(i, 4) = wProvisions ' provisions
            Tableau_SYNTHESE(i, 5) = wCotBrutes ' cotisations brutes
            
            If wCotBrutes <> 0 Then 'chargements
            wChargements = Round(1 - wCotNettes / wCotBrutes, 4) '= (1 - CotNette / CotBrute) ' chargements
            Tableau_SYNTHESE(i, 6) = wChargements
            End If
            
            Tableau_SYNTHESE(i, 7) = wCotNettes ' cotisations nettes
            
            If wCotNettes <> 0 Then ' ratio
            wRatio = Round((wPrestations + wProvisions) / wCotNettes, 4) ' ratio
            Tableau_SYNTHESE(i, 8) = wRatio
            End If
            
            Tableau_SYNTHESE(i, 9) = wGainsPertes ' gains/pertes
            Tableau_SYNTHESE(i, 10) = 0 ' écart forcé à 0
            
            
            NbAnneeLue = NbAnneeLue + 1
            
            i = i + 1
            If i > iMax Then             ' controle de iMax
                LibelleFamille = "ATTENTION AUGMENTER le nombre de lignes i pour le Tableau_Synthese, la limite iMax est atteinte =  " & iMax
            GoTo err_CONTROLE:
            Else
            End If
            
    Next AnneeTraitee
    
Else
End If

'********************************************************************************************************
'1 E) CHARGEMENT DU TABLEAU PREV_1AN
'********************************************************************************************************

Case PREV_1AN  ' modèle PREV_1AN
        
        Set shResultats_RESULTATS = Worksheets(nomFeuille_RESULTATS)
        SourceSYNTHESE = shResultats_RESULTATS.Name  ' onglet RESULTATS
        
        Ligne_Depart_SYNTHESE = 17  ' ligne avec l'année
        Ligne_Final_SYNTHESE = 23  ' ligne avec le Total
        Libelle_TOTAL = "TOTAL" ' valeur RECHERCHée pour identifier la ligne TOTAL
        
        COL_SYNTHESE_SURVENANCE = "C" ' année de Survenance
        COL_SYNTHESE_GARANTIES = "E"
        COL_SYNTHESE_PRESTATIONS = "G"
        COL_SYNTHESE_PROVISIONS = "I"
        COL_SYNTHESE_COTISATIONS_BRUTES = "K"
        COL_SYNTHESE_CHARGEMENTS = "O" ' à recalculer
        COL_SYNTHESE_COTISATIONS_NETTES = "Q"
        COL_SYNTHESE_RATIO = "S" ' à recalculer
        COL_SYNTHESE_GAINS_PERTES = "U"
        
'CHARGEMENT des valeurs lues dans LE TABLEAU SYNTHESE
i = 1
j = 0
Ligne_SYNTHESE = Ligne_Depart_SYNTHESE

If NbAnneeATraiter <> 0 Then ' aucun chargement des données SYNTHESE à faire

            Tableau_SYNTHESE(i, 1) = SourceSYNTHESE  '
            Ligne_SYNTHESE = Ligne_Final_SYNTHESE ' ligne avec le total
            Tableau_SYNTHESE(i, 2) = shResultats_RESULTATS.Range(COL_SYNTHESE_SURVENANCE & Ligne_SYNTHESE) ' année
            Tableau_SYNTHESE(i, 3) = shResultats_RESULTATS.Range(COL_SYNTHESE_PRESTATIONS & Ligne_SYNTHESE) ' prestations
            Tableau_SYNTHESE(i, 4) = shResultats_RESULTATS.Range(COL_SYNTHESE_PROVISIONS & Ligne_SYNTHESE) ' provisions
            Tableau_SYNTHESE(i, 5) = shResultats_RESULTATS.Range(COL_SYNTHESE_COTISATIONS_BRUTES & Ligne_SYNTHESE) ' cotisations brutes
            Tableau_SYNTHESE(i, 7) = shResultats_RESULTATS.Range(COL_SYNTHESE_COTISATIONS_NETTES & Ligne_SYNTHESE) ' cotisations nettes
            Tableau_SYNTHESE(i, 8) = shResultats_RESULTATS.Range(COL_SYNTHESE_RATIO & Ligne_SYNTHESE) ' ratio
            Tableau_SYNTHESE(i, 9) = shResultats_RESULTATS.Range(COL_SYNTHESE_GAINS_PERTES & Ligne_SYNTHESE) ' gains/pertes
            Tableau_SYNTHESE(i, 10) = 0 ' écart forcé à 0
            
            If Tableau_SYNTHESE(i, 5) <> 0 Then 'chargements
            Tableau_SYNTHESE(i, 6) = Round(1 - Tableau_SYNTHESE(i, 7) / Tableau_SYNTHESE(i, 5), 4) '= '(1 - CotNette / CotBrute) ' chargements
            Else
            End If
End If

'ProgressBarre
Bloc = 0
TotalBloc = 12
PgBar.SetProgressValue 0: Bloc = 0
PgBar.Show
DoEvents

' ProgressBarre
Bloc = Bloc + 1
'bAffichageProgressbarre = AffichageProgressbarre(Bloc, TotalBloc)


'********************************************************************************************************
'1 F) CHARGEMENT DU TABLEAU SANTE_1AN
'********************************************************************************************************

Case SANTE_1AN  ' modèle SANTE_1AN
        
        Set shResultats_RESULTATS = Worksheets(nomFeuille_RESULTATS)
        SourceSYNTHESE = shResultats_RESULTATS.Name  ' onglet RESULTATS
        
        Ligne_Depart_SYNTHESE = 15  ' ' ligne début tableau
        Libelle_TOTAL = "GLOBAL" ' valeur RECHERCHE pour identifier la ligne TOTAL
        COL_SYNTHESE_SURVENANCE = "C" ' année de Survenance
        COL_SYNTHESE_GARANTIES = "E"
        COL_SYNTHESE_PRESTATIONS = "G"
        COL_SYNTHESE_PROVISIONS = "I"
        COL_SYNTHESE_COTISATIONS_BRUTES = "K"
        COL_SYNTHESE_CHARGEMENTS = "O" ' ancienement "M"
        COL_SYNTHESE_COTISATIONS_NETTES = "Q" ' ancienement "O"
        COL_SYNTHESE_RATIO = "S" ' ancienement"Q"
        COL_SYNTHESE_GAINS_PERTES = "" ' à calculer
        
        
'CHARGEMENT des valeurs lues dans LE TABLEAU SYNTHESE
        'Ligne_SYNTHESE_ANNEE1 = 25
        Ligne_SYNTHESE_ANNEE2 = 20
        
'CHARGEMENT des valeurs lues dans LE TABLEAU SYNTHESE
i = 1
j = 0
        ' chargement de la ligne Ligne_SYNTHESE_ANNEE2
        Ligne_SYNTHESE = Ligne_SYNTHESE_ANNEE2
        If shResultats_RESULTATS.Range(COL_SYNTHESE_GARANTIES & Ligne_SYNTHESE) = Libelle_TOTAL And shResultats_RESULTATS.Range(COL_SYNTHESE_SURVENANCE & Ligne_SYNTHESE) <> "" Then
            Tableau_SYNTHESE(i, 1) = SourceSYNTHESE  '
            Tableau_SYNTHESE(i, 2) = shResultats_RESULTATS.Range(COL_SYNTHESE_SURVENANCE & Ligne_SYNTHESE) ' année
            Tableau_SYNTHESE(i, 3) = shResultats_RESULTATS.Range(COL_SYNTHESE_PRESTATIONS & Ligne_SYNTHESE) ' prestations
            Tableau_SYNTHESE(i, 4) = shResultats_RESULTATS.Range(COL_SYNTHESE_PROVISIONS & Ligne_SYNTHESE) ' provisions
            Tableau_SYNTHESE(i, 5) = shResultats_RESULTATS.Range(COL_SYNTHESE_COTISATIONS_BRUTES & Ligne_SYNTHESE) ' cotisations brutes
            Tableau_SYNTHESE(i, 6) = shResultats_RESULTATS.Range(COL_SYNTHESE_CHARGEMENTS & Ligne_SYNTHESE) ' chargements
            Tableau_SYNTHESE(i, 7) = shResultats_RESULTATS.Range(COL_SYNTHESE_COTISATIONS_NETTES & Ligne_SYNTHESE) ' cotisations nettes
            Tableau_SYNTHESE(i, 8) = shResultats_RESULTATS.Range(COL_SYNTHESE_RATIO & Ligne_SYNTHESE) ' ratio
            Tableau_SYNTHESE(i, 9) = Tableau_SYNTHESE(i, 7) - (Tableau_SYNTHESE(i, 3) + Tableau_SYNTHESE(i, 4)) ' gains/pertes recalculés
            Tableau_SYNTHESE(i, 10) = 0 ' écart forcé à 0
        
            NbAnneeLue = NbAnneeLue + 1
            i = i + 1
        Else
            'pas de chargement la ligne n'est pas correcte
            'LibelleFamille = " ANNEE " & shResultats_RESULTATS.Range(COL_SYNTHESE_SURVENANCE & Ligne_SYNTHESE) & " Le numéro de la ligne GLOBAL dans Tableau_Synthese n'est correct, il est différent de " & Ligne_SYNTHESE
            'GoTo err_CONTROLE:
        End If
 
'ProgressBarre
Bloc = Bloc + 1
'bAffichageProgressbarre = AffichageProgressbarre(Bloc, TotalBloc)

If NbAnneeATraiter <> 0 Then ' aucun chargement des données SYNTHESE à faire

    
Else
End If

'********************************************************************************************************
'1 G) CHARGEMENT DU TABLEAU SANTE_2ANS
'********************************************************************************************************

Case SANTE_2ANS  ' modèle SANTE_2ANS
        
        Set shResultats_RESULTATS = Worksheets(nomFeuille_RESULTATS)
        SourceSYNTHESE = shResultats_RESULTATS.Name  ' onglet RESULTATS
        
        Ligne_Depart_SYNTHESE = 15  ' ' ligne début tableau
        Libelle_TOTAL = "GLOBAL" ' valeur RECHERCHE pour identifier la ligne TOTAL
        COL_SYNTHESE_SURVENANCE = "C" ' année de Survenance
        COL_SYNTHESE_GARANTIES = "E"
        COL_SYNTHESE_PRESTATIONS = "G"
        COL_SYNTHESE_PROVISIONS = "I"
        COL_SYNTHESE_COTISATIONS_BRUTES = "K"
        COL_SYNTHESE_CHARGEMENTS = "O" ' ancienement "M"
        COL_SYNTHESE_COTISATIONS_NETTES = "Q" ' ancienement "O"
        COL_SYNTHESE_RATIO = "S" ' ancienement"Q"
        COL_SYNTHESE_GAINS_PERTES = "" ' à calculer
        
        ' CHARGEMENT DE CES 2 LIGNES UNIQUEMENT
        Ligne_SYNTHESE_ANNEE1 = 25
        Ligne_SYNTHESE_ANNEE2 = 20
        
'CHARGEMENT des valeurs lues dans LE TABLEAU SYNTHESE
i = 1
j = 0

        ' chargement de la ligne Ligne_SYNTHESE_ANNEE1
        Ligne_SYNTHESE = Ligne_SYNTHESE_ANNEE1
        If shResultats_RESULTATS.Range(COL_SYNTHESE_GARANTIES & Ligne_SYNTHESE) = Libelle_TOTAL And shResultats_RESULTATS.Range(COL_SYNTHESE_SURVENANCE & Ligne_SYNTHESE) <> "" Then
            Tableau_SYNTHESE(i, 1) = SourceSYNTHESE  '
            Tableau_SYNTHESE(i, 2) = shResultats_RESULTATS.Range(COL_SYNTHESE_SURVENANCE & Ligne_SYNTHESE) ' année
            Tableau_SYNTHESE(i, 3) = shResultats_RESULTATS.Range(COL_SYNTHESE_PRESTATIONS & Ligne_SYNTHESE) ' prestations
            Tableau_SYNTHESE(i, 4) = shResultats_RESULTATS.Range(COL_SYNTHESE_PROVISIONS & Ligne_SYNTHESE) ' provisions
            Tableau_SYNTHESE(i, 5) = shResultats_RESULTATS.Range(COL_SYNTHESE_COTISATIONS_BRUTES & Ligne_SYNTHESE) ' cotisations brutes
            Tableau_SYNTHESE(i, 6) = shResultats_RESULTATS.Range(COL_SYNTHESE_CHARGEMENTS & Ligne_SYNTHESE) ' chargements
            Tableau_SYNTHESE(i, 7) = shResultats_RESULTATS.Range(COL_SYNTHESE_COTISATIONS_NETTES & Ligne_SYNTHESE) ' cotisations nettes
            Tableau_SYNTHESE(i, 8) = shResultats_RESULTATS.Range(COL_SYNTHESE_RATIO & Ligne_SYNTHESE) ' ratio
            Tableau_SYNTHESE(i, 9) = Tableau_SYNTHESE(i, 7) - (Tableau_SYNTHESE(i, 3) + Tableau_SYNTHESE(i, 4)) ' gains/pertes recalculés
            Tableau_SYNTHESE(i, 10) = 0 ' écart forcé à 0
        
            NbAnneeLue = NbAnneeLue + 1
            i = i + 1
        Else
            'pas de chargement la ligne n'est pas correcte
            'LibelleFamille = " ANNEE " & shResultats_RESULTATS.Range(COL_SYNTHESE_SURVENANCE & Ligne_SYNTHESE) & " Le numéro de la ligne GLOBAL dans Tableau_Synthese n'est correct, il est différent de " & Ligne_SYNTHESE
            'GoTo err_CONTROLE:
        End If
        
        ' chargement de la ligne Ligne_SYNTHESE_ANNEE2
        Ligne_SYNTHESE = Ligne_SYNTHESE_ANNEE2
        If shResultats_RESULTATS.Range(COL_SYNTHESE_GARANTIES & Ligne_SYNTHESE) = Libelle_TOTAL And shResultats_RESULTATS.Range(COL_SYNTHESE_SURVENANCE & Ligne_SYNTHESE) <> "" Then
            Tableau_SYNTHESE(i, 1) = SourceSYNTHESE  '
            Tableau_SYNTHESE(i, 2) = shResultats_RESULTATS.Range(COL_SYNTHESE_SURVENANCE & Ligne_SYNTHESE) ' année
            Tableau_SYNTHESE(i, 3) = shResultats_RESULTATS.Range(COL_SYNTHESE_PRESTATIONS & Ligne_SYNTHESE) ' prestations
            Tableau_SYNTHESE(i, 4) = shResultats_RESULTATS.Range(COL_SYNTHESE_PROVISIONS & Ligne_SYNTHESE) ' provisions
            Tableau_SYNTHESE(i, 5) = shResultats_RESULTATS.Range(COL_SYNTHESE_COTISATIONS_BRUTES & Ligne_SYNTHESE) ' cotisations brutes
            Tableau_SYNTHESE(i, 6) = shResultats_RESULTATS.Range(COL_SYNTHESE_CHARGEMENTS & Ligne_SYNTHESE) ' chargements
            Tableau_SYNTHESE(i, 7) = shResultats_RESULTATS.Range(COL_SYNTHESE_COTISATIONS_NETTES & Ligne_SYNTHESE) ' cotisations nettes
            Tableau_SYNTHESE(i, 8) = shResultats_RESULTATS.Range(COL_SYNTHESE_RATIO & Ligne_SYNTHESE) ' ratio
            Tableau_SYNTHESE(i, 9) = Tableau_SYNTHESE(i, 7) - (Tableau_SYNTHESE(i, 3) + Tableau_SYNTHESE(i, 4)) ' gains/pertes recalculés
            Tableau_SYNTHESE(i, 10) = 0 ' écart forcé à 0
        
            NbAnneeLue = NbAnneeLue + 1
            i = i + 1
        Else
            'pas de chargement la ligne n'est pas correcte
            'LibelleFamille = " ANNEE " & shResultats_RESULTATS.Range(COL_SYNTHESE_SURVENANCE & Ligne_SYNTHESE) & " Le numéro de la ligne GLOBAL dans Tableau_Synthese n'est correct, il est différent de " & Ligne_SYNTHESE
            'GoTo err_CONTROLE:
        End If
 
'ProgressBarre
Bloc = 0
TotalBloc = 12
PgBar.SetProgressValue 0: Bloc = 0
PgBar.Show
DoEvents

' ProgressBarre
Bloc = Bloc + 1
'bAffichageProgressbarre = AffichageProgressbarre(Bloc, TotalBloc)

If NbAnneeATraiter <> 0 Then ' aucun chargement des données SYNTHESE à faire

    
Else
End If

'********************************************************************************************************
'1 c) NON TRAITE
'********************************************************************************************************
Case Else
    NbAnneeATraiter = 0
End Select


'********************************************************************************************************
'2 a) CHARGEMENT DU TABLEAU CONTROLE POUR LES COMPTES PAR SURVENANCE
'    chargement des toutes lignes TOTALES lues dans la feuille CONTROLE annee2 et annee1
'********************************************************************************************************

'*************************************
If bTypeCompte = False Then 'Survenance
'*************************************

SourceCONTROLE = shResultats_CONTROLE.Name
Ligne_Titre_CONTROLE = 1 ' ligne titre du tableau CONTROLE
Ligne_Rubriques_CONTROLE = 2 ' ligne des rubriques du tableau CONTROLE
Ligne_Depart_CONTROLE = 3 ' début RECHERCHE année dans tableau CONTROLE
COL_CONTROLE_SURVENANCE = "A" ' colonne de la SURVENANCE dans le CONTROLE
'COL_CONTROLE_GARANTIES = "D" 4 non utilisée
COL_CONTROLE_PRESTATIONS = "B"
COL_CONTROLE_PROVISIONS = "C"
COL_CONTROLE_COTISATIONS_BRUTES = "D"
COL_CONTROLE_CHARGEMENTS = "E"
COL_CONTROLE_COTISATIONS_NETTES = "F"
COL_CONTROLE_RATIO = "G"
COL_CONTROLE_GAINS_PERTES = "H"

' taux frais de Sante
COL_TITRE_FRAIS_SANTE = "J"
COL_CONTROLE_TAUX_GESTION = "J"
COL_CONTROLE_TAUX_TAXE_COVID = "K"
COL_CONTROLE_TAUX_TAXES = "L"

LIB_CONTROLE_TAUX_GESTION = "FRAIS"
LIB_CONTROLE_TAUX_TAXE_COVID = "TAXE_COVID"
LIB_CONTROLE_TAUX_TAXES = "TCA_CMU"


Presence_CONTROLE_ANNEE = False

'CHARGEMENT des valeurs lues dans LE TABLEAU CONTROLE
i = 1
j = 0
Ligne_CONTROLE = Ligne_Depart_CONTROLE
Ligne_Fin_CONTROLE = Ligne_Depart_CONTROLE

'********** DEBUT chargement dans Tableau_CONTROLE **********
While shResultats_CONTROLE.Range(COL_CONTROLE_SURVENANCE & Ligne_CONTROLE) <> "" And Ligne_CONTROLE < 100
        
        ' chargement des données de la ligne dans le Tableau_CONTROLE em mémoire
        Presence_CONTROLE_ANNEE = True
        wAnnee = shResultats_CONTROLE.Range(COL_CONTROLE_SURVENANCE & Ligne_CONTROLE) ' année
        wPrestations = shResultats_CONTROLE.Range(COL_CONTROLE_PRESTATIONS & Ligne_CONTROLE) ' prestations
        wProvisions = shResultats_CONTROLE.Range(COL_CONTROLE_PROVISIONS & Ligne_CONTROLE) ' provisions
        wCotBrutes = shResultats_CONTROLE.Range(COL_CONTROLE_COTISATIONS_BRUTES & Ligne_CONTROLE) ' cotisations brutes
        wChargements = shResultats_CONTROLE.Range(COL_CONTROLE_CHARGEMENTS & Ligne_CONTROLE) ' chargements
        wCotNettes = shResultats_CONTROLE.Range(COL_CONTROLE_COTISATIONS_NETTES & Ligne_CONTROLE) ' cotisations nettes
        
        wRatio = 0
        If wCotNettes <> 0 Then
        wRatio = shResultats_CONTROLE.Range(COL_CONTROLE_RATIO & Ligne_CONTROLE) ' ratio
        Else
        End If
        wGainsPertes = shResultats_CONTROLE.Range(COL_CONTROLE_GAINS_PERTES & Ligne_CONTROLE) ' gains/pertes
        
        Tableau_CONTROLE(i, 1) = SourceCONTROLE  '
        Tableau_CONTROLE(i, 2) = Round(wAnnee, 0) ' année
        Tableau_CONTROLE(i, 3) = Round(wPrestations, 2) ' prestations
        Tableau_CONTROLE(i, 4) = Round(wProvisions, 2) ' provisions
        Tableau_CONTROLE(i, 5) = Round(wCotBrutes, 2) ' cotisations brutes
        Tableau_CONTROLE(i, 6) = Round(wChargements, 4) ' chargements
        Tableau_CONTROLE(i, 7) = Round(wCotNettes, 2) ' cotisations nettes
        Tableau_CONTROLE(i, 8) = Round(wRatio, 4) ' ratio
        Tableau_CONTROLE(i, 9) = Round(wGainsPertes, 2) ' gains/pertes
        Tableau_CONTROLE(i, 10) = 0 ' écart forcé à 0
        
        i = i + 1
        Ligne_Fin_CONTROLE = Ligne_Fin_CONTROLE + 1
        
        ' controle de iMax
        If i > iMax Then
            LibelleFamille = "ATTENTION AUGMENTER le nombre de lignes i pour le Tableau_CONTROLE, la limite iMax est atteinte =  " & iMax
        GoTo err_CONTROLE:
        Else
        End If

' ligne suivante
Ligne_CONTROLE = Ligne_CONTROLE + 1

Wend

End If
'********** FIN chargement dans Tableau_CONTROLE PAR SURVENANCE **********


'********************************************************************************************************
'2 b) CHARGEMENT DU TABLEAU CONTROLE POUR LES COMPTES PAR ANNEE COMPTABLE
'    chargement des toutes lignes TOTALES lues dans la feuille CONTROLE annee2 et annee1
'********************************************************************************************************

'*************************************
If bTypeCompte = True Then 'Comptable
'*************************************

SourceCONTROLE = shResultats_CONTROLE.Name
Ligne_Titre_CONTROLE = 1 ' ligne titre du tableau CONTROLE
Ligne_Rubriques_CONTROLE = 2 ' ligne des rubriques du tableau CONTROLE
Ligne_Depart_CONTROLE = 3 ' début RECHERCHE année dans tableau CONTROLE
COL_CONTROLE_SURVENANCE = "A" ' colonne de la SURVENANCE dans le CONTROLE
'COL_CONTROLE_GARANTIES = "D" 4 non utilisée
COL_CONTROLE_PRESTATIONS = "B"
COL_CONTROLE_PROVISIONS = "C"
COL_CONTROLE_COTISATIONS_BRUTES = "D"
COL_CONTROLE_CHARGEMENTS = "E"
COL_CONTROLE_COTISATIONS_NETTES = "F"
COL_CONTROLE_RATIO = "G"
COL_CONTROLE_GAINS_PERTES = "H"
Presence_CONTROLE_ANNEE = False

'CHARGEMENT des valeurs lues dans LE TABLEAU CONTROLE
i = 1
j = 0
Ligne_CONTROLE = Ligne_Depart_CONTROLE
Ligne_Fin_CONTROLE = Ligne_Depart_CONTROLE

wPrestations = 0
wProvisions = 0
wCotBrutes = 0
wCotNettes = 0

'********** DEBUT chargement dans Tableau_CONTROLE **********
While shResultats_CONTROLE.Range(COL_CONTROLE_SURVENANCE & Ligne_CONTROLE) <> "" And Ligne_CONTROLE < 100
        
        ' CALCUL DES CUMULS DONNEES des lignes dans le Tableau_CONTROLE em mémoire
        Presence_CONTROLE_ANNEE = True
        wAnnee = shResultats_CONTROLE.Range(COL_CONTROLE_SURVENANCE & Ligne_CONTROLE) ' année
        
        LibelleFamille = "Erreur cumul Prestations annnee " & wAnnee
        wPrestations = Round(shResultats_CONTROLE.Range(COL_CONTROLE_PRESTATIONS & Ligne_CONTROLE) * 1, 2) + wPrestations ' prestations
        
        LibelleFamille = "Erreur cumul Provisions annnee " & wAnnee
        wProvisions = Round(shResultats_CONTROLE.Range(COL_CONTROLE_PROVISIONS & Ligne_CONTROLE) * 1, 2) + wProvisions ' provisions
        
        LibelleFamille = "Erreur cumul CotBrutes annnee " & wAnnee
        wCotBrutes = Round(shResultats_CONTROLE.Range(COL_CONTROLE_COTISATIONS_BRUTES & Ligne_CONTROLE) * 1, 2) + wCotBrutes ' cotisations brutes
        
        LibelleFamille = "Erreur cumul CotNettes annnee " & wAnnee
        wCotNettes = Round(shResultats_CONTROLE.Range(COL_CONTROLE_COTISATIONS_NETTES & Ligne_CONTROLE) * 1, 2) + wCotNettes ' cotisations nettes
        
        LibelleFamille = ""
        i = i + 1
        Ligne_Fin_CONTROLE = Ligne_Fin_CONTROLE + 1
        
        ' controle de iMax
        If i > iMax Then
            LibelleFamille = "ATTENTION AUGMENTER le nombre de lignes i pour le Tableau_CONTROLE, la limite iMax est atteinte =  " & iMax
        GoTo err_CONTROLE:
        Else
        End If

        ' ligne suivante
        Ligne_CONTROLE = Ligne_CONTROLE + 1
Wend
       
        ' CHARGEMENT DES CUMULS DANS LA LIGNE ANNEE COMPTABLE pour le Tableau_CONTROLE
        
        'année comptable = année début affichage
        wAnnee = AnneeDebutAffichage ' année début affichage = année comptable

        'calcul Chargements
        wChargements = 0
        If wCotBrutes <> 0 Then 'chargements
        wChargements = Round(1 - wCotNettes / wCotBrutes, 4) '= (1 - CotNette / CotBrute) ' chargements
        End If
        
        ' calcul Ratio
        wRatio = 0
        If wCotNettes <> 0 Then ' ratio
        wRatio = Round((wPrestations + wProvisions) / wCotNettes, 4) ' ratio
        End If
        
        ' calcul gains/pertes
        wGainsPertes = wCotNettes - wPrestations - wProvisions
        
        i = 1
        Tableau_CONTROLE(i, 1) = SourceCONTROLE  '
        Tableau_CONTROLE(i, 2) = Round(wAnnee, 0) ' année
        Tableau_CONTROLE(i, 3) = Round(wPrestations, 2) ' prestations
        Tableau_CONTROLE(i, 4) = Round(wProvisions, 2) ' provisions
        Tableau_CONTROLE(i, 5) = Round(wCotBrutes, 2) ' cotisations brutes
        Tableau_CONTROLE(i, 6) = Round(wChargements, 4) ' chargements
        Tableau_CONTROLE(i, 7) = Round(wCotNettes, 2) ' cotisations nettes
        Tableau_CONTROLE(i, 8) = Round(wRatio, 4) ' ratio
        Tableau_CONTROLE(i, 9) = Round(wGainsPertes, 2) ' gains/pertes
        Tableau_CONTROLE(i, 10) = 0 ' écart forcé à 0

End If
'********** FIN chargement dans Tableau_CONTROLE PAR ANNEE COMPTABLE **********


'********************************************************************************************************
'3) FUSION DES 2 TABLEAUX mémoire SYNTHESE + CONTROLE
'********************************************************************************************************

'a) AJOUT Tableau_SYNTHESE dans le Tableau_FUSION
DerniereLigneFusion = 0
i = 1
While Tableau_SYNTHESE(i, 2) <> 0  ' année de survenance ou comptable <> 0
    'chargement de la ligne SYNTHESE dans FUSION
    For j = 1 To jMax
         Tableau_FUSION(i, j) = Tableau_SYNTHESE(i, j)
    Next j
    DerniereLigneFusion = i
    i = i + 1
    ' controle de iMaxFusion
    If i > iMaxFusion Then
        LibelleFamille = "ATTENTION AUGMENTER le nombre de lignes i pour le Tableau_FUSION, la limite iMaxFusion est atteinte =  " & iMaxFusion
    GoTo err_CONTROLE:
    Else
    End If

Wend

'b) AJOUT Tableau_CONTROLE dans le Tableau_FUSION
i = 1
While Tableau_CONTROLE(i, 2) <> 0  ' année de survenance ou comptable <> 0
    'chargement de la ligne SYNTHESE dans FUSION
    For j = 1 To jMax
         Tableau_FUSION(DerniereLigneFusion + i, j) = Tableau_CONTROLE(i, j)
    Next j
i = i + 1

' controle de iMaxFusion
If i > iMaxFusion Then
    LibelleFamille = "ATTENTION AUGMENTER le nombre de lignes i pour le Tableau_FUSION, la limite iMaxFusion est atteinte =  " & iMaxFusion
GoTo err_CONTROLE:
Else
End If

Wend


'********************************************************************************************************
'4) CHARGEMENT DES ANNEES lues dans FUSION SANS DOUBLON dans le Tableau_ANNEE
'********************************************************************************************************

i = 1
While Tableau_FUSION(i, 2) <> 0  ' année de survenance ou comptable <> 0
    
    'Recherche de la PRESENCE de l'année dans Tableau_FUSION
    PresenceAnnee = False
    j = 1
    While (PresenceAnnee = False) And (j <= iMaxFusion) And (Tableau_ANNEE(j) <> 0)
        If Tableau_ANNEE(j) = Tableau_FUSION(i, 2) Then ' année
            PresenceAnnee = True
        Else
            j = j + 1
        End If
    Wend
      
    'AJOUT de l'année dans Tableau_ANNEE si elle est ABSENTE
    AjoutAnnee = False
    j = 1
    While (PresenceAnnee = False) And (AjoutAnnee = False) And (j <= iMaxFusion)
        If Tableau_ANNEE(j) = 0 Then ' année
            Tableau_ANNEE(j) = Tableau_FUSION(i, 2)
            AjoutAnnee = True
        Else
        j = j + 1
        End If
    Wend
      
i = i + 1

' controle de iMaxFusion
If i > iMaxFusion Then
    LibelleFamille = "ATTENTION AUGMENTER le nombre de lignes i pour le Tableau_FUSION, la limite iMaxFusion est atteinte =  " & iMaxFusion
    GoTo err_CONTROLE:
Else
End If

Wend

'********************************************************************************************************
'5) CONSTRUCTION DU TABLEAU_ECART=(SYNTHESE-CONTROLE) pour les années SANS DOUBLONS de Tableau_ANNEE
'********************************************************************************************************

PresenceECART = False ' pas d'ECART DANS LE TABLEAU
k = 1
While Tableau_ANNEE(k) <> 0  ' année de survenance ou comptable <> 0
    
    'Recherche de la PRESENCE de l'année de type SYNTHESE dans Tableau_FUSION
    PresenceAnnee = False
    i = 1
    iSYNTHESE = 0
    While (PresenceAnnee = False) And (i <= iMaxFusion) And (Tableau_ANNEE(k) <> 0)
        If Tableau_ANNEE(k) = Tableau_FUSION(i, 2) Then ' année
            
            If Tableau_FUSION(i, 1) = SourceSYNTHESE Then
                PresenceAnnee = True
                iSYNTHESE = i
            Else
            End If
        Else
        End If
    i = i + 1
    Wend
      
    'Recherche de la PRESENCE de l'année de type CONTROLE dans Tableau_FUSION
    PresenceAnnee = False
    i = 1
    iCONTROLE = 0
    While (PresenceAnnee = False) And (i <= iMaxFusion) And (Tableau_ANNEE(k) <> 0)
        If Tableau_ANNEE(k) = Tableau_FUSION(i, 2) Then ' année
            If Tableau_FUSION(i, 1) = SourceCONTROLE Then
                PresenceAnnee = True
                iCONTROLE = i
            Else
            End If
        Else
        End If
    i = i + 1
    Wend
      
    'AJOUT de l'ECART (SYNTHESE-CONTROLE) pour l'année
        
        ' chargement des données lignes (SYNTHESE - CONTROLE) dans le Tableau_CONTROLE
        ' remarque (si iSYNTHESE=0 pas de ligne SYNTHESE)
        ' remarque (si iCONTROLE=0 pas de ligne CONTROLE)
        SourceECART = "ECART"
        Tableau_ECART(k, 1) = SourceECART  '
        Tableau_ECART(k, 2) = Tableau_ANNEE(k) ' année
        Tableau_ECART(k, 3) = Tableau_FUSION(iSYNTHESE, 3) - Tableau_FUSION(iCONTROLE, 3) ' prestations
        Tableau_ECART(k, 4) = Tableau_FUSION(iSYNTHESE, 4) - Tableau_FUSION(iCONTROLE, 4)  ' provisions
        Tableau_ECART(k, 5) = Tableau_FUSION(iSYNTHESE, 5) - Tableau_FUSION(iCONTROLE, 5) ' cotisations brutes
        Tableau_ECART(k, 6) = Tableau_FUSION(iSYNTHESE, 6) - Tableau_FUSION(iCONTROLE, 6) ' chargements
        Tableau_ECART(k, 7) = Tableau_FUSION(iSYNTHESE, 7) - Tableau_FUSION(iCONTROLE, 7) ' cotisations nettes
        Tableau_ECART(k, 8) = Tableau_FUSION(iSYNTHESE, 8) - Tableau_FUSION(iCONTROLE, 8) ' ratio
        Tableau_ECART(k, 9) = Tableau_FUSION(iSYNTHESE, 9) - Tableau_FUSION(iCONTROLE, 9) ' gains/pertes
        Tableau_ECART(k, 10) = 0 ' initialisation = pas d'écart
        
        ' RECHERCHE des ECARTS pour la ligne
        For j = 3 To 9  ' presta à gains/pertes
            If Tableau_ECART(k, j) <> 0 Then
                Tableau_ECART(k, 10) = 1 ' il y a un ecart dans la ligne
                PresenceECART = True ' il y a un écart dans le tableau
            Else
            End If
        Next j
      
k = k + 1
' controle de iMaxFusion
If k > iMaxFusion Then
    LibelleFamille = "ATTENTION AUGMENTER le nombre de lignes i pour le Tableau_FUSION, la limite iMaxFusion est atteinte =  " & iMaxFusion
GoTo err_CONTROLE:
Else
End If

Wend

'***********************************************************************************************************
'6) onglet CONTROLE - SUPPRESSION DE TOUTES LES LIGNES sauf celles du tableau CONTROLE
'***********************************************************************************************************
shResultats_CONTROLE.Select

' recherche Ligne_AFFICHAGE_MESSAGE_ECART
Ligne_AFFICHAGE_MESSAGE_ECART = Ligne_Fin_CONTROLE
Presence_Ligne_AFFICHAGE_MESSAGE_ECART = False

While Presence_Ligne_AFFICHAGE_MESSAGE_ECART = False

If (shResultats_CONTROLE.Range(COL_CONTROLE_SURVENANCE & Ligne_AFFICHAGE_MESSAGE_ECART) = LibelleEcart) Or (shResultats_CONTROLE.Range(COL_CONTROLE_SURVENANCE & Ligne_AFFICHAGE_MESSAGE_ECART) = LibelleAucunEcart) Then
    Presence_Ligne_AFFICHAGE_MESSAGE_ECART = True
    Else
    End If

' ligne suivante
Ligne_AFFICHAGE_MESSAGE_ECART = Ligne_AFFICHAGE_MESSAGE_ECART + 1

' controle recherche
If Ligne_AFFICHAGE_MESSAGE_ECART > 200 Then ' recherche sur 200 lignes
    Presence_Ligne_AFFICHAGE_MESSAGE_ECART = True ' arret recherche
    'LibelleFamille = "ATTENTION RECHERCHE Ligne_AFFICHAGE_MESSAGE_ECARTS INTERROMPUE dans l'ONGLET CONTROLE ..il y a " & Ligne_AFFICHAGE_MESSAGE_ECART & " lignes analysées sans " & LibelleEcart & " ou" & LibelleAucunEcart
    'GoTo err_CONTROLE:
Else
End If

Wend

Rows(Ligne_Fin_CONTROLE & ":" & Ligne_AFFICHAGE_MESSAGE_ECART).Select
Range(Selection, Selection.End(xlDown)).Select
Selection.Delete Shift:=xlUp

'********************************************************************************************************
'7) onglet CONTROLE - AFFICHAGE tableau SYNTHESE
'********************************************************************************************************

'titre tableau SYNTHESE
Ligne_titre_AFFICHAGE_SYNTHESE = Ligne_Fin_CONTROLE + 2
Ligne_AFFICHAGE_SYNTHESE = Ligne_titre_AFFICHAGE_SYNTHESE
Ligne_AFFICHAGE_SYNTHESE = Ligne_AFFICHAGE_SYNTHESE + 1

'Recopie du (titre + rubriques) de CONTROLE dans tableau SYNTHESE
Rows(Ligne_Titre_CONTROLE & ":" & Ligne_Rubriques_CONTROLE).Select
Selection.Copy
Rows(Ligne_AFFICHAGE_SYNTHESE & ":" & Ligne_AFFICHAGE_SYNTHESE).Select
ActiveSheet.Paste
' titre
'shResultats_CONTROLE.Range(COL_CONTROLE_SURVENANCE & Ligne_AFFICHAGE_SYNTHESE) = "Source -> Onglet " & shResultats_SYNTHESE.Name ' nom du tableau SYNTHESE
shResultats_CONTROLE.Range(COL_CONTROLE_SURVENANCE & Ligne_AFFICHAGE_SYNTHESE) = "Source -> Onglet " & SourceSYNTHESE ' nom du tableau SYNTHESE


'*********  TITRE TABLEAU FRAIS SANTE ******

If TYPE_CONTROLE = SANTE_1AN Or TYPE_CONTROLE = SANTE_2ANS Then

shResultats_CONTROLE.Range(COL_TITRE_FRAIS_SANTE & Ligne_AFFICHAGE_SYNTHESE) = "Taux des Frais de SANTE"  ' nom du tableau FRAIS DE SANTE
'Fusion des 3 cellules et changement de couleur
shResultats_CONTROLE.Range(COL_TITRE_FRAIS_SANTE & Ligne_AFFICHAGE_SYNTHESE & ":" & COL_CONTROLE_TAUX_TAXES & Ligne_AFFICHAGE_SYNTHESE).Select
'Range("J6:L6").Select
    With Selection
        .HorizontalAlignment = xlCenter
        .VerticalAlignment = xlBottom
        .WrapText = False
        .Orientation = 0
        .AddIndent = False
        .IndentLevel = 0
        .ShrinkToFit = False
        .ReadingOrder = xlContext
        .MergeCells = False
    End With
    Selection.Merge
    With Selection.Interior
        .Pattern = xlSolid
        .PatternColorIndex = xlAutomatic
        .Color = 12495951
        .TintAndShade = 0
        .PatternTintAndShade = 0
    End With
    With Selection.Font
        .ThemeColor = xlThemeColorDark1
        .TintAndShade = -4.99893185216834E-02
    End With

' libellé des frais
shResultats_CONTROLE.Range(COL_CONTROLE_TAUX_GESTION & Ligne_AFFICHAGE_SYNTHESE + 1) = LIB_CONTROLE_TAUX_GESTION ' LIB_CONTROLE_TAUX_GESTION
shResultats_CONTROLE.Range(COL_CONTROLE_TAUX_TAXE_COVID & Ligne_AFFICHAGE_SYNTHESE + 1) = LIB_CONTROLE_TAUX_TAXE_COVID ' LIB_CONTROLE_TAUX_TAXE_COVID
shResultats_CONTROLE.Range(COL_CONTROLE_TAUX_TAXES & Ligne_AFFICHAGE_SYNTHESE + 1) = LIB_CONTROLE_TAUX_TAXES ' LIB_CONTROLE_TAUX_TAXES

End If

'*********  TITRE TABLEAU FRAIS PREVOYANCE NON TRAITE ******

If TYPE_CONTROLE = PREV_1AN Then
End If


'********** DEBUT AFFICHAGE des lignes tableau SYNTHESE dans CONTROLE **********
Ligne_AFFICHAGE_SYNTHESE = Ligne_AFFICHAGE_SYNTHESE + 2 ' on a ajouté 2 lignes titre + rubriques
i = 1

While (Tableau_SYNTHESE(i, 2)) <> 0 And (i <= iMax)
        'Tableau_SYNTHESE(i, 1) = SourceCONTROLE  '
        
        'recopie du format des données du CONTROLE
        Zone1 = COL_CONTROLE_SURVENANCE & Ligne_Depart_CONTROLE & ":" & COL_CONTROLE_GAINS_PERTES & Ligne_Depart_CONTROLE
        shResultats_CONTROLE.Range(Zone1).Select
        Application.CutCopyMode = False
        Selection.Copy
        Zone2 = COL_CONTROLE_SURVENANCE & Ligne_AFFICHAGE_SYNTHESE & ":" & COL_CONTROLE_GAINS_PERTES & Ligne_AFFICHAGE_SYNTHESE
        shResultats_CONTROLE.Range(Zone2).Select
        Selection.PasteSpecial Paste:=xlPasteFormats, Operation:=xlNone, _
        SkipBlanks:=False, Transpose:=False
        
        'données synthese
        shResultats_CONTROLE.Range(COL_CONTROLE_SURVENANCE & Ligne_AFFICHAGE_SYNTHESE) = Tableau_SYNTHESE(i, 2) ' année
        shResultats_CONTROLE.Range(COL_CONTROLE_SURVENANCE & Ligne_AFFICHAGE_SYNTHESE).Select
        Selection.NumberFormat = "0_ ;[Red]-0 "
        
        shResultats_CONTROLE.Range(COL_CONTROLE_PRESTATIONS & Ligne_AFFICHAGE_SYNTHESE) = Tableau_SYNTHESE(i, 3) ' prestations
        shResultats_CONTROLE.Range(COL_CONTROLE_PRESTATIONS & Ligne_AFFICHAGE_SYNTHESE).Select
        Selection.NumberFormat = "$#,##0.00_);[Red]($#,##0.00)"
        
        shResultats_CONTROLE.Range(COL_CONTROLE_PROVISIONS & Ligne_AFFICHAGE_SYNTHESE) = Tableau_SYNTHESE(i, 4) ' provisions
        shResultats_CONTROLE.Range(COL_CONTROLE_PROVISIONS & Ligne_AFFICHAGE_SYNTHESE).Select
        Selection.NumberFormat = "$#,##0.00_);[Red]($#,##0.00)"
        
        shResultats_CONTROLE.Range(COL_CONTROLE_COTISATIONS_BRUTES & Ligne_AFFICHAGE_SYNTHESE) = Tableau_SYNTHESE(i, 5) ' cotisations brutes
        shResultats_CONTROLE.Range(COL_CONTROLE_COTISATIONS_BRUTES & Ligne_AFFICHAGE_SYNTHESE).Select
        Selection.NumberFormat = "$#,##0.00_);[Red]($#,##0.00)"
        
        shResultats_CONTROLE.Range(COL_CONTROLE_CHARGEMENTS & Ligne_AFFICHAGE_SYNTHESE) = Tableau_SYNTHESE(i, 6) ' chargements
        shResultats_CONTROLE.Range(COL_CONTROLE_CHARGEMENTS & Ligne_AFFICHAGE_SYNTHESE).Select
        Selection.NumberFormat = "0.00%"
        
        shResultats_CONTROLE.Range(COL_CONTROLE_COTISATIONS_NETTES & Ligne_AFFICHAGE_SYNTHESE) = Tableau_SYNTHESE(i, 7) ' cotisations nettes
        shResultats_CONTROLE.Range(COL_CONTROLE_COTISATIONS_NETTES & Ligne_AFFICHAGE_SYNTHESE).Select
        Selection.NumberFormat = "$#,##0.00_);[Red]($#,##0.00)"
        
        shResultats_CONTROLE.Range(COL_CONTROLE_RATIO & Ligne_AFFICHAGE_SYNTHESE) = Tableau_SYNTHESE(i, 8) ' ratio
        shResultats_CONTROLE.Range(COL_CONTROLE_RATIO & Ligne_AFFICHAGE_SYNTHESE).Select
        Selection.NumberFormat = "0.00%"
        
        shResultats_CONTROLE.Range(COL_CONTROLE_GAINS_PERTES & Ligne_AFFICHAGE_SYNTHESE) = Tableau_SYNTHESE(i, 9) ' gains/pertes
        shResultats_CONTROLE.Range(COL_CONTROLE_GAINS_PERTES & Ligne_AFFICHAGE_SYNTHESE).Select
        Selection.NumberFormat = "$#,##0.00_);[Red]($#,##0.00)"
        
        'Tableau_CONTROLE(i, 10) = 0 ' écart forcé à 0
        
' ajout taux de FRAIS SANTE
If TYPE_CONTROLE = SANTE_1AN Or TYPE_CONTROLE = SANTE_2ANS Then
       ' frais de gestion "FRAIS"
        ANNEE = Tableau_SYNTHESE(i, 2)
        FraisGestion = Frais(shResultats_AFFICHAGE_2, ANNEE, LIB_CONTROLE_TAUX_GESTION)
        shResultats_CONTROLE.Range(COL_CONTROLE_TAUX_GESTION & Ligne_AFFICHAGE_SYNTHESE) = FraisGestion ' taux frais de gestion
        shResultats_CONTROLE.Range(COL_CONTROLE_TAUX_GESTION & Ligne_AFFICHAGE_SYNTHESE).Select
        Selection.NumberFormat = "0.00%"

         ' taxe covid "TAXE_COVID"
        ANNEE = Tableau_SYNTHESE(i, 2)
        FraisGestion = Frais(shResultats_AFFICHAGE_2, ANNEE, LIB_CONTROLE_TAUX_TAXE_COVID)
        shResultats_CONTROLE.Range(COL_CONTROLE_TAUX_TAXE_COVID & Ligne_AFFICHAGE_SYNTHESE) = FraisGestion ' taux taxe covid
        shResultats_CONTROLE.Range(COL_CONTROLE_TAUX_TAXE_COVID & Ligne_AFFICHAGE_SYNTHESE).Select
        Selection.NumberFormat = "0.00%"
        
        ' taxe "TCA_CMU"
        ANNEE = Tableau_SYNTHESE(i, 2)
        FraisGestion = Frais(shResultats_AFFICHAGE_2, ANNEE, LIB_CONTROLE_TAUX_TAXES)
        shResultats_CONTROLE.Range(COL_CONTROLE_TAUX_TAXES & Ligne_AFFICHAGE_SYNTHESE) = FraisGestion ' taux taxe TCA_CMU
        shResultats_CONTROLE.Range(COL_CONTROLE_TAUX_TAXES & Ligne_AFFICHAGE_SYNTHESE).Select
        Selection.NumberFormat = "0.00%"

End If

' ajout taux de FRAIS PREV
If TYPE_CONTROLE = PREV_1AN Then
End If



i = i + 1
' ligne suivante
Ligne_AFFICHAGE_SYNTHESE = Ligne_AFFICHAGE_SYNTHESE + 1

Wend


'********************************************************************************************************
'8) onglet CONTROLE - AFFICHAGE tableau ECART
'********************************************************************************************************

'titre tableau ECART
Ligne_titre_AFFICHAGE_ECART = Ligne_AFFICHAGE_SYNTHESE + 2
Ligne_AFFICHAGE_ECART = Ligne_titre_AFFICHAGE_ECART
Ligne_AFFICHAGE_ECART = Ligne_AFFICHAGE_ECART + 1

'Recopie du (titre + rubriques) de CONTROLE dans tableau SYNTHESE
Rows(Ligne_Titre_CONTROLE & ":" & Ligne_Rubriques_CONTROLE).Select
Selection.Copy
Rows(Ligne_AFFICHAGE_ECART & ":" & Ligne_AFFICHAGE_ECART).Select
ActiveSheet.Paste
' titre
shResultats_CONTROLE.Range(COL_CONTROLE_SURVENANCE & Ligne_AFFICHAGE_ECART) = SourceECART & " ( = " & SourceSYNTHESE & " - " & SourceCONTROLE & ")" ' nom du tableau ECART


'********** DEBUT AFFICHAGE des lignes tableau SYNTHESE dans CONTROLE **********
Ligne_AFFICHAGE_ECART = Ligne_AFFICHAGE_ECART + 2 ' on a ajouté 2 lignes titre + rubriques
i = 1

While (Tableau_ECART(i, 2)) <> 0 And (i <= iMax)
        'Tableau_ECART(i, 1) = SourceCONTROLE  '
        
        'recopie du format des données du CONTROLE
        Zone1 = COL_CONTROLE_SURVENANCE & Ligne_Depart_CONTROLE & ":" & COL_CONTROLE_GAINS_PERTES & Ligne_Depart_CONTROLE
        shResultats_CONTROLE.Range(Zone1).Select
        Application.CutCopyMode = False
        Selection.Copy
        Zone2 = COL_CONTROLE_SURVENANCE & Ligne_AFFICHAGE_ECART & ":" & COL_CONTROLE_GAINS_PERTES & Ligne_AFFICHAGE_ECART
        shResultats_CONTROLE.Range(Zone2).Select
        Selection.PasteSpecial Paste:=xlPasteFormats, Operation:=xlNone, _
        SkipBlanks:=False, Transpose:=False

        If Tableau_ECART(i, 10) = 1 Then ' il y a des écarts dans la ligne
            Range(Zone2).Select
            With Selection.Font
            .Color = -16776961
            .TintAndShade = 0
            End With
        Else
        End If



        ' données écarts

If Tableau_ECART(i, 10) = 1 Then ' 0=pas d'écart 1=présence écart
        
        If Tableau_ECART(i, 2) <> 0 Then
        shResultats_CONTROLE.Range(COL_CONTROLE_SURVENANCE & Ligne_AFFICHAGE_ECART) = Tableau_ECART(i, 2) ' année
        shResultats_CONTROLE.Range(COL_CONTROLE_SURVENANCE & Ligne_AFFICHAGE_ECART).Select
        With Selection.Font
        .Color = -16776961 ' rouge
        .TintAndShade = 0
        End With
        End If
        
        If Tableau_ECART(i, 3) <> 0 Then
        shResultats_CONTROLE.Range(COL_CONTROLE_PRESTATIONS & Ligne_AFFICHAGE_ECART) = Tableau_ECART(i, 3) ' prestations
        shResultats_CONTROLE.Range(COL_CONTROLE_PRESTATIONS & Ligne_AFFICHAGE_ECART).Select
        Selection.NumberFormat = "$#,##0.00_);[Red]($#,##0.00)"
        End If
        
        If Tableau_ECART(i, 4) <> 0 Then
        shResultats_CONTROLE.Range(COL_CONTROLE_PROVISIONS & Ligne_AFFICHAGE_ECART) = Tableau_ECART(i, 4) ' provisions
        shResultats_CONTROLE.Range(COL_CONTROLE_PROVISIONS & Ligne_AFFICHAGE_ECART).Select
        Selection.NumberFormat = "$#,##0.00_);[Red]($#,##0.00)"
        End If
        
        If Tableau_ECART(i, 5) <> 0 Then
        shResultats_CONTROLE.Range(COL_CONTROLE_COTISATIONS_BRUTES & Ligne_AFFICHAGE_ECART) = Tableau_ECART(i, 5) ' cotisations brutes
        shResultats_CONTROLE.Range(COL_CONTROLE_COTISATIONS_BRUTES & Ligne_AFFICHAGE_ECART).Select
        Selection.NumberFormat = "$#,##0.00_);[Red]($#,##0.00)"
        End If
        
        If Tableau_ECART(i, 6) <> 0 Then
        shResultats_CONTROLE.Range(COL_CONTROLE_CHARGEMENTS & Ligne_AFFICHAGE_ECART) = Tableau_ECART(i, 6) ' chargements
        shResultats_CONTROLE.Range(COL_CONTROLE_CHARGEMENTS & Ligne_AFFICHAGE_ECART).Select
        Selection.NumberFormat = "0.00%"
        End If
        
        If Tableau_ECART(i, 7) <> 0 Then
        shResultats_CONTROLE.Range(COL_CONTROLE_COTISATIONS_NETTES & Ligne_AFFICHAGE_ECART) = Tableau_ECART(i, 7) ' cotisations nettes
        shResultats_CONTROLE.Range(COL_CONTROLE_COTISATIONS_NETTES & Ligne_AFFICHAGE_ECART).Select
        Selection.NumberFormat = "$#,##0.00_);[Red]($#,##0.00)"
       End If
        
        If Tableau_ECART(i, 8) <> 0 Then
        shResultats_CONTROLE.Range(COL_CONTROLE_RATIO & Ligne_AFFICHAGE_ECART) = Tableau_ECART(i, 8) ' ratio
        shResultats_CONTROLE.Range(COL_CONTROLE_RATIO & Ligne_AFFICHAGE_ECART).Select
        Selection.NumberFormat = "0.00%"
        End If
        
        If Tableau_ECART(i, 9) <> 0 Then
        shResultats_CONTROLE.Range(COL_CONTROLE_GAINS_PERTES & Ligne_AFFICHAGE_ECART) = Tableau_ECART(i, 9) ' gains/pertes
        shResultats_CONTROLE.Range(COL_CONTROLE_GAINS_PERTES & Ligne_AFFICHAGE_ECART).Select
        Selection.NumberFormat = "$#,##0.00_);[Red]($#,##0.00)"
        End If

Else
End If
        
i = i + 1
' ligne suivante
Ligne_AFFICHAGE_ECART = Ligne_AFFICHAGE_ECART + 1

Wend


'********************************************************************************************************
'9) onglet CONTROLE - AFFICHAGE MESSAGE ECART et COULEUR ONGLET
'********************************************************************************************************

' chargement paramètres ligne ECART
Ligne_AFFICHAGE_MESSAGE_ECART = Ligne_AFFICHAGE_ECART + 3

If PresenceECART = False Then ' AUCUN ECART
    Libelle = LibelleAucunEcart
    Couleur = CouleurVerte
    CouleurCaractere = CouleurCaractereRouge
Else
    Libelle = LibelleEcart
    Couleur = CouleurRouge
    CouleurCaractere = CouleurCaractereNoir
End If

' Mise en page ligne ECART
    shResultats_CONTROLE.Range(COL_CONTROLE_SURVENANCE & Ligne_AFFICHAGE_MESSAGE_ECART) = Libelle
    Zone1 = COL_CONTROLE_SURVENANCE & Ligne_AFFICHAGE_MESSAGE_ECART & ":" & COL_CONTROLE_GAINS_PERTES & Ligne_AFFICHAGE_MESSAGE_ECART
    shResultats_CONTROLE.Range(Zone1).Select
    With Selection
        .HorizontalAlignment = xlCenter
        .VerticalAlignment = xlBottom
        .WrapText = False
        .Orientation = 0
        .AddIndent = False
        .IndentLevel = 0
        .ShrinkToFit = False
        .ReadingOrder = xlContext
        .MergeCells = False
    End With
    Selection.Merge
    With Selection.Font
        .Color = CouleurCaractere ' couleur caractère
        .TintAndShade = 0
    End With
    With Selection.Interior
        .Pattern = xlSolid
        .PatternColorIndex = xlAutomatic
        .Color = Couleur ' couleur
        .TintAndShade = 0
        .PatternTintAndShade = 0
    End With
    Selection.Font.Bold = True
   
    ' changer la couleur onglet CONTROLE
    With shResultats_CONTROLE.Tab
        .Color = Couleur ' couleur
        .TintAndShade = 0
    End With



GoTo FIN_CONTROLE:
               
err_CONTROLE:

  Set shErreurs = Worksheets("Erreurs")
  Application.Cursor = xlDefault
  
 ' recherche du dernier numéro de la ligne <> "" pour permettre d'ajouter un message à la ligne suivante
    p = 1
    While shErreurs.Cells(p, 1) <> ""
    p = p + 1
    Wend
    p = p - 1
    NoLigneEnErreur = p - 1
  
  MsgBox "Erreur dans CONTROLE() : " & Err.Number & vbLf & Err.Description, vbCritical
  
  ' affichage du message d'erreur dans la feuille "Erreurs"
  'MessageErreur = "CONTROLE() : " & Err.Number & " - " & Err.Description
  MessageErreur = NomModule & " - " & LibelleFamille
  MsgBox MessageErreur
    
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
    
FIN_CONTROLE:

' SUPPRESION AFFICHAGE ProgressBarre
PgBar.Hide
Set PgBar = Nothing

End Sub
Function Frais(shDonnees As Worksheet, ANNEE As String, LIBELLE_FRAIS As String) As Double
' Frais

Dim Col_ANNEE As Double
Dim Col_LIBELLE_FRAIS As Double
Dim Col_MONTANT_FRAIS As Double
Col_ANNEE = 8
Col_LIBELLE_FRAIS = 9
Col_MONTANT_FRAIS = 10

Dim NbLigneAnnee As Double
Dim i As Double

'Recherche Nb de lignes ANNEE dans la colonne 8
NbLigneAnnee = shDonnees.Cells(Rows.Count, Col_ANNEE).End(xlUp).Row + 1
Frais = 0
For i = 2 To NbLigneAnnee
    If (shDonnees.Cells(i, Col_ANNEE) = ANNEE And shDonnees.Cells(i, Col_LIBELLE_FRAIS) = LIBELLE_FRAIS) Then
        Frais = shDonnees.Cells(i, Col_MONTANT_FRAIS) * 1
        i = NbLigneAnnee
    Else
    End If
Next i
End Function


' cumul Montant pour une rubrique
Function CumulMontant(shDonnees As Worksheet, SelectionMontant As String, SelectionAnnee As String, ANNEE As Integer, bTypeCompte As Boolean) As Double
Dim NbDecimales As Integer
NbDecimales = 2
CumulMontant = 0
If bTypeCompte = True Then 'COMPTABLE
        CumulMontant = Round(Application.WorksheetFunction.Sum(shDonnees.Range(SelectionMontant)), NbDecimales)
Else 'SURVENANCE
        CumulMontant = Round(Application.WorksheetFunction.SumIfs(shDonnees.Range(SelectionMontant), shDonnees.Range(SelectionAnnee), ANNEE), NbDecimales)
End If
End Function


Function AffichageProgressbarre(Bloc As Integer, TotalBloc As Integer) As Boolean
If TotalBloc <> 0 Then
PgBar.SetProgressValue (Bloc) / (TotalBloc): DoEvents
End If
End Function


