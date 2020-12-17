using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using INFITF;
using MECMOD;
using PARTITF;
using HybridShapeTypeLib;
using KnowledgewareTypeLib;
using ProductStructureTypeLib;

namespace GUI_Zahnradrechner_Gruppe_I
{
    class CatiaConnection
    {
        INFITF.Application hsp_catiaApp;
        MECMOD.PartDocument hsp_catiaPart;
        MECMOD.Sketch hsp_catiaProfil;

        //Catia laeuft
        public bool CATIALaeuft()
        {
            try
            {
                object catiaObject = System.Runtime.InteropServices.Marshal.GetActiveObject(
                    "CATIA.Application");
                hsp_catiaApp = (INFITF.Application)catiaObject;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        //Part anlegen
        public Boolean ErzeugePart()
        {
            INFITF.Documents catDocuments1 = hsp_catiaApp.Documents;
            hsp_catiaPart = catDocuments1.Add("Part") as MECMOD.PartDocument;
            return true;
        }

        //Achsensystem anlegen
        private void ErzeugeAchsensystem()
        {
            object[] arr = new object[] {0.0, 0.0, 0.0,
                                         0.0, 1.0, 0.0,
                                         0.0, 0.0, 1.0 };
            hsp_catiaProfil.SetAbsoluteAxisData(arr);
        }



        //Außenverzahntes Stirnrad
        public void ErzeugeProfilAußen(Data dat)
        {
            //ERZEUGE SKIZZE
            //Geometrisches Set auswählen und umbennen
            
            HybridBodies catHybridBodies1 = hsp_catiaPart.Part.HybridBodies;
            HybridBody catHybridBody1;
            try
            {
                catHybridBody1 = catHybridBodies1.Item("Geometrisches Set.1");
            }
            catch (Exception)
            {
                MessageBox.Show("Kein geometrisches Set gefunden! " + Environment.NewLine +
                    "Ein PART manuell erzeugen und ein darauf achten, dass 'Geometisches Set' aktiviert ist.",
                    "Fehler", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            catHybridBody1.set_Name("Profile");
            Sketches catSketches1 = catHybridBody1.HybridSketches;
            OriginElements catOriginElements = hsp_catiaPart.Part.OriginElements;
            Reference catReference1 = (Reference)catOriginElements.PlaneYZ;
            hsp_catiaProfil = catSketches1.Add(catReference1);

            //Erstelle Achsensystem

            ErzeugeAchsensystem();

            //Update
            hsp_catiaPart.Part.Update();



            //KOORDINATEN ANLEGEN
            //Nullpunkt
            double x0 = 0;
            double y0 = 0;

            //Radien und Winkel
            double teilkreisradius = dat.getTeilkreisdurchmesser() / 2;
            double hilfskreisradius = teilkreisradius * 0.94;
            double fußkreisradius = teilkreisradius - (1.25 * dat.getModul());
            double kopfkreisradius = teilkreisradius + dat.getModul();
            double verrundungsradius = 0.35 * dat.getModul();

            double alpha = 20;
            double beta = 90 / dat.getZähnezahl();
            double betarad = Math.PI * beta / 180;
            double gamma = 90 - (alpha - beta);
            double gammarad = Math.PI * gamma / 180;
            double totalangle = 360.0 / dat.getZähnezahl();
            double totalanglerad = Math.PI * totalangle / 180;

            //Schnittpunkte und Koordinaten
            //Evolventenkreis Center Koordinaten
            double mittelpunktEvol_x = hilfskreisradius * Math.Cos(gammarad);
            double mittelpunktEvol_y = hilfskreisradius * Math.Sin(gammarad);

            //Schnittpkt. der Evolvente und dem Teilkreisradius
            double punktEvol_x = -teilkreisradius * Math.Sin(betarad);
            double punktEvol_y = teilkreisradius * Math.Cos(betarad);

            //Evolvente Radius
            double EvolventenkreisRadius = Math.Sqrt(Math.Pow((mittelpunktEvol_x - punktEvol_x), 2) + Math.Pow((mittelpunktEvol_y - punktEvol_y), 2));

            //Schnittpunkt Kopfkreis und Evolventenkreis
            double evolventenKopfk_x = Schnittpunkt_X(x0, y0, kopfkreisradius, mittelpunktEvol_x, mittelpunktEvol_y, EvolventenkreisRadius);
            double evolventenKopfk_y = Schnittpunkt_Y(x0, y0, kopfkreisradius, mittelpunktEvol_x, mittelpunktEvol_y, EvolventenkreisRadius);

            //Center Verrundung
            double mittelpunktVer_x = Schnittpunkt_X(x0, y0, fußkreisradius + verrundungsradius, mittelpunktEvol_x, mittelpunktEvol_y, EvolventenkreisRadius + verrundungsradius);
            double mittelpunktVer_y = Schnittpunkt_Y(x0, y0, fußkreisradius + verrundungsradius, mittelpunktEvol_x, mittelpunktEvol_y, EvolventenkreisRadius + verrundungsradius);

            //Schnittpunkt Verrundung Evolventenkreis
            double evolventenVer_x = Schnittpunkt_X(mittelpunktEvol_x, mittelpunktEvol_y, EvolventenkreisRadius, mittelpunktVer_x, mittelpunktVer_y, verrundungsradius);
            double evolventenVer_y = Schnittpunkt_Y(mittelpunktEvol_x, mittelpunktEvol_y, EvolventenkreisRadius, mittelpunktVer_x, mittelpunktVer_y, verrundungsradius);

            //Schnittpunkt Verrundung Fußkreis
            double fußkreisVer_x = Schnittpunkt_X(x0, y0, fußkreisradius, mittelpunktVer_x, mittelpunktVer_y, verrundungsradius);
            double fußkreisVer_y = Schnittpunkt_Y(x0, y0, fußkreisradius, mittelpunktVer_x, mittelpunktVer_y, verrundungsradius);

            //Anfangspunkt Fußkreis
            double hilfswinkel = totalanglerad - Math.Atan(Math.Abs(fußkreisVer_x) / Math.Abs(fußkreisVer_y));
            double anfangspunktFußk_x = -fußkreisradius * Math.Sin(hilfswinkel);
            double anfangspunktFußk_y = fußkreisradius * Math.Cos(hilfswinkel);

            //Skizze umbenennen und öffnen
            hsp_catiaProfil.set_Name("Außenverzahntes Stirnrad");
            Factory2D catfactory2D1 = hsp_catiaProfil.OpenEdition();

            //Punkte in Skizze einzeichnen
            Point2D point_Ursprung = catfactory2D1.CreatePoint(x0, y0);
            Point2D pointAnfangFußkreis = catfactory2D1.CreatePoint(anfangspunktFußk_x, anfangspunktFußk_y);
            Point2D pointFußkreisVer_l = catfactory2D1.CreatePoint(fußkreisVer_x, fußkreisVer_y);
            Point2D pointFußkreisVer_r = catfactory2D1.CreatePoint(-fußkreisVer_x, fußkreisVer_y);
            Point2D pointMittelpunktVer_l = catfactory2D1.CreatePoint(mittelpunktVer_x, mittelpunktVer_y);
            Point2D pointMittelpunktVer_r = catfactory2D1.CreatePoint(-mittelpunktVer_x, mittelpunktVer_y);
            Point2D pointVerrundungEvol_l = catfactory2D1.CreatePoint(evolventenVer_x, evolventenVer_y);
            Point2D pointVerrundungEvol_r = catfactory2D1.CreatePoint(-evolventenVer_x, evolventenVer_y);
            Point2D pointMittelpunktevol_l = catfactory2D1.CreatePoint(mittelpunktEvol_x, mittelpunktEvol_y);
            Point2D pointMittelpunktevol_r = catfactory2D1.CreatePoint(-mittelpunktEvol_x, mittelpunktEvol_y);
            Point2D pointEvolventenKopfkreis_l = catfactory2D1.CreatePoint(evolventenKopfk_x, evolventenKopfk_y);
            Point2D pointEvolventenKopfkreis_r = catfactory2D1.CreatePoint(-evolventenKopfk_x, evolventenKopfk_y);

            //Kreise in Skizze einzeichnen
            Circle2D kreisFußkreis = catfactory2D1.CreateCircle(x0, y0, fußkreisradius, 0, Math.PI * 2);
            kreisFußkreis.CenterPoint = point_Ursprung;
            kreisFußkreis.StartPoint = pointFußkreisVer_l;
            kreisFußkreis.EndPoint = pointAnfangFußkreis;

            Circle2D kreisVerrundung_l = catfactory2D1.CreateCircle(mittelpunktVer_x, mittelpunktVer_y, verrundungsradius, 0, Math.PI * 2);
            kreisVerrundung_l.CenterPoint = pointMittelpunktVer_l;
            kreisVerrundung_l.StartPoint = pointFußkreisVer_l;
            kreisVerrundung_l.EndPoint = pointVerrundungEvol_l;

            Circle2D kreisEvolventenkreis_l = catfactory2D1.CreateCircle(mittelpunktEvol_x, mittelpunktEvol_y, EvolventenkreisRadius, 0, Math.PI * 2);
            kreisEvolventenkreis_l.CenterPoint = pointMittelpunktevol_l;
            kreisEvolventenkreis_l.StartPoint = pointEvolventenKopfkreis_l;
            kreisEvolventenkreis_l.EndPoint = pointVerrundungEvol_l;

            Circle2D kreisKopfkreis = catfactory2D1.CreateCircle(x0, y0, kopfkreisradius, 0, Math.PI * 2);
            kreisKopfkreis.CenterPoint = point_Ursprung;
            kreisKopfkreis.StartPoint = pointEvolventenKopfkreis_r;
            kreisKopfkreis.EndPoint = pointEvolventenKopfkreis_l;

            Circle2D kreisEvolventenkreis_r = catfactory2D1.CreateCircle(-mittelpunktEvol_x, mittelpunktEvol_y, EvolventenkreisRadius, 0, Math.PI * 2);
            kreisEvolventenkreis_r.CenterPoint = pointMittelpunktevol_r;
            kreisEvolventenkreis_r.StartPoint = pointVerrundungEvol_r;
            kreisEvolventenkreis_r.EndPoint = pointEvolventenKopfkreis_r;

            Circle2D kreisVerrundung_r = catfactory2D1.CreateCircle(-mittelpunktVer_x, mittelpunktVer_y, verrundungsradius, 0, Math.PI * 2);
            kreisVerrundung_r.CenterPoint = pointMittelpunktVer_r;
            kreisVerrundung_r.StartPoint = pointVerrundungEvol_r;
            kreisVerrundung_r.EndPoint = pointFußkreisVer_r;


            //Skizze schließen
            hsp_catiaProfil.CloseEdition();

            //Update
            hsp_catiaPart.Part.Update();



            //ERSTELLE KREISMUSTER
            ShapeFactory shapeFactory1 = (ShapeFactory)hsp_catiaPart.Part.ShapeFactory;
            HybridShapeFactory hybridShapeFactory1 = (HybridShapeFactory)hsp_catiaPart.Part.HybridShapeFactory;

            Factory2D factory2D1 = hsp_catiaProfil.Factory2D;

            HybridShapePointCoord ursprung = hybridShapeFactory1.AddNewPointCoord(0, 0, 0);
            Reference refUrsprung = hsp_catiaPart.Part.CreateReferenceFromObject(ursprung);

            HybridShapeDirection xRichtung = hybridShapeFactory1.AddNewDirectionByCoord(1, 0, 0);
            Reference refxRichtung = hsp_catiaPart.Part.CreateReferenceFromObject(xRichtung);

            CircPattern kreismuster = shapeFactory1.AddNewSurfacicCircPattern(factory2D1, 1, 2, 0, 0, 1, 1, refUrsprung, refxRichtung, false, 0, true, false);
            kreismuster.CircularPatternParameters = CatCircularPatternParameters.catInstancesandAngularSpacing;
            AngularRepartition angularRepartition1 = kreismuster.AngularRepartition;
            Angle angle1 = angularRepartition1.AngularSpacing;
            angle1.Value = Convert.ToDouble(360 / dat.getZähnezahl());
            AngularRepartition angularRepartition2 = kreismuster.AngularRepartition;
            IntParam intParam1 = angularRepartition2.InstancesCount;
            intParam1.Value = Convert.ToInt32(dat.getZähnezahl()) + 1;


            //Kreismusterenden verbinden

            Reference refKreismuster = hsp_catiaPart.Part.CreateReferenceFromObject(kreismuster);
            HybridShapeAssemble verbindung = hybridShapeFactory1.AddNewJoin(refKreismuster, refKreismuster);
            Reference refVerbindung = hsp_catiaPart.Part.CreateReferenceFromObject(verbindung);

            hybridShapeFactory1.GSMVisibility(refVerbindung, 0);

            hsp_catiaPart.Part.MainBody.InsertHybridShape(verbindung);

            //Update
            hsp_catiaPart.Part.Update();


            //ERSTELLE BLOCK
            ErzeugeBlock(refVerbindung, shapeFactory1, dat);


            if (dat.Bohrungsauswahl == 1)
            {
                //Skizze
                Sketches sketchesBohrung = catHybridBody1.HybridSketches;
                OriginElements catoriginelements = hsp_catiaPart.Part.OriginElements;
                Reference refmxPlaneX = (Reference)catoriginelements.PlaneYZ;
                hsp_catiaProfil = catSketches1.Add(refmxPlaneX);

                //Achsensystem
                ErzeugeAchsensystem();

                //Update
                hsp_catiaPart.Part.Update();

                //Skizze Umbennen
                hsp_catiaProfil.set_Name("Bohrung");

                //Öffnen
                Factory2D catfactory2D2 = hsp_catiaProfil.OpenEdition();

                //Kreis
                Circle2D KreisFürBohrungsskizze = catfactory2D2.CreateClosedCircle(x0, y0, dat.getBohrung());

                //Schließen
                hsp_catiaProfil.CloseEdition();
                
                //Update
                hsp_catiaPart.Part.Update();

                //Tasche
                hsp_catiaPart.Part.InWorkObject = hsp_catiaPart.Part.MainBody;
                Pocket Tasche = shapeFactory1.AddNewPocket(hsp_catiaProfil, dat.getBreite());
                hsp_catiaPart.Part.Update();
            }
            if (dat.Bohrungsauswahl == 2)
            {
                //Schnittpunkte und Koordinaten
                //LH Schnittpunkt Passfederhöhe mit Bohrung
                double x_AnfangkreisZuPassfeder = -dat.getPassfederbreite() / 2;
                double y_AnfangkreisZuPassfeder = Math.Sqrt(Math.Pow(dat.getBohrung(), 2) - Math.Pow((dat.getPassfederbreite() / 2), 2));

                //LH Schnittpunkt Breite und Höhe
                double x_Passfederecke = -dat.getPassfederbreite() / 2;
                double y_Passfederecke = dat.getPassfederhöhe();

                //Skizze
                Sketches sketchesBohrung = catHybridBody1.HybridSketches;
                OriginElements catoriginelements = hsp_catiaPart.Part.OriginElements;
                Reference refmxPlaneX = (Reference)catoriginelements.PlaneYZ;
                hsp_catiaProfil = catSketches1.Add(refmxPlaneX);

                //Achsensystem
                ErzeugeAchsensystem();

                //Update
                hsp_catiaPart.Part.Update();

                //Umbennen und öffnen
                hsp_catiaProfil.set_Name("Bohrung mit Passfedernut");
                Factory2D catfactory2D2 = hsp_catiaProfil.OpenEdition();



                //Punkte in Skizze einzeichnen
                Point2D POINTLinksAnfangKreisZuPassfeder = catfactory2D2.CreatePoint(x_AnfangkreisZuPassfeder, y_AnfangkreisZuPassfeder);
                Point2D POINTLinksPassfederEcke = catfactory2D2.CreatePoint(x_Passfederecke, y_Passfederecke);
                Point2D POINTRechtsPassfederEcke = catfactory2D2.CreatePoint(-x_Passfederecke, y_Passfederecke);
                Point2D POINTRechtsAnfangKreisZuPassfeder = catfactory2D2.CreatePoint(-x_AnfangkreisZuPassfeder, y_AnfangkreisZuPassfeder);

                //Linien einzeichnen
                Line2D PassfederKanteLinks = catfactory2D2.CreateLine(x_AnfangkreisZuPassfeder, y_AnfangkreisZuPassfeder, x_Passfederecke, y_Passfederecke);
                PassfederKanteLinks.StartPoint = POINTLinksAnfangKreisZuPassfeder;
                PassfederKanteLinks.EndPoint = POINTLinksPassfederEcke;

                Line2D PassfederHöhenkante = catfactory2D2.CreateLine(x_Passfederecke, y_Passfederecke, -x_Passfederecke, y_Passfederecke);
                PassfederHöhenkante.StartPoint = POINTLinksPassfederEcke;
                PassfederHöhenkante.EndPoint = POINTRechtsPassfederEcke;

                Line2D PassfederKanteRechts = catfactory2D2.CreateLine(-x_Passfederecke, y_Passfederecke, -x_AnfangkreisZuPassfeder, y_AnfangkreisZuPassfeder);
                PassfederKanteRechts.StartPoint = POINTRechtsPassfederEcke;
                PassfederKanteRechts.EndPoint = POINTRechtsAnfangKreisZuPassfeder;

                Circle2D KreisFürPassfeder = catfactory2D2.CreateCircle(x0, y0, dat.getBohrung(), 0, Math.PI * 2);
                KreisFürPassfeder.CenterPoint = point_Ursprung;
                KreisFürPassfeder.EndPoint = POINTRechtsAnfangKreisZuPassfeder;
                KreisFürPassfeder.StartPoint = POINTLinksAnfangKreisZuPassfeder;

                //Skizze schließen
                hsp_catiaProfil.CloseEdition();


                //Update
                hsp_catiaPart.Part.Update();

                //Tasche
                hsp_catiaPart.Part.InWorkObject = hsp_catiaPart.Part.MainBody;
                Pocket Tasche = shapeFactory1.AddNewPocket(hsp_catiaProfil, dat.getBreite());
                hsp_catiaPart.Part.Update();
            }
        }

        //Block
        public void ErzeugeBlock(Reference refVerbindung, ShapeFactory sf1, Data dat)
        {
            //Block
            hsp_catiaPart.Part.InWorkObject = hsp_catiaPart.Part.MainBody;
            Pad catPad1 = sf1.AddNewPadFromRef(refVerbindung, dat.getBreite());

            //Update
            hsp_catiaPart.Part.Update();
        }



        //Innenverzahntes Stirnrad
        public void ErzeugeProfilInnen(Data dat)
        {

            //KOORDINATEN ANLEGEN
            //Nullpunkt
            double x0 = 0;
            double y0 = 0;

            //Radien und Winkel umgestellt
            double Teilkreisradius = dat.getTeilkreisdurchmesser() / 2;
            //Angenährt
            double Hilfskreisradius = Teilkreisradius * 1.12;
            double Kopfkreisradius = Teilkreisradius - (1.25 * dat.getModul());
            double Fußkreisradius = Teilkreisradius + dat.getModul();
            double Verrundungsradius = 0.35 * dat.getModul();

            double Alpha = 20;
            double Beta = 90 / dat.getZähnezahl();
            double Betarad = Math.PI * Beta / 180;
            double Gamma = 90 - (Alpha - Beta);
            double Gammarad = Math.PI * Gamma / 180;
            double Totalangel = 360.0 / dat.getZähnezahl();
            double Totalangelrad = Math.PI * Totalangel / 180;

            //Schnittpunkte und Koordinaten
            //Evolventenkreis Center Koordinaten
            double xMittelpunktaufEvol_links = Hilfskreisradius * Math.Cos(Gammarad);
            double yMittelpunktaufEvol_links = Hilfskreisradius * Math.Sin(Gammarad);

            //Schnittpkt. der Evolvente und dem Teilkreisradius
            double xPunktAufEvolvente = -Teilkreisradius * Math.Sin(Betarad);
            double yPunktAufEvolvente = Teilkreisradius * Math.Cos(Betarad);

            //Evolvente Radius
            double EvolventenkreisRadius = Math.Sqrt(Math.Pow((xMittelpunktaufEvol_links - xPunktAufEvolvente), 2) + Math.Pow((yMittelpunktaufEvol_links - yPunktAufEvolvente), 2));

            //Schnittpunkt Kopfkreis und Evolventenkreis
            double xEvolventenkopfkreis_links = Schnittpunkt_X(x0, y0, Kopfkreisradius, xMittelpunktaufEvol_links, yMittelpunktaufEvol_links, EvolventenkreisRadius);
            double yEvolventenkopfkreis_links = Schnittpunkt_Y(x0, y0, Kopfkreisradius, xMittelpunktaufEvol_links, yMittelpunktaufEvol_links, EvolventenkreisRadius);

            //Center Verrundung
            double xMittelpunktVerrundung_links = Schnittpunkt_X(x0, y0, Fußkreisradius - Verrundungsradius, xMittelpunktaufEvol_links, yMittelpunktaufEvol_links, EvolventenkreisRadius + Verrundungsradius);
            double yMittelpunktVerrundung_links = Schnittpunkt_Y(x0, y0, Fußkreisradius - Verrundungsradius, xMittelpunktaufEvol_links, yMittelpunktaufEvol_links, EvolventenkreisRadius + Verrundungsradius);

            //Schnittpunkt Verrundung Evolventenkreis
            double x_SP_EvolventeVerrundung_links = Schnittpunkt_X(xMittelpunktaufEvol_links, yMittelpunktaufEvol_links, EvolventenkreisRadius, xMittelpunktVerrundung_links, yMittelpunktVerrundung_links, Verrundungsradius);
            double y_SP_EvolventeVerrundung_links = Schnittpunkt_Y(xMittelpunktaufEvol_links, yMittelpunktaufEvol_links, EvolventenkreisRadius, xMittelpunktVerrundung_links, yMittelpunktVerrundung_links, Verrundungsradius);

            //Schnittpunkt Verrundung Fußkreis
            double x_SP_FußkreisradiusVerrundung_links = Schnittpunkt_X(x0, y0, Fußkreisradius, xMittelpunktVerrundung_links, yMittelpunktVerrundung_links, Verrundungsradius);
            double y_SP_FußkreisradiusVerrundung_links = Schnittpunkt_Y(x0, y0, Fußkreisradius, xMittelpunktVerrundung_links, yMittelpunktVerrundung_links, Verrundungsradius);

            //Anfangspunkt Fußkreis
            double Hilfswinkel = Totalangelrad - Math.Atan(Math.Abs(x_SP_FußkreisradiusVerrundung_links) / Math.Abs(y_SP_FußkreisradiusVerrundung_links));
            double x_AnfangspunktFußkreis = -Fußkreisradius * Math.Sin(Hilfswinkel);
            double y_AnfangspunktFußkreis = Fußkreisradius * Math.Cos(Hilfswinkel);



            //Geometrisches Set auswählen und umbennen
            HybridBodies catHybridBodies1 = hsp_catiaPart.Part.HybridBodies;
            HybridBody catHybridBody1;
            try
            {
                catHybridBody1 = catHybridBodies1.Item("Geometrisches Set.1");
            }
            catch (Exception)
            {
                MessageBox.Show("Kein geometrisches Set gefunden! " + Environment.NewLine +
                    "Ein PART manuell erzeugen und ein darauf achten, dass 'Geometisches Set' aktiviert ist.",
                    "Fehler", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            catHybridBody1.set_Name("Profile");



            //ERSTELLE SKIZZE FÜR SCHEIBE
            Sketches sketchesBohrung = catHybridBody1.HybridSketches;
            OriginElements catoriginelements = hsp_catiaPart.Part.OriginElements;
            Reference refmxPlaneX = (Reference)catoriginelements.PlaneYZ;
            hsp_catiaProfil = sketchesBohrung.Add(refmxPlaneX);

            ErzeugeAchsensystem();

            hsp_catiaPart.Part.Update();


            //Skizze umbenennen und öffnen
            hsp_catiaProfil.set_Name("Scheibe");
            Factory2D catfactory2D2 = hsp_catiaProfil.OpenEdition();

            //Kreis
            Circle2D Scheibe = catfactory2D2.CreateClosedCircle(x0, y0, dat.getAußenradius());

            //Skizze schließen
            hsp_catiaProfil.CloseEdition();

            //Update
            hsp_catiaPart.Part.Update();

            //Erzeuge Block
            ShapeFactory shapeFactoryScheibe = (ShapeFactory)hsp_catiaPart.Part.ShapeFactory;
            hsp_catiaPart.Part.InWorkObject = hsp_catiaPart.Part.MainBody;
            Pad Block = shapeFactoryScheibe.AddNewPad(hsp_catiaProfil, dat.getBreite());
            hsp_catiaPart.Part.Update();



            //Zahnradskizze erstellen

            //Skizze umbenennen und öffnen
            Sketches catSketches1 = catHybridBody1.HybridSketches;
            OriginElements catOriginElements = hsp_catiaPart.Part.OriginElements;
            Reference catReference1 = (Reference)catOriginElements.PlaneYZ;
            hsp_catiaProfil = catSketches1.Add(catReference1);

            //Achsensystem in Skizze erstellen 
            ErzeugeAchsensystem();

            //Update
            hsp_catiaPart.Part.Update();

            //Umbennen und öffnen
            hsp_catiaProfil.set_Name("Innenverzahntes Stirnrad");
            Factory2D catfactory2D1 = hsp_catiaProfil.OpenEdition();

            //Punkte in Skizze einzeichnen
            Point2D point_Ursprung = catfactory2D1.CreatePoint(x0, y0);
            Point2D pointAnfangFußkreisLinks = catfactory2D1.CreatePoint(x_AnfangspunktFußkreis, y_AnfangspunktFußkreis);
            Point2D pointFußkreisVerrundungLinks = catfactory2D1.CreatePoint(x_SP_FußkreisradiusVerrundung_links, y_SP_FußkreisradiusVerrundung_links);
            Point2D pointFußkreisVerrundungRechts = catfactory2D1.CreatePoint(-x_SP_FußkreisradiusVerrundung_links, y_SP_FußkreisradiusVerrundung_links);
            Point2D pointMittelpunktVerrundungLinks = catfactory2D1.CreatePoint(xMittelpunktVerrundung_links, yMittelpunktVerrundung_links);
            Point2D pointMittelpunktVerrundungRechts = catfactory2D1.CreatePoint(-xMittelpunktVerrundung_links, yMittelpunktVerrundung_links);
            Point2D pointVerrundungEvolventeLinks = catfactory2D1.CreatePoint(x_SP_EvolventeVerrundung_links, y_SP_EvolventeVerrundung_links);
            Point2D pointVerrundungEvolventeRechts = catfactory2D1.CreatePoint(-x_SP_EvolventeVerrundung_links, y_SP_EvolventeVerrundung_links);
            Point2D pointMittelpunktevolventeLinks = catfactory2D1.CreatePoint(xMittelpunktaufEvol_links, yMittelpunktaufEvol_links);
            Point2D pointMittelpunktevolventeRechts = catfactory2D1.CreatePoint(-xMittelpunktaufEvol_links, yMittelpunktaufEvol_links);
            Point2D pointEvolventenKopfkreisLinks = catfactory2D1.CreatePoint(xEvolventenkopfkreis_links, yEvolventenkopfkreis_links);
            Point2D pointEvolventenKopfkreisRechts = catfactory2D1.CreatePoint(-xEvolventenkopfkreis_links, yEvolventenkopfkreis_links);

            //Kreise einzeichnen
            Circle2D KreisFrußkreis = catfactory2D1.CreateCircle(x0, y0, Fußkreisradius, 0, Math.PI * 2);
            KreisFrußkreis.CenterPoint = point_Ursprung;
            KreisFrußkreis.StartPoint = pointFußkreisVerrundungLinks;
            KreisFrußkreis.EndPoint = pointAnfangFußkreisLinks;

            Circle2D KreisVerrundungLinks = catfactory2D1.CreateCircle(xMittelpunktVerrundung_links, yMittelpunktVerrundung_links, Verrundungsradius, 0, Math.PI * 2);
            KreisVerrundungLinks.CenterPoint = pointMittelpunktVerrundungLinks;
            KreisVerrundungLinks.EndPoint = pointFußkreisVerrundungLinks;
            KreisVerrundungLinks.StartPoint = pointVerrundungEvolventeLinks;

            Circle2D KreisEvolventenkreisLinks = catfactory2D1.CreateCircle(xMittelpunktaufEvol_links, yMittelpunktaufEvol_links, EvolventenkreisRadius, 0, Math.PI * 2);
            KreisEvolventenkreisLinks.CenterPoint = pointMittelpunktevolventeLinks;
            KreisEvolventenkreisLinks.EndPoint = pointEvolventenKopfkreisLinks;
            KreisEvolventenkreisLinks.StartPoint = pointVerrundungEvolventeLinks;

            Circle2D KreisKopfkreis = catfactory2D1.CreateCircle(x0, y0, Kopfkreisradius, 0, Math.PI * 2);
            KreisKopfkreis.CenterPoint = point_Ursprung;
            KreisKopfkreis.StartPoint = pointEvolventenKopfkreisRechts;
            KreisKopfkreis.EndPoint = pointEvolventenKopfkreisLinks;

            Circle2D KreisEvolventenkreisRechts = catfactory2D1.CreateCircle(-xMittelpunktaufEvol_links, yMittelpunktaufEvol_links, EvolventenkreisRadius, 0, Math.PI * 2);
            KreisEvolventenkreisRechts.CenterPoint = pointMittelpunktevolventeRechts;
            KreisEvolventenkreisRechts.EndPoint = pointVerrundungEvolventeRechts;
            KreisEvolventenkreisRechts.StartPoint = pointEvolventenKopfkreisRechts;

            Circle2D KreisVerrundungRechts = catfactory2D1.CreateCircle(-xMittelpunktVerrundung_links, yMittelpunktVerrundung_links, Verrundungsradius, 0, Math.PI * 2);
            KreisVerrundungRechts.CenterPoint = pointMittelpunktVerrundungRechts;
            KreisVerrundungRechts.EndPoint = pointVerrundungEvolventeRechts;
            KreisVerrundungRechts.StartPoint = pointFußkreisVerrundungRechts;


            //Schließen
            hsp_catiaProfil.CloseEdition();

            //Updaten
            hsp_catiaPart.Part.Update();



            //ERSTELLE KREISMUSTER
            HybridShapeFactory hybridShapeFactory1 = (HybridShapeFactory)hsp_catiaPart.Part.HybridShapeFactory;
            ShapeFactory shapeFactory1 = (ShapeFactory)hsp_catiaPart.Part.ShapeFactory;

            Factory2D factory2D1 = hsp_catiaProfil.Factory2D;

            HybridShapePointCoord ursprung = hybridShapeFactory1.AddNewPointCoord(0, 0, 0);
            Reference refUrsprung = hsp_catiaPart.Part.CreateReferenceFromObject(ursprung);

            HybridShapeDirection xRichtung = hybridShapeFactory1.AddNewDirectionByCoord(1, 0, 0);
            Reference refxRichtung = hsp_catiaPart.Part.CreateReferenceFromObject(xRichtung);

            CircPattern kreismuster = shapeFactory1.AddNewSurfacicCircPattern(factory2D1, 1, 2, 0, 0, 1, 1, refUrsprung, refxRichtung, false, 0, true, false);
            kreismuster.CircularPatternParameters = CatCircularPatternParameters.catInstancesandAngularSpacing;
            AngularRepartition angularRepartition1 = kreismuster.AngularRepartition;
            Angle angle1 = angularRepartition1.AngularSpacing;
            angle1.Value = Convert.ToDouble(360 / dat.getZähnezahl());
            AngularRepartition angularRepartition2 = kreismuster.AngularRepartition;
            IntParam intParam1 = angularRepartition2.InstancesCount;
            intParam1.Value = Convert.ToInt32(dat.getZähnezahl()) + 1;


            //Kreismusterenden verbinden

            Reference refKreismuster = hsp_catiaPart.Part.CreateReferenceFromObject(kreismuster);
            HybridShapeAssemble verbindung = hybridShapeFactory1.AddNewJoin(refKreismuster, refKreismuster);
            Reference refVerbindung = hsp_catiaPart.Part.CreateReferenceFromObject(verbindung);

            hybridShapeFactory1.GSMVisibility(refVerbindung, 0);

            hsp_catiaPart.Part.MainBody.InsertHybridShape(verbindung);


            //Update
            hsp_catiaPart.Part.Update();

            //Tasche
            ErzeugeTasche(dat, refVerbindung, shapeFactory1);
            hsp_catiaProfil = catSketches1.Parent as MECMOD.Sketch;
            hsp_catiaPart.Part.Update();
        }

        //Tasche
        public void ErzeugeTasche(Data dat, Reference refVerbindung, ShapeFactory sf1)
        {
            //Tasche
            hsp_catiaPart.Part.InWorkObject = hsp_catiaPart.Part.MainBody;
            Pocket catPad1 = sf1.AddNewPocketFromRef(refVerbindung, dat.getBreite());
            //Update
            hsp_catiaPart.Part.Update();
        }



        //Schnittpunkte X & Y
        private double Schnittpunkt_X(double mittelpunkt_x, double mittelpunkt_y, double radius1, double mittelpunkt2_x, double mittelpunkt2_y, double radius2)
        {
            
            //Schnittpunkte X berechnen
            double d = Math.Sqrt(Math.Pow((mittelpunkt_x - mittelpunkt2_x), 2) + Math.Pow((mittelpunkt_y - mittelpunkt2_y), 2));
            double l = (Math.Pow(radius1, 2) - Math.Pow(radius2, 2) + Math.Pow(d, 2)) / (d * 2);
            double h;
            double h_fehler = 0.00001;

            if (radius1 - l < -h_fehler)
            {
                MessageBox.Show("Fehler");
            }
            if (Math.Abs(radius1 - l) < h_fehler)
            {
                h = 0;
            }
            else
            {
                h = Math.Sqrt(Math.Pow(radius1, 2) - Math.Pow(l, 2));
            }

            return l * (mittelpunkt2_x - mittelpunkt_x) / d - h * (mittelpunkt2_y - mittelpunkt_y) / d + mittelpunkt_x;
        }

        private double Schnittpunkt_Y(double mittelpunkt_x, double mittelpunkt_y, double radius1, double mittelpunkt2_x, double mittelpunkt2_y, double Radius2)
        {
            //Schnittpunkte Y berechnen
            double d = Math.Sqrt(Math.Pow((mittelpunkt_x - mittelpunkt2_x), 2) + Math.Pow((mittelpunkt_y - mittelpunkt2_y), 2));
            double l = (Math.Pow(radius1, 2) - Math.Pow(Radius2, 2) + Math.Pow(d, 2)) / (d * 2);
            double h;
            double h_fehler = 0.00001;

            if (radius1 - l < -h_fehler)
            {
                MessageBox.Show("Fehler");
            }
            if (Math.Abs(radius1 - l) < h_fehler)
            {
                h = 0;
            }
            else
            {
                h = Math.Sqrt(Math.Pow(radius1, 2) - Math.Pow(l, 2));
            }

            return l * (mittelpunkt2_y - mittelpunkt_y) / d + h * (mittelpunkt2_x - mittelpunkt_x) / d + mittelpunkt_y;
        }


    }
}
